using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BF RID: 191
internal class LosDrawer
{
	// Token: 0x060006CB RID: 1739 RVA: 0x00033CFC File Offset: 0x00031EFC
	public LosDrawer()
	{
		this.m_material = (Resources.Load("circletest") as Material);
		this.m_quadMaterial = (Resources.Load("cutplane") as Material);
		this.m_clearDepthMaterial = (Resources.Load("cleardepth") as Material);
		this.m_circleMesh = this.CreateCircle(new Vector3(0f, 0f, 0f), 1f, 40);
		this.m_quadMesh = this.CreateBoxMesh(5000f, 0f);
		this.m_clearMesh = this.CreateBoxMesh(5000f, -5f);
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x00033DA4 File Offset: 0x00031FA4
	private Mesh CreateBoxMesh(float size, float height)
	{
		Vector3[] array = new Vector3[4];
		Vector2[] array2 = new Vector2[4];
		int[] array3 = new int[6];
		array[0] = new Vector3(-size, height, -size);
		array[1] = new Vector3(size, height, -size);
		array[2] = new Vector3(size, height, size);
		array[3] = new Vector3(-size, height, size);
		array2[0] = new Vector2(-size, -size);
		array2[1] = new Vector2(size, -size);
		array2[2] = new Vector2(size, size);
		array2[3] = new Vector2(-size, size);
		array3[0] = 0;
		array3[1] = 1;
		array3[2] = 3;
		array3[3] = 1;
		array3[4] = 2;
		array3[5] = 3;
		Mesh mesh = new Mesh();
		mesh.name = "LosMesh";
		mesh.vertices = array;
		mesh.triangles = array3;
		mesh.uv = array2;
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x00033EB0 File Offset: 0x000320B0
	private bool SetupLineDrawer()
	{
		if (!(Camera.main != null))
		{
			return false;
		}
		this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
		if (!this.m_lineDrawer)
		{
			return false;
		}
		this.m_sightRangeLineType = this.m_lineDrawer.GetTypeID("sightRange");
		this.m_sightRangeSelectedLineType = this.m_lineDrawer.GetTypeID("sightRangeSelected");
		DebugUtils.Assert(this.m_sightRangeLineType >= 0 && this.m_sightRangeSelectedLineType >= 0);
		return true;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x00033F40 File Offset: 0x00032140
	public void Draw()
	{
		if (!this.SetupLineDrawer())
		{
			return;
		}
		int localPlayer = NetObj.GetLocalPlayer();
		if (localPlayer < 0)
		{
			return;
		}
		int playerTeam = TurnMan.instance.GetPlayerTeam(localPlayer);
		Graphics.DrawMesh(this.m_clearMesh, Matrix4x4.identity, this.m_clearDepthMaterial, 0, null, 0, null, false, false);
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Unit unit = netObj as Unit;
			if (unit)
			{
				if (unit.GetOwnerTeam() == playerTeam)
				{
					if (unit.CanLOS())
					{
						Vector3 position = unit.transform.position;
						position.y = 3f;
						float sightRange = unit.GetSightRange();
						if (sightRange != 0f)
						{
							if (unit.IsSelected())
							{
								this.m_lineDrawer.DrawXZCircle(position, sightRange, 40, this.m_sightRangeSelectedLineType, 0.2f);
							}
							Matrix4x4 matrix = default(Matrix4x4);
							matrix.SetTRS(position, Quaternion.identity, new Vector3(sightRange, sightRange, sightRange));
							Graphics.DrawMesh(this.m_circleMesh, matrix, this.m_material, 0, null, 0, null, false, false);
						}
					}
				}
			}
		}
		Graphics.DrawMesh(this.m_quadMesh, Matrix4x4.identity, this.m_quadMaterial, 0, null, 0, null, false, false);
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000340D0 File Offset: 0x000322D0
	private Mesh CreateCircle(Vector3 center, float radius, int sections)
	{
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector2> list3 = new List<Vector2>();
		list.Add(center);
		list3.Add(new Vector2(center.x, center.z));
		Vector3 item = center + new Vector3(0f, 0f, radius);
		list.Add(item);
		list3.Add(new Vector2(item.x, item.z));
		float num = 6.2831855f / (float)sections;
		for (float num2 = 0f; num2 <= 6.2831855f + num; num2 += num)
		{
			Vector3 item2 = center + new Vector3(Mathf.Sin(num2) * radius, 0f, Mathf.Cos(num2) * radius);
			list2.Add(0);
			list2.Add(list.Count - 1);
			list2.Add(list.Count);
			list.Add(item2);
			list3.Add(new Vector2(item2.x, item2.z));
		}
		Mesh mesh = new Mesh();
		mesh.name = "LosCircle";
		mesh.vertices = list.ToArray();
		mesh.triangles = list2.ToArray();
		mesh.uv = list3.ToArray();
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x040005BA RID: 1466
	private Material m_material;

	// Token: 0x040005BB RID: 1467
	private LineDrawer m_lineDrawer;

	// Token: 0x040005BC RID: 1468
	private int m_sightRangeLineType;

	// Token: 0x040005BD RID: 1469
	private int m_sightRangeSelectedLineType;

	// Token: 0x040005BE RID: 1470
	private Mesh m_circleMesh;

	// Token: 0x040005BF RID: 1471
	private Material m_quadMaterial;

	// Token: 0x040005C0 RID: 1472
	private Material m_clearDepthMaterial;

	// Token: 0x040005C1 RID: 1473
	private Mesh m_quadMesh;

	// Token: 0x040005C2 RID: 1474
	private Mesh m_clearMesh;
}
