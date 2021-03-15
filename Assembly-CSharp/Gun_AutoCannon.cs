using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D5 RID: 213
[AddComponentMenu("Scripts/Modules/Gun_AutoCannon")]
public class Gun_AutoCannon : Gun
{
	// Token: 0x0600080A RID: 2058 RVA: 0x0003BF28 File Offset: 0x0003A128
	protected override bool FireProjectile(Vector3 targetPos)
	{
		int num = this.SetUpProjectiles(base.GetMuzzlePos(), targetPos, 9999);
		return num > 0;
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0003BF54 File Offset: 0x0003A154
	protected int SetUpProjectiles(Vector3 muzzlePos, Vector3 targetPos, int max)
	{
		float num = Vector3.Distance(muzzlePos, targetPos);
		float range = (!this.m_aim.m_spreadIgnoresRange) ? this.m_aim.m_maxRange : num;
		Quaternion randomSpreadDirection = base.GetRandomSpreadDirection(0f, range);
		Quaternion rotation;
		if (base.FindOptimalFireDir(muzzlePos, targetPos, out rotation))
		{
			Vector3 dir = rotation * (randomSpreadDirection * Vector3.forward);
			GameObject gameObject = ObjectFactory.Clone(this.m_projectile, muzzlePos, this.m_elevationJoint[0].rotation);
			Projectile component = gameObject.GetComponent<Projectile>();
			component.Setup(dir, base.GetOwner(), this.m_muzzleVel, this.m_gravity, this, this.GetRandomDamage(), this.m_Damage.m_armorPiercing, this.m_Damage.m_splashRadius, this.m_Damage.m_splashDamageFactor, this.m_aim.m_minRange, this.m_aim.m_maxRange);
			return 1;
		}
		return 0;
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0003C03C File Offset: 0x0003A23C
	public override Dictionary<string, string> GetShipEditorInfo()
	{
		return base.GetShipEditorInfo();
	}
}
