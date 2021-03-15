using System;
using System.IO;
using UnityEngine;

// Token: 0x0200008C RID: 140
[AddComponentMenu("Scripts/Mission/MNTrigger")]
public class MNTrigger : MNode
{
	// Token: 0x06000576 RID: 1398 RVA: 0x0002BF60 File Offset: 0x0002A160
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0002BF68 File Offset: 0x0002A168
	public override void DoAction()
	{
		this.m_disabled = !this.m_disabled;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0002BF7C File Offset: 0x0002A17C
	public virtual void OnDrawGizmos()
	{
		if (this.GetTargetObj() != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.GetComponent<Transform>().position, this.GetTargetObj().GetComponent<Transform>().position);
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0002BFC4 File Offset: 0x0002A1C4
	public virtual void Trigger()
	{
		if (this.m_disabled)
		{
			return;
		}
		if (this.m_oneShot)
		{
			this.m_disabled = true;
		}
		if (this.GetTargetObj() != null)
		{
			this.GetTargetObj().GetComponent<MNode>().DoAction();
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0002C010 File Offset: 0x0002A210
	public override void SaveState(BinaryWriter writer)
	{
		this.GetTargetObj();
		base.SaveState(writer);
		writer.Write(this.m_oneShot);
		writer.Write(this.m_disabled);
		if (this.m_target == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(this.m_target.GetComponent<NetObj>().GetNetID());
		}
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0002C078 File Offset: 0x0002A278
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_oneShot = reader.ReadBoolean();
		this.m_disabled = reader.ReadBoolean();
		this.m_targetNetID = reader.ReadInt32();
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0002C0B0 File Offset: 0x0002A2B0
	public GameObject GetTargetObj()
	{
		if (this.m_target != null)
		{
			return this.m_target;
		}
		if (this.m_targetNetID == 0)
		{
			return null;
		}
		this.m_target = NetObj.GetByID(this.m_targetNetID).gameObject;
		return this.m_target;
	}

	// Token: 0x04000496 RID: 1174
	private int m_targetNetID;

	// Token: 0x04000497 RID: 1175
	public GameObject m_target;

	// Token: 0x04000498 RID: 1176
	public bool m_oneShot = true;

	// Token: 0x04000499 RID: 1177
	public bool m_disabled;
}
