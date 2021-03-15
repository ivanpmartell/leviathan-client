using System;

// Token: 0x020000A6 RID: 166
internal class ShipThink : AIState<Ship>
{
	// Token: 0x06000612 RID: 1554 RVA: 0x0002E714 File Offset: 0x0002C914
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x0002E718 File Offset: 0x0002C918
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		if (owner.GetOwner() <= 3)
		{
			sm.PushState("human");
			return;
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Inactive)
		{
			sm.PushState("inactive");
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Defend)
		{
			sm.PushState("guard");
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Patrol)
		{
			sm.PushState("patrol");
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Goto)
		{
			sm.PushState("patrol");
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Attack)
		{
			sm.PushState("attack");
		}
		if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.BossC1M3)
		{
			sm.PushState("BossC1M3");
		}
	}
}
