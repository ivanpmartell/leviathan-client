using System;
using System.IO;
using UnityEngine;

// Token: 0x0200009A RID: 154
internal class ShipBossC1m3 : AIState<Ship>
{
	// Token: 0x060005DB RID: 1499 RVA: 0x0002DA6C File Offset: 0x0002BC6C
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		if (owner.GetAiSettings().GetTarget() != null)
		{
			this.points = owner.GetAiSettings().GetTarget().GetComponent<global::Path>().GetPoints();
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0002DAAC File Offset: 0x0002BCAC
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		float num = (float)owner.GetHealth();
		float num2 = (float)owner.GetMaxHealth();
		float num3 = num / num2;
		if ((double)num3 > 0.96)
		{
			return;
		}
		if (owner.GetPath() != null)
		{
			this.points = owner.GetPath().GetComponent<global::Path>().GetPoints();
		}
		if (this.points == null)
		{
			return;
		}
		if (this.index >= this.points.Length || this.index < 0)
		{
			PLog.Log(string.Concat(new object[]
			{
				"ShipPatrol::Update Out of range index ",
				this.index,
				" Count ",
				this.points.Length
			}));
			this.index = 0;
			return;
		}
		Vector3 vector = this.points[this.index];
		float num4 = Utils.DistanceXZ(owner.transform.position, vector);
		if (num4 >= owner.GetLength() / 2f + 1f)
		{
			Order order = new Order(owner, Order.Type.MoveForward, vector);
			owner.ClearMoveOrders();
			owner.AddOrder(order);
			return;
		}
		this.index++;
		if (this.index == this.points.Length)
		{
			this.index = 0;
		}
		Order order2 = new Order(owner, Order.Type.MoveForward, this.points[this.index]);
		owner.ClearMoveOrders();
		owner.AddOrder(order2);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0002DC30 File Offset: 0x0002BE30
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.index);
		writer.Write(this.points.Length);
		foreach (Vector3 vector in this.points)
		{
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0002DCA8 File Offset: 0x0002BEA8
	public override void Load(BinaryReader reader)
	{
		this.index = reader.ReadInt32();
		int num = reader.ReadInt32();
		this.points = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			this.points[i].x = reader.ReadSingle();
			this.points[i].y = reader.ReadSingle();
			this.points[i].z = reader.ReadSingle();
		}
	}

	// Token: 0x040004B3 RID: 1203
	private Vector3[] points;

	// Token: 0x040004B4 RID: 1204
	private int index;
}
