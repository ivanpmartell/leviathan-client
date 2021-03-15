using System;
using System.Collections.Generic;
using System.Xml;
using PTech;

// Token: 0x0200014A RID: 330
public class PackMan
{
	// Token: 0x06000CA2 RID: 3234 RVA: 0x0005AAD4 File Offset: 0x00058CD4
	public PackMan()
	{
		this.AddPacksInDir("shared_settings/packs");
		this.AddPacksInDir("shared_settings/campaign_packs");
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0005AB00 File Offset: 0x00058D00
	public void AddPacksInDir(string dir)
	{
		XmlDocument[] array = Utils.LoadXmlInDirectory(dir);
		foreach (XmlDocument xmlDoc in array)
		{
			ContentPack contentPack = new ContentPack();
			contentPack.Load(xmlDoc);
			this.AddPack(contentPack);
		}
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0005AB48 File Offset: 0x00058D48
	public ContentPack GetPack(string name)
	{
		ContentPack result;
		if (this.m_packs.TryGetValue(name, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0005AB6C File Offset: 0x00058D6C
	public ContentPack[] GetAllPacks()
	{
		List<ContentPack> list = new List<ContentPack>();
		foreach (KeyValuePair<string, ContentPack> keyValuePair in this.m_packs)
		{
			list.Add(keyValuePair.Value);
		}
		return list.ToArray();
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0005ABE4 File Offset: 0x00058DE4
	public int GetTotalNrOfFlags()
	{
		int num = 0;
		foreach (KeyValuePair<string, ContentPack> keyValuePair in this.m_packs)
		{
			num += keyValuePair.Value.m_flags.Count;
		}
		return num;
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0005AC5C File Offset: 0x00058E5C
	private void AddPack(ContentPack pack)
	{
		this.m_packs.Add(pack.m_name, pack);
	}

	// Token: 0x04000A58 RID: 2648
	private Dictionary<string, ContentPack> m_packs = new Dictionary<string, ContentPack>();
}
