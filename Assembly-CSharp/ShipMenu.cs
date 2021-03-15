using System;
using System.Collections.Generic;
using System.Xml;
using PTech;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class ShipMenu
{
	// Token: 0x06000418 RID: 1048 RVA: 0x00021C1C File Offset: 0x0001FE1C
	public ShipMenu(GameObject gui, GameObject guiCamera, UserManClient userManClient, FleetShip fleetship, int campaignID, int fleetPoints, FleetMenu fleetmenu, FleetSize fleetSize)
	{
		this.m_guiCamera = guiCamera;
		this.m_userManClient = userManClient;
		this.m_campaignID = campaignID;
		this.m_gui = gui;
		this.m_fleetMenu = fleetmenu;
		this.m_editShip = fleetship;
		this.m_fleetSize = fleetSize;
		this.RegisterDelegatesToComponents();
		byte[] data = this.m_editShip.m_definition.ToArray();
		this.m_portShipDef = new ShipDef();
		this.m_portShipDef.FromArray(data);
		this.m_shipSeries = this.m_portShipDef.m_prefab;
		int shipValue = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		this.m_fleetBaseCost = fleetPoints - shipValue;
		this.RecalcCost();
		this.FillPartsList();
		fleetmenu.CleanUp();
		this.m_shipModified = false;
		this.OnLevelWasLoaded();
		this.m_panelFleetTop.Dismiss();
		this.m_panelShipBrowser.Dismiss();
		this.m_panelShipName.BringIn();
		this.m_panelShipInfo.BringIn();
		this.m_panelShipEquipment.BringIn();
		this.m_allowZoomTime = Time.time + 1f;
		this.ResetShipMenuGui(true);
		this.m_infoArmament.SetActiveRecursively(false);
		this.m_infoBody.SetActiveRecursively(false);
		this.m_infoBodyTop.SetActiveRecursively(false);
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00021DD8 File Offset: 0x0001FFD8
	~ShipMenu()
	{
		PLog.Log("ShipMenu DESTROYED");
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00021E18 File Offset: 0x00020018
	private GameObject ShowInfoPanel(GameObject go)
	{
		this.m_infoArmament.SetActiveRecursively(false);
		this.m_infoBody.SetActiveRecursively(false);
		this.m_infoBodyTop.SetActiveRecursively(false);
		go.SetActiveRecursively(true);
		return go;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00021E54 File Offset: 0x00020054
	private void ResetShipMenuGui(bool body)
	{
		if (body)
		{
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_CostLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_DescriptionLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_FlavorLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_OfficialNameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = string.Empty;
		}
		else
		{
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_OfficialNameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_SizeLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_CostLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_DescriptionLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_FlavorLabel").GetComponent<SpriteText>().Text = string.Empty;
			SimpleSprite component = GuiUtils.FindChildOf(this.m_infoArmament.transform, "ShipInfoPanel_Image").GetComponent<SimpleSprite>();
			component.gameObject.SetActiveRecursively(false);
			for (int i = 1; i <= 8; i++)
			{
				string name = "ShipInfoPanel_Stats" + i.ToString() + "Label";
				string name2 = "ShipInfoPanel_Stats" + i.ToString() + "Value";
				GuiUtils.FindChildOf(this.m_infoArmament.transform, name).GetComponent<SpriteText>().Text = string.Empty;
				GuiUtils.FindChildOf(this.m_infoArmament.transform, name2).GetComponent<SpriteText>().Text = string.Empty;
			}
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000220C4 File Offset: 0x000202C4
	public void SpawnMarkers(string type, List<Vector3> points, Quaternion rotation)
	{
		Texture2D texture = null;
		Vector3 b = new Vector3(0f, 0f, 0f);
		if (type == "used")
		{
			texture = (Resources.Load("shipeditor/hardpoint_red_diffuse") as Texture2D);
			b = new Vector3(0f, 0f, 0f);
		}
		else if (type == "all")
		{
			texture = (Resources.Load("shipeditor/hardpoint_green_diffuse") as Texture2D);
			b = new Vector3(0f, 0.01f, 0f);
		}
		else if (type == "offensive")
		{
			texture = (Resources.Load("shipeditor/hardpoint_yellow_diffuse") as Texture2D);
			b = new Vector3(0f, 0.01f, 0f);
		}
		else if (type == "defensive")
		{
			texture = (Resources.Load("shipeditor/hardpoint_blue_diffuse") as Texture2D);
			b = new Vector3(0f, 0.01f, 0f);
		}
		foreach (Vector3 a in points)
		{
			GameObject gameObject = ObjectFactory.instance.Create("HpMarkerShow", a + b, rotation);
			gameObject.transform.parent = this.markerBase.transform;
			gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
		}
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00022264 File Offset: 0x00020464
	public void SpawnBatteryMarkers()
	{
		for (int i = 0; i < this.markerBase.transform.GetChildCount(); i++)
		{
			UnityEngine.Object.Destroy(this.markerBase.transform.GetChild(i).gameObject);
		}
		Battery[] componentsInChildren = this.m_editShip.m_ship.GetComponentsInChildren<Battery>();
		Quaternion rotation = default(Quaternion);
		foreach (Battery battery in componentsInChildren)
		{
			List<Vector3> points;
			battery.GetUnusedTiles(out points);
			rotation = battery.transform.rotation;
			this.SpawnMarkers("used", points, rotation);
		}
		foreach (Battery battery2 in componentsInChildren)
		{
			List<Vector3> points2;
			battery2.GetFitTiles(out points2, this.m_dragWidth, this.m_dragHeight, true);
			rotation = battery2.transform.rotation;
			string type = "all";
			if (battery2.m_allowDefensive && !battery2.m_allowOffensive)
			{
				type = "defensive";
			}
			else if (!battery2.m_allowDefensive && battery2.m_allowOffensive)
			{
				type = "offensive";
			}
			if (this.m_dragType == HPModule.HPModuleType.Defensive && !battery2.m_allowDefensive)
			{
				type = "used";
			}
			if (this.m_dragType == HPModule.HPModuleType.Offensive && !battery2.m_allowOffensive)
			{
				type = "used";
			}
			this.SpawnMarkers(type, points2, rotation);
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x000223E4 File Offset: 0x000205E4
	private void ResetBatteryMarkers()
	{
		this.m_dragWidth = 1;
		this.m_dragHeight = 1;
		this.m_dragType = HPModule.HPModuleType.Any;
		this.SpawnBatteryMarkers();
		GuiUtils.FindChildOf(this.m_gui.transform, "MaxArmsPanel").GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0002242C File Offset: 0x0002062C
	private void Zoom(float zoomDelta, float dt)
	{
		if (Time.time < this.m_allowZoomTime)
		{
			return;
		}
		if (zoomDelta == 0f)
		{
			return;
		}
		if (zoomDelta > 0f)
		{
			this.m_camsize -= 75f * zoomDelta * this.m_camsize * 0.0002f;
			if (this.m_camsize < this.m_zoomMin)
			{
				this.m_camsize = this.m_zoomMin;
			}
		}
		else if (zoomDelta < 0f)
		{
			this.m_camsize += 75f * -zoomDelta * this.m_camsize * 0.0002f;
			if (this.m_camsize > this.m_zoomMax)
			{
				this.m_camsize = this.m_zoomMax;
			}
		}
		Camera component = this.m_sceneCamera.GetComponent<Camera>();
		component.orthographicSize = this.m_camsize;
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00022508 File Offset: 0x00020708
	public void CreateInformationList(GameObject panel, Dictionary<string, string> dict)
	{
		foreach (KeyValuePair<string, string> keyValuePair in dict)
		{
			this.SetLine(this.GenerateLine(panel), keyValuePair.Key, keyValuePair.Value);
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00022580 File Offset: 0x00020780
	public GameObject GenerateLine(GameObject root)
	{
		GameObject original = Resources.Load("gui/ShipEditorInfoLine") as GameObject;
		Vector3 b = new Vector3(0f, 0f, -4f);
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = root.transform;
		gameObject.transform.localPosition = this.m_listPost + b;
		this.m_listPost += new Vector3(0f, -20f, 0f);
		return gameObject;
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x00022610 File Offset: 0x00020810
	public void SetLine(GameObject line, string text, string value)
	{
		GuiUtils.FindChildOf(line, "Text").GetComponent<SpriteText>().Text = Localize.instance.Translate(text);
		GuiUtils.FindChildOf(line, "Value").GetComponent<SpriteText>().Text = Localize.instance.Translate(value);
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00022660 File Offset: 0x00020860
	public void RemoveChildren(GameObject obj)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			Transform child = obj.transform.GetChild(i);
			UnityEngine.Object.Destroy(child.gameObject);
		}
		this.m_listPost = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x000226BC File Offset: 0x000208BC
	private void SetInformationName(string prefabName)
	{
		GameObject prefab = ObjectFactory.instance.GetPrefab(prefabName);
		if (prefab)
		{
			this.m_part = prefabName;
			this.ShowInfoPanel(this.m_infoBody);
			this.SetInformation(prefab);
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x000226FC File Offset: 0x000208FC
	private string ColorCodedString(float value)
	{
		if (value == 0f)
		{
			return value.ToString();
		}
		if (value > 0f)
		{
			return Constants.m_buffColor + value.ToString();
		}
		return Constants.m_nerfColor + value.ToString();
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x0002274C File Offset: 0x0002094C
	public void SetInformation(GameObject go)
	{
		if (go == null)
		{
			this.ShowInfoPanel(this.m_infoArmament);
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_OfficialNameLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_SizeLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_CostLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_DescriptionLabel").GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_FlavorLabel").GetComponent<SpriteText>().Text = string.Empty;
			for (int i = 1; i <= 8; i++)
			{
				string name = "ShipInfoPanel_Stats" + i.ToString() + "Label";
				string name2 = "ShipInfoPanel_Stats" + i.ToString() + "Value";
				GuiUtils.FindChildOf(this.m_panelShipInfo.transform, name).GetComponent<SpriteText>().Text = string.Empty;
				GuiUtils.FindChildOf(this.m_panelShipInfo.transform, name2).GetComponent<SpriteText>().Text = string.Empty;
			}
			SimpleSprite component = GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_Image").GetComponent<SimpleSprite>();
			component.gameObject.SetActiveRecursively(false);
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_Stats_bg").SetActiveRecursively(false);
			return;
		}
		HPModule component2 = go.GetComponent<HPModule>();
		if (component2)
		{
			this.ShowInfoPanel(this.m_infoArmament);
			HPModuleSettings module = ComponentDB.instance.GetModule(component2.name);
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_name");
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_OfficialNameLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_productname");
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_SizeLabel").GetComponent<SpriteText>().Text = component2.GetWidth().ToString() + "x" + component2.GetLength().ToString();
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_CostLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(module.m_value.ToString() + " $label_pointssmall");
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_DescriptionLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_details");
			GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_FlavorLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_flavor");
			SimpleSprite component3 = GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "ShipInfoPanel_Image").GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component3, "EquipmentImages/EquipmentImg_" + this.m_part);
			component3.gameObject.SetActiveRecursively(true);
			Dictionary<string, string> shipEditorInfo = go.GetComponent<HPModule>().GetShipEditorInfo();
			int num = 1;
			foreach (KeyValuePair<string, string> keyValuePair in shipEditorInfo)
			{
				string name3 = "ShipInfoPanel_Stats" + num.ToString() + "Label";
				string name4 = "ShipInfoPanel_Stats" + num.ToString() + "Value";
				GuiUtils.FindChildOf(this.m_panelShipInfo.transform, name3).GetComponent<SpriteText>().Text = Localize.instance.Translate(keyValuePair.Key);
				GuiUtils.FindChildOf(this.m_panelShipInfo.transform, name4).GetComponent<SpriteText>().Text = Localize.instance.Translate(keyValuePair.Value);
				num++;
			}
		}
		Section component4 = go.GetComponent<Section>();
		if (component4)
		{
			GameObject gameObject;
			if (component4.GetSectionType() == Section.SectionType.Top)
			{
				gameObject = this.ShowInfoPanel(this.m_infoBodyTop);
			}
			else
			{
				gameObject = this.ShowInfoPanel(this.m_infoBody);
			}
			SectionSettings section = ComponentDB.instance.GetSection(component4.name);
			GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_NameLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_name");
			GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_OfficialNameLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_productname");
			GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_CostLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(section.m_value.ToString() + " $label_pointssmall");
			GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_DescriptionLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_details");
			GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_FlavorLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + this.m_part + "_flavor");
			if (component4.GetSectionType() == Section.SectionType.Top)
			{
				GuiUtils.FindChildOf(gameObject.transform, "ShipInfoPanel_SightStatValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_sightRange);
				this.SetDotsAllOn(this.m_infoBody, "SightDot");
				this.SetDots(this.m_infoBody, "SightDot", component4.m_rating.m_sight);
			}
			else
			{
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_ArmorStatValue").GetComponent<SpriteText>().Text = component4.m_armorClass.ToString();
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_HealthStatValue").GetComponent<SpriteText>().Text = this.ColorCodedString((float)component4.m_maxHealth);
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_ForwardMaxSpeedValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_speed);
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_ForwardAccValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_acceleration);
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_ReverseMaxSpeedValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_reverseSpeed);
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_ReverseAccValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_reverseAcceleration);
				GuiUtils.FindChildOf(this.m_infoBody.transform, "ShipInfoPanel_TurnSpeedValue").GetComponent<SpriteText>().Text = this.ColorCodedString(component4.m_modifiers.m_turnSpeed);
				this.SetDotsAllOn(this.m_infoBody, "HealthDot");
				this.SetDotsAllOn(this.m_infoBody, "ArmorDot");
				this.SetDotsAllOn(this.m_infoBody, "SpeedDot");
				this.SetDots(this.m_infoBody, "HealthDot", component4.m_rating.m_health);
				this.SetDots(this.m_infoBody, "ArmorDot", component4.m_rating.m_armor);
				this.SetDots(this.m_infoBody, "SpeedDot", component4.m_rating.m_speed);
			}
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00022FB4 File Offset: 0x000211B4
	private bool IsDeveloper()
	{
		return Application.isEditor;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00022FBC File Offset: 0x000211BC
	public void SetDragObject(string partName)
	{
		if (this.m_dragObject != null)
		{
			UnityEngine.Object.Destroy(this.m_dragObject);
			if (this.m_dragObjectIcon != null)
			{
				UnityEngine.Object.Destroy(this.m_dragObjectIcon);
			}
		}
		this.m_dragObject = null;
		if (partName.Length != 0)
		{
			Quaternion rot = default(Quaternion);
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 50f;
			this.m_dragObject = ObjectFactory.instance.Create(partName, mousePosition, rot);
			if (this.m_dragObject == null)
			{
				return;
			}
			this.m_dragObject.GetComponent<BoxCollider>().enabled = false;
			this.PrepareDraging(Input.mousePosition, Input.mousePosition, this.m_dragObject);
			this.StartDraging(Input.mousePosition);
			HPModule component = this.m_dragObject.GetComponent<HPModule>();
			if (this.m_currentDir == Direction.Forward || this.m_currentDir == Direction.Backward)
			{
				this.m_dragWidth = component.GetWidth();
				this.m_dragHeight = component.GetLength();
			}
			else
			{
				this.m_dragWidth = component.GetLength();
				this.m_dragHeight = component.GetWidth();
			}
			this.m_dragType = component.m_type;
			this.SetInformation(this.m_dragObject);
			if (component.m_type == HPModule.HPModuleType.Defensive)
			{
				this.m_dragObjectIcon = GuiUtils.CreateGui("Shipyard/ArmsDefensivePickedUp", this.m_guiCamera);
			}
			else
			{
				this.m_dragObjectIcon = GuiUtils.CreateGui("Shipyard/ArmsOffensivePickedUp", this.m_guiCamera);
			}
			SimpleSprite component2 = GuiUtils.FindChildOf(this.m_dragObjectIcon, "ArmamentListItemThumbnail").GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component2, GuiUtils.GetArmamentThumbnail(partName));
			GuiUtils.FindChildOf(this.m_dragObjectIcon, "ArmamentListItemSize").GetComponent<SpriteText>().Text = component.m_width.ToString() + "X" + component.m_length.ToString();
			this.m_dragObjectIcon.SetActiveRecursively(false);
			if (!this.IsDeveloper() && this.m_portShipDef.NumberOfHardpoints() >= this.m_maxHardpoints)
			{
				this.m_dragWidth = 128;
				this.m_dragHeight = 128;
				GuiUtils.FindChildOf(this.m_gui.transform, "MaxArmsPanel").GetComponent<UIPanel>().BringIn();
			}
			this.SpawnBatteryMarkers();
		}
		else
		{
			this.m_dragWidth = 1;
			this.m_dragHeight = 1;
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00023208 File Offset: 0x00021408
	private void PrepareDraging(Vector3 hitPos, Vector3 mousePos, GameObject go)
	{
		this.SetSelectedHpModule(null);
		this.m_dragStart = hitPos;
		this.m_dragStartMousePos = mousePos;
		this.m_dragObject = go;
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00023228 File Offset: 0x00021428
	private void StartDraging(Vector3 mousePos)
	{
		this.m_draging = true;
		this.m_dragLastPos = mousePos;
		this.OnDragStarted(this.m_dragStart, this.m_dragStartMousePos, this.m_dragObject);
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x0002325C File Offset: 0x0002145C
	private void OnMouseReleased(Vector3 pos, GameObject go)
	{
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00023260 File Offset: 0x00021460
	private void OnDragStarted(Vector3 pos, Vector3 mousePos, GameObject go)
	{
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00023264 File Offset: 0x00021464
	private void ListPartsDelegate(ref POINTER_INFO ptr)
	{
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00023268 File Offset: 0x00021468
	private bool IsInsideHardPointList(GameObject go)
	{
		if (!go)
		{
			return false;
		}
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "ShipEquipmentPanel");
		return go.transform.position.x <= gameObject.transform.position.x + 280f;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x000232CC File Offset: 0x000214CC
	private GameObject PlayEffect(string name, Vector3 position)
	{
		GameObject original = Resources.Load(name) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00023300 File Offset: 0x00021500
	private void OnDragUpdate(Vector3 startPos, Vector3 pos, Vector3 mouseDelta, GameObject go)
	{
		if (this.m_dragObject == null)
		{
			Vector3 vector = -mouseDelta * this.m_mouseMoveSpeed * 50f;
			this.m_sceneCamera.transform.localPosition += new Vector3(vector.x, 0f, vector.y);
			Vector3 b = this.m_sceneCamera.transform.localPosition - this.portShip.transform.position;
			if (b.x < -10f)
			{
				b.x = -10f;
			}
			if (b.x > 10f)
			{
				b.x = 10f;
			}
			if (b.z < -10f)
			{
				b.z = -10f;
			}
			if (b.z > 10f)
			{
				b.z = 10f;
			}
			this.m_sceneCamera.transform.localPosition = this.portShip.transform.position + b;
		}
		else
		{
			pos.z = 50f;
			Vector3 position = this.m_sceneCamera.camera.ScreenToWorldPoint(pos);
			this.m_dragObject.transform.position = position;
			if (this.m_dragObjectIcon)
			{
				pos.z = -10f;
				this.m_dragObjectIcon.transform.position = this.m_guiCamera.camera.ScreenToWorldPoint(pos);
			}
			GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "ShipEquipmentPanel");
			if (this.IsInsideHardPointList(this.m_dragObjectIcon))
			{
				this.m_dragObjectIcon.SetActiveRecursively(true);
				this.m_dragObject.SetActiveRecursively(false);
			}
			else
			{
				if (!this.m_dragObject.active)
				{
					this.PlayEffect("GUI_effects/GUI_smoke", this.m_dragObject.transform.position);
				}
				this.m_dragObjectIcon.SetActiveRecursively(false);
				this.m_dragObject.SetActiveRecursively(true);
			}
			this.SnapDragObject2();
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x0002352C File Offset: 0x0002172C
	private void OnDragStoped(Vector3 pos, GameObject go)
	{
		this.SetDragObject(string.Empty);
		if (!this.IsInsideHardPointList(this.m_dragObjectIcon))
		{
			this.SetInformation(null);
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00023554 File Offset: 0x00021754
	private float GetPinchDistance()
	{
		if (Input.touchCount == 2)
		{
			return Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
		}
		return 0f;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x0002359C File Offset: 0x0002179C
	private void DragSelected()
	{
		if (this.m_selectedHPModule == null)
		{
			return;
		}
		this.PlayEffect("GUI_effects/GUI_smoke", this.m_selectedHPModule.transform.position);
		string name = this.m_selectedHPModule.name;
		this.RemovePart(this.m_selectedHPModule);
		this.SetSelectedHpModule(null);
		this.SetPart(name, false);
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00023600 File Offset: 0x00021800
	private void UpdateMotion()
	{
		if (this.m_saveDialog != null)
		{
			return;
		}
		UIManager component = this.m_guiCamera.GetComponent<UIManager>();
		bool flag = component.DidAnyPointerHitUI();
		if (Input.touchCount <= 1)
		{
			if (!this.m_draging)
			{
				if (flag)
				{
					return;
				}
				if (Input.GetMouseButtonDown(0))
				{
					if (this.m_selectedHPModule && this.m_selectedHPModule == this.GetHardpointAtMouse())
					{
						this.DragSelected();
						return;
					}
					this.PrepareDraging(Input.mousePosition, Input.mousePosition, null);
				}
				if (Input.GetMouseButton(0))
				{
					float num = Vector3.Distance(this.m_dragStartMousePos, Input.mousePosition);
					if (num > 10f)
					{
						this.StartDraging(Input.mousePosition);
					}
				}
				if (Input.GetMouseButtonUp(0))
				{
					this.OnMouseReleased(Input.mousePosition, null);
				}
			}
			else
			{
				if (Input.GetMouseButton(0))
				{
					Vector3 mouseDelta = Input.mousePosition - this.m_dragLastPos;
					this.m_dragLastPos = Input.mousePosition;
					this.OnDragUpdate(this.m_dragStart, this.MousePosition(), mouseDelta, this.m_dragObject);
				}
				if (Input.GetMouseButtonUp(0) && this.m_draging)
				{
					this.m_draging = false;
					this.DropModule2();
					this.OnDragStoped(Input.mousePosition, this.m_dragObject);
				}
			}
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		this.Zoom(axis * 100f, Time.deltaTime);
		if (Input.touchCount == 2 && !this.m_draging)
		{
			float pinchDistance = this.GetPinchDistance();
			if (!this.m_pinchZoom)
			{
				this.m_pinchZoom = true;
				this.m_pinchStartDistance = pinchDistance;
			}
			else
			{
				float num2 = -(this.m_pinchStartDistance - pinchDistance) / 5f;
				this.m_pinchStartDistance = pinchDistance;
				this.Zoom(num2 * 10f, Time.deltaTime);
			}
		}
		if (this.m_pinchZoom && Input.touchCount < 2)
		{
			this.m_pinchZoom = false;
		}
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00023800 File Offset: 0x00021A00
	public void SnapDragObject()
	{
		this.m_dragObject.transform.rotation = Battery.GetRotation(this.m_currentDir);
		int layerMask = 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("EDITOR ONLY");
		GameObject gameObject = GameObject.Find("MainCamera");
		Ray ray = gameObject.camera.ScreenPointToRay(this.MousePosition());
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, out raycastHit, 10000f, layerMask))
		{
			return;
		}
		if (raycastHit.collider.gameObject.name != "visual")
		{
			return;
		}
		Battery component = raycastHit.collider.gameObject.transform.parent.gameObject.GetComponent<Battery>();
		int num;
		int num2;
		component.WorldToTile(raycastHit.point, out num, out num2);
		if (!component.CanPlaceAt(num, num2, this.m_dragWidth, this.m_dragHeight, null))
		{
			return;
		}
		this.m_dragObject.transform.position = component.transform.position + component.GetModulePosition(num, num2, this.m_dragWidth, this.m_dragHeight);
		this.m_dragObject.transform.rotation = this.m_dragObject.transform.rotation * component.transform.rotation;
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x0002395C File Offset: 0x00021B5C
	public void SnapDragObject2()
	{
		HPModule component = this.m_dragObject.GetComponent<HPModule>();
		this.m_dragObject.transform.rotation = Battery.GetRotation(this.m_currentDir);
		Vector3 position = this.MousePosition();
		position.z = 50f;
		Vector3 vector = this.m_sceneCamera.camera.ScreenToWorldPoint(position);
		this.m_dragObject.transform.position = vector;
		Battery[] componentsInChildren = this.portShip.GetComponentsInChildren<Battery>();
		float num = 500000f;
		foreach (Battery battery in componentsInChildren)
		{
			if (battery.AllowedModule(component))
			{
				List<Vector3> list;
				battery.GetFitTiles(out list, this.m_dragWidth, this.m_dragHeight, false);
				foreach (Vector3 vector2 in list)
				{
					vector.y = vector2.y;
					Vector3 vector3 = vector2 - vector;
					if (vector3.magnitude < num)
					{
						num = vector3.magnitude;
						this.m_dropPosition = vector2;
						this.m_dropBattery = battery;
					}
				}
			}
		}
		if (this.m_dropBattery == null)
		{
			return;
		}
		if (num > 5f)
		{
			this.m_dropBattery = null;
			return;
		}
		int num2;
		int num3;
		this.m_dropBattery.WorldToTile(this.m_dropPosition, out num2, out num3);
		if (!this.m_dropBattery.CanPlaceAt(num2, num3, this.m_dragWidth, this.m_dragHeight, null))
		{
			this.m_dropBattery = null;
			return;
		}
		this.m_dragObject.transform.position = this.m_dropBattery.GetWorldPlacePos(num2, num3, component);
		this.m_dragObject.transform.rotation = this.m_dragObject.transform.rotation * this.m_dropBattery.transform.rotation;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00023B70 File Offset: 0x00021D70
	public void DropModule2()
	{
		if (this.m_portShipDef.NumberOfHardpoints() >= this.m_maxHardpoints)
		{
			if (!this.IsDeveloper())
			{
				this.ResetBatteryMarkers();
				return;
			}
			PLog.LogWarning("To many hardpoints on boat. This will not be a valid Playerboat.");
		}
		if (this.m_dropBattery == null)
		{
			if (this.m_dragObject)
			{
				this.PlayEffect("GUI_effects/GUI_Watersplash", this.m_dragObject.transform.position);
			}
			this.ResetBatteryMarkers();
			return;
		}
		int x;
		int y;
		this.m_dropBattery.WorldToTile(this.m_dropPosition, out x, out y);
		ShipMenu.HpPosition hpPosition = new ShipMenu.HpPosition();
		hpPosition.m_OrderNum = this.m_dropBattery.GetOrderNumber();
		hpPosition.m_sectionType = this.m_dropBattery.GetSectionType();
		hpPosition.m_gridPosition.x = x;
		hpPosition.m_gridPosition.y = y;
		this.m_dragWidth = 1;
		this.m_dragHeight = 1;
		this.m_dragType = HPModule.HPModuleType.Any;
		this.AddPart(this.m_part, this.m_dropBattery.gameObject, x, y, this.m_currentDir, true);
		this.SetSelectedHpModule(this.GetHPModule(hpPosition));
		this.PlayEffect("GUI_effects/GUI_sparks", this.m_dropPosition);
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00023CA0 File Offset: 0x00021EA0
	public void DropModule()
	{
		PLog.Log("DropModule 1");
		int layerMask = 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("EDITOR ONLY");
		GameObject gameObject = GameObject.Find("Camera");
		Ray ray = gameObject.camera.ScreenPointToRay(this.MousePosition());
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(ray, out raycastHit, 10000f, layerMask);
		Color color = new Color(1f, 0f, 0.5f, 1f);
		if (!flag)
		{
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10000f);
			return;
		}
		Debug.DrawLine(ray.origin, raycastHit.point);
		Debug.DrawLine(raycastHit.point, ray.origin + ray.direction * 10000f, color);
		PLog.Log("DropModule 2: " + raycastHit.collider.gameObject.name);
		if (raycastHit.collider.gameObject.name != "visual")
		{
			return;
		}
		PLog.Log("DropModule 3");
		Battery component = raycastHit.collider.gameObject.transform.parent.gameObject.GetComponent<Battery>();
		int x;
		int y;
		component.WorldToTile(raycastHit.point, out x, out y);
		ShipMenu.HpPosition hpPosition = new ShipMenu.HpPosition();
		hpPosition.m_OrderNum = component.GetOrderNumber();
		hpPosition.m_sectionType = component.GetSectionType();
		hpPosition.m_gridPosition.x = x;
		hpPosition.m_gridPosition.y = y;
		this.AddPart(this.m_part, raycastHit.collider.gameObject.transform.parent.gameObject, x, y, this.m_currentDir, true);
		this.SetSelectedHpModule(this.GetHPModule(hpPosition));
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00023E90 File Offset: 0x00022090
	private GameObject GetHardpointAtMouse()
	{
		int layerMask = 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("EDITOR ONLY");
		GameObject gameObject = GameObject.Find("MainCamera");
		Ray ray = gameObject.camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(ray, out raycastHit, 10000f, layerMask);
		Color color = new Color(1f, 0f, 0.5f, 1f);
		if (!flag)
		{
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10000f);
			return null;
		}
		Debug.DrawLine(ray.origin, raycastHit.point);
		Debug.DrawLine(raycastHit.point, ray.origin + ray.direction * 10000f, color);
		GameObject result = null;
		if (raycastHit.collider.gameObject.name != "visual")
		{
			result = raycastHit.collider.gameObject;
		}
		return result;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00023FA8 File Offset: 0x000221A8
	public void Update()
	{
		this.UpdateMotion();
		GameObject hardpointAtMouse = this.GetHardpointAtMouse();
		if (hardpointAtMouse && Input.GetMouseButtonUp(0))
		{
			if (this.m_selectedHPModule)
			{
				PLog.Log("Should dragn drop");
			}
			PLog.Log("Eggg dragn drop");
			this.SetSelectedHpModule(hardpointAtMouse);
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Update();
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x0002401C File Offset: 0x0002221C
	private SectionDef GetSectionDef(Section.SectionType type, bool recreate)
	{
		if (type == Section.SectionType.Front)
		{
			if (recreate)
			{
				this.m_portShipDef.m_frontSection = new SectionDef();
			}
			return this.m_portShipDef.m_frontSection;
		}
		if (type == Section.SectionType.Mid)
		{
			if (recreate)
			{
				this.m_portShipDef.m_midSection = new SectionDef();
			}
			return this.m_portShipDef.m_midSection;
		}
		if (type == Section.SectionType.Rear)
		{
			if (recreate)
			{
				this.m_portShipDef.m_rearSection = new SectionDef();
			}
			return this.m_portShipDef.m_rearSection;
		}
		if (recreate)
		{
			this.m_portShipDef.m_topSection = new SectionDef();
		}
		return this.m_portShipDef.m_topSection;
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000240C4 File Offset: 0x000222C4
	private string RemovePart(GameObject go)
	{
		Battery component = go.transform.parent.GetComponent<Battery>();
		HPModule component2 = go.GetComponent<HPModule>();
		Vector2i gridPos = component2.GetGridPos();
		Section component3 = component.transform.parent.GetComponent<Section>();
		SectionDef sectionDef = this.GetSectionDef(component3.m_type, false);
		sectionDef.RemoveModule(component.GetOrderNumber(), gridPos);
		this.SpawnPortShip();
		return string.Empty;
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x0002412C File Offset: 0x0002232C
	private void AddPart(string name, GameObject go, int x, int y, Direction dir, bool verifyPosition)
	{
		Battery component = go.GetComponent<Battery>();
		GameObject prefab = ObjectFactory.instance.GetPrefab(name);
		if (prefab == null)
		{
			PLog.LogError("Missing prefab " + name);
			return;
		}
		HPModule component2 = prefab.GetComponent<HPModule>();
		if (verifyPosition && !component.CanPlaceAt(x, y, this.m_dragWidth, this.m_dragHeight, null))
		{
			return;
		}
		Section component3 = component.transform.parent.GetComponent<Section>();
		SectionDef sectionDef = this.GetSectionDef(component3.m_type, false);
		sectionDef.m_modules.Add(new ModuleDef(name, component.GetOrderNumber(), new Vector2i(x, y), dir));
		this.SpawnPortShip();
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x000241DC File Offset: 0x000223DC
	private List<string> GetAvailableShips()
	{
		List<string> availableShips = this.m_userManClient.GetAvailableShips(this.m_campaignID);
		List<string> list = new List<string>();
		foreach (string text in availableShips)
		{
			GameObject prefab = ObjectFactory.instance.GetPrefab(text);
			if (!(prefab == null))
			{
				Ship component = prefab.GetComponent<Ship>();
				if (!(component == null))
				{
					if (this.IsDeveloper() || component.m_editByPlayer)
					{
						if (!list.Contains(text))
						{
							list.Add(text);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x000242B8 File Offset: 0x000224B8
	public void Close()
	{
		this.CloseDialog();
		this.SetDragObject(string.Empty);
		this.UnRegisterDelegatesToComponents();
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "BodyBowScrollList");
		gameObject.GetComponent<UIScrollList>().ClearList(true);
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_gui.transform, "BodyMidScrollList");
		gameObject2.GetComponent<UIScrollList>().ClearList(true);
		GameObject gameObject3 = GuiUtils.FindChildOf(this.m_gui.transform, "BodySternScrollList");
		gameObject3.GetComponent<UIScrollList>().ClearList(true);
		GameObject gameObject4 = GuiUtils.FindChildOf(this.m_gui.transform, "BodyTopScrollList");
		gameObject4.GetComponent<UIScrollList>().ClearList(true);
		GameObject gameObject5 = GuiUtils.FindChildOf(this.m_gui.transform, "ArmsDefensiveScrollList");
		gameObject5.GetComponent<UIScrollList>().ClearList(true);
		GameObject gameObject6 = GuiUtils.FindChildOf(this.m_gui.transform, "ArmsOffensiveScrollList");
		gameObject6.GetComponent<UIScrollList>().ClearList(true);
		this.m_panelFleetTop.BringIn();
		this.m_panelShipBrowser.BringIn();
		this.m_panelShipName.Dismiss();
		this.m_panelShipInfo.Dismiss();
		this.m_panelShipEquipment.Dismiss();
		if (this.portShip != null)
		{
			UnityEngine.Object.Destroy(this.portShip);
		}
		GameObject gameObject7 = GameObject.Find("Main");
		DebugUtils.Assert(this.m_sceneCamera != null, "Failed to find Main viewpoint");
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00024424 File Offset: 0x00022624
	private List<string> GetAvailableHPModules()
	{
		List<string> availableHPModules = this.m_userManClient.GetAvailableHPModules(this.m_campaignID);
		List<string> list = new List<string>();
		foreach (string text in availableHPModules)
		{
			GameObject prefab = ObjectFactory.instance.GetPrefab(text);
			if (!(prefab == null))
			{
				HPModule component = prefab.GetComponent<HPModule>();
				if (!(component == null))
				{
					if (this.IsDeveloper() || component.m_editByPlayer)
					{
						list.Add(text);
					}
				}
			}
		}
		HashSet<string> collection = new HashSet<string>(list);
		return new List<string>(collection);
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x00024508 File Offset: 0x00022708
	private List<string> GetFilteredHPModules(HPModule.HPModuleType filterType)
	{
		List<string> availableHPModules = this.GetAvailableHPModules();
		if (availableHPModules == null)
		{
			PLog.Log("No HPModules are listed by the userManClient.");
			return new List<string>();
		}
		List<string> list = new List<string>();
		foreach (string text in availableHPModules)
		{
			GameObject prefab = ObjectFactory.instance.GetPrefab(text);
			if (prefab != null)
			{
				HPModule component = prefab.GetComponent<HPModule>();
				if (component.m_type == filterType)
				{
					list.Add(text);
				}
			}
			else
			{
				PLog.LogWarning("HPModule prefab do not exist." + text);
				list.Add(text);
			}
		}
		return list;
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x000245DC File Offset: 0x000227DC
	private List<string> GetFilteredSections(Section.SectionType filterType)
	{
		List<string> availableSections = this.m_userManClient.GetAvailableSections(this.m_campaignID);
		List<string> list = new List<string>();
		if (availableSections == null)
		{
			PLog.Log("No Sections are listed by the userManClient.");
			return new List<string>();
		}
		foreach (string text in availableSections)
		{
			GameObject prefab = ObjectFactory.instance.GetPrefab(text);
			if (prefab != null)
			{
				Section component = prefab.GetComponent<Section>();
				if (this.m_shipSeries == component.m_series && component.m_type == filterType)
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x000246B4 File Offset: 0x000228B4
	private void SetDotsAllOn(GameObject bodyItem, string name)
	{
		for (int i = 1; i <= 5; i++)
		{
			string name2 = name + i.ToString();
			GameObject gameObject = GuiUtils.FindChildOf(bodyItem, name2);
			if (gameObject == null)
			{
				return;
			}
			SimpleSprite component = gameObject.GetComponent<SimpleSprite>();
			string name3 = string.Empty;
			if (name == "HealthDot")
			{
				name3 = "ShipyardBodyLamps/gui_shipyard_bodylamp_on_health";
			}
			if (name == "ArmorDot")
			{
				name3 = "ShipyardBodyLamps/gui_shipyard_bodylamp_on_armor";
			}
			if (name == "SpeedDot")
			{
				name3 = "ShipyardBodyLamps/gui_shipyard_bodylamp_on_speed";
			}
			GuiUtils.SetImage(component, name3);
		}
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x00024754 File Offset: 0x00022954
	private void SetDots(GameObject bodyItem, string name, int level)
	{
		for (int i = level; i <= 5; i++)
		{
			string name2 = name + i.ToString();
			GameObject gameObject = GuiUtils.FindChildOf(bodyItem, name2);
			if (gameObject == null)
			{
				return;
			}
			SimpleSprite component = gameObject.GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component, "ShipyardBodyLamps/gui_shipyard_bodylamp_off");
		}
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x000247A8 File Offset: 0x000229A8
	public void FillScrollList(UIScrollList list, List<string> content, string prefabName, bool hardpoints)
	{
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui.transform, "ShipBrowserClassScrollList").GetComponent<UIScrollList>();
		GameObject original = Resources.Load("gui/Shipyard/" + prefabName) as GameObject;
		foreach (string text in content)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			GuiUtils.LocalizeGui(gameObject);
			if (hardpoints)
			{
				GUI_Blueprint_Part component2 = GuiUtils.FindChildOf(gameObject, prefabName + "Button").GetComponent<GUI_Blueprint_Part>();
				component2.Initialize(text, this);
				GuiUtils.FindChildOf(gameObject, "InfoButton").GetComponent<GUI_Blueprint_Part>().Initialize(text, this);
				SpriteText component3 = GuiUtils.FindChildOf(gameObject, "ArmamentListItemHeader").GetComponent<SpriteText>();
				GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext1").GetComponent<SpriteText>().Text = string.Empty;
				GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext2").GetComponent<SpriteText>().Text = string.Empty;
				GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext3").GetComponent<SpriteText>().Text = string.Empty;
				GameObject prefab = ObjectFactory.instance.GetPrefab(text);
				HPModule component4 = prefab.GetComponent<HPModule>();
				List<string> hardpointInfo = component4.GetHardpointInfo();
				HPModuleSettings module = ComponentDB.instance.GetModule(text);
				GuiUtils.FindChildOf(gameObject, "ArmamentListItemSize").GetComponent<SpriteText>().Text = component4.m_width.ToString() + "X" + component4.m_length.ToString();
				SimpleSprite component5 = GuiUtils.FindChildOf(gameObject, "ArmamentListItemThumbnail").GetComponent<SimpleSprite>();
				GuiUtils.SetImage(component5, "ArmamentThumbnails/ArmamentThumb_" + text);
				component3.Text = Localize.instance.TranslateRecursive("$" + text + "_name");
				GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext1").GetComponent<SpriteText>().Text = module.m_value.ToString() + Localize.instance.Translate(" $label_pointssmall");
				if (hardpointInfo.Count >= 1)
				{
					GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext2").GetComponent<SpriteText>().Text = hardpointInfo[0];
				}
				if (hardpointInfo.Count >= 2)
				{
					GuiUtils.FindChildOf(gameObject, "ArmamentListItemSubtext3").GetComponent<SpriteText>().Text = hardpointInfo[1];
				}
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "InfoButton").GetComponent<GUI_Blueprint_Part>().Initialize(text, this);
				SpriteText component6 = GuiUtils.FindChildOf(gameObject, "BodyListItemHeader").GetComponent<SpriteText>();
				GameObject prefab2 = ObjectFactory.instance.GetPrefab(text);
				Section component7 = prefab2.GetComponent<Section>();
				component6.Text = Localize.instance.TranslateRecursive("$" + text + "_name");
				SectionSettings section = ComponentDB.instance.GetSection(text);
				GuiUtils.FindChildOf(gameObject, "BodyListItemCost").GetComponent<SpriteText>().Text = section.m_value.ToString() + Localize.instance.Translate(" $label_pointssmall");
				this.SetDots(gameObject, "HealthDot", component7.m_rating.m_health);
				this.SetDots(gameObject, "ArmorDot", component7.m_rating.m_armor);
				this.SetDots(gameObject, "SpeedDot", component7.m_rating.m_speed);
				this.SetDots(gameObject, "SightDot", component7.m_rating.m_sight);
				GameObject gameObject2 = GuiUtils.FindChildOf(gameObject, "BodyListItemCheckmark");
				GUI_Blueprint_Part gui_Blueprint_Part = gameObject2.AddComponent<GUI_Blueprint_Part>();
				gui_Blueprint_Part.Initialize(text, this);
				UIRadioBtn component8 = gameObject2.GetComponent<UIRadioBtn>();
				component8.useParentForGrouping = false;
				component8.SetGroup(this.m_buttonGroup);
				component8.scriptWithMethodToInvoke = gui_Blueprint_Part;
				component8.methodToInvoke = "OnPress";
				component8.whenToInvoke = POINTER_INFO.INPUT_EVENT.PRESS;
				if (text == this.m_portShipDef.m_frontSection.m_prefab)
				{
					component8.Value = true;
				}
				if (text == this.m_portShipDef.m_midSection.m_prefab)
				{
					component8.Value = true;
				}
				if (text == this.m_portShipDef.m_rearSection.m_prefab)
				{
					component8.Value = true;
				}
				if (text == this.m_portShipDef.m_topSection.m_prefab)
				{
					component8.Value = true;
				}
			}
			list.AddItem(gameObject);
		}
		this.m_buttonGroup++;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00024C30 File Offset: 0x00022E30
	public void FillPartsList()
	{
		this.m_buttonGroup = 0;
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "BodyBowScrollList");
		gameObject.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject.GetComponent<UIScrollList>(), this.GetFilteredSections(Section.SectionType.Front), "BodyListItem", false);
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_gui.transform, "BodyMidScrollList");
		gameObject2.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject2.GetComponent<UIScrollList>(), this.GetFilteredSections(Section.SectionType.Mid), "BodyListItem", false);
		GameObject gameObject3 = GuiUtils.FindChildOf(this.m_gui.transform, "BodySternScrollList");
		gameObject3.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject3.GetComponent<UIScrollList>(), this.GetFilteredSections(Section.SectionType.Rear), "BodyListItem", false);
		GameObject gameObject4 = GuiUtils.FindChildOf(this.m_gui.transform, "BodyTopScrollList");
		gameObject4.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject4.GetComponent<UIScrollList>(), this.GetFilteredSections(Section.SectionType.Top), "BodyListItem_Top", false);
		GameObject gameObject5 = GuiUtils.FindChildOf(this.m_gui.transform, "ArmsDefensiveScrollList");
		gameObject5.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject5.GetComponent<UIScrollList>(), this.GetFilteredHPModules(HPModule.HPModuleType.Defensive), "ArmsDefensiveListItem", true);
		GameObject gameObject6 = GuiUtils.FindChildOf(this.m_gui.transform, "ArmsOffensiveScrollList");
		gameObject6.GetComponent<UIScrollList>().ClearList(true);
		this.FillScrollList(gameObject6.GetComponent<UIScrollList>(), this.GetFilteredHPModules(HPModule.HPModuleType.Offensive), "ArmsOffensiveListItem", true);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00024DAC File Offset: 0x00022FAC
	public void ShowInfo(string partName)
	{
		PLog.Log("ShowInfo: " + partName);
		this.m_part = partName;
		Quaternion rot = default(Quaternion);
		Vector3 mousePosition = Input.mousePosition;
		GameObject gameObject = ObjectFactory.instance.Create(partName, mousePosition, rot);
		this.SetInformation(gameObject);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00024DFC File Offset: 0x00022FFC
	public void SetPart(string partName, bool refreshship)
	{
		GameObject prefab = ObjectFactory.instance.GetPrefab(partName);
		if (prefab.GetComponent<Section>() != null)
		{
			if (prefab == null)
			{
				PLog.LogError("Missing prefab " + partName);
				return;
			}
			Section.SectionType type = prefab.GetComponent<Section>().m_type;
			this.GetSectionDef(type, true).m_prefab = partName;
			if (refreshship)
			{
				this.SpawnPortShip();
			}
			GameObject prefab2 = ObjectFactory.instance.GetPrefab(partName);
			if (prefab2)
			{
				this.m_part = partName;
				this.ShowInfoPanel(this.m_infoBody);
				this.SetInformation(prefab2);
			}
		}
		else
		{
			this.m_part = partName;
			this.SetSelectedHpModule(null);
			this.SetDragObject(this.m_part);
		}
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00024EBC File Offset: 0x000230BC
	private void SetShowAllViewCones(bool show)
	{
		Gun[] componentsInChildren = this.portShip.GetComponentsInChildren<Gun>();
		foreach (Gun gun in componentsInChildren)
		{
			if (show)
			{
				gun.ShowViewCone();
			}
			else
			{
				gun.HideViewCone();
			}
		}
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00024F08 File Offset: 0x00023108
	private void SetSelectedHpModule(GameObject go)
	{
		if (this.m_selectedHPModule != null)
		{
			HPModule component = this.m_selectedHPModule.GetComponent<HPModule>();
			component.SetHighlight(false);
			component.EnableSelectionMarker(false);
			if (!this.m_showAllViewCones)
			{
				Gun component2 = this.m_selectedHPModule.GetComponent<Gun>();
				if (component2 != null)
				{
					component2.HideViewCone();
				}
			}
		}
		this.m_selectedHPModule = go;
		if (go == null)
		{
			this.SetInformation(null);
		}
		else
		{
			this.m_part = go.name;
			this.SetInformation(go);
			HPModule component = this.m_selectedHPModule.GetComponent<HPModule>();
			component.SetHighlight(true);
			component.EnableSelectionMarker(true);
			Gun component3 = this.m_selectedHPModule.GetComponent<Gun>();
			if (component3 != null)
			{
				component3.ShowViewCone();
			}
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00024FD8 File Offset: 0x000231D8
	private void RecalcCost()
	{
		int shipValue = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		this.m_lblTotalCost.GetComponent<SpriteText>().Text = shipValue.ToString() + " " + Localize.instance.Translate(" $label_pointssmall");
		int num = this.m_fleetBaseCost + shipValue;
		this.m_fleetMenu.SetFleetCost(num);
		bool flag = this.m_fleetMenu.HasValidFleet(num);
		string text;
		if (flag)
		{
			text = Constants.m_shipYardSize_Valid.ToString();
		}
		else
		{
			text = Constants.m_shipYardSize_Invalid.ToString();
		}
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeValue").GetComponent<SpriteText>().Text = string.Concat(new string[]
		{
			text,
			num.ToString(),
			" / ",
			this.m_fleetSize.max.ToString(),
			" ",
			Localize.instance.Translate(" $label_pointssmall")
		});
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x000250E4 File Offset: 0x000232E4
	private void SetShipClassName(string name)
	{
		GuiUtils.FindChildOf(this.m_gui, "ShipClassValueLabel").GetComponent<SpriteText>().Text = name;
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00025104 File Offset: 0x00023304
	private void SpawnPortShip()
	{
		if (this.m_editShip.m_ship != null)
		{
			UnityEngine.Object.Destroy(this.m_editShip.m_ship);
		}
		this.SetShipClassName(Localize.instance.TranslateRecursive("$" + this.m_portShipDef.m_prefab + "_name"));
		this.SetShipTitle(this.m_portShipDef.m_name);
		this.m_editShip.m_ship = ShipFactory.CreateShip(this.m_portShipDef, this.m_editShip.m_shipPosition, Quaternion.identity, -1);
		this.portShip = this.m_editShip.m_ship;
		NetObj[] componentsInChildren = this.m_editShip.m_ship.GetComponentsInChildren<NetObj>();
		foreach (NetObj netObj in componentsInChildren)
		{
			netObj.SetVisible(true);
		}
		ParticleSystem[] componentsInChildren2 = this.portShip.GetComponentsInChildren<ParticleSystem>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].gameObject.SetActiveRecursively(false);
		}
		this.m_maxHardpoints = this.portShip.GetComponent<Ship>().m_maxHardpoints;
		this.RecalcCost();
		this.markerBase = new GameObject();
		this.markerBase.transform.parent = this.m_editShip.m_ship.transform;
		this.SpawnBatteryMarkers();
		this.m_shipModified = true;
		string text = string.Concat(new string[]
		{
			this.m_portShipDef.NumberOfHardpoints().ToString(),
			"/",
			this.m_maxHardpoints.ToString(),
			" ",
			Localize.instance.Translate("$arms")
		});
		GuiUtils.FindChildOf(this.m_gui.transform, "ArmsLabel").GetComponent<SpriteText>().Text = text;
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x000252DC File Offset: 0x000234DC
	public void OnLevelWasLoaded()
	{
		this.SpawnPortShip();
		this.m_sceneCamera = GameObject.Find("MainCamera");
		DebugUtils.Assert(this.m_sceneCamera != null, "Failed to find camera");
		this.m_shipModified = false;
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00025314 File Offset: 0x00023514
	private void Exit(bool save)
	{
		if (save && this.m_onSave != null)
		{
			this.m_onSave(this.m_portShipDef);
		}
		if (this.m_onExit == null)
		{
			return;
		}
		this.m_onExit();
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00025350 File Offset: 0x00023550
	private void OpenSaveDialog(string title, string text)
	{
		this.m_saveDialog = GuiUtils.CreateGui("GenericInputDialog", this.m_guiCamera);
		GenericTextInput component = this.m_saveDialog.GetComponent<GenericTextInput>();
		DebugUtils.Assert(component != null, "Failed to create GenericTextInput, prefab does not have a GenericTextInput-script on it!");
		component.Initialize(title, "Cancel", "Ok", text, new GenericTextInput.InputTextCancel(this.OnSaveDialogCancelPressed), new GenericTextInput.InputTextCommit(this.OnSaveDialogOkPressed));
		component.AllowEmptyInput = false;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x000253C4 File Offset: 0x000235C4
	private void UnRegisterDelegatesToComponents()
	{
		GuiUtils.FindChildOf(this.m_gui.transform, "DoneButton").GetComponent<UIButton>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnBackPressed));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyBowButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyMidButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodySternButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyTopButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ArmsDefensiveButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ArmsOffensiveButton").GetComponent<UIPanelTab>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ShipNameInputBox").GetComponent<UITextField>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnRenameShip));
		GuiUtils.FindChildOf(this.m_gui.transform, "SaveBlueprintButton").GetComponent<UIButton>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveBluePrint));
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00025554 File Offset: 0x00023754
	private void RegisterDelegatesToComponents()
	{
		GuiUtils.FindChildOf(this.m_gui, "HelpButton2").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		this.m_panelFleetTop = GuiUtils.FindChildOf(this.m_gui.transform, "FleetTopPanel").GetComponent<UIPanel>();
		this.m_panelShipBrowser = GuiUtils.FindChildOf(this.m_gui.transform, "FleetShipBrowserPanel").GetComponent<UIPanel>();
		this.m_panelShipName = GuiUtils.FindChildOf(this.m_gui.transform, "ShipNamePanel").GetComponent<UIPanel>();
		this.m_panelShipInfo = GuiUtils.FindChildOf(this.m_gui.transform, "ShipInfoPanel").GetComponent<UIPanel>();
		this.m_panelShipEquipment = GuiUtils.FindChildOf(this.m_gui.transform, "ShipEquipmentPanel").GetComponent<UIPanel>();
		this.m_infoArmament = GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "Armament");
		this.m_infoBody = GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "Body");
		this.m_infoBodyTop = GuiUtils.FindChildOf(this.m_panelShipInfo.transform, "Body_Top");
		this.m_HPMenu = GuiUtils.FindChildOf(this.m_gui.transform, "ShipManipulators");
		this.m_HPDeleta = GuiUtils.FindChildOf(this.m_gui.transform, "DeleteArmButton");
		this.m_HPRotate = GuiUtils.FindChildOf(this.m_gui.transform, "RotateArmButton");
		this.m_HPViewCone = GuiUtils.FindChildOf(this.m_gui.transform, "HPViewCone");
		GuiUtils.FindChildOf(this.m_gui.transform, "DoneButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnBackPressed));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyBowButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyMidButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodySternButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "BodyTopButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ArmsDefensiveButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ArmsOffensiveButton").GetComponent<UIPanelTab>().AddValueChangedDelegate(new EZValueChangedDelegate(this.RefreshList));
		GuiUtils.FindChildOf(this.m_gui.transform, "ShipNameInputBox").GetComponent<UITextField>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnRenameShip));
		GuiUtils.FindChildOf(this.m_gui.transform, "SaveBlueprintButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveBluePrint));
		this.m_lblTotalCost = GuiUtils.FindChildOf(this.m_gui.transform, "PointsLabel");
		GuiUtils.FindChildOf(this.m_gui.transform, "btnSaveXml").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveXml));
		if (!this.IsDeveloper())
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSaveXml").transform.position = new Vector3(5000f, 0f, 0f);
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x000258EC File Offset: 0x00023AEC
	private void onHelp(IUIObject button)
	{
		ToolTipDisplay toolTip = ToolTipDisplay.GetToolTip(this.m_gui);
		if (toolTip == null)
		{
			return;
		}
		if (button.gameObject.GetComponent<UIStateToggleBtn>().StateNum == 0)
		{
			toolTip.SetHelpMode(false);
		}
		if (button.gameObject.GetComponent<UIStateToggleBtn>().StateNum == 1)
		{
			toolTip.SetHelpMode(true);
		}
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0002594C File Offset: 0x00023B4C
	private void OnRenameShip(IUIObject control)
	{
		string text = GuiUtils.FindChildOf(this.m_gui.transform, "ShipNameInputBox").GetComponent<UITextField>().Text;
		text = text.Trim();
		if (this.m_portShipDef.m_name == text)
		{
			return;
		}
		this.m_portShipDef.m_name = text;
		this.SetShipTitle(this.m_portShipDef.m_name);
		this.m_shipModified = true;
		PLog.Log("*************************************************");
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x000259C8 File Offset: 0x00023BC8
	private void OnSaveBluePrint(IUIObject obj)
	{
		this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$label_saveblueprintas"), this.m_portShipDef.m_name, new GenericTextInput.InputTextCancel(this.DialogCancelPressed), new GenericTextInput.InputTextCommit(this.SaveBluePrintDialogOkPressed));
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00025A18 File Offset: 0x00023C18
	private void DialogCancelPressed2(IUIObject obj)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00025A2C File Offset: 0x00023C2C
	private void DialogCancelPressed()
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00025A40 File Offset: 0x00023C40
	private void RenameDialogOkPressed(string text)
	{
		string name = text.Trim();
		this.m_portShipDef.m_name = name;
		this.SetShipTitle(this.m_portShipDef.m_name);
		this.m_shipModified = true;
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x00025A8C File Offset: 0x00023C8C
	private void SaveBluePrintDialogOkPressed(string text)
	{
		string name = text.Trim();
		this.m_portShipDef.m_name = name;
		this.m_portShipDef.m_value = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		this.m_portShipDef.m_campaignID = this.m_campaignID;
		this.m_userManClient.AddShip(this.m_portShipDef);
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		this.m_saveDialog = GuiUtils.OpenAlertDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_blueprint_created"), string.Empty);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "DismissButton").GetComponent<UIButton>().Text = Localize.instance.Translate("$button_dismiss");
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "DismissButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.DialogCancelPressed2));
		this.m_userManClient.UnlockAchievement(0);
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00025B88 File Offset: 0x00023D88
	private void ShowInfoSection(Section.SectionType type)
	{
		SectionDef sectionDef = this.GetSectionDef(type, false);
		this.SetInformationName(sectionDef.m_prefab);
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00025BAC File Offset: 0x00023DAC
	private void RefreshList(IUIObject obj)
	{
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00025BB0 File Offset: 0x00023DB0
	private bool IsDialogVisible()
	{
		return this.m_saveDialog != null || this.m_msgBox != null;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00025BD4 File Offset: 0x00023DD4
	private void OnBackPressed(IUIObject obj)
	{
		if (this.IsDialogVisible())
		{
			return;
		}
		this.m_portShipDef.m_value = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		if (this.m_shipModified)
		{
			this.m_saveDialog = GuiUtils.OpenMultiChoiceDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_savechange"), new EZValueChangedDelegate(this.OnDoneCancel), new EZValueChangedDelegate(this.OnDoneSaveNo), new EZValueChangedDelegate(this.OnDoneSaveYes));
		}
		else
		{
			this.Exit(false);
		}
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00025C64 File Offset: 0x00023E64
	private void CloseDialog()
	{
		if (this.m_saveDialog == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x00025C98 File Offset: 0x00023E98
	private void OnDoneCancel(IUIObject obj)
	{
		this.CloseDialog();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00025CA0 File Offset: 0x00023EA0
	private void OnDoneSaveNo(IUIObject obj)
	{
		this.CloseDialog();
		this.Exit(false);
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x00025CB0 File Offset: 0x00023EB0
	private void OnDoneSaveYes(IUIObject obj)
	{
		this.CloseDialog();
		if (string.IsNullOrEmpty(this.m_portShipDef.m_name))
		{
			this.m_onSaveExit = true;
			this.OpenSaveDialog("Save As", string.Empty);
		}
		else
		{
			this.SetShipTitle(this.m_portShipDef.m_name);
			this.Exit(true);
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00025D0C File Offset: 0x00023F0C
	private void OnSaveDialogCancelPressed()
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00025D20 File Offset: 0x00023F20
	private bool ShipExist(string shipname)
	{
		List<ShipDef> shipDefs = this.m_userManClient.GetShipDefs(this.m_campaignID);
		foreach (ShipDef shipDef in shipDefs)
		{
			if (shipDef.m_name == shipname)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x00025DA8 File Offset: 0x00023FA8
	private void OnSaveDialogOkPressed(string text)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		string text2 = text.Trim();
		if (this.ShipExist(text2))
		{
			this.m_saveDialog.SetActiveRecursively(false);
			this.m_msgBox = new MsgBox(this.m_guiCamera, MsgBox.Type.YesNo, string.Format(Localize.instance.Translate("$shipedit_overwrite"), new object[0]), null, null, new MsgBox.YesHandler(this.OverwriteSave), new MsgBox.NoHandler(this.DontOverwriteSave));
		}
		else
		{
			UnityEngine.Object.Destroy(this.m_saveDialog);
			this.m_saveDialog = null;
			this.m_portShipDef.m_name = text2;
			this.SetShipTitle(this.m_portShipDef.m_name);
			this.m_shipModified = false;
			this.m_onSave(this.m_portShipDef);
			if (this.m_onSaveExit)
			{
				this.Exit(false);
			}
		}
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x00025E8C File Offset: 0x0002408C
	private void OverwriteSave()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		string name = this.m_saveDialog.GetComponent<GenericTextInput>().Text.Trim();
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		this.m_portShipDef.m_name = name;
		this.SetShipTitle(this.m_portShipDef.m_name);
		this.m_onSave(this.m_portShipDef);
		if (this.m_onSaveExit)
		{
			this.Exit(false);
		}
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00025F14 File Offset: 0x00024114
	private void DontOverwriteSave()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_saveDialog.SetActiveRecursively(true);
		this.m_saveDialog.GetComponent<GenericTextInput>().Text = string.Empty;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00025F54 File Offset: 0x00024154
	private void OnSavePressed(IUIObject obj)
	{
		this.m_portShipDef.m_value = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		this.OpenSaveDialog("Enter the name of this ship", this.m_portShipDef.m_name);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00025F88 File Offset: 0x00024188
	private void OnSaveXml(IUIObject obj)
	{
		this.m_portShipDef.m_value = ShipDefUtils.GetShipValue(this.m_portShipDef, ComponentDB.instance);
		XmlWriter xmlWriter = XmlWriter.Create("editorship.xml", new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "\t"
		});
		xmlWriter.WriteStartElement("root");
		this.m_portShipDef.Save(xmlWriter);
		xmlWriter.WriteEndElement();
		xmlWriter.Flush();
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00025FF8 File Offset: 0x000241F8
	public Direction GetNextDirection(Direction dir)
	{
		dir++;
		if (dir > Direction.Left)
		{
			dir = Direction.Forward;
		}
		return dir;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0002600C File Offset: 0x0002420C
	private void OnRotate(IUIObject obj)
	{
		if (this.m_selectedHPModule == null)
		{
			return;
		}
		ShipMenu.HpPosition hpPosition = new ShipMenu.HpPosition();
		GameObject gameObject = this.m_selectedHPModule.transform.parent.gameObject;
		Battery component = gameObject.GetComponent<Battery>();
		hpPosition.m_OrderNum = component.GetOrderNumber();
		hpPosition.m_sectionType = component.GetSectionType();
		string name = this.m_selectedHPModule.name;
		PLog.LogWarning("partName: " + name);
		HPModule component2 = this.m_selectedHPModule.GetComponent<HPModule>();
		hpPosition.m_gridPosition = component2.GetGridPos();
		Direction dir = component2.GetDir();
		Direction nextDirection = this.GetNextDirection(dir);
		PLog.Log("Trying Position: " + nextDirection.ToString());
		component2.SetDir(nextDirection);
		if (!component.CanPlaceAt(hpPosition.m_gridPosition.x, hpPosition.m_gridPosition.y, component2.GetWidth(), component2.GetLength(), component2))
		{
			nextDirection = this.GetNextDirection(nextDirection);
			PLog.Log("Trying Position: " + nextDirection.ToString());
			component2.SetDir(nextDirection);
			if (!component.CanPlaceAt(hpPosition.m_gridPosition.x, hpPosition.m_gridPosition.y, component2.GetWidth(), component2.GetLength(), component2))
			{
				return;
			}
			component2.SetDir(dir);
		}
		this.RemovePart(this.m_selectedHPModule);
		PLog.LogWarning("batteryGo: " + gameObject.name);
		this.AddPart(name, gameObject, hpPosition.m_gridPosition.x, hpPosition.m_gridPosition.y, nextDirection, false);
		this.m_currentDir = nextDirection;
		this.SetSelectedHpModule(this.GetHPModule(hpPosition));
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x000261C8 File Offset: 0x000243C8
	private void OnViewCone(IUIObject obj)
	{
		this.m_showAllViewCones = !this.m_showAllViewCones;
		this.SetShowAllViewCones(this.m_showAllViewCones);
		if (!this.m_showAllViewCones && this.m_selectedHPModule != null)
		{
			Gun component = this.m_selectedHPModule.GetComponent<Gun>();
			if (component != null)
			{
				component.ShowViewCone();
			}
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0002622C File Offset: 0x0002442C
	private GameObject GetHPModule(ShipMenu.HpPosition hpPos)
	{
		Ship component = this.portShip.GetComponent<Ship>();
		Section section = component.GetSection(hpPos.m_sectionType);
		Battery battery = section.GetBattery(hpPos.m_OrderNum);
		HPModule moduleAt = battery.GetModuleAt(hpPos.m_gridPosition.x, hpPos.m_gridPosition.y);
		if (moduleAt == null)
		{
			return null;
		}
		return moduleAt.gameObject;
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00026290 File Offset: 0x00024490
	private void SetShipTitle(string name)
	{
		GuiUtils.FindChildOf(this.m_gui.transform, "ShipNameInputBox").GetComponent<UITextField>().Text = name;
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x000262C0 File Offset: 0x000244C0
	private Vector3 MousePosition()
	{
		return Input.mousePosition;
	}

	// Token: 0x04000376 RID: 886
	private const float m_moveTreshold = 20f;

	// Token: 0x04000377 RID: 887
	public ShipMenu.OnExitDelegate m_onExit;

	// Token: 0x04000378 RID: 888
	public ShipMenu.OnSaveDelegate m_onSave;

	// Token: 0x04000379 RID: 889
	private int m_campaignID;

	// Token: 0x0400037A RID: 890
	private string m_shipSeries = string.Empty;

	// Token: 0x0400037B RID: 891
	private GameObject portShip;

	// Token: 0x0400037C RID: 892
	private GameObject markerBase;

	// Token: 0x0400037D RID: 893
	private ShipDef m_portShipDef;

	// Token: 0x0400037E RID: 894
	private string m_part = string.Empty;

	// Token: 0x0400037F RID: 895
	private Direction m_currentDir;

	// Token: 0x04000380 RID: 896
	private GameObject m_selectedHPModule;

	// Token: 0x04000381 RID: 897
	private GameObject m_lblTotalCost;

	// Token: 0x04000382 RID: 898
	private GameObject m_HPMenu;

	// Token: 0x04000383 RID: 899
	private GameObject m_HPDeleta;

	// Token: 0x04000384 RID: 900
	private GameObject m_HPRotate;

	// Token: 0x04000385 RID: 901
	private GameObject m_HPViewCone;

	// Token: 0x04000386 RID: 902
	private GameObject m_gui;

	// Token: 0x04000387 RID: 903
	private GameObject m_saveDialog;

	// Token: 0x04000388 RID: 904
	private GameObject m_guiCamera;

	// Token: 0x04000389 RID: 905
	private GameObject m_sceneCamera;

	// Token: 0x0400038A RID: 906
	private UserManClient m_userManClient;

	// Token: 0x0400038B RID: 907
	private Vector3 m_minEdge;

	// Token: 0x0400038C RID: 908
	private Vector3 m_maxEdge;

	// Token: 0x0400038D RID: 909
	private bool m_draging;

	// Token: 0x0400038E RID: 910
	private Vector3 m_dragStart;

	// Token: 0x0400038F RID: 911
	private Vector3 m_dragStartMousePos;

	// Token: 0x04000390 RID: 912
	private Vector3 m_dragLastPos;

	// Token: 0x04000391 RID: 913
	private GameObject m_dragObject;

	// Token: 0x04000392 RID: 914
	private GameObject m_dragObjectIcon;

	// Token: 0x04000393 RID: 915
	private int m_dragWidth = 1;

	// Token: 0x04000394 RID: 916
	private int m_dragHeight = 1;

	// Token: 0x04000395 RID: 917
	private HPModule.HPModuleType m_dragType = HPModule.HPModuleType.Any;

	// Token: 0x04000396 RID: 918
	private Vector3 m_dropPosition;

	// Token: 0x04000397 RID: 919
	private Battery m_dropBattery;

	// Token: 0x04000398 RID: 920
	private float m_mouseMoveSpeed = 0.001f;

	// Token: 0x04000399 RID: 921
	private float m_camsize = 30f;

	// Token: 0x0400039A RID: 922
	private float m_zoomMin = 20f;

	// Token: 0x0400039B RID: 923
	private float m_zoomMax = 60f;

	// Token: 0x0400039C RID: 924
	private float m_allowZoomTime;

	// Token: 0x0400039D RID: 925
	private Vector3 m_listPost = new Vector3(0f, 0f, 0f);

	// Token: 0x0400039E RID: 926
	private bool m_pinchZoom;

	// Token: 0x0400039F RID: 927
	private float m_pinchStartDistance = -1f;

	// Token: 0x040003A0 RID: 928
	private bool m_showAllViewCones;

	// Token: 0x040003A1 RID: 929
	private bool m_shipModified;

	// Token: 0x040003A2 RID: 930
	private MsgBox m_msgBox;

	// Token: 0x040003A3 RID: 931
	private bool m_onSaveExit;

	// Token: 0x040003A4 RID: 932
	private string m_prevScene;

	// Token: 0x040003A5 RID: 933
	private UIPanel m_panelFleetTop;

	// Token: 0x040003A6 RID: 934
	private UIPanel m_panelShipBrowser;

	// Token: 0x040003A7 RID: 935
	private UIPanel m_panelShipName;

	// Token: 0x040003A8 RID: 936
	private UIPanel m_panelShipInfo;

	// Token: 0x040003A9 RID: 937
	private UIPanel m_panelShipEquipment;

	// Token: 0x040003AA RID: 938
	private GameObject m_infoArmament;

	// Token: 0x040003AB RID: 939
	private GameObject m_infoBody;

	// Token: 0x040003AC RID: 940
	private GameObject m_infoBodyTop;

	// Token: 0x040003AD RID: 941
	private FleetShip m_editShip;

	// Token: 0x040003AE RID: 942
	private int m_fleetBaseCost;

	// Token: 0x040003AF RID: 943
	private int m_buttonGroup;

	// Token: 0x040003B0 RID: 944
	private FleetMenu m_fleetMenu;

	// Token: 0x040003B1 RID: 945
	private FleetSize m_fleetSize;

	// Token: 0x040003B2 RID: 946
	private int m_maxHardpoints = 10;

	// Token: 0x02000061 RID: 97
	public class HpPosition
	{
		// Token: 0x040003B3 RID: 947
		public int m_OrderNum;

		// Token: 0x040003B4 RID: 948
		public Section.SectionType m_sectionType;

		// Token: 0x040003B5 RID: 949
		public Vector2i m_gridPosition;
	}

	// Token: 0x02000062 RID: 98
	public enum PartType
	{
		// Token: 0x040003B7 RID: 951
		Hull,
		// Token: 0x040003B8 RID: 952
		Hadpoints
	}

	// Token: 0x020001AA RID: 426
	// (Invoke) Token: 0x06000F58 RID: 3928
	public delegate void OnExitDelegate();

	// Token: 0x020001AB RID: 427
	// (Invoke) Token: 0x06000F5C RID: 3932
	public delegate void OnSaveDelegate(ShipDef shipDef);
}
