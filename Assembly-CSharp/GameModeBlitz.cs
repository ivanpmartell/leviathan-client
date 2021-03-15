using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200006C RID: 108
[AddComponentMenu("Scripts/GameModes/GameModeBlitz")]
internal class GameModeBlitz : GameMode
{
	// Token: 0x060004C0 RID: 1216 RVA: 0x00028FC8 File Offset: 0x000271C8
	public override void Setup(GameSettings gameSettings)
	{
		base.Setup(gameSettings);
		if (gameSettings.m_nrOfPlayers >= 3)
		{
			this.m_targetScore = (int)((float)gameSettings.m_fleetSizeLimits.max * gameSettings.m_targetScore * 2f);
		}
		else
		{
			this.m_targetScore = (int)((float)gameSettings.m_fleetSizeLimits.max * gameSettings.m_targetScore);
		}
		string @string = PlayerPrefs.GetString("VO", string.Empty);
		VOSystem.instance.SetAnnouncer(@string);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x00029044 File Offset: 0x00027244
	public override void InitializeGame(List<PlayerInitData> players)
	{
		this.m_achYouAreTiny = true;
		this.m_achAtlasCrush = true;
		int[] array = new int[4];
		foreach (PlayerInitData playerInitData in players)
		{
			int teamPlayerNR = array[playerInitData.m_team]++;
			this.SpawnPlayerShips(playerInitData.m_id, playerInitData.m_team, teamPlayerNR, playerInitData.m_fleet);
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x000290E4 File Offset: 0x000272E4
	public override void OnSimulationComplete()
	{
		int targetScore = this.GetTargetScore();
		int teamScore = TurnMan.instance.GetTeamScore(0);
		int teamScore2 = TurnMan.instance.GetTeamScore(1);
		int num = targetScore / 4;
		int num2 = targetScore / 2;
		int num3 = targetScore / 4 * 3;
		if (teamScore > num3 || teamScore2 > num3)
		{
			TurnMan.instance.SetTurnMusic("action_4");
		}
		else if (teamScore > num2 || teamScore2 > num2)
		{
			TurnMan.instance.SetTurnMusic("action_3");
		}
		else if (teamScore > num || teamScore2 > num)
		{
			TurnMan.instance.SetTurnMusic("action_2");
		}
		else
		{
			for (int i = 0; i < TurnMan.instance.GetNrOfPlayers(); i++)
			{
				if (TurnMan.instance.GetPlayerTurnDamage(i) > 0)
				{
					TurnMan.instance.SetTurnMusic("action_1");
					break;
				}
			}
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x000291D0 File Offset: 0x000273D0
	public override int GetTargetScore()
	{
		return this.m_targetScore;
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x000291D8 File Offset: 0x000273D8
	private void SpawnPlayerShips(int playerID, int team, int teamPlayerNR, FleetDef playerFleet)
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
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00029238 File Offset: 0x00027438
	public override GameOutcome GetOutcome()
	{
		if (TurnMan.instance.GetTeamScore(0) >= this.m_targetScore)
		{
			return GameOutcome.GameOver;
		}
		if (TurnMan.instance.GetTeamScore(1) >= this.m_targetScore)
		{
			return GameOutcome.GameOver;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
		{
			if (!this.IsPlayerDead(i))
			{
				if (TurnMan.instance.GetPlayerTeam(i) == 0)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
		}
		if (num == 0 || num2 == 0)
		{
			return GameOutcome.GameOver;
		}
		return GameOutcome.None;
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x000292D0 File Offset: 0x000274D0
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x000292DC File Offset: 0x000274DC
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

	// Token: 0x060004C8 RID: 1224 RVA: 0x00029314 File Offset: 0x00027514
	public override int GetPlayerPlace(int playerID, int nrOfPlayers)
	{
		int playerTeam = TurnMan.instance.GetPlayerTeam(playerID);
		if (playerTeam < 0 || playerTeam > 1)
		{
			return 2;
		}
		int teamScore = TurnMan.instance.GetTeamScore(0);
		int teamScore2 = TurnMan.instance.GetTeamScore(1);
		if (teamScore >= this.m_targetScore && teamScore2 >= this.m_targetScore)
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

	// Token: 0x060004C9 RID: 1225 RVA: 0x0002938C File Offset: 0x0002758C
	public override void OnUnitKilled(Unit unit)
	{
		int totalValue = unit.GetTotalValue();
		int owner = unit.GetOwner();
		int playerTeam = TurnMan.instance.GetPlayerTeam(owner);
		PLog.Log(string.Concat(new object[]
		{
			"Unit killed ",
			unit.name,
			" value ",
			totalValue,
			" owner ",
			owner,
			" team ",
			playerTeam
		}));
		if (playerTeam == 0)
		{
			TurnMan.instance.AddTeamScore(1, totalValue);
		}
		else if (playerTeam == 1)
		{
			TurnMan.instance.AddTeamScore(0, totalValue);
		}
		if (unit.IsVisible())
		{
			HitText.instance.AddDmgText(unit.GetNetID(), unit.transform.position, totalValue.ToString(), Constants.m_pointsText);
		}
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x00029464 File Offset: 0x00027664
	public override bool HasObjectives()
	{
		return false;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00029468 File Offset: 0x00027668
	public override void CheckAchivements(UserManClient m_userManClient)
	{
		base.CheckAchivements(m_userManClient);
		base.CheckVersusAchivements(m_userManClient);
	}

	// Token: 0x0400040F RID: 1039
	private int m_targetScore = 250;
}
