using System;
using System.IO;
using UnityEngine;

// Token: 0x020000EB RID: 235
[AddComponentMenu("Scripts/Projectiles/Projectile")]
public class Projectile : NetObj
{
	// Token: 0x0600092C RID: 2348 RVA: 0x000424E4 File Offset: 0x000406E4
	public override void Awake()
	{
		base.Awake();
		this.m_rayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("shield") | 1 << LayerMask.NameToLayer("mines"));
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0004255C File Offset: 0x0004075C
	private void SetMobileChildActivity(bool active)
	{
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00042560 File Offset: 0x00040760
	private void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			this.m_ttl -= Time.fixedDeltaTime;
			if (this.m_ttl <= 0f)
			{
				this.Remove();
			}
			this.SpecializedFixedUpdate();
			this.DoRayTest();
		}
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x000425AC File Offset: 0x000407AC
	protected virtual void SpecializedFixedUpdate()
	{
		this.m_vel.y = this.m_vel.y - this.m_gravity * Time.fixedDeltaTime;
		base.transform.position += this.m_vel * Time.fixedDeltaTime;
		this.UpdateLookDirection();
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x00042604 File Offset: 0x00040804
	private void UpdateLookDirection()
	{
		base.transform.rotation = Quaternion.LookRotation(this.m_vel);
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0004261C File Offset: 0x0004081C
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(100);
		writer.Write(this.m_gunID);
		writer.Write(base.transform.position.x);
		writer.Write(base.transform.position.y);
		writer.Write(base.transform.position.z);
		writer.Write(this.m_startPos.x);
		writer.Write(this.m_startPos.y);
		writer.Write(this.m_startPos.z);
		writer.Write(this.m_prevPos.x);
		writer.Write(this.m_prevPos.y);
		writer.Write(this.m_prevPos.z);
		writer.Write(base.transform.rotation.x);
		writer.Write(base.transform.rotation.y);
		writer.Write(base.transform.rotation.z);
		writer.Write(base.transform.rotation.w);
		writer.Write(this.m_vel.x);
		writer.Write(this.m_vel.y);
		writer.Write(this.m_vel.z);
		writer.Write(this.m_gravity);
		writer.Write(this.m_ttl);
		writer.Write(this.m_originalPower);
		writer.Write((short)this.m_damage);
		writer.Write((short)this.m_armorPiercing);
		writer.Write(this.m_splashRadius);
		writer.Write(this.m_splashDamageFactor);
		writer.Write(this.m_hasHit);
		Trace[] componentsInChildren = base.gameObject.GetComponentsInChildren<Trace>(true);
		foreach (Trace trace in componentsInChildren)
		{
			trace.Save(writer);
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00042828 File Offset: 0x00040A28
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		int num = reader.ReadInt32();
		this.m_gunID = reader.ReadInt32();
		Vector3 position = default(Vector3);
		position.x = reader.ReadSingle();
		position.y = reader.ReadSingle();
		position.z = reader.ReadSingle();
		base.transform.position = position;
		this.m_startPos.x = reader.ReadSingle();
		this.m_startPos.y = reader.ReadSingle();
		this.m_startPos.z = reader.ReadSingle();
		this.m_prevPos.x = reader.ReadSingle();
		this.m_prevPos.y = reader.ReadSingle();
		this.m_prevPos.z = reader.ReadSingle();
		Quaternion rotation = default(Quaternion);
		rotation.x = reader.ReadSingle();
		rotation.y = reader.ReadSingle();
		rotation.z = reader.ReadSingle();
		rotation.w = reader.ReadSingle();
		base.transform.rotation = rotation;
		this.m_vel.x = reader.ReadSingle();
		this.m_vel.y = reader.ReadSingle();
		this.m_vel.z = reader.ReadSingle();
		this.m_gravity = reader.ReadSingle();
		this.m_ttl = reader.ReadSingle();
		this.m_originalPower = reader.ReadSingle();
		this.m_damage = (int)reader.ReadInt16();
		this.m_armorPiercing = (int)reader.ReadInt16();
		if (num >= 100)
		{
			this.m_splashRadius = reader.ReadSingle();
			this.m_splashDamageFactor = reader.ReadSingle();
		}
		this.m_hasHit = reader.ReadBoolean();
		Trace[] componentsInChildren = base.gameObject.GetComponentsInChildren<Trace>(true);
		foreach (Trace trace in componentsInChildren)
		{
			trace.Load(reader);
		}
		this.UpdateLookDirection();
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x00042A14 File Offset: 0x00040C14
	public virtual void Setup(Vector3 dir, int owner, float vel, float gravity, Gun gun, int damage, int armorPiercing, float splashRadius, float splashDamageFactor, float minRange, float maxRange)
	{
		this.SetOwner(owner);
		this.SetVisible(gun.GetUnit().IsVisible());
		base.SetSeenByMask(gun.GetUnit().GetSeenByMask());
		this.m_startPos = base.transform.position;
		this.m_gunID = gun.GetNetID();
		this.m_dir = dir;
		this.m_prevPos = base.transform.position;
		this.m_originalPower = vel;
		this.m_damage = damage;
		this.m_armorPiercing = armorPiercing;
		this.m_splashRadius = splashRadius;
		this.m_splashDamageFactor = splashDamageFactor;
		this.m_minRange = minRange;
		this.m_maxRange = maxRange;
		this.m_vel = this.m_dir * this.m_originalPower;
		this.m_gravity = gravity;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00042AD8 File Offset: 0x00040CD8
	protected void HitEffect(Vector3 pos, Projectile.HitType type)
	{
		if (this.IsVisible())
		{
			GameObject gameObject = null;
			switch (type)
			{
			case Projectile.HitType.Normal:
				gameObject = this.m_hitEffect;
				break;
			case Projectile.HitType.Armor:
				gameObject = this.m_armorHitEffect;
				break;
			case Projectile.HitType.Ground:
				gameObject = this.m_groundHitEffect;
				break;
			case Projectile.HitType.Water:
				gameObject = this.m_waterHitEffect;
				break;
			}
			if (gameObject != null)
			{
				UnityEngine.Object.Instantiate(gameObject, pos, Quaternion.identity);
			}
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00042B58 File Offset: 0x00040D58
	public Unit GetOwnerUnit()
	{
		Gun gun = NetObj.GetByID(this.m_gunID) as Gun;
		if (gun != null)
		{
			return gun.GetUnit();
		}
		return null;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00042B8C File Offset: 0x00040D8C
	public Gun GetOwnerGun()
	{
		return NetObj.GetByID(this.m_gunID) as Gun;
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00042BA0 File Offset: 0x00040DA0
	protected void DoSplashDamage(Vector3 pos, Collider hitCollider, int damage)
	{
		if (this.m_splashDamageFactor > 0f && this.m_splashRadius > 0f)
		{
			int maxSplashDamage = (int)((float)damage * this.m_splashDamageFactor);
			GameRules.DoAreaDamage(this.GetOwnerGun(), pos, this.m_splashRadius, maxSplashDamage, this.m_armorPiercing, hitCollider);
		}
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00042BF4 File Offset: 0x00040DF4
	private void DoRayTest()
	{
		float num = Vector3.SqrMagnitude(this.m_prevPos - base.transform.position);
		if (num < 4f)
		{
			return;
		}
		Vector3 vector = base.transform.position - this.m_prevPos;
		float num2 = vector.magnitude;
		vector.Normalize();
		this.m_prevPos = base.transform.position;
		num2 *= 2f;
		Vector3 origin = base.transform.position - vector * num2;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, vector, out raycastHit, num2, this.m_rayMask))
		{
			this.OnHit(raycastHit.point, raycastHit.collider);
		}
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00042CAC File Offset: 0x00040EAC
	public static Platform GetPlatform(GameObject go)
	{
		Platform component = go.GetComponent<Platform>();
		if (component == null && go.transform.parent != null)
		{
			component = go.transform.parent.GetComponent<Platform>();
		}
		return component;
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00042CF4 File Offset: 0x00040EF4
	public static Section GetSection(GameObject go)
	{
		Section component = go.GetComponent<Section>();
		if (!component && go.transform.parent != null)
		{
			component = go.transform.parent.GetComponent<Section>();
		}
		return component;
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00042D3C File Offset: 0x00040F3C
	protected virtual void OnHit(Vector3 pos, Collider other)
	{
		if (this.m_hasHit)
		{
			return;
		}
		int damage = this.m_damage;
		float num = Vector3.Distance(this.m_startPos, pos);
		if (num < this.m_minRange || num > this.m_maxRange)
		{
			damage = 0;
		}
		Section section = Projectile.GetSection(other.gameObject);
		HPModule component = other.gameObject.GetComponent<HPModule>();
		Platform platform = Projectile.GetPlatform(other.gameObject);
		ShieldGeometry component2 = other.gameObject.GetComponent<ShieldGeometry>();
		Mine component3 = other.gameObject.GetComponent<Mine>();
		Unit ownerUnit = this.GetOwnerUnit();
		Gun ownerGun = this.GetOwnerGun();
		if (ownerUnit == null || ownerUnit == null)
		{
			this.Remove();
			return;
		}
		if (section != null)
		{
			if (section.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				if (section.Damage(new Hit(ownerGun, damage, this.m_armorPiercing, pos, this.m_vel.normalized)))
				{
					this.HitEffect(pos, Projectile.HitType.Normal);
				}
				else
				{
					this.HitEffect(pos, Projectile.HitType.Armor);
				}
				this.DoSplashDamage(pos, other, damage);
				this.Remove();
			}
		}
		else if (component3 != null)
		{
			if (component3.Damage(new Hit(ownerGun, damage, this.m_armorPiercing, pos, this.m_vel.normalized)))
			{
				this.HitEffect(component3.transform.position, Projectile.HitType.Normal);
			}
			else
			{
				this.HitEffect(component3.transform.position, Projectile.HitType.Armor);
			}
			this.Remove();
		}
		else if (component != null)
		{
			if (component.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				Hit hit = new Hit(ownerGun, damage, this.m_armorPiercing, pos, this.m_vel.normalized);
				if (component.Damage(hit, true))
				{
					Section section2 = component.GetSection();
					if (section2 != null)
					{
						section2.Damage(hit);
					}
					this.HitEffect(pos, Projectile.HitType.Normal);
				}
				else
				{
					this.HitEffect(pos, Projectile.HitType.Armor);
				}
				this.DoSplashDamage(pos, other, damage);
				this.Remove();
			}
		}
		else if (platform != null)
		{
			if (platform.GetNetID() != ownerUnit.GetNetID())
			{
				if (platform.Damage(new Hit(ownerGun, damage, this.m_armorPiercing, pos, this.m_vel.normalized)))
				{
					this.HitEffect(pos, Projectile.HitType.Normal);
				}
				else
				{
					this.HitEffect(pos, Projectile.HitType.Armor);
				}
				this.DoSplashDamage(pos, other, damage);
				this.Remove();
			}
		}
		else if (component2 != null)
		{
			if (component2.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				component2.Damage(new Hit(ownerGun, damage, this.m_armorPiercing, pos, this.m_vel.normalized), true);
				this.Remove();
			}
		}
		else
		{
			if (other.tag == "Water")
			{
				this.HitEffect(pos, Projectile.HitType.Water);
			}
			else
			{
				this.HitEffect(pos, Projectile.HitType.Ground);
			}
			this.DoSplashDamage(pos, null, damage);
			this.Remove();
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x0004305C File Offset: 0x0004125C
	protected void Remove()
	{
		this.m_hasHit = true;
		Trace componentInChildren = base.GetComponentInChildren<Trace>();
		if (componentInChildren != null)
		{
			componentInChildren.Die();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00043094 File Offset: 0x00041294
	public override void SetVisible(bool visible)
	{
		if (this.IsVisible() == visible)
		{
			return;
		}
		base.SetVisible(visible);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
		Trace componentInChildren = base.GetComponentInChildren<Trace>();
		if (componentInChildren != null)
		{
			componentInChildren.SetVisible(visible);
		}
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00043100 File Offset: 0x00041300
	protected override void OnSetSimulating(bool enabled)
	{
		ParticleEmitter componentInChildren = base.gameObject.GetComponentInChildren<ParticleEmitter>();
		if (componentInChildren)
		{
			componentInChildren.enabled = enabled;
		}
	}

	// Token: 0x0400075F RID: 1887
	public GameObject m_hitEffect;

	// Token: 0x04000760 RID: 1888
	public GameObject m_armorHitEffect;

	// Token: 0x04000761 RID: 1889
	public GameObject m_waterHitEffect;

	// Token: 0x04000762 RID: 1890
	public GameObject m_groundHitEffect;

	// Token: 0x04000763 RID: 1891
	public GameObject m_hitEffectLow;

	// Token: 0x04000764 RID: 1892
	public GameObject m_armorHitEffectLow;

	// Token: 0x04000765 RID: 1893
	public GameObject m_waterHitEffectLow;

	// Token: 0x04000766 RID: 1894
	public GameObject m_groundHitEffectLow;

	// Token: 0x04000767 RID: 1895
	private float m_ttl = 60f;

	// Token: 0x04000768 RID: 1896
	protected Vector3 m_startPos;

	// Token: 0x04000769 RID: 1897
	protected Vector3 m_dir = Vector3.one;

	// Token: 0x0400076A RID: 1898
	protected Vector3 m_vel = Vector3.one;

	// Token: 0x0400076B RID: 1899
	protected Vector3 m_prevPos = Vector3.zero;

	// Token: 0x0400076C RID: 1900
	protected float m_originalPower;

	// Token: 0x0400076D RID: 1901
	protected int m_gunID;

	// Token: 0x0400076E RID: 1902
	protected float m_gravity = -1f;

	// Token: 0x0400076F RID: 1903
	protected int m_damage;

	// Token: 0x04000770 RID: 1904
	protected int m_armorPiercing;

	// Token: 0x04000771 RID: 1905
	protected float m_splashRadius;

	// Token: 0x04000772 RID: 1906
	protected float m_splashDamageFactor;

	// Token: 0x04000773 RID: 1907
	protected float m_minRange;

	// Token: 0x04000774 RID: 1908
	protected float m_maxRange = 10000f;

	// Token: 0x04000775 RID: 1909
	protected bool m_hasHit;

	// Token: 0x04000776 RID: 1910
	private int m_rayMask;

	// Token: 0x020000EC RID: 236
	protected enum HitType
	{
		// Token: 0x04000778 RID: 1912
		Normal,
		// Token: 0x04000779 RID: 1913
		Armor,
		// Token: 0x0400077A RID: 1914
		Ground,
		// Token: 0x0400077B RID: 1915
		Water
	}
}
