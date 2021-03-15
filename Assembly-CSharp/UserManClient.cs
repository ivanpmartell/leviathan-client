using System;
using System.Collections.Generic;
using PTech;

// Token: 0x0200016D RID: 365
public abstract class UserManClient
{
	// Token: 0x06000DB8 RID: 3512 RVA: 0x00062084 File Offset: 0x00060284
	public List<ShipDef> GetShipListFromFleet(string fleetName, int campaignID)
	{
		FleetDef fleet = this.GetFleet(fleetName, campaignID);
		if (fleet == null)
		{
			return null;
		}
		return fleet.m_ships;
	}

	// Token: 0x06000DB9 RID: 3513
	public abstract List<ShipDef> GetShipDefs(int campaignID);

	// Token: 0x06000DBA RID: 3514
	public abstract List<FleetDef> GetFleetDefs(int campaignID);

	// Token: 0x06000DBB RID: 3515
	public abstract List<string> GetAvailableMaps();

	// Token: 0x06000DBC RID: 3516
	public abstract List<string> GetUnlockedCampaignMaps(string campaign);

	// Token: 0x06000DBD RID: 3517
	public abstract List<string> GetAvailableCampaigns();

	// Token: 0x06000DBE RID: 3518
	public abstract List<string> GetAvailableShips(int campaignID);

	// Token: 0x06000DBF RID: 3519
	public abstract List<string> GetAvailableSections(int campaignID);

	// Token: 0x06000DC0 RID: 3520
	public abstract List<string> GetAvailableHPModules(int campaignID);

	// Token: 0x06000DC1 RID: 3521
	public abstract List<int> GetAvailableFlags();

	// Token: 0x06000DC2 RID: 3522
	public abstract void AddShip(ShipDef ship);

	// Token: 0x06000DC3 RID: 3523
	public abstract void AddFleet(FleetDef fleet);

	// Token: 0x06000DC4 RID: 3524
	public abstract void RemoveShip(string name);

	// Token: 0x06000DC5 RID: 3525
	public abstract void RemoveFleet(string fleet);

	// Token: 0x06000DC6 RID: 3526
	public abstract FleetDef GetFleet(string name, int campaignID);

	// Token: 0x06000DC7 RID: 3527
	public abstract void SetFlag(int flag);

	// Token: 0x06000DC8 RID: 3528
	public abstract void UnlockAchievement(int id);

	// Token: 0x06000DC9 RID: 3529
	public abstract void AddShipyardTime(float time);

	// Token: 0x06000DCA RID: 3530
	public abstract void BuyPackage(string packageName);

	// Token: 0x06000DCB RID: 3531
	public abstract void SetOwnedPackages(List<string> owned);

	// Token: 0x06000DCC RID: 3532 RVA: 0x000620A8 File Offset: 0x000602A8
	protected void UpdateShipAvailability(List<ShipDef> shipDefs)
	{
		ComponentDB instance = ComponentDB.instance;
		List<string> availableShips = this.GetAvailableShips(0);
		List<string> availableSections = this.GetAvailableSections(0);
		List<string> availableHPModules = this.GetAvailableHPModules(0);
		foreach (ShipDef shipDef in shipDefs)
		{
			if (shipDef.m_campaignID == 0)
			{
				shipDef.UpdateAvailability(instance, availableShips, availableSections, availableHPModules);
			}
		}
	}

	// Token: 0x06000DCD RID: 3533 RVA: 0x0006213C File Offset: 0x0006033C
	protected void UpdateFleetAvailability(List<FleetDef> fleetDefs)
	{
		ComponentDB instance = ComponentDB.instance;
		List<string> availableShips = this.GetAvailableShips(0);
		List<string> availableSections = this.GetAvailableSections(0);
		List<string> availableHPModules = this.GetAvailableHPModules(0);
		foreach (FleetDef fleetDef in fleetDefs)
		{
			if (fleetDef.m_campaignID == 0)
			{
				fleetDef.UpdateAvailability(instance, availableShips, availableSections, availableHPModules);
			}
		}
	}

	// Token: 0x04000B33 RID: 2867
	public UserManClient.UpdatedHandler m_onUpdated;

	// Token: 0x020001B4 RID: 436
	// (Invoke) Token: 0x06000F80 RID: 3968
	public delegate void UpdatedHandler();
}
