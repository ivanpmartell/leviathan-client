using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class MusicManager
{
	// Token: 0x060006F1 RID: 1777 RVA: 0x00034E04 File Offset: 0x00033004
	public MusicManager(AudioSource[] sources)
	{
		DebugUtils.Assert(sources.Length == 2);
		this.m_sources = sources;
		foreach (AudioSource audioSource in this.m_sources)
		{
			audioSource.ignoreListenerVolume = true;
			audioSource.volume = 0f;
		}
		MusicManager.m_instance = this;
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060006F2 RID: 1778 RVA: 0x00034E98 File Offset: 0x00033098
	public static MusicManager instance
	{
		get
		{
			return MusicManager.m_instance;
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00034EA0 File Offset: 0x000330A0
	public void Close()
	{
		MusicManager.m_instance = null;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00034EA8 File Offset: 0x000330A8
	public void Update(float dt)
	{
		if (this.m_fadeTimer >= 0f)
		{
			this.m_fadeTimer += dt;
			float num = this.m_fadeTimer / this.m_crossFadeTime;
			if (num >= 1f)
			{
				this.m_fadeTimer = -1f;
				int primarySource = this.m_primarySource;
				this.m_primarySource = ((this.m_primarySource != 0) ? 0 : 1);
				this.m_sources[primarySource].volume = 0f;
				this.m_sources[this.m_primarySource].volume = this.m_volume * this.m_volumeModifier;
				this.m_sources[primarySource].Stop();
			}
			else
			{
				int num2 = (this.m_primarySource != 0) ? 0 : 1;
				this.m_sources[num2].volume = num * this.m_volume * this.m_volumeModifier;
				this.m_sources[this.m_primarySource].volume = (1f - num) * this.m_volume * this.m_volumeModifier;
			}
		}
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00034FB0 File Offset: 0x000331B0
	public void SetMusic(string name)
	{
		if (this.m_currentMusic == name)
		{
			return;
		}
		this.m_currentMusic = name;
		if (name == string.Empty)
		{
			this.SetMusic(null);
		}
		else
		{
			AudioClip audioClip = Resources.Load("music/" + name) as AudioClip;
			if (audioClip == null)
			{
				PLog.LogWarning("Missing music " + name);
				return;
			}
			this.SetMusic(audioClip);
		}
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x0003502C File Offset: 0x0003322C
	private void SetMusic(AudioClip clip)
	{
		this.m_fadeTimer = 0f;
		int num = (this.m_primarySource != 0) ? 0 : 1;
		this.m_sources[num].clip = clip;
		if (clip != null)
		{
			this.m_sources[num].Play();
		}
		else
		{
			this.m_sources[num].Stop();
		}
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x00035090 File Offset: 0x00033290
	public float GetVolume()
	{
		return this.m_volume;
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00035098 File Offset: 0x00033298
	public void SetVolume(float volume)
	{
		volume = Mathf.Clamp(volume, 0f, 1f);
		this.m_volume = volume;
		if (this.m_fadeTimer < 0f)
		{
			this.m_sources[this.m_primarySource].volume = this.m_volume * this.m_volumeModifier;
		}
	}

	// Token: 0x040005D6 RID: 1494
	private float m_fadeTimer = -1f;

	// Token: 0x040005D7 RID: 1495
	private float m_crossFadeTime = 1f;

	// Token: 0x040005D8 RID: 1496
	private AudioSource[] m_sources;

	// Token: 0x040005D9 RID: 1497
	private int m_primarySource;

	// Token: 0x040005DA RID: 1498
	private float m_volume = 1f;

	// Token: 0x040005DB RID: 1499
	private float m_volumeModifier = 0.25f;

	// Token: 0x040005DC RID: 1500
	private string m_currentMusic = string.Empty;

	// Token: 0x040005DD RID: 1501
	private static MusicManager m_instance;
}
