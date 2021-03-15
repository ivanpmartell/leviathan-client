using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000079 RID: 121
[AddComponentMenu("Scripts/Mission/MNMarker")]
public class MNMarker : MNode
{
	// Token: 0x06000522 RID: 1314 RVA: 0x0002A580 File Offset: 0x00028780
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0002A588 File Offset: 0x00028788
	private void FixedUpdate()
	{
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0002A58C File Offset: 0x0002878C
	public override void DoAction()
	{
		GameObject marker = this.GetMarker();
		if (marker == null)
		{
			return;
		}
		List<GameObject> targets = MNode.GetTargets(marker);
		for (int i = 0; i < targets.Count; i++)
		{
			GameObject gameObject = targets[i];
			Marker component = gameObject.GetComponent<Marker>();
			if (component != null)
			{
				component.SetVisible(this.m_show);
			}
			Unit component2 = gameObject.GetComponent<Unit>();
			if (component2 != null)
			{
				component2.SetObjective(this.m_objectiveType);
			}
			MNSpawn component3 = gameObject.GetComponent<MNSpawn>();
			if (component3 != null)
			{
				component3.SetObjective(this.m_objectiveType);
			}
		}
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0002A63C File Offset: 0x0002883C
	public void OnDrawGizmos()
	{
		if (this.GetMarker() != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.GetComponent<Transform>().position, this.GetMarker().GetComponent<Transform>().position);
		}
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0002A684 File Offset: 0x00028884
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		if (this.GetMarker() == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(this.GetMarker().GetComponent<NetObj>().GetNetID());
		}
		writer.Write(this.m_show);
		writer.Write((int)this.m_objectiveType);
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0002A6E4 File Offset: 0x000288E4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_markerNetID = reader.ReadInt32();
		this.m_show = reader.ReadBoolean();
		this.m_objectiveType = (Unit.ObjectiveTypes)reader.ReadInt32();
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0002A71C File Offset: 0x0002891C
	public GameObject GetMarker()
	{
		if (this.m_marker != null)
		{
			return this.m_marker;
		}
		if (this.m_markerNetID == 0)
		{
			return null;
		}
		this.m_marker = NetObj.GetByID(this.m_markerNetID).gameObject;
		return this.m_marker;
	}

	// Token: 0x04000434 RID: 1076
	public GameObject m_marker;

	// Token: 0x04000435 RID: 1077
	public bool m_show;

	// Token: 0x04000436 RID: 1078
	public Unit.ObjectiveTypes m_objectiveType = Unit.ObjectiveTypes.Move;

	// Token: 0x04000437 RID: 1079
	private int m_markerNetID;
}
