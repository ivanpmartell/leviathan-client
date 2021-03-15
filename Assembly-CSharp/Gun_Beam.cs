using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000D6 RID: 214
[AddComponentMenu("Scripts/Modules/Gun_Beam")]
public class Gun_Beam : Gun
{
	// Token: 0x0600080E RID: 2062 RVA: 0x0003C058 File Offset: 0x0003A258
	public override void Awake()
	{
		base.Awake();
		this.m_rayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water") | 1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("shield") | 1 << LayerMask.NameToLayer("mines"));
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0003C0D0 File Offset: 0x0003A2D0
	protected override bool FireProjectile(Vector3 targetPos)
	{
		return false;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0003C0D4 File Offset: 0x0003A2D4
	public override bool IsContinuous()
	{
		return true;
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0003C0D8 File Offset: 0x0003A2D8
	public override bool IsFiring()
	{
		return this.m_firing;
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0003C0E0 File Offset: 0x0003A2E0
	public override void StartFiring()
	{
		this.m_firing = true;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0003C0EC File Offset: 0x0003A2EC
	public override void StopFiring()
	{
		this.m_firing = false;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0003C0F8 File Offset: 0x0003A2F8
	public override float EstimateTimeToImpact(Vector3 targetPos)
	{
		return 0f;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0003C100 File Offset: 0x0003A300
	public override void Update()
	{
		base.Update();
		if (NetObj.m_simulating)
		{
			this.m_rayVisTime += Time.deltaTime;
			if (this.m_rayVisualizer != null)
			{
				this.m_rayVisualizer.renderer.material.SetFloat("_rayTime", this.m_rayVisTime);
			}
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0003C160 File Offset: 0x0003A360
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating)
		{
			this.m_dmgTimer -= Time.fixedDeltaTime;
			bool flag = false;
			if (this.m_target == null)
			{
				this.StopFiring();
			}
			Vector3 vector;
			if (this.m_firing && this.m_target.GetTargetWorldPos(out vector, base.GetOwnerTeam()))
			{
				if (this.m_loadedSalvo > 0f)
				{
					if (this.m_unit.m_onFireWeapon != null)
					{
						this.m_unit.m_onFireWeapon();
					}
					this.m_loadedSalvo -= Time.fixedDeltaTime;
					if (this.m_loadedSalvo < 0f)
					{
						this.m_loadedSalvo = 0f;
					}
					Vector3 muzzlePos = base.GetMuzzlePos();
					Vector3 forward = this.m_elevationJoint[0].forward;
					Vector3 targetPos;
					this.RayTest(muzzlePos, forward, this.m_aim.m_maxRange, out targetPos);
					if (this.IsVisible())
					{
						flag = true;
						this.EnableRayVisualizer(targetPos);
					}
				}
				else
				{
					this.StopFiring();
				}
			}
			if (!flag)
			{
				this.DisableRayVisualizer();
			}
			if (this.m_hitEffect != null && this.m_hitEffect.audio != null)
			{
				this.m_hitEffect.audio.volume = 1f;
			}
		}
		else if (this.m_hitEffect != null && this.m_hitEffect.audio != null)
		{
			this.m_hitEffect.audio.volume = 0f;
		}
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0003C2F8 File Offset: 0x0003A4F8
	private void EnableRayVisualizer(Vector3 targetPos)
	{
		this.m_rayTargetPos = targetPos;
		if (this.m_rayVisualizer == null)
		{
			this.m_rayVisualizer = (UnityEngine.Object.Instantiate(this.m_rayPrefab) as GameObject);
			this.m_rayVisualizer.transform.parent = base.transform;
		}
		Vector3 muzzlePos = base.GetMuzzlePos();
		Vector3 position = (muzzlePos + targetPos) * 0.5f;
		float num = Vector3.Distance(muzzlePos, targetPos);
		this.m_rayVisualizer.transform.position = position;
		this.m_rayVisualizer.transform.localScale = new Vector3(this.m_rayWidth, 1f, num);
		this.m_rayVisualizer.transform.rotation = Quaternion.LookRotation(targetPos - muzzlePos, Vector3.up);
		float y = num / 8f;
		this.m_rayVisualizer.renderer.material.mainTextureScale = new Vector2(1f, y);
		if (this.m_hitEffect == null)
		{
			if (this.m_hitEffectHiPrefab != null)
			{
				this.m_hitEffect = (UnityEngine.Object.Instantiate(this.m_hitEffectHiPrefab) as GameObject);
			}
			if (this.m_hitEffect != null)
			{
				this.m_hitEffect.transform.parent = base.transform;
				if (this.m_hitEffect.audio != null)
				{
					this.m_hitEffect.audio.volume = 0f;
				}
			}
		}
		if (this.m_hitEffect != null)
		{
			this.m_hitEffect.transform.position = targetPos;
		}
		if (this.m_muzzleEffectInstance == null)
		{
			if (this.m_muzzleEffect != null)
			{
				this.m_muzzleEffectInstance = (UnityEngine.Object.Instantiate(this.m_muzzleEffect) as GameObject);
			}
			if (this.m_muzzleEffectInstance != null)
			{
				this.m_muzzleEffectInstance.transform.parent = this.m_muzzleJoints[0].joint;
				this.m_muzzleEffectInstance.transform.localPosition = Vector3.zero;
				this.m_muzzleEffectInstance.transform.localRotation = Quaternion.identity;
			}
		}
		if (this.m_muzzleEffectInstance != null)
		{
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0003C538 File Offset: 0x0003A738
	private void DisableRayVisualizer()
	{
		if (this.m_rayVisualizer != null)
		{
			UnityEngine.Object.Destroy(this.m_rayVisualizer);
			this.m_rayVisualizer = null;
		}
		if (this.m_hitEffect != null)
		{
			UnityEngine.Object.Destroy(this.m_hitEffect);
			this.m_hitEffect = null;
		}
		if (this.m_muzzleEffectInstance != null)
		{
			UnityEngine.Object.Destroy(this.m_muzzleEffectInstance);
			this.m_muzzleEffectInstance = null;
		}
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0003C5B0 File Offset: 0x0003A7B0
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_firing);
		writer.Write(this.m_dmgTimer);
		writer.Write(this.m_rayTargetPos.x);
		writer.Write(this.m_rayTargetPos.y);
		writer.Write(this.m_rayTargetPos.z);
		writer.Write(this.m_rayVisTime);
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0003C61C File Offset: 0x0003A81C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_firing = reader.ReadBoolean();
		this.m_dmgTimer = reader.ReadSingle();
		this.m_rayTargetPos.x = reader.ReadSingle();
		this.m_rayTargetPos.y = reader.ReadSingle();
		this.m_rayTargetPos.z = reader.ReadSingle();
		this.m_rayVisTime = reader.ReadSingle();
		if (this.m_firing)
		{
			this.EnableRayVisualizer(this.m_rayTargetPos);
		}
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0003C6A0 File Offset: 0x0003A8A0
	private void RayTest(Vector3 muzzlePos, Vector3 direction, float maxDistance, out Vector3 hitPos)
	{
		RaycastHit[] array = Physics.RaycastAll(muzzlePos, direction, maxDistance, this.m_rayMask);
		int netID = base.GetUnit().GetNetID();
		float num = 999999f;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		foreach (RaycastHit raycastHit2 in array)
		{
			if (raycastHit2.distance < num)
			{
				Section component = raycastHit2.collider.GetComponent<Section>();
				if (component == null && raycastHit2.collider.transform.parent)
				{
					component = raycastHit2.collider.transform.parent.GetComponent<Section>();
				}
				if (component != null)
				{
					if (component.GetUnit().GetNetID() != netID)
					{
						num = raycastHit2.distance;
						raycastHit = raycastHit2;
						flag = true;
					}
				}
				else
				{
					HPModule component2 = raycastHit2.collider.GetComponent<HPModule>();
					if (component2 != null)
					{
						if (component2.GetUnit().GetNetID() != netID)
						{
							num = raycastHit2.distance;
							raycastHit = raycastHit2;
							flag = true;
						}
					}
					else
					{
						ShieldGeometry component3 = raycastHit2.collider.GetComponent<ShieldGeometry>();
						if (component3 != null)
						{
							if (component3.GetUnit().GetNetID() != netID)
							{
								num = raycastHit2.distance;
								raycastHit = raycastHit2;
								flag = true;
							}
						}
						else
						{
							num = raycastHit2.distance;
							raycastHit = raycastHit2;
							flag = true;
						}
					}
				}
			}
		}
		if (flag)
		{
			hitPos = raycastHit.point;
			if (this.m_dmgTimer <= 0f)
			{
				this.m_dmgTimer = 0.1f;
				int damage = (int)((float)this.GetRandomDamage() * 0.1f);
				this.DoRayDamage(damage, raycastHit.collider, raycastHit.point, direction);
			}
			return;
		}
		hitPos = muzzlePos + direction * maxDistance;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0003C88C File Offset: 0x0003AA8C
	private void DoRayDamage(int damage, Collider collider, Vector3 hitPos, Vector3 dir)
	{
		Section component = collider.GetComponent<Section>();
		if (component == null && collider.transform.parent)
		{
			component = collider.transform.parent.GetComponent<Section>();
		}
		if (component != null)
		{
			component.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return;
		}
		HPModule component2 = collider.GetComponent<HPModule>();
		if (component2 != null)
		{
			component2.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir), true);
			return;
		}
		ShieldGeometry component3 = collider.GetComponent<ShieldGeometry>();
		if (component3 != null)
		{
			component3.Damage(new Hit(this, damage * 2, this.m_Damage.m_armorPiercing, hitPos, dir), true);
			return;
		}
		Platform platform = base.GetPlatform(collider.gameObject);
		if (platform != null)
		{
			platform.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return;
		}
		Mine component4 = collider.GetComponent<Mine>();
		if (component4 != null)
		{
			component4.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return;
		}
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0003C9C8 File Offset: 0x0003ABC8
	public override Dictionary<string, string> GetShipEditorInfo()
	{
		return base.GetShipEditorInfo();
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0003C9D0 File Offset: 0x0003ABD0
	protected override float FindElevationAngle(Vector3 muzzlePos, Vector3 target, float muzzleVel)
	{
		target.y -= 0.5f;
		float num = Vector2.Distance(new Vector2(target.x, target.z), new Vector2(muzzlePos.x, muzzlePos.z));
		float num2 = target.y - muzzlePos.y;
		float num3 = Mathf.Atan(num2 / num);
		return num3 * 57.29578f;
	}

	// Token: 0x040006A4 RID: 1700
	public GameObject m_rayPrefab;

	// Token: 0x040006A5 RID: 1701
	public GameObject m_hitEffectLowPrefab;

	// Token: 0x040006A6 RID: 1702
	public GameObject m_hitEffectHiPrefab;

	// Token: 0x040006A7 RID: 1703
	public float m_rayWidth = 1f;

	// Token: 0x040006A8 RID: 1704
	private GameObject m_rayVisualizer;

	// Token: 0x040006A9 RID: 1705
	private GameObject m_hitEffect;

	// Token: 0x040006AA RID: 1706
	private GameObject m_muzzleEffectInstance;

	// Token: 0x040006AB RID: 1707
	private bool m_firing;

	// Token: 0x040006AC RID: 1708
	private Vector3 m_rayTargetPos;

	// Token: 0x040006AD RID: 1709
	private float m_dmgTimer;

	// Token: 0x040006AE RID: 1710
	private int m_rayMask;

	// Token: 0x040006AF RID: 1711
	private float m_rayVisTime;
}
