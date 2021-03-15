using System;
using UnityEngine;

// Token: 0x0200006F RID: 111
[AddComponentMenu("Scripts/GameModes/GameModeFreeForAll")]
internal class GameModeFreeForAll : GameMode
{
	// Token: 0x060004DB RID: 1243 RVA: 0x000297B4 File Offset: 0x000279B4
	public override void SimulationUpdate(float dt)
	{
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x000297B8 File Offset: 0x000279B8
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
		if (num <= 1)
		{
			return GameOutcome.GameOver;
		}
		return GameOutcome.None;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00029800 File Offset: 0x00027A00
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}
}
