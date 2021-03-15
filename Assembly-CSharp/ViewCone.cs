using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class ViewCone : MonoBehaviour
{
	// Token: 0x060009F4 RID: 2548 RVA: 0x00047AA8 File Offset: 0x00045CA8
	public void Setup(float minDist, float maxDist, float fow)
	{
		if (this.hasSetMeshData)
		{
			return;
		}
		if (fow > 179.9999f)
		{
			fow = 179.9999f;
		}
		Color item = new Color(0f, 1f, 0f, 0.5f);
		List<Vector3> list = new List<Vector3>();
		List<Color> list2 = new List<Color>();
		Vector3 zero = Vector3.zero;
		Vector3 forward = Vector3.forward;
		Vector3 vector = minDist * forward;
		Vector3 start = zero + Quaternion.AngleAxis(-fow, Vector3.up) * vector;
		Vector3 end = zero + Quaternion.AngleAxis(fow, Vector3.up) * vector;
		Vector3 point = maxDist * forward;
		Vector3 end2 = zero + Quaternion.AngleAxis(-fow, Vector3.up) * point;
		Vector3 start2 = zero + Quaternion.AngleAxis(fow, Vector3.up) * point;
		List<Vector3> list3 = new List<Vector3>();
		list3.Clear();
		list3 = ViewCone.AddPointsBetween(start, end2, this.m_detail - 1, true, true);
		list.AddRange(list3);
		for (int i = 0; i < list3.Count; i++)
		{
			list2.Add(item);
		}
		list3.Clear();
		if (this.m_detail > 1)
		{
			for (int j = 1; j < this.m_detail + 1; j++)
			{
				float num = 1f - (float)j / ((float)this.m_detail + 1f);
				Vector3 item2 = zero + Quaternion.AngleAxis(-(fow * num), Vector3.up) * point;
				list3.Add(item2);
			}
		}
		if (this.m_detail > 1)
		{
			for (int k = 1; k < this.m_detail + 1; k++)
			{
				float num2 = (float)k / ((float)this.m_detail + 1f);
				Vector3 item3 = zero + Quaternion.AngleAxis(fow * num2, Vector3.up) * point;
				list3.Add(item3);
			}
		}
		list.AddRange(list3);
		for (int l = 0; l < list3.Count; l++)
		{
			list2.Add(item);
		}
		list3 = ViewCone.AddPointsBetween(start2, end, this.m_detail - 1, true, false);
		list.AddRange(list3);
		for (int m = 0; m < list3.Count; m++)
		{
			list2.Add(item);
		}
		Vector3 point2 = vector;
		List<Vector3> list4 = new List<Vector3>();
		if (this.m_detail > 1)
		{
			for (int n = this.m_detail + 1; n > 1; n--)
			{
				float num3 = (float)n / ((float)this.m_detail + 1f);
				Vector3 item4 = zero + Quaternion.AngleAxis(fow * num3, Vector3.up) * point2;
				list4.Add(item4);
			}
		}
		if (this.m_detail > 1)
		{
			for (int num4 = this.m_detail + 1; num4 > 1; num4--)
			{
				float num5 = 1f - (float)num4 / ((float)this.m_detail + 1f);
				Vector3 item5 = zero + Quaternion.AngleAxis(-(fow * num5), Vector3.up) * point2;
				list4.Add(item5);
			}
		}
		list.AddRange(list4);
		for (int num6 = 0; num6 < list4.Count; num6++)
		{
			list2.Add(item);
		}
		Vector2[] array = new Vector2[list.Count];
		for (int num7 = 0; num7 < list.Count; num7++)
		{
			array[num7] = new Vector2(0f, 0f);
		}
		this.m_mesh = new Mesh();
		this.m_mesh.name = "ViewCone";
		this.m_mesh.vertices = list.ToArray();
		this.m_mesh.colors = list2.ToArray();
		this.m_mesh.uv = array;
		List<Vector2> list5 = new List<Vector2>();
		for (int num8 = 0; num8 < list.Count; num8++)
		{
			Vector3 vector2 = list[num8];
			list5.Add(new Vector2(vector2.x, vector2.z));
		}
		Triangulator triangulator = new Triangulator(list5.ToArray());
		int[] triangles = triangulator.Triangulate();
		this.m_mesh.triangles = triangles;
		this.m_mesh.RecalculateNormals();
		this.m_mesh.RecalculateBounds();
		this.m_mesh.Optimize();
		MeshFilter component = base.GetComponent<MeshFilter>();
		component.mesh = this.m_mesh;
		this.hasSetMeshData = (component != null && component.mesh != null && component.mesh.vertexCount > 2);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00047F78 File Offset: 0x00046178
	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.m_mesh);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00047F88 File Offset: 0x00046188
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

	// Token: 0x04000831 RID: 2097
	private Mesh m_mesh;

	// Token: 0x04000832 RID: 2098
	public bool hasSetMeshData;

	// Token: 0x04000833 RID: 2099
	public int m_detail = 8;
}
