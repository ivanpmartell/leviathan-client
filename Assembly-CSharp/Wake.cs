using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class Wake : MonoBehaviour
{
	// Token: 0x060009FE RID: 2558 RVA: 0x000487AC File Offset: 0x000469AC
	private void Start()
	{
		this.m_lineMesh = new Mesh[]
		{
			new Mesh(),
			new Mesh()
		};
		this.m_lineMesh[0].name = "Trace mesh 0";
		this.m_lineMesh[1].name = "Trace mesh 1";
		if (this.m_maxSpeed <= this.m_minSpeed)
		{
			this.m_maxSpeed = this.m_minSpeed + 0.01f;
		}
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0004881C File Offset: 0x00046A1C
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
		}
		this.m_dirty = true;
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x000488EC File Offset: 0x00046AEC
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

	// Token: 0x06000A01 RID: 2561 RVA: 0x00048954 File Offset: 0x00046B54
	public void Die()
	{
		this.m_die = true;
		base.transform.parent = null;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0004896C File Offset: 0x00046B6C
	public void SetVisible(bool visible)
	{
		this.m_visible = visible;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00048978 File Offset: 0x00046B78
	private void UpdateMesh()
	{
		this.m_currentMesh = ((this.m_currentMesh != 0) ? 0 : 1);
		float num = this.m_width / 2f;
		float num2 = (float)(-(float)this.m_points.Count) * this.m_uvScale;
		float particleLength = this.m_particleLength;
		for (int i = 0; i < this.m_points.Count; i++)
		{
			Vector3 pos = this.m_points[i].pos;
			Vector3 dir = this.m_points[i].dir;
			float num3 = Mathf.Max(0f, this.m_time - this.m_points[i].time);
			float alpha = this.m_points[i].alpha;
			float num4 = 1f - num3 / this.m_ttl;
			num4 = Mathf.Pow(num4, this.m_scalePower);
			num4 = 1f - num4;
			float d = num * num4;
			Vector3 b = Vector3.Cross(dir, Vector3.up) * d;
			this.m_vertises.Add(pos - b - dir * particleLength);
			this.m_vertises.Add(pos + b - dir * particleLength);
			this.m_vertises.Add(pos - b);
			this.m_vertises.Add(pos + b);
			this.m_uvs.Add(new Vector2(0f, 0f));
			this.m_uvs.Add(new Vector2(0f, 1f));
			this.m_uvs.Add(new Vector2(1f, 0f));
			this.m_uvs.Add(new Vector2(1f, 1f));
			float a = num3 / this.m_fadeIn;
			float b2 = (this.m_ttl - num3) / this.m_ttl;
			float a2 = Mathf.Min(a, b2) * alpha;
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			this.m_colors.Add(new Color(1f, 1f, 1f, a2));
			if (i > 0)
			{
				num2 += this.m_uvScale;
				int num5 = this.m_vertises.Count - 4;
				this.m_indices.Add(num5);
				this.m_indices.Add(num5 + 1);
				this.m_indices.Add(num5 + 2);
				this.m_indices.Add(num5 + 2);
				this.m_indices.Add(num5 + 1);
				this.m_indices.Add(num5 + 3);
			}
		}
		Mesh mesh = this.m_lineMesh[this.m_currentMesh];
		mesh.Clear();
		mesh.vertices = this.m_vertises.ToArray();
		mesh.uv = this.m_uvs.ToArray();
		mesh.colors = this.m_colors.ToArray();
		mesh.triangles = this.m_indices.ToArray();
		Vector3 center = (this.m_points[0].pos + this.m_points[this.m_points.Count - 1].pos) * 0.5f;
		float num6 = Vector3.Distance(this.m_points[0].pos, this.m_points[this.m_points.Count - 1].pos);
		mesh.bounds = new Bounds(center, new Vector3(num6, num6, num6));
		this.m_vertises.Clear();
		this.m_indices.Clear();
		this.m_uvs.Clear();
		this.m_colors.Clear();
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00048DB4 File Offset: 0x00046FB4
	private void AddPoint(Vector3 point)
	{
		if (this.m_points.Count == 0)
		{
			Wake.Point item = default(Wake.Point);
			item.pos = point;
			item.time = this.m_time;
			item.alpha = 0f;
			this.m_points.Add(item);
		}
		else
		{
			float num = Vector3.Distance(point, this.m_points[this.m_points.Count - 1].pos);
			if (num > this.m_sectionLength)
			{
				Wake.Point item2 = default(Wake.Point);
				item2.pos = point;
				item2.dir = base.transform.forward;
				item2.time = this.m_time;
				item2.alpha = 1f;
				if (this.m_minSpeed >= 0f)
				{
					float num2 = this.m_time - this.m_points[this.m_points.Count - 1].time;
					Vector3 rhs = point - this.m_points[this.m_points.Count - 1].pos;
					float num3 = Vector3.Dot(base.transform.forward, rhs) / num2;
					if (this.m_canReverse)
					{
						num3 = Mathf.Abs(num3);
					}
					float num4 = (num3 - this.m_minSpeed) / (this.m_maxSpeed - this.m_minSpeed);
					num4 = Mathf.Clamp(num4, 0f, 1f);
					item2.alpha = num4;
				}
				this.m_points.Add(item2);
			}
		}
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00048F48 File Offset: 0x00047148
	public void Save(BinaryWriter writer)
	{
		writer.Write(this.m_time);
		writer.Write((short)this.m_points.Count);
		foreach (Wake.Point point in this.m_points)
		{
			writer.Write(point.pos.x);
			writer.Write(point.pos.y);
			writer.Write(point.pos.z);
			writer.Write(point.dir.x);
			writer.Write(point.dir.z);
			writer.Write(point.time);
			writer.Write((byte)(point.alpha * 255f));
		}
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00049040 File Offset: 0x00047240
	public void Load(BinaryReader reader)
	{
		this.m_time = reader.ReadSingle();
		int num = (int)reader.ReadInt16();
		this.m_points.Clear();
		for (int i = 0; i < num; i++)
		{
			Wake.Point item = default(Wake.Point);
			item.pos.x = reader.ReadSingle();
			item.pos.y = reader.ReadSingle();
			item.pos.z = reader.ReadSingle();
			item.dir.x = reader.ReadSingle();
			item.dir.y = 0f;
			item.dir.z = reader.ReadSingle();
			item.time = reader.ReadSingle();
			item.alpha = (float)reader.ReadByte() / 255f;
			this.m_points.Add(item);
		}
		this.m_dirty = true;
	}

	// Token: 0x04000838 RID: 2104
	public Material m_material;

	// Token: 0x04000839 RID: 2105
	public float m_width = 5f;

	// Token: 0x0400083A RID: 2106
	public float m_ttl = 3f;

	// Token: 0x0400083B RID: 2107
	public float m_fadeIn = 1f;

	// Token: 0x0400083C RID: 2108
	public float m_sectionLength = 4f;

	// Token: 0x0400083D RID: 2109
	public float m_particleLength = 4f;

	// Token: 0x0400083E RID: 2110
	public float m_uvScale = 0.1f;

	// Token: 0x0400083F RID: 2111
	public float m_minSpeed = -1f;

	// Token: 0x04000840 RID: 2112
	public float m_maxSpeed = -1f;

	// Token: 0x04000841 RID: 2113
	public float m_scalePower = 2f;

	// Token: 0x04000842 RID: 2114
	public bool m_canReverse;

	// Token: 0x04000843 RID: 2115
	private Mesh[] m_lineMesh;

	// Token: 0x04000844 RID: 2116
	private int m_currentMesh;

	// Token: 0x04000845 RID: 2117
	private bool m_dirty = true;

	// Token: 0x04000846 RID: 2118
	private bool m_die;

	// Token: 0x04000847 RID: 2119
	private bool m_visible = true;

	// Token: 0x04000848 RID: 2120
	private float m_time;

	// Token: 0x04000849 RID: 2121
	private List<Vector3> m_vertises = new List<Vector3>();

	// Token: 0x0400084A RID: 2122
	private List<int> m_indices = new List<int>();

	// Token: 0x0400084B RID: 2123
	private List<Vector2> m_uvs = new List<Vector2>();

	// Token: 0x0400084C RID: 2124
	private List<Color> m_colors = new List<Color>();

	// Token: 0x0400084D RID: 2125
	private List<Wake.Point> m_points = new List<Wake.Point>();

	// Token: 0x0200010F RID: 271
	private struct Point
	{
		// Token: 0x0400084E RID: 2126
		public Vector3 pos;

		// Token: 0x0400084F RID: 2127
		public Vector3 dir;

		// Token: 0x04000850 RID: 2128
		public float time;

		// Token: 0x04000851 RID: 2129
		public float alpha;
	}
}
