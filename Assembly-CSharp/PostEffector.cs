using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class PostEffector : MonoBehaviour
{
	// Token: 0x06000924 RID: 2340 RVA: 0x000422BC File Offset: 0x000404BC
	private void Start()
	{
		PostEffector.m_postEffectors.Add(this);
		if (PostEffector.m_first)
		{
			PostEffector.m_first = false;
			PostEffector.m_fxaaEnabled = (PlayerPrefs.GetInt("fxaa", (!PostEffector.m_fxaaDefault) ? 0 : 1) == 1);
			PostEffector.m_bloomEnabled = (PlayerPrefs.GetInt("bloom", (!PostEffector.m_bloomDefault) ? 0 : 1) == 1);
			PLog.Log(string.Concat(new object[]
			{
				"first ",
				PostEffector.m_fxaaEnabled,
				"  ",
				PostEffector.m_bloomEnabled
			}));
		}
		PostEffector.UpdateEnabled();
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0004236C File Offset: 0x0004056C
	private void OnDestroy()
	{
		PostEffector.m_postEffectors.Remove(this);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0004237C File Offset: 0x0004057C
	public static void SetFXAAEnabled(bool enabled)
	{
		PostEffector.m_fxaaEnabled = enabled;
		PlayerPrefs.SetInt("fxaa", (!enabled) ? 0 : 1);
		PostEffector.UpdateEnabled();
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x000423AC File Offset: 0x000405AC
	public static void SetBloomEnabled(bool enabled)
	{
		PostEffector.m_bloomEnabled = enabled;
		PlayerPrefs.SetInt("bloom", (!enabled) ? 0 : 1);
		PostEffector.UpdateEnabled();
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x000423DC File Offset: 0x000405DC
	public static bool IsFXAAEnabled()
	{
		return PostEffector.m_fxaaEnabled;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x000423E4 File Offset: 0x000405E4
	public static bool IsBloomEnabled()
	{
		return PostEffector.m_bloomEnabled;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x000423EC File Offset: 0x000405EC
	private static void UpdateEnabled()
	{
		foreach (PostEffector postEffector in PostEffector.m_postEffectors)
		{
			if (postEffector.m_bloomEffect)
			{
				postEffector.m_bloomEffect.enabled = PostEffector.m_bloomEnabled;
			}
			if (postEffector.m_fxaaEffect)
			{
				postEffector.m_fxaaEffect.enabled = PostEffector.m_fxaaEnabled;
			}
		}
	}

	// Token: 0x04000757 RID: 1879
	private static bool m_fxaaDefault = true;

	// Token: 0x04000758 RID: 1880
	private static bool m_bloomDefault = true;

	// Token: 0x04000759 RID: 1881
	public MonoBehaviour m_fxaaEffect;

	// Token: 0x0400075A RID: 1882
	public MonoBehaviour m_bloomEffect;

	// Token: 0x0400075B RID: 1883
	private static bool m_first = true;

	// Token: 0x0400075C RID: 1884
	private static bool m_fxaaEnabled = false;

	// Token: 0x0400075D RID: 1885
	private static bool m_bloomEnabled = false;

	// Token: 0x0400075E RID: 1886
	private static List<PostEffector> m_postEffectors = new List<PostEffector>();
}
