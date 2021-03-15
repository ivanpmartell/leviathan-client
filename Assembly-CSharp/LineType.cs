using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DE RID: 222
[Serializable]
public class LineType
{
	// Token: 0x0600088C RID: 2188 RVA: 0x0003EF34 File Offset: 0x0003D134
	public void Draw(float dt)
	{
		this.m_uvOffset -= this.m_uvScroll * dt;
		if (this.m_indices.Count == 0)
		{
			return;
		}
		if (this.m_lineMesh == null)
		{
			this.m_lineMesh = new Mesh[]
			{
				new Mesh(),
				new Mesh()
			};
			this.m_lineMesh[0].name = "Line mesh 0:" + this.m_name;
			this.m_lineMesh[1].name = "Line mesh 1:" + this.m_name;
		}
		Mesh mesh = this.m_lineMesh[this.m_currentMesh];
		mesh.Clear();
		mesh.vertices = this.m_vertises.ToArray();
		mesh.uv = this.m_uvs.ToArray();
		mesh.triangles = this.m_indices.ToArray();
		Graphics.DrawMesh(mesh, Matrix4x4.identity, this.m_material, 0, null, 0, null, false, false);
		this.m_vertises.Clear();
		this.m_indices.Clear();
		this.m_uvs.Clear();
		this.m_currentMesh = ((this.m_currentMesh != 0) ? 0 : 1);
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0003F060 File Offset: 0x0003D260
	public void DrawLine(float scale, Vector3 camPos, Vector3 start, Vector3 end, float width)
	{
		if (start == end)
		{
			return;
		}
		Vector3 normalized = (end - start).normalized;
		Vector3 a = Vector3.Cross(normalized, Vector3.up) * (width / 2f);
		int count = this.m_vertises.Count;
		this.m_indices.Add(count);
		this.m_indices.Add(count + 1);
		this.m_indices.Add(count + 2);
		this.m_indices.Add(count + 2);
		this.m_indices.Add(count + 1);
		this.m_indices.Add(count + 3);
		float d = 1f;
		float d2 = 1f;
		if (this.m_distanceScale)
		{
			d = 0.05f * scale * Vector3.Distance(start, camPos);
			d2 = 0.05f * scale * Vector3.Distance(end, camPos);
		}
		this.m_vertises.Add(start - a * d);
		this.m_vertises.Add(start + a * d);
		this.m_vertises.Add(end - a * d2);
		this.m_vertises.Add(end + a * d2);
		float num = this.m_uvScale * Vector3.Distance(start, end);
		this.m_uvs.Add(new Vector2(this.m_uvOffset, 0f));
		this.m_uvs.Add(new Vector2(this.m_uvOffset, 1f));
		this.m_uvs.Add(new Vector2(this.m_uvOffset + num, 0f));
		this.m_uvs.Add(new Vector2(this.m_uvOffset + num, 1f));
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0003F228 File Offset: 0x0003D428
	public void DrawLine(float scale, Vector3 camPos, List<Vector3> points, float width)
	{
		float num = this.m_uvOffset;
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 vector = points[i];
			float num2 = 1f;
			if (this.m_distanceScale)
			{
				num2 = 0.05f * scale * Vector3.Distance(vector, camPos);
			}
			Vector3 vector2 = Vector3.zero;
			Vector3 a;
			if (i == points.Count - 1)
			{
				Vector3 b = points[i - 1];
				Vector3 normalized = (vector - b).normalized;
				a = Vector3.Cross(normalized, Vector3.up) * (width / 2f);
			}
			else
			{
				vector2 = points[i + 1];
				Vector3 normalized2 = (vector2 - vector).normalized;
				a = Vector3.Cross(normalized2, Vector3.up) * (width / 2f);
			}
			this.m_vertises.Add(vector - a * num2);
			this.m_vertises.Add(vector + a * num2);
			this.m_uvs.Add(new Vector2(num, 0f));
			this.m_uvs.Add(new Vector2(num, 1f));
			if (i < points.Count - 1)
			{
				num += this.m_uvScale * Vector3.Distance(vector, vector2) * (1f / num2);
				int num3 = this.m_vertises.Count - 2;
				this.m_indices.Add(num3);
				this.m_indices.Add(num3 + 1);
				this.m_indices.Add(num3 + 2);
				this.m_indices.Add(num3 + 2);
				this.m_indices.Add(num3 + 1);
				this.m_indices.Add(num3 + 3);
			}
		}
	}

	// Token: 0x040006F9 RID: 1785
	public string m_name = "Give Me A Name";

	// Token: 0x040006FA RID: 1786
	public Material m_material;

	// Token: 0x040006FB RID: 1787
	public float m_uvScale = 1f;

	// Token: 0x040006FC RID: 1788
	public float m_uvScroll;

	// Token: 0x040006FD RID: 1789
	public bool m_distanceScale = true;

	// Token: 0x040006FE RID: 1790
	private float m_uvOffset;

	// Token: 0x040006FF RID: 1791
	private Mesh[] m_lineMesh;

	// Token: 0x04000700 RID: 1792
	private int m_currentMesh;

	// Token: 0x04000701 RID: 1793
	private List<Vector3> m_vertises = new List<Vector3>();

	// Token: 0x04000702 RID: 1794
	private List<int> m_indices = new List<int>();

	// Token: 0x04000703 RID: 1795
	private List<Vector2> m_uvs = new List<Vector2>();
}
