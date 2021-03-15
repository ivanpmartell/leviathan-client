using System;
using UnityEngine;

// Token: 0x0200006D RID: 109
[AddComponentMenu("Scripts/GameModes/GameModeCampaign")]
internal class GameModeCampaign : GameMode
{
	// Token: 0x060004CD RID: 1229 RVA: 0x00029480 File Offset: 0x00027680
	public override void Setup(GameSettings gameSettings)
	{
		base.Setup(gameSettings);
		this.m_turnTime = 0f;
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00029494 File Offset: 0x00027694
	public override void SimulationUpdate(float dt)
	{
		this.m_turnTime += dt;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x000294A4 File Offset: 0x000276A4
	public override GameOutcome GetOutcome()
	{
		if (this.m_turnTime > 1f)
		{
			bool flag = true;
			for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
			{
				if (!this.IsPlayerDead(i))
				{
					flag = false;
				}
			}
			if (flag)
			{
				return GameOutcome.Defeat;
			}
		}
		return TurnMan.instance.GetOutcome();
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x00029500 File Offset: 0x00027700
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0002950C File Offset: 0x0002770C
	public override void OnUnitKilled(Unit unit)
	{
		int totalValue = unit.GetTotalValue();
		int owner = unit.GetOwner();
		int playerTeam = TurnMan.instance.GetPlayerTeam(owner);
		if (this.IsPlayerDead(owner) && owner <= 3)
		{
			if (this.m_gameSettings.m_localPlayerID == owner)
			{
				MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, "$message_campaign_idead", "$message_campaign_failedteam", string.Empty, 2f);
			}
			else
			{
				MessageLog.instance.ShowMessage(MessageLog.TextPosition.Top, TurnMan.instance.GetPlayer(owner).m_name + " $message_campaign_xdead", string.Empty, string.Empty, 2f);
			}
		}
		if (playerTeam != 0)
		{
			TurnMan.instance.AddTeamScore(0, totalValue);
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x000295C4 File Offset: 0x000277C4
	public override bool HasObjectives()
	{
		return true;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x000295C8 File Offset: 0x000277C8
	public override void CheckAchivements(UserManClient m_userManClient)
	{
		base.CheckAchivements(m_userManClient);
		if (TurnMan.instance.GetOutcome() == GameOutcome.Victory)
		{
			if (this.m_gameSettings.m_level == "t1m3")
			{
				m_userManClient.UnlockAchievement(17);
			}
			if (this.m_gameSettings.m_level == "c1m3")
			{
				m_userManClient.UnlockAchievement(18);
			}
			if (this.m_gameSettings.m_level == "c1m6")
			{
				m_userManClient.UnlockAchievement(19);
			}
			if (this.m_gameSettings.m_level == "c1m9")
			{
				m_userManClient.UnlockAchievement(11);
			}
			if (this.m_gameSettings.m_level == "cm_wave")
			{
				m_userManClient.UnlockAchievement(4);
			}
			if (this.m_gameSettings.m_level == "cm_wave_2")
			{
				m_userManClient.UnlockAchievement(5);
			}
			if (this.m_gameSettings.m_level == "cm_wave_3")
			{
				m_userManClient.UnlockAchievement(6);
			}
			if (this.m_gameSettings.m_level == "cm_arctic")
			{
				m_userManClient.UnlockAchievement(10);
			}
			if (this.m_gameSettings.m_level == "cm_sluice")
			{
				m_userManClient.UnlockAchievement(7);
			}
			if (TurnMan.instance.m_missionAchievement != -1)
			{
				m_userManClient.UnlockAchievement(TurnMan.instance.m_missionAchievement);
			}
		}
	}

	// Token: 0x04000410 RID: 1040
	private float m_turnTime;
}
