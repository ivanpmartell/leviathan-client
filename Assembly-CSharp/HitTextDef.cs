using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
internal class HitTextDef
{
	// Token: 0x060002E9 RID: 745 RVA: 0x00016684 File Offset: 0x00014884
	public HitTextDef(HitTextDef.FontSize fontSize, Color color)
	{
		this.m_fontSize = fontSize;
		this.m_color = color;
	}

	// Token: 0x060002EA RID: 746 RVA: 0x000166BC File Offset: 0x000148BC
	public HitTextDef(HitTextDef.FontSize fontSize, Color color, string prefix, string postfix)
	{
		this.m_fontSize = fontSize;
		this.m_color = color;
		this.m_prefix = prefix;
		this.m_postfix = postfix;
	}

	// Token: 0x0400022A RID: 554
	public HitTextDef.FontSize m_fontSize;

	// Token: 0x0400022B RID: 555
	public Color m_color;

	// Token: 0x0400022C RID: 556
	public string m_prefix = string.Empty;

	// Token: 0x0400022D RID: 557
	public string m_postfix = string.Empty;

	// Token: 0x02000044 RID: 68
	public enum FontSize
	{
		// Token: 0x0400022F RID: 559
		Small,
		// Token: 0x04000230 RID: 560
		Medium,
		// Token: 0x04000231 RID: 561
		Large
	}
}
