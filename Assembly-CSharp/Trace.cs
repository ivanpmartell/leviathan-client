using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000106 RID: 262
public class Trace : MonoBehaviour
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x00047144 File Offset: 0x00045344
	private void Start()
	{
		this.m_lineMesh = new Mesh[]
		{
			new Mesh(),
			new Mesh()
		};
		this.m_lineMesh[0].name = "Trace mesh 0";
		this.m_lineMesh[1].name = "Trace mesh 1";
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00047194 File Offset: 0x00045394
	private void FixedUpdate()
	{
		if (!this.m_die && !NetObj.IsSimulating())
		{
			return;
		}
		this.m_time += Time.fixedDeltaTime;
		if (!this.m_die)
		{
			this.AddPoint(base.transform.position);
		}
		if (this.m_die && (this.m_points.Count <= 2 || !this.m_visible))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (this.m_points.Count > 1 && this.m_time - this.m_points[1].time > this.m_ttl)
		{
			this.m_points.RemoveAt(0);
			this.m_dirty = true;
		}
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00047264 File Offset: 0x00045464
	private void Update()
	{
		if (!this.m_visible)
		{
			return;
		}
		if (this.m_points.Count < 2)
		{
			return;
		}
		if (this.m_dirty)
		{
			this.m_dirty = false;
			this.UpdateMesh();
		}
		Graphics.DrawMesh(this.m_lineMesh[this.m_currentMesh], Matrix4x4.identity, this.m_material, 0, null, 0, null, false, false);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x000472CC File Offset: 0x000454CC
	public void Die()
	{
		this.m_die = true;
		base.transform.parent = null;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x000472E4 File Offset: 0x000454E4
	public void SetVisible(bool visible)
	{
		this.m_visible = visible;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x000472F0 File Offset: 0x000454F0
	private void UpdateMesh()
	{
		this.m_currentMesh = ((this.m_currentMesh != 0) ? 0 : 1);
		float d = this.m_width / 2f;
		float num = (float)(-(float)this.m_points.Count) * this.m_uvScale;
		for (int i = 0; i < this.m_points.Count; i++)
		{
			Trace.Point? point = new Trace.Point?(this.m_points[i]);
			Vector3 pos = point.Value.pos;
			Vector3 a = Vector3.zero;
			Vector3 lhs;
			if (i == this.m_points.Count - 1)
			{
				Vector3 pos2 = this.m_points[i - 1].pos;
				lhs = pos - pos2;
			}
			else
			{
				a = this.m_points[i + 1].pos;
				lhs = a - pos;
			}
			if (lhs.magnitude < 0.001f)
			{
				lhs.Set(1f, 0f, 0f);
			}
			lhs.Normalize();
			float num2 = Mathf.Max(0f, this.m_time - point.Value.time);
			Vector3 b = Vector3.Cross(lhs, Vector3.up) * d;
			this.m_vertises.Add(pos - b);
			this.m_vertises.Add(pos + b);
			float time = point.Value.time;
			this.m_uvs.Add(new Vector2(time, 0f));
			this.m_uvs.Add(new Vector2(time, 1f));
			float a2 = (this.m_ttl - num2) / this.m_ttl;
			if (i == this.m_points.Count - 1)
			{
				a2 = 0f;
			}
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			if (i < this.m_points.Count - 1)
			{
				num += this.m_uvScale;
				int num3 = this.m_vertises.Count - 2;
				this.m_indices.Add(num3);
				this.m_indices.Add(num3 + 1);
				this.m_indices.Add(num3 + 2);
				this.m_indices.Add(num3 + 2);
				this.m_indices.Add(num3 + 1);
				this.m_indices.Add(num3 + 3);
			}
		}
		Mesh mesh = this.m_lineMesh[this.m_currentMesh];
		mesh.Clear();
		mesh.vertices = this.m_vertises.ToArray();
		mesh.uv = this.m_uvs.ToArray();
		mesh.colors = this.m_colors.ToArray();
		mesh.triangles = this.m_indices.ToArray();
		Vector3 center = (this.m_points[0].pos + this.m_points[this.m_points.Count - 1].pos) * 0.5f;
		float num4 = Vector3.Distance(this.m_points[0].pos, this.m_points[this.m_points.Count - 1].pos);
		mesh.bounds = new Bounds(center, new Vector3(num4, num4, num4));
		this.m_vertises.Clear();
		this.m_indices.Clear();
		this.m_uvs.Clear();
		this.m_colors.Clear();
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x000476C4 File Offset: 0x000458C4
	private void AddPoint(Vector3 point)
	{
		if (this.m_points.Count == 0)
		{
			Trace.Point item = default(Trace.Point);
			item.pos = point;
			item.time = this.m_time;
			this.m_points.Add(item);
			this.m_dirty = true;
		}
		else
		{
			float num = Vector3.Distance(point, this.m_points[this.m_points.Count - 1].pos);
			if (num > this.m_sectionLength)
			{
				Trace.Point item2 = default(Trace.Point);
				item2.pos = point;
				item2.time = this.m_time;
				this.m_points.Add(item2);
				this.m_dirty = true;
			}
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x0004777C File Offset: 0x0004597C
	public void Save(BinaryWriter writer)
	{
		writer.Write(this.m_time);
		writer.Write((short)this.m_points.Count);
		foreach (Trace.Point point in this.m_points)
		{
			writer.Write(point.pos.x);
			writer.Write(point.pos.y);
			writer.Write(point.pos.z);
			writer.Write(point.time);
		}
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x0004783C File Offset: 0x00045A3C
	public void Load(BinaryReader reader)
	{
		this.m_time = reader.ReadSingle();
		int num = (int)reader.ReadInt16();
		this.m_points.Clear();
		for (int i = 0; i < num; i++)
		{
			Trace.Point item = default(Trace.Point);
			item.pos.x = reader.ReadSingle();
			item.pos.y = reader.ReadSingle();
			item.pos.z = reader.ReadSingle();
			item.time = reader.ReadSingle();
			this.m_points.Add(item);
		}
		this.m_dirty = true;
	}

	// Token: 0x04000818 RID: 2072
	public Material m_material;

	// Token: 0x04000819 RID: 2073
	public float m_width = 5f;

	// Token: 0x0400081A RID: 2074
	public float m_ttl = 3f;

	// Token: 0x0400081B RID: 2075
	public float m_sectionLength = 4f;

	// Token: 0x0400081C RID: 2076
	public float m_uvScale = 0.1f;

	// Token: 0x0400081D RID: 2077
	private Mesh[] m_lineMesh;

	// Token: 0x0400081E RID: 2078
	private int m_currentMesh;

	// Token: 0x0400081F RID: 2079
	private bool m_dirty = true;

	// Token: 0x04000820 RID: 2080
	private bool m_die;

	// Token: 0x04000821 RID: 2081
	private bool m_visible = true;

	// Token: 0x04000822 RID: 2082
	private float m_time;

	// Token: 0x04000823 RID: 2083
	private List<Vector3> m_vertises = new List<Vector3>();

	// Token: 0x04000824 RID: 2084
	private List<int> m_indices = new List<int>();

	// Token: 0x04000825 RID: 2085
	private List<Vector2> m_uvs = new List<Vector2>();

	// Token: 0x04000826 RID: 2086
	private List<Color> m_colors = new List<Color>();

	// Token: 0x04000827 RID: 2087
	private List<Trace.Point> m_points = new List<Trace.Point>();

	// Token: 0x02000107 RID: 263
	private struct Point
	{
		// Token: 0x04000828 RID: 2088
		public Vector3 pos;

		// Token: 0x04000829 RID: 2089
		public float time;
	}
}
