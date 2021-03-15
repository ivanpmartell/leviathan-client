using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
public class StatusWnd_Ship
{
	// Token: 0x0600015F RID: 351 RVA: 0x000088E8 File Offset: 0x00006AE8
	public StatusWnd_Ship(Ship ship, GameObject GUICam, bool friendOrFoe)
	{
		this.m_guiCam = GUICam;
		this.m_ship = ship;
		this.SetupGui();
		this.Update();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00008914 File Offset: 0x00006B14
	public void Update()
	{
		if (this.m_ship.IsDead())
		{
			this.m_gui.transform.gameObject.SetActiveRecursively(false);
			return;
		}
		this.m_lblClassName.Text = Localize.instance.Translate(this.m_ship.GetClassName());
		this.m_lblShipName.Text = this.m_ship.GetName();
		this.m_healthBar.Value = (float)this.m_ship.GetHealth() / (float)this.m_ship.GetMaxHealth();
		this.m_healthText.Text = this.m_ship.GetHealth().ToString() + "/" + this.m_ship.GetMaxHealth().ToString();
		SupportShip supportShip = this.m_ship as SupportShip;
		if (supportShip)
		{
			this.m_supplyBar.Value = (float)supportShip.GetResources() / (float)supportShip.GetMaxResources();
			this.m_supplyText.Text = supportShip.GetResources().ToString() + "/" + supportShip.GetMaxResources().ToString();
		}
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00008A40 File Offset: 0x00006C40
	public void Close()
	{
		UnityEngine.Object.DestroyImmediate(this.m_gui);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00008A50 File Offset: 0x00006C50
	private void SetupGui()
	{
		if (this.m_ship is SupportShip)
		{
			this.m_gui = GuiUtils.CreateGui("IngameGui/StatusWnd_Ship_Supply", this.m_guiCam);
			this.m_supplyWnd = GuiUtils.FindChildOf(this.m_gui, "Supplybar");
			this.m_supplyBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Supply").GetComponent<UIProgressBar>();
			this.m_supplyText = GuiUtils.FindChildOf(this.m_gui, "lblCurrentSupply").GetComponent<SpriteText>();
		}
		else
		{
			this.m_gui = GuiUtils.CreateGui("IngameGui/StatusWnd_Ship", this.m_guiCam);
		}
		this.m_background = this.m_gui.transform.GetComponent<SimpleSprite>();
		DebugUtils.Assert(this.m_background != null, "StatusWnd_Ship has no SimpleSprite-component to be used as background !");
		this.m_lblShipName = GuiUtils.FindChildOf(this.m_gui, "lblShipName").GetComponent<SpriteText>();
		this.m_healthBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Health").GetComponent<UIProgressBar>();
		this.m_lblClassName = GuiUtils.FindChildOf(this.m_gui, "lblClassName").GetComponent<SpriteText>();
		this.m_healthText = GuiUtils.FindChildOf(this.m_gui, "lblCurrentHealth").GetComponent<SpriteText>();
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000163 RID: 355 RVA: 0x00008B84 File Offset: 0x00006D84
	// (set) Token: 0x06000164 RID: 356 RVA: 0x00008B8C File Offset: 0x00006D8C
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

	// Token: 0x040000CA RID: 202
	private const float m_iconSpacingX = 3f;

	// Token: 0x040000CB RID: 203
	private bool m_currentlyViewedAsFriend = true;

	// Token: 0x040000CC RID: 204
	private GameObject m_guiCam;

	// Token: 0x040000CD RID: 205
	private Ship m_ship;

	// Token: 0x040000CE RID: 206
	private GameObject m_gui;

	// Token: 0x040000CF RID: 207
	private SpriteText m_lblShipName;

	// Token: 0x040000D0 RID: 208
	private SpriteText m_lblClassName;

	// Token: 0x040000D1 RID: 209
	private SimpleSprite m_background;

	// Token: 0x040000D2 RID: 210
	private UIProgressBar m_healthBar;

	// Token: 0x040000D3 RID: 211
	private SpriteText m_healthText;

	// Token: 0x040000D4 RID: 212
	private GameObject m_supplyWnd;

	// Token: 0x040000D5 RID: 213
	private UIProgressBar m_supplyBar;

	// Token: 0x040000D6 RID: 214
	private SpriteText m_supplyText;
}
