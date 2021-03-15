using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class MissionLog
{
	// Token: 0x06000373 RID: 883 RVA: 0x0001BF8C File Offset: 0x0001A18C
	public MissionLog(GameObject gui, GameObject guiCam)
	{
		this.m_gui = gui;
		this.SetupGui(guiCam);
	}

	// Token: 0x06000374 RID: 884 RVA: 0x0001BFA4 File Offset: 0x0001A1A4
	public void Close()
	{
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0001BFA8 File Offset: 0x0001A1A8
	public void Hide()
	{
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0001BFAC File Offset: 0x0001A1AC
	public void Show()
	{
	}

	// Token: 0x06000377 RID: 887 RVA: 0x0001BFB0 File Offset: 0x0001A1B0
	public void Toggle()
	{
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0001BFB4 File Offset: 0x0001A1B4
	private void SetupGui(GameObject guiCam)
	{
		this.m_gui = GuiUtils.FindChildOf(this.m_gui.transform, "ObjectivesContainer");
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0001BFD4 File Offset: 0x0001A1D4
	public void Update(Camera camera, float dt)
	{
		if (!this.m_gui.active)
		{
			return;
		}
		GuiUtils.FindChildOf(this.m_gui.transform, "Line1").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "Line2").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "Line3").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "Line4").SetActiveRecursively(false);
		GuiUtils.FindChildOf(this.m_gui.transform, "Line5").SetActiveRecursively(false);
		UIScrollList component = GuiUtils.FindChildOf(this.m_gui.transform, "PrimaryObjectivesScrollist").GetComponent<UIScrollList>();
		component.ClearList(true);
		foreach (TurnMan.MissionObjective missionObjective in TurnMan.instance.m_missionObjectives)
		{
			if (missionObjective.m_status != MNAction.ObjectiveStatus.Hide)
			{
				GameObject gameObject;
				if (missionObjective.m_status == MNAction.ObjectiveStatus.Active)
				{
					gameObject = GuiUtils.CreateGui("Briefing/PrimaryObjectivesListItem", camera.gameObject);
				}
				else if (missionObjective.m_status == MNAction.ObjectiveStatus.Done)
				{
					gameObject = GuiUtils.CreateGui("Briefing/PrimaryObjectivesListItem_Done", camera.gameObject);
				}
				else
				{
					gameObject = GuiUtils.CreateGui("Briefing/PrimaryObjectivesListItem", camera.gameObject);
				}
				GuiUtils.FindChildOf(gameObject.transform, "PrimaryObjectivesText").GetComponent<SpriteText>().Text = Localize.instance.Translate("$" + missionObjective.m_text);
				component.AddItem(gameObject);
			}
		}
	}

	// Token: 0x040002E9 RID: 745
	public GameObject m_gui;
}
