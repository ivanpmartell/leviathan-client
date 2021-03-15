using System;
using System.IO;
using UnityEngine;

// Token: 0x02000078 RID: 120
[AddComponentMenu("Scripts/Mission/MNGameOver")]
public class MNGameOver : MNode
{
	// Token: 0x0600051D RID: 1309 RVA: 0x0002A51C File Offset: 0x0002871C
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0002A524 File Offset: 0x00028724
	public override void DoAction()
	{
		PLog.Log("End Game" + TurnMan.instance.ToString());
		TurnMan.instance.m_endGame = GameOutcome.Victory;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0002A558 File Offset: 0x00028758
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0002A564 File Offset: 0x00028764
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
	}
}
