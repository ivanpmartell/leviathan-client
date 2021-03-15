using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class StatusWnd_Gun : StatusWnd_HPModule
{
	// Token: 0x06000131 RID: 305 RVA: 0x00007A08 File Offset: 0x00005C08
	public StatusWnd_Gun(Gun gun, GameObject GUICam, bool friend)
	{
		this.m_gun = gun;
		this.Initialize(GUICam, true);
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00007A28 File Offset: 0x00005C28
	private void Initialize(GameObject GUICam, bool friendOrFoe)
	{
		this.CurrentlyViewedAsFriend = friendOrFoe;
		this.m_guiCam = GUICam;
		this.m_gui = GuiUtils.CreateGui("IngameGui/StatusWnd_Gun", this.m_guiCam);
		this.ValidateComponents();
		this.LoadStatusLights(this.m_guiCam);
		this.Update();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00007A74 File Offset: 0x00005C74
	public void Update()
	{
		if (this.m_gun.GetUnit().IsDead())
		{
			this.m_gui.SetActiveRecursively(false);
			return;
		}
		this.m_gui.SetActiveRecursively(true);
		this.m_lblGunName.Text = this.m_gun.GetName();
		this.m_lblGunStatus.Text = Localize.instance.Translate(this.m_gun.GetStatusText());
		this.SetIcon(this.m_gun.m_GUITexture);
		float num = (float)this.m_gun.GetHealth() / (float)this.m_gun.GetMaxHealth();
		if (num <= 0.25f)
		{
			this.m_status_generalWarning.SetOnOff(true);
		}
		else
		{
			this.m_status_generalWarning.SetOnOff(false);
		}
		this.SetHealthPercentage(this.m_gun.GetHealth(), this.m_gun.GetMaxHealth());
		int maxAmmo = this.m_gun.GetMaxAmmo();
		if (maxAmmo < 0)
		{
			this.m_lblAmmo.Text = ((int)this.m_gun.GetLoadedSalvo()).ToString();
			this.m_status_ammo.SetIcon_Percentage(100f);
		}
		else
		{
			int num2 = this.m_gun.GetAmmo() + (int)this.m_gun.GetLoadedSalvo();
			this.m_lblAmmo.Text = num2.ToString();
			float icon_Percentage = (float)num2 / (float)this.m_gun.GetMaxAmmo() * 100f;
			this.m_status_ammo.SetIcon_Percentage(icon_Percentage);
		}
		if (this.m_currentlyViewedAsFriend)
		{
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007BF4 File Offset: 0x00005DF4
	public void Close()
	{
		UnityEngine.Object.DestroyImmediate(this.m_gui);
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00007C04 File Offset: 0x00005E04
	public void SetTint(Color color)
	{
		if (this.m_background == null)
		{
			return;
		}
		this.m_background.Color = color;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00007C24 File Offset: 0x00005E24
	public void TryChangeBackground(string textureUrl)
	{
		string o = string.Format("StatusWnd_Gun failed to load background \"{0}\"", textureUrl);
		if (string.IsNullOrEmpty(textureUrl))
		{
			PLog.LogWarning(o);
			return;
		}
		Texture2D texture2D = Resources.Load(textureUrl) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning(o);
			return;
		}
		this.Background = texture2D;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007C78 File Offset: 0x00005E78
	public void SetHealthPercentage(int health, int maxHealth)
	{
		this.m_lblHealth.Text = health.ToString();
		this.m_healthbar.Value = Mathf.Clamp((float)health / (float)maxHealth, 0f, 1f);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007CB8 File Offset: 0x00005EB8
	private void ValidateComponents()
	{
		if (this.m_isValid)
		{
			return;
		}
		this.m_lblHealth = this.m_gui.transform.Find("lblHealth").GetComponent<SpriteText>();
		this.m_background = this.m_gui.GetComponent<SimpleSprite>();
		DebugUtils.Assert(this.m_background != null, "StatusWnd_Gun has no SimpleSprite-component to be used as background !");
		DebugUtils.Assert(this.Validate_GunNameLabel(), "StatusWnd_Gun failed to validate label named \"lblGunName\"");
		DebugUtils.Assert(this.Validate_AmmoIcon(), "StatusWnd_Gun failed to validate StatusLight_Advanced named \"Ammo\"");
		DebugUtils.Assert(this.Validate_AmmoLabel(), "StatusWnd_Gun failed to validate label named \"lblAmmo\"");
		DebugUtils.Assert(this.Validate_StatusLabel(), "StatusWnd_Gun failed to validate label named \"lblGunStatus\"");
		DebugUtils.Assert(this.Validate_Icon(), "StatusWnd_Gun failed to validate SimpleSprite named \"Icon\"");
		DebugUtils.Assert(this.Validate_Health(), "StatusWnd_Gun failed to validate UIProgressBar named \"Progressbar_Health\"");
		DebugUtils.Assert(this.ValidateModifierIconList(), "StatusWnd_Gun failed to validate transform named \"ModifierIndications\"");
		this.m_isValid = true;
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00007D90 File Offset: 0x00005F90
	private bool Validate_GunNameLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblGunName", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_lblGunName = gameObject.GetComponent<SpriteText>();
		return this.m_lblGunName != null;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00007DD8 File Offset: 0x00005FD8
	private bool Validate_AmmoLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblAmmo", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_lblAmmo = gameObject.GetComponent<SpriteText>();
		return this.m_lblAmmo != null;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00007E20 File Offset: 0x00006020
	private bool Validate_StatusLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblGunStatus", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_lblGunStatus = gameObject.GetComponent<SpriteText>();
		return this.m_lblGunStatus != null;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00007E68 File Offset: 0x00006068
	private bool Validate_Icon()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("Icon", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_icon = gameObject.GetComponent<SimpleSprite>();
		return this.m_icon != null;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00007EB0 File Offset: 0x000060B0
	private bool Validate_Health()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("Progressbar_Health", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_healthbar = gameObject.GetComponent<UIProgressBar>();
		return this.m_healthbar != null;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00007EF8 File Offset: 0x000060F8
	private bool ValidateModifierIconList()
	{
		this.m_indicationLightsList = this.m_gui.transform.Find("ModifierIndications");
		return this.m_indicationLightsList != null;
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00007F24 File Offset: 0x00006124
	private bool Validate_AmmoIcon()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("Ammo", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_status_ammo = gameObject.GetComponent<StatusLight_Advanced>();
		if (this.m_status_ammo == null)
		{
			return false;
		}
		this.m_status_ammo.Initialize();
		return true;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00007F80 File Offset: 0x00006180
	private void LoadStatusLights(GameObject guiCamera)
	{
		float num = 0f;
		this.m_status_generalWarning = StatusLight_Basic.Create(guiCamera, "Warning");
		this.m_status_generalWarning.gameObject.transform.parent = this.m_gui.transform.Find("Icon");
		this.m_status_generalWarning.gameObject.transform.localPosition = new Vector3(-24f, 23f);
		this.m_status_generalWarning.DisableWhenOff = true;
		this.m_status_generalWarning.SetOnOff(false);
		this.m_status_acid = StatusLight_Basic.Create(guiCamera, "Acid");
		this.m_status_acid.DisableWhenOff = false;
		this.m_status_acid.gameObject.transform.parent = this.m_indicationLightsList;
		this.m_status_acid.gameObject.transform.localPosition = new Vector3(num, 0f, -1f);
		num += this.m_status_acid.ScaledWidth + 3f;
		this.m_status_repair = StatusLight_Basic.Create(guiCamera, "BeingRepaired");
		this.m_status_repair.DisableWhenOff = false;
		this.m_status_repair.gameObject.transform.parent = this.m_indicationLightsList;
		this.m_status_repair.gameObject.transform.localPosition = new Vector3(num, 0f, -1f);
		num += this.m_status_repair.ScaledWidth + 3f;
		this.m_status_electricity = StatusLight_Basic.Create(guiCamera, "Electricity");
		this.m_status_electricity.DisableWhenOff = false;
		this.m_status_electricity.gameObject.transform.parent = this.m_indicationLightsList;
		this.m_status_electricity.gameObject.transform.localPosition = new Vector3(num, 0f, -1f);
		num += this.m_status_electricity.ScaledWidth + 3f;
		this.m_status_fire = StatusLight_Basic.Create(guiCamera, "Fire");
		this.m_status_fire.DisableWhenOff = false;
		this.m_status_fire.gameObject.transform.parent = this.m_indicationLightsList;
		this.m_status_fire.gameObject.transform.localPosition = new Vector3(num, 0f, -1f);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x000081BC File Offset: 0x000063BC
	private bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = this.m_gui.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000142 RID: 322 RVA: 0x000081F8 File Offset: 0x000063F8
	// (set) Token: 0x06000143 RID: 323 RVA: 0x00008200 File Offset: 0x00006400
	public bool IsValid
	{
		get
		{
			return this.m_isValid;
		}
		private set
		{
			this.m_isValid = value;
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000820C File Offset: 0x0000640C
	private void SetIcon(Texture2D value)
	{
		if (!(value == null))
		{
			this.m_icon.SetTexture(value);
			this.m_icon.Setup(this.m_icon.width, this.m_icon.height, new Vector2(0f, (float)value.width), new Vector2((float)value.width, (float)value.height));
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000145 RID: 325 RVA: 0x0000827C File Offset: 0x0000647C
	// (set) Token: 0x06000146 RID: 326 RVA: 0x000082B0 File Offset: 0x000064B0
	public string GunName
	{
		get
		{
			return (!(this.m_lblGunName == null)) ? this.m_lblGunName.Text : string.Empty;
		}
		private set
		{
			if (this.m_lblGunName != null)
			{
				this.m_lblGunName.Text = value;
			}
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000147 RID: 327 RVA: 0x000082D0 File Offset: 0x000064D0
	// (set) Token: 0x06000148 RID: 328 RVA: 0x00008310 File Offset: 0x00006510
	public Texture2D Background
	{
		get
		{
			return (!(this.m_background == null)) ? (this.m_background.renderer.material.mainTexture as Texture2D) : null;
		}
		set
		{
			if (this.m_background == null)
			{
				return;
			}
			float width = this.m_background.width;
			float height = this.m_background.height;
			this.m_background.SetTexture(value);
			float num = (float)value.width;
			float num2 = (float)value.height;
			this.m_background.Setup(num, num2, new Vector2(0f, num2), new Vector2(num, num2));
			this.m_background.gameObject.transform.localScale = new Vector3(width / (float)value.width, height / (float)value.height, 1f);
			this.m_background.UpdateUVs();
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000149 RID: 329 RVA: 0x000083C0 File Offset: 0x000065C0
	// (set) Token: 0x0600014A RID: 330 RVA: 0x000083C8 File Offset: 0x000065C8
	public bool CurrentlyViewedAsFriend
	{
		get
		{
			return this.m_currentlyViewedAsFriend;
		}
		private set
		{
			this.m_currentlyViewedAsFriend = value;
		}
	}

	// Token: 0x040000A9 RID: 169
	private const float m_iconSpacingX = 3f;

	// Token: 0x040000AA RID: 170
	private bool m_currentlyViewedAsFriend = true;

	// Token: 0x040000AB RID: 171
	private bool m_isValid;

	// Token: 0x040000AC RID: 172
	private GameObject m_guiCam;

	// Token: 0x040000AD RID: 173
	private Gun m_gun;

	// Token: 0x040000AE RID: 174
	private GameObject m_gui;

	// Token: 0x040000AF RID: 175
	private SpriteText m_lblGunName;

	// Token: 0x040000B0 RID: 176
	private SpriteText m_lblAmmo;

	// Token: 0x040000B1 RID: 177
	private SimpleSprite m_background;

	// Token: 0x040000B2 RID: 178
	private UIProgressBar m_healthbar;

	// Token: 0x040000B3 RID: 179
	private SpriteText m_lblHealth;

	// Token: 0x040000B4 RID: 180
	private SimpleSprite m_icon;

	// Token: 0x040000B5 RID: 181
	private Transform m_indicationLightsList;

	// Token: 0x040000B6 RID: 182
	private StatusLight_Basic m_status_repair;

	// Token: 0x040000B7 RID: 183
	private StatusLight_Basic m_status_acid;

	// Token: 0x040000B8 RID: 184
	private StatusLight_Basic m_status_electricity;

	// Token: 0x040000B9 RID: 185
	private StatusLight_Basic m_status_fire;

	// Token: 0x040000BA RID: 186
	private StatusLight_Basic m_status_generalWarning;

	// Token: 0x040000BB RID: 187
	private StatusLight_Advanced m_status_ammo;

	// Token: 0x040000BC RID: 188
	private SpriteText m_lblGunStatus;
}
