using System;
using UnityEngine;

// Token: 0x02000093 RID: 147
internal class GunFollowOrder : AIState<Gun>
{
	// Token: 0x060005B4 RID: 1460 RVA: 0x0002D31C File Offset: 0x0002B51C
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0002D320 File Offset: 0x0002B520
	public override void Exit(Gun owner)
	{
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0002D324 File Offset: 0x0002B524
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		Order firstOrder = owner.GetFirstOrder();
		if (firstOrder == null)
		{
			sm.PopState();
			return;
		}
		if (firstOrder.m_type == Order.Type.Fire)
		{
			GunTarget gunTarget;
			if (firstOrder.IsStaticTarget())
			{
				gunTarget = new GunTarget(firstOrder.GetLocalTargetPos());
			}
			else
			{
				gunTarget = new GunTarget(firstOrder.GetTargetID(), firstOrder.GetLocalTargetPos());
			}
			Vector3 point;
			if (gunTarget.GetTargetWorldPos(out point, owner.GetOwnerTeam()))
			{
				if (owner.InFiringCone(point))
				{
					owner.SetTarget(gunTarget);
					sm.PushState("aim");
					if (!owner.IsLastOrder(firstOrder) || !owner.GetBarrage())
					{
						owner.RemoveFirstOrder();
					}
				}
				else if (owner.GetRemoveInvalidTarget())
				{
					owner.RemoveFirstOrder();
				}
			}
			else
			{
				owner.RemoveFirstOrder();
			}
		}
	}
}
