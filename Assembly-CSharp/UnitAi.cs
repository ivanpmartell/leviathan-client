using System;
using System.IO;
using UnityEngine;

// Token: 0x02000119 RID: 281
[Serializable]
public class UnitAi
{
	// Token: 0x06000ADE RID: 2782 RVA: 0x00050BBC File Offset: 0x0004EDBC
	public new string ToString()
	{
		string text = "UnitAi:\n";
		text = text + "   Target: " + this.m_targetId.ToString();
		Unit unit = NetObj.GetByID(this.m_targetId) as Unit;
		if (unit)
		{
			text = text + " " + unit.name;
		}
		text += "\n";
		Vector3? goalPosition = this.m_goalPosition;
		if (goalPosition == null)
		{
			text += "   Pos=NULL";
		}
		else
		{
			text = text + "   Pos=" + this.m_goalPosition.ToString();
		}
		Vector3? goalFacing = this.m_goalFacing;
		if (goalFacing == null)
		{
			text += "/Face=NULL\n";
		}
		else
		{
			text = text + "/Face=" + this.m_goalFacing.ToString() + "\n";
		}
		return text;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00050CA4 File Offset: 0x0004EEA4
	public virtual void SaveState(BinaryWriter writer)
	{
		writer.Write(this.m_inactive);
		writer.Write(this.m_targetId);
		writer.Write(this.m_nextScan);
		Utils.WriteVector3(writer, this.m_position);
		Utils.WriteVector3Nullable(writer, this.m_goalPosition);
		Utils.WriteVector3Nullable(writer, this.m_goalFacing);
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00050CFC File Offset: 0x0004EEFC
	public virtual void LoadState(BinaryReader reader)
	{
		this.m_inactive = reader.ReadBoolean();
		this.m_targetId = reader.ReadInt32();
		this.m_nextScan = reader.ReadSingle();
		this.m_position = Utils.ReadVector3(reader);
		Utils.ReadVector3Nullable(reader, out this.m_goalPosition);
		Utils.ReadVector3Nullable(reader, out this.m_goalFacing);
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00050D54 File Offset: 0x0004EF54
	public virtual UnitAi.AttackDirection GetAttackDirection(Vector3 position)
	{
		return UnitAi.AttackDirection.None;
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00050D58 File Offset: 0x0004EF58
	public bool HasEnemy()
	{
		return this.m_targetId > 0;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00050D6C File Offset: 0x0004EF6C
	public virtual void SetTargetId(Unit target)
	{
		this.m_targetId = target.GetNetID();
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00050D7C File Offset: 0x0004EF7C
	public Unit VerifyTarget()
	{
		Unit unit = NetObj.GetByID(this.m_targetId) as Unit;
		if (unit == null)
		{
			this.m_targetId = 0;
			return null;
		}
		if (unit.IsDead())
		{
			this.m_targetId = 0;
			return null;
		}
		return unit;
	}

	// Token: 0x04000909 RID: 2313
	public bool m_inactive;

	// Token: 0x0400090A RID: 2314
	public int m_targetId = -1;

	// Token: 0x0400090B RID: 2315
	public float m_nextScan;

	// Token: 0x0400090C RID: 2316
	public Vector3 m_position = default(Vector3);

	// Token: 0x0400090D RID: 2317
	public Vector3? m_goalPosition;

	// Token: 0x0400090E RID: 2318
	public Vector3? m_goalFacing;

	// Token: 0x0200011A RID: 282
	public enum AttackDirection
	{
		// Token: 0x04000910 RID: 2320
		Front,
		// Token: 0x04000911 RID: 2321
		Back,
		// Token: 0x04000912 RID: 2322
		Left,
		// Token: 0x04000913 RID: 2323
		Right,
		// Token: 0x04000914 RID: 2324
		None
	}
}
