using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x02000138 RID: 312
	public class FleetDef
	{
		// Token: 0x06000BEA RID: 3050 RVA: 0x000556A4 File Offset: 0x000538A4
		public FleetDef()
		{
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x000556CC File Offset: 0x000538CC
		public FleetDef(byte[] data)
		{
			this.FromArray(data);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00055704 File Offset: 0x00053904
		public void Save(BinaryWriter writer)
		{
			writer.Write(this.m_name);
			writer.Write(this.m_campaignID);
			writer.Write(this.m_value);
			writer.Write(this.m_ships.Count);
			foreach (ShipDef shipDef in this.m_ships)
			{
				shipDef.Save(writer);
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x000557A0 File Offset: 0x000539A0
		public void Load(BinaryReader reader)
		{
			this.m_name = reader.ReadString();
			this.m_campaignID = reader.ReadInt32();
			this.m_value = reader.ReadInt32();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ShipDef shipDef = new ShipDef();
				shipDef.Load(reader);
				this.m_ships.Add(shipDef);
			}
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00055804 File Offset: 0x00053A04
		public byte[] ToArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			this.Save(writer);
			return memoryStream.ToArray();
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0005582C File Offset: 0x00053A2C
		public void FromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader reader = new BinaryReader(input);
			this.Load(reader);
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x00055850 File Offset: 0x00053A50
		public bool IsValid(User user, ComponentDB cdb)
		{
			int num = 0;
			foreach (ShipDef shipDef in this.m_ships)
			{
				shipDef.IsValid(user, cdb);
				num += shipDef.m_value;
			}
			if (this.m_value != num)
			{
				PLog.LogWarning("Fleet value is invalid");
				return false;
			}
			return true;
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x000558DC File Offset: 0x00053ADC
		public void UpdateAvailability(ComponentDB cdb, List<string> ships, List<string> sections, List<string> modules)
		{
			this.m_available = true;
			foreach (ShipDef shipDef in this.m_ships)
			{
				shipDef.UpdateAvailability(cdb, ships, sections, modules);
				if (!shipDef.m_available)
				{
					this.m_available = false;
				}
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x00055960 File Offset: 0x00053B60
		public void UpdateValue()
		{
			this.m_value = 0;
			foreach (ShipDef shipDef in this.m_ships)
			{
				this.m_value += shipDef.m_value;
			}
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x000559DC File Offset: 0x00053BDC
		public FleetDef Clone()
		{
			FleetDef fleetDef = new FleetDef();
			fleetDef.m_name = this.m_name;
			fleetDef.m_campaignID = this.m_campaignID;
			fleetDef.m_value = this.m_value;
			foreach (ShipDef shipDef in this.m_ships)
			{
				fleetDef.m_ships.Add(shipDef.Clone());
			}
			return fleetDef;
		}

		// Token: 0x040009BF RID: 2495
		public string m_name = string.Empty;

		// Token: 0x040009C0 RID: 2496
		public int m_campaignID;

		// Token: 0x040009C1 RID: 2497
		public int m_value;

		// Token: 0x040009C2 RID: 2498
		public bool m_available = true;

		// Token: 0x040009C3 RID: 2499
		public List<ShipDef> m_ships = new List<ShipDef>();
	}
}
