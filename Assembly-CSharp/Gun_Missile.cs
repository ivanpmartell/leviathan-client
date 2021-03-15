using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[AddComponentMenu("Scripts/Modules/Gun_Missile")]
public class Gun_Missile : Gun
{
	// Token: 0x0600081F RID: 2079 RVA: 0x0003CA3C File Offset: 0x0003AC3C
	public Gun_Missile()
	{
		this.m_aim.m_stance = Gun.Stance.HoldFire;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0003CAB4 File Offset: 0x0003ACB4
	protected override bool FireProjectile(Vector3 targetPos)
	{
		int num = this.SetUpProjectiles(base.GetMuzzlePos(), targetPos, 9999);
		if (num > 0)
		{
			this.ClearOrders();
			return true;
		}
		return false;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0003CAE4 File Offset: 0x0003ACE4
	public override float GetTargetRadius(Vector3 targetPos)
	{
		return this.m_accurateRadius;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0003CAEC File Offset: 0x0003ACEC
	private void RandomTargetPos(ref Vector3 target)
	{
		float num = PRand.Range(1f, this.m_accurateRadius);
		float f = PRand.Range(1f, 359f) * 0.017453292f;
		float num2 = Mathf.Cos(f) * num;
		float num3 = Mathf.Sin(f) * num;
		target.x += num2;
		target.z += num3;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0003CB50 File Offset: 0x0003AD50
	private void ModifyTarget(Vector3 muzzlePos, ref Vector3 targetPos)
	{
		int num = this.m_accuracy;
		float num2 = Mathf.Clamp(this.m_accuracyScaleWithLength, 0f, 100f);
		if (num2 != 0f)
		{
			float num3 = Mathf.Abs(Vector3.Distance(muzzlePos, targetPos)) / this.m_aim.m_maxRange;
			num = (int)((float)num - num2 * num3);
		}
		this.RandomTargetPos(ref targetPos);
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0003CBB4 File Offset: 0x0003ADB4
	protected override void UpdateFireOrder(Order o)
	{
		if (o.m_type == Order.Type.Fire)
		{
			Vector3 pos = o.GetPos();
			bool losblocked = false;
			bool inFiringCone = base.InFiringCone(pos);
			o.SetLOSBlocked(losblocked);
			o.SetInFiringCone(inFiringCone);
		}
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0003CBEC File Offset: 0x0003ADEC
	protected int SetUpProjectiles(Vector3 muzzlePos, Vector3 targetPos, int max)
	{
		this.ModifyTarget(muzzlePos, ref targetPos);
		GameObject gameObject = ObjectFactory.Clone(this.m_projectile, muzzlePos, Quaternion.identity);
		Vector3 localScale = gameObject.transform.localScale;
		float d = 1.5f + (localScale.x + localScale.y + localScale.z) * 0.333f;
		muzzlePos += Vector3.up * d;
		Projectile_Missile component = gameObject.GetComponent<Projectile_Missile>();
		component.Setup(Vector3.up, base.GetOwner(), this.m_muzzleVel, this.m_gravity, this, this.GetRandomDamage(), this.m_Damage.m_armorPiercing, this.m_Damage.m_splashRadius, this.m_Damage.m_splashDamageFactor, this.m_aim.m_minRange, this.m_aim.m_maxRange);
		component.SetFlightPath(muzzlePos, targetPos, Vector3.up);
		return 1;
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0003CCC8 File Offset: 0x0003AEC8
	public override bool AimAt(Vector3 target)
	{
		return true;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0003CCCC File Offset: 0x0003AECC
	public override Dictionary<string, string> GetShipEditorInfo()
	{
		return base.GetShipEditorInfo();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0003CCD4 File Offset: 0x0003AED4
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_saltPathRandomness.x);
		writer.Write(this.m_saltPathRandomness.y);
		writer.Write(this.m_saltPathRandomness.z);
		writer.Write(this.m_useSaltPath);
		writer.Write(this.m_saltPathDetail);
		writer.Write(this.m_accuracy);
		writer.Write(this.m_accurateRadius);
		writer.Write(this.m_accuracyScaleWithLength);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0003CD58 File Offset: 0x0003AF58
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_saltPathRandomness.x = reader.ReadSingle();
		this.m_saltPathRandomness.y = reader.ReadSingle();
		this.m_saltPathRandomness.z = reader.ReadSingle();
		this.m_useSaltPath = reader.ReadBoolean();
		this.m_saltPathDetail = reader.ReadInt32();
		this.m_accuracy = reader.ReadInt32();
		this.m_accurateRadius = reader.ReadSingle();
		this.m_accuracyScaleWithLength = reader.ReadSingle();
	}

	// Token: 0x040006B2 RID: 1714
	public Vector3 m_saltPathRandomness = new Vector3(5f, 1f, 5f);

	// Token: 0x040006B3 RID: 1715
	public bool m_useSaltPath = true;

	// Token: 0x040006B4 RID: 1716
	public int m_saltPathDetail = 2;

	// Token: 0x040006B5 RID: 1717
	public bool m_useSmoothPath = true;

	// Token: 0x040006B6 RID: 1718
	public float m_smoothPathDetail = 45f;

	// Token: 0x040006B7 RID: 1719
	public float m_missileCurvature;

	// Token: 0x040006B8 RID: 1720
	public int m_accuracy = 85;

	// Token: 0x040006B9 RID: 1721
	public float m_accurateRadius = 80f;

	// Token: 0x040006BA RID: 1722
	public float m_accuracyScaleWithLength = 33.33f;
}
