using System;
using System.IO;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class WaterSurface : MonoBehaviour
{
	// Token: 0x06000A08 RID: 2568 RVA: 0x00049160 File Offset: 0x00047360
	private void Start()
	{
		if (this.m_camera)
		{
			this.m_camera.camera.depthTextureMode = DepthTextureMode.Depth;
		}
		if (QualitySettings.names[QualitySettings.GetQualityLevel()] == "Very Low")
		{
			base.renderer.material.shader = this.m_lowQualityShader;
		}
		this.m_size = 120 / this.m_detail;
		this.m_mesh = new Mesh();
		this.m_vertises = new Vector3[this.m_size * this.m_size];
		this.m_normals = new Vector3[this.m_size * this.m_size];
		this.m_tangents = new Vector4[this.m_size * this.m_size];
		int[] array = new int[this.m_size * this.m_size * 6];
		int num = 0;
		for (int i = 0; i < this.m_size - 1; i++)
		{
			for (int j = 0; j < this.m_size - 1; j++)
			{
				array[num++] = i * this.m_size + j;
				array[num++] = (i + 1) * this.m_size + j;
				array[num++] = i * this.m_size + j + 1;
				array[num++] = i * this.m_size + j + 1;
				array[num++] = (i + 1) * this.m_size + j;
				array[num++] = (i + 1) * this.m_size + j + 1;
			}
		}
		Vector2[] array2 = new Vector2[this.m_size * this.m_size];
		int num2 = 0;
		for (int k = 0; k < this.m_size; k++)
		{
			for (int l = 0; l < this.m_size; l++)
			{
				array2[num2++] = new Vector2((float)l, (float)k);
			}
		}
		Vector3[] array3 = new Vector3[this.m_size * this.m_size];
		int num3 = 0;
		for (int m = 0; m < this.m_size; m++)
		{
			for (int n = 0; n < this.m_size; n++)
			{
				array3[num3++] = new Vector3(0f, 1f, 0f);
			}
		}
		this.m_mesh.name = "WaterMesh";
		this.m_mesh.vertices = new Vector3[this.m_size * this.m_size];
		this.m_mesh.normals = array3;
		this.m_mesh.uv = array2;
		this.m_mesh.SetTriangles(array, 0);
		this.m_mesh.bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(10000f, 10000f, 10000f));
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		component.mesh = this.m_mesh;
		this.GenerateMesh();
		this.GenerateDepthTexture();
		this.m_rotX = new Vector2(Mathf.Cos(this.m_direction), Mathf.Sin(this.m_direction));
		this.m_rotY = new Vector2(-Mathf.Sin(this.m_direction), Mathf.Cos(this.m_direction));
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x000494B0 File Offset: 0x000476B0
	private void GenerateDepthTexture()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float y = 10f;
		int num = 256;
		int mapSize = this.m_mapSize;
		float num2 = -10f;
		float num3 = -6f;
		int layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("beach");
		int layerMask2 = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("shallow") | 1 << LayerMask.NameToLayer("beach");
		float[] array = new float[num * num];
		float[] array2 = new float[num * num];
		Vector3 direction = new Vector3(0f, -1f, 0f);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 origin = new Vector3(((float)j + 0.5f) / (float)num * (float)mapSize - (float)(mapSize / 2), y, ((float)i + 0.5f) / (float)num * (float)mapSize - (float)(mapSize / 2));
				float num4 = num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(origin, direction, out raycastHit, 100f, layerMask2))
				{
					num4 = raycastHit.point.y;
				}
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				array[j + i * num] = num4;
				float num5 = num2;
				if (Physics.Raycast(origin, direction, out raycastHit, 100f, layerMask))
				{
					num5 = raycastHit.point.y;
				}
				if (num5 > 1f)
				{
					num5 = 1f;
				}
				array2[j + i * num] = num5;
			}
		}
		Texture2D texture2D = new Texture2D(num, num);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < num; l++)
			{
				Color color = default(Color);
				float num6 = array[l + k * num];
				color.r = Mathf.Clamp(1f - num6 / num2, 0f, 1f);
				float num7 = array2[l + k * num];
				float num8 = Mathf.Clamp(1f - num7 / num3, 0f, 1f);
				if ((double)num8 > 0.8)
				{
					num8 = 0f;
				}
				color.a = num8;
				texture2D.SetPixel(l, k, color);
			}
		}
		texture2D.Apply();
		base.renderer.material.SetTexture("_DepthMapTex", texture2D);
		PLog.Log(string.Concat(new object[]
		{
			" GenerateDepthTexture time ",
			Time.realtimeSinceStartup - realtimeSinceStartup,
			"  size ",
			this.m_mapSize
		}));
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00049778 File Offset: 0x00047978
	public void SetMapSize(int size)
	{
		this.m_mapSize = size;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00049784 File Offset: 0x00047984
	public void SaveState(BinaryWriter writer)
	{
		writer.Write(this.m_time);
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00049794 File Offset: 0x00047994
	public void LoadState(BinaryReader reader)
	{
		this.m_time = reader.ReadSingle();
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x000497A4 File Offset: 0x000479A4
	private void Update()
	{
		if (this.m_camera != null)
		{
			float num = Mathf.Tan(0.017453292f * this.m_camera.camera.fieldOfView * this.m_camera.camera.aspect * 0.5f);
			num /= (float)this.m_size;
			num *= 3.5f;
			float num2 = 32f * (float)this.m_detail;
			float num3 = this.m_camera.transform.position.y;
			if (this.m_forceCameraHeight > 0f)
			{
				num3 = this.m_forceCameraHeight;
			}
			float num4 = Mathf.Clamp(num3 * num * (float)this.m_detail, (float)(4 * this.m_detail), num2);
			int minPow = Utils.GetMinPow2((int)num4);
			Vector3 position = this.m_camera.transform.position;
			float num5 = Vector3.Angle(this.m_camera.transform.TransformDirection(Vector3.forward), Vector3.down);
			float num6 = Mathf.Tan(0.017453292f * num5) * this.m_camera.transform.position.y;
			position.z += num6;
			Vector3 position2 = position - new Vector3((float)(this.m_size * minPow) * 0.5f, 0f, (float)(this.m_size * minPow) * 0.5f);
			position2.x = (float)((int)(position2.x / num2)) * num2;
			position2.z = (float)((int)(position2.z / num2)) * num2;
			position2.y = 0f;
			base.transform.position = position2;
			if ((float)minPow != this.m_tileSize)
			{
				this.m_tileSize = (float)minPow;
				this.GenerateMesh();
			}
		}
		base.renderer.material.SetMatrix("modelMatrix", base.transform.localToWorldMatrix);
		base.renderer.material.SetFloat("mapSize", (float)this.m_mapSize);
		base.renderer.material.SetFloat("_WaveDirection", this.m_direction);
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x000499C0 File Offset: 0x00047BC0
	private void FixedUpdate()
	{
		if (this.m_simulating || this.m_alwaysUpdate)
		{
			this.m_time += Time.fixedDeltaTime;
		}
		base.renderer.material.SetFloat("waveHeight", this.m_waveHeight);
		base.renderer.material.SetFloat("waterTime", this.m_time);
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x00049A2C File Offset: 0x00047C2C
	private void GenerateMesh()
	{
		int num = 0;
		for (int i = 0; i < this.m_size; i++)
		{
			for (int j = 0; j < this.m_size; j++)
			{
				Vector3 vector = new Vector3((float)j * this.m_tileSize, 0f, (float)i * this.m_tileSize);
				this.m_vertises[num++] = vector;
			}
		}
		int num2 = 0;
		for (int k = 0; k < this.m_size - 1; k++)
		{
			for (int l = 0; l < this.m_size - 1; l++)
			{
				Vector3 b = this.m_vertises[k * this.m_size + l];
				Vector3 a = this.m_vertises[k * this.m_size + l + 1];
				Vector3 a2 = this.m_vertises[(k + 1) * this.m_size + l];
				Vector3 normalized = (a - b).normalized;
				Vector3 normalized2 = (a2 - b).normalized;
				Vector3 vector2 = Vector3.Cross(normalized2, normalized);
				this.m_normals[k * this.m_size + l] = vector2;
				this.m_tangents[k * this.m_size + l] = new Vector4(normalized.x, normalized.y, normalized.z, 1f);
				num2++;
			}
		}
		this.m_mesh.vertices = this.m_vertises;
		this.m_mesh.normals = this.m_normals;
		this.m_mesh.tangents = this.m_tangents;
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00049C00 File Offset: 0x00047E00
	private float GetLocalWaveHeightAtWorldPos(Vector3 worldPos)
	{
		Vector2 a = new Vector2(0f, 0f);
		a += worldPos.x * this.m_rotX;
		a += worldPos.z * this.m_rotY;
		float num = 0f;
		num += Mathf.Sin(this.m_time * 0.3f + a.x * 0.1f) * Mathf.Sin(this.m_time * 0.2f + a.x * 0.05f);
		num += Mathf.Sin(this.m_time * 0.6f + (a.x + a.y) * 0.5f) * Mathf.Sin(this.m_time * 0.7f + (a.x + a.y) * 0.2f) * 0.1f;
		return num * this.m_waveHeight;
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x00049CF8 File Offset: 0x00047EF8
	public float GetForceAt(Vector3 worldPos, float boyancy)
	{
		float worldWaveHeight = this.GetWorldWaveHeight(worldPos);
		if (worldPos.y < worldWaveHeight)
		{
			float num = Mathf.Abs(worldWaveHeight - worldPos.y);
			return num * num * boyancy;
		}
		return 0f;
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00049D34 File Offset: 0x00047F34
	public float GetWorldWaveHeight(Vector3 worldPos)
	{
		return base.transform.position.y + this.GetLocalWaveHeightAtWorldPos(worldPos);
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x00049D5C File Offset: 0x00047F5C
	public void SetSimulating(bool simulating)
	{
		this.m_simulating = simulating;
	}

	// Token: 0x04000852 RID: 2130
	public GameObject m_camera;

	// Token: 0x04000853 RID: 2131
	public float m_waveHeight = 1.5f;

	// Token: 0x04000854 RID: 2132
	public bool m_alwaysUpdate;

	// Token: 0x04000855 RID: 2133
	public float m_forceCameraHeight = -1f;

	// Token: 0x04000856 RID: 2134
	public int m_mapSize = 500;

	// Token: 0x04000857 RID: 2135
	public float m_direction;

	// Token: 0x04000858 RID: 2136
	public Shader m_lowQualityShader;

	// Token: 0x04000859 RID: 2137
	private float m_tileSize = 1f;

	// Token: 0x0400085A RID: 2138
	private int m_detail = 1;

	// Token: 0x0400085B RID: 2139
	private int m_size;

	// Token: 0x0400085C RID: 2140
	private float m_time;

	// Token: 0x0400085D RID: 2141
	private bool m_simulating;

	// Token: 0x0400085E RID: 2142
	private Mesh m_mesh;

	// Token: 0x0400085F RID: 2143
	private Vector3[] m_vertises;

	// Token: 0x04000860 RID: 2144
	private Vector3[] m_normals;

	// Token: 0x04000861 RID: 2145
	private Vector4[] m_tangents;

	// Token: 0x04000862 RID: 2146
	private Vector2 m_rotX;

	// Token: 0x04000863 RID: 2147
	private Vector2 m_rotY;
}
