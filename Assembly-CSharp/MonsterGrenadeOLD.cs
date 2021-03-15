using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class MonsterGrenadeOLD : Deployable
{
	// Token: 0x060008E8 RID: 2280 RVA: 0x00041188 File Offset: 0x0003F388
	public override void Awake()
	{
		base.Awake();
		this.m_effect = (UnityEngine.Object.Instantiate(this.m_effectHigh, base.transform.position, Quaternion.identity) as GameObject);
		this.m_effect.transform.parent = base.transform;
		this.m_psystem = this.m_effect.GetComponentInChildren<ParticleSystem>();
		this.Pause(NetObj.m_simulating);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000411F4 File Offset: 0x0003F3F4
	private void Pause(bool simulating)
	{
		Animation[] componentsInChildren = base.transform.GetComponentsInChildren<Animation>();
		if (simulating)
		{
			this.m_psystem.Play();
			foreach (Animation animation in componentsInChildren)
			{
				if (animation["rolling"])
				{
					animation["rolling"].speed = 1f;
				}
			}
		}
		else
		{
			this.m_psystem.Pause();
			foreach (Animation animation2 in componentsInChildren)
			{
				if (animation2["rolling"])
				{
					animation2["rolling"].speed = 0f;
				}
			}
		}
		if (this.m_ttl < this.m_fadeoutTime)
		{
			this.m_psystem.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x000412E4 File Offset: 0x0003F4E4
	private void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			this.m_spawnTimer -= Time.fixedDeltaTime;
			this.m_ttl -= Time.fixedDeltaTime;
			if (this.m_spawnTimer < 0f)
			{
				this.m_spawnTimer = UnityEngine.Random.Range(this.m_spawnDelayMin, this.m_spawnDelayMax);
				this.SpawnBodyPart();
			}
			if (this.m_ttl < this.m_fadeoutTime)
			{
				ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem particleSystem in componentsInChildren)
				{
					particleSystem.enableEmission = false;
				}
				foreach (Animation animation in this.m_bodyParts)
				{
					animation.transform.position -= new Vector3(0f, Time.fixedDeltaTime, 0f);
				}
			}
			this.RemoveOldBodyParts();
			if (this.m_ttl <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x0004142C File Offset: 0x0003F62C
	private void RemoveOldBodyParts()
	{
		for (int i = 0; i < this.m_bodyParts.Count; i++)
		{
			if (!this.m_bodyParts[i].isPlaying)
			{
				PLog.Log("Removing old body part " + i);
				UnityEngine.Object.Destroy(this.m_bodyParts[i].gameObject);
				this.m_bodyParts.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x000414A4 File Offset: 0x0003F6A4
	private void SpawnBodyPart()
	{
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			this.SpawnHead();
			break;
		case 1:
			this.SpawnBody();
			break;
		case 2:
			this.SpawnTail();
			break;
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x000414F4 File Offset: 0x0003F6F4
	private void SpawnHead()
	{
		Vector3 b = new Vector3(UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius), -9f, UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius));
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f));
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_headPrefab, base.transform.position + b, rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_bodyParts.Add(gameObject.animation);
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00041598 File Offset: 0x0003F798
	private void SpawnBody()
	{
		Vector3 b = new Vector3(UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius), -13f, UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius));
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f));
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_bodyPrefab, base.transform.position + b, rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_bodyParts.Add(gameObject.animation);
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0004163C File Offset: 0x0003F83C
	private void SpawnTail()
	{
		Vector3 b = new Vector3(UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius), -13f, UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius));
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f));
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_tailPrefab, base.transform.position + b, rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_bodyParts.Add(gameObject.animation);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x000416E0 File Offset: 0x0003F8E0
	private void OnTriggerStay(Collider other)
	{
		Section component = other.GetComponent<Section>();
		if (!component)
		{
			return;
		}
		Ship ship = component.GetUnit() as Ship;
		ship.SetInMonsterMineField(base.gameObject);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00041718 File Offset: 0x0003F918
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

	// Token: 0x060008F2 RID: 2290 RVA: 0x000417EC File Offset: 0x0003F9EC
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

	// Token: 0x060008F3 RID: 2291 RVA: 0x000418C0 File Offset: 0x0003FAC0
	protected override void OnSetSimulating(bool enabled)
	{
		base.OnSetSimulating(enabled);
		this.Pause(NetObj.m_simulating);
	}

	// Token: 0x04000730 RID: 1840
	public float m_ttl = 30f;

	// Token: 0x04000731 RID: 1841
	public float m_fadeoutTime = 8f;

	// Token: 0x04000732 RID: 1842
	public float m_effectRadius = 20f;

	// Token: 0x04000733 RID: 1843
	public float m_spawnDelayMin = 0.9f;

	// Token: 0x04000734 RID: 1844
	public float m_spawnDelayMax = 1.2f;

	// Token: 0x04000735 RID: 1845
	public GameObject m_effectLow;

	// Token: 0x04000736 RID: 1846
	public GameObject m_effectHigh;

	// Token: 0x04000737 RID: 1847
	public GameObject m_bodyPrefab;

	// Token: 0x04000738 RID: 1848
	public GameObject m_headPrefab;

	// Token: 0x04000739 RID: 1849
	public GameObject m_tailPrefab;

	// Token: 0x0400073A RID: 1850
	private List<Animation> m_bodyParts = new List<Animation>();

	// Token: 0x0400073B RID: 1851
	private GameObject m_effect;

	// Token: 0x0400073C RID: 1852
	private ParticleSystem m_psystem;

	// Token: 0x0400073D RID: 1853
	private float m_spawnTimer;
}
