using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class Hud
{
	// Token: 0x060002FD RID: 765 RVA: 0x00017038 File Offset: 0x00015238
	public Hud(PTech.RPC rpc, GameObject guiCamera, TurnMan turnMan, ChatClient chatClient, bool replayMode, bool onlineGame)
	{
		this.m_rpc = rpc;
		this.m_guiCamera = guiCamera;
		this.m_turnMan = turnMan;
		this.m_chatClient = chatClient;
		this.m_onlineGame = onlineGame;
		this.m_replayMode = replayMode;
		this.m_shipTagMan = new ShipTagMan(guiCamera);
		this.SetupGui();
		this.SetupChat();
		this.SetupIngameMenu();
		this.m_messageLog = new MessageLog(this.m_rpc, this.m_guiCamera);
		this.SetupBattlebar(GameType.None);
		this.SetControlPanel(Hud.Mode.Waiting);
		this.SetVisible(false, true, true);
		this.m_rpc.Register("GameList", new PTech.RPC.Handler(this.RPC_GameList));
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00017118 File Offset: 0x00015318
	private void SetupGui()
	{
		this.m_gui = GuiUtils.CreateGui("IngameGui/Hud NEW", this.m_guiCamera);
		this.m_missionLog = new MissionLog(this.m_gui, this.m_guiCamera);
		this.m_gameNameLabel = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "GamenameLabel");
		this.m_hideHudButton = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_gui, "HideUIButton");
		this.m_hideHudButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnHideGui));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton1").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton2").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		GuiUtils.FindChildOf(this.m_gui, "HelpButton3").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		this.m_currentTurn = GuiUtils.FindChildOf(this.m_gui, "TurnLabel").GetComponent<SpriteText>();
		this.m_battlebarAssassinate = GuiUtils.FindChildOf(this.m_gui, "Battlebar_Versus_Assassinate");
		this.m_battlebarPoints = GuiUtils.FindChildOf(this.m_gui, "Battlebar_Versus_Points");
		this.m_battlebarCampaign = GuiUtils.FindChildOf(this.m_gui, "Battlebar_Campaign_Challenge");
		this.m_controlPanelPlanning = GuiUtils.FindChildOf(this.m_gui, "ControlPanel_Planning");
		this.m_restartOutcomeButton = GuiUtils.FindChildOf(this.m_controlPanelPlanning, "Replay_Button").GetComponent<UIButton>();
		this.m_restartOutcomeButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRestartOutcomePressed));
		this.m_commitButton = GuiUtils.FindChildOf(this.m_controlPanelPlanning, "Commit_Button").GetComponent<UIButton>();
		this.m_commitButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCommitPressed));
		this.m_turnTimeRemainingLabel = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "LblPlanningTimer");
		this.m_turnTimeRemainingLabel.Text = string.Empty;
		this.m_controlPanelOutcome = GuiUtils.FindChildOf(this.m_gui, "ControlPanel_Outcome_Replay_Record");
		this.m_nextTurnButton = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Next_Button").GetComponent<UIButton>();
		this.m_nextTurnButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnNextTurnPressed));
		this.m_prevTurnButton = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Prev_Button").GetComponent<UIButton>();
		this.m_prevTurnButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPrevTurnPressed));
		this.m_startOutcomeButton = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Play_Button").GetComponent<UIButton>();
		this.m_startOutcomeButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnStartOutcomePressed));
		this.m_stopOutcomeButton = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Stop_Button").GetComponent<UIButton>();
		this.m_stopOutcomeButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnStopOutcomePressed));
		this.m_pauseOutcomeButton = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Pause_Button").GetComponent<UIButton>();
		this.m_pauseOutcomeButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPauseOutcomePressed));
		this.m_disbandGameButton = GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "BtnDisband");
		if (this.m_disbandGameButton != null)
		{
			this.m_disbandGameButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnDisbandGame));
		}
		UIPanelTab uipanelTab = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarAssassinate, "Button_SwitchGame");
		UIPanelTab uipanelTab2 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarPoints, "Button_SwitchGame");
		UIPanelTab uipanelTab3 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarCampaign, "Button_SwitchGame");
		uipanelTab.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenSwitchGame));
		uipanelTab2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenSwitchGame));
		uipanelTab3.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenSwitchGame));
		if (!this.m_onlineGame)
		{
			uipanelTab.Hide(true);
			uipanelTab2.Hide(true);
			uipanelTab3.Hide(true);
		}
		this.m_panManMainMenu = GuiUtils.FindChildOfComponent<UIPanelManager>(this.m_gui, "MainMenuPanelMan");
		this.m_chat.m_panMan = GuiUtils.FindChildOfComponent<UIPanelManager>(this.m_gui, "ChatPanelMan");
		UIPanelTab uipanelTab4 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarAssassinate, "Button_TeamChat");
		UIPanelTab uipanelTab5 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarPoints, "Button_TeamChat");
		uipanelTab4.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnTeamChatOpen));
		uipanelTab5.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnTeamChatOpen));
		if (!this.m_onlineGame)
		{
			uipanelTab4.Hide(true);
			uipanelTab5.Hide(true);
		}
		UIPanelTab uipanelTab6 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarAssassinate, "Button_GlobalChat");
		UIPanelTab uipanelTab7 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarPoints, "Button_GlobalChat");
		UIPanelTab uipanelTab8 = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarCampaign, "Button_GlobalChat");
		uipanelTab6.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGlobalChatOpen));
		uipanelTab7.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGlobalChatOpen));
		uipanelTab8.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGlobalChatOpen));
		if (!this.m_onlineGame)
		{
			uipanelTab6.Hide(true);
			uipanelTab7.Hide(true);
			uipanelTab8.Hide(true);
		}
		this.m_statusTexts.Add(Hud.Mode.Waiting, GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Status_Commit"));
		this.m_statusTexts.Add(Hud.Mode.Planning, GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Status_Planning"));
		this.m_statusTexts.Add(Hud.Mode.Outcome, GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Status_Outcome"));
		this.m_statusTexts.Add(Hud.Mode.Replay, GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Status_Record"));
		this.m_statusTexts.Add(Hud.Mode.ReplayOutcome, GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Status_Replay"));
		this.m_newTurnSoundPrefab = (Resources.Load("NewTurnSound") as GameObject);
		this.m_FPS_label = GuiUtils.FindChildOf(this.m_gui, "lblFPS").GetComponent<SpriteText>();
		this.m_FPS_label.Text = string.Empty;
		this.m_FPS_label.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x060002FF RID: 767 RVA: 0x000176E0 File Offset: 0x000158E0
	public void SetBattleBarButtonState(string name, int state)
	{
		UIPanelTab uipanelTab = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarAssassinate, name);
		if (uipanelTab != null && uipanelTab.controlIsEnabled)
		{
			uipanelTab.SetState(state);
		}
		uipanelTab = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarPoints, name);
		if (uipanelTab != null && uipanelTab.controlIsEnabled)
		{
			uipanelTab.SetState(state);
		}
		uipanelTab = GuiUtils.FindChildOfComponent<UIPanelTab>(this.m_battlebarCampaign, name);
		if (uipanelTab != null && uipanelTab.controlIsEnabled)
		{
			uipanelTab.SetState(state);
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x00017770 File Offset: 0x00015970
	public bool DismissAnyPopup()
	{
		if (this.m_panManMainMenu.CurrentPanel)
		{
			this.SetBattleBarButtonState("Button_SwitchGame", 1);
			this.SetBattleBarButtonState("Button_MainMenu", 1);
			this.m_panManMainMenu.Dismiss();
			return true;
		}
		if (this.m_chat.m_panMan.CurrentPanel)
		{
			this.SetBattleBarButtonState("Button_TeamChat", 1);
			this.SetBattleBarButtonState("Button_GlobalChat", 1);
			this.SetBattleBarButtonState("Button_Objectives", 1);
			this.m_chat.m_panMan.Dismiss();
			return true;
		}
		return false;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00017808 File Offset: 0x00015A08
	private void OnTeamChatOpen(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (uipanelTab.Value)
		{
			UIManager.instance.FocusObject = this.m_chat.m_teamTextInput;
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0001783C File Offset: 0x00015A3C
	private void OnGlobalChatOpen(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (uipanelTab.Value)
		{
			UIManager.instance.FocusObject = this.m_chat.m_globalTextInput;
		}
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00017870 File Offset: 0x00015A70
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

	// Token: 0x06000304 RID: 772 RVA: 0x000178D0 File Offset: 0x00015AD0
	private void OnOpenSwitchGame(IUIObject obj)
	{
		UIPanelTab uipanelTab = obj as UIPanelTab;
		if (uipanelTab.Value)
		{
			this.m_rpc.Invoke("RequestGameList", new object[]
			{
				false
			});
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00017910 File Offset: 0x00015B10
	private void RPC_GameList(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("Gto games ");
		this.m_switchGameList.Clear();
		foreach (object obj in args)
		{
			GamePost gamePost = new GamePost();
			gamePost.FromArray((byte[])obj);
			if (gamePost.m_gameID != this.m_gameID)
			{
				this.m_switchGameList.Add(gamePost);
			}
		}
		this.m_switchGameList.Sort();
		UIScrollList uiscrollList = GuiUtils.FindChildOfComponent<UIScrollList>(this.m_gui, "SwitchGame_Scrollist");
		uiscrollList.ClearList(true);
		foreach (GamePost gamePost2 in this.m_switchGameList)
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
			GameObject gameObject = GuiUtils.CreateGui("gamelist/Gamelist_listitem", this.m_guiCamera);
			GuiUtils.FindChildOf(gameObject, "NameValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_gameName;
			GuiUtils.FindChildOf(gameObject, "TypeValueLabel").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gametype_" + gamePost2.m_gameType.ToString().ToLower());
			GuiUtils.FindChildOf(gameObject, "MapValueLabel").GetComponent<SpriteText>().Text = CreateGame.TranslatedMapName(gamePost2.m_level);
			GuiUtils.FindChildOf(gameObject, "PlayersValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_connectedPlayers.ToString() + "/" + gamePost2.m_maxPlayers.ToString();
			GuiUtils.FindChildOf(gameObject, "TurnValueLabel").GetComponent<SpriteText>().Text = text;
			GuiUtils.FindChildOf(gameObject, "CreatedValueLabel").GetComponent<SpriteText>().Text = gamePost2.m_createDate.ToString("yyyy-MM-d HH:mm");
			if (gamePost2.m_needAttention)
			{
				GuiUtils.FindChildOf(gameObject, "StateIcon_Default").transform.Translate(0f, 0f, 20f);
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "StateIcon_NewTurn").transform.Translate(0f, 0f, 20f);
			}
			GuiUtils.FindChildOfComponent<UIButton>(gameObject, "Button").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSwitchGamePressed));
			uiscrollList.AddItem(gameObject);
		}
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00017BD8 File Offset: 0x00015DD8
	private void OnSwitchGamePressed(IUIObject obj)
	{
		int index = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
		GamePost gamePost = this.m_switchGameList[index];
		this.m_onExit(ExitState.JoinGame, gamePost.m_gameID);
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00017C1C File Offset: 0x00015E1C
	private void SetControlPanel(Hud.Mode type)
	{
		if (type == Hud.Mode.Waiting)
		{
			this.m_controlPanelPlanning.GetComponent<UIPanel>().Dismiss();
			this.m_controlPanelOutcome.GetComponent<UIPanel>().Dismiss();
		}
		else if (type == Hud.Mode.Planning)
		{
			this.m_controlPanelPlanning.GetComponent<UIPanel>().BringIn();
			this.m_controlPanelOutcome.GetComponent<UIPanel>().Dismiss();
			this.m_timeline = GuiUtils.FindChildOf(this.m_controlPanelPlanning, "Timeline SuperSprite");
		}
		else if (type == Hud.Mode.Outcome)
		{
			this.m_controlPanelPlanning.GetComponent<UIPanel>().Dismiss();
			this.m_controlPanelOutcome.GetComponent<UIPanel>().BringIn();
			this.m_timeline = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Timeline SuperSprite");
			this.m_stopOutcomeButton.gameObject.SetActiveRecursively(false);
			this.m_nextTurnButton.gameObject.SetActiveRecursively(false);
			this.m_prevTurnButton.gameObject.SetActiveRecursively(false);
		}
		else if (type == Hud.Mode.Replay)
		{
			this.m_controlPanelPlanning.GetComponent<UIPanel>().Dismiss();
			this.m_controlPanelOutcome.GetComponent<UIPanel>().BringIn();
			this.m_timeline = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Timeline SuperSprite");
			this.m_stopOutcomeButton.gameObject.SetActiveRecursively(true);
			this.m_nextTurnButton.gameObject.SetActiveRecursively(true);
			this.m_prevTurnButton.gameObject.SetActiveRecursively(true);
		}
		else if (type == Hud.Mode.ReplayOutcome)
		{
			this.m_controlPanelPlanning.GetComponent<UIPanel>().Dismiss();
			this.m_controlPanelOutcome.GetComponent<UIPanel>().BringIn();
			this.m_timeline = GuiUtils.FindChildOf(this.m_controlPanelOutcome, "Timeline SuperSprite");
			this.m_stopOutcomeButton.gameObject.SetActiveRecursively(true);
			this.m_nextTurnButton.gameObject.SetActiveRecursively(false);
			this.m_prevTurnButton.gameObject.SetActiveRecursively(false);
		}
		if (this.m_replayMode)
		{
			this.m_stopOutcomeButton.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00017E0C File Offset: 0x0001600C
	private void SetupBattlebar(GameType gameType)
	{
		this.m_battlebarAssassinate.SetActiveRecursively(false);
		this.m_battlebarCampaign.SetActiveRecursively(false);
		this.m_battlebarPoints.SetActiveRecursively(false);
		this.m_battlebarCurrent = null;
		if (this.m_battlebar != null)
		{
			this.m_battlebar.Close();
			this.m_battlebar = null;
		}
		switch (gameType)
		{
		case GameType.Challenge:
		case GameType.Campaign:
			this.m_battlebarCurrent = this.m_battlebarCampaign;
			break;
		case GameType.Points:
			this.m_battlebarCurrent = this.m_battlebarPoints;
			break;
		case GameType.Assassination:
			this.m_battlebarCurrent = this.m_battlebarAssassinate;
			break;
		}
		if (this.m_battlebarCurrent != null)
		{
			this.m_battlebar = new Battlebar(this.m_gameType, this.m_battlebarCurrent, this.m_guiCamera, this.m_turnMan);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00017EEC File Offset: 0x000160EC
	private void SetupIngameMenu()
	{
		this.m_ingameMenu = new IngameMenu(GuiUtils.FindChildOf(this.m_gui, "MainMenuContainer"));
		this.m_ingameMenu.m_OnSurrender = new Action(this.IngameMenu_Surrender);
		this.m_ingameMenu.m_OnOptions = new Action(this.IngameMenu_Options);
		this.m_ingameMenu.m_OnSwitchGame = new Action(this.IngameMenu_SwitchGame);
		this.m_ingameMenu.m_OnBackToMenu = new Action(this.IngameMenu_BackToMenu);
		this.m_ingameMenu.m_OnQuitGame = new Action(this.IngameMenu_QuitGame);
		this.m_ingameMenu.m_OnLeave = new Action(this.IngameMenu_LeaveGame);
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00017FA0 File Offset: 0x000161A0
	private void SetupChat()
	{
		this.m_chat.m_listItem = GuiUtils.CreateGui("Lobby/GlobalChatListItem", this.m_guiCamera);
		this.m_chat.m_listItem.transform.Translate(new Vector3(1000000f, 0f, 0f));
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui, "GlobalChatContainer");
		this.m_chat.m_globalChatPanel = gameObject.GetComponent<UIPanel>();
		this.m_chat.m_globalMessageList = GuiUtils.FindChildOf(gameObject, "GlobalFeedList").GetComponent<UIScrollList>();
		this.m_chat.m_globalTextInput = GuiUtils.FindChildOf(gameObject, "GlobalMessageBox").GetComponent<UITextField>();
		this.m_chat.m_globalTextInput.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnSendGlobalChatMessage));
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_gui, "TeamChatContainer");
		this.m_chat.m_teamChatPanel = gameObject2.GetComponent<UIPanel>();
		this.m_chat.m_teamMessageList = GuiUtils.FindChildOf(gameObject2, "TeamFeedList").GetComponent<UIScrollList>();
		this.m_chat.m_teamTextInput = GuiUtils.FindChildOf(gameObject2, "TeamMessageBox").GetComponent<UITextField>();
		this.m_chat.m_teamTextInput.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnSendTeamChatMessage));
		List<ChatClient.ChatMessage> allMessages = this.m_chatClient.GetAllMessages(ChannelID.General);
		foreach (ChatClient.ChatMessage msg in allMessages)
		{
			this.AddChatMessage(ChannelID.General, msg);
		}
		List<ChatClient.ChatMessage> allMessages2 = this.m_chatClient.GetAllMessages(ChannelID.Team0);
		foreach (ChatClient.ChatMessage msg2 in allMessages2)
		{
			this.AddChatMessage(ChannelID.Team0, msg2);
		}
		List<ChatClient.ChatMessage> allMessages3 = this.m_chatClient.GetAllMessages(ChannelID.Team1);
		foreach (ChatClient.ChatMessage msg3 in allMessages3)
		{
			this.AddChatMessage(ChannelID.Team1, msg3);
		}
		ChatClient chatClient = this.m_chatClient;
		chatClient.m_onNewMessage = (Action<ChannelID, ChatClient.ChatMessage>)Delegate.Combine(chatClient.m_onNewMessage, new Action<ChannelID, ChatClient.ChatMessage>(this.OnNewChatMessage));
	}

	// Token: 0x0600030B RID: 779 RVA: 0x0001822C File Offset: 0x0001642C
	private void OnSendGlobalChatMessage(IKeyFocusable control)
	{
		string text = control.Content;
		if (text.IndexOf("/") == 0)
		{
			text = text.Replace("/", string.Empty);
			CheatMan.instance.ActivateCheat(text, this);
			this.m_chat.m_globalTextInput.Text = string.Empty;
			return;
		}
		if (control.Content != string.Empty)
		{
			this.m_chatClient.SendMessage(ChannelID.General, control.Content);
			this.m_chat.m_globalTextInput.Text = string.Empty;
		}
		UIManager.instance.FocusObject = this.m_chat.m_globalTextInput;
	}

	// Token: 0x0600030C RID: 780 RVA: 0x000182D4 File Offset: 0x000164D4
	private void OnSendTeamChatMessage(IKeyFocusable control)
	{
		if (control.Content != string.Empty)
		{
			int playerTeam = this.m_turnMan.GetPlayerTeam(this.m_localPlayerID);
			if (playerTeam == -1)
			{
				return;
			}
			ChannelID channel = (playerTeam != 0) ? ChannelID.Team1 : ChannelID.Team0;
			this.m_chatClient.SendMessage(channel, control.Content);
			this.m_chat.m_teamTextInput.Text = string.Empty;
		}
		UIManager.instance.FocusObject = this.m_chat.m_teamTextInput;
	}

	// Token: 0x0600030D RID: 781 RVA: 0x0001835C File Offset: 0x0001655C
	private void OnNewChatMessage(ChannelID channel, ChatClient.ChatMessage msg)
	{
		this.AddChatMessage(channel, msg);
		if (this.m_battlebar != null)
		{
			if (channel == ChannelID.General)
			{
				this.m_battlebar.SetGlobalChatGlow(true);
			}
			else
			{
				this.m_battlebar.SetTeamChatGlow(true);
			}
		}
	}

	// Token: 0x0600030E RID: 782 RVA: 0x000183A0 File Offset: 0x000165A0
	public void AddChatMessage(ChannelID channel, ChatClient.ChatMessage msg)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_chat.m_listItem) as GameObject;
		SpriteText component = gameObject.transform.Find("GlobalChatTimestampLabel").GetComponent<SpriteText>();
		SpriteText component2 = gameObject.transform.Find("GlobalChatNameLabel").GetComponent<SpriteText>();
		SpriteText component3 = gameObject.transform.Find("GlobalChatMessageLabel").GetComponent<SpriteText>();
		component.Text = msg.m_date.ToString("yyyy-MM-d HH:mm");
		component2.Text = msg.m_name;
		component3.Text = msg.m_message;
		UIScrollList uiscrollList = (channel != ChannelID.General) ? this.m_chat.m_teamMessageList : this.m_chat.m_globalMessageList;
		uiscrollList.AddItem(gameObject);
		while (uiscrollList.Count > 40)
		{
			uiscrollList.RemoveItem(0, true);
		}
		uiscrollList.ScrollListTo(1f);
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0001848C File Offset: 0x0001668C
	public void ShowFps(bool enabled)
	{
		if (enabled == this.m_showFps)
		{
			return;
		}
		this.m_FPS_label.gameObject.SetActiveRecursively(enabled);
		this.m_showFps = enabled;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x000184B4 File Offset: 0x000166B4
	public void Close()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		if (this.m_missionLog != null)
		{
			this.m_missionLog.Close();
		}
		if (this.m_messageLog != null)
		{
			this.m_messageLog.Close();
		}
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_shipTagMan.Close();
		UnityEngine.Object.Destroy(this.m_chat.m_listItem);
		ChatClient chatClient = this.m_chatClient;
		chatClient.m_onNewMessage = (Action<ChannelID, ChatClient.ChatMessage>)Delegate.Remove(chatClient.m_onNewMessage, new Action<ChannelID, ChatClient.ChatMessage>(this.OnNewChatMessage));
	}

	// Token: 0x06000311 RID: 785 RVA: 0x00018558 File Offset: 0x00016758
	private void SetGameType(GameType type)
	{
		if (type == this.m_gameType)
		{
			return;
		}
		this.m_gameType = type;
		this.m_shipTagMan.SetGameType(type);
		this.SetupBattlebar(type);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00018584 File Offset: 0x00016784
	public void SetVisible(bool visible, bool hideAll, bool affectBattleBar)
	{
		this.m_visible = visible;
		HitText.instance.SetVisible(visible);
		this.m_shipTagMan.SetVisible(visible);
		this.m_messageLog.SetVisible(visible);
		Route.m_drawGui = visible;
		if (affectBattleBar && this.m_battlebar != null)
		{
			this.m_battlebar.SetVisible(visible);
		}
		if (hideAll)
		{
			this.m_hideHudButton.Hide(!this.m_visible);
		}
		if (this.m_visible)
		{
			if (this.m_battlebarCurrent != null)
			{
				GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_GlobalChat").SetActiveRecursively(true);
			}
			if (this.m_globalChatWasVisible)
			{
				GuiUtils.FindChildOf(this.m_gui, "GlobalChatContainer").GetComponent<UIPanel>().BringIn();
			}
			if (this.m_switchGameWasVisible)
			{
				GuiUtils.FindChildOf(this.m_gui, "SwitchGameContainer").GetComponent<UIPanel>().BringIn();
			}
			if (this.m_gameType == GameType.Campaign || this.m_gameType == GameType.Challenge)
			{
				if (this.m_battlebarCurrent != null)
				{
					GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_Objectives").SetActiveRecursively(true);
				}
				if (this.m_objectivesWasVisible)
				{
					GuiUtils.FindChildOf(this.m_gui, "ObjectivesContainer").GetComponent<UIPanel>().BringIn();
				}
			}
			else
			{
				if (this.m_battlebarCurrent != null)
				{
					GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_TeamChat").SetActiveRecursively(true);
				}
				if (this.m_teamChatWasVisible)
				{
					GuiUtils.FindChildOf(this.m_gui, "TeamChatContainer").GetComponent<UIPanel>().BringIn();
				}
			}
			this.m_currentTurn.gameObject.SetActiveRecursively(true);
			this.m_gameNameLabel.gameObject.SetActiveRecursively(true);
			this.m_turnTimeRemainingLabel.gameObject.SetActiveRecursively(true);
			this.SetStatusText(this.m_mode);
			this.SetControlPanel(this.m_mode);
			this.m_FPS_label.gameObject.SetActiveRecursively(this.m_showFps);
		}
		else
		{
			if (this.m_battlebarCurrent != null)
			{
				GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_GlobalChat").SetActiveRecursively(false);
			}
			this.m_globalChatWasVisible = GuiUtils.FindChildOf(this.m_gui, "GlobalChatContainer").active;
			GuiUtils.FindChildOf(this.m_gui, "GlobalChatContainer").GetComponent<UIPanel>().Dismiss();
			this.m_switchGameWasVisible = GuiUtils.FindChildOf(this.m_gui, "SwitchGameContainer").active;
			GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "SwitchGameContainer").Dismiss();
			if (this.m_gameType == GameType.Campaign || this.m_gameType == GameType.Challenge)
			{
				if (this.m_battlebarCurrent != null)
				{
					GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_Objectives").SetActiveRecursively(false);
				}
				this.m_objectivesWasVisible = GuiUtils.FindChildOf(this.m_gui, "ObjectivesContainer").active;
				GuiUtils.FindChildOf(this.m_gui, "ObjectivesContainer").GetComponent<UIPanel>().Dismiss();
			}
			else
			{
				if (this.m_battlebarCurrent != null)
				{
					this.m_teamChatWasVisible = GuiUtils.FindChildOf(this.m_gui, "TeamChatContainer").active;
					GuiUtils.FindChildOf(this.m_battlebarCurrent, "Button_TeamChat").SetActiveRecursively(false);
				}
				GuiUtils.FindChildOf(this.m_gui, "TeamChatContainer").GetComponent<UIPanel>().Dismiss();
			}
			this.m_controlPanelOutcome.SetActiveRecursively(false);
			this.m_controlPanelPlanning.SetActiveRecursively(false);
			this.m_FPS_label.gameObject.SetActiveRecursively(false);
			this.m_currentTurn.gameObject.SetActiveRecursively(false);
			this.m_gameNameLabel.gameObject.SetActiveRecursively(false);
			this.m_turnTimeRemainingLabel.gameObject.SetActiveRecursively(false);
			this.HideAllStatusText();
			GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "MainMenuContainer").Dismiss();
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00018968 File Offset: 0x00016B68
	public void SetMode(Hud.Mode mode)
	{
		this.m_mode = mode;
		if (!this.m_visible)
		{
			return;
		}
		this.SetStatusText(mode);
		this.SetControlPanel(mode);
		switch (this.m_mode)
		{
		case Hud.Mode.Outcome:
			this.m_messageLog.ShowMessage(MessageLog.TextPosition.Middle, "$hud_announcement_outcome", "$hud_announcement_turnx " + this.m_turn.ToString(), "TurnMessage", 0.6f);
			UnityEngine.Object.Instantiate(this.m_newTurnSoundPrefab);
			this.m_restartOutcomeButton.Hide(true);
			this.m_startOutcomeButton.Hide(true);
			this.m_pauseOutcomeButton.Hide(false);
			this.m_startOutcomeButton.controlIsEnabled = true;
			break;
		case Hud.Mode.Planning:
			this.m_messageLog.ShowMessage(MessageLog.TextPosition.Middle, "$hud_announcement_planning", "$hud_announcement_turnx " + this.m_turn.ToString(), "TurnMessage", 0.6f);
			this.m_restartOutcomeButton.Hide(false);
			this.m_restartOutcomeButton.controlIsEnabled = true;
			this.m_startOutcomeButton.Hide(true);
			this.m_pauseOutcomeButton.Hide(true);
			break;
		case Hud.Mode.Waiting:
			this.m_restartOutcomeButton.Hide(false);
			this.m_restartOutcomeButton.controlIsEnabled = false;
			this.m_startOutcomeButton.Hide(true);
			this.m_pauseOutcomeButton.Hide(true);
			break;
		case Hud.Mode.Replay:
			this.m_restartOutcomeButton.Hide(true);
			this.m_startOutcomeButton.Hide(true);
			this.m_pauseOutcomeButton.Hide(false);
			this.m_startOutcomeButton.controlIsEnabled = true;
			break;
		}
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00018AF8 File Offset: 0x00016CF8
	public void SetGameInfo(string name, GameType gameType, int gameID, bool localAdmin)
	{
		this.m_gameNameLabel.Text = name;
		this.m_gameID = gameID;
		if (this.m_disbandGameButton != null)
		{
			this.m_disbandGameButton.controlIsEnabled = localAdmin;
		}
		this.SetGameType(gameType);
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00018B40 File Offset: 0x00016D40
	public void SetPlaybackData(int frame, int totalFrames, int turn, double turnTimeLeft)
	{
		float num = Time.fixedDeltaTime * (float)frame;
		float i = (totalFrames <= 0) ? 0f : ((float)frame / (float)totalFrames);
		if (this.m_timeline != null)
		{
			GuiUtils.SetAnimationSetProgress(this.m_timeline, i);
		}
		string text = (turnTimeLeft <= 0.0) ? string.Empty : Localize.instance.Translate(Utils.FormatTimeLeftString(turnTimeLeft));
		if (this.m_currentTurn != null)
		{
			if (text.Length > 0)
			{
				this.m_currentTurn.Text = string.Concat(new string[]
				{
					Localize.instance.Translate("$hud_turn"),
					" ",
					turn.ToString(),
					"  (",
					text,
					")"
				});
			}
			else
			{
				this.m_currentTurn.Text = Localize.instance.Translate("$hud_turn") + " " + turn.ToString();
			}
		}
		this.m_turnTimeRemainingLabel.Text = text;
		this.m_turn = turn;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00018C64 File Offset: 0x00016E64
	private void SetStatusText(Hud.Mode mode)
	{
		for (int i = 0; i < this.m_statusTexts.Count; i++)
		{
			if (i == (int)mode)
			{
				this.m_statusTexts[(Hud.Mode)i].BringIn();
			}
			else
			{
				this.m_statusTexts[(Hud.Mode)i].Dismiss();
			}
		}
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00018CBC File Offset: 0x00016EBC
	private void HideAllStatusText()
	{
		for (int i = 0; i < this.m_statusTexts.Count; i++)
		{
			this.m_statusTexts[(Hud.Mode)i].Dismiss();
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00018CF8 File Offset: 0x00016EF8
	public void SetTargetScore(int targetScore)
	{
		if (this.m_battlebar != null)
		{
			this.m_battlebar.SetTargetScore(targetScore);
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00018D14 File Offset: 0x00016F14
	private void IngameMenu_Surrender()
	{
		PLog.Log("::: Hud::IngameMenu_Surrender() Called via delegate :::");
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$label_dialog_surrender_message", new MsgBox.YesHandler(this.OnSurrenderConfirm), new MsgBox.NoHandler(this.OnMsgBoxCancel));
	}

	// Token: 0x0600031A RID: 794 RVA: 0x00018D5C File Offset: 0x00016F5C
	private void OnSurrenderConfirm()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_onSurrender();
	}

	// Token: 0x0600031B RID: 795 RVA: 0x00018D7C File Offset: 0x00016F7C
	private void IngameMenu_Options()
	{
		PLog.Log("::: Hud::IngameMenu_Options() Called via delegate :::");
		OptionsWindow optionsWindow = new OptionsWindow(this.m_guiCamera, true);
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00018DA0 File Offset: 0x00016FA0
	private void IngameMenu_SwitchGame()
	{
		PLog.Log("::: Hud::IngameMenu_SwitchGame() Called via delegate :::");
	}

	// Token: 0x0600031D RID: 797 RVA: 0x00018DAC File Offset: 0x00016FAC
	private void IngameMenu_BackToMenu()
	{
		PLog.Log("::: Hud::IngameMenu_BackToMenu() Called via delegate :::");
		if (this.m_onExit != null)
		{
			this.m_onExit(ExitState.Normal, 0);
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x00018DDC File Offset: 0x00016FDC
	private void IngameMenu_QuitGame()
	{
		if (this.m_onQuitGame != null)
		{
			this.m_onQuitGame();
		}
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00018DF4 File Offset: 0x00016FF4
	private void IngameMenu_LeaveGame()
	{
		this.m_rpc.Invoke("KickSelf", new object[0]);
		if (this.m_onExit != null)
		{
			this.m_onExit(ExitState.Normal, 0);
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00018E30 File Offset: 0x00017030
	public void Update(Camera camera)
	{
		if (this.m_showFps)
		{
			string text = "FPS: " + (int)(1f / Time.smoothDeltaTime) + "\n";
			text = text + "FrameTime: " + (Time.smoothDeltaTime * 1000f).ToString("F1") + " \n";
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Total sent: ",
				this.m_rpc.GetTotalSentData() / 1000,
				" kB\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Total recv: ",
				this.m_rpc.GetTotalRecvData() / 1000,
				" kB\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Sent/s: ",
				this.m_rpc.GetSentDataPerSec(),
				" B/s\n"
			});
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Recv/s: ",
				this.m_rpc.GetRecvDataPerSec(),
				" B/s\n"
			});
			this.m_FPS_label.Text = text;
		}
		UIManager component = this.m_guiCamera.GetComponent<UIManager>();
		bool flag = component.FocusObject != null;
		if (this.m_visible)
		{
			if (this.m_mode == Hud.Mode.Planning && Input.GetKeyDown(KeyCode.Space) && !flag)
			{
				this.m_onCommit();
			}
			if (this.m_mode == Hud.Mode.Outcome && Input.GetKeyDown(KeyCode.Space) && !flag)
			{
				this.TogglePaus();
			}
			this.ShowFps(CheatMan.instance.m_showFps);
			if (Input.GetKeyDown(KeyCode.Return) && this.m_onlineGame && !this.m_chat.m_globalChatPanel.gameObject.active && !this.m_chat.m_teamChatPanel.gameObject.active)
			{
				this.m_chat.m_panMan.BringIn(this.m_chat.m_globalChatPanel);
				this.SetBattleBarButtonState("Button_GlobalChat", 0);
				UIManager.instance.FocusObject = this.m_chat.m_globalTextInput;
			}
			if (this.m_chat.m_globalChatPanel.gameObject.active && this.m_battlebar != null)
			{
				this.m_battlebar.SetGlobalChatGlow(false);
			}
			if (this.m_chat.m_teamChatPanel.gameObject.active && this.m_battlebar != null)
			{
				this.m_battlebar.SetTeamChatGlow(false);
			}
		}
		this.m_shipTagMan.Update(camera, Time.deltaTime);
		this.m_missionLog.Update(camera, Time.deltaTime);
		this.m_messageLog.Update();
	}

	// Token: 0x06000321 RID: 801 RVA: 0x0001911C File Offset: 0x0001731C
	public void FixedUpdate()
	{
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00019120 File Offset: 0x00017320
	public void UpdatePlayerStates(List<ClientPlayer> players, int localPlayerID, bool inPlanning, bool inReplayMode, bool isAdmin)
	{
		this.m_localPlayerID = localPlayerID;
		if (this.m_battlebar != null)
		{
			this.m_battlebar.Update(players, localPlayerID);
		}
		if (this.m_ingameMenu != null)
		{
			foreach (ClientPlayer clientPlayer in players)
			{
				if (clientPlayer.m_id == localPlayerID)
				{
					this.m_ingameMenu.SetSurrenderStatus(clientPlayer.m_surrender, inPlanning, inReplayMode, isAdmin);
				}
			}
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x000191C8 File Offset: 0x000173C8
	private void OnCommitPressed(IUIObject obj)
	{
		if (this.m_onCommit != null)
		{
			this.m_onCommit();
		}
	}

	// Token: 0x06000324 RID: 804 RVA: 0x000191E0 File Offset: 0x000173E0
	private void OnStartOutcomePressed(IUIObject obj)
	{
		this.m_restartOutcomeButton.Hide(true);
		this.m_pauseOutcomeButton.Hide(false);
		this.m_startOutcomeButton.Hide(true);
		this.m_onPlayPause();
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00019214 File Offset: 0x00017414
	private void OnRestartOutcomePressed(IUIObject obj)
	{
		this.m_restartOutcomeButton.Hide(true);
		this.m_pauseOutcomeButton.Hide(false);
		this.m_startOutcomeButton.Hide(true);
		this.m_onPlayPause();
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00019248 File Offset: 0x00017448
	private void OnNextTurnPressed(IUIObject obj)
	{
		this.m_onNextTurn();
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00019258 File Offset: 0x00017458
	private void OnPrevTurnPressed(IUIObject obj)
	{
		this.m_onPrevTurn();
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00019268 File Offset: 0x00017468
	private void OnStopOutcomePressed(IUIObject obj)
	{
		if (this.m_onStopOutcome != null)
		{
			this.m_onStopOutcome();
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00019280 File Offset: 0x00017480
	private void OnPauseOutcomePressed(IUIObject obj)
	{
		this.m_pauseOutcomeButton.Hide(true);
		this.m_startOutcomeButton.Hide(false);
		this.m_restartOutcomeButton.Hide(true);
		this.m_onPlayPause();
	}

	// Token: 0x0600032A RID: 810 RVA: 0x000192B4 File Offset: 0x000174B4
	private void OnHideGui(IUIObject obj)
	{
		this.SetVisible(!this.m_visible, false, true);
	}

	// Token: 0x0600032B RID: 811 RVA: 0x000192C8 File Offset: 0x000174C8
	private void TogglePaus()
	{
		bool flag = this.m_pauseOutcomeButton.IsHidden();
		this.m_pauseOutcomeButton.Hide(!flag);
		this.m_startOutcomeButton.Hide(flag);
		this.m_restartOutcomeButton.Hide(!flag);
		this.m_onPlayPause();
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00019318 File Offset: 0x00017518
	private void OnDisbandGame(IUIObject button)
	{
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$hud_confirmdisband", new MsgBox.YesHandler(this.OnDisbandYes), new MsgBox.NoHandler(this.OnMsgBoxCancel));
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00019354 File Offset: 0x00017554
	private void OnKickPlayer(IUIObject button)
	{
		this.m_tempKickUserName = "qwe";
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$hud_confirmkick " + this.m_tempKickUserName, new MsgBox.YesHandler(this.OnKickPlayerYes), new MsgBox.NoHandler(this.OnMsgBoxCancel));
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000193A8 File Offset: 0x000175A8
	private void OnKickPlayerYes()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_rpc.Invoke("KickPlayer", new object[]
		{
			this.m_tempKickUserName
		});
	}

	// Token: 0x0600032F RID: 815 RVA: 0x000193E8 File Offset: 0x000175E8
	private void OnDisbandYes()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
		this.m_rpc.Invoke("DisbandGame", new object[0]);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00019420 File Offset: 0x00017620
	private void OnMsgBoxCancel()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
	}

	// Token: 0x04000248 RID: 584
	public Action m_onCommit;

	// Token: 0x04000249 RID: 585
	public Action<ExitState, int> m_onExit;

	// Token: 0x0400024A RID: 586
	public Action m_onPlayPause;

	// Token: 0x0400024B RID: 587
	public Action m_onStartTest;

	// Token: 0x0400024C RID: 588
	public Action m_onStopTest;

	// Token: 0x0400024D RID: 589
	public Action m_onStopOutcome;

	// Token: 0x0400024E RID: 590
	public Action m_onNextTurn;

	// Token: 0x0400024F RID: 591
	public Action m_onPrevTurn;

	// Token: 0x04000250 RID: 592
	public Action m_onQuitGame;

	// Token: 0x04000251 RID: 593
	public Action m_onSurrender;

	// Token: 0x04000252 RID: 594
	public Action m_onFadeComplete;

	// Token: 0x04000253 RID: 595
	public bool m_showObjectives;

	// Token: 0x04000254 RID: 596
	private GameObject m_gui;

	// Token: 0x04000255 RID: 597
	private PTech.RPC m_rpc;

	// Token: 0x04000256 RID: 598
	private TurnMan m_turnMan;

	// Token: 0x04000257 RID: 599
	private ChatClient m_chatClient;

	// Token: 0x04000258 RID: 600
	private Hud.Mode m_mode;

	// Token: 0x04000259 RID: 601
	private bool m_visible = true;

	// Token: 0x0400025A RID: 602
	private GameType m_gameType;

	// Token: 0x0400025B RID: 603
	private GameObject m_battlebarCampaign;

	// Token: 0x0400025C RID: 604
	private GameObject m_battlebarAssassinate;

	// Token: 0x0400025D RID: 605
	private GameObject m_battlebarPoints;

	// Token: 0x0400025E RID: 606
	private GameObject m_battlebarCurrent;

	// Token: 0x0400025F RID: 607
	private Dictionary<Hud.Mode, UIPanel> m_statusTexts = new Dictionary<Hud.Mode, UIPanel>();

	// Token: 0x04000260 RID: 608
	private GameObject m_controlPanelPlanning;

	// Token: 0x04000261 RID: 609
	private GameObject m_controlPanelOutcome;

	// Token: 0x04000262 RID: 610
	private GameObject m_timeline;

	// Token: 0x04000263 RID: 611
	private GameObject m_newTurnSoundPrefab;

	// Token: 0x04000264 RID: 612
	private UIButton m_nextTurnButton;

	// Token: 0x04000265 RID: 613
	private UIButton m_prevTurnButton;

	// Token: 0x04000266 RID: 614
	private UIButton m_startOutcomeButton;

	// Token: 0x04000267 RID: 615
	private UIButton m_restartOutcomeButton;

	// Token: 0x04000268 RID: 616
	private UIButton m_stopOutcomeButton;

	// Token: 0x04000269 RID: 617
	private UIButton m_pauseOutcomeButton;

	// Token: 0x0400026A RID: 618
	private UIButton m_commitButton;

	// Token: 0x0400026B RID: 619
	private UIStateToggleBtn m_hideHudButton;

	// Token: 0x0400026C RID: 620
	private SpriteText m_currentTurn;

	// Token: 0x0400026D RID: 621
	private SpriteText m_FPS_label;

	// Token: 0x0400026E RID: 622
	private SpriteText m_gameNameLabel;

	// Token: 0x0400026F RID: 623
	private SpriteText m_turnTimeRemainingLabel;

	// Token: 0x04000270 RID: 624
	private UIButton m_disbandGameButton;

	// Token: 0x04000271 RID: 625
	private bool m_teamChatWasVisible;

	// Token: 0x04000272 RID: 626
	private bool m_globalChatWasVisible;

	// Token: 0x04000273 RID: 627
	private bool m_objectivesWasVisible;

	// Token: 0x04000274 RID: 628
	private bool m_switchGameWasVisible;

	// Token: 0x04000275 RID: 629
	public UIPanelManager m_panManMainMenu;

	// Token: 0x04000276 RID: 630
	private Hud.Chat m_chat = new Hud.Chat();

	// Token: 0x04000277 RID: 631
	private bool m_onlineGame = true;

	// Token: 0x04000278 RID: 632
	private bool m_replayMode;

	// Token: 0x04000279 RID: 633
	private bool m_showFps;

	// Token: 0x0400027A RID: 634
	private GameObject m_guiCamera;

	// Token: 0x0400027B RID: 635
	private int m_turn;

	// Token: 0x0400027C RID: 636
	private int m_localPlayerID = -1;

	// Token: 0x0400027D RID: 637
	private int m_gameID;

	// Token: 0x0400027E RID: 638
	private List<GamePost> m_switchGameList = new List<GamePost>();

	// Token: 0x0400027F RID: 639
	private IngameMenu m_ingameMenu;

	// Token: 0x04000280 RID: 640
	private MissionLog m_missionLog;

	// Token: 0x04000281 RID: 641
	private MessageLog m_messageLog;

	// Token: 0x04000282 RID: 642
	private ShipTagMan m_shipTagMan;

	// Token: 0x04000283 RID: 643
	private Battlebar m_battlebar;

	// Token: 0x04000284 RID: 644
	private MsgBox m_msgBox;

	// Token: 0x04000285 RID: 645
	private string m_tempKickUserName;

	// Token: 0x02000048 RID: 72
	public enum Mode
	{
		// Token: 0x04000287 RID: 647
		Outcome,
		// Token: 0x04000288 RID: 648
		Planning,
		// Token: 0x04000289 RID: 649
		Waiting,
		// Token: 0x0400028A RID: 650
		Replay,
		// Token: 0x0400028B RID: 651
		ReplayOutcome
	}

	// Token: 0x02000049 RID: 73
	private enum ControlType
	{
		// Token: 0x0400028D RID: 653
		Planning,
		// Token: 0x0400028E RID: 654
		Outcome,
		// Token: 0x0400028F RID: 655
		Replay,
		// Token: 0x04000290 RID: 656
		None
	}

	// Token: 0x0200004A RID: 74
	private class Chat
	{
		// Token: 0x04000291 RID: 657
		public UIPanel m_globalChatPanel;

		// Token: 0x04000292 RID: 658
		public UIPanel m_teamChatPanel;

		// Token: 0x04000293 RID: 659
		public UITextField m_globalTextInput;

		// Token: 0x04000294 RID: 660
		public UITextField m_teamTextInput;

		// Token: 0x04000295 RID: 661
		public UIScrollList m_globalMessageList;

		// Token: 0x04000296 RID: 662
		public UIScrollList m_teamMessageList;

		// Token: 0x04000297 RID: 663
		public GameObject m_listItem;

		// Token: 0x04000298 RID: 664
		public UIPanelManager m_panMan;
	}
}
