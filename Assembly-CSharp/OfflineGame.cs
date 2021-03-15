using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using PTech;
using UnityEngine;

// Token: 0x02000120 RID: 288
internal class OfflineGame : ServerBase
{
	// Token: 0x06000B32 RID: 2866 RVA: 0x00051D64 File Offset: 0x0004FF64
	public OfflineGame(string overrideGameName, byte[] data, bool replay, User user, UserManClient userManClient, MapMan mapman, PackMan pacMan, GameObject guiCamera, MusicManager musMan, OfflineGameDB gameDB) : base(mapman)
	{
		this.m_userManClient = userManClient;
		this.m_musicMan = musMan;
		this.m_pacMan = pacMan;
		this.m_guiCamera = guiCamera;
		this.m_user = user;
		this.m_gameDB = gameDB;
		this.m_replayMode = replay;
		this.SetupSockets();
		Game game = base.CreateGameFromArray(data, overrideGameName);
		List<string> userNames = game.GetUserNames();
		game.InternalSetUser(userNames[0], this.m_user);
		if (replay)
		{
			game.SetupReplayMode();
		}
		this.JoinGame(game, replay);
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x00051DF8 File Offset: 0x0004FFF8
	public OfflineGame(string campaign, string levelName, string gameName, GameType gameMode, User user, UserManClient userManClient, MapMan mapman, PackMan pacMan, GameObject guiCamera, MusicManager musMan, int nrOfPlayers, FleetSizeClass fleetSize, float targetScore, OfflineGameDB gameDB) : base(mapman)
	{
		this.m_userManClient = userManClient;
		this.m_musicMan = musMan;
		this.m_pacMan = pacMan;
		this.m_guiCamera = guiCamera;
		this.m_user = user;
		this.m_gameDB = gameDB;
		this.SetupSockets();
		MapInfo mapByName = mapman.GetMapByName(gameMode, campaign, levelName);
		DebugUtils.Assert(mapByName != null, "map info not found for " + campaign + " " + levelName);
		Game game = this.CreateGame(gameName, 0, 0, gameMode, campaign, levelName, 1, fleetSize, targetScore, -1.0, mapByName);
		game.AddUserToGame(user, true);
		this.JoinGame(game, false);
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x00051EA8 File Offset: 0x000500A8
	private void SetupSockets()
	{
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		socket.NoDelay = true;
		int num = this.FindValidPort(ref socket, 12345);
		DebugUtils.Assert(num > 0);
		PLog.Log("Started server on port " + num.ToString());
		socket.Listen(100);
		socket.Blocking = false;
		Socket socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		socket2.Connect("localhost", num);
		socket2.Blocking = false;
		socket2.NoDelay = true;
		Socket socket3 = null;
		while (socket3 == null)
		{
			try
			{
				socket3 = socket.Accept();
				socket3.Blocking = false;
			}
			catch (SocketException)
			{
			}
		}
		PacketSocket socket4 = new PacketSocket(socket3);
		this.m_serverRpc = new PTech.RPC(socket4);
		PacketSocket socket5 = new PacketSocket(socket2);
		this.m_clientRpc = new PTech.RPC(socket5);
		this.m_user.Connect(this.m_serverRpc, Utils.GetPlatform());
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x00051FA8 File Offset: 0x000501A8
	private void JoinGame(Game game, bool replayMode)
	{
		if (this.m_game != null)
		{
			this.m_game.Close();
		}
		this.m_game = new ClientGame(this.m_clientRpc, this.m_guiCamera, this.m_userManClient, this.m_mapMan, this.m_musicMan, replayMode, false, string.Empty);
		this.m_game.m_onExit = new Action<ExitState, int>(this.OnExit);
		this.m_game.m_onQuitGame = new Action(this.OnQuitGame);
		game.JoinGame(this.m_user);
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00052038 File Offset: 0x00050238
	protected override Game CreateGame(string gameName, int gameID, int campaignID, GameType gameType, string campaignName, string levelName, int nrOfPlayers, FleetSizeClass fleetSizeClass, float targetScore, double turnTime, MapInfo mapinfo)
	{
		Game game = new Game(gameName, gameID, campaignID, gameType, campaignName, levelName, nrOfPlayers, fleetSizeClass, targetScore, turnTime, mapinfo, this.m_pacMan);
		this.m_serverGames.Add(game);
		game.m_onGameOver = new Action<Game>(base.OnGameOver);
		game.m_onSaveReplay = new Game.OnSaveReplayDelegate(this.OnSaveReplay);
		if (this.m_onSaveUserRequest != null)
		{
			this.m_onSaveUserRequest();
		}
		return game;
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x000520AC File Offset: 0x000502AC
	private int FindValidPort(ref Socket socket, int startPort)
	{
		for (int i = 0; i < 1000; i++)
		{
			int num = startPort + i;
			IPEndPoint local_end = new IPEndPoint(IPAddress.Any, num);
			try
			{
				socket.Bind(local_end);
			}
			catch (SocketException)
			{
				goto IL_31;
			}
			return num;
			IL_31:;
		}
		return -1;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00052118 File Offset: 0x00050318
	public void OnLevelWasLoaded()
	{
		if (this.m_game != null)
		{
			this.m_game.OnLevelWasLoaded();
		}
		if (this.m_joinTransitionState == OfflineGame.JoinState.LoadMenuLevel)
		{
			this.m_joinTransitionState = OfflineGame.JoinState.JoinGame;
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00052144 File Offset: 0x00050344
	public void FixedUpdate()
	{
		if (this.m_game != null)
		{
			this.m_game.FixedUpdate();
		}
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x0005215C File Offset: 0x0005035C
	public void Update(float dt)
	{
		this.ServerUpdate(dt);
		this.ClientUpdates(dt);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0005216C File Offset: 0x0005036C
	private void ClientUpdates(float dt)
	{
		this.m_clientRpc.Update(false);
		if (this.m_game != null)
		{
			this.m_game.Update();
		}
		if (this.m_joinTransitionState == OfflineGame.JoinState.JoinGame && this.m_joinGame != 0)
		{
			Game serverGame = this.GetServerGame(this.m_joinGame);
			this.JoinGame(serverGame, false);
			this.m_joinGame = 0;
			this.m_joinTransitionState = OfflineGame.JoinState.None;
		}
		if (this.m_joinTransitionState == OfflineGame.JoinState.CloseGame)
		{
			this.m_joinTransitionState = OfflineGame.JoinState.LoadMenuLevel;
			this.m_game.Close();
			this.m_game = null;
			main.LoadLevel("menu", true);
		}
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00052208 File Offset: 0x00050408
	private void ServerUpdate(float dt)
	{
		this.m_serverRpc.Update(true);
		foreach (Game game in this.m_serverGames)
		{
			game.Update(dt);
		}
		this.RemoveOldGames();
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00052284 File Offset: 0x00050484
	private void RemoveOldGames()
	{
		foreach (Game game in this.m_serverGames)
		{
			if (game.IsFinished())
			{
				if (this.m_gameDB != null)
				{
					this.m_gameDB.RemoveGame(game.GetGameID());
				}
				this.m_serverGames.Remove(game);
				break;
			}
		}
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0005231C File Offset: 0x0005051C
	public void LateUpdate()
	{
		if (this.m_game != null)
		{
			this.m_game.LateUpdate();
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00052334 File Offset: 0x00050534
	public void Close()
	{
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		this.ServerUpdate(0.1f);
		this.SaveGames();
		foreach (Game game in this.m_serverGames)
		{
			game.Close();
		}
		this.m_serverGames.Clear();
		this.m_serverRpc.Close();
		this.m_clientRpc.Close();
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x000523E8 File Offset: 0x000505E8
	private void OnExit(ExitState exitState, int joinGameID)
	{
		if (exitState == ExitState.JoinGame)
		{
			DebugUtils.Assert(joinGameID != 0);
			this.m_joinGame = joinGameID;
			this.m_joinTransitionState = OfflineGame.JoinState.CloseGame;
		}
		else
		{
			this.m_onExit(exitState);
		}
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00052428 File Offset: 0x00050628
	private void OnQuitGame()
	{
		if (this.m_onQuitGame != null)
		{
			this.m_onQuitGame();
		}
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00052440 File Offset: 0x00050640
	private bool OnSaveReplay(Game game, User user, string replayName)
	{
		return this.SaveGame(game, true, replayName);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x0005244C File Offset: 0x0005064C
	private Game GetServerGame(int gameID)
	{
		foreach (Game game in this.m_serverGames)
		{
			if (game.GetGameID() == gameID)
			{
				return game;
			}
		}
		return null;
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x000524C4 File Offset: 0x000506C4
	private void SaveGames()
	{
		foreach (Game game in this.m_serverGames)
		{
			if (!game.IsFinished())
			{
				this.SaveGame(game, false, string.Empty);
			}
		}
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0005253C File Offset: 0x0005073C
	private bool SaveGame(Game game, bool replay, string replayName)
	{
		if (this.m_gameDB == null)
		{
			return false;
		}
		GamePost gamePost = new GamePost();
		gamePost.m_campaign = game.GetCampaign();
		gamePost.m_connectedPlayers = 1;
		gamePost.m_createDate = game.GetCreateDate();
		gamePost.m_fleetSizeClass = game.GetFleetSizeClass();
		gamePost.m_gameID = game.GetGameID();
		gamePost.m_gameName = ((!replay) ? game.GetName() : replayName);
		gamePost.m_gameType = game.GetGameType();
		gamePost.m_level = game.GetLevelName();
		gamePost.m_maxPlayers = 1;
		gamePost.m_nrOfPlayers = 1;
		gamePost.m_turn = game.GetTurn();
		byte[] gameData = base.GameToArray(game);
		return this.m_gameDB.SaveGame(gamePost, gameData, replay);
	}

	// Token: 0x0400093D RID: 2365
	public Action<ExitState> m_onExit;

	// Token: 0x0400093E RID: 2366
	public Action m_onQuitGame;

	// Token: 0x0400093F RID: 2367
	public Action m_onSaveUserRequest;

	// Token: 0x04000940 RID: 2368
	private PTech.RPC m_serverRpc;

	// Token: 0x04000941 RID: 2369
	private PTech.RPC m_clientRpc;

	// Token: 0x04000942 RID: 2370
	private List<Game> m_serverGames = new List<Game>();

	// Token: 0x04000943 RID: 2371
	private ClientGame m_game;

	// Token: 0x04000944 RID: 2372
	private GameObject m_guiCamera;

	// Token: 0x04000945 RID: 2373
	private UserManClient m_userManClient;

	// Token: 0x04000946 RID: 2374
	private MusicManager m_musicMan;

	// Token: 0x04000947 RID: 2375
	private PackMan m_pacMan;

	// Token: 0x04000948 RID: 2376
	private OfflineGameDB m_gameDB;

	// Token: 0x04000949 RID: 2377
	private User m_user;

	// Token: 0x0400094A RID: 2378
	private int m_joinGame;

	// Token: 0x0400094B RID: 2379
	private OfflineGame.JoinState m_joinTransitionState;

	// Token: 0x0400094C RID: 2380
	private bool m_replayMode;

	// Token: 0x02000121 RID: 289
	private enum JoinState
	{
		// Token: 0x0400094E RID: 2382
		None,
		// Token: 0x0400094F RID: 2383
		CloseGame,
		// Token: 0x04000950 RID: 2384
		LoadMenuLevel,
		// Token: 0x04000951 RID: 2385
		JoinGame
	}
}
