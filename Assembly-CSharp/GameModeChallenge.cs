using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
[AddComponentMenu("Scripts/GameModes/GameModeChallenge")]
internal class GameModeChallenge : GameMode
{
	// Token: 0x060004D5 RID: 1237 RVA: 0x00029744 File Offset: 0x00027944
	public override void SimulationUpdate(float dt)
	{
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00029748 File Offset: 0x00027948
	public override GameOutcome GetOutcome()
	{
		int num = 0;
		for (int i = 0; i < this.m_gameSettings.m_nrOfPlayers; i++)
		{
			if (!this.IsPlayerDead(i))
			{
				num++;
			}
		}
		if (num <= 0)
		{
			return GameOutcome.Defeat;
		}
		return GameOutcome.None;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00029790 File Offset: 0x00027990
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0002979C File Offset: 0x0002799C
	public override bool HasObjectives()
	{
		return true;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000297A0 File Offset: 0x000279A0
	public override void CheckAchivements(UserManClient m_userManClient)
	{
		base.CheckAchivements(m_userManClient);
	}
}
