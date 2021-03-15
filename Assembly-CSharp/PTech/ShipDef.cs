using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PTech
{
	// Token: 0x02000156 RID: 342
	public class ShipDef
	{
		// Token: 0x06000CD0 RID: 3280 RVA: 0x0005BF2C File Offset: 0x0005A12C
		public ShipDef()
		{
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x0005BF3C File Offset: 0x0005A13C
		public ShipDef(byte[] data)
		{
			this.FromArray(data);
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0005BF54 File Offset: 0x0005A154
		public void Load(XmlNode xmlFile)
		{
			this.m_name = xmlFile.Attributes["name"].Value;
			this.m_prefab = xmlFile.Attributes["prefab"].Value;
			XmlNode node = xmlFile.SelectSingleNode("front");
			this.m_frontSection = new SectionDef();
			this.LoadSection(node, this.m_frontSection);
			XmlNode node2 = xmlFile.SelectSingleNode("mid");
			this.m_midSection = new SectionDef();
			this.LoadSection(node2, this.m_midSection);
			XmlNode node3 = xmlFile.SelectSingleNode("rear");
			this.m_rearSection = new SectionDef();
			this.LoadSection(node3, this.m_rearSection);
			XmlNode node4 = xmlFile.SelectSingleNode("top");
			this.m_topSection = new SectionDef();
			this.LoadSection(node4, this.m_topSection);
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x0005C028 File Offset: 0x0005A228
		private void LoadSection(XmlNode node, SectionDef section)
		{
			section.m_prefab = node.Attributes["prefab"].Value;
			for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType != XmlNodeType.Comment && xmlNode.Name == "module")
				{
					string value = xmlNode.Attributes["prefab"].Value;
					int battery = int.Parse(xmlNode.Attributes["battery"].Value);
					int x = int.Parse(xmlNode.Attributes["x"].Value);
					int y = int.Parse(xmlNode.Attributes["y"].Value);
					Direction direction = (Direction)int.Parse(xmlNode.Attributes["dir"].Value);
					section.m_modules.Add(new ModuleDef(value, battery, new Vector2i(x, y), direction));
				}
			}
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x0005C12C File Offset: 0x0005A32C
		private void SaveSection(XmlWriter writer, SectionDef section, string sectionName)
		{
			writer.WriteStartElement(sectionName);
			writer.WriteAttributeString("prefab", section.m_prefab);
			foreach (ModuleDef moduleDef in section.m_modules)
			{
				writer.WriteStartElement("module");
				writer.WriteAttributeString("prefab", moduleDef.m_prefab);
				writer.WriteAttributeString("battery", moduleDef.m_battery.ToString());
				writer.WriteAttributeString("x", moduleDef.m_pos.x.ToString());
				writer.WriteAttributeString("y", moduleDef.m_pos.y.ToString());
				int direction = (int)moduleDef.m_direction;
				writer.WriteAttributeString("dir", direction.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0005C230 File Offset: 0x0005A430
		public void Save(XmlWriter writer)
		{
			writer.WriteStartElement("ship");
			writer.WriteAttributeString("name", this.m_name);
			writer.WriteAttributeString("prefab", this.m_prefab);
			this.SaveSection(writer, this.m_frontSection, "front");
			this.SaveSection(writer, this.m_midSection, "mid");
			this.SaveSection(writer, this.m_rearSection, "rear");
			this.SaveSection(writer, this.m_topSection, "top");
			writer.WriteEndElement();
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0005C2B8 File Offset: 0x0005A4B8
		public void Save(BinaryWriter writer)
		{
			writer.Write(this.m_name);
			writer.Write(this.m_campaignID);
			writer.Write(this.m_prefab);
			writer.Write(this.m_value);
			this.SaveSection(writer, this.m_frontSection);
			this.SaveSection(writer, this.m_midSection);
			this.SaveSection(writer, this.m_rearSection);
			this.SaveSection(writer, this.m_topSection);
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0005C32C File Offset: 0x0005A52C
		private void SaveSection(BinaryWriter writer, SectionDef section)
		{
			writer.Write(section.m_prefab);
			writer.Write(section.m_modules.Count);
			foreach (ModuleDef moduleDef in section.m_modules)
			{
				writer.Write(moduleDef.m_prefab);
				writer.Write(moduleDef.m_battery);
				writer.Write(moduleDef.m_pos.x);
				writer.Write(moduleDef.m_pos.y);
				writer.Write((int)moduleDef.m_direction);
			}
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x0005C3F0 File Offset: 0x0005A5F0
		public void Load(BinaryReader reader)
		{
			this.m_name = reader.ReadString();
			this.m_campaignID = reader.ReadInt32();
			this.m_prefab = reader.ReadString();
			this.m_value = reader.ReadInt32();
			this.m_frontSection = new SectionDef();
			this.LoadSection(reader, this.m_frontSection);
			this.m_midSection = new SectionDef();
			this.LoadSection(reader, this.m_midSection);
			this.m_rearSection = new SectionDef();
			this.LoadSection(reader, this.m_rearSection);
			this.m_topSection = new SectionDef();
			this.LoadSection(reader, this.m_topSection);
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x0005C490 File Offset: 0x0005A690
		private void LoadSection(BinaryReader reader, SectionDef section)
		{
			section.m_prefab = reader.ReadString();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string prefab = reader.ReadString();
				int battery = reader.ReadInt32();
				Vector2i pos = new Vector2i(reader.ReadInt32(), reader.ReadInt32());
				Direction direction = (Direction)reader.ReadInt32();
				section.m_modules.Add(new ModuleDef(prefab, battery, pos, direction));
			}
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x0005C504 File Offset: 0x0005A704
		public byte[] ToArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			this.Save(writer);
			return memoryStream.ToArray();
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x0005C52C File Offset: 0x0005A72C
		public void FromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader reader = new BinaryReader(input);
			this.Load(reader);
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x0005C550 File Offset: 0x0005A750
		public bool IsValid(User user, ComponentDB cdb)
		{
			List<string> availableShips = user.GetAvailableShips(this.m_campaignID);
			List<string> availableSections = user.GetAvailableSections(this.m_campaignID);
			List<string> availableHPModules = user.GetAvailableHPModules(this.m_campaignID);
			if (!availableShips.Contains(this.m_prefab))
			{
				return false;
			}
			if (!this.IsSectionValid(this.m_frontSection, availableSections, availableHPModules))
			{
				return false;
			}
			if (!this.IsSectionValid(this.m_midSection, availableSections, availableHPModules))
			{
				return false;
			}
			if (!this.IsSectionValid(this.m_rearSection, availableSections, availableHPModules))
			{
				return false;
			}
			if (!this.IsSectionValid(this.m_topSection, availableSections, availableHPModules))
			{
				return false;
			}
			int shipValue = ShipDefUtils.GetShipValue(this, cdb);
			if (shipValue != this.m_value)
			{
				PLog.LogError(string.Concat(new object[]
				{
					"Player ",
					user.m_name,
					" uploaded ship ",
					this.m_name,
					" with value missmatch , user value ",
					this.m_value,
					"  server value ",
					shipValue
				}));
				return false;
			}
			return true;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0005C65C File Offset: 0x0005A85C
		public void UpdateAvailability(ComponentDB cdb, List<string> ships, List<string> sections, List<string> modules)
		{
			this.m_available = true;
			if (!ships.Contains(this.m_prefab))
			{
				this.m_available = false;
				return;
			}
			if (!this.IsSectionValid(this.m_frontSection, sections, modules))
			{
				this.m_available = false;
				return;
			}
			if (!this.IsSectionValid(this.m_midSection, sections, modules))
			{
				this.m_available = false;
				return;
			}
			if (!this.IsSectionValid(this.m_rearSection, sections, modules))
			{
				this.m_available = false;
				return;
			}
			if (!this.IsSectionValid(this.m_topSection, sections, modules))
			{
				this.m_available = false;
				return;
			}
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x0005C6FC File Offset: 0x0005A8FC
		private bool IsSectionValid(SectionDef section, List<string> sections, List<string> modules)
		{
			if (!sections.Contains(section.m_prefab))
			{
				PLog.LogWarning(" missing section " + section.m_prefab);
				return false;
			}
			foreach (ModuleDef moduleDef in section.m_modules)
			{
				if (!modules.Contains(moduleDef.m_prefab))
				{
					PLog.LogWarning(" missing module " + moduleDef.m_prefab);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x0005C7B4 File Offset: 0x0005A9B4
		public int NumberOfHardpoints()
		{
			int num = 0;
			num += this.m_frontSection.m_modules.Count;
			num += this.m_midSection.m_modules.Count;
			num += this.m_rearSection.m_modules.Count;
			return num + this.m_topSection.m_modules.Count;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x0005C810 File Offset: 0x0005AA10
		public List<string> GetHardpointNames()
		{
			List<string> list = new List<string>();
			list.AddRange(this.m_frontSection.GetHardpointNames());
			list.AddRange(this.m_midSection.GetHardpointNames());
			list.AddRange(this.m_rearSection.GetHardpointNames());
			list.AddRange(this.m_topSection.GetHardpointNames());
			return list;
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x0005C868 File Offset: 0x0005AA68
		public ShipDef Clone()
		{
			byte[] data = this.ToArray();
			return new ShipDef(data)
			{
				m_available = this.m_available
			};
		}

		// Token: 0x04000A9A RID: 2714
		public string m_name;

		// Token: 0x04000A9B RID: 2715
		public int m_campaignID;

		// Token: 0x04000A9C RID: 2716
		public string m_prefab;

		// Token: 0x04000A9D RID: 2717
		public int m_value;

		// Token: 0x04000A9E RID: 2718
		public bool m_available = true;

		// Token: 0x04000A9F RID: 2719
		public SectionDef m_frontSection;

		// Token: 0x04000AA0 RID: 2720
		public SectionDef m_midSection;

		// Token: 0x04000AA1 RID: 2721
		public SectionDef m_rearSection;

		// Token: 0x04000AA2 RID: 2722
		public SectionDef m_topSection;
	}
}
