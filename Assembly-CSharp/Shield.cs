using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000FC RID: 252
[AddComponentMenu("Scripts/Modules/Shield")]
public class Shield : HPModule
{
	// Token: 0x0600099E RID: 2462 RVA: 0x000457D8 File Offset: 0x000439D8
	public override void Awake()
	{
		base.Awake();
		this.m_energy = this.m_maxEnergy;
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x000457EC File Offset: 0x000439EC
	public override StatusWnd_HPModule CreateStatusWindow(GameObject guiCamera)
	{
		bool friendly = base.GetOwner() == NetObj.m_localPlayerID;
		return new StatusWnd_Shield(this, guiCamera, friendly);
	}

	// Token: 0x060009A0 RID: 2464 RVA: 0x00045810 File Offset: 0x00043A10
	public void SetDeployShield(Shield.DeployType type)
	{
		this.m_deploy = type;
		if (NetObj.m_phase == TurnPhase.Planning && NetObj.m_localPlayerID == base.GetOwner())
		{
			this.SetupShieldPreview(type);
		}
	}

	// Token: 0x060009A1 RID: 2465 RVA: 0x0004583C File Offset: 0x00043A3C
	private void SetupShieldPreview(Shield.DeployType type)
	{
		if (this.m_shieldPreview != null)
		{
			UnityEngine.Object.Destroy(this.m_shieldPreview);
			this.m_shieldPreview = null;
		}
		if (type == Shield.DeployType.None)
		{
			return;
		}
		this.m_shieldPreview = (UnityEngine.Object.Instantiate(this.m_shieldPreviewPrefab) as GameObject);
		this.SetupShieldDirection(type, this.m_shieldPreview);
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x00045898 File Offset: 0x00043A98
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (this.m_shield != null)
		{
			ShieldGeometry component = this.m_shield.GetComponent<ShieldGeometry>();
			if (component != null)
			{
				component.SetVisible(visible);
			}
		}
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x000458DC File Offset: 0x00043ADC
	public Shield.DeployType GetDeployShield()
	{
		return this.m_deploy;
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x000458E4 File Offset: 0x00043AE4
	public override string GetStatusText()
	{
		if (this.m_deployed != Shield.DeployType.None)
		{
			return "Draining";
		}
		if (this.m_energy < this.m_maxEnergy)
		{
			return "Charging";
		}
		return "Standby";
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x00045914 File Offset: 0x00043B14
	public override void SaveOrders(BinaryWriter stream)
	{
		base.SaveOrders(stream);
		stream.Write((byte)this.m_deploy);
		stream.Write((byte)this.m_deployed);
		stream.Write(this.m_energy);
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x00045950 File Offset: 0x00043B50
	public override void LoadOrders(BinaryReader stream)
	{
		base.LoadOrders(stream);
		Shield.DeployType deployShield = (Shield.DeployType)stream.ReadByte();
		this.SetDeployShield(deployShield);
		this.m_deployed = (Shield.DeployType)stream.ReadByte();
		this.m_energy = stream.ReadSingle();
		if (this.m_deployed != Shield.DeployType.None)
		{
			this.Activate(this.m_deployed, false);
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x000459A4 File Offset: 0x00043BA4
	public override void GetChargeLevel(out float i, out float time)
	{
		if (this.m_disabled)
		{
			base.GetChargeLevel(out i, out time);
			return;
		}
		i = this.m_energy / this.m_maxEnergy;
		if (this.m_deployed != Shield.DeployType.None)
		{
			time = this.m_energy / this.m_drainRate;
		}
		else
		{
			time = (this.m_maxEnergy - this.m_energy) / this.m_chargeRate;
		}
	}

	// Token: 0x060009A8 RID: 2472 RVA: 0x00045A0C File Offset: 0x00043C0C
	private void Activate(Shield.DeployType type, bool firstTime)
	{
		this.m_deployed = type;
		if (this.m_shield == null)
		{
			this.m_shield = (UnityEngine.Object.Instantiate(this.m_shieldPrefab) as GameObject);
			this.SetupShieldDirection(type, this.m_shield);
			this.m_shield.GetComponent<ShieldGeometry>().Setup(this.m_unit, this, firstTime);
		}
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x00045A6C File Offset: 0x00043C6C
	private void SetupShieldDirection(Shield.DeployType type, GameObject shield)
	{
		Vector3 b = Vector3.zero;
		Vector3 forward = Vector3.zero;
		switch (type)
		{
		case Shield.DeployType.Forward:
			b = this.m_unit.transform.TransformDirection(new Vector3(0f, 0f, this.m_unit.GetLength() / 2f));
			forward = this.m_unit.transform.TransformDirection(Vector3.forward);
			break;
		case Shield.DeployType.Backward:
			b = this.m_unit.transform.TransformDirection(new Vector3(0f, 0f, -this.m_unit.GetLength() / 2f));
			forward = this.m_unit.transform.TransformDirection(-Vector3.forward);
			break;
		case Shield.DeployType.Left:
			b = this.m_unit.transform.TransformDirection(new Vector3(-this.m_unit.GetWidth() / 2f, 0f, 0f));
			forward = this.m_unit.transform.TransformDirection(Vector3.left);
			break;
		case Shield.DeployType.Right:
			b = this.m_unit.transform.TransformDirection(new Vector3(this.m_unit.GetWidth() / 2f, 0f, 0f));
			forward = this.m_unit.transform.TransformDirection(Vector3.right);
			break;
		}
		shield.transform.parent = base.gameObject.transform;
		shield.transform.position = this.m_unit.transform.position;
		shield.transform.position += b;
		shield.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x00045C3C File Offset: 0x00043E3C
	private void Deactivate(bool destroyed)
	{
		if (this.m_shield != null)
		{
			this.m_shield.GetComponent<ShieldGeometry>().Deactivate(destroyed);
		}
		this.m_deployed = Shield.DeployType.None;
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x00045C68 File Offset: 0x00043E68
	private UnitAi.AttackDirection GetShieldDirection()
	{
		Battery component = base.transform.parent.gameObject.GetComponent<Battery>();
		Section.SectionType sectionType = component.GetSectionType();
		if (sectionType == Section.SectionType.Front)
		{
			return UnitAi.AttackDirection.Front;
		}
		if (sectionType == Section.SectionType.Rear)
		{
			return UnitAi.AttackDirection.Back;
		}
		if (sectionType != Section.SectionType.Mid)
		{
			return UnitAi.AttackDirection.None;
		}
		if ((base.transform.position - base.GetUnit().transform.position).x < 0f)
		{
			return UnitAi.AttackDirection.Right;
		}
		return UnitAi.AttackDirection.Left;
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x00045CE4 File Offset: 0x00043EE4
	private Shield.DeployType AiToShieldDir(UnitAi.AttackDirection dir)
	{
		if (dir == UnitAi.AttackDirection.None)
		{
			return Shield.DeployType.None;
		}
		if (dir == UnitAi.AttackDirection.Front)
		{
			return Shield.DeployType.Forward;
		}
		if (dir == UnitAi.AttackDirection.Back)
		{
			return Shield.DeployType.Backward;
		}
		if (dir == UnitAi.AttackDirection.Left)
		{
			return Shield.DeployType.Left;
		}
		if (dir == UnitAi.AttackDirection.Right)
		{
			return Shield.DeployType.Right;
		}
		return Shield.DeployType.None;
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x00045D14 File Offset: 0x00043F14
	private void UpdateShieldAi()
	{
		if ((double)this.m_timeInTurn == 0.0)
		{
			this.m_timeInTurn += Time.fixedDeltaTime;
			if (base.GetOwner() <= 3)
			{
				return;
			}
			if (this.m_energy < this.m_maxEnergy)
			{
				return;
			}
			Unit unit = NetObj.GetByID(base.GetUnit().GetAi().m_targetId) as Unit;
			if (unit == null)
			{
				return;
			}
			UnitAi.AttackDirection attackDirection = base.GetUnit().GetAi().GetAttackDirection(unit.transform.position);
			Shield.DeployType deploy = this.AiToShieldDir(attackDirection);
			this.m_deploy = deploy;
		}
		else
		{
			this.m_timeInTurn += Time.fixedDeltaTime;
		}
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x00045DD4 File Offset: 0x00043FD4
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating)
		{
			this.UpdateShieldAi();
			if (this.m_deployed == Shield.DeployType.None && this.m_deploy != Shield.DeployType.None && this.m_energy >= this.m_maxEnergy)
			{
				this.Activate(this.m_deploy, true);
				this.m_deploy = Shield.DeployType.None;
			}
			if (this.m_deployed != Shield.DeployType.None)
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

	// Token: 0x060009AF RID: 2479 RVA: 0x00045EA8 File Offset: 0x000440A8
	protected override void OnDisabled()
	{
		base.OnDisabled();
		this.Drain(this.m_maxEnergy, true);
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x00045EC0 File Offset: 0x000440C0
	public void Drain(float energy, bool damaged)
	{
		this.m_energy -= energy;
		if (this.m_energy <= 0f)
		{
			this.m_energy = 0f;
			this.Deactivate(damaged);
		}
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x00045F00 File Offset: 0x00044100
	public override float GetMaxEnergy()
	{
		return this.m_maxEnergy;
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x00045F08 File Offset: 0x00044108
	public override float GetEnergy()
	{
		return this.m_energy;
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00045F10 File Offset: 0x00044110
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

	// Token: 0x060009B4 RID: 2484 RVA: 0x00045F78 File Offset: 0x00044178
	public override List<string> GetHardpointInfo()
	{
		return new List<string>
		{
			Localize.instance.Translate("$Energy") + " " + this.m_maxEnergy.ToString()
		};
	}

	// Token: 0x040007D9 RID: 2009
	public GameObject m_shieldPrefab;

	// Token: 0x040007DA RID: 2010
	public GameObject m_shieldPreviewPrefab;

	// Token: 0x040007DB RID: 2011
	public float m_maxEnergy = 100f;

	// Token: 0x040007DC RID: 2012
	public float m_chargeRate = 1f;

	// Token: 0x040007DD RID: 2013
	public float m_drainRate = 10f;

	// Token: 0x040007DE RID: 2014
	private Shield.DeployType m_deploy;

	// Token: 0x040007DF RID: 2015
	private Shield.DeployType m_deployed;

	// Token: 0x040007E0 RID: 2016
	private float m_energy = 100f;

	// Token: 0x040007E1 RID: 2017
	private GameObject m_shield;

	// Token: 0x040007E2 RID: 2018
	private GameObject m_shieldPreview;

	// Token: 0x040007E3 RID: 2019
	private float m_timeInTurn;

	// Token: 0x020000FD RID: 253
	public enum DeployType
	{
		// Token: 0x040007E5 RID: 2021
		None,
		// Token: 0x040007E6 RID: 2022
		Forward,
		// Token: 0x040007E7 RID: 2023
		Backward,
		// Token: 0x040007E8 RID: 2024
		Left,
		// Token: 0x040007E9 RID: 2025
		Right
	}
}
