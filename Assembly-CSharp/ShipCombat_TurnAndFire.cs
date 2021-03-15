using System;
using UnityEngine;

// Token: 0x0200009C RID: 156
internal class ShipCombat_TurnAndFire : ShipCombat_Base
{
	// Token: 0x060005E5 RID: 1509 RVA: 0x0002DEA8 File Offset: 0x0002C0A8
	public override string DebugString(Ship owner)
	{
		return base.DebugString(owner);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0002DEB4 File Offset: 0x0002C0B4
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		Unit unit = NetObj.GetByID(owner.GetAi().m_targetId) as Unit;
		Vector3 facing = unit.transform.position - owner.transform.position;
		facing.Normalize();
		owner.ClearMoveOrders();
		Order order = new Order(owner, Order.Type.MoveForward, owner.transform.position);
		order.SetFacing(facing);
		owner.AddOrder(order);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0002DF24 File Offset: 0x0002C124
	private bool CheckChickenRun(Ship owner, Unit Target)
	{
		return true;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0002DF28 File Offset: 0x0002C128
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		if (base.SwitchState(owner, sm))
		{
			return;
		}
		Unit unit = base.VerifyTarget(owner);
		float optimalAttackRange = owner.GetShipAi().GetOptimalAttackRange(UnitAi.AttackDirection.None);
		float num = base.RangeToTarget(owner);
		if (Mathf.Abs(num - optimalAttackRange) > 50f)
		{
			if (this.CheckChickenRun(owner, unit))
			{
				sm.ChangeState("c_chicken");
			}
			else
			{
				sm.ChangeState("c_driveby");
			}
			return;
		}
		if (owner.GetShipAi().GetAttackDirection(unit.transform.position) == UnitAi.AttackDirection.Front)
		{
			return;
		}
		Vector3 facing = unit.transform.position - owner.transform.position;
		facing.Normalize();
		owner.ClearMoveOrders();
		Order order = new Order(owner, Order.Type.MoveForward, owner.transform.position);
		order.SetFacing(facing);
		owner.AddOrder(order);
	}
}
