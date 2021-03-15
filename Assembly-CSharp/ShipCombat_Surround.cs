using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
internal class ShipCombat_Surround : ShipCombat_Base
{
	// Token: 0x060005F5 RID: 1525 RVA: 0x0002E250 File Offset: 0x0002C450
	public override string DebugString(Ship owner)
	{
		return base.DebugString(owner);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0002E25C File Offset: 0x0002C45C
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

	// Token: 0x060005F7 RID: 1527 RVA: 0x0002E2CC File Offset: 0x0002C4CC
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		Unit unit = base.VerifyTarget(owner);
		if (unit == null)
		{
			sm.PopState();
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
