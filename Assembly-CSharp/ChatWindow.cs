using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class ChatWindow : LogWindow
{
	// Token: 0x060000ED RID: 237 RVA: 0x00006608 File Offset: 0x00004808
	public override void Initialize(GameObject guiCam, bool startVisible, List<string> messages, LogWindow.LogWindow_ScreenAlignment alignment)
	{
		base.Initialize(guiCam, startVisible, messages, alignment);
		this.txtInput.Text = string.Empty;
		this.txtInput.AddCommitDelegate(new EZKeyboardCommitDelegate(this.OnInputCommited));
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00006648 File Offset: 0x00004848
	private void OnInputCommited(IKeyFocusable control)
	{
		UITextField uitextField = control as UITextField;
		if (uitextField == null)
		{
			return;
		}
		string text = uitextField.Text;
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		if (this.m_OnTextCommit != null)
		{
			this.m_OnTextCommit(text);
		}
		this.txtInput.Text = string.Empty;
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000066A4 File Offset: 0x000048A4
	protected override void LoadGUI()
	{
		if (this.m_gui == null)
		{
			this.m_gui = GuiUtils.CreateGui("LogDisplay/ChatWindow", this.m_guiCam);
		}
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000066D0 File Offset: 0x000048D0
	protected override void DoAdditionalValidation()
	{
		DebugUtils.Assert(this.Validate_TextInput(), "ChatWindow failed to validate label named UIScrollList named list");
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x000066E4 File Offset: 0x000048E4
	private bool Validate_TextInput()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("txtInput", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.txtInput = gameObject.GetComponent<UITextField>();
		return this.txtInput != null;
	}

	// Token: 0x0400007F RID: 127
	public ChatWindow.OnTextCommit m_OnTextCommit;

	// Token: 0x04000080 RID: 128
	private UITextField txtInput;

	// Token: 0x0200019A RID: 410
	// (Invoke) Token: 0x06000F18 RID: 3864
	public delegate void OnTextCommit(string text);
}
