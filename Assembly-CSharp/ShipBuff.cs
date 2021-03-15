using System;

// Token: 0x02000018 RID: 24
public abstract class ShipBuff
{
	// Token: 0x06000106 RID: 262 RVA: 0x00006F94 File Offset: 0x00005194
	protected void AssertValidate()
	{
		DebugUtils.Assert(this.Validate());
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00006FA4 File Offset: 0x000051A4
	protected virtual bool Validate()
	{
		return !string.IsNullOrEmpty(this.m_iconPath) && !string.IsNullOrEmpty(this.m_name) && !string.IsNullOrEmpty(this.m_description) && this.m_cost >= 1;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00006FF0 File Offset: 0x000051F0
	protected static string FloatVarToDescription(float f, bool isPercent, string varName)
	{
		string text = (f >= 0f) ? ((f <= 0f) ? string.Empty : "+") : "-";
		return string.Format("{0}{1}{2} {3}", new object[]
		{
			text,
			f.ToString("F2"),
			(!isPercent) ? string.Empty : "%",
			varName.Trim().ToUpper()
		});
	}

	// Token: 0x04000099 RID: 153
	public string m_iconPath;

	// Token: 0x0400009A RID: 154
	public string m_name;

	// Token: 0x0400009B RID: 155
	public string m_description;

	// Token: 0x0400009C RID: 156
	public int m_cost;
}
