using System;
using System.IO;
using UnityEngine;

// Token: 0x02000099 RID: 153
internal class Ship_Base : AIState<Ship>
{
	// Token: 0x060005D6 RID: 1494 RVA: 0x0002D99C File Offset: 0x0002BB9C
	public override string DebugString(Ship owner)
	{
		return string.Empty;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0002D9B0 File Offset: 0x0002BBB0
	public override void Save(BinaryWriter writer)
	{
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0002D9B4 File Offset: 0x0002BBB4
	public override void Load(BinaryReader reader)
	{
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0002D9B8 File Offset: 0x0002BBB8
	public void UpdateMovement(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		Vector3? goalPosition = owner.GetAi().m_goalPosition;
		if (goalPosition == null)
		{
			owner.ClearMoveOrders();
			Vector3? goalFacing = owner.GetAi().m_goalFacing;
			if (goalFacing != null)
			{
				Order order = new Order(owner, Order.Type.MoveRotate, owner.transform.position);
				order.SetFacing(owner.GetAi().m_goalFacing.Value);
				owner.AddOrder(order);
			}
			return;
		}
		if (owner.IsOrdersEmpty())
		{
			owner.SetOrdersTo(owner.GetAi().m_goalPosition.Value);
		}
		if (owner.IsBlocked())
		{
			owner.ClearMoveOrders();
		}
	}
}
