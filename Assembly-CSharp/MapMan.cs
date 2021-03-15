using System;
using System.Collections.Generic;
using System.Xml;
using PTech;

// Token: 0x02000146 RID: 326
public class MapMan
{
	// Token: 0x06000C86 RID: 3206 RVA: 0x00059DC4 File Offset: 0x00057FC4
	public void AddLevels(XmlDocument xmlDoc)
	{
		XmlNode firstChild = xmlDoc.FirstChild;
		XmlNode root = firstChild.SelectSingleNode("skirmish");
		this.AddMaps(root, GameType.Points, this.m_skirmish);
		XmlNode root2 = firstChild.SelectSingleNode("challenge");
		this.AddMaps(root2, GameType.Challenge, this.m_challenge);
		XmlNode root3 = firstChild.SelectSingleNode("custom");
		this.AddMaps(root3, GameType.Custom, this.m_custom);
		XmlNode root4 = firstChild.SelectSingleNode("assassination");
		this.AddMaps(root4, GameType.Assassination, this.m_assassination);
		XmlNode root5 = firstChild.SelectSingleNode("campaigns");
		this.AddCampaigns(root5);
		PLog.Log(string.Concat(new object[]
		{
			"added levels, skirmish:",
			this.m_skirmish.Count,
			" challenge: ",
			this.m_challenge.Count,
			" campaigns: ",
			this.m_campaigns.Count
		}));
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00059EB8 File Offset: 0x000580B8
	private void AddCampaigns(XmlNode root)
	{
		for (XmlNode xmlNode = root.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "campaign")
			{
				CampaignInfo campaignInfo = new CampaignInfo();
				campaignInfo.m_name = xmlNode.Attributes["name"].Value;
				campaignInfo.m_thumbnail = xmlNode.Attributes["thumbnail"].Value;
				if (xmlNode.Attributes["description"] != null)
				{
					campaignInfo.m_description = xmlNode.Attributes["description"].Value;
				}
				if (xmlNode.Attributes["tutorial"] != null)
				{
					campaignInfo.m_tutorial = bool.Parse(xmlNode.Attributes["tutorial"].Value);
				}
				this.AddMaps(xmlNode, GameType.Campaign, campaignInfo.m_maps);
				this.m_campaigns.Add(campaignInfo);
			}
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00059FB0 File Offset: 0x000581B0
	private void AddMaps(XmlNode root, GameType type, List<MapInfo> mapList)
	{
		for (XmlNode xmlNode = root.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "map")
			{
				MapInfo mapInfo = new MapInfo();
				mapInfo.m_gameMode = type;
				mapInfo.m_name = xmlNode.Attributes["name"].Value;
				mapInfo.m_scene = xmlNode.Attributes["scene"].Value;
				mapInfo.m_thumbnail = xmlNode.Attributes["thumbnail"].Value;
				mapInfo.m_player = int.Parse(xmlNode.Attributes["players"].Value);
				mapInfo.m_size = int.Parse(xmlNode.Attributes["size"].Value);
				if (xmlNode.Attributes["nofleet"] != null)
				{
					mapInfo.m_noFleet = bool.Parse(xmlNode.Attributes["nofleet"].Value);
				}
				if (xmlNode.Attributes["loadscreen"] != null)
				{
					mapInfo.m_loadscreen = xmlNode.Attributes["loadscreen"].Value;
				}
				if (xmlNode.Attributes["description"] != null)
				{
					mapInfo.m_description = xmlNode.Attributes["description"].Value;
				}
				if (xmlNode.Attributes["briefdescription"] != null)
				{
					mapInfo.m_briefDescription = xmlNode.Attributes["briefdescription"].Value;
				}
				if (xmlNode.Attributes["fleetlimit"] != null)
				{
					int min = (xmlNode.Attributes["fleetmin"] == null) ? 1 : int.Parse(xmlNode.Attributes["fleetmin"].Value);
					int max = int.Parse(xmlNode.Attributes["fleetlimit"].Value);
					mapInfo.m_fleetLimit = new FleetSize(min, max);
				}
				if (xmlNode.Attributes["defaults"] != null)
				{
					mapInfo.m_defaults = xmlNode.Attributes["defaults"].Value;
				}
				if (xmlNode.Attributes["content"] != null)
				{
					mapInfo.m_contentPack = xmlNode.Attributes["content"].Value;
				}
				mapList.Add(mapInfo);
			}
		}
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0005A228 File Offset: 0x00058428
	public MapInfo GetMapByName(GameType mode, string campaign, string name)
	{
		switch (mode)
		{
		case GameType.Challenge:
			foreach (MapInfo mapInfo in this.m_challenge)
			{
				if (mapInfo.m_name == name)
				{
					return mapInfo;
				}
			}
			break;
		case GameType.Campaign:
		{
			CampaignInfo campaign2 = this.GetCampaign(campaign);
			if (campaign2 != null)
			{
				return campaign2.GetMap(name);
			}
			break;
		}
		case GameType.Points:
			foreach (MapInfo mapInfo2 in this.m_skirmish)
			{
				if (mapInfo2.m_name == name)
				{
					return mapInfo2;
				}
			}
			break;
		case GameType.Assassination:
			foreach (MapInfo mapInfo3 in this.m_assassination)
			{
				if (mapInfo3.m_name == name)
				{
					return mapInfo3;
				}
			}
			break;
		case GameType.Custom:
			foreach (MapInfo mapInfo4 in this.m_custom)
			{
				if (mapInfo4.m_name == name)
				{
					return mapInfo4;
				}
			}
			break;
		}
		return null;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0005A440 File Offset: 0x00058640
	public CampaignInfo GetCampaign(string campaign)
	{
		foreach (CampaignInfo campaignInfo in this.m_campaigns)
		{
			if (campaignInfo.m_name == campaign)
			{
				return campaignInfo;
			}
		}
		return null;
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x0005A4BC File Offset: 0x000586BC
	public List<MapInfo> GetChallengeMaps()
	{
		return this.m_challenge;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x0005A4C4 File Offset: 0x000586C4
	public List<MapInfo> GetCustomMaps()
	{
		return this.m_custom;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0005A4CC File Offset: 0x000586CC
	public List<MapInfo> GetSkirmishMaps()
	{
		return this.m_skirmish;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0005A4D4 File Offset: 0x000586D4
	public List<MapInfo> GetAssassinationMaps()
	{
		return this.m_assassination;
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0005A4DC File Offset: 0x000586DC
	public List<MapInfo> GetCampaignMaps(string campaign)
	{
		CampaignInfo campaign2 = this.GetCampaign(campaign);
		if (campaign2 != null)
		{
			return campaign2.m_maps;
		}
		return new List<MapInfo>();
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0005A504 File Offset: 0x00058704
	public List<CampaignInfo> GetCampaigns()
	{
		return this.m_campaigns;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x0005A50C File Offset: 0x0005870C
	public MapInfo GetNextCampaignMap(string campaign, string map)
	{
		CampaignInfo campaign2 = this.GetCampaign(campaign);
		for (int i = 0; i < campaign2.m_maps.Count; i++)
		{
			if (campaign2.m_maps[i].m_name == map && i + 1 < campaign2.m_maps.Count)
			{
				return campaign2.m_maps[i + 1];
			}
		}
		return null;
	}

	// Token: 0x04000A3C RID: 2620
	private List<CampaignInfo> m_campaigns = new List<CampaignInfo>();

	// Token: 0x04000A3D RID: 2621
	private List<MapInfo> m_skirmish = new List<MapInfo>();

	// Token: 0x04000A3E RID: 2622
	private List<MapInfo> m_assassination = new List<MapInfo>();

	// Token: 0x04000A3F RID: 2623
	private List<MapInfo> m_challenge = new List<MapInfo>();

	// Token: 0x04000A40 RID: 2624
	private List<MapInfo> m_custom = new List<MapInfo>();
}
