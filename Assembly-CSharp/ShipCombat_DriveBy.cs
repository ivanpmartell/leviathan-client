using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
internal class ShipCombat_DriveBy : ShipCombat_Base
{
	// Token: 0x060005EA RID: 1514 RVA: 0x0002E010 File Offset: 0x0002C210
	public override string DebugString(Ship owner)
	{
		return base.DebugString(owner);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0002E01C File Offset: 0x0002C21C
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0002E020 File Offset: 0x0002C220
	private Vector3 GetBehindPosition(Ship ship)
	{
		return ship.transform.position - ship.transform.forward * 50f;
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0002E054 File Offset: 0x0002C254
	private bool GetDriveByPosition(Unit target, Ship ship, out Vector3 pos)
	{
		float d = 100f;
		float f = (float)PRand.Range(0, 360);
		Vector3 a = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		pos = target.transform.position + a * d;
		return ship.IsWater(pos);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0002E0C0 File Offset: 0x0002C2C0
	private void UpdateMovement(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		if (owner.IsBlocked())
		{
			owner.ClearMoveOrders();
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0002E0D4 File Offset: 0x0002C2D4
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		if (base.SwitchState(owner, sm))
		{
			return;
		}
		Unit target = base.VerifyTarget(owner);
		float optimalAttackRange = owner.GetShipAi().GetOptimalAttackRange(UnitAi.AttackDirection.None);
		float num = base.RangeToTarget(owner);
		float num2 = Mathf.Abs(num - optimalAttackRange);
		if (num2 < 30f)
		{
			sm.ChangeState("c_turnandfire");
			return;
		}
		this.UpdateMovement(owner, sm, dt);
		if (!owner.IsOrdersEmpty())
		{
			return;
		}
		bool flag = true;
		if (flag)
		{
			Vector3 ordersTo = default(Vector3);
			if (this.GetDriveByPosition(target, owner, out ordersTo))
			{
				owner.SetOrdersTo(ordersTo);
			}
			return;
		}
		owner.GetAi().m_targetId = 0;
		sm.PopState();
	}
}
