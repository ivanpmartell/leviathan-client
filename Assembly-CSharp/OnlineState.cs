using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using PTech;
using UnityEngine;

// Token: 0x02000124 RID: 292
internal class OnlineState : StateBase
{
	// Token: 0x06000B63 RID: 2915 RVA: 0x00053024 File Offset: 0x00051224
	public OnlineState(GameObject guiCamera, MusicManager musMan, GDPBackend gdpBackend, PdxNews pdxNews) : base(guiCamera, musMan, gdpBackend)
	{
		this.m_guiCamera = guiCamera;
		this.m_musicMan = musMan;
		this.m_pdxNews = pdxNews;
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0005305C File Offset: 0x0005125C
	public override void Close()
	{
		base.Close();
		this.CloseMenu();
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Close();
			this.m_fleetEditor = null;
		}
		if (this.m_toastMaster != null)
		{
			this.m_toastMaster.Close();
			this.m_toastMaster = null;
		}
		if (this.m_rpc != null)
		{
			this.m_rpc.Close();
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x000530E4 File Offset: 0x000512E4
	public bool Login(string hostVisalName, string host, int port, string user, string passwd, string token)
	{
		if (!this.Connect(host, port))
		{
			return false;
		}
		this.m_hostName = hostVisalName;
		this.m_localUserName = user;
		PlatformType platform = Utils.GetPlatform();
		string text = PwdUtils.GenerateWeakPasswordHash(passwd);
		this.m_rpc.Invoke("Login", new object[]
		{
			user,
			text,
			token,
			VersionInfo.GetMajorVersionString(),
			(int)platform,
			Localize.instance.GetLanguage()
		});
		this.m_rpc.Register("LoginOK", new PTech.RPC.Handler(this.RPC_LoginOK));
		this.m_rpc.Register("LoginFail", new PTech.RPC.Handler(this.RPC_LoginFail));
		this.m_rpc.Register("JoinOK", new PTech.RPC.Handler(this.RPC_JoinOK));
		this.m_rpc.Register("JoinFail", new PTech.RPC.Handler(this.RPC_JoinFail));
		return true;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x000531D0 File Offset: 0x000513D0
	public bool CreateAccount(string host, int port, string user, string passwd, string email)
	{
		if (!this.Connect(host, port))
		{
			return false;
		}
		string text = PwdUtils.GenerateWeakPasswordHash(passwd);
		this.m_rpc.Invoke("CreateAccount", new object[]
		{
			user,
			text,
			email,
			VersionInfo.GetMajorVersionString(),
			Localize.instance.GetLanguage()
		});
		this.m_rpc.Register("CreateSuccess", new PTech.RPC.Handler(this.RPC_CreateSuccess));
		this.m_rpc.Register("CreateFail", new PTech.RPC.Handler(this.RPC_CreateFail));
		return true;
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00053268 File Offset: 0x00051468
	public bool RequestVerificationEmail(string host, int port, string user)
	{
		if (!this.Connect(host, port))
		{
			return false;
		}
		this.m_rpc.Invoke("RequestVerificationEmail", new object[]
		{
			user
		});
		this.m_rpc.Register("RequestVerificationRespons", new PTech.RPC.Handler(this.RPC_RequestVerificationRespons));
		return true;
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x000532BC File Offset: 0x000514BC
	public bool RequestPasswordReset(string host, int port, string email)
	{
		if (!this.Connect(host, port))
		{
			return false;
		}
		this.m_rpc.Invoke("RequestPasswordReset", new object[]
		{
			email
		});
		return true;
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x000532F4 File Offset: 0x000514F4
	public bool ResetPassword(string host, int port, string email, string token, string newPassword)
	{
		if (!this.Connect(host, port))
		{
			return false;
		}
		string text = PwdUtils.GenerateWeakPasswordHash(newPassword);
		this.m_rpc.Invoke("ResetPassword", new object[]
		{
			email,
			token,
			text
		});
		this.m_rpc.Register("ResetPasswordOK", new PTech.RPC.Handler(this.RPC_ResetPasswordOk));
		this.m_rpc.Register("ResetPasswordFail", new PTech.RPC.Handler(this.RPC_ResetPasswordFail));
		return true;
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x00053374 File Offset: 0x00051574
	private bool Connect(string host, int port)
	{
		this.m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		this.m_socket.NoDelay = true;
		PLog.LogWarning(string.Concat(new object[]
		{
			"(not really a warning) Connecting to ",
			host,
			" : ",
			port
		}));
		IAsyncResult asyncResult;
		try
		{
			asyncResult = this.m_socket.BeginConnect(host, port, null, null);
		}
		catch (SocketException ex)
		{
			this.m_socket.Close();
			this.m_socket = null;
			PLog.LogWarning("Failed to connect (dns lookup) " + ex.ToString());
			return false;
		}
		bool flag = asyncResult.AsyncWaitHandle.WaitOne(10000, true);
		if (!this.m_socket.Connected)
		{
			this.m_socket.Close();
			this.m_socket = null;
			PLog.LogWarning("Socket connection timed out");
			return false;
		}
		PacketSocket socket = new PacketSocket(this.m_socket);
		this.m_rpc = new PTech.RPC(socket);
		PLog.Log("Connected ");
		return true;
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00053494 File Offset: 0x00051694
	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (this.m_game != null)
		{
			this.m_game.FixedUpdate();
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x000534B4 File Offset: 0x000516B4
	public override void Update()
	{
		if (this.m_rpc != null && !this.m_rpc.Update(false))
		{
			this.m_rpc.Close();
			if (this.m_onDisconnect != null)
			{
				this.m_onDisconnect(true);
			}
			return;
		}
		base.Update();
		if (this.m_game != null)
		{
			this.m_game.Update();
		}
		if (this.m_menu != null)
		{
			this.m_menu.Update();
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Update();
		}
		if (this.m_toastMaster != null)
		{
			this.m_toastMaster.Update(Time.deltaTime);
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00053564 File Offset: 0x00051764
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.m_game != null)
		{
			this.m_game.LateUpdate();
		}
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00053584 File Offset: 0x00051784
	public override void OnLevelWasLoaded()
	{
		base.OnLevelWasLoaded();
		if (this.m_game != null)
		{
			this.m_game.OnLevelWasLoaded();
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.OnLevelWasLoaded();
		}
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x000535C4 File Offset: 0x000517C4
	public void OnGui()
	{
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x000535C8 File Offset: 0x000517C8
	private void RPC_LoginOK(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("Login ok");
		if (this.m_onLoginSuccess != null)
		{
			this.m_onLoginSuccess();
		}
		rpc.Register("ReplayData", new PTech.RPC.Handler(this.RPC_ReplayData));
		rpc.Register("ReplayMissing", new PTech.RPC.Handler(this.RPC_ReplayMissing));
		this.m_userManClient = new UserManClientRemote(rpc, this.m_gdpBackend);
		this.m_mapMan = new ClientMapMan();
		this.m_toastMaster = new ToastMaster(this.m_rpc, this.m_guiCamera, this.m_userManClient);
		if (this.m_gdpBackend != null)
		{
			base.RegisterGDPPackets();
		}
		this.SetupMenu(MenuBase.StartStatus.Normal);
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x00053678 File Offset: 0x00051878
	protected override void SetupMenu(MenuBase.StartStatus startStatus)
	{
		base.SetupMenu(startStatus);
		this.m_musicMan.SetMusic("menu");
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		if (this.m_fleetEditor != null)
		{
			this.m_fleetEditor.Close();
			this.m_fleetEditor = null;
		}
		this.m_menu = new OnlineMenu(this.m_guiCamera, this.m_rpc, this.m_hostName, this.m_localUserName, this.m_mapMan, this.m_userManClient, startStatus, this.m_musicMan, this.m_gdpBackend, this.m_pdxNews);
		this.m_menu.m_onJoin = new Action<int>(this.OnJoinGame);
		this.m_menu.m_onWatchReplay = new Action<int, string>(this.OnMenuWatchReplay);
		this.m_menu.m_onLogout = new Action(this.OnLogout);
		this.m_menu.m_onStartFleetEditor = new Action(this.SetupChooseFleet);
		this.m_menu.m_onProceed = new Action<string, int>(this.OnChooseFleetDialogProceed);
		this.m_menu.m_onItemBought = new Action(this.OnItemBought);
		main.LoadLevel("menu", true);
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x000537B0 File Offset: 0x000519B0
	private void RPC_LoginFail(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("Failed to login");
		ErrorCode errorCode = (ErrorCode)((int)args[0]);
		if (this.m_onLoginFailed != null)
		{
			this.m_onLoginFailed(errorCode);
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x000537EC File Offset: 0x000519EC
	private void RPC_CreateSuccess(PTech.RPC rpc, List<object> args)
	{
		if (this.m_onCreateSuccess != null)
		{
			this.m_onCreateSuccess();
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00053804 File Offset: 0x00051A04
	private void RPC_CreateFail(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("Failed to create");
		ErrorCode obj = (ErrorCode)((int)args[0]);
		if (this.m_onCreateFailed != null)
		{
			this.m_onCreateFailed(obj);
		}
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x00053840 File Offset: 0x00051A40
	private void RPC_RequestVerificationRespons(PTech.RPC rpc, List<object> args)
	{
		ErrorCode obj = (ErrorCode)((int)args[0]);
		if (this.m_onRequestVerificationRespons != null)
		{
			this.m_onRequestVerificationRespons(obj);
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00053874 File Offset: 0x00051A74
	private void RPC_ResetPasswordOk(PTech.RPC rpc, List<object> args)
	{
		this.m_onResetPasswordOk();
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x00053884 File Offset: 0x00051A84
	private void RPC_ResetPasswordFail(PTech.RPC rpc, List<object> args)
	{
		this.m_onResetPasswordFail();
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x00053894 File Offset: 0x00051A94
	private void RPC_ReplayData(PTech.RPC rpc, List<object> args)
	{
		int majorVersion = (int)args[0];
		byte[] replayData = (byte[])args[1];
		base.ReplayData(this.m_watchReplayName, majorVersion, replayData);
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x000538CC File Offset: 0x00051ACC
	private void RPC_ReplayMissing(PTech.RPC rpc, List<object> args)
	{
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$replay_missing", new MsgBox.OkHandler(this.OnMsgBoxOk));
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x000538FC File Offset: 0x00051AFC
	protected override void CloseMenu()
	{
		if (this.m_menu != null)
		{
			this.m_menu.Close();
			this.m_menu = null;
		}
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0005391C File Offset: 0x00051B1C
	private void OnJoinGame(int gameID)
	{
		if (this.m_game == null && !this.m_waitingForJoinResponse)
		{
			this.m_rpc.Invoke("Join", new object[]
			{
				gameID
			});
			this.m_waitingForJoinResponse = true;
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00053968 File Offset: 0x00051B68
	private void OnMenuWatchReplay(int gameID, string replayName)
	{
		this.WatchReplay(gameID, replayName);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00053974 File Offset: 0x00051B74
	private void WatchReplay(int gameID, string replayName)
	{
		this.m_watchReplayName = replayName;
		this.m_rpc.Invoke("WatchReplay", new object[]
		{
			gameID
		});
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x000539A8 File Offset: 0x00051BA8
	private void RPC_JoinOK(PTech.RPC rpc, List<object> args)
	{
		if (this.m_game != null)
		{
			return;
		}
		this.CloseMenu();
		this.m_game = new ClientGame(this.m_rpc, this.m_guiCamera, this.m_userManClient, this.m_mapMan, this.m_musicMan, false, true, this.m_hostName);
		this.m_game.m_onExit = new Action<ExitState, int>(this.OnExitGame);
		this.m_game.m_onQuitGame = new Action(this.OnQuitGame);
		this.m_waitingForJoinResponse = false;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00053A30 File Offset: 0x00051C30
	private void RPC_JoinFail(PTech.RPC rpc, List<object> args)
	{
		this.m_waitingForJoinResponse = false;
		this.m_msgBox = MsgBox.CreateOkMsgBox(this.m_guiCamera, "$JoinGameFailed", new MsgBox.OkHandler(this.OnMsgBoxOk));
		if (this.m_menu == null)
		{
			this.SetupMenu(MenuBase.StartStatus.ShowGameView);
		}
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00053A70 File Offset: 0x00051C70
	private void OnMsgBoxOk()
	{
		this.m_msgBox.Close();
		this.m_msgBox = null;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00053A84 File Offset: 0x00051C84
	private void OnExitGame(ExitState exitState, int joinGameID)
	{
		if (exitState == ExitState.Normal || exitState == ExitState.Kicked)
		{
			this.SetupMenu(MenuBase.StartStatus.ShowGameView);
			if (exitState == ExitState.Kicked)
			{
				this.m_menu.OnKicked();
			}
			return;
		}
		if (exitState == ExitState.ShowCredits)
		{
			this.SetupMenu(MenuBase.StartStatus.ShowCredits);
			return;
		}
		DebugUtils.Assert(joinGameID > 0);
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		main.LoadLevel("menu", true);
		if (exitState == ExitState.JoinGame)
		{
			this.m_rpc.Invoke("Join", new object[]
			{
				joinGameID
			});
			this.m_waitingForJoinResponse = true;
		}
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00053B28 File Offset: 0x00051D28
	protected override void OnQuitGame()
	{
		PLog.LogWarning("onlinestate recives delegate that we should quit the game. Stacktrace:\n" + StackTraceUtility.ExtractStackTrace());
		this.Close();
		this.m_onQuitGame();
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x00053B50 File Offset: 0x00051D50
	private void OnLogout()
	{
		this.m_rpc.Close();
		this.m_onDisconnect(false);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00053B6C File Offset: 0x00051D6C
	private void OnItemBought()
	{
		PLog.Log("Updating owned items");
		base.RegisterGDPPackets();
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00053B80 File Offset: 0x00051D80
	private void SetupChooseFleet()
	{
		this.CloseMenu();
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		Thread.Sleep(0);
		this.m_chooseFleetDialog = new ChooseFleetMenu(this.m_guiCamera, this.m_userManClient, FleetSizeClass.None, null, "online", 0);
		this.m_chooseFleetDialog.m_onExit = new ChooseFleetMenu.OnExitDelegate(this.OnChooseFleetDialogExit);
		this.m_chooseFleetDialog.m_onProceed = new Action<string, int>(this.OnChooseFleetDialogProceed);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00053C04 File Offset: 0x00051E04
	private void OnChooseFleetDialogExit()
	{
		if (this.m_chooseFleetDialog != null)
		{
			ChooseFleetMenu chooseFleetDialog = this.m_chooseFleetDialog;
			chooseFleetDialog.m_onExit = (ChooseFleetMenu.OnExitDelegate)Delegate.Remove(chooseFleetDialog.m_onExit, new ChooseFleetMenu.OnExitDelegate(this.OnChooseFleetDialogExit));
			this.m_chooseFleetDialog.Close();
			this.m_chooseFleetDialog = null;
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00053C58 File Offset: 0x00051E58
	private void OnChooseFleetDialogProceed(string selectedFleetName, int campaignID)
	{
		this.CloseMenu();
		if (this.m_game != null)
		{
			this.m_game.Close();
			this.m_game = null;
		}
		this.m_fleetEditor = new FleetMenu(this.m_guiCamera, this.m_userManClient, selectedFleetName, campaignID, new FleetSize(0, 8000), false, this.m_musicMan);
		this.m_fleetEditor.m_onExit = new FleetMenu.OnExitDelegate(this.OnFleetEditorExit);
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00053CCC File Offset: 0x00051ECC
	private void OnFleetEditorExit()
	{
		if (this.m_fleetEditor != null)
		{
			FleetMenu fleetEditor = this.m_fleetEditor;
			fleetEditor.m_onExit = (FleetMenu.OnExitDelegate)Delegate.Remove(fleetEditor.m_onExit, new FleetMenu.OnExitDelegate(this.OnFleetEditorExit));
			this.m_fleetEditor.Close();
			this.m_fleetEditor = null;
		}
		this.SetupMenu(MenuBase.StartStatus.ShowShipyard);
	}

	// Token: 0x04000959 RID: 2393
	private ChooseFleetMenu m_chooseFleetDialog;

	// Token: 0x0400095A RID: 2394
	private FleetMenu m_fleetEditor;

	// Token: 0x0400095B RID: 2395
	public OnlineState.LoginFailHandler m_onLoginFailed;

	// Token: 0x0400095C RID: 2396
	public OnlineState.LoginSuccessHandler m_onLoginSuccess;

	// Token: 0x0400095D RID: 2397
	public OnlineState.DisconnectHandler m_onDisconnect;

	// Token: 0x0400095E RID: 2398
	public Action<ErrorCode> m_onCreateFailed;

	// Token: 0x0400095F RID: 2399
	public Action m_onCreateSuccess;

	// Token: 0x04000960 RID: 2400
	public Action m_onResetPasswordOk;

	// Token: 0x04000961 RID: 2401
	public Action m_onResetPasswordFail;

	// Token: 0x04000962 RID: 2402
	public Action m_onQuitGame;

	// Token: 0x04000963 RID: 2403
	public Action<ErrorCode> m_onRequestVerificationRespons;

	// Token: 0x04000964 RID: 2404
	private Socket m_socket;

	// Token: 0x04000965 RID: 2405
	private PTech.RPC m_rpc;

	// Token: 0x04000966 RID: 2406
	private string m_localUserName;

	// Token: 0x04000967 RID: 2407
	private string m_hostName;

	// Token: 0x04000968 RID: 2408
	private ClientGame m_game;

	// Token: 0x04000969 RID: 2409
	private OnlineMenu m_menu;

	// Token: 0x0400096A RID: 2410
	private ToastMaster m_toastMaster;

	// Token: 0x0400096B RID: 2411
	private PdxNews m_pdxNews;

	// Token: 0x0400096C RID: 2412
	private bool m_waitingForJoinResponse;

	// Token: 0x0400096D RID: 2413
	private string m_watchReplayName = string.Empty;

	// Token: 0x020001AF RID: 431
	// (Invoke) Token: 0x06000F6C RID: 3948
	public delegate void LoginFailHandler(ErrorCode errorCode);

	// Token: 0x020001B0 RID: 432
	// (Invoke) Token: 0x06000F70 RID: 3952
	public delegate void LoginSuccessHandler();

	// Token: 0x020001B1 RID: 433
	// (Invoke) Token: 0x06000F74 RID: 3956
	public delegate void DisconnectHandler(bool error);
}
