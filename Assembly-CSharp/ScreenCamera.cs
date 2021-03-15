using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class ScreenCamera : MonoBehaviour
{
	// Token: 0x06000973 RID: 2419 RVA: 0x00044AE8 File Offset: 0x00042CE8
	private void Awake()
	{
		int num = Screen.height;
		if (num < this.m_minHeight)
		{
			num = this.m_minHeight;
		}
		if (num > this.m_maxHeightPC)
		{
			num = this.m_maxHeightPC;
		}
		base.camera.orthographicSize = (float)(num / 2);
		bool scaled = num != Screen.height;
		this.SetupFontFiltering(scaled);
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00044B44 File Offset: 0x00042D44
	private void SetupFontFiltering(bool scaled)
	{
		foreach (Texture2D texture2D in this.m_fontTextures)
		{
			if (scaled)
			{
				texture2D.filterMode = FilterMode.Bilinear;
			}
			else
			{
				texture2D.filterMode = FilterMode.Point;
			}
		}
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x00044B8C File Offset: 0x00042D8C
	private void OnPreRender()
	{
		if (this.m_disableFog)
		{
			this.m_fogStatus = RenderSettings.fog;
			RenderSettings.fog = false;
		}
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00044BAC File Offset: 0x00042DAC
	private void OnPostRender()
	{
		if (this.m_disableFog)
		{
			RenderSettings.fog = this.m_fogStatus;
		}
	}

	// Token: 0x040007B3 RID: 1971
	public bool m_disableFog = true;

	// Token: 0x040007B4 RID: 1972
	public int m_minHeight = 720;

	// Token: 0x040007B5 RID: 1973
	public int m_maxHeightPC = 1080;

	// Token: 0x040007B6 RID: 1974
	public int m_maxHeightTablet = 720;

	// Token: 0x040007B7 RID: 1975
	public Texture2D[] m_fontTextures;

	// Token: 0x040007B8 RID: 1976
	private bool m_fogStatus;
}
