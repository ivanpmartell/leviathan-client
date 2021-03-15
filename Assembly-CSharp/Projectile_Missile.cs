using System;
using System.IO;
using UnityEngine;

// Token: 0x020000EE RID: 238
[AddComponentMenu("Scripts/Projectiles/Projectile_Missile")]
public class Projectile_Missile : Projectile_MultiTarget
{
	// Token: 0x06000943 RID: 2371 RVA: 0x00043398 File Offset: 0x00041598
	protected override void SpecializedFixedUpdate()
	{
		base.SpecializedFixedUpdate();
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x000433A0 File Offset: 0x000415A0
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		Utils.SaveTrail(this.Trail, writer);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x000433B8 File Offset: 0x000415B8
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		Utils.LoadTrail(ref this.Trail, reader);
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x000433D0 File Offset: 0x000415D0
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x000433D8 File Offset: 0x000415D8
	public void SetFlightPath(Vector3 muzzlePos, Vector3 targetPos, Vector3 dir)
	{
		base.AddTarget(muzzlePos + dir * 10f);
		Vector3 vector = muzzlePos + dir * 10f;
		base.AddTarget(vector);
		Vector3 a = targetPos - muzzlePos;
		Vector3 inTarget = muzzlePos + a * 0.5f + new Vector3(0f, this.PrefferedHeight, 0f);
		Vector3 vector2 = muzzlePos + a * 0.25f + new Vector3(0f, this.PrefferedHeight, 0f);
		Vector3 control = muzzlePos + new Vector3(0f, this.PrefferedHeight, 0f);
		int num = 30;
		for (int i = 1; i < num; i++)
		{
			float delta = (float)i / (float)(num - 1);
			Vector3 inTarget2 = Utils.Bezier2(vector, control, vector2, delta);
			base.AddTarget(inTarget2);
		}
		base.AddTarget(vector2);
		base.AddTarget(inTarget);
		Vector3 vector3 = muzzlePos + a * 0.75f + new Vector3(0f, this.PrefferedHeight, 0f);
		base.AddTarget(vector3);
		control = targetPos + new Vector3(0f, this.PrefferedHeight, 0f);
		for (int j = 1; j < num; j++)
		{
			float delta2 = (float)j / (float)(num - 1);
			Vector3 inTarget3 = Utils.Bezier2(vector3, control, targetPos, delta2);
			base.AddTarget(inTarget3);
		}
		base.AddTarget(targetPos);
		targetPos.y = -50f;
		base.AddTarget(targetPos);
		targetPos.y = -100f;
		base.AddTarget(targetPos);
	}

	// Token: 0x0400077D RID: 1917
	public TrailRenderer Trail;

	// Token: 0x0400077E RID: 1918
	public float PrefferedHeight = 40f;
}
