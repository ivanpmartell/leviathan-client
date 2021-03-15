using System;
using System.Collections.Generic;
using System.IO;
using PTech;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class TurnMan
{
	// Token: 0x06000D85 RID: 3461 RVA: 0x000612AC File Offset: 0x0005F4AC
	public TurnMan()
	{
		TurnMan.m_instance = this;
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x06000D86 RID: 3462 RVA: 0x000612F8 File Offset: 0x0005F4F8
	public static TurnMan instance
	{
		get
		{
			return TurnMan.m_instance;
		}
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00061300 File Offset: 0x0005F500
	public void Close()
	{
		TurnMan.m_instance = null;
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00061308 File Offset: 0x0005F508
	public bool IsHostile(int playerID, int otherPlayerID)
	{
		if (playerID == otherPlayerID)
		{
			return false;
		}
		int team = this.GetPlayer(playerID).m_team;
		int team2 = this.GetPlayer(otherPlayerID).m_team;
		return team != 4 && team2 != 4 && (team == -1 || team2 == -1 || team != team2);
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00061360 File Offset: 0x0005F560
	public void SetGameType(GameType gameType)
	{
		this.m_gameType = gameType;
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x0006136C File Offset: 0x0005F56C
	public GameType GetGameType()
	{
		return this.m_gameType;
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00061374 File Offset: 0x0005F574
	public void SetNrOfPlayers(int nr)
	{
		this.m_nrOfPlayers = nr;
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00061380 File Offset: 0x0005F580
	public int GetNrOfPlayers()
	{
		return this.m_nrOfPlayers;
	}

	// Token: 0x06000D8D RID: 3469 RVA: 0x00061388 File Offset: 0x0005F588
	public void SetPlayerHuman(int playerID, bool ishuman)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_isHuman = ishuman;
	}

	// Token: 0x06000D8E RID: 3470 RVA: 0x000613A4 File Offset: 0x0005F5A4
	public void SetPlayerName(int playerID, string name)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_name = name;
	}

	// Token: 0x06000D8F RID: 3471 RVA: 0x000613C0 File Offset: 0x0005F5C0
	public void SetPlayerFlag(int playerID, int flag)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_flag = flag;
	}

	// Token: 0x06000D90 RID: 3472 RVA: 0x000613DC File Offset: 0x0005F5DC
	public void SetPlayerColors(int playerID, Color color)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_primaryColor = color;
	}

	// Token: 0x06000D91 RID: 3473 RVA: 0x000613F8 File Offset: 0x0005F5F8
	public bool IsHuman(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_isHuman;
	}

	// Token: 0x06000D92 RID: 3474 RVA: 0x00061414 File Offset: 0x0005F614
	public void SetPlayerTeam(int playerID, int team)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_team = team;
	}

	// Token: 0x06000D93 RID: 3475 RVA: 0x00061430 File Offset: 0x0005F630
	public int GetPlayerTeam(int playerID)
	{
		if (playerID < 0)
		{
			return -1;
		}
		return this.GetPlayer(playerID).m_team;
	}

	// Token: 0x06000D94 RID: 3476 RVA: 0x00061448 File Offset: 0x0005F648
	public string GetPlayerName(int playerID)
	{
		if (playerID < 0)
		{
			return string.Empty;
		}
		return this.GetPlayer(playerID).m_name;
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00061464 File Offset: 0x0005F664
	public int GetPlayerFlag(int playerID)
	{
		if (playerID < 0)
		{
			return -1;
		}
		return this.GetPlayer(playerID).m_flag;
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x0006147C File Offset: 0x0005F67C
	public void GetPlayerColors(int playerID, out Color primaryColor)
	{
		if (playerID < 0 || playerID >= this.m_players.Count)
		{
			primaryColor = Color.white;
			return;
		}
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		primaryColor = player.m_primaryColor;
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x000614C4 File Offset: 0x0005F6C4
	public void ResetTurnStats()
	{
		foreach (TurnMan.PlayerTurnData playerTurnData in this.m_players)
		{
			playerTurnData.m_turnDamage = 0;
			playerTurnData.m_turnFriendlyDamage = 0;
			playerTurnData.m_turnShipsSunk = 0;
			playerTurnData.m_turnGunDamage.Clear();
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x00061544 File Offset: 0x0005F744
	public void AddFlagshipKiller(int playerID, int flagshipKillerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		if (player.m_flagshipKilledBy[0] != -1)
		{
			player.m_flagshipKilledBy[1] = flagshipKillerID;
		}
		else
		{
			player.m_flagshipKilledBy[0] = flagshipKillerID;
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x00061580 File Offset: 0x0005F780
	public int GetFlagshipKiller(int playerID, int flagShipNr)
	{
		DebugUtils.Assert(flagShipNr == 0 || flagShipNr == 1);
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_flagshipKilledBy[flagShipNr];
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x000615B0 File Offset: 0x0005F7B0
	public void AddPlayerScore(int playerID, int score)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_score += score;
	}

	// Token: 0x06000D9B RID: 3483 RVA: 0x000615D4 File Offset: 0x0005F7D4
	public void AddShieldAbsorb(int playerID, int damage)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_totalDamageAbsorbed += damage;
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000615F8 File Offset: 0x0005F7F8
	public void AddPlayerDamage(int playerID, int damage, bool friendly, string gunName)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_turnDamage += damage;
		player.m_totalDamageInflicted += damage;
		if (friendly)
		{
			player.m_turnFriendlyDamage += damage;
		}
		if (gunName.Length > 0)
		{
			int num;
			if (player.m_turnGunDamage.TryGetValue(gunName, out num))
			{
				player.m_turnGunDamage[gunName] = num + damage;
			}
			else
			{
				player.m_turnGunDamage.Add(gunName, damage);
			}
		}
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x00061684 File Offset: 0x0005F884
	public int GetTotalShipsSunk(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_totalShipsSunk;
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x000616A0 File Offset: 0x0005F8A0
	public int GetTotalShipsLost(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_totalShipsLost;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000616BC File Offset: 0x0005F8BC
	public int GetPlayerTurnDamage(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_turnDamage;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x000616D8 File Offset: 0x0005F8D8
	public void AddShipsSunk(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_turnShipsSunk++;
		player.m_totalShipsSunk++;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x0006170C File Offset: 0x0005F90C
	public void AddShipsLost(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		player.m_totalShipsLost++;
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00061730 File Offset: 0x0005F930
	public void AddTeamScore(int team, int score)
	{
		while (this.m_teamScore.Count < team + 1)
		{
			this.m_teamScore.Add(0);
		}
		List<int> teamScore;
		List<int> list = teamScore = this.m_teamScore;
		int num = teamScore[team];
		list[team] = num + score;
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0006177C File Offset: 0x0005F97C
	public int GetPlayerScore(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		return player.m_score;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x00061798 File Offset: 0x0005F998
	public int GetTeamSize(int team)
	{
		int num = 0;
		foreach (TurnMan.PlayerTurnData playerTurnData in this.m_players)
		{
			if (playerTurnData.m_team == team)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000DA5 RID: 3493 RVA: 0x0006180C File Offset: 0x0005FA0C
	public int GetTeamScoreForPlayer(int playerID)
	{
		TurnMan.PlayerTurnData player = this.GetPlayer(playerID);
		if (player.m_team >= 0)
		{
			return this.GetTeamScore(player.m_team);
		}
		return 0;
	}

	// Token: 0x06000DA6 RID: 3494 RVA: 0x0006183C File Offset: 0x0005FA3C
	public int GetTeamScore(int team)
	{
		if (this.m_teamScore.Count <= team)
		{
			return 0;
		}
		return this.m_teamScore[team];
	}

	// Token: 0x06000DA7 RID: 3495 RVA: 0x00061860 File Offset: 0x0005FA60
	public int[] GetPlayerScoreList()
	{
		int[] array = new int[this.m_players.Count];
		for (int i = 0; i < this.m_players.Count; i++)
		{
			array[i] = this.m_players[i].m_score;
		}
		return array;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x000618B0 File Offset: 0x0005FAB0
	public TurnMan.PlayerTurnData GetAccoladeAbsorbed()
	{
		TurnMan.PlayerTurnData playerTurnData = this.m_players[0];
		for (int i = 0; i < this.m_players.Count; i++)
		{
			if (this.m_players[i].m_totalDamageAbsorbed > playerTurnData.m_totalDamageAbsorbed)
			{
				playerTurnData = this.m_players[i];
			}
		}
		return playerTurnData;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00061910 File Offset: 0x0005FB10
	public TurnMan.PlayerTurnData GetAccoladeDestory(bool highest)
	{
		TurnMan.PlayerTurnData playerTurnData = this.m_players[0];
		int totalDamageInflicted = this.m_players[0].m_totalDamageInflicted;
		for (int i = 0; i < this.m_players.Count; i++)
		{
			if (this.m_players[i].m_isHuman)
			{
				if (highest)
				{
					if (this.m_players[i].m_totalDamageInflicted > playerTurnData.m_totalDamageInflicted)
					{
						playerTurnData = this.m_players[i];
					}
				}
				else if (this.m_players[i].m_totalDamageInflicted < playerTurnData.m_totalDamageInflicted)
				{
					playerTurnData = this.m_players[i];
				}
			}
		}
		return playerTurnData;
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x000619D4 File Offset: 0x0005FBD4
	public int[] GetTeamScoreList()
	{
		return this.m_teamScore.ToArray();
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x000619E4 File Offset: 0x0005FBE4
	public void Save(BinaryWriter writer)
	{
		writer.Write(this.m_nrOfPlayers);
		writer.Write((byte)this.m_gameType);
		writer.Write(this.m_players.Count);
		foreach (TurnMan.PlayerTurnData playerTurnData in this.m_players)
		{
			writer.Write(playerTurnData.m_name);
			writer.Write(playerTurnData.m_score);
			writer.Write((byte)playerTurnData.m_team);
			writer.Write(playerTurnData.m_isHuman);
			writer.Write((short)playerTurnData.m_flag);
			writer.Write((short)playerTurnData.m_flagshipKilledBy[0]);
			writer.Write((short)playerTurnData.m_flagshipKilledBy[1]);
			writer.Write((short)playerTurnData.m_totalShipsSunk);
			writer.Write((short)playerTurnData.m_totalShipsLost);
			writer.Write((short)playerTurnData.m_totalDamageInflicted);
			writer.Write((short)playerTurnData.m_totalDamageAbsorbed);
			writer.Write((short)playerTurnData.m_totalTimeTraveled);
			writer.Write(playerTurnData.m_primaryColor.r);
			writer.Write(playerTurnData.m_primaryColor.g);
			writer.Write(playerTurnData.m_primaryColor.b);
			writer.Write(playerTurnData.m_primaryColor.a);
		}
		writer.Write(this.m_teamScore.Count);
		foreach (int value in this.m_teamScore)
		{
			writer.Write(value);
		}
		writer.Write((int)this.m_endGame);
		writer.Write(this.m_music);
		writer.Write(this.m_missionObjectives.Count);
		foreach (TurnMan.MissionObjective missionObjective in this.m_missionObjectives)
		{
			missionObjective.SaveState(writer);
		}
		writer.Write(this.m_missionAchievement);
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00061C44 File Offset: 0x0005FE44
	public void Load(BinaryReader reader)
	{
		this.m_nrOfPlayers = reader.ReadInt32();
		this.m_gameType = (GameType)reader.ReadByte();
		this.m_players.Clear();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			TurnMan.PlayerTurnData playerTurnData = new TurnMan.PlayerTurnData();
			playerTurnData.m_name = reader.ReadString();
			playerTurnData.m_score = reader.ReadInt32();
			playerTurnData.m_team = (int)reader.ReadByte();
			playerTurnData.m_isHuman = reader.ReadBoolean();
			playerTurnData.m_flag = (int)reader.ReadInt16();
			playerTurnData.m_flagshipKilledBy[0] = (int)reader.ReadInt16();
			playerTurnData.m_flagshipKilledBy[1] = (int)reader.ReadInt16();
			playerTurnData.m_totalShipsSunk = (int)reader.ReadInt16();
			playerTurnData.m_totalShipsLost = (int)reader.ReadInt16();
			playerTurnData.m_totalDamageInflicted = (int)reader.ReadInt16();
			playerTurnData.m_totalDamageAbsorbed = (int)reader.ReadInt16();
			playerTurnData.m_totalTimeTraveled = (int)reader.ReadInt16();
			playerTurnData.m_primaryColor.r = reader.ReadSingle();
			playerTurnData.m_primaryColor.g = reader.ReadSingle();
			playerTurnData.m_primaryColor.b = reader.ReadSingle();
			playerTurnData.m_primaryColor.a = reader.ReadSingle();
			this.m_players.Add(playerTurnData);
		}
		this.m_teamScore.Clear();
		int num2 = reader.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			this.m_teamScore.Add(reader.ReadInt32());
		}
		this.m_endGame = (GameOutcome)reader.ReadInt32();
		this.m_music = reader.ReadString();
		int num3 = reader.ReadInt32();
		this.m_missionObjectives = new List<TurnMan.MissionObjective>();
		for (int k = 0; k < num3; k++)
		{
			TurnMan.MissionObjective missionObjective = new TurnMan.MissionObjective();
			missionObjective.LoadState(reader);
			this.m_missionObjectives.Add(missionObjective);
		}
		this.m_missionAchievement = reader.ReadInt32();
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00061E1C File Offset: 0x0006001C
	public TurnMan.PlayerTurnData GetPlayer(int id)
	{
		while (this.m_players.Count < id + 1)
		{
			TurnMan.PlayerTurnData playerTurnData = new TurnMan.PlayerTurnData();
			playerTurnData.m_team = this.m_players.Count;
			this.m_players.Add(playerTurnData);
		}
		return this.m_players[id];
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00061E70 File Offset: 0x00060070
	public void SetMissionObjective(string name, MNAction.ObjectiveStatus status)
	{
		foreach (TurnMan.MissionObjective missionObjective in this.m_missionObjectives)
		{
			if (missionObjective.m_text == name)
			{
				missionObjective.m_status = status;
				if (status == MNAction.ObjectiveStatus.Done)
				{
					MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, "$" + name, "$message_objectivedone", "ObjectiveDoneMessage", 3f);
				}
				return;
			}
		}
		TurnMan.MissionObjective missionObjective2 = new TurnMan.MissionObjective();
		missionObjective2.m_text = name;
		missionObjective2.m_status = status;
		this.m_missionObjectives.Add(missionObjective2);
		MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, "$message_newobjective", "$" + name, string.Empty, 4f);
	}

	// Token: 0x06000DAF RID: 3503 RVA: 0x00061F60 File Offset: 0x00060160
	public GameOutcome GetOutcome()
	{
		GameOutcome endGameStatus = CheatMan.instance.GetEndGameStatus();
		if (endGameStatus != GameOutcome.None)
		{
			return endGameStatus;
		}
		return this.m_endGame;
	}

	// Token: 0x06000DB0 RID: 3504 RVA: 0x00061F88 File Offset: 0x00060188
	public void SetTurnMusic(string music)
	{
		if (music == this.m_music)
		{
			return;
		}
		this.m_music = music;
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00061FA4 File Offset: 0x000601A4
	public string GetTurnMusic()
	{
		return this.m_music;
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00061FAC File Offset: 0x000601AC
	public void PlayBriefing(string name)
	{
		MNAction.MNActionElement[] array = new MNAction.MNActionElement[]
		{
			new MNAction.MNActionElement()
		};
		array[0].m_type = MNAction.ActionType.ShowDebriefing;
		array[0].m_parameter = name;
		this.m_dialog = array;
	}

	// Token: 0x04000B17 RID: 2839
	private static TurnMan m_instance;

	// Token: 0x04000B18 RID: 2840
	public GameOutcome m_endGame;

	// Token: 0x04000B19 RID: 2841
	public MNAction.MNActionElement[] m_dialog;

	// Token: 0x04000B1A RID: 2842
	public List<TurnMan.MissionObjective> m_missionObjectives = new List<TurnMan.MissionObjective>();

	// Token: 0x04000B1B RID: 2843
	private List<TurnMan.PlayerTurnData> m_players = new List<TurnMan.PlayerTurnData>();

	// Token: 0x04000B1C RID: 2844
	private List<int> m_teamScore = new List<int>();

	// Token: 0x04000B1D RID: 2845
	private int m_nrOfPlayers;

	// Token: 0x04000B1E RID: 2846
	private GameType m_gameType;

	// Token: 0x04000B1F RID: 2847
	private string m_music = string.Empty;

	// Token: 0x04000B20 RID: 2848
	public int m_missionAchievement = -1;

	// Token: 0x0200016B RID: 363
	public class PlayerTurnData
	{
		// Token: 0x04000B21 RID: 2849
		public string m_name = string.Empty;

		// Token: 0x04000B22 RID: 2850
		public int m_score;

		// Token: 0x04000B23 RID: 2851
		public int m_team = -1;

		// Token: 0x04000B24 RID: 2852
		public bool m_isHuman;

		// Token: 0x04000B25 RID: 2853
		public int m_flag = 4;

		// Token: 0x04000B26 RID: 2854
		public Color m_primaryColor = Color.red;

		// Token: 0x04000B27 RID: 2855
		public int[] m_flagshipKilledBy = new int[]
		{
			-1,
			-1
		};

		// Token: 0x04000B28 RID: 2856
		public int m_totalShipsSunk;

		// Token: 0x04000B29 RID: 2857
		public int m_totalShipsLost;

		// Token: 0x04000B2A RID: 2858
		public int m_totalDamageInflicted;

		// Token: 0x04000B2B RID: 2859
		public int m_totalDamageAbsorbed;

		// Token: 0x04000B2C RID: 2860
		public int m_totalTimeTraveled;

		// Token: 0x04000B2D RID: 2861
		public int m_turnDamage;

		// Token: 0x04000B2E RID: 2862
		public int m_turnFriendlyDamage;

		// Token: 0x04000B2F RID: 2863
		public int m_turnShipsSunk;

		// Token: 0x04000B30 RID: 2864
		public Dictionary<string, int> m_turnGunDamage = new Dictionary<string, int>();
	}

	// Token: 0x0200016C RID: 364
	public class MissionObjective
	{
		// Token: 0x06000DB5 RID: 3509 RVA: 0x00062044 File Offset: 0x00060244
		public void SaveState(BinaryWriter writer)
		{
			writer.Write((int)this.m_status);
			writer.Write(this.m_text);
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00062060 File Offset: 0x00060260
		public void LoadState(BinaryReader reader)
		{
			this.m_status = (MNAction.ObjectiveStatus)reader.ReadInt32();
			this.m_text = reader.ReadString();
		}

		// Token: 0x04000B31 RID: 2865
		public MNAction.ObjectiveStatus m_status;

		// Token: 0x04000B32 RID: 2866
		public string m_text;
	}
}
