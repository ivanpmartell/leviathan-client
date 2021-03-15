using System;
using System.Xml;

// Token: 0x0200012F RID: 303
public class UnitSettings
{
	// Token: 0x06000BCC RID: 3020 RVA: 0x00054990 File Offset: 0x00052B90
	public static UnitSettings FromXml(XmlNode node)
	{
		return new UnitSettings
		{
			m_prefab = node.Attributes["prefab"].Value,
			m_value = int.Parse(node.Attributes["value"].Value)
		};
	}

	// Token: 0x04000994 RID: 2452
	public string m_prefab = string.Empty;

	// Token: 0x04000995 RID: 2453
	public int m_value;
}
