using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000DA RID: 218
[AddComponentMenu("Scripts/Modules/HPModule")]
public class HPModule : NetObj, IOrderable
{
	// Token: 0x0600083F RID: 2111 RVA: 0x0003D9F4 File Offset: 0x0003BBF4
	public override void Awake()
	{
		base.Awake();
		this.m_save = false;
		this.m_updateSeenBy = false;
		this.m_health = this.m_maxHealth;
		this.m_settings = ComponentDB.instance.GetModule(base.name.Substring(0, base.name.Length - 7));
		DebugUtils.Assert(this.m_settings != null);
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0003DA5C File Offset: 0x0003BC5C
	public override void OnDestroy()
	{
		base.OnDestroy();
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0003DA84 File Offset: 0x0003BC84
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write((short)this.m_health);
		writer.Write(this.m_disabled);
		writer.Write(this.m_destructionTimer);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0003DAC0 File Offset: 0x0003BCC0
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_health = (int)reader.ReadInt16();
		this.m_disabled = reader.ReadBoolean();
		this.m_destructionTimer = reader.ReadSingle();
		if (!this.GetUnit().IsDead())
		{
			this.SetPersistantDisableEffect(this.m_disabled);
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0003DB14 File Offset: 0x0003BD14
	protected virtual void FixedUpdate()
	{
		if (NetObj.m_simulating)
		{
			if (this.m_destructionTimer >= 0f)
			{
				this.m_destructionTimer -= Time.fixedDeltaTime;
				if (this.m_destructionTimer < 0f)
				{
					this.Damage(new Hit(this.GetMaxHealth(), 100), false);
				}
			}
			if (this.m_disabled && !this.m_unit.IsSinking())
			{
				this.UpdateRepair(Time.fixedDeltaTime);
			}
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0003DB98 File Offset: 0x0003BD98
	private void UpdateRepair(float dt)
	{
		this.m_repairTimer += dt;
		if (this.m_repairTimer >= 1f)
		{
			this.m_repairTimer = 0f;
			this.Heal(this.m_repairAmount);
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0003DBD0 File Offset: 0x0003BDD0
	public void Heal(int health)
	{
		this.m_health += health;
		if (this.m_health > this.m_maxHealth)
		{
			this.m_health = this.m_maxHealth;
		}
		if (this.m_disabled && this.m_maxHealth == this.m_health)
		{
			this.OnEnabled();
		}
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0003DC2C File Offset: 0x0003BE2C
	public virtual List<string> GetHardpointInfo()
	{
		return new List<string>();
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0003DC40 File Offset: 0x0003BE40
	public virtual void Update()
	{
		if (NetObj.m_drawOrders)
		{
			this.DrawOrders();
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0003DC54 File Offset: 0x0003BE54
	public virtual void Supply(ref int resources)
	{
		if (resources > 12 && this.m_health < this.m_maxHealth)
		{
			resources -= 12;
			this.m_health += 10;
			if (this.m_health > this.m_maxHealth)
			{
				this.m_health = this.m_maxHealth;
			}
		}
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0003DCB0 File Offset: 0x0003BEB0
	public void SetDir(Direction dir)
	{
		this.m_dir = dir;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0003DCBC File Offset: 0x0003BEBC
	public virtual void Setup(Unit unit, Battery battery, int x, int y, Direction dir, HPModule.DestroyedHandler destroyedCallback)
	{
		this.m_unit = unit;
		this.m_battery = battery;
		this.m_gridPos = new Vector2i(x, y);
		this.m_dir = dir;
		this.m_onDestroyed = destroyedCallback;
		switch (this.m_dir)
		{
		case Direction.Right:
			base.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
			break;
		case Direction.Backward:
			base.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			break;
		case Direction.Left:
			base.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
			break;
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0003DD88 File Offset: 0x0003BF88
	public Direction GetDir()
	{
		return this.m_dir;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0003DD90 File Offset: 0x0003BF90
	public virtual Vector3 GetVelocity()
	{
		return this.m_unit.GetVelocity();
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0003DDA0 File Offset: 0x0003BFA0
	public virtual void GetChargeLevel(out float i, out float time)
	{
		if (this.m_disabled)
		{
			i = (float)(this.m_health / this.m_maxHealth);
			time = (float)((this.m_maxHealth - this.m_health) / this.m_repairAmount);
		}
		i = -1f;
		time = -1f;
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0003DDF0 File Offset: 0x0003BFF0
	public virtual string GetTooltip()
	{
		return base.gameObject.name;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0003DE00 File Offset: 0x0003C000
	public void EnableSelectionMarker(bool enabled)
	{
		if (enabled == this.IsSelected())
		{
			return;
		}
		if (enabled)
		{
			this.m_marker = (UnityEngine.Object.Instantiate(this.m_selectionMarkerPrefab, base.transform.position, base.transform.rotation) as GameObject);
			this.m_marker.transform.parent = base.transform;
			Vector3 size = (base.collider as BoxCollider).bounds.size;
			float num = (size.x <= size.z) ? size.x : size.x;
			this.m_marker.transform.localScale = new Vector3(num, num, num);
			this.m_marker.transform.localPosition = new Vector3(0f, 0.2f, 0f);
		}
		else
		{
			UnityEngine.Object.Destroy(this.m_marker);
			this.m_marker = null;
		}
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0003DEF8 File Offset: 0x0003C0F8
	public virtual void SetSelected(bool selected, bool explicitSelected)
	{
		if (selected == this.IsSelected())
		{
			return;
		}
		this.EnableSelectionMarker(selected);
		if (selected && this.m_selectSound != null && explicitSelected)
		{
			UnityEngine.Object.Instantiate(this.m_selectSound, base.transform.position, Quaternion.identity);
		}
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0003DF54 File Offset: 0x0003C154
	public Section GetSection()
	{
		return this.m_battery.GetSection();
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0003DF64 File Offset: 0x0003C164
	public bool IsSelected()
	{
		return this.m_marker != null;
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0003DF74 File Offset: 0x0003C174
	public void TimedDestruction(float time)
	{
		if (this.m_destructionTimer < 0f)
		{
			this.m_destructionTimer = time;
		}
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0003DF90 File Offset: 0x0003C190
	public bool Damage(Hit hit, bool showDmgText)
	{
		if (this.m_health <= 0 || this.m_disabled)
		{
			if (this.GetUnit().IsDead())
			{
				this.OnEnabled();
				this.m_disabled = true;
			}
			return true;
		}
		int num;
		GameRules.HitOutcome hitOutcome = GameRules.CalculateDamage(this.m_health, this.m_armorClass, hit.m_damage, hit.m_armorPiercing, out num);
		this.m_health -= num;
		if (hit.m_dealer != null && num > 0)
		{
			Unit unit = hit.GetUnit();
			Gun gun = hit.GetGun();
			if (unit != null)
			{
				UnitAi ai = this.m_unit.GetAi();
				ai.SetTargetId(unit);
			}
			string gunName = (!(gun != null)) ? string.Empty : gun.name;
			TurnMan.instance.AddPlayerDamage(hit.m_dealer.GetOwner(), num, hit.m_dealer.GetOwnerTeam() == base.GetOwnerTeam(), gunName);
		}
		if (this.IsVisible() && showDmgText)
		{
			switch (hitOutcome)
			{
			case GameRules.HitOutcome.CritHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_moduleCriticalHit);
				break;
			case GameRules.HitOutcome.PiercedArmor:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_modulePiercingHit);
				break;
			case GameRules.HitOutcome.GlancingHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_moduleGlancingHit);
				break;
			case GameRules.HitOutcome.Deflected:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_moduleDeflectHit);
				break;
			}
			if (this.m_health <= 0)
			{
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, this.GetName(), Constants.m_moduleDisabledHit);
			}
		}
		if (this.m_health <= 0)
		{
			this.OnDisabled();
		}
		if (hit.m_havePoint && hit.m_dir != Vector3.zero)
		{
			Ship ship = this.GetUnit() as Ship;
			if (ship != null)
			{
				ship.ApplyImpulse(hit.m_point, hit.m_dir * (float)num, false);
			}
		}
		if (this.m_unit.m_onTakenDamage != null)
		{
			this.m_unit.m_onTakenDamage();
		}
		return num > 0;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0003E220 File Offset: 0x0003C420
	public void Remove()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		this.m_onDestroyed(this);
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0003E23C File Offset: 0x0003C43C
	private void OnEnabled()
	{
		if (!this.m_disabled)
		{
			return;
		}
		if (this.m_enableEffect != null)
		{
			UnityEngine.Object.Instantiate(this.m_enableEffect, base.transform.position, base.transform.rotation);
		}
		this.SetPersistantDisableEffect(false);
		this.m_disabled = false;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0003E298 File Offset: 0x0003C498
	protected virtual void OnDisabled()
	{
		if (this.m_disabled)
		{
			return;
		}
		this.m_disabled = true;
		if (this.GetUnit().IsDead())
		{
			return;
		}
		if (this.m_disableEffect != null && this.IsVisible())
		{
			UnityEngine.Object.Instantiate(this.m_disableEffect, base.transform.position, base.transform.rotation);
		}
		this.SetPersistantDisableEffect(true);
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0003E310 File Offset: 0x0003C510
	private void SetPersistantDisableEffect(bool enable)
	{
		if (enable)
		{
			GameObject persistantDisableEffectPrefab = this.m_persistantDisableEffectPrefab;
			if (this.m_persistantDisableEffect == null && persistantDisableEffectPrefab != null)
			{
				this.m_persistantDisableEffect = (UnityEngine.Object.Instantiate(persistantDisableEffectPrefab, base.transform.position, base.transform.rotation) as GameObject);
				this.m_persistantDisableEffect.transform.parent = base.transform;
				Renderer[] componentsInChildren = this.m_persistantDisableEffect.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in componentsInChildren)
				{
					renderer.enabled = this.IsVisible();
				}
			}
		}
		else if (this.m_persistantDisableEffect != null)
		{
			UnityEngine.Object.Destroy(this.m_persistantDisableEffect);
			this.m_persistantDisableEffect = null;
		}
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0003E3E4 File Offset: 0x0003C5E4
	public virtual Dictionary<string, string> GetShipEditorInfo()
	{
		return new Dictionary<string, string>();
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0003E3F8 File Offset: 0x0003C5F8
	public Unit GetUnit()
	{
		return this.m_unit;
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0003E400 File Offset: 0x0003C600
	public string GetName()
	{
		return Localize.instance.TranslateKey(base.name + "_name");
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0003E41C File Offset: 0x0003C61C
	public string GetAbbr()
	{
		return Localize.instance.TranslateKey(base.name + "_abbr");
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0003E438 File Offset: 0x0003C638
	public string GetProductName()
	{
		return Localize.instance.TranslateKey(base.name + "_productname");
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0003E454 File Offset: 0x0003C654
	public int GetHealth()
	{
		return this.m_health;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0003E45C File Offset: 0x0003C65C
	public int GetMaxHealth()
	{
		return this.m_maxHealth;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0003E464 File Offset: 0x0003C664
	public int GetWidth()
	{
		if (this.m_dir == Direction.Forward || this.m_dir == Direction.Backward)
		{
			return this.m_width;
		}
		return this.m_length;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0003E498 File Offset: 0x0003C698
	public int GetLength()
	{
		if (this.m_dir == Direction.Forward || this.m_dir == Direction.Backward)
		{
			return this.m_length;
		}
		return this.m_width;
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0003E4CC File Offset: 0x0003C6CC
	public Vector2i GetGridPos()
	{
		return this.m_gridPos;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0003E4D4 File Offset: 0x0003C6D4
	public int GetTotalValue()
	{
		return this.m_settings.m_value;
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0003E4E4 File Offset: 0x0003C6E4
	public bool IsDisabled()
	{
		return this.m_disabled;
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0003E4EC File Offset: 0x0003C6EC
	public virtual float GetMaxEnergy()
	{
		return 0f;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0003E4F4 File Offset: 0x0003C6F4
	public virtual float GetEnergy()
	{
		return 0f;
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0003E4FC File Offset: 0x0003C6FC
	public virtual void SetDeploy(bool deploy)
	{
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0003E500 File Offset: 0x0003C700
	public virtual bool GetDeploy()
	{
		return false;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0003E504 File Offset: 0x0003C704
	public virtual string GetStatusText()
	{
		return string.Empty;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0003E50C File Offset: 0x0003C70C
	public virtual StatusWnd_HPModule CreateStatusWindow(GameObject guiCamera)
	{
		return null;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0003E510 File Offset: 0x0003C710
	public virtual void OnOrdersChanged()
	{
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0003E514 File Offset: 0x0003C714
	public virtual void ClearOrders()
	{
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0003E534 File Offset: 0x0003C734
	public Order GetFirstOrder()
	{
		if (this.m_orders.Count > 0)
		{
			return this.m_orders.First.Value;
		}
		return null;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0003E55C File Offset: 0x0003C75C
	public virtual bool RemoveFirstOrder()
	{
		if (this.m_orders.Count > 0)
		{
			Order value = this.m_orders.First.Value;
			this.m_orders.RemoveFirst();
			value.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			return true;
		}
		return false;
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0003E5A8 File Offset: 0x0003C7A8
	public virtual bool RemoveLastOrder()
	{
		if (this.m_orders.Count > 0)
		{
			Order value = this.m_orders.Last.Value;
			this.m_orders.RemoveLast();
			value.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			return true;
		}
		return false;
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0003E5F4 File Offset: 0x0003C7F4
	protected override void OnSetDrawOrders(bool enabled)
	{
		foreach (Order order in this.m_orders)
		{
			order.SetMarkerEnabled(enabled, this.m_orderMarkerPrefab);
		}
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0003E660 File Offset: 0x0003C860
	public virtual bool RemoveOrder(Order order)
	{
		if (this.m_orders.Remove(order))
		{
			order.SetMarkerEnabled(false, this.m_orderMarkerPrefab);
			return true;
		}
		return false;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0003E684 File Offset: 0x0003C884
	public virtual bool IsLastOrder(Order order)
	{
		return this.m_orders.Count > 0 && this.m_orders.Last.Value == order;
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x0003E6B8 File Offset: 0x0003C8B8
	public virtual void AddOrder(Order order)
	{
		this.m_orders.AddLast(order);
		order.SetMarkerEnabled(NetObj.m_drawOrders, this.m_orderMarkerPrefab);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0003E6D8 File Offset: 0x0003C8D8
	public virtual void LoadOrders(BinaryReader stream)
	{
		while (this.m_orders.Count > 0)
		{
			this.RemoveLastOrder();
		}
		int num = stream.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Order order = new Order(this, stream);
			this.AddOrder(order);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0003E72C File Offset: 0x0003C92C
	public virtual void SaveOrders(BinaryWriter stream)
	{
		stream.Write(this.m_orders.Count);
		foreach (Order order in this.m_orders)
		{
			order.Save(stream);
		}
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0003E7A4 File Offset: 0x0003C9A4
	public virtual void DrawOrders()
	{
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0003E7A8 File Offset: 0x0003C9A8
	public override bool IsSeenByPlayer(int playerID)
	{
		return this.m_unit.IsSeenByPlayer(playerID);
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0003E7B8 File Offset: 0x0003C9B8
	public override bool IsSeenByTeam(int teamID)
	{
		return this.m_unit.IsSeenByTeam(teamID);
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0003E7C8 File Offset: 0x0003C9C8
	public void SetHighlight(bool enabled)
	{
		Renderer[] componentsInChildren = this.m_visual.GetComponentsInChildren<Renderer>();
		if (enabled)
		{
			if (this.m_originalMaterials != null)
			{
				return;
			}
			List<Material> list = new List<Material>();
			foreach (Renderer renderer in componentsInChildren)
			{
				foreach (Material item in renderer.sharedMaterials)
				{
					list.Add(item);
				}
				foreach (Material material in renderer.materials)
				{
					material.SetFloat("_Highlight", 0.3f);
				}
			}
			this.m_originalMaterials = list.ToArray();
		}
		else
		{
			if (this.m_originalMaterials == null)
			{
				return;
			}
			int num = 0;
			foreach (Renderer renderer2 in componentsInChildren)
			{
				Material[] array3 = new Material[renderer2.sharedMaterials.Length];
				for (int m = 0; m < renderer2.sharedMaterials.Length; m++)
				{
					array3[m] = this.m_originalMaterials[num++];
				}
				renderer2.materials = array3;
			}
			this.m_originalMaterials = null;
		}
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0003E918 File Offset: 0x0003CB18
	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		Renderer[] componentsInChildren = this.m_visual.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = visible;
		}
		if (this.m_persistantDisableEffect != null)
		{
			Renderer[] componentsInChildren2 = this.m_persistantDisableEffect.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer2 in componentsInChildren2)
			{
				renderer2.enabled = this.IsVisible();
			}
		}
		if (this.m_disableIcon != null)
		{
			this.m_disableIcon.renderer.enabled = visible;
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0003E9CC File Offset: 0x0003CBCC
	private void SetDisableIcon(bool enabled)
	{
		if (this.m_disableIconPrefab == null)
		{
			return;
		}
		if (enabled)
		{
			if (this.m_disableIcon == null)
			{
				this.m_disableIcon = (UnityEngine.Object.Instantiate(this.m_disableIconPrefab) as GameObject);
				this.m_disableIcon.transform.parent = base.transform;
				this.m_disableIcon.transform.localPosition = new Vector3(-0.5f, 2f, 0f);
				this.m_disableIcon.renderer.enabled = this.IsVisible();
			}
		}
		else if (this.m_disableIcon != null)
		{
			UnityEngine.Object.Destroy(this.m_disableIcon);
			this.m_disableIcon = null;
		}
	}

	// Token: 0x040006CA RID: 1738
	public GameObject m_visual;

	// Token: 0x040006CB RID: 1739
	public GameObject m_disableIconPrefab;

	// Token: 0x040006CC RID: 1740
	public Texture2D m_GUITexture;

	// Token: 0x040006CD RID: 1741
	public bool m_editByPlayer = true;

	// Token: 0x040006CE RID: 1742
	public HPModuleSettings m_settings;

	// Token: 0x040006CF RID: 1743
	public int m_width = 1;

	// Token: 0x040006D0 RID: 1744
	public int m_length = 1;

	// Token: 0x040006D1 RID: 1745
	public HPModule.HPModuleType m_type;

	// Token: 0x040006D2 RID: 1746
	public int m_maxHealth = 20;

	// Token: 0x040006D3 RID: 1747
	public int m_armorClass = 10;

	// Token: 0x040006D4 RID: 1748
	public int m_repairAmount = 2;

	// Token: 0x040006D5 RID: 1749
	public GameObject m_selectionMarkerPrefab;

	// Token: 0x040006D6 RID: 1750
	public GameObject m_orderMarkerPrefab;

	// Token: 0x040006D7 RID: 1751
	public GameObject m_disableEffect;

	// Token: 0x040006D8 RID: 1752
	public GameObject m_enableEffect;

	// Token: 0x040006D9 RID: 1753
	public GameObject m_persistantDisableEffectPrefab;

	// Token: 0x040006DA RID: 1754
	public GameObject m_persistantDisableEffectPrefabLow;

	// Token: 0x040006DB RID: 1755
	public GameObject m_selectSound;

	// Token: 0x040006DC RID: 1756
	protected int m_health = 20;

	// Token: 0x040006DD RID: 1757
	protected Vector2i m_gridPos;

	// Token: 0x040006DE RID: 1758
	protected Unit m_unit;

	// Token: 0x040006DF RID: 1759
	protected Battery m_battery;

	// Token: 0x040006E0 RID: 1760
	protected Direction m_dir;

	// Token: 0x040006E1 RID: 1761
	protected LinkedList<Order> m_orders = new LinkedList<Order>();

	// Token: 0x040006E2 RID: 1762
	protected HPModule.DestroyedHandler m_onDestroyed;

	// Token: 0x040006E3 RID: 1763
	protected bool m_disabled;

	// Token: 0x040006E4 RID: 1764
	private Material[] m_originalMaterials;

	// Token: 0x040006E5 RID: 1765
	private GameObject m_marker;

	// Token: 0x040006E6 RID: 1766
	private GameObject m_disableIcon;

	// Token: 0x040006E7 RID: 1767
	private GameObject m_persistantDisableEffect;

	// Token: 0x040006E8 RID: 1768
	private float m_destructionTimer = -1f;

	// Token: 0x040006E9 RID: 1769
	private float m_repairTimer;

	// Token: 0x020000DB RID: 219
	public enum HPModuleType
	{
		// Token: 0x040006EB RID: 1771
		Offensive,
		// Token: 0x040006EC RID: 1772
		Defensive,
		// Token: 0x040006ED RID: 1773
		Any
	}

	// Token: 0x020001AC RID: 428
	// (Invoke) Token: 0x06000F60 RID: 3936
	public delegate void DestroyedHandler(HPModule module);
}
