using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class WSEffect : MonoBehaviour
{
	// Token: 0x060009FB RID: 2555 RVA: 0x0004868C File Offset: 0x0004688C
	private void Awake()
	{
		base.Invoke("DestroyNow", this.m_timeout);
		ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000486CC File Offset: 0x000468CC
	private void DestroyNow()
	{
		if (this.m_detachChildren)
		{
			base.transform.DetachChildren();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000836 RID: 2102
	public float m_timeout = 1f;

	// Token: 0x04000837 RID: 2103
	public bool m_detachChildren;
}
