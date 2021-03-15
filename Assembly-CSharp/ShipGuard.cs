using System;

// Token: 0x020000A2 RID: 162
internal class ShipGuard : AIState<Ship>
{
	// Token: 0x06000600 RID: 1536 RVA: 0x0002E394 File Offset: 0x0002C594
	public override string DebugString(Ship owner)
	{
		return string.Empty;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002E39C File Offset: 0x0002C59C
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x0002E3A0 File Offset: 0x0002C5A0
	public override void Exit(Ship owner)
	{
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x0002E3A4 File Offset: 0x0002C5A4
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		owner.GetShipAi().FindEnemy(dt);
		if (owner.GetShipAi().HasEnemy())
		{
			sm.PushState("combat");
			return;
		}
	}
}
