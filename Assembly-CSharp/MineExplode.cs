using System;
using System.IO;
using UnityEngine;

// Token: 0x020000E4 RID: 228
[AddComponentMenu("Scripts/Weapons/MineExplode")]
public class MineExplode : Mine
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x000401E0 File Offset: 0x0003E3E0
	public override void Setup(int ownerID, int unitID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		base.Setup(ownerID, unitID, visible, seenByMask, damage, ap, splashRadius, splashDmgFactor);
		this.m_damage = damage;
		this.m_armorPiercing = ap;
		this.m_splashRadius = splashRadius;
		this.m_splashDamageFactor = splashDmgFactor;
		this.m_radiusMesh.renderer.enabled = false;
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00040234 File Offset: 0x0003E434
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x0004023C File Offset: 0x0003E43C
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00040244 File Offset: 0x0003E444
	public override void Update()
	{
		base.Update();
		this.m_radiusMesh.transform.Rotate(0f, Time.deltaTime * 50f, 0f);
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0004027C File Offset: 0x0003E47C
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write((short)this.m_damage);
		writer.Write((short)this.m_armorPiercing);
		writer.Write(this.m_splashRadius);
		writer.Write(this.m_splashDamageFactor);
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x000402C4 File Offset: 0x0003E4C4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_damage = (int)reader.ReadInt16();
		this.m_armorPiercing = (int)reader.ReadInt16();
		this.m_splashRadius = reader.ReadSingle();
		this.m_splashDamageFactor = reader.ReadSingle();
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x00040308 File Offset: 0x0003E508
	protected override void OnDeploy()
	{
		base.OnDeploy();
		if (this.IsVisible())
		{
			this.m_radiusMesh.renderer.enabled = true;
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00040338 File Offset: 0x0003E538
	private void OnTriggerEnter(Collider other)
	{
		if (!NetObj.m_simulating)
		{
			return;
		}
		Section component = other.GetComponent<Section>();
		if (!component && other.gameObject.transform.parent != null)
		{
			component = other.gameObject.transform.parent.GetComponent<Section>();
		}
		if (component != null)
		{
			if (NetObj.m_phase == TurnPhase.Testing && !this.IsSeenByPlayer(NetObj.m_localPlayerID))
			{
				return;
			}
			if (base.GetArmTimer() <= 0f)
			{
				Gun dealer = NetObj.GetByID(this.m_gunID) as Gun;
				int maxSplashDamage = (int)((float)this.m_damage * this.m_splashDamageFactor);
				component.Damage(new Hit(dealer, this.m_damage, this.m_armorPiercing, base.transform.position, (other.transform.position - base.transform.position).normalized));
				GameRules.DoAreaDamage(dealer, base.transform.position, this.m_splashRadius, maxSplashDamage, this.m_armorPiercing, other);
				if (this.m_hitEffect != null && this.IsVisible())
				{
					UnityEngine.Object.Instantiate(this.m_hitEffect, base.transform.position, Quaternion.identity);
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00040494 File Offset: 0x0003E694
	public float GetVisibleDistance()
	{
		return this.m_visibleDistance;
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0004049C File Offset: 0x0003E69C
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (visible)
		{
			this.m_radiusMesh.renderer.enabled = this.m_deployed;
		}
		else
		{
			this.m_radiusMesh.renderer.enabled = false;
		}
	}

	// Token: 0x04000712 RID: 1810
	public int m_damage = 500;

	// Token: 0x04000713 RID: 1811
	public int m_armorPiercing = 25;

	// Token: 0x04000714 RID: 1812
	public float m_splashRadius = 10f;

	// Token: 0x04000715 RID: 1813
	public float m_splashDamageFactor = 0.5f;

	// Token: 0x04000716 RID: 1814
	public float m_visibleDistance = 10f;

	// Token: 0x04000717 RID: 1815
	public GameObject m_radiusMesh;
}
