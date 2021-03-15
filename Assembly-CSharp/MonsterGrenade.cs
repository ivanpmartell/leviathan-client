using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class MonsterGrenade : Deployable
{
	// Token: 0x060008D5 RID: 2261 RVA: 0x0004064C File Offset: 0x0003E84C
	public override void Awake()
	{
		base.Awake();
		if (this.m_ttl == -1f)
		{
			this.m_ttl = this.m_lifeTime;
		}
		this.m_effect = (UnityEngine.Object.Instantiate(this.m_effectHigh, base.transform.position, Quaternion.identity) as GameObject);
		this.m_effect.transform.parent = base.transform;
		this.m_psystem = this.m_effect.GetComponentInChildren<ParticleSystem>();
		this.m_rayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("units"));
		this.m_whirlMaterial = this.m_effect.transform.FindChild("whirl").renderer.material;
		this.UpdateWhirl();
		this.Pause(NetObj.m_simulating);
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00040724 File Offset: 0x0003E924
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
		if (this.m_head)
		{
			if (!NetObj.m_simulating)
			{
				this.m_head["leviathan_idle"].speed = 0f;
			}
			else
			{
				this.m_head["leviathan_idle"].speed = 1f;
			}
		}
		if (this.m_ttl < this.m_fadeoutTime)
		{
			this.m_psystem.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x00040868 File Offset: 0x0003EA68
	private void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			this.m_spawnTimer -= Time.fixedDeltaTime;
			this.m_ttl -= Time.fixedDeltaTime;
			this.UpdateHead(Time.fixedDeltaTime);
			this.UpdateArms(Time.fixedDeltaTime);
			this.UpdateWhirl();
			if (this.m_ttl < this.m_fadeoutTime)
			{
				float num = 1f - this.m_ttl / this.m_fadeoutTime;
				Vector3 position = base.transform.position;
				position.y = -20f * num;
				base.transform.position = position;
			}
			if (this.m_ttl <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00040928 File Offset: 0x0003EB28
	private void UpdateWhirl()
	{
		float value = 1f;
		if (this.m_ttl <= this.m_fadeoutTime)
		{
			value = this.m_ttl / this.m_fadeoutTime;
		}
		if (this.m_ttl >= this.m_lifeTime - this.m_fadeinTime)
		{
			value = (this.m_lifeTime - this.m_ttl) / this.m_fadeinTime;
		}
		this.m_whirlMaterial.SetFloat("_Opacity", value);
		this.m_whirlMaterial.SetFloat("_SimTime", this.m_ttl);
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x000409B0 File Offset: 0x0003EBB0
	private void UpdateArms(float dt)
	{
		if (this.m_spawnTimer < 0f && this.m_ttl > this.m_fadeoutTime)
		{
			this.m_spawnTimer = UnityEngine.Random.Range(this.m_spawnDelayMin, this.m_spawnDelayMax);
			this.SpawnBodyPart();
		}
		this.RemoveOldBodyParts();
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00040A04 File Offset: 0x0003EC04
	private void UpdateHead(float dt)
	{
		if (this.m_ttl < this.m_fadeoutTime)
		{
			if (this.m_headActive)
			{
				this.DespawnHead();
			}
			return;
		}
		this.m_headUpdate += dt;
		if (this.m_headUpdate > 1f)
		{
			this.m_headUpdate = 0f;
			if (!this.m_headActive)
			{
				if (this.m_head && !this.m_head.isPlaying)
				{
					UnityEngine.Object.Destroy(this.m_head);
					this.m_head = null;
				}
				Quaternion quaternion = Quaternion.Euler(0f, (float)PRand.Range(0, 360), 0f);
				Vector3 b = quaternion * new Vector3(0f, 0f, -this.m_effectRadius);
				Vector3 pos = base.transform.position + b;
				if (!this.Blocked(pos))
				{
					this.SpawnHead(pos, quaternion, true);
				}
			}
			else if (this.Blocked(this.m_head.transform.position))
			{
				this.DespawnHead();
			}
		}
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x00040B28 File Offset: 0x0003ED28
	private bool Blocked(Vector3 pos)
	{
		return Physics.CheckSphere(pos, 5f, this.m_rayMask);
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x00040B44 File Offset: 0x0003ED44
	private void RemoveOldBodyParts()
	{
		for (int i = 0; i < this.m_bodyParts.Count; i++)
		{
			if (!this.m_bodyParts[i].isPlaying)
			{
				UnityEngine.Object.Destroy(this.m_bodyParts[i].gameObject);
				this.m_bodyParts.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00040BA8 File Offset: 0x0003EDA8
	private void SpawnBodyPart()
	{
		this.SpawnTail();
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00040BB0 File Offset: 0x0003EDB0
	private void SpawnHead(Vector3 pos, Quaternion rot, bool firstTime)
	{
		this.m_headActive = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_headPrefab, pos, rot) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_head = gameObject.animation;
		if (firstTime)
		{
			gameObject.animation.CrossFade("leviathan_ascend", 0f);
			gameObject.animation.CrossFadeQueued("leviathan_idle", 0.2f);
			UnityEngine.Object.Instantiate(this.m_spawnEffectHigh, pos, Quaternion.identity);
		}
		else
		{
			gameObject.animation.CrossFade("leviathan_idle", 0f);
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x00040C54 File Offset: 0x0003EE54
	private void DespawnHead()
	{
		this.m_headActive = false;
		if (this.m_head == null)
		{
			return;
		}
		this.m_head.CrossFade("leviathan_descend", 0.2f);
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x00040C90 File Offset: 0x0003EE90
	private void SpawnBody()
	{
		Vector3 b = new Vector3(UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius), -13f, UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius));
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f));
		this.FixFacing(ref rotation);
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_bodyPrefab, base.transform.position + b, rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_bodyParts.Add(gameObject.animation);
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00040D3C File Offset: 0x0003EF3C
	private void FixFacing(ref Quaternion rot)
	{
		if (this.m_head != null)
		{
			float num = Quaternion.Dot(rot, this.m_head.transform.rotation);
			if (num < 0f)
			{
				rot *= Quaternion.Euler(0f, 180f, 0f);
			}
		}
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x00040DA8 File Offset: 0x0003EFA8
	private void SpawnTail()
	{
		Vector3 b = new Vector3(UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius), -13f, UnityEngine.Random.Range(-this.m_effectRadius, this.m_effectRadius));
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f));
		this.FixFacing(ref rotation);
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_tailPrefab, base.transform.position + b, rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_bodyParts.Add(gameObject.animation);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x00040E54 File Offset: 0x0003F054
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

	// Token: 0x060008E4 RID: 2276 RVA: 0x00040E8C File Offset: 0x0003F08C
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_ttl);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(this.m_headActive);
		writer.Write(this.m_headUpdate);
		if (this.m_headActive)
		{
			writer.Write(this.m_head.transform.position.x);
			writer.Write(this.m_head.transform.position.y);
			writer.Write(this.m_head.transform.position.z);
			writer.Write(this.m_head.transform.rotation.x);
			writer.Write(this.m_head.transform.rotation.y);
			writer.Write(this.m_head.transform.rotation.z);
			writer.Write(this.m_head.transform.rotation.w);
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00040FF4 File Offset: 0x0003F1F4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_ttl = reader.ReadSingle();
		Vector3 position = default(Vector3);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		base.transform.position = position;
		this.m_headActive = reader.ReadBoolean();
		this.m_headUpdate = reader.ReadSingle();
		if (this.m_headActive)
		{
			Vector3 pos = default(Vector3);
			pos.x = reader.ReadSingle();
			pos.y = reader.ReadSingle();
			pos.z = reader.ReadSingle();
			Quaternion rot = new Quaternion
			{
				x = reader.ReadSingle(),
				y = reader.ReadSingle(),
				z = reader.ReadSingle(),
				w = reader.ReadSingle()
			};
			this.SpawnHead(pos, rot, false);
		}
		for (int i = 0; i < 10; i++)
		{
			this.m_psystem.Simulate(1f);
		}
		this.m_psystem.Play();
		this.UpdateWhirl();
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0004111C File Offset: 0x0003F31C
	protected override void OnSetSimulating(bool enabled)
	{
		base.OnSetSimulating(enabled);
		this.Pause(NetObj.m_simulating);
	}

	// Token: 0x04000719 RID: 1817
	public float m_lifeTime = 30f;

	// Token: 0x0400071A RID: 1818
	public float m_fadeoutTime = 8f;

	// Token: 0x0400071B RID: 1819
	public float m_fadeinTime = 1f;

	// Token: 0x0400071C RID: 1820
	public float m_effectRadius = 20f;

	// Token: 0x0400071D RID: 1821
	public float m_spawnDelayMin = 0.9f;

	// Token: 0x0400071E RID: 1822
	public float m_spawnDelayMax = 1.2f;

	// Token: 0x0400071F RID: 1823
	public GameObject m_effectLow;

	// Token: 0x04000720 RID: 1824
	public GameObject m_effectHigh;

	// Token: 0x04000721 RID: 1825
	public GameObject m_spawnEffectHigh;

	// Token: 0x04000722 RID: 1826
	public GameObject m_spawnEffectLow;

	// Token: 0x04000723 RID: 1827
	public GameObject m_bodyPrefab;

	// Token: 0x04000724 RID: 1828
	public GameObject m_headPrefab;

	// Token: 0x04000725 RID: 1829
	public GameObject m_tailPrefab;

	// Token: 0x04000726 RID: 1830
	private List<Animation> m_bodyParts = new List<Animation>();

	// Token: 0x04000727 RID: 1831
	private Material m_whirlMaterial;

	// Token: 0x04000728 RID: 1832
	private Animation m_head;

	// Token: 0x04000729 RID: 1833
	private bool m_headActive;

	// Token: 0x0400072A RID: 1834
	private float m_headUpdate;

	// Token: 0x0400072B RID: 1835
	private float m_ttl = -1f;

	// Token: 0x0400072C RID: 1836
	private GameObject m_effect;

	// Token: 0x0400072D RID: 1837
	private ParticleSystem m_psystem;

	// Token: 0x0400072E RID: 1838
	private float m_spawnTimer;

	// Token: 0x0400072F RID: 1839
	private int m_rayMask;
}
