using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class FleetMenu
{
	// Token: 0x06000264 RID: 612 RVA: 0x000112EC File Offset: 0x0000F4EC
	public FleetMenu(GameObject guiCamera, UserManClient userManClient, string fleetName, int campaignID, FleetSize fleetSize, bool oneFleetOnly, MusicManager musicMan)
	{
		this.m_musicMan = musicMan;
		this.m_oneFleetOnly = oneFleetOnly;
		NetObj.SetSimulating(false);
		this.m_guiCamera = guiCamera;
		this.m_userManClient = userManClient;
		this.m_campaignID = campaignID;
		this.m_gui = GuiUtils.CreateGui("Shipyard/Shipyard", this.m_guiCamera);
		this.m_fleetSize = fleetSize;
		this.m_sceneCamera = GameObject.Find("MainCamera");
		DebugUtils.Assert(this.m_sceneCamera != null, "Failed to find camera");
		this.RegisterDelegatesToComponents();
		this.m_originalFleetName = fleetName;
		this.m_removeOrginalFleet = false;
		this.m_fleetName = fleetName;
		this.SetFleetName(this.m_fleetName);
		this.m_ShipBrowserPanel.BringIn();
		this.m_FleetTopPanel.BringIn();
		this.m_InfoBlueprint.Dismiss();
		this.m_InfoClass.Dismiss();
		this.m_openFleetDlg.Dismiss();
		List<ShipDef> shipDefs = userManClient.GetShipDefs(campaignID);
		List<ShipDef> shipListFromFleet = userManClient.GetShipListFromFleet(fleetName, campaignID);
		this.SetupCamera();
		this.m_musicMan.SetMusic("music-credits");
		this.SetSizeIndicators(fleetSize);
		this.SetupClassList();
		this.SetupBluePrint();
		this.SetAddShipButtonStatus(true);
		this.FleetShipsFill();
		this.RecalcFleetCost();
		this.SetFleetModified(false);
		this.CleanUp();
		UserManClient userManClient2 = this.m_userManClient;
		userManClient2.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Combine(userManClient2.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
	}

	// Token: 0x06000266 RID: 614 RVA: 0x000114D4 File Offset: 0x0000F6D4
	~FleetMenu()
	{
		PLog.Log("FleetMenu DESTROYED");
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00011514 File Offset: 0x0000F714
	public void CleanUp()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00011524 File Offset: 0x0000F724
	private void OnUserManUpdate()
	{
		this.SetupBluePrint();
		this.RefreshShipButtonStatus();
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00011534 File Offset: 0x0000F734
	private void SetSizeIndicators(FleetSize fleetSize)
	{
		PLog.Log("SetSizeIndicators: " + fleetSize.min.ToString() + "  " + fleetSize.max.ToString());
		string text = "$fleetsize_freesize_shipyard";
		if (FleetSizes.GetSizeClass(fleetSize) == FleetSizeClass.Small)
		{
			text = "$fleetsize_small_shipyard";
		}
		if (FleetSizes.GetSizeClass(fleetSize) == FleetSizeClass.Medium)
		{
			text = "$fleetsize_medium_shipyard";
		}
		if (FleetSizes.GetSizeClass(fleetSize) == FleetSizeClass.Heavy)
		{
			text = "$fleetsize_large_shipyard";
		}
		if (FleetSizes.GetSizeClass(fleetSize) == FleetSizeClass.Custom)
		{
			text = fleetSize.max.ToString() + " $label_pointssmall";
		}
		this.SetFleetCost(4242);
		GuiUtils.FindChildOf(this.m_gui.transform, "SizeRestrictionLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(text);
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00011600 File Offset: 0x0000F800
	private void SetupClassList()
	{
		GameObject original = Resources.Load("gui/Shipyard/ShipBrowserClassListItem") as GameObject;
		List<string> availableShips = this.GetAvailableShips();
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui.transform, "ShipBrowserClassScrollList").GetComponent<UIScrollList>();
		component.ClearList(true);
		foreach (string text in availableShips)
		{
			ShipDef shipDef = this.GetShipDef(text, string.Empty);
			if (shipDef != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				GuiUtils.LocalizeGui(gameObject);
				GuiUtils.FindChildOf(gameObject, "ClassMainButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnAddShip));
				GuiUtils.FindChildOf(gameObject, "ClassInfoButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnShowClassInfo));
				SpriteText component2 = GuiUtils.FindChildOf(gameObject, "ClassButtonLabel").GetComponent<SpriteText>();
				component2.Text = Localize.instance.TranslateRecursive("$" + text + "_name");
				SimpleTag simpleTag = GuiUtils.FindChildOf(gameObject, "ClassMainButton").transform.parent.gameObject.AddComponent<SimpleTag>();
				simpleTag.m_tag = text;
				SimpleSprite component3 = GuiUtils.FindChildOf(gameObject, "ClassImage").GetComponent<SimpleSprite>();
				GuiUtils.SetImage(component3, "ClassSilouettes/Silouette" + shipDef.m_prefab);
				SpriteText component4 = GuiUtils.FindChildOf(gameObject, "ClassCostValue").GetComponent<SpriteText>();
				component4.Text = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance).ToString() + Localize.instance.Translate(" $label_pointssmall");
				component.AddItem(gameObject);
			}
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x000117DC File Offset: 0x0000F9DC
	private void SetupBluePrint()
	{
		GameObject original = Resources.Load("gui/Shipyard/ShipBrowserBlueprintListItem") as GameObject;
		this.m_bluePrints = this.m_userManClient.GetShipDefs(this.m_campaignID);
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui.transform, "ShipBrowserBlueprintScrollList").GetComponent<UIScrollList>();
		component.ClearList(true);
		foreach (ShipDef shipDef in this.m_bluePrints)
		{
			if (!(this.m_selectedBlueprint == shipDef.m_name))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				GuiUtils.LocalizeGui(gameObject);
				GuiUtils.FindChildOf(gameObject, "BlueprintMainButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnAddBlueprint));
				GuiUtils.FindChildOf(gameObject, "BlueprintInfoButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnShowBluePrintInfo));
				SpriteText component2 = GuiUtils.FindChildOf(gameObject, "BlueprintButtonLabel").GetComponent<SpriteText>();
				component2.Text = shipDef.m_name;
				SimpleSprite component3 = GuiUtils.FindChildOf(gameObject, "ClassImage").GetComponent<SimpleSprite>();
				GuiUtils.SetImage(component3, "ClassSilouettes/Silouette" + shipDef.m_prefab);
				SpriteText component4 = GuiUtils.FindChildOf(gameObject, "BlueprintCostValue").GetComponent<SpriteText>();
				component4.Text = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance).ToString() + Localize.instance.Translate(" $label_pointssmall");
				component.AddItem(gameObject);
			}
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0001198C File Offset: 0x0000FB8C
	private void SetZ(GameObject go, float z)
	{
		Vector3 localPosition = go.transform.localPosition;
		localPosition.z = z;
		go.transform.localPosition = localPosition;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x000119BC File Offset: 0x0000FBBC
	private void SetAddShipButtonStatus(bool enable)
	{
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui.transform, "ShipBrowserClassScrollList").GetComponent<UIScrollList>();
		for (int i = 0; i < component.Count; i++)
		{
			IUIListObject item = component.GetItem(i);
			GuiUtils.FindChildOf(item.gameObject, "ClassMainButton").GetComponent<UIButton>().controlIsEnabled = enable;
		}
		UIScrollList component2 = GuiUtils.FindChildOf(this.m_gui.transform, "ShipBrowserBlueprintScrollList").GetComponent<UIScrollList>();
		for (int j = 0; j < component2.Count; j++)
		{
			IUIListObject item2 = component2.GetItem(j);
			GameObject gameObject = GuiUtils.FindChildOf(item2.transform, "BlueprintButtonLabel");
			SpriteText component3 = gameObject.GetComponent<SpriteText>();
			ShipDef bluePrintSipDef = this.GetBluePrintSipDef(component3.Text);
			if (bluePrintSipDef.m_available)
			{
				this.SetZ(GuiUtils.FindChildOf(item2.gameObject, "LblDLCNotAvailable"), 5f);
				GuiUtils.FindChildOf(item2.gameObject, "BlueprintMainButton").GetComponent<UIButton>().controlIsEnabled = enable;
				GuiUtils.FindChildOf(item2.gameObject, "BlueprintInfoButton").GetComponent<UIButton>().controlIsEnabled = enable;
			}
			else
			{
				this.SetZ(GuiUtils.FindChildOf(item2.gameObject, "LblDLCNotAvailable"), -5f);
				GuiUtils.FindChildOf(item2.gameObject, "BlueprintMainButton").GetComponent<UIButton>().controlIsEnabled = false;
				GuiUtils.FindChildOf(item2.gameObject, "BlueprintInfoButton").GetComponent<UIButton>().controlIsEnabled = false;
			}
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00011B44 File Offset: 0x0000FD44
	private void FleetShipsClear()
	{
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			fleetShip.Destroy();
		}
		this.m_fleetShips.Clear();
		this.m_editShip = null;
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00011BBC File Offset: 0x0000FDBC
	private void FleetShipsHide()
	{
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			fleetShip.Destroy();
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00011C24 File Offset: 0x0000FE24
	private void FleetShipsShow()
	{
		string name = "Fleet";
		GameObject gameObject = GameObject.Find(name);
		this.m_xOffset = 0f;
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			fleetShip.Destroy();
			GameObject gameObject2 = this.SpawnShip(fleetShip.m_definition);
			Ship component = gameObject2.GetComponent<Ship>();
			fleetShip.m_maxHardpoints = component.m_maxHardpoints;
			fleetShip.m_ship = gameObject2;
			fleetShip.m_width = component.m_Width;
			fleetShip.m_length = component.m_length;
			fleetShip.m_basePosition = gameObject.transform.position + new Vector3(this.m_xOffset, 0f, 5f);
			fleetShip.m_shipPosition = fleetShip.m_basePosition + new Vector3(0f, 0f, -(fleetShip.m_length / 2f + 18f));
			gameObject2.transform.position = fleetShip.m_shipPosition;
			this.m_xOffset += 25f;
			this.CreateFloater(fleetShip);
			fleetShip.m_cost = ShipDefUtils.GetShipValue(fleetShip.m_definition, ComponentDB.instance);
		}
		this.RecalcFleetCost();
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00011D8C File Offset: 0x0000FF8C
	private void FleetShipsFill()
	{
		this.FleetShipsClear();
		List<ShipDef> shipListFromFleet = this.m_userManClient.GetShipListFromFleet(this.m_fleetName, this.m_campaignID);
		if (shipListFromFleet == null)
		{
			return;
		}
		if (shipListFromFleet.Count == 0)
		{
			return;
		}
		string name = "Fleet";
		GameObject gameObject = GameObject.Find(name);
		foreach (ShipDef definition in shipListFromFleet)
		{
			FleetShip fleetShip = new FleetShip();
			fleetShip.m_definition = definition;
			this.m_fleetShips.Add(fleetShip);
		}
		this.FleetShipsShow();
		this.RefreshShipButtonStatus();
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00011E50 File Offset: 0x00010050
	private void RefreshShipButtonStatus()
	{
		if (this.m_fleetShips.Count == 8)
		{
			this.SetAddShipButtonStatus(false);
		}
		else
		{
			this.SetAddShipButtonStatus(true);
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00011E84 File Offset: 0x00010084
	private GameObject SpawnShip(ShipDef def)
	{
		string name = "Fleet";
		GameObject gameObject = GameObject.Find(name);
		GameObject gameObject2 = ShipFactory.CreateShip(def, gameObject.transform.position, gameObject.transform.rotation, -1);
		NetObj[] componentsInChildren = gameObject2.GetComponentsInChildren<NetObj>();
		foreach (NetObj netObj in componentsInChildren)
		{
			netObj.SetVisible(true);
		}
		ParticleSystem[] componentsInChildren2 = gameObject2.GetComponentsInChildren<ParticleSystem>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].gameObject.SetActiveRecursively(false);
		}
		return gameObject2;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00011F20 File Offset: 0x00010120
	public void CreateFloater(FleetShip fleetInfo)
	{
		GameObject original = GuiUtils.FindChildOf(this.m_gui.transform, "FloatingInfo");
		fleetInfo.m_floatingInfo = (UnityEngine.Object.Instantiate(original) as GameObject);
		fleetInfo.m_floatingInfo.transform.parent = this.m_gui.transform;
		GuiUtils.FindChildOf(fleetInfo.m_floatingInfo, "FloatingInfoRemoveButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnDeleteShip));
		GuiUtils.FindChildOf(fleetInfo.m_floatingInfo, "FloatingInfoCloneButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnCloneShip));
		GuiUtils.FindChildOf(fleetInfo.m_floatingInfo, "BtnMoveLeft").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnMoveShipLeft));
		GuiUtils.FindChildOf(fleetInfo.m_floatingInfo, "BtnMoveRight").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnMoveShipRight));
		if (this.m_fleetShips.Count == 8)
		{
			GuiUtils.FindChildOf(fleetInfo.m_floatingInfo, "FloatingInfoCloneButton").GetComponent<UIButton>().controlIsEnabled = false;
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00012034 File Offset: 0x00010234
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
					if (!list.Contains(text))
					{
						list.Add(text);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06000276 RID: 630 RVA: 0x000120F4 File Offset: 0x000102F4
	public Unit GetUnit(GameObject go)
	{
		if (go.name == "boxcollider")
		{
			go = go.transform.parent.gameObject;
		}
		Section component = go.GetComponent<Section>();
		if (component)
		{
			return component.GetUnit();
		}
		HPModule component2 = go.GetComponent<HPModule>();
		if (component2)
		{
			return component2.GetUnit();
		}
		return null;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0001215C File Offset: 0x0001035C
	public FleetShip SelectShip()
	{
		ToolTipDisplay toolTip = ToolTipDisplay.GetToolTip(this.m_gui);
		if (toolTip && toolTip.GetHelpMode())
		{
			return null;
		}
		int layerMask = 1 << LayerMask.NameToLayer("hpmodules") | 1 << LayerMask.NameToLayer("units");
		GameObject gameObject = GameObject.Find("MainCamera");
		Ray ray = gameObject.camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, out raycastHit, 10000f, layerMask))
		{
			return null;
		}
		Unit unit = this.GetUnit(raycastHit.collider.gameObject);
		if (!unit)
		{
			return null;
		}
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			if (fleetShip.m_ship.GetComponent<Unit>() == unit)
			{
				return fleetShip;
			}
		}
		return null;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00012280 File Offset: 0x00010480
	private void UpdateFocus()
	{
		Vector3 a = this.m_cameraGoalPosition - this.m_cameraCurrentPosition;
		if (a.magnitude < 5f)
		{
			this.m_cameraCurrentPosition = this.m_cameraGoalPosition;
		}
		else
		{
			a.Normalize();
			Vector3 vector = this.m_cameraCurrentPosition;
			vector += a * Time.deltaTime * this.m_cameraPositionSpeed;
			this.m_sceneCamera.transform.position = vector + new Vector3(0f, 100f, 0f);
			this.m_cameraCurrentPosition = vector;
		}
		float f = this.m_cameraGoalSize - this.m_cameraCurrentSize;
		if (Mathf.Abs(f) < 2f)
		{
			this.m_cameraCurrentSize = this.m_cameraGoalSize;
		}
		else
		{
			float num = this.m_cameraCurrentSize;
			num += Time.deltaTime * this.m_cameraSizeSpeed;
			this.m_sceneCamera.GetComponent<Camera>().orthographicSize = num;
			this.m_cameraCurrentSize = num;
		}
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0001237C File Offset: 0x0001057C
	public void Update()
	{
		this.m_shipyardTime += Time.deltaTime;
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			fleetShip.Update(this.m_sceneCamera, this.m_guiCamera);
		}
		this.UpdateFocus();
		if (this.m_setFleetCost != -1)
		{
			this.SetFleetCost(this.m_setFleetCost);
		}
		if (this.m_shipMenu != null)
		{
			this.m_shipMenu.Update();
			return;
		}
		if (!this.IsDialogVisible() && Input.GetMouseButtonDown(0))
		{
			this.m_editShip = this.SelectShip();
			if (this.m_editShip != null && (Application.isEditor || this.m_editShip.m_ship.GetComponent<Ship>().m_editByPlayer))
			{
				this.FleetShipsHide();
				this.m_shipMenu = null;
				this.m_shipMenu = new ShipMenu(this.m_gui, this.m_guiCamera, this.m_userManClient, this.m_editShip, this.m_campaignID, this.m_fleetCost, this, this.m_fleetSize);
				this.m_shipMenu.m_onExit = new ShipMenu.OnExitDelegate(this.OnEditShipExit);
				this.m_shipMenu.m_onSave = new ShipMenu.OnSaveDelegate(this.OnSaveShip);
				this.SetView(this.m_editShip.m_shipPosition, this.m_editShip.m_length / 2f + 10f);
			}
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Update();
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00012534 File Offset: 0x00010734
	public void Close()
	{
		this.CloseDialog();
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_musicMan.SetMusic("menu");
		PLog.Log("Closing FleetEditor !");
		this.m_userManClient.AddShipyardTime(this.m_shipyardTime);
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		this.FleetShipsClear();
		if (this.m_shipMenu != null)
		{
			this.m_shipMenu.Close();
			this.m_shipMenu = null;
		}
		this.UnRegisterDelegatesFromComponents();
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_gui = null;
		GameObject gameObject = GameObject.Find("Main");
		DebugUtils.Assert(this.m_sceneCamera != null, "Failed to find Main viewpoint");
		this.m_sceneCamera.GetComponent<Camera>().orthographic = false;
		this.m_sceneCamera.transform.position = gameObject.transform.position;
		this.m_sceneCamera.transform.rotation = gameObject.transform.rotation;
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0001265C File Offset: 0x0001085C
	public void OnLevelWasLoaded()
	{
		if (this.m_shipMenu != null)
		{
			this.m_shipMenu.OnLevelWasLoaded();
		}
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00012674 File Offset: 0x00010874
	public void OnShipSelectet(ShipDef def)
	{
		this.m_shipToEdit = def;
	}

	// Token: 0x0600027D RID: 637 RVA: 0x00012680 File Offset: 0x00010880
	private void Exit()
	{
		if (this.m_onExit != null)
		{
			this.m_onExit();
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00012698 File Offset: 0x00010898
	private void OpenQuestionDialog(string title, string text, EZValueChangedDelegate cancel, EZValueChangedDelegate ok)
	{
		this.m_saveDialog = GuiUtils.CreateGui("dialogs/Dialog_Question", this.m_guiCamera);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "Header").GetComponent<SpriteText>().Text = title;
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "Message").GetComponent<SpriteText>().Text = text;
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "CancelButton").GetComponent<UIButton>().AddValueChangedDelegate(cancel);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "OkButton").GetComponent<UIButton>().AddValueChangedDelegate(ok);
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0001273C File Offset: 0x0001093C
	private void OpenMultiChoiceDialog(string text, EZValueChangedDelegate cancel, EZValueChangedDelegate nosave, EZValueChangedDelegate save)
	{
		this.m_saveDialog = GuiUtils.CreateGui("MsgBoxMultichoice", this.m_guiCamera);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "TextLabel").GetComponent<SpriteText>().Text = text;
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "BtnCancel").GetComponent<UIButton>().AddValueChangedDelegate(cancel);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "BtnDontSave").GetComponent<UIButton>().AddValueChangedDelegate(nosave);
		GuiUtils.FindChildOf(this.m_saveDialog.transform, "BtnSave").GetComponent<UIButton>().AddValueChangedDelegate(save);
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000127E0 File Offset: 0x000109E0
	private void CloseDialog()
	{
		if (this.m_saveDialog == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00012814 File Offset: 0x00010A14
	private void DialogCancelPressed()
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00012828 File Offset: 0x00010A28
	private void DialogDeleteCancelPressed(IUIObject obj)
	{
		this.m_clearFleet = false;
		this.m_afterSaveOpenFleet = false;
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00012858 File Offset: 0x00010A58
	private void DialogDeletePressed(IUIObject obj)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		if (this.m_fleetName.Length != 0)
		{
			this.m_userManClient.RemoveFleet(this.m_fleetName);
		}
		this.DialogNewFleet(null);
	}

	// Token: 0x06000284 RID: 644 RVA: 0x000128A0 File Offset: 0x00010AA0
	private List<ShipInstanceDef> GetAllShipsAsInstanceDefs()
	{
		List<ShipInstanceDef> list = new List<ShipInstanceDef>();
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			list.Add(new ShipInstanceDef(fleetShip.m_definition.m_name));
			PLog.Log("Adding Ship: " + fleetShip.m_name);
		}
		return list;
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00012934 File Offset: 0x00010B34
	private void SaveFleet(string newName)
	{
		FleetDef fleetDef = new FleetDef();
		fleetDef.m_name = newName;
		fleetDef.m_campaignID = this.m_campaignID;
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			PLog.Log("Adding Ship: " + fleetShip.m_name);
			fleetDef.m_ships.Add(fleetShip.m_definition);
		}
		fleetDef.UpdateValue();
		this.m_userManClient.AddFleet(fleetDef);
		this.m_userManClient.UnlockAchievement(1);
		this.m_fleetName = newName;
		this.SetFleetName(this.m_fleetName);
		this.SetFleetModified(false);
		if (this.m_removeOrginalFleet)
		{
			this.m_removeOrginalFleet = false;
			if (this.m_originalFleetName.Length != 0)
			{
				this.m_userManClient.RemoveFleet(this.m_originalFleetName);
			}
			this.m_originalFleetName = this.m_fleetName;
		}
		if (this.m_clearFleet)
		{
			this.DialogNewFleet(null);
		}
		if (this.m_afterSaveOpenFleet)
		{
			this.DialogOpenFleet(null);
		}
		PLog.Log("SAVE DONE");
	}

	// Token: 0x06000286 RID: 646 RVA: 0x00012A78 File Offset: 0x00010C78
	private void RecalcFleetCost()
	{
		int num = 0;
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			num += fleetShip.m_cost;
		}
		string text = this.m_fleetShips.Count.ToString() + Localize.instance.Translate("$num_of_ships");
		GuiUtils.FindChildOf(this.m_gui.transform, "ShipCounterLabel").GetComponent<SpriteText>().Text = text;
		this.SetFleetCost(num);
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00012B34 File Offset: 0x00010D34
	public bool HasValidFleet(int cost)
	{
		return cost == 0 || (cost >= this.m_fleetSize.min && cost <= this.m_fleetSize.max);
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00012B64 File Offset: 0x00010D64
	public void SetFleetCost(int cost)
	{
		this.m_setFleetCost = cost;
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeLabel").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeValue").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeIconNotOk").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeIconOk").SetActiveRecursively(false);
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeSlider_SetIndicator");
		if (gameObject == null)
		{
			PLog.Log("SLIDER NOT ALIVE ");
			return;
		}
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeSlider_SetSize");
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.GetComponent<UISlider>().Value = (float)this.m_fleetSize.max / 8000f;
		if (gameObject2.GetComponent<UISlider>().GetKnob() == null)
		{
			return;
		}
		this.m_setFleetCost = -1;
		UISlider component = gameObject.GetComponent<UISlider>();
		component.controlIsEnabled = false;
		float num = (float)cost / 8000f;
		if (num > 1f)
		{
			num = 1f;
		}
		component.Value = num;
		bool flag = this.HasValidFleet(cost);
		this.m_fleetCost = cost;
		string text = this.m_fleetCost.ToString();
		while (text.Length < 4)
		{
			text = "0" + text;
		}
		string text2;
		if (flag)
		{
			text2 = Constants.m_shipYardSize_Valid.ToString();
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeIconOk").SetActiveRecursively(true);
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeValue").SetActiveRecursively(true);
		}
		else
		{
			text2 = Constants.m_shipYardSize_Invalid.ToString();
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeIconNotOk").SetActiveRecursively(true);
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeLabel").SetActiveRecursively(true);
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeLabel").GetComponent<SpriteText>().Text = string.Concat(new string[]
			{
				text2,
				Localize.instance.Translate(" $fleetsize_exceeded"),
				" ",
				text,
				"/",
				this.m_fleetSize.max.ToString(),
				" ",
				Localize.instance.Translate(" $label_pointssmall")
			});
		}
		this.SetFleetCostLabel(text2 + text);
	}

	// Token: 0x06000289 RID: 649 RVA: 0x00012E0C File Offset: 0x0001100C
	private void OnSaveShip(ShipDef shipDef)
	{
		if (shipDef != null)
		{
			this.m_editShip.m_definition = shipDef;
			this.SetFleetModified(true);
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x00012E28 File Offset: 0x00011028
	private void OnEditShipExit()
	{
		ShipMenu shipMenu = this.m_shipMenu;
		shipMenu.m_onExit = (ShipMenu.OnExitDelegate)Delegate.Remove(shipMenu.m_onExit, new ShipMenu.OnExitDelegate(this.OnEditShipExit));
		this.m_shipMenu.Close();
		this.m_shipMenu = null;
		GameObject gameObject = GameObject.Find("Center");
		Vector3 pos = gameObject.transform.position + new Vector3(0f, 100f, -50f);
		this.SetView(pos, 80f);
		this.m_editShip = null;
		this.FleetShipsShow();
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00012EB8 File Offset: 0x000110B8
	private void OkToOverridePressed()
	{
		this.SaveFleet(this.m_fleetName);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00012EC8 File Offset: 0x000110C8
	private void NotOkToOverridePressed()
	{
		this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_saveas"), this.m_fleetName + " (copy)", new GenericTextInput.InputTextCancel(this.SaveDialogCancelPressed), new GenericTextInput.InputTextCommit(this.SaveDialogOkPressed));
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00012F20 File Offset: 0x00011120
	private void CancelOverridePressed()
	{
		if (FleetMenu.m_okToOverideOnSaveDialog != null)
		{
			FleetMenu.m_okToOverideOnSaveDialog.Close();
			FleetMenu.m_okToOverideOnSaveDialog = null;
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00012F3C File Offset: 0x0001113C
	private void RenameDialogOkPressed(string fleetName)
	{
		this.m_removeOrginalFleet = true;
		this.SetFleetModified(true);
		this.m_fleetName = fleetName;
		this.SetFleetName(this.m_fleetName);
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00012F74 File Offset: 0x00011174
	private void SaveDialogOkPressed(string fleetName)
	{
		if (this.m_userManClient.GetFleet(fleetName, this.m_campaignID) != null)
		{
			this.m_saveDialog.SetActiveRecursively(false);
			this.m_msgBox = new MsgBox(this.m_guiCamera, MsgBox.Type.YesNo, string.Format(Localize.instance.Translate("$fleetedit_overwrite"), new object[0]), null, null, new MsgBox.YesHandler(this.OverwriteSave), new MsgBox.NoHandler(this.DontOverwriteSave));
		}
		else
		{
			UnityEngine.Object.Destroy(this.m_saveDialog);
			this.m_saveDialog = null;
			this.SaveFleet(fleetName);
			this.SetFleetModified(false);
			if (this.m_onSaveExit)
			{
				this.Exit();
			}
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00013020 File Offset: 0x00011220
	private void OverwriteSave()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		string newName = this.m_saveDialog.GetComponent<GenericTextInput>().Text.Trim();
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		this.SaveFleet(newName);
		this.SetFleetModified(false);
		if (this.m_clearFleet)
		{
			this.m_clearFleet = false;
			this.DialogNewFleet(null);
		}
		if (this.m_onSaveExit)
		{
			this.Exit();
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x000130A0 File Offset: 0x000112A0
	private void DontOverwriteSave()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_saveDialog.SetActiveRecursively(true);
		this.m_saveDialog.GetComponent<GenericTextInput>().Text = string.Empty;
	}

	// Token: 0x06000292 RID: 658 RVA: 0x000130E0 File Offset: 0x000112E0
	private void SaveDialogCancelPressed()
	{
		Debug.Log("SaveDialog Cancel pressed !");
		this.m_clearFleet = false;
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000293 RID: 659 RVA: 0x00013108 File Offset: 0x00011308
	private void DoneSaveNo(IUIObject obj)
	{
		this.CloseDialog();
		this.Exit();
	}

	// Token: 0x06000294 RID: 660 RVA: 0x00013118 File Offset: 0x00011318
	private void DoneSaveYes(IUIObject obj)
	{
		this.CloseDialog();
		if (string.IsNullOrEmpty(this.m_fleetName))
		{
			this.m_onSaveExit = true;
			this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_saveas"), string.Empty, new GenericTextInput.InputTextCancel(this.SaveDialogCancelPressed), new GenericTextInput.InputTextCommit(this.SaveDialogOkPressed));
		}
		else
		{
			this.SaveFleet(this.m_fleetName);
			this.Exit();
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00013198 File Offset: 0x00011398
	private bool IsDialogVisible()
	{
		if (this.m_saveDialog != null)
		{
			return true;
		}
		if (this.m_msgBox != null)
		{
			return true;
		}
		if (GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class").active)
		{
			return true;
		}
		if (GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").active)
		{
			return true;
		}
		UIManager component = this.m_guiCamera.GetComponent<UIManager>();
		return component.DidAnyPointerHitUI();
	}

	// Token: 0x06000296 RID: 662 RVA: 0x00013224 File Offset: 0x00011424
	private void RegisterDelegatesToComponents()
	{
		this.m_ShipBrowserPanel = GuiUtils.FindChildOf(this.m_gui.transform, "FleetShipBrowserPanel").GetComponent<UIPanel>();
		this.m_FleetTopPanel = GuiUtils.FindChildOf(this.m_gui.transform, "FleetTopPanel").GetComponent<UIPanel>();
		this.m_ShipNamePanel = GuiUtils.FindChildOf(this.m_gui.transform, "ShipNamePanel").GetComponent<UIPanel>();
		this.m_InfoBlueprint = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").GetComponent<UIPanel>();
		this.m_InfoClass = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class").GetComponent<UIPanel>();
		this.m_openFleetDlg = GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog").GetComponent<UIPanel>();
		this.lblFleetName = GuiUtils.FindChildOf(this.m_gui.transform, "FleetNameInputBox");
		this.lblPoints = GuiUtils.FindChildOf(this.m_gui.transform, "FleetSizeValue");
		GuiUtils.FindChildOf(this.m_gui.transform, "NewFleetButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNewFleet));
		GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenFleet));
		GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveFleet));
		GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetAsButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveFleetAs));
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetNameInputBox").GetComponent<UITextField>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnRenameFleet));
		GuiUtils.FindChildOf(this.m_gui.transform, "FleetInfoButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnFleetInfo));
		GuiUtils.FindChildOf(this.m_gui.transform, "ExitButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnDonePressed));
		GuiUtils.FindChildOf(GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class"), "OkButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnHideClassInfo));
		GuiUtils.FindChildOf(GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint"), "OkButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnHideluePrintInfo));
		GuiUtils.FindChildOf(GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint"), "DeleteBlueprintButton").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnDeleteBluePrint));
		GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog_Cancel_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenFleetCancel));
		GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog_Open_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenFleetOk));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton1").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_gui, "ShipBlueprintButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnBlueprintTabPressed));
		GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_gui, "ShipClassButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShipClassTabPressed));
		if (this.m_oneFleetOnly)
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "NewFleetButton").GetComponent<UIButton>().controlIsEnabled = false;
			GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetButton").GetComponent<UIButton>().controlIsEnabled = false;
			GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetAsButton").GetComponent<UIButton>().controlIsEnabled = false;
			GuiUtils.FindChildOf(this.m_gui.transform, "FleetNameInputBox").GetComponent<UITextField>().controlIsEnabled = false;
		}
		this.m_ShipNamePanel.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x06000297 RID: 663 RVA: 0x00013650 File Offset: 0x00011850
	private void OnBlueprintTabPressed(IUIObject obj)
	{
		GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "ShipBrowserBlueprintPanel").AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnBluprintTabTransitionComplete));
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00013674 File Offset: 0x00011874
	private void OnBluprintTabTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00013678 File Offset: 0x00011878
	private void OnShipClassTabPressed(IUIObject obj)
	{
		GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "ShipBrowserClassPanel").AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnShipClassTabTransitionComplete));
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0001369C File Offset: 0x0001189C
	private void OnShipClassTabTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
	}

	// Token: 0x0600029B RID: 667 RVA: 0x000136A0 File Offset: 0x000118A0
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

	// Token: 0x0600029C RID: 668 RVA: 0x00013700 File Offset: 0x00011900
	private void UnRegisterDelegatesFromComponents()
	{
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00013704 File Offset: 0x00011904
	private void OnNewFleet(IUIObject obj)
	{
		if (this.m_fleetModified)
		{
			this.m_clearFleet = true;
			this.OpenMultiChoiceDialog(Localize.instance.Translate("$fleetedit_savechange"), new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DialogNewFleet), new EZValueChangedDelegate(this.OnSaveFleet));
			return;
		}
		this.DialogNewFleet(null);
	}

	// Token: 0x0600029E RID: 670 RVA: 0x00013764 File Offset: 0x00011964
	private void DialogNewFleet(IUIObject obj)
	{
		this.CloseDialog();
		this.m_clearFleet = false;
		this.FleetShipsClear();
		this.m_fleetName = string.Empty;
		this.SetFleetName(this.m_fleetName);
		this.SetFleetModified(false);
		this.RecalcFleetCost();
		this.SetAddShipButtonStatus(true);
	}

	// Token: 0x0600029F RID: 671 RVA: 0x000137B0 File Offset: 0x000119B0
	private void OnOpenFleet(IUIObject obj)
	{
		if (this.m_fleetModified)
		{
			this.m_afterSaveOpenFleet = true;
			this.OpenMultiChoiceDialog(Localize.instance.Translate("$fleetedit_savechange"), new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DialogOpenFleet), new EZValueChangedDelegate(this.OnSaveFleet));
			return;
		}
		this.DialogOpenFleet(null);
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00013810 File Offset: 0x00011A10
	private void DialogOpenFleet(IUIObject obj)
	{
		this.m_afterSaveOpenFleet = false;
		this.CloseDialog();
		this.FillFleetList();
		UIPanel component = GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog").GetComponent<UIPanel>();
		component.BringIn();
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00013854 File Offset: 0x00011A54
	private void OnOpenFleetCancel(IUIObject obj)
	{
		UIPanel component = GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog").GetComponent<UIPanel>();
		component.Dismiss();
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x00013884 File Offset: 0x00011A84
	private void OnOpenFleetOk(IUIObject obj)
	{
		UIPanel component = GuiUtils.FindChildOf(this.m_gui.transform, "OpenFleetDialog").GetComponent<UIPanel>();
		component.Dismiss();
		PLog.Log("Open Fleet: " + this.m_selectedFleet);
		this.m_originalFleetName = this.m_selectedFleet;
		this.m_fleetName = this.m_selectedFleet;
		this.FleetShipsFill();
		this.RecalcFleetCost();
		this.SetFleetModified(false);
		this.SetFleetName(this.m_fleetName);
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00013900 File Offset: 0x00011B00
	private void FillFleetList()
	{
		GameObject original = Resources.Load("gui/Shipyard/FleetListItem") as GameObject;
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui, "OpenFleetDialogList").GetComponent<UIScrollList>();
		this.m_fleets.Clear();
		component.ClearList(true);
		List<FleetDef> fleetDefs = this.m_userManClient.GetFleetDefs(0);
		foreach (FleetDef fleetDef in fleetDefs)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Name").GetComponent<SpriteText>().Text = fleetDef.m_name;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Ships").GetComponent<SpriteText>().Text = fleetDef.m_ships.Count.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Points").GetComponent<SpriteText>().Text = fleetDef.m_value.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Size").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey(FleetSizes.GetSizeClassName(fleetDef.m_value));
			UIRadioBtn component2 = GuiUtils.FindChildOf(gameObject, "RadioButton").GetComponent<UIRadioBtn>();
			component2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFleetSelected));
			if (!fleetDef.m_available)
			{
				component2.controlIsEnabled = false;
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "LblDLCNotAvailable").transform.Translate(new Vector3(0f, 0f, 20f));
			}
			UIListItemContainer component3 = gameObject.GetComponent<UIListItemContainer>();
			component.AddItem(component3);
			this.m_fleets.Add(fleetDef.m_name);
		}
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x00013ACC File Offset: 0x00011CCC
	private void OnFleetSelected(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		this.m_selectedFleet = this.m_fleets[component.Index];
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x00013B04 File Offset: 0x00011D04
	private void OnSaveFleet(IUIObject obj)
	{
		if (this.m_saveDialog)
		{
			UnityEngine.Object.Destroy(this.m_saveDialog);
			this.m_saveDialog = null;
		}
		if (string.IsNullOrEmpty(this.m_fleetName))
		{
			this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_saveas"), string.Empty, new GenericTextInput.InputTextCancel(this.SaveDialogCancelPressed), new GenericTextInput.InputTextCommit(this.SaveDialogOkPressed));
		}
		else
		{
			this.SaveFleet(this.m_fleetName);
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00013B94 File Offset: 0x00011D94
	private void OnSaveFleetAs(IUIObject obj)
	{
		this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$shipedit_saveas"), this.m_fleetName, new GenericTextInput.InputTextCancel(this.SaveDialogCancelPressed), new GenericTextInput.InputTextCommit(this.SaveDialogOkPressed));
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00013BE0 File Offset: 0x00011DE0
	private void OnRenameFleet(IUIObject control)
	{
		string text = GuiUtils.FindChildOf(this.m_gui.transform, "FleetNameInputBox").GetComponent<UITextField>().Text;
		if (text == this.m_fleetName)
		{
			return;
		}
		this.m_removeOrginalFleet = true;
		this.SetFleetModified(true);
		this.m_fleetName = text;
		this.SetFleetName(this.m_fleetName);
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00013C54 File Offset: 0x00011E54
	private void OnDeleteFleet(IUIObject obj)
	{
		this.OpenQuestionDialog(Localize.instance.Translate("$shipedit_delete_fleet_title"), Localize.instance.Translate("$shipedit_delete_fleet_text " + this.m_fleetName), new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DialogDeletePressed));
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00013CA8 File Offset: 0x00011EA8
	private void OnFleetInfo(IUIObject obj)
	{
		PLog.Log("OnFleetInfo");
	}

	// Token: 0x060002AA RID: 682 RVA: 0x00013CB4 File Offset: 0x00011EB4
	private void OnDonePressed(IUIObject obj)
	{
		if (this.IsDialogVisible())
		{
			return;
		}
		if (!this.m_fleetModified)
		{
			this.Exit();
			return;
		}
		this.OpenMultiChoiceDialog(Localize.instance.Translate("$fleetedit_savechange"), new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DoneSaveNo), new EZValueChangedDelegate(this.DoneSaveYes));
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00013D18 File Offset: 0x00011F18
	private void OnShowBluePrintInfo(IUIObject obj)
	{
		GameObject gameObject = GuiUtils.FindChildOf(obj.transform.parent, "BlueprintButtonLabel");
		SpriteText component = gameObject.GetComponent<SpriteText>();
		this.m_selectedBlueprint = component.Text;
		GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").GetComponent<UIPanel>().BringIn();
		this.SetBlueprintInformation(component.Text);
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00013D7C File Offset: 0x00011F7C
	private void OnShowClassInfo(IUIObject obj)
	{
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class");
		gameObject.GetComponent<UIPanel>().BringIn();
		GameObject gameObject2 = obj.transform.parent.gameObject;
		SimpleTag component = gameObject2.GetComponent<SimpleTag>();
		this.SetClassInformation(component.m_tag);
	}

	// Token: 0x060002AD RID: 685 RVA: 0x00013DD0 File Offset: 0x00011FD0
	private string DONEVALUE(string value)
	{
		return Localize.instance.Translate(value);
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00013DE0 File Offset: 0x00011FE0
	private void SetClassInformation(string className)
	{
		ShipDef shipDef = this.GetShipDef(className, string.Empty);
		if (shipDef == null)
		{
			return;
		}
		GameObject gameObject = this.SpawnShip(shipDef);
		Ship component = gameObject.GetComponent<Ship>();
		GameObject go = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class");
		GuiUtils.FindChildOf(go, "ClassHeader").GetComponent<SpriteText>().Text = this.DONEVALUE(component.m_displayClassName);
		SimpleSprite component2 = GuiUtils.FindChildOf(go, "ClassImage").GetComponent<SimpleSprite>();
		GuiUtils.SetImage(component2, GuiUtils.GetShipThumbnail(className));
		GuiUtils.FindChildOf(go, "ClassDescriptionText").GetComponent<SpriteText>().Text = this.DONEVALUE(Localize.instance.TranslateRecursive("$" + className + "_flavor"));
		if (component.m_deepKeel)
		{
			GuiUtils.FindChildOf(go, "ClassStandardClassLabel1").GetComponent<SpriteText>().Text = this.DONEVALUE("$label_deepkeel: $button_yes");
		}
		else
		{
			GuiUtils.FindChildOf(go, "ClassStandardClassLabel1").GetComponent<SpriteText>().Text = this.DONEVALUE("$label_deepkeel: $button_no");
		}
		GuiUtils.FindChildOf(go, "ClassStandardClassLabel2").GetComponent<SpriteText>().Text = this.DONEVALUE("$Health " + component.GetMaxHealth().ToString());
		int armorClass = component.GetSectionFront().m_armorClass;
		int armorClass2 = component.GetSectionMid().m_armorClass;
		int armorClass3 = component.GetSectionRear().m_armorClass;
		GuiUtils.FindChildOf(go, "ClassStandardClassLabel3").GetComponent<SpriteText>().Text = this.DONEVALUE(string.Concat(new string[]
		{
			"$label_armor: $shipedit_shortsection_bow ",
			armorClass.ToString(),
			", $shipedit_shortsection_middle ",
			armorClass2.ToString(),
			", $shipedit_shortsection_stern ",
			armorClass3.ToString()
		}));
		GuiUtils.FindChildOf(go, "ClassStandardClassLabel4").GetComponent<SpriteText>().Text = this.DONEVALUE(string.Concat(new string[]
		{
			"$Speed ",
			component.GetMaxSpeed().ToString(),
			" $shipedit_ahead, ",
			component.GetMaxReverseSpeed().ToString(),
			" $shipedit_astern"
		}));
		float num = component.GetLength() * 10f;
		GuiUtils.FindChildOf(go, "ClassStandardClassLabel5").GetComponent<SpriteText>().Text = this.DONEVALUE("$shipedit_length: " + num.ToString() + " $shipedit_meters");
		GuiUtils.FindChildOf(go, "ClassStandardClassLabel6").GetComponent<SpriteText>().Text = string.Empty;
		int shipValue = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance);
		GuiUtils.FindChildOf(go, "ClassStandardClassTotalCost").GetComponent<SpriteText>().Text = this.DONEVALUE("$shipedit_totalcost: " + shipValue.ToString() + " $label_pointssmall");
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x000140A4 File Offset: 0x000122A4
	private int SetBlueprintSectionInfo(Section section, string type)
	{
		SectionSettings section2 = ComponentDB.instance.GetSection(section.name);
		GameObject go = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint");
		GuiUtils.FindChildOf(go, "ModifiedClass_" + type + "NameLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + section.name + "_name");
		GuiUtils.FindChildOf(go, "ModifiedClass_" + type + "HealthLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate("$HEALTH " + section.m_maxHealth.ToString());
		GuiUtils.FindChildOf(go, "ModifiedClass_" + type + "ArmorLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate("$ARMOR " + section.m_armorClass.ToString());
		GuiUtils.FindChildOf(go, "ModifiedClass_" + type + "CostLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(string.Empty + section2.m_value.ToString() + " $label_pointssmall");
		return section2.m_value;
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x000141E0 File Offset: 0x000123E0
	private void SetBlueprintInformation(string blueprintName)
	{
		ShipDef bluePrintSipDef = this.GetBluePrintSipDef(blueprintName);
		if (bluePrintSipDef == null)
		{
			return;
		}
		GameObject gameObject = this.SpawnShip(bluePrintSipDef);
		Ship component = gameObject.GetComponent<Ship>();
		GameObject go = GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint");
		GuiUtils.FindChildOf(go, "BlueprintHeader").GetComponent<SpriteText>().Text = blueprintName;
		GuiUtils.FindChildOf(go, "BlueprintDescriptionText").GetComponent<SpriteText>().Text = Localize.instance.Translate("$" + bluePrintSipDef.m_prefab + "_flavor");
		SimpleSprite component2 = GuiUtils.FindChildOf(this.m_gui.transform, "ModifiedClass_ClassImage").GetComponent<SimpleSprite>();
		GuiUtils.SetImage(component2, GuiUtils.GetShipSilhouette(bluePrintSipDef.m_prefab));
		int num = 0;
		num += this.SetBlueprintSectionInfo(component.GetSectionFront(), "Bow");
		num += this.SetBlueprintSectionInfo(component.GetSectionMid(), "Mid");
		num += this.SetBlueprintSectionInfo(component.GetSectionRear(), "Stern");
		num += this.SetBlueprintSectionInfo(component.GetSectionTop(), "Top");
		GuiUtils.FindChildOf(go, "ModifiedClass_TotalCostLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate("$shipedit_baseoperationalcost: " + num.ToString() + " $label_pointssmall");
		string text = bluePrintSipDef.NumberOfHardpoints().ToString() + "/12 ";
		GuiUtils.FindChildOf(go, "BlueprintArmsTotalArms").GetComponent<SpriteText>().Text = text;
		int num2 = 0;
		int num3 = 1;
		List<string> hardpointNames = bluePrintSipDef.GetHardpointNames();
		foreach (string name in hardpointNames)
		{
			HPModuleSettings module = ComponentDB.instance.GetModule(name);
			GameObject prefab = ObjectFactory.instance.GetPrefab(name);
			HPModule component3 = prefab.GetComponent<HPModule>();
			GuiUtils.FindChildOf(go, "BluePrintArmsLabel" + num3.ToString()).GetComponent<SpriteText>().Text = Localize.instance.TranslateRecursive("$" + component3.name + "_name");
			GuiUtils.FindChildOf(go, "BluePrintArmsCost" + num3.ToString()).GetComponent<SpriteText>().Text = module.m_value.ToString() + Localize.instance.Translate(" $label_pointssmall");
			num2 += module.m_value;
			num3++;
		}
		for (int i = num3; i <= 12; i++)
		{
			GuiUtils.FindChildOf(go, "BluePrintArmsLabel" + i.ToString()).GetComponent<SpriteText>().Text = string.Empty;
			GuiUtils.FindChildOf(go, "BluePrintArmsCost" + i.ToString()).GetComponent<SpriteText>().Text = string.Empty;
		}
		GuiUtils.FindChildOf(go, "BlueprintArmsCost").GetComponent<SpriteText>().Text = Localize.instance.Translate("$shipedit_totalarmscost: " + num2.ToString() + " $label_pointssmall");
		int num4 = num + num2;
		GuiUtils.FindChildOf(go, "BlueprintTotalCostValue").GetComponent<SpriteText>().Text = Localize.instance.Translate(num4.ToString() + " $label_pointssmall");
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001454C File Offset: 0x0001274C
	private void OnHideluePrintInfo(IUIObject obj)
	{
		GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00014570 File Offset: 0x00012770
	private void OnDeleteBluePrint(IUIObject obj)
	{
		GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").GetComponent<UIPanel>().Dismiss();
		this.OpenQuestionDialog(string.Empty, Localize.instance.Translate("$shipedit_delete_blueprint") + " " + this.m_selectedBlueprint, new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DialogDeleteBlueprint));
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000145E0 File Offset: 0x000127E0
	private void DialogDeleteBlueprint(IUIObject obj)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		this.m_userManClient.RemoveShip(this.m_selectedBlueprint);
		this.SetupBluePrint();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0001460C File Offset: 0x0001280C
	private void OnHideClassInfo(IUIObject obj)
	{
		GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Class").GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00014630 File Offset: 0x00012830
	private void OnAddBlueprint(IUIObject obj)
	{
		GameObject gameObject = GuiUtils.FindChildOf(obj.transform, "BlueprintButtonLabel");
		SpriteText component = gameObject.GetComponent<SpriteText>();
		ShipDef bluePrintSipDef = this.GetBluePrintSipDef(component.Text);
		if (bluePrintSipDef == null)
		{
			return;
		}
		FleetShip fleetShip = new FleetShip();
		fleetShip.m_definition = bluePrintSipDef;
		this.m_fleetShips.Add(fleetShip);
		this.FleetShipsShow();
		this.SetFleetModified(true);
		this.RecalcFleetCost();
		if (this.m_fleetShips.Count == 8)
		{
			this.SetAddShipButtonStatus(false);
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x000146B0 File Offset: 0x000128B0
	private void OnAddShip(IUIObject obj)
	{
		GameObject gameObject = obj.transform.parent.gameObject;
		GameObject gameObject2 = GuiUtils.FindChildOf(obj.transform, "ClassButtonLabel");
		SimpleTag component = gameObject.GetComponent<SimpleTag>();
		SpriteText component2 = gameObject2.GetComponent<SpriteText>();
		ShipDef shipDef = this.GetShipDef(component.m_tag, component2.Text);
		if (shipDef == null)
		{
			return;
		}
		FleetShip fleetShip = new FleetShip();
		fleetShip.m_definition = shipDef;
		this.m_fleetShips.Add(fleetShip);
		this.FleetShipsShow();
		this.SetFleetModified(true);
		this.RecalcFleetCost();
		if (this.m_fleetShips.Count == 8)
		{
			this.SetAddShipButtonStatus(false);
		}
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x00014754 File Offset: 0x00012954
	private FleetShip GetFleetShip(IUIObject obj)
	{
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			if (fleetShip.m_floatingInfo == obj.transform.parent.gameObject)
			{
				return fleetShip;
			}
		}
		return null;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x000147E0 File Offset: 0x000129E0
	private void OnDeleteShip(IUIObject obj)
	{
		PLog.Log("OnDeleteShip");
		this.m_deleteShipInfo = this.GetFleetShip(obj);
		if (this.m_deleteShipInfo == null)
		{
			return;
		}
		GuiUtils.FindChildOf(this.m_gui.transform, "InfoDialog_Blueprint").GetComponent<UIPanel>().Dismiss();
		this.OpenQuestionDialog(string.Empty, Localize.instance.Translate("$FloatingInfoRemoveButton_tooltip") + " " + this.m_deleteShipInfo.m_definition.m_name + "?", new EZValueChangedDelegate(this.DialogDeleteCancelPressed), new EZValueChangedDelegate(this.DialogDeleteShip));
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x00014880 File Offset: 0x00012A80
	private void DialogDeleteShip(IUIObject obj)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		if (this.m_deleteShipInfo == null)
		{
			return;
		}
		this.m_deleteShipInfo.Destroy();
		this.m_fleetShips.Remove(this.m_deleteShipInfo);
		this.FleetShipsShow();
		this.SetFleetModified(true);
		this.RecalcFleetCost();
		this.SetAddShipButtonStatus(true);
		if (this.m_fleetShips.Count == 0)
		{
			this.SetFleetModified(false);
		}
		this.m_deleteShipInfo = null;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00014900 File Offset: 0x00012B00
	private void OnCloneShip(IUIObject obj)
	{
		if (this.m_fleetShips.Count == 8)
		{
			return;
		}
		foreach (FleetShip fleetShip in this.m_fleetShips)
		{
			if (fleetShip.m_floatingInfo == obj.transform.parent.gameObject)
			{
				FleetShip fleetShip2 = new FleetShip();
				fleetShip2.m_definition = fleetShip.m_definition.Clone();
				this.m_fleetShips.Add(fleetShip2);
				this.FleetShipsShow();
				this.SetFleetModified(true);
				this.RecalcFleetCost();
				if (this.m_fleetShips.Count == 8)
				{
					this.SetAddShipButtonStatus(false);
				}
				break;
			}
		}
	}

	// Token: 0x060002BB RID: 699 RVA: 0x000149E4 File Offset: 0x00012BE4
	private void MoveShip(int shipIndex, bool left)
	{
		int num;
		if (left)
		{
			num = shipIndex - 1;
			if (num < 0)
			{
				num = this.m_fleetShips.Count - 1;
			}
		}
		else
		{
			num = shipIndex + 1;
			if (num > this.m_fleetShips.Count - 1)
			{
				num = 0;
			}
		}
		FleetShip value = this.m_fleetShips[num];
		this.m_fleetShips[num] = this.m_fleetShips[shipIndex];
		this.m_fleetShips[shipIndex] = value;
		this.FleetShipsShow();
		this.SetFleetModified(true);
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00014A70 File Offset: 0x00012C70
	private void OnMoveShipLeft(IUIObject obj)
	{
		int shipIndex = this.m_fleetShips.FindIndex((FleetShip s) => s.m_floatingInfo == obj.transform.parent.gameObject);
		this.MoveShip(shipIndex, true);
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00014AAC File Offset: 0x00012CAC
	private void OnMoveShipRight(IUIObject obj)
	{
		int shipIndex = this.m_fleetShips.FindIndex((FleetShip s) => s.m_floatingInfo == obj.transform.parent.gameObject);
		this.MoveShip(shipIndex, false);
	}

	// Token: 0x060002BE RID: 702 RVA: 0x00014AE8 File Offset: 0x00012CE8
	public DefaultSections GetDefaultSections(string shipSeries)
	{
		List<string> availableSections = this.m_userManClient.GetAvailableSections(this.m_campaignID);
		DefaultSections defaultSections = new DefaultSections();
		if (availableSections == null)
		{
			PLog.LogError("Ship of series " + shipSeries + " has no sections listed.");
			return defaultSections;
		}
		foreach (string text in availableSections)
		{
			GameObject prefab = ObjectFactory.instance.GetPrefab(text);
			if (prefab != null)
			{
				Section component = prefab.GetComponent<Section>();
				if (shipSeries == component.m_series && component.m_defaultSection)
				{
					if (component.m_type == Section.SectionType.Front)
					{
						defaultSections.m_front = text;
					}
					if (component.m_type == Section.SectionType.Mid)
					{
						defaultSections.m_mid = text;
					}
					if (component.m_type == Section.SectionType.Rear)
					{
						defaultSections.m_rear = text;
					}
					if (component.m_type == Section.SectionType.Top)
					{
						defaultSections.m_top = text;
					}
				}
			}
		}
		return defaultSections;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00014C08 File Offset: 0x00012E08
	private ShipDef GetShipDef(string shipSeries, string name)
	{
		DefaultSections defaultSections = this.GetDefaultSections(shipSeries);
		if (!defaultSections.IsValid())
		{
			PLog.LogError(shipSeries + " is missing default sections: " + defaultSections.ErrorMessage());
			return null;
		}
		ShipDef shipDef = new ShipDef();
		shipDef.m_name = name;
		shipDef.m_prefab = shipSeries;
		shipDef.m_frontSection = new SectionDef();
		shipDef.m_frontSection.m_prefab = defaultSections.m_front;
		shipDef.m_midSection = new SectionDef();
		shipDef.m_midSection.m_prefab = defaultSections.m_mid;
		shipDef.m_rearSection = new SectionDef();
		shipDef.m_rearSection.m_prefab = defaultSections.m_rear;
		shipDef.m_topSection = new SectionDef();
		shipDef.m_topSection.m_prefab = defaultSections.m_top;
		shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance);
		return shipDef;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00014CD8 File Offset: 0x00012ED8
	private ShipDef GetBluePrintSipDef(string name)
	{
		foreach (ShipDef shipDef in this.m_bluePrints)
		{
			if (shipDef.m_name == name)
			{
				return shipDef.Clone();
			}
		}
		return null;
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00014D58 File Offset: 0x00012F58
	private void SetFleetName(string name)
	{
		this.lblFleetName.GetComponent<UITextField>().Text = name;
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00014D6C File Offset: 0x00012F6C
	private void SetFleetCostLabel(string value)
	{
		this.lblPoints.GetComponent<SpriteText>().Text = string.Concat(new string[]
		{
			value,
			"/",
			this.m_fleetSize.max.ToString(),
			" ",
			Localize.instance.Translate(" $label_pointssmall")
		});
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00014DD0 File Offset: 0x00012FD0
	private void SetFleetModified(bool isModified)
	{
		this.m_fleetModified = isModified;
		GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetButton").GetComponent<UIButton>().controlIsEnabled = this.m_fleetModified;
		if (this.m_oneFleetOnly)
		{
			return;
		}
		if (this.m_fleetShips.Count == 0)
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetAsButton").GetComponent<UIButton>().controlIsEnabled = false;
		}
		else
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "SaveFleetAsButton").GetComponent<UIButton>().controlIsEnabled = true;
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00014E6C File Offset: 0x0001306C
	private void SetupCamera()
	{
		GameObject gameObject = GameObject.Find("Center");
		gameObject.transform.localPosition = new Vector3(88f, 0f, 0f);
		Quaternion rotation = default(Quaternion);
		rotation.eulerAngles = new Vector3(90f, 0f, 0f);
		this.m_sceneCamera.transform.position = gameObject.transform.position + new Vector3(0f, 100f, -50f);
		this.m_sceneCamera.transform.rotation = rotation;
		this.m_cameraCurrentPosition = this.m_sceneCamera.transform.position;
		this.m_sceneCamera.GetComponent<Camera>().orthographic = true;
		this.m_sceneCamera.GetComponent<Camera>().orthographicSize = 80f;
		this.m_cameraGoalPosition = (this.m_cameraCurrentPosition = this.m_sceneCamera.transform.position);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00014F68 File Offset: 0x00013168
	private void SetView(Vector3 pos, float size)
	{
		this.m_cameraGoalPosition = pos;
		this.m_cameraGoalSize = size;
		this.m_cameraPositionSpeed = (this.m_cameraGoalPosition - this.m_cameraCurrentPosition).magnitude / 0.5f;
		float num = this.m_cameraGoalSize - this.m_cameraCurrentSize;
		this.m_cameraSizeSpeed = num / 0.5f;
	}

	// Token: 0x040001CC RID: 460
	private static MsgBox m_okToOverideOnSaveDialog;

	// Token: 0x040001CD RID: 461
	public string m_selectedFleet = string.Empty;

	// Token: 0x040001CE RID: 462
	public List<string> m_fleets = new List<string>();

	// Token: 0x040001CF RID: 463
	private FleetShip m_deleteShipInfo;

	// Token: 0x040001D0 RID: 464
	public FleetMenu.OnExitDelegate m_onExit;

	// Token: 0x040001D1 RID: 465
	private Vector3 m_cameraCurrentPosition = default(Vector3);

	// Token: 0x040001D2 RID: 466
	private float m_cameraCurrentSize = 70f;

	// Token: 0x040001D3 RID: 467
	private Vector3 m_cameraGoalPosition = default(Vector3);

	// Token: 0x040001D4 RID: 468
	private float m_cameraGoalSize = 70f;

	// Token: 0x040001D5 RID: 469
	private float m_cameraPositionSpeed = 1f;

	// Token: 0x040001D6 RID: 470
	private float m_cameraSizeSpeed = 1f;

	// Token: 0x040001D7 RID: 471
	private GameObject m_gui;

	// Token: 0x040001D8 RID: 472
	private GameObject m_guiCamera;

	// Token: 0x040001D9 RID: 473
	private UserManClient m_userManClient;

	// Token: 0x040001DA RID: 474
	private int m_campaignID;

	// Token: 0x040001DB RID: 475
	private float m_shipyardTime;

	// Token: 0x040001DC RID: 476
	private MsgBox m_msgBox;

	// Token: 0x040001DD RID: 477
	private GameObject m_saveDialog;

	// Token: 0x040001DE RID: 478
	private ShipMenu m_shipMenu;

	// Token: 0x040001DF RID: 479
	private ShipDef m_shipToEdit;

	// Token: 0x040001E0 RID: 480
	private GameObject lblFleetName;

	// Token: 0x040001E1 RID: 481
	private bool m_fleetModified;

	// Token: 0x040001E2 RID: 482
	private bool m_onSaveExit;

	// Token: 0x040001E3 RID: 483
	private string m_fleetName;

	// Token: 0x040001E4 RID: 484
	private string m_originalFleetName;

	// Token: 0x040001E5 RID: 485
	private bool m_removeOrginalFleet;

	// Token: 0x040001E6 RID: 486
	private int m_fleetCost;

	// Token: 0x040001E7 RID: 487
	private int m_setFleetCost = -1;

	// Token: 0x040001E8 RID: 488
	private GameObject m_sceneCamera;

	// Token: 0x040001E9 RID: 489
	private UIPanel m_ShipBrowserPanel;

	// Token: 0x040001EA RID: 490
	private UIPanel m_FleetTopPanel;

	// Token: 0x040001EB RID: 491
	private UIPanel m_ShipNamePanel;

	// Token: 0x040001EC RID: 492
	private UIPanel m_InfoBlueprint;

	// Token: 0x040001ED RID: 493
	private UIPanel m_InfoClass;

	// Token: 0x040001EE RID: 494
	private UIPanel m_openFleetDlg;

	// Token: 0x040001EF RID: 495
	private GameObject lblPoints;

	// Token: 0x040001F0 RID: 496
	private List<FleetShip> m_fleetShips = new List<FleetShip>();

	// Token: 0x040001F1 RID: 497
	private FleetShip m_editShip;

	// Token: 0x040001F2 RID: 498
	private float m_xOffset;

	// Token: 0x040001F3 RID: 499
	private List<ShipDef> m_bluePrints = new List<ShipDef>();

	// Token: 0x040001F4 RID: 500
	private string m_selectedBlueprint;

	// Token: 0x040001F5 RID: 501
	private FleetSize m_fleetSize;

	// Token: 0x040001F6 RID: 502
	private bool m_oneFleetOnly;

	// Token: 0x040001F7 RID: 503
	private MusicManager m_musicMan;

	// Token: 0x040001F8 RID: 504
	private bool m_clearFleet;

	// Token: 0x040001F9 RID: 505
	private bool m_afterSaveOpenFleet;

	// Token: 0x020001A3 RID: 419
	// (Invoke) Token: 0x06000F3C RID: 3900
	public delegate void OnExitDelegate();
}
