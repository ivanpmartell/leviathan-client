using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
internal class AudioManager
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x0600062A RID: 1578 RVA: 0x0002EBFC File Offset: 0x0002CDFC
	public static AudioManager instance
	{
		get
		{
			if (AudioManager.m_instance == null)
			{
				AudioManager.m_instance = new AudioManager();
			}
			return AudioManager.m_instance;
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0002EC18 File Offset: 0x0002CE18
	public static void ResetInstance()
	{
		AudioManager.m_instance = null;
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0002EC20 File Offset: 0x0002CE20
	public void Update(float dt)
	{
		if (this.m_enableSoundDelay > 0f)
		{
			this.m_enableSoundDelay -= Mathf.Min(0.05f, dt);
			float num = Mathf.Clamp(1f - this.m_enableSoundDelay / 1f, 0f, 1f);
			if (this.m_currentVolume != num)
			{
				this.m_currentVolume = num;
				AudioListener.volume = num * this.m_volume;
			}
		}
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0002EC98 File Offset: 0x0002CE98
	public void SetSFXEnabled(bool enabled)
	{
		if (enabled)
		{
			this.m_enableSoundDelay = 1f;
		}
		else
		{
			this.m_currentVolume = 0f;
			AudioListener.volume = 0f;
			this.m_enableSoundDelay = -1f;
		}
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0002ECDC File Offset: 0x0002CEDC
	public void SetVolume(float volume)
	{
		this.m_volume = volume;
		AudioListener.volume = this.m_currentVolume * this.m_volume;
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0002ECF8 File Offset: 0x0002CEF8
	public float GetVolume()
	{
		return this.m_volume;
	}

	// Token: 0x040004BA RID: 1210
	private const float m_soundEnableFadein = 1f;

	// Token: 0x040004BB RID: 1211
	private static AudioManager m_instance;

	// Token: 0x040004BC RID: 1212
	private float m_enableSoundDelay = -1f;

	// Token: 0x040004BD RID: 1213
	private float m_currentVolume = 1f;

	// Token: 0x040004BE RID: 1214
	private float m_volume = 1f;
}
