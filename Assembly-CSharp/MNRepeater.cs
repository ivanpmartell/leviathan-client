using System;
using System.IO;
using UnityEngine;

// Token: 0x0200007E RID: 126
[AddComponentMenu("Scripts/Mission/MNRepeater")]
public class MNRepeater : MNode
{
	// Token: 0x0600053B RID: 1339 RVA: 0x0002ACD0 File Offset: 0x00028ED0
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0002ACD8 File Offset: 0x00028ED8
	protected virtual void Destroy()
	{
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0002ACDC File Offset: 0x00028EDC
	protected virtual void Update()
	{
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0002ACE0 File Offset: 0x00028EE0
	public virtual void OnDrawGizmos()
	{
		for (int i = 0; i < this.m_repeatTargets.Length; i++)
		{
			GameObject targetObj = this.GetTargetObj(i);
			if (targetObj != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(base.GetComponent<Transform>().position, targetObj.GetComponent<Transform>().position);
			}
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0002AD40 File Offset: 0x00028F40
	public override void DoAction()
	{
		for (int i = 0; i < this.m_repeatTargets.Length; i++)
		{
			GameObject targetObj = this.GetTargetObj(i);
			if (targetObj != null)
			{
				targetObj.GetComponent<MNode>().DoAction();
			}
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0002AD88 File Offset: 0x00028F88
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		for (int i = 0; i < this.m_repeatTargets.Length; i++)
		{
			this.GetTargetObj(i);
		}
		writer.Write(this.m_repeatTargets.Length);
		foreach (GameObject gameObject in this.m_repeatTargets)
		{
			if (gameObject == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(gameObject.GetComponent<NetObj>().GetNetID());
			}
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0002AE14 File Offset: 0x00029014
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		int num = reader.ReadInt32();
		this.m_targetNetID = new int[num];
		this.m_repeatTargets = new GameObject[num];
		for (int i = 0; i < num; i++)
		{
			this.m_targetNetID[i] = reader.ReadInt32();
		}
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0002AE68 File Offset: 0x00029068
	public GameObject GetTargetObj(int index)
	{
		if (this.m_repeatTargets[index] != null)
		{
			return this.m_repeatTargets[index];
		}
		if (index >= this.m_targetNetID.Length)
		{
			return null;
		}
		if (this.m_targetNetID[index] == 0)
		{
			return null;
		}
		this.m_repeatTargets[index] = NetObj.GetByID(this.m_targetNetID[index]).gameObject;
		return this.m_repeatTargets[index];
	}

	// Token: 0x04000445 RID: 1093
	public int[] m_targetNetID = new int[0];

	// Token: 0x04000446 RID: 1094
	public GameObject[] m_repeatTargets = new GameObject[0];
}
