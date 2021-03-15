using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

// Token: 0x02000086 RID: 134
[AddComponentMenu("Scripts/Mission/MNSpawnWave")]
public class MNSpawnWave : MNode
{
	// Token: 0x06000557 RID: 1367 RVA: 0x0002B570 File Offset: 0x00029770
	public override void Awake()
	{
		base.Awake();
		this.LoadWaves();
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0002B580 File Offset: 0x00029780
	private void FixedUpdate()
	{
		if (NetObj.m_simulating && this.m_status == MNSpawnWave.Mode.On)
		{
			this.m_time += Time.fixedDeltaTime;
			this.UpdateSpawn();
		}
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0002B5BC File Offset: 0x000297BC
	public override void DoAction()
	{
		if (this.m_status == MNSpawnWave.Mode.Off)
		{
			this.OnEvent("on");
		}
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0002B5D4 File Offset: 0x000297D4
	public override void OnEvent(string eventName)
	{
		if (eventName == "on")
		{
			PLog.Log("MNSpawnWave: On");
			this.m_time = 0f;
			this.m_status = MNSpawnWave.Mode.On;
			this.Reset();
		}
		else if (eventName == "off")
		{
			PLog.Log("MNSpawnWave: off");
			this.m_status = MNSpawnWave.Mode.Off;
		}
		else
		{
			base.EventWarning(eventName);
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0002B648 File Offset: 0x00029848
	private void Reset()
	{
		foreach (MNSpawnWave.WaveAction waveAction in this.m_waveaction)
		{
			waveAction.Reset();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0002B6B0 File Offset: 0x000298B0
	private void UpdateSpawn()
	{
		foreach (MNSpawnWave.WaveAction waveAction in this.m_waveaction)
		{
			waveAction.Update(this, this.m_time);
		}
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0002B71C File Offset: 0x0002991C
	public void SpawnIt(string shipname)
	{
		NetObj[] spawners = this.GetSpawners(this.m_name);
		foreach (NetObj netObj in spawners)
		{
			netObj.GetComponent<MNSpawn>().m_spawnShip = shipname;
			netObj.GetComponent<MNSpawn>().DoAction();
		}
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0002B768 File Offset: 0x00029968
	public NetObj[] GetSpawners(string name)
	{
		List<NetObj> list = new List<NetObj>();
		NetObj[] allToSave = NetObj.GetAllToSave();
		foreach (NetObj netObj in allToSave)
		{
			MNSpawn component = netObj.GetComponent<MNSpawn>();
			if (!(component == null))
			{
				if (!(component.m_area != this.m_area))
				{
					if (component.m_name == name)
					{
						list.Add(netObj);
					}
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x0002B7F8 File Offset: 0x000299F8
	public void LoadWaves()
	{
		if (this.m_wavefile.Length == 0)
		{
			return;
		}
		TextAsset textAsset = Resources.Load(this.m_wavefile) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(textAsset.text);
		for (XmlNode xmlNode = xmlDocument.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "wave" && xmlNode.Attributes["name"].Value == this.m_wavename)
			{
				this.LoadWave(xmlNode.FirstChild);
			}
		}
		this.m_wavefile = string.Empty;
		this.Reset();
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x0002B8B0 File Offset: 0x00029AB0
	public void LoadWave(XmlNode it)
	{
		while (it != null)
		{
			if (it.Name == "waveActions")
			{
				MNSpawnWave.WaveAction waveAction = new MNSpawnWave.WaveAction();
				waveAction.m_name = it.Attributes["area"].Value;
				waveAction.m_delay = float.Parse(it.Attributes["delay"].Value);
				for (XmlNode xmlNode = it.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.Name == "spawn")
					{
						MNSpawnWave.Spawn spawn = new MNSpawnWave.Spawn();
						spawn.m_spawnType = MNSpawnWave.SpawnType.Spawn;
						spawn.m_type = xmlNode.Attributes["type"].Value;
						spawn.m_number = int.Parse(xmlNode.Attributes["nr"].Value);
						spawn.m_delay = float.Parse(xmlNode.Attributes["delay"].Value);
						waveAction.m_spawns.Add(spawn);
					}
					if (xmlNode.Name == "message")
					{
						MNSpawnWave.Spawn spawn2 = new MNSpawnWave.Spawn();
						spawn2.m_spawnType = MNSpawnWave.SpawnType.Message;
						spawn2.m_type = xmlNode.Attributes["text"].Value;
						spawn2.m_delay = float.Parse(xmlNode.Attributes["delay"].Value);
						waveAction.m_spawns.Add(spawn2);
					}
				}
				this.m_waveaction.Add(waveAction);
			}
			it = it.NextSibling;
		}
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0002BA40 File Offset: 0x00029C40
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_wavefile);
		writer.Write(this.m_time);
		writer.Write(this.m_area);
		writer.Write((int)this.m_status);
		writer.Write(this.m_waveaction.Count);
		foreach (MNSpawnWave.WaveAction waveAction in this.m_waveaction)
		{
			waveAction.SaveState(writer);
		}
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0002BAF0 File Offset: 0x00029CF0
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_wavefile = reader.ReadString();
		this.m_time = reader.ReadSingle();
		this.m_area = reader.ReadString();
		this.m_status = (MNSpawnWave.Mode)reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			MNSpawnWave.WaveAction waveAction = new MNSpawnWave.WaveAction();
			waveAction.LoadState(reader);
			this.m_waveaction.Add(waveAction);
		}
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0002BB68 File Offset: 0x00029D68
	public void SetName(string name)
	{
		this.m_name = name;
	}

	// Token: 0x0400047F RID: 1151
	public string m_area;

	// Token: 0x04000480 RID: 1152
	public string m_wavefile;

	// Token: 0x04000481 RID: 1153
	public string m_wavename;

	// Token: 0x04000482 RID: 1154
	public MNSpawnWave.Mode m_status;

	// Token: 0x04000483 RID: 1155
	private List<MNSpawnWave.WaveAction> m_waveaction = new List<MNSpawnWave.WaveAction>();

	// Token: 0x04000484 RID: 1156
	private string m_name;

	// Token: 0x04000485 RID: 1157
	private float m_time;

	// Token: 0x02000087 RID: 135
	public enum SpawnType
	{
		// Token: 0x04000487 RID: 1159
		Spawn,
		// Token: 0x04000488 RID: 1160
		Message
	}

	// Token: 0x02000088 RID: 136
	[Serializable]
	public class Spawn
	{
		// Token: 0x06000565 RID: 1381 RVA: 0x0002BB84 File Offset: 0x00029D84
		public void SaveState(BinaryWriter writer)
		{
			writer.Write(this.m_delay);
			writer.Write(this.m_type);
			writer.Write(this.m_number);
			writer.Write(this.m_total);
			writer.Write((int)this.m_spawnType);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x0002BBD0 File Offset: 0x00029DD0
		public void LoadState(BinaryReader reader)
		{
			this.m_delay = reader.ReadSingle();
			this.m_type = reader.ReadString();
			this.m_number = reader.ReadInt32();
			this.m_total = reader.ReadInt32();
			this.m_spawnType = (MNSpawnWave.SpawnType)reader.ReadInt32();
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x0002BC1C File Offset: 0x00029E1C
		public void Update(MNSpawnWave wave, float time)
		{
			if (time < this.m_delay)
			{
				return;
			}
			if (this.m_total == 0)
			{
				return;
			}
			this.m_total = 0;
			if (this.m_spawnType == MNSpawnWave.SpawnType.Spawn)
			{
				wave.SpawnIt(this.m_type);
			}
			if (this.m_spawnType == MNSpawnWave.SpawnType.Message)
			{
				MessageLog.instance.ShowMessage(MessageLog.TextPosition.Middle, this.m_type, string.Empty, string.Empty, 2f);
			}
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0002BC8C File Offset: 0x00029E8C
		public void Reset()
		{
			this.m_total = this.m_number;
		}

		// Token: 0x04000489 RID: 1161
		public MNSpawnWave.SpawnType m_spawnType;

		// Token: 0x0400048A RID: 1162
		public float m_delay;

		// Token: 0x0400048B RID: 1163
		public string m_type;

		// Token: 0x0400048C RID: 1164
		public int m_number = 1;

		// Token: 0x0400048D RID: 1165
		private int m_total;
	}

	// Token: 0x02000089 RID: 137
	[Serializable]
	public class WaveAction
	{
		// Token: 0x0600056A RID: 1386 RVA: 0x0002BCB0 File Offset: 0x00029EB0
		public void SaveState(BinaryWriter writer)
		{
			writer.Write(this.m_name);
			writer.Write(this.m_delay);
			writer.Write(this.m_spawns.Count);
			foreach (MNSpawnWave.Spawn spawn in this.m_spawns)
			{
				spawn.SaveState(writer);
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0002BD40 File Offset: 0x00029F40
		public void LoadState(BinaryReader reader)
		{
			this.m_name = reader.ReadString();
			this.m_delay = reader.ReadSingle();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				MNSpawnWave.Spawn spawn = new MNSpawnWave.Spawn();
				spawn.LoadState(reader);
				this.m_spawns.Add(spawn);
			}
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0002BD98 File Offset: 0x00029F98
		public void Update(MNSpawnWave wave, float time)
		{
			if (time < this.m_delay)
			{
				return;
			}
			wave.SetName(this.m_name);
			foreach (MNSpawnWave.Spawn spawn in this.m_spawns)
			{
				spawn.Update(wave, time - this.m_delay);
			}
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x0002BE20 File Offset: 0x0002A020
		public void Reset()
		{
			foreach (MNSpawnWave.Spawn spawn in this.m_spawns)
			{
				spawn.Reset();
			}
		}

		// Token: 0x0400048E RID: 1166
		public string m_name;

		// Token: 0x0400048F RID: 1167
		public float m_delay;

		// Token: 0x04000490 RID: 1168
		public List<MNSpawnWave.Spawn> m_spawns = new List<MNSpawnWave.Spawn>();
	}

	// Token: 0x0200008A RID: 138
	public enum Mode
	{
		// Token: 0x04000492 RID: 1170
		Off,
		// Token: 0x04000493 RID: 1171
		On
	}
}
