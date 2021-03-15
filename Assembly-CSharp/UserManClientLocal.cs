using System;
using System.Collections.Generic;
using PTech;

// Token: 0x0200016E RID: 366
internal class UserManClientLocal : UserManClient
{
	// Token: 0x06000DCE RID: 3534 RVA: 0x000621D0 File Offset: 0x000603D0
	public UserManClientLocal(User user, PackMan packMan, MapMan mapMan, GDPBackend gdpBackend)
	{
		this.m_user = user;
		this.m_packMan = packMan;
		this.m_mapMan = mapMan;
		this.m_gdpBackend = gdpBackend;
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x000621F8 File Offset: 0x000603F8
	public override List<FleetDef> GetFleetDefs(int campaignID)
	{
		List<FleetDef> list = new List<FleetDef>();
		foreach (FleetDef fleetDef in this.m_user.GetFleetDefs())
		{
			if (fleetDef.m_campaignID == campaignID)
			{
				list.Add(fleetDef);
			}
		}
		return list;
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00062278 File Offset: 0x00060478
	public override List<ShipDef> GetShipDefs(int campaignID)
	{
		List<ShipDef> list = new List<ShipDef>();
		foreach (ShipDef shipDef in this.m_user.GetShipDefs())
		{
			if (shipDef.m_campaignID == campaignID)
			{
				list.Add(shipDef);
			}
		}
		return list;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x000622F8 File Offset: 0x000604F8
	public override List<string> GetAvailableMaps()
	{
		return Utils.GetDistinctList(this.m_user.GetAvailableMaps());
	}

	// Token: 0x06000DD2 RID: 3538 RVA: 0x0006230C File Offset: 0x0006050C
	public override List<int> GetAvailableFlags()
	{
		return Utils.GetDistinctList(this.m_user.GetAvailableFlags());
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x00062320 File Offset: 0x00060520
	public override List<string> GetUnlockedCampaignMaps(string campaign)
	{
		List<KeyValuePair<string, string>> unlockedCampaignMaps = this.m_user.GetUnlockedCampaignMaps();
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> keyValuePair in unlockedCampaignMaps)
		{
			if (keyValuePair.Key == campaign)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x000623AC File Offset: 0x000605AC
	public override List<string> GetAvailableCampaigns()
	{
		return this.m_user.GetAvailableCampaigns();
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x000623BC File Offset: 0x000605BC
	public override List<string> GetAvailableShips(int campaignID)
	{
		return Utils.GetDistinctList(this.m_user.GetAvailableShips(campaignID));
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x000623D0 File Offset: 0x000605D0
	public override List<string> GetAvailableSections(int campaignID)
	{
		return Utils.GetDistinctList(this.m_user.GetAvailableSections(campaignID));
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x000623E4 File Offset: 0x000605E4
	public override List<string> GetAvailableHPModules(int campaignID)
	{
		return Utils.GetDistinctList(this.m_user.GetAvailableHPModules(campaignID));
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x000623F8 File Offset: 0x000605F8
	public override void AddShip(ShipDef ship)
	{
		this.m_user.AddShipDef(ship);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00062428 File Offset: 0x00060628
	public override void AddFleet(FleetDef fleet)
	{
		this.m_user.AddFleetDef(fleet);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00062458 File Offset: 0x00060658
	public override void RemoveShip(string name)
	{
		this.m_user.RemoveShipDef(name);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00062480 File Offset: 0x00060680
	public override void RemoveFleet(string fleet)
	{
		this.m_user.RemoveFleetDef(fleet);
		if (this.m_onUpdated != null)
		{
			this.m_onUpdated();
		}
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x000624B0 File Offset: 0x000606B0
	public override FleetDef GetFleet(string name, int campaignID)
	{
		return this.m_user.GetFleetDef(name, campaignID);
	}

	// Token: 0x06000DDD RID: 3549 RVA: 0x000624C0 File Offset: 0x000606C0
	public override void SetFlag(int flag)
	{
		this.m_user.SetFlag(flag);
	}

	// Token: 0x06000DDE RID: 3550 RVA: 0x000624D0 File Offset: 0x000606D0
	public override void AddShipyardTime(float time)
	{
		this.m_user.m_stats.m_totalShipyardTime += (long)time;
	}

	// Token: 0x06000DDF RID: 3551 RVA: 0x000624EC File Offset: 0x000606EC
	public override void UnlockAchievement(int id)
	{
		PLog.Log("UnlockAchievement: " + id.ToString());
		if (this.m_gdpBackend != null)
		{
			this.m_gdpBackend.UnlockAchievement(id);
		}
		this.m_user.m_stats.UnlockAchievement(id);
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x00062538 File Offset: 0x00060738
	public override void BuyPackage(string packageName)
	{
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x0006253C File Offset: 0x0006073C
	private void UnlockContentPack(string packageName)
	{
		PLog.Log("unlocking content pack " + packageName);
		bool unlockAllMaps = false;
		ContentPack pack = this.m_packMan.GetPack(packageName);
		if (pack != null)
		{
			this.m_user.AddContentPack(pack, this.m_mapMan, unlockAllMaps);
		}
		else
		{
			PLog.LogError("Tried to unlock missing content pack " + packageName);
		}
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x00062598 File Offset: 0x00060798
	public void ResetContentPacks()
	{
		this.m_user.RemoveAllContentPacks();
		this.UnlockContentPack("Base");
	}

	// Token: 0x06000DE3 RID: 3555 RVA: 0x000625B0 File Offset: 0x000607B0
	public override void SetOwnedPackages(List<string> owned)
	{
		this.ResetContentPacks();
		foreach (string packageName in owned)
		{
			this.UnlockContentPack(packageName);
		}
		base.UpdateShipAvailability(this.m_user.GetShipDefs());
		base.UpdateFleetAvailability(this.m_user.GetFleetDefs());
	}

	// Token: 0x04000B34 RID: 2868
	private User m_user;

	// Token: 0x04000B35 RID: 2869
	private PackMan m_packMan;

	// Token: 0x04000B36 RID: 2870
	private MapMan m_mapMan;

	// Token: 0x04000B37 RID: 2871
	private GDPBackend m_gdpBackend;
}
