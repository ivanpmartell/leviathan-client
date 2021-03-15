using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class ChooseFleetMenu
{
	// Token: 0x06000186 RID: 390 RVA: 0x00009B18 File Offset: 0x00007D18
	public ChooseFleetMenu(GameObject guiCamera, UserManClient userManClient, FleetSizeClass fleetClass, FleetSize fleetSize, string fleetType, int campaignID)
	{
		this.m_guiCamera = guiCamera;
		this.m_fleetSizeClass = fleetClass;
		this.m_fleetSizeLimit = fleetSize;
		this.m_campaignID = campaignID;
		DebugUtils.Assert(guiCamera != null, "ChooseFleetMenu ctor called with NULL camera !");
		this.m_gui = GuiUtils.CreateGui("ChooseFleetDialog", guiCamera);
		DebugUtils.Assert(this.m_gui != null, "ChooseFleetMenu failed to validate root object m_gui !");
		this.m_userManClient = userManClient;
		if (this.m_userManClient == null)
		{
			Debug.LogWarning("ChooseFleetMenu ctor called with NULL UserManClient, will not be able to fill data !");
		}
		this.InitializeAndValidate();
		this.InfoFiller.Initialize();
		this.FillData();
		this.RegisterDelegatesToComponents();
		this.btnProceed.controlIsEnabled = false;
		if (fleetClass != FleetSizeClass.None)
		{
			this.SetTitle(Localize.instance.Translate(string.Concat(new string[]
			{
				"$choosefleet_library_fleet ",
				fleetClass.ToString(),
				" (",
				fleetSize.min.ToString(),
				"-",
				fleetSize.max.ToString(),
				")"
			})), true);
			this.btnProceed.GetComponent<UIButton>().Text = "OK";
		}
		else
		{
			this.SetTitle(Localize.instance.Translate("$choosefleet_library_fleet"), true);
		}
		this.btnDelete.GetComponent<UIButton>().controlIsEnabled = false;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Combine(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00009C98 File Offset: 0x00007E98
	public void Close()
	{
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		this.UnRegisterDelegatesFromComponents();
		UnityEngine.Object.Destroy(this.m_gui);
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00009CE0 File Offset: 0x00007EE0
	private void Exit()
	{
		if (this.m_onExit != null)
		{
			this.m_onExit();
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00009CF8 File Offset: 0x00007EF8
	public void SetTitle(string title, bool toUpper)
	{
		string text = title.Trim();
		if (!string.IsNullOrEmpty(text) && toUpper)
		{
			text = text.ToUpper();
		}
		this.Label_Title.GetComponent<SpriteText>().Text = text;
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00009D38 File Offset: 0x00007F38
	private void OnUserManUpdate()
	{
		this.FillData();
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00009D40 File Offset: 0x00007F40
	private void RegisterDelegatesToComponents()
	{
		this.btnProceed.GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnProceedPressed));
		this.btnNewFleet.GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNewFleetPressed));
		this.btnCancel.GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnCancelPressed));
		this.btnDelete.GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnDeletePressed));
		ListBox_FleetInfo_Filler infoFiller = this.InfoFiller;
		infoFiller.m_onFleetChangedDelegate = (ListBox_FleetInfo_Filler.OnFleetChanged)Delegate.Combine(infoFiller.m_onFleetChangedDelegate, new ListBox_FleetInfo_Filler.OnFleetChanged(this.SelectedFleetChanged));
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00009DE4 File Offset: 0x00007FE4
	private void UnRegisterDelegatesFromComponents()
	{
		this.Button_Proceed.GetComponent<UIButton>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnProceedPressed));
		this.Button_NewFleet.GetComponent<UIButton>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnNewFleetPressed));
		this.Button_Cancel.GetComponent<UIButton>().RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnCancelPressed));
		ListBox_FleetInfo_Filler infoFiller = this.InfoFiller;
		infoFiller.m_onFleetChangedDelegate = (ListBox_FleetInfo_Filler.OnFleetChanged)Delegate.Remove(infoFiller.m_onFleetChangedDelegate, new ListBox_FleetInfo_Filler.OnFleetChanged(this.SelectedFleetChanged));
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00009E6C File Offset: 0x0000806C
	private void FillData()
	{
		List<ShipDef> shipDefs = this.m_userManClient.GetShipDefs(this.m_campaignID);
		List<FleetDef> fleetDefs = this.m_userManClient.GetFleetDefs(this.m_campaignID);
		this.InfoFiller.Clear();
		int num = int.MaxValue;
		foreach (FleetDef fleetDef in fleetDefs)
		{
			int value = fleetDef.m_value;
			if (value < num)
			{
				num = value;
			}
			this.InfoFiller.AddItem(fleetDef.m_name, value.ToString(), "2011/01/12");
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00009F4C File Offset: 0x0000814C
	private void ForceCreateNewFleet()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
		}
		if (this.m_onProceed == null)
		{
			PLog.LogWarning("~~~ NOONE is listening to ONPROCEED !");
		}
		if (this.m_onProceed != null)
		{
			this.m_onProceed(string.Empty, this.m_campaignID);
		}
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00009FA8 File Offset: 0x000081A8
	private void DontForceCreateNewFleet()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
		}
		if (this.m_onExit != null)
		{
			this.m_onExit();
		}
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00009FE4 File Offset: 0x000081E4
	public string GetSelectedItem()
	{
		return this.InfoFiller.GetSelectedItemsName();
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00009FF4 File Offset: 0x000081F4
	public void Hide()
	{
		this.m_gui.SetActiveRecursively(false);
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000A02C File Offset: 0x0000822C
	public void Show()
	{
		this.InfoFiller.enabled = true;
		this.InfoFiller.UnSelect();
		this.FillData();
		this.btnProceed.controlIsEnabled = false;
		this.btnDelete.controlIsEnabled = false;
		this.m_gui.SetActiveRecursively(true);
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		UserManClient userManClient2 = this.m_userManClient;
		userManClient2.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Combine(userManClient2.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000A0C8 File Offset: 0x000082C8
	private void OnProceedPressed(IUIObject obj)
	{
		if (this.m_onProceed == null)
		{
			return;
		}
		string selectedItem = this.GetSelectedItem();
		if (string.IsNullOrEmpty(selectedItem))
		{
			return;
		}
		this.m_onProceed(selectedItem, this.m_campaignID);
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000A108 File Offset: 0x00008308
	private void OnNewFleetPressed(IUIObject obj)
	{
		if (this.m_onProceed == null)
		{
			return;
		}
		this.m_onProceed(string.Empty, this.m_campaignID);
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000A138 File Offset: 0x00008338
	private void OnCancelPressed(IUIObject obj)
	{
		this.Exit();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000A140 File Offset: 0x00008340
	private void OnDeletePressed(IUIObject obj)
	{
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "Are you sure you want to delete the selected fleet", new MsgBox.YesHandler(this.OnConfirmDelete), null);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000A168 File Offset: 0x00008368
	private void OnConfirmDelete()
	{
		string selectedItem = this.GetSelectedItem();
		if (string.IsNullOrEmpty(selectedItem))
		{
			Debug.Log("No item selected to delete, this should never happen since the button should be disabled !");
			return;
		}
		this.InfoFiller.RemoveFleet(selectedItem);
		this.m_userManClient.RemoveFleet(selectedItem);
		this.SelectedFleetChanged(string.Empty);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000A1B8 File Offset: 0x000083B8
	private void SelectedFleetChanged(string fleetName)
	{
		if (string.IsNullOrEmpty(fleetName))
		{
			this.btnProceed.controlIsEnabled = false;
			this.btnDelete.controlIsEnabled = false;
		}
		else
		{
			this.btnProceed.controlIsEnabled = true;
			this.btnDelete.controlIsEnabled = true;
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000A208 File Offset: 0x00008408
	private void OnDonePressed(IUIObject obj)
	{
		this.Exit();
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000A210 File Offset: 0x00008410
	public void InitializeAndValidate()
	{
		if (this.IsValid)
		{
			return;
		}
		DebugUtils.Assert(this.Validate_lblTitle(), "ChooseFleetMenu failed to validate label named lblTitle !");
		DebugUtils.Assert(this.Validate_ListContainer(), "ChooseFleetMenu failed to validate transform named ListContainer !");
		DebugUtils.Assert(this.Validate_lstFleetInfos(), "ChooseFleetMenu failed to validate list named lstFleetInfos !");
		DebugUtils.Assert(this.Validate_fleetInfoFiller(), "ChooseFleetMenu failed to validate ListBox_FleetInfo_Filler-script on the lstFleetInfo-object !");
		DebugUtils.Assert(this.Validate_buttonsPanel(), "ChooseFleetMenu failed to validate panel named ButtonsPanel !");
		DebugUtils.Assert(this.Validate_btnProceed(), "ChooseFleetMenu failed to validate button named btnEdit !");
		DebugUtils.Assert(this.Validate_btnNewFleet(), "ChooseFleetMenu failed to validate button named btnNewFleet !");
		DebugUtils.Assert(this.Validate_btnCancel(), "ChooseFleetMenu failed to validate button named btnCancel !");
		DebugUtils.Assert(this.Validate_btnDelete(), "ChooseFleetMenu failed to validate button named btnDelete !");
		this.IsValid = true;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000A2C0 File Offset: 0x000084C0
	private bool Validate_buttonsPanel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ButtonsPanel", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.panelButtons = gameObject.GetComponent<UIPanel>();
		return this.panelButtons != null;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000A308 File Offset: 0x00008508
	private bool Validate_lblTitle()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/lblTitle", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.lblTitle = gameObject.GetComponent<SpriteText>();
		return this.lblTitle != null;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000A350 File Offset: 0x00008550
	private bool Validate_ListContainer()
	{
		this.ListContainer = this.m_gui.transform.FindChild("dialog_bg/ListContainer");
		return this.ListContainer != null;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000A384 File Offset: 0x00008584
	private bool Validate_lstFleetInfos()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ListContainer/lstFleetInfos", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.lstFleetInfos = gameObject.GetComponent<UIScrollList>();
		return this.lstFleetInfos != null;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000A3CC File Offset: 0x000085CC
	private bool Validate_btnProceed()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ButtonsPanel/btnEdit", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnProceed = gameObject.GetComponent<UIButton>();
		return this.btnProceed != null;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000A414 File Offset: 0x00008614
	private bool Validate_btnNewFleet()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ButtonsPanel/btnNewFleet", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnNewFleet = gameObject.GetComponent<UIButton>();
		return this.btnNewFleet != null;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000A45C File Offset: 0x0000865C
	private bool Validate_btnCancel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ButtonsPanel/btnCancel", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnCancel = gameObject.GetComponent<UIButton>();
		return this.btnCancel != null;
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000A4A4 File Offset: 0x000086A4
	private bool Validate_btnDelete()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/ButtonsPanel/btnDelete", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnDelete = gameObject.GetComponent<UIButton>();
		return this.btnDelete != null;
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000A4EC File Offset: 0x000086EC
	private bool Validate_fleetInfoFiller()
	{
		this.InfoFiller = this.List_FleetInfos.GetComponent<ListBox_FleetInfo_Filler>();
		return this.InfoFiller != null;
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000A518 File Offset: 0x00008718
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

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000A554 File Offset: 0x00008754
	// (set) Token: 0x060001A6 RID: 422 RVA: 0x0000A55C File Offset: 0x0000875C
	public Transform ListContainer
	{
		get
		{
			return this.listContainer;
		}
		private set
		{
			this.listContainer = value;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000A568 File Offset: 0x00008768
	// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000A570 File Offset: 0x00008770
	public ListBox_FleetInfo_Filler InfoFiller
	{
		get
		{
			return this.fleetInfoFiller;
		}
		private set
		{
			this.fleetInfoFiller = value;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x060001A9 RID: 425 RVA: 0x0000A57C File Offset: 0x0000877C
	// (set) Token: 0x060001AA RID: 426 RVA: 0x0000A584 File Offset: 0x00008784
	public UIPanel Panel_Buttons
	{
		get
		{
			return this.panelButtons;
		}
		private set
		{
			this.panelButtons = value;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x060001AB RID: 427 RVA: 0x0000A590 File Offset: 0x00008790
	// (set) Token: 0x060001AC RID: 428 RVA: 0x0000A598 File Offset: 0x00008798
	public UIScrollList List_FleetInfos
	{
		get
		{
			return this.lstFleetInfos;
		}
		private set
		{
			this.lstFleetInfos = value;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x060001AD RID: 429 RVA: 0x0000A5A4 File Offset: 0x000087A4
	// (set) Token: 0x060001AE RID: 430 RVA: 0x0000A5AC File Offset: 0x000087AC
	public UIButton Button_Proceed
	{
		get
		{
			return this.btnProceed;
		}
		private set
		{
			this.btnProceed = value;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x060001AF RID: 431 RVA: 0x0000A5B8 File Offset: 0x000087B8
	// (set) Token: 0x060001B0 RID: 432 RVA: 0x0000A5C0 File Offset: 0x000087C0
	public UIButton Button_NewFleet
	{
		get
		{
			return this.btnNewFleet;
		}
		private set
		{
			this.btnNewFleet = value;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000A5CC File Offset: 0x000087CC
	// (set) Token: 0x060001B2 RID: 434 RVA: 0x0000A5D4 File Offset: 0x000087D4
	public UIButton Button_Cancel
	{
		get
		{
			return this.btnCancel;
		}
		private set
		{
			this.btnCancel = value;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060001B3 RID: 435 RVA: 0x0000A5E0 File Offset: 0x000087E0
	// (set) Token: 0x060001B4 RID: 436 RVA: 0x0000A5E8 File Offset: 0x000087E8
	public UIButton Button_Delete
	{
		get
		{
			return this.btnDelete;
		}
		private set
		{
			this.btnDelete = value;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x060001B5 RID: 437 RVA: 0x0000A5F4 File Offset: 0x000087F4
	// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000A5FC File Offset: 0x000087FC
	public SpriteText Label_Title
	{
		get
		{
			return this.lblTitle;
		}
		private set
		{
			this.lblTitle = value;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000A608 File Offset: 0x00008808
	// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000A610 File Offset: 0x00008810
	public bool IsValid
	{
		get
		{
			return this.isValid;
		}
		private set
		{
			this.isValid = value;
		}
	}

	// Token: 0x040000FF RID: 255
	public ChooseFleetMenu.OnExitDelegate m_onExit;

	// Token: 0x04000100 RID: 256
	public Action<string, int> m_onProceed;

	// Token: 0x04000101 RID: 257
	private GameObject m_gui;

	// Token: 0x04000102 RID: 258
	private bool isValid;

	// Token: 0x04000103 RID: 259
	private UserManClient m_userManClient;

	// Token: 0x04000104 RID: 260
	private FleetSizeClass m_fleetSizeClass;

	// Token: 0x04000105 RID: 261
	private FleetSize m_fleetSizeLimit;

	// Token: 0x04000106 RID: 262
	private int m_campaignID;

	// Token: 0x04000107 RID: 263
	private GameObject m_guiCamera;

	// Token: 0x04000108 RID: 264
	private MsgBox m_msgBox;

	// Token: 0x04000109 RID: 265
	private Transform listContainer;

	// Token: 0x0400010A RID: 266
	private UIScrollList lstFleetInfos;

	// Token: 0x0400010B RID: 267
	private UIButton btnProceed;

	// Token: 0x0400010C RID: 268
	private UIButton btnNewFleet;

	// Token: 0x0400010D RID: 269
	private UIButton btnCancel;

	// Token: 0x0400010E RID: 270
	private UIButton btnDelete;

	// Token: 0x0400010F RID: 271
	private UIPanel panelButtons;

	// Token: 0x04000110 RID: 272
	private SpriteText lblTitle;

	// Token: 0x04000111 RID: 273
	private ListBox_FleetInfo_Filler fleetInfoFiller;

	// Token: 0x0200019B RID: 411
	// (Invoke) Token: 0x06000F1C RID: 3868
	public delegate void OnExitDelegate();
}
