using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002E RID: 46
public class ContextMenu
{
	// Token: 0x060001E5 RID: 485 RVA: 0x0000BD80 File Offset: 0x00009F80
	public ContextMenu(Camera guiCamera)
	{
		this.m_guiCamera = guiCamera;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000BDB8 File Offset: 0x00009FB8
	public void AddDragButton(Texture image, string tooltip, Vector3 pos, global::ContextMenu.ButtonHandler handler)
	{
		global::ContextMenu.ButtonData buttonData = new global::ContextMenu.ButtonData();
		buttonData.m_type = global::ContextMenu.ButtonType.DragButton;
		buttonData.m_image = image;
		buttonData.m_tooltip = tooltip;
		buttonData.m_pos = pos;
		buttonData.m_handler = handler;
		this.m_buttons.Add(buttonData);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000BDFC File Offset: 0x00009FFC
	public void AddClickButton(Texture image, string tooltip, Vector3 pos, bool small, global::ContextMenu.ButtonHandler handler)
	{
		global::ContextMenu.ButtonData buttonData = new global::ContextMenu.ButtonData();
		buttonData.m_type = global::ContextMenu.ButtonType.ClickButton;
		buttonData.m_image = image;
		buttonData.m_tooltip = tooltip;
		buttonData.m_pos = pos;
		buttonData.m_handler = handler;
		buttonData.m_small = small;
		this.m_buttons.Add(buttonData);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000BE48 File Offset: 0x0000A048
	public void DrawGui(Camera camera)
	{
		float num = this.m_guiCamera.orthographicSize * 2f;
		float num2 = (float)Screen.height / num;
		bool flag = global::ContextMenu.lastFrameHack != Time.frameCount;
		global::ContextMenu.lastFrameHack = Time.frameCount;
		bool flag2 = false;
		Vector2 vector = Utils.ScreenToGUIPos(Input.mousePosition);
		for (int i = 0; i < this.m_buttons.Count; i++)
		{
			global::ContextMenu.ButtonData buttonData = this.m_buttons[i];
			float num3 = ((!buttonData.m_small) ? 40f : 26.666666f) * num2;
			Vector2 vector2 = Utils.ScreenToGUIPos(camera.WorldToScreenPoint(buttonData.m_pos)) - new Vector2(num3 / 2f, num3 / 2f);
			Rect position = new Rect(vector2.x, vector2.y, num3, num3);
			if (buttonData.m_type == global::ContextMenu.ButtonType.DragButton)
			{
				GUI.DrawTexture(position, buttonData.m_image);
				if (position.Contains(vector))
				{
					this.m_mouseOver = 0f;
					GUI.Label(new Rect(vector2.x - 20f, vector2.y - 30f, 85f, 20f), buttonData.m_tooltip);
					if (Input.GetMouseButton(0))
					{
						flag2 = true;
						if (this.m_grabbed != i)
						{
							this.m_grabbed = i;
							this.m_grabPoint = vector;
						}
					}
				}
			}
			if (buttonData.m_type == global::ContextMenu.ButtonType.ClickButton)
			{
				GUI.DrawTexture(position, buttonData.m_image);
				if (position.Contains(vector))
				{
					this.m_mouseOver = 0f;
					GUI.Label(new Rect(vector2.x - 20f, vector2.y - 30f, 85f, 20f), buttonData.m_tooltip);
					if (Input.GetMouseButtonUp(0) && flag && !this.m_firstFrameHack)
					{
						buttonData.m_handler();
					}
				}
			}
		}
		if (this.m_grabbed != -1 && Vector2.Distance(this.m_grabPoint, vector) > 5f)
		{
			this.m_buttons[this.m_grabbed].m_handler();
		}
		if (!flag2)
		{
			this.m_grabbed = -1;
		}
		this.m_firstFrameHack = false;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000C0AC File Offset: 0x0000A2AC
	public void Update(float dt)
	{
		this.m_mouseOver += dt;
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000C0BC File Offset: 0x0000A2BC
	public bool IsMouseOver()
	{
		return (double)this.m_mouseOver < 0.1;
	}

	// Token: 0x0400013C RID: 316
	private const float m_buttonSize = 40f;

	// Token: 0x0400013D RID: 317
	private static int lastFrameHack;

	// Token: 0x0400013E RID: 318
	private float m_mouseOver = 10f;

	// Token: 0x0400013F RID: 319
	private int m_grabbed = -1;

	// Token: 0x04000140 RID: 320
	private Vector2 m_grabPoint;

	// Token: 0x04000141 RID: 321
	private bool m_firstFrameHack = true;

	// Token: 0x04000142 RID: 322
	private List<global::ContextMenu.ButtonData> m_buttons = new List<global::ContextMenu.ButtonData>();

	// Token: 0x04000143 RID: 323
	private Camera m_guiCamera;

	// Token: 0x0200002F RID: 47
	private enum ButtonType
	{
		// Token: 0x04000145 RID: 325
		DragButton,
		// Token: 0x04000146 RID: 326
		ClickButton,
		// Token: 0x04000147 RID: 327
		ToggleButton
	}

	// Token: 0x02000030 RID: 48
	private class ButtonData
	{
		// Token: 0x04000148 RID: 328
		public global::ContextMenu.ButtonType m_type;

		// Token: 0x04000149 RID: 329
		public Texture m_image;

		// Token: 0x0400014A RID: 330
		public string m_tooltip;

		// Token: 0x0400014B RID: 331
		public Vector3 m_pos;

		// Token: 0x0400014C RID: 332
		public global::ContextMenu.ButtonHandler m_handler;

		// Token: 0x0400014D RID: 333
		public bool m_small;
	}

	// Token: 0x020001A1 RID: 417
	// (Invoke) Token: 0x06000F34 RID: 3892
	public delegate void ButtonHandler();
}
