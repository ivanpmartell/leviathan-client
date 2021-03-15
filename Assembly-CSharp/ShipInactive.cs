using System;

// Token: 0x020000A4 RID: 164
internal class ShipInactive : AIState<Ship>
{
	// Token: 0x06000608 RID: 1544 RVA: 0x0002E3F4 File Offset: 0x0002C5F4
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		owner.GetAi().m_inactive = true;
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0002E404 File Offset: 0x0002C604
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		owner.GetAi().m_inactive = true;
	}
}
