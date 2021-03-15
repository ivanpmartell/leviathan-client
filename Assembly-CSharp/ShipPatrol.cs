using System;
using System.IO;
using UnityEngine;

// Token: 0x020000A5 RID: 165
internal class ShipPatrol : AIState<Ship>
{
	// Token: 0x0600060B RID: 1547 RVA: 0x0002E41C File Offset: 0x0002C61C
	public override string DebugString(Ship owner)
	{
		return this.index.ToString() + "/" + this.points.Length.ToString();
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x0002E450 File Offset: 0x0002C650
	public override void Enter(Ship owner, AIStateMachine<Ship> sm)
	{
		if (owner.GetAiSettings().GetTarget() != null)
		{
			this.points = owner.GetAiSettings().GetTarget().GetComponent<global::Path>().GetPoints();
		}
		else
		{
			PLog.Log("ShipPatrol:Enter: No path, exit patrol." + owner.GetNetID());
			owner.GetAiSettings().m_mission = ShipAISettings.AiMission.Defend;
			sm.PopState();
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0002E4C0 File Offset: 0x0002C6C0
	public override void Exit(Ship owner)
	{
		PLog.Log("ShipPatrol:Exit");
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0002E4CC File Offset: 0x0002C6CC
	public override void Update(Ship owner, AIStateMachine<Ship> sm, float dt)
	{
		owner.GetShipAi().FindEnemy(dt);
		if (owner.GetShipAi().HasEnemy())
		{
			sm.PushState("combat");
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
		if (!owner.IsOrdersEmpty())
		{
			return;
		}
		this.index++;
		if (this.index == this.points.Length)
		{
			if (owner.GetAiSettings().m_mission == ShipAISettings.AiMission.Goto)
			{
				owner.GetAiSettings().m_mission = ShipAISettings.AiMission.Defend;
				sm.PopState();
				return;
			}
			this.index = 0;
		}
		owner.SetOrdersTo(this.points[this.index]);
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x0002E610 File Offset: 0x0002C810
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

	// Token: 0x06000610 RID: 1552 RVA: 0x0002E688 File Offset: 0x0002C888
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

	// Token: 0x040004B5 RID: 1205
	private Vector3[] points;

	// Token: 0x040004B6 RID: 1206
	private int index;
}
