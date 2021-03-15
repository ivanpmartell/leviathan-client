using System;
using System.Collections.Generic;
using System.Xml;
using PTech;
using UnityEngine;

// Token: 0x02000166 RID: 358
internal class ShipFactory
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000D5C RID: 3420 RVA: 0x0006010C File Offset: 0x0005E30C
	public static ShipFactory instance
	{
		get
		{
			if (ShipFactory.m_instance == null)
			{
				ShipFactory.m_instance = new ShipFactory();
			}
			return ShipFactory.m_instance;
		}
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00060128 File Offset: 0x0005E328
	public static void ResetInstance()
	{
		ShipFactory.m_instance = null;
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00060130 File Offset: 0x0005E330
	public void RegisterShips(string file)
	{
		TextAsset textAsset = Resources.Load(file) as TextAsset;
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(textAsset.text);
		ShipDefUtils.LoadFromXMLFile(xmlDocument, this.m_ships, ComponentDB.instance);
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0006016C File Offset: 0x0005E36C
	public void RegisterShip(string name, ShipDef def)
	{
		this.m_ships.Add(name, def);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0006017C File Offset: 0x0005E37C
	public ShipDef GetShipDef(string name)
	{
		ShipDef result;
		if (this.m_ships.TryGetValue(name, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x000601A0 File Offset: 0x0005E3A0
	public float GetShipWidth(string name)
	{
		ShipDef shipDef = this.GetShipDef(name);
		if (shipDef == null)
		{
			PLog.LogError("Could not find ship " + name + " in factory");
			return 0f;
		}
		return ShipFactory.GetShipWidth(shipDef);
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x000601DC File Offset: 0x0005E3DC
	public static float GetShipWidth(ShipDef def)
	{
		GameObject prefab = ObjectFactory.instance.GetPrefab(def.m_prefab);
		if (prefab == null)
		{
			return 0f;
		}
		Ship component = prefab.GetComponent<Ship>();
		return component.GetWidth();
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0006021C File Offset: 0x0005E41C
	public bool ShipExist(string name)
	{
		return this.GetShipDef(name) != null;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x0006023C File Offset: 0x0005E43C
	public GameObject CreateShip(string name, Vector3 pos, Quaternion rot, int owner)
	{
		ShipDef shipDef = this.GetShipDef(name);
		if (shipDef == null)
		{
			PLog.LogError("Could not find ship " + name + " in factory");
			return null;
		}
		return ShipFactory.CreateShip(shipDef, pos, rot, owner);
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x00060278 File Offset: 0x0005E478
	public static GameObject CreateShip(ShipDef def, Vector3 pos, Quaternion rot, int owner)
	{
		GameObject gameObject = ObjectFactory.instance.Create(def.m_prefab, pos, rot);
		if (gameObject == null)
		{
			PLog.LogError("Could not find prefab " + def.m_prefab + " for ship " + def.m_name);
			return null;
		}
		Ship component = gameObject.GetComponent<Ship>();
		component.SetName(def.m_name);
		Section section = component.SetSection(Section.SectionType.Front, def.m_frontSection.m_prefab);
		ShipFactory.AddHPModules(section, def.m_frontSection.m_modules);
		Section section2 = component.SetSection(Section.SectionType.Mid, def.m_midSection.m_prefab);
		ShipFactory.AddHPModules(section2, def.m_midSection.m_modules);
		Section section3 = component.SetSection(Section.SectionType.Rear, def.m_rearSection.m_prefab);
		ShipFactory.AddHPModules(section3, def.m_rearSection.m_modules);
		Section section4 = component.SetSection(Section.SectionType.Top, def.m_topSection.m_prefab);
		ShipFactory.AddHPModules(section4, def.m_topSection.m_modules);
		component.SetOwner(owner);
		component.ResetStats();
		NetObj[] componentsInChildren = gameObject.GetComponentsInChildren<NetObj>();
		foreach (NetObj netObj in componentsInChildren)
		{
			netObj.SetVisible(false);
		}
		return gameObject;
	}

	// Token: 0x06000D66 RID: 3430 RVA: 0x000603B0 File Offset: 0x0005E5B0
	private static void AddHPModules(Section section, List<ModuleDef> modules)
	{
		foreach (ModuleDef moduleDef in modules)
		{
			Battery battery = section.GetBattery(moduleDef.m_battery);
			if (battery == null)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"Error , tried to add module to non existing battery: ",
					moduleDef.m_battery,
					" on section ",
					section.name
				}));
			}
			else
			{
				battery.AddHPModule(moduleDef.m_prefab, moduleDef.m_pos.x, moduleDef.m_pos.y, moduleDef.m_direction);
			}
		}
	}

	// Token: 0x04000B03 RID: 2819
	private static ShipFactory m_instance;

	// Token: 0x04000B04 RID: 2820
	private Dictionary<string, ShipDef> m_ships = new Dictionary<string, ShipDef>();
}
