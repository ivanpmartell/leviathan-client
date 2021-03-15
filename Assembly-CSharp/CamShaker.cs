using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class CamShaker : MonoBehaviour
{
	// Token: 0x06000734 RID: 1844 RVA: 0x00036254 File Offset: 0x00034454
	public void Update()
	{
		if (this.m_triggerOnAwake)
		{
			this.m_delay -= Time.deltaTime;
			if (this.m_delay <= 0f)
			{
				this.Trigger();
				this.m_triggerOnAwake = false;
			}
		}
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0003629C File Offset: 0x0003449C
	public void Trigger()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			GameCamera component = main.GetComponent<GameCamera>();
			if (component != null)
			{
				component.AddShake(base.transform.position, this.m_intensity);
			}
		}
	}

	// Token: 0x040005ED RID: 1517
	public float m_intensity = 1f;

	// Token: 0x040005EE RID: 1518
	public bool m_triggerOnAwake;

	// Token: 0x040005EF RID: 1519
	public float m_delay;
}
