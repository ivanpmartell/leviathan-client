using System;
using System.IO;
using UnityEngine;

// Token: 0x02000083 RID: 131
[AddComponentMenu("Scripts/Mission/MNSpawn")]
public class MNSpawn : MNode
{
	// Token: 0x0600054C RID: 1356 RVA: 0x0002B1A8 File Offset: 0x000293A8
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0002B1B0 File Offset: 0x000293B0
	private void FixedUpdate()
	{
		if (NetObj.m_simulating && this.m_atStartup)
		{
			this.DoAction();
			this.m_atStartup = false;
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0002B1E0 File Offset: 0x000293E0
	public virtual void OnDrawGizmosSelected()
	{
		this.m_aiSettings.OnDrawGizmosSelected(base.gameObject);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0002B1F4 File Offset: 0x000293F4
	public bool ShouldSpawn()
	{
		if (this.m_onlyIfInGame && !TurnMan.instance.IsHuman((int)this.m_aiSettings.m_targetOwner))
		{
			return false;
		}
		int nrOfPlayers = TurnMan.instance.GetNrOfPlayers();
		if (this.m_whenNumPlayers != MNSpawn.PlayerAmount.Always)
		{
			if (this.m_whenNumPlayers == MNSpawn.PlayerAmount.OnePlayer && nrOfPlayers != 1)
			{
				return false;
			}
			if (this.m_whenNumPlayers == MNSpawn.PlayerAmount.TwoPlayers && nrOfPlayers != 2)
			{
				return false;
			}
			if (this.m_whenNumPlayers == MNSpawn.PlayerAmount.ThreePlayers && nrOfPlayers != 3)
			{
				return false;
			}
			if (this.m_whenNumPlayers == MNSpawn.PlayerAmount.FourPlayers && nrOfPlayers != 4)
			{
				return false;
			}
			if (this.m_whenNumPlayers >= MNSpawn.PlayerAmount.OnePlayerOrMore)
			{
				int num = this.m_whenNumPlayers - MNSpawn.PlayerAmount.FourPlayers;
				if (TurnMan.instance.GetNrOfPlayers() < num)
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0002B2BC File Offset: 0x000294BC
	public override void DoAction()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (!ShipFactory.instance.ShipExist(this.m_spawnShip))
		{
			PLog.LogError("Could not find ship " + this.m_spawnShip + " in factory :(");
			return;
		}
		ShipAISettings.PlayerId playerId = this.m_aiSettings.m_targetOwner;
		if (playerId == ShipAISettings.PlayerId.NoChange)
		{
			PLog.Log("NoChange owner can not be used on spawning ship. Will spawn as Neutral. Spawner NetId: " + base.GetNetID().ToString());
			playerId = ShipAISettings.PlayerId.Neutral;
		}
		GameObject gameObject = ShipFactory.instance.CreateShip(this.m_spawnShip, base.transform.position, base.transform.rotation, (int)playerId);
		gameObject.GetComponent<Unit>().SetGroup(this.m_group);
		gameObject.GetComponent<Ship>().SetAiSettings(this.m_aiSettings);
		this.m_spawnedId = gameObject.GetComponent<NetObj>().GetNetID();
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0002B394 File Offset: 0x00029594
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_group);
		writer.Write(this.m_spawnShip);
		writer.Write(this.m_atStartup);
		writer.Write(this.m_onlyIfInGame);
		writer.Write((int)this.m_whenNumPlayers);
		writer.Write(this.m_area);
		writer.Write(this.m_name);
		writer.Write(this.m_spawnedId);
		this.m_aiSettings.SaveState(writer);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0002B414 File Offset: 0x00029614
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_group = reader.ReadString();
		this.m_spawnShip = reader.ReadString();
		this.m_atStartup = reader.ReadBoolean();
		this.m_onlyIfInGame = reader.ReadBoolean();
		this.m_whenNumPlayers = (MNSpawn.PlayerAmount)reader.ReadInt32();
		this.m_area = reader.ReadString();
		this.m_name = reader.ReadString();
		this.m_spawnedId = reader.ReadInt32();
		this.m_aiSettings.LoadState(reader);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0002B494 File Offset: 0x00029694
	public void SetObjective(Unit.ObjectiveTypes objectivety)
	{
		GameObject spawnedShip = this.GetSpawnedShip();
		if (spawnedShip == null)
		{
			return;
		}
		Unit component = spawnedShip.GetComponent<Unit>();
		if (component != null)
		{
			component.SetObjective(objectivety);
		}
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0002B4D0 File Offset: 0x000296D0
	public GameObject GetSpawnedShip()
	{
		if (this.m_spawnedId == -1)
		{
			return null;
		}
		NetObj byID = NetObj.GetByID(this.m_spawnedId);
		if (byID == null)
		{
			return null;
		}
		return byID.gameObject;
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0002B50C File Offset: 0x0002970C
	public bool SpawnedBeenDestroyed()
	{
		if (this.m_spawnedId == -1)
		{
			return false;
		}
		NetObj byID = NetObj.GetByID(this.m_spawnedId);
		if (byID == null)
		{
			return true;
		}
		Unit unit = byID as Unit;
		return unit == null || unit.IsDead();
	}

	// Token: 0x04000467 RID: 1127
	public string m_spawnShip = "Phantom";

	// Token: 0x04000468 RID: 1128
	public string m_group = string.Empty;

	// Token: 0x04000469 RID: 1129
	public bool m_atStartup;

	// Token: 0x0400046A RID: 1130
	public bool m_onlyIfInGame;

	// Token: 0x0400046B RID: 1131
	public MNSpawn.PlayerAmount m_whenNumPlayers;

	// Token: 0x0400046C RID: 1132
	public string m_area;

	// Token: 0x0400046D RID: 1133
	public string m_name;

	// Token: 0x0400046E RID: 1134
	public ShipAISettings m_aiSettings = new ShipAISettings();

	// Token: 0x0400046F RID: 1135
	private int m_spawnedId = -1;

	// Token: 0x02000084 RID: 132
	public enum PlayerAmount
	{
		// Token: 0x04000471 RID: 1137
		Always,
		// Token: 0x04000472 RID: 1138
		OnePlayer,
		// Token: 0x04000473 RID: 1139
		TwoPlayers,
		// Token: 0x04000474 RID: 1140
		ThreePlayers,
		// Token: 0x04000475 RID: 1141
		FourPlayers,
		// Token: 0x04000476 RID: 1142
		OnePlayerOrMore,
		// Token: 0x04000477 RID: 1143
		TwoPlayerOrMore,
		// Token: 0x04000478 RID: 1144
		ThreePlayerOrMore,
		// Token: 0x04000479 RID: 1145
		FourPlayerOrMore
	}

	// Token: 0x02000085 RID: 133
	public enum AiState
	{
		// Token: 0x0400047B RID: 1147
		Inactive,
		// Token: 0x0400047C RID: 1148
		Guard,
		// Token: 0x0400047D RID: 1149
		Patrol,
		// Token: 0x0400047E RID: 1150
		BossC1M3
	}
}
