using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class OnlineMenu : MenuBase
{
	// Token: 0x060003A2 RID: 930 RVA: 0x0001D250 File Offset: 0x0001B450
	public OnlineMenu(GameObject guiCamera, PTech.RPC rpc, string hostName, string localUserName, MapMan mapMan, UserManClient userMan, MenuBase.StartStatus startStatus, MusicManager musMan, GDPBackend gdpBackend, PdxNews pdxNews)
	{
		this.m_guiCamera = guiCamera;
		this.m_rpc = rpc;
		this.m_localUserName = localUserName;
		this.m_mapMan = mapMan;
		this.m_userMan = userMan;
		this.m_musicMan = musMan;
		this.m_gdpBackend = gdpBackend;
		this.m_newsTicker = new NewsTicker(pdxNews, gdpBackend, this.m_guiCamera);
		this.m_rpc.Register("CreateOK", new PTech.RPC.Handler(this.RPC_CreateOK));
		this.m_rpc.Register("CreateFail", new PTech.RPC.Handler(this.RPC_CreateFail));
		this.m_rpc.Register("GameList", new PTech.RPC.Handler(this.RPC_GameList));
		this.m_rpc.Register("ArchivedGameList", new PTech.RPC.Handler(this.RPC_ArchivedGameList));
		this.m_rpc.Register("Friends", new PTech.RPC.Handler(this.RPC_Friends));
		this.m_rpc.Register("FriendRequestReply", new PTech.RPC.Handler(this.RPC_FriendRequestReply));
		this.m_rpc.Register("NrOfFriendRequests", new PTech.RPC.Handler(this.RPC_NrOfFriendRequests));
		this.m_rpc.Register("Profile", new PTech.RPC.Handler(this.RPC_Profile));
		this.m_rpc.Register("News", new PTech.RPC.Handler(this.RPC_News));
		this.m_gui = GuiUtils.CreateGui("MainmenuOnline", guiCamera);
		this.m_panelMan = GuiUtils.FindChildOf(this.m_gui, "PanelMan").GetComponent<UIPanelManager>();
		GuiUtils.FindChildOf(this.m_gui, "VersionLbl").GetComponent<SpriteText>().Text = Localize.instance.Translate("$version " + VersionInfo.GetFullVersionString());
		GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "ActiveUserLabel").Text = this.m_localUserName;
		GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "ActiveHostLabel").Text = hostName;
		GuiUtils.FindChildOf(this.m_gui, "CreateGameButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenCreateGame));
		GuiUtils.FindChildOf(this.m_gui, "ButtonPlay").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowPlay));
		GuiUtils.FindChildOf(this.m_gui, "ButtonProfile").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowProfile));
		GuiUtils.FindChildOf(this.m_gui, "ButtonArchive").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowArchive));
		GuiUtils.FindChildOf(this.m_gui, "ButtonFriends").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowFriends));
		GuiUtils.FindChildOf(this.m_gui, "ButtonNews").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowNews));
		GuiUtils.FindChildOf(this.m_gui, "ButtonCredits").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowCreditsPressed));
		GuiUtils.FindChildOf(this.m_gui, "ExitButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnBackPressed));
		GuiUtils.FindChildOf(this.m_gui, "OptionsButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOptionsPressed));
		GuiUtils.FindChildOf(this.m_gui, "NewFleetButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnNewFleet));
		GuiUtils.FindChildOf(this.m_gui, "DeleteFleetButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnDeleteFleet));
		GuiUtils.FindChildOf(this.m_gui, "EditFleetButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnEditleet));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "facebookButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenFaceBook));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "twitterButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenTwitter));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "paradoxButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenForum));
		this.SetModifyFleetButtonsStatus(false);
		GameObject gameObject = this.m_gui.transform.FindChild("CreateGameView").gameObject;
		this.m_createGame = new CreateGame(this.m_guiCamera, gameObject, this.m_mapMan, this.m_userMan);
		this.m_createGame.m_onClose = new Action(this.OnCreateGameClose);
		this.m_createGame.m_onCreateGame = new CreateGame.CreateDelegate(this.OnCreateGame);
		this.m_newsTab.m_newsItem = GuiUtils.FindChildOf(this.m_gui, "NewsListItem");
		this.m_newsTab.m_newsList = GuiUtils.FindChildOf(this.m_gui, "NewsScrollList").GetComponent<UIScrollList>();
		this.m_playTab.m_gameListItem = GuiUtils.CreateGui("gamelist/Gamelist_listitem", this.m_guiCamera);
		this.m_playTab.m_gameListItem.transform.Translate(100000f, 0f, 0f);
		this.m_playTab.m_gameList = GuiUtils.FindChildOf(this.m_gui, "CurrentGamesScrollList").GetComponent<UIScrollList>();
		this.m_playTab.m_panel = GuiUtils.FindChildOf(this.m_gui, "PanelPlay").GetComponent<UIPanel>();
		this.m_playTab.m_listPublicGames = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_gui, "CheckboxPublicGames");
		this.m_playTab.m_listPublicGames.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnListPublicGamesChanged));
		this.m_archiveTab.m_gameListItem = GuiUtils.FindChildOf(this.m_gui, "ArchivedGameListItem");
		this.m_archiveTab.m_gameList = GuiUtils.FindChildOf(this.m_gui, "ArchivedGamesScrollList").GetComponent<UIScrollList>();
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "FindReplayButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPlayReplayID));
		this.m_friendsTab.m_panel = GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "PanelFriends");
		this.m_friendsTab.m_friendListItem = GuiUtils.FindChildOf(this.m_gui, "FriendListItem");
		this.m_friendsTab.m_confirmedFriendListItem = GuiUtils.FindChildOf(this.m_gui, "ConfirmedFriendListItem");
		this.m_friendsTab.m_pendingFriendListItem = GuiUtils.FindChildOf(this.m_gui, "PendingFriendListItem");
		this.m_friendsTab.m_friendList = GuiUtils.FindChildOf(this.m_gui, "FriendsScrollList").GetComponent<UIScrollList>();
		GuiUtils.FindChildOf(this.m_gui, "FriendsNotificationWidget").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui, "AddFriendButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAddFriendPressed));
		this.m_fleetTab.m_gameFleetItem = (Resources.Load("gui/Shipyard/FleetListItem") as GameObject);
		this.m_fleetTab.m_fleetList = GuiUtils.FindChildOf(this.m_gui, "FleetList").GetComponent<UIScrollList>();
		GameObject shopPanel = GuiUtils.FindChildOf(this.m_gui, "PanelShop");
		this.m_shop = new Shop(this.m_guiCamera, shopPanel, this.m_gdpBackend, this.m_rpc, this.m_userMan);
		this.m_shop.m_onItemBought = new Action(this.OnItemBought);
		GuiUtils.FindChildOf(this.m_gui, "ButtonShop").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.m_shop.OnShowShop));
		GameObject creditsRoot = GuiUtils.FindChildOf(this.m_gui, "PanelCredits");
		this.m_credits = new Credits(creditsRoot, this.m_musicMan);
		this.m_matchMaking.m_panel = GuiUtils.FindChildOf(this.m_gui, "MatchmakingPanel");
		this.m_matchMaking.m_gameTypeCampaign = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Campaign");
		this.m_matchMaking.m_gameTypeChallenge = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Challenge");
		this.m_matchMaking.m_gameTypePoints = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Points");
		this.m_matchMaking.m_gameTypeAssassination = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Assassination");
		this.m_matchMaking.m_playersAny = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "CheckboxAnyPlayers");
		this.m_matchMaking.m_playersTwo = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Two");
		this.m_matchMaking.m_playersThree = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Three");
		this.m_matchMaking.m_playersFour = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Four");
		this.m_matchMaking.m_fleetSizeAny = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "CheckboxAnyFleetsize");
		this.m_matchMaking.m_fleetSizeSmall = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Small");
		this.m_matchMaking.m_fleetSizeMedium = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Medium");
		this.m_matchMaking.m_fleetSizeLarge = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "Large");
		this.m_matchMaking.m_targetPointsEnabled = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "CheckboxOn");
		this.m_matchMaking.m_targetPointsDisabled = GuiUtils.FindChildOfComponent<UIRadioBtn>(this.m_matchMaking.m_panel, "CheckboxAnyPointsgoal");
		this.m_matchMaking.m_targetPointsMinus = GuiUtils.FindChildOfComponent<UIActionBtn>(this.m_matchMaking.m_panel, "MinusButton");
		this.m_matchMaking.m_targetPointsPlus = GuiUtils.FindChildOfComponent<UIActionBtn>(this.m_matchMaking.m_panel, "PlusButton");
		this.m_matchMaking.m_targetPointsPercentage = GuiUtils.FindChildOfComponent<SpriteText>(this.m_matchMaking.m_panel, "PointsPercentage");
		this.m_matchMaking.m_targetPointsConversion = GuiUtils.FindChildOfComponent<SpriteText>(this.m_matchMaking.m_panel, "PointsConversion");
		GuiUtils.FindChildOfComponent<UIButton>(this.m_matchMaking.m_panel, "ButtonMatchmake").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFindGame));
		this.m_matchMaking.m_gameTypeCampaign.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeGameTypeChanged));
		this.m_matchMaking.m_gameTypeChallenge.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeGameTypeChanged));
		this.m_matchMaking.m_gameTypePoints.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeGameTypeChanged));
		this.m_matchMaking.m_gameTypeAssassination.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeGameTypeChanged));
		this.m_matchMaking.m_playersAny.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakePlayersChanged));
		this.m_matchMaking.m_playersTwo.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakePlayersChanged));
		this.m_matchMaking.m_playersThree.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakePlayersChanged));
		this.m_matchMaking.m_playersFour.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakePlayersChanged));
		this.m_matchMaking.m_fleetSizeAny.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeFleetSizeChanged));
		this.m_matchMaking.m_fleetSizeSmall.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeFleetSizeChanged));
		this.m_matchMaking.m_fleetSizeMedium.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeFleetSizeChanged));
		this.m_matchMaking.m_fleetSizeLarge.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeFleetSizeChanged));
		this.m_matchMaking.m_targetPointsMinus.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeTargetPointsMinus));
		this.m_matchMaking.m_targetPointsPlus.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeTargetPointsPlus));
		this.m_matchMaking.m_targetPointsEnabled.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeTargetPointsStatusChanged));
		this.m_matchMaking.m_targetPointsDisabled.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeTargetPointsStatusChanged));
		this.m_matchMaking.m_anyTurnTime = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_matchMaking.m_panel, "CheckboxAnyPlanningTime");
		this.m_matchMaking.m_anyTurnTime.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAnyTurnTimeStatusChangeD));
		for (int i = 0; i < 10; i++)
		{
			UIStateToggleBtn uistateToggleBtn = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_matchMaking.m_panel, "TurnTime" + i.ToString());
			if (uistateToggleBtn != null)
			{
				uistateToggleBtn.Text = Localize.instance.Translate(Utils.FormatTimeLeftString(Constants.m_turnTimeLimits[i]));
				this.m_matchMaking.m_turnTimeCheckBoxes.Add(new KeyValuePair<UIStateToggleBtn, double>(uistateToggleBtn, Constants.m_turnTimeLimits[i]));
			}
		}
		this.UpdateMatchmakingTargetScoreWidgets();
		switch (startStatus)
		{
		case MenuBase.StartStatus.ShowGameView:
			this.m_panelMan.BringIn("PanelPlay");
			this.RequestGameList();
			break;
		case MenuBase.StartStatus.ShowArchiveView:
			this.m_panelMan.BringIn("PanelArchive");
			this.RequestArchivedGames();
			break;
		case MenuBase.StartStatus.ShowShipyard:
			this.m_panelMan.BringIn("PanelShipyard");
			break;
		case MenuBase.StartStatus.ShowCredits:
			this.StartCredits();
			break;
		default:
			this.m_panelMan.BringIn("PanelNews");
			this.RequestNews();
			break;
		}
		UserManClient userMan2 = this.m_userMan;
		userMan2.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Combine(userMan2.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		this.FillFleetList();
		this.UpdateNotifications();
		this.LoadMatchmakingSettings();
		if (PlayerPrefs.GetInt("playedtutorial", 0) != 1)
		{
			this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$mainmenu_starttutorial", new MsgBox.YesHandler(this.OnStartTutorialYes), new MsgBox.NoHandler(this.OnStartTutorialNo));
		}
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x0001E044 File Offset: 0x0001C244
	public void Close()
	{
		UserManClient userMan = this.m_userMan;
		userMan.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userMan.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		this.m_rpc.Unregister("CreateOK");
		this.m_rpc.Unregister("CreateFail");
		this.m_rpc.Unregister("GameList");
		this.m_rpc.Unregister("ArchivedGameList");
		this.m_rpc.Unregister("Profile");
		this.m_rpc.Unregister("Friends");
		this.m_rpc.Unregister("FriendRequestReply");
		this.m_rpc.Unregister("NrOfFriendRequests");
		this.m_rpc.Unregister("News");
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_credits = null;
		if (this.m_shop != null)
		{
			this.m_shop.Close();
			this.m_shop = null;
		}
		if (this.m_newsTicker != null)
		{
			this.m_newsTicker.Close();
			this.m_newsTicker = null;
		}
		if (this.m_archiveTab.m_playReplayDialog != null)
		{
			UnityEngine.Object.Destroy(this.m_archiveTab.m_playReplayDialog);
		}
		if (this.m_matchMaking.m_progressDialog != null)
		{
			UnityEngine.Object.Destroy(this.m_matchMaking.m_progressDialog);
		}
		UnityEngine.Object.Destroy(this.m_playTab.m_gameListItem);
		UnityEngine.Object.Destroy(this.m_archiveTab.m_gameListItem);
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_gui = null;
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x0001E1E4 File Offset: 0x0001C3E4
	public void OnKicked()
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_kicked", new MsgBox.OkHandler(this.OnMsgBoxOK));
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x0001E214 File Offset: 0x0001C414
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

	// Token: 0x060003A6 RID: 934 RVA: 0x0001E274 File Offset: 0x0001C474
	private void OnStartTutorialYes()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		PlayerPrefs.SetInt("playedtutorial", 1);
		this.m_createGame.CreateCampaignGame("t1_name", 0, 1);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0001E2A8 File Offset: 0x0001C4A8
	private void OnStartTutorialNo()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		PlayerPrefs.SetInt("playedtutorial", 1);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x0001E2C8 File Offset: 0x0001C4C8
	private void UpdateNotifications()
	{
		this.m_rpc.Invoke("RequestNrOfFriendRequests", new object[0]);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0001E2E0 File Offset: 0x0001C4E0
	private void SetModifyFleetButtonsStatus(bool enable)
	{
		GuiUtils.FindChildOf(this.m_gui, "DeleteFleetButton").GetComponent<UIButton>().controlIsEnabled = enable;
		GuiUtils.FindChildOf(this.m_gui, "EditFleetButton").GetComponent<UIButton>().controlIsEnabled = enable;
	}

	// Token: 0x060003AA RID: 938 RVA: 0x0001E324 File Offset: 0x0001C524
	private void OnMatchmakeGameTypeChanged(IUIObject button)
	{
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003AB RID: 939 RVA: 0x0001E32C File Offset: 0x0001C52C
	private void OnMatchmakePlayersChanged(IUIObject button)
	{
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0001E334 File Offset: 0x0001C534
	private void OnMatchmakeFleetSizeChanged(IUIObject button)
	{
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0001E33C File Offset: 0x0001C53C
	private void OnMatchmakeTargetPointsMinus(IUIObject button)
	{
		int num = 10;
		this.m_matchMaking.m_targetPointsValue -= 10;
		if (this.m_matchMaking.m_targetPointsValue < num)
		{
			this.m_matchMaking.m_targetPointsValue = num;
		}
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0001E384 File Offset: 0x0001C584
	private void OnMatchmakeTargetPointsPlus(IUIObject button)
	{
		this.m_matchMaking.m_targetPointsValue += 10;
		if (this.m_matchMaking.m_targetPointsValue > 100)
		{
			this.m_matchMaking.m_targetPointsValue = 100;
		}
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0001E3C0 File Offset: 0x0001C5C0
	private void OnMatchmakeTargetPointsStatusChanged(IUIObject button)
	{
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0001E3C8 File Offset: 0x0001C5C8
	private void OnAnyTurnTimeStatusChangeD(IUIObject button)
	{
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0001E3D0 File Offset: 0x0001C5D0
	private void UpdateFleetSizes()
	{
		GameType selectedMatchmakingGameType = this.GetSelectedMatchmakingGameType();
		if (selectedMatchmakingGameType == GameType.Assassination || selectedMatchmakingGameType == GameType.Points)
		{
			this.m_matchMaking.m_fleetSizeAny.controlIsEnabled = true;
			this.m_matchMaking.m_fleetSizeLarge.controlIsEnabled = true;
			this.m_matchMaking.m_fleetSizeMedium.controlIsEnabled = true;
			this.m_matchMaking.m_fleetSizeSmall.controlIsEnabled = true;
		}
		else
		{
			this.m_matchMaking.m_fleetSizeAny.controlIsEnabled = false;
			this.m_matchMaking.m_fleetSizeLarge.controlIsEnabled = false;
			this.m_matchMaking.m_fleetSizeMedium.controlIsEnabled = false;
			this.m_matchMaking.m_fleetSizeSmall.controlIsEnabled = false;
		}
		this.m_matchMaking.m_targetPointsEnabled.controlIsEnabled = (selectedMatchmakingGameType == GameType.Points);
		this.m_matchMaking.m_targetPointsDisabled.controlIsEnabled = (selectedMatchmakingGameType == GameType.Points);
		this.m_matchMaking.m_targetPointsMinus.controlIsEnabled = (selectedMatchmakingGameType == GameType.Points);
		this.m_matchMaking.m_targetPointsPlus.controlIsEnabled = (selectedMatchmakingGameType == GameType.Points);
		int selectedMatchmakingPlayers = this.GetSelectedMatchmakingPlayers();
		if ((selectedMatchmakingGameType == GameType.Points || selectedMatchmakingGameType == GameType.Assassination) && selectedMatchmakingPlayers == 3)
		{
			this.m_matchMaking.m_fleetSizeLarge.controlIsEnabled = false;
			this.m_matchMaking.m_fleetSizeMedium.controlIsEnabled = false;
		}
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0001E510 File Offset: 0x0001C710
	private void UpdateMatchmakingTurnTimes()
	{
		this.m_matchMaking.m_turnTimes.Clear();
		bool flag = this.m_matchMaking.m_anyTurnTime.StateNum == 1;
		foreach (KeyValuePair<UIStateToggleBtn, double> keyValuePair in this.m_matchMaking.m_turnTimeCheckBoxes)
		{
			keyValuePair.Key.controlIsEnabled = !flag;
			if (keyValuePair.Key.StateNum == 1 || flag)
			{
				this.m_matchMaking.m_turnTimes.Add(keyValuePair.Value);
			}
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001E5D8 File Offset: 0x0001C7D8
	private void UpdateMatchmakingTargetScoreWidgets()
	{
		GameType selectedMatchmakingGameType = this.GetSelectedMatchmakingGameType();
		int num = 10;
		bool flag = this.m_matchMaking.m_targetPointsEnabled.Value && selectedMatchmakingGameType == GameType.Points;
		this.m_matchMaking.m_targetPointsMinus.controlIsEnabled = (this.m_matchMaking.m_targetPointsValue > num && flag);
		this.m_matchMaking.m_targetPointsPlus.controlIsEnabled = (this.m_matchMaking.m_targetPointsValue < 100 && flag);
		int selectedMatchmakingPlayers = this.GetSelectedMatchmakingPlayers();
		FleetSizeClass fleetSizeClass = this.GetSelectedMatchmakingFleetSize();
		if ((selectedMatchmakingGameType == GameType.Points || selectedMatchmakingGameType == GameType.Assassination) && selectedMatchmakingPlayers == 3 && (fleetSizeClass == FleetSizeClass.Heavy || fleetSizeClass == FleetSizeClass.Medium))
		{
			this.m_matchMaking.m_fleetSizeSmall.Value = true;
			fleetSizeClass = FleetSizeClass.Small;
		}
		this.UpdateFleetSizes();
		this.m_matchMaking.m_targetPointsPercentage.Text = this.m_matchMaking.m_targetPointsValue.ToString() + "%";
		if (selectedMatchmakingPlayers < 2 || fleetSizeClass == FleetSizeClass.None || selectedMatchmakingGameType != GameType.Points)
		{
			this.m_matchMaking.m_targetPointsConversion.Text = string.Empty;
		}
		else
		{
			int num2 = (int)((float)FleetSizes.sizes[(int)fleetSizeClass].max * ((float)this.m_matchMaking.m_targetPointsValue / 100f));
			if (selectedMatchmakingPlayers >= 3)
			{
				num2 *= 2;
			}
			this.m_matchMaking.m_targetPointsConversion.Text = num2.ToString() + Localize.instance.Translate(" $creategame_pointstowin");
		}
		this.UpdateMatchmakingTurnTimes();
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001E764 File Offset: 0x0001C964
	private GameType GetSelectedMatchmakingGameType()
	{
		GameType result = GameType.Points;
		if (this.m_matchMaking.m_gameTypeAssassination.Value)
		{
			result = GameType.Assassination;
		}
		if (this.m_matchMaking.m_gameTypeCampaign.Value)
		{
			result = GameType.Campaign;
		}
		if (this.m_matchMaking.m_gameTypeChallenge.Value)
		{
			result = GameType.Challenge;
		}
		if (this.m_matchMaking.m_gameTypePoints.Value)
		{
			result = GameType.Points;
		}
		return result;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0001E7D0 File Offset: 0x0001C9D0
	private int GetSelectedMatchmakingPlayers()
	{
		int result = -1;
		if (this.m_matchMaking.m_playersTwo.Value)
		{
			result = 2;
		}
		if (this.m_matchMaking.m_playersThree.Value)
		{
			result = 3;
		}
		if (this.m_matchMaking.m_playersFour.Value)
		{
			result = 4;
		}
		return result;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0001E828 File Offset: 0x0001CA28
	private FleetSizeClass GetSelectedMatchmakingFleetSize()
	{
		FleetSizeClass result = FleetSizeClass.None;
		if (this.m_matchMaking.m_fleetSizeLarge.Value)
		{
			result = FleetSizeClass.Heavy;
		}
		if (this.m_matchMaking.m_fleetSizeMedium.Value)
		{
			result = FleetSizeClass.Medium;
		}
		if (this.m_matchMaking.m_fleetSizeSmall.Value)
		{
			result = FleetSizeClass.Small;
		}
		return result;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0001E880 File Offset: 0x0001CA80
	public static string DoubleToString(double f)
	{
		return f.ToString();
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001E88C File Offset: 0x0001CA8C
	public static double StringToDouble(string s)
	{
		return double.Parse(s);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0001E894 File Offset: 0x0001CA94
	private void SaveMatchmakingSettings(GameType gameType, int players, FleetSizeClass fleetSize, bool targetScoreEnabled, int targetScore, double[] turnTimes)
	{
		PlayerPrefs.SetInt("Match_GameType", (int)gameType);
		PlayerPrefs.SetInt("Match_Players", players);
		PlayerPrefs.SetInt("Match_FleetSize", (int)fleetSize);
		PlayerPrefs.SetInt("Match_TargetScoreEnabled", (!targetScoreEnabled) ? 0 : 1);
		PlayerPrefs.SetInt("Match_TargetScore", targetScore);
		string[] value = Array.ConvertAll<double, string>(turnTimes, new Converter<double, string>(OnlineMenu.DoubleToString));
		string value2 = string.Join(",", value);
		PlayerPrefs.SetString("Match_TurnTimes", value2);
	}

	// Token: 0x060003BA RID: 954 RVA: 0x0001E914 File Offset: 0x0001CB14
	private void LoadMatchmakingSettings()
	{
		GameType @int = (GameType)PlayerPrefs.GetInt("Match_GameType", 5);
		this.m_matchMaking.m_gameTypeAssassination.Value = false;
		this.m_matchMaking.m_gameTypeCampaign.Value = false;
		this.m_matchMaking.m_gameTypePoints.Value = false;
		this.m_matchMaking.m_gameTypeChallenge.Value = false;
		switch (@int)
		{
		case GameType.Challenge:
			this.m_matchMaking.m_gameTypeChallenge.Value = true;
			break;
		case GameType.Campaign:
			this.m_matchMaking.m_gameTypeCampaign.Value = true;
			break;
		case GameType.Points:
			this.m_matchMaking.m_gameTypePoints.Value = true;
			break;
		case GameType.Assassination:
			this.m_matchMaking.m_gameTypeAssassination.Value = true;
			break;
		}
		int int2 = PlayerPrefs.GetInt("Match_Players", -1);
		this.m_matchMaking.m_playersAny.Value = false;
		this.m_matchMaking.m_playersTwo.Value = false;
		this.m_matchMaking.m_playersThree.Value = false;
		this.m_matchMaking.m_playersFour.Value = false;
		int num = int2;
		switch (num + 1)
		{
		case 0:
			this.m_matchMaking.m_playersAny.Value = true;
			break;
		case 3:
			this.m_matchMaking.m_playersTwo.Value = true;
			break;
		case 4:
			this.m_matchMaking.m_playersThree.Value = true;
			break;
		case 5:
			this.m_matchMaking.m_playersFour.Value = true;
			break;
		}
		FleetSizeClass int3 = (FleetSizeClass)PlayerPrefs.GetInt("Match_FleetSize", 5);
		this.m_matchMaking.m_fleetSizeAny.Value = false;
		this.m_matchMaking.m_fleetSizeLarge.Value = false;
		this.m_matchMaking.m_fleetSizeMedium.Value = false;
		this.m_matchMaking.m_fleetSizeSmall.Value = false;
		switch (int3)
		{
		case FleetSizeClass.Small:
			this.m_matchMaking.m_fleetSizeSmall.Value = true;
			break;
		case FleetSizeClass.Medium:
			this.m_matchMaking.m_fleetSizeMedium.Value = true;
			break;
		case FleetSizeClass.Heavy:
			this.m_matchMaking.m_fleetSizeLarge.Value = true;
			break;
		case FleetSizeClass.None:
			this.m_matchMaking.m_fleetSizeAny.Value = true;
			break;
		}
		bool value = PlayerPrefs.GetInt("Match_TargetScoreEnabled", 0) == 1;
		this.m_matchMaking.m_targetPointsEnabled.Value = value;
		this.m_matchMaking.m_targetPointsValue = PlayerPrefs.GetInt("Match_TargetScore", this.m_matchMaking.m_targetPointsValue);
		List<double> list = new List<double>();
		string @string = PlayerPrefs.GetString("Match_TurnTimes", string.Empty);
		if (@string != string.Empty)
		{
			string[] array = @string.Split(new char[]
			{
				','
			});
			list = new List<double>(Array.ConvertAll<string, double>(array, new Converter<string, double>(OnlineMenu.StringToDouble)));
		}
		bool flag = list.Count == 6 || list.Count == 0;
		this.m_matchMaking.m_anyTurnTime.SetToggleState((!flag) ? 0 : 1);
		foreach (KeyValuePair<UIStateToggleBtn, double> keyValuePair in this.m_matchMaking.m_turnTimeCheckBoxes)
		{
			bool flag2 = flag || list.Contains(keyValuePair.Value);
			keyValuePair.Key.SetToggleState((!flag2) ? 0 : 1);
		}
		this.m_matchMaking.m_turnTimes = list;
		this.UpdateMatchmakingTargetScoreWidgets();
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0001ED0C File Offset: 0x0001CF0C
	private void OnFindGame(IUIObject obj)
	{
		this.UpdateMatchmakingTurnTimes();
		this.m_matchMaking.m_progressDialog = GuiUtils.CreateGui("dialogs/Dialog_Progress_Interruptable", this.m_guiCamera);
		GuiUtils.FindChildOfComponent<UIButton>(this.m_matchMaking.m_progressDialog, "CancelButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFindGameCancel));
		GameType selectedMatchmakingGameType = this.GetSelectedMatchmakingGameType();
		int selectedMatchmakingPlayers = this.GetSelectedMatchmakingPlayers();
		FleetSizeClass selectedMatchmakingFleetSize = this.GetSelectedMatchmakingFleetSize();
		float num = (float)this.m_matchMaking.m_targetPointsValue / 100f;
		bool flag = !this.m_matchMaking.m_targetPointsDisabled.Value;
		if (!flag)
		{
			num = -1f;
		}
		this.SaveMatchmakingSettings(selectedMatchmakingGameType, selectedMatchmakingPlayers, selectedMatchmakingFleetSize, flag, this.m_matchMaking.m_targetPointsValue, this.m_matchMaking.m_turnTimes.ToArray());
		this.m_rpc.Invoke("FindGame", new object[]
		{
			(int)selectedMatchmakingGameType,
			selectedMatchmakingPlayers,
			num,
			(int)selectedMatchmakingFleetSize,
			this.m_matchMaking.m_turnTimes.ToArray()
		});
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001EE20 File Offset: 0x0001D020
	private void OnFindGameCancel(IUIObject button)
	{
		UnityEngine.Object.Destroy(this.m_matchMaking.m_progressDialog);
		this.m_matchMaking.m_progressDialog = null;
		this.m_rpc.Invoke("CancelFindGame", new object[0]);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0001EE60 File Offset: 0x0001D060
	private void OnOpenCreateGame(IUIObject obj)
	{
		this.m_createGame.Show();
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0001EE70 File Offset: 0x0001D070
	private void OnCreateGameClose()
	{
		this.m_panelMan.BringIn("PanelPlay");
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0001EE84 File Offset: 0x0001D084
	private void OnCreateGame(GameType gametype, string campaign, string mapName, int players, int fleetSize, float targetScore, double turnTime, bool autoJoin)
	{
		this.m_rpc.Invoke("CreateGame", new object[]
		{
			(int)gametype,
			campaign,
			mapName,
			players,
			fleetSize,
			targetScore,
			turnTime,
			autoJoin
		});
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0001EEEC File Offset: 0x0001D0EC
	private void OnNewFleet(IUIObject obj)
	{
		if (this.m_onProceed == null)
		{
			return;
		}
		this.m_onProceed(string.Empty, 0);
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0001EF0C File Offset: 0x0001D10C
	private void OnDeleteFleet(IUIObject obj)
	{
		if (this.m_onProceed == null)
		{
			return;
		}
		if (this.m_fleetTab.m_selectedFleet.Length == 0)
		{
			return;
		}
		string text = Localize.instance.Translate("$fleetedit_delete");
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, text, new MsgBox.YesHandler(this.OnConfirmDelete), null);
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x0001EF6C File Offset: 0x0001D16C
	private void OnConfirmDelete()
	{
		this.m_userMan.RemoveFleet(this.m_fleetTab.m_selectedFleet);
		this.m_fleetTab.m_selectedFleet = string.Empty;
		this.SetModifyFleetButtonsStatus(false);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x0001EF9C File Offset: 0x0001D19C
	private void OnEditleet(IUIObject obj)
	{
		if (this.m_onProceed == null)
		{
			return;
		}
		if (this.m_fleetTab.m_selectedFleet.Length == 0)
		{
			return;
		}
		this.m_onProceed(this.m_fleetTab.m_selectedFleet, 0);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x0001EFD8 File Offset: 0x0001D1D8
	private void OnListPublicGamesChanged(IUIObject obj)
	{
		this.RequestGameList();
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0001EFE0 File Offset: 0x0001D1E0
	private void RequestGameList()
	{
		bool flag = this.m_playTab.m_listPublicGames.StateNum == 1;
		GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "CurrentGamesLbl").gameObject.SetActiveRecursively(!flag);
		GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "PublicGamesLbl").gameObject.SetActiveRecursively(flag);
		this.m_rpc.Invoke("RequestGameList", new object[]
		{
			flag
		});
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x0001F05C File Offset: 0x0001D25C
	private void RPC_CreateOK(PTech.RPC rpc, List<object> args)
	{
		this.m_createGame.Hide();
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0001F06C File Offset: 0x0001D26C
	private void RPC_CreateFail(PTech.RPC rpc, List<object> args)
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "Failed to create game", null);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0001F088 File Offset: 0x0001D288
	private void OnAddFriendPressed(IUIObject obj)
	{
		this.m_friendsTab.m_addFriendDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, "$add_friend", string.Empty, new GenericTextInput.InputTextCancel(this.OnAddFriendCancel), new GenericTextInput.InputTextCommit(this.OnAddFriendOk));
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0001F0D0 File Offset: 0x0001D2D0
	private void OnAddFriendCancel()
	{
		UnityEngine.Object.Destroy(this.m_friendsTab.m_addFriendDialog);
	}

	// Token: 0x060003CA RID: 970 RVA: 0x0001F0E4 File Offset: 0x0001D2E4
	private void OnAddFriendOk(string text)
	{
		UnityEngine.Object.Destroy(this.m_friendsTab.m_addFriendDialog);
		this.m_rpc.Invoke("FriendRequest", new object[]
		{
			text
		});
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0001F11C File Offset: 0x0001D31C
	private void RPC_FriendRequestReply(PTech.RPC rpc, List<object> args)
	{
		ErrorCode errorCode = (ErrorCode)((int)args[0]);
		string text;
		if (errorCode == ErrorCode.FriendUserDoesNotExist)
		{
			text = "$user_does_not_exist";
		}
		else
		{
			text = "$already_friends";
		}
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, text, null);
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0001F164 File Offset: 0x0001D364
	private void RPC_Friends(PTech.RPC rpc, List<object> args)
	{
		this.m_friendsTab.m_friends.Clear();
		foreach (object obj in args)
		{
			FriendData friendData = new FriendData();
			friendData.FromArray((byte[])obj);
			this.m_friendsTab.m_friends.Add(friendData);
		}
		float scrollPosition = this.m_friendsTab.m_friendList.ScrollPosition;
		this.m_friendsTab.m_friendList.ClearList(true);
		bool flag = false;
		foreach (FriendData friendData2 in this.m_friendsTab.m_friends)
		{
			UIListItemContainer item = null;
			if (friendData2.m_status == FriendData.FriendStatus.IsFriend)
			{
				flag = true;
				GameObject gameObject = UnityEngine.Object.Instantiate(this.m_friendsTab.m_confirmedFriendListItem) as GameObject;
				gameObject.transform.FindChild("FriendRemoveButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFriendRemove));
				SimpleSprite component = gameObject.transform.FindChild("FriendOnlineStatus").GetComponent<SimpleSprite>();
				SimpleSprite component2 = gameObject.transform.FindChild("FriendOfflineStatus").GetComponent<SimpleSprite>();
				if (friendData2.m_online)
				{
					component2.transform.Translate(new Vector3(10000f, 0f, 0f));
				}
				if (!friendData2.m_online)
				{
					component.transform.Translate(new Vector3(10000f, 0f, 0f));
				}
				SimpleSprite component3 = gameObject.transform.FindChild("FriendFlag").GetComponent<SimpleSprite>();
				Texture2D flagTexture = GuiUtils.GetFlagTexture(friendData2.m_flagID);
				GuiUtils.SetImage(component3, flagTexture);
				GuiUtils.FindChildOf(gameObject, "FriendNameLabel").GetComponent<SpriteText>().Text = friendData2.m_name;
				item = gameObject.GetComponent<UIListItemContainer>();
			}
			else if (friendData2.m_status == FriendData.FriendStatus.NeedAccept)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(this.m_friendsTab.m_friendListItem) as GameObject;
				DebugUtils.Assert(gameObject2 != null);
				gameObject2.transform.FindChild("FriendDeclineButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFriendRemove));
				gameObject2.transform.FindChild("FriendAcceptButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFriendAccept));
				item = gameObject2.GetComponent<UIListItemContainer>();
				SpriteText component4 = gameObject2.transform.FindChild("FriendRequestLabel").GetComponent<SpriteText>();
				component4.Text = Localize.instance.Translate(friendData2.m_name + " $label_friend_request");
			}
			else if (friendData2.m_status == FriendData.FriendStatus.Requested)
			{
				GameObject gameObject3 = UnityEngine.Object.Instantiate(this.m_friendsTab.m_pendingFriendListItem) as GameObject;
				gameObject3.transform.FindChild("FriendCancelButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFriendRemove));
				item = gameObject3.GetComponent<UIListItemContainer>();
				SpriteText component5 = gameObject3.transform.FindChild("FriendPendingLabel").GetComponent<SpriteText>();
				component5.Text = Localize.instance.Translate("$label_friend_request_pending " + friendData2.m_name + ".");
			}
			GuiUtils.FixedItemContainerInstance(item);
			this.m_friendsTab.m_friendList.AddItem(item);
		}
		this.m_friendsTab.m_friendList.ScrollPosition = scrollPosition;
		if (flag)
		{
			this.m_userMan.UnlockAchievement(3);
		}
	}

	// Token: 0x060003CD RID: 973 RVA: 0x0001F538 File Offset: 0x0001D738
	private void OnFriendAccept(IUIObject button)
	{
		UIListItemContainer component = button.transform.parent.GetComponent<UIListItemContainer>();
		FriendData friendData = this.m_friendsTab.m_friends[component.Index];
		this.m_userMan.UnlockAchievement(3);
		this.m_rpc.Invoke("AcceptFriendRequest", new object[]
		{
			friendData.m_friendID
		});
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0001F5A0 File Offset: 0x0001D7A0
	private void OnFriendRemove(IUIObject button)
	{
		UIListItemContainer component = button.transform.parent.GetComponent<UIListItemContainer>();
		this.m_removeFriendTempData = this.m_friendsTab.m_friends[component.Index];
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$label_defriend", new MsgBox.YesHandler(this.OnRemoveFriendConfirm), new MsgBox.NoHandler(this.OnMsgBoxOK));
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0001F608 File Offset: 0x0001D808
	private void OnRemoveFriendConfirm()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_rpc.Invoke("RemoveFriend", new object[]
		{
			this.m_removeFriendTempData.m_friendID
		});
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0001F650 File Offset: 0x0001D850
	private void OnPlayReplayID(IUIObject button)
	{
		if (this.m_archiveTab.m_playReplayDialog == null)
		{
			this.m_archiveTab.m_playReplayDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$replay_enterreplayid"), string.Empty, new GenericTextInput.InputTextCancel(this.OnPlayReplayCancel), new GenericTextInput.InputTextCommit(this.OnPlayReplayOk));
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0001F6B8 File Offset: 0x0001D8B8
	private void OnPlayReplayCancel()
	{
		UnityEngine.Object.Destroy(this.m_archiveTab.m_playReplayDialog);
		this.m_archiveTab.m_playReplayDialog = null;
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0001F6D8 File Offset: 0x0001D8D8
	private void OnPlayReplayOk(string id)
	{
		UnityEngine.Object.Destroy(this.m_archiveTab.m_playReplayDialog);
		this.m_archiveTab.m_playReplayDialog = null;
		try
		{
			int arg = int.Parse(id);
			this.m_onWatchReplay(arg, "Replay " + arg.ToString());
		}
		catch
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$replay_invalidid", new MsgBox.OkHandler(this.OnMsgBoxOK));
		}
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x0001F770 File Offset: 0x0001D970
	private void OnMsgBoxOK()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x0001F790 File Offset: 0x0001D990
	private void RPC_ArchivedGameList(PTech.RPC rpc, List<object> args)
	{
		this.m_archiveTab.m_games.Clear();
		foreach (object obj in args)
		{
			GamePost gamePost = new GamePost();
			gamePost.FromArray((byte[])obj);
			this.m_archiveTab.m_games.Add(gamePost);
		}
		this.m_archiveTab.m_games.Sort();
		this.m_archiveTab.m_gameList.ClearList(true);
		foreach (GamePost gamePost2 in this.m_archiveTab.m_games)
		{
			string text;
			if (gamePost2.m_turn >= 0)
			{
				text = (gamePost2.m_turn + 1).ToString();
			}
			else
			{
				text = "-";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_archiveTab.m_gameListItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "NameLbl").GetComponent<SpriteText>().Text = gamePost2.m_gameName;
			GuiUtils.FindChildOf(gameObject, "TypeLbl").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gametype_" + gamePost2.m_gameType.ToString().ToLower());
			GuiUtils.FindChildOf(gameObject, "MapLbl").GetComponent<SpriteText>().Text = CreateGame.TranslatedMapName(gamePost2.m_level);
			GuiUtils.FindChildOf(gameObject, "PlayersLbl").GetComponent<SpriteText>().Text = gamePost2.m_maxPlayers.ToString();
			GuiUtils.FindChildOf(gameObject, "TurnLbl").GetComponent<SpriteText>().Text = text;
			GuiUtils.FindChildOf(gameObject, "IdLbl").GetComponent<SpriteText>().Text = gamePost2.m_gameID.ToString();
			GuiUtils.FindChildOf(gameObject, "CreatedLbl").GetComponent<SpriteText>().Text = gamePost2.m_createDate.ToString("yyyy-MM-d HH:mm");
			GuiUtils.FindChildOf(gameObject, "ArchivedGameListitemRadioButton").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnReplayArchiveGameSelected));
			GuiUtils.FindChildOf(gameObject, "RemoveButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRemoveReplay));
			UIListItemContainer component = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component);
			this.m_archiveTab.m_gameList.AddItem(component);
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x0001FA38 File Offset: 0x0001DC38
	private void OnReplayArchiveGameSelected(IUIObject obj)
	{
		int index = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
		GamePost gamePost = this.m_archiveTab.m_games[index];
		this.m_onWatchReplay(gamePost.m_gameID, gamePost.m_gameName);
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0001FA84 File Offset: 0x0001DC84
	private void OnRemoveReplay(IUIObject obj)
	{
		int index = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
		GamePost gamePost = this.m_archiveTab.m_games[index];
		this.m_rpc.Invoke("RemoveReplay", new object[]
		{
			gamePost.m_gameName,
			gamePost.m_gameID
		});
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0001FAE8 File Offset: 0x0001DCE8
	private void RPC_GameList(PTech.RPC rpc, List<object> args)
	{
		this.m_playTab.m_games.Clear();
		foreach (object obj in args)
		{
			GamePost gamePost = new GamePost();
			gamePost.FromArray((byte[])obj);
			this.m_playTab.m_games.Add(gamePost);
		}
		this.m_playTab.m_games.Sort();
		float scrollPosition = this.m_playTab.m_gameList.ScrollPosition;
		this.m_playTab.m_gameList.ClearList(true);
		foreach (GamePost gamePost2 in this.m_playTab.m_games)
		{
			string text;
			if (gamePost2.m_turn >= 0)
			{
				text = (gamePost2.m_turn + 1).ToString();
			}
			else
			{
				text = "-";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_playTab.m_gameListItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "NameValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_gameName;
			GuiUtils.FindChildOf(gameObject, "TypeValueLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gametype_" + gamePost2.m_gameType.ToString().ToLower());
			GuiUtils.FindChildOf(gameObject, "MapValueLabel").GetComponent<SpriteText>().Text = CreateGame.TranslatedMapName(gamePost2.m_level);
			GuiUtils.FindChildOf(gameObject, "PlayersValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_connectedPlayers.ToString() + "/" + gamePost2.m_maxPlayers.ToString();
			GuiUtils.FindChildOf(gameObject, "TurnValueLabel").GetComponent<SpriteText>().Text = text;
			GuiUtils.FindChildOf(gameObject, "CreatedValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_createDate.ToString("yyyy-MM-dd HH:mm");
			if (gamePost2.m_needAttention)
			{
				GuiUtils.FindChildOf(gameObject, "StateIcon_Default").transform.Translate(0f, 0f, 20f);
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "StateIcon_NewTurn").transform.Translate(0f, 0f, 20f);
			}
			GuiUtils.FindChildOf(gameObject, "Button").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGameListSelection));
			UIListItemContainer component = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component);
			this.m_playTab.m_gameList.AddItem(component);
		}
		this.m_playTab.m_gameList.ScrollPosition = scrollPosition;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0001FDE8 File Offset: 0x0001DFE8
	private void FillFleetList()
	{
		this.m_fleetTab.m_selectedFleet = string.Empty;
		this.m_fleetTab.m_fleets.Clear();
		this.m_fleetTab.m_fleetList.ClearList(true);
		List<FleetDef> fleetDefs = this.m_userMan.GetFleetDefs(0);
		foreach (FleetDef fleetDef in fleetDefs)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_fleetTab.m_gameFleetItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Name").GetComponent<SpriteText>().Text = fleetDef.m_name;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Ships").GetComponent<SpriteText>().Text = fleetDef.m_ships.Count.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Points").GetComponent<SpriteText>().Text = fleetDef.m_value.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Size").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey(FleetSizes.GetSizeClassName(fleetDef.m_value));
			UIRadioBtn component = GuiUtils.FindChildOf(gameObject, "RadioButton").GetComponent<UIRadioBtn>();
			component.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFleetSelected));
			UIListItemContainer component2 = gameObject.GetComponent<UIListItemContainer>();
			if (!fleetDef.m_available)
			{
				component.controlIsEnabled = false;
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "LblDLCNotAvailable").transform.Translate(new Vector3(0f, 0f, 20f));
			}
			this.m_fleetTab.m_fleetList.AddItem(component2);
			this.m_fleetTab.m_fleets.Add(fleetDef.m_name);
		}
		this.SetModifyFleetButtonsStatus(false);
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0001FFC4 File Offset: 0x0001E1C4
	private void OnFleetSelected(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		this.m_fleetTab.m_selectedFleet = this.m_fleetTab.m_fleets[component.Index];
		this.SetModifyFleetButtonsStatus(true);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x0002000C File Offset: 0x0001E20C
	public void Update()
	{
		this.m_refreshTimer += Time.deltaTime;
		if (this.m_refreshTimer > 5f)
		{
			this.m_refreshTimer = 0f;
			if (this.m_playTab.m_panel.gameObject.active)
			{
				this.RequestGameList();
			}
			if (this.m_friendsTab.m_panel.gameObject.active)
			{
				this.RequestFriends();
			}
		}
		this.m_credits.Update(Time.deltaTime);
		this.m_shop.Update(Time.deltaTime);
		this.m_newsTicker.Update(Time.deltaTime);
	}

	// Token: 0x060003DB RID: 987 RVA: 0x000200B8 File Offset: 0x0001E2B8
	private void OnBackPressed(IUIObject obj)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$mainmenu_logout", new MsgBox.YesHandler(this.OnExitYes), new MsgBox.NoHandler(this.OnExitNo));
		}
		else if (this.m_onLogout != null)
		{
			this.Close();
			this.m_onLogout();
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00020120 File Offset: 0x0001E320
	private void OnExitYes()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		if (this.m_onLogout != null)
		{
			this.Close();
			this.m_onLogout();
		}
	}

	// Token: 0x060003DD RID: 989 RVA: 0x0002015C File Offset: 0x0001E35C
	private void OnExitNo()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00020170 File Offset: 0x0001E370
	private void OnOptionsPressed(IUIObject obj)
	{
		OptionsWindow optionsWindow = new OptionsWindow(this.m_guiCamera, false);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x0002018C File Offset: 0x0001E38C
	private void OnUserManUpdate()
	{
		this.FillFleetList();
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00020194 File Offset: 0x0001E394
	private void OnFleetEditorPressed(IUIObject obj)
	{
		if (this.m_onStartFleetEditor != null)
		{
			this.m_onStartFleetEditor();
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x000201AC File Offset: 0x0001E3AC
	private void OnShowArchive(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		UIPanel component = GuiUtils.FindChildOf(this.m_gui, "PanelArchive").GetComponent<UIPanel>();
		component.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnArchiveTransitionComplete));
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x000201F4 File Offset: 0x0001E3F4
	private void OnArchiveTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.RequestArchivedGames();
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x000201FC File Offset: 0x0001E3FC
	private void OnShowNews(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		UIPanel component = GuiUtils.FindChildOf(this.m_gui, "PanelNews").GetComponent<UIPanel>();
		component.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnNewsTransitionComplete));
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00020244 File Offset: 0x0001E444
	private void OnNewsTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.RequestNews();
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x0002024C File Offset: 0x0001E44C
	private void OnShowCreditsPressed(IUIObject obj)
	{
		this.StartCredits();
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00020254 File Offset: 0x0001E454
	private void StartCredits()
	{
		this.m_credits.Start();
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00020264 File Offset: 0x0001E464
	private void RequestNews()
	{
		this.m_rpc.Invoke("RequestNews", new object[0]);
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x0002027C File Offset: 0x0001E47C
	private void RPC_News(PTech.RPC rpc, List<object> args)
	{
		this.m_newsTab.m_newsList.ClearList(true);
		foreach (object obj in args)
		{
			byte[] data = (byte[])obj;
			NewsPost newsPost = new NewsPost(data);
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_newsTab.m_newsItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "NewsTitleLabel").GetComponent<SpriteText>().Text = newsPost.m_title;
			GuiUtils.FindChildOf(gameObject, "NewsCategoryLabel").GetComponent<SpriteText>().Text = newsPost.m_category;
			GuiUtils.FindChildOf(gameObject, "NewsDateLabel").GetComponent<SpriteText>().Text = newsPost.m_date.ToString("yyyy-MM-d HH:mm");
			GuiUtils.FindChildOf(gameObject, "NewsContentLabel").GetComponent<SpriteText>().Text = newsPost.m_content;
			UIListItemContainer component = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component);
			this.m_newsTab.m_newsList.AddItem(component);
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000203A8 File Offset: 0x0001E5A8
	private void OnShowFriends(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		UIPanel component = GuiUtils.FindChildOf(this.m_gui, "PanelFriends").GetComponent<UIPanel>();
		component.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnFriendsTransitionComplete));
		this.m_friendsTab.m_friendList.ClearList(true);
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x00020404 File Offset: 0x0001E604
	private void OnFriendsTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.RequestFriends();
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0002040C File Offset: 0x0001E60C
	private void RequestFriends()
	{
		this.m_rpc.Invoke("RequestFriends", new object[0]);
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x00020424 File Offset: 0x0001E624
	private void RequestArchivedGames()
	{
		this.m_rpc.Invoke("RequestArchivedGames", new object[0]);
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0002043C File Offset: 0x0001E63C
	private void OnShowPlay(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		UIPanel component = GuiUtils.FindChildOf(this.m_gui, "PanelPlay").GetComponent<UIPanel>();
		component.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnPlayTransitionComplete));
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00020484 File Offset: 0x0001E684
	private void OnPlayTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.RequestGameList();
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x0002048C File Offset: 0x0001E68C
	private void OnShowProfile(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (!uipanelTab.Value)
		{
			return;
		}
		UIPanel component = GuiUtils.FindChildOf(this.m_gui, "PanelProfile").GetComponent<UIPanel>();
		component.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnProfileTransitionComplete));
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x000204D4 File Offset: 0x0001E6D4
	private void OnProfileTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.RequestProfileData();
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x000204DC File Offset: 0x0001E6DC
	private void RequestProfileData()
	{
		this.m_rpc.Invoke("RequestProfile", new object[0]);
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x000204F4 File Offset: 0x0001E6F4
	private void RPC_NrOfFriendRequests(PTech.RPC rpc, List<object> args)
	{
		int num = (int)args[0];
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui, "FriendsNotificationWidget");
		SpriteText component = GuiUtils.FindChildOf(this.m_gui, "FriendsNotificationLabel").GetComponent<SpriteText>();
		if (num > 0)
		{
			component.Text = num.ToString();
			gameObject.SetActiveRecursively(true);
		}
		else
		{
			gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x0002055C File Offset: 0x0001E75C
	private void RPC_Profile(PTech.RPC rpc, List<object> args)
	{
		int num = 0;
		string userName = (string)args[num++];
		int flag = (int)args[num++];
		int mailFlags = (int)args[num++];
		int totalNrOfFlags = (int)args[num++];
		string email = (string)args[num++];
		byte[] data = (byte[])args[num++];
		UserStats userStats = new UserStats();
		userStats.FromArray(data);
		this.SetProfileData(userName, flag, mailFlags, totalNrOfFlags, email, userStats);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x000205F4 File Offset: 0x0001E7F4
	private string GetTimeString(long seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)seconds);
		string text = string.Empty;
		if (timeSpan.Days > 0)
		{
			text = text + timeSpan.Days.ToString() + " $time_days ";
		}
		if (timeSpan.Hours > 0)
		{
			text = text + timeSpan.Hours.ToString() + " $time_hours ";
		}
		if (timeSpan.Minutes > 0)
		{
			text = text + timeSpan.Minutes.ToString() + " $time_minutes ";
		}
		return Localize.instance.Translate(text);
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00020698 File Offset: 0x0001E898
	private string GetMostUsedShip(Dictionary<string, int> shipUsage)
	{
		string result = string.Empty;
		int num = 0;
		foreach (KeyValuePair<string, int> keyValuePair in shipUsage)
		{
			if (keyValuePair.Value > num)
			{
				result = keyValuePair.Key;
				num = keyValuePair.Value;
			}
		}
		return result;
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00020718 File Offset: 0x0001E918
	private string GetBestGun(Dictionary<string, UserStats.ModuleStat> moduleData)
	{
		string result = string.Empty;
		int num = 0;
		foreach (KeyValuePair<string, UserStats.ModuleStat> keyValuePair in moduleData)
		{
			if (keyValuePair.Value.m_damage > num)
			{
				result = keyValuePair.Key;
				num = keyValuePair.Value.m_damage;
			}
		}
		return result;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x000207A4 File Offset: 0x0001E9A4
	private string GetMostUsedUtilityModulen(Dictionary<string, UserStats.ModuleStat> moduleData)
	{
		string result = string.Empty;
		int num = 0;
		foreach (KeyValuePair<string, UserStats.ModuleStat> keyValuePair in moduleData)
		{
			if (keyValuePair.Value.m_uses > num)
			{
				GameObject prefab = ObjectFactory.instance.GetPrefab(keyValuePair.Key);
				if (prefab.GetComponent<HPModule>().m_type == HPModule.HPModuleType.Defensive)
				{
					result = keyValuePair.Key;
					num = keyValuePair.Value.m_uses;
				}
			}
		}
		return result;
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x00020854 File Offset: 0x0001EA54
	private void SetProfileData(string userName, int flag, int mailFlags, int totalNrOfFlags, string email, UserStats stats)
	{
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui, "PanelProfile");
		SpriteText component = GuiUtils.FindChildOf(gameObject, "UserNameLabel").GetComponent<SpriteText>();
		SpriteText component2 = GuiUtils.FindChildOf(gameObject, "UserEmailLabel").GetComponent<SpriteText>();
		SimpleSprite component3 = GuiUtils.FindChildOf(gameObject, "FlagImage").GetComponent<SimpleSprite>();
		SpriteText component4 = GuiUtils.FindChildOf(gameObject, "GamesPlayedValueLabel").GetComponent<SpriteText>();
		SpriteText component5 = GuiUtils.FindChildOf(gameObject, "TotalDamageAmountValueLabel").GetComponent<SpriteText>();
		SpriteText component6 = GuiUtils.FindChildOf(gameObject, "FFRatioValueLabel").GetComponent<SpriteText>();
		SpriteText component7 = GuiUtils.FindChildOf(gameObject, "TotalTimeValueLabel").GetComponent<SpriteText>();
		SpriteText component8 = GuiUtils.FindChildOf(gameObject, "PlanningTimeValueLabel").GetComponent<SpriteText>();
		SpriteText component9 = GuiUtils.FindChildOf(gameObject, "ShipyardTimeValueLabel").GetComponent<SpriteText>();
		SimpleSprite component10 = GuiUtils.FindChildOf(gameObject, "PreferredClassImage").GetComponent<SimpleSprite>();
		SpriteText component11 = GuiUtils.FindChildOf(gameObject, "PreferredClassValueLabel").GetComponent<SpriteText>();
		SimpleSprite component12 = GuiUtils.FindChildOf(gameObject, "DeadliestWeaponImage").GetComponent<SimpleSprite>();
		SpriteText component13 = GuiUtils.FindChildOf(gameObject, "DeadliestWeaponValueLabel").GetComponent<SpriteText>();
		SimpleSprite component14 = GuiUtils.FindChildOf(gameObject, "PreferredUtilityImage").GetComponent<SimpleSprite>();
		SpriteText component15 = GuiUtils.FindChildOf(gameObject, "PreferredUtilityValueLabel").GetComponent<SpriteText>();
		UIStateToggleBtn uistateToggleBtn = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(gameObject, "EmailNotificationsCheckbox");
		uistateToggleBtn.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMailCheckboxChanged));
		component.Text = userName;
		component2.Text = email;
		bool flag2 = (mailFlags & 1) != 0;
		uistateToggleBtn.SetState((!flag2) ? 0 : 1);
		float num = (stats.m_vsTotalDamage <= 0) ? 0f : ((float)stats.m_vsTotalFriendlyDamage / (float)stats.m_vsTotalDamage);
		component4.Text = stats.m_vsGamesWon + "/" + stats.m_vsGamesLost;
		component5.Text = stats.m_vsTotalDamage.ToString();
		component6.Text = (int)(num * 100f) + "%";
		component7.Text = this.GetTimeString(stats.m_totalPlayTime);
		component8.Text = this.GetTimeString(stats.m_totalPlanningTime);
		component9.Text = this.GetTimeString(stats.m_totalShipyardTime);
		string mostUsedShip = this.GetMostUsedShip(stats.m_vsShipUsage);
		if (mostUsedShip != string.Empty)
		{
			GuiUtils.SetImage(component10, GuiUtils.GetProfileShipSilhouette(mostUsedShip));
			component11.Text = Localize.instance.TranslateKey(mostUsedShip + "_name");
		}
		else
		{
			component10.gameObject.SetActiveRecursively(false);
			component11.Text = string.Empty;
		}
		string bestGun = this.GetBestGun(stats.m_vsModuleStats);
		if (bestGun != string.Empty)
		{
			Texture2D profileArmamentThumbnail = GuiUtils.GetProfileArmamentThumbnail(bestGun);
			if (profileArmamentThumbnail != null)
			{
				GuiUtils.SetImage(component12, profileArmamentThumbnail);
			}
			component13.Text = Localize.instance.TranslateKey(bestGun + "_name");
		}
		else
		{
			component12.gameObject.SetActiveRecursively(false);
			component13.Text = string.Empty;
		}
		string mostUsedUtilityModulen = this.GetMostUsedUtilityModulen(stats.m_vsModuleStats);
		if (mostUsedUtilityModulen != string.Empty)
		{
			Texture2D profileArmamentThumbnail2 = GuiUtils.GetProfileArmamentThumbnail(mostUsedUtilityModulen);
			if (profileArmamentThumbnail2 != null)
			{
				GuiUtils.SetImage(component14, profileArmamentThumbnail2);
			}
			component15.Text = Localize.instance.TranslateKey(mostUsedUtilityModulen + "_name");
		}
		else
		{
			component14.gameObject.SetActiveRecursively(false);
			component15.Text = string.Empty;
		}
		Texture2D flagTexture = GuiUtils.GetFlagTexture(flag);
		if (flagTexture != null)
		{
			component3.SetTexture(flagTexture);
			component3.Setup((float)flagTexture.width, (float)flagTexture.height, new Vector2(0f, (float)flagTexture.height), new Vector2((float)flagTexture.width, (float)flagTexture.height));
		}
		this.FillAchivementList(gameObject, stats.m_achievements);
		this.FillFlagList(gameObject, totalNrOfFlags);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00020C5C File Offset: 0x0001EE5C
	private void OnMailCheckboxChanged(IUIObject button)
	{
		UIStateToggleBtn uistateToggleBtn = button as UIStateToggleBtn;
		int num = 0;
		if (uistateToggleBtn.StateNum == 1)
		{
			num |= 1;
		}
		PLog.Log("Mail " + num);
		this.m_rpc.Invoke("SetMailFlags", new object[]
		{
			num
		});
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x00020CB8 File Offset: 0x0001EEB8
	private void FillAchivementList(GameObject panelProfile, Dictionary<int, long> unlocked)
	{
		GameObject original = GuiUtils.FindChildOf(this.m_gui, "AchievementListItem_Unlocked");
		GameObject original2 = GuiUtils.FindChildOf(this.m_gui, "AchievementListItem_Locked");
		UIScrollList uiscrollList = GuiUtils.FindChildOfComponent<UIScrollList>(this.m_gui, "AchievementList");
		uiscrollList.ClearList(true);
		List<KeyValuePair<int, DateTime>> list = new List<KeyValuePair<int, DateTime>>();
		foreach (KeyValuePair<int, long> keyValuePair in unlocked)
		{
			list.Add(new KeyValuePair<int, DateTime>(keyValuePair.Key, DateTime.FromBinary(keyValuePair.Value)));
		}
		list.Sort((KeyValuePair<int, DateTime> a, KeyValuePair<int, DateTime> b) => b.Value.CompareTo(a.Value));
		foreach (KeyValuePair<int, DateTime> keyValuePair2 in list)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			SpriteText spriteText = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "AchievementName");
			spriteText.Text = Localize.instance.TranslateKey("achievement_name" + keyValuePair2.Key.ToString());
			SpriteText spriteText2 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "AchievementDescription");
			spriteText2.Text = Localize.instance.TranslateKey("achievement_desc" + keyValuePair2.Key.ToString());
			SpriteText spriteText3 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "AchievementUnlockDate");
			spriteText3.Text = keyValuePair2.Value.ToString("yyyy-MM-d HH:mm");
			SimpleSprite sprite = GuiUtils.FindChildOfComponent<SimpleSprite>(gameObject, "AchievementIcon");
			Texture2D achievementIconTexture = GuiUtils.GetAchievementIconTexture(keyValuePair2.Key, true);
			if (achievementIconTexture != null)
			{
				GuiUtils.SetImage(sprite, achievementIconTexture);
			}
			GuiUtils.FixedItemContainerInstance(gameObject.GetComponent<UIListItemContainer>());
			uiscrollList.AddItem(gameObject);
		}
		foreach (int num in Constants.m_achivements)
		{
			if (!unlocked.ContainsKey(num) && !Constants.m_achivementHidden[num])
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
				SpriteText spriteText4 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject2, "AchievementName");
				spriteText4.Text = Localize.instance.TranslateKey("achievement_name" + num.ToString());
				SpriteText spriteText5 = GuiUtils.FindChildOfComponent<SpriteText>(gameObject2, "AchievementDescription");
				spriteText5.Text = Localize.instance.TranslateKey("achievement_desc" + num.ToString());
				SimpleSprite sprite2 = GuiUtils.FindChildOfComponent<SimpleSprite>(gameObject2, "AchievementIcon");
				Texture2D achievementIconTexture2 = GuiUtils.GetAchievementIconTexture(num, false);
				if (achievementIconTexture2 != null)
				{
					GuiUtils.SetImage(sprite2, achievementIconTexture2);
				}
				GuiUtils.FixedItemContainerInstance(gameObject2.GetComponent<UIListItemContainer>());
				uiscrollList.AddItem(gameObject2);
			}
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00020FC8 File Offset: 0x0001F1C8
	private void FillFlagList(GameObject panelProfile, int totalNrOfFlags)
	{
		this.m_profileTab.m_flags = this.m_userMan.GetAvailableFlags();
		UIScrollList uiscrollList = GuiUtils.FindChildOfComponent<UIScrollList>(this.m_gui, "FlagList");
		uiscrollList.ClearList(true);
		int num = 6;
		int num2 = 1 + totalNrOfFlags / num;
		for (int i = 0; i < num2; i++)
		{
			GameObject gameObject = GuiUtils.CreateGui("FlagListItem", this.m_guiCamera);
			for (int j = 0; j < num; j++)
			{
				int num3 = i * num + j;
				string text = "Flag " + (j + 1).ToString();
				UIButton uibutton = GuiUtils.FindChildOfComponent<UIButton>(gameObject, text);
				if (num3 < this.m_profileTab.m_flags.Count)
				{
					int num4 = this.m_profileTab.m_flags[num3];
					Texture2D flagTexture = GuiUtils.GetFlagTexture(num4);
					DebugUtils.Assert(flagTexture != null, "Missing flag " + num4);
					DebugUtils.Assert(uibutton != null, "Missing button " + text);
					GuiUtils.SetButtonImage(uibutton, flagTexture);
					uibutton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFlagListSelection));
					uibutton.name = num4.ToString();
				}
				else if (num3 >= totalNrOfFlags)
				{
					uibutton.transform.Translate(new Vector3(10000f, 0f, 0f));
				}
			}
			uiscrollList.AddItem(gameObject);
		}
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x0002113C File Offset: 0x0001F33C
	private void OnFlagListSelection(IUIObject obj)
	{
		int flag = int.Parse(obj.name);
		this.m_userMan.SetFlag(flag);
		this.RequestProfileData();
		UIPanelManager uipanelManager = GuiUtils.FindChildOfComponent<UIPanelManager>(this.m_gui, "FlagSelectionPanelMan");
		UIPanelBase currentPanel = uipanelManager.CurrentPanel;
		uipanelManager.Dismiss();
		currentPanel.Dismiss();
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0002118C File Offset: 0x0001F38C
	public string GetLocalUserName()
	{
		return this.m_localUserName;
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00021194 File Offset: 0x0001F394
	private void OnGameListSelection(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		GamePost gamePost = this.m_playTab.m_games[component.Index];
		this.m_onJoin(gamePost.m_gameID);
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000211DC File Offset: 0x0001F3DC
	private void OnOpenFaceBook(IUIObject obj)
	{
		Application.OpenURL("http://www.facebook.com/LeviathanWarships");
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x000211E8 File Offset: 0x0001F3E8
	private void OnOpenTwitter(IUIObject obj)
	{
		Application.OpenURL("http://www.twitter.com/LeviathanGame");
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x000211F4 File Offset: 0x0001F3F4
	private void OnOpenForum(IUIObject obj)
	{
		Application.OpenURL("http://forum.paradoxplaza.com/forum/forumdisplay.php?771-Leviathan-Warships");
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00021200 File Offset: 0x0001F400
	private void OnItemBought()
	{
		if (this.m_onItemBought != null)
		{
			this.m_onItemBought();
		}
	}

	// Token: 0x0400031A RID: 794
	public Action<string, int> m_onProceed;

	// Token: 0x0400031B RID: 795
	public Action<int> m_onJoin;

	// Token: 0x0400031C RID: 796
	public Action<int, string> m_onWatchReplay;

	// Token: 0x0400031D RID: 797
	public Action m_onLogout;

	// Token: 0x0400031E RID: 798
	public Action m_onStartFleetEditor;

	// Token: 0x0400031F RID: 799
	public Action m_onItemBought;

	// Token: 0x04000320 RID: 800
	private PTech.RPC m_rpc;

	// Token: 0x04000321 RID: 801
	private GameObject m_guiCamera;

	// Token: 0x04000322 RID: 802
	private CreateGame m_createGame;

	// Token: 0x04000323 RID: 803
	private MapMan m_mapMan;

	// Token: 0x04000324 RID: 804
	private UserManClient m_userMan;

	// Token: 0x04000325 RID: 805
	private MsgBox m_msgBox;

	// Token: 0x04000326 RID: 806
	private NewsTicker m_newsTicker;

	// Token: 0x04000327 RID: 807
	private GameObject m_gui;

	// Token: 0x04000328 RID: 808
	private UIPanelManager m_panelMan;

	// Token: 0x04000329 RID: 809
	private OnlineMenu.NewsTab m_newsTab = new OnlineMenu.NewsTab();

	// Token: 0x0400032A RID: 810
	private OnlineMenu.PlayTab m_playTab = new OnlineMenu.PlayTab();

	// Token: 0x0400032B RID: 811
	private OnlineMenu.ArchiveTab m_archiveTab = new OnlineMenu.ArchiveTab();

	// Token: 0x0400032C RID: 812
	private OnlineMenu.FriendsTab m_friendsTab = new OnlineMenu.FriendsTab();

	// Token: 0x0400032D RID: 813
	private OnlineMenu.FleetTab m_fleetTab = new OnlineMenu.FleetTab();

	// Token: 0x0400032E RID: 814
	private OnlineMenu.ProfileTab m_profileTab = new OnlineMenu.ProfileTab();

	// Token: 0x0400032F RID: 815
	private OnlineMenu.MatchMaking m_matchMaking = new OnlineMenu.MatchMaking();

	// Token: 0x04000330 RID: 816
	private string m_localUserName;

	// Token: 0x04000331 RID: 817
	private float m_refreshTimer;

	// Token: 0x04000332 RID: 818
	private Credits m_credits;

	// Token: 0x04000333 RID: 819
	private Shop m_shop;

	// Token: 0x04000334 RID: 820
	private FriendData m_removeFriendTempData;

	// Token: 0x04000335 RID: 821
	private MusicManager m_musicMan;

	// Token: 0x04000336 RID: 822
	private GDPBackend m_gdpBackend;

	// Token: 0x02000058 RID: 88
	private class NewsTab
	{
		// Token: 0x04000338 RID: 824
		public GameObject m_newsItem;

		// Token: 0x04000339 RID: 825
		public UIScrollList m_newsList;
	}

	// Token: 0x02000059 RID: 89
	private class PlayTab
	{
		// Token: 0x0400033A RID: 826
		public List<GamePost> m_games = new List<GamePost>();

		// Token: 0x0400033B RID: 827
		public UIScrollList m_gameList;

		// Token: 0x0400033C RID: 828
		public GameObject m_gameListItem;

		// Token: 0x0400033D RID: 829
		public UIPanel m_panel;

		// Token: 0x0400033E RID: 830
		public UIStateToggleBtn m_listPublicGames;
	}

	// Token: 0x0200005A RID: 90
	private class ArchiveTab
	{
		// Token: 0x0400033F RID: 831
		public List<GamePost> m_games = new List<GamePost>();

		// Token: 0x04000340 RID: 832
		public UIScrollList m_gameList;

		// Token: 0x04000341 RID: 833
		public GameObject m_gameListItem;

		// Token: 0x04000342 RID: 834
		public GameObject m_playReplayDialog;
	}

	// Token: 0x0200005B RID: 91
	private class FriendsTab
	{
		// Token: 0x04000343 RID: 835
		public UIPanel m_panel;

		// Token: 0x04000344 RID: 836
		public List<FriendData> m_friends = new List<FriendData>();

		// Token: 0x04000345 RID: 837
		public UIScrollList m_friendList;

		// Token: 0x04000346 RID: 838
		public GameObject m_friendListItem;

		// Token: 0x04000347 RID: 839
		public GameObject m_confirmedFriendListItem;

		// Token: 0x04000348 RID: 840
		public GameObject m_pendingFriendListItem;

		// Token: 0x04000349 RID: 841
		public GameObject m_addFriendDialog;
	}

	// Token: 0x0200005C RID: 92
	private class FleetTab
	{
		// Token: 0x0400034A RID: 842
		public GameObject m_gameFleetItem;

		// Token: 0x0400034B RID: 843
		public List<string> m_fleets = new List<string>();

		// Token: 0x0400034C RID: 844
		public UIScrollList m_fleetList;

		// Token: 0x0400034D RID: 845
		public string m_selectedFleet;
	}

	// Token: 0x0200005D RID: 93
	private class ProfileTab
	{
		// Token: 0x0400034E RID: 846
		public List<int> m_flags;
	}

	// Token: 0x0200005E RID: 94
	private class MatchMaking
	{
		// Token: 0x0400034F RID: 847
		public GameObject m_progressDialog;

		// Token: 0x04000350 RID: 848
		public GameObject m_panel;

		// Token: 0x04000351 RID: 849
		public UIRadioBtn m_gameTypeCampaign;

		// Token: 0x04000352 RID: 850
		public UIRadioBtn m_gameTypeChallenge;

		// Token: 0x04000353 RID: 851
		public UIRadioBtn m_gameTypePoints;

		// Token: 0x04000354 RID: 852
		public UIRadioBtn m_gameTypeAssassination;

		// Token: 0x04000355 RID: 853
		public UIRadioBtn m_playersAny;

		// Token: 0x04000356 RID: 854
		public UIRadioBtn m_playersTwo;

		// Token: 0x04000357 RID: 855
		public UIRadioBtn m_playersThree;

		// Token: 0x04000358 RID: 856
		public UIRadioBtn m_playersFour;

		// Token: 0x04000359 RID: 857
		public UIRadioBtn m_fleetSizeAny;

		// Token: 0x0400035A RID: 858
		public UIRadioBtn m_fleetSizeSmall;

		// Token: 0x0400035B RID: 859
		public UIRadioBtn m_fleetSizeMedium;

		// Token: 0x0400035C RID: 860
		public UIRadioBtn m_fleetSizeLarge;

		// Token: 0x0400035D RID: 861
		public UIRadioBtn m_targetPointsEnabled;

		// Token: 0x0400035E RID: 862
		public UIRadioBtn m_targetPointsDisabled;

		// Token: 0x0400035F RID: 863
		public UIButton m_targetPointsMinus;

		// Token: 0x04000360 RID: 864
		public UIButton m_targetPointsPlus;

		// Token: 0x04000361 RID: 865
		public SpriteText m_targetPointsPercentage;

		// Token: 0x04000362 RID: 866
		public SpriteText m_targetPointsConversion;

		// Token: 0x04000363 RID: 867
		public UIStateToggleBtn m_anyTurnTime;

		// Token: 0x04000364 RID: 868
		public List<KeyValuePair<UIStateToggleBtn, double>> m_turnTimeCheckBoxes = new List<KeyValuePair<UIStateToggleBtn, double>>();

		// Token: 0x04000365 RID: 869
		public int m_targetPointsValue = 60;

		// Token: 0x04000366 RID: 870
		public List<double> m_turnTimes = new List<double>();
	}
}
