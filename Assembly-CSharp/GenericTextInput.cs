using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class GenericTextInput : MonoBehaviour
{
	// Token: 0x0600006C RID: 108 RVA: 0x00004030 File Offset: 0x00002230
	public void Initialize(string title, string btn1text, string btn2text, string textfieldText, GenericTextInput.InputTextCancel cancel, GenericTextInput.InputTextCommit commit)
	{
		this.ValidateComponents();
		this.m_onCancel = cancel;
		this.m_onCommit = commit;
		this.lblTitle.Text = Localize.instance.Translate(title);
		this.btn1.Text = Localize.instance.Translate(btn1text);
		this.btn2.Text = Localize.instance.Translate(btn2text);
		this.txtInput.Text = textfieldText;
		this.txtInput.allowClickCaretPlacement = false;
		if (!string.IsNullOrEmpty(textfieldText.Trim()))
		{
			this.txtInput.AddInputDelegate(new EZInputDelegate(this.TextFieldClicked));
		}
		this.txtInput.AddValueChangedDelegate(new EZValueChangedDelegate(this.InputChanged));
		this.txtInput.SetCommitDelegate(new EZKeyboardCommitDelegate(this.OnTextCommit));
		this.btn1.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCancel));
		this.btn2.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnOk));
		this.InputChanged(null);
		UIManager.instance.FocusObject = this.txtInput;
	}

	// Token: 0x0600006D RID: 109 RVA: 0x0000414C File Offset: 0x0000234C
	private void OnTextCommit(IKeyFocusable control)
	{
		if (this.m_onCommit != null)
		{
			this.m_onCommit(control.Content);
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x0000416C File Offset: 0x0000236C
	private void OnOk(IUIObject obj)
	{
		if (this.m_onCommit != null)
		{
			this.m_onCommit(this.txtInput.Text);
		}
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00004190 File Offset: 0x00002390
	private void OnCancel(IUIObject obj)
	{
		if (this.m_onCancel != null)
		{
			this.m_onCancel();
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x000041A8 File Offset: 0x000023A8
	private void InputChanged(IUIObject obj)
	{
		string newValue = string.Empty;
		if (!this.m_allowEmptyInput && string.IsNullOrEmpty(this.txtInput.Text.Trim()))
		{
			this.btn2.controlIsEnabled = false;
		}
		else
		{
			this.btn2.controlIsEnabled = true;
			newValue = this.txtInput.Text;
		}
		if (this.m_onInputChanged != null)
		{
			this.m_onInputChanged(newValue);
		}
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00004220 File Offset: 0x00002420
	private void TextFieldClicked(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			this.txtInput.Text = string.Empty;
			this.txtInput.RemoveInputDelegate(new EZInputDelegate(this.TextFieldClicked));
		}
	}

	// Token: 0x06000072 RID: 114 RVA: 0x0000426C File Offset: 0x0000246C
	public void Hide()
	{
		foreach (Renderer renderer in base.gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = false;
		}
	}

	// Token: 0x06000073 RID: 115 RVA: 0x000042A4 File Offset: 0x000024A4
	public void Show()
	{
		foreach (Renderer renderer in base.gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = true;
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000042DC File Offset: 0x000024DC
	private void ValidateComponents()
	{
		if (this.IsValid)
		{
			return;
		}
		DebugUtils.Assert(this.Validate_lblTitle(), "GenericTextInput failed to validate label named lblTitle !");
		DebugUtils.Assert(this.Validate_btn1(), "GenericTextInput failed to validate button named btn1 !");
		DebugUtils.Assert(this.Validate_btn2(), "GenericTextInput failed to validate button named btn2 !");
		DebugUtils.Assert(this.Validate_txtInput(), "GenericTextInput failed to validate textfield named txtInput !");
		this.IsValid = true;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x0000433C File Offset: 0x0000253C
	private bool Validate_lblTitle()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("lblTitle", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.lblTitle = gameObject.GetComponent<SpriteText>();
		return this.lblTitle != null;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00004384 File Offset: 0x00002584
	private bool Validate_btn1()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("ButtonsPanel/btn1", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btn1 = gameObject.GetComponent<UIButton>();
		return this.btn1 != null;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000043CC File Offset: 0x000025CC
	private bool Validate_btn2()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("ButtonsPanel/btn2", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btn2 = gameObject.GetComponent<UIButton>();
		return this.btn2 != null;
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004414 File Offset: 0x00002614
	private bool Validate_txtInput()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("txtInput", out gameObject))
		{
			return false;
		}
		this.txtInput = gameObject.GetComponent<UITextField>();
		return this.txtInput != null;
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004450 File Offset: 0x00002650
	private bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = base.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x0600007A RID: 122 RVA: 0x00004484 File Offset: 0x00002684
	// (set) Token: 0x0600007B RID: 123 RVA: 0x0000448C File Offset: 0x0000268C
	public bool AllowEmptyInput
	{
		get
		{
			return this.m_allowEmptyInput;
		}
		set
		{
			this.m_allowEmptyInput = value;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x0600007C RID: 124 RVA: 0x00004498 File Offset: 0x00002698
	// (set) Token: 0x0600007D RID: 125 RVA: 0x000044A0 File Offset: 0x000026A0
	public UnityEngine.Object AdditionalData
	{
		get
		{
			return this.m_AdditionalData;
		}
		set
		{
			this.m_AdditionalData = value;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600007E RID: 126 RVA: 0x000044AC File Offset: 0x000026AC
	// (set) Token: 0x0600007F RID: 127 RVA: 0x000044C0 File Offset: 0x000026C0
	public string Text
	{
		get
		{
			return this.txtInput.GetComponent<UITextField>().Text;
		}
		set
		{
			this.txtInput.GetComponent<UITextField>().Text = value;
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000080 RID: 128 RVA: 0x000044D4 File Offset: 0x000026D4
	// (set) Token: 0x06000081 RID: 129 RVA: 0x000044DC File Offset: 0x000026DC
	public bool IsValid
	{
		get
		{
			return this.isValid;
		}
		private set
		{
			this.isValid = value;
		}
	}

	// Token: 0x04000038 RID: 56
	private SpriteText lblTitle;

	// Token: 0x04000039 RID: 57
	private UIButton btn1;

	// Token: 0x0400003A RID: 58
	private UIButton btn2;

	// Token: 0x0400003B RID: 59
	private UITextField txtInput;

	// Token: 0x0400003C RID: 60
	private UnityEngine.Object m_AdditionalData;

	// Token: 0x0400003D RID: 61
	private bool isValid;

	// Token: 0x0400003E RID: 62
	private bool m_allowEmptyInput;

	// Token: 0x0400003F RID: 63
	private GenericTextInput.InputTextChanged m_onInputChanged;

	// Token: 0x04000040 RID: 64
	private GenericTextInput.InputTextCommit m_onCommit;

	// Token: 0x04000041 RID: 65
	private GenericTextInput.InputTextCancel m_onCancel;

	// Token: 0x02000196 RID: 406
	// (Invoke) Token: 0x06000F08 RID: 3848
	private delegate void InputTextChanged(string newValue);

	// Token: 0x02000197 RID: 407
	// (Invoke) Token: 0x06000F0C RID: 3852
	public delegate void InputTextCommit(string text);

	// Token: 0x02000198 RID: 408
	// (Invoke) Token: 0x06000F10 RID: 3856
	public delegate void InputTextCancel();
}
