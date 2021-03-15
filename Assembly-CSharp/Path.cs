using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000111 RID: 273
public class Path : NetObj
{
	// Token: 0x06000A15 RID: 2581 RVA: 0x00049D7C File Offset: 0x00047F7C
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00049D84 File Offset: 0x00047F84
	public List<GameObject> GetNodes()
	{
		List<GameObject> list = new List<GameObject>();
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < base.gameObject.transform.GetChildCount(); i++)
		{
			GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
			try
			{
				hashSet.Add(Convert.ToInt32(gameObject.name));
			}
			catch (Exception)
			{
			}
		}
		List<int> list2 = new List<int>();
		foreach (int item in hashSet)
		{
			list2.Add(item);
		}
		list2.Sort();
		foreach (int value in list2)
		{
			list.Add(base.gameObject.transform.FindChild(Convert.ToString(value)).gameObject);
		}
		return list;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00049EE4 File Offset: 0x000480E4
	private Vector3[] TransferNodes()
	{
		List<GameObject> nodes = this.GetNodes();
		if (nodes.Count == 0)
		{
			return null;
		}
		Vector3[] array = new Vector3[nodes.Count];
		for (int i = 0; i < nodes.Count; i++)
		{
			array[i] = nodes[i].transform.position;
		}
		return array;
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00049F48 File Offset: 0x00048148
	public void OnDrawGizmos()
	{
		Vector3[] array = this.TransferNodes();
		if (array == null)
		{
			return;
		}
		if (array.Length == 0)
		{
			return;
		}
		Vector3 to = array[0];
		foreach (Vector3 vector in array)
		{
			Gizmos.DrawSphere(vector, 1f);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(vector, to);
			to = vector;
		}
		to = array[0];
		foreach (Vector3 vector2 in array)
		{
			Gizmos.DrawLine(vector2, to);
			to = vector2;
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0004A004 File Offset: 0x00048204
	private void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
		}
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0004A010 File Offset: 0x00048210
	public override void SaveState(BinaryWriter writer)
	{
		Vector3[] array = this.TransferNodes();
		if (array != null)
		{
			this.m_nodes = array;
		}
		base.SaveState(writer);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(this.m_nodes.Length);
		foreach (Vector3 vector in this.m_nodes)
		{
			writer.Write(vector.x);
			writer.Write(vector.y);
			writer.Write(vector.z);
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x0004A0E4 File Offset: 0x000482E4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		Vector3 position = default(Vector3);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		base.transform.position = position;
		int num = reader.ReadInt32();
		this.m_nodes = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			this.m_nodes[i].x = reader.ReadSingle();
			this.m_nodes[i].y = reader.ReadSingle();
			this.m_nodes[i].z = reader.ReadSingle();
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0004A1A0 File Offset: 0x000483A0
	public Vector3[] GetPoints()
	{
		return this.m_nodes;
	}

	// Token: 0x04000864 RID: 2148
	private Vector3[] m_nodes = new Vector3[0];
}
