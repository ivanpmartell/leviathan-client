using System;
using System.Collections.Generic;

// Token: 0x02000145 RID: 325
public class CampaignInfo
{
	// Token: 0x06000C84 RID: 3204 RVA: 0x00059D08 File Offset: 0x00057F08
	public MapInfo GetMap(string name)
	{
		foreach (MapInfo mapInfo in this.m_maps)
		{
			if (mapInfo.m_name == name)
			{
				return mapInfo;
			}
		}
		return null;
	}

	// Token: 0x04000A37 RID: 2615
	public string m_name = string.Empty;

	// Token: 0x04000A38 RID: 2616
	public string m_thumbnail = string.Empty;

	// Token: 0x04000A39 RID: 2617
	public string m_description = string.Empty;

	// Token: 0x04000A3A RID: 2618
	public bool m_tutorial;

	// Token: 0x04000A3B RID: 2619
	public List<MapInfo> m_maps = new List<MapInfo>();
}
