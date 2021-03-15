using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
[AddComponentMenu("Scripts/Gui/ListBox_Captains_Filler")]
public class ListBox_Captains_Filler : MonoBehaviour
{
	// Token: 0x0600008C RID: 140 RVA: 0x00004764 File Offset: 0x00002964
	private void Start()
	{
		this.m_parent = base.gameObject.GetComponent<UIScrollList>();
		DebugUtils.Assert(this.m_parent != null, "ListBox_Captains_Filler script must be attached to an UIScrollList");
		if (this.m_parent == null)
		{
			return;
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000047AC File Offset: 0x000029AC
	private void Update()
	{
	}

	// Token: 0x0400004E RID: 78
	private UIScrollList m_parent;
}
