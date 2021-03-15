using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class LogWindow
{
	// Token: 0x060000C5 RID: 197 RVA: 0x00005AE4 File Offset: 0x00003CE4
	public void Close()
	{
		if (this.m_gui != null)
		{
			foreach (SpriteText spriteText in this.m_gui.GetComponentsInChildren<SpriteText>())
			{
				spriteText.Delete();
			}
			this.m_gui.SetActiveRecursively(false);
			UnityEngine.Object.DestroyImmediate(this.m_gui);
		}
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00005B44 File Offset: 0x00003D44
	public virtual void Initialize(GameObject guiCam, bool startVisible, List<string> messages, LogWindow.LogWindow_ScreenAlignment alignment)
	{
		this.m_guiCam = guiCam;
		this.LoadGUI();
		this.ValidateComponents();
		this.m_newMessageIcon.SetOnOff(false);
		EZTransition transition = this.m_interactivePanel.GetTransition(0);
		EZTransition transition2 = this.m_interactivePanel.GetTransition(2);
		if (alignment == LogWindow.LogWindow_ScreenAlignment.Left)
		{
			this.SetToLeftOfScreen();
		}
		else if (alignment == LogWindow.LogWindow_ScreenAlignment.Right)
		{
			this.SetToRightOfScreen();
		}
		else
		{
			this.m_definedRevealedPos = transition.animParams[0].vec;
			this.m_definedHiddenPos = transition2.animParams[0].vec;
		}
		transition.AddTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.RevealDone));
		transition2.AddTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.DismissDone));
		this.m_interactivePanel.SetDragDropDelegate(null);
		this.m_interactivePanel.SetDragDropInternalDelegate(null);
		this.m_interactivePanel.SetValueChangedDelegate(null);
		this.m_interactivePanel.SetInputDelegate(null);
		this.m_interactivePanel.controlIsEnabled = false;
		this.btnExpand.AddInputDelegate(new EZInputDelegate(this.OnExpand));
		this.m_fillPercentNeededBeforeScrollEnabled = Mathf.Clamp01(this.m_fillPercentNeededBeforeScrollEnabled);
		this.m_totalHeightOfItemsInList = ((!this.m_UIList.spacingAtEnds) ? 0f : this.m_UIList.itemSpacing);
		this.m_latestEntry = null;
		if (messages != null && messages.Count > 0)
		{
			messages.ForEach(delegate(string m)
			{
				this.AddText(m);
			});
		}
		else
		{
			this.UpdateSlider();
		}
		if (startVisible)
		{
			this.QuickSetRevealed();
		}
		else
		{
			this.QuickSetHidden();
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00005CD8 File Offset: 0x00003ED8
	public void SetToRightOfScreen()
	{
		this.SetToRightOfScreen(0f, -1f);
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00005CEC File Offset: 0x00003EEC
	public void SetToRightOfScreen(float yFromCenter, float z)
	{
		if (this.m_guiCam.camera == null)
		{
			PLog.LogError("LogWindow could not be set to the right of screen, invalid camera !");
			return;
		}
		float pixelWidth = this.m_guiCam.camera.pixelWidth;
		float num = this.m_guiCam.camera.pixelHeight / 2f;
		Vector3 vector = new Vector3(pixelWidth + this.Width / 2f, num + yFromCenter, z);
		vector = this.m_guiCam.camera.ScreenToWorldPoint(vector);
		Vector3 visiblePos = vector - new Vector3(this.Width, 0f, 0f);
		this.SetVisiblePos(visiblePos);
		this.SetHiddenPos(vector);
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00005D98 File Offset: 0x00003F98
	public void SetToLeftOfScreen()
	{
		this.SetToLeftOfScreen(0f, -1f);
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00005DAC File Offset: 0x00003FAC
	public void SetToLeftOfScreen(float yFromCenter, float z)
	{
		if (this.m_guiCam.camera == null)
		{
			PLog.LogError("LogWindow could not be set to the left of screen, invalid camera !");
			return;
		}
		float num = 0f;
		float num2 = this.m_guiCam.camera.pixelHeight / 2f;
		Vector3 vector = new Vector3(num - this.Width / 2f, num2 + yFromCenter, z);
		vector = this.m_guiCam.camera.ScreenToWorldPoint(vector);
		Vector3 visiblePos = vector + new Vector3(this.Width, 0f, 0f);
		this.SetVisiblePos(visiblePos);
		this.SetHiddenPos(vector);
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00005E4C File Offset: 0x0000404C
	public void SetVisiblePos(Vector3 screenspacePos)
	{
		DebugUtils.Assert(this.m_isValid, "Can't call SetVisiblePos, need to call Initialize() first !");
		EZTransition transition = this.m_interactivePanel.GetTransition(0);
		DebugUtils.Assert(transition != null, "Failed to find transition[0] !");
		transition.animParams[0].vec = screenspacePos;
		this.m_definedRevealedPos = screenspacePos;
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00005EA0 File Offset: 0x000040A0
	public void SetHiddenPos(Vector3 screenspacePos)
	{
		DebugUtils.Assert(this.m_isValid, "Can't call SetHiddenPos, need to call Initialize() first !");
		EZTransition transition = this.m_interactivePanel.GetTransition(2);
		DebugUtils.Assert(transition != null, "Failed to find transition[2] !");
		transition.animParams[0].vec = screenspacePos;
		this.m_definedHiddenPos = screenspacePos;
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00005EF4 File Offset: 0x000040F4
	public virtual void AddText(string text)
	{
		if (this.m_isHidden)
		{
			this.m_newMessageIcon.SetOnOff(true);
		}
		this.CreateTextItem(text);
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00005F14 File Offset: 0x00004114
	protected virtual void CreateTextItem(string text)
	{
		LogMsg logMsg = new LogMsg_Text(this.m_guiCam, text);
		this.m_totalHeightOfItemsInList += logMsg.Height();
		this.m_totalHeightOfItemsInList += this.m_UIList.itemSpacing;
		this.m_latestEntry = logMsg.m_listItemComponent;
		this.m_UIList.AddItem(this.m_latestEntry);
		this.UpdateSlider();
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00005F7C File Offset: 0x0000417C
	protected virtual void UpdateSlider()
	{
		bool flag = this.m_totalHeightOfItemsInList >= this.m_fillPercentNeededBeforeScrollEnabled * (this.m_UIList.viewableArea.y - this.m_UIList.extraEndSpacing);
		this.m_slider.enabled = flag;
		UIScrollKnob knob = this.m_slider.GetKnob();
		if (knob != null)
		{
			knob.controlIsEnabled = flag;
			knob.enabled = true;
		}
		if (this.m_autoSlideToFocusLastItem && flag && this.m_latestEntry != null)
		{
			this.m_UIList.ScrollToItem(this.m_latestEntry, 0f);
		}
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00006024 File Offset: 0x00004224
	protected virtual void LoadGUI()
	{
		if (this.m_gui == null)
		{
			this.m_gui = GuiUtils.CreateGui("LogDisplay/LogWindow", this.m_guiCam);
		}
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00006050 File Offset: 0x00004250
	public void TurnOffRenderers()
	{
		if (this.m_gui != null)
		{
			this.m_gui.SetActiveRecursively(false);
		}
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00006070 File Offset: 0x00004270
	public void TurnOnRenderers()
	{
		if (this.m_gui != null)
		{
			this.m_gui.SetActiveRecursively(true);
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00006090 File Offset: 0x00004290
	public void SetActiveRecursively(bool p)
	{
		if (!p && this.m_gui == null)
		{
			return;
		}
		if (p && this.m_gui == null)
		{
			this.LoadGUI();
		}
		this.m_gui.SetActiveRecursively(p);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x000060E0 File Offset: 0x000042E0
	public void Hide()
	{
		this.btnExpand.controlIsEnabled = false;
		this.m_isRevealed = false;
		this.m_interactivePanel.Hide();
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00006100 File Offset: 0x00004300
	public void Reveal()
	{
		this.btnExpand.controlIsEnabled = false;
		this.m_isHidden = false;
		this.m_interactivePanel.Reveal();
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00006120 File Offset: 0x00004320
	private void OnExpand(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP || ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
		{
			if (this.m_isHidden)
			{
				this.Reveal();
			}
			else if (this.m_isRevealed)
			{
				this.Hide();
			}
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x0000616C File Offset: 0x0000436C
	private string DEBUG_CurrentState()
	{
		return "We are now " + ((!this.m_isHidden) ? ((!this.m_isRevealed) ? "In transition" : "Revealed") : "Hidden");
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x000061A8 File Offset: 0x000043A8
	private void RevealDone(EZTransition transition)
	{
		this.btnExpand.controlIsEnabled = true;
		this.m_isRevealed = true;
		this.m_newMessageIcon.SetOnOff(false);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000061CC File Offset: 0x000043CC
	private void DismissDone(EZTransition transition)
	{
		this.btnExpand.controlIsEnabled = true;
		this.m_isHidden = true;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x000061E4 File Offset: 0x000043E4
	public void QuickSetHidden()
	{
		this.m_interactivePanel.StopAllCoroutines();
		this.m_gui.transform.position = this.m_definedHiddenPos;
		this.m_isRevealed = false;
		this.m_isHidden = true;
		this.btnExpand.controlIsEnabled = true;
	}

	// Token: 0x060000DB RID: 219 RVA: 0x0000622C File Offset: 0x0000442C
	public void QuickSetRevealed()
	{
		this.m_interactivePanel.StopAllCoroutines();
		this.m_gui.transform.position = this.m_definedRevealedPos;
		this.m_isRevealed = true;
		this.m_isHidden = false;
		this.btnExpand.controlIsEnabled = true;
		this.m_newMessageIcon.SetOnOff(false);
		this.UpdateSlider();
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00006288 File Offset: 0x00004488
	private void DEBUG_DefinedTransitionPositions()
	{
	}

	// Token: 0x060000DD RID: 221 RVA: 0x0000628C File Offset: 0x0000448C
	private void ValidateComponents()
	{
		if (this.m_isValid)
		{
			return;
		}
		DebugUtils.Assert(this.Validate_InteractivePanel(), "LogWindow base does not have a UIInteractivePanel-component, or m_gui is null !");
		DebugUtils.Assert(this.Validate_ExpandButton(), "LogWindow failed to validate button named btnExpand !");
		DebugUtils.Assert(this.Validate_List(), "LogWindow failed to validate label named UIScrollList named list !");
		DebugUtils.Assert(this.Validate_NewMessageIcon(), "LogWindow failed to validate StatusLight_Basic named iconNewMessage !");
		DebugUtils.Assert(this.Validate_Slider(), "LogWindow failed to validate UISlider named slider !");
		this.DoAdditionalValidation();
		this.m_isValid = true;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00006304 File Offset: 0x00004504
	protected virtual void DoAdditionalValidation()
	{
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00006308 File Offset: 0x00004508
	private bool Validate_InteractivePanel()
	{
		if (this.m_gui == null)
		{
			return false;
		}
		this.m_interactivePanel = this.m_gui.GetComponent<UIInteractivePanel>();
		return this.m_interactivePanel != null;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006348 File Offset: 0x00004548
	private bool Validate_Slider()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("slider", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_slider = gameObject.GetComponent<UISlider>();
		return this.m_slider != null;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00006390 File Offset: 0x00004590
	private bool Validate_ExpandButton()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("btnExpand", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnExpand = gameObject.GetComponent<UIButton>();
		return this.btnExpand != null;
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000063D8 File Offset: 0x000045D8
	private bool Validate_NewMessageIcon()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("iconNewMessage", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_newMessageIcon = gameObject.GetComponent<StatusLight_Basic>();
		this.m_newMessageIcon.Initialize();
		this.m_newMessageIcon.SetOnOff(false);
		return this.m_newMessageIcon != null;
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00006438 File Offset: 0x00004638
	private bool Validate_List()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("list", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.m_UIList = gameObject.GetComponent<UIScrollList>();
		return this.m_UIList != null;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006480 File Offset: 0x00004680
	protected virtual bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = this.m_gui.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060000E5 RID: 229 RVA: 0x000064BC File Offset: 0x000046BC
	// (set) Token: 0x060000E6 RID: 230 RVA: 0x000064C4 File Offset: 0x000046C4
	public bool IsValid
	{
		get
		{
			return this.m_isValid;
		}
		private set
		{
			this.m_isValid = value;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060000E7 RID: 231 RVA: 0x000064D0 File Offset: 0x000046D0
	// (set) Token: 0x060000E8 RID: 232 RVA: 0x00006528 File Offset: 0x00004728
	public float Width
	{
		get
		{
			if (this.m_UIList == null)
			{
				return 0f;
			}
			float x = this.m_UIList.viewableArea.x;
			if (this.m_slider == null)
			{
				return x;
			}
			return x + this.m_slider.height;
		}
		set
		{
			if (this.m_UIList == null)
			{
				return;
			}
			float num = 0f;
			if (this.m_slider == null)
			{
				num = this.m_slider.height;
			}
			this.m_UIList.viewableArea = new Vector2(value - num, this.m_UIList.viewableArea.y);
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060000E9 RID: 233 RVA: 0x00006590 File Offset: 0x00004790
	// (set) Token: 0x060000EA RID: 234 RVA: 0x000065BC File Offset: 0x000047BC
	public float Height
	{
		get
		{
			if (this.m_UIList == null)
			{
				return 0f;
			}
			return this.m_UIList.viewableArea.y;
		}
		set
		{
			if (this.m_UIList == null)
			{
				return;
			}
			this.m_UIList.viewableArea = new Vector2(this.m_UIList.viewableArea.x, value);
		}
	}

	// Token: 0x0400006A RID: 106
	public GameObject m_gui;

	// Token: 0x0400006B RID: 107
	public bool m_autoSlideToFocusLastItem = true;

	// Token: 0x0400006C RID: 108
	public float m_fillPercentNeededBeforeScrollEnabled = 0.95f;

	// Token: 0x0400006D RID: 109
	protected bool m_isValid;

	// Token: 0x0400006E RID: 110
	protected GameObject m_guiCam;

	// Token: 0x0400006F RID: 111
	protected bool m_isRevealed;

	// Token: 0x04000070 RID: 112
	protected bool m_isHidden;

	// Token: 0x04000071 RID: 113
	protected float m_totalHeightOfItemsInList;

	// Token: 0x04000072 RID: 114
	protected UIListItem m_latestEntry;

	// Token: 0x04000073 RID: 115
	private UIButton btnExpand;

	// Token: 0x04000074 RID: 116
	private UIInteractivePanel m_interactivePanel;

	// Token: 0x04000075 RID: 117
	protected UIScrollList m_UIList;

	// Token: 0x04000076 RID: 118
	protected UISlider m_slider;

	// Token: 0x04000077 RID: 119
	private Vector3 m_definedRevealedPos;

	// Token: 0x04000078 RID: 120
	private Vector3 m_definedHiddenPos;

	// Token: 0x04000079 RID: 121
	private StatusLight_Basic m_newMessageIcon;

	// Token: 0x0400007A RID: 122
	private Transform m_anchor;

	// Token: 0x02000010 RID: 16
	public enum LogWindow_ScreenAlignment
	{
		// Token: 0x0400007C RID: 124
		PrefabDefined,
		// Token: 0x0400007D RID: 125
		Left,
		// Token: 0x0400007E RID: 126
		Right
	}
}
