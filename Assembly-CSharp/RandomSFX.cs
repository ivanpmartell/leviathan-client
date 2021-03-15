using System;
using UnityEngine;

// Token: 0x020000F3 RID: 243
internal class RandomSFX : MonoBehaviour
{
	// Token: 0x0600096D RID: 2413 RVA: 0x000447A0 File Offset: 0x000429A0
	public void Awake()
	{
		this.m_delay = UnityEngine.Random.Range(this.m_minDelay, this.m_maxDelay);
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000447BC File Offset: 0x000429BC
	public void Update()
	{
		this.m_time += Time.deltaTime;
		if (this.m_setPrioTime > 0f && this.m_time > this.m_delay + this.m_setPrioTime)
		{
			base.audio.priority = this.m_setPrio;
			this.m_setPrioTime = -1f;
		}
		if (this.m_setPrioTime2 > 0f && this.m_time > this.m_delay + this.m_setPrioTime2)
		{
			base.audio.priority = this.m_setPrio2;
			this.m_setPrioTime2 = -1f;
		}
		if (this.m_setPrioTime3 > 0f && this.m_time > this.m_delay + this.m_setPrioTime3)
		{
			base.audio.priority = this.m_setPrio3;
			this.m_setPrioTime3 = -1f;
		}
		if (this.m_delay >= 0f && this.m_time >= this.m_delay)
		{
			this.m_delay = -1f;
			if (base.audio != null && this.m_audioClips.Length > 0)
			{
				int num = UnityEngine.Random.Range(0, this.m_audioClips.Length);
				base.audio.clip = this.m_audioClips[num];
				base.audio.pitch = UnityEngine.Random.Range(this.m_minPitch, this.m_maxPitch);
				base.audio.volume = UnityEngine.Random.Range(this.m_minVol, this.m_maxVol);
				if (this.m_playOnAwake && (this.m_playInPhase == RandomSFX.PlayPhase.All || (this.m_playInPhase == RandomSFX.PlayPhase.Planning && !NetObj.IsSimulating()) || (this.m_playInPhase == RandomSFX.PlayPhase.Simulating && NetObj.IsSimulating())))
				{
					base.audio.Play();
				}
			}
		}
	}

	// Token: 0x04000798 RID: 1944
	public AudioClip[] m_audioClips = new AudioClip[0];

	// Token: 0x04000799 RID: 1945
	public bool m_playOnAwake = true;

	// Token: 0x0400079A RID: 1946
	public RandomSFX.PlayPhase m_playInPhase;

	// Token: 0x0400079B RID: 1947
	public float m_maxPitch = 1f;

	// Token: 0x0400079C RID: 1948
	public float m_minPitch = 1f;

	// Token: 0x0400079D RID: 1949
	public float m_maxVol = 1f;

	// Token: 0x0400079E RID: 1950
	public float m_minVol = 1f;

	// Token: 0x0400079F RID: 1951
	public float m_setPrioTime = -1f;

	// Token: 0x040007A0 RID: 1952
	public int m_setPrio = 128;

	// Token: 0x040007A1 RID: 1953
	public float m_setPrioTime2 = -1f;

	// Token: 0x040007A2 RID: 1954
	public int m_setPrio2 = 128;

	// Token: 0x040007A3 RID: 1955
	public float m_setPrioTime3 = 5f;

	// Token: 0x040007A4 RID: 1956
	public int m_setPrio3 = 255;

	// Token: 0x040007A5 RID: 1957
	public float m_maxDelay;

	// Token: 0x040007A6 RID: 1958
	public float m_minDelay;

	// Token: 0x040007A7 RID: 1959
	private float m_delay;

	// Token: 0x040007A8 RID: 1960
	private float m_time;

	// Token: 0x020000F4 RID: 244
	public enum PlayPhase
	{
		// Token: 0x040007AA RID: 1962
		All,
		// Token: 0x040007AB RID: 1963
		Planning,
		// Token: 0x040007AC RID: 1964
		Simulating
	}
}
