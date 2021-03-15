using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class CreateGame
{
	// Token: 0x060001ED RID: 493 RVA: 0x0000C0D8 File Offset: 0x0000A2D8
	public CreateGame(GameObject guiCamera, GameObject createGameView, MapMan mapman, UserManClient userMan)
	{
		this.m_guiCamera = guiCamera;
		this.m_mapMan = mapman;
		this.m_userMan = userMan;
		this.m_createGameView = createGameView;
		GuiUtils.FindChildOf(this.m_createGameView, "CancelButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateGameCancel));
		this.m_mapDescriptionLabel = GuiUtils.FindChildOf(this.m_createGameView, "MapDescLbl").GetComponent<SpriteText>();
		this.m_mapSizeLabel = GuiUtils.FindChildOf(this.m_createGameView, "MapSizeLbl").GetComponent<SpriteText>();
		this.m_mapNameLabel = GuiUtils.FindChildOf(this.m_createGameView, "MapNameLbl").GetComponent<SpriteText>();
		this.m_mapImage = GuiUtils.FindChildOf(this.m_createGameView, "MapImg").GetComponent<SimpleSprite>();
		this.m_skirmishTabButton = GuiUtils.FindChildOf(this.m_createGameView, "SkirmishTabButton").GetComponent<UIButton>();
		this.m_campaignTabButton = GuiUtils.FindChildOf(this.m_createGameView, "CampaignTabButton").GetComponent<UIButton>();
		this.m_challengeTabButton = GuiUtils.FindChildOf(this.m_createGameView, "ChallengeTabButton").GetComponent<UIButton>();
		this.m_assTabButton = GuiUtils.FindChildOf(this.m_createGameView, "AssTabButton").GetComponent<UIButton>();
		this.m_skirmishTabButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateSkirmishTab));
		this.m_campaignTabButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateCampaignTab));
		this.m_challengeTabButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateChallengeTab));
		this.m_assTabButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateAssTab));
		this.m_createSkirmishTab = GuiUtils.FindChildOf(this.m_createGameView, "Settings_Points");
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "CreateButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateSkirmishGame));
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "MapmenuButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowSkirmishMapMenu));
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "ScrollList").GetComponent<UIScrollList>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishMapSelected));
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "PlusButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPlusTargetScore));
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "MinusButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMinusTargetScore));
		this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishFleetSizeChange));
		this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishFleetSizeChange));
		this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishFleetSizeChange));
		this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishPlayersChange));
		this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishPlayersChange));
		this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSkirmishPlayersChange));
		this.m_createCampaignTab = GuiUtils.FindChildOf(this.m_createGameView, "Settings_Campaign");
		GuiUtils.FindChildOf(this.m_createCampaignTab, "CreateButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateCampaignGame));
		GuiUtils.FindChildOf(this.m_createCampaignTab, "MapmenuButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowCampaignMapMenu));
		GuiUtils.FindChildOf(this.m_createCampaignTab, "ArrowLeftButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPrevCampaign));
		GuiUtils.FindChildOf(this.m_createCampaignTab, "ArrowRightButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnNextCampaign));
		GuiUtils.FindChildOf(this.m_createCampaignTab, "ScrollList").GetComponent<UIScrollList>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCampaignMapSelected));
		this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/One").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCampaignPlayersChange));
		this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCampaignPlayersChange));
		this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCampaignPlayersChange));
		this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCampaignPlayersChange));
		this.m_createChallengeTab = GuiUtils.FindChildOf(this.m_createGameView, "Settings_Challenge");
		GuiUtils.FindChildOf(this.m_createChallengeTab, "CreateButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateChallengeGame));
		GuiUtils.FindChildOf(this.m_createChallengeTab, "MapmenuButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowChallengeMapMenu));
		GuiUtils.FindChildOf(this.m_createChallengeTab, "ScrollList").GetComponent<UIScrollList>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnChallengeMapSelected));
		this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/One").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnChallengePlayersChange));
		this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnChallengePlayersChange));
		this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnChallengePlayersChange));
		this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnChallengePlayersChange));
		this.m_createAssTab = GuiUtils.FindChildOf(this.m_createGameView, "Settings_Ass");
		GuiUtils.FindChildOf(this.m_createAssTab, "CreateButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateAssassinateGame));
		GuiUtils.FindChildOf(this.m_createAssTab, "MapmenuButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowAssassinationMapMenu));
		GuiUtils.FindChildOf(this.m_createAssTab, "ScrollList").GetComponent<UIScrollList>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAssassinationMapSelected));
		this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAssassinatePlayersChange));
		this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAssassinatePlayersChange));
		this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAssassinatePlayersChange));
		this.m_createGameView.SetActiveRecursively(false);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000C834 File Offset: 0x0000AA34
	public void Show()
	{
		this.m_hasCreated = false;
		this.m_createGameView.SetActiveRecursively(true);
		this.ShowCreateCampaignGame();
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000C850 File Offset: 0x0000AA50
	public void Hide()
	{
		this.m_createGameView.SetActiveRecursively(false);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000C860 File Offset: 0x0000AA60
	private void OnShowSkirmishMapMenu(IUIObject obj)
	{
		PLog.Log("show mapmenu");
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createSkirmishTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (component.IsShowing)
		{
			component.Dismiss();
		}
		else
		{
			component.BringIn();
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000C8B0 File Offset: 0x0000AAB0
	private void OnShowCampaignMapMenu(IUIObject obj)
	{
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createCampaignTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (component.IsShowing)
		{
			component.Dismiss();
		}
		else
		{
			component.BringIn();
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000C8F4 File Offset: 0x0000AAF4
	private void OnShowChallengeMapMenu(IUIObject obj)
	{
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createChallengeTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (component.IsShowing)
		{
			component.Dismiss();
		}
		else
		{
			component.BringIn();
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000C938 File Offset: 0x0000AB38
	private void OnShowAssassinationMapMenu(IUIObject obj)
	{
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createAssTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (component.IsShowing)
		{
			component.Dismiss();
		}
		else
		{
			component.BringIn();
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000C97C File Offset: 0x0000AB7C
	private void OnPlusTargetScore(IUIObject obj)
	{
		this.m_targetScore += 10;
		if (this.m_targetScore > 100)
		{
			this.m_targetScore = 100;
		}
		this.UpdateTargetScoreWidgets();
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000C9B4 File Offset: 0x0000ABB4
	private void OnSkirmishFleetSizeChange(IUIObject obj)
	{
		this.UpdateTargetScoreWidgets();
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000C9BC File Offset: 0x0000ABBC
	private void OnSkirmishPlayersChange(IUIObject obj)
	{
		int num = 1;
		if (obj.name == "Two")
		{
			num = 2;
		}
		if (obj.name == "Three")
		{
			num = 3;
		}
		if (obj.name == "Four")
		{
			num = 4;
		}
		if (num == 3)
		{
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().controlIsEnabled = false;
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().controlIsEnabled = false;
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().Value = true;
			this.UpdateTargetScoreWidgets();
		}
		else
		{
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().controlIsEnabled = true;
		}
		this.UpdateTargetScoreWidgets();
		this.SetupTurnTimers(this.m_createSkirmishTab, num);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000CB18 File Offset: 0x0000AD18
	private void OnCampaignPlayersChange(IUIObject obj)
	{
		this.SetupTurnTimers(this.m_createCampaignTab, this.GetCampaignPlayers());
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000CB2C File Offset: 0x0000AD2C
	private void OnChallengePlayersChange(IUIObject obj)
	{
		this.SetupTurnTimers(this.m_createChallengeTab, this.GetChallengePlayers());
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000CB40 File Offset: 0x0000AD40
	private void OnAssassinatePlayersChange(IUIObject obj)
	{
		int num = 1;
		if (obj.name == "Two")
		{
			num = 2;
		}
		if (obj.name == "Three")
		{
			num = 3;
		}
		if (obj.name == "Four")
		{
			num = 4;
		}
		if (num == 3)
		{
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().controlIsEnabled = false;
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().controlIsEnabled = false;
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().Value = true;
		}
		else
		{
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().controlIsEnabled = true;
			this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().controlIsEnabled = true;
		}
		this.SetupTurnTimers(this.m_createAssTab, num);
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000CC90 File Offset: 0x0000AE90
	private void OnMinusTargetScore(IUIObject obj)
	{
		int num = 10;
		this.m_targetScore -= 10;
		if (this.m_targetScore < num)
		{
			this.m_targetScore = num;
		}
		this.UpdateTargetScoreWidgets();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000CCC8 File Offset: 0x0000AEC8
	private void UpdateTargetScoreWidgets()
	{
		int num = 10;
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "MinusButton").GetComponent<UIButton>().controlIsEnabled = (this.m_targetScore > num);
		GuiUtils.FindChildOf(this.m_createSkirmishTab, "PlusButton").GetComponent<UIButton>().controlIsEnabled = (this.m_targetScore < 100);
		int num2 = (int)((float)FleetSizes.sizes[(int)this.GetSkirmishFleetSize()].max * ((float)this.m_targetScore / 100f));
		int skirmishGamePlayers = this.GetSkirmishGamePlayers();
		if (skirmishGamePlayers >= 3)
		{
			num2 *= 2;
		}
		GuiUtils.FindChildOf(this.m_createSkirmishTab.gameObject, "Points").GetComponent<SpriteText>().Text = this.m_targetScore.ToString() + "%";
		GuiUtils.FindChildOf(this.m_createSkirmishTab.gameObject, "PointsConversion").GetComponent<SpriteText>().Text = num2.ToString();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000CDB0 File Offset: 0x0000AFB0
	private void OnSkirmishMapSelected(IUIObject obj)
	{
		UIScrollList uiscrollList = obj as UIScrollList;
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createSkirmishTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (uiscrollList.SelectedItem != null)
		{
			GuiUtils.FindChildOf(this.m_createSkirmishTab, "MapmenuButton").GetComponent<UIButton>().Text = uiscrollList.SelectedItem.Text;
			MapInfo mapByName = this.m_mapMan.GetMapByName(GameType.Points, string.Empty, this.m_mapList[uiscrollList.SelectedItem.Index]);
			this.SetMap(mapByName);
			this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 2);
			this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 3);
			this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 4);
			this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value = true;
		}
		component.Dismiss();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000CEE8 File Offset: 0x0000B0E8
	private void OnCampaignMapSelected(IUIObject obj)
	{
		UIScrollList uiscrollList = obj as UIScrollList;
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createCampaignTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (uiscrollList.SelectedItem != null)
		{
			GuiUtils.FindChildOf(this.m_createCampaignTab, "MapmenuButton").GetComponent<UIButton>().Text = uiscrollList.SelectedItem.Text;
			MapInfo mapByName = this.m_mapMan.GetMapByName(GameType.Campaign, this.m_availableCampaigns[this.m_selectedCampaignID].m_name, this.m_mapList[uiscrollList.SelectedItem.Index]);
			DebugUtils.Assert(mapByName != null);
			this.SetMap(mapByName);
			this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 2);
			this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 3);
			this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 4);
			this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/One").GetComponent<UIRadioBtn>().Value = true;
		}
		component.Dismiss();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000D03C File Offset: 0x0000B23C
	private void OnChallengeMapSelected(IUIObject obj)
	{
		UIScrollList uiscrollList = obj as UIScrollList;
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createChallengeTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (uiscrollList.SelectedItem != null)
		{
			GuiUtils.FindChildOf(this.m_createChallengeTab, "MapmenuButton").GetComponent<UIButton>().Text = uiscrollList.SelectedItem.Text;
			MapInfo mapByName = this.m_mapMan.GetMapByName(GameType.Challenge, string.Empty, this.m_mapList[uiscrollList.SelectedItem.Index]);
			this.SetMap(mapByName);
			this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 2);
			this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 3);
			this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 4);
			this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/One").GetComponent<UIRadioBtn>().Value = true;
		}
		component.Dismiss();
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000D174 File Offset: 0x0000B374
	private void OnAssassinationMapSelected(IUIObject obj)
	{
		UIScrollList uiscrollList = obj as UIScrollList;
		UIBistateInteractivePanel component = GuiUtils.FindChildOf(this.m_createAssTab.gameObject, "MapMenuDropdown").GetComponent<UIBistateInteractivePanel>();
		if (uiscrollList.SelectedItem != null)
		{
			GuiUtils.FindChildOf(this.m_createAssTab, "MapmenuButton").GetComponent<UIButton>().Text = uiscrollList.SelectedItem.Text;
			MapInfo mapByName = this.m_mapMan.GetMapByName(GameType.Assassination, string.Empty, this.m_mapList[uiscrollList.SelectedItem.Index]);
			this.SetMap(mapByName);
			this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 2);
			this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 3);
			this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().controlIsEnabled = (mapByName.m_player >= 4);
			this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value = true;
		}
		component.Dismiss();
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000D2AC File Offset: 0x0000B4AC
	private FleetSizeClass GetSkirmishFleetSize()
	{
		FleetSizeClass result = FleetSizeClass.Medium;
		if (this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Small;
		}
		if (this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Medium;
		}
		if (this.m_createSkirmishTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Heavy;
		}
		return result;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000D330 File Offset: 0x0000B530
	private int GetSkirmishGamePlayers()
	{
		int result = 1;
		if (this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value)
		{
			result = 2;
		}
		if (this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().Value)
		{
			result = 3;
		}
		if (this.m_createSkirmishTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().Value)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000D3B4 File Offset: 0x0000B5B4
	private void OnCreateSkirmishGame(IUIObject obj)
	{
		if (this.m_hasCreated)
		{
			return;
		}
		int skirmishGamePlayers = this.GetSkirmishGamePlayers();
		IUIListObject selectedItem = GuiUtils.FindChildOf(this.m_createSkirmishTab, "ScrollList").GetComponent<UIScrollList>().SelectedItem;
		if (selectedItem == null)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "No map selected", null);
			return;
		}
		string map = this.m_mapList[selectedItem.Index];
		bool autoJoin = true;
		GameType gametype = GameType.Points;
		string empty = string.Empty;
		float targetScore = (float)this.m_targetScore / 100f;
		FleetSizeClass skirmishFleetSize = this.GetSkirmishFleetSize();
		this.m_onCreateGame(gametype, empty, map, skirmishGamePlayers, (int)skirmishFleetSize, targetScore, this.GetSelectedTurnTime(skirmishGamePlayers), autoJoin);
		this.m_hasCreated = true;
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000D468 File Offset: 0x0000B668
	private int GetCampaignPlayers()
	{
		int result = 1;
		if (this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value)
		{
			result = 2;
		}
		if (this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().Value)
		{
			result = 3;
		}
		if (this.m_createCampaignTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().Value)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000D4EC File Offset: 0x0000B6EC
	private void OnCreateCampaignGame(IUIObject obj)
	{
		int campaignPlayers = this.GetCampaignPlayers();
		IUIListObject selectedItem = GuiUtils.FindChildOf(this.m_createCampaignTab, "ScrollList").GetComponent<UIScrollList>().SelectedItem;
		if (selectedItem == null)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "No map selected", null);
			return;
		}
		string name = this.m_availableCampaigns[this.m_selectedCampaignID].m_name;
		this.CreateCampaignGame(name, selectedItem.Index, campaignPlayers);
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000D560 File Offset: 0x0000B760
	public void CreateCampaignGame(string campaignName, int mapNr, int players)
	{
		if (this.m_hasCreated)
		{
			return;
		}
		List<MapInfo> campaignMaps = this.m_mapMan.GetCampaignMaps(campaignName);
		MapInfo mapInfo = campaignMaps[mapNr];
		bool autoJoin = true;
		FleetSizeClass fleetSize = FleetSizeClass.None;
		float targetScore = 0f;
		this.m_onCreateGame(GameType.Campaign, campaignName, mapInfo.m_name, players, (int)fleetSize, targetScore, this.GetSelectedTurnTime(players), autoJoin);
		this.m_hasCreated = true;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000D5C0 File Offset: 0x0000B7C0
	private void OnNextCampaign(IUIObject obj)
	{
		this.SetSelectedCampaign(this.m_selectedCampaignID + 1);
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000D5D0 File Offset: 0x0000B7D0
	private void OnPrevCampaign(IUIObject obj)
	{
		this.SetSelectedCampaign(this.m_selectedCampaignID - 1);
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000D5E0 File Offset: 0x0000B7E0
	private int GetChallengePlayers()
	{
		int result = 1;
		if (this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value)
		{
			result = 2;
		}
		if (this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().Value)
		{
			result = 3;
		}
		if (this.m_createChallengeTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().Value)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000D664 File Offset: 0x0000B864
	private void OnCreateChallengeGame(IUIObject obj)
	{
		if (this.m_hasCreated)
		{
			return;
		}
		int challengePlayers = this.GetChallengePlayers();
		IUIListObject selectedItem = GuiUtils.FindChildOf(this.m_createChallengeTab, "ScrollList").GetComponent<UIScrollList>().SelectedItem;
		if (selectedItem == null)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "No map selected", null);
			return;
		}
		string map = this.m_mapList[selectedItem.Index];
		bool autoJoin = true;
		FleetSizeClass fleetSize = FleetSizeClass.None;
		float targetScore = 0f;
		string empty = string.Empty;
		this.m_onCreateGame(GameType.Challenge, empty, map, challengePlayers, (int)fleetSize, targetScore, this.GetSelectedTurnTime(challengePlayers), autoJoin);
		this.m_hasCreated = true;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000D708 File Offset: 0x0000B908
	private FleetSizeClass GetAssassinationFleetSize()
	{
		FleetSizeClass result = FleetSizeClass.Medium;
		if (this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Small").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Small;
		}
		if (this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Medium").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Medium;
		}
		if (this.m_createAssTab.transform.FindChild("FleetSize/FleetSizeRow/interior/Large").GetComponent<UIRadioBtn>().Value)
		{
			result = FleetSizeClass.Heavy;
		}
		return result;
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000D78C File Offset: 0x0000B98C
	private int GetAssassinationPlayers()
	{
		int result = 1;
		if (this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Two").GetComponent<UIRadioBtn>().Value)
		{
			result = 2;
		}
		if (this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Three").GetComponent<UIRadioBtn>().Value)
		{
			result = 3;
		}
		if (this.m_createAssTab.transform.FindChild("Players/PlayerRow/interior/Four").GetComponent<UIRadioBtn>().Value)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000D810 File Offset: 0x0000BA10
	private void OnCreateAssassinateGame(IUIObject obj)
	{
		if (this.m_hasCreated)
		{
			return;
		}
		int assassinationPlayers = this.GetAssassinationPlayers();
		IUIListObject selectedItem = GuiUtils.FindChildOf(this.m_createAssTab, "ScrollList").GetComponent<UIScrollList>().SelectedItem;
		if (selectedItem == null)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "No map selected", null);
			return;
		}
		string map = this.m_mapList[selectedItem.Index];
		bool autoJoin = true;
		float targetScore = 1f;
		string empty = string.Empty;
		FleetSizeClass assassinationFleetSize = this.GetAssassinationFleetSize();
		this.m_onCreateGame(GameType.Assassination, empty, map, assassinationPlayers, (int)assassinationFleetSize, targetScore, this.GetSelectedTurnTime(assassinationPlayers), autoJoin);
		this.m_hasCreated = true;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000D8B8 File Offset: 0x0000BAB8
	private void OnCreateGameCancel(IUIObject obj)
	{
		this.m_createGameView.SetActiveRecursively(false);
		if (this.m_onClose != null)
		{
			this.m_onClose();
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000D8E8 File Offset: 0x0000BAE8
	private void OnOpenCreateGame(IUIObject obj)
	{
		this.m_createGameView.SetActiveRecursively(true);
		this.ShowCreateSkirmishGame();
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000D8FC File Offset: 0x0000BAFC
	private void OnCreateSkirmishTab(IUIObject obj)
	{
		this.ShowCreateSkirmishGame();
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000D904 File Offset: 0x0000BB04
	private void ShowCreateSkirmishGame()
	{
		this.m_createSkirmishTab.SetActiveRecursively(true);
		this.m_createCampaignTab.SetActiveRecursively(false);
		this.m_createChallengeTab.SetActiveRecursively(false);
		this.m_createAssTab.SetActiveRecursively(false);
		this.m_skirmishTabButton.controlIsEnabled = false;
		this.m_campaignTabButton.controlIsEnabled = true;
		this.m_challengeTabButton.controlIsEnabled = true;
		this.m_assTabButton.controlIsEnabled = true;
		UIScrollList component = GuiUtils.FindChildOf(this.m_createSkirmishTab, "ScrollList").GetComponent<UIScrollList>();
		GameObject gameObject = GuiUtils.FindChildOf(this.m_createSkirmishTab, "LevelListItem").gameObject;
		List<string> availableMaps = this.m_userMan.GetAvailableMaps();
		List<MapInfo> skirmishMaps = this.m_mapMan.GetSkirmishMaps();
		component.ClearList(true);
		this.m_mapList.Clear();
		foreach (MapInfo mapInfo in skirmishMaps)
		{
			if (availableMaps.Contains(mapInfo.m_name))
			{
				component.CreateItem(gameObject, CreateGame.TranslatedMapName(mapInfo.m_name));
				this.m_mapList.Add(mapInfo.m_name);
			}
		}
		if (component.Count > 0)
		{
			component.SetSelectedItem(0);
		}
		this.UpdateTargetScoreWidgets();
		this.SetupTurnTimers(this.m_createSkirmishTab, this.GetSkirmishGamePlayers());
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000DA7C File Offset: 0x0000BC7C
	private void OnCreateCampaignTab(IUIObject obj)
	{
		this.ShowCreateCampaignGame();
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000DA84 File Offset: 0x0000BC84
	private void SetupTurnTimers(GameObject panel, int players)
	{
		this.m_turnTimeButtons.Clear();
		for (int i = 0; i < 10; i++)
		{
			UIRadioBtn uiradioBtn = GuiUtils.FindChildOfComponent<UIRadioBtn>(panel, "TurnTime" + i.ToString());
			if (uiradioBtn != null)
			{
				uiradioBtn.controlIsEnabled = (players != 1);
				uiradioBtn.Text = Localize.instance.Translate(Utils.FormatTimeLeftString(Constants.m_turnTimeLimits[i]));
				this.m_turnTimeButtons.Add(new KeyValuePair<UIRadioBtn, double>(uiradioBtn, Constants.m_turnTimeLimits[i]));
			}
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000DB14 File Offset: 0x0000BD14
	private double GetSelectedTurnTime(int players)
	{
		if (players == 1)
		{
			return -1.0;
		}
		foreach (KeyValuePair<UIRadioBtn, double> keyValuePair in this.m_turnTimeButtons)
		{
			if (keyValuePair.Key.Value)
			{
				return keyValuePair.Value;
			}
		}
		return -1.0;
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000DBB0 File Offset: 0x0000BDB0
	private void ShowCreateCampaignGame()
	{
		this.m_createSkirmishTab.SetActiveRecursively(false);
		this.m_createCampaignTab.SetActiveRecursively(true);
		this.m_createChallengeTab.SetActiveRecursively(false);
		this.m_createAssTab.SetActiveRecursively(false);
		this.m_skirmishTabButton.controlIsEnabled = true;
		this.m_campaignTabButton.controlIsEnabled = false;
		this.m_challengeTabButton.controlIsEnabled = true;
		this.m_assTabButton.controlIsEnabled = true;
		List<CampaignInfo> campaigns = this.m_mapMan.GetCampaigns();
		List<string> availableCampaigns = this.m_userMan.GetAvailableCampaigns();
		this.m_availableCampaigns.Clear();
		foreach (CampaignInfo campaignInfo in campaigns)
		{
			if (availableCampaigns.Contains(campaignInfo.m_name))
			{
				this.m_availableCampaigns.Add(campaignInfo);
			}
		}
		this.SetSelectedCampaign(0);
		this.SetupTurnTimers(this.m_createCampaignTab, this.GetCampaignPlayers());
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000DCC4 File Offset: 0x0000BEC4
	private void SetSelectedCampaign(int id)
	{
		if (id < 0 || id >= this.m_availableCampaigns.Count)
		{
			return;
		}
		this.m_selectedCampaignID = id;
		GuiUtils.FindChildOf(this.m_createCampaignTab, "CampaignNameLbl").GetComponent<SpriteText>().Text = Localize.instance.Translate("$" + this.m_availableCampaigns[this.m_selectedCampaignID].m_name);
		GuiUtils.FindChildOf(this.m_createCampaignTab, "CampaignDescLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(this.m_availableCampaigns[this.m_selectedCampaignID].m_description);
		GuiUtils.FindChildOf(this.m_createCampaignTab, "ArrowLeftButton").GetComponent<UIButton>().controlIsEnabled = (this.m_selectedCampaignID != 0);
		GuiUtils.FindChildOf(this.m_createCampaignTab, "ArrowRightButton").GetComponent<UIButton>().controlIsEnabled = (this.m_selectedCampaignID != this.m_availableCampaigns.Count - 1);
		this.UpdateCampaignMapList();
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
	private void OnCreateChallengeTab(IUIObject obj)
	{
		this.ShowCreateChallengeGame();
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
	private void OnCreateAssTab(IUIObject obj)
	{
		this.ShowCreateAssassinateGame();
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000DDE0 File Offset: 0x0000BFE0
	private void ShowCreateChallengeGame()
	{
		this.m_createSkirmishTab.SetActiveRecursively(false);
		this.m_createCampaignTab.SetActiveRecursively(false);
		this.m_createAssTab.SetActiveRecursively(false);
		this.m_createChallengeTab.SetActiveRecursively(true);
		this.m_skirmishTabButton.controlIsEnabled = true;
		this.m_campaignTabButton.controlIsEnabled = true;
		this.m_challengeTabButton.controlIsEnabled = false;
		this.m_assTabButton.controlIsEnabled = true;
		UIScrollList component = GuiUtils.FindChildOf(this.m_createChallengeTab, "ScrollList").GetComponent<UIScrollList>();
		GameObject gameObject = GuiUtils.FindChildOf(this.m_createChallengeTab, "LevelListItem").gameObject;
		List<string> availableMaps = this.m_userMan.GetAvailableMaps();
		List<MapInfo> challengeMaps = this.m_mapMan.GetChallengeMaps();
		component.ClearList(true);
		this.m_mapList.Clear();
		foreach (MapInfo mapInfo in challengeMaps)
		{
			if (availableMaps.Contains(mapInfo.m_name))
			{
				component.CreateItem(gameObject, CreateGame.TranslatedMapName(mapInfo.m_name));
				this.m_mapList.Add(mapInfo.m_name);
			}
		}
		if (component.Count > 0)
		{
			component.SetSelectedItem(0);
		}
		this.SetupTurnTimers(this.m_createChallengeTab, this.GetChallengePlayers());
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000DF50 File Offset: 0x0000C150
	private void ShowCreateAssassinateGame()
	{
		this.m_createSkirmishTab.SetActiveRecursively(false);
		this.m_createCampaignTab.SetActiveRecursively(false);
		this.m_createChallengeTab.SetActiveRecursively(false);
		this.m_createAssTab.SetActiveRecursively(true);
		this.m_skirmishTabButton.controlIsEnabled = true;
		this.m_campaignTabButton.controlIsEnabled = true;
		this.m_challengeTabButton.controlIsEnabled = true;
		this.m_assTabButton.controlIsEnabled = false;
		UIScrollList component = GuiUtils.FindChildOf(this.m_createAssTab, "ScrollList").GetComponent<UIScrollList>();
		GameObject gameObject = GuiUtils.FindChildOf(this.m_createAssTab, "LevelListItem").gameObject;
		List<string> availableMaps = this.m_userMan.GetAvailableMaps();
		List<MapInfo> assassinationMaps = this.m_mapMan.GetAssassinationMaps();
		component.ClearList(true);
		this.m_mapList.Clear();
		foreach (MapInfo mapInfo in assassinationMaps)
		{
			if (availableMaps.Contains(mapInfo.m_name))
			{
				component.CreateItem(gameObject, CreateGame.TranslatedMapName(mapInfo.m_name));
				this.m_mapList.Add(mapInfo.m_name);
			}
		}
		if (component.Count > 0)
		{
			component.SetSelectedItem(0);
		}
		this.SetupTurnTimers(this.m_createAssTab, this.GetAssassinationPlayers());
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000E0C0 File Offset: 0x0000C2C0
	private void UpdateCampaignMapList()
	{
		UIScrollList component = GuiUtils.FindChildOf(this.m_createCampaignTab, "ScrollList").GetComponent<UIScrollList>();
		GameObject gameObject = GuiUtils.FindChildOf(this.m_createCampaignTab, "LevelListItem").gameObject;
		string name = this.m_availableCampaigns[this.m_selectedCampaignID].m_name;
		List<string> unlockedCampaignMaps = this.m_userMan.GetUnlockedCampaignMaps(name);
		component.ClearList(true);
		List<MapInfo> campaignMaps = this.m_mapMan.GetCampaignMaps(name);
		this.m_mapList.Clear();
		foreach (MapInfo mapInfo in campaignMaps)
		{
			UIListItem uilistItem = component.CreateItem(gameObject, CreateGame.TranslatedMapName(mapInfo.m_name)) as UIListItem;
			this.m_mapList.Add(mapInfo.m_name);
			if (!unlockedCampaignMaps.Contains(mapInfo.m_name))
			{
				uilistItem.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			}
		}
		component.SetSelectedItem(0);
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000E1DC File Offset: 0x0000C3DC
	public static string TranslatedMapName(string mapName)
	{
		return Localize.instance.TranslateKey("mapname_" + mapName);
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
	private void SetMap(MapInfo map)
	{
		this.m_mapNameLabel.Text = CreateGame.TranslatedMapName(map.m_name);
		this.m_mapDescriptionLabel.Text = Localize.instance.Translate(map.m_description);
		this.m_mapSizeLabel.Text = map.m_size.ToString() + "x" + map.m_size.ToString();
		Texture2D texture2D = Resources.Load("MapThumbs/" + map.m_thumbnail) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning("Map Thumbnail " + map.m_thumbnail + " is missing");
			return;
		}
		this.m_mapImage.SetTexture(texture2D);
		this.m_mapImage.UpdateUVs();
	}

	// Token: 0x0400014E RID: 334
	public Action m_onClose;

	// Token: 0x0400014F RID: 335
	public CreateGame.CreateDelegate m_onCreateGame;

	// Token: 0x04000150 RID: 336
	private GameObject m_createGameView;

	// Token: 0x04000151 RID: 337
	private MapMan m_mapMan;

	// Token: 0x04000152 RID: 338
	private UserManClient m_userMan;

	// Token: 0x04000153 RID: 339
	private GameObject m_guiCamera;

	// Token: 0x04000154 RID: 340
	private GameObject m_createSkirmishTab;

	// Token: 0x04000155 RID: 341
	private GameObject m_createCampaignTab;

	// Token: 0x04000156 RID: 342
	private GameObject m_createChallengeTab;

	// Token: 0x04000157 RID: 343
	private GameObject m_createAssTab;

	// Token: 0x04000158 RID: 344
	private UIButton m_skirmishTabButton;

	// Token: 0x04000159 RID: 345
	private UIButton m_campaignTabButton;

	// Token: 0x0400015A RID: 346
	private UIButton m_challengeTabButton;

	// Token: 0x0400015B RID: 347
	private UIButton m_assTabButton;

	// Token: 0x0400015C RID: 348
	private SpriteText m_mapSizeLabel;

	// Token: 0x0400015D RID: 349
	private SpriteText m_mapDescriptionLabel;

	// Token: 0x0400015E RID: 350
	private SpriteText m_mapNameLabel;

	// Token: 0x0400015F RID: 351
	private SimpleSprite m_mapImage;

	// Token: 0x04000160 RID: 352
	private bool m_hasCreated;

	// Token: 0x04000161 RID: 353
	private int m_selectedCampaignID = -1;

	// Token: 0x04000162 RID: 354
	private int m_targetScore = 60;

	// Token: 0x04000163 RID: 355
	private List<CampaignInfo> m_availableCampaigns = new List<CampaignInfo>();

	// Token: 0x04000164 RID: 356
	private List<string> m_mapList = new List<string>();

	// Token: 0x04000165 RID: 357
	private List<KeyValuePair<UIRadioBtn, double>> m_turnTimeButtons = new List<KeyValuePair<UIRadioBtn, double>>();

	// Token: 0x04000166 RID: 358
	private MsgBox m_msgBox;

	// Token: 0x020001A2 RID: 418
	// (Invoke) Token: 0x06000F38 RID: 3896
	public delegate void CreateDelegate(GameType gametype, string campaign, string map, int players, int fleetSize, float targetScore, double turnTime, bool autoJoin);
}
