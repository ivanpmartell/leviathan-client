using System;
using UnityEngine;

// Token: 0x02000169 RID: 361
public class waves : MonoBehaviour
{
	// Token: 0x06000D7A RID: 3450 RVA: 0x000607AC File Offset: 0x0005E9AC
	private void Start()
	{
		this.m_heights = new float[this.m_size * this.m_size];
		this.m_vel = new float[this.m_size * this.m_size];
		this.m_waterDepth = new float[this.m_size * this.m_size];
		this.m_mesh = new Mesh();
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
		this.m_mesh.vertices = new Vector3[this.m_size * this.m_size];
		this.m_mesh.uv = array2;
		this.m_mesh.SetTriangles(array, 0);
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		component.mesh = this.m_mesh;
		this.GenerateMesh();
		this.m_mesh.RecalculateBounds();
		this.DetectDepths();
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0006099C File Offset: 0x0005EB9C
	private void DetectDepths()
	{
		int layerMask = 1;
		float num = 2f;
		for (int i = 0; i < this.m_size; i++)
		{
			for (int j = 0; j < this.m_size; j++)
			{
				Vector3 vector = base.transform.position + new Vector3((float)j, num, (float)i);
				RaycastHit raycastHit;
				if (Physics.Linecast(vector, vector + new Vector3(0f, -this.m_maxDepth, 0f), out raycastHit, layerMask))
				{
					this.m_waterDepth[i * this.m_size + j] = raycastHit.distance - num;
				}
				else
				{
					this.m_waterDepth[i * this.m_size + j] = this.m_maxDepth;
				}
			}
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x00060A60 File Offset: 0x0005EC60
	private void Update()
	{
		this.UpdateSurface();
		this.GenerateMesh();
		if (Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			if (base.collider.Raycast(ray, out raycastHit, 100f))
			{
				this.SetVel(raycastHit.point, -10f);
			}
		}
		for (int i = 0; i < this.m_size; i++)
		{
			this.SetOffset(0, i, Mathf.Sin(Time.time) * 2f);
		}
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00060AF0 File Offset: 0x0005ECF0
	private void GenerateMesh()
	{
		Vector3[] array = new Vector3[this.m_size * this.m_size];
		int num = 0;
		for (int i = 0; i < this.m_size; i++)
		{
			for (int j = 0; j < this.m_size; j++)
			{
				float y = this.GetWaveHeight(j, i) + this.m_heights[i * this.m_size + j];
				array[num++] = new Vector3((float)j, y, (float)i);
			}
		}
		this.m_mesh.vertices = array;
		Color[] array2 = new Color[this.m_size * this.m_size];
		for (int k = 0; k < this.m_size; k++)
		{
			for (int l = 0; l < this.m_size; l++)
			{
				float num2 = this.m_waterDepth[k * this.m_size + l] / this.m_maxDepth;
				array2[k * this.m_size + l] = new Color(num2, num2, num2, 1f);
			}
		}
		this.m_mesh.colors = array2;
		this.m_mesh.RecalculateNormals();
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00060C2C File Offset: 0x0005EE2C
	private void UpdateSurface()
	{
		for (int i = 1; i < this.m_size - 1; i++)
		{
			for (int j = 1; j < this.m_size - 1; j++)
			{
				float num = this.m_heights[i * this.m_size + j];
				float num2 = this.m_heights[i * this.m_size + j + 1] - num;
				float num3 = this.m_heights[i * this.m_size + j - 1] - num;
				float num4 = this.m_heights[(i + 1) * this.m_size + j] - num;
				float num5 = this.m_heights[(i - 1) * this.m_size + j] - num;
				float num6 = (num2 + num3 + num4 + num5) / 4f;
				this.m_vel[i * this.m_size + j] += num6;
				this.m_vel[i * this.m_size + j] -= this.m_vel[i * this.m_size + j] * this.m_damp;
				if (this.m_vel[i * this.m_size + j] > 0f)
				{
				}
			}
		}
		for (int k = 0; k < this.m_size; k++)
		{
			for (int l = 0; l < this.m_size; l++)
			{
				float num7 = this.m_vel[k * this.m_size + l] * Time.deltaTime;
				this.m_heights[k * this.m_size + l] += num7;
			}
		}
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00060DC0 File Offset: 0x0005EFC0
	private float GetWaveHeight(int x, int y)
	{
		return Mathf.Sin(Time.time * 2f + (float)x * 0.2f) * this.m_waveHeight;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x00060DF0 File Offset: 0x0005EFF0
	private float GetWorldWaveHeight(Vector3 worldPos)
	{
		Vector3 vector = worldPos - base.transform.position;
		int num = (int)vector.x;
		int num2 = (int)vector.z;
		if (num < 0 || num2 < 0 || num >= this.m_size || num2 >= this.m_size)
		{
			return 0f;
		}
		return base.transform.position.y + this.GetWaveHeight(num, num2);
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00060E6C File Offset: 0x0005F06C
	private void SetOffset(int x, int y, float offset)
	{
		if (x < 0 || y < 0 || x >= this.m_size || y >= this.m_size)
		{
			return;
		}
		this.m_heights[y * this.m_size + x] = offset;
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00060EA8 File Offset: 0x0005F0A8
	private void SetOffset(Vector3 worldPos, float offset)
	{
		Vector3 vector = worldPos - base.transform.position;
		int num = (int)vector.x;
		int num2 = (int)vector.z;
		if (num < 0 || num2 < 0 || num >= this.m_size || num2 >= this.m_size)
		{
			return;
		}
		this.m_heights[num2 * this.m_size + num] = offset;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00060F14 File Offset: 0x0005F114
	private void SetVel(Vector3 worldPos, float vel)
	{
		Vector3 vector = worldPos - base.transform.position;
		int num = (int)vector.x;
		int num2 = (int)vector.z;
		if (num < 0 || num2 < 0 || num >= this.m_size || num2 >= this.m_size)
		{
			return;
		}
		this.m_vel[num2 * this.m_size + num] = vel;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00060F80 File Offset: 0x0005F180
	private void OnTriggerStay(Collider other)
	{
		if (other.attachedRigidbody)
		{
			BoxCollider boxCollider = other as BoxCollider;
			if (boxCollider == null)
			{
				return;
			}
			Vector3[] array = new Vector3[8];
			Vector3 vector = boxCollider.size * 0.5f;
			array[0] = other.transform.TransformPoint(boxCollider.center + new Vector3(vector.x, vector.y, vector.z));
			array[1] = other.transform.TransformPoint(boxCollider.center + new Vector3(-vector.x, vector.y, vector.z));
			array[2] = other.transform.TransformPoint(boxCollider.center + new Vector3(vector.x, vector.y, -vector.z));
			array[3] = other.transform.TransformPoint(boxCollider.center + new Vector3(-vector.x, vector.y, -vector.z));
			array[4] = other.transform.TransformPoint(boxCollider.center + new Vector3(vector.x, -vector.y, vector.z));
			array[5] = other.transform.TransformPoint(boxCollider.center + new Vector3(-vector.x, -vector.y, vector.z));
			array[6] = other.transform.TransformPoint(boxCollider.center + new Vector3(vector.x, -vector.y, -vector.z));
			array[7] = other.transform.TransformPoint(boxCollider.center + new Vector3(-vector.x, -vector.y, -vector.z));
			float num = 0.3f;
			float d = 0.05f;
			foreach (Vector3 vector2 in array)
			{
				Debug.DrawLine(vector2, vector2 + new Vector3(0f, 1f, 0f));
				float worldWaveHeight = this.GetWorldWaveHeight(vector2);
				if (vector2.y < worldWaveHeight)
				{
					float num2 = vector2.y - worldWaveHeight;
					Vector3 pointVelocity = other.attachedRigidbody.GetPointVelocity(vector2);
					if (Mathf.Abs(num2) < 2f)
					{
						this.SetVel(vector2, -pointVelocity.magnitude);
					}
					float y = Mathf.Abs(num2 * num2) * num;
					other.attachedRigidbody.AddForceAtPosition(new Vector3(0f, y, 0f), vector2);
					other.attachedRigidbody.AddForceAtPosition(-pointVelocity * d, vector2);
				}
			}
		}
	}

	// Token: 0x04000B0F RID: 2831
	public float m_waveHeight;

	// Token: 0x04000B10 RID: 2832
	public float m_damp;

	// Token: 0x04000B11 RID: 2833
	public int m_size = 100;

	// Token: 0x04000B12 RID: 2834
	private float m_maxDepth = 20f;

	// Token: 0x04000B13 RID: 2835
	private Mesh m_mesh;

	// Token: 0x04000B14 RID: 2836
	private float[] m_heights;

	// Token: 0x04000B15 RID: 2837
	private float[] m_vel;

	// Token: 0x04000B16 RID: 2838
	private float[] m_waterDepth;
}
