using System;

// Token: 0x020000A0 RID: 160
internal class ShipCombat : ShipCombat_Base
{
	// Token: 0x060005F9 RID: 1529 RVA: 0x0002E348 File Offset: 0x0002C548
	public override string DebugString(Ship owner)
	{
		return base.DebugString(owner);
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0002E354 File Offset: 0x0002C554
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		owner.GetAiSettings().RunOnCombatEvent();
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0002E364 File Offset: 0x0002C564
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		sm.ChangeState("c_driveby");
	}
}
