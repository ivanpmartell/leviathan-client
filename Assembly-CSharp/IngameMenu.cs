using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class IngameMenu
{
	// Token: 0x06000082 RID: 130 RVA: 0x000044E8 File Offset: 0x000026E8
	public IngameMenu(GameObject menuRoot)
	{
		this.m_gui = menuRoot;
		this.m_btnBackToMenu = GuiUtils.FindChildOf(this.m_gui, "btnBackToMenu").GetComponent<UIButton>();
		this.m_btnExit = GuiUtils.FindChildOf(this.m_gui, "btnExit").GetComponent<UIButton>();
		this.m_btnOptions = GuiUtils.FindChildOf(this.m_gui, "btnOptions").GetComponent<UIButton>();
		this.m_btnSurrender = GuiUtils.FindChildOf(this.m_gui, "btnSurrender").GetComponent<UIButton>();
		this.m_btnLeave = GuiUtils.FindChildOf(this.m_gui, "btnLeave").GetComponent<UIButton>();
		this.m_btnSurrender.SetValueChangedDelegate(new EZValueChangedDelegate(this.SurrenderClicked));
		this.m_btnOptions.SetValueChangedDelegate(new EZValueChangedDelegate(this.OptionsClicked));
		this.m_btnBackToMenu.SetValueChangedDelegate(new EZValueChangedDelegate(this.BackToMenuClicked));
		this.m_btnExit.SetValueChangedDelegate(new EZValueChangedDelegate(this.ExitClicked));
		this.m_btnLeave.SetValueChangedDelegate(new EZValueChangedDelegate(this.LeaveClicked));
		this.m_btnSurrender.controlIsEnabled = false;
		this.m_btnLeave.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x06000083 RID: 131 RVA: 0x0000461C File Offset: 0x0000281C
	public void SetSurrenderStatus(bool surrendered, bool inPlanning, bool inReplayMode, bool isAdmin)
	{
		if (inReplayMode || !this.m_gui.active)
		{
			this.m_btnSurrender.gameObject.SetActiveRecursively(false);
			this.m_btnLeave.gameObject.SetActiveRecursively(false);
		}
		else if (surrendered)
		{
			this.m_btnSurrender.gameObject.SetActiveRecursively(false);
			if (!isAdmin)
			{
				this.m_btnLeave.gameObject.SetActiveRecursively(true);
			}
		}
		else
		{
			this.m_btnSurrender.gameObject.SetActiveRecursively(true);
			this.m_btnLeave.gameObject.SetActiveRecursively(false);
			this.m_btnSurrender.controlIsEnabled = inPlanning;
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x000046C8 File Offset: 0x000028C8
	public void Close()
	{
	}

	// Token: 0x06000085 RID: 133 RVA: 0x000046CC File Offset: 0x000028CC
	private void SurrenderClicked(IUIObject ignore)
	{
		if (this.m_OnSurrender != null)
		{
			this.m_OnSurrender();
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x000046E4 File Offset: 0x000028E4
	private void OptionsClicked(IUIObject ignore)
	{
		if (this.m_OnOptions != null)
		{
			this.m_OnOptions();
		}
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000046FC File Offset: 0x000028FC
	private void SwitchGameClicked(IUIObject ignore)
	{
		if (this.m_OnSwitchGame != null)
		{
			this.m_OnSwitchGame();
		}
	}

	// Token: 0x06000088 RID: 136 RVA: 0x00004714 File Offset: 0x00002914
	private void BackToMenuClicked(IUIObject ignore)
	{
		if (this.m_OnBackToMenu != null)
		{
			this.m_OnBackToMenu();
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x0000472C File Offset: 0x0000292C
	private void ExitClicked(IUIObject ignore)
	{
		if (this.m_OnQuitGame != null)
		{
			this.m_OnQuitGame();
		}
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00004744 File Offset: 0x00002944
	private void LeaveClicked(IUIObject ignore)
	{
		if (this.m_OnLeave != null)
		{
			this.m_OnLeave();
		}
	}

	// Token: 0x04000042 RID: 66
	public Action m_OnSurrender;

	// Token: 0x04000043 RID: 67
	public Action m_OnLeave;

	// Token: 0x04000044 RID: 68
	public Action m_OnOptions;

	// Token: 0x04000045 RID: 69
	public Action m_OnSwitchGame;

	// Token: 0x04000046 RID: 70
	public Action m_OnBackToMenu;

	// Token: 0x04000047 RID: 71
	public Action m_OnQuitGame;

	// Token: 0x04000048 RID: 72
	private GameObject m_gui;

	// Token: 0x04000049 RID: 73
	private UIButton m_btnSurrender;

	// Token: 0x0400004A RID: 74
	private UIButton m_btnLeave;

	// Token: 0x0400004B RID: 75
	private UIButton m_btnOptions;

	// Token: 0x0400004C RID: 76
	private UIButton m_btnBackToMenu;

	// Token: 0x0400004D RID: 77
	private UIButton m_btnExit;
}
