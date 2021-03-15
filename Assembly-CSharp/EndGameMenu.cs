using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class EndGameMenu
{
	// Token: 0x06000246 RID: 582 RVA: 0x0000FEFC File Offset: 0x0000E0FC
	public EndGameMenu(GameObject guiCamera, EndGameData endgameData, GameSettings gameSettings, PTech.RPC rpc, ChatClient chatClient, MusicManager musMan)
	{
		this.m_guiCamera = guiCamera;
		this.m_endgameData = endgameData;
		this.m_gameSettings = gameSettings;
		this.m_rpc = rpc;
		this.m_chatClient = chatClient;
		switch (gameSettings.m_gameType)
		{
		case GameType.Challenge:
		case GameType.Campaign:
			this.m_gui = GuiUtils.CreateGui("EndGame/EndGameWindow_CampaignChallenge", guiCamera);
			this.SetupCampaignPlayerList();
			break;
		case GameType.Points:
			this.m_gui = GuiUtils.CreateGui("EndGame/EndGameWindow_Points", guiCamera);
			this.SetupPointsPlayerList();
			break;
		case GameType.Assassination:
			this.m_gui = GuiUtils.CreateGui("EndGame/EndGameWindow_Assassin", guiCamera);
			this.SetupAssasinatePlayerList();
			break;
		}
		this.SetupAccolades();
		GuiUtils.FindChildOf(this.m_gui, "ExitButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnLeavePressed));
		this.m_saveReplayButton = GuiUtils.FindChildOf(this.m_gui, "SaveReplayButton").GetComponent<UIButton>();
		this.m_saveReplayButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSaveReplayPressed));
		SpriteText component = GuiUtils.FindChildOf(this.m_gui, "EndGameTitleLabel").GetComponent<SpriteText>();
		bool flag = false;
		bool flag2;
		if (gameSettings.m_gameType == GameType.Campaign || gameSettings.m_gameType == GameType.Challenge)
		{
			flag2 = (endgameData.m_outcome == GameOutcome.Victory);
		}
		else
		{
			int team = endgameData.m_players[endgameData.m_localPlayerID].m_team;
			flag2 = (endgameData.m_winnerTeam == team);
			flag = (endgameData.m_winnerTeam == -1);
		}
		if (flag2)
		{
			component.Text = Localize.instance.Translate("$Victory");
			musMan.SetMusic("victory");
		}
		else if (flag)
		{
			component.Text = Localize.instance.Translate("$Draw");
			musMan.SetMusic("victory");
		}
		else
		{
			component.Text = Localize.instance.Translate("$Defeat");
			musMan.SetMusic("defeat");
		}
		this.SetupChat();
		this.m_rpc.Register("FriendRequestReply", new PTech.RPC.Handler(this.RPC_FriendRequestReply));
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00010138 File Offset: 0x0000E338
	private void SetupAccolades()
	{
		GuiUtils.FindChildOf(this.m_gui, "DestroyerValueLabel").GetComponent<SpriteText>().Text = this.m_endgameData.m_AccoladeDestroy;
		GuiUtils.FindChildOf(this.m_gui, "MostlyHarmlessValueLabel").GetComponent<SpriteText>().Text = this.m_endgameData.m_AccoladeHarmless;
		GuiUtils.FindChildOf(this.m_gui, "BestUseOfShieldsValueLabel").GetComponent<SpriteText>().Text = this.m_endgameData.m_AccoladeShields;
		GuiUtils.FindChildOf(this.m_gui, "LongDistanceMarinerValueLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate("$label_notapplicable");
		GuiUtils.FindChildOf(this.m_gui, "SlowpokeValueLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate("$label_notapplicable");
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00010208 File Offset: 0x0000E408
	private void SetupCampaignPlayerList()
	{
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui, "TeamScrollList").GetComponent<UIScrollList>();
		foreach (EndGame_PlayerStatistics endGame_PlayerStatistics in this.m_endgameData.m_players)
		{
			GameObject gameObject = GuiUtils.CreateGui("EndGame/TeamListItem_CampaignChallenge", this.m_guiCamera);
			GuiUtils.FindChildOf(gameObject, "PlayerNameLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_name;
			GuiUtils.FindChildOf(gameObject, "PlayerShipsSunkLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsSunk.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsLostLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsLost.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsDamagedLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsDamaged.ToString();
			UIButton component2 = GuiUtils.FindChildOf(gameObject, "PlayerAddFriendButton").GetComponent<UIButton>();
			if (endGame_PlayerStatistics.m_playerID == this.m_endgameData.m_localPlayerID)
			{
				component2.transform.Translate(new Vector3(10000f, 0f, 0f));
			}
			else
			{
				component2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAddFriend));
			}
			Texture2D flagTexture = GuiUtils.GetFlagTexture(endGame_PlayerStatistics.m_flag);
			SimpleSprite component3 = GuiUtils.FindChildOf(gameObject, "PlayerFlag").GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component3, flagTexture);
			component.AddItem(gameObject.GetComponent<UIListItemContainer>());
		}
		component.ScrollToItem(0, 0f);
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000103B0 File Offset: 0x0000E5B0
	private void SetupPointsPlayerList()
	{
		bool flag = this.m_endgameData.m_winnerTeam == -1;
		SpriteText component = GuiUtils.FindChildOf(this.m_gui, "LosingTeamTotalScoreLabel").GetComponent<SpriteText>();
		SpriteText component2 = GuiUtils.FindChildOf(this.m_gui, "WinningTeamTotalScoreLabel").GetComponent<SpriteText>();
		if (flag)
		{
			SpriteText component3 = GuiUtils.FindChildOf(this.m_gui, "WinningTeamTitleLabel").GetComponent<SpriteText>();
			SpriteText component4 = GuiUtils.FindChildOf(this.m_gui, "LosingTeamTitleLabel").GetComponent<SpriteText>();
			component3.Text = string.Empty;
			component4.Text = string.Empty;
		}
		UIScrollList component5 = GuiUtils.FindChildOf(this.m_gui, "LosingTeamScrollList").GetComponent<UIScrollList>();
		UIScrollList component6 = GuiUtils.FindChildOf(this.m_gui, "WinningTeamScrollList").GetComponent<UIScrollList>();
		foreach (EndGame_PlayerStatistics endGame_PlayerStatistics in this.m_endgameData.m_players)
		{
			GameObject gameObject = GuiUtils.CreateGui("EndGame/TeamListItem_Points", this.m_guiCamera);
			GuiUtils.FindChildOf(gameObject, "PlayerNameLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_name;
			GuiUtils.FindChildOf(gameObject, "PlayerPointsLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_score.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsSunkLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsSunk.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsLostLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsLost.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsDamagedLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsDamaged.ToString();
			UIButton component7 = GuiUtils.FindChildOf(gameObject, "PlayerAddFriendButton").GetComponent<UIButton>();
			if (endGame_PlayerStatistics.m_playerID == this.m_endgameData.m_localPlayerID)
			{
				component7.transform.Translate(new Vector3(10000f, 0f, 0f));
			}
			else
			{
				component7.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAddFriend));
			}
			Texture2D flagTexture = GuiUtils.GetFlagTexture(endGame_PlayerStatistics.m_flag);
			SimpleSprite component8 = GuiUtils.FindChildOf(gameObject, "PlayerFlag").GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component8, flagTexture);
			if (endGame_PlayerStatistics.m_team == this.m_endgameData.m_winnerTeam || (flag && endGame_PlayerStatistics.m_team == 0))
			{
				component6.AddItem(gameObject.GetComponent<UIListItemContainer>());
				component2.Text = endGame_PlayerStatistics.m_teamScore.ToString() + " " + Localize.instance.TranslateKey("label_pointssmall");
			}
			else
			{
				component5.AddItem(gameObject.GetComponent<UIListItemContainer>());
				component.Text = endGame_PlayerStatistics.m_teamScore.ToString() + " " + Localize.instance.TranslateKey("label_pointssmall");
			}
		}
		component5.ScrollToItem(0, 0f);
		component6.ScrollToItem(0, 0f);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000106C8 File Offset: 0x0000E8C8
	private void SetupAssasinatePlayerList()
	{
		bool flag = this.m_endgameData.m_winnerTeam == -1;
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui, "LosingTeamScrollList").GetComponent<UIScrollList>();
		UIScrollList component2 = GuiUtils.FindChildOf(this.m_gui, "WinningTeamScrollList").GetComponent<UIScrollList>();
		if (flag)
		{
			SpriteText component3 = GuiUtils.FindChildOf(this.m_gui, "WinningTeamTitleLabel").GetComponent<SpriteText>();
			SpriteText component4 = GuiUtils.FindChildOf(this.m_gui, "LosingTeamTitleLabel").GetComponent<SpriteText>();
			component3.Text = string.Empty;
			component4.Text = string.Empty;
		}
		foreach (EndGame_PlayerStatistics endGame_PlayerStatistics in this.m_endgameData.m_players)
		{
			GameObject gameObject = GuiUtils.CreateGui("EndGame/TeamListItem_Assassin", this.m_guiCamera);
			GuiUtils.FindChildOf(gameObject, "PlayerNameLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_name;
			GuiUtils.FindChildOf(gameObject, "PlayerShipsSunkLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsSunk.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsLostLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsLost.ToString();
			GuiUtils.FindChildOf(gameObject, "PlayerShipsDamagedLabel").GetComponent<SpriteText>().Text = endGame_PlayerStatistics.m_shipsDamaged.ToString();
			string text = this.CreateAssasinatedList(endGame_PlayerStatistics.m_playerID);
			GuiUtils.FindChildOfComponent<SpriteText>(gameObject, "PlayerFlagshipsSunkLabel").Text = text;
			UIButton component5 = GuiUtils.FindChildOf(gameObject, "PlayerAddFriendButton").GetComponent<UIButton>();
			if (endGame_PlayerStatistics.m_playerID == this.m_endgameData.m_localPlayerID)
			{
				component5.transform.Translate(new Vector3(10000f, 0f, 0f));
			}
			else
			{
				component5.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnAddFriend));
			}
			Texture2D flagTexture = GuiUtils.GetFlagTexture(endGame_PlayerStatistics.m_flag);
			SimpleSprite component6 = GuiUtils.FindChildOf(gameObject, "PlayerFlag").GetComponent<SimpleSprite>();
			GuiUtils.SetImage(component6, flagTexture);
			if (endGame_PlayerStatistics.m_team == this.m_endgameData.m_winnerTeam || (flag && endGame_PlayerStatistics.m_team == 0))
			{
				component2.AddItem(gameObject.GetComponent<UIListItemContainer>());
			}
			else
			{
				component.AddItem(gameObject.GetComponent<UIListItemContainer>());
			}
		}
		component.ScrollToItem(0, 0f);
		component2.ScrollToItem(0, 0f);
	}

	// Token: 0x0600024B RID: 587 RVA: 0x00010958 File Offset: 0x0000EB58
	private string CreateAssasinatedList(int killer)
	{
		string text = string.Empty;
		foreach (EndGame_PlayerStatistics endGame_PlayerStatistics in this.m_endgameData.m_players)
		{
			if (endGame_PlayerStatistics.m_flagshipKiller0 == killer)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += endGame_PlayerStatistics.m_name;
			}
			if (endGame_PlayerStatistics.m_flagshipKiller1 == killer)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text = text + endGame_PlayerStatistics.m_name + "#2";
			}
		}
		return text;
	}

	// Token: 0x0600024C RID: 588 RVA: 0x00010A2C File Offset: 0x0000EC2C
	public void Close()
	{
		if (this.m_gui != null)
		{
			UnityEngine.Object.Destroy(this.m_gui);
			this.m_gui = null;
		}
		if (this.m_saveReplayProgress != null)
		{
			UnityEngine.Object.Destroy(this.m_saveReplayProgress);
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_rpc.Unregister("FriendRequestReply");
	}

	// Token: 0x0600024D RID: 589 RVA: 0x00010AA8 File Offset: 0x0000ECA8
	private void OnAddFriend(IUIObject button)
	{
		SpriteText component = GuiUtils.FindChildOf(button.transform.parent, "PlayerNameLabel").GetComponent<SpriteText>();
		this.m_addFriendTempName = component.Text;
		this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$endgame_addfriend " + this.m_addFriendTempName, new MsgBox.YesHandler(this.OnAddFriendYes), new MsgBox.NoHandler(this.OnAddFriendNo));
	}

	// Token: 0x0600024E RID: 590 RVA: 0x00010B18 File Offset: 0x0000ED18
	public void Update()
	{
		if (Utils.AndroidBack())
		{
			this.OnLeavePressed(null);
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x00010B2C File Offset: 0x0000ED2C
	private void OnAddFriendYes()
	{
		this.m_rpc.Invoke("FriendRequest", new object[]
		{
			this.m_addFriendTempName
		});
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00010B60 File Offset: 0x0000ED60
	private void OnAddFriendNo()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x06000251 RID: 593 RVA: 0x00010B74 File Offset: 0x0000ED74
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

	// Token: 0x06000252 RID: 594 RVA: 0x00010BBC File Offset: 0x0000EDBC
	private void OnLeavePressed(IUIObject obj)
	{
		PLog.Log("EndGameMenu:OnLeavePressed()");
		this.m_rpc.Invoke("SeenEndGame", new object[0]);
		if (this.m_onLeavePressed != null)
		{
			this.m_onLeavePressed(this.m_endgameData.m_autoJoinGameID);
		}
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00010C0C File Offset: 0x0000EE0C
	private void OnSaveReplayPressed(IUIObject obj)
	{
		if (this.m_saveDialog == null)
		{
			this.m_saveDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$label_savereplayas"), this.m_gameSettings.m_gameName, new GenericTextInput.InputTextCancel(this.OnSaveReplayCancel), new GenericTextInput.InputTextCommit(this.OnSaveReplayOk));
		}
	}

	// Token: 0x06000254 RID: 596 RVA: 0x00010C70 File Offset: 0x0000EE70
	private void OnSaveReplayCancel()
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00010C84 File Offset: 0x0000EE84
	private void OnSaveReplayOk(string text)
	{
		UnityEngine.Object.Destroy(this.m_saveDialog);
		this.m_saveDialog = null;
		this.m_saveReplayProgress = GuiUtils.CreateGui("dialogs/Dialog_Progress", this.m_guiCamera);
		this.m_rpc.Register("SaveReplayReply", new PTech.RPC.Handler(this.RPC_SaveReplayReply));
		this.m_rpc.Invoke("SaveReplay", new object[]
		{
			text
		});
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00010CF0 File Offset: 0x0000EEF0
	private void RPC_SaveReplayReply(PTech.RPC rpc, List<object> args)
	{
		rpc.Unregister("SaveReplayReply");
		UnityEngine.Object.Destroy(this.m_saveReplayProgress);
		this.m_saveReplayProgress = null;
		if (!(bool)args[0])
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_replayexists", new MsgBox.OkHandler(this.OnMsgBoxOK));
		}
		else
		{
			this.m_saveReplayButton.controlIsEnabled = false;
		}
	}

	// Token: 0x06000257 RID: 599 RVA: 0x00010D60 File Offset: 0x0000EF60
	private void SetupChat()
	{
		GameObject go = GuiUtils.FindChildOf(this.m_gui, "GlobalChatContainer");
		this.m_chat.m_messageList = GuiUtils.FindChildOf(go, "GlobalFeedList").GetComponent<UIScrollList>();
		this.m_chat.m_textInput = GuiUtils.FindChildOf(go, "GlobalMessageBox").GetComponent<UITextField>();
		this.m_chat.m_listItem = GuiUtils.CreateGui("Lobby/GlobalChatListItem", this.m_guiCamera);
		this.m_chat.m_listItem.transform.Translate(new Vector3(1000000f, 0f, 0f));
		this.m_chat.m_textInput.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnSendChatMessage));
		ChatClient chatClient = this.m_chatClient;
		chatClient.m_onNewMessage = (Action<ChannelID, ChatClient.ChatMessage>)Delegate.Combine(chatClient.m_onNewMessage, new Action<ChannelID, ChatClient.ChatMessage>(this.OnNewChatMessage));
		List<ChatClient.ChatMessage> allMessages = this.m_chatClient.GetAllMessages(ChannelID.General);
		foreach (ChatClient.ChatMessage msg in allMessages)
		{
			this.AddChatMessage(msg);
		}
	}

	// Token: 0x06000258 RID: 600 RVA: 0x00010E9C File Offset: 0x0000F09C
	private void OnSendChatMessage(IKeyFocusable control)
	{
		if (control.Content != string.Empty)
		{
			this.m_chatClient.SendMessage(ChannelID.General, control.Content);
			this.m_chat.m_textInput.Text = string.Empty;
		}
		UIManager.instance.FocusObject = this.m_chat.m_textInput;
	}

	// Token: 0x06000259 RID: 601 RVA: 0x00010EFC File Offset: 0x0000F0FC
	private void OnNewChatMessage(ChannelID channel, ChatClient.ChatMessage msg)
	{
		this.AddChatMessage(msg);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00010F08 File Offset: 0x0000F108
	private void AddChatMessage(ChatClient.ChatMessage msg)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_chat.m_listItem) as GameObject;
		SpriteText component = gameObject.transform.Find("GlobalChatTimestampLabel").GetComponent<SpriteText>();
		SpriteText component2 = gameObject.transform.Find("GlobalChatNameLabel").GetComponent<SpriteText>();
		SpriteText component3 = gameObject.transform.Find("GlobalChatMessageLabel").GetComponent<SpriteText>();
		component.Text = msg.m_date.ToString("yyyy-MM-d HH:mm");
		component2.Text = msg.m_name;
		component3.Text = msg.m_message;
		this.m_chat.m_messageList.AddItem(gameObject);
		while (this.m_chat.m_messageList.Count > 40)
		{
			this.m_chat.m_messageList.RemoveItem(0, true);
		}
		this.m_chat.m_messageList.ScrollListTo(1f);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00010FF4 File Offset: 0x0000F1F4
	private void OnMsgBoxOK()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x040001AD RID: 429
	public Action<int> m_onLeavePressed;

	// Token: 0x040001AE RID: 430
	private GameObject m_gui;

	// Token: 0x040001AF RID: 431
	private GameObject m_guiCamera;

	// Token: 0x040001B0 RID: 432
	private PTech.RPC m_rpc;

	// Token: 0x040001B1 RID: 433
	private ChatClient m_chatClient;

	// Token: 0x040001B2 RID: 434
	private EndGameData m_endgameData;

	// Token: 0x040001B3 RID: 435
	private GameSettings m_gameSettings;

	// Token: 0x040001B4 RID: 436
	private MsgBox m_msgBox;

	// Token: 0x040001B5 RID: 437
	private GameObject m_saveReplayProgress;

	// Token: 0x040001B6 RID: 438
	private GameObject m_saveDialog;

	// Token: 0x040001B7 RID: 439
	private UIButton m_saveReplayButton;

	// Token: 0x040001B8 RID: 440
	private string m_addFriendTempName = string.Empty;

	// Token: 0x040001B9 RID: 441
	private EndGameMenu.Chat m_chat = new EndGameMenu.Chat();

	// Token: 0x02000039 RID: 57
	private class Chat
	{
		// Token: 0x040001BA RID: 442
		public UITextField m_textInput;

		// Token: 0x040001BB RID: 443
		public UIScrollList m_messageList;

		// Token: 0x040001BC RID: 444
		public GameObject m_listItem;
	}
}
