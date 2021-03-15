using System;
using System.IO;
using UnityEngine;

// Token: 0x0200008B RID: 139
[AddComponentMenu("Scripts/Mission/MNTimer")]
public class MNTimer : MNTrigger
{
	// Token: 0x0600056F RID: 1391 RVA: 0x0002BE90 File Offset: 0x0002A090
	public override void Awake()
	{
		base.Awake();
		this.m_currentTime = this.m_time;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0002BEA4 File Offset: 0x0002A0A4
	protected void Destroy()
	{
		Debug.Break();
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x0002BEAC File Offset: 0x0002A0AC
	protected void Update()
	{
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x0002BEB0 File Offset: 0x0002A0B0
	protected void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			if (this.m_disabled)
			{
				return;
			}
			this.m_currentTime -= Time.fixedDeltaTime;
			if (this.m_currentTime <= 0f)
			{
				this.m_currentTime = this.m_time;
				this.Trigger();
			}
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0002BF08 File Offset: 0x0002A108
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_currentTime);
		writer.Write(this.m_time);
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x0002BF2C File Offset: 0x0002A12C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_currentTime = reader.ReadSingle();
		this.m_time = reader.ReadSingle();
	}

	// Token: 0x04000494 RID: 1172
	private float m_currentTime;

	// Token: 0x04000495 RID: 1173
	public float m_time;
}
