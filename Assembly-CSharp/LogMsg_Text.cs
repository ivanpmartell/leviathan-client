using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
internal class LogMsg_Text : LogMsg
{
	// Token: 0x060000F8 RID: 248 RVA: 0x000067F4 File Offset: 0x000049F4
	public LogMsg_Text(GameObject guiCam, string message)
	{
		this.m_gui = GuiUtils.CreateGui("LogDisplay/LogMsg_Text", guiCam);
		this.m_listItemComponent = this.m_gui.GetComponent<UIListItem>();
		this.lblMsg = this.m_gui.transform.Find("lblMsg").gameObject.GetComponent<SpriteText>();
		message = message.Trim();
		this.lblMsg.Text = message;
	}

	// Token: 0x04000083 RID: 131
	private SpriteText lblMsg;
}
