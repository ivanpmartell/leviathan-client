using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class FloatingProp : MonoBehaviour
{
	// Token: 0x06000766 RID: 1894 RVA: 0x0003711C File Offset: 0x0003531C
	public void Awake()
	{
		GameObject gameObject = GameObject.Find("WaterSurface");
		if (gameObject != null)
		{
			this.m_waterSurface = gameObject.GetComponent<WaterSurface>();
		}
		this.m_offset = base.transform.position.y;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00037168 File Offset: 0x00035368
	private void FixedUpdate()
	{
		if (this.m_waterSurface != null)
		{
			Vector3 position = base.transform.position;
			position.y = this.m_waterSurface.GetWorldWaveHeight(position) + this.m_offset;
			base.transform.position = position;
		}
	}

	// Token: 0x0400060E RID: 1550
	private WaterSurface m_waterSurface;

	// Token: 0x0400060F RID: 1551
	private float m_offset;
}
