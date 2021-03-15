using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class StatusWnd_Shield : StatusWnd_HPModule
{
	// Token: 0x0600014D RID: 333 RVA: 0x000083D4 File Offset: 0x000065D4
	public StatusWnd_Shield(HPModule shield, GameObject GUICam, bool friendly)
	{
		this.m_guiCam = GUICam;
		this.m_shield = shield;
		this.m_gui = GuiUtils.CreateGui("IngameGui/StatusWnd_Shield", this.m_guiCam);
		this.CurrentlyViewedAsFriend = friendly;
		this.ValidateComponents();
		this.Update();
	}

	// Token: 0x0600014E RID: 334 RVA: 0x00008428 File Offset: 0x00006628
	public void Update()
	{
		if (this.m_shield.GetUnit().IsDead())
		{
			this.m_gui.SetActiveRecursively(false);
			return;
		}
		this.m_lblName.Text = this.m_shield.GetName();
		this.m_lblStatus.Text = Localize.instance.Translate(this.m_shield.GetStatusText());
		this.SetHealthPercentage(this.m_shield.GetHealth(), this.m_shield.GetMaxHealth());
		this.SetEnergy(this.m_shield.GetEnergy(), this.m_shield.GetMaxEnergy());
	}

	// Token: 0x0600014F RID: 335 RVA: 0x000084C8 File Offset: 0x000066C8
	private void SetHealthPercentage(int health, int maxHealth)
	{
		this.m_lblHealth.Text = health.ToString();
		this.m_healthbar.Value = Mathf.Clamp((float)health / (float)maxHealth, 0f, 1f);
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00008508 File Offset: 0x00006708
	public void Close()
	{
		UnityEngine.Object.DestroyImmediate(this.m_gui);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x00008518 File Offset: 0x00006718
	public void SetTint(Color color)
	{
		if (this.m_background == null)
		{
			return;
		}
		this.m_background.Color = color;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00008538 File Offset: 0x00006738
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

	// Token: 0x06000153 RID: 339 RVA: 0x0000858C File Offset: 0x0000678C
	public void SetEnergy(float energy, float maxEnergy)
	{
		this.m_lblEnergy.Text = energy.ToString("F0") + " / " + maxEnergy.ToString("F0");
		this.m_energyBar.Value = Mathf.Clamp(energy / maxEnergy, 0f, 1f);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x000085E4 File Offset: 0x000067E4
	private void ValidateComponents()
	{
		if (this.m_isValid)
		{
			return;
		}
		this.m_lblHealth = this.m_gui.transform.Find("lblHealth").GetComponent<SpriteText>();
		this.m_background = this.m_gui.GetComponent<SimpleSprite>();
		this.m_lblEnergy = GuiUtils.FindChildOf(this.m_gui, "lblEnergy").GetComponent<SpriteText>();
		this.m_energyBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Energy").GetComponent<UIProgressBar>();
		this.m_healthbar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Health").GetComponent<UIProgressBar>();
		DebugUtils.Assert(this.m_background != null, "StatusWnd_Gun has no SimpleSprite-component to be used as background !");
		DebugUtils.Assert(this.Validate_NameLabel(), "StatusWnd_Gun failed to validate label named \"lblGunName\"");
		DebugUtils.Assert(this.Validate_StatusLabel(), "StatusWnd_Gun failed to validate label named \"lblGunStatus\"");
		this.m_isValid = true;
	}

	// Token: 0x06000155 RID: 341 RVA: 0x000086BC File Offset: 0x000068BC
	private bool Validate_NameLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblName", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_lblName = gameObject.GetComponent<SpriteText>();
		return this.m_lblName != null;
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00008704 File Offset: 0x00006904
	private bool Validate_StatusLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblStatus", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_lblStatus = gameObject.GetComponent<SpriteText>();
		return this.m_lblStatus != null;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000874C File Offset: 0x0000694C
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

	// Token: 0x06000158 RID: 344 RVA: 0x00008794 File Offset: 0x00006994
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

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000159 RID: 345 RVA: 0x000087D0 File Offset: 0x000069D0
	// (set) Token: 0x0600015A RID: 346 RVA: 0x000087D8 File Offset: 0x000069D8
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

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x0600015B RID: 347 RVA: 0x000087E4 File Offset: 0x000069E4
	// (set) Token: 0x0600015C RID: 348 RVA: 0x00008824 File Offset: 0x00006A24
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

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600015D RID: 349 RVA: 0x000088D4 File Offset: 0x00006AD4
	// (set) Token: 0x0600015E RID: 350 RVA: 0x000088DC File Offset: 0x00006ADC
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

	// Token: 0x040000BD RID: 189
	private const float m_iconSpacingX = 3f;

	// Token: 0x040000BE RID: 190
	private bool m_currentlyViewedAsFriend = true;

	// Token: 0x040000BF RID: 191
	private bool m_isValid;

	// Token: 0x040000C0 RID: 192
	private GameObject m_guiCam;

	// Token: 0x040000C1 RID: 193
	private HPModule m_shield;

	// Token: 0x040000C2 RID: 194
	private GameObject m_gui;

	// Token: 0x040000C3 RID: 195
	private SpriteText m_lblName;

	// Token: 0x040000C4 RID: 196
	private SimpleSprite m_background;

	// Token: 0x040000C5 RID: 197
	private UIProgressBar m_healthbar;

	// Token: 0x040000C6 RID: 198
	private SpriteText m_lblHealth;

	// Token: 0x040000C7 RID: 199
	private UIProgressBar m_energyBar;

	// Token: 0x040000C8 RID: 200
	private SpriteText m_lblEnergy;

	// Token: 0x040000C9 RID: 201
	private SpriteText m_lblStatus;
}
