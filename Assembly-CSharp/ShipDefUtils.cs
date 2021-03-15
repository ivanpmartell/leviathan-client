using System;
using System.Collections.Generic;
using System.Xml;
using PTech;

// Token: 0x02000157 RID: 343
internal class ShipDefUtils
{
	// Token: 0x06000CE3 RID: 3299 RVA: 0x0005C898 File Offset: 0x0005AA98
	public static void LoadFromXMLFile(XmlDocument xmlDoc, Dictionary<string, ShipDef> ships, ComponentDB cdb)
	{
		for (XmlNode xmlNode = xmlDoc.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "ship")
			{
				ShipDef shipDef = new ShipDef();
				shipDef.Load(xmlNode);
				shipDef.m_value = ShipDefUtils.GetShipValue(shipDef, cdb);
				ships[shipDef.m_name] = shipDef;
			}
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0005C900 File Offset: 0x0005AB00
	public static int GetShipValue(ShipDef ship, ComponentDB cdb)
	{
		int num = 0;
		UnitSettings unit = cdb.GetUnit(ship.m_prefab);
		if (unit == null)
		{
			PLog.LogError("Missing prefab in cdb: " + ship.m_prefab);
			return -1;
		}
		num += unit.m_value;
		num += ShipDefUtils.GetSectionValue(ship.m_frontSection, cdb);
		num += ShipDefUtils.GetSectionValue(ship.m_midSection, cdb);
		num += ShipDefUtils.GetSectionValue(ship.m_rearSection, cdb);
		return num + ShipDefUtils.GetSectionValue(ship.m_topSection, cdb);
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x0005C980 File Offset: 0x0005AB80
	private static int GetSectionValue(SectionDef section, ComponentDB cdb)
	{
		int num = 0;
		SectionSettings section2 = cdb.GetSection(section.m_prefab);
		if (section2 == null)
		{
			PLog.LogError("Missing section prefab in cdb:" + section.m_prefab);
			return -1;
		}
		num += section2.m_value;
		foreach (ModuleDef moduleDef in section.m_modules)
		{
			HPModuleSettings module = cdb.GetModule(moduleDef.m_prefab);
			if (module == null)
			{
				PLog.LogError("Missing module prefab in cdb:" + moduleDef.m_prefab);
				return -1;
			}
			num += module.m_value;
		}
		return num;
	}
}
