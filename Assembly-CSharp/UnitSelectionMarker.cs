using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class UnitSelectionMarker : MonoBehaviour
{
	// Token: 0x060009F0 RID: 2544 RVA: 0x00047910 File Offset: 0x00045B10
	public void Setup(Vector3 size, Color color)
	{
		Vector3 vector = size * 0.5f;
		float num = Mathf.Min(this.m_maxScale, this.m_scaleFactor * Mathf.Min(size.x, size.z));
		foreach (GameObject gameObject in this.m_corners)
		{
			gameObject.transform.localScale = new Vector3(num, num, num);
		}
		this.m_corners[0].transform.localPosition = new Vector3(-vector.x, 0f, vector.z);
		this.m_corners[1].transform.localPosition = new Vector3(vector.x, 0f, vector.z);
		this.m_corners[2].transform.localPosition = new Vector3(-vector.x, 0f, -vector.z);
		this.m_corners[3].transform.localPosition = new Vector3(vector.x, 0f, -vector.z);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.material.color = color;
		}
	}

	// Token: 0x0400082A RID: 2090
	public GameObject[] m_corners = new GameObject[4];

	// Token: 0x0400082B RID: 2091
	public float m_scaleFactor = 1f;

	// Token: 0x0400082C RID: 2092
	public float m_maxScale = 1f;
}
