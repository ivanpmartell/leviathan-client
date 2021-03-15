using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class Smoke : MonoBehaviour
{
	// Token: 0x060009C3 RID: 2499 RVA: 0x00046650 File Offset: 0x00044850
	private void OnTriggerStay(Collider other)
	{
		Section component = other.GetComponent<Section>();
		if (!component && other.transform.parent != null)
		{
			component = other.transform.parent.GetComponent<Section>();
		}
		if (component != null)
		{
			component.OnSmokeEnter();
			return;
		}
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x000466AC File Offset: 0x000448AC
	private void Awake()
	{
		this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
		this.m_lineType = this.m_lineDrawer.GetTypeID("smokeArea");
		this.m_radius = (base.collider as SphereCollider).radius;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x000466F8 File Offset: 0x000448F8
	private void Update()
	{
		this.m_lineDrawer.DrawXZCircle(base.transform.position, this.m_radius, 16, this.m_lineType, 1f);
	}

	// Token: 0x040007FE RID: 2046
	private LineDrawer m_lineDrawer;

	// Token: 0x040007FF RID: 2047
	private int m_lineType = -1;

	// Token: 0x04000800 RID: 2048
	private float m_radius = 1f;
}
