using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PTech
{
	// Token: 0x02000140 RID: 320
	internal class Game
	{
		// Token: 0x06000C09 RID: 3081 RVA: 0x00056168 File Offset: 0x00054368
		public Game(string gameName, int gameID, int campaignID, GameType gameType, string campaign, string level, int players, FleetSizeClass fleetClass, float targetScore, double maxTurnTime, MapInfo mapinfo, PackMan packMan)
		{
			int gameID2;
			if (gameID <= 0)
			{
				Game.m_nextGameID = (gameID2 = Game.m_nextGameID) + 1;
			}
			else
			{
				gameID2 = gameID;
			}
			this.m_gameID = gameID2;
			this.m_campaignID = campaignID;
			this.m_gameName = gameName;
			this.m_campaignName = campaign;
			this.m_levelName = level;
			this.m_gameType = gameType;
			this.m_nrOfPlayers = players;
			this.m_mapInfo = mapinfo;
			this.m_fleetSizeClass = fleetClass;
			this.m_fleetSizeLimits = FleetSizes.sizes[(int)fleetClass];
			this.m_targetScore = targetScore;
			this.m_packMan = packMan;
			this.m_createDate = DateTime.Now;
			this.m_maxTurnTime = maxTurnTime;
			if (gameType == GameType.Campaign || gameType == GameType.Challenge)
			{
				if (this.m_mapInfo.m_fleetLimit != null)
				{
					this.m_fleetSizeClass = FleetSizeClass.Custom;
					this.m_fleetSizeLimits = new FleetSize(this.m_mapInfo.m_fleetLimit.min, this.m_mapInfo.m_fleetLimit.max / players);
				}
				if (this.m_campaignID <= 0)
				{
					this.m_isNewCampaign = true;
					this.m_campaignID = Game.m_nextCampaignID++;
				}
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x000562F4 File Offset: 0x000544F4
		public void SetupReplayMode()
		{
			this.m_replayMode = true;
			this.m_currentTurn = 0;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00056304 File Offset: 0x00054504
		public void Close()
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				User user = gamePlayer.GetUser();
				if (user != null)
				{
					user.m_inGames--;
				}
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x00056380 File Offset: 0x00054580
		public static void SetNextGameID(int gameID)
		{
			Game.m_nextGameID = gameID;
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x00056388 File Offset: 0x00054588
		public static int GetNextGameID()
		{
			return Game.m_nextGameID;
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x00056390 File Offset: 0x00054590
		public static void SetNextCampaignID(int campaignID)
		{
			Game.m_nextCampaignID = campaignID;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00056398 File Offset: 0x00054598
		public static int GetNextCampaignID()
		{
			return Game.m_nextCampaignID;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x000563A0 File Offset: 0x000545A0
		public bool MayJoin(User user)
		{
			return this.GetGamePlayerByUser(user) != null || this.m_players.Count < this.m_nrOfPlayers;
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x000563DC File Offset: 0x000545DC
		public List<User> GetNextGameUserList()
		{
			List<User> list = new List<User>();
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (!gamePlayer.m_leftGame)
				{
					list.Add(gamePlayer.GetUser());
				}
			}
			return list;
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x0005645C File Offset: 0x0005465C
		public List<KeyValuePair<User, bool>> GetActiveUserList()
		{
			List<KeyValuePair<User, bool>> list = new List<KeyValuePair<User, bool>>();
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (!gamePlayer.m_leftGame && !gamePlayer.m_seenEndGame)
				{
					bool value = this.NeedPlayerAttention(gamePlayer);
					list.Add(new KeyValuePair<User, bool>(gamePlayer.GetUser(), value));
				}
			}
			return list;
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x000564F4 File Offset: 0x000546F4
		public int GetOnlinePlayers()
		{
			int num = 0;
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x00056568 File Offset: 0x00054768
		public List<string> GetUserNames()
		{
			List<string> list = new List<string>();
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				list.Add(gamePlayer.m_userName);
			}
			return list;
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x000565DC File Offset: 0x000547DC
		public void InternalSetUser(string name, User user)
		{
			GamePlayer gamePlayerByName = this.GetGamePlayerByName(name);
			if (gamePlayerByName != null)
			{
				gamePlayerByName.SetUser(user);
			}
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x00056600 File Offset: 0x00054800
		public bool AddUserToGame(User user, bool admin)
		{
			if (!this.MayJoin(user))
			{
				return false;
			}
			if (this.GetGamePlayerByUser(user) == null)
			{
				this.AddNewPlayer(user, admin);
				if ((this.m_gameType == GameType.Assassination || this.m_gameType == GameType.Points) && this.m_nrOfPlayers == 3)
				{
					this.UnreadyAllPlayers();
				}
				this.SendLobbyToAll();
			}
			return true;
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x00056664 File Offset: 0x00054864
		public bool JoinGame(User user)
		{
			GamePlayer gamePlayerByUser = this.GetGamePlayerByUser(user);
			if (gamePlayerByUser == null)
			{
				return false;
			}
			gamePlayerByUser.m_inGame = true;
			user.m_rpc.Register("Commit", new RPC.Handler(this.RPC_Commit));
			user.m_rpc.Register("LeaveGame", new RPC.Handler(this.RPC_LeaveGame));
			user.m_rpc.Register("KickSelf", new RPC.Handler(this.RPC_KickSelf));
			user.m_rpc.Register("SetFleet", new RPC.Handler(this.RPC_SetFleet));
			user.m_rpc.Register("FleetUpdated", new RPC.Handler(this.RPC_FleetUpdated));
			user.m_rpc.Register("SwitchTeam", new RPC.Handler(this.RPC_SwitchTeam));
			user.m_rpc.Register("ReadyToStart", new RPC.Handler(this.RPC_ReadyToStart));
			user.m_rpc.Register("SaveReplay", new RPC.Handler(this.RPC_SaveReplay));
			user.m_rpc.Register("SeenEndGame", new RPC.Handler(this.RPC_SeenEndGame));
			user.m_rpc.Register("SimulationResults", new RPC.Handler(this.RPC_SimulationResults));
			if (gamePlayerByUser.m_admin)
			{
				user.m_rpc.Register("KickPlayer", new RPC.Handler(this.RPC_KickPlayer));
				user.m_rpc.Register("DisbandGame", new RPC.Handler(this.RPC_DisbandGame));
				user.m_rpc.Register("Invite", new RPC.Handler(this.RPC_Invite));
				user.m_rpc.Register("RenameGame", new RPC.Handler(this.RPC_RenameGame));
				user.m_rpc.Register("AllowMatchmaking", new RPC.Handler(this.RPC_AllowMatchmaking));
			}
			if (this.m_replayMode)
			{
				user.m_rpc.Register("RequestReplayTurn", new RPC.Handler(this.RPC_RequestReplayTurn));
			}
			this.m_chatServer.Register(gamePlayerByUser, true);
			this.SendGameSettings(gamePlayerByUser);
			this.SendPlayerStatusToAll();
			if (!this.m_gameStarted)
			{
				this.SendLobbyToAll();
			}
			else
			{
				this.SendGameStarted(gamePlayerByUser);
				TurnData currentTurnData = this.GetCurrentTurnData();
				if (currentTurnData != null)
				{
					if (this.AllCommited(currentTurnData) && !this.m_replayMode)
					{
						this.RequestSimulation(gamePlayerByUser, currentTurnData);
					}
					else
					{
						this.SendCurrentTurn(gamePlayerByUser, true);
					}
				}
			}
			return true;
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x000568D0 File Offset: 0x00054AD0
		private int GetUnusedPlayerID()
		{
			int num = 0;
			while (this.GetGamePlayerByID(num) != null)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x000568F8 File Offset: 0x00054AF8
		public bool IsAdmin(User user)
		{
			GamePlayer gamePlayerByUser = this.GetGamePlayerByUser(user);
			return gamePlayerByUser != null && gamePlayerByUser.m_admin;
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x0005691C File Offset: 0x00054B1C
		private GamePlayer AddNewPlayer(User user, bool admin)
		{
			int unusedPlayerID = this.GetUnusedPlayerID();
			GamePlayer gamePlayer = new GamePlayer(unusedPlayerID);
			gamePlayer.SetUser(user);
			this.m_players.Add(gamePlayer);
			gamePlayer.m_admin = admin;
			if (this.m_campaignID > 0)
			{
				this.AddCampaignContent(gamePlayer);
				if (!user.HaveCampaignFleet(this.m_campaignID))
				{
					this.CreateNewCampaignFleet(gamePlayer);
				}
			}
			if (this.m_gameType == GameType.Points || this.m_gameType == GameType.Assassination)
			{
				if (this.m_nrOfPlayers == 2)
				{
					if (gamePlayer.m_id == 0)
					{
						gamePlayer.m_team = 0;
					}
					else
					{
						gamePlayer.m_team = 1;
					}
				}
				if (this.m_nrOfPlayers == 4)
				{
					if (gamePlayer.m_id <= 1)
					{
						gamePlayer.m_team = 0;
					}
					else
					{
						gamePlayer.m_team = 1;
					}
				}
			}
			return gamePlayer;
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x000569EC File Offset: 0x00054BEC
		private void AddCampaignContent(GamePlayer p)
		{
			string name = (!(this.m_mapInfo.m_contentPack == string.Empty)) ? this.m_mapInfo.m_contentPack : "Base";
			ContentPack pack = this.m_packMan.GetPack(name);
			if (pack == null)
			{
				PLog.LogError("Failed to find campaign content pack " + this.m_mapInfo.m_contentPack);
				return;
			}
			p.GetUser().SetCampaignContentPack(this.m_campaignID, pack);
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x00056A6C File Offset: 0x00054C6C
		private void CreateNewCampaignFleet(GamePlayer player)
		{
			if (this.m_mapInfo.m_defaults != string.Empty)
			{
				XmlDocument xmlDocument = Utils.LoadXml("shared_settings/campaign_defs/" + this.m_mapInfo.m_defaults);
				if (xmlDocument == null)
				{
					PLog.LogError("Failed to load shipfleet file " + this.m_mapInfo.m_defaults);
					return;
				}
				List<FleetDef> list;
				List<ShipDef> list2;
				FleetDefUtils.LoadFleetsAndShipsXMLFile(xmlDocument, out list, out list2, ComponentDB.instance);
				foreach (ShipDef shipDef in list2)
				{
					shipDef.m_campaignID = this.m_campaignID;
					player.GetUser().AddShipDef(shipDef);
				}
				if (list.Count > 0)
				{
					FleetDef fleetDef;
					if (list.Count < this.m_nrOfPlayers)
					{
						PLog.LogError(string.Concat(new object[]
						{
							"Missing fleet for ",
							this.m_nrOfPlayers,
							" players in map ",
							this.m_levelName,
							" using fleet 0"
						}));
						fleetDef = list[0];
					}
					else
					{
						fleetDef = list[this.m_nrOfPlayers - 1];
					}
					fleetDef.m_campaignID = this.m_campaignID;
					player.GetUser().AddFleetDef(fleetDef);
					this.SetPlayerFleet(player.GetUser().m_name, fleetDef.m_name);
				}
			}
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x00056BF8 File Offset: 0x00054DF8
		public void Update(float dt)
		{
			if (!this.m_gameStarted && this.IsReadyToStart())
			{
				this.StartGame();
			}
			if (this.m_gameStarted && !this.m_gotInitState && this.m_sentInitiateRequestTo == null)
			{
				this.SendInitRequest();
			}
			this.UpdateTimeSync(dt);
			this.UpdateAutoCommit();
			if (this.GetConnectedPlayers() == 0)
			{
				this.m_timeSinceActive += dt;
			}
			else
			{
				this.m_timeSinceActive = 0f;
			}
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x00056C80 File Offset: 0x00054E80
		private void StartGame()
		{
			this.m_gameStarted = true;
			this.SendGameStartedToAll();
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					this.m_chatServer.Unregister(gamePlayer);
					this.m_chatServer.Register(gamePlayer, false);
				}
			}
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x00056D10 File Offset: 0x00054F10
		private void UpdateModuleAndshipUsageStats(GamePlayer player)
		{
			UserStats stats = player.GetUser().m_stats;
			Dictionary<string, int> moduleUsage = FleetDefUtils.GetModuleUsage(player.m_fleet);
			foreach (KeyValuePair<string, int> keyValuePair in moduleUsage)
			{
				stats.AddModuleUsage(keyValuePair.Key, keyValuePair.Value);
			}
			Dictionary<string, int> shipUsage = FleetDefUtils.GetShipUsage(player.m_fleet);
			foreach (KeyValuePair<string, int> keyValuePair2 in shipUsage)
			{
				stats.AddShipUsage(keyValuePair2.Key, keyValuePair2.Value);
			}
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00056E04 File Offset: 0x00055004
		private void SendEndGameToAll()
		{
			foreach (GamePlayer p in this.m_players)
			{
				this.SendEndGame(p);
			}
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00056E6C File Offset: 0x0005506C
		private void SendEndGame(GamePlayer p)
		{
			if (!p.m_inGame)
			{
				return;
			}
			List<object> list = new List<object>();
			list.Add(p.m_id);
			list.Add((int)this.m_gameOutcome);
			list.Add(this.m_winnerTeam);
			list.Add(this.m_autoJoinNextGameID);
			list.Add(this.m_currentTurn);
			list.Add(this.m_players.Count);
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				list.Add(gamePlayer.m_id);
				list.Add(gamePlayer.m_team);
				list.Add(gamePlayer.GetUser().m_name);
				list.Add(gamePlayer.GetUser().m_flag);
				list.Add(gamePlayer.m_place);
				list.Add(gamePlayer.m_score);
				list.Add(gamePlayer.m_teamScore);
				list.Add(gamePlayer.m_flagshipKiller[0]);
				list.Add(gamePlayer.m_flagshipKiller[1]);
			}
			p.GetUser().m_rpc.Invoke("EndGame", list);
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x00057000 File Offset: 0x00055200
		private void SendLobbyToAll()
		{
			if (this.m_gameStarted)
			{
				return;
			}
			foreach (GamePlayer p in this.m_players)
			{
				this.SendLobby(p);
			}
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x00057074 File Offset: 0x00055274
		private void SendLobby(GamePlayer p)
		{
			if (!p.m_inGame)
			{
				return;
			}
			List<object> list = new List<object>();
			list.Add(p.GetUser().m_name);
			list.Add(this.m_publicGame);
			list.Add(this.m_mapInfo.m_noFleet);
			list.Add(this.m_players.Count);
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				list.Add(gamePlayer.m_id);
				list.Add(gamePlayer.m_team);
				list.Add(gamePlayer.GetUser().m_name);
				list.Add(gamePlayer.GetUser().m_flag);
				list.Add(gamePlayer.m_admin);
				list.Add(gamePlayer.m_readyToStart);
				list.Add((int)gamePlayer.GetPlayerPresenceStatus());
				if (!this.m_mapInfo.m_noFleet)
				{
					if (gamePlayer.m_fleet != null)
					{
						list.Add(gamePlayer.m_fleet.m_name);
						list.Add(gamePlayer.m_fleet.m_value);
					}
					else
					{
						int num = -1;
						if (gamePlayer.m_selectedFleetName != string.Empty)
						{
							FleetDef fleetDef = gamePlayer.GetUser().GetFleetDef(gamePlayer.m_selectedFleetName, this.m_campaignID);
							if (fleetDef != null)
							{
								num = fleetDef.m_value;
							}
						}
						if (num == -1)
						{
							gamePlayer.m_selectedFleetName = string.Empty;
						}
						list.Add(gamePlayer.m_selectedFleetName);
						list.Add(num);
					}
				}
				else
				{
					list.Add(string.Empty);
					list.Add(-1);
				}
			}
			p.GetUser().m_rpc.Invoke("Lobby", list);
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00057294 File Offset: 0x00055494
		private void SendGameSettingsToAll()
		{
			foreach (GamePlayer p in this.m_players)
			{
				this.SendGameSettings(p);
			}
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x000572FC File Offset: 0x000554FC
		private void SendGameSettings(GamePlayer p)
		{
			if (!p.m_inGame)
			{
				return;
			}
			List<object> list = new List<object>();
			list.Add(p.m_id);
			list.Add(p.m_admin);
			list.Add(this.m_campaignID);
			list.Add(this.m_gameID);
			list.Add(this.m_gameName);
			list.Add((int)this.m_gameType);
			list.Add(this.m_campaignName);
			list.Add(this.m_levelName);
			list.Add((int)this.m_fleetSizeClass);
			list.Add(this.m_fleetSizeLimits.min);
			list.Add(this.m_fleetSizeLimits.max);
			list.Add(this.m_targetScore);
			list.Add(this.m_maxTurnTime);
			list.Add(this.m_nrOfPlayers);
			p.GetUser().m_rpc.Invoke("GameSettings", list);
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0005741C File Offset: 0x0005561C
		private void SendGameStartedToAll()
		{
			foreach (GamePlayer p in this.m_players)
			{
				this.SendGameStarted(p);
			}
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x00057484 File Offset: 0x00055684
		private void SendGameStarted(GamePlayer p)
		{
			if (p.m_inGame)
			{
				p.GetUser().m_rpc.Invoke("GameStarted", new object[0]);
			}
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x000574B8 File Offset: 0x000556B8
		public void SetAutoJoinNextGameID(int gameID)
		{
			this.m_autoJoinNextGameID = gameID;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x000574C4 File Offset: 0x000556C4
		public int GetNrOfPlayers()
		{
			return this.m_players.Count;
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x000574D4 File Offset: 0x000556D4
		public int GetMaxPlayers()
		{
			return this.m_nrOfPlayers;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x000574DC File Offset: 0x000556DC
		public string GetName()
		{
			return this.m_gameName;
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x000574E4 File Offset: 0x000556E4
		public int GetGameID()
		{
			return this.m_gameID;
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x000574EC File Offset: 0x000556EC
		public int GetCampaignID()
		{
			return this.m_campaignID;
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x000574F4 File Offset: 0x000556F4
		public string GetLevelName()
		{
			return this.m_levelName;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x000574FC File Offset: 0x000556FC
		public string GetCampaign()
		{
			return this.m_campaignName;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00057504 File Offset: 0x00055704
		public GameType GetGameType()
		{
			return this.m_gameType;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x0005750C File Offset: 0x0005570C
		public float GetTargetScore()
		{
			return this.m_targetScore;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x00057514 File Offset: 0x00055714
		public FleetSizeClass GetFleetSizeClass()
		{
			return this.m_fleetSizeClass;
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x0005751C File Offset: 0x0005571C
		public FleetSize GetFleetSizeLimits()
		{
			return this.m_fleetSizeLimits;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00057524 File Offset: 0x00055724
		public int GetTurn()
		{
			return this.m_currentTurn;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x0005752C File Offset: 0x0005572C
		public bool IsPublicGame()
		{
			return this.m_publicGame;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x00057534 File Offset: 0x00055734
		public void SetPublicGame(bool enabled)
		{
			if (this.m_publicGame == enabled)
			{
				return;
			}
			this.m_publicGame = enabled;
			this.SendLobbyToAll();
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x00057550 File Offset: 0x00055750
		public bool GameIsVisibleForPlayer(User user)
		{
			GamePlayer gamePlayerByUser = this.GetGamePlayerByUser(user);
			return gamePlayerByUser != null && !gamePlayerByUser.m_leftGame && !gamePlayerByUser.m_seenEndGame;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x00057584 File Offset: 0x00055784
		public int GetConnectedPlayers()
		{
			int num = 0;
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x000575F8 File Offset: 0x000557F8
		public bool IsUserInGame(User user)
		{
			return this.GetGamePlayerByUser(user) != null;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x00057608 File Offset: 0x00055808
		public float GetTimeSinceActive()
		{
			return this.m_timeSinceActive;
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x00057610 File Offset: 0x00055810
		private TurnData GetCurrentTurnData()
		{
			if (this.m_currentTurn >= 0)
			{
				return this.m_turns[this.m_currentTurn];
			}
			return null;
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x00057634 File Offset: 0x00055834
		private void RPC_SwitchTeam(RPC rpc, List<object> args)
		{
			if (this.m_gameStarted)
			{
				return;
			}
			if (this.m_gameType != GameType.Points && this.m_gameType != GameType.Assassination)
			{
				return;
			}
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null)
			{
				return;
			}
			gamePlayerByRPC.m_team = ((gamePlayerByRPC.m_team != 0) ? 0 : 1);
			gamePlayerByRPC.m_readyToStart = false;
			if ((this.m_gameType == GameType.Assassination || this.m_gameType == GameType.Points) && this.m_nrOfPlayers == 3)
			{
				this.UnreadyAllPlayers();
			}
			this.SendLobbyToAll();
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000576C4 File Offset: 0x000558C4
		private void UnreadyAllPlayers()
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				gamePlayer.m_readyToStart = false;
			}
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x0005772C File Offset: 0x0005592C
		private void RPC_FleetUpdated(RPC rpc, List<object> args)
		{
			if (this.m_gameStarted)
			{
				return;
			}
			if (this.GetGamePlayerByRPC(rpc) == null)
			{
				return;
			}
			this.SendLobbyToAll();
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x0005775C File Offset: 0x0005595C
		private void RPC_SetFleet(RPC rpc, List<object> args)
		{
			if (this.m_gameStarted)
			{
				return;
			}
			if (this.m_gameType == GameType.Campaign || this.m_gameType == GameType.Challenge)
			{
				return;
			}
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null)
			{
				return;
			}
			string fleetName = args[0] as string;
			this.SetPlayerFleet(gamePlayerByRPC.GetUser().m_name, fleetName);
			this.SendLobbyToAll();
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x000577C4 File Offset: 0x000559C4
		public void SetPlayerFleet(string player, string fleetName)
		{
			GamePlayer gamePlayerByName = this.GetGamePlayerByName(player);
			if (gamePlayerByName == null)
			{
				return;
			}
			if (gamePlayerByName.GetUser().GetFleetDef(fleetName, this.m_campaignID) == null)
			{
				PLog.LogError("failed to find player " + gamePlayerByName.GetUser().m_name + " fleet " + fleetName);
				return;
			}
			gamePlayerByName.m_selectedFleetName = fleetName;
			gamePlayerByName.m_fleet = null;
			gamePlayerByName.m_readyToStart = false;
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x00057830 File Offset: 0x00055A30
		public string GetPlayerFleet(string player)
		{
			GamePlayer gamePlayerByName = this.GetGamePlayerByName(player);
			if (gamePlayerByName == null)
			{
				return null;
			}
			return gamePlayerByName.m_selectedFleetName;
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x00057854 File Offset: 0x00055A54
		private void RPC_SeenEndGame(RPC rpc, List<object> args)
		{
			if (!this.m_gameEnded)
			{
				return;
			}
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null)
			{
				return;
			}
			gamePlayerByRPC.m_seenEndGame = true;
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x00057884 File Offset: 0x00055A84
		private void RPC_ReadyToStart(RPC rpc, List<object> args)
		{
			if (this.m_gameStarted)
			{
				return;
			}
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null)
			{
				return;
			}
			if (gamePlayerByRPC.m_readyToStart)
			{
				gamePlayerByRPC.m_readyToStart = false;
				gamePlayerByRPC.m_fleet = null;
			}
			else
			{
				if (!this.m_mapInfo.m_noFleet && gamePlayerByRPC.m_selectedFleetName == string.Empty)
				{
					return;
				}
				if (!this.m_mapInfo.m_noFleet)
				{
					FleetDef fleetDef = gamePlayerByRPC.GetUser().GetFleetDef(gamePlayerByRPC.m_selectedFleetName, this.m_campaignID);
					bool dubble = false;
					if ((this.m_gameType == GameType.Assassination || this.m_gameType == GameType.Points) && this.m_nrOfPlayers == 3 && this.GetPlayersInTeam(gamePlayerByRPC.m_team) == 1)
					{
						dubble = true;
					}
					if (!this.m_fleetSizeLimits.ValidSize(fleetDef.m_value, dubble))
					{
						return;
					}
					gamePlayerByRPC.m_fleet = fleetDef.Clone();
				}
				gamePlayerByRPC.m_readyToStart = true;
			}
			this.SendLobbyToAll();
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x00057984 File Offset: 0x00055B84
		private void RPC_KickSelf(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null || gamePlayerByRPC.m_admin)
			{
				return;
			}
			if (this.m_gameStarted)
			{
				if (gamePlayerByRPC.m_surrender)
				{
					gamePlayerByRPC.m_leftGame = true;
				}
				return;
			}
			string userName = gamePlayerByRPC.m_userName;
			this.KickUser(userName);
			this.SendLobbyToAll();
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x000579E0 File Offset: 0x00055BE0
		private void RPC_KickPlayer(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null || !gamePlayerByRPC.m_admin)
			{
				return;
			}
			if (this.m_gameStarted)
			{
				return;
			}
			string name = (string)args[0];
			this.KickUser(name);
			this.SendLobbyToAll();
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x00057A30 File Offset: 0x00055C30
		private void RPC_DisbandGame(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null || !gamePlayerByRPC.m_admin)
			{
				return;
			}
			this.m_disbanded = true;
			while (this.m_players.Count > 0)
			{
				this.KickUser(this.m_players[0].GetUser().m_name);
			}
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x00057A90 File Offset: 0x00055C90
		private void RPC_AllowMatchmaking(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null || !gamePlayerByRPC.m_admin)
			{
				return;
			}
			if (this.m_gameStarted)
			{
				return;
			}
			bool publicGame = (bool)args[0];
			this.SetPublicGame(publicGame);
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x00057AD8 File Offset: 0x00055CD8
		private void RPC_RenameGame(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null || !gamePlayerByRPC.m_admin)
			{
				return;
			}
			this.m_gameName = (string)args[0];
			this.SendGameSettingsToAll();
			this.SendLobbyToAll();
		}

		// Token: 0x06000C4A RID: 3146 RVA: 0x00057B20 File Offset: 0x00055D20
		private void RPC_Invite(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			int arg = (int)args[0];
			if (this.m_onInviteFriend != null)
			{
				this.m_onInviteFriend(this, gamePlayerByRPC.GetUser(), arg);
			}
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x00057B60 File Offset: 0x00055D60
		private bool IsReadyToStart()
		{
			if (this.m_players.Count < this.m_nrOfPlayers)
			{
				return false;
			}
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (!gamePlayer.m_readyToStart)
				{
					return false;
				}
				if (!this.m_mapInfo.m_noFleet && gamePlayer.m_selectedFleetName == string.Empty)
				{
					return false;
				}
			}
			if (this.m_gameType != GameType.Points && this.m_gameType != GameType.Assassination)
			{
				return true;
			}
			if (this.m_nrOfPlayers != 2 && this.m_nrOfPlayers != 4 && this.m_nrOfPlayers != 3)
			{
				return false;
			}
			int playersInTeam = this.GetPlayersInTeam(0);
			int playersInTeam2 = this.GetPlayersInTeam(1);
			if (this.m_nrOfPlayers == 2)
			{
				return playersInTeam == 1 && playersInTeam2 == 1;
			}
			if (this.m_nrOfPlayers == 3)
			{
				return (playersInTeam == 2 && playersInTeam2 == 1) || (playersInTeam == 1 && playersInTeam2 == 2);
			}
			return this.m_nrOfPlayers == 4 && playersInTeam == 2 && playersInTeam2 == 2;
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00057CD0 File Offset: 0x00055ED0
		private int GetPlayersInTeam(int team)
		{
			int num = 0;
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_team == team)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x00057D44 File Offset: 0x00055F44
		private void KickUser(string name)
		{
			GamePlayer gamePlayerByName = this.GetGamePlayerByName(name);
			if (gamePlayerByName == null)
			{
				return;
			}
			if (gamePlayerByName.m_inGame)
			{
				this.OnUserDisconnected(gamePlayerByName.GetUser());
				gamePlayerByName.GetUser().m_rpc.Invoke("Kicked", new object[0]);
			}
			this.m_players.Remove(gamePlayerByName);
			gamePlayerByName.GetUser().m_inGames--;
			if (this.m_campaignID > 0)
			{
				gamePlayerByName.GetUser().ClearCampaign(this.m_campaignID);
			}
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x00057DD0 File Offset: 0x00055FD0
		private void RPC_LeaveGame(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (gamePlayerByRPC == null)
			{
				return;
			}
			float num = (float)args[0];
			this.AddUserPlanningTime(gamePlayerByRPC, (double)num);
			this.OnUserDisconnected(gamePlayerByRPC.GetUser());
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x00057E10 File Offset: 0x00056010
		private void AddUserPlanningTime(GamePlayer player, double time)
		{
			player.GetUser().m_stats.m_totalPlanningTime += (long)time;
		}

		// Token: 0x06000C50 RID: 3152 RVA: 0x00057E2C File Offset: 0x0005602C
		public void OnUserConnectedToServer(User user)
		{
			if (this.GetGamePlayerByUser(user) == null)
			{
				return;
			}
			this.SendPlayerStatusToAll();
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x00057E50 File Offset: 0x00056050
		public void OnUserDisconnected(User user)
		{
			GamePlayer gamePlayerByUser = this.GetGamePlayerByUser(user);
			if (gamePlayerByUser == null)
			{
				return;
			}
			if (gamePlayerByUser.m_inGame)
			{
				this.m_chatServer.Unregister(gamePlayerByUser);
				gamePlayerByUser.m_inGame = false;
				if (user.m_rpc != null)
				{
					user.m_rpc.Unregister("Commit");
					user.m_rpc.Unregister("LeaveGame");
					user.m_rpc.Unregister("KickSelf");
					user.m_rpc.Unregister("InitialState");
					user.m_rpc.Unregister("SimulationResults");
					user.m_rpc.Unregister("Chat");
					user.m_rpc.Unregister("SetFleet");
					user.m_rpc.Unregister("KickPlayer");
					user.m_rpc.Unregister("SeenEndGame");
					user.m_rpc.Unregister("RequestReplayTurn");
					user.m_rpc.Unregister("RenameGame");
					user.m_rpc.Unregister("AllowMatchmaking");
				}
				if (this.m_sentInitiateRequestTo == gamePlayerByUser)
				{
					PLog.LogWarning("Player " + gamePlayerByUser.m_userName + " disconnected, reseting init request from that player");
					this.m_sentInitiateRequestTo = null;
				}
			}
			this.SendPlayerStatusToAll();
			this.SendLobbyToAll();
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x00057F90 File Offset: 0x00056190
		public bool HasStarted()
		{
			return this.m_gameStarted;
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x00057F98 File Offset: 0x00056198
		public double GetMaxTurnTime()
		{
			return this.m_maxTurnTime;
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x00057FA0 File Offset: 0x000561A0
		public bool IsFinished()
		{
			if (this.m_players.Count == 0)
			{
				return true;
			}
			if (this.m_disbanded)
			{
				return true;
			}
			if (!this.m_gameEnded)
			{
				return false;
			}
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					return false;
				}
				if (!gamePlayer.m_leftGame && !gamePlayer.m_seenEndGame)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x00058060 File Offset: 0x00056260
		public GameOutcome GetOutcome()
		{
			return this.m_gameOutcome;
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x00058068 File Offset: 0x00056268
		private void SendCurrentTurn(GamePlayer player, bool startReplay)
		{
			TurnData currentTurnData = this.GetCurrentTurnData();
			bool flag = currentTurnData.Commited(player.m_id);
			bool flag2 = !flag && !player.m_dead && !this.m_gameEnded;
			byte[] item = (!flag) ? new byte[0] : currentTurnData.m_newOrders[player.m_id];
			List<object> list = new List<object>();
			list.Add(this.m_currentTurn);
			list.Add((int)currentTurnData.GetTurnType());
			list.Add(flag2);
			list.Add(this.GetCurrentTurnDuration());
			list.Add(item);
			list.Add(currentTurnData.m_startState);
			list.Add(currentTurnData.m_startOrders);
			list.Add(currentTurnData.m_endState);
			list.Add(currentTurnData.m_endOrders);
			list.Add(currentTurnData.GetPlaybackFrames());
			list.Add(currentTurnData.GetFrames());
			list.Add(currentTurnData.m_startSurrenders);
			list.Add(startReplay);
			player.GetUser().m_rpc.Invoke("TurnData", list);
			if (this.m_gameEnded && !this.m_replayMode)
			{
				this.SendEndGame(player);
			}
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x000581C4 File Offset: 0x000563C4
		private void SendCurrentTurnToAll(bool includeReplay)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					this.SendCurrentTurn(gamePlayer, includeReplay);
				}
			}
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00058238 File Offset: 0x00056438
		private bool SendInitRequest()
		{
			GamePlayer firstConnectedPlayer = this.GetFirstConnectedPlayer();
			if (firstConnectedPlayer != null)
			{
				this.RequestInitialState(firstConnectedPlayer);
				return true;
			}
			return false;
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x0005825C File Offset: 0x0005645C
		private void RequestInitialState(GamePlayer player)
		{
			List<object> list = new List<object>();
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (!this.m_mapInfo.m_noFleet)
				{
					list.Add(true);
					list.Add(gamePlayer.m_fleet.ToArray());
				}
				else
				{
					list.Add(false);
				}
				list.Add(gamePlayer.m_id);
				list.Add(gamePlayer.m_userName);
				list.Add(gamePlayer.m_team);
				list.Add(gamePlayer.GetUser().m_flag);
			}
			player.GetUser().m_rpc.Invoke("DoInitiation", list);
			player.GetUser().m_rpc.Register("InitialState", new RPC.Handler(this.RPC_InitialState));
			this.m_sentInitiateRequestTo = player;
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00058384 File Offset: 0x00056584
		private void RPC_InitialState(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (this.m_sentInitiateRequestTo != gamePlayerByRPC)
			{
				return;
			}
			byte[] startState = (byte[])args[0];
			byte[] startOrders = (byte[])args[1];
			byte[] endState = (byte[])args[2];
			byte[] endOrders = (byte[])args[3];
			int playbackFrames = (int)args[4];
			int[] startSurrenders = new int[0];
			this.SetupNewTurn(startState, startOrders, startSurrenders, endState, endOrders, TurnType.Normal, playbackFrames, 300);
			rpc.Unregister("InitialState");
			this.m_gotInitState = true;
			this.m_sentInitiateRequestTo = null;
			this.SendCurrentTurnToAll(true);
			this.SendNewTurnNotifications();
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0005842C File Offset: 0x0005662C
		private void SetupNewTurn(byte[] startState, byte[] startOrders, int[] startSurrenders, byte[] endState, byte[] endOrders, TurnType type, int playbackFrames, int frames)
		{
			this.m_currentTurn = this.m_turns.Count;
			TurnData turnData = new TurnData(this.m_currentTurn, this.m_nrOfPlayers, playbackFrames, frames, type);
			turnData.m_startState = startState;
			turnData.m_startOrders = startOrders;
			turnData.m_startSurrenders = startSurrenders;
			turnData.m_endState = endState;
			turnData.m_endOrders = endOrders;
			this.m_turns.Add(turnData);
			this.m_turnStartDate = DateTime.Now;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x000584A0 File Offset: 0x000566A0
		private double GetCurrentTurnDuration()
		{
			return DateTime.Now.Subtract(this.m_turnStartDate).TotalSeconds;
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x000584C8 File Offset: 0x000566C8
		private void UpdateTimeSync(float dt)
		{
			if (this.m_maxTurnTime <= 0.0)
			{
				return;
			}
			this.m_timeSyncTimer += dt;
			if (this.m_timeSyncTimer >= this.m_timeSyncSendDelay)
			{
				this.m_timeSyncTimer = 0f;
				if (this.m_gameEnded || !this.m_gameStarted || !this.m_gotInitState)
				{
					return;
				}
				if (this.GetCurrentTurnData() == null)
				{
					return;
				}
				double currentTurnDuration = this.GetCurrentTurnDuration();
				foreach (GamePlayer gamePlayer in this.m_players)
				{
					if (gamePlayer.m_inGame)
					{
						gamePlayer.GetUser().m_rpc.Invoke("TimeSync", new object[]
						{
							currentTurnDuration
						});
					}
				}
			}
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x000585CC File Offset: 0x000567CC
		private void UpdateAutoCommit()
		{
			if (this.m_maxTurnTime <= 0.0)
			{
				return;
			}
			if (this.m_gameEnded || !this.m_gameStarted || !this.m_gotInitState)
			{
				return;
			}
			TurnData currentTurnData = this.GetCurrentTurnData();
			if (currentTurnData == null)
			{
				return;
			}
			if (this.AllCommited(currentTurnData))
			{
				return;
			}
			double currentTurnDuration = this.GetCurrentTurnDuration();
			if (currentTurnDuration > this.m_maxTurnTime)
			{
				foreach (GamePlayer gamePlayer in this.m_players)
				{
					if (!currentTurnData.Commited(gamePlayer.m_id))
					{
						if (!gamePlayer.m_inGame || currentTurnDuration > this.m_maxTurnTime + (double)this.m_autoCommitTimeout)
						{
							byte[] orders = new byte[0];
							bool flag = this.Commit(gamePlayer, this.m_currentTurn, false, this.m_maxTurnTime, orders);
							if (flag)
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x000586F4 File Offset: 0x000568F4
		private void RPC_Commit(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			int turn = (int)args[0];
			bool surrender = (bool)args[1];
			float num = (float)args[2];
			byte[] orders = (byte[])args[3];
			this.Commit(gamePlayerByRPC, turn, surrender, (double)num, orders);
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x0005874C File Offset: 0x0005694C
		private bool Commit(GamePlayer gplayer, int turn, bool surrender, double planningTime, byte[] orders)
		{
			if (this.m_gameEnded)
			{
				return false;
			}
			if (gplayer.m_surrender || gplayer.m_dead)
			{
				return false;
			}
			if (turn != this.m_currentTurn)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"ERROR: player commited turn ",
					turn,
					" but current turn is ",
					this.m_currentTurn
				}));
				return false;
			}
			TurnData currentTurnData = this.GetCurrentTurnData();
			if (currentTurnData == null || currentTurnData.GetTurnType() != TurnType.Normal)
			{
				return false;
			}
			if (currentTurnData.Commited(gplayer.m_id))
			{
				PLog.LogError(string.Concat(new object[]
				{
					"ERROR: player ",
					gplayer.GetUser().m_name,
					" already commited turn ",
					turn
				}));
				return false;
			}
			this.AddUserPlanningTime(gplayer, planningTime);
			gplayer.m_surrender = surrender;
			if (surrender)
			{
				currentTurnData.SetSurrender(gplayer.m_id);
			}
			currentTurnData.m_newOrders[gplayer.m_id] = orders;
			this.SendPlayerStatusToAll();
			if (this.AllCommited(currentTurnData))
			{
				this.SendSimulationRequestToAll(currentTurnData);
				return true;
			}
			return false;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00058878 File Offset: 0x00056A78
		private bool AllCommited(TurnData turn)
		{
			foreach (GamePlayer p in this.m_players)
			{
				if (!this.PlayerCommited(turn, p))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x000588F0 File Offset: 0x00056AF0
		private bool PlayerCommited(TurnData turn, GamePlayer p)
		{
			return p.m_dead || p.m_leftGame || turn.Commited(p.m_id);
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00058918 File Offset: 0x00056B18
		public bool NeedPlayerAttention(User user)
		{
			GamePlayer gamePlayerByUser = this.GetGamePlayerByUser(user);
			return gamePlayerByUser != null && this.NeedPlayerAttention(gamePlayerByUser);
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x0005893C File Offset: 0x00056B3C
		private bool NeedPlayerAttention(GamePlayer player)
		{
			if (player.m_leftGame)
			{
				return false;
			}
			if (this.m_gameEnded && !player.m_seenEndGame)
			{
				return true;
			}
			if (!this.m_gameStarted && !player.m_readyToStart)
			{
				return true;
			}
			TurnData currentTurnData = this.GetCurrentTurnData();
			if (currentTurnData != null && currentTurnData.GetTurnType() == TurnType.Normal)
			{
				if (!this.PlayerCommited(currentTurnData, player))
				{
					return true;
				}
				if (this.AllCommited(currentTurnData))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x000589BC File Offset: 0x00056BBC
		public DateTime GetCreateDate()
		{
			return this.m_createDate;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x000589C4 File Offset: 0x00056BC4
		public void SetCreateDate(DateTime date)
		{
			this.m_createDate = date;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x000589D0 File Offset: 0x00056BD0
		private void SendSimulationRequestToAll(TurnData turn)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					this.RequestSimulation(gamePlayer, turn);
				}
			}
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x00058A44 File Offset: 0x00056C44
		private void RequestSimulation(GamePlayer gplayer, TurnData turn)
		{
			List<object> list = new List<object>();
			list.Add(turn.GetTurn());
			list.Add(turn.GetFrames());
			list.Add(turn.m_endState);
			list.Add(turn.m_endOrders);
			list.Add(turn.m_newSurrenders.ToArray());
			list.Add(turn.m_newOrders.Count);
			foreach (byte[] array in turn.m_newOrders)
			{
				bool flag = array != null && array.Length != 0;
				list.Add(flag);
				if (flag)
				{
					list.Add(array);
				}
			}
			gplayer.GetUser().m_rpc.Invoke("DoSimulation", list);
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00058B4C File Offset: 0x00056D4C
		private void RPC_SimulationResults(RPC rpc, List<object> args)
		{
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			int num = 0;
			int num2 = (int)args[num++];
			if (num2 == this.m_currentTurn)
			{
				GameOutcome gameOutcome = (GameOutcome)((int)args[num++]);
				int winnerTeam = (int)args[num++];
				int num3 = (int)args[num++];
				for (int i = 0; i < num3; i++)
				{
					int id = (int)args[num++];
					int place = (int)args[num++];
					int score = (int)args[num++];
					int teamScore = (int)args[num++];
					int num4 = (int)args[num++];
					int num5 = (int)args[num++];
					bool dead = (bool)args[num++];
					int num6 = (int)args[num++];
					int num7 = (int)args[num++];
					int num8 = (int)args[num++];
					Dictionary<string, int> damages = (Dictionary<string, int>)args[num++];
					GamePlayer gamePlayerByID = this.GetGamePlayerByID(id);
					if (gamePlayerByID == null)
					{
						PLog.LogError("got invalid rank player");
						return;
					}
					gamePlayerByID.m_place = place;
					gamePlayerByID.m_score = score;
					gamePlayerByID.m_teamScore = teamScore;
					gamePlayerByID.m_flagshipKiller[0] = num4;
					gamePlayerByID.m_flagshipKiller[1] = num5;
					gamePlayerByID.m_dead = dead;
					if (this.m_gameType == GameType.Points || this.m_gameType == GameType.Assassination)
					{
						gamePlayerByID.GetUser().m_stats.m_vsTotalDamage += num6;
						gamePlayerByID.GetUser().m_stats.m_vsTotalFriendlyDamage += num7;
						gamePlayerByID.GetUser().m_stats.m_vsShipsSunk += num8;
						gamePlayerByID.GetUser().m_stats.AddModuleDamages(damages);
					}
				}
				byte[] array = args[num++] as byte[];
				byte[] array2 = args[num++] as byte[];
				byte[] array3 = args[num++] as byte[];
				byte[] array4 = args[num++] as byte[];
				int[] array5 = args[num++] as int[];
				int playbackFrames = (int)args[num++];
				if (array == null || array2 == null || array3 == null || array4 == null || array5 == null)
				{
					PLog.LogError("Got null data from client, BAD CLIENT ");
					return;
				}
				TurnType type = TurnType.Normal;
				if (gameOutcome != GameOutcome.None)
				{
					this.m_gameEnded = true;
					this.m_gameOutcome = gameOutcome;
					this.m_winnerTeam = winnerTeam;
					type = TurnType.EndGame;
					this.AddUserStats();
					if (this.m_onGameOver != null)
					{
						this.m_onGameOver(this);
					}
				}
				this.SetupNewTurn(array, array2, array5, array3, array4, type, playbackFrames, 300);
				this.SendNewTurnNotifications();
			}
			this.SendCurrentTurn(gamePlayerByRPC, false);
			this.SendPlayerStatusToAll();
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x00058E68 File Offset: 0x00057068
		private void AddUserStats()
		{
			if (this.m_gameType == GameType.Points || this.m_gameType == GameType.Assassination)
			{
				foreach (GamePlayer gamePlayer in this.m_players)
				{
					if (this.m_winnerTeam != -1)
					{
						if (gamePlayer.m_team == this.m_winnerTeam)
						{
							gamePlayer.GetUser().m_stats.m_vsGamesWon++;
							if (this.m_gameType == GameType.Points)
							{
								gamePlayer.GetUser().m_stats.m_vsPointsWon++;
							}
							if (this.m_gameType == GameType.Assassination)
							{
								gamePlayer.GetUser().m_stats.m_vsAssWon++;
							}
						}
						else
						{
							gamePlayer.GetUser().m_stats.m_vsGamesLost++;
							if (this.m_gameType == GameType.Points)
							{
								gamePlayer.GetUser().m_stats.m_vsPointsLost++;
							}
							if (this.m_gameType == GameType.Assassination)
							{
								gamePlayer.GetUser().m_stats.m_vsAssLost++;
							}
						}
					}
					this.UpdateModuleAndshipUsageStats(gamePlayer);
				}
			}
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00058FC8 File Offset: 0x000571C8
		private GamePlayer GetFirstConnectedPlayer()
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_inGame)
				{
					return gamePlayer;
				}
			}
			return null;
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0005903C File Offset: 0x0005723C
		private GamePlayer GetGamePlayerByID(int id)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_id == id)
				{
					return gamePlayer;
				}
			}
			return null;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x000590B4 File Offset: 0x000572B4
		private GamePlayer GetGamePlayerByName(string name)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (gamePlayer.m_userName == name)
				{
					return gamePlayer;
				}
			}
			return null;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x00059130 File Offset: 0x00057330
		private GamePlayer GetGamePlayerByRPC(RPC rpc)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				User user = gamePlayer.GetUser();
				if (user != null && user.m_rpc == rpc)
				{
					return gamePlayer;
				}
			}
			return null;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x000591B4 File Offset: 0x000573B4
		private GamePlayer GetGamePlayerByUser(User user)
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (user == gamePlayer.GetUser())
				{
					return gamePlayer;
				}
			}
			return null;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x0005922C File Offset: 0x0005742C
		private void SendPlayerStatusToAll()
		{
			foreach (GamePlayer player in this.m_players)
			{
				this.SendPlayerStatusTo(player);
			}
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00059294 File Offset: 0x00057494
		private PlayerTurnStatus GetPlayerTurnStatus(GamePlayer p)
		{
			if (this.m_currentTurn < 0)
			{
				return PlayerTurnStatus.Planning;
			}
			if (p.m_dead)
			{
				return PlayerTurnStatus.Dead;
			}
			if (this.m_turns[this.m_currentTurn].Commited(p.m_id))
			{
				return PlayerTurnStatus.Done;
			}
			return PlayerTurnStatus.Planning;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x000592E0 File Offset: 0x000574E0
		private void SendPlayerStatusTo(GamePlayer player)
		{
			if (!player.m_inGame)
			{
				return;
			}
			List<object> list = new List<object>();
			list.Add(this.m_players.Count);
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				list.Add(gamePlayer.m_id);
				list.Add(gamePlayer.m_surrender);
				list.Add(gamePlayer.m_leftGame);
				list.Add((int)gamePlayer.GetPlayerPresenceStatus());
				list.Add((int)this.GetPlayerTurnStatus(gamePlayer));
			}
			player.GetUser().m_rpc.Invoke("PlayerStatus", list);
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000593D4 File Offset: 0x000575D4
		public void SaveData(BinaryWriter stream)
		{
			stream.Write(2);
			stream.Write(this.m_publicGame);
			stream.Write(this.m_autoJoinNextGameID);
			stream.Write(this.m_isNewCampaign);
			stream.Write(this.m_createDate.ToBinary());
			stream.Write(this.m_turnStartDate.ToBinary());
			stream.Write(this.m_gotInitState);
			stream.Write(this.m_gameStarted);
			stream.Write(this.m_gameEnded);
			stream.Write(this.m_disbanded);
			stream.Write(this.m_currentTurn);
			stream.Write(this.m_winnerTeam);
			stream.Write((int)this.m_gameOutcome);
			stream.Write(this.m_players.Count);
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				stream.Write(gamePlayer.m_id);
				gamePlayer.Save(stream);
			}
			stream.Write(this.m_turns.Count);
			foreach (TurnData turnData in this.m_turns)
			{
				turnData.Save(stream);
			}
			this.m_chatServer.Save(stream);
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x0005956C File Offset: 0x0005776C
		public void LoadData(BinaryReader stream)
		{
			int num = stream.ReadInt32();
			if (num >= 2)
			{
				this.m_publicGame = stream.ReadBoolean();
			}
			this.m_autoJoinNextGameID = stream.ReadInt32();
			this.m_isNewCampaign = stream.ReadBoolean();
			this.m_createDate = DateTime.FromBinary(stream.ReadInt64());
			this.m_turnStartDate = DateTime.FromBinary(stream.ReadInt64());
			this.m_gotInitState = stream.ReadBoolean();
			this.m_gameStarted = stream.ReadBoolean();
			this.m_gameEnded = stream.ReadBoolean();
			this.m_disbanded = stream.ReadBoolean();
			this.m_currentTurn = stream.ReadInt32();
			this.m_winnerTeam = stream.ReadInt32();
			this.m_gameOutcome = (GameOutcome)stream.ReadInt32();
			int num2 = stream.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				int id = stream.ReadInt32();
				GamePlayer gamePlayer = new GamePlayer(id);
				gamePlayer.Load(stream);
				this.m_players.Add(gamePlayer);
			}
			int num3 = stream.ReadInt32();
			for (int j = 0; j < num3; j++)
			{
				TurnData turnData = new TurnData();
				turnData.Load(stream);
				this.m_turns.Add(turnData);
			}
			this.m_chatServer.Load(stream);
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000596A8 File Offset: 0x000578A8
		private void SendNewTurnNotifications()
		{
			foreach (GamePlayer gamePlayer in this.m_players)
			{
				if (!gamePlayer.m_inGame && !gamePlayer.m_leftGame && this.m_onNewTurnNotification != null)
				{
					this.m_onNewTurnNotification(gamePlayer.GetUser(), this);
				}
			}
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x0005973C File Offset: 0x0005793C
		private void RPC_RequestReplayTurn(RPC rpc, List<object> args)
		{
			int num = (int)args[0];
			if (num < 0 || num >= this.m_turns.Count)
			{
				return;
			}
			this.m_currentTurn = num;
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			this.SendCurrentTurn(gamePlayerByRPC, true);
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00059788 File Offset: 0x00057988
		private void RPC_SaveReplay(RPC rpc, List<object> args)
		{
			if (this.m_gameOutcome == GameOutcome.None)
			{
				return;
			}
			string name = (string)args[0];
			GamePlayer gamePlayerByRPC = this.GetGamePlayerByRPC(rpc);
			if (this.m_onSaveReplay != null)
			{
				bool flag = this.m_onSaveReplay(this, gamePlayerByRPC.GetUser(), name);
				rpc.Invoke("SaveReplayReply", new object[]
				{
					flag
				});
			}
		}

		// Token: 0x040009DF RID: 2527
		private const int m_turnFrames = 300;

		// Token: 0x040009E0 RID: 2528
		public Action<Game> m_onGameOver;

		// Token: 0x040009E1 RID: 2529
		public Action<User, Game> m_onNewTurnNotification;

		// Token: 0x040009E2 RID: 2530
		public Action<Game, User, int> m_onInviteFriend;

		// Token: 0x040009E3 RID: 2531
		public Game.OnSaveReplayDelegate m_onSaveReplay;

		// Token: 0x040009E4 RID: 2532
		private List<GamePlayer> m_players = new List<GamePlayer>();

		// Token: 0x040009E5 RID: 2533
		private List<TurnData> m_turns = new List<TurnData>();

		// Token: 0x040009E6 RID: 2534
		private PackMan m_packMan;

		// Token: 0x040009E7 RID: 2535
		private ChatServer m_chatServer = new ChatServer();

		// Token: 0x040009E8 RID: 2536
		private static int m_nextGameID = 1;

		// Token: 0x040009E9 RID: 2537
		private static int m_nextCampaignID = 1;

		// Token: 0x040009EA RID: 2538
		private int m_gameID;

		// Token: 0x040009EB RID: 2539
		private int m_campaignID;

		// Token: 0x040009EC RID: 2540
		private int m_autoJoinNextGameID;

		// Token: 0x040009ED RID: 2541
		private bool m_isNewCampaign;

		// Token: 0x040009EE RID: 2542
		private DateTime m_createDate;

		// Token: 0x040009EF RID: 2543
		private DateTime m_turnStartDate;

		// Token: 0x040009F0 RID: 2544
		private float m_timeSinceActive;

		// Token: 0x040009F1 RID: 2545
		private bool m_replayMode;

		// Token: 0x040009F2 RID: 2546
		private string m_gameName;

		// Token: 0x040009F3 RID: 2547
		private string m_campaignName;

		// Token: 0x040009F4 RID: 2548
		private string m_levelName;

		// Token: 0x040009F5 RID: 2549
		private GameType m_gameType;

		// Token: 0x040009F6 RID: 2550
		private int m_nrOfPlayers;

		// Token: 0x040009F7 RID: 2551
		private FleetSizeClass m_fleetSizeClass;

		// Token: 0x040009F8 RID: 2552
		private FleetSize m_fleetSizeLimits;

		// Token: 0x040009F9 RID: 2553
		private float m_targetScore = 0.5f;

		// Token: 0x040009FA RID: 2554
		private double m_maxTurnTime = 10.0;

		// Token: 0x040009FB RID: 2555
		private float m_autoCommitTimeout = 5f;

		// Token: 0x040009FC RID: 2556
		private MapInfo m_mapInfo;

		// Token: 0x040009FD RID: 2557
		private float m_timeSyncTimer;

		// Token: 0x040009FE RID: 2558
		private float m_timeSyncSendDelay = 5f;

		// Token: 0x040009FF RID: 2559
		private GamePlayer m_sentInitiateRequestTo;

		// Token: 0x04000A00 RID: 2560
		private bool m_gotInitState;

		// Token: 0x04000A01 RID: 2561
		private bool m_gameStarted;

		// Token: 0x04000A02 RID: 2562
		private bool m_gameEnded;

		// Token: 0x04000A03 RID: 2563
		private bool m_disbanded;

		// Token: 0x04000A04 RID: 2564
		private bool m_publicGame;

		// Token: 0x04000A05 RID: 2565
		private int m_currentTurn = -1;

		// Token: 0x04000A06 RID: 2566
		private int m_winnerTeam = -1;

		// Token: 0x04000A07 RID: 2567
		private GameOutcome m_gameOutcome;

		// Token: 0x020001B2 RID: 434
		// (Invoke) Token: 0x06000F78 RID: 3960
		public delegate bool OnSaveReplayDelegate(Game game, User user, string name);
	}
}
