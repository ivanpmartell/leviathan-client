using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
internal class GunAim : AIState<Gun>
{
	// Token: 0x06000598 RID: 1432 RVA: 0x0002CDAC File Offset: 0x0002AFAC
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0002CDB0 File Offset: 0x0002AFB0
	public override string GetStatusText()
	{
		return "Aiming ";
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0002CDB8 File Offset: 0x0002AFB8
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		GunTarget target = owner.GetTarget();
		Vector3 vector;
		if (target != null && owner.GetOptimalTargetPosition(target, out vector))
		{
			if (!owner.InFiringCone(vector))
			{
				sm.PopState();
				return;
			}
			Unit unit = target.GetTargetObject() as Unit;
			if (unit != null)
			{
				Ship ship = owner.GetUnit() as Ship;
				if (ship != null)
				{
					ship.GetAi().m_targetId = unit.GetNetID();
				}
			}
			if (owner.AimAt(vector))
			{
				if (owner.IsContinuous())
				{
					sm.ChangeState("firebeam");
				}
				else
				{
					sm.ChangeState("fire");
				}
			}
		}
		else
		{
			PLog.Log("target is gone, returning to previus state");
			sm.PopState();
		}
	}
}
