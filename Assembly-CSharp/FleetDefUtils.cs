using System;
using System.Collections.Generic;
using System.Xml;
using PTech;

// Token: 0x02000139 RID: 313
internal class FleetDefUtils
{
	// Token: 0x06000BF5 RID: 3061 RVA: 0x00055A80 File Offset: 0x00053C80
	private static int GetShipValue(List<ShipDef> ships, string name)
	{
		foreach (ShipDef shipDef in ships)
		{
			if (shipDef.m_name == name)
			{
				return shipDef.m_value;
			}
		}
		return -1;
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00055AFC File Offset: 0x00053CFC
	public static void LoadFleetsAndShipsXMLFile(XmlDocument xmlDoc, out List<FleetDef> fleets, out List<ShipDef> blueprints, ComponentDB cdb)
	{
		fleets = new List<FleetDef>();
		blueprints = new List<ShipDef>();
		for (XmlNode xmlNode = xmlDoc.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "fleet")
			{
				FleetDef item = FleetDefUtils.LoadFleet(xmlNode, cdb);
				fleets.Add(item);
			}
			if (xmlNode.Name == "blueprint")
			{
				ShipDef shipDef = new ShipDef();
				shipDef.Load(xmlNode);
				shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, cdb);
				blueprints.Add(shipDef);
			}
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00055B94 File Offset: 0x00053D94
	public static FleetDef LoadFleet(XmlNode root, ComponentDB cdb)
	{
		FleetDef fleetDef = new FleetDef();
		fleetDef.m_name = root.Attributes["name"].Value;
		for (XmlNode xmlNode = root.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "ship")
			{
				ShipDef shipDef = new ShipDef();
				shipDef.Load(xmlNode);
				shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, cdb);
				fleetDef.m_ships.Add(shipDef);
			}
		}
		fleetDef.UpdateValue();
		return fleetDef;
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x00055C20 File Offset: 0x00053E20
	public static Dictionary<string, int> GetModuleUsage(FleetDef fleet)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (ShipDef shipDef in fleet.m_ships)
		{
			List<string> hardpointNames = shipDef.GetHardpointNames();
			foreach (string key in hardpointNames)
			{
				int num;
				if (dictionary.TryGetValue(key, out num))
				{
					dictionary[key] = num + 1;
				}
				else
				{
					dictionary.Add(key, 1);
				}
			}
		}
		return dictionary;
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00055D04 File Offset: 0x00053F04
	public static Dictionary<string, int> GetShipUsage(FleetDef fleet)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (ShipDef shipDef in fleet.m_ships)
		{
			int num;
			if (dictionary.TryGetValue(shipDef.m_prefab, out num))
			{
				dictionary[shipDef.m_prefab] = num + 1;
			}
			else
			{
				dictionary.Add(shipDef.m_prefab, 1);
			}
		}
		return dictionary;
	}
}
