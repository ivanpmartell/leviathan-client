using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class SplashTrigger : MonoBehaviour
{
	// Token: 0x060009CD RID: 2509 RVA: 0x00046A24 File Offset: 0x00044C24
	private void Awake()
	{
		this.m_lastPos = base.transform.position.y;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00046A4C File Offset: 0x00044C4C
	private void Update()
	{
		float y = base.transform.position.y;
		if ((y >= 0f && this.m_lastPos < 0f) || (y < 0f && this.m_lastPos >= 0f))
		{
			this.Trigger();
		}
		this.m_lastPos = y;
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00046AB0 File Offset: 0x00044CB0
	private void Trigger()
	{
		GameObject effectPrefabHigh = this.m_effectPrefabHigh;
		if (effectPrefabHigh == null)
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y = 0f;
		UnityEngine.Object.Instantiate(effectPrefabHigh, position, Quaternion.identity);
	}

	// Token: 0x04000807 RID: 2055
	public GameObject m_effectPrefabLow;

	// Token: 0x04000808 RID: 2056
	public GameObject m_effectPrefabHigh;

	// Token: 0x04000809 RID: 2057
	private float m_lastPos;
}
