using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class ConnectMenu
{
	// Token: 0x060001B9 RID: 441 RVA: 0x0000A61C File Offset: 0x0000881C
	public ConnectMenu(GameObject guiCamera, MusicManager musMan, PdxNews pdxNews, GDPBackend gdpBackend)
	{
		this.m_guiCamera = guiCamera;
		this.m_musicMan = musMan;
		this.m_gdpBackend = gdpBackend;
		foreach (string text in Constants.m_languages)
		{
			this.m_languages.Add(Localize.instance.Translate(text));
		}
		this.SetupGui();
		this.LoadServerList();
		string @string = PlayerPrefs.GetString("LastHost");
		if (@string != string.Empty)
		{
			this.SetSelectedServer(@string);
		}
		this.m_newsTicker = new NewsTicker(pdxNews, gdpBackend, this.m_guiCamera);
		if (this.m_nameField.Text == string.Empty)
		{
			this.m_guiCamera.GetComponent<UIManager>().FocusObject = this.m_nameField;
		}
		else
		{
			this.m_guiCamera.GetComponent<UIManager>().FocusObject = this.m_passwordField;
		}
		this.m_musicMan.SetMusic("menu");
	}

	// Token: 0x060001BA RID: 442
	private void SetupGui()
	{
		if (this.m_gui != null)
		{
			UnityEngine.Object.Destroy(this.m_gui);
		}
		this.m_gui = GuiUtils.CreateGui("Login", this.m_guiCamera);
		this.m_loginButton = GuiUtils.FindChildOf(this.m_gui, "LoginButton").GetComponent<UIButton>();
		this.m_nameField = GuiUtils.FindChildOf(this.m_gui, "NameField").GetComponent<UITextField>();
		this.m_passwordField = GuiUtils.FindChildOf(this.m_gui, "PasswordField").GetComponent<UITextField>();
		this.m_loginButton.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnLoginPressed));
		GuiUtils.FindChildOf(this.m_gui, "OfflineButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOfflinePressed));
		GuiUtils.FindChildOf(this.m_gui, "ExitButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnExitPressed));
		GuiUtils.FindChildOf(this.m_gui, "OptionsButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOptionsPressed));
		GuiUtils.FindChildOf(this.m_gui, "LangArrowLeftButton").GetComponent<UIActionBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnLanguageLeft));
		GuiUtils.FindChildOf(this.m_gui, "LangArrowRightButton").GetComponent<UIActionBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnLanguageRight));
		GuiUtils.FindChildOf(this.m_gui, "VersionLbl").GetComponent<SpriteText>().Text = Localize.instance.Translate("$version " + VersionInfo.GetFullVersionString());
		GuiUtils.FindChildOf(this.m_gui, "HelpButton").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onHelp));
		this.m_serverNameLbl = GuiUtils.FindChildOfComponent<SpriteText>(this.m_gui, "ServerNameLbl");
		this.m_serverPrev = GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "ArrowLeftButton");
		this.m_serverNext = GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "ArrowRightButton");
		this.m_serverPrev.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnPrevServer));
		this.m_serverNext.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnNextServer));
		this.m_newAccountPanel = GuiUtils.FindChildOf(this.m_gui, "NewAccountPanel").GetComponent<UIPanel>();
		this.m_createName = GuiUtils.FindChildOf(this.m_newAccountPanel.gameObject, "UsernameField").GetComponent<UITextField>();
		this.m_createEmail = GuiUtils.FindChildOf(this.m_newAccountPanel.gameObject, "EmailField").GetComponent<UITextField>();
		this.m_createPwd1 = GuiUtils.FindChildOf(this.m_newAccountPanel.gameObject, "PasswordField_reg").GetComponent<UITextField>();
		this.m_createPwd2 = GuiUtils.FindChildOf(this.m_newAccountPanel.gameObject, "PasswordRepeatField").GetComponent<UITextField>();
		this.m_createButton = GuiUtils.FindChildOfComponent<UIButton>(this.m_newAccountPanel.gameObject, "CreateAccountAcceptButton");
		this.m_createButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCreateAccount));
		this.m_resetEmailField = GuiUtils.FindChildOfComponent<UITextField>(this.m_gui, "ResetEmailField");
		this.m_resetEmailField.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnRequestResetPasswordCodeFieldOk));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "ResetAcceptButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRequestResetPasswordCodeOk));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "ResetCancelButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRequestResetPasswordCodeCancel));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_gui, "BtnResendVerification").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRequestVerificationMail));
		this.m_nameField.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnNameEnter));
		this.m_passwordField.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnPwdEnter));
		this.m_nameField.Text = PlayerPrefs.GetString("LastUserName");
		this.SetupLanguageButtons();
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000ABA8 File Offset: 0x00008DA8
	public void Close()
	{
		if (this.m_tokenDialog != null)
		{
			UnityEngine.Object.Destroy(this.m_tokenDialog);
			this.m_tokenDialog = null;
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_newsTicker.Close();
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000AC0C File Offset: 0x00008E0C
	public void Update()
	{
		if (Utils.IsAndroidBack())
		{
			this.OnExitPressed(null);
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			UIManager component = this.m_guiCamera.GetComponent<UIManager>();
			if (component.FocusObject == this.m_nameField)
			{
				component.FocusObject = this.m_passwordField;
			}
			else if (component.FocusObject == this.m_passwordField)
			{
				component.FocusObject = this.m_nameField;
			}
			if (component.FocusObject == this.m_createEmail)
			{
				component.FocusObject = this.m_createName;
			}
			else if (component.FocusObject == this.m_createName)
			{
				component.FocusObject = this.m_createPwd1;
			}
			else if (component.FocusObject == this.m_createPwd1)
			{
				component.FocusObject = this.m_createPwd2;
			}
			else if (component.FocusObject == this.m_createPwd2)
			{
				component.FocusObject = this.m_createEmail;
			}
			if (this.m_resetPasswordDialog != null)
			{
				if (component.FocusObject == this.m_resetVerificationCode)
				{
					component.FocusObject = this.m_resetPwd1;
				}
				else if (component.FocusObject == this.m_resetPwd1)
				{
					component.FocusObject = this.m_resetPwd2;
				}
				else if (component.FocusObject == this.m_resetPwd2)
				{
					component.FocusObject = this.m_resetVerificationCode;
				}
			}
		}
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Update();
		}
		this.m_newsTicker.Update(Time.deltaTime);
		this.m_loginButton.controlIsEnabled = (this.m_nameField.Text.Length > 0 && this.m_passwordField.Text.Length > 0);
		this.UpdateCreateAccount();
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000ADDC File Offset: 0x00008FDC
	private void UpdateCreateAccount()
	{
		if (!this.m_newAccountPanel.gameObject.active)
		{
			return;
		}
		bool controlIsEnabled = this.m_createEmail.text.Length != 0 && this.m_createName.Text.Length != 0 && this.m_createPwd1.Text.Length != 0 && this.m_createPwd2.Text.Length != 0;
		this.m_createButton.controlIsEnabled = controlIsEnabled;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000AE64 File Offset: 0x00009064
	public void OnLoginFailed(ErrorCode errorCode)
	{
		switch (errorCode)
		{
		case ErrorCode.WrongUserPassword:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_incorrect", null);
			break;
		case ErrorCode.UserNotVerified:
			this.m_tokenDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$login_entertoken"), string.Empty, new GenericTextInput.InputTextCancel(this.OnTokenCancel), new GenericTextInput.InputTextCommit(this.OnTokenOk));
			break;
		case ErrorCode.InvalidVerificationToken:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_invalidtoken", null);
			break;
		case ErrorCode.VersionMissmatch:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_versionmissmatch", null);
			break;
		case ErrorCode.ServerFull:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_serverfull", null);
			break;
		}
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000AF48 File Offset: 0x00009148
	private void OnTokenCancel()
	{
		UnityEngine.Object.Destroy(this.m_tokenDialog);
		this.m_tokenDialog = null;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000AF5C File Offset: 0x0000915C
	private void OnTokenOk(string token)
	{
		if (token.Length == 0)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.m_tokenDialog);
		this.m_tokenDialog = null;
		this.Login(token);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000AF84 File Offset: 0x00009184
	public void OnConnectFailed()
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_connectfail", null);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000AFA0 File Offset: 0x000091A0
	private void OnNameEnter(IKeyFocusable field)
	{
		if (this.m_passwordField.Text.Length == 0)
		{
			UIManager component = this.m_guiCamera.GetComponent<UIManager>();
			component.FocusObject = this.m_passwordField;
		}
		else
		{
			this.Login(string.Empty);
		}
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000AFEC File Offset: 0x000091EC
	private void OnPwdEnter(IKeyFocusable field)
	{
		this.Login(string.Empty);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000AFFC File Offset: 0x000091FC
	private void OnLoginPressed(IUIObject obj)
	{
		this.Login(string.Empty);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000B00C File Offset: 0x0000920C
	private void Login(string token)
	{
		if (this.m_nameField.Text.Length > 0 && this.m_passwordField.Text.Length > 0)
		{
			ConnectMenu.ServerData serverData = this.GetServerData(this.m_selectedServer);
			if (serverData == null)
			{
				PLog.LogWarning("No server selected");
				return;
			}
			PlayerPrefs.SetString("LastHost", this.m_selectedServer);
			PlayerPrefs.SetString("LastUserName", this.m_nameField.Text);
			this.m_onConnect(serverData.m_name, serverData.m_host, serverData.m_port, this.m_nameField.Text, this.m_passwordField.Text, token);
		}
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000B0BC File Offset: 0x000092BC
	private void OnOfflinePressed(IUIObject obj)
	{
		this.m_onSinglePlayer();
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000B0CC File Offset: 0x000092CC
	private void OnExitPressed(IUIObject obj)
	{
		this.m_onExit();
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000B0DC File Offset: 0x000092DC
	private void OnOptionsPressed(IUIObject obj)
	{
		this.m_optionsWindow = new OptionsWindow(this.m_guiCamera, false);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000B0F0 File Offset: 0x000092F0
	private void OnCreateAccount(IUIObject obj)
	{
		Utils.ValidationStatus validationStatus = Utils.IsValidUsername(this.m_createName.Text);
		if (validationStatus == Utils.ValidationStatus.ToShort)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_usernametooshort", null);
			return;
		}
		if (validationStatus == Utils.ValidationStatus.InvalidCharacter)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_invalidusername", null);
			return;
		}
		if (!Utils.IsEmailAddress(this.m_createEmail.Text))
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_invalidemail", null);
			return;
		}
		Utils.ValidationStatus validationStatus2 = Utils.IsValidPassword(this.m_createPwd1.Text);
		if (validationStatus2 == Utils.ValidationStatus.ToShort)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_passwordtooshort", null);
			return;
		}
		if (validationStatus2 == Utils.ValidationStatus.InvalidCharacter)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_invalidpassword", null);
			return;
		}
		if (this.m_createPwd1.Text != this.m_createPwd2.Text)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_passwordmissmatch", null);
			return;
		}
		ConnectMenu.ServerData serverData = this.GetServerData(this.m_selectedServer);
		if (serverData == null)
		{
			PLog.LogWarning("No server selected");
			return;
		}
		this.m_onCreateAccount(serverData.m_host, serverData.m_port, this.m_createName.Text, this.m_createPwd1.Text, this.m_createEmail.Text);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000B258 File Offset: 0x00009458
	public void OnCreateFailed(ErrorCode error)
	{
		PLog.LogWarning("Creation failed , error code:" + error.ToString());
		if (error != ErrorCode.VersionMissmatch)
		{
			if (error == ErrorCode.AccountExist)
			{
				this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_accountexist", null);
			}
		}
		else
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_versionmissmatch", null);
		}
	}

	// Token: 0x060001CB RID: 459 RVA: 0x0000B2CC File Offset: 0x000094CC
	public void OnCreateSuccess()
	{
		PLog.Log("Account creation success");
		UIPanelManager uipanelManager = GuiUtils.FindChildOfComponent<UIPanelManager>(this.m_gui, "AdminPanelMan");
		uipanelManager.Dismiss();
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_accountcreated", new MsgBox.OkHandler(this.OnCloseCreatedMsgbox));
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000B31C File Offset: 0x0000951C
	private void OnCloseCreatedMsgbox()
	{
		GameObject gameObject = GuiUtils.FindChildOf(this.m_gui, "NewAccountPanel");
		gameObject.GetComponent<UIPanel>().Dismiss();
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000B348 File Offset: 0x00009548
	private void SetupLanguageButtons()
	{
		string language = Localize.instance.GetLanguage();
		for (int i = 0; i < this.m_languages.Count; i++)
		{
			if (this.m_languages[i] == language)
			{
				this.m_selectedLanguage = i;
				break;
			}
		}
		GuiUtils.FindChildOf(this.m_gui, "LanguageNameLbl").GetComponent<SpriteText>().Text = Localize.instance.Translate("$language_" + language);
		GuiUtils.FindChildOf(this.m_gui, "LangArrowLeftButton").GetComponent<UIButton>().controlIsEnabled = (this.m_selectedLanguage > 0);
		GuiUtils.FindChildOf(this.m_gui, "LangArrowRightButton").GetComponent<UIButton>().controlIsEnabled = (this.m_selectedLanguage < this.m_languages.Count - 1);
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000B420 File Offset: 0x00009620
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

	// Token: 0x060001CF RID: 463 RVA: 0x0000B480 File Offset: 0x00009680
	private void OnLanguageLeft(IUIObject button)
	{
		if (this.m_selectedLanguage <= 0)
		{
			return;
		}
		this.m_selectedLanguage--;
		string text = this.m_languages[this.m_selectedLanguage];
		Localize.instance.SetLanguage(text);
		PlayerPrefs.SetString("Language", text);
		this.SetupGui();
		this.SetSelectedServer(this.m_selectedServer);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000B4E4 File Offset: 0x000096E4
	private void OnLanguageRight(IUIObject button)
	{
		if (this.m_selectedLanguage >= this.m_languages.Count - 1)
		{
			return;
		}
		this.m_selectedLanguage++;
		string text = this.m_languages[this.m_selectedLanguage];
		Localize.instance.SetLanguage(text);
		PlayerPrefs.SetString("Language", text);
		this.SetupGui();
		this.SetSelectedServer(this.m_selectedServer);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000B554 File Offset: 0x00009754
	private int GetSelectedServerId(string name)
	{
		int num = 0;
		foreach (ConnectMenu.ServerData serverData in this.m_servers)
		{
			if (serverData.m_name == name)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000B5D4 File Offset: 0x000097D4
	private ConnectMenu.ServerData GetServerData(string name)
	{
		foreach (ConnectMenu.ServerData serverData in this.m_servers)
		{
			if (serverData.m_name == name)
			{
				return serverData;
			}
		}
		return null;
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000B650 File Offset: 0x00009850
	private void LoadServerList()
	{
		XmlDocument xmlDocument = Utils.LoadXml("serverlist");
		DebugUtils.Assert(xmlDocument != null);
		bool isDebugBuild = Debug.isDebugBuild;
		for (XmlNode xmlNode = xmlDocument.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "server")
			{
				ConnectMenu.ServerData serverData = new ConnectMenu.ServerData();
				serverData.m_name = xmlNode.Attributes["name"].Value;
				serverData.m_host = xmlNode.Attributes["host"].Value;
				serverData.m_port = int.Parse(xmlNode.Attributes["port"].Value);
				if (xmlNode.Attributes["dev"] != null)
				{
					bool flag = bool.Parse(xmlNode.Attributes["dev"].Value);
					if ((flag && !isDebugBuild) || (!flag && isDebugBuild))
					{
						xmlNode = xmlNode.NextSibling;
						continue;
					}
				}
				else if (Debug.isDebugBuild)
				{
					continue;
				}
				PLog.Log(string.Concat(new object[]
				{
					"Server ",
					serverData.m_name,
					" ",
					serverData.m_host,
					" ",
					serverData.m_port
				}));
				this.m_servers.Add(serverData);
			}
		}
		if (this.m_servers.Count > 0)
		{
			int index = 0;
			this.SetSelectedServer(this.m_servers[index].m_name);
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000B7F4 File Offset: 0x000099F4
	private void OnNextServer(IUIObject button)
	{
		int num = this.GetSelectedServerId(this.m_selectedServer);
		num++;
		if (num >= this.m_servers.Count - 1)
		{
			num = this.m_servers.Count - 1;
		}
		this.SetSelectedServer(this.m_servers[num].m_name);
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000B84C File Offset: 0x00009A4C
	private void OnPrevServer(IUIObject button)
	{
		int num = this.GetSelectedServerId(this.m_selectedServer);
		num--;
		if (num < 0)
		{
			num = 0;
		}
		this.SetSelectedServer(this.m_servers[num].m_name);
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000B88C File Offset: 0x00009A8C
	private void SetSelectedServer(string name)
	{
		PLog.LogWarning("Selecting " + name);
		int selectedServerId = this.GetSelectedServerId(name);
		if (selectedServerId < 0)
		{
			return;
		}
		this.m_selectedServer = name;
		this.m_serverNameLbl.Text = name;
		this.m_serverPrev.controlIsEnabled = (selectedServerId != 0);
		this.m_serverNext.controlIsEnabled = (selectedServerId != this.m_servers.Count - 1);
		this.m_serverNameLbl.Text = name;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000B908 File Offset: 0x00009B08
	private void OnRequestResetPasswordCodeFieldOk(IKeyFocusable field)
	{
		this.RequestResetPasswordCode();
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000B910 File Offset: 0x00009B10
	private void OnRequestResetPasswordCodeOk(IUIObject button)
	{
		this.RequestResetPasswordCode();
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000B918 File Offset: 0x00009B18
	private void RequestResetPasswordCode()
	{
		string text = this.m_resetEmailField.Text;
		if (!Utils.IsEmailAddress(text))
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_invalidemail", null);
			return;
		}
		ConnectMenu.ServerData serverData = this.GetServerData(this.m_selectedServer);
		if (serverData == null)
		{
			PLog.LogWarning("No server selected");
			return;
		}
		this.m_tempResetEmail = text;
		this.m_onRequestResetPasswordCode(serverData.m_host, serverData.m_port, text);
		this.m_resetPasswordDialog = GuiUtils.CreateGui("GenericInputDialog_passwordReset", this.m_guiCamera);
		GuiUtils.FindChildOfComponent<UIButton>(this.m_resetPasswordDialog, "OkButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnResetPasswordOk));
		GuiUtils.FindChildOfComponent<UIButton>(this.m_resetPasswordDialog, "CancelButton").SetValueChangedDelegate(new EZValueChangedDelegate(this.OnResetPasswordCancel));
		this.m_resetVerificationCode = GuiUtils.FindChildOfComponent<UITextField>(this.m_resetPasswordDialog, "txtInput_verification");
		this.m_resetPwd1 = GuiUtils.FindChildOfComponent<UITextField>(this.m_resetPasswordDialog, "txtInput_password");
		this.m_resetPwd2 = GuiUtils.FindChildOfComponent<UITextField>(this.m_resetPasswordDialog, "txtInput_passwordrepeat");
		UIManager component = this.m_guiCamera.GetComponent<UIManager>();
		component.FocusObject = this.m_resetVerificationCode;
		UIPanelManager uipanelManager = GuiUtils.FindChildOfComponent<UIPanelManager>(this.m_gui, "AdminPanelMan");
		uipanelManager.Dismiss();
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000BA5C File Offset: 0x00009C5C
	private void OnRequestResetPasswordCodeCancel(IUIObject button)
	{
		this.m_resetEmailField.Text = string.Empty;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000BA70 File Offset: 0x00009C70
	private void OnResetPasswordOk(IUIObject button)
	{
		if (this.m_resetVerificationCode.Text.Length == 0)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_needverificationcode", null);
			return;
		}
		Utils.ValidationStatus validationStatus = Utils.IsValidPassword(this.m_resetPwd1.Text);
		if (validationStatus == Utils.ValidationStatus.ToShort)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_passwordtooshort", null);
			return;
		}
		if (validationStatus == Utils.ValidationStatus.InvalidCharacter)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_invalidpassword", null);
			return;
		}
		if (this.m_resetPwd1.Text != this.m_resetPwd2.Text)
		{
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$createaccount_passwordmissmatch", null);
			return;
		}
		ConnectMenu.ServerData serverData = this.GetServerData(this.m_selectedServer);
		if (serverData == null)
		{
			PLog.LogWarning("No server selected");
			return;
		}
		this.m_onResetPassword(serverData.m_host, serverData.m_port, this.m_tempResetEmail, this.m_resetVerificationCode.Text, this.m_resetPwd1.Text);
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000BB84 File Offset: 0x00009D84
	private void OnResetPasswordCancel(IUIObject button)
	{
		UnityEngine.Object.Destroy(this.m_resetPasswordDialog);
		this.m_resetPasswordDialog = null;
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000BB98 File Offset: 0x00009D98
	public void OnResetPasswordConfirmed()
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_passwordresetconfirmed", null);
		UnityEngine.Object.Destroy(this.m_resetPasswordDialog);
		this.m_resetPasswordDialog = null;
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000BBC4 File Offset: 0x00009DC4
	public void OnResetPasswordFail()
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$label_passwordresetfailed", null);
		UnityEngine.Object.Destroy(this.m_resetPasswordDialog);
		this.m_resetPasswordDialog = null;
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000BBF0 File Offset: 0x00009DF0
	private void OnRequestVerificationMail(IUIObject button)
	{
		this.m_reqVerMailDialog = GuiUtils.OpenInputDialog(this.m_guiCamera, Localize.instance.Translate("$dialog_resendverificationcode"), string.Empty, new GenericTextInput.InputTextCancel(this.OnReqVerMailCancel), new GenericTextInput.InputTextCommit(this.OnReqVerMailOk));
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000BC3C File Offset: 0x00009E3C
	private void OnReqVerMailCancel()
	{
		UnityEngine.Object.Destroy(this.m_reqVerMailDialog);
		this.m_reqVerMailDialog = null;
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000BC50 File Offset: 0x00009E50
	private void OnReqVerMailOk(string text)
	{
		this.RequestVerificationEmail(text);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000BC5C File Offset: 0x00009E5C
	private void RequestVerificationEmail(string email)
	{
		ConnectMenu.ServerData serverData = this.GetServerData(this.m_selectedServer);
		if (serverData == null)
		{
			PLog.LogWarning("No server selected");
			return;
		}
		this.m_onRequestVerificationMail(serverData.m_host, serverData.m_port, email);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000BCA0 File Offset: 0x00009EA0
	public void OnRequestVerificaionRespons(ErrorCode errorCode)
	{
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		switch (errorCode)
		{
		case ErrorCode.UserAlreadyVerified:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$dialog_resendverificationcode_fail_alreadyverified", null);
			UnityEngine.Object.Destroy(this.m_reqVerMailDialog);
			this.m_reqVerMailDialog = null;
			break;
		case ErrorCode.AccountDoesNotExist:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$dialog_resendverificationcode_fail_accountnotfound", null);
			break;
		case ErrorCode.NoError:
			this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$dialog_resendverificationcode_success", null);
			UnityEngine.Object.Destroy(this.m_reqVerMailDialog);
			this.m_reqVerMailDialog = null;
			break;
		}
	}

	// Token: 0x04000112 RID: 274
	public ConnectMenu.LoginDelegatetHandler m_onConnect;

	// Token: 0x04000113 RID: 275
	public ConnectMenu.CreateAccountHandler m_onCreateAccount;

	// Token: 0x04000114 RID: 276
	public ConnectMenu.RequestVerificationHandler m_onRequestVerificationMail;

	// Token: 0x04000115 RID: 277
	public Action<string, int, string> m_onRequestResetPasswordCode;

	// Token: 0x04000116 RID: 278
	public ConnectMenu.ResetPasswordDelegate m_onResetPassword;

	// Token: 0x04000117 RID: 279
	public ConnectMenu.SingleHandler m_onSinglePlayer;

	// Token: 0x04000118 RID: 280
	public Action m_onExit;

	// Token: 0x04000119 RID: 281
	private UITextField m_nameField;

	// Token: 0x0400011A RID: 282
	private UITextField m_passwordField;

	// Token: 0x0400011B RID: 283
	private UIButton m_loginButton;

	// Token: 0x0400011C RID: 284
	private SpriteText m_serverNameLbl;

	// Token: 0x0400011D RID: 285
	private UIButton m_serverPrev;

	// Token: 0x0400011E RID: 286
	private UIButton m_serverNext;

	// Token: 0x0400011F RID: 287
	private UIPanel m_newAccountPanel;

	// Token: 0x04000120 RID: 288
	private UITextField m_createName;

	// Token: 0x04000121 RID: 289
	private UITextField m_createEmail;

	// Token: 0x04000122 RID: 290
	private UITextField m_createPwd1;

	// Token: 0x04000123 RID: 291
	private UITextField m_createPwd2;

	// Token: 0x04000124 RID: 292
	private UIButton m_createButton;

	// Token: 0x04000125 RID: 293
	private UITextField m_resetEmailField;

	// Token: 0x04000126 RID: 294
	private UITextField m_resetVerificationCode;

	// Token: 0x04000127 RID: 295
	private UITextField m_resetPwd1;

	// Token: 0x04000128 RID: 296
	private UITextField m_resetPwd2;

	// Token: 0x04000129 RID: 297
	private string m_selectedServer = string.Empty;

	// Token: 0x0400012A RID: 298
	private List<ConnectMenu.ServerData> m_servers = new List<ConnectMenu.ServerData>();

	// Token: 0x0400012B RID: 299
	private GameObject m_gui;

	// Token: 0x0400012C RID: 300
	private GameObject m_guiCamera;

	// Token: 0x0400012D RID: 301
	private MsgBox m_msgBox;

	// Token: 0x0400012E RID: 302
	private GameObject m_tokenDialog;

	// Token: 0x0400012F RID: 303
	private GameObject m_resetPasswordDialog;

	// Token: 0x04000130 RID: 304
	private string m_tempResetEmail;

	// Token: 0x04000131 RID: 305
	private GameObject m_reqVerMailDialog;

	// Token: 0x04000132 RID: 306
	private MusicManager m_musicMan;

	// Token: 0x04000133 RID: 307
	private List<string> m_languages = new List<string>();

	// Token: 0x04000134 RID: 308
	private int m_selectedLanguage;

	// Token: 0x04000135 RID: 309
	private OptionsWindow m_optionsWindow;

	// Token: 0x04000136 RID: 310
	private NewsTicker m_newsTicker;

	// Token: 0x04000137 RID: 311
	private GDPBackend m_gdpBackend;

	// Token: 0x0200002D RID: 45
	private class ServerData
	{
		// Token: 0x04000138 RID: 312
		public string m_name = string.Empty;

		// Token: 0x04000139 RID: 313
		public string m_host = string.Empty;

		// Token: 0x0400013A RID: 314
		public int m_port;

		// Token: 0x0400013B RID: 315
		public bool m_dev;
	}

	// Token: 0x0200019C RID: 412
	// (Invoke) Token: 0x06000F20 RID: 3872
	public delegate void CreateAccountHandler(string host, int port, string userName, string password, string email);

	// Token: 0x0200019D RID: 413
	// (Invoke) Token: 0x06000F24 RID: 3876
	public delegate void LoginDelegatetHandler(string visualName, string host, int port, string userName, string password, string token);

	// Token: 0x0200019E RID: 414
	// (Invoke) Token: 0x06000F28 RID: 3880
	public delegate void RequestVerificationHandler(string host, int port, string userName);

	// Token: 0x0200019F RID: 415
	// (Invoke) Token: 0x06000F2C RID: 3884
	public delegate void ResetPasswordDelegate(string host, int port, string email, string token, string password);

	// Token: 0x020001A0 RID: 416
	// (Invoke) Token: 0x06000F30 RID: 3888
	public delegate void SingleHandler();
}
