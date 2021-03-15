using System;
using System.Collections.Generic;
using System.IO;
using PTech;
using UnityEngine;

// Token: 0x02000071 RID: 113
[AddComponentMenu("Scripts/GameModes/GameMode")]
public abstract class GameMode : MonoBehaviour
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x00029830 File Offset: 0x00027A30
	public virtual void Setup(GameSettings gameSettings)
	{
		this.m_gameSettings = gameSettings;
		Unit.m_onKilled = new Action<Unit>(this.OnUnitKilled);
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0002984C File Offset: 0x00027A4C
	public virtual void Close()
	{
		Unit.m_onKilled = null;
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00029854 File Offset: 0x00027A54
	public virtual void InitializeGame(List<PlayerInitData> players)
	{
		foreach (PlayerInitData playerInitData in players)
		{
			if (playerInitData.m_fleet != null)
			{
				this.SpawnPlayerShips(playerInitData.m_id, playerInitData.m_fleet);
			}
		}
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x000298CC File Offset: 0x00027ACC
	public GameObject GetSpawnPoint()
	{
		if (this.m_spawns == null)
		{
			this.m_spawns = new List<GameObject>();
			GameObject gameObject = GameObject.Find("playerspawns");
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				Transform child = gameObject.transform.GetChild(i);
				this.m_spawns.Add(child.gameObject);
			}
		}
		if (this.m_spawns.Count == 0)
		{
			PLog.LogError("Out of spawnpoints on level.");
		}
		int index = UnityEngine.Random.Range(0, this.m_spawns.Count - 1);
		GameObject result = this.m_spawns[index];
		this.m_spawns.RemoveAt(index);
		return result;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00029980 File Offset: 0x00027B80
	public virtual void OnStateLoaded()
	{
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00029984 File Offset: 0x00027B84
	public virtual void OnSimulationComplete()
	{
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00029988 File Offset: 0x00027B88
	protected int CheckNrOfUnits(int owner)
	{
		int num = 0;
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Ship component = netObj.GetComponent<Ship>();
			if (component != null && !component.IsDead() && component.GetOwner() == owner)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00029A20 File Offset: 0x00027C20
	public virtual int GetTargetScore()
	{
		return 0;
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00029A24 File Offset: 0x00027C24
	public virtual void SimulationUpdate(float dt)
	{
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00029A28 File Offset: 0x00027C28
	public virtual void LoadState(BinaryReader reader)
	{
		this.m_achYouAreTiny = reader.ReadBoolean();
		this.m_achAtlasCrush = reader.ReadBoolean();
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00029A44 File Offset: 0x00027C44
	public virtual void SaveState(BinaryWriter writer)
	{
		writer.Write(this.m_achYouAreTiny);
		writer.Write(this.m_achAtlasCrush);
	}

	// Token: 0x060004EE RID: 1262
	public abstract GameOutcome GetOutcome();

	// Token: 0x060004EF RID: 1263 RVA: 0x00029A60 File Offset: 0x00027C60
	public virtual int GetWinnerTeam(int nrOfPlayers)
	{
		return -1;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00029A64 File Offset: 0x00027C64
	public virtual int GetPlayerPlace(int playerID, int nrOfPlayers)
	{
		return 0;
	}

	// Token: 0x060004F1 RID: 1265
	public abstract bool IsPlayerDead(int playerID);

	// Token: 0x060004F2 RID: 1266 RVA: 0x00029A68 File Offset: 0x00027C68
	public virtual void OnUnitKilled(Unit unit)
	{
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00029A6C File Offset: 0x00027C6C
	public virtual void OnHPModuleKilled(HPModule module)
	{
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00029A70 File Offset: 0x00027C70
	public virtual bool HasObjectives()
	{
		return true;
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00029A74 File Offset: 0x00027C74
	protected void SpawnPlayerShips(int playerID, FleetDef playerFleet)
	{
		GameObject gameObject = GameObject.Find("PlayerStart" + playerID);
		if (gameObject == null)
		{
			PLog.LogError("Could not find spawnpoint for player " + playerID);
			return;
		}
		this.SpawnFleet(gameObject, playerID, playerFleet);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00029AC4 File Offset: 0x00027CC4
	protected void SpawnFleet(GameObject spawnPoint, int playerID, FleetDef playerFleet)
	{
		float num = 4f;
		float num2 = 0f;
		foreach (ShipDef def in playerFleet.m_ships)
		{
			num2 += num + ShipFactory.GetShipWidth(def) + num;
		}
		float num3 = -num2 / 2f;
		for (int i = 0; i < playerFleet.m_ships.Count; i++)
		{
			ShipDef shipDef = playerFleet.m_ships[i];
			float shipWidth = ShipFactory.GetShipWidth(shipDef);
			num3 += num + shipWidth / 2f;
			Vector3 position = new Vector3(num3, 0f, 0f);
			num3 += shipWidth / 2f + num;
			Vector3 pos = spawnPoint.transform.TransformPoint(position);
			pos.y = 0f;
			ShipFactory.CreateShip(shipDef, pos, spawnPoint.transform.rotation, playerID);
			if (shipDef.m_prefab != "MP-MB1")
			{
				this.m_achYouAreTiny = false;
			}
			if (shipDef.m_prefab != "MP-CS1")
			{
				this.m_achAtlasCrush = false;
			}
		}
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x00029C1C File Offset: 0x00027E1C
	public virtual void CheckAchivements(UserManClient m_userManClient)
	{
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00029C20 File Offset: 0x00027E20
	public void CheckVersusAchivements(UserManClient m_userManClient)
	{
		if (TurnMan.instance.GetPlayer(this.m_gameSettings.m_localPlayerID).m_team == this.GetWinnerTeam(this.m_gameSettings.m_nrOfPlayers))
		{
			if (!this.IsPlayerDead(this.m_gameSettings.m_localPlayerID) && TurnMan.instance.GetTotalShipsLost(this.m_gameSettings.m_localPlayerID) == 0)
			{
				m_userManClient.UnlockAchievement(14);
			}
			if (this.m_achYouAreTiny)
			{
				m_userManClient.UnlockAchievement(13);
			}
			if (this.m_achAtlasCrush)
			{
				m_userManClient.UnlockAchievement(12);
			}
		}
	}

	// Token: 0x04000411 RID: 1041
	protected GameSettings m_gameSettings;

	// Token: 0x04000412 RID: 1042
	private int m_currentSpawn;

	// Token: 0x04000413 RID: 1043
	private List<GameObject> m_spawns;

	// Token: 0x04000414 RID: 1044
	public bool m_achYouAreTiny;

	// Token: 0x04000415 RID: 1045
	public bool m_achAtlasCrush;
}
