using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017D RID: 381
internal class GameRules
{
	// Token: 0x06000E4A RID: 3658 RVA: 0x00064C10 File Offset: 0x00062E10
	public static GameRules.HitOutcome CalculateDamage(int health, int armorClass, int damage, int armorPiercing, out int healthDamage)
	{
		if (damage <= 0)
		{
			healthDamage = 0;
			return GameRules.HitOutcome.Deflected;
		}
		if (armorClass < 1)
		{
			armorClass = 1;
		}
		if (armorPiercing >= armorClass)
		{
			int num = armorPiercing - armorClass;
			float num2 = (float)(num / armorClass);
			bool flag = PRand.Value() <= num2;
			if (flag)
			{
				healthDamage = damage * 2;
			}
			else
			{
				healthDamage = damage;
			}
			if (healthDamage > health)
			{
				healthDamage = health;
			}
			return (!flag) ? GameRules.HitOutcome.PiercedArmor : GameRules.HitOutcome.CritHit;
		}
		float num3 = Mathf.Clamp((float)armorPiercing / (float)armorClass, 0f, 1f);
		bool flag2 = PRand.Value() <= num3;
		if (flag2)
		{
			float num4 = (float)armorPiercing / (float)armorClass * (float)damage;
			int num5 = PRand.Range(1, (int)num4);
			healthDamage = num5;
			if (healthDamage > health)
			{
				healthDamage = health;
			}
			return GameRules.HitOutcome.GlancingHit;
		}
		healthDamage = 0;
		return GameRules.HitOutcome.Deflected;
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00064CD8 File Offset: 0x00062ED8
	public static void DoAreaDamage(Gun dealer, Vector3 center, float radius, int maxSplashDamage, int armorPiercing, Collider hitCollider)
	{
		int layerMask = 1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("hpmodules");
		Collider[] array = Physics.OverlapSphere(center, radius, layerMask);
		HashSet<Unit> hashSet = new HashSet<Unit>();
		if (hitCollider != null)
		{
			Section section = Projectile.GetSection(hitCollider.gameObject);
			if (section != null)
			{
				hashSet.Add(section.GetUnit());
			}
			Platform platform = Projectile.GetPlatform(hitCollider.gameObject);
			if (platform != null)
			{
				hashSet.Add(platform);
			}
		}
		foreach (Collider collider in array)
		{
			Vector3 a = collider.ClosestPointOnBounds(center);
			float num = Vector3.Distance(a, center);
			float num2 = 1f - Mathf.Clamp01(num / radius);
			int num3 = (int)(num2 * (float)maxSplashDamage);
			if (num3 > 0)
			{
				Section section2 = Projectile.GetSection(collider.gameObject);
				if (section2 != null)
				{
					if (!hashSet.Contains(section2.GetUnit()))
					{
						hashSet.Add(section2.GetUnit());
						section2.Damage(new Hit(dealer, num3, armorPiercing));
					}
				}
				else
				{
					Platform platform2 = Projectile.GetPlatform(collider.gameObject);
					if (platform2 != null)
					{
						if (!hashSet.Contains(platform2))
						{
							hashSet.Add(platform2);
							platform2.Damage(new Hit(dealer, num3, armorPiercing));
						}
					}
					else
					{
						HPModule component = collider.gameObject.GetComponent<HPModule>();
						if (component != null)
						{
							component.Damage(new Hit(dealer, num3, armorPiercing), true);
						}
					}
				}
			}
		}
	}

	// Token: 0x0200017E RID: 382
	public enum HitOutcome
	{
		// Token: 0x04000B6F RID: 2927
		CritHit,
		// Token: 0x04000B70 RID: 2928
		PiercedArmor,
		// Token: 0x04000B71 RID: 2929
		GlancingHit,
		// Token: 0x04000B72 RID: 2930
		Deflected
	}
}
