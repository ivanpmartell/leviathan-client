using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000113 RID: 275
[AddComponentMenu("Scripts/Units/Platform")]
public class Platform : Unit
{
	// Token: 0x06000A37 RID: 2615 RVA: 0x0004AEA8 File Offset: 0x000490A8
	public Platform()
	{
		UnitAi ai = new UnitAi();
		this.m_Ai = ai;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x0004AF58 File Offset: 0x00049158
	public override void Awake()
	{
		base.Awake();
		this.m_health = this.m_maxHealth;
		this.m_supplyMask = (1 << LayerMask.NameToLayer("units") | 1 << LayerMask.NameToLayer("projectiles"));
		if (this.m_supplyEffect)
		{
			this.m_supplyEffect.GetComponent<ParticleSystem>().enableEmission = false;
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0004AFC0 File Offset: 0x000491C0
	public virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		if (this.m_platformPrefab)
		{
			Vector3 size = new Vector3(40f, 40f, 40f);
			Gizmos.DrawWireCube(base.transform.position, size);
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0004B010 File Offset: 0x00049210
	public virtual void OnEvent(string eventName)
	{
		Animation componentInChildren = base.GetComponentInChildren<Animation>();
		if (componentInChildren == null)
		{
			return;
		}
		this.m_currentAnimation = eventName;
		if (eventName == string.Empty)
		{
			componentInChildren.Stop();
		}
		else
		{
			componentInChildren.Play(eventName);
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0004B05C File Offset: 0x0004925C
	public void EventWarning(string eventName)
	{
		if (Application.isEditor)
		{
			string text = string.Concat(new string[]
			{
				base.name,
				"(",
				base.GetNetID().ToString(),
				") of type ",
				base.GetType().ToString()
			});
			string text2 = "Recived event '" + eventName + "' that it do not care about.";
			MessageLog.instance.ShowMessage(MessageLog.TextPosition.Bottom, text, text2, string.Empty, 2f);
			PLog.Log(text + " " + text2);
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0004B0F0 File Offset: 0x000492F0
	public override void Start()
	{
		base.Start();
		this.SetOwner(this.m_player);
		base.SetName(Localize.instance.Translate(this.m_displayName));
		if (base.IsDead())
		{
			if (this.m_destroyedSmoke)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.m_destroyedSmoke, base.transform.position, Quaternion.identity) as GameObject;
				gameObject.transform.parent = base.transform;
			}
			if (this.m_destroyedPlatformPrefab != null)
			{
				this.m_visual = (UnityEngine.Object.Instantiate(this.m_destroyedPlatformPrefab, base.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 0f)), base.transform.rotation) as GameObject);
				this.m_visual.transform.parent = base.transform;
			}
			else if (base.collider.enabled)
			{
				base.collider.enabled = false;
			}
		}
		else
		{
			this.SetupBatterys();
			if (this.m_platformPrefab != null)
			{
				this.m_visual = (UnityEngine.Object.Instantiate(this.m_platformPrefab, base.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 0f)), base.transform.rotation) as GameObject);
				this.m_visual.transform.parent = base.transform;
			}
			this.OnEvent(this.m_currentAnimation);
		}
		if (this.m_visual)
		{
			this.SetVisualVisiblility(this.m_visual, this.IsVisible());
		}
		base.Start();
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0004B2B0 File Offset: 0x000494B0
	public override void SetOwner(int owner)
	{
		base.SetOwner(owner);
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		foreach (Battery battery in componentsInChildren)
		{
			battery.SetOwner(owner);
			battery.Setup(this, 0);
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0004B2FC File Offset: 0x000494FC
	public override void Update()
	{
		base.Update();
		base.SetSightRange(this.m_baseSightRange);
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

	// Token: 0x06000A3F RID: 2623 RVA: 0x0004B380 File Offset: 0x00049580
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!NetObj.m_simulating)
		{
			return;
		}
		if (this.m_destructionTimer >= 0f)
		{
			this.m_destructionTimer -= Time.fixedDeltaTime;
			if (this.m_destructionTimer < 0f)
			{
				Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
				foreach (Battery battery in componentsInChildren)
				{
					battery.RemoveAll();
				}
				this.m_destructionTimer = -1f;
				UnityEngine.Object.Destroy(this.m_visual);
				if (this.m_destroyedPlatformPrefab != null)
				{
					this.m_visual = (UnityEngine.Object.Instantiate(this.m_destroyedPlatformPrefab, base.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 0f)), base.transform.rotation) as GameObject);
					this.m_visual.transform.parent = base.transform;
				}
				else if (base.collider.enabled)
				{
					base.collider.enabled = false;
				}
				this.OnKilled();
			}
		}
		if (!base.IsDead())
		{
			this.m_supplyTimer += Time.fixedDeltaTime;
			if (this.m_supplyTimer >= this.m_supplyDelay)
			{
				this.m_supplyTimer -= this.m_supplyDelay;
				if (!this.SupplyUnitsInRadius() && this.m_resources < this.m_maxResources)
				{
					this.m_resources += (int)((float)this.m_resupplyRate * this.m_supplyDelay);
					if (this.m_resources > this.m_maxResources)
					{
						this.m_resources = this.m_maxResources;
					}
				}
			}
		}
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0004B540 File Offset: 0x00049740
	public override bool IsValidTarget()
	{
		return this.m_allowAutotarget && !base.IsDead();
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x0004B560 File Offset: 0x00049760
	private bool SupplyUnitsInRadius()
	{
		if (!this.m_supplyEnabled)
		{
			return false;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.m_supplyRadius, this.m_supplyMask);
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		int resources = this.m_resources;
		int ownerTeam = base.GetOwnerTeam();
		foreach (Collider collider in array)
		{
			if (!(collider.attachedRigidbody == null) && !(collider.attachedRigidbody.gameObject == base.gameObject))
			{
				Unit component = collider.attachedRigidbody.GetComponent<Unit>();
				if (!(component == null))
				{
					if (!(this == component))
					{
						if (component.GetOwnerTeam() == ownerTeam)
						{
							if (!hashSet.Contains(component.gameObject))
							{
								hashSet.Add(component.gameObject);
							}
						}
					}
				}
			}
		}
		bool result = hashSet.Count != 0;
		if (this.m_resources <= 0)
		{
			return result;
		}
		foreach (GameObject gameObject in hashSet)
		{
			Unit component2 = gameObject.GetComponent<Unit>();
			component2.Supply(ref this.m_resources);
			if (this.m_resources <= 0)
			{
				break;
			}
		}
		if (this.m_resources != resources)
		{
			this.m_supplyEffectTimer = this.m_supplyDelay + 0.1f;
			if (this.m_supplyEffect != null)
			{
				this.m_supplyEffect.GetComponent<ParticleSystem>().enableEmission = true;
			}
			return true;
		}
		return result;
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x0004B744 File Offset: 0x00049944
	private void SetupBatterys()
	{
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		if (this.m_batteryModules == null || this.m_batteryModules.Count < 1)
		{
			foreach (Battery battery in componentsInChildren)
			{
				if (this.m_placeGun.Length != 0 && battery.CanPlaceAt(0, 0, 1, 1, null))
				{
					battery.AddHPModule(this.m_placeGun, 0, 0, Direction.Forward);
				}
			}
			return;
		}
		int num = 0;
		foreach (Battery battery2 in componentsInChildren)
		{
			if (num < this.m_batteryModules.Count && battery2.CanPlaceAt(0, 0, 1, 1, null))
			{
				battery2.AddHPModule(this.m_batteryModules[num], 0, 0, Direction.Forward);
			}
			num++;
		}
	}

	// Token: 0x06000A43 RID: 2627 RVA: 0x0004B82C File Offset: 0x00049A2C
	protected override void OnSetSimulating(bool enabled)
	{
		base.OnSetSimulating(enabled);
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x0004B838 File Offset: 0x00049A38
	public override bool TestLOS(NetObj obj)
	{
		float num = Vector3.Distance(base.transform.position, obj.transform.position);
		return num <= this.m_baseSightRange;
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x0004B870 File Offset: 0x00049A70
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_destructionTimer);
		writer.Write(this.m_baseSightRange);
		writer.Write((short)this.m_health);
		writer.Write((short)this.m_maxHealth);
		writer.Write(this.m_Width);
		writer.Write(this.m_length);
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		writer.Write((byte)componentsInChildren.Length);
		foreach (Battery battery in componentsInChildren)
		{
			battery.SaveState(writer);
		}
		writer.Write(this.m_alwaysVisible);
		writer.Write(this.m_currentAnimation);
		writer.Write(this.m_resources);
		writer.Write(this.m_supplyTimer);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x0004B938 File Offset: 0x00049B38
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_destructionTimer = reader.ReadSingle();
		this.m_baseSightRange = reader.ReadSingle();
		this.m_player = base.GetOwner();
		this.m_health = (int)reader.ReadInt16();
		this.m_maxHealth = (int)reader.ReadInt16();
		this.m_Width = reader.ReadSingle();
		this.m_length = reader.ReadSingle();
		Battery[] componentsInChildren = base.transform.GetComponentsInChildren<Battery>();
		int num = (int)reader.ReadByte();
		DebugUtils.Assert(num == componentsInChildren.Length, "number of batteries missmatch");
		foreach (Battery battery in componentsInChildren)
		{
			battery.LoadState(reader);
		}
		this.m_alwaysVisible = reader.ReadBoolean();
		this.m_currentAnimation = reader.ReadString();
		this.m_resources = reader.ReadInt32();
		this.m_supplyTimer = reader.ReadSingle();
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x0004BA1C File Offset: 0x00049C1C
	public override string GetTooltip()
	{
		string text = string.Empty;
		text = text + this.GetName() + "\n";
		if (this.m_health > 0)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Health: ",
				this.m_health,
				"\n"
			});
		}
		else
		{
			text += "Health: Destroyed\n";
		}
		return text + "Resources: " + this.m_resources.ToString() + "\n";
	}

	// Token: 0x06000A48 RID: 2632 RVA: 0x0004BAAC File Offset: 0x00049CAC
	public override bool Damage(Hit hit)
	{
		if (this.m_health <= 0 || this.m_dead)
		{
			return true;
		}
		if (this.m_destructionTimer > 0f)
		{
			return false;
		}
		if (this.m_immuneToDamage)
		{
			HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipDeflectHit);
			return false;
		}
		int num;
		GameRules.HitOutcome hitOutcome = GameRules.CalculateDamage(this.m_health, this.m_armorClass, hit.m_damage, hit.m_armorPiercing, out num);
		this.m_health -= num;
		if (this.IsVisible())
		{
			switch (hitOutcome)
			{
			case GameRules.HitOutcome.CritHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipCriticalHit);
				break;
			case GameRules.HitOutcome.PiercedArmor:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipPiercingHit);
				break;
			case GameRules.HitOutcome.GlancingHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipGlancingHit);
				break;
			case GameRules.HitOutcome.Deflected:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipDeflectHit);
				break;
			}
			if (this.m_health <= 0)
			{
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, this.GetName(), Constants.m_shipDestroyedHit);
			}
		}
		if (this.m_health <= 0)
		{
			this.m_destructionTimer = 2f;
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_explosionPrefab, base.transform.localToWorldMatrix.MultiplyPoint3x4(new Vector3(0f, 0f, 0f)), Quaternion.identity) as GameObject;
			gameObject.transform.position = base.transform.position;
		}
		return num > 0;
	}

	// Token: 0x06000A49 RID: 2633 RVA: 0x0004BCB0 File Offset: 0x00049EB0
	public override void SetVisible(bool visible)
	{
		if (this.IsVisible() == visible)
		{
			return;
		}
		base.SetVisible(visible);
		this.SetVisualVisiblility(base.gameObject, visible);
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0004BCE0 File Offset: 0x00049EE0
	public void SetVisualVisiblility(GameObject visual, bool visible)
	{
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = visible;
		}
		if (visual.renderer)
		{
			visual.renderer.renderer.enabled = visible;
		}
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0004BD34 File Offset: 0x00049F34
	public override int GetHealth()
	{
		if (this.m_showTrueHealth)
		{
			return this.m_health;
		}
		if (!this.m_immuneToDamage)
		{
			return this.m_health;
		}
		HPModule componentInChildren = base.transform.GetComponentInChildren<HPModule>();
		if (componentInChildren)
		{
			return componentInChildren.GetHealth();
		}
		return this.m_health;
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x0004BD8C File Offset: 0x00049F8C
	public override int GetMaxHealth()
	{
		if (this.m_showTrueHealth)
		{
			return this.m_maxHealth;
		}
		if (!this.m_immuneToDamage)
		{
			return this.m_maxHealth;
		}
		HPModule componentInChildren = base.transform.GetComponentInChildren<HPModule>();
		if (componentInChildren)
		{
			return componentInChildren.GetMaxHealth();
		}
		return this.m_health;
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x0004BDE4 File Offset: 0x00049FE4
	public override float GetLength()
	{
		return this.m_length;
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x0004BDEC File Offset: 0x00049FEC
	public override float GetWidth()
	{
		return this.m_Width;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x0004BDF4 File Offset: 0x00049FF4
	protected bool SetupLineDrawer()
	{
		if (!(this.m_lineDrawer == null))
		{
			return true;
		}
		this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
		if (this.m_lineDrawer == null)
		{
			return false;
		}
		this.m_supplyAreaLineType = this.m_lineDrawer.GetTypeID("supplyArea");
		this.m_supplyAreaDisabledLineType = this.m_lineDrawer.GetTypeID("supplyAreaDisabled");
		DebugUtils.Assert(this.m_supplyAreaLineType > 0);
		return true;
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x0004BE74 File Offset: 0x0004A074
	private void DrawSupplyArea()
	{
		if (!this.m_supplyEnabled)
		{
			return;
		}
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
		if (this.IsSupplying())
		{
			this.m_lineDrawer.DrawXZCircle(position, this.m_supplyRadius, 40, this.m_supplyAreaLineType, 0.15f);
		}
		else
		{
			this.m_lineDrawer.DrawXZCircle(position, this.m_supplyRadius, 40, this.m_supplyAreaDisabledLineType, 0.15f);
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x0004BF24 File Offset: 0x0004A124
	private bool IsSupplying()
	{
		return this.m_supplyEffectTimer > 0f;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x0004BF3C File Offset: 0x0004A13C
	public int GetResources()
	{
		return this.m_resources;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0004BF44 File Offset: 0x0004A144
	public int GetMaxResources()
	{
		return this.m_maxResources;
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0004BF4C File Offset: 0x0004A14C
	public override string GetName()
	{
		if (this.m_placeGun.Length == 0)
		{
			return base.GetName();
		}
		return Localize.instance.TranslateKey(this.m_placeGun + "_name");
	}

	// Token: 0x04000880 RID: 2176
	private int m_health;

	// Token: 0x04000881 RID: 2177
	public GameObject m_explosionPrefab;

	// Token: 0x04000882 RID: 2178
	public GameObject m_explosionLowPrefab;

	// Token: 0x04000883 RID: 2179
	public GameObject m_destroyedSmoke;

	// Token: 0x04000884 RID: 2180
	public GameObject m_platformPrefab;

	// Token: 0x04000885 RID: 2181
	public GameObject m_destroyedPlatformPrefab;

	// Token: 0x04000886 RID: 2182
	public int m_player = 7;

	// Token: 0x04000887 RID: 2183
	public int m_maxHealth = 500;

	// Token: 0x04000888 RID: 2184
	public int m_armorClass = 10;

	// Token: 0x04000889 RID: 2185
	public float m_baseSightRange = 100f;

	// Token: 0x0400088A RID: 2186
	public float m_Width = 40f;

	// Token: 0x0400088B RID: 2187
	public float m_length = 40f;

	// Token: 0x0400088C RID: 2188
	public List<string> m_batteryModules;

	// Token: 0x0400088D RID: 2189
	public string m_placeGun;

	// Token: 0x0400088E RID: 2190
	public string m_displayName = "$platform";

	// Token: 0x0400088F RID: 2191
	public string m_currentAnimation = string.Empty;

	// Token: 0x04000890 RID: 2192
	private float m_destructionTimer = -1f;

	// Token: 0x04000891 RID: 2193
	private GameObject m_visual;

	// Token: 0x04000892 RID: 2194
	public bool m_immuneToDamage;

	// Token: 0x04000893 RID: 2195
	public bool m_showTrueHealth;

	// Token: 0x04000894 RID: 2196
	public bool m_alwaysVisible;

	// Token: 0x04000895 RID: 2197
	public bool m_supplyEnabled;

	// Token: 0x04000896 RID: 2198
	public float m_supplyRadius = 20f;

	// Token: 0x04000897 RID: 2199
	public int m_resources = 1000;

	// Token: 0x04000898 RID: 2200
	public int m_maxResources = 1000;

	// Token: 0x04000899 RID: 2201
	public float m_supplyDelay = 0.5f;

	// Token: 0x0400089A RID: 2202
	public int m_resupplyRate = 15;

	// Token: 0x0400089B RID: 2203
	public GameObject m_supplyEffect;

	// Token: 0x0400089C RID: 2204
	protected LineDrawer m_lineDrawer;

	// Token: 0x0400089D RID: 2205
	private int m_supplyAreaLineType;

	// Token: 0x0400089E RID: 2206
	private int m_supplyAreaDisabledLineType;

	// Token: 0x0400089F RID: 2207
	private float m_supplyTimer;

	// Token: 0x040008A0 RID: 2208
	private float m_supplyEffectTimer;

	// Token: 0x040008A1 RID: 2209
	private int m_supplyMask;
}
