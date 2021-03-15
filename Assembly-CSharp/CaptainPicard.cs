using System;

// Token: 0x02000020 RID: 32
public class CaptainPicard : ShipBuff, IBuff_CrewXPGain, IBuff_HullIntegrity
{
	// Token: 0x06000110 RID: 272 RVA: 0x00007078 File Offset: 0x00005278
	public CaptainPicard()
	{
		this.m_name = "PICARD";
		this.m_description = ShipBuff.FloatVarToDescription(this.m_crewXPGainPercentage, true, "crew experience gain");
		this.m_description = this.m_description + "\n" + ShipBuff.FloatVarToDescription(this.m_hullIntegretyPercentage, true, "hull integrity");
		this.m_cost = 10;
		this.m_iconPath = "Assets\\Textures\\Gui\\Buff_Icon_Captains";
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007100 File Offset: 0x00005300
	void IBuff_HullIntegrity.AppendToHullIntegrity(ref float hullIntegrity)
	{
		hullIntegrity *= this.m_hullIntegretyPercentage / 100f;
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00007114 File Offset: 0x00005314
	void IBuff_CrewXPGain.AppendToCrewXP(ref float crewXPRate)
	{
		crewXPRate += this.m_crewXPGainPercentage;
	}

	// Token: 0x0400009D RID: 157
	private float m_hullIntegretyPercentage = -1f;

	// Token: 0x0400009E RID: 158
	private float m_crewXPGainPercentage = 1.5f;
}
