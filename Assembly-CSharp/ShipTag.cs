using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000063 RID: 99
internal class ShipTag
{
	// Token: 0x06000470 RID: 1136 RVA: 0x000262D0 File Offset: 0x000244D0
	public ShipTag(Unit ship, GameObject guiCamera, GameType gameType, List<GameObject> iconPrefabs)
	{
		this.m_unit = ship;
		this.m_guiCamera = guiCamera.camera;
		this.m_gameType = gameType;
		this.m_iconPrefabs = iconPrefabs;
		if (this.m_unit is Ship && this.m_unit.m_shipTag == Unit.ShipTagType.Normal)
		{
			this.m_cullDistance = 800f;
			if (this.m_unit is SupportShip)
			{
				this.m_gui = GuiUtils.CreateGui("ShipTag_Supply", guiCamera);
				this.m_supplyBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Supply").GetComponent<UIProgressBar>();
				this.m_supplyText = GuiUtils.FindChildOf(this.m_gui, "lblCurrentSupply").GetComponent<SpriteText>();
			}
			else
			{
				this.m_gui = GuiUtils.CreateGui("ShipTag", guiCamera);
			}
			this.m_flag = GuiUtils.FindChildOf(this.m_gui, "Flag").GetComponent<SimpleSprite>();
			this.m_sinkIcon = GuiUtils.FindChildOf(this.m_gui, "SinkIcon").GetComponent<SimpleSprite>();
			this.m_supplyIcon = GuiUtils.FindChildOf(this.m_gui, "SupplyIcon").GetComponent<SimpleSprite>();
		}
		else
		{
			this.m_cullDistance = 800f;
			Platform platform = this.m_unit as Platform;
			if (platform && platform.m_supplyEnabled)
			{
				this.m_gui = GuiUtils.CreateGui("ShipTag_Supply", guiCamera);
				this.m_supplyBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Supply").GetComponent<UIProgressBar>();
				this.m_supplyText = GuiUtils.FindChildOf(this.m_gui, "lblCurrentSupply").GetComponent<SpriteText>();
				this.m_flag = GuiUtils.FindChildOf(this.m_gui, "Flag").GetComponent<SimpleSprite>();
				this.m_sinkIcon = GuiUtils.FindChildOf(this.m_gui, "SinkIcon").GetComponent<SimpleSprite>();
				this.m_supplyIcon = GuiUtils.FindChildOf(this.m_gui, "SupplyIcon").GetComponent<SimpleSprite>();
			}
			else
			{
				this.m_gui = GuiUtils.CreateGui("ShipTag_mini", guiCamera);
			}
		}
		this.m_shipName = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "ShipNameLabel");
		this.m_healthBar = GuiUtils.FindChildOf(this.m_gui, "Progressbar_Health").GetComponent<UIProgressBar>();
		this.m_iconList = GuiUtils.FindChildOfComponent<UIScrollList>(this.m_gui, "StatusScrollist");
		this.m_healthText = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "lblCurrentHealth");
		for (int i = 0; i < 7; i++)
		{
			this.m_statusIcons.Add(null);
		}
		this.m_gui.SetActiveRecursively(false);
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0002657C File Offset: 0x0002477C
	public void Close()
	{
		UnityEngine.Object.Destroy(this.m_gui);
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0002658C File Offset: 0x0002478C
	public void SetVisible(bool visible)
	{
		if (this.m_gui.active)
		{
			this.m_gui.SetActiveRecursively(false);
		}
		this.m_visible = visible;
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000265B4 File Offset: 0x000247B4
	public void Update(Camera camera, int localPlayerID, int localTeamID)
	{
		if (this.m_visible && this.m_unit.IsVisible() && !this.m_unit.IsDead() && camera.transform.position.y < this.m_cullDistance)
		{
			if (!this.m_gui.active)
			{
				this.m_gui.SetActiveRecursively(true);
			}
			this.UpdateTagPosition(camera);
			this.m_healthBar.Value = (float)this.m_unit.GetHealth() / (float)this.m_unit.GetMaxHealth();
			if (this.m_healthText != null)
			{
				this.m_healthText.Text = this.m_unit.GetHealth().ToString();
			}
			if (this.m_shipName != null)
			{
				this.m_shipName.Text = this.m_unit.GetName();
			}
			this.UpdateSupplyBar();
			this.UpdateTeamColor(localPlayerID, localTeamID);
			this.UpdateFlag();
			this.UpdateIcons();
		}
		else if (this.m_gui.active)
		{
			this.m_gui.SetActiveRecursively(false);
		}
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x000266E4 File Offset: 0x000248E4
	private void UpdateIcons()
	{
		if (this.m_iconList == null)
		{
			return;
		}
		Ship ship = this.m_unit as Ship;
		if (ship == null)
		{
			return;
		}
		if (ship.IsTakingWater())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.Sink).m_statusText.Text = ship.GetTimeToSink().ToString("F0");
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.Sink);
		}
		if (ship.IsSupplied() || ship.IsAutoRepairing())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.Repair);
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.Repair);
		}
		if (ship.IsEngineDamaged())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.MoveDisabled).m_statusText.Text = ship.GetEngineRepairTime().ToString("F0");
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.MoveDisabled);
		}
		if (ship.IsBridgeDamaged())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.ViewDisabled).m_statusText.Text = ship.GetBridgeRepairTime().ToString("F0");
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.ViewDisabled);
		}
		if (ship.IsOutOfControl())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.OOC).m_statusText.Text = ship.GetControlRepairTime().ToString("F1");
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.OOC);
		}
		if (ship.IsGrounded())
		{
			this.EnableStatusIcon(ShipTag.StatusIconType.Grounded).m_statusText.Text = ship.GetGroundedTime().ToString("F1");
		}
		else
		{
			this.DisableStatusIcon(ShipTag.StatusIconType.Grounded);
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00026870 File Offset: 0x00024A70
	private ShipTag.StatusIcon EnableStatusIcon(ShipTag.StatusIconType type)
	{
		ShipTag.StatusIcon statusIcon = this.m_statusIcons[(int)type];
		if (statusIcon != null)
		{
			return statusIcon;
		}
		statusIcon = new ShipTag.StatusIcon();
		this.m_statusIcons[(int)type] = statusIcon;
		statusIcon.m_root = (UnityEngine.Object.Instantiate(this.m_iconPrefabs[(int)type]) as GameObject);
		statusIcon.m_statusText = GuiUtils.FindChildOfComponent<SpriteText>(statusIcon.m_root, "StatusLabel");
		this.m_iconList.AddItem(statusIcon.m_root);
		return statusIcon;
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x000268EC File Offset: 0x00024AEC
	private void DisableStatusIcon(ShipTag.StatusIconType type)
	{
		ShipTag.StatusIcon statusIcon = this.m_statusIcons[(int)type];
		if (statusIcon == null)
		{
			return;
		}
		int index = statusIcon.m_root.GetComponent<UIListItemContainer>().Index;
		this.m_iconList.RemoveItem(index, true);
		this.m_statusIcons[(int)type] = null;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x00026938 File Offset: 0x00024B38
	private void UpdateTagPosition(Camera camera)
	{
		GameCamera component = camera.GetComponent<GameCamera>();
		Vector3 vector = this.m_unit.transform.position;
		if (!this.m_unit.m_centerShipTag)
		{
			float num = -Math.Abs((this.m_unit.transform.forward * this.m_unit.GetLength()).z * 0.5f) - this.m_unit.GetWidth() * 0.5f;
			vector.z += num;
		}
		vector = GuiUtils.WorldToGuiPos(camera, this.m_guiCamera, vector);
		vector.z = 8f;
		FlowerMenu flowerMenu = component.GetFlowerMenu();
		if (flowerMenu != null && flowerMenu.GetShip() == this.m_unit)
		{
			float lowestScreenPos = component.GetFlowerMenu().GetLowestScreenPos();
			if (vector.y > lowestScreenPos)
			{
				vector.y = lowestScreenPos;
			}
		}
		this.m_gui.transform.position = vector;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00026A38 File Offset: 0x00024C38
	private void UpdateSupplyBar()
	{
		SupportShip supportShip = this.m_unit as SupportShip;
		if (supportShip != null)
		{
			this.m_supplyBar.Value = (float)supportShip.GetResources() / (float)supportShip.GetMaxResources();
			this.m_supplyText.Text = supportShip.GetResources().ToString() + "/" + supportShip.GetMaxResources().ToString();
		}
		Platform platform = this.m_unit as Platform;
		if (platform && platform.m_supplyEnabled)
		{
			this.m_supplyBar.Value = (float)platform.GetResources() / (float)platform.GetMaxResources();
			this.m_supplyText.Text = platform.GetResources().ToString() + "/" + platform.GetMaxResources().ToString();
			this.m_sinkIcon.renderer.enabled = false;
			this.m_supplyIcon.renderer.enabled = false;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00026B3C File Offset: 0x00024D3C
	private void UpdateTeamColor(int localPlayerID, int localTeamID)
	{
		if (this.m_gameType == GameType.Campaign || this.m_gameType == GameType.Challenge)
		{
			if (this.m_unit.GetOwner() == localPlayerID)
			{
				this.m_healthBar.SetColor(Color.green);
			}
			else if (this.m_unit.GetOwnerTeam() == localTeamID)
			{
				this.m_healthBar.SetColor(Color.yellow);
			}
			else
			{
				this.m_healthBar.SetColor(Color.red);
			}
		}
		else
		{
			Color color;
			TurnMan.instance.GetPlayerColors(this.m_unit.GetOwner(), out color);
			this.m_healthBar.Color = color;
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00026BE8 File Offset: 0x00024DE8
	private void UpdateFlag()
	{
		if (this.m_unit.GetOwner() != this.m_owner && this.m_flag != null)
		{
			this.m_owner = this.m_unit.GetOwner();
			int playerFlag = TurnMan.instance.GetPlayerFlag(this.m_owner);
			Texture2D flagTexture = GuiUtils.GetFlagTexture(playerFlag);
			if (flagTexture != null)
			{
				GuiUtils.SetImage(this.m_flag, flagTexture);
				this.m_flag.renderer.enabled = true;
			}
			else
			{
				this.m_flag.renderer.enabled = false;
			}
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x00026C84 File Offset: 0x00024E84
	public Unit GetUnit()
	{
		return this.m_unit;
	}

	// Token: 0x040003B9 RID: 953
	private GameObject m_gui;

	// Token: 0x040003BA RID: 954
	private UIProgressBar m_healthBar;

	// Token: 0x040003BB RID: 955
	private UIProgressBar m_supplyBar;

	// Token: 0x040003BC RID: 956
	private SpriteText m_healthText;

	// Token: 0x040003BD RID: 957
	private SpriteText m_supplyText;

	// Token: 0x040003BE RID: 958
	private SpriteText m_shipName;

	// Token: 0x040003BF RID: 959
	private SimpleSprite m_flag;

	// Token: 0x040003C0 RID: 960
	private SimpleSprite m_sinkIcon;

	// Token: 0x040003C1 RID: 961
	private SimpleSprite m_supplyIcon;

	// Token: 0x040003C2 RID: 962
	private UIScrollList m_iconList;

	// Token: 0x040003C3 RID: 963
	private Camera m_guiCamera;

	// Token: 0x040003C4 RID: 964
	private Unit m_unit;

	// Token: 0x040003C5 RID: 965
	private int m_owner = -1;

	// Token: 0x040003C6 RID: 966
	private bool m_visible = true;

	// Token: 0x040003C7 RID: 967
	private GameType m_gameType;

	// Token: 0x040003C8 RID: 968
	private float m_cullDistance = 500f;

	// Token: 0x040003C9 RID: 969
	private List<GameObject> m_iconPrefabs;

	// Token: 0x040003CA RID: 970
	private List<ShipTag.StatusIcon> m_statusIcons = new List<ShipTag.StatusIcon>();

	// Token: 0x02000064 RID: 100
	public enum StatusIconType
	{
		// Token: 0x040003CC RID: 972
		HPDisabled,
		// Token: 0x040003CD RID: 973
		MoveDisabled,
		// Token: 0x040003CE RID: 974
		OOC,
		// Token: 0x040003CF RID: 975
		Repair,
		// Token: 0x040003D0 RID: 976
		Sink,
		// Token: 0x040003D1 RID: 977
		ViewDisabled,
		// Token: 0x040003D2 RID: 978
		Grounded,
		// Token: 0x040003D3 RID: 979
		MAX_STATUSICONS
	}

	// Token: 0x02000065 RID: 101
	private class StatusIcon
	{
		// Token: 0x040003D4 RID: 980
		public GameObject m_root;

		// Token: 0x040003D5 RID: 981
		public SpriteText m_statusText;
	}
}
