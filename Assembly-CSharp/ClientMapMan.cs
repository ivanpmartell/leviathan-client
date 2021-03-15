using System;
using System.Xml;
using UnityEngine;

// Token: 0x020000B3 RID: 179
internal class ClientMapMan : MapMan
{
	// Token: 0x0600068A RID: 1674 RVA: 0x00032120 File Offset: 0x00030320
	public ClientMapMan()
	{
		TextAsset textAsset = Resources.Load("shared_settings/levels") as TextAsset;
		DebugUtils.Assert(textAsset != null, "Missing levels.xml");
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(textAsset.text);
		base.AddLevels(xmlDocument);
	}
}
