using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000103 RID: 259
[AddComponentMenu("Scripts/Units/SupportShip")]
public class SupportShip : Ship
{
	// Token: 0x060009D1 RID: 2513 RVA: 0x00046B48 File Offset: 0x00044D48
	public override void Awake()
	{
		base.Awake();
		this.m_supplyMask = (1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("projectiles"));
		this.m_supplyEffect.GetComponent<ParticleSystem>().enableEmission = false;
		this.m_resources = this.m_maxResources;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00046BA0 File Offset: 0x00044DA0
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (NetObj.m_simulating && !base.IsDead())
		{
			this.m_supplyTimer += Time.fixedDeltaTime;
			if (this.m_supplyTimer >= this.m_supplyDelay)
			{
				this.m_supplyTimer -= this.m_supplyDelay;
				if (this.m_resources < this.m_maxResources)
				{
					this.m_resources += (int)((float)this.m_resupplyRate * this.m_supplyDelay);
					if (this.m_resources > this.m_maxResources)
					{
						this.m_resources = this.m_maxResources;
					}
				}
				this.SupplyUnitsInRadius();
			}
		}
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00046C50 File Offset: 0x00044E50
	public override void Update()
	{
		base.Update();
		if (!base.IsDead())
		{
			this.DrawSupplyArea();
		}
		if (this.m_supplyEffectTimer > 0f)
		{
			this.m_supplyEffectTimer -= Time.deltaTime;
			if (this.m_supplyEffectTimer <= 0f && this.m_supplyEffect != null)
			{
				this.m_supplyEffect.GetComponent<ParticleSystem>().enableEmission = false;
			}
		}
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00046CC8 File Offset: 0x00044EC8
	public override string GetTooltip()
	{
		string tooltip = base.GetTooltip();
		return tooltip + "Supply:" + this.m_resources;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00046CF4 File Offset: 0x00044EF4
	protected override bool SetupLineDrawer()
	{
		if (base.SetupLineDrawer())
		{
			this.m_supplyAreaLineType = this.m_lineDrawer.GetTypeID("supplyArea");
			this.m_supplyAreaDisabledLineType = this.m_lineDrawer.GetTypeID("supplyAreaDisabled");
			DebugUtils.Assert(this.m_supplyAreaLineType > 0);
			return true;
		}
		return false;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00046D4C File Offset: 0x00044F4C
	private void DrawSupplyArea()
	{
		if (!this.SetupLineDrawer())
		{
			return;
		}
		if (base.GetOwnerTeam() != TurnMan.instance.GetPlayerTeam(NetObj.m_localPlayerID))
		{
			return;
		}
		Vector3 position = base.transform.position;
		position.y += 2f;
		if (this.m_supplyEnabled)
		{
			this.m_lineDrawer.DrawXZCircle(position, this.m_supplyRadius, 40, this.m_supplyAreaLineType, 0.15f);
		}
		else
		{
			this.m_lineDrawer.DrawXZCircle(position, this.m_supplyRadius, 40, this.m_supplyAreaDisabledLineType, 0.15f);
		}
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00046DF0 File Offset: 0x00044FF0
	private void SupplyUnitsInRadius()
	{
		if (!this.m_supplyEnabled)
		{
			return;
		}
		if (this.m_resources <= 0)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.m_supplyRadius, this.m_supplyMask);
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		int resources = this.m_resources;
		int ownerTeam = base.GetOwnerTeam();
		foreach (Collider collider in array)
		{
			Mine component = collider.GetComponent<Mine>();
			if (component != null)
			{
				component.Disarm();
			}
			else if (!(collider.attachedRigidbody == null) && !(collider.attachedRigidbody.gameObject == base.gameObject))
			{
				Unit component2 = collider.attachedRigidbody.GetComponent<Unit>();
				if (!(component2 == null))
				{
					if (!(this == component2))
					{
						if (component2.GetOwnerTeam() == ownerTeam)
						{
							if (!hashSet.Contains(component2.gameObject))
							{
								hashSet.Add(component2.gameObject);
								component2.Supply(ref this.m_resources);
								if (this.m_resources <= 0)
								{
									break;
								}
							}
						}
					}
				}
			}
		}
		if (this.m_resources != resources)
		{
			this.m_supplyEffectTimer = this.m_supplyDelay + 0.1f;
			if (this.m_supplyEffect != null)
			{
				this.m_supplyEffect.GetComponent<ParticleSystem>().enableEmission = true;
			}
		}
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00046F88 File Offset: 0x00045188
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_resources);
		writer.Write(this.m_supplyTimer);
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00046FAC File Offset: 0x000451AC
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_resources = reader.ReadInt32();
		this.m_supplyTimer = reader.ReadSingle();
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00046FD0 File Offset: 0x000451D0
	public override void SaveOrders(BinaryWriter stream)
	{
		base.SaveOrders(stream);
		stream.Write(this.m_supplyEnabled);
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00046FE8 File Offset: 0x000451E8
	public override void LoadOrders(BinaryReader stream)
	{
		base.LoadOrders(stream);
		this.m_supplyEnabled = stream.ReadBoolean();
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00047000 File Offset: 0x00045200
	public int GetResources()
	{
		return this.m_resources;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00047008 File Offset: 0x00045208
	public int GetMaxResources()
	{
		return this.m_maxResources;
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00047010 File Offset: 0x00045210
	public bool IsSupplyEnabled()
	{
		return this.m_supplyEnabled;
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00047018 File Offset: 0x00045218
	public void SetSupplyEnabled(bool enabled)
	{
		this.m_supplyEnabled = enabled;
	}

	// Token: 0x0400080A RID: 2058
	public int m_resources = 1000;

	// Token: 0x0400080B RID: 2059
	public int m_maxResources = 1000;

	// Token: 0x0400080C RID: 2060
	public float m_supplyDelay = 0.5f;

	// Token: 0x0400080D RID: 2061
	public float m_supplyRadius = 20f;

	// Token: 0x0400080E RID: 2062
	public GameObject m_supplyEffect;

	// Token: 0x0400080F RID: 2063
	public int m_resupplyRate = 2;

	// Token: 0x04000810 RID: 2064
	private float m_supplyEffectTimer;

	// Token: 0x04000811 RID: 2065
	private float m_supplyTimer;

	// Token: 0x04000812 RID: 2066
	private int m_supplyMask;

	// Token: 0x04000813 RID: 2067
	private int m_supplyAreaLineType;

	// Token: 0x04000814 RID: 2068
	private int m_supplyAreaDisabledLineType;

	// Token: 0x04000815 RID: 2069
	private bool m_supplyEnabled = true;
}
