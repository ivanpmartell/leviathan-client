using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
[AddComponentMenu("Scripts/GameModes/GameModeTest")]
internal class GameModeTest : GameMode
{
	// Token: 0x060004DF RID: 1247 RVA: 0x00029814 File Offset: 0x00027A14
	public override void SimulationUpdate(float dt)
	{
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x00029818 File Offset: 0x00027A18
	public override GameOutcome GetOutcome()
	{
		return GameOutcome.None;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0002981C File Offset: 0x00027A1C
	public override bool IsPlayerDead(int playerID)
	{
		return base.CheckNrOfUnits(playerID) == 0;
	}
}
