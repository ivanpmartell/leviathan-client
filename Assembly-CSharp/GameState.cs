using System;
using System.Collections.Generic;
using System.IO;
using PTech;
using UnityEngine;

// Token: 0x020000BB RID: 187
internal class GameState
{
	// Token: 0x06000699 RID: 1689 RVA: 0x000329C8 File Offset: 0x00030BC8
	public GameState(GameObject guiCamera, GameSettings gameSettings, TurnMan turnMan, TurnPhase phase, bool fastSimulation, byte[] state, int localPlayerID, int frames, GameState.SetupCompleteHandler setupStatecomplete, GameState.SimulationCompleteHandler simulationComplete)
	{
		this.m_gameSettings = gameSettings;
		this.m_turnMan = turnMan;
		this.m_phase = phase;
		this.m_fastSimulation = fastSimulation;
		this.m_guiCamera = guiCamera;
		this.m_localPlayerID = localPlayerID;
		this.m_state = state;
		this.m_frames = frames;
		this.m_onSetupComplete = setupStatecomplete;
		this.m_onSimulationComplete = simulationComplete;
		NetObj.SetLocalPlayer(localPlayerID);
		NetObj.SetPhase(this.m_phase);
		TurnMan.instance.ResetTurnStats();
		if (this.m_state == null)
		{
			NetObj.ResetObjectDB();
			NetObj.SetNextNetID(1);
		}
		if (Application.loadedLevelName == gameSettings.m_mapInfo.m_scene)
		{
			this.m_levelWasLoaded = true;
		}
		else
		{
			PLog.Log("GameState: queueing load scene " + gameSettings.m_mapInfo.m_scene);
			main.LoadLevel(gameSettings.m_mapInfo.m_scene, false);
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00032ABC File Offset: 0x00030CBC
	public void FixedUpdate()
	{
		if (this.m_levelWasLoaded && !this.m_setupComplete)
		{
			this.DoSetup();
			this.m_setupComplete = true;
		}
		if (this.IsSimulating())
		{
			this.m_currentFrame++;
			this.DoIntervalLineOfSightUpdate();
			this.m_levelScript.SimulationUpdate(Time.fixedDeltaTime);
			if (this.m_levelScript.GetGameModeScript().GetOutcome() != GameOutcome.None && !this.m_gameHasEnded)
			{
				this.m_gameHasEnded = true;
				if (this.m_gameSettings.m_gameType == GameType.Campaign)
				{
					this.m_frames = this.m_currentFrame;
				}
				else
				{
					this.m_frames = this.m_currentFrame + 150;
				}
				PLog.Log("game has ended");
			}
			if (this.m_currentFrame >= this.m_frames)
			{
				this.UpdateLineOfSightAll();
				this.SetSimulating(false);
				this.m_levelScript.GetGameModeScript().OnSimulationComplete();
				if (this.m_onSimulationComplete != null)
				{
					this.m_onSimulationComplete();
				}
			}
		}
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00032BC4 File Offset: 0x00030DC4
	public void OnLevelWasLoaded()
	{
		this.m_levelWasLoaded = true;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00032BD0 File Offset: 0x00030DD0
	private void DoSetup()
	{
		this.m_levelScript = GameObject.Find("LevelObject").GetComponent<LevelScript>();
		DebugUtils.Assert(this.m_levelScript != null);
		this.m_turnMan.SetTurnMusic(this.m_levelScript.m_music);
		this.m_levelScript.SetupGameMode(this.m_gameSettings);
		if (this.m_state != null)
		{
			this.SetState(this.m_state);
			this.m_levelScript.GetGameModeScript().OnStateLoaded();
		}
		else
		{
			PRand.SetSeed(DateTime.Now.Second * 10);
		}
		this.m_gameCamera = this.m_levelScript.transform.FindChild("GameCamera").GetComponent<GameCamera>();
		this.m_gameCamera.Setup(this.m_localPlayerID, (float)this.m_levelScript.GetMapSize(), this.m_guiCamera);
		this.SetSimulating(false);
		if (this.m_onSetupComplete != null)
		{
			this.m_onSetupComplete();
		}
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00032CCC File Offset: 0x00030ECC
	private void SetState(byte[] data)
	{
		NetObj[] array = UnityEngine.Object.FindObjectsOfType(typeof(NetObj)) as NetObj[];
		foreach (NetObj netObj in array)
		{
			if (netObj != null)
			{
				UnityEngine.Object.DestroyImmediate(netObj.gameObject);
			}
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
		NetObj.ResetObjectDB();
		MemoryStream memoryStream = new MemoryStream(data);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		PRand.SetSeed(binaryReader.ReadInt32());
		this.m_levelScript.LoadState(binaryReader);
		this.m_turnMan.Load(binaryReader);
		int num = binaryReader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			string text = binaryReader.ReadString();
			GameObject gameObject = ObjectFactory.instance.Create(text);
			DebugUtils.Assert(gameObject, "Faield to create object instance " + text);
			long position = memoryStream.Position;
			NetObj component = gameObject.GetComponent<NetObj>();
			component.LoadState(binaryReader);
			long num2 = memoryStream.Position - position;
		}
		NetObj.SetNextNetID(binaryReader.ReadInt32());
		this.UpdateLineOfSightAll();
		if (this.m_localPlayerID >= 0)
		{
			this.UpdateVisability(this.m_localPlayerID);
		}
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00032E08 File Offset: 0x00031008
	public byte[] GetState()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(PRand.GetSeed());
		this.m_levelScript.SaveState(binaryWriter);
		this.m_turnMan.Save(binaryWriter);
		NetObj[] allToSave = NetObj.GetAllToSave();
		binaryWriter.Write(allToSave.Length);
		foreach (NetObj netObj in allToSave)
		{
			binaryWriter.Write(netObj.name);
			netObj.SaveState(binaryWriter);
		}
		binaryWriter.Write(NetObj.GetNextNetID());
		return memoryStream.ToArray();
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00032E9C File Offset: 0x0003109C
	public void SetOrders(int playerID, byte[] orders)
	{
		MemoryStream input = new MemoryStream(orders);
		BinaryReader binaryReader = new BinaryReader(input);
		for (int num = binaryReader.ReadInt32(); num != 0; num = binaryReader.ReadInt32())
		{
			Unit unit = NetObj.GetByID(num) as Unit;
			if (unit == null)
			{
				PLog.LogError("Could not find unit " + num + " in state");
			}
			if (!MNTutorial.IsTutorialActive() && playerID != -1 && unit.GetOwner() != playerID)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"Player ",
					playerID,
					" gave order to unit ",
					num,
					" owned by ",
					unit.GetOwner()
				}));
			}
			unit.LoadOrders(binaryReader);
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00032F70 File Offset: 0x00031170
	public void ClearNonLocalOrders()
	{
		if (this.m_localPlayerID == -1)
		{
			return;
		}
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Unit unit = netObj as Unit;
			if (unit != null && unit.GetOwner() != this.m_localPlayerID)
			{
				unit.ClearOrders();
			}
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00033008 File Offset: 0x00031208
	public byte[] GetOrders(int playerID)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		NetObj[] allToSave = NetObj.GetAllToSave();
		foreach (NetObj netObj in allToSave)
		{
			Unit unit = netObj as Unit;
			if (unit != null && (playerID == -1 || unit.GetOwner() == playerID || MNTutorial.IsTutorialActive()))
			{
				binaryWriter.Write(unit.GetNetID());
				unit.SaveOrders(binaryWriter);
			}
		}
		binaryWriter.Write(0);
		return memoryStream.ToArray();
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x000330A0 File Offset: 0x000312A0
	public int GetCurrentFrame()
	{
		return this.m_currentFrame;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x000330A8 File Offset: 0x000312A8
	public int GetTotalFrames()
	{
		return this.m_frames;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000330B0 File Offset: 0x000312B0
	private void UpdateLineOfSightAll()
	{
		this.UpdateLineOfSightIterative(1000000);
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000330C0 File Offset: 0x000312C0
	private void UpdateLineOfSightIterative(int toUpdate)
	{
		List<NetObj> all = NetObj.GetAll();
		if (this.m_updateLOSIndex >= all.Count)
		{
			this.m_updateLOSIndex = 0;
		}
		int num = this.m_updateLOSIndex + toUpdate;
		if (num > all.Count)
		{
			num = all.Count;
		}
		for (int i = this.m_updateLOSIndex; i < num; i++)
		{
			NetObj netObj = all[i];
			if (netObj.GetUpdateSeenBy())
			{
				int num2 = 0;
				int owner = netObj.GetOwner();
				int playerTeam = this.m_turnMan.GetPlayerTeam(owner);
				foreach (NetObj netObj2 in all)
				{
					Unit unit = netObj2 as Unit;
					if (unit != null && unit.CanLOS())
					{
						int owner2 = unit.GetOwner();
						int playerTeam2 = this.m_turnMan.GetPlayerTeam(owner2);
						if (CheatMan.instance.GetNoFogOfWar() && this.m_turnMan.IsHuman(owner2))
						{
							num2 |= 1 << playerTeam2;
						}
						else if (owner2 == owner)
						{
							num2 |= 1 << playerTeam2;
						}
						else if (playerTeam2 == playerTeam)
						{
							num2 |= 1 << playerTeam2;
						}
						else if (unit.TestLOS(netObj))
						{
							num2 |= 1 << playerTeam2;
						}
					}
				}
				netObj.UpdateSeenByMask(num2);
			}
		}
		this.m_updateLOSIndex = num;
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x0003327C File Offset: 0x0003147C
	private void DoIntervalLineOfSightUpdate()
	{
		this.m_lineOfSightUpdateTimer -= Time.fixedDeltaTime;
		if (this.m_lineOfSightUpdateTimer <= 0f)
		{
			this.m_lineOfSightUpdateTimer += 0.01f;
			this.UpdateLineOfSightIterative(this.m_LOSUpdatesPerFrame);
			if (this.m_localPlayerID >= 0)
			{
				this.UpdateVisability(this.m_localPlayerID);
			}
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000332E4 File Offset: 0x000314E4
	private void UpdateVisability(int localPlayerID)
	{
		int playerTeam = this.m_turnMan.GetPlayerTeam(localPlayerID);
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			if (netObj.IsSeenByTeam(playerTeam))
			{
				netObj.SetVisible(true);
			}
			else
			{
				netObj.SetVisible(false);
			}
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x00033370 File Offset: 0x00031570
	public void SetSimulating(bool enabled)
	{
		if (enabled && this.m_currentFrame >= this.m_frames)
		{
			PLog.LogWarning("GameState: tried to enable simulation at end of simulation");
			return;
		}
		NetObj.SetSimulating(enabled);
		if (this.m_levelScript != null)
		{
			this.m_levelScript.SetSimulating(enabled);
		}
		if (enabled && this.m_fastSimulation)
		{
			Time.timeScale = 99f;
			Time.captureFramerate = 1;
		}
		else
		{
			Time.timeScale = 1f;
			Time.captureFramerate = 0;
		}
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000333F8 File Offset: 0x000315F8
	public bool IsSimulating()
	{
		return NetObj.IsSimulating();
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x00033400 File Offset: 0x00031600
	public GameCamera GetGameCamera()
	{
		return this.m_gameCamera;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00033408 File Offset: 0x00031608
	public GameMode GetGameModeScript()
	{
		if (this.m_levelScript == null)
		{
			return null;
		}
		return this.m_levelScript.GetGameModeScript();
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00033428 File Offset: 0x00031628
	public LevelScript GetLevelScript()
	{
		return this.m_levelScript;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00033430 File Offset: 0x00031630
	public TurnMan GetTurnMan()
	{
		return this.m_turnMan;
	}

	// Token: 0x0400059B RID: 1435
	public GameState.SetupCompleteHandler m_onSetupComplete;

	// Token: 0x0400059C RID: 1436
	public GameState.SimulationCompleteHandler m_onSimulationComplete;

	// Token: 0x0400059D RID: 1437
	private GameSettings m_gameSettings;

	// Token: 0x0400059E RID: 1438
	private LevelScript m_levelScript;

	// Token: 0x0400059F RID: 1439
	private GameCamera m_gameCamera;

	// Token: 0x040005A0 RID: 1440
	private GameObject m_guiCamera;

	// Token: 0x040005A1 RID: 1441
	private TurnMan m_turnMan;

	// Token: 0x040005A2 RID: 1442
	private float m_lineOfSightUpdateTimer;

	// Token: 0x040005A3 RID: 1443
	private int m_updateLOSIndex;

	// Token: 0x040005A4 RID: 1444
	private int m_LOSUpdatesPerFrame = 2;

	// Token: 0x040005A5 RID: 1445
	private int m_localPlayerID = -1;

	// Token: 0x040005A6 RID: 1446
	private int m_frames;

	// Token: 0x040005A7 RID: 1447
	private bool m_gameHasEnded;

	// Token: 0x040005A8 RID: 1448
	private TurnPhase m_phase;

	// Token: 0x040005A9 RID: 1449
	private bool m_fastSimulation;

	// Token: 0x040005AA RID: 1450
	private byte[] m_state;

	// Token: 0x040005AB RID: 1451
	private bool m_setupComplete;

	// Token: 0x040005AC RID: 1452
	private bool m_levelWasLoaded;

	// Token: 0x040005AD RID: 1453
	private int m_currentFrame;

	// Token: 0x020001AD RID: 429
	// (Invoke) Token: 0x06000F64 RID: 3940
	public delegate void SetupCompleteHandler();

	// Token: 0x020001AE RID: 430
	// (Invoke) Token: 0x06000F68 RID: 3944
	public delegate void SimulationCompleteHandler();
}
