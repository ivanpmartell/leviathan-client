using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000167 RID: 359
internal abstract class StateBase
{
	// Token: 0x06000D67 RID: 3431 RVA: 0x00060488 File Offset: 0x0005E688
	public StateBase(GameObject guiCamera, MusicManager musMan, GDPBackend gdpBackend)
	{
		this.m_guiCamera = guiCamera;
		this.m_musicMan = musMan;
		this.m_gdpBackend = gdpBackend;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x000604BC File Offset: 0x0005E6BC
	public virtual void Close()
	{
		this.CloseMenu();
		if (this.m_msgBox != null)
		{
			this.m_msgBox.Close();
			this.m_msgBox = null;
		}
		if (this.m_replayGame != null)
		{
			this.m_replayGame.Close();
			this.m_replayGame = null;
		}
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0006050C File Offset: 0x0005E70C
	public virtual void FixedUpdate()
	{
		if (this.m_replayGame != null)
		{
			this.m_replayGame.FixedUpdate();
		}
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00060524 File Offset: 0x0005E724
	public virtual void Update()
	{
		if (this.m_replayGame != null)
		{
			this.m_replayGame.Update(Time.deltaTime);
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00060544 File Offset: 0x0005E744
	public virtual void LateUpdate()
	{
		if (this.m_replayGame != null)
		{
			this.m_replayGame.LateUpdate();
		}
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x0006055C File Offset: 0x0005E75C
	public virtual void OnLevelWasLoaded()
	{
		if (this.m_replayGame != null)
		{
			this.m_replayGame.OnLevelWasLoaded();
		}
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x00060574 File Offset: 0x0005E774
	protected void ReplayData(string replayName, int majorVersion, byte[] replayData)
	{
		this.m_watchReplayName = replayName;
		this.m_tempReplayData = replayData;
		if (VersionInfo.m_majorVersion != majorVersion)
		{
			this.m_msgBox = MsgBox.CreateYesNoMsgBox(this.m_guiCamera, "$replay_version_missmatch", new MsgBox.YesHandler(this.OnReplayVersionVarningYes), new MsgBox.NoHandler(this.OnReplayVersionVarningNo));
		}
		else
		{
			this.StartReplay();
		}
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x000605D4 File Offset: 0x0005E7D4
	private void OnReplayVersionVarningYes()
	{
		this.m_msgBox.Close();
		this.StartReplay();
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x000605E8 File Offset: 0x0005E7E8
	private void OnReplayVersionVarningNo()
	{
		this.m_tempReplayData = null;
		this.m_msgBox.Close();
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x000605FC File Offset: 0x0005E7FC
	private void StartReplay()
	{
		this.CloseMenu();
		PackMan packMan = new PackMan();
		User user = new User("player", 1);
		UserManClient userManClient = new UserManClientLocal(user, packMan, this.m_mapMan, this.m_gdpBackend);
		this.m_replayGame = new OfflineGame(this.m_watchReplayName, this.m_tempReplayData, true, user, userManClient, this.m_mapMan, packMan, this.m_guiCamera, this.m_musicMan, null);
		this.m_replayGame.m_onExit = new Action<ExitState>(this.OnExitReplay);
		this.m_replayGame.m_onQuitGame = new Action(this.OnQuitGame);
		this.m_tempReplayData = null;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0006069C File Offset: 0x0005E89C
	protected virtual void OnQuitGame()
	{
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x000606A0 File Offset: 0x0005E8A0
	private void OnExitReplay(ExitState exitState)
	{
		this.SetupMenu(MenuBase.StartStatus.ShowArchiveView);
	}

	// Token: 0x06000D73 RID: 3443
	protected abstract void CloseMenu();

	// Token: 0x06000D74 RID: 3444 RVA: 0x000606AC File Offset: 0x0005E8AC
	protected virtual void SetupMenu(MenuBase.StartStatus status)
	{
		if (this.m_replayGame != null)
		{
			this.m_replayGame.Close();
			this.m_replayGame = null;
		}
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x000606CC File Offset: 0x0005E8CC
	protected void RegisterGDPPackets()
	{
		List<GDPBackend.GDPOwnedItem> list = this.m_gdpBackend.RequestOwned();
		List<string> list2 = new List<string>();
		foreach (GDPBackend.GDPOwnedItem gdpownedItem in list)
		{
			list2.Add(gdpownedItem.m_packName);
		}
		this.m_userManClient.SetOwnedPackages(list2);
	}

	// Token: 0x04000B05 RID: 2821
	protected GameObject m_guiCamera;

	// Token: 0x04000B06 RID: 2822
	protected MsgBox m_msgBox;

	// Token: 0x04000B07 RID: 2823
	protected UserManClient m_userManClient;

	// Token: 0x04000B08 RID: 2824
	protected MapMan m_mapMan;

	// Token: 0x04000B09 RID: 2825
	protected MusicManager m_musicMan;

	// Token: 0x04000B0A RID: 2826
	protected GDPBackend m_gdpBackend;

	// Token: 0x04000B0B RID: 2827
	private OfflineGame m_replayGame;

	// Token: 0x04000B0C RID: 2828
	private string m_watchReplayName = string.Empty;

	// Token: 0x04000B0D RID: 2829
	private byte[] m_tempReplayData;
}
