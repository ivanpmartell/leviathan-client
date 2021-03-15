using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
internal abstract class LogMsg
{
	// Token: 0x060000F3 RID: 243 RVA: 0x00006734 File Offset: 0x00004934
	public virtual void Remove()
	{
		if (this.m_gui != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_gui);
		}
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00006754 File Offset: 0x00004954
	public virtual void Hide()
	{
		if (this.m_gui != null)
		{
			this.m_gui.SetActiveRecursively(false);
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00006774 File Offset: 0x00004974
	public virtual void Show()
	{
		if (this.m_gui != null)
		{
			this.m_gui.SetActiveRecursively(true);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00006794 File Offset: 0x00004994
	public float Height()
	{
		if (this.m_listItemComponent == null)
		{
			return 0f;
		}
		return this.m_listItemComponent.height;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x000067C4 File Offset: 0x000049C4
	public float Width()
	{
		if (this.m_listItemComponent == null)
		{
			return 0f;
		}
		return this.m_listItemComponent.width;
	}

	// Token: 0x04000081 RID: 129
	public UIListItem m_listItemComponent;

	// Token: 0x04000082 RID: 130
	protected GameObject m_gui;
}
