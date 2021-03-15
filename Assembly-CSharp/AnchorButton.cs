using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
internal class AnchorButton
{
	// Token: 0x060002CC RID: 716 RVA: 0x0001547C File Offset: 0x0001367C
	public AnchorButton(Ship ship, GameObject guiCamera, bool localOwner, bool canOrder)
	{
		this.m_ship = ship;
		this.m_button = GuiUtils.CreateGui("IngameGui/FlowerButtonAnchor", guiCamera);
		this.m_disableButton = GuiUtils.FindChildOf(this.m_button, "Disable").GetComponent<UIButton>();
		this.m_enableButton = GuiUtils.FindChildOf(this.m_button, "Enable").GetComponent<UIButton>();
		this.m_rechargeAnim = GuiUtils.FindChildOf(this.m_button, "RechargeAnimation").GetComponent<PackedSprite>();
		if (canOrder)
		{
			this.m_enableButton.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnToggleModePressed));
			this.m_disableButton.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnToggleModePressed));
		}
		this.UpdateStatus();
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00015540 File Offset: 0x00013740
	public void Close()
	{
		UnityEngine.Object.Destroy(this.m_button);
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00015550 File Offset: 0x00013750
	public bool MouseOver()
	{
		return GuiUtils.HasPointerRecursive(UIManager.instance, this.m_button);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00015564 File Offset: 0x00013764
	private void UpdateStatus()
	{
		if (this.m_ship.GetRequestedMaintenanceMode())
		{
			this.m_enableButton.gameObject.SetActiveRecursively(false);
			this.m_disableButton.gameObject.SetActiveRecursively(true);
		}
		else
		{
			this.m_enableButton.gameObject.SetActiveRecursively(true);
			this.m_disableButton.gameObject.SetActiveRecursively(false);
		}
		this.SetCharge(this.m_ship.GetMaintenanceTimer());
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x000155DC File Offset: 0x000137DC
	public void UpdatePosition(float guiScale, Camera guiCamera, Camera gameCamera, ref float lowestScreenPos)
	{
		Vector3 pos = this.m_ship.transform.position + new Vector3(0f, this.m_ship.m_deckHeight, 0f);
		Vector3 position = GuiUtils.WorldToGuiPos(gameCamera, guiCamera, pos);
		this.m_button.transform.position = position;
		this.UpdateStatus();
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x0001563C File Offset: 0x0001383C
	private void OnToggleModePressed(IUIObject obj)
	{
		this.m_ship.SetRequestedMaintenanceMode(!this.m_ship.GetRequestedMaintenanceMode());
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x00015658 File Offset: 0x00013858
	public void SetCharge(float i)
	{
		if (i < 0f || i >= 1f)
		{
			this.m_rechargeAnim.Hide(true);
		}
		else
		{
			this.m_rechargeAnim.Hide(false);
			int frameCount = this.m_rechargeAnim.animations[0].GetFrameCount();
			this.m_rechargeAnim.SetFrame(0, (int)((float)frameCount * i));
		}
	}

	// Token: 0x0400020A RID: 522
	private GameObject m_button;

	// Token: 0x0400020B RID: 523
	private UIButton m_disableButton;

	// Token: 0x0400020C RID: 524
	private UIButton m_enableButton;

	// Token: 0x0400020D RID: 525
	private PackedSprite m_rechargeAnim;

	// Token: 0x0400020E RID: 526
	private Ship m_ship;
}
