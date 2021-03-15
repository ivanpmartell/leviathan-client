using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000054 RID: 84
internal class OfflineMenu : MenuBase
{
	// Token: 0x0600038B RID: 907 RVA: 0x0001C700 File Offset: 0x0001A900
	public OfflineMenu(GameObject guiCamera, UserManClient userMan, OfflineGameDB gameDB, MenuBase.StartStatus startStatus, MusicManager musicMan)
	{
		this.m_userMan = userMan;
		this.m_gameDB = gameDB;
		this.m_guiCamera = guiCamera;
		this.m_musicMan = musicMan;
		this.m_gui = GuiUtils.CreateGui("MainmenuOffline", guiCamera);
		this.m_panelMan = GuiUtils.FindChildOf(this.m_gui, "PanelMan").GetComponent<UIPanelManager>();
		GuiUtils.FindChildOf(this.m_gui, "VersionLbl").GetComponent<SpriteText>().Text = Localize.instance.Translate("$version " + VersionInfo.GetFullVersionString());
		GuiUtils.FindChildOf(this.m_gui, "CreateGameButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenCreateGame));
		GuiUtils.FindChildOf(this.m_gui, "ButtonPlay").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowPlay));
		GuiUtils.FindChildOf(this.m_gui, "ButtonCredits").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowCreditsPressed));
		GuiUtils.FindChildOf(this.m_gui, "ExitButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnBackPressed));
		GuiUtils.FindChildOf(this.m_gui, "OptionsButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOptionsPressed));
		GuiUtils.FindChildOf(this.m_gui, "ButtonArchive").GetComponent<UIPanelTab>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnShowArchive));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		GameObject gameObject = this.m_gui.transform.FindChild("CreateGameView").gameObject;
		this.m_createGame = new CreateGame(this.m_guiCamera, gameObject, this.m_mapMan, this.m_userMan);
		this.m_createGame.m_onClose = new Action(this.OnCreateGameClose);
		this.m_createGame.m_onCreateGame = new CreateGame.CreateDelegate(this.OnCreateGame);
		this.m_playTab.m_gameListItem = GuiUtils.CreateGui("gamelist/Gamelist_listitem", this.m_guiCamera);
		this.m_playTab.m_gameListItem.transform.Translate(100000f, 0f, 0f);
		this.m_playTab.m_gameList = GuiUtils.FindChildOf(this.m_gui, "CurrentGamesScrollList").GetComponent<UIScrollList>();
		this.m_playTab.m_panel = GuiUtils.FindChildOf(this.m_gui, "PanelPlay").GetComponent<UIPanel>();
		this.m_archiveTab.m_gameListItem = GuiUtils.FindChildOf(this.m_gui, "ArchivedGameListItem");
		this.m_archiveTab.m_gameList = GuiUtils.FindChildOf(this.m_gui, "ArchivedGamesScrollList").GetComponent<UIScrollList>();
		GameObject creditsRoot = GuiUtils.FindChildOf(this.m_gui, "PanelCredits");
		this.m_credits = new Credits(creditsRoot, this.m_musicMan);
		if (PlayerPrefs.GetInt("playedtutorial", 0) != 1)
		{
			this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$mainmenu_starttutorial", new MsgBox.YesHandler(this.OnStartTutorialYes), new MsgBox.NoHandler(this.OnStartTutorialNo));
		}
		switch (startStatus)
		{
		case MenuBase.StartStatus.ShowArchiveView:
			this.m_panelMan.BringIn("PanelArchive");
			this.FillReplayGameList();
			return;
		case MenuBase.StartStatus.ShowCredits:
			this.m_credits.Start();
			return;
		}
		this.m_panelMan.BringIn("PanelPlay");
		this.FillGameList();
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0001CAA8 File Offset: 0x0001ACA8
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

	// Token: 0x0600038D RID: 909 RVA: 0x0001CB08 File Offset: 0x0001AD08
	private void OnStartTutorialYes()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		PlayerPrefs.SetInt("playedtutorial", 1);
		this.m_createGame.CreateCampaignGame("t1_name", 0, 1);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0001CB3C File Offset: 0x0001AD3C
	private void OnStartTutorialNo()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		PlayerPrefs.SetInt("playedtutorial", 1);
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0001CB5C File Offset: 0x0001AD5C
	private void OnOpenCreateGame(IUIObject obj)
	{
		this.m_createGame.Show();
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0001CB6C File Offset: 0x0001AD6C
	private void OnCreateGameClose()
	{
		this.m_panelMan.BringIn("PanelPlay");
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0001CB80 File Offset: 0x0001AD80
	private void OnCreateGame(GameType gametype, string campaign, string mapName, int players, int fleetSize, float targetScore, double turnTime, bool autoJoin)
	{
		this.m_onStartLevel(gametype, campaign, mapName);
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0001CB90 File Offset: 0x0001AD90
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

	// Token: 0x06000393 RID: 915 RVA: 0x0001CBD8 File Offset: 0x0001ADD8
	private void OnPlayTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.FillGameList();
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0001CBE0 File Offset: 0x0001ADE0
	private void OnShowCreditsPressed(IUIObject obj)
	{
		this.m_credits.Start();
	}

	// Token: 0x06000395 RID: 917 RVA: 0x0001CBF0 File Offset: 0x0001ADF0
	private void FillGameList()
	{
		this.m_playTab.m_games.Clear();
		this.m_playTab.m_games = this.m_gameDB.GetGameList();
		this.m_playTab.m_games.Sort();
		float scrollPosition = this.m_playTab.m_gameList.ScrollPosition;
		this.m_playTab.m_gameList.ClearList(true);
		foreach (GamePost gamePost in this.m_playTab.m_games)
		{
			string text;
			if (gamePost.m_turn >= 0)
			{
				text = (gamePost.m_turn + 1).ToString();
			}
			else
			{
				text = "-";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_playTab.m_gameListItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "NameValueLabel").GetComponent<SpriteText>().Text = gamePost.m_gameName;
			GuiUtils.FindChildOf(gameObject, "TypeValueLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gametype_" + gamePost.m_gameType.ToString().ToLower());
			GuiUtils.FindChildOf(gameObject, "MapValueLabel").GetComponent<SpriteText>().Text = CreateGame.TranslatedMapName(gamePost.m_level);
			GuiUtils.FindChildOf(gameObject, "PlayersValueLabel").GetComponent<SpriteText>().Text = gamePost.m_connectedPlayers.ToString() + "/" + gamePost.m_maxPlayers.ToString();
			GuiUtils.FindChildOf(gameObject, "TurnValueLabel").GetComponent<SpriteText>().Text = text;
			GuiUtils.FindChildOf(gameObject, "CreatedValueLabel").GetComponent<SpriteText>().Text = gamePost.m_createDate.ToString("yyyy-MM-d HH:mm");
			GuiUtils.FindChildOf(gameObject, "Button").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGameListSelection));
			UIListItemContainer component = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component);
			this.m_playTab.m_gameList.AddItem(component);
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x0001CE20 File Offset: 0x0001B020
	private void OnGameListSelection(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		GamePost gamePost = this.m_playTab.m_games[component.Index];
		this.m_onJoin(gamePost.m_gameID);
	}

	// Token: 0x06000397 RID: 919 RVA: 0x0001CE68 File Offset: 0x0001B068
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

	// Token: 0x06000398 RID: 920 RVA: 0x0001CEB0 File Offset: 0x0001B0B0
	private void OnArchiveTransitionComplete(UIPanelBase panel, EZTransition transition)
	{
		this.FillReplayGameList();
	}

	// Token: 0x06000399 RID: 921 RVA: 0x0001CEB8 File Offset: 0x0001B0B8
	private void FillReplayGameList()
	{
		this.m_archiveTab.m_games = this.m_gameDB.GetReplayList();
		this.m_archiveTab.m_games.Sort();
		this.m_archiveTab.m_gameList.ClearList(true);
		foreach (GamePost gamePost in this.m_archiveTab.m_games)
		{
			string text;
			if (gamePost.m_turn >= 0)
			{
				text = (gamePost.m_turn + 1).ToString();
			}
			else
			{
				text = "-";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_archiveTab.m_gameListItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "NameLbl").GetComponent<SpriteText>().Text = gamePost.m_gameName;
			GuiUtils.FindChildOf(gameObject, "TypeLbl").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gametype_" + gamePost.m_gameType.ToString().ToLower());
			GuiUtils.FindChildOf(gameObject, "MapLbl").GetComponent<SpriteText>().Text = CreateGame.TranslatedMapName(gamePost.m_level);
			GuiUtils.FindChildOf(gameObject, "PlayersLbl").GetComponent<SpriteText>().Text = gamePost.m_maxPlayers.ToString();
			GuiUtils.FindChildOf(gameObject, "TurnLbl").GetComponent<SpriteText>().Text = text;
			GuiUtils.FindChildOf(gameObject, "CreatedLbl").GetComponent<SpriteText>().Text = gamePost.m_createDate.ToString("yyyy-MM-d HH:mm");
			GuiUtils.FindChildOf(gameObject, "ArchivedGameListitemRadioButton").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnReplayArchiveGameSelected));
			GuiUtils.FindChildOf(gameObject, "RemoveButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRemoveReplay));
			UIListItemContainer component = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component);
			this.m_archiveTab.m_gameList.AddItem(component);
		}
		this.m_archiveTab.m_gameList.ScrollToItem(0, 0f);
	}

	// Token: 0x0600039A RID: 922 RVA: 0x0001D0E0 File Offset: 0x0001B2E0
	private void OnReplayArchiveGameSelected(IUIObject obj)
	{
		int index = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
		GamePost gamePost = this.m_archiveTab.m_games[index];
		this.m_onWatchReplay(gamePost.m_gameID, gamePost.m_gameName);
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0001D12C File Offset: 0x0001B32C
	private void OnRemoveReplay(IUIObject obj)
	{
		int index = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
		GamePost gamePost = this.m_archiveTab.m_games[index];
		this.m_gameDB.RemoveReplay(gamePost.m_gameID);
		this.FillReplayGameList();
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0001D178 File Offset: 0x0001B378
	private void OnOptionsPressed(IUIObject obj)
	{
		OptionsWindow optionsWindow = new OptionsWindow(this.m_guiCamera, false);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0001D194 File Offset: 0x0001B394
	public void Close()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_credits = null;
		UnityEngine.Object.Destroy(this.m_gui);
		UnityEngine.Object.Destroy(this.m_playTab.m_gameListItem);
		UnityEngine.Object.Destroy(this.m_archiveTab.m_gameListItem);
		this.m_gui = null;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x0001D1F8 File Offset: 0x0001B3F8
	public void Update(float dt)
	{
		this.m_credits.Update(dt);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x0001D208 File Offset: 0x0001B408
	private void OnBackPressed(IUIObject obj)
	{
		if (this.m_onBack != null)
		{
			this.Close();
			this.m_onBack();
		}
	}

	// Token: 0x04000303 RID: 771
	public OfflineMenu.BackHandler m_onBack;

	// Token: 0x04000304 RID: 772
	public Action<int> m_onJoin;

	// Token: 0x04000305 RID: 773
	public Action<int, string> m_onWatchReplay;

	// Token: 0x04000306 RID: 774
	public OfflineMenu.StartLevelHandler m_onStartLevel;

	// Token: 0x04000307 RID: 775
	private MapMan m_mapMan = new ClientMapMan();

	// Token: 0x04000308 RID: 776
	private UserManClient m_userMan;

	// Token: 0x04000309 RID: 777
	private OfflineGameDB m_gameDB;

	// Token: 0x0400030A RID: 778
	private GameObject m_guiCamera;

	// Token: 0x0400030B RID: 779
	private CreateGame m_createGame;

	// Token: 0x0400030C RID: 780
	private GameObject m_gui;

	// Token: 0x0400030D RID: 781
	private UIPanelManager m_panelMan;

	// Token: 0x0400030E RID: 782
	private OfflineMenu.PlayTab m_playTab = new OfflineMenu.PlayTab();

	// Token: 0x0400030F RID: 783
	private OfflineMenu.ArchiveTab m_archiveTab = new OfflineMenu.ArchiveTab();

	// Token: 0x04000310 RID: 784
	private MsgBox m_msgBox;

	// Token: 0x04000311 RID: 785
	private Credits m_credits;

	// Token: 0x04000312 RID: 786
	private MusicManager m_musicMan;

	// Token: 0x02000055 RID: 85
	private class PlayTab
	{
		// Token: 0x04000313 RID: 787
		public List<GamePost> m_games = new List<GamePost>();

		// Token: 0x04000314 RID: 788
		public UIScrollList m_gameList;

		// Token: 0x04000315 RID: 789
		public GameObject m_gameListItem;

		// Token: 0x04000316 RID: 790
		public UIPanel m_panel;
	}

	// Token: 0x02000056 RID: 86
	private class ArchiveTab
	{
		// Token: 0x04000317 RID: 791
		public List<GamePost> m_games = new List<GamePost>();

		// Token: 0x04000318 RID: 792
		public UIScrollList m_gameList;

		// Token: 0x04000319 RID: 793
		public GameObject m_gameListItem;
	}

	// Token: 0x020001A8 RID: 424
	// (Invoke) Token: 0x06000F50 RID: 3920
	public delegate void BackHandler();

	// Token: 0x020001A9 RID: 425
	// (Invoke) Token: 0x06000F54 RID: 3924
	public delegate void StartLevelHandler(GameType mode, string campaign, string levelName);
}
