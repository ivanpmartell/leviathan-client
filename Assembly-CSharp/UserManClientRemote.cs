using System;
using System.Collections.Generic;
using PTech;

// Token: 0x0200016F RID: 367
internal class UserManClientRemote : UserManClient
{
	// Token: 0x06000DE4 RID: 3556 RVA: 0x0006263C File Offset: 0x0006083C
	public UserManClientRemote(RPC rpc, GDPBackend gdpBackend)
	{
		this.m_rpc = rpc;
		this.m_gdpBackend = gdpBackend;
		this.m_rpc.Register("FleetList", new RPC.Handler(this.RPC_FleetList));
		this.m_rpc.Register("ShipList", new RPC.Handler(this.RPC_ShipList));
		this.m_rpc.Register("ContentPack", new RPC.Handler(this.RPC_ContentPack));
		this.RequestUpdate();
	}

	// Token: 0x06000DE5 RID: 3557 RVA: 0x000626F0 File Offset: 0x000608F0
	private void RequestUpdate()
	{
		this.m_rpc.Invoke("RequestShips", new object[0]);
		this.m_rpc.Invoke("RequestFleets", new object[0]);
		this.m_rpc.Invoke("RequestContentPack", new object[0]);
	}

	// Token: 0x06000DE6 RID: 3558 RVA: 0x00062740 File Offset: 0x00060940
	public override void SetFlag(int flag)
	{
		this.m_rpc.Invoke("SetFlag", new object[]
		{
			flag
		});
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00062764 File Offset: 0x00060964
	public override List<FleetDef> GetFleetDefs(int campaignID)
	{
		List<FleetDef> list = new List<FleetDef>();
		foreach (FleetDef fleetDef in this.m_fleetDefs)
		{
			if (fleetDef.m_campaignID == campaignID)
			{
				list.Add(fleetDef);
			}
		}
		return list;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x000627E0 File Offset: 0x000609E0
	public override List<ShipDef> GetShipDefs(int campaignID)
	{
		List<ShipDef> list = new List<ShipDef>();
		foreach (ShipDef shipDef in this.m_shipDefs)
		{
			if (shipDef.m_campaignID == campaignID)
			{
				list.Add(shipDef);
			}
		}
		return list;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x0006285C File Offset: 0x00060A5C
	public override List<string> GetAvailableMaps()
	{
		List<string> list = new List<string>();
		foreach (ContentPack contentPack in this.m_contentPacks)
		{
			list.AddRange(contentPack.m_maps);
		}
		return Utils.GetDistinctList(list);
	}

	// Token: 0x06000DEA RID: 3562 RVA: 0x000628D4 File Offset: 0x00060AD4
	public override List<string> GetAvailableCampaigns()
	{
		List<string> list = new List<string>();
		foreach (ContentPack contentPack in this.m_contentPacks)
		{
			list.AddRange(contentPack.m_campaigns);
		}
		return list;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00062948 File Offset: 0x00060B48
	public override List<int> GetAvailableFlags()
	{
		List<int> list = new List<int>();
		foreach (ContentPack contentPack in this.m_contentPacks)
		{
			list.AddRange(contentPack.m_flags);
		}
		return Utils.GetDistinctList(list);
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x000629C0 File Offset: 0x00060BC0
	public override List<string> GetUnlockedCampaignMaps(string campaign)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> keyValuePair in this.m_unlockedCampaignMaps)
		{
			if (keyValuePair.Key == campaign)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x00062A48 File Offset: 0x00060C48
	public override List<string> GetAvailableShips(int campaignID)
	{
		if (campaignID <= 0)
		{
			List<string> list = new List<string>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_ships);
			}
			return Utils.GetDistinctList(list);
		}
		ContentPack contentPack2;
		if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
		{
			return contentPack2.m_ships;
		}
		return null;
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00062AE4 File Offset: 0x00060CE4
	public override List<string> GetAvailableSections(int campaignID)
	{
		if (campaignID <= 0)
		{
			List<string> list = new List<string>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_sections);
			}
			return Utils.GetDistinctList(list);
		}
		ContentPack contentPack2;
		if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
		{
			return contentPack2.m_sections;
		}
		return null;
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x00062B80 File Offset: 0x00060D80
	public override List<string> GetAvailableHPModules(int campaignID)
	{
		if (campaignID <= 0)
		{
			List<string> list = new List<string>();
			foreach (ContentPack contentPack in this.m_contentPacks)
			{
				list.AddRange(contentPack.m_hpmodulse);
			}
			return Utils.GetDistinctList(list);
		}
		ContentPack contentPack2;
		if (this.m_campaignContentPacks.TryGetValue(campaignID, out contentPack2))
		{
			return contentPack2.m_hpmodulse;
		}
		return null;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x00062C1C File Offset: 0x00060E1C
	public override void AddShip(ShipDef ship)
	{
		this.m_rpc.Invoke("AddShip", new object[]
		{
			ship.ToArray()
		});
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x00062C40 File Offset: 0x00060E40
	public override void AddFleet(FleetDef fleet)
	{
		this.m_rpc.Invoke("AddFleet", new object[]
		{
			fleet.ToArray()
		});
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x00062C64 File Offset: 0x00060E64
	public override void RemoveShip(string shipName)
	{
		this.m_rpc.Invoke("RemoveShip", new object[]
		{
			shipName
		});
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x00062C80 File Offset: 0x00060E80
	public override void RemoveFleet(string fleetName)
	{
		this.m_rpc.Invoke("RemoveFleet", new object[]
		{
			fleetName
		});
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x00062C9C File Offset: 0x00060E9C
	public override FleetDef GetFleet(string name, int campaignID)
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

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00062D24 File Offset: 0x00060F24
	private void RPC_FleetList(RPC rpc, List<object> args)
	{
		int num = 0;
		int num2 = (int)args[num++];
		this.m_fleetDefs.Clear();
		for (int i = 0; i < num2; i++)
		{
			byte[] data = args[num++] as byte[];
			FleetDef item = new FleetDef(data);
			this.m_fleetDefs.Add(item);
		}
		base.UpdateFleetAvailability(this.m_fleetDefs);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00062DAC File Offset: 0x00060FAC
	private void RPC_ShipList(RPC rpc, List<object> args)
	{
		int num = 0;
		int num2 = (int)args[num++];
		this.m_shipDefs.Clear();
		for (int i = 0; i < num2; i++)
		{
			byte[] data = args[num++] as byte[];
			ShipDef item = new ShipDef(data);
			this.m_shipDefs.Add(item);
		}
		base.UpdateShipAvailability(this.m_shipDefs);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00062E34 File Offset: 0x00061034
	private void RPC_ContentPack(RPC rpc, List<object> args)
	{
		this.m_contentPacks.Clear();
		this.m_campaignContentPacks.Clear();
		this.m_unlockedCampaignMaps.Clear();
		int num = 0;
		int num2 = (int)args[num++];
		for (int i = 0; i < num2; i++)
		{
			byte[] data = (byte[])args[num++];
			ContentPack contentPack = new ContentPack();
			contentPack.FromArray(data);
			this.m_contentPacks.Add(contentPack);
		}
		int num3 = (int)args[num++];
		for (int j = 0; j < num3; j++)
		{
			int key = (int)args[num++];
			byte[] data2 = (byte[])args[num++];
			ContentPack contentPack2 = new ContentPack();
			contentPack2.FromArray(data2);
			this.m_campaignContentPacks.Add(key, contentPack2);
		}
		int num4 = (int)args[num++];
		for (int k = 0; k < num4; k++)
		{
			KeyValuePair<string, string> item = new KeyValuePair<string, string>((string)args[num++], (string)args[num++]);
			this.m_unlockedCampaignMaps.Add(item);
		}
		base.UpdateShipAvailability(this.m_shipDefs);
		base.UpdateFleetAvailability(this.m_fleetDefs);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00062FAC File Offset: 0x000611AC
	public override void AddShipyardTime(float time)
	{
		this.m_rpc.Invoke("AddShipyardTime", new object[]
		{
			time
		});
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00062FD0 File Offset: 0x000611D0
	public override void UnlockAchievement(int id)
	{
		PLog.Log("UnlockAchievement: " + id.ToString());
		if (this.m_gdpBackend != null)
		{
			this.m_gdpBackend.UnlockAchievement(id);
		}
		this.m_rpc.Invoke("UnlockAchievement", new object[]
		{
			id
		});
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x0006302C File Offset: 0x0006122C
	public override void BuyPackage(string packageName)
	{
		this.m_rpc.Invoke("BuyPackage", new object[]
		{
			packageName
		});
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x00063048 File Offset: 0x00061248
	public override void SetOwnedPackages(List<string> owned)
	{
		string[] item = owned.ToArray();
		List<object> list = new List<object>();
		list.Add(item);
		this.m_rpc.Invoke("SetOwnedPackages", list);
	}

	// Token: 0x04000B38 RID: 2872
	private RPC m_rpc;

	// Token: 0x04000B39 RID: 2873
	private GDPBackend m_gdpBackend;

	// Token: 0x04000B3A RID: 2874
	private List<ShipDef> m_shipDefs = new List<ShipDef>();

	// Token: 0x04000B3B RID: 2875
	private List<FleetDef> m_fleetDefs = new List<FleetDef>();

	// Token: 0x04000B3C RID: 2876
	private List<ContentPack> m_contentPacks = new List<ContentPack>();

	// Token: 0x04000B3D RID: 2877
	private Dictionary<int, ContentPack> m_campaignContentPacks = new Dictionary<int, ContentPack>();

	// Token: 0x04000B3E RID: 2878
	private List<KeyValuePair<string, string>> m_unlockedCampaignMaps = new List<KeyValuePair<string, string>>();
}
