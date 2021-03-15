using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class LineDrawer : MonoBehaviour
{
	// Token: 0x06000890 RID: 2192 RVA: 0x0003F400 File Offset: 0x0003D600
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		foreach (LineType lineType in this.m_lineTypes)
		{
			lineType.Draw(deltaTime);
		}
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0003F43C File Offset: 0x0003D63C
	public void DrawLine(Vector3 start, Vector3 end, int type)
	{
		this.DrawLine(start, end, type, 1f);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0003F44C File Offset: 0x0003D64C
	public void DrawLine(Vector3 start, Vector3 end, int type, float width)
	{
		float scale = Mathf.Tan(0.017453292f * base.camera.fieldOfView * 0.5f);
		LineType lineType = this.GetLineType(type);
		if (lineType != null)
		{
			lineType.DrawLine(scale, base.transform.position, start, end, width);
		}
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0003F49C File Offset: 0x0003D69C
	public void DrawCurvedLine(Vector3 start, Vector3 end, Vector3 offset, int type, float width, int sections)
	{
		LineType lineType = this.GetLineType(type);
		if (lineType == null)
		{
			return;
		}
		float scale = Mathf.Tan(0.017453292f * base.camera.fieldOfView * 0.5f);
		Vector3 a = (end - start) / (float)sections;
		Vector3 start2 = start;
		for (int i = 0; i <= sections; i++)
		{
			float f = (float)i / (float)sections * 3.1415927f;
			Vector3 vector = start + (float)i * a + Mathf.Sin(f) * offset;
			lineType.DrawLine(scale, base.transform.position, start2, vector, width);
			start2 = vector;
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0003F54C File Offset: 0x0003D74C
	public void DrawLine(List<Vector3> points, int type, float width)
	{
		if (points.Count < 2)
		{
			return;
		}
		float scale = Mathf.Tan(0.017453292f * base.camera.fieldOfView * 0.5f);
		LineType lineType = this.GetLineType(type);
		if (lineType != null)
		{
			lineType.DrawLine(scale, base.transform.position, points, width);
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0003F5A8 File Offset: 0x0003D7A8
	public void DrawXZCircle(Vector3 center, float radius, int sections, int type, float lineWidth)
	{
		float scale = Mathf.Tan(0.017453292f * base.camera.fieldOfView * 0.5f);
		Vector3 position = base.transform.position;
		LineType lineType = this.GetLineType(type);
		Vector3 start = center + new Vector3(0f, 0f, radius);
		float num = 6.2831855f / (float)sections;
		for (float num2 = 0f; num2 <= 6.2831855f + num; num2 += num)
		{
			Vector3 vector = center + new Vector3(Mathf.Sin(num2) * radius, 0f, Mathf.Cos(num2) * radius);
			lineType.DrawLine(scale, position, start, vector, lineWidth);
			start = vector;
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0003F660 File Offset: 0x0003D860
	private LineType GetLineType(int id)
	{
		if (id < 0 || id >= this.m_lineTypes.Length)
		{
			return null;
		}
		return this.m_lineTypes[id];
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0003F684 File Offset: 0x0003D884
	public int GetTypeID(string name)
	{
		for (int i = 0; i < this.m_lineTypes.Length; i++)
		{
			if (this.m_lineTypes[i].m_name == name)
			{
				return i;
			}
		}
		PLog.LogError("Missing line type " + name);
		return -1;
	}

	// Token: 0x04000704 RID: 1796
	public LineType[] m_lineTypes;
}
