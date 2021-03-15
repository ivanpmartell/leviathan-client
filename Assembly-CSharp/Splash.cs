using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class Splash
{
	// Token: 0x060004A6 RID: 1190 RVA: 0x00028424 File Offset: 0x00026624
	public Splash(GameObject guiCamera, MusicManager musMan)
	{
		this.m_guiCamera = guiCamera;
		this.m_musicMan = musMan;
		this.m_musicMan.SetMusic("menu");
		this.m_gui = GuiUtils.CreateGui("Splash", this.m_guiCamera);
		this.m_panels = new UIPanel[this.m_maxScreens];
		for (int i = 0; i < this.m_maxScreens; i++)
		{
			string text = "Splash" + i.ToString();
			this.m_panels[i] = GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, text);
			this.m_panels[i].Dismiss();
			DebugUtils.Assert(this.m_panels[i] != null, "Missing panel " + text);
		}
		this.m_time = this.m_delay;
		this.ShowPanel(0);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0002850C File Offset: 0x0002670C
	public void Close()
	{
		PLog.Log("Derp close ");
		UnityEngine.Object.Destroy(this.m_gui);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00028524 File Offset: 0x00026724
	public void Update()
	{
		if (this.m_finished)
		{
			PLog.Log("FADE OUT");
			this.m_onFadeoutComplete();
			return;
		}
		if (this.m_hiding)
		{
			PLog.Log("Dismissing");
			GuiUtils.FindChildOfComponent<UIPanel>(this.m_gui, "Bkg").Dismiss();
			this.m_currentPanel.Dismiss();
			this.m_currentPanel.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.OnFadedOut));
			this.m_hiding = false;
		}
		if (!this.m_done)
		{
			this.m_time -= Time.deltaTime;
			if (this.m_time <= 0f)
			{
				this.NextScreen();
			}
			if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
			{
				this.m_screen = this.m_maxScreens;
				this.m_time = 0f;
			}
		}
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x00028608 File Offset: 0x00026808
	private void NextScreen()
	{
		this.m_screen++;
		if (this.m_screen >= this.m_maxScreens)
		{
			this.m_done = true;
			this.m_hiding = true;
			PLog.Log("Splash done");
			this.m_onDone();
			return;
		}
		this.ShowPanel(this.m_screen);
		this.m_time = this.m_delay;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x00028670 File Offset: 0x00026870
	private void OnFadedOut(UIPanelBase panel, EZTransition transition)
	{
		PLog.Log("On faded out");
		this.m_finished = true;
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00028684 File Offset: 0x00026884
	private void ShowPanel(int id)
	{
		if (this.m_currentPanel != null)
		{
			this.m_currentPanel.Dismiss();
			this.m_currentPanel = null;
		}
		this.m_currentPanel = this.m_panels[id];
		this.m_currentPanel.BringIn();
	}

	// Token: 0x040003F7 RID: 1015
	public Action m_onDone;

	// Token: 0x040003F8 RID: 1016
	public Action m_onFadeoutComplete;

	// Token: 0x040003F9 RID: 1017
	private GameObject m_gui;

	// Token: 0x040003FA RID: 1018
	private GameObject m_guiCamera;

	// Token: 0x040003FB RID: 1019
	private MusicManager m_musicMan;

	// Token: 0x040003FC RID: 1020
	private float m_delay = 2f;

	// Token: 0x040003FD RID: 1021
	private float m_time;

	// Token: 0x040003FE RID: 1022
	private int m_screen;

	// Token: 0x040003FF RID: 1023
	private int m_maxScreens = 2;

	// Token: 0x04000400 RID: 1024
	private bool m_done;

	// Token: 0x04000401 RID: 1025
	private bool m_hiding;

	// Token: 0x04000402 RID: 1026
	private bool m_finished;

	// Token: 0x04000403 RID: 1027
	private UIPanel[] m_panels;

	// Token: 0x04000404 RID: 1028
	private UIPanel m_currentPanel;
}
