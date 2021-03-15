using System;

// Token: 0x0200014E RID: 334
internal class ProfileTimer
{
	// Token: 0x06000CA8 RID: 3240 RVA: 0x0005AC70 File Offset: 0x00058E70
	public ProfileTimer()
	{
		this.m_updateTime = Utils.GetTimeMS();
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0005AC94 File Offset: 0x00058E94
	public void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_time = Utils.GetTimeMS();
		this.m_started = true;
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0005ACB4 File Offset: 0x00058EB4
	public void Stop()
	{
		if (!this.m_started)
		{
			return;
		}
		long timeMS = Utils.GetTimeMS();
		long num = timeMS - this.m_time;
		this.m_totalTime += num;
		this.m_samples++;
		this.m_started = false;
		if (timeMS - this.m_updateTime > ProfileTimer.m_averageInterval)
		{
			this.m_average = (float)this.m_totalTime / (float)this.m_samples;
			this.m_totalTime = 0L;
			this.m_samples = 0;
			this.m_updateTime = timeMS;
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0005AD3C File Offset: 0x00058F3C
	public float GetAverage()
	{
		return this.m_average;
	}

	// Token: 0x04000A68 RID: 2664
	private static long m_averageInterval = 10000L;

	// Token: 0x04000A69 RID: 2665
	private long m_time;

	// Token: 0x04000A6A RID: 2666
	private bool m_started;

	// Token: 0x04000A6B RID: 2667
	private long m_totalTime;

	// Token: 0x04000A6C RID: 2668
	private int m_samples;

	// Token: 0x04000A6D RID: 2669
	private float m_average;

	// Token: 0x04000A6E RID: 2670
	private long m_updateTime;
}
