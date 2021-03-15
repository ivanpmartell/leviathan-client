using System;
using System.IO;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class SmokeGrenade : Deployable
{
	// Token: 0x060009C7 RID: 2503 RVA: 0x00046750 File Offset: 0x00044950
	public override void Awake()
	{
		base.Awake();
		this.m_effect = (UnityEngine.Object.Instantiate(this.m_effectHigh, base.transform.position, Quaternion.identity) as GameObject);
		this.m_effect.transform.parent = base.transform;
		this.m_psystem = this.m_effect.GetComponentInChildren<ParticleSystem>();
		if (NetObj.m_simulating)
		{
			this.m_psystem.Play();
		}
		else
		{
			this.m_psystem.Pause();
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000467D8 File Offset: 0x000449D8
	private void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			this.m_ttl -= Time.fixedDeltaTime;
			if (this.m_ttl < this.m_fadeoutTime)
			{
				this.m_psystem.enableEmission = false;
			}
			if (this.m_ttl <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0004683C File Offset: 0x00044A3C
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_ttl);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00046910 File Offset: 0x00044B10
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_ttl = reader.ReadSingle();
		Vector3 position = default(Vector3);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		base.transform.position = position;
		Quaternion rotation = default(Quaternion);
		rotation.x = reader.ReadSingle();
		rotation.y = reader.ReadSingle();
		rotation.z = reader.ReadSingle();
		rotation.w = reader.ReadSingle();
		base.transform.rotation = rotation;
		for (int i = 0; i < 10; i++)
		{
			this.m_psystem.Simulate(1f);
		}
		this.m_psystem.Play();
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x000469E4 File Offset: 0x00044BE4
	protected override void OnSetSimulating(bool enabled)
	{
		base.OnSetSimulating(enabled);
		if (enabled)
		{
			this.m_psystem.Play();
		}
		else
		{
			this.m_psystem.Pause();
		}
	}

	// Token: 0x04000801 RID: 2049
	public float m_ttl = 60f;

	// Token: 0x04000802 RID: 2050
	public float m_fadeoutTime = 8f;

	// Token: 0x04000803 RID: 2051
	public GameObject m_effectLow;

	// Token: 0x04000804 RID: 2052
	public GameObject m_effectHigh;

	// Token: 0x04000805 RID: 2053
	private GameObject m_effect;

	// Token: 0x04000806 RID: 2054
	private ParticleSystem m_psystem;
}
