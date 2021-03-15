using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class TimedDestruction : MonoBehaviour
{
	// Token: 0x060009E3 RID: 2531 RVA: 0x00047088 File Offset: 0x00045288
	private void Awake()
	{
		base.Invoke("DestroyNow", this.m_timeout);
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0004709C File Offset: 0x0004529C
	private void DestroyNow()
	{
		if (this.m_detachChildren)
		{
			base.transform.DetachChildren();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000816 RID: 2070
	public float m_timeout = 1f;

	// Token: 0x04000817 RID: 2071
	public bool m_detachChildren;
}
