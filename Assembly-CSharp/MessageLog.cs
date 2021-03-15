using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class MessageLog
{
	// Token: 0x060000F9 RID: 249 RVA: 0x00006864 File Offset: 0x00004A64
	public MessageLog(PTech.RPC rpc, GameObject guiCamera)
	{
		MessageLog.m_instance = this;
		this.Clear();
		if (guiCamera == null)
		{
			return;
		}
		this.m_guiCamera = guiCamera;
		this.m_gui = GuiUtils.CreateGui("IngameGui/MessageLog", this.m_guiCamera);
		this.m_areaTop.m_dlg = GuiUtils.CreateGui("IngameGui/TurnMessage", this.m_guiCamera);
		this.m_areaTop.m_dlg.GetComponent<UIPanel>().Dismiss();
		this.m_areaTop.m_dlg.transform.position = new Vector3(0f, 375f, 0f);
		this.m_areaTop.m_position = new Vector3(0f, 375f, 0f);
		this.m_areaMiddle.m_dlg = GuiUtils.CreateGui("IngameGui/TurnMessage", this.m_guiCamera);
		this.m_areaMiddle.m_dlg.GetComponent<UIPanel>().Dismiss();
		this.m_areaMiddle.m_dlg.transform.position = new Vector3(0f, 0f, 0f);
		this.m_areaMiddle.m_position = new Vector3(0f, 0f, 0f);
		this.m_areaBottom.m_dlg = GuiUtils.CreateGui("IngameGui/TurnMessage", this.m_guiCamera);
		this.m_areaBottom.m_dlg.GetComponent<UIPanel>().Dismiss();
		this.m_areaBottom.m_dlg.transform.position = new Vector3(0f, -375f, 0f);
		this.m_areaBottom.m_position = new Vector3(0f, -375f, 0f);
		this.SetVisible(false);
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000FA RID: 250 RVA: 0x00006A50 File Offset: 0x00004C50
	public static MessageLog instance
	{
		get
		{
			return MessageLog.m_instance;
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00006A58 File Offset: 0x00004C58
	public void Close()
	{
		UnityEngine.Object.Destroy(this.m_gui);
		if (this.m_areaTop.m_dlg)
		{
			UnityEngine.Object.Destroy(this.m_areaTop.m_dlg);
		}
		if (this.m_areaMiddle.m_dlg)
		{
			UnityEngine.Object.Destroy(this.m_areaMiddle.m_dlg);
		}
		if (this.m_areaBottom.m_dlg)
		{
			UnityEngine.Object.Destroy(this.m_areaBottom.m_dlg);
		}
		MessageLog.m_instance = null;
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00006AE8 File Offset: 0x00004CE8
	public void Clear()
	{
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00006AEC File Offset: 0x00004CEC
	public void SetVisible(bool visible)
	{
		this.m_visible = visible;
		if (!this.m_visible)
		{
			this.m_gui.SetActiveRecursively(false);
			if (this.m_areaTop.m_dlg)
			{
				this.m_areaTop.m_dlg.SetActiveRecursively(false);
			}
			if (this.m_areaMiddle.m_dlg)
			{
				this.m_areaMiddle.m_dlg.SetActiveRecursively(false);
			}
			if (this.m_areaBottom.m_dlg)
			{
				this.m_areaBottom.m_dlg.SetActiveRecursively(false);
			}
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00006B8C File Offset: 0x00004D8C
	public void Update(List<ClientPlayer> players)
	{
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00006B90 File Offset: 0x00004D90
	private void SetMessage(MessageLog.MessageArea area, MessageLog.Message msg)
	{
		if (area.m_dlg)
		{
			UnityEngine.Object.Destroy(area.m_dlg);
			area.m_dlg = null;
		}
		SpriteText spriteText = null;
		SpriteText spriteText2 = null;
		if (msg.m_prefab.Length == 0 || msg.m_prefab == "IngameGui/")
		{
			area.m_dlg = GuiUtils.CreateGui("IngameGui/ObjectiveMessage", this.m_guiCamera);
			spriteText = GuiUtils.FindChildOf(area.m_dlg.transform, "ObjectiveLabel").GetComponent<SpriteText>();
			spriteText2 = GuiUtils.FindChildOf(area.m_dlg.transform, "ObjectiveSubLabel").GetComponent<SpriteText>();
		}
		if (msg.m_prefab == "IngameGui/NewsflashMessage")
		{
			area.m_dlg = GuiUtils.CreateGui(msg.m_prefab, this.m_guiCamera);
			spriteText = GuiUtils.FindChildOf(area.m_dlg.transform, "ObjectiveLabel").GetComponent<SpriteText>();
		}
		if (msg.m_prefab == "IngameGui/ObjectiveDoneMessage")
		{
			area.m_dlg = GuiUtils.CreateGui(msg.m_prefab, this.m_guiCamera);
			spriteText = GuiUtils.FindChildOf(area.m_dlg.transform, "ObjectiveDoneLabel").GetComponent<SpriteText>();
			spriteText2 = GuiUtils.FindChildOf(area.m_dlg.transform, "ObjectiveDoneSubLabel").GetComponent<SpriteText>();
		}
		if (msg.m_prefab == "IngameGui/TurnMessage")
		{
			area.m_dlg = GuiUtils.CreateGui(msg.m_prefab, this.m_guiCamera);
			spriteText = GuiUtils.FindChildOf(area.m_dlg.transform, "TurnLabel").GetComponent<SpriteText>();
			spriteText2 = GuiUtils.FindChildOf(area.m_dlg.transform, "TurnSubLabel").GetComponent<SpriteText>();
		}
		spriteText.Text = msg.m_mainText;
		if (spriteText2)
		{
			spriteText2.Text = msg.m_subText;
		}
		area.m_dlg.GetComponent<UIPanel>().BringIn();
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00006D74 File Offset: 0x00004F74
	private void UpdateMessage(MessageLog.MessageArea area)
	{
		area.m_Timer -= Time.deltaTime;
		if (area.m_Timer >= this.m_fadeTime)
		{
			return;
		}
		if (!area.m_fading)
		{
			area.m_fading = true;
			area.m_dlg.GetComponent<UIPanel>().Dismiss();
		}
		if (area.m_Timer >= 0f)
		{
			return;
		}
		if (area.m_Queue.Count > 0)
		{
			MessageLog.Message message = area.m_Queue.Dequeue();
			area.m_Timer = message.m_displayTime + this.m_fadeTime;
			area.m_fading = false;
			if (area.m_dlg.GetComponent<UIPanel>().IsTransitioning)
			{
				foreach (EZTransition eztransition in area.m_dlg.GetComponent<UIPanel>().Transitions.list)
				{
					eztransition.StopSafe();
				}
			}
			this.SetMessage(area, message);
		}
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00006E60 File Offset: 0x00005060
	public void Update()
	{
		this.UpdateMessage(this.m_areaTop);
		this.UpdateMessage(this.m_areaMiddle);
		this.UpdateMessage(this.m_areaBottom);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00006E94 File Offset: 0x00005094
	public void ShowMessage(MessageLog.TextPosition position, string maintext, string subtext, string prefab, float displayTime)
	{
		GameObject exists = GuiUtils.FindChildOf(this.m_guiCamera, "Dialog_Briefing(Clone)");
		if (exists)
		{
			PLog.Log("Skipping showmessage");
			return;
		}
		MessageLog.Message message = new MessageLog.Message();
		message.m_mainText = Localize.instance.Translate(maintext);
		message.m_subText = Localize.instance.Translate(subtext);
		message.m_prefab = "IngameGui/" + prefab;
		message.m_displayTime = displayTime;
		if (position == MessageLog.TextPosition.Top)
		{
			this.m_areaTop.m_Queue.Enqueue(message);
		}
		if (position == MessageLog.TextPosition.Middle)
		{
			this.m_areaMiddle.m_Queue.Enqueue(message);
		}
		if (position == MessageLog.TextPosition.Bottom)
		{
			this.m_areaBottom.m_Queue.Enqueue(message);
		}
	}

	// Token: 0x04000084 RID: 132
	private GameObject m_gui;

	// Token: 0x04000085 RID: 133
	private GameObject m_guiCamera;

	// Token: 0x04000086 RID: 134
	private bool m_visible = true;

	// Token: 0x04000087 RID: 135
	private static MessageLog m_instance;

	// Token: 0x04000088 RID: 136
	private float m_fadeTime = 0.5f;

	// Token: 0x04000089 RID: 137
	private MessageLog.MessageArea m_areaTop = new MessageLog.MessageArea();

	// Token: 0x0400008A RID: 138
	private MessageLog.MessageArea m_areaMiddle = new MessageLog.MessageArea();

	// Token: 0x0400008B RID: 139
	private MessageLog.MessageArea m_areaBottom = new MessageLog.MessageArea();

	// Token: 0x02000015 RID: 21
	public enum TextPosition
	{
		// Token: 0x0400008D RID: 141
		Top,
		// Token: 0x0400008E RID: 142
		Middle,
		// Token: 0x0400008F RID: 143
		Bottom
	}

	// Token: 0x02000016 RID: 22
	public class Message
	{
		// Token: 0x04000090 RID: 144
		public string m_mainText;

		// Token: 0x04000091 RID: 145
		public string m_subText;

		// Token: 0x04000092 RID: 146
		public string m_prefab;

		// Token: 0x04000093 RID: 147
		public float m_displayTime;
	}

	// Token: 0x02000017 RID: 23
	public class MessageArea
	{
		// Token: 0x04000094 RID: 148
		public Queue<MessageLog.Message> m_Queue = new Queue<MessageLog.Message>();

		// Token: 0x04000095 RID: 149
		public float m_Timer;

		// Token: 0x04000096 RID: 150
		public GameObject m_dlg;

		// Token: 0x04000097 RID: 151
		public bool m_fading;

		// Token: 0x04000098 RID: 152
		public Vector3 m_position = default(Vector3);
	}
}
