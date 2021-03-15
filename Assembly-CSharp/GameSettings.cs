using System;
using PTech;

// Token: 0x020000B9 RID: 185
public class GameSettings
{
	// Token: 0x04000587 RID: 1415
	public int m_localPlayerID;

	// Token: 0x04000588 RID: 1416
	public bool m_localAdmin;

	// Token: 0x04000589 RID: 1417
	public int m_campaignID;

	// Token: 0x0400058A RID: 1418
	public int m_gameID;

	// Token: 0x0400058B RID: 1419
	public string m_gameName;

	// Token: 0x0400058C RID: 1420
	public string m_level;

	// Token: 0x0400058D RID: 1421
	public string m_campaign;

	// Token: 0x0400058E RID: 1422
	public GameType m_gameType;

	// Token: 0x0400058F RID: 1423
	public FleetSizeClass m_fleetSizeClass;

	// Token: 0x04000590 RID: 1424
	public FleetSize m_fleetSizeLimits = new FleetSize(0, 0);

	// Token: 0x04000591 RID: 1425
	public float m_targetScore;

	// Token: 0x04000592 RID: 1426
	public double m_maxTurnTime = -1.0;

	// Token: 0x04000593 RID: 1427
	public int m_nrOfPlayers;

	// Token: 0x04000594 RID: 1428
	public MapInfo m_mapInfo;

	// Token: 0x04000595 RID: 1429
	public CampaignInfo m_campaignInfo;
}
