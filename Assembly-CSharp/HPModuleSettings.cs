using System;
using System.Xml;

// Token: 0x0200012E RID: 302
public class HPModuleSettings
{
	// Token: 0x06000BCA RID: 3018 RVA: 0x0005492C File Offset: 0x00052B2C
	public static HPModuleSettings FromXml(XmlNode node)
	{
		return new HPModuleSettings
		{
			m_prefab = node.Attributes["prefab"].Value,
			m_value = int.Parse(node.Attributes["value"].Value)
		};
	}

	// Token: 0x04000992 RID: 2450
	public string m_prefab = string.Empty;

	// Token: 0x04000993 RID: 2451
	public int m_value;
}
