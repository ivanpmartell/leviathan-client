using System;
using System.IO;
using UnityEngine;

// Token: 0x020000E1 RID: 225
[AddComponentMenu("Scripts/Weapons/Mine")]
public class Mine : Unit
{
	// Token: 0x060008A0 RID: 2208 RVA: 0x0003FA44 File Offset: 0x0003DC44
	public virtual void Setup(int ownerID, int gunID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		this.m_gunID = gunID;
		this.SetOwner(ownerID);
		this.SetVisible(visible);
		base.SetSeenByMask(seenByMask);
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0003FA64 File Offset: 0x0003DC64
	public override void Awake()
	{
		base.Awake();
		GameObject gameObject = GameObject.Find("WaterSurface");
		if (gameObject != null)
		{
			this.m_waterSurface = gameObject.GetComponent<WaterSurface>();
		}
		this.m_armTimer = this.m_armDelay;
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0003FAA8 File Offset: 0x0003DCA8
	protected override void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			if (this.m_ttl != 0f)
			{
				this.m_ttl -= Time.fixedDeltaTime;
				if (this.m_ttl <= 0f)
				{
					this.OnTimeout();
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
			}
			if (this.m_waterSurface != null)
			{
				Vector3 position = base.transform.position;
				position.y = this.m_waterSurface.GetWorldWaveHeight(position);
				base.transform.position = position;
			}
			if (this.m_armTimer > 0f)
			{
				this.m_armTimer -= Time.fixedDeltaTime;
			}
			if (!this.m_deployed && this.m_armTimer <= 0f)
			{
				this.m_deployed = true;
				this.OnDeploy();
			}
		}
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0003FB8C File Offset: 0x0003DD8C
	protected virtual void OnTimeout()
	{
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0003FB90 File Offset: 0x0003DD90
	protected virtual void OnDeploy()
	{
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0003FB94 File Offset: 0x0003DD94
	public override bool IsValidTarget()
	{
		return false;
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0003FB98 File Offset: 0x0003DD98
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_deployed);
		writer.Write(this.m_armTimer);
		writer.Write(this.m_gunID);
		writer.Write(this.m_ttl);
		writer.Write(this.m_health);
		writer.Write(this.m_armorClass);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0003FBF4 File Offset: 0x0003DDF4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_deployed = reader.ReadBoolean();
		this.m_armTimer = reader.ReadSingle();
		this.m_gunID = reader.ReadInt32();
		this.m_ttl = reader.ReadSingle();
		this.m_health = reader.ReadInt32();
		this.m_armorClass = reader.ReadInt32();
		if (this.m_deployed)
		{
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0003FC5C File Offset: 0x0003DE5C
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
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0003FCAC File Offset: 0x0003DEAC
	public float GetArmTimer()
	{
		return this.m_armTimer;
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0003FCB4 File Offset: 0x0003DEB4
	public bool IsDeployed()
	{
		return this.m_deployed;
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0003FCBC File Offset: 0x0003DEBC
	public override float GetWidth()
	{
		return 2f;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0003FCC4 File Offset: 0x0003DEC4
	public override float GetLength()
	{
		return 2f;
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0003FCCC File Offset: 0x0003DECC
	public override bool Damage(Hit hit)
	{
		if (this.m_health <= 0)
		{
			return true;
		}
		int num;
		GameRules.HitOutcome hitOutcome = GameRules.CalculateDamage(this.m_health, this.m_armorClass, hit.m_damage, hit.m_armorPiercing, out num);
		this.m_health -= num;
		if (this.m_health <= 0)
		{
			this.Explode();
		}
		if (this.IsVisible())
		{
			switch (hitOutcome)
			{
			case GameRules.HitOutcome.CritHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipCriticalHit);
				break;
			case GameRules.HitOutcome.PiercedArmor:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipPiercingHit);
				break;
			case GameRules.HitOutcome.GlancingHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipGlancingHit);
				break;
			case GameRules.HitOutcome.Deflected:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipDeflectHit);
				break;
			}
			if (this.m_health <= 0)
			{
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipDestroyedHit);
			}
		}
		return true;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0003FE20 File Offset: 0x0003E020
	public void Disarm()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		HitText.instance.AddDmgText(-1, base.gameObject.transform.position, "Mine Dissarmed", Constants.m_shipCriticalHit);
		if (this.m_disarmEffect != null)
		{
			UnityEngine.Object.Instantiate(this.m_disarmEffect, base.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0003FE8C File Offset: 0x0003E08C
	protected void Explode()
	{
		if (this.m_hitEffect != null && this.IsVisible())
		{
			UnityEngine.Object.Instantiate(this.m_hitEffect, base.transform.position, Quaternion.identity);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x04000706 RID: 1798
	public float m_ttl;

	// Token: 0x04000707 RID: 1799
	public int m_health = 20;

	// Token: 0x04000708 RID: 1800
	public int m_armorClass = 8;

	// Token: 0x04000709 RID: 1801
	public GameObject m_hitEffect;

	// Token: 0x0400070A RID: 1802
	public GameObject m_disarmEffect;

	// Token: 0x0400070B RID: 1803
	public GameObject m_disarmEffectLow;

	// Token: 0x0400070C RID: 1804
	public float m_armDelay = 20f;

	// Token: 0x0400070D RID: 1805
	protected bool m_deployed;

	// Token: 0x0400070E RID: 1806
	protected int m_gunID;

	// Token: 0x0400070F RID: 1807
	private float m_armTimer;

	// Token: 0x04000710 RID: 1808
	private WaterSurface m_waterSurface;
}
