using System;
using UnityEngine;

// Token: 0x020000ED RID: 237
[AddComponentMenu("Scripts/Projectiles/Projectile_Deploy")]
public class Projectile_Deploy : Projectile
{
	// Token: 0x06000940 RID: 2368 RVA: 0x00043134 File Offset: 0x00041334
	protected override void OnHit(Vector3 pos, Collider other)
	{
		if (this.m_hasHit)
		{
			return;
		}
		Section component = other.gameObject.GetComponent<Section>();
		HPModule component2 = other.gameObject.GetComponent<HPModule>();
		ShieldGeometry component3 = other.gameObject.GetComponent<ShieldGeometry>();
		if (!component && other.gameObject.transform.parent != null)
		{
			component = other.gameObject.transform.parent.GetComponent<Section>();
		}
		Unit ownerUnit = base.GetOwnerUnit();
		if (component != null)
		{
			if (ownerUnit != null && component.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				base.HitEffect(pos, Projectile.HitType.Armor);
				base.Remove();
			}
		}
		else if (component2 != null)
		{
			if (ownerUnit != null && component2.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				base.HitEffect(pos, Projectile.HitType.Armor);
				base.Remove();
			}
		}
		else if (component3 != null)
		{
			if (component3.GetUnit().GetNetID() != ownerUnit.GetNetID())
			{
				base.HitEffect(pos, Projectile.HitType.Armor);
				base.Remove();
			}
		}
		else
		{
			if (other.tag == "Water")
			{
				base.HitEffect(pos, Projectile.HitType.Water);
				this.Deploy(pos);
			}
			else
			{
				base.HitEffect(pos, Projectile.HitType.Ground);
			}
			base.Remove();
		}
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x000432A4 File Offset: 0x000414A4
	private void Deploy(Vector3 pos)
	{
		if (this.m_objectPrefab != null)
		{
			GameObject gameObject = ObjectFactory.Clone(this.m_objectPrefab, pos, Quaternion.Euler(new Vector3(0f, (float)PRand.Range(0, 360), 0f)));
			Deployable component = gameObject.GetComponent<Deployable>();
			if (component != null)
			{
				component.Setup(base.GetOwner(), this.m_gunID, this.IsVisible(), base.GetSeenByMask(), this.m_damage, this.m_armorPiercing, this.m_splashRadius, this.m_splashDamageFactor);
			}
			Mine component2 = gameObject.GetComponent<Mine>();
			if (component2 != null)
			{
				component2.Setup(base.GetOwner(), this.m_gunID, this.IsVisible(), base.GetSeenByMask(), this.m_damage, this.m_armorPiercing, this.m_splashRadius, this.m_splashDamageFactor);
			}
		}
	}

	// Token: 0x0400077C RID: 1916
	public GameObject m_objectPrefab;
}
