using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
internal class MsgBox
{
	// Token: 0x0600037A RID: 890 RVA: 0x0001C198 File Offset: 0x0001A398
	public MsgBox(GameObject guiCamera, MsgBox.Type type, string text, MsgBox.OkHandler okHandler, MsgBox.CancelHandler cancelHandler, MsgBox.YesHandler yesHandler, MsgBox.NoHandler noHandler)
	{
		this.m_type = type;
		this.m_onOk = okHandler;
		this.m_onCancel = cancelHandler;
		this.m_onYes = yesHandler;
		this.m_onNo = noHandler;
		this.m_gui = GuiUtils.CreateGui("MsgBox", guiCamera);
		this.m_gui.transform.FindChild("MsgBoxAnchor/Wnd/TextLabel").GetComponent<SpriteText>().Text = Localize.instance.Translate(text);
		this.m_okButton = this.m_gui.transform.FindChild("MsgBoxAnchor/Wnd/OkButton").GetComponent<UIButton>();
		this.m_cancelButton = this.m_gui.transform.FindChild("MsgBoxAnchor/Wnd/CancelButton").GetComponent<UIButton>();
		this.m_yesButton = this.m_gui.transform.FindChild("MsgBoxAnchor/Wnd/YesButton").GetComponent<UIButton>();
		this.m_noButton = this.m_gui.transform.FindChild("MsgBoxAnchor/Wnd/NoButton").GetComponent<UIButton>();
		this.m_okButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOk));
		this.m_cancelButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCancel));
		this.m_yesButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnYes));
		this.m_noButton.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnNo));
		this.m_okButton.gameObject.SetActiveRecursively(false);
		this.m_cancelButton.gameObject.SetActiveRecursively(false);
		this.m_yesButton.gameObject.SetActiveRecursively(false);
		this.m_noButton.gameObject.SetActiveRecursively(false);
		switch (type)
		{
		case MsgBox.Type.Ok:
			this.m_okButton.gameObject.SetActiveRecursively(true);
			break;
		case MsgBox.Type.YesNo:
			this.m_yesButton.gameObject.SetActiveRecursively(true);
			this.m_noButton.gameObject.SetActiveRecursively(true);
			break;
		case MsgBox.Type.OkCancel:
			this.m_okButton.gameObject.SetActiveRecursively(true);
			this.m_cancelButton.gameObject.SetActiveRecursively(true);
			break;
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0001C3B4 File Offset: 0x0001A5B4
	public static MsgBox CreateOkMsgBox(GameObject guiCamera, string text, MsgBox.OkHandler okHandler)
	{
		return new MsgBox(guiCamera, MsgBox.Type.Ok, text, okHandler, null, null, null);
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0001C3C4 File Offset: 0x0001A5C4
	public static MsgBox CreateTextOnlyMsgBox(GameObject guiCamera, string text)
	{
		return new MsgBox(guiCamera, MsgBox.Type.TextOnly, text, null, null, null, null);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0001C3D4 File Offset: 0x0001A5D4
	public static MsgBox CreateYesNoMsgBox(GameObject guiCamera, string text, MsgBox.YesHandler yesHandler, MsgBox.NoHandler noHandler)
	{
		return new MsgBox(guiCamera, MsgBox.Type.YesNo, text, null, null, yesHandler, noHandler);
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0001C3E4 File Offset: 0x0001A5E4
	public void Update()
	{
		if (this.m_type == MsgBox.Type.Ok && Input.GetKeyDown(KeyCode.Return))
		{
			this.Close();
			if (this.m_onOk != null)
			{
				this.m_onOk();
			}
		}
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0001C41C File Offset: 0x0001A61C
	public void Close()
	{
		UnityEngine.Object.Destroy(this.m_gui);
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0001C42C File Offset: 0x0001A62C
	public void OnCancel(IUIObject obj)
	{
		this.Close();
		if (this.m_onCancel != null)
		{
			this.m_onCancel();
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0001C44C File Offset: 0x0001A64C
	public void OnOk(IUIObject obj)
	{
		this.Close();
		if (this.m_onOk != null)
		{
			this.m_onOk();
		}
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0001C46C File Offset: 0x0001A66C
	public void OnYes(IUIObject obj)
	{
		this.Close();
		if (this.m_onYes != null)
		{
			this.m_onYes();
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0001C48C File Offset: 0x0001A68C
	public void OnNo(IUIObject obj)
	{
		this.Close();
		if (this.m_onNo != null)
		{
			this.m_onNo();
		}
	}

	// Token: 0x040002EA RID: 746
	public MsgBox.OkHandler m_onOk;

	// Token: 0x040002EB RID: 747
	public MsgBox.CancelHandler m_onCancel;

	// Token: 0x040002EC RID: 748
	public MsgBox.YesHandler m_onYes;

	// Token: 0x040002ED RID: 749
	public MsgBox.NoHandler m_onNo;

	// Token: 0x040002EE RID: 750
	private MsgBox.Type m_type;

	// Token: 0x040002EF RID: 751
	private UIButton m_okButton;

	// Token: 0x040002F0 RID: 752
	private UIButton m_cancelButton;

	// Token: 0x040002F1 RID: 753
	private UIButton m_yesButton;

	// Token: 0x040002F2 RID: 754
	private UIButton m_noButton;

	// Token: 0x040002F3 RID: 755
	private GameObject m_gui;

	// Token: 0x02000052 RID: 82
	public enum Type
	{
		// Token: 0x040002F5 RID: 757
		Ok,
		// Token: 0x040002F6 RID: 758
		YesNo,
		// Token: 0x040002F7 RID: 759
		OkCancel,
		// Token: 0x040002F8 RID: 760
		TextOnly
	}

	// Token: 0x020001A4 RID: 420
	// (Invoke) Token: 0x06000F40 RID: 3904
	public delegate void OkHandler();

	// Token: 0x020001A5 RID: 421
	// (Invoke) Token: 0x06000F44 RID: 3908
	public delegate void CancelHandler();

	// Token: 0x020001A6 RID: 422
	// (Invoke) Token: 0x06000F48 RID: 3912
	public delegate void YesHandler();

	// Token: 0x020001A7 RID: 423
	// (Invoke) Token: 0x06000F4C RID: 3916
	public delegate void NoHandler();
}
