using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class main : MonoBehaviour
{
	// Token: 0x060006D2 RID: 1746 RVA: 0x00034238 File Offset: 0x00032438
	private void Start()
	{
		Application.targetFrameRate = 60;
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		Gun.RegisterAIStates();
		Ship.RegisterAIStates();
		AudioSource[] components = base.GetComponents<AudioSource>();
		this.m_musicMan = new MusicManager(components);
		this.m_musicMan.SetVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
		AudioManager.instance.SetVolume(PlayerPrefs.GetFloat("SfxVolume", 1f));
		string @string = PlayerPrefs.GetString("Language", "english");
		Localize.instance.SetLanguage(@string);
		main.m_loadScreenGuiCamera = this.m_guiCamera;
		CheatMan cheatMan = new CheatMan();
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x000342DC File Offset: 0x000324DC
	private bool VideoModeCheck()
	{
		if (Screen.width < 1024 || Screen.height < 720)
		{
			Screen.SetResolution(1024, 720, false);
			return false;
		}
		return true;
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00034310 File Offset: 0x00032510
	private void SetupSplash()
	{
		PLog.Log("Setup splash screen");
		this.m_splash = new Splash(this.m_guiCamera, this.m_musicMan);
		this.m_splash.m_onDone = new Action(this.OnSplashDone);
		this.m_splash.m_onFadeoutComplete = new Action(this.OnSplashFadedOut);
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x0003436C File Offset: 0x0003256C
	private void OnSplashDone()
	{
		bool flag = this.SetupGDP();
		this.m_pdxNews = new PdxNews("leviathan", false);
		if (flag)
		{
			this.SetupConnectMenu(false);
		}
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x000343A0 File Offset: 0x000325A0
	private void OnSplashFadedOut()
	{
		this.m_splash.Close();
		this.m_splash = null;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x000343B4 File Offset: 0x000325B4
	private void OnApplicationPause(bool pause)
	{
		PLog.Log("OnApplicationPause: " + pause.ToString());
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x000343CC File Offset: 0x000325CC
	private bool SetupGDP()
	{
		try
		{
			bool live = true;
			this.m_gdpBackend = new PSteam(live);
		}
		catch (Exception ex)
		{
			PLog.LogWarning("PSteam exception:" + ex.ToString());
			this.m_msgbox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$steam_init_failed ", new MsgBox.OkHandler(this.OnNoSteamOk));
			return false;
		}
		if (!this.m_gdpBackend.IsBackendOnline())
		{
			this.m_msgbox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$steam_offline_mode_only", null);
		}
		return true;
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00034478 File Offset: 0x00032678
	private void OnNoSteamOk()
	{
		Application.Quit();
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00034480 File Offset: 0x00032680
	private void SetupConnectMenu(bool showLoadscreen)
	{
		if (this.m_offlineState != null)
		{
			this.m_offlineState.Close();
			this.m_offlineState = null;
		}
		if (this.m_onlineState != null)
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
		}
		if (this.m_connectMenu != null)
		{
			this.m_connectMenu.Close();
			this.m_connectMenu = null;
		}
		this.m_connectMenu = new ConnectMenu(this.m_guiCamera, this.m_musicMan, this.m_pdxNews, this.m_gdpBackend);
		this.m_connectMenu.m_onConnect = new ConnectMenu.LoginDelegatetHandler(this.OnConnect);
		this.m_connectMenu.m_onCreateAccount = new ConnectMenu.CreateAccountHandler(this.OnCreateAccount);
		this.m_connectMenu.m_onRequestResetPasswordCode = new Action<string, int, string>(this.OnRequestResetPasswordCode);
		this.m_connectMenu.m_onRequestVerificationMail = new ConnectMenu.RequestVerificationHandler(this.OnRequestVerificationEmail);
		this.m_connectMenu.m_onResetPassword = new ConnectMenu.ResetPasswordDelegate(this.OnPasswordReset);
		this.m_connectMenu.m_onSinglePlayer = new ConnectMenu.SingleHandler(this.OnSinglePlayer);
		this.m_connectMenu.m_onExit = new Action(this.OnQuit);
		main.LoadLevel("menu", showLoadscreen);
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000345B4 File Offset: 0x000327B4
	private void FixedUpdate()
	{
		if (this.m_gdpBackend != null)
		{
			this.m_gdpBackend.Update();
		}
		if (this.m_onlineState != null)
		{
			this.m_onlineState.FixedUpdate();
		}
		if (this.m_offlineState != null)
		{
			this.m_offlineState.FixedUpdate();
		}
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00034604 File Offset: 0x00032804
	private void Update()
	{
		Utils.UpdateAndroidBack();
		if (this.m_showSplash >= 0)
		{
			if (this.m_showSplash == 1)
			{
				this.VideoModeCheck();
			}
			if (this.m_showSplash == 0)
			{
				this.SetupSplash();
			}
			this.m_showSplash--;
		}
		if (main.m_loadScreen != null)
		{
			main.m_loadScreen.Update();
		}
		if (this.m_splash != null)
		{
			this.m_splash.Update();
		}
		if (this.m_connectMenu != null)
		{
			this.m_connectMenu.Update();
		}
		if (this.m_onlineState != null)
		{
			this.m_onlineState.Update();
		}
		if (this.m_offlineState != null)
		{
			this.m_offlineState.Update();
		}
		if (this.m_msgbox != null)
		{
			this.m_msgbox.Update();
		}
		AudioManager.instance.Update(Time.deltaTime);
		this.m_musicMan.Update(Time.deltaTime);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x000346F8 File Offset: 0x000328F8
	private void LateUpdate()
	{
		if (this.m_onlineState != null)
		{
			this.m_onlineState.LateUpdate();
		}
		if (this.m_offlineState != null)
		{
			this.m_offlineState.LateUpdate();
		}
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00034734 File Offset: 0x00032934
	private void OnLevelWasLoaded()
	{
		if (Application.loadedLevelName == "empty")
		{
			PLog.Log(" purging unused resources ");
			Resources.UnloadUnusedAssets();
			GC.Collect();
			PLog.Log(" loading actual scene");
			Application.LoadLevel(main.m_nextLevel);
			main.m_nextLevel = string.Empty;
			return;
		}
		PLog.Log(" done loading " + Application.loadedLevelName);
		if (this.m_onlineState != null)
		{
			this.m_onlineState.OnLevelWasLoaded();
		}
		if (this.m_offlineState != null)
		{
			this.m_offlineState.OnLevelWasLoaded();
		}
		if (main.m_loadScreen != null)
		{
			main.m_loadScreen.SetVisible(false);
		}
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000347E0 File Offset: 0x000329E0
	public static void LoadLevel(string name, bool showLoadScreen)
	{
		if (Application.loadedLevelName == name)
		{
			return;
		}
		PLog.Log("Level change started: " + name);
		if (main.m_loadScreenGuiCamera != null && showLoadScreen)
		{
			main.m_loadScreen = new LoadScreen(main.m_loadScreenGuiCamera);
			main.m_loadScreen.SetImage(string.Empty);
			main.m_loadScreen.SetVisible(true);
		}
		main.m_nextLevel = name;
		Application.LoadLevel("empty");
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00034860 File Offset: 0x00032A60
	private void OnGUI()
	{
		if (this.m_onlineState != null)
		{
			this.m_onlineState.OnGui();
		}
		if (this.m_offlineState != null)
		{
			this.m_offlineState.OnGui();
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0003489C File Offset: 0x00032A9C
	private void OnConnect(string hostVisualName, string host, int port, string userName, string password, string token)
	{
		if (this.m_onlineState != null)
		{
			PLog.Log("Already trying to connect");
			return;
		}
		this.m_onlineState = new OnlineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		this.m_onlineState.m_onLoginFailed = new OnlineState.LoginFailHandler(this.OnLoginFail);
		this.m_onlineState.m_onLoginSuccess = new OnlineState.LoginSuccessHandler(this.OnLoginSuccess);
		this.m_onlineState.m_onDisconnect = new OnlineState.DisconnectHandler(this.OnDisconnected);
		this.m_onlineState.m_onQuitGame = new Action(this.OnQuit);
		if (!this.m_onlineState.Login(hostVisualName, host, port, userName, password, token))
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
			this.m_connectMenu.OnConnectFailed();
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00034974 File Offset: 0x00032B74
	private void OnCreateAccount(string host, int port, string userName, string password, string email)
	{
		if (this.m_onlineState != null)
		{
			PLog.Log("Already trying to connect");
			return;
		}
		this.m_onlineState = new OnlineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		this.m_onlineState.m_onCreateFailed = new Action<ErrorCode>(this.OnCreateFail);
		this.m_onlineState.m_onCreateSuccess = new Action(this.OnCreateSuccess);
		this.m_onlineState.m_onDisconnect = new OnlineState.DisconnectHandler(this.OnDisconnected);
		if (!this.m_onlineState.CreateAccount(host, port, userName, password, email))
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
			this.m_connectMenu.OnConnectFailed();
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00034A34 File Offset: 0x00032C34
	private void OnRequestVerificationEmail(string host, int port, string email)
	{
		if (this.m_onlineState != null)
		{
			PLog.Log("Already trying to connect");
			return;
		}
		this.m_onlineState = new OnlineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		this.m_onlineState.m_onRequestVerificationRespons = new Action<ErrorCode>(this.OnRequestVerificationRespons);
		this.m_onlineState.m_onDisconnect = new OnlineState.DisconnectHandler(this.OnDisconnected);
		if (!this.m_onlineState.RequestVerificationEmail(host, port, email))
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
			this.m_connectMenu.OnConnectFailed();
		}
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00034AD8 File Offset: 0x00032CD8
	private void OnRequestVerificationRespons(ErrorCode errorCode)
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnRequestVerificaionRespons(errorCode);
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00034AF8 File Offset: 0x00032CF8
	private void OnRequestResetPasswordCode(string host, int port, string email)
	{
		if (this.m_onlineState != null)
		{
			PLog.Log("Already trying to connect");
			return;
		}
		this.m_onlineState = new OnlineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		if (!this.m_onlineState.RequestPasswordReset(host, port, email))
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
			this.m_connectMenu.OnConnectFailed();
		}
		else
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00034B88 File Offset: 0x00032D88
	private void OnPasswordReset(string host, int port, string email, string token, string password)
	{
		if (this.m_onlineState != null)
		{
			PLog.Log("Already trying to connect");
			return;
		}
		this.m_onlineState = new OnlineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		this.m_onlineState.m_onResetPasswordOk = new Action(this.OnResetPasswordOk);
		this.m_onlineState.m_onResetPasswordFail = new Action(this.OnResetPasswordFail);
		this.m_onlineState.m_onDisconnect = new OnlineState.DisconnectHandler(this.OnDisconnected);
		if (!this.m_onlineState.ResetPassword(host, port, email, token, password))
		{
			this.m_onlineState.Close();
			this.m_onlineState = null;
			this.m_connectMenu.OnConnectFailed();
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00034C48 File Offset: 0x00032E48
	private void OnResetPasswordFail()
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnResetPasswordFail();
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00034C68 File Offset: 0x00032E68
	private void OnResetPasswordOk()
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnResetPasswordConfirmed();
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00034C88 File Offset: 0x00032E88
	private void OnLoginFail(ErrorCode errorCode)
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnLoginFailed(errorCode);
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00034CA8 File Offset: 0x00032EA8
	private void OnCreateFail(ErrorCode errorCode)
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnCreateFailed(errorCode);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x00034CC8 File Offset: 0x00032EC8
	private void OnCreateSuccess()
	{
		this.m_onlineState.Close();
		this.m_onlineState = null;
		this.m_connectMenu.OnCreateSuccess();
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00034CE8 File Offset: 0x00032EE8
	private void OnLoginSuccess()
	{
		this.m_connectMenu.Close();
		this.m_connectMenu = null;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00034CFC File Offset: 0x00032EFC
	private void OnDisconnected(bool error)
	{
		PLog.Log("Disconnected");
		if (error)
		{
			this.m_msgbox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$login_connectfail", null);
		}
		this.SetupConnectMenu(true);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00034D38 File Offset: 0x00032F38
	private void OnQuit()
	{
		PLog.Log("On quit");
		if (this.m_gdpBackend != null)
		{
			this.m_gdpBackend.Close();
			this.m_gdpBackend = null;
		}
		PlayerPrefs.SetString("Language", Localize.instance.GetLanguage());
		PLog.Log("Application QUIT");
		Application.Quit();
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00034D90 File Offset: 0x00032F90
	private void OnSinglePlayer()
	{
		this.m_connectMenu.Close();
		this.m_connectMenu = null;
		this.m_offlineState = new OfflineState(this.m_guiCamera, this.m_musicMan, this.m_gdpBackend);
		this.m_offlineState.m_onBack = new Action<bool>(this.OnDisconnected);
		this.m_offlineState.m_onQuitGame = new Action(this.OnQuit);
	}

	// Token: 0x040005C3 RID: 1475
	private static string m_nextLevel = string.Empty;

	// Token: 0x040005C4 RID: 1476
	public GameObject m_guiCamera;

	// Token: 0x040005C5 RID: 1477
	private static LoadScreen m_loadScreen;

	// Token: 0x040005C6 RID: 1478
	private static GameObject m_loadScreenGuiCamera;

	// Token: 0x040005C7 RID: 1479
	private ConnectMenu m_connectMenu;

	// Token: 0x040005C8 RID: 1480
	private OnlineState m_onlineState;

	// Token: 0x040005C9 RID: 1481
	private Splash m_splash;

	// Token: 0x040005CA RID: 1482
	private OfflineState m_offlineState;

	// Token: 0x040005CB RID: 1483
	private MusicManager m_musicMan;

	// Token: 0x040005CC RID: 1484
	private GDPBackend m_gdpBackend;

	// Token: 0x040005CD RID: 1485
	private MsgBox m_msgbox;

	// Token: 0x040005CE RID: 1486
	private PdxNews m_pdxNews;

	// Token: 0x040005CF RID: 1487
	private int m_showSplash = 5;
}
