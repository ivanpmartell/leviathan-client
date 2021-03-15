using System;

// Token: 0x0200009E RID: 158
internal class ShipCombat_ChickenRace : ShipCombat_Base
{
	// Token: 0x060005F1 RID: 1521 RVA: 0x0002E18C File Offset: 0x0002C38C
	public override string DebugString(Ship owner)
	{
		return string.Empty;
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0002E194 File Offset: 0x0002C394
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		Unit unit = base.VerifyTarget(owner);
		if (unit == null)
		{
			sm.ChangeState("combat");
			return;
		}
		owner.SetOrdersTo(unit.transform.position);
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0002E1D4 File Offset: 0x0002C3D4
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		Unit unit = base.VerifyTarget(owner);
		if (unit == null)
		{
			sm.ChangeState("combat");
			return;
		}
		if (base.RangeToTarget(owner) < 60f)
		{
			owner.ClearMoveOrders();
			sm.ChangeState("c_driveby");
			return;
		}
		owner.ClearMoveOrders();
		Order order = new Order(owner, Order.Type.MoveForward, unit.transform.position);
		owner.AddOrder(order);
	}
}
