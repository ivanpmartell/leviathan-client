using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using PTech;
using UnityEngine;

// Token: 0x02000123 RID: 291
internal class OfflineState : StateBase
{
	// Token: 0x06000B50 RID: 2896 RVA: 0x00052904 File Offset: 0x00050B04
	public OfflineState(GameObject guiCamera, MusicManager musMan, GDPBackend gdpBackend) : base(guiCamera, musMan, gdpBackend)
	{
		this.m_guiCamera = guiCamera;
		this.m_mapMan = new ClientMapMan();
		this.m_packMan = new PackMan();
		this.m_gameDB = new OfflineGameDB();
		this.m_user = new User("Admiral", 1);
		this.LoadUser();
		this.m_userManClient = new UserManClientLocal(this.m_user, this.m_packMan, this.m_mapMan, this.m_gdpBackend);
		if (this.m_gdpBackend != null)
		{
			base.RegisterGDPPackets();
		}
		else
		{
			this.AddAllContentPacks(this.m_user);
		}
		this.SetupMenu(MenuBase.StartStatus.Normal);
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x000529A8 File Offset: 0x00050BA8
	protected override void SetupMenu(MenuBase.StartStatus status)
	{
		base.SetupMenu(status);
		if (this.m_offlineGame != null)
		{
			this.m_offlineGame.Close();
			this.m_offlineGame = null;
		}
		this.m_musicMan.SetMusic("menu");
		this.m_menu = new OfflineMenu(this.m_guiCamera, this.m_userManClient, this.m_gameDB, status, this.m_musicMan);
		this.m_menu.m_onBack = new OfflineMenu.BackHandler(this.OnMenuBack);
		this.m_menu.m_onStartLevel = new OfflineMenu.StartLevelHandler(this.OnMenuStartLevel);
		this.m_menu.m_onJoin = new Action<int>(this.OnMenuJoinGame);
		this.m_menu.m_onWatchReplay = new Action<int, string>(this.OnMenuWatchReplay);
		main.LoadLevel("menu", true);
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x00052A74 File Offset: 0x00050C74
	public override void Close()
	{
		base.Close();
		this.SaveUser();
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00052A84 File Offset: 0x00050C84
	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (this.m_offlineGame != null)
		{
			this.m_offlineGame.FixedUpdate();
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00052AA4 File Offset: 0x00050CA4
	public override void OnLevelWasLoaded()
	{
		base.OnLevelWasLoaded();
		if (this.m_offlineGame != null)
		{
			this.m_offlineGame.OnLevelWasLoaded();
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00052AC4 File Offset: 0x00050CC4
	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (this.m_offlineGame != null)
		{
			this.m_offlineGame.Update(deltaTime);
		}
		if (this.m_menu != null)
		{
			this.m_menu.Update(deltaTime);
		}
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00052B0C File Offset: 0x00050D0C
	public override void LateUpdate()
	{
		base.LateUpdate();
		if (this.m_offlineGame != null)
		{
			this.m_offlineGame.LateUpdate();
		}
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00052B2C File Offset: 0x00050D2C
	public void OnGui()
	{
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00052B30 File Offset: 0x00050D30
	private void OnMenuBack()
	{
		if (this.m_onBack != null)
		{
			this.m_onBack(false);
		}
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x00052B4C File Offset: 0x00050D4C
	protected override void OnQuitGame()
	{
		this.m_offlineGame.Close();
		this.m_offlineGame = null;
		if (this.m_onQuitGame != null)
		{
			this.m_onQuitGame();
		}
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00052B84 File Offset: 0x00050D84
	private void OnSaveUserRequest()
	{
		this.SaveUser();
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00052B8C File Offset: 0x00050D8C
	private void OnMenuStartLevel(GameType mode, string campaign, string levelName)
	{
		this.CloseMenu();
		int num = this.m_user.m_gamesCreated + 1;
		this.m_user.m_gamesCreated++;
		string gameName = "Game " + num.ToString();
		this.m_offlineGame = new OfflineGame(campaign, levelName, gameName, mode, this.m_user, this.m_userManClient, this.m_mapMan, this.m_packMan, this.m_guiCamera, this.m_musicMan, 1, FleetSizeClass.Heavy, 1f, this.m_gameDB);
		this.m_offlineGame.m_onExit = new Action<ExitState>(this.OnExitGame);
		this.m_offlineGame.m_onQuitGame = new Action(this.OnQuitGame);
		this.m_offlineGame.m_onSaveUserRequest = new Action(this.OnSaveUserRequest);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x00052C58 File Offset: 0x00050E58
	private void OnMenuJoinGame(int gameID)
	{
		this.CloseMenu();
		byte[] data = this.m_gameDB.LoadGame(gameID, false);
		this.m_offlineGame = new OfflineGame(string.Empty, data, false, this.m_user, this.m_userManClient, this.m_mapMan, this.m_packMan, this.m_guiCamera, this.m_musicMan, this.m_gameDB);
		this.m_offlineGame.m_onExit = new Action<ExitState>(this.OnExitGame);
		this.m_offlineGame.m_onQuitGame = new Action(this.OnQuitGame);
		this.m_offlineGame.m_onSaveUserRequest = new Action(this.OnSaveUserRequest);
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x00052CFC File Offset: 0x00050EFC
	private void OnMenuWatchReplay(int gameID, string replayName)
	{
		int majorVersion = VersionInfo.m_majorVersion;
		byte[] replayData = this.m_gameDB.LoadGame(gameID, true);
		base.ReplayData(replayName, majorVersion, replayData);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x00052D28 File Offset: 0x00050F28
	private void OnExitGame(ExitState exitState)
	{
		PLog.Log("Disconnect");
		this.m_offlineGame.Close();
		this.m_offlineGame = null;
		this.SaveUser();
		if (exitState == ExitState.Normal || exitState == ExitState.Kicked)
		{
			this.SetupMenu(MenuBase.StartStatus.ShowGameView);
			return;
		}
		if (exitState == ExitState.ShowCredits)
		{
			this.SetupMenu(MenuBase.StartStatus.ShowCredits);
			return;
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x00052D7C File Offset: 0x00050F7C
	private void AddAllContentPacks(User user)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = true;
		UnityEngine.Object[] array = Resources.LoadAll("shared_settings/packs");
		foreach (UnityEngine.Object @object in array)
		{
			TextAsset textAsset = @object as TextAsset;
			if (!(textAsset == null))
			{
				XmlReader xmlReader = XmlReader.Create(new StringReader(textAsset.text), xmlReaderSettings);
				XmlDocument xmlDocument = new XmlDocument();
				try
				{
					xmlDocument.Load(xmlReader);
				}
				catch (XmlException ex)
				{
					PLog.LogError("Parse error " + ex.ToString());
					goto IL_D0;
				}
				ContentPack contentPack = new ContentPack();
				contentPack.Load(xmlDocument);
				bool unlockAllMaps = false;
				if (contentPack.m_dev)
				{
					user.AddContentPack(contentPack, this.m_mapMan, unlockAllMaps);
				}
				else
				{
					user.AddContentPack(contentPack, this.m_mapMan, unlockAllMaps);
				}
			}
			IL_D0:;
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x00052E88 File Offset: 0x00051088
	private void LoadUser()
	{
		string path = Application.persistentDataPath + "/user.dat";
		try
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			int nextGameID = binaryReader.ReadInt32();
			Game.SetNextGameID(nextGameID);
			int nextCampaignID = binaryReader.ReadInt32();
			Game.SetNextCampaignID(nextCampaignID);
			this.m_user.OfflineLoad(binaryReader, this.m_packMan);
			fileStream.Close();
			PLog.Log("Loaded user");
		}
		catch (FileNotFoundException)
		{
			PLog.Log("No user.dat found, assuming new install");
		}
		catch (IsolatedStorageException)
		{
			PLog.Log("No user.dat found, assuming new install");
		}
		catch (IOException)
		{
			PLog.LogError("IOerror while loading user, try clearing your application data");
		}
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x00052F74 File Offset: 0x00051174
	private void SaveUser()
	{
		try
		{
			string path = Application.persistentDataPath + "/user.dat";
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(Game.GetNextGameID());
			binaryWriter.Write(Game.GetNextCampaignID());
			this.m_user.OfflineSave(binaryWriter);
			fileStream.Close();
			PLog.Log("SAVED user");
		}
		catch (IOException)
		{
			PLog.LogError("IOerror while saving user");
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x00053004 File Offset: 0x00051204
	protected override void CloseMenu()
	{
		if (this.m_menu != null)
		{
			this.m_menu.Close();
			this.m_menu = null;
		}
	}

	// Token: 0x04000952 RID: 2386
	public Action<bool> m_onBack;

	// Token: 0x04000953 RID: 2387
	public Action m_onQuitGame;

	// Token: 0x04000954 RID: 2388
	private OfflineGame m_offlineGame;

	// Token: 0x04000955 RID: 2389
	private OfflineMenu m_menu;

	// Token: 0x04000956 RID: 2390
	private User m_user;

	// Token: 0x04000957 RID: 2391
	private PackMan m_packMan;

	// Token: 0x04000958 RID: 2392
	private OfflineGameDB m_gameDB;
}
