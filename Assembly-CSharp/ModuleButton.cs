using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
internal class ModuleButton
{
	// Token: 0x060002C9 RID: 713 RVA: 0x00015224 File Offset: 0x00013424
	public ModuleButton(HPModule module, GameObject guiCamera, bool localOwner, EZValueChangedDelegate onPressed, EZDragDropDelegate onDraged, bool canOrder)
	{
		this.m_button = GuiUtils.CreateGui("IngameGui/FlowerButton", guiCamera);
		this.m_module = module;
		this.m_button.GetComponent<ToolTip>().m_toolTip = Localize.instance.TranslateRecursive("$" + module.name + "_name");
		this.m_disabledButton = GuiUtils.FindChildOf(this.m_button, "DisabledButton");
		this.m_noammoIcon = GuiUtils.FindChildOf(this.m_button, "NoAmmo");
		this.m_rechargeAnim = GuiUtils.FindChildOf(this.m_button, "RechargeAnimation").GetComponent<PackedSprite>();
		this.m_rechargeText = GuiUtils.FindChildOfComponent<SpriteText>(this.m_button, "RechargeTextLabel");
		this.m_disabledButton.SetActiveRecursively(false);
		this.m_noammoIcon.SetActiveRecursively(false);
		GameObject gameObject = GuiUtils.FindChildOf(this.m_button, "StockEar");
		this.m_ammoText = GuiUtils.FindChildOf(gameObject, "StockValueLabel").GetComponent<SpriteText>();
		SpriteText component = GuiUtils.FindChildOf(this.m_button, "SizeLabel").GetComponent<SpriteText>();
		component.Text = module.GetAbbr();
		gameObject.SetActiveRecursively(false);
		Gun gun = module as Gun;
		if (gun && gun.GetMaxAmmo() > 0 && localOwner)
		{
			gameObject.SetActiveRecursively(true);
		}
		Texture2D guitexture = module.m_GUITexture;
		UIButton component2 = this.m_button.GetComponent<UIButton>();
		GuiUtils.SetButtonImageSheet(component2, guitexture);
		component2.SetValueChangedDelegate(onPressed);
		this.m_disabledButton.GetComponent<UIButton>().SetValueChangedDelegate(onPressed);
		if (canOrder)
		{
			component2.SetDragDropDelegate(onDraged);
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000153B8 File Offset: 0x000135B8
	public bool MouseOver()
	{
		return GuiUtils.HasPointerRecursive(UIManager.instance, this.m_button);
	}

	// Token: 0x060002CB RID: 715 RVA: 0x000153CC File Offset: 0x000135CC
	public void SetCharge(float i, float time)
	{
		if (i < 0f || i >= 1f)
		{
			this.m_rechargeAnim.Hide(true);
			this.m_rechargeText.Hide(true);
		}
		else
		{
			this.m_rechargeAnim.Hide(false);
			int frameCount = this.m_rechargeAnim.animations[0].GetFrameCount();
			this.m_rechargeAnim.SetFrame(0, (int)((float)frameCount * i));
			if (time >= 0f)
			{
				this.m_rechargeText.Hide(false);
				this.m_rechargeText.Text = time.ToString("F1");
			}
			else
			{
				this.m_rechargeText.Hide(true);
			}
		}
	}

	// Token: 0x04000200 RID: 512
	public GameObject m_button;

	// Token: 0x04000201 RID: 513
	public GameObject m_disabledButton;

	// Token: 0x04000202 RID: 514
	public GameObject m_noammoIcon;

	// Token: 0x04000203 RID: 515
	public PackedSprite m_rechargeAnim;

	// Token: 0x04000204 RID: 516
	public SpriteText m_rechargeText;

	// Token: 0x04000205 RID: 517
	public SpriteText m_ammoText;

	// Token: 0x04000206 RID: 518
	public HPModule m_module;

	// Token: 0x04000207 RID: 519
	public float m_sortPos;

	// Token: 0x04000208 RID: 520
	public Vector3 m_point0;

	// Token: 0x04000209 RID: 521
	public Vector3 m_point1;
}
