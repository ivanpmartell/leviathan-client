using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x02000153 RID: 339
	internal abstract class ServerBase
	{
		// Token: 0x06000CC6 RID: 3270 RVA: 0x0005BA58 File Offset: 0x00059C58
		public ServerBase(MapMan mapman)
		{
			this.m_mapMan = mapman;
		}

		// Token: 0x06000CC7 RID: 3271
		protected abstract Game CreateGame(string name, int gameID, int campaignID, GameType gameType, string campaignName, string levelName, int nrOfPlayers, FleetSizeClass fleetSizeClass, float targetScore, double turnTime, MapInfo mapinfo);

		// Token: 0x06000CC8 RID: 3272 RVA: 0x0005BA68 File Offset: 0x00059C68
		protected void OnGameOver(Game game)
		{
			if (game.GetGameType() == GameType.Campaign)
			{
				this.OnCampaignGameEnded(game);
			}
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x0005BA80 File Offset: 0x00059C80
		private void OnCampaignGameEnded(Game game)
		{
			GameOutcome outcome = game.GetOutcome();
			if (outcome == GameOutcome.Victory || outcome == GameOutcome.Defeat)
			{
				MapInfo mapInfo;
				if (outcome == GameOutcome.Victory)
				{
					mapInfo = this.m_mapMan.GetNextCampaignMap(game.GetCampaign(), game.GetLevelName());
				}
				else
				{
					mapInfo = this.m_mapMan.GetMapByName(game.GetGameType(), game.GetCampaign(), game.GetLevelName());
				}
				if (mapInfo != null)
				{
					string name = game.GetName();
					Game game2 = this.CreateGame(name, 0, game.GetCampaignID(), game.GetGameType(), game.GetCampaign(), mapInfo.m_name, game.GetNrOfPlayers(), game.GetFleetSizeClass(), game.GetTargetScore(), game.GetMaxTurnTime(), mapInfo);
					game.SetAutoJoinNextGameID(game2.GetGameID());
					List<User> nextGameUserList = game.GetNextGameUserList();
					foreach (User user in nextGameUserList)
					{
						user.UnlockCampaignMap(game2.GetCampaign(), mapInfo.m_name);
						game2.AddUserToGame(user, game.IsAdmin(user));
						string playerFleet = game.GetPlayerFleet(user.m_name);
						if (playerFleet != string.Empty)
						{
							game2.SetPlayerFleet(user.m_name, playerFleet);
						}
					}
				}
			}
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x0005BBE8 File Offset: 0x00059DE8
		protected Game CreateGameFromArray(byte[] data, string overrideGameName)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			string name = binaryReader.ReadString();
			int gameID = binaryReader.ReadInt32();
			int campaignID = binaryReader.ReadInt32();
			GameType gameType = (GameType)binaryReader.ReadInt32();
			string text = binaryReader.ReadString();
			string text2 = binaryReader.ReadString();
			int nrOfPlayers = binaryReader.ReadInt32();
			FleetSizeClass fleetSizeClass = (FleetSizeClass)binaryReader.ReadInt32();
			float targetScore = binaryReader.ReadSingle();
			double turnTime = binaryReader.ReadDouble();
			MapInfo mapByName = this.m_mapMan.GetMapByName(gameType, text, text2);
			if (mapByName == null)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"Missing map info for ",
					gameType,
					" ",
					text,
					" ",
					text2
				}));
				throw new Exception(string.Concat(new object[]
				{
					"Missing map info for ",
					gameType,
					" ",
					text,
					" ",
					text2
				}));
			}
			if (overrideGameName != string.Empty)
			{
				name = overrideGameName;
			}
			Game game = this.CreateGame(name, gameID, campaignID, gameType, text, text2, nrOfPlayers, fleetSizeClass, targetScore, turnTime, mapByName);
			game.LoadData(binaryReader);
			return game;
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x0005BD28 File Offset: 0x00059F28
		protected byte[] GameToArray(Game game)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(1);
			binaryWriter.Write(game.GetName());
			binaryWriter.Write(game.GetGameID());
			binaryWriter.Write(game.GetCampaignID());
			binaryWriter.Write((int)game.GetGameType());
			binaryWriter.Write(game.GetCampaign());
			binaryWriter.Write(game.GetLevelName());
			binaryWriter.Write(game.GetMaxPlayers());
			binaryWriter.Write((int)game.GetFleetSizeClass());
			binaryWriter.Write(game.GetTargetScore());
			binaryWriter.Write(game.GetMaxTurnTime());
			game.SaveData(binaryWriter);
			return memoryStream.ToArray();
		}

		// Token: 0x04000A93 RID: 2707
		protected MapMan m_mapMan;
	}
}
