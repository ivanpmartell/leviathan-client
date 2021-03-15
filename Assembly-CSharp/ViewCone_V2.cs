using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class ViewCone_V2 : MonoBehaviour
{
	// Token: 0x060009F8 RID: 2552 RVA: 0x00048040 File Offset: 0x00046240
	public void Setup(float minDist, float maxDist, float fow, int detail)
	{
		if (this.hasSetMeshData)
		{
			return;
		}
		Mesh mesh = new Mesh();
		Color item = new Color(1f, 0f, 0f, 0.3f);
		Color item2 = new Color(0f, 1f, 0f, 0.25f);
		List<Vector3> list = new List<Vector3>();
		List<Color> list2 = new List<Color>();
		Vector3 zero = Vector3.zero;
		Vector3 a = base.transform.localRotation * Vector3.forward;
		Vector3 vector = minDist * a;
		Vector3 vector2 = zero + Quaternion.AngleAxis(-fow, Vector3.up) * vector;
		Vector3 vector3 = zero + vector;
		Vector3 vector4 = zero + Quaternion.AngleAxis(fow, Vector3.up) * vector;
		Vector3 vector5 = maxDist * a;
		Vector3 end = vector3 + Quaternion.AngleAxis(-fow, Vector3.up) * vector5;
		Vector3 item3 = vector3 + vector5;
		Vector3 start = vector3 + Quaternion.AngleAxis(fow, Vector3.up) * vector5;
		List<Vector3> list3 = new List<Vector3>();
		list3 = ViewCone_V2.AddPointsBetween(zero, vector2, detail - 1, true, true);
		list.AddRange(list3);
		for (int i = 0; i < list3.Count; i++)
		{
			list2.Add(item);
		}
		list3.Clear();
		if (detail > 1)
		{
			for (int j = 1; j < detail + 1; j++)
			{
				float num = 1f - (float)j / ((float)detail + 1f);
				Vector3 item4 = zero + Quaternion.AngleAxis(-(fow * num), Vector3.up) * vector;
				list3.Add(item4);
			}
		}
		list.AddRange(list3);
		for (int k = 0; k < list3.Count; k++)
		{
			list2.Add(item);
		}
		list.Add(vector3);
		list2.Add(item);
		list3.Clear();
		if (detail > 1)
		{
			for (int l = 1; l < detail + 1; l++)
			{
				float num2 = (float)l / ((float)detail + 1f);
				Vector3 item5 = zero + Quaternion.AngleAxis(fow * num2, Vector3.up) * vector;
				list3.Add(item5);
			}
		}
		list.AddRange(list3);
		for (int m = 0; m < list3.Count; m++)
		{
			list2.Add(item);
		}
		list3 = ViewCone_V2.AddPointsBetween(vector4, zero, detail - 1, true, false);
		list.AddRange(list3);
		for (int n = 0; n < list3.Count; n++)
		{
			list2.Add(item);
		}
		list3 = ViewCone_V2.AddPointsBetween(vector2, end, detail - 1, false, true);
		list.AddRange(list3);
		for (int num3 = 0; num3 < list3.Count; num3++)
		{
			list2.Add(item2);
		}
		list3.Clear();
		if (detail > 1)
		{
			for (int num4 = 1; num4 < detail + 1; num4++)
			{
				float num5 = (float)num4 / ((float)detail + 1f);
				Vector3 item6 = zero + Quaternion.AngleAxis(fow * num5, Vector3.up) * vector5;
				list3.Add(item6);
			}
		}
		list.AddRange(list3);
		list2.Add(item2);
		for (int num6 = 0; num6 < list3.Count; num6++)
		{
			list2.Add(item2);
		}
		list.Add(item3);
		list3.Clear();
		if (detail > 1)
		{
			for (int num7 = 1; num7 < detail + 1; num7++)
			{
				float num8 = 1f - (float)num7 / ((float)detail + 1f);
				Vector3 item7 = zero + Quaternion.AngleAxis(-(fow * num8), Vector3.up) * vector5;
				list3.Add(item7);
			}
		}
		list.AddRange(list3);
		for (int num9 = 0; num9 < list3.Count; num9++)
		{
			list2.Add(item2);
		}
		list3 = ViewCone_V2.AddPointsBetween(start, vector4, detail - 1, false, false);
		list.AddRange(list3);
		for (int num10 = 0; num10 < list3.Count; num10++)
		{
			list2.Add(item2);
		}
		mesh.vertices = list.ToArray();
		mesh.colors = list2.ToArray();
		List<Vector2> list4 = new List<Vector2>();
		for (int num11 = 0; num11 < list.Count; num11++)
		{
			Vector3 vector6 = list[num11];
			list4.Add(new Vector2(vector6.x, vector6.z));
		}
		Triangulator triangulator = new Triangulator(list4.ToArray());
		int[] triangles = triangulator.Triangulate();
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		this.meshFilter = base.GetComponent<MeshFilter>();
		if (this.meshFilter == null)
		{
			this.meshFilter = (base.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter);
		}
		this.meshFilter.mesh = mesh;
		this.hasSetMeshData = (this.meshFilter != null && this.meshFilter.mesh != null && this.meshFilter.mesh.vertexCount > 2);
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x000485C8 File Offset: 0x000467C8
	private static List<Vector3> AddPointsBetween(Vector3 start, Vector3 end, int numNew, bool includeStart, bool includeEnd)
	{
		List<Vector3> list = new List<Vector3>();
		if (numNew == 0)
		{
			if (includeStart)
			{
				list.Add(start);
			}
			if (includeEnd)
			{
				list.Add(end);
			}
			return list;
		}
		if (includeStart)
		{
			list.Add(start);
		}
		Vector3 vector = end - start;
		Vector3 normalized = vector.normalized;
		normalized.y = 0f;
		float num = vector.magnitude / (float)(numNew + 1);
		for (int i = 1; i < numNew + 1; i++)
		{
			Vector3 item = start + normalized * (num * (float)i);
			list.Add(item);
		}
		if (includeEnd)
		{
			list.Add(end);
		}
		return list;
	}

	// Token: 0x04000834 RID: 2100
	private MeshFilter meshFilter;

	// Token: 0x04000835 RID: 2101
	public bool hasSetMeshData;
}
