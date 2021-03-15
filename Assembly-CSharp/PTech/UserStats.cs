using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PTech
{
	// Token: 0x0200015F RID: 351
	public class UserStats
	{
		// Token: 0x06000D32 RID: 3378 RVA: 0x0005E428 File Offset: 0x0005C628
		public string SaveToXml()
		{
			StringWriterWithEncoding stringWriterWithEncoding = new StringWriterWithEncoding(Encoding.UTF8);
			XmlWriter xmlWriter = XmlWriter.Create(stringWriterWithEncoding, new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace,
				Encoding = Encoding.UTF8
			});
			xmlWriter.WriteStartElement("root");
			xmlWriter.WriteElementString("lastLogin", this.m_lastLoginTime.ToString("yyyy-MM-d HH:mm"));
			xmlWriter.WriteElementString("pcLogins", this.m_pcLogins.ToString());
			xmlWriter.WriteElementString("iosLogins", this.m_iosLogins.ToString());
			xmlWriter.WriteElementString("androidLogins", this.m_androidLogins.ToString());
			xmlWriter.WriteElementString("osxLogins", this.m_osxLogins.ToString());
			xmlWriter.WriteElementString("otherLogins", this.m_otherLogins.ToString());
			xmlWriter.WriteElementString("vsGamesWon", this.m_vsGamesWon.ToString());
			xmlWriter.WriteElementString("vsPointsWon", this.m_vsPointsWon.ToString());
			xmlWriter.WriteElementString("vsAssWon", this.m_vsAssWon.ToString());
			xmlWriter.WriteElementString("vsGamesLost", this.m_vsGamesLost.ToString());
			xmlWriter.WriteElementString("vsPointsLost", this.m_vsPointsLost.ToString());
			xmlWriter.WriteElementString("vsAssLost", this.m_vsAssLost.ToString());
			xmlWriter.WriteElementString("vsTotalDamage", this.m_vsTotalDamage.ToString());
			xmlWriter.WriteElementString("vsTotalFriendlyDamage", this.m_vsTotalFriendlyDamage.ToString());
			xmlWriter.WriteElementString("vsShipsSunk", this.m_vsShipsSunk.ToString());
			xmlWriter.WriteElementString("totalPlayTime", this.m_totalPlayTime.ToString());
			xmlWriter.WriteElementString("totalPlanningTime", this.m_totalPlanningTime.ToString());
			xmlWriter.WriteElementString("totalShipyardTime", this.m_totalShipyardTime.ToString());
			xmlWriter.WriteStartElement("vsModuleStats");
			foreach (KeyValuePair<string, UserStats.ModuleStat> keyValuePair in this.m_vsModuleStats)
			{
				xmlWriter.WriteStartElement("module");
				xmlWriter.WriteAttributeString("name", keyValuePair.Key);
				xmlWriter.WriteAttributeString("uses", keyValuePair.Value.m_uses.ToString());
				xmlWriter.WriteAttributeString("damage", keyValuePair.Value.m_damage.ToString());
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("vsShipUsage");
			foreach (KeyValuePair<string, int> keyValuePair2 in this.m_vsShipUsage)
			{
				xmlWriter.WriteElementString(keyValuePair2.Key, keyValuePair2.Value.ToString());
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("achievements");
			foreach (KeyValuePair<int, long> keyValuePair3 in this.m_achievements)
			{
				xmlWriter.WriteStartElement("achievement");
				xmlWriter.WriteAttributeString("id", keyValuePair3.Key.ToString());
				xmlWriter.WriteAttributeString("date", keyValuePair3.Value.ToString());
				xmlWriter.WriteAttributeString("TextDate", DateTime.FromBinary(keyValuePair3.Value).ToString("yyyy-MM-d HH:mm"));
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.Flush();
			return stringWriterWithEncoding.ToString();
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x0005E838 File Offset: 0x0005CA38
		public void LoadFromXml(string str)
		{
			TextReader reader = new StringReader(str);
			XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings
			{
				IgnoreComments = true
			});
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(xmlReader);
			for (XmlNode xmlNode = xmlDocument.FirstChild.NextSibling.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.Name == "pcLogins")
				{
					this.m_pcLogins = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "iosLogins")
				{
					this.m_iosLogins = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "androidLogins")
				{
					this.m_androidLogins = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "osxLogins")
				{
					this.m_osxLogins = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "otherLogins")
				{
					this.m_otherLogins = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsGamesWon")
				{
					this.m_vsGamesWon = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsPointsWon")
				{
					this.m_vsPointsLost = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsAssWon")
				{
					this.m_vsAssWon = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsGamesLost")
				{
					this.m_vsGamesLost = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsPointsLost")
				{
					this.m_vsPointsLost = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsAssLost")
				{
					this.m_vsAssLost = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsTotalDamage")
				{
					this.m_vsTotalDamage = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsTotalFriendlyDamage")
				{
					this.m_vsTotalFriendlyDamage = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsShipsSunk")
				{
					this.m_vsShipsSunk = int.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "totalPlayTime")
				{
					this.m_totalPlayTime = long.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "totalPlanningTime")
				{
					this.m_totalPlanningTime = long.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "totalShipyardTime")
				{
					this.m_totalShipyardTime = long.Parse(xmlNode.FirstChild.Value);
				}
				if (xmlNode.Name == "vsModuleStats")
				{
					for (XmlElement xmlElement = xmlNode.FirstChild as XmlElement; xmlElement != null; xmlElement = (xmlElement.NextSibling as XmlElement))
					{
						string attribute = xmlElement.GetAttribute("name");
						UserStats.ModuleStat moduleStat = new UserStats.ModuleStat();
						moduleStat.m_uses = int.Parse(xmlElement.GetAttribute("uses"));
						moduleStat.m_damage = int.Parse(xmlElement.GetAttribute("damage"));
						this.m_vsModuleStats.Add(attribute, moduleStat);
					}
				}
				if (xmlNode.Name == "vsShipUsage")
				{
					for (XmlNode xmlNode2 = xmlNode.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
					{
						string name = xmlNode2.Name;
						int value = int.Parse(xmlNode2.FirstChild.Value);
						this.m_vsShipUsage.Add(name, value);
					}
				}
				if (xmlNode.Name == "achievements")
				{
					for (XmlElement xmlElement2 = xmlNode.FirstChild as XmlElement; xmlElement2 != null; xmlElement2 = (xmlElement2.NextSibling as XmlElement))
					{
						int key = int.Parse(xmlElement2.GetAttribute("id"));
						long value2 = long.Parse(xmlElement2.GetAttribute("date"));
						this.m_achievements.Add(key, value2);
					}
				}
			}
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x0005ECF8 File Offset: 0x0005CEF8
		public byte[] ToArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_lastLoginTime.ToBinary());
			binaryWriter.Write(this.m_pcLogins);
			binaryWriter.Write(this.m_iosLogins);
			binaryWriter.Write(this.m_androidLogins);
			binaryWriter.Write(this.m_osxLogins);
			binaryWriter.Write(this.m_otherLogins);
			binaryWriter.Write(this.m_totalPlayTime);
			binaryWriter.Write(this.m_totalPlanningTime);
			binaryWriter.Write(this.m_totalShipyardTime);
			binaryWriter.Write(this.m_vsGamesLost);
			binaryWriter.Write(this.m_vsPointsLost);
			binaryWriter.Write(this.m_vsAssLost);
			binaryWriter.Write(this.m_vsGamesWon);
			binaryWriter.Write(this.m_vsPointsWon);
			binaryWriter.Write(this.m_vsAssWon);
			binaryWriter.Write(this.m_vsShipsSunk);
			binaryWriter.Write(this.m_vsTotalDamage);
			binaryWriter.Write(this.m_vsTotalFriendlyDamage);
			binaryWriter.Write(this.m_vsModuleStats.Count);
			foreach (KeyValuePair<string, UserStats.ModuleStat> keyValuePair in this.m_vsModuleStats)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.m_damage);
				binaryWriter.Write(keyValuePair.Value.m_uses);
			}
			binaryWriter.Write(this.m_vsShipUsage.Count);
			foreach (KeyValuePair<string, int> keyValuePair2 in this.m_vsShipUsage)
			{
				binaryWriter.Write(keyValuePair2.Key);
				binaryWriter.Write(keyValuePair2.Value);
			}
			binaryWriter.Write(this.m_achievements.Count);
			foreach (KeyValuePair<int, long> keyValuePair3 in this.m_achievements)
			{
				binaryWriter.Write((short)keyValuePair3.Key);
				binaryWriter.Write(keyValuePair3.Value);
			}
			return memoryStream.ToArray();
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x0005EF84 File Offset: 0x0005D184
		public void FromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			this.m_lastLoginTime = DateTime.FromBinary(binaryReader.ReadInt64());
			this.m_pcLogins = binaryReader.ReadInt32();
			this.m_iosLogins = binaryReader.ReadInt32();
			this.m_androidLogins = binaryReader.ReadInt32();
			this.m_osxLogins = binaryReader.ReadInt32();
			this.m_otherLogins = binaryReader.ReadInt32();
			this.m_totalPlayTime = binaryReader.ReadInt64();
			this.m_totalPlanningTime = binaryReader.ReadInt64();
			this.m_totalShipyardTime = binaryReader.ReadInt64();
			this.m_vsGamesLost = binaryReader.ReadInt32();
			this.m_vsPointsLost = binaryReader.ReadInt32();
			this.m_vsAssLost = binaryReader.ReadInt32();
			this.m_vsGamesWon = binaryReader.ReadInt32();
			this.m_vsPointsWon = binaryReader.ReadInt32();
			this.m_vsAssWon = binaryReader.ReadInt32();
			this.m_vsShipsSunk = binaryReader.ReadInt32();
			this.m_vsTotalDamage = binaryReader.ReadInt32();
			this.m_vsTotalFriendlyDamage = binaryReader.ReadInt32();
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string key = binaryReader.ReadString();
				UserStats.ModuleStat moduleStat = new UserStats.ModuleStat();
				moduleStat.m_damage = binaryReader.ReadInt32();
				moduleStat.m_uses = binaryReader.ReadInt32();
				this.m_vsModuleStats.Add(key, moduleStat);
			}
			int num2 = binaryReader.ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				string key2 = binaryReader.ReadString();
				int value = binaryReader.ReadInt32();
				this.m_vsShipUsage.Add(key2, value);
			}
			int num3 = binaryReader.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				int key3 = (int)binaryReader.ReadInt16();
				long value2 = binaryReader.ReadInt64();
				this.m_achievements.Add(key3, value2);
			}
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x0005F14C File Offset: 0x0005D34C
		public void AddModuleUsage(string module, int uses)
		{
			UserStats.ModuleStat moduleStat;
			if (this.m_vsModuleStats.TryGetValue(module, out moduleStat))
			{
				moduleStat.m_uses += uses;
			}
			else
			{
				moduleStat = new UserStats.ModuleStat();
				moduleStat.m_uses = uses;
				this.m_vsModuleStats.Add(module, moduleStat);
			}
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0005F19C File Offset: 0x0005D39C
		public void AddModuleDamage(string module, int damage)
		{
			UserStats.ModuleStat moduleStat;
			if (this.m_vsModuleStats.TryGetValue(module, out moduleStat))
			{
				moduleStat.m_damage += damage;
			}
			else
			{
				moduleStat = new UserStats.ModuleStat();
				moduleStat.m_damage = damage;
				this.m_vsModuleStats.Add(module, moduleStat);
			}
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x0005F1EC File Offset: 0x0005D3EC
		public void AddModuleDamages(Dictionary<string, int> damages)
		{
			foreach (KeyValuePair<string, int> keyValuePair in damages)
			{
				this.AddModuleDamage(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0005F25C File Offset: 0x0005D45C
		public void AddShipUsage(string module, int newUses)
		{
			int num;
			if (this.m_vsShipUsage.TryGetValue(module, out num))
			{
				Dictionary<string, int> vsShipUsage;
				Dictionary<string, int> dictionary = vsShipUsage = this.m_vsShipUsage;
				int num2 = vsShipUsage[module];
				dictionary[module] = num2 + num;
			}
			else
			{
				this.m_vsShipUsage.Add(module, newUses);
			}
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x0005F2AC File Offset: 0x0005D4AC
		public void AddLogin(PlatformType platform)
		{
			this.m_lastLoginTime = DateTime.Now;
			switch (platform)
			{
			case PlatformType.WindowsPC:
				this.m_pcLogins++;
				break;
			case PlatformType.Ios:
				this.m_iosLogins++;
				break;
			case PlatformType.Android:
				this.m_androidLogins++;
				break;
			case PlatformType.Osx:
				this.m_osxLogins++;
				break;
			case PlatformType.Other:
				this.m_otherLogins++;
				break;
			}
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x0005F348 File Offset: 0x0005D548
		public bool UnlockAchievement(int id)
		{
			if (this.m_achievements.ContainsKey(id))
			{
				return false;
			}
			DateTime now = DateTime.Now;
			this.m_achievements.Add(id, now.ToBinary());
			return true;
		}

		// Token: 0x04000AD7 RID: 2775
		public DateTime m_lastLoginTime = default(DateTime);

		// Token: 0x04000AD8 RID: 2776
		public int m_pcLogins;

		// Token: 0x04000AD9 RID: 2777
		public int m_iosLogins;

		// Token: 0x04000ADA RID: 2778
		public int m_androidLogins;

		// Token: 0x04000ADB RID: 2779
		public int m_osxLogins;

		// Token: 0x04000ADC RID: 2780
		public int m_otherLogins;

		// Token: 0x04000ADD RID: 2781
		public int m_vsGamesWon;

		// Token: 0x04000ADE RID: 2782
		public int m_vsPointsWon;

		// Token: 0x04000ADF RID: 2783
		public int m_vsAssWon;

		// Token: 0x04000AE0 RID: 2784
		public int m_vsGamesLost;

		// Token: 0x04000AE1 RID: 2785
		public int m_vsPointsLost;

		// Token: 0x04000AE2 RID: 2786
		public int m_vsAssLost;

		// Token: 0x04000AE3 RID: 2787
		public int m_vsTotalDamage;

		// Token: 0x04000AE4 RID: 2788
		public int m_vsTotalFriendlyDamage;

		// Token: 0x04000AE5 RID: 2789
		public int m_vsShipsSunk;

		// Token: 0x04000AE6 RID: 2790
		public Dictionary<string, UserStats.ModuleStat> m_vsModuleStats = new Dictionary<string, UserStats.ModuleStat>();

		// Token: 0x04000AE7 RID: 2791
		public Dictionary<string, int> m_vsShipUsage = new Dictionary<string, int>();

		// Token: 0x04000AE8 RID: 2792
		public long m_totalPlayTime;

		// Token: 0x04000AE9 RID: 2793
		public long m_totalPlanningTime;

		// Token: 0x04000AEA RID: 2794
		public long m_totalShipyardTime;

		// Token: 0x04000AEB RID: 2795
		public Dictionary<int, long> m_achievements = new Dictionary<int, long>();

		// Token: 0x02000160 RID: 352
		public class ModuleStat
		{
			// Token: 0x04000AEC RID: 2796
			public int m_uses;

			// Token: 0x04000AED RID: 2797
			public int m_damage;
		}
	}
}
