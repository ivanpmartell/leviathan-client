using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class ClientGame
{
	// Token: 0x06000644 RID: 1604 RVA: 0x0002F234 File Offset: 0x0002D434
	public ClientGame(PTech.RPC rpc, GameObject guiCamera, UserManClient userManClient, MapMan mapman, MusicManager musMan, bool replayMode, bool onlineGame, string hostName)
	{
		ClientGame.m_instance = this;
		this.m_userManClient = userManClient;
		this.m_mapMan = mapman;
		this.m_guiCamera = guiCamera;
		this.m_rpc = rpc;
		this.m_musMan = musMan;
		this.m_replayMode = replayMode;
		this.m_onlineGame = onlineGame;
		this.m_hostName = hostName;
		this.m_turnMan = new TurnMan();
		this.m_chatClient = new ChatClient(rpc);
		this.m_loadScreen = new LoadScreen(this.m_guiCamera);
		this.m_hitText = new HitText(guiCamera);
		this.m_losDrawer = new LosDrawer();
		this.m_voSystem = new VOSystem();
		this.m_rpc.Register("GameSettings", new PTech.RPC.Handler(this.RPC_GameSettings));
		this.m_rpc.Register("DoInitiation", new PTech.RPC.Handler(this.RPC_DoInitiation));
		this.m_rpc.Register("DoSimulation", new PTech.RPC.Handler(this.RPC_DoSimulation));
		this.m_rpc.Register("TurnData", new PTech.RPC.Handler(this.RPC_TurnData));
		this.m_rpc.Register("EndGame", new PTech.RPC.Handler(this.RPC_EndGame));
		this.m_rpc.Register("PlayerStatus", new PTech.RPC.Handler(this.RPC_PlayerStatus));
		this.m_rpc.Register("Lobby", new PTech.RPC.Handler(this.RPC_Lobby));
		this.m_rpc.Register("GameStarted", new PTech.RPC.Handler(this.RPC_GameStarted));
		this.m_rpc.Register("Kicked", new PTech.RPC.Handler(this.RPC_Kicked));
		this.m_rpc.Register("TimeSync", new PTech.RPC.Handler(this.RPC_TimeSync));
		this.m_hud = new Hud(this.m_rpc, this.m_guiCamera, this.m_turnMan, this.m_chatClient, replayMode, this.m_onlineGame);
		this.m_hud.m_onCommit = new Action(this.OnHudCommit);
		this.m_hud.m_onPlayPause = new Action(this.OnPlayPause);
		this.m_hud.m_onStartTest = new Action(this.StartTest);
		this.m_hud.m_onStopTest = new Action(this.StopTest);
		this.m_hud.m_onStopOutcome = new Action(this.StopOutcome);
		this.m_hud.m_onNextTurn = new Action(this.OnNextReplayTurn);
		this.m_hud.m_onPrevTurn = new Action(this.OnPrevReplayTurn);
		this.m_hud.m_onSurrender = new Action(this.OnSurrender);
		this.m_hud.m_onExit = new Action<ExitState, int>(this.OnHudExit);
		this.m_hud.m_onQuitGame = new Action(this.OnHudQuitGame);
		this.m_hud.SetMode(Hud.Mode.Waiting);
		this.m_dialog = new Dialog(this.m_rpc, this.m_guiCamera, this.m_hud, this.m_turnMan);
		this.m_dialog.m_onPlayDialog = new Action<bool>(this.OnPlayDialog);
		this.m_dialog.m_onEndDialog = new Action(this.OnEndDialog);
		ObjectFactory.ResetInstance();
		ShipFactory.ResetInstance();
		ShipFactory.instance.RegisterShips("shared_settings/ship_defs/default_enemy_ships");
		this.m_countdownPrefab = (Resources.Load("DownCountBlipp") as GameObject);
		this.m_autoCommitPrefab = (Resources.Load("AutoCommitSound") as GameObject);
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000645 RID: 1605 RVA: 0x0002F5C8 File Offset: 0x0002D7C8
	public static ClientGame instance
	{
		get
		{
			return ClientGame.m_instance;
		}
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x0002F5D0 File Offset: 0x0002D7D0
	public void Close()
	{
		ClientGame.m_instance = null;
		float num = (this.m_state != ClientGame.State.Planning || this.m_turnData == null) ? 0f : this.m_turnData.m_planningTime;
		this.m_rpc.Invoke("LeaveGame", new object[]
		{
			num
		});
		this.m_rpc.Unregister("GameSettings");
		this.m_rpc.Unregister("DoInitiation");
		this.m_rpc.Unregister("DoSimulation");
		this.m_rpc.Unregister("TurnData");
		this.m_rpc.Unregister("EndGame");
		this.m_rpc.Unregister("PlayerStatus");
		this.m_rpc.Unregister("Lobby");
		this.m_rpc.Unregister("GameStarted");
		this.m_rpc.Unregister("Kicked");
		this.m_rpc.Unregister("TimeSync");
		if (this.m_voSystem != null)
		{
			this.m_voSystem.Close();
		}
		if (this.m_lobbyMenu != null)
		{
			this.m_lobbyMenu.Close();
		}
		if (this.m_hud != null)
		{
			this.m_hud.Close();
		}
		if (this.m_hitText != null)
		{
			this.m_hitText.Close();
		}
		if (this.m_dialog != null)
		{
			this.m_dialog.Close();
		}
		if (this.m_endGameMenu != null)
		{
			this.m_endGameMenu.Close();
		}
		if (this.m_endGameMsg != null)
		{
			UnityEngine.Object.Destroy(this.m_endGameMsg);
		}
		if (this.m_loadScreen != null)
		{
			this.m_loadScreen.Close();
			this.m_loadScreen = null;
		}
		this.m_chatClient.Close();
		NetObj.ResetObjectDB();
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0002F79C File Offset: 0x0002D99C
	public void FixedUpdate()
	{
		if (this.m_gameState != null)
		{
			this.m_gameState.FixedUpdate();
		}
		this.UpdateHudPlaybackData();
		if (this.m_endGameMenu == null)
		{
			if (this.m_gameState != null && this.m_gameSettings != null)
			{
				bool inPlanning = this.m_state == ClientGame.State.Planning;
				this.m_hud.UpdatePlayerStates(this.m_players, this.m_gameSettings.m_localPlayerID, inPlanning, this.m_replayMode, this.m_gameSettings.m_localAdmin);
			}
			this.m_hud.FixedUpdate();
			this.m_dialog.FixedUpdate();
		}
		if (this.m_focusCamera >= 0)
		{
			this.m_focusCamera--;
			Vector3 pos;
			if (this.m_focusCamera < 0 && !this.m_replayMode && GameUtils.FindCameraStartPos(this.m_gameSettings.m_localPlayerID, out pos))
			{
				this.m_gameState.GetGameCamera().SetFocus(pos, 450f);
			}
		}
		if (this.m_state == ClientGame.State.Planning && this.m_endGameMenu == null && this.m_lobbyMenu == null && this.m_turnData != null)
		{
			this.m_turnData.m_planningTime += Time.fixedDeltaTime;
		}
		this.UpdateAutoCommit(Time.fixedDeltaTime);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0002F8E4 File Offset: 0x0002DAE4
	private void UpdateAutoCommit(float dt)
	{
		if (this.m_turnData == null || this.m_gameSettings.m_maxTurnTime <= 0.0 || this.m_turnData.m_turnDuration > this.m_gameSettings.m_maxTurnTime)
		{
			return;
		}
		double turnDuration = this.m_turnData.m_turnDuration;
		this.m_turnData.m_turnDuration += (double)dt;
		double num = this.m_gameSettings.m_maxTurnTime - this.m_turnData.m_turnDuration;
		if (num < 6.0 && num > 1.0 && (long)turnDuration != (long)this.m_turnData.m_turnDuration)
		{
			UnityEngine.Object.Instantiate(this.m_countdownPrefab);
		}
		if (num <= 0.0 && this.m_state == ClientGame.State.Planning && this.m_turnData.m_needCommit)
		{
			PLog.Log("autocommiting");
			UnityEngine.Object.Instantiate(this.m_autoCommitPrefab);
			this.Commit();
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x0002F9F0 File Offset: 0x0002DBF0
	private void UpdateHudPlaybackData()
	{
		int frame = 0;
		int totalFrames = 0;
		int num = 0;
		if (this.m_gameState != null)
		{
			frame = this.m_gameState.GetCurrentFrame();
			totalFrames = this.m_gameState.GetTotalFrames();
		}
		if (this.m_state == ClientGame.State.Simulating && this.m_simData != null)
		{
			num = this.m_simData.m_simulationTurn;
		}
		else if (this.m_turnData != null)
		{
			if (this.m_state == ClientGame.State.Outcome)
			{
				num = this.m_turnData.m_turn - 1;
			}
			else
			{
				num = this.m_turnData.m_turn;
			}
		}
		double turnTimeLeft = -1.0;
		if (!this.m_replayMode && this.m_turnData != null && this.m_gameSettings.m_maxTurnTime > 0.0)
		{
			turnTimeLeft = this.m_gameSettings.m_maxTurnTime - this.m_turnData.m_turnDuration;
		}
		num++;
		this.m_hud.SetPlaybackData(frame, totalFrames, num, turnTimeLeft);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x0002FAE8 File Offset: 0x0002DCE8
	public void Update()
	{
		if (this.m_loadScreen != null)
		{
			this.m_loadScreen.Update();
		}
		if (this.m_gameState != null && this.m_gameState.GetGameModeScript() != null)
		{
			this.m_hud.SetTargetScore(this.m_gameState.GetGameModeScript().GetTargetScore());
			this.m_hud.m_showObjectives = this.m_gameState.GetGameModeScript().HasObjectives();
		}
		if (this.m_gameState != null && this.m_gameState.GetGameCamera() != null)
		{
			this.m_hud.Update(this.m_gameState.GetGameCamera().camera);
		}
		if (!this.m_replayMode && !CheatMan.instance.GetNoFogOfWar())
		{
			this.m_losDrawer.Draw();
		}
		this.m_hitText.Update(Time.deltaTime);
		this.m_dialog.Update(this.m_players);
		this.m_voSystem.Update(Time.deltaTime);
		if (this.m_lobbyMenu != null)
		{
			this.m_lobbyMenu.Update();
		}
		if (this.m_endGameMenu != null)
		{
			this.m_endGameMenu.Update();
		}
		if (Utils.IsAndroidBack() && !this.m_hud.DismissAnyPopup())
		{
			PLog.Log("Now Quit Game");
			Utils.AndroidBack();
			this.OnHudExit(ExitState.Normal, 0);
		}
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x0002FC54 File Offset: 0x0002DE54
	public void LateUpdate()
	{
		if (this.m_gameState != null && this.m_gameState.GetGameCamera() != null)
		{
			this.m_hitText.LateUpdate(this.m_gameState.GetGameCamera().GetComponent<Camera>());
		}
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0002FCA0 File Offset: 0x0002DEA0
	public void OnLevelWasLoaded()
	{
		this.TryCloseEndGameMenu();
		if (this.m_gameState != null)
		{
			this.m_gameState.OnLevelWasLoaded();
		}
		if (this.m_lobbyMenu != null)
		{
			this.m_lobbyMenu.OnLevelWasLoaded();
		}
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0002FCE0 File Offset: 0x0002DEE0
	private void RPC_GameSettings(PTech.RPC rpc, List<object> args)
	{
		this.m_gameSettings = new GameSettings();
		int num = 0;
		this.m_gameSettings.m_localPlayerID = (int)args[num++];
		this.m_gameSettings.m_localAdmin = (bool)args[num++];
		this.m_gameSettings.m_campaignID = (int)args[num++];
		this.m_gameSettings.m_gameID = (int)args[num++];
		this.m_gameSettings.m_gameName = (string)args[num++];
		this.m_gameSettings.m_gameType = (GameType)((int)args[num++]);
		this.m_gameSettings.m_campaign = (string)args[num++];
		this.m_gameSettings.m_level = (string)args[num++];
		this.m_gameSettings.m_fleetSizeClass = (FleetSizeClass)((int)args[num++]);
		this.m_gameSettings.m_fleetSizeLimits.min = (int)args[num++];
		this.m_gameSettings.m_fleetSizeLimits.max = (int)args[num++];
		this.m_gameSettings.m_targetScore = (float)args[num++];
		this.m_gameSettings.m_maxTurnTime = (double)args[num++];
		this.m_gameSettings.m_nrOfPlayers = (int)args[num++];
		this.m_gameSettings.m_mapInfo = this.m_mapMan.GetMapByName(this.m_gameSettings.m_gameType, this.m_gameSettings.m_campaign, this.m_gameSettings.m_level);
		this.m_gameSettings.m_campaignInfo = this.m_mapMan.GetCampaign(this.m_gameSettings.m_campaign);
		if (this.m_replayMode)
		{
			this.m_gameSettings.m_localPlayerID = -1;
		}
		if (this.m_loadScreen != null)
		{
			this.m_loadScreen.SetImage(this.m_gameSettings.m_mapInfo.m_loadscreen);
		}
		this.m_hud.SetGameInfo(this.m_gameSettings.m_gameName, this.m_gameSettings.m_gameType, this.m_gameSettings.m_gameID, this.m_gameSettings.m_localAdmin);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0002FF4C File Offset: 0x0002E14C
	private void RPC_GameStarted(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("game started");
		DebugUtils.Assert(this.m_gameSettings != null);
		this.m_musMan.SetMusic(string.Empty);
		this.m_hud.SetVisible(true, true, true);
		this.m_loadScreen.SetVisible(true);
		this.CloseLobby();
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
	private void OnLobbyExit()
	{
		PLog.Log("clientgame:OnLobbyExit()");
		this.CloseLobby();
		if (this.m_onExit != null)
		{
			this.m_onExit(ExitState.Normal, 0);
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0002FFDC File Offset: 0x0002E1DC
	private void Lobby_OnInviteClicked(LobbyPlayer sender)
	{
		PLog.Log("ClientGame recived from Lobby that " + sender.m_name + " wants to invite friends.");
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0002FFF8 File Offset: 0x0002E1F8
	private void RPC_Lobby(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("Got lobby data");
		int num = 0;
		string localName = (string)args[num++];
		bool allowMatchmaking = (bool)args[num++];
		bool noFleet = (bool)args[num++];
		int num2 = (int)args[num++];
		List<LobbyPlayer> list = new List<LobbyPlayer>();
		for (int i = 0; i < num2; i++)
		{
			list.Add(new LobbyPlayer
			{
				m_id = (int)args[num++],
				m_team = (int)args[num++],
				m_name = (string)args[num++],
				m_flag = (int)args[num++],
				m_admin = (bool)args[num++],
				m_readyToStart = (bool)args[num++],
				m_status = (PlayerPresenceStatus)((int)args[num++]),
				m_fleet = (string)args[num++],
				m_fleetValue = (int)args[num++]
			});
		}
		this.SetupLobby(localName, this.m_gameSettings, noFleet, allowMatchmaking, list);
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00030164 File Offset: 0x0002E364
	private void SetupLobby(string localName, GameSettings gameSettings, bool noFleet, bool allowMatchmaking, List<LobbyPlayer> playerList)
	{
		this.m_hud.SetVisible(false, true, true);
		if (this.m_lobbyMenu == null)
		{
			this.m_lobbyMenu = new LobbyMenu(this.m_guiCamera, this.m_gameSettings.m_mapInfo, this.m_rpc, this.m_userManClient, this.m_gameSettings.m_campaignID, this.m_chatClient, this.m_musMan, this.m_hostName);
			this.m_lobbyMenu.m_onExit = new Action(this.OnLobbyExit);
			this.m_lobbyMenu.m_playerRemovedDelegate = new Action<string>(this.Lobby_RemovedPlayer);
		}
		this.m_lobbyMenu.Setup(noFleet, allowMatchmaking, this.m_gameSettings, playerList);
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00030214 File Offset: 0x0002E414
	private void Lobby_RemovedPlayer(string playerName)
	{
		PLog.Log("ClientGame recived from Lobby that " + playerName + " wants to leave the loby friends. (or be kicked).");
		this.m_rpc.Invoke("KickPlayer", new object[]
		{
			playerName
		});
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00030248 File Offset: 0x0002E448
	private void CloseLobby()
	{
		if (this.m_lobbyMenu == null)
		{
			return;
		}
		LobbyMenu lobbyMenu = this.m_lobbyMenu;
		lobbyMenu.m_onExit = (Action)Delegate.Remove(lobbyMenu.m_onExit, new Action(this.OnLobbyExit));
		this.m_lobbyMenu.Close();
		this.m_lobbyMenu = null;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0003029C File Offset: 0x0002E49C
	private void SetupEndgameMenu()
	{
		DebugUtils.Assert(this.m_endGameData != null);
		this.m_hitText.Clear();
		this.m_hud.SetVisible(false, true, true);
		this.m_dialog.Hide();
		if (this.m_gameState != null && this.m_gameState.GetGameCamera() != null)
		{
			this.m_gameState.GetGameCamera().SetEnabled(false);
		}
		if (this.m_gameState != null && this.m_gameState.GetGameCamera() != null)
		{
			this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Disabled);
		}
		this.m_endGameMenu = new EndGameMenu(this.m_guiCamera, this.m_endGameData, this.m_gameSettings, this.m_rpc, this.m_chatClient, this.m_musMan);
		this.m_endGameMenu.m_onLeavePressed = new Action<int>(this.OnEndGameExit);
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x00030388 File Offset: 0x0002E588
	private void OnEndGameExit(int joinGameID)
	{
		PLog.Log("** OnEndGame_Leave **");
		if (joinGameID > 0)
		{
			this.m_onExit(ExitState.JoinGame, joinGameID);
		}
		else if (this.m_gameSettings.m_gameType == GameType.Campaign && this.m_gameSettings.m_campaignInfo != null && !this.m_gameSettings.m_campaignInfo.m_tutorial)
		{
			this.m_onExit(ExitState.ShowCredits, 0);
		}
		else
		{
			this.m_onExit(ExitState.Normal, 0);
		}
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00030410 File Offset: 0x0002E610
	private void TryCloseEndGameMenu()
	{
		this.m_endGameData = null;
		if (this.m_endGameMenu != null)
		{
			this.m_endGameMenu.Close();
			this.m_endGameMenu = null;
		}
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00030444 File Offset: 0x0002E644
	private void OnHudExit(ExitState state, int gameID)
	{
		PLog.LogWarning("Clientgame::OnHudExit()");
		this.m_onExit(state, gameID);
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x00030460 File Offset: 0x0002E660
	private void OnHudQuitGame()
	{
		if (this.m_onQuitGame != null)
		{
			this.m_onQuitGame();
		}
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00030478 File Offset: 0x0002E678
	private void RPC_DoInitiation(PTech.RPC rpc, List<object> args)
	{
		DebugUtils.Assert(this.m_gameSettings != null);
		int num = 0;
		this.m_initializeData = new ClientGame.InitializeData();
		for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
		{
			PlayerInitData playerInitData = new PlayerInitData();
			bool flag = (bool)args[num++];
			if (flag)
			{
				byte[] data = (byte[])args[num++];
				FleetDef fleet = new FleetDef(data);
				playerInitData.m_fleet = fleet;
			}
			playerInitData.m_id = (int)args[num++];
			playerInitData.m_name = (string)args[num++];
			playerInitData.m_team = (int)args[num++];
			playerInitData.m_flag = (int)args[num++];
			this.m_initializeData.m_players.Add(playerInitData);
		}
		int frames = 60;
		this.m_gameState = new GameState(this.m_guiCamera, this.m_gameSettings, this.m_turnMan, TurnPhase.Simulating, false, null, -1, frames, new GameState.SetupCompleteHandler(this.OnInitiationSetupComplete), new GameState.SimulationCompleteHandler(this.OnInitiationComplete));
		this.m_turnMan.SetNrOfPlayers(this.m_gameSettings.m_nrOfPlayers);
		this.m_turnMan.SetGameType(this.m_gameSettings.m_gameType);
		foreach (PlayerInitData playerInitData2 in this.m_initializeData.m_players)
		{
			this.m_turnMan.SetPlayerName(playerInitData2.m_id, playerInitData2.m_name);
			this.m_turnMan.SetPlayerTeam(playerInitData2.m_id, playerInitData2.m_team);
			this.m_turnMan.SetPlayerHuman(playerInitData2.m_id, true);
			this.m_turnMan.SetPlayerFlag(playerInitData2.m_id, playerInitData2.m_flag);
		}
		int num2 = 0;
		int num3 = 0;
		foreach (PlayerInitData playerInitData3 in this.m_initializeData.m_players)
		{
			if (this.m_gameSettings.m_gameType == GameType.Campaign || this.m_gameSettings.m_gameType == GameType.Challenge)
			{
				this.m_turnMan.SetPlayerColors(playerInitData3.m_id, Constants.m_coopColors[playerInitData3.m_id]);
			}
			else
			{
				Color color;
				if (playerInitData3.m_team == 0)
				{
					color = Constants.m_teamColors1[num2++];
				}
				else
				{
					color = Constants.m_teamColors2[num3++];
				}
				this.m_turnMan.SetPlayerColors(playerInitData3.m_id, color);
			}
		}
		CheatMan.instance.ResetCheats();
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00030798 File Offset: 0x0002E998
	private void OnInitiationSetupComplete()
	{
		PLog.Log(" OnInitiationSetupComplete");
		this.m_gameState.GetGameModeScript().InitializeGame(this.m_initializeData.m_players);
		this.m_initializeData.m_startState = this.m_gameState.GetState();
		this.m_initializeData.m_StartOrders = this.m_gameState.GetOrders(-1);
		this.m_gameState.SetSimulating(true);
		this.m_gameState.GetGameCamera().SetEnabled(false);
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00030814 File Offset: 0x0002EA14
	private void OnInitiationComplete()
	{
		PLog.Log(" OnInitiationComplete");
		int totalFrames = this.m_gameState.GetTotalFrames();
		byte[] state = this.m_gameState.GetState();
		byte[] orders = this.m_gameState.GetOrders(-1);
		this.m_rpc.Invoke("InitialState", new object[]
		{
			CompressUtils.Compress(this.m_initializeData.m_startState),
			CompressUtils.Compress(this.m_initializeData.m_StartOrders),
			CompressUtils.Compress(state),
			CompressUtils.Compress(orders),
			totalFrames
		});
		this.m_gameState = null;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x000308B0 File Offset: 0x0002EAB0
	private void RPC_DoSimulation(PTech.RPC rpc, List<object> args)
	{
		this.m_loadScreen.SetVisible(false);
		this.m_simData = new ClientGame.SimulationData();
		int num = 0;
		this.m_simData.m_simulationTurn = (int)args[num++];
		this.m_simData.m_frames = (int)args[num++];
		this.m_simData.m_startState = CompressUtils.Decompress((byte[])args[num++]);
		this.m_simData.m_combinedStartOrders = CompressUtils.Decompress((byte[])args[num++]);
		this.m_simData.m_surrenders = (int[])args[num++];
		int num2 = (int)args[num++];
		this.m_simData.m_playerOrders = new List<byte[]>();
		for (int i = 0; i < num2; i++)
		{
			bool flag = (bool)args[num++];
			if (flag)
			{
				this.m_simData.m_playerOrders.Add(CompressUtils.Decompress((byte[])args[num++]));
			}
			else
			{
				this.m_simData.m_playerOrders.Add(null);
			}
		}
		this.UpdateHudPlaybackData();
		this.m_hud.SetMode(Hud.Mode.Outcome);
		this.StartSimulation();
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x00030A04 File Offset: 0x0002EC04
	private void StartSimulation()
	{
		this.m_gameState = new GameState(this.m_guiCamera, this.m_gameSettings, this.m_turnMan, TurnPhase.Playback, false, this.m_simData.m_startState, this.m_gameSettings.m_localPlayerID, this.m_simData.m_frames, new GameState.SetupCompleteHandler(this.OnSimulationSetupComplete), new GameState.SimulationCompleteHandler(this.OnSimulationComplete));
		this.m_state = ClientGame.State.Simulating;
		NetObj.SetDrawOrders(false);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00030A78 File Offset: 0x0002EC78
	private void OnSimulationSetupComplete()
	{
		this.m_gameState.SetOrders(-1, this.m_simData.m_combinedStartOrders);
		for (int i = 0; i < this.m_simData.m_playerOrders.Count; i++)
		{
			byte[] array = this.m_simData.m_playerOrders[i];
			if (array != null)
			{
				this.m_gameState.SetOrders(i, array);
			}
		}
		this.m_simData.m_combinedOrders = this.m_gameState.GetOrders(-1);
		this.m_gameState.SetSimulating(true);
		this.m_gameState.GetGameCamera().GetComponent<GameCamera>().SetMode(GameCamera.Mode.Passive);
		this.SetupTurnMusic();
		this.ShowSurrenderMessages(this.m_simData.m_surrenders);
		VOSystem.instance.ResetTurnflags();
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00030B3C File Offset: 0x0002ED3C
	private void OnSimulationComplete()
	{
		GameMode gameModeScript = this.m_gameState.GetGameModeScript();
		TurnMan turnMan = this.m_gameState.GetTurnMan();
		if (!this.m_replayMode && gameModeScript.GetOutcome() != GameOutcome.None)
		{
			this.CheckLocalEndGameAchivements();
		}
		List<object> list = new List<object>();
		list.Add(this.m_simData.m_simulationTurn);
		list.Add((int)gameModeScript.GetOutcome());
		list.Add(gameModeScript.GetWinnerTeam(this.m_gameSettings.m_nrOfPlayers));
		list.Add(this.m_gameSettings.m_nrOfPlayers);
		for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
		{
			int num = i;
			int playerPlace = gameModeScript.GetPlayerPlace(num, this.m_gameSettings.m_nrOfPlayers);
			int playerScore = turnMan.GetPlayerScore(num);
			int teamScoreForPlayer = turnMan.GetTeamScoreForPlayer(num);
			int flagshipKiller = turnMan.GetFlagshipKiller(num, 0);
			int flagshipKiller2 = turnMan.GetFlagshipKiller(num, 1);
			list.Add(num);
			list.Add(playerPlace);
			list.Add(playerScore);
			list.Add(teamScoreForPlayer);
			list.Add(flagshipKiller);
			list.Add(flagshipKiller2);
			list.Add(gameModeScript.IsPlayerDead(i));
			TurnMan.PlayerTurnData player = turnMan.GetPlayer(num);
			list.Add(player.m_turnDamage);
			list.Add(player.m_turnFriendlyDamage);
			list.Add(player.m_turnShipsSunk);
			list.Add(player.m_turnGunDamage);
		}
		list.Add(CompressUtils.Compress(this.m_simData.m_startState));
		list.Add(CompressUtils.Compress(this.m_simData.m_combinedOrders));
		list.Add(CompressUtils.Compress(this.m_gameState.GetState()));
		list.Add(CompressUtils.Compress(this.m_gameState.GetOrders(-1)));
		list.Add(this.m_simData.m_surrenders);
		list.Add(this.m_gameState.GetCurrentFrame());
		this.m_rpc.Invoke("SimulationResults", list);
		this.m_simData = null;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00030D80 File Offset: 0x0002EF80
	private void RPC_TimeSync(PTech.RPC rpc, List<object> args)
	{
		double turnDuration = (double)args[0];
		if (this.m_turnData != null)
		{
			this.m_turnData.m_turnDuration = turnDuration;
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x00030DB4 File Offset: 0x0002EFB4
	private void RPC_TurnData(PTech.RPC rpc, List<object> args)
	{
		this.m_loadScreen.SetVisible(false);
		int num = 0;
		this.m_turnData = new ClientGame.TurnData();
		this.m_turnData.m_turn = (int)args[num++];
		this.m_turnData.m_turnType = (TurnType)((int)args[num++]);
		this.m_turnData.m_needCommit = (bool)args[num++];
		this.m_turnData.m_turnDuration = (double)args[num++];
		this.m_turnData.m_myOrders = CompressUtils.Decompress((byte[])args[num++]);
		this.m_turnData.m_startState = CompressUtils.Decompress((byte[])args[num++]);
		this.m_turnData.m_orders = CompressUtils.Decompress((byte[])args[num++]);
		this.m_turnData.m_endState = CompressUtils.Decompress((byte[])args[num++]);
		this.m_turnData.m_endOrders = CompressUtils.Decompress((byte[])args[num++]);
		this.m_turnData.m_playbackFrames = (int)args[num++];
		this.m_turnData.m_frames = (int)args[num++];
		this.m_turnData.m_surrenders = (int[])args[num++];
		bool flag = (bool)args[num++];
		if (flag)
		{
			PLog.Log("Got replay data, starting replay");
			this.StartOutcomePlayback(false);
		}
		else
		{
			this.m_state = ClientGame.State.Outcome;
			this.OnOutcomeComplete();
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00030F74 File Offset: 0x0002F174
	private void StartOutcomePlayback(bool isReplay)
	{
		this.m_state = ClientGame.State.Outcome;
		this.m_isOutcomeReplay = isReplay;
		this.m_gameState = new GameState(this.m_guiCamera, this.m_gameSettings, this.m_turnMan, TurnPhase.Playback, false, this.m_turnData.m_startState, this.m_gameSettings.m_localPlayerID, this.m_turnData.m_playbackFrames, new GameState.SetupCompleteHandler(this.OnOutcomeSetupComplete), new GameState.SimulationCompleteHandler(this.OnOutcomeComplete));
		NetObj.SetDrawOrders(false);
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00030FF0 File Offset: 0x0002F1F0
	private void OnOutcomeSetupComplete()
	{
		this.m_gameState.SetOrders(-1, this.m_turnData.m_orders);
		this.m_gameState.SetSimulating(true);
		this.UpdateHudPlaybackData();
		if (this.m_replayMode)
		{
			this.m_hud.SetMode(Hud.Mode.Replay);
		}
		else
		{
			this.m_hud.SetMode((!this.m_isOutcomeReplay) ? Hud.Mode.Outcome : Hud.Mode.ReplayOutcome);
		}
		this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Passive);
		this.m_gameState.GetGameCamera().SetEnabled(true);
		if (this.m_firstLook)
		{
			this.m_firstLook = false;
			this.m_focusCamera = 10;
		}
		this.ShowSurrenderMessages(this.m_turnData.m_surrenders);
		this.SetupTurnMusic();
		VOSystem.instance.ResetTurnflags();
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000310BC File Offset: 0x0002F2BC
	private void ShowSurrenderMessages(int[] surrenders)
	{
		foreach (int playerID in surrenders)
		{
			string playerName = this.m_turnMan.GetPlayerName(playerID);
			MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, playerName + " $label_surrender_newsflash", string.Empty, "NewsflashMessage", 2f);
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00031118 File Offset: 0x0002F318
	public void AwardAchievement(int owner, int id)
	{
		if (owner != this.m_gameSettings.m_localPlayerID)
		{
			return;
		}
		PLog.Log("AwardAchievement " + id.ToString());
		this.m_userManClient.UnlockAchievement(id);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0003115C File Offset: 0x0002F35C
	private void CheckLocalEndGameAchivements()
	{
		this.m_gameState.GetGameModeScript().CheckAchivements(this.m_userManClient);
		if (this.m_gameSettings.m_nrOfPlayers == 4)
		{
			this.m_userManClient.UnlockAchievement(2);
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x00031194 File Offset: 0x0002F394
	private void SetupTurnMusic()
	{
		if (this.m_gameSettings.m_gameType == GameType.Points || this.m_gameSettings.m_gameType == GameType.Assassination)
		{
			string @string = PlayerPrefs.GetString("CustomVSMusic", string.Empty);
			if (@string != string.Empty)
			{
				this.m_musMan.SetMusic(@string);
				return;
			}
		}
		if (this.m_replayMode)
		{
			this.m_musMan.SetMusic("replay");
		}
		else
		{
			this.m_musMan.SetMusic(this.m_turnMan.GetTurnMusic());
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00031228 File Offset: 0x0002F428
	private void OnOutcomeComplete()
	{
		GameMode gameModeScript = this.m_gameState.GetGameModeScript();
		if (!this.m_replayMode && gameModeScript.GetOutcome() != GameOutcome.None)
		{
			this.CheckLocalEndGameAchivements();
		}
		if (this.m_endGameData != null)
		{
			this.ShowEndGameMessage();
		}
		else if (this.m_replayMode)
		{
			this.RequestNextTurn();
		}
		else
		{
			this.SetupPlanning();
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00031290 File Offset: 0x0002F490
	private void SetupPlanning()
	{
		this.m_gameState = new GameState(this.m_guiCamera, this.m_gameSettings, this.m_turnMan, TurnPhase.Planning, false, this.m_turnData.m_endState, this.m_gameSettings.m_localPlayerID, this.m_turnData.m_frames, new GameState.SetupCompleteHandler(this.OnPlanningSetupComplete), null);
		this.m_state = ClientGame.State.Planning;
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x000312F4 File Offset: 0x0002F4F4
	private void OnPlanningSetupComplete()
	{
		NetObj.SetDrawOrders(true);
		this.m_gameState.SetOrders(-1, this.m_turnData.m_endOrders);
		if (this.m_turnData.m_myOrders.Length > 0)
		{
			this.m_gameState.SetOrders(this.m_gameSettings.m_localPlayerID, this.m_turnData.m_myOrders);
		}
		if (this.m_turnData.m_tempOrders != null)
		{
			this.m_gameState.SetOrders(this.m_gameSettings.m_localPlayerID, this.m_turnData.m_tempOrders);
		}
		this.m_gameState.ClearNonLocalOrders();
		this.SetupTurnMusic();
		if (this.m_turnData.m_turn == 0)
		{
			VOSystem.instance.DoEvent("Match start");
		}
		if (this.m_turnData.m_needCommit)
		{
			this.UpdateHudPlaybackData();
			this.m_hud.SetMode(Hud.Mode.Planning);
			this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Active);
		}
		else
		{
			this.SetupWaiting();
		}
		if (this.m_turnData.m_turnType == TurnType.EndGame)
		{
			this.SetupEndgame();
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00031408 File Offset: 0x0002F608
	private void OnPlayDialog(bool hideBattlebar)
	{
		this.m_dialog.Show();
		if (this.m_state != ClientGame.State.Planning)
		{
			this.OnPlayPause();
		}
		this.m_hud.SetVisible(false, true, hideBattlebar);
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00031440 File Offset: 0x0002F640
	private void OnEndDialog()
	{
		if (TurnMan.instance.m_endGame == GameOutcome.None)
		{
			this.m_dialog.Hide();
			this.m_hud.SetVisible(true, true, true);
		}
		if (this.m_state != ClientGame.State.Planning)
		{
			this.OnPlayPause();
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00031488 File Offset: 0x0002F688
	private void OnHudCommit()
	{
		this.Commit();
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00031490 File Offset: 0x0002F690
	private void Commit()
	{
		if (this.m_state == ClientGame.State.Planning && this.m_turnData.m_needCommit)
		{
			List<object> list = new List<object>();
			list.Add(this.m_turnData.m_turn);
			list.Add(this.m_turnData.m_localSurrender);
			list.Add(this.m_turnData.m_planningTime);
			list.Add(CompressUtils.Compress(this.m_gameState.GetOrders(this.m_gameSettings.m_localPlayerID)));
			this.m_rpc.Invoke("Commit", list);
			this.m_turnData.m_needCommit = false;
			this.SetupWaiting();
			VOSystem.instance.DoEvent("Press commit");
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00031554 File Offset: 0x0002F754
	private void SetupEndgame()
	{
		this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Passive);
		this.m_hud.SetMode((!this.m_replayMode) ? Hud.Mode.Outcome : Hud.Mode.Replay);
		this.m_state = ClientGame.State.Waiting;
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00031598 File Offset: 0x0002F798
	private void SetupWaiting()
	{
		this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Passive);
		this.m_hud.SetMode(Hud.Mode.Waiting);
		this.m_state = ClientGame.State.Waiting;
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x000315CC File Offset: 0x0002F7CC
	private void ClearNonLocalOrders(int playerID)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject gameObject in array)
		{
			Unit unit = gameObject.GetComponent<NetObj>() as Unit;
			if (unit != null && unit.GetOwner() != playerID)
			{
				unit.ClearOrders();
			}
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0003162C File Offset: 0x0002F82C
	private void OnPlayPause()
	{
		if (this.m_state == ClientGame.State.Outcome || this.m_state == ClientGame.State.Simulating)
		{
			this.m_gameState.SetSimulating(!this.m_gameState.IsSimulating());
		}
		else if (this.m_state == ClientGame.State.Planning)
		{
			this.m_turnData.m_tempOrders = this.m_gameState.GetOrders(this.m_gameSettings.m_localPlayerID);
			this.StartOutcomePlayback(true);
		}
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x000316A4 File Offset: 0x0002F8A4
	private void StartTest()
	{
		if (this.m_state != ClientGame.State.Planning)
		{
			return;
		}
		this.m_state = ClientGame.State.Test;
		this.m_turnData.m_tempOrders = this.m_gameState.GetOrders(this.m_gameSettings.m_localPlayerID);
		this.m_gameState = new GameState(this.m_guiCamera, this.m_gameSettings, this.m_turnMan, TurnPhase.Testing, false, this.m_turnData.m_endState, this.m_gameSettings.m_localPlayerID, this.m_turnData.m_frames, new GameState.SetupCompleteHandler(this.OnTestingSetupComplete), new GameState.SimulationCompleteHandler(this.OnTestingComplete));
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00031740 File Offset: 0x0002F940
	private void OnTestingSetupComplete()
	{
		this.m_gameState.SetOrders(-1, this.m_turnData.m_endOrders);
		this.m_gameState.SetOrders(-1, this.m_turnData.m_tempOrders);
		this.m_gameState.SetSimulating(true);
		NetObj.SetDrawOrders(false);
		this.m_gameState.GetGameCamera().SetMode(GameCamera.Mode.Passive);
		this.UpdateHudPlaybackData();
		this.m_hud.SetMode(Hud.Mode.Outcome);
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x000317B0 File Offset: 0x0002F9B0
	private void OnTestingComplete()
	{
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x000317B4 File Offset: 0x0002F9B4
	private void StopOutcome()
	{
		if (this.m_state == ClientGame.State.Outcome && this.m_isOutcomeReplay)
		{
			this.SetupPlanning();
		}
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x000317D4 File Offset: 0x0002F9D4
	private void StopTest()
	{
		if (this.m_state == ClientGame.State.Test)
		{
			this.SetupPlanning();
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000317E8 File Offset: 0x0002F9E8
	private void RequestNextTurn()
	{
		if (this.m_turnData != null && this.m_turnData.m_turn != -1)
		{
			this.m_rpc.Invoke("RequestReplayTurn", new object[]
			{
				this.m_turnData.m_turn + 1
			});
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0003183C File Offset: 0x0002FA3C
	private void OnNextReplayTurn()
	{
		this.RequestNextTurn();
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00031844 File Offset: 0x0002FA44
	private void OnPrevReplayTurn()
	{
		if (this.m_turnData != null && this.m_turnData.m_turn != -1)
		{
			this.m_rpc.Invoke("RequestReplayTurn", new object[]
			{
				this.m_turnData.m_turn - 1
			});
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00031898 File Offset: 0x0002FA98
	private int CheckNrOfUnits(int owner)
	{
		int num = 0;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Unit");
		foreach (GameObject gameObject in array)
		{
			Unit component = gameObject.GetComponent<Unit>();
			if (component != null && !component.IsDead() && component.GetOwner() == owner)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00031904 File Offset: 0x0002FB04
	private bool IsSimulating()
	{
		return NetObj.IsSimulating();
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0003190C File Offset: 0x0002FB0C
	private void RPC_EndGame(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("RPC_EndGame");
		if (this.m_endGameMenu != null)
		{
			return;
		}
		this.m_endGameData = new EndGameData();
		int num = 0;
		this.m_endGameData.m_localPlayerID = (int)args[num++];
		this.m_endGameData.m_outcome = (GameOutcome)((int)args[num++]);
		this.m_endGameData.m_winnerTeam = (int)args[num++];
		this.m_endGameData.m_autoJoinGameID = (int)args[num++];
		this.m_endGameData.m_turns = (int)args[num++];
		int num2 = (int)args[num++];
		for (int i = 0; i < num2; i++)
		{
			EndGame_PlayerStatistics endGame_PlayerStatistics = new EndGame_PlayerStatistics
			{
				m_playerID = (int)args[num++],
				m_team = (int)args[num++],
				m_name = (string)args[num++],
				m_flag = (int)args[num++],
				m_place = (int)args[num++],
				m_score = (int)args[num++],
				m_teamScore = (int)args[num++],
				m_flagshipKiller0 = (int)args[num++],
				m_flagshipKiller1 = (int)args[num++],
				m_shipsSunk = -1,
				m_shipsLost = -1,
				m_shipsDamaged = -1
			};
			endGame_PlayerStatistics.m_shipsSunk = this.m_turnMan.GetTotalShipsSunk(endGame_PlayerStatistics.m_playerID);
			endGame_PlayerStatistics.m_shipsLost = this.m_turnMan.GetTotalShipsLost(endGame_PlayerStatistics.m_playerID);
			if (endGame_PlayerStatistics.m_playerID == this.m_endGameData.m_localPlayerID)
			{
				this.m_endGameData.m_localPlayer = endGame_PlayerStatistics;
			}
			this.m_endGameData.m_players.Add(endGame_PlayerStatistics);
		}
		TurnMan.PlayerTurnData accoladeDestory = TurnMan.instance.GetAccoladeDestory(true);
		if (accoladeDestory.m_totalDamageInflicted == 0)
		{
			this.m_endGameData.m_AccoladeDestroy = Localize.instance.Translate("$label_notapplicable");
		}
		else
		{
			this.m_endGameData.m_AccoladeDestroy = string.Concat(new object[]
			{
				accoladeDestory.m_name,
				": ",
				accoladeDestory.m_totalDamageInflicted,
				" ",
				Localize.instance.Translate("$accolade_destroyer")
			});
		}
		TurnMan.PlayerTurnData accoladeDestory2 = TurnMan.instance.GetAccoladeDestory(false);
		if (accoladeDestory2.m_totalDamageInflicted == 0)
		{
			this.m_endGameData.m_AccoladeHarmless = Localize.instance.Translate("$label_notapplicable");
		}
		else
		{
			this.m_endGameData.m_AccoladeHarmless = string.Concat(new object[]
			{
				accoladeDestory2.m_name,
				": ",
				accoladeDestory2.m_totalDamageInflicted,
				" ",
				Localize.instance.Translate("$accolade_destroyer")
			});
		}
		TurnMan.PlayerTurnData accoladeAbsorbed = TurnMan.instance.GetAccoladeAbsorbed();
		if (accoladeAbsorbed.m_totalDamageAbsorbed == 0)
		{
			this.m_endGameData.m_AccoladeShields = Localize.instance.Translate("$label_notapplicable");
		}
		else
		{
			this.m_endGameData.m_AccoladeShields = string.Concat(new object[]
			{
				accoladeAbsorbed.m_name,
				": ",
				accoladeAbsorbed.m_totalDamageAbsorbed,
				" ",
				Localize.instance.Translate("$accolade_absorbed")
			});
		}
		DebugUtils.Assert(this.m_endGameData.m_localPlayer != null);
		if (this.m_state == ClientGame.State.Planning || this.m_state == ClientGame.State.Waiting || this.m_state == ClientGame.State.None)
		{
			this.ShowEndGameMessage();
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00031D14 File Offset: 0x0002FF14
	private void RPC_PlayerStatus(PTech.RPC rpc, List<object> args)
	{
		int num = 0;
		int num2 = (int)args[num++];
		this.m_players.Clear();
		for (int i = 0; i < num2; i++)
		{
			ClientPlayer clientPlayer = new ClientPlayer();
			clientPlayer.m_id = (int)args[num++];
			clientPlayer.m_surrender = (bool)args[num++];
			clientPlayer.m_left = (bool)args[num++];
			clientPlayer.m_status = (PlayerPresenceStatus)((int)args[num++]);
			clientPlayer.m_turnStatus = (PlayerTurnStatus)((int)args[num++]);
			this.m_players.Add(clientPlayer);
		}
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x00031DD4 File Offset: 0x0002FFD4
	private void RPC_Kicked(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("You have been kicked");
		this.m_onExit(ExitState.Kicked, 0);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x00031DF0 File Offset: 0x0002FFF0
	private void OnSurrender()
	{
		this.Surrender(this.m_gameSettings.m_localPlayerID);
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00031E04 File Offset: 0x00030004
	private void Surrender(int playerID)
	{
		if (this.m_state == ClientGame.State.Planning)
		{
			List<NetObj> all = NetObj.GetAll();
			foreach (NetObj netObj in all)
			{
				Ship ship = netObj as Ship;
				if (ship != null && ship.GetOwner() == playerID)
				{
					ship.SelfDestruct();
				}
			}
		}
		this.m_turnData.m_localSurrender = true;
		this.Commit();
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00031EA8 File Offset: 0x000300A8
	private void ShowEndGameMessage()
	{
		if (this.m_endGameMsg != null)
		{
			return;
		}
		bool flag = this.m_endGameData.m_outcome == GameOutcome.Victory || (this.m_endGameData.m_outcome == GameOutcome.GameOver && this.m_endGameData.m_winnerTeam == this.m_endGameData.m_localPlayer.m_team);
		bool flag2 = this.m_endGameData.m_outcome == GameOutcome.GameOver && this.m_endGameData.m_winnerTeam == -1;
		if (flag)
		{
			PLog.Log("Show winner splash");
			this.m_endGameMsg = GuiUtils.CreateGui("IngameGui/VictoryMessage", this.m_guiCamera);
			this.m_musMan.SetMusic("victory");
			VOSystem.instance.DoEvent("Match won");
		}
		else if (flag2)
		{
			PLog.Log("Show tie splash");
			this.m_endGameMsg = GuiUtils.CreateGui("IngameGui/DrawMessage", this.m_guiCamera);
			this.m_musMan.SetMusic("victory");
		}
		else
		{
			PLog.Log("Show looser splash");
			this.m_endGameMsg = GuiUtils.CreateGui("IngameGui/DefeatMessage", this.m_guiCamera);
			this.m_musMan.SetMusic("defeat");
			VOSystem.instance.DoEvent("Match lost");
		}
		GuiUtils.FindChildOf(this.m_endGameMsg, "ContinueButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnEndGameMsgContinue));
		this.m_endGameMsg.GetComponent<UIPanel>().BringIn();
		this.m_hud.SetVisible(false, true, true);
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00032038 File Offset: 0x00030238
	private void OnEndGameMsgContinue(IUIObject button)
	{
		UnityEngine.Object.Destroy(this.m_endGameMsg);
		this.m_endGameMsg = null;
		if (this.m_endGameData.m_outcome == GameOutcome.Victory && (this.m_gameSettings.m_gameType == GameType.Campaign || this.m_gameSettings.m_gameType == GameType.Challenge))
		{
			this.m_dialog.ForceEndDialog();
			this.m_dialog.m_onEndDialog = new Action(this.SetupEndgameMenu);
			this.m_turnMan.PlayBriefing("leveldata/campaign/briefings/" + this.m_gameSettings.m_level + "_debriefing");
		}
		else
		{
			this.SetupEndgameMenu();
		}
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x000320DC File Offset: 0x000302DC
	public GameType GetGameType()
	{
		return this.m_gameSettings.m_gameType;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x000320EC File Offset: 0x000302EC
	public bool IsReplayMode()
	{
		return this.m_replayMode;
	}

	// Token: 0x040004D3 RID: 1235
	public Action<ExitState, int> m_onExit;

	// Token: 0x040004D4 RID: 1236
	public Action m_onQuitGame;

	// Token: 0x040004D5 RID: 1237
	private GameObject m_guiCamera;

	// Token: 0x040004D6 RID: 1238
	private PTech.RPC m_rpc;

	// Token: 0x040004D7 RID: 1239
	private Hud m_hud;

	// Token: 0x040004D8 RID: 1240
	private Dialog m_dialog;

	// Token: 0x040004D9 RID: 1241
	private UserManClient m_userManClient;

	// Token: 0x040004DA RID: 1242
	private MapMan m_mapMan;

	// Token: 0x040004DB RID: 1243
	private HitText m_hitText;

	// Token: 0x040004DC RID: 1244
	private LosDrawer m_losDrawer;

	// Token: 0x040004DD RID: 1245
	private LobbyMenu m_lobbyMenu;

	// Token: 0x040004DE RID: 1246
	private EndGameMenu m_endGameMenu;

	// Token: 0x040004DF RID: 1247
	private LoadScreen m_loadScreen;

	// Token: 0x040004E0 RID: 1248
	private MusicManager m_musMan;

	// Token: 0x040004E1 RID: 1249
	private TurnMan m_turnMan;

	// Token: 0x040004E2 RID: 1250
	private ChatClient m_chatClient;

	// Token: 0x040004E3 RID: 1251
	private VOSystem m_voSystem;

	// Token: 0x040004E4 RID: 1252
	private GameState m_gameState;

	// Token: 0x040004E5 RID: 1253
	private GameSettings m_gameSettings;

	// Token: 0x040004E6 RID: 1254
	private ClientGame.InitializeData m_initializeData;

	// Token: 0x040004E7 RID: 1255
	private ClientGame.TurnData m_turnData;

	// Token: 0x040004E8 RID: 1256
	private ClientGame.SimulationData m_simData;

	// Token: 0x040004E9 RID: 1257
	private EndGameData m_endGameData;

	// Token: 0x040004EA RID: 1258
	private GameObject m_endGameMsg;

	// Token: 0x040004EB RID: 1259
	private GameObject m_countdownPrefab;

	// Token: 0x040004EC RID: 1260
	private GameObject m_autoCommitPrefab;

	// Token: 0x040004ED RID: 1261
	private bool m_firstLook = true;

	// Token: 0x040004EE RID: 1262
	private int m_focusCamera = -1;

	// Token: 0x040004EF RID: 1263
	private bool m_replayMode;

	// Token: 0x040004F0 RID: 1264
	private bool m_isOutcomeReplay;

	// Token: 0x040004F1 RID: 1265
	private bool m_onlineGame = true;

	// Token: 0x040004F2 RID: 1266
	private string m_hostName = string.Empty;

	// Token: 0x040004F3 RID: 1267
	private ClientGame.State m_state;

	// Token: 0x040004F4 RID: 1268
	private List<ClientPlayer> m_players = new List<ClientPlayer>();

	// Token: 0x040004F5 RID: 1269
	private static ClientGame m_instance;

	// Token: 0x020000AF RID: 175
	private class InitializeData
	{
		// Token: 0x040004F6 RID: 1270
		public List<PlayerInitData> m_players = new List<PlayerInitData>();

		// Token: 0x040004F7 RID: 1271
		public byte[] m_startState;

		// Token: 0x040004F8 RID: 1272
		public byte[] m_StartOrders;
	}

	// Token: 0x020000B0 RID: 176
	private class SimulationData
	{
		// Token: 0x040004F9 RID: 1273
		public int m_simulationTurn;

		// Token: 0x040004FA RID: 1274
		public int m_frames;

		// Token: 0x040004FB RID: 1275
		public byte[] m_startState;

		// Token: 0x040004FC RID: 1276
		public byte[] m_combinedStartOrders;

		// Token: 0x040004FD RID: 1277
		public List<byte[]> m_playerOrders;

		// Token: 0x040004FE RID: 1278
		public byte[] m_combinedOrders;

		// Token: 0x040004FF RID: 1279
		public int[] m_surrenders;
	}

	// Token: 0x020000B1 RID: 177
	private class TurnData
	{
		// Token: 0x04000500 RID: 1280
		public int m_turn = -1;

		// Token: 0x04000501 RID: 1281
		public bool m_needCommit;

		// Token: 0x04000502 RID: 1282
		public bool m_localSurrender;

		// Token: 0x04000503 RID: 1283
		public byte[] m_startState;

		// Token: 0x04000504 RID: 1284
		public byte[] m_orders;

		// Token: 0x04000505 RID: 1285
		public byte[] m_endState;

		// Token: 0x04000506 RID: 1286
		public byte[] m_endOrders;

		// Token: 0x04000507 RID: 1287
		public byte[] m_myOrders;

		// Token: 0x04000508 RID: 1288
		public int[] m_surrenders;

		// Token: 0x04000509 RID: 1289
		public bool m_waitingForLoad;

		// Token: 0x0400050A RID: 1290
		public int m_playbackFrames;

		// Token: 0x0400050B RID: 1291
		public int m_frames;

		// Token: 0x0400050C RID: 1292
		public TurnType m_turnType;

		// Token: 0x0400050D RID: 1293
		public byte[] m_tempOrders;

		// Token: 0x0400050E RID: 1294
		public float m_planningTime;

		// Token: 0x0400050F RID: 1295
		public double m_turnDuration;
	}

	// Token: 0x020000B2 RID: 178
	private enum State
	{
		// Token: 0x04000511 RID: 1297
		None,
		// Token: 0x04000512 RID: 1298
		Simulating,
		// Token: 0x04000513 RID: 1299
		Initializing,
		// Token: 0x04000514 RID: 1300
		Planning,
		// Token: 0x04000515 RID: 1301
		Outcome,
		// Token: 0x04000516 RID: 1302
		Test,
		// Token: 0x04000517 RID: 1303
		Waiting
	}
}
