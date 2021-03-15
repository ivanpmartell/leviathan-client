using System;
using System.IO;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class GunTarget
{
	// Token: 0x060006AE RID: 1710 RVA: 0x00033438 File Offset: 0x00031638
	public GunTarget(Vector3 worldPos)
	{
		this.m_targetPos = worldPos;
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00033450 File Offset: 0x00031650
	public GunTarget(int targetNetID, Vector3 localTargetPos)
	{
		this.m_targetID = targetNetID;
		this.m_targetPos = localTargetPos;
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00033470 File Offset: 0x00031670
	public GunTarget(BinaryReader reader)
	{
		this.Load(reader);
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x00033488 File Offset: 0x00031688
	public bool IsEqual(GunTarget other)
	{
		return other.m_targetID == this.m_targetID && other.m_targetPos == this.m_targetPos;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000334B0 File Offset: 0x000316B0
	public bool IsValid()
	{
		return this.m_targetID < 1 || NetObj.GetByID(this.m_targetID) != null;
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x000334D4 File Offset: 0x000316D4
	public bool GetTargetWorldPos(out Vector3 worldPos, int ownerTeam)
	{
		if (this.m_targetID < 1)
		{
			worldPos = this.m_targetPos;
			return true;
		}
		NetObj byID = NetObj.GetByID(this.m_targetID);
		if (!(byID != null))
		{
			worldPos = Vector3.zero;
			return false;
		}
		if (!this.IsTargetAlive(byID))
		{
			worldPos = Vector3.zero;
			return false;
		}
		if (!byID.IsSeenByTeam(ownerTeam))
		{
			worldPos = Vector3.zero;
			return false;
		}
		worldPos = byID.transform.TransformPoint(this.m_targetPos);
		Ship ship = byID as Ship;
		if (ship != null && worldPos.y > ship.m_deckHeight)
		{
			worldPos.y = ship.m_deckHeight;
		}
		return true;
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0003359C File Offset: 0x0003179C
	public bool IsTargetAlive()
	{
		if (this.m_targetID >= 1)
		{
			NetObj byID = NetObj.GetByID(this.m_targetID);
			return byID != null && this.IsTargetAlive(byID);
		}
		return true;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x000335D8 File Offset: 0x000317D8
	private bool IsTargetAlive(NetObj obj)
	{
		Unit unit = obj as Unit;
		if (unit != null)
		{
			return !unit.IsDead();
		}
		HPModule hpmodule = obj as HPModule;
		return hpmodule != null && !hpmodule.IsDisabled();
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00033620 File Offset: 0x00031820
	public NetObj GetTargetObject()
	{
		if (this.m_targetID >= 1)
		{
			return NetObj.GetByID(this.m_targetID);
		}
		return null;
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0003363C File Offset: 0x0003183C
	public void Save(BinaryWriter writer)
	{
		writer.Write(this.m_targetID);
		writer.Write(this.m_targetPos.x);
		writer.Write(this.m_targetPos.y);
		writer.Write(this.m_targetPos.z);
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00033688 File Offset: 0x00031888
	public void Load(BinaryReader reader)
	{
		this.m_targetID = reader.ReadInt32();
		this.m_targetPos.x = reader.ReadSingle();
		this.m_targetPos.y = reader.ReadSingle();
		this.m_targetPos.z = reader.ReadSingle();
	}

	// Token: 0x040005AE RID: 1454
	private int m_targetID = -1;

	// Token: 0x040005AF RID: 1455
	private Vector3 m_targetPos;
}
