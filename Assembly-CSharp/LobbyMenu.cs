using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class LobbyMenu
{
	// Token: 0x0600033A RID: 826 RVA: 0x000197EC File Offset: 0x000179EC
	public LobbyMenu(GameObject guiCamera, MapInfo map, PTech.RPC rpc, UserManClient userman, int campaignID, ChatClient chatClient, MusicManager musMan, string hostName)
	{
		this.m_musMan = musMan;
		this.m_guiCamera = guiCamera;
		this.m_mapInfo = map;
		this.m_rpc = rpc;
		this.m_userManClient = userman;
		this.m_campaignID = campaignID;
		this.m_chatClient = chatClient;
		switch (map.m_gameMode)
		{
		case GameType.Challenge:
		case GameType.Campaign:
			this.m_gui = GuiUtils.CreateGui("Lobby/LobbyWindow_CampaignChallenge", guiCamera);
			break;
		case GameType.Points:
		case GameType.Assassination:
			this.m_gui = GuiUtils.CreateGui("Lobby/LobbyWindow_AssassinPoints", guiCamera);
			break;
		}
		this.m_btnReady = GuiUtils.FindChildOf(this.m_gui, "btnReady").GetComponent<UIButton>();
		this.m_btnReady.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnReadyPressed));
		this.m_btnSwitchTeam = GuiUtils.FindChildOf(this.m_gui, "btnSwitchTeam").GetComponent<UIButton>();
		this.m_btnSwitchTeam.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSwitchTeam));
		this.m_btnSelectFleet = GuiUtils.FindChildOf(this.m_gui, "btnSelectFleet").GetComponent<UIButton>();
		this.m_btnSelectFleet.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSelectFleet));
		this.m_btnFleetEdit = GuiUtils.FindChildOf(this.m_gui, "btnFleetEdit").GetComponent<UIButton>();
		this.m_btnFleetEdit.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnEditFleet));
		this.m_btnInvite = GuiUtils.FindChildOf(this.m_gui, "btnInviteFriend").GetComponent<UIButton>();
		this.m_btnInvite.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnInviteClicked));
		this.m_matchmakingSearchIcon = GuiUtils.FindChildOfComponent<PackedSprite>(this.m_gui, "loadingIconAnimated");
		this.m_matchmakingSearchText = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "LblSearchingForPlayers");
		this.m_matchmakeCheckbox = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(this.m_gui, "MatchmakeCheckbox");
		this.m_matchmakeCheckbox.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMatchmakeChanged));
		this.m_lblGameName = GuiUtils.FindChildOf(this.m_gui, "LobbyTitleLabel").GetComponent<SpriteText>();
		GuiUtils.FindChildOf(this.m_gui, "ActiveHostLabel").GetComponent<SpriteText>().Text = hostName;
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui, "Team1List");
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_gui, "Team2List");
		if (gameObject != null)
		{
			this.m_team1List = gameObject.GetComponent<UIScrollList>();
		}
		if (gameObject2 != null)
		{
			this.m_team2List = gameObject2.GetComponent<UIScrollList>();
		}
		this.m_mapIcon = GuiUtils.FindChildOf(this.m_gui, "Map").GetComponent<SimpleSprite>();
		this.m_lblMapName = GuiUtils.FindChildOf(this.m_gui, "MapValueLabel").GetComponent<SpriteText>();
		this.m_lblMapSize = GuiUtils.FindChildOf(this.m_gui, "MapSizeValueLabel").GetComponent<SpriteText>();
		this.m_btnDisband = GuiUtils.FindChildOf(this.m_gui, "btnDisband").GetComponent<UIButton>();
		this.m_btnLeave = GuiUtils.FindChildOf(this.m_gui, "btnLeave").GetComponent<UIButton>();
		this.m_btnRename = GuiUtils.FindChildOf(this.m_gui, "RenameGameButton").GetComponent<UIButton>();
		this.m_lblFleetSize = GuiUtils.FindChildOf(this.m_gui, "FleetSizeValueLabel").GetComponent<SpriteText>();
		this.m_lblNrOfPlayers = GuiUtils.FindChildOf(this.m_gui, "PlayersValueLabel").GetComponent<SpriteText>();
		this.m_lblMaxTurnTime = GuiUtils.FindChildOf(this.m_gui, "PlanningTimerValueLabel").GetComponent<SpriteText>();
		this.m_lblGameType = GuiUtils.FindChildOf(this.m_gui, "GametypeValueLabel").GetComponent<SpriteText>();
		this.m_lblMissionDescription = GuiUtils.FindChildOf(this.m_gui, "GametypeMissionDescriptionValueLabel").GetComponent<SpriteText>();
		GuiUtils.FindChildOf(this.m_gui, "btnBack").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnBackPressed));
		this.m_btnDisband.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnDisbandGamePressed));
		this.m_btnLeave.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnLeaveGamePressed));
		this.m_btnRename.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRenameGamePressed));
		GameObject gameObject3 = GuiUtils.FindChildOf(this.m_gui, "PointGoalTitleLabel");
		if (gameObject3 != null)
		{
			this.m_lblPointGoalTitel = gameObject3.GetComponent<SpriteText>();
		}
		GameObject gameObject4 = GuiUtils.FindChildOf(this.m_gui, "PointGoalValueLabel");
		if (gameObject4 != null)
		{
			this.m_lblPointGoalValue = gameObject4.GetComponent<SpriteText>();
		}
		this.m_invitePanel.m_inviteGui = GuiUtils.CreateGui("Lobby/LobbyInviteFriendDialog", guiCamera);
		this.m_invitePanel.m_inviteFriendDialog = this.m_invitePanel.m_inviteGui.GetComponent<UIPanel>();
		this.m_invitePanel.m_friendList = GuiUtils.FindChildOf(this.m_invitePanel.m_inviteFriendDialog.gameObject, "InviteFriendDialogList").GetComponent<UIScrollList>();
		this.m_invitePanel.m_friendListItem = GuiUtils.FindChildOf(this.m_invitePanel.m_inviteFriendDialog.gameObject, "InviteFriendListItem");
		GuiUtils.FindChildOf(this.m_invitePanel.m_inviteFriendDialog.gameObject, "InviteFriendDialog_Cancel_Button").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnInviteCancel));
		GuiUtils.FindChildOf(this.m_invitePanel.m_inviteFriendDialog.gameObject, "InviteFriendDialog_Select_Button").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnInviteSelected));
		this.m_invitePanel.m_inviteFriendDialog.Dismiss();
		this.m_selectFleetGui = GuiUtils.CreateGui("Lobby/LobbyOpenFleetDialog", guiCamera);
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Cancel_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnCancelOpenFleet));
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Edit_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnEditOpenFleet));
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Select_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSelectOpenFleet));
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Createnewfleet_Button").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNewFleet));
		this.SetModifyFleetButtonsStatus(false);
		this.m_fleetList = GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialogList").GetComponent<UIScrollList>();
		this.m_gameFleetItem = (Resources.Load("gui/Shipyard/FleetListItem") as GameObject);
		DebugUtils.Assert(this.m_gameFleetItem != null);
		this.SetupChat();
		this.FillFleetList();
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Combine(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
		this.m_selectFleetGui.GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00019EB0 File Offset: 0x000180B0
	private void OnRenameGamePressed(IUIObject obj)
	{
		PLog.Log("rename");
		if (this.m_renameDialog == null)
		{
			this.m_renameDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$label_renamegame"), this.m_settings.m_gameName, new GenericTextInput.InputTextCancel(this.OnRenameCancel), new GenericTextInput.InputTextCommit(this.OnRenameOk));
		}
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00019F1C File Offset: 0x0001811C
	private void OnRenameOk(string name)
	{
		this.m_rpc.Invoke("RenameGame", new object[]
		{
			name
		});
		UnityEngine.Object.Destroy(this.m_renameDialog);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00019F44 File Offset: 0x00018144
	private void OnRenameCancel()
	{
		UnityEngine.Object.Destroy(this.m_renameDialog);
	}

	// Token: 0x0600033E RID: 830 RVA: 0x00019F54 File Offset: 0x00018154
	private void OnUserManUpdate()
	{
		this.FillFleetList();
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00019F5C File Offset: 0x0001815C
	private int GetPlayersInTeam(int team)
	{
		int num = 0;
		foreach (LobbyPlayer lobbyPlayer in this.m_playerList)
		{
			if (lobbyPlayer.m_team == team)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000340 RID: 832 RVA: 0x00019FD0 File Offset: 0x000181D0
	private void OnMatchmakeChanged(IUIObject obj)
	{
		this.m_rpc.Invoke("AllowMatchmaking", new object[]
		{
			this.m_matchmakeCheckbox.StateNum == 0
		});
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0001A00C File Offset: 0x0001820C
	public void Setup(bool noFleet, bool allowMatchmaking, GameSettings settings, List<LobbyPlayer> players)
	{
		DebugUtils.Assert(players != null);
		DebugUtils.Assert(players.Count > 0);
		this.m_noFleet = noFleet;
		this.m_allowMatchmaking = allowMatchmaking;
		this.m_settings = settings;
		this.m_playerList = players;
		if (!this.m_visible)
		{
			return;
		}
		if (settings.m_nrOfPlayers == 1 || !settings.m_localAdmin)
		{
			this.m_matchmakeCheckbox.controlIsEnabled = false;
		}
		else
		{
			this.m_matchmakeCheckbox.SetToggleState((!allowMatchmaking) ? 1 : 0);
		}
		if (settings.m_nrOfPlayers > 1 && players.Count < settings.m_nrOfPlayers && allowMatchmaking)
		{
			this.m_matchmakingSearchIcon.gameObject.SetActiveRecursively(true);
			this.m_matchmakingSearchText.gameObject.SetActiveRecursively(true);
		}
		else
		{
			this.m_matchmakingSearchIcon.gameObject.SetActiveRecursively(false);
			this.m_matchmakingSearchText.gameObject.SetActiveRecursively(false);
		}
		foreach (LobbyPlayer lobbyPlayer in players)
		{
			if (lobbyPlayer.m_id == this.m_settings.m_localPlayerID)
			{
				this.m_currentPlayer = lobbyPlayer;
				this.m_selectedFleet = lobbyPlayer.m_fleet;
				break;
			}
		}
		DebugUtils.Assert(this.m_currentPlayer != null);
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialogSubHeader").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey("gamelobby_fleetsize") + " " + this.GetFleetSize(this.m_currentPlayer);
		this.m_lblGameName.Text = this.m_settings.m_gameName;
		this.SetupMapIfno(this.m_mapInfo);
		if (this.m_team1List != null)
		{
			this.m_team1List.ClearList(true);
		}
		if (this.m_team2List != null)
		{
			this.m_team2List.ClearList(true);
		}
		foreach (LobbyPlayer player in players)
		{
			this.AddPlayer(player);
		}
		this.m_btnReady.Text = Localize.instance.Translate((!this.m_currentPlayer.m_readyToStart) ? "$gamelobby_ready" : "$gamelobby_unready");
		if (this.m_noFleet)
		{
			this.m_btnSelectFleet.gameObject.SetActiveRecursively(false);
			this.m_btnFleetEdit.gameObject.SetActiveRecursively(false);
		}
		else if (this.m_settings.m_gameType == GameType.Campaign || this.m_settings.m_gameType == GameType.Challenge)
		{
			this.m_btnSelectFleet.gameObject.SetActiveRecursively(false);
			this.m_btnFleetEdit.gameObject.SetActiveRecursively(true);
			this.m_btnFleetEdit.controlIsEnabled = (this.m_selectedFleet != string.Empty && !this.m_currentPlayer.m_readyToStart);
		}
		else
		{
			this.m_btnSelectFleet.gameObject.SetActiveRecursively(true);
			this.m_btnFleetEdit.gameObject.SetActiveRecursively(false);
			bool activeRecursively = !this.HasValidFleet(this.m_currentPlayer);
			this.m_btnSelectFleet.transform.FindChild("glow").gameObject.SetActiveRecursively(activeRecursively);
		}
		bool flag = this.m_settings.m_gameType == GameType.Points || this.m_settings.m_gameType == GameType.Assassination;
		if (flag)
		{
			this.m_btnSwitchTeam.gameObject.SetActiveRecursively(true);
		}
		else
		{
			this.m_btnSwitchTeam.gameObject.SetActiveRecursively(false);
		}
		this.m_btnDisband.gameObject.SetActiveRecursively(this.m_currentPlayer.m_admin);
		this.m_btnLeave.gameObject.SetActiveRecursively(!this.m_currentPlayer.m_admin);
		this.m_btnRename.gameObject.SetActiveRecursively(this.m_currentPlayer.m_admin);
		this.m_btnInvite.gameObject.SetActiveRecursively(this.m_currentPlayer.m_admin && this.m_settings.m_nrOfPlayers > 1 && players.Count < this.m_settings.m_nrOfPlayers);
		if (noFleet)
		{
			this.m_btnReady.gameObject.GetComponent<UIButton>().controlIsEnabled = true;
		}
		else if (this.HasValidFleet(this.m_currentPlayer))
		{
			this.m_btnReady.gameObject.GetComponent<UIButton>().controlIsEnabled = true;
		}
		else
		{
			this.m_btnReady.gameObject.GetComponent<UIButton>().controlIsEnabled = false;
		}
		if (this.m_currentPlayer.m_readyToStart)
		{
			this.m_btnSelectFleet.gameObject.GetComponent<UIButton>().controlIsEnabled = false;
			this.m_btnSwitchTeam.gameObject.GetComponent<UIButton>().controlIsEnabled = false;
			this.m_btnReady.transform.FindChild("glow").gameObject.SetActiveRecursively(false);
		}
		else
		{
			this.m_btnSelectFleet.gameObject.GetComponent<UIButton>().controlIsEnabled = true;
			this.m_btnSwitchTeam.gameObject.GetComponent<UIButton>().controlIsEnabled = true;
			this.m_btnReady.transform.FindChild("glow").gameObject.SetActiveRecursively(true);
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0001A5B8 File Offset: 0x000187B8
	private bool HasValidFleet(LobbyPlayer player)
	{
		return this.HasValidFleetSize(player, player.m_fleetValue);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0001A5C8 File Offset: 0x000187C8
	private bool Is2VS1Game()
	{
		return (this.m_settings.m_gameType == GameType.Assassination || this.m_settings.m_gameType == GameType.Points) && this.m_settings.m_nrOfPlayers == 3;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x0001A60C File Offset: 0x0001880C
	private bool HasValidFleetSize(LobbyPlayer player, int size)
	{
		bool dubble = this.NeedDoubleFleet(player);
		return this.m_settings.m_fleetSizeLimits.ValidSize(size, dubble);
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0001A63C File Offset: 0x0001883C
	private void AddPlayer(LobbyPlayer player)
	{
		GameObject gameObject = null;
		if (this.m_settings.m_gameType == GameType.Campaign || this.m_settings.m_gameType == GameType.Challenge)
		{
			switch (player.m_id)
			{
			case 0:
				gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam1PlayerItem", this.m_guiCamera);
				break;
			case 1:
				gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam2PlayerItem", this.m_guiCamera);
				break;
			case 2:
				gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam3PlayerItem", this.m_guiCamera);
				break;
			case 3:
				gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam4PlayerItem", this.m_guiCamera);
				break;
			}
		}
		else
		{
			int team = player.m_team;
			if (team != 0)
			{
				if (team == 1)
				{
					gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam2PlayerItem", this.m_guiCamera);
				}
			}
			else
			{
				gameObject = GuiUtils.CreateGui("Lobby/LobbyTeam1PlayerItem", this.m_guiCamera);
			}
		}
		bool flag = player.m_id == this.m_settings.m_localPlayerID;
		GuiUtils.FindChildOf(gameObject, "lblPlayerName").GetComponent<SpriteText>().Text = player.m_name;
		SimpleSprite component = GuiUtils.FindChildOf(gameObject, "playerFlag").GetComponent<SimpleSprite>();
		Texture2D flagTexture = GuiUtils.GetFlagTexture(player.m_flag);
		GuiUtils.SetImage(component, flagTexture);
		if (!player.m_readyToStart)
		{
			GuiUtils.FindChildOf(gameObject, "PlayerReady").transform.position = new Vector3(0f, 10000f, 0f);
		}
		SpriteText component2 = GuiUtils.FindChildOf(gameObject, "SelectedFleetLabel").GetComponent<SpriteText>();
		SpriteText component3 = GuiUtils.FindChildOf(gameObject, "FleetValueLabel").GetComponent<SpriteText>();
		if (this.m_noFleet)
		{
			component2.Text = string.Empty;
			component3.Text = string.Empty;
		}
		else if (flag)
		{
			if (player.m_fleet == string.Empty)
			{
				component2.Text = Localize.instance.Translate("$lobby_nofleetselected");
				component3.Text = string.Empty;
			}
			else if (this.HasValidFleet(player))
			{
				component2.Text = player.m_fleet;
				component3.Text = player.m_fleetValue.ToString();
			}
			else
			{
				component2.Text = Localize.instance.Translate("$lobby_invalidsize");
				component3.Text = player.m_fleetValue.ToString();
				component2.SetColor(Color.red);
				component3.SetColor(Color.red);
			}
		}
		else if (player.m_fleet != string.Empty)
		{
			component2.Text = Localize.instance.Translate("$lobby_fleetselected");
			component3.Text = player.m_fleetValue.ToString();
		}
		else
		{
			component2.Text = string.Empty;
			component3.Text = string.Empty;
		}
		if (player.m_status != PlayerPresenceStatus.Offline || flag)
		{
			GuiUtils.FindChildOf(gameObject, "PlayerStatus_Offline").transform.position = new Vector3(0f, 10000f, 0f);
		}
		if (player.m_status != PlayerPresenceStatus.Online || flag)
		{
			GuiUtils.FindChildOf(gameObject, "PlayerStatus_Online").transform.position = new Vector3(0f, 10000f, 0f);
		}
		if (player.m_status != PlayerPresenceStatus.InGame || flag)
		{
			GuiUtils.FindChildOf(gameObject, "PlayerStatus_Present").transform.position = new Vector3(0f, 10000f, 0f);
		}
		SimpleSprite component4 = GuiUtils.FindChildOf(gameObject, "Player_Admin").GetComponent<SimpleSprite>();
		if (!player.m_admin)
		{
			component4.transform.position = new Vector3(0f, 10000f, 0f);
		}
		UIButton component5 = GuiUtils.FindChildOf(gameObject, "btnRemove").GetComponent<UIButton>();
		if (flag || !this.m_currentPlayer.m_admin)
		{
			component5.gameObject.transform.position = new Vector3(0f, 10000f, 0f);
		}
		else
		{
			component5.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRemoveClicked));
		}
		if (player.m_team == 1)
		{
			this.m_team2List.AddItem(gameObject.GetComponent<UIListItemContainer>());
		}
		else
		{
			this.m_team1List.AddItem(gameObject.GetComponent<UIListItemContainer>());
		}
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0001AAA0 File Offset: 0x00018CA0
	public void Update()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Update();
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Update();
		}
	}

	// Token: 0x06000347 RID: 839 RVA: 0x0001AAD4 File Offset: 0x00018CD4
	public void OnLevelWasLoaded()
	{
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.OnLevelWasLoaded();
		}
	}

	// Token: 0x06000348 RID: 840 RVA: 0x0001AAEC File Offset: 0x00018CEC
	private void FillFleetList()
	{
		this.m_selectedOpenFleet = string.Empty;
		this.m_fleets.Clear();
		this.m_fleetsSize.Clear();
		this.m_fleetList.SelectedItem = null;
		this.m_fleetList.ClearList(true);
		List<FleetDef> fleetDefs = this.m_userManClient.GetFleetDefs(0);
		foreach (FleetDef fleetDef in fleetDefs)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_gameFleetItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Name").GetComponent<SpriteText>().Text = fleetDef.m_name;
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Ships").GetComponent<SpriteText>().Text = fleetDef.m_ships.Count.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Points").GetComponent<SpriteText>().Text = fleetDef.m_value.ToString();
			GuiUtils.FindChildOf(gameObject, "FleetListItem_Size").GetComponent<SpriteText>().Text = Localize.instance.TranslateKey(FleetSizes.GetSizeClassName(fleetDef.m_value));
			UIRadioBtn component = GuiUtils.FindChildOf(gameObject, "RadioButton").GetComponent<UIRadioBtn>();
			component.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOpenFleetListSelection));
			UIListItemContainer component2 = gameObject.GetComponent<UIListItemContainer>();
			if (!fleetDef.m_available)
			{
				component.controlIsEnabled = false;
			}
			else
			{
				GuiUtils.FindChildOf(gameObject, "LblDLCNotAvailable").transform.Translate(new Vector3(0f, 0f, 20f));
			}
			this.m_fleetList.AddItem(component2);
			this.m_fleets.Add(fleetDef.m_name);
			this.m_fleetsSize.Add(fleetDef.m_value);
		}
		this.SetModifyFleetButtonsStatus(false);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0001ACD0 File Offset: 0x00018ED0
	public void Hide()
	{
		DebugUtils.Assert(this.m_gui != null);
		this.m_gui.SetActiveRecursively(false);
		this.m_selectFleetGui.SetActiveRecursively(false);
		this.m_invitePanel.m_inviteGui.SetActiveRecursively(false);
		this.m_visible = false;
	}

	// Token: 0x0600034A RID: 842 RVA: 0x0001AD20 File Offset: 0x00018F20
	public void Show()
	{
		if (this.m_visible)
		{
			return;
		}
		this.m_visible = true;
		this.m_gui.SetActiveRecursively(true);
		this.Setup(this.m_noFleet, this.m_allowMatchmaking, this.m_settings, this.m_playerList);
	}

	// Token: 0x0600034B RID: 843 RVA: 0x0001AD60 File Offset: 0x00018F60
	public void Close()
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		this.m_rpc.Unregister("Friends");
		if (this.m_chooseFleetMenu != null)
		{
			this.m_chooseFleetMenu.Close();
			this.m_chooseFleetMenu = null;
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Close();
			this.m_fleetEditor = null;
		}
		if (this.m_renameDialog != null)
		{
			UnityEngine.Object.Destroy(this.m_renameDialog);
			this.m_renameDialog = null;
		}
		if (this.m_gui != null)
		{
			UnityEngine.Object.Destroy(this.m_gui);
			this.m_gui = null;
		}
		UnityEngine.Object.Destroy(this.m_selectFleetGui);
		UnityEngine.Object.Destroy(this.m_invitePanel.m_inviteGui);
		UnityEngine.Object.Destroy(this.m_chat.m_listItem);
		ChatClient chatClient = this.m_chatClient;
		chatClient.m_onNewMessage = (Action<ChannelID, ChatClient.ChatMessage>)Delegate.Remove(chatClient.m_onNewMessage, new Action<ChannelID, ChatClient.ChatMessage>(this.OnNewChatMessage));
		UserManClient userManClient = this.m_userManClient;
		userManClient.m_onUpdated = (UserManClient.UpdatedHandler)Delegate.Remove(userManClient.m_onUpdated, new UserManClient.UpdatedHandler(this.OnUserManUpdate));
	}

	// Token: 0x0600034C RID: 844 RVA: 0x0001AE94 File Offset: 0x00019094
	private void LoadGUI(GameObject guiCamera)
	{
		DebugUtils.Assert(guiCamera != null, "LobbyMenu ctor called with NULL camera !");
		this.m_gui = GuiUtils.CreateGui("LobbyMenu", guiCamera);
		DebugUtils.Assert(this.m_gui != null, "LobbyMenu failed to validate root object m_gui !");
	}

	// Token: 0x0600034D RID: 845 RVA: 0x0001AEDC File Offset: 0x000190DC
	public static string TranslatedMapName(string mapName)
	{
		return Localize.instance.TranslateKey("mapname_" + mapName);
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0001AEF4 File Offset: 0x000190F4
	private bool NeedDoubleFleet(LobbyPlayer player)
	{
		return this.Is2VS1Game() && this.GetPlayersInTeam(player.m_team) == 1;
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0001AF24 File Offset: 0x00019124
	private string GetFleetSize(LobbyPlayer player)
	{
		string result = string.Empty;
		if (this.m_settings.m_gameType == GameType.Campaign || this.m_settings.m_gameType == GameType.Challenge)
		{
			result = this.m_settings.m_fleetSizeLimits.ToString();
		}
		else
		{
			int num = this.m_settings.m_fleetSizeLimits.max;
			if (this.NeedDoubleFleet(player))
			{
				num *= 2;
			}
			result = Localize.instance.TranslateKey(FleetSizes.GetSizeClassName(this.m_settings.m_fleetSizeLimits)) + " (" + num.ToString() + ")";
		}
		return result;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x0001AFC4 File Offset: 0x000191C4
	private void SetupMapIfno(MapInfo map)
	{
		DebugUtils.Assert(this.m_mapIcon != null);
		DebugUtils.Assert(this.m_lblMapName != null);
		if (map.m_thumbnail == string.Empty)
		{
			return;
		}
		Texture2D texture2D = Resources.Load("MapThumbs/" + map.m_thumbnail) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogWarning("Map Thumbnail " + map.m_thumbnail + " is missing");
			return;
		}
		GuiUtils.SetImage(this.m_mapIcon, texture2D);
		switch (this.m_settings.m_gameType)
		{
		case GameType.Challenge:
			this.m_lblGameType.Text = Localize.instance.Translate("$lobby_challenge");
			goto IL_13F;
		case GameType.Campaign:
			this.m_lblGameType.Text = Localize.instance.Translate("$lobby_campaign");
			goto IL_13F;
		case GameType.Points:
			this.m_lblGameType.Text = Localize.instance.Translate("$lobby_skirmish");
			goto IL_13F;
		case GameType.Assassination:
			this.m_lblGameType.Text = Localize.instance.Translate("$lobby_assassination");
			goto IL_13F;
		}
		DebugUtils.Assert(false, "Invalid game mode");
		IL_13F:
		this.m_lblMapName.Text = LobbyMenu.TranslatedMapName(map.m_name);
		this.m_lblMapSize.Text = Localize.instance.Translate(map.m_size.ToString() + "x" + map.m_size.ToString());
		if (this.m_settings.m_gameType == GameType.Campaign || this.m_settings.m_gameType == GameType.Challenge)
		{
			string text = "$creategame_fleetsize: " + this.m_settings.m_fleetSizeLimits.ToString() + " $label_pointssmall";
			GuiUtils.FindChildOf(this.m_gui, "FleetSizeBigLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(text);
			this.m_lblFleetSize.Text = this.m_settings.m_fleetSizeLimits.ToString();
		}
		else
		{
			this.m_lblFleetSize.Text = Localize.instance.TranslateKey(FleetSizes.GetSizeClassName(this.m_settings.m_fleetSizeLimits)) + " (" + this.m_settings.m_fleetSizeLimits.ToString() + ")";
		}
		this.m_lblNrOfPlayers.Text = Localize.instance.Translate(this.m_settings.m_nrOfPlayers.ToString() + Localize.instance.Translate(" $label_players"));
		if (this.m_settings.m_maxTurnTime > 0.0)
		{
			this.m_lblMaxTurnTime.Text = Localize.instance.Translate(Utils.FormatTimeLeftString(this.m_settings.m_maxTurnTime));
		}
		else
		{
			this.m_lblMaxTurnTime.Text = Localize.instance.Translate("$label_none");
		}
		if (this.m_settings.m_gameType == GameType.Campaign || this.m_settings.m_gameType == GameType.Challenge || this.m_settings.m_gameType == GameType.Custom)
		{
			this.m_lblMissionDescription.Text = Localize.instance.Translate(map.m_description);
		}
		else
		{
			GameType gameType = this.m_settings.m_gameType;
			if (gameType != GameType.Points)
			{
				if (gameType == GameType.Assassination)
				{
					this.m_lblMissionDescription.Text = Localize.instance.Translate("$creategame_desc_ass");
				}
			}
			else
			{
				this.m_lblMissionDescription.Text = Localize.instance.Translate("$creategame_desc_skirmish");
			}
		}
		if (this.m_settings.m_gameType == GameType.Points)
		{
			int num = (int)((float)this.m_settings.m_fleetSizeLimits.max * this.m_settings.m_targetScore);
			if (this.m_settings.m_nrOfPlayers >= 3)
			{
				num *= 2;
			}
			this.m_lblPointGoalValue.Text = num.ToString();
		}
		else
		{
			if (this.m_lblPointGoalTitel != null)
			{
				this.m_lblPointGoalTitel.gameObject.SetActiveRecursively(false);
			}
			if (this.m_lblPointGoalValue != null)
			{
				this.m_lblPointGoalValue.gameObject.SetActiveRecursively(false);
			}
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0001B40C File Offset: 0x0001960C
	private void Exit()
	{
		if (this.m_onExit != null)
		{
			this.m_onExit();
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x0001B424 File Offset: 0x00019624
	private void SetModifyFleetButtonsStatus(bool enable)
	{
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Select_Button").GetComponent<UIButton>().controlIsEnabled = enable;
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Edit_Button").GetComponent<UIButton>().controlIsEnabled = enable;
	}

	// Token: 0x06000353 RID: 851 RVA: 0x0001B468 File Offset: 0x00019668
	private void OnSwitchTeam(IUIObject obj)
	{
		if (!this.m_visible)
		{
			return;
		}
		this.m_rpc.Invoke("SwitchTeam", new object[0]);
	}

	// Token: 0x06000354 RID: 852 RVA: 0x0001B498 File Offset: 0x00019698
	private void OnReadyPressed(IUIObject obj)
	{
		if (!this.m_visible)
		{
			return;
		}
		this.m_rpc.Invoke("ReadyToStart", new object[0]);
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0001B4C8 File Offset: 0x000196C8
	private void OnBackPressed(IUIObject obj)
	{
		if (!this.m_visible)
		{
			return;
		}
		this.Exit();
	}

	// Token: 0x06000356 RID: 854 RVA: 0x0001B4DC File Offset: 0x000196DC
	private void OnLeaveGamePressed(IUIObject obj)
	{
		if (!this.m_visible)
		{
			return;
		}
		if (this.m_currentPlayer.m_admin)
		{
			return;
		}
		this.m_msgBox = new MsgBox(this.m_guiCamera, MsgBox.Type.YesNo, "$Lobby_LeaveGame", null, null, new MsgBox.YesHandler(this.OnLeaveGameYes), new MsgBox.NoHandler(this.OnMsgBoxCancel));
	}

	// Token: 0x06000357 RID: 855 RVA: 0x0001B538 File Offset: 0x00019738
	private void OnDisbandGamePressed(IUIObject obj)
	{
		if (!this.m_visible)
		{
			return;
		}
		if (!this.m_currentPlayer.m_admin)
		{
			return;
		}
		this.m_msgBox = new MsgBox(this.m_guiCamera, MsgBox.Type.YesNo, "$Lobby_DisbandGame", null, null, new MsgBox.YesHandler(this.OnDisbandGameYes), new MsgBox.NoHandler(this.OnMsgBoxCancel));
	}

	// Token: 0x06000358 RID: 856 RVA: 0x0001B594 File Offset: 0x00019794
	private void OnMsgBoxCancel()
	{
		this.m_msgBox = null;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0001B5A0 File Offset: 0x000197A0
	private void OnDisbandGameYes()
	{
		this.m_msgBox = null;
		this.m_rpc.Invoke("DisbandGame", new object[0]);
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0001B5C0 File Offset: 0x000197C0
	private void OnLeaveGameYes()
	{
		this.m_msgBox = null;
		this.m_rpc.Invoke("KickSelf", new object[0]);
	}

	// Token: 0x0600035B RID: 859 RVA: 0x0001B5E0 File Offset: 0x000197E0
	private void OnRemoveClicked(IUIObject button)
	{
		if (!this.m_currentPlayer.m_admin)
		{
			return;
		}
		string text = GuiUtils.FindChildOf(button.transform.parent, "lblPlayerName").GetComponent<SpriteText>().Text;
		if (this.m_playerRemovedDelegate != null)
		{
			this.m_playerRemovedDelegate(text);
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0001B638 File Offset: 0x00019838
	private void OnChooseFleetClicked(LobbyMenu_Player sender)
	{
		PLog.Log("Lobby recived delegate that " + sender.GetPlayer().m_name + " wants to choose fleet.");
		this.SetupChooseFleet();
	}

	// Token: 0x0600035D RID: 861 RVA: 0x0001B660 File Offset: 0x00019860
	private void OnInviteClicked(IUIObject button)
	{
		DebugUtils.Assert(this.m_currentPlayer != null, "LobbyMenu::OnInviteClicked() no current player specified !");
		DebugUtils.Assert(this.m_currentPlayer.m_admin, "LobbyMenu::OnInviteClicked() current player is not admin !");
		this.m_invitePanel.m_inviteFriendDialog.gameObject.SetActiveRecursively(true);
		this.m_invitePanel.m_inviteFriendDialog.BringIn();
		this.m_invitePanel.m_selectedIndex = -1;
		this.m_rpc.Register("Friends", new PTech.RPC.Handler(this.RPC_Friends));
		this.m_rpc.Invoke("RequestFriends", new object[0]);
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0001B6FC File Offset: 0x000198FC
	private void OnInviteCancel(IUIObject obj)
	{
		this.m_invitePanel.m_friendList.ClearList(true);
		this.m_invitePanel.m_inviteFriendDialog.Dismiss();
	}

	// Token: 0x0600035F RID: 863 RVA: 0x0001B720 File Offset: 0x00019920
	private void OnInviteSelected(IUIObject obj)
	{
		if (this.m_invitePanel.m_selectedIndex == -1)
		{
			return;
		}
		FriendData friendData = this.m_invitePanel.m_friends[this.m_invitePanel.m_selectedIndex];
		PLog.Log("invite friend " + friendData.m_name);
		this.m_rpc.Invoke("Invite", new object[]
		{
			friendData.m_friendID
		});
		this.m_invitePanel.m_friendList.ClearList(true);
		this.m_invitePanel.m_inviteFriendDialog.Dismiss();
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0001B7B8 File Offset: 0x000199B8
	private void OnFriendPressed(IUIObject obj)
	{
		this.m_invitePanel.m_selectedIndex = obj.transform.parent.GetComponent<UIListItemContainer>().Index;
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0001B7E8 File Offset: 0x000199E8
	private void RPC_Friends(PTech.RPC rpc, List<object> args)
	{
		rpc.Unregister("Friends");
		this.m_invitePanel.m_friends.Clear();
		foreach (object obj in args)
		{
			FriendData friendData = new FriendData();
			friendData.FromArray((byte[])obj);
			if (friendData.m_status == FriendData.FriendStatus.IsFriend)
			{
				this.m_invitePanel.m_friends.Add(friendData);
			}
		}
		this.m_invitePanel.m_friendList.ClearList(true);
		foreach (FriendData friendData2 in this.m_invitePanel.m_friends)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.m_invitePanel.m_friendListItem) as GameObject;
			GuiUtils.FindChildOf(gameObject, "FriendNameLabel").GetComponent<SpriteText>().Text = friendData2.m_name;
			SimpleSprite component = GuiUtils.FindChildOf(gameObject, "FriendOnlineStatus").GetComponent<SimpleSprite>();
			SimpleSprite component2 = GuiUtils.FindChildOf(gameObject, "FriendOfflineStatus").GetComponent<SimpleSprite>();
			if (friendData2.m_online)
			{
				component2.transform.Translate(0f, 0f, 20f);
			}
			else
			{
				component.transform.Translate(0f, 0f, 20f);
			}
			GuiUtils.FindChildOf(gameObject, "InviteFriendRadioButton").GetComponent<UIRadioBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnFriendPressed));
			SimpleSprite component3 = GuiUtils.FindChildOf(gameObject, "FriendFlag").GetComponent<SimpleSprite>();
			Texture2D flagTexture = GuiUtils.GetFlagTexture(friendData2.m_flagID);
			GuiUtils.SetImage(component3, flagTexture);
			UIListItemContainer component4 = gameObject.GetComponent<UIListItemContainer>();
			GuiUtils.FixedItemContainerInstance(component4);
			this.m_invitePanel.m_friendList.AddItem(component4);
		}
	}

	// Token: 0x06000362 RID: 866 RVA: 0x0001BA04 File Offset: 0x00019C04
	private void SetupChooseFleet()
	{
		this.m_selectFleetGui.SetActiveRecursively(true);
		this.m_selectFleetGui.GetComponent<UIPanel>().BringIn();
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0001BA24 File Offset: 0x00019C24
	private void OnOpenFleetListSelection(IUIObject obj)
	{
		UIListItemContainer component = obj.transform.parent.GetComponent<UIListItemContainer>();
		this.m_selectedOpenFleet = this.m_fleets[component.Index];
		this.SetModifyFleetButtonsStatus(true);
		bool flag = this.HasValidFleetSize(this.m_currentPlayer, this.m_fleetsSize[component.Index]) || this.Is2VS1Game();
		PLog.Log(string.Concat(new object[]
		{
			"valid size ",
			flag,
			" size ",
			this.m_fleetsSize[component.Index]
		}));
		GuiUtils.FindChildOf(this.m_selectFleetGui, "OpenFleetDialog_Select_Button").GetComponent<UIButton>().controlIsEnabled = flag;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x0001BAEC File Offset: 0x00019CEC
	private void OnCancelOpenFleet(IUIObject obj)
	{
		this.m_selectFleetGui.GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0001BB00 File Offset: 0x00019D00
	private void OnNewFleet(IUIObject obj)
	{
		this.m_selectedFleet = string.Empty;
		this.SetupFleetEditor(true);
	}

	// Token: 0x06000366 RID: 870 RVA: 0x0001BB14 File Offset: 0x00019D14
	private void OnEditOpenFleet(IUIObject obj)
	{
		this.m_selectedFleet = this.m_selectedOpenFleet;
		this.SetupFleetEditor(true);
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0001BB2C File Offset: 0x00019D2C
	private void OnSelectOpenFleet(IUIObject obj)
	{
		this.OnCancelOpenFleet(null);
		this.m_rpc.Invoke("SetFleet", new object[]
		{
			this.m_selectedOpenFleet
		});
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0001BB60 File Offset: 0x00019D60
	private void OnSelectFleet(IUIObject obj)
	{
		this.SetupChooseFleet();
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0001BB68 File Offset: 0x00019D68
	private void OnEditFleet(IUIObject obj)
	{
		this.SetupFleetEditor(false);
	}

	// Token: 0x0600036A RID: 874 RVA: 0x0001BB74 File Offset: 0x00019D74
	private bool IsOneFleetOnly()
	{
		return this.m_mapInfo.m_gameMode == GameType.Campaign || this.m_mapInfo.m_gameMode == GameType.Challenge;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0001BBA0 File Offset: 0x00019DA0
	private void SetupFleetEditor(bool returnToFleetSelector)
	{
		this.Hide();
		this.m_fleetSelectorWasOpen = returnToFleetSelector;
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Close();
		}
		string text = this.m_selectedFleet;
		FleetSize fleetSize = this.m_settings.m_fleetSizeLimits;
		if (this.NeedDoubleFleet(this.m_currentPlayer))
		{
			fleetSize = FleetSizes.sizes[1];
		}
		if (string.IsNullOrEmpty(text))
		{
			text = string.Empty;
		}
		this.m_fleetEditor = new FleetMenu(this.m_guiCamera, this.m_userManClient, text, this.m_campaignID, fleetSize, this.IsOneFleetOnly(), this.m_musMan);
		this.m_fleetEditor.m_onExit = new FleetMenu.OnExitDelegate(this.OnFleetEditorExit);
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0001BC50 File Offset: 0x00019E50
	private void OnFleetEditorExit()
	{
		FleetMenu fleetEditor = this.m_fleetEditor;
		fleetEditor.m_onExit = (FleetMenu.OnExitDelegate)Delegate.Remove(fleetEditor.m_onExit, new FleetMenu.OnExitDelegate(this.OnFleetEditorExit));
		this.m_fleetEditor.Close();
		this.m_fleetEditor = null;
		this.Show();
		if (this.m_fleetSelectorWasOpen)
		{
			this.SetupChooseFleet();
		}
		if (!string.IsNullOrEmpty(this.m_selectedFleet))
		{
			this.m_rpc.Invoke("FleetUpdated", new object[0]);
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x0001BCD4 File Offset: 0x00019ED4
	private void SetupChat()
	{
		GameObject go = GuiUtils.FindChildOf(this.m_gui, "ChatContainer");
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

	// Token: 0x0600036E RID: 878 RVA: 0x0001BE10 File Offset: 0x0001A010
	private void OnSendChatMessage(IKeyFocusable control)
	{
		if (control.Content != string.Empty)
		{
			this.m_chatClient.SendMessage(ChannelID.General, control.Content);
			this.m_chat.m_textInput.Text = string.Empty;
		}
		UIManager.instance.FocusObject = this.m_chat.m_textInput;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x0001BE70 File Offset: 0x0001A070
	private void OnNewChatMessage(ChannelID channel, ChatClient.ChatMessage msg)
	{
		this.AddChatMessage(msg);
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0001BE7C File Offset: 0x0001A07C
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

	// Token: 0x040002AA RID: 682
	public Action m_onExit;

	// Token: 0x040002AB RID: 683
	public Action<string> m_playerRemovedDelegate;

	// Token: 0x040002AC RID: 684
	private GameObject m_gui;

	// Token: 0x040002AD RID: 685
	private PTech.RPC m_rpc;

	// Token: 0x040002AE RID: 686
	private ChatClient m_chatClient;

	// Token: 0x040002AF RID: 687
	private UserManClient m_userManClient;

	// Token: 0x040002B0 RID: 688
	private bool m_visible = true;

	// Token: 0x040002B1 RID: 689
	private LobbyPlayer m_currentPlayer;

	// Token: 0x040002B2 RID: 690
	private MapInfo m_mapInfo;

	// Token: 0x040002B3 RID: 691
	private GameSettings m_settings;

	// Token: 0x040002B4 RID: 692
	private GameObject m_guiCamera;

	// Token: 0x040002B5 RID: 693
	private int m_campaignID;

	// Token: 0x040002B6 RID: 694
	private string m_selectedFleet;

	// Token: 0x040002B7 RID: 695
	private bool m_noFleet;

	// Token: 0x040002B8 RID: 696
	private bool m_allowMatchmaking;

	// Token: 0x040002B9 RID: 697
	private SpriteText m_lblPointGoalTitel;

	// Token: 0x040002BA RID: 698
	private SpriteText m_lblPointGoalValue;

	// Token: 0x040002BB RID: 699
	private SpriteText m_lblGameName;

	// Token: 0x040002BC RID: 700
	private SpriteText m_lblMapName;

	// Token: 0x040002BD RID: 701
	private SpriteText m_lblMapSize;

	// Token: 0x040002BE RID: 702
	private SimpleSprite m_mapIcon;

	// Token: 0x040002BF RID: 703
	private SpriteText m_lblFleetSize;

	// Token: 0x040002C0 RID: 704
	private SpriteText m_lblNrOfPlayers;

	// Token: 0x040002C1 RID: 705
	private SpriteText m_lblMaxTurnTime;

	// Token: 0x040002C2 RID: 706
	private SpriteText m_lblGameType;

	// Token: 0x040002C3 RID: 707
	private SpriteText m_lblMissionDescription;

	// Token: 0x040002C4 RID: 708
	private UIScrollList m_team1List;

	// Token: 0x040002C5 RID: 709
	private UIScrollList m_team2List;

	// Token: 0x040002C6 RID: 710
	private UIButton m_btnDisband;

	// Token: 0x040002C7 RID: 711
	private UIButton m_btnLeave;

	// Token: 0x040002C8 RID: 712
	private UIButton m_btnRename;

	// Token: 0x040002C9 RID: 713
	private UIButton m_btnReady;

	// Token: 0x040002CA RID: 714
	private UIButton m_btnSwitchTeam;

	// Token: 0x040002CB RID: 715
	private UIButton m_btnFleetEdit;

	// Token: 0x040002CC RID: 716
	private UIButton m_btnSelectFleet;

	// Token: 0x040002CD RID: 717
	private UIButton m_btnInvite;

	// Token: 0x040002CE RID: 718
	private UIStateToggleBtn m_matchmakeCheckbox;

	// Token: 0x040002CF RID: 719
	private PackedSprite m_matchmakingSearchIcon;

	// Token: 0x040002D0 RID: 720
	private SpriteText m_matchmakingSearchText;

	// Token: 0x040002D1 RID: 721
	private LobbyMenu.InvitePanel m_invitePanel = new LobbyMenu.InvitePanel();

	// Token: 0x040002D2 RID: 722
	private LobbyMenu.Chat m_chat = new LobbyMenu.Chat();

	// Token: 0x040002D3 RID: 723
	private List<LobbyPlayer> m_playerList = new List<LobbyPlayer>();

	// Token: 0x040002D4 RID: 724
	private ChooseFleetMenu m_chooseFleetMenu;

	// Token: 0x040002D5 RID: 725
	private FleetMenu m_fleetEditor;

	// Token: 0x040002D6 RID: 726
	private MsgBox m_msgBox;

	// Token: 0x040002D7 RID: 727
	private GameObject m_renameDialog;

	// Token: 0x040002D8 RID: 728
	private GameObject m_selectFleetGui;

	// Token: 0x040002D9 RID: 729
	private UIScrollList m_fleetList;

	// Token: 0x040002DA RID: 730
	private GameObject m_gameFleetItem;

	// Token: 0x040002DB RID: 731
	private List<string> m_fleets = new List<string>();

	// Token: 0x040002DC RID: 732
	private List<int> m_fleetsSize = new List<int>();

	// Token: 0x040002DD RID: 733
	private string m_selectedOpenFleet;

	// Token: 0x040002DE RID: 734
	private bool m_fleetSelectorWasOpen;

	// Token: 0x040002DF RID: 735
	private MusicManager m_musMan;

	// Token: 0x0200004E RID: 78
	private class InvitePanel
	{
		// Token: 0x040002E0 RID: 736
		public GameObject m_inviteGui;

		// Token: 0x040002E1 RID: 737
		public UIPanel m_inviteFriendDialog;

		// Token: 0x040002E2 RID: 738
		public List<FriendData> m_friends = new List<FriendData>();

		// Token: 0x040002E3 RID: 739
		public UIScrollList m_friendList;

		// Token: 0x040002E4 RID: 740
		public GameObject m_friendListItem;

		// Token: 0x040002E5 RID: 741
		public int m_selectedIndex = -1;
	}

	// Token: 0x0200004F RID: 79
	private class Chat
	{
		// Token: 0x040002E6 RID: 742
		public UITextField m_textInput;

		// Token: 0x040002E7 RID: 743
		public UIScrollList m_messageList;

		// Token: 0x040002E8 RID: 744
		public GameObject m_listItem;
	}
}
