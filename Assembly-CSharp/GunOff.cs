using System;

// Token: 0x02000095 RID: 149
internal class GunOff : AIState<Gun>
{
	// Token: 0x060005BE RID: 1470 RVA: 0x0002D5B4 File Offset: 0x0002B7B4
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		owner.SetTarget(null);
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0002D5C0 File Offset: 0x0002B7C0
	public override string GetStatusText()
	{
		return "Offline";
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0002D5C8 File Offset: 0x0002B7C8
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
	}
}
