using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000C8 RID: 200
[AddComponentMenu("Scripts/Modules/Cloak")]
public class Cloak : HPModule
{
	// Token: 0x06000737 RID: 1847 RVA: 0x00036340 File Offset: 0x00034540
	public override void Awake()
	{
		base.Awake();
		this.m_energy = this.m_maxEnergy;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00036354 File Offset: 0x00034554
	public override void Setup(Unit unit, Battery battery, int x, int y, Direction dir, HPModule.DestroyedHandler destroyedCallback)
	{
		base.Setup(unit, battery, x, y, dir, destroyedCallback);
		unit.m_onFireWeapon = (Action)Delegate.Combine(unit.m_onFireWeapon, new Action(this.OnShipFireWeapon));
		unit.m_onTakenDamage = (Action)Delegate.Combine(unit.m_onTakenDamage, new Action(this.OnShipTakenDamage));
		unit.m_onMaintenanceActivation = (Action)Delegate.Combine(unit.m_onMaintenanceActivation, new Action(this.OnMaintenanceModeActivation));
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000363D8 File Offset: 0x000345D8
	private void OnShipFireWeapon()
	{
		if (NetObj.m_phase == TurnPhase.Testing && base.GetOwner() != NetObj.m_localPlayerID)
		{
			return;
		}
		if (this.m_deployed)
		{
			PLog.Log("Gun fired, disabling cloak");
			this.Drain(this.m_maxEnergy, false);
		}
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00036418 File Offset: 0x00034618
	public override void GetChargeLevel(out float i, out float time)
	{
		if (this.m_disabled)
		{
			base.GetChargeLevel(out i, out time);
			return;
		}
		i = this.m_energy / this.m_maxEnergy;
		if (this.m_deployed)
		{
			time = this.m_energy / this.m_drainRate;
		}
		else
		{
			time = (this.m_maxEnergy - this.m_energy) / this.m_chargeRate;
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00036480 File Offset: 0x00034680
	private void OnMaintenanceModeActivation()
	{
		if (this.m_deployed)
		{
			PLog.Log("Maintenance mode, disabling cloak");
			this.Drain(this.m_maxEnergy, false);
		}
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x000364B0 File Offset: 0x000346B0
	private void OnShipTakenDamage()
	{
		if (NetObj.m_phase == TurnPhase.Testing && base.GetOwner() != NetObj.m_localPlayerID)
		{
			return;
		}
		if (this.m_deployed)
		{
			PLog.Log("Damage taken, disabling cloak");
			this.Drain(this.m_maxEnergy, false);
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x000364F0 File Offset: 0x000346F0
	public override StatusWnd_HPModule CreateStatusWindow(GameObject guiCamera)
	{
		bool friendly = base.GetOwner() == NetObj.m_localPlayerID;
		return new StatusWnd_Shield(this, guiCamera, friendly);
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00036514 File Offset: 0x00034714
	public override void SetDeploy(bool deploy)
	{
		this.m_deploy = deploy;
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00036520 File Offset: 0x00034720
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00036564 File Offset: 0x00034764
	public override bool GetDeploy()
	{
		return this.m_deploy;
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0003656C File Offset: 0x0003476C
	public override string GetStatusText()
	{
		if (this.m_deployed)
		{
			return "Draining";
		}
		if (this.m_energy < this.m_maxEnergy)
		{
			return "Charging";
		}
		return "Standby";
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0003659C File Offset: 0x0003479C
	public override void SaveOrders(BinaryWriter stream)
	{
		base.SaveOrders(stream);
		stream.Write(this.m_deploy);
		stream.Write(this.m_deployed);
		stream.Write(this.m_energy);
		stream.Write(this.m_deployTimer);
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x000365E0 File Offset: 0x000347E0
	public override void LoadOrders(BinaryReader stream)
	{
		base.LoadOrders(stream);
		this.m_deploy = stream.ReadBoolean();
		this.m_deployed = stream.ReadBoolean();
		this.m_energy = stream.ReadSingle();
		this.m_deployTimer = stream.ReadSingle();
		if (this.m_deployed)
		{
			this.Activate(false);
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00036638 File Offset: 0x00034838
	private void Activate(bool firstTime)
	{
		PLog.Log("Activating cloaking field");
		if (firstTime)
		{
			this.m_deployed = true;
			this.m_deployTimer = 0f;
			if (this.m_activation == null && this.IsVisible())
			{
				this.m_activation = (UnityEngine.Object.Instantiate(this.m_activateEffectPrefab, this.m_unit.transform.position, this.m_unit.transform.rotation) as GameObject);
				this.m_activation.transform.parent = base.transform;
				this.UpdateCloakScale();
			}
		}
		else
		{
			this.SetupCloakEffect();
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x000366E0 File Offset: 0x000348E0
	private void Deactivate(bool destroyed)
	{
		PLog.Log("Deactivating cloaking field");
		this.m_deploy = false;
		this.m_deployed = false;
		this.m_energy = 0f;
		this.m_unit.SetCloaked(false);
		this.m_deployTimer = -1f;
		if (this.m_activation != null)
		{
			UnityEngine.Object.Destroy(this.m_activation);
			this.m_activation = null;
		}
		if (this.m_cloak != null)
		{
			UnityEngine.Object.Destroy(this.m_cloak);
			this.m_cloak = null;
		}
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00036770 File Offset: 0x00034970
	private void UpdateCloakScale()
	{
		if (this.m_activation == null)
		{
			return;
		}
		float num = this.m_deployTimer / (this.m_cloakDelay * 2f);
		float num2 = Mathf.Sin(num * 3.1415927f);
		float length = this.m_unit.GetLength();
		float width = this.m_unit.GetWidth();
		this.m_activation.transform.localScale = new Vector3(num2 * width, num2 * 10f, num2 * length);
		if (num > 1f)
		{
			UnityEngine.Object.Destroy(this.m_activation);
			this.m_activation = null;
		}
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00036808 File Offset: 0x00034A08
	private void SetupCloakEffect()
	{
		if (this.m_cloak != null)
		{
			return;
		}
		float length = this.m_unit.GetLength();
		float width = this.m_unit.GetWidth();
		this.m_cloak = (UnityEngine.Object.Instantiate(this.m_cloakEffectPrefab, this.m_unit.transform.position, this.m_unit.transform.rotation) as GameObject);
		this.m_cloak.transform.parent = base.transform;
		this.m_cloak.transform.localScale = new Vector3(width * 1.5f, 1f, length);
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = this.IsVisible();
		}
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x000368E8 File Offset: 0x00034AE8
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating)
		{
			if (this.m_deployTimer >= 0f)
			{
				this.m_deployTimer += Time.fixedDeltaTime;
				if (this.m_deployTimer > this.m_cloakDelay && !this.m_unit.IsCloaked())
				{
					this.m_unit.SetCloaked(true);
					PLog.Log("Cloak active");
					this.SetupCloakEffect();
				}
				this.UpdateCloakScale();
			}
			if (!this.m_deployed && this.m_deploy && this.m_energy >= this.m_maxEnergy)
			{
				this.Activate(true);
			}
			if (this.m_deployed && !this.m_deploy)
			{
				this.Deactivate(false);
			}
			if (this.m_deployed)
			{
				this.Drain(this.m_drainRate * Time.fixedDeltaTime, false);
			}
			else if (!base.IsDisabled() && !this.m_unit.IsDead())
			{
				this.m_energy += this.m_chargeRate * Time.fixedDeltaTime;
				if (this.m_energy > this.m_maxEnergy)
				{
					this.m_energy = this.m_maxEnergy;
				}
			}
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00036A2C File Offset: 0x00034C2C
	protected override void OnDisabled()
	{
		base.OnDisabled();
		this.Drain(this.m_maxEnergy, true);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00036A44 File Offset: 0x00034C44
	public void Drain(float energy, bool damaged)
	{
		if (this.m_energy <= 0f)
		{
			return;
		}
		this.m_energy -= energy;
		if (this.m_energy <= 0f)
		{
			this.m_energy = 0f;
			this.Deactivate(damaged);
		}
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00036A94 File Offset: 0x00034C94
	public override float GetMaxEnergy()
	{
		return this.m_maxEnergy;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00036A9C File Offset: 0x00034C9C
	public override float GetEnergy()
	{
		return this.m_energy;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00036AA4 File Offset: 0x00034CA4
	public override string GetTooltip()
	{
		string text = base.GetName() + "\nHP: " + this.m_health;
		string text2 = text;
		return string.Concat(new string[]
		{
			text2,
			"\nEnergy: ",
			this.m_energy.ToString(),
			" / ",
			this.m_maxEnergy.ToString()
		});
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00036B0C File Offset: 0x00034D0C
	public override List<string> GetHardpointInfo()
	{
		return new List<string>
		{
			Localize.instance.Translate("$Energy") + " " + this.m_maxEnergy.ToString()
		};
	}

	// Token: 0x040005F0 RID: 1520
	public float m_maxEnergy = 100f;

	// Token: 0x040005F1 RID: 1521
	public float m_chargeRate = 1f;

	// Token: 0x040005F2 RID: 1522
	public float m_drainRate = 10f;

	// Token: 0x040005F3 RID: 1523
	private float m_cloakDelay = 1f;

	// Token: 0x040005F4 RID: 1524
	public GameObject m_activateEffectPrefab;

	// Token: 0x040005F5 RID: 1525
	public GameObject m_cloakEffectPrefab;

	// Token: 0x040005F6 RID: 1526
	private GameObject m_activation;

	// Token: 0x040005F7 RID: 1527
	private GameObject m_cloak;

	// Token: 0x040005F8 RID: 1528
	private float m_energy = 100f;

	// Token: 0x040005F9 RID: 1529
	private bool m_deploy;

	// Token: 0x040005FA RID: 1530
	private bool m_deployed;

	// Token: 0x040005FB RID: 1531
	private float m_deployTimer = -1f;
}
