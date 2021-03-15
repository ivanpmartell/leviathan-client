using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000D9 RID: 217
[AddComponentMenu("Scripts/Modules/Gun_Railgun")]
public class Gun_Railgun : Gun
{
	// Token: 0x0600082B RID: 2091 RVA: 0x0003CE18 File Offset: 0x0003B018
	public override void Awake()
	{
		base.Awake();
		this.m_rayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("shield") | 1 << LayerMask.NameToLayer("mines"));
		this.m_solidsRayMask = 1 << LayerMask.NameToLayer("Default");
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0003CE98 File Offset: 0x0003B098
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.DisableRayVisualizer();
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0003CEA8 File Offset: 0x0003B0A8
	protected override bool FireProjectile(Vector3 targetPos)
	{
		base.StopPreFire();
		Vector3 muzzlePos = base.GetMuzzlePos();
		float num = Vector3.Distance(muzzlePos, targetPos);
		float range = (!this.m_aim.m_spreadIgnoresRange) ? this.m_aim.m_maxRange : num;
		Quaternion randomSpreadDirection = base.GetRandomSpreadDirection(0f, range);
		Quaternion quaternion;
		base.FindOptimalFireDir(muzzlePos, targetPos, out quaternion);
		Vector3 vector = Vector3.Normalize(targetPos - muzzlePos);
		Vector3 direction = this.m_elevationJoint[0].rotation * (randomSpreadDirection * Vector3.forward);
		direction.y = 0f;
		direction.Normalize();
		bool flag = false;
		Vector3 targetPos2;
		this.RayTest(muzzlePos, direction, this.m_aim.m_maxRange, out targetPos2, ref flag);
		if (this.IsVisible() || flag)
		{
			this.EnableRayVisualizer(muzzlePos, targetPos2);
		}
		return true;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0003CF7C File Offset: 0x0003B17C
	public override float EstimateTimeToImpact(Vector3 targetPos)
	{
		return 0f;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0003CF84 File Offset: 0x0003B184
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating && this.m_rayVisualizer != null)
		{
			this.m_rayVisTime += Time.fixedDeltaTime;
			float num = Mathf.Clamp(this.m_rayVisTime / this.m_rayFadeTime, 0f, 1f);
			if (num >= 1f)
			{
				this.DisableRayVisualizer();
			}
			else if (this.m_rayVisualizer.renderer.material.HasProperty("_TintColor"))
			{
				Color color = this.m_rayVisualizer.renderer.material.GetColor("_TintColor");
				color.a = 1f - num;
				this.m_rayVisualizer.renderer.material.SetColor("_TintColor", color);
			}
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0003D05C File Offset: 0x0003B25C
	private void EnableRayVisualizer(Vector3 muzzlePos, Vector3 targetPos)
	{
		this.m_rayVisTime = 0f;
		this.m_rayTargetPos = targetPos;
		if (this.m_rayVisualizer == null)
		{
			this.m_rayVisualizer = (UnityEngine.Object.Instantiate(this.m_rayPrefab) as GameObject);
		}
		Vector3 position = (muzzlePos + targetPos) * 0.5f;
		float num = Vector3.Distance(muzzlePos, targetPos);
		this.m_rayVisualizer.transform.position = position;
		this.m_rayVisualizer.transform.localScale = new Vector3(this.m_rayWidth, 1f, num);
		this.m_rayVisualizer.transform.rotation = Quaternion.LookRotation(targetPos - muzzlePos, Vector3.up);
		float y = num / 8f;
		this.m_rayVisualizer.renderer.material.mainTextureScale = new Vector2(1f, y);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0003D138 File Offset: 0x0003B338
	private void DisableRayVisualizer()
	{
		if (this.m_rayVisualizer != null)
		{
			UnityEngine.Object.Destroy(this.m_rayVisualizer);
			this.m_rayVisualizer = null;
		}
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0003D160 File Offset: 0x0003B360
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_rayTargetPos.x);
		writer.Write(this.m_rayTargetPos.y);
		writer.Write(this.m_rayTargetPos.z);
		writer.Write(this.m_rayVisTime);
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0003D1B4 File Offset: 0x0003B3B4
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_rayTargetPos.x = reader.ReadSingle();
		this.m_rayTargetPos.y = reader.ReadSingle();
		this.m_rayTargetPos.z = reader.ReadSingle();
		this.m_rayVisTime = reader.ReadSingle();
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0003D208 File Offset: 0x0003B408
	private void SortRaytests(RaycastHit[] hits, out List<RaycastHit> sorted)
	{
		sorted = new List<RaycastHit>();
		foreach (RaycastHit item in hits)
		{
			sorted.Add(item);
		}
		sorted.Sort((RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0003D26C File Offset: 0x0003B46C
	private void DoMultiHitAchievement(int hits)
	{
		if (hits >= 5)
		{
			ClientGame.instance.AwardAchievement(base.GetOwner(), 16);
		}
		if (hits >= 3)
		{
			ClientGame.instance.AwardAchievement(base.GetOwner(), 15);
		}
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0003D2AC File Offset: 0x0003B4AC
	private void RayTest(Vector3 muzzlePos, Vector3 direction, float maxDistance, out Vector3 hitPoint, ref bool hitLocalPlayer)
	{
		Vector3 vector = muzzlePos;
		vector.y = this.m_testOffset;
		hitLocalPlayer = false;
		RaycastHit[] hits = Physics.RaycastAll(vector, direction, maxDistance, this.m_rayMask);
		int netID = base.GetUnit().GetNetID();
		List<RaycastHit> list;
		this.SortRaytests(hits, out list);
		int num = this.GetRandomDamage();
		HashSet<Unit> hashSet = new HashSet<Unit>();
		int num2 = 0;
		foreach (RaycastHit raycastHit in list)
		{
			if (num > 0)
			{
				if ((1 << raycastHit.collider.gameObject.layer & this.m_solidsRayMask) != 0)
				{
					hitPoint = raycastHit.point;
					hitPoint.y = muzzlePos.y;
					this.DoHitEffect(hitPoint, false);
					this.DoMultiHitAchievement(num2);
					return;
				}
				if (raycastHit.distance >= this.m_aim.m_minRange && raycastHit.distance <= this.m_aim.m_maxRange)
				{
					Section component = raycastHit.collider.GetComponent<Section>();
					if (component == null && raycastHit.collider.transform.parent)
					{
						component = raycastHit.collider.transform.parent.GetComponent<Section>();
					}
					if (component != null)
					{
						if (component.GetUnit().GetNetID() == netID)
						{
							continue;
						}
						if (hashSet.Contains(component.GetUnit()))
						{
							continue;
						}
						hashSet.Add(component.GetUnit());
					}
					HPModule component2 = raycastHit.collider.GetComponent<HPModule>();
					if (!(component2 != null) || component2.GetUnit().GetNetID() != netID)
					{
						ShieldGeometry component3 = raycastHit.collider.GetComponent<ShieldGeometry>();
						if (component3 != null)
						{
							if (component3.GetUnit().GetNetID() != netID)
							{
								bool flag = component3.GetUnit().GetOwner() == NetObj.m_localPlayerID;
								if (flag)
								{
									hitLocalPlayer = true;
								}
								hitPoint = raycastHit.point;
								hitPoint.y = muzzlePos.y;
								component3.Damage(new Hit(this, num, this.m_Damage.m_armorPiercing, raycastHit.point, direction), true);
								this.DoHitEffect(raycastHit.point, flag);
								this.DoMultiHitAchievement(num2);
								return;
							}
						}
						else
						{
							this.DoRayDamage(num, raycastHit.collider.collider, raycastHit.point, direction, ref hitLocalPlayer);
							if (component && TurnMan.instance.IsHostile(component.GetOwner(), base.GetOwner()))
							{
								num2++;
							}
							num /= 2;
						}
					}
				}
			}
		}
		this.DoMultiHitAchievement(num2);
		hitPoint = vector + direction * maxDistance;
		this.DoEndOfRayEffect(hitPoint, hitLocalPlayer);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0003D5E4 File Offset: 0x0003B7E4
	private bool DoRayDamage(int damage, Collider collider, Vector3 hitPos, Vector3 dir, ref bool hitLocalPlayer)
	{
		Section component = collider.GetComponent<Section>();
		if (component == null && collider.transform.parent)
		{
			component = collider.transform.parent.GetComponent<Section>();
		}
		if (component != null)
		{
			bool flag = component.GetOwner() == NetObj.m_localPlayerID;
			if (flag)
			{
				hitLocalPlayer = true;
			}
			this.DoHitEffect(hitPos, flag);
			component.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return true;
		}
		HPModule component2 = collider.GetComponent<HPModule>();
		if (component2 != null)
		{
			bool flag2 = component2.GetOwner() == NetObj.m_localPlayerID;
			if (flag2)
			{
				hitLocalPlayer = true;
			}
			this.DoHitEffect(hitPos, flag2);
			component2.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir), true);
			return true;
		}
		Platform platform = base.GetPlatform(collider.gameObject);
		if (platform != null)
		{
			bool flag3 = platform.GetOwner() == NetObj.m_localPlayerID;
			if (flag3)
			{
				hitLocalPlayer = true;
			}
			this.DoHitEffect(hitPos, flag3);
			platform.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return true;
		}
		Mine component3 = collider.GetComponent<Mine>();
		if (component3 != null)
		{
			bool flag4 = component3.GetOwner() == NetObj.m_localPlayerID;
			if (flag4)
			{
				hitLocalPlayer = true;
			}
			this.DoHitEffect(hitPos, flag4);
			component3.Damage(new Hit(this, damage, this.m_Damage.m_armorPiercing, hitPos, dir));
			return true;
		}
		return false;
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0003D780 File Offset: 0x0003B980
	private void DoHitEffect(Vector3 pos, bool hitLocalPlayer)
	{
		if (!this.IsVisible() && !hitLocalPlayer)
		{
			return;
		}
		if (this.m_hitEffectHiPrefab != null)
		{
			UnityEngine.Object.Instantiate(this.m_hitEffectHiPrefab, pos, Quaternion.identity);
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0003D7B8 File Offset: 0x0003B9B8
	private void DoEndOfRayEffect(Vector3 pos, bool hitLocalPlayer)
	{
		if (!this.IsVisible() && !hitLocalPlayer)
		{
			return;
		}
		if (this.m_rayEndEffectHiPrefab != null)
		{
			UnityEngine.Object.Instantiate(this.m_rayEndEffectHiPrefab, pos, Quaternion.identity);
		}
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0003D7F0 File Offset: 0x0003B9F0
	public override Dictionary<string, string> GetShipEditorInfo()
	{
		return base.GetShipEditorInfo();
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0003D7F8 File Offset: 0x0003B9F8
	protected override float FindElevationAngle(Vector3 muzzlePos, Vector3 target, float muzzleVel)
	{
		target.y -= 0.5f;
		float num = Vector2.Distance(new Vector2(target.x, target.z), new Vector2(muzzlePos.x, muzzlePos.z));
		float num2 = target.y - muzzlePos.y;
		float num3 = Mathf.Atan(num2 / num);
		return num3 * 57.29578f;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0003D864 File Offset: 0x0003BA64
	public override void DrawOrders()
	{
		base.DrawOrders();
		if (this.m_lineDrawer == null)
		{
			return;
		}
		if (this.m_rayLineType == -1)
		{
			this.m_rayLineType = this.m_lineDrawer.GetTypeID("railAim");
		}
		Vector3 position = base.transform.position;
		position.y = this.m_testOffset;
		foreach (Order order in this.m_orders)
		{
			if (order.m_type == Order.Type.Fire)
			{
				Vector3 a = order.GetPos() - position;
				a.y = 0f;
				a.Normalize();
				this.m_lineDrawer.DrawLine(position, position + a * this.m_aim.m_maxRange, this.m_rayLineType, 2.5f);
			}
		}
	}

	// Token: 0x040006BB RID: 1723
	public GameObject m_rayPrefab;

	// Token: 0x040006BC RID: 1724
	public GameObject m_hitEffectLowPrefab;

	// Token: 0x040006BD RID: 1725
	public GameObject m_hitEffectHiPrefab;

	// Token: 0x040006BE RID: 1726
	public GameObject m_rayEndEffectLowPrefab;

	// Token: 0x040006BF RID: 1727
	public GameObject m_rayEndEffectHiPrefab;

	// Token: 0x040006C0 RID: 1728
	public float m_rayWidth = 1f;

	// Token: 0x040006C1 RID: 1729
	public float m_rayFadeTime = 0.5f;

	// Token: 0x040006C2 RID: 1730
	public float m_testOffset = 1f;

	// Token: 0x040006C3 RID: 1731
	private GameObject m_rayVisualizer;

	// Token: 0x040006C4 RID: 1732
	private Vector3 m_rayTargetPos;

	// Token: 0x040006C5 RID: 1733
	private int m_rayMask;

	// Token: 0x040006C6 RID: 1734
	private int m_solidsRayMask;

	// Token: 0x040006C7 RID: 1735
	private int m_rayLineType = -1;

	// Token: 0x040006C8 RID: 1736
	private float m_rayVisTime;
}
