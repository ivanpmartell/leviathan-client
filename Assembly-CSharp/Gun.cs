using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000D0 RID: 208
[AddComponentMenu("Scripts/Modules/Gun")]
public abstract class Gun : HPModule
{
	// Token: 0x060007B3 RID: 1971 RVA: 0x00039CEC File Offset: 0x00037EEC
	public static void RegisterAIStates()
	{
		Gun.m_aiStateFactory.Register<GunGuard>("guard");
		Gun.m_aiStateFactory.Register<GunReload>("reload");
		Gun.m_aiStateFactory.Register<GunAim>("aim");
		Gun.m_aiStateFactory.Register<GunFire>("fire");
		Gun.m_aiStateFactory.Register<GunFireBeam>("firebeam");
		Gun.m_aiStateFactory.Register<GunThink>("think");
		Gun.m_aiStateFactory.Register<GunFollowOrder>("followorder");
		Gun.m_aiStateFactory.Register<GunDeploy>("deploy");
		Gun.m_aiStateFactory.Register<GunOff>("off");
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00039D80 File Offset: 0x00037F80
	public override void Awake()
	{
		base.Awake();
		this.m_landRayMask = 1 << LayerMask.NameToLayer("Default");
		this.m_unitsRayMask = 1 << LayerMask.NameToLayer("units");
		if (this.m_gravity < 0f)
		{
			this.m_gravity = 5f;
		}
		this.m_stateMachine = new AIStateMachine<Gun>(this, Gun.m_aiStateFactory);
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x00039DEC File Offset: 0x00037FEC
	public override void OnDestroy()
	{
		base.OnDestroy();
		TimedDestruction[] componentsInChildren = base.transform.GetComponentsInChildren<TimedDestruction>();
		foreach (TimedDestruction timedDestruction in componentsInChildren)
		{
			timedDestruction.transform.parent = null;
		}
		if (this.m_preFireEffect)
		{
			UnityEngine.Object.Destroy(this.m_preFireEffect);
		}
		this.m_preFireEffect = null;
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x00039E54 File Offset: 0x00038054
	public override void Setup(Unit unit, Battery battery, int x, int y, Direction dir, HPModule.DestroyedHandler destroyedCallback)
	{
		base.Setup(unit, battery, x, y, dir, destroyedCallback);
		this.UpdateStats();
		this.m_stateMachine.PushState("think");
		this.m_ammo = this.m_maxAmmo;
		this.LoadGun();
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x00039E9C File Offset: 0x0003809C
	public override StatusWnd_HPModule CreateStatusWindow(GameObject guiCamera)
	{
		bool friend = base.GetOwner() == NetObj.m_localPlayerID;
		return new StatusWnd_Gun(this, guiCamera, friend);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00039EC0 File Offset: 0x000380C0
	public override void Supply(ref int resources)
	{
		if (resources <= 0)
		{
			return;
		}
		base.Supply(ref resources);
		if (resources >= this.m_fire.m_ammoSupplyCost && this.m_maxAmmo >= 0 && (float)this.m_maxAmmo - ((float)this.m_ammo + this.m_loadedSalvo) >= (float)this.m_fire.m_ammoPerSupply)
		{
			this.m_ammo += this.m_fire.m_ammoPerSupply;
			resources -= this.m_fire.m_ammoSupplyCost;
		}
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00039F4C File Offset: 0x0003814C
	public override List<string> GetHardpointInfo()
	{
		List<string> list = new List<string>();
		if (this.m_Damage.m_damage_min != 0 && this.m_Damage.m_damage_max != 0)
		{
			if (this.m_Damage.m_damage_min == this.m_Damage.m_damage_max)
			{
				list.Add(Localize.instance.Translate("$DamageRange") + ": " + this.m_Damage.m_damage_max.ToString());
			}
			else
			{
				list.Add(string.Concat(new string[]
				{
					Localize.instance.Translate("$DamageRange"),
					": ",
					this.m_Damage.m_damage_min.ToString(),
					" - ",
					this.m_Damage.m_damage_max.ToString()
				}));
			}
		}
		if (this.m_Damage.m_armorPiercing != 0)
		{
			list.Add(Localize.instance.Translate("$ArmorPiercingShort") + ": " + this.m_Damage.m_armorPiercing.ToString());
		}
		return list;
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0003A068 File Offset: 0x00038268
	public void HideViewCone()
	{
		if (this.m_viewConeInstance != null)
		{
			UnityEngine.Object.Destroy(this.m_viewConeInstance);
			this.m_viewConeInstance = null;
		}
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0003A090 File Offset: 0x00038290
	public void ShowViewCone()
	{
		if (this.m_aim.viewConePrefab == null)
		{
			return;
		}
		if (this.m_viewConeInstance != null)
		{
			return;
		}
		this.m_viewConeInstance = (UnityEngine.Object.Instantiate(this.m_aim.viewConePrefab) as GameObject);
		this.m_viewConeInstance.transform.parent = base.transform;
		this.m_viewConeInstance.transform.localPosition = Vector3.zero;
		this.m_viewConeInstance.transform.localRotation = Quaternion.identity;
		ViewCone component = this.m_viewConeInstance.GetComponent<ViewCone>();
		DebugUtils.Assert(component != null, "Missing viewcone script on viewcone");
		component.Setup(this.m_aim.m_minRange, this.m_aim.m_maxRange, this.m_aim.m_maxRot);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0003A168 File Offset: 0x00038368
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(1);
		this.m_stateMachine.Save(writer);
		writer.Write(this.m_loadedSalvo);
		writer.Write((short)this.m_ammo);
		if (this.m_target != null)
		{
			writer.Write(true);
			this.m_target.Save(writer);
		}
		else
		{
			writer.Write(false);
		}
		writer.Write(this.m_visual.transform.localRotation.x);
		writer.Write(this.m_visual.transform.localRotation.y);
		writer.Write(this.m_visual.transform.localRotation.z);
		writer.Write(this.m_visual.transform.localRotation.w);
		if (this.m_elevationJoint.Length > 0)
		{
			writer.Write(this.m_elevationJoint[0].localRotation.x);
			writer.Write(this.m_elevationJoint[0].localRotation.y);
			writer.Write(this.m_elevationJoint[0].localRotation.z);
			writer.Write(this.m_elevationJoint[0].localRotation.w);
		}
		writer.Write(this.m_fire.repeatMuzzleCycle);
		writer.Write((byte)this.currentMuzzleJointIndex);
		if (this.m_preFireEffect)
		{
			writer.Write(true);
		}
		else
		{
			writer.Write(false);
		}
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0003A30C File Offset: 0x0003850C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		short num = reader.ReadInt16();
		this.m_stateMachine.Load(reader);
		this.m_loadedSalvo = reader.ReadSingle();
		this.m_ammo = (int)reader.ReadInt16();
		bool flag = reader.ReadBoolean();
		if (flag)
		{
			this.m_target = new GunTarget(reader);
		}
		Quaternion localRotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		Utils.NormalizeQuaternion(ref localRotation);
		this.m_visual.transform.localRotation = localRotation;
		if (this.m_elevationJoint.Length > 0)
		{
			Quaternion localRotation2 = default(Quaternion);
			localRotation2.x = reader.ReadSingle();
			localRotation2.y = reader.ReadSingle();
			localRotation2.z = reader.ReadSingle();
			localRotation2.w = reader.ReadSingle();
			foreach (Transform transform in this.m_elevationJoint)
			{
				transform.localRotation = localRotation2;
			}
		}
		this.m_fire.repeatMuzzleCycle = reader.ReadBoolean();
		this.currentMuzzleJointIndex = (int)reader.ReadByte();
		bool flag2 = reader.ReadBoolean();
		if (flag2)
		{
			this.PreFireGun();
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0003A448 File Offset: 0x00038648
	public override void ClearOrders()
	{
		base.ClearOrders();
		this.m_deploy = false;
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0003A458 File Offset: 0x00038658
	public override void AddOrder(Order order)
	{
		base.AddOrder(order);
		this.UpdateFireOrder(order);
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0003A468 File Offset: 0x00038668
	public override void OnOrdersChanged()
	{
		base.OnOrdersChanged();
		this.UpdateFireOrders();
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0003A478 File Offset: 0x00038678
	public override void SaveOrders(BinaryWriter stream)
	{
		base.SaveOrders(stream);
		stream.Write((int)this.m_aim.m_stance);
		stream.Write(this.m_deploy);
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0003A4AC File Offset: 0x000386AC
	public override void LoadOrders(BinaryReader stream)
	{
		base.LoadOrders(stream);
		this.SetStance((Gun.Stance)stream.ReadInt32());
		this.m_deploy = stream.ReadBoolean();
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0003A4D8 File Offset: 0x000386D8
	public override void DrawOrders()
	{
		if (base.GetOwner() != NetObj.m_localPlayerID)
		{
			return;
		}
		this.SetupLineDrawer();
		Vector3 vector = base.transform.position + new Vector3(0f, 1f, 0f);
		this.DrawTargetLine(vector);
		foreach (Order order in this.m_orders)
		{
			if (order.m_type == Order.Type.Fire)
			{
				Vector3 pos = order.GetPos();
				int type = (!order.IsInFiringCone()) ? this.m_outOfRangeLineMaterialID : this.m_inRangeLineMaterialID;
				if (this.m_aim.m_useHighAngle)
				{
					float num = Vector3.Distance(vector, pos);
					float y = num / 2f;
					int sections = Mathf.Max(8, (int)(num / 10f));
					this.m_lineDrawer.DrawCurvedLine(vector, pos, new Vector3(0f, y, 0f), type, 0.1f, sections);
				}
				else if (order.IsLOSBlocked())
				{
					if (Mathf.Sin(Time.time * 30f) > 0f)
					{
						this.m_lineDrawer.DrawLine(vector, pos, type, 0.1f);
					}
				}
				else
				{
					this.m_lineDrawer.DrawLine(vector, pos, type, 0.1f);
				}
			}
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x0003A660 File Offset: 0x00038860
	protected bool SetupLineDrawer()
	{
		if (!(this.m_lineDrawer == null))
		{
			return true;
		}
		this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
		if (!this.m_lineDrawer)
		{
			return false;
		}
		this.m_targetLineMaterialID = this.m_lineDrawer.GetTypeID("target");
		this.m_inRangeLineMaterialID = this.m_lineDrawer.GetTypeID("orderInRange");
		this.m_outOfRangeLineMaterialID = this.m_lineDrawer.GetTypeID("orderOutOfRange");
		return true;
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x0003A6E8 File Offset: 0x000388E8
	protected void DrawTargetLine(Vector3 line)
	{
		if (this.m_target != null)
		{
			Vector3 vector = base.transform.position + new Vector3(0f, 1f, 0f);
			Vector3 vector2;
			if (this.m_target.GetTargetWorldPos(out vector2, base.GetOwnerTeam()))
			{
				if (this.m_aim.m_useHighAngle)
				{
					float num = Vector3.Distance(vector, vector2);
					float y = num / 2f;
					int sections = Mathf.Max(8, (int)(num / 10f));
					this.m_lineDrawer.DrawCurvedLine(vector, vector2, new Vector3(0f, y, 0f), this.m_targetLineMaterialID, 0.1f, sections);
				}
				else
				{
					this.m_lineDrawer.DrawLine(vector, vector2, this.m_targetLineMaterialID, 0.1f);
				}
			}
		}
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0003A7B4 File Offset: 0x000389B4
	private void UpdateFireOrders()
	{
		foreach (Order o in this.m_orders)
		{
			this.UpdateFireOrder(o);
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x0003A81C File Offset: 0x00038A1C
	protected virtual void UpdateFireOrder(Order o)
	{
		int ownerTeam = base.GetOwnerTeam();
		if (o.m_type == Order.Type.Fire)
		{
			Vector3 pos = o.GetPos();
			bool losblocked = !this.TestLOF(pos);
			bool inFiringCone = this.InFiringCone(pos);
			o.SetLOSBlocked(losblocked);
			o.SetInFiringCone(inFiringCone);
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x0003A864 File Offset: 0x00038A64
	public virtual void StartFiring()
	{
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x0003A868 File Offset: 0x00038A68
	public virtual void StopFiring()
	{
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0003A86C File Offset: 0x00038A6C
	protected override void OnDisabled()
	{
		base.OnDisabled();
		this.SetTarget(null);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0003A87C File Offset: 0x00038A7C
	public virtual void PreFireGun()
	{
		if (this.m_preFireEffect != null)
		{
			return;
		}
		if (this.m_preFireHiPrefab != null)
		{
			this.m_preFireEffect = (UnityEngine.Object.Instantiate(this.m_preFireHiPrefab, this.GetMuzzlePos(), this.m_muzzleJoints[0].joint.rotation) as GameObject);
			this.m_preFireEffect.transform.parent = base.transform;
		}
		if (this.m_preFireEffect != null)
		{
			Renderer[] componentsInChildren = this.m_preFireEffect.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = this.IsVisible();
			}
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0003A938 File Offset: 0x00038B38
	public void StopPreFire()
	{
		if (this.m_preFireEffect)
		{
			UnityEngine.Object.Destroy(this.m_preFireEffect);
		}
		this.m_preFireEffect = null;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0003A968 File Offset: 0x00038B68
	public bool FireGun()
	{
		if (this.m_loadedSalvo <= 0f || this.m_target == null)
		{
			return false;
		}
		Vector3 targetPos;
		if (this.GetOptimalTargetPosition(this.m_target, out targetPos) && this.FireProjectile(targetPos))
		{
			if (this.m_unit.m_onFireWeapon != null)
			{
				this.m_unit.m_onFireWeapon();
			}
			this.AnimateFire();
			this.m_loadedSalvo -= 1f;
			return true;
		}
		return false;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0003A9EC File Offset: 0x00038BEC
	public bool LoadGun()
	{
		if (this.m_maxAmmo < 0)
		{
			this.m_loadedSalvo = (float)this.m_salvo;
			return true;
		}
		int num = this.m_salvo - (int)this.m_loadedSalvo;
		if (num > 0 && this.m_ammo <= 0)
		{
			return false;
		}
		while (num > 0 && this.m_ammo > 0)
		{
			num--;
			this.m_ammo--;
			this.m_loadedSalvo += 1f;
		}
		return true;
	}

	// Token: 0x060007CF RID: 1999
	protected abstract bool FireProjectile(Vector3 targetPos);

	// Token: 0x060007D0 RID: 2000 RVA: 0x0003AA78 File Offset: 0x00038C78
	public override void SetSelected(bool selected, bool explicitSelected)
	{
		if (selected == base.IsSelected())
		{
			return;
		}
		base.SetSelected(selected, explicitSelected);
		if (selected)
		{
			this.ShowViewCone();
		}
		else
		{
			this.HideViewCone();
		}
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0003AAB4 File Offset: 0x00038CB4
	public float GetLoadedSalvo()
	{
		return this.m_loadedSalvo;
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0003AABC File Offset: 0x00038CBC
	public float GetReloadTime()
	{
		return this.m_reloadTime;
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0003AAC4 File Offset: 0x00038CC4
	public float GetPreFireTime()
	{
		return this.m_preFireTime;
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0003AACC File Offset: 0x00038CCC
	public float GetSalvoDelay()
	{
		return this.m_fire.m_salvoDelay;
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0003AADC File Offset: 0x00038CDC
	public int GetAmmo()
	{
		return this.m_ammo;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0003AAE4 File Offset: 0x00038CE4
	public int GetSalvoSize()
	{
		return this.m_salvo;
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0003AAEC File Offset: 0x00038CEC
	public int GetMaxAmmo()
	{
		return this.m_maxAmmo;
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0003AAF4 File Offset: 0x00038CF4
	public bool GetBarrage()
	{
		return this.m_fire.m_barrage;
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0003AB04 File Offset: 0x00038D04
	public Gun.Stance GetStance()
	{
		return this.m_aim.m_stance;
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0003AB14 File Offset: 0x00038D14
	public void SetStance(Gun.Stance stance)
	{
		this.m_aim.m_stance = stance;
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0003AB24 File Offset: 0x00038D24
	public override void SetDeploy(bool deploy)
	{
		this.m_deploy = deploy;
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0003AB30 File Offset: 0x00038D30
	public override bool GetDeploy()
	{
		return this.m_deploy;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0003AB38 File Offset: 0x00038D38
	public virtual float GetTargetRadius(Vector3 targetPos)
	{
		return 0f;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0003AB40 File Offset: 0x00038D40
	public virtual bool IsContinuous()
	{
		return false;
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0003AB44 File Offset: 0x00038D44
	public virtual bool IsFiring()
	{
		return false;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0003AB48 File Offset: 0x00038D48
	public bool GetStaticTargetOnly()
	{
		return this.m_aim.m_staticTargetOnly;
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0003AB58 File Offset: 0x00038D58
	public Order.FireVisual GetOrderMarkerType()
	{
		return this.m_aim.m_orderMarkerType;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0003AB68 File Offset: 0x00038D68
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating && !this.m_disabled)
		{
			if (base.transform.position.y < -2f && this.m_unit.IsSinking())
			{
				base.Damage(new Hit(this.m_health, 100), false);
				return;
			}
			this.m_stateMachine.Update(Time.fixedDeltaTime);
		}
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0003ABE4 File Offset: 0x00038DE4
	protected virtual void AnimateFire()
	{
		if (this.IsVisible() && this.m_visual != null)
		{
			string animationName = this.GetCurrentMuzzleJointDef().animationName;
			Animation[] componentsInChildren = this.m_visual.GetComponentsInChildren<Animation>();
			int num = (componentsInChildren != null) ? componentsInChildren.Length : 0;
			if (num > 0)
			{
				foreach (Animation animation in componentsInChildren)
				{
					if (!(animation.GetClip(animationName) == null))
					{
						animation.Play(animationName);
					}
				}
			}
			if (this.m_muzzleJoints.Count > 0 && this.m_muzzleEffect != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.m_muzzleEffect, this.GetMuzzlePos(), this.m_muzzleJoints[0].joint.rotation) as GameObject;
				gameObject.transform.parent = base.transform;
			}
		}
		this.SetNextMuzzleJoint();
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0003ACE8 File Offset: 0x00038EE8
	public GunTarget GetTarget()
	{
		return this.m_target;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0003ACF0 File Offset: 0x00038EF0
	public void SetTarget(GunTarget target)
	{
		this.m_target = target;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0003ACFC File Offset: 0x00038EFC
	public bool GetRemoveInvalidTarget()
	{
		return this.m_fire.m_removeInvalidOrder;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0003AD0C File Offset: 0x00038F0C
	public GunTarget FindTarget()
	{
		Unit unit = null;
		Vector3 position = Vector3.zero;
		float num = float.MaxValue;
		int owner = base.GetOwner();
		int ownerTeam = base.GetOwnerTeam();
		TurnMan instance = TurnMan.instance;
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Unit unit2 = netObj as Unit;
			if (unit2 != null && unit2.IsValidTarget() && instance.IsHostile(owner, unit2.GetOwner()) && unit2.IsSeenByTeam(ownerTeam))
			{
				Ship ship = netObj as Ship;
				if (!(ship != null) || !ship.GetAi().m_inactive)
				{
					Vector3 vector;
					if (this.InFiringCone(unit2, out vector) && this.TestLOF(vector))
					{
						float num2 = Vector3.Distance(base.transform.position, vector);
						if (num2 < num)
						{
							unit = unit2;
							position = vector;
							num = num2;
						}
					}
				}
			}
		}
		if (unit != null)
		{
			int netID = unit.GetNetID();
			Vector3 localTargetPos = unit.transform.InverseTransformPoint(position);
			return new GunTarget(netID, localTargetPos);
		}
		return null;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0003AE74 File Offset: 0x00039074
	public virtual bool ResetTower()
	{
		if (this.m_elevationJoint.Length == 0)
		{
			return true;
		}
		Quaternion quaternion = Quaternion.RotateTowards(this.m_visual.transform.localRotation, Quaternion.identity, this.m_aim.m_rotationSpeed);
		this.m_visual.transform.localRotation = quaternion;
		Quaternion quaternion2 = Quaternion.RotateTowards(this.m_elevationJoint[0].localRotation, Quaternion.identity, this.m_aim.m_elevationSpeed);
		foreach (Transform transform in this.m_elevationJoint)
		{
			transform.localRotation = quaternion2;
		}
		float num = Quaternion.Angle(quaternion, Quaternion.identity);
		float num2 = Quaternion.Angle(quaternion2, Quaternion.identity);
		return num < 1f && num2 < 1f;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0003AF48 File Offset: 0x00039148
	protected bool FindOptimalFireDir(Vector3 muzzlePos, Vector3 target, out Quaternion dir)
	{
		Vector3 normalized = (target - muzzlePos).normalized;
		Vector3 forward = normalized;
		forward.y = 0f;
		forward.Normalize();
		Quaternion lhs = Quaternion.LookRotation(forward, Vector3.up);
		float num = this.FindElevationAngle(muzzlePos, target, this.m_muzzleVel);
		if (float.IsNaN(num))
		{
			dir = this.m_elevationJoint[0].rotation;
			return false;
		}
		Quaternion rhs = Quaternion.Euler(-num, 0f, 0f);
		dir = lhs * rhs;
		return true;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0003AFD8 File Offset: 0x000391D8
	public virtual bool AimAt(Vector3 target)
	{
		if (this.m_elevationJoint.Length == 0)
		{
			return true;
		}
		Vector3 muzzlePos = this.GetMuzzlePos();
		Vector3 normalized = (target - muzzlePos).normalized;
		Vector3 forward = normalized;
		forward.y = 0f;
		forward.Normalize();
		Quaternion b = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		Quaternion rhs = Quaternion.LookRotation(forward, base.transform.up);
		Quaternion quaternion = Quaternion.Inverse(base.transform.rotation) * rhs;
		Vector3 eulerAngles = quaternion.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		quaternion.eulerAngles = eulerAngles;
		Quaternion quaternion2 = Quaternion.RotateTowards(this.m_visual.transform.localRotation, quaternion, this.m_aim.m_rotationSpeed);
		bool flag = Quaternion.Angle(quaternion2, quaternion) < 0.5f;
		if (flag)
		{
			quaternion2 = quaternion;
		}
		if (this.m_aim.m_maxRot < 0f || Quaternion.Angle(quaternion2, b) <= this.m_aim.m_maxRot)
		{
			this.m_visual.transform.localRotation = quaternion2;
		}
		float num = this.FindElevationAngle(this.GetMuzzlePos(), target, this.m_muzzleVel);
		if (float.IsNaN(num))
		{
			return false;
		}
		Vector3 forward2 = this.m_visual.transform.forward;
		forward2.y = 0f;
		forward2.Normalize();
		float num2 = Vector3.Angle(forward2, this.m_visual.transform.forward);
		if (this.m_visual.transform.forward.y < 0f)
		{
			num2 = -num2;
		}
		Quaternion quaternion3 = Quaternion.Euler(-num + num2, 0f, 0f);
		Quaternion quaternion4 = Quaternion.RotateTowards(this.m_elevationJoint[0].localRotation, quaternion3, this.m_aim.m_elevationSpeed);
		bool flag2 = Quaternion.Angle(quaternion4, quaternion3) < 0.5f;
		if (flag2)
		{
			quaternion4 = quaternion3;
		}
		foreach (Transform transform in this.m_elevationJoint)
		{
			transform.localRotation = quaternion4;
		}
		return flag && flag2;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0003B224 File Offset: 0x00039424
	protected virtual float FindElevationAngle(Vector3 muzzlePos, Vector3 target, float muzzleVel)
	{
		float num = Vector2.Distance(new Vector2(target.x, target.z), new Vector2(muzzlePos.x, muzzlePos.z));
		float num2 = target.y - muzzlePos.y;
		float num3 = Mathf.Atan(num2 / num);
		float num4 = 0.5f * Mathf.Asin(this.m_gravity * num / (muzzleVel * muzzleVel));
		num4 += num3;
		num4 *= 57.29578f;
		if (this.m_aim.m_useHighAngle)
		{
			num4 = 90f - num4;
		}
		return num4;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0003B2B4 File Offset: 0x000394B4
	public override string GetStatusText()
	{
		AIState<Gun> activeState = this.m_stateMachine.GetActiveState();
		if (activeState != null)
		{
			return activeState.GetStatusText();
		}
		return string.Empty;
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0003B2E0 File Offset: 0x000394E0
	public override void GetChargeLevel(out float i, out float time)
	{
		if (this.m_disabled)
		{
			base.GetChargeLevel(out i, out time);
			return;
		}
		AIState<Gun> activeState = this.m_stateMachine.GetActiveState();
		if (activeState != null)
		{
			activeState.GetCharageLevel(out i, out time);
			return;
		}
		i = -1f;
		time = -1f;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0003B32C File Offset: 0x0003952C
	public override string GetTooltip()
	{
		string str = base.GetName() + "\nHP: " + this.m_health;
		return str + "\nStatus: " + this.GetStatusText();
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0003B368 File Offset: 0x00039568
	public bool InRange(Unit unit)
	{
		float num = unit.GetLength() / 2f;
		float num2 = Vector3.Distance(unit.transform.position, base.transform.position);
		return num2 + num >= this.m_aim.m_minRange && num2 - num <= this.m_aim.m_maxRange;
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0003B3C8 File Offset: 0x000395C8
	public bool InRange(Vector3 pos)
	{
		float num = Vector3.Distance(pos, base.transform.position);
		return num >= this.m_aim.m_minRange && num <= this.m_aim.m_maxRange;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0003B40C File Offset: 0x0003960C
	public bool InFiringCone(Unit unit, out Vector3 inConePoint)
	{
		inConePoint = Vector3.zero;
		if (!this.InRange(unit))
		{
			return false;
		}
		if (this.m_aim.m_maxRot < 0f)
		{
			return true;
		}
		Vector3[] targetPoints = unit.GetTargetPoints();
		foreach (Vector3 vector in targetPoints)
		{
			Vector3 vector2 = vector - base.transform.position;
			float magnitude = vector2.magnitude;
			Vector3 to = vector2;
			to.y = 0f;
			to.Normalize();
			float num = Vector3.Angle(base.transform.forward, to);
			if (num <= this.m_aim.m_maxRot && magnitude >= this.m_aim.m_minRange && magnitude <= this.m_aim.m_maxRange)
			{
				inConePoint = vector;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x0003B4FC File Offset: 0x000396FC
	public bool InFiringCone(GunTarget target)
	{
		Vector3 point;
		return target.GetTargetWorldPos(out point, base.GetOwnerTeam()) && this.InFiringCone(point);
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x0003B528 File Offset: 0x00039728
	public bool InFiringCone(Vector3 point)
	{
		if (!this.InRange(point))
		{
			return false;
		}
		if (this.m_aim.m_maxRot < 0f)
		{
			return true;
		}
		Vector3 to = point - base.transform.position;
		to.y = 0f;
		to.Normalize();
		float num = Vector3.Angle(base.transform.forward, to);
		return num <= this.m_aim.m_maxRot;
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0003B5A4 File Offset: 0x000397A4
	private Unit GetCollisionUnit(RaycastHit hit)
	{
		Unit component;
		if (hit.rigidbody != null)
		{
			component = hit.rigidbody.GetComponent<Unit>();
		}
		else
		{
			component = hit.collider.GetComponent<Unit>();
			if (component == null)
			{
				component = hit.collider.transform.parent.GetComponent<Unit>();
			}
		}
		return component;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0003B608 File Offset: 0x00039808
	public bool TestLOF(Vector3 point)
	{
		if (this.m_aim.m_useHighAngle)
		{
			return true;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = point - base.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		if (magnitude <= 1f)
		{
			return true;
		}
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(position, normalized, out raycastHit, magnitude - 1f, this.m_landRayMask) && raycastHit.distance > 1f)
		{
			return false;
		}
		position.y = 1f;
		normalized.y = 0f;
		normalized.Normalize();
		int ownerTeam = base.GetOwnerTeam();
		RaycastHit[] array = Physics.RaycastAll(position, normalized, magnitude - 1f, this.m_unitsRayMask);
		foreach (RaycastHit hit in array)
		{
			Unit collisionUnit = this.GetCollisionUnit(hit);
			if (collisionUnit != this.m_unit && collisionUnit.GetOwnerTeam() == ownerTeam)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0003B730 File Offset: 0x00039930
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (this.m_outOfAmmoIcon != null)
		{
			this.m_outOfAmmoIcon.renderer.enabled = visible;
		}
		if (this.m_preFireEffect != null)
		{
			Renderer[] componentsInChildren = this.m_preFireEffect.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.enabled = visible;
			}
		}
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0003B7A4 File Offset: 0x000399A4
	protected virtual int GetRandomDamage()
	{
		if (CheatMan.instance.GetInstaGib() && TurnMan.instance.IsHuman(base.GetOwner()))
		{
			return 25000;
		}
		return PRand.Range(this.m_Damage.m_damage_min, this.m_Damage.m_damage_max);
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0003B7F8 File Offset: 0x000399F8
	protected static void ApplyAccuracyOnTarget(ref Vector3 target, int accuracy, float accuracyRadius)
	{
		if (accuracy == 100 || accuracyRadius == 0f)
		{
			return;
		}
		if (PRand.Range(1, 100) <= accuracy)
		{
			return;
		}
		float max = accuracyRadius * Mathf.Clamp(1.3f - (float)accuracy / 100f, 0.05f, 1f);
		float num = PRand.Range(1f, max);
		bool flag = PRand.Range(0, 100) > 33;
		bool flag2 = PRand.Range(0, 100) > 33;
		if (!flag && !flag2)
		{
			flag2 = (flag = true);
		}
		float num2 = 0f;
		float num3 = 0f;
		float f = PRand.Range(1f, 359f);
		if (flag)
		{
			num2 = Mathf.Cos(f) * num;
		}
		if (flag2)
		{
			num3 = Mathf.Sin(f) * num;
		}
		target.x += num2;
		target.z += num3;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0003B8E8 File Offset: 0x00039AE8
	protected GunMuzzleDef GetCurrentMuzzleJointDef()
	{
		if (this.currentMuzzleJointIndex >= 0 && this.currentMuzzleJointIndex <= this.m_muzzleJoints.Count - 1)
		{
			return this.m_muzzleJoints[this.currentMuzzleJointIndex];
		}
		return null;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0003B924 File Offset: 0x00039B24
	protected void SetNextMuzzleJoint()
	{
		if (this.m_muzzleJoints == null || this.m_muzzleJoints.Count <= 1)
		{
			return;
		}
		this.currentMuzzleJointIndex++;
		if (this.currentMuzzleJointIndex > this.m_muzzleJoints.Count - 1)
		{
			if (this.m_fire.repeatMuzzleCycle)
			{
				this.currentMuzzleJointIndex = 0;
			}
			else
			{
				this.currentMuzzleJointIndex = this.m_muzzleJoints.Count - 1;
			}
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0003B9A4 File Offset: 0x00039BA4
	protected void SetPrevMuzzleJoint()
	{
		if (this.m_muzzleJoints == null || this.m_muzzleJoints.Count <= 1)
		{
			return;
		}
		this.currentMuzzleJointIndex--;
		if (this.currentMuzzleJointIndex < 0)
		{
			if (this.m_fire.repeatMuzzleCycle)
			{
				this.currentMuzzleJointIndex = this.m_muzzleJoints.Count - 1;
			}
			else
			{
				this.currentMuzzleJointIndex = 0;
			}
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060007FC RID: 2044 RVA: 0x0003BA18 File Offset: 0x00039C18
	// (set) Token: 0x060007FD RID: 2045 RVA: 0x0003BA20 File Offset: 0x00039C20
	public GameObject ViewConeInstance
	{
		get
		{
			return this.m_viewConeInstance;
		}
		private set
		{
			this.m_viewConeInstance = value;
		}
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0003BA2C File Offset: 0x00039C2C
	protected Vector3 GetMuzzlePos()
	{
		GunMuzzleDef currentMuzzleJointDef = this.GetCurrentMuzzleJointDef();
		DebugUtils.Assert(currentMuzzleJointDef != null);
		return currentMuzzleJointDef.joint.position;
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0003BA58 File Offset: 0x00039C58
	public virtual float EstimateTimeToImpact(Vector3 targetPos)
	{
		float num = Vector3.Distance(base.transform.position, targetPos);
		return num / this.m_muzzleVel;
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0003BA80 File Offset: 0x00039C80
	public bool GetOptimalTargetPosition(GunTarget target, out Vector3 targetPos)
	{
		if (!target.GetTargetWorldPos(out targetPos, base.GetOwnerTeam()))
		{
			return false;
		}
		NetObj targetObject = target.GetTargetObject();
		if (targetObject == null)
		{
			return true;
		}
		float d = this.EstimateTimeToImpact(targetPos);
		Unit unit = targetObject as Unit;
		if (unit != null)
		{
			targetPos += unit.GetVelocity() * d;
		}
		else
		{
			HPModule hpmodule = target.GetTargetObject() as HPModule;
			if (hpmodule != null)
			{
				targetPos += hpmodule.GetVelocity() * d;
			}
		}
		return true;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0003BB30 File Offset: 0x00039D30
	private void UpdateStats()
	{
		this.m_spread = this.m_aim.m_baseSpread;
		this.m_salvo = this.m_fire.m_baseSalvo;
		this.m_reloadTime = this.m_fire.m_baseReloadTime;
		this.m_preFireTime = this.m_fire.m_basePreFireTime;
		this.m_maxAmmo = this.m_fire.m_baseMaxAmmo;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0003BB94 File Offset: 0x00039D94
	private void SetOutOfAmmoIcon(bool enabled)
	{
		if (this.m_outOfAmmoIconPrefab == null)
		{
			return;
		}
		if (enabled)
		{
			if (this.m_outOfAmmoIcon == null)
			{
				this.m_outOfAmmoIcon = (UnityEngine.Object.Instantiate(this.m_outOfAmmoIconPrefab) as GameObject);
				this.m_outOfAmmoIcon.transform.parent = base.transform;
				this.m_outOfAmmoIcon.transform.localPosition = new Vector3(0.5f, base.collider.bounds.max.y + 0.1f, 0f);
				this.m_outOfAmmoIcon.renderer.enabled = this.IsVisible();
			}
		}
		else if (this.m_outOfAmmoIcon != null)
		{
			UnityEngine.Object.Destroy(this.m_outOfAmmoIcon);
			this.m_outOfAmmoIcon = null;
		}
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0003BC74 File Offset: 0x00039E74
	protected Quaternion GetRandomSpreadDirection(float aim, float range)
	{
		float num = 57.29578f * Mathf.Atan(this.m_spread / range);
		num -= num * aim;
		return Quaternion.Euler((PRand.Value() - 0.5f) * num * 0.5f, (PRand.Value() - 0.5f) * num, 0f);
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0003BCC8 File Offset: 0x00039EC8
	public Platform GetPlatform(GameObject go)
	{
		Platform component = go.GetComponent<Platform>();
		if (component == null && go.transform.parent != null)
		{
			component = go.transform.parent.GetComponent<Platform>();
		}
		return component;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0003BD10 File Offset: 0x00039F10
	public override Dictionary<string, string> GetShipEditorInfo()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		if (this.m_Damage.m_damage_min == this.m_Damage.m_damage_max)
		{
			dictionary["$Damage"] = this.m_Damage.m_damage_max.ToString();
		}
		else
		{
			dictionary["$Damage"] = this.m_Damage.m_damage_min.ToString() + " - " + this.m_Damage.m_damage_max.ToString();
		}
		dictionary["$Range"] = this.m_aim.m_minRange.ToString() + " - " + this.m_aim.m_maxRange.ToString();
		dictionary["$Salvo"] = this.m_fire.m_baseSalvo.ToString();
		dictionary["$SplashDamage"] = this.m_Damage.m_splashRadius.ToString();
		dictionary["$ArmorPiercing"] = this.m_Damage.m_armorPiercing.ToString();
		dictionary["$Reload"] = this.m_fire.m_baseReloadTime.ToString();
		dictionary["$Rotation"] = (this.m_aim.m_maxRot * 2f).ToString();
		dictionary["$Ammo"] = this.m_fire.m_baseMaxAmmo.ToString();
		return dictionary;
	}

	// Token: 0x0400065F RID: 1631
	public static GenericFactory<AIState<Gun>> m_aiStateFactory = new GenericFactory<AIState<Gun>>();

	// Token: 0x04000660 RID: 1632
	protected AIStateMachine<Gun> m_stateMachine;

	// Token: 0x04000661 RID: 1633
	public Gun.Aim m_aim = new Gun.Aim();

	// Token: 0x04000662 RID: 1634
	public Gun.GunDamage m_Damage = new Gun.GunDamage();

	// Token: 0x04000663 RID: 1635
	public Gun.FireSettings m_fire = new Gun.FireSettings();

	// Token: 0x04000664 RID: 1636
	public Transform[] m_elevationJoint = new Transform[0];

	// Token: 0x04000665 RID: 1637
	public List<GunMuzzleDef> m_muzzleJoints;

	// Token: 0x04000666 RID: 1638
	public GameObject m_muzzleEffect;

	// Token: 0x04000667 RID: 1639
	public GameObject m_muzzleEffectLow;

	// Token: 0x04000668 RID: 1640
	public GameObject m_outOfAmmoIconPrefab;

	// Token: 0x04000669 RID: 1641
	public GameObject m_preFireLowPrefab;

	// Token: 0x0400066A RID: 1642
	public GameObject m_preFireHiPrefab;

	// Token: 0x0400066B RID: 1643
	private GameObject m_preFireEffect;

	// Token: 0x0400066C RID: 1644
	public float m_muzzleVel = 30f;

	// Token: 0x0400066D RID: 1645
	public float m_gravity = -1f;

	// Token: 0x0400066E RID: 1646
	public GameObject m_projectile;

	// Token: 0x0400066F RID: 1647
	public bool m_canDeploy;

	// Token: 0x04000670 RID: 1648
	protected GunTarget m_target;

	// Token: 0x04000671 RID: 1649
	protected ViewCone viewConeScript;

	// Token: 0x04000672 RID: 1650
	protected bool m_viewConeVisible;

	// Token: 0x04000673 RID: 1651
	protected GameObject m_viewConeInstance;

	// Token: 0x04000674 RID: 1652
	protected int currentMuzzleJointIndex;

	// Token: 0x04000675 RID: 1653
	protected int m_landRayMask;

	// Token: 0x04000676 RID: 1654
	protected int m_unitsRayMask;

	// Token: 0x04000677 RID: 1655
	protected float m_loadedSalvo;

	// Token: 0x04000678 RID: 1656
	private float m_spread;

	// Token: 0x04000679 RID: 1657
	private float m_reloadTime;

	// Token: 0x0400067A RID: 1658
	private float m_preFireTime;

	// Token: 0x0400067B RID: 1659
	private int m_salvo;

	// Token: 0x0400067C RID: 1660
	private int m_maxAmmo;

	// Token: 0x0400067D RID: 1661
	private int m_ammo;

	// Token: 0x0400067E RID: 1662
	private bool m_deploy;

	// Token: 0x0400067F RID: 1663
	private GameObject m_outOfAmmoIcon;

	// Token: 0x04000680 RID: 1664
	protected LineDrawer m_lineDrawer;

	// Token: 0x04000681 RID: 1665
	private int m_targetLineMaterialID = -1;

	// Token: 0x04000682 RID: 1666
	private int m_inRangeLineMaterialID = -1;

	// Token: 0x04000683 RID: 1667
	private int m_outOfRangeLineMaterialID = -1;

	// Token: 0x020000D1 RID: 209
	public enum Stance
	{
		// Token: 0x04000685 RID: 1669
		FireAtWill,
		// Token: 0x04000686 RID: 1670
		HoldFire
	}

	// Token: 0x020000D2 RID: 210
	[Serializable]
	public class Aim
	{
		// Token: 0x04000687 RID: 1671
		public Order.FireVisual m_orderMarkerType;

		// Token: 0x04000688 RID: 1672
		public bool m_noAutotarget;

		// Token: 0x04000689 RID: 1673
		public bool m_spreadIgnoresRange;

		// Token: 0x0400068A RID: 1674
		public bool m_staticTargetOnly;

		// Token: 0x0400068B RID: 1675
		public bool m_useHighAngle;

		// Token: 0x0400068C RID: 1676
		public bool m_manualTarget = true;

		// Token: 0x0400068D RID: 1677
		public Gun.Stance m_stance;

		// Token: 0x0400068E RID: 1678
		public float m_maxRot = 75f;

		// Token: 0x0400068F RID: 1679
		public float m_rotationSpeed = 0.8f;

		// Token: 0x04000690 RID: 1680
		public float m_elevationSpeed = 0.8f;

		// Token: 0x04000691 RID: 1681
		public float m_minRange = 15f;

		// Token: 0x04000692 RID: 1682
		public float m_maxRange = 50f;

		// Token: 0x04000693 RID: 1683
		public float m_baseSpread;

		// Token: 0x04000694 RID: 1684
		public GameObject viewConePrefab;
	}

	// Token: 0x020000D3 RID: 211
	[Serializable]
	public class GunDamage
	{
		// Token: 0x04000695 RID: 1685
		public int m_damage_min = 5;

		// Token: 0x04000696 RID: 1686
		public int m_damage_max = 5;

		// Token: 0x04000697 RID: 1687
		public int m_armorPiercing;

		// Token: 0x04000698 RID: 1688
		public float m_splashDamageFactor;

		// Token: 0x04000699 RID: 1689
		public float m_splashRadius;
	}

	// Token: 0x020000D4 RID: 212
	[Serializable]
	public class FireSettings
	{
		// Token: 0x0400069A RID: 1690
		public float m_basePreFireTime;

		// Token: 0x0400069B RID: 1691
		public int m_baseSalvo = 1;

		// Token: 0x0400069C RID: 1692
		public float m_salvoDelay = 0.1f;

		// Token: 0x0400069D RID: 1693
		public bool repeatMuzzleCycle = true;

		// Token: 0x0400069E RID: 1694
		public bool m_barrage = true;

		// Token: 0x0400069F RID: 1695
		public bool m_removeInvalidOrder;

		// Token: 0x040006A0 RID: 1696
		public float m_baseReloadTime;

		// Token: 0x040006A1 RID: 1697
		public int m_baseMaxAmmo = -1;

		// Token: 0x040006A2 RID: 1698
		public int m_ammoSupplyCost = 1;

		// Token: 0x040006A3 RID: 1699
		public int m_ammoPerSupply = 1;
	}
}
