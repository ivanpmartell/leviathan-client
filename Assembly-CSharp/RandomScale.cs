using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class RandomScale : MonoBehaviour
{
	// Token: 0x06000970 RID: 2416 RVA: 0x000449D8 File Offset: 0x00042BD8
	private void Start()
	{
		float num = UnityEngine.Random.Range(this.m_min, this.m_max);
		base.transform.localScale = new Vector3(num, num, num);
		this.m_timeOffset = UnityEngine.Random.value * 10f;
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x00044A1C File Offset: 0x00042C1C
	private void Update()
	{
		float num = this.m_timeOffset + Time.time;
		float num2 = Mathf.Sin(num * this.m_frequency) * Mathf.Cos(num * this.m_frequency * 2.2532f);
		float num3 = this.m_min + (num2 * 0.5f + 0.5f) * (this.m_max - this.m_min);
		base.transform.localScale = new Vector3(num3, num3, num3);
		if (this.m_resetRotation)
		{
			base.transform.rotation = Quaternion.identity;
		}
	}

	// Token: 0x040007AD RID: 1965
	public float m_min = 1f;

	// Token: 0x040007AE RID: 1966
	public float m_max = 1.5f;

	// Token: 0x040007AF RID: 1967
	public bool m_continuous = true;

	// Token: 0x040007B0 RID: 1968
	public float m_frequency = 0.1f;

	// Token: 0x040007B1 RID: 1969
	public bool m_resetRotation;

	// Token: 0x040007B2 RID: 1970
	private float m_timeOffset;
}
