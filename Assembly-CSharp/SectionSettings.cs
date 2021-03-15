using System;
using System.Xml;

// Token: 0x02000130 RID: 304
public class SectionSettings
{
	// Token: 0x06000BCE RID: 3022 RVA: 0x000549F4 File Offset: 0x00052BF4
	public static SectionSettings FromXml(XmlNode node)
	{
		return new SectionSettings
		{
			m_prefab = node.Attributes["prefab"].Value,
			m_value = int.Parse(node.Attributes["value"].Value)
		};
	}

	// Token: 0x04000996 RID: 2454
	public string m_prefab = string.Empty;

	// Token: 0x04000997 RID: 2455
	public int m_value;
}
