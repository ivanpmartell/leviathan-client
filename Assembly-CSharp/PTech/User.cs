using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x0200015B RID: 347
	public class User
	{
		// Token: 0x06000CF5 RID: 3317 RVA: 0x0005CEB4 File Offset: 0x0005B0B4
		public User(string name, int userid)
		{
			this.m_name = name;
			this.m_userID = userid;
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x0005CF3C File Offset: 0x0005B13C
		public User()
		{
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x0005CFB8 File Offset: 0x0005B1B8
		public bool Update(float dt)
		{
			if (this.m_rpc != null)
			{
				this.m_onlineTime += dt;
				return this.m_rpc.Update(false);
			}
			return false;
		}

		// Token: 0x06000CF8 RID: 3320 RVA: 0x0005CFF0 File Offset: 0x0005B1F0
		public void Connect(RPC rpc, PlatformType platform)
		{
			this.m_rpc = rpc;
			this.m_platform = platform;
			this.m_stats.AddLogin(platform);
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0005D00C File Offset: 0x0005B20C
		public void Disconnect()
		{
			if (this.m_rpc != null)
			{
				this.m_rpc.Close();
				this.m_rpc = null;
				this.m_stats.m_totalPlayTime += (long)this.m_onlineTime;
				this.m_onlineTime = 0f;
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x0005D05C File Offset: 0x0005B25C
		public bool IsConnected()
		{
			return this.m_rpc != null;
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x0005D06C File Offset: 0x0005B26C
		public void ClearCampaign(int campaignID)
		{
			if (campaignID <= 0)
			{
				PLog.LogError("Cant clear campaign " + campaignID + " its not a valid campaign ID");
				return;
			}
			this.m_fleetDefs.RemoveAll((FleetDef item) => item.m_campaignID == campaignID);
			this.m_shipDefs.RemoveAll((ShipDef item) => item.m_campaignID == campaignID);
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x0005D0E4 File Offset: 0x0005B2E4
		public bool HaveCampaignFleet(int campaignID)
		{
			foreach (FleetDef fleetDef in this.m_fleetDefs)
			{
				if (fleetDef.m_campaignID == campaignID)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0005D15C File Offset: 0x0005B35C
		public FleetDef GetFleetDef(string name, int campaignID)
		{
			foreach (FleetDef fleetDef in this.m_fleetDefs)
			{
				if (fleetDef.m_name == name && fleetDef.m_campaignID == campaignID)
				{
					return fleetDef;
				}
			}
			return null;
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0005D1E4 File Offset: 0x0005B3E4
		public ShipDef GetShipDef(string name, int campaignID)
		{
			foreach (ShipDef shipDef in this.m_shipDefs)
			{
				if (shipDef.m_name == name && shipDef.m_campaignID == campaignID)
				{
					return shipDef;
				}
			}
			return null;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0005D26C File Offset: 0x0005B46C
		public void AddFleetDef(FleetDef newFleet)
		{
			for (int i = 0; i < this.m_fleetDefs.Count; i++)
			{
				if (this.m_fleetDefs[i].m_name == newFleet.m_name && this.m_fleetDefs[i].m_campaignID == newFleet.m_campaignID)
				{
					this.m_fleetDefs[i] = newFleet;
					if (this.m_onFleetsUpdate != null)
					{
						this.m_onFleetsUpdate(this);
					}
					return;
				}
			}
			this.m_fleetDefs.Add(newFleet);
			if (this.m_onFleetsUpdate != null)
			{
				this.m_onFleetsUpdate(this);
			}
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0005D31C File Offset: 0x0005B51C
		public void AddShipDef(ShipDef newShip)
		{
			for (int i = 0; i < this.m_shipDefs.Count; i++)
			{
				if (this.m_shipDefs[i].m_name == newShip.m_name)
				{
					this.m_shipDefs[i] = newShip;
					if (this.m_onShipsUpdate != null)
					{
						this.m_onShipsUpdate(this);
					}
					return;
				}
			}
			this.m_shipDefs.Add(newShip);
			if (this.m_onShipsUpdate != null)
			{
				this.m_onShipsUpdate(this);
			}
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0005D3B0 File Offset: 0x0005B5B0
		public void RemoveFleetDef(string name)
		{
			for (int i = 0; i < this.m_fleetDefs.Count; i++)
			{
				if (this.m_fleetDefs[i].m_name == name)
				{
					this.m_fleetDefs.RemoveAt(i);
					if (this.m_onFleetsUpdate != null)
					{
						this.m_onFleetsUpdate(this);
					}
					return;
				}
			}
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0005D41C File Offset: 0x0005B61C
		public bool RemoveShipDef(string name)
		{
			for (int i = 0; i < this.m_shipDefs.Count; i++)
			{
				if (this.m_shipDefs[i].m_name == name)
				{
					this.m_shipDefs.RemoveAt(i);
					if (this.m_onShipsUpdate != null)
					{
						this.m_onShipsUpdate(this);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x0005D488 File Offset: 0x0005B688
		public List<FleetDef> GetFleetDefs()
		{
			return this.m_fleetDefs;
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0005D490 File Offset: 0x0005B690
		public List<ShipDef> GetShipDefs()
		{
			return this.m_shipDefs;
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0005D498 File Offset: 0x0005B698
		public void SetCampaignContentPack(int campaignID, ContentPack pack)
		{
			this.m_campaignContentPacks[campaignID] = pack;
			if (this.m_onContentPackUpdate != null)
			{
				this.m_onContentPackUpdate(this);
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0005D4CC File Offset: 0x0005B6CC
		public void AddContentPack(ContentPack pack, MapMan mapman, bool unlockAllMaps)
		{
			if (!this.m_contentPacks.Contains(pack))
			{
				this.m_contentPacks.Add(pack);
				foreach (string campaign in pack.m_campaigns)
				{
					List<MapInfo> campaignMaps = mapman.GetCampaignMaps(campaign);
					if (campaignMaps.Count > 0)
					{
						this.UnlockCampaignMap(campaign, campaignMaps[0].m_name);
					}
				}
				if (this.m_onContentPackUpdate != null)
				{
					this.m_onContentPackUpdate(this);
				}
			}
			if (unlockAllMaps || this.m_unlockCampaignMaps)
			{
				foreach (string campaign2 in pack.m_campaigns)
				{
					List<MapInfo> campaignMaps2 = mapman.GetCampaignMaps(campaign2);
					foreach (MapInfo mapInfo in campaignMaps2)
					{
						this.UnlockCampaignMap(campaign2, mapInfo.m_name);
					}
				}
			}
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0005D64C File Offset: 0x0005B84C
		public void RemoveAllContentPacks()
		{
			this.m_contentPacks.Clear();
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0005D65C File Offset: 0x0005B85C
		public List<string> GetAvailableMaps()
		{
			List<string> list = new List<string>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_maps);
			}
			return list;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0005D6D0 File Offset: 0x0005B8D0
		public List<string> GetAvailableCampaigns()
		{
			List<string> list = new List<string>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_campaigns);
			}
			return list;
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0005D744 File Offset: 0x0005B944
		public List<string> GetAvailableShips(int campaignID)
		{
			if (campaignID <= 0)
			{
				List<string> list = new List<string>();
				foreach (ContentPack contentPack in this.m_contentPacks)
				{
					list.AddRange(contentPack.m_ships);
				}
				return list;
			}
			ContentPack contentPack2;
			if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
			{
				return contentPack2.m_ships;
			}
			return null;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0005D7DC File Offset: 0x0005B9DC
		public List<string> GetAvailableSections(int campaignID)
		{
			if (campaignID <= 0)
			{
				List<string> list = new List<string>();
				foreach (ContentPack contentPack in this.m_contentPacks)
				{
					list.AddRange(contentPack.m_sections);
				}
				return list;
			}
			ContentPack contentPack2;
			if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
			{
				return contentPack2.m_sections;
			}
			return null;
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0005D874 File Offset: 0x0005BA74
		public List<string> GetAvailableHPModules(int campaignID)
		{
			if (campaignID <= 0)
			{
				List<string> list = new List<string>();
				foreach (ContentPack contentPack in this.m_contentPacks)
				{
					list.AddRange(contentPack.m_hpmodulse);
				}
				return list;
			}
			ContentPack contentPack2;
			if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
			{
				return contentPack2.m_hpmodulse;
			}
			return null;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x0005D90C File Offset: 0x0005BB0C
		public List<int> GetAvailableFlags()
		{
			List<int> list = new List<int>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_flags);
			}
			return list;
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x0005D980 File Offset: 0x0005BB80
		public bool SetFlag(int flagID)
		{
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				if (contentPack.m_flags.Contains(flagID))
				{
					this.m_flag = flagID;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x0005DA04 File Offset: 0x0005BC04
		public List<KeyValuePair<string, string>> GetUnlockedCampaignMaps()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, string> item in this.m_unlockedCampaignMaps)
			{
				list.Add(item);
			}
			return list;
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x0005DA74 File Offset: 0x0005BC74
		public void UnlockCampaignMap(string campaign, string mapName)
		{
			foreach (KeyValuePair<string, string> keyValuePair in this.m_unlockedCampaignMaps)
			{
				if (keyValuePair.Key == campaign && keyValuePair.Value == mapName)
				{
					return;
				}
			}
			this.m_unlockedCampaignMaps.Add(new KeyValuePair<string, string>(campaign, mapName));
			if (this.m_onContentPackUpdate != null)
			{
				this.m_onContentPackUpdate(this);
			}
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x0005DB28 File Offset: 0x0005BD28
		public bool IsCampaignMapUnlocked(string campaign, string map)
		{
			foreach (KeyValuePair<string, string> keyValuePair in this.m_unlockedCampaignMaps)
			{
				if (keyValuePair.Key == campaign && keyValuePair.Value == map)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x0005DBB8 File Offset: 0x0005BDB8
		public List<ContentPack> GetContentPacks()
		{
			return this.m_contentPacks;
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0005DBC0 File Offset: 0x0005BDC0
		public Dictionary<int, ContentPack> GetCampaignContentPacks()
		{
			return this.m_campaignContentPacks;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0005DBC8 File Offset: 0x0005BDC8
		public byte[] GetFleetsAsArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_fleetDefs.Count);
			foreach (FleetDef fleetDef in this.m_fleetDefs)
			{
				fleetDef.Save(binaryWriter);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x0005DC54 File Offset: 0x0005BE54
		public void SetFleetsFromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				FleetDef fleetDef = new FleetDef();
				fleetDef.Load(binaryReader);
				foreach (ShipDef shipDef in fleetDef.m_ships)
				{
					shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance);
				}
				fleetDef.UpdateValue();
				this.m_fleetDefs.Add(fleetDef);
			}
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0005DD14 File Offset: 0x0005BF14
		public byte[] GetShipsAsArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_shipDefs.Count);
			foreach (ShipDef shipDef in this.m_shipDefs)
			{
				shipDef.Save(binaryWriter);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0005DDA0 File Offset: 0x0005BFA0
		public void SetShipsFromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ShipDef shipDef = new ShipDef();
				shipDef.Load(binaryReader);
				shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, ComponentDB.instance);
				this.m_shipDefs.Add(shipDef);
			}
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0005DE04 File Offset: 0x0005C004
		public byte[] GetCampaignCPsAsArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_campaignContentPacks.Count);
			foreach (KeyValuePair<int, ContentPack> keyValuePair in this.m_campaignContentPacks)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.m_name);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0005DEA8 File Offset: 0x0005C0A8
		public void SetCampaignCPsFromArray(byte[] data, PackMan packman)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int key = binaryReader.ReadInt32();
				string name = binaryReader.ReadString();
				ContentPack pack = packman.GetPack(name);
				this.m_campaignContentPacks.Add(key, pack);
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0005DF08 File Offset: 0x0005C108
		public byte[] GetUnlockedMapsAsArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_unlockedCampaignMaps.Count);
			foreach (KeyValuePair<string, string> keyValuePair in this.m_unlockedCampaignMaps)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0005DFA8 File Offset: 0x0005C1A8
		public void SetUnlockedMapsFromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = binaryReader.ReadString();
				string value = binaryReader.ReadString();
				this.m_unlockedCampaignMaps.Add(new KeyValuePair<string, string>(key, value));
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0005E000 File Offset: 0x0005C200
		public void OfflineSave(BinaryWriter stream)
		{
			stream.Write(1);
			stream.Write(this.m_name);
			stream.Write(this.m_flag);
			stream.Write(this.m_gamesCreated);
			byte[] fleetsAsArray = this.GetFleetsAsArray();
			stream.Write(fleetsAsArray.Length);
			stream.Write(fleetsAsArray);
			byte[] shipsAsArray = this.GetShipsAsArray();
			stream.Write(shipsAsArray.Length);
			stream.Write(shipsAsArray);
			byte[] campaignCPsAsArray = this.GetCampaignCPsAsArray();
			stream.Write(campaignCPsAsArray.Length);
			stream.Write(campaignCPsAsArray);
			byte[] unlockedMapsAsArray = this.GetUnlockedMapsAsArray();
			stream.Write(unlockedMapsAsArray.Length);
			stream.Write(unlockedMapsAsArray);
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0005E098 File Offset: 0x0005C298
		public void OfflineLoad(BinaryReader stream, PackMan packman)
		{
			int num = stream.ReadInt32();
			this.m_name = stream.ReadString();
			this.m_flag = stream.ReadInt32();
			this.m_gamesCreated = stream.ReadInt32();
			int count = stream.ReadInt32();
			byte[] fleetsFromArray = stream.ReadBytes(count);
			this.SetFleetsFromArray(fleetsFromArray);
			int count2 = stream.ReadInt32();
			byte[] shipsFromArray = stream.ReadBytes(count2);
			this.SetShipsFromArray(shipsFromArray);
			int count3 = stream.ReadInt32();
			byte[] data = stream.ReadBytes(count3);
			this.SetCampaignCPsFromArray(data, packman);
			int count4 = stream.ReadInt32();
			byte[] unlockedMapsFromArray = stream.ReadBytes(count4);
			this.SetUnlockedMapsFromArray(unlockedMapsFromArray);
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0005E134 File Offset: 0x0005C334
		public void AddEvent(UserEvent e)
		{
			this.m_events.Add(e);
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0005E144 File Offset: 0x0005C344
		public void ClearEvents()
		{
			this.m_events.Clear();
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0005E154 File Offset: 0x0005C354
		public byte[] GetEventsAsArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_events.Count);
			foreach (UserEvent userEvent in this.m_events)
			{
				userEvent.Save(binaryWriter);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0005E1E0 File Offset: 0x0005C3E0
		public void SetEventsFromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			this.m_events.Clear();
			for (int i = 0; i < num; i++)
			{
				UserEvent userEvent = new UserEvent();
				userEvent.Load(binaryReader);
				this.m_events.Add(userEvent);
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0005E23C File Offset: 0x0005C43C
		public void SetStatsFromXml(string xml)
		{
			this.m_stats = new UserStats();
			this.m_stats.LoadFromXml(xml);
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0005E258 File Offset: 0x0005C458
		public string GetStatsAsXml()
		{
			return this.m_stats.SaveToXml();
		}

		// Token: 0x04000AB2 RID: 2738
		public RPC m_rpc;

		// Token: 0x04000AB3 RID: 2739
		public int m_userID = -1;

		// Token: 0x04000AB4 RID: 2740
		public string m_name = string.Empty;

		// Token: 0x04000AB5 RID: 2741
		public int m_gamesCreated;

		// Token: 0x04000AB6 RID: 2742
		public int m_inGames;

		// Token: 0x04000AB7 RID: 2743
		public float m_onlineTime;

		// Token: 0x04000AB8 RID: 2744
		public PlatformType m_platform;

		// Token: 0x04000AB9 RID: 2745
		public int m_mailFlags;

		// Token: 0x04000ABA RID: 2746
		public bool m_unlockCampaignMaps;

		// Token: 0x04000ABB RID: 2747
		public int m_flag = 4;

		// Token: 0x04000ABC RID: 2748
		public UserStats m_stats = new UserStats();

		// Token: 0x04000ABD RID: 2749
		public List<UserEvent> m_events = new List<UserEvent>();

		// Token: 0x04000ABE RID: 2750
		public Action<User> m_onShipsUpdate;

		// Token: 0x04000ABF RID: 2751
		public Action<User> m_onFleetsUpdate;

		// Token: 0x04000AC0 RID: 2752
		public Action<User> m_onContentPackUpdate;

		// Token: 0x04000AC1 RID: 2753
		private List<FleetDef> m_fleetDefs = new List<FleetDef>();

		// Token: 0x04000AC2 RID: 2754
		private List<ShipDef> m_shipDefs = new List<ShipDef>();

		// Token: 0x04000AC3 RID: 2755
		private List<ContentPack> m_contentPacks = new List<ContentPack>();

		// Token: 0x04000AC4 RID: 2756
		private Dictionary<int, ContentPack> m_campaignContentPacks = new Dictionary<int, ContentPack>();

		// Token: 0x04000AC5 RID: 2757
		private List<KeyValuePair<string, string>> m_unlockedCampaignMaps = new List<KeyValuePair<string, string>>();

		// Token: 0x0200015C RID: 348
		public enum MailFlags
		{
			// Token: 0x04000AC7 RID: 2759
			AllMail = 1
		}
	}
}
