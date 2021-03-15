using System;

// Token: 0x020000A1 RID: 161
internal class ShipFollow : AIState<Ship>
{
	// Token: 0x060005FD RID: 1533 RVA: 0x0002E37C File Offset: 0x0002C57C
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0002E380 File Offset: 0x0002C580
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		PLog.Log("Folllooow");
	}
}
