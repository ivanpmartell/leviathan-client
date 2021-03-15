using System;
using System.Collections.Generic;
using System.Xml;

// Token: 0x02000131 RID: 305
public class ComponentDB
{
	// Token: 0x06000BCF RID: 3023 RVA: 0x00054A44 File Offset: 0x00052C44
	public ComponentDB()
	{
		this.AddPacksInDir("shared_settings/components");
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00054A84 File Offset: 0x00052C84
	public static ComponentDB instance
	{
		get
		{
			if (ComponentDB.m_instance == null)
			{
				ComponentDB.m_instance = new ComponentDB();
			}
			return ComponentDB.m_instance;
		}
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00054AA0 File Offset: 0x00052CA0
	public static void ResetInstance()
	{
		ComponentDB.m_instance = null;
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x00054AA8 File Offset: 0x00052CA8
	public void AddPacksInDir(string dir)
	{
		XmlDocument[] array = Utils.LoadXmlInDirectory(dir);
		foreach (XmlDocument xmlDoc in array)
		{
			this.AddSettings(xmlDoc);
		}
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00054AE0 File Offset: 0x00052CE0
	public void AddSettings(XmlDocument xmlDoc)
	{
		for (XmlNode xmlNode = xmlDoc.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			if (xmlNode.Name == "modules")
			{
				this.LoadModules(xmlNode);
			}
			else if (xmlNode.Name == "units")
			{
				this.LoadUnits(xmlNode);
			}
			else if (xmlNode.Name == "sections")
			{
				this.LoadSections(xmlNode);
			}
		}
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00054B6C File Offset: 0x00052D6C
	private void LoadModules(XmlNode it)
	{
		for (it = it.FirstChild; it != null; it = it.NextSibling)
		{
			HPModuleSettings hpmoduleSettings = HPModuleSettings.FromXml(it);
			this.m_modules.Add(hpmoduleSettings.m_prefab, hpmoduleSettings);
		}
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00054BB0 File Offset: 0x00052DB0
	private void LoadUnits(XmlNode it)
	{
		for (it = it.FirstChild; it != null; it = it.NextSibling)
		{
			UnitSettings unitSettings = UnitSettings.FromXml(it);
			this.m_units.Add(unitSettings.m_prefab, unitSettings);
		}
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00054BF4 File Offset: 0x00052DF4
	private void LoadSections(XmlNode it)
	{
		for (it = it.FirstChild; it != null; it = it.NextSibling)
		{
			SectionSettings sectionSettings = SectionSettings.FromXml(it);
			this.m_sections.Add(sectionSettings.m_prefab, sectionSettings);
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00054C38 File Offset: 0x00052E38
	public UnitSettings GetUnit(string name)
	{
		string key = name;
		if (name.Contains("(Clone)"))
		{
			key = name.Substring(0, name.Length - 7);
		}
		UnitSettings result;
		if (this.m_units.TryGetValue(key, out result))
		{
			return result;
		}
		PLog.LogError("Failed to find unit " + name);
		return null;
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00054C90 File Offset: 0x00052E90
	public HPModuleSettings GetModule(string name)
	{
		HPModuleSettings result;
		if (this.m_modules.TryGetValue(name, out result))
		{
			return result;
		}
		PLog.LogError("Failed to find module " + name);
		return null;
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00054CC4 File Offset: 0x00052EC4
	public SectionSettings GetSection(string name)
	{
		SectionSettings result;
		if (this.m_sections.TryGetValue(name, out result))
		{
			return result;
		}
		PLog.LogError("Failed to find section " + name + " Check that it is included in base_components.xml");
		return null;
	}

	// Token: 0x04000998 RID: 2456
	private static ComponentDB m_instance;

	// Token: 0x04000999 RID: 2457
	private Dictionary<string, HPModuleSettings> m_modules = new Dictionary<string, HPModuleSettings>();

	// Token: 0x0400099A RID: 2458
	private Dictionary<string, UnitSettings> m_units = new Dictionary<string, UnitSettings>();

	// Token: 0x0400099B RID: 2459
	private Dictionary<string, SectionSettings> m_sections = new Dictionary<string, SectionSettings>();
}
