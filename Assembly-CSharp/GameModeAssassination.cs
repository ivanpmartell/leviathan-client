using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200006B RID: 107
[AddComponentMenu("Scripts/GameModes/GameModeAssassination")]
internal class GameModeAssassination : GameMode
{
	// Token: 0x060004B2 RID: 1202 RVA: 0x00028A24 File Offset: 0x00026C24
	public override void Setup(GameSettings gameSettings)
	{
		base.Setup(gameSettings);
		string @string = PlayerPrefs.GetString("VO", string.Empty);
		VOSystem.instance.SetAnnouncer(@string);
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00028A54 File Offset: 0x00026C54
	private int GetPlayersInTeam(List<PlayerInitData> players, int team)
	{
		int num = 0;
		foreach (PlayerInitData playerInitData in players)
		{
			if (playerInitData.m_team == team)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00028AC4 File Offset: 0x00026CC4
	public override void OnSimulationComplete()
	{
		TurnMan instance = TurnMan.instance;
		int num = 0;
		for (int i = 0; i < instance.GetNrOfPlayers(); i++)
		{
			int totalShipsSunk = instance.GetTotalShipsSunk(i);
			if (totalShipsSunk > num)
			{
				num = totalShipsSunk;
			}
		}
		if (num >= 3)
		{
			instance.SetTurnMusic("action_4");
		}
		else if (num >= 2)
		{
			instance.SetTurnMusic("action_3");
		}
		else if (num >= 1)
		{
			instance.SetTurnMusic("action_2");
		}
		else
		{
			for (int j = 0; j < instance.GetNrOfPlayers(); j++)
			{
				if (instance.GetPlayerTurnDamage(j) > 0)
				{
					instance.SetTurnMusic("action_1");
					break;
				}
			}
		}
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00028B80 File Offset: 0x00026D80
	public override void InitializeGame(List<PlayerInitData> players)
	{
		this.m_achYouAreTiny = true;
		this.m_achAtlasCrush = true;
		int[] array = new int[4];
		foreach (PlayerInitData playerInitData in players)
		{
			bool twoKings = players.Count == 3 && this.GetPlayersInTeam(players, playerInitData.m_team) == 1;
			int teamPlayerNR = array[playerInitData.m_team]++;
			this.SpawnPlayerShips(playerInitData.m_id, playerInitData.m_team, teamPlayerNR, playerInitData.m_fleet, twoKings);
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00028C44 File Offset: 0x00026E44
	private void SpawnPlayerShips(int playerID, int team, int teamPlayerNR, FleetDef playerFleet, bool twoKings)
	{
		GameObject spawnPoint = base.GetSpawnPoint();
		if (spawnPoint == null)
		{
			PLog.LogError(string.Concat(new object[]
			{
				"Could not find spawnpoint for team ",
				team,
				" playernr ",
				teamPlayerNR
			}));
			return;
		}
		base.SpawnFleet(spawnPoint, playerID, playerFleet);
		if (twoKings)
		{
			this.CreateKing(spawnPoint, new Vector3(25f, 0f, -50f), playerID);
			this.CreateKing(spawnPoint, new Vector3(-25f, 0f, -50f), playerID);
		}
		else
		{
			this.CreateKing(spawnPoint, new Vector3(0f, 0f, -50f), playerID);
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x00028D04 File Offset: 0x00026F04
	private void CreateKing(GameObject spawnPoint, Vector3 offset, int playerID)
	{
		Vector3 pos = spawnPoint.transform.TransformPoint(offset);
		pos.y = 0f;
		GameObject gameObject = ShipFactory.instance.CreateShip(this.m_kingShip, pos, spawnPoint.transform.rotation, playerID);
		if (gameObject != null)
		{
			Unit component = gameObject.GetComponent<Unit>();
			if (component != null)
			{
				component.SetKing(true);
			}
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00028D70 File Offset: 0x00026F70
	public override GameOutcome GetOutcome()
	{
		if (this.m_gameSettings.m_nrOfPlayers == 1)
		{
			return GameOutcome.None;
		}
		int num = (this.m_gameSettings.m_nrOfPlayers != 4 && this.m_gameSettings.m_nrOfPlayers != 3) ? 1 : 2;
		if (TurnMan.instance.GetTeamScore(0) >= num)
		{
			return GameOutcome.GameOver;
		}
		if (TurnMan.instance.GetTeamScore(1) >= num)
		{
			return GameOutcome.GameOver;
		}
		int num2 = 0;
		for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
		{
			if (!this.IsPlayerDead(i))
			{
				num2++;
			}
		}
		if (num2 <= 1)
		{
			return GameOutcome.GameOver;
		}
		return GameOutcome.None;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00028E18 File Offset: 0x00027018
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00028E24 File Offset: 0x00027024
	public override int GetWinnerTeam(int nrOfPlayers)
	{
		int teamScore = TurnMan.instance.GetTeamScore(0);
		int teamScore2 = TurnMan.instance.GetTeamScore(1);
		if (teamScore == teamScore2)
		{
			return -1;
		}
		if (teamScore > teamScore2)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00028E5C File Offset: 0x0002705C
	public override int GetPlayerPlace(int playerID, int nrOfPlayers)
	{
		int playerTeam = TurnMan.instance.GetPlayerTeam(playerID);
		if (playerTeam < 0 || playerTeam > 1)
		{
			return 2;
		}
		int teamScore = TurnMan.instance.GetTeamScore(0);
		int teamScore2 = TurnMan.instance.GetTeamScore(1);
		int num = (nrOfPlayers != 4 && nrOfPlayers != 3) ? 1 : 2;
		if (teamScore >= num && teamScore2 >= num)
		{
			return 0;
		}
		if (playerTeam == 0)
		{
			if (teamScore > teamScore2)
			{
				return 0;
			}
			return 1;
		}
		else
		{
			if (teamScore2 > teamScore)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00028EE0 File Offset: 0x000270E0
	public override void OnUnitKilled(Unit unit)
	{
		if (!unit.IsKing())
		{
			return;
		}
		int num = unit.GetLastDamageDealer();
		int owner = unit.GetOwner();
		int playerTeam = TurnMan.instance.GetPlayerTeam(owner);
		if (num < 0)
		{
			num = owner;
		}
		TurnMan.instance.AddFlagshipKiller(owner, num);
		if (playerTeam == 0)
		{
			TurnMan.instance.AddTeamScore(1, 1);
		}
		else if (playerTeam == 1)
		{
			TurnMan.instance.AddTeamScore(0, 1);
		}
		else
		{
			DebugUtils.Assert(false, "Invalid team " + playerTeam.ToString());
		}
		if (unit.IsVisible())
		{
			HitText.instance.AddDmgText(unit.GetNetID(), unit.transform.position, string.Empty, Constants.m_assassinatedText);
		}
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00028FA0 File Offset: 0x000271A0
	public override bool HasObjectives()
	{
		return false;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00028FA4 File Offset: 0x000271A4
	public override void CheckAchivements(UserManClient m_userManClient)
	{
		base.CheckAchivements(m_userManClient);
		base.CheckVersusAchivements(m_userManClient);
	}

	// Token: 0x0400040E RID: 1038
	public string m_kingShip;
}
