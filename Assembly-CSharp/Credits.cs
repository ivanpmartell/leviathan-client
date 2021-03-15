using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
internal class Credits
{
	// Token: 0x0600021D RID: 541 RVA: 0x0000E2B8 File Offset: 0x0000C4B8
	public Credits(GameObject creditsRoot, MusicManager musMan)
	{
		this.m_creditsPanel = creditsRoot.GetComponent<UIPanel>();
		this.m_creditsPanel.gameObject.SetActiveRecursively(false);
		this.m_musicMan = musMan;
		if (GuiUtils.FindChildOf(creditsRoot, "ExitCreditsButton"))
		{
			this.m_button = GuiUtils.FindChildOf(creditsRoot, "ExitCreditsButton").GetComponent<UIButton>();
			this.m_button.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.onExit));
		}
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000E348 File Offset: 0x0000C548
	private void onExit(IUIObject button)
	{
		this.Close();
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000E350 File Offset: 0x0000C550
	public void Start()
	{
		this.m_musicMan.SetMusic("music-credits");
		this.m_creditsPanel.gameObject.SetActiveRecursively(false);
		this.m_creditsPanel.gameObject.active = true;
		if (this.m_button)
		{
			this.m_button.gameObject.active = true;
		}
		this.m_creditsTimer = 0f;
		this.m_creditsIndex = 0;
		this.m_creditsPanel.GetComponent<UIPanelManager>().DismissImmediate();
		this.m_creditsPanel.GetComponent<UIPanelManager>().BringIn(0);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000E3E4 File Offset: 0x0000C5E4
	private void Close()
	{
		this.m_musicMan.SetMusic("menu");
		this.m_creditsPanel.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000E408 File Offset: 0x0000C608
	public void Update(float dt)
	{
		if (!this.m_creditsPanel.gameObject.active)
		{
			return;
		}
		if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Space))
		{
			this.Close();
		}
		this.m_creditsTimer += dt;
		if (this.m_creditsTimer > this.m_creditsDelay)
		{
			this.m_creditsTimer = 0f;
			this.m_creditsIndex++;
			PLog.Log("Next credits screen " + this.m_creditsIndex);
			if (this.m_creditsIndex >= this.m_creditsMaxIndex)
			{
				PLog.Log("done with credits");
				this.Close();
			}
			else
			{
				UIPanelManager component = this.m_creditsPanel.GetComponent<UIPanelManager>();
				component.BringIn(this.m_creditsIndex);
			}
		}
	}

	// Token: 0x04000167 RID: 359
	private UIButton m_button;

	// Token: 0x04000168 RID: 360
	private UIPanel m_creditsPanel;

	// Token: 0x04000169 RID: 361
	private float m_creditsTimer;

	// Token: 0x0400016A RID: 362
	private float m_creditsDelay = 8f;

	// Token: 0x0400016B RID: 363
	private int m_creditsIndex;

	// Token: 0x0400016C RID: 364
	private int m_creditsMaxIndex = 6;

	// Token: 0x0400016D RID: 365
	private MusicManager m_musicMan;
}
