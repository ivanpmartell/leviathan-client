using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PTech
{
	// Token: 0x02000132 RID: 306
	public class ContentPack
	{
		// Token: 0x06000BDB RID: 3035 RVA: 0x00054D68 File Offset: 0x00052F68
		private ContentPack.Category CategoryFromString(string str)
		{
			if (str == "maps")
			{
				return ContentPack.Category.Maps;
			}
			if (str == "ships")
			{
				return ContentPack.Category.Ships;
			}
			if (str == "campaign")
			{
				return ContentPack.Category.Campaigns;
			}
			if (str == "flags")
			{
				return ContentPack.Category.Flags;
			}
			return ContentPack.Category.None;
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x00054DC0 File Offset: 0x00052FC0
		public void Load(XmlDocument xmlDoc)
		{
			for (XmlNode xmlNode = xmlDoc.FirstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "name")
				{
					this.m_name = xmlNode.FirstChild.Value;
				}
				else if (xmlNode.Name == "dev")
				{
					this.m_dev = bool.Parse(xmlNode.FirstChild.Value);
				}
				else if (xmlNode.Name == "new")
				{
					this.m_newItem = bool.Parse(xmlNode.FirstChild.Value);
				}
				else if (xmlNode.Name == "type")
				{
					this.m_type = this.CategoryFromString(xmlNode.FirstChild.Value);
				}
				else if (xmlNode.Name == "description")
				{
					this.m_description = xmlNode.FirstChild.Value;
				}
				else if (xmlNode.Name == "maps")
				{
					this.LoadMaps(xmlNode);
				}
				else if (xmlNode.Name == "campaigns")
				{
					this.LoadCampaigns(xmlNode);
				}
				else if (xmlNode.Name == "ships")
				{
					this.LoadShips(xmlNode);
				}
				else if (xmlNode.Name == "sections")
				{
					this.LoadSections(xmlNode);
				}
				else if (xmlNode.Name == "hpmodules")
				{
					this.LoadHPModules(xmlNode);
				}
				else if (xmlNode.Name == "flags")
				{
					this.LoadFlags(xmlNode);
				}
			}
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x00054F94 File Offset: 0x00053194
		public byte[] ToArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_name);
			binaryWriter.Write((byte)this.m_type);
			binaryWriter.Write(this.m_description);
			binaryWriter.Write(this.m_campaigns.Count);
			foreach (string value in this.m_campaigns)
			{
				binaryWriter.Write(value);
			}
			binaryWriter.Write(this.m_maps.Count);
			foreach (string value2 in this.m_maps)
			{
				binaryWriter.Write(value2);
			}
			binaryWriter.Write(this.m_hpmodulse.Count);
			foreach (string value3 in this.m_hpmodulse)
			{
				binaryWriter.Write(value3);
			}
			binaryWriter.Write(this.m_sections.Count);
			foreach (string value4 in this.m_sections)
			{
				binaryWriter.Write(value4);
			}
			binaryWriter.Write(this.m_ships.Count);
			foreach (string value5 in this.m_ships)
			{
				binaryWriter.Write(value5);
			}
			binaryWriter.Write((short)this.m_flags.Count);
			foreach (int num in this.m_flags)
			{
				binaryWriter.Write((short)num);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x00055258 File Offset: 0x00053458
		public void FromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			this.m_name = binaryReader.ReadString();
			this.m_type = (ContentPack.Category)binaryReader.ReadByte();
			this.m_description = binaryReader.ReadString();
			this.m_campaigns.Clear();
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.m_campaigns.Add(binaryReader.ReadString());
			}
			this.m_maps.Clear();
			int num2 = binaryReader.ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				this.m_maps.Add(binaryReader.ReadString());
			}
			this.m_hpmodulse.Clear();
			int num3 = binaryReader.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				this.m_hpmodulse.Add(binaryReader.ReadString());
			}
			this.m_sections.Clear();
			int num4 = binaryReader.ReadInt32();
			for (int l = 0; l < num4; l++)
			{
				this.m_sections.Add(binaryReader.ReadString());
			}
			this.m_ships.Clear();
			int num5 = binaryReader.ReadInt32();
			for (int m = 0; m < num5; m++)
			{
				this.m_ships.Add(binaryReader.ReadString());
			}
			this.m_flags.Clear();
			int num6 = (int)binaryReader.ReadInt16();
			for (int n = 0; n < num6; n++)
			{
				this.m_flags.Add((int)binaryReader.ReadInt16());
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x000553F4 File Offset: 0x000535F4
		private void LoadCampaigns(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "campaign")
				{
					this.m_campaigns.Add(xmlNode.Attributes["name"].Value);
				}
			}
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x00055450 File Offset: 0x00053650
		private void LoadMaps(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "map")
				{
					this.m_maps.Add(xmlNode.Attributes["name"].Value);
				}
			}
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x000554AC File Offset: 0x000536AC
		private void LoadShips(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "ship")
				{
					this.m_ships.Add(xmlNode.Attributes["prefab"].Value);
				}
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x00055508 File Offset: 0x00053708
		private void LoadSections(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "section")
				{
					this.m_sections.Add(xmlNode.Attributes["prefab"].Value);
				}
			}
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x00055564 File Offset: 0x00053764
		private void LoadHPModules(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "hpmodule")
				{
					this.m_hpmodulse.Add(xmlNode.Attributes["prefab"].Value);
				}
			}
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x000555C0 File Offset: 0x000537C0
		private void LoadFlags(XmlNode parent)
		{
			for (XmlNode xmlNode = parent.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "flag")
				{
					this.m_flags.Add(int.Parse(xmlNode.Attributes["id"].Value));
				}
			}
		}

		// Token: 0x0400099C RID: 2460
		public string m_name = string.Empty;

		// Token: 0x0400099D RID: 2461
		public string m_description = string.Empty;

		// Token: 0x0400099E RID: 2462
		public bool m_dev;

		// Token: 0x0400099F RID: 2463
		public bool m_newItem;

		// Token: 0x040009A0 RID: 2464
		public ContentPack.Category m_type;

		// Token: 0x040009A1 RID: 2465
		public List<string> m_maps = new List<string>();

		// Token: 0x040009A2 RID: 2466
		public List<string> m_campaigns = new List<string>();

		// Token: 0x040009A3 RID: 2467
		public List<string> m_ships = new List<string>();

		// Token: 0x040009A4 RID: 2468
		public List<string> m_sections = new List<string>();

		// Token: 0x040009A5 RID: 2469
		public List<string> m_hpmodulse = new List<string>();

		// Token: 0x040009A6 RID: 2470
		public List<int> m_flags = new List<int>();

		// Token: 0x02000133 RID: 307
		public enum Category
		{
			// Token: 0x040009A8 RID: 2472
			None,
			// Token: 0x040009A9 RID: 2473
			Maps,
			// Token: 0x040009AA RID: 2474
			Ships,
			// Token: 0x040009AB RID: 2475
			Campaigns = 4,
			// Token: 0x040009AC RID: 2476
			Flags = 8
		}
	}
}
