using System;

// Token: 0x0200003B RID: 59
public class DefaultSections
{
	// Token: 0x0600025F RID: 607 RVA: 0x00011058 File Offset: 0x0000F258
	public bool IsValid()
	{
		return this.m_front.Length != 0 && this.m_mid.Length != 0 && this.m_rear.Length != 0 && this.m_top.Length != 0;
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000110B0 File Offset: 0x0000F2B0
	public string ErrorMessage()
	{
		string text = string.Empty;
		if (this.m_front.Length == 0)
		{
			text += " Front";
		}
		if (this.m_mid.Length == 0)
		{
			text += " Mid";
		}
		if (this.m_rear.Length == 0)
		{
			text += " Rear";
		}
		if (this.m_top.Length == 0)
		{
			text += " Top";
		}
		return text;
	}

	// Token: 0x040001BE RID: 446
	public string m_front = string.Empty;

	// Token: 0x040001BF RID: 447
	public string m_mid = string.Empty;

	// Token: 0x040001C0 RID: 448
	public string m_rear = string.Empty;

	// Token: 0x040001C1 RID: 449
	public string m_top = string.Empty;
}
