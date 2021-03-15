using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class FlowerMenu
{
	// Token: 0x060002D3 RID: 723 RVA: 0x000156BC File Offset: 0x000138BC
	public FlowerMenu(Camera gameCamera, GameObject guiCamera, Ship ship, bool canOrder, bool localOwner)
	{
		this.m_guiCamera = guiCamera;
		this.m_gameCamera = gameCamera;
		this.m_ship = ship;
		this.m_canOrder = canOrder;
		this.m_localOwner = localOwner;
		this.m_lineDrawer = this.m_gameCamera.GetComponent<LineDrawer>();
		this.m_lineType = this.m_lineDrawer.GetTypeID("flowerLine");
		List<HPModule> list = new List<HPModule>();
		this.m_ship.GetAllHPModules(ref list);
		this.m_bkg = GuiUtils.CreateGui("IngameGui/CommandRose/CommandRose", this.m_guiCamera);
		DebugUtils.Assert(this.m_bkg);
		bool currentMaintenanceMode = ship.GetCurrentMaintenanceMode();
		foreach (HPModule module in list)
		{
			this.m_buttons.Add(new ModuleButton(module, this.m_guiCamera, this.m_localOwner, new EZValueChangedDelegate(this.OnButtonPressed), new EZDragDropDelegate(this.OnButtonDragged), canOrder && !currentMaintenanceMode));
		}
		this.SortButtons();
		if (this.m_canOrder && !currentMaintenanceMode)
		{
			this.m_forwardButton = new MoveButton(MoveButton.MoveType.Forward, this.m_guiCamera, new EZDragDropDelegate(this.OnButtonForward));
			this.m_reverseButton = new MoveButton(MoveButton.MoveType.Reverse, this.m_guiCamera, new EZDragDropDelegate(this.OnButtonReverse));
			this.m_rotateButton = new MoveButton(MoveButton.MoveType.Rotate, this.m_guiCamera, new EZDragDropDelegate(this.OnButtonRotate));
			SupportShip supportShip = ship as SupportShip;
			if (supportShip != null)
			{
				this.m_supplyButton = this.CreateSupplyButton(supportShip);
			}
		}
		this.UpdateGuiPos();
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0001589C File Offset: 0x00013A9C
	private GameObject CreateSupplyButton(SupportShip supportShip)
	{
		GameObject gameObject = GuiUtils.CreateGui("IngameGui/FlowerButtonSupply", this.m_guiCamera);
		GameObject gameObject2 = GuiUtils.FindChildOf(gameObject, "Enable");
		GameObject gameObject3 = GuiUtils.FindChildOf(gameObject, "Disable");
		gameObject2.SetActiveRecursively(!supportShip.IsSupplyEnabled());
		gameObject3.SetActiveRecursively(supportShip.IsSupplyEnabled());
		gameObject2.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSupplyToggle));
		gameObject3.GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSupplyToggle));
		return gameObject;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0001591C File Offset: 0x00013B1C
	private void PlaceSupplyButton()
	{
		if (this.m_supplyButton != null)
		{
			Camera camera = this.m_guiCamera.camera;
			Vector3 pos = this.m_ship.transform.position + new Vector3(0f, this.m_ship.m_deckHeight, 0f);
			Vector3 position = GuiUtils.WorldToGuiPos(this.m_gameCamera, camera, pos);
			this.m_supplyButton.transform.position = position;
		}
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00015998 File Offset: 0x00013B98
	private void OnSupplyToggle(IUIObject obj)
	{
		this.m_onToggleSupply(this.m_ship);
		UnityEngine.Object.Destroy(this.m_supplyButton);
		this.m_supplyButton = this.CreateSupplyButton(this.m_ship as SupportShip);
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000159D0 File Offset: 0x00013BD0
	private void SortButtons()
	{
		Vector3 forward = this.m_ship.transform.forward;
		Vector3 position = this.m_ship.transform.position;
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			Vector3 position2 = moduleButton.m_module.transform.position;
			Vector3 rhs = position2 - position;
			moduleButton.m_sortPos = -Vector3.Dot(forward, rhs);
		}
		this.m_buttons.Sort((ModuleButton a, ModuleButton b) => a.m_sortPos.CompareTo(b.m_sortPos));
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00015AA4 File Offset: 0x00013CA4
	public Ship GetShip()
	{
		return this.m_ship;
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00015AAC File Offset: 0x00013CAC
	public void Close()
	{
		if (this.m_dragging != null)
		{
			this.m_dragging.m_button.GetComponent<UIButton>().CancelDrag();
		}
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			UnityEngine.Object.Destroy(moduleButton.m_button);
		}
		if (this.m_forwardButton != null)
		{
			UnityEngine.Object.Destroy(this.m_forwardButton.m_button);
		}
		if (this.m_reverseButton != null)
		{
			UnityEngine.Object.Destroy(this.m_reverseButton.m_button);
		}
		if (this.m_rotateButton != null)
		{
			UnityEngine.Object.Destroy(this.m_rotateButton.m_button);
		}
		if (this.m_anchorButton != null)
		{
			this.m_anchorButton.Close();
		}
		UnityEngine.Object.Destroy(this.m_bkg);
		if (this.m_supplyButton != null)
		{
			UnityEngine.Object.Destroy(this.m_supplyButton);
		}
	}

	// Token: 0x060002DB RID: 731 RVA: 0x00015BC4 File Offset: 0x00013DC4
	public void LateUpdate(float dt)
	{
		this.UpdateGuiPos();
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00015BCC File Offset: 0x00013DCC
	private void UpdateGuiPos()
	{
		this.m_lowestScreenPos = 10000f;
		float num = Mathf.Tan(0.017453292f * this.m_gameCamera.fieldOfView * 0.5f);
		float guiScale = Vector3.Distance(this.m_gameCamera.transform.position, this.m_ship.transform.position) * num * 0.02f;
		Camera camera = this.m_guiCamera.camera;
		this.PlaceBkg(guiScale);
		this.PlaceModuleButtons(guiScale);
		if (this.m_supplyButton != null)
		{
			this.PlaceSupplyButton();
		}
		if (this.m_anchorButton != null)
		{
			this.m_anchorButton.UpdatePosition(guiScale, camera, this.m_gameCamera, ref this.m_lowestScreenPos);
		}
		if (this.m_forwardButton != null)
		{
			this.m_forwardButton.UpdatePosition(guiScale, this.m_ship, camera, this.m_gameCamera, ref this.m_lowestScreenPos);
		}
		if (this.m_reverseButton != null)
		{
			this.m_reverseButton.UpdatePosition(guiScale, this.m_ship, camera, this.m_gameCamera, ref this.m_lowestScreenPos);
		}
		if (this.m_rotateButton != null)
		{
			this.m_rotateButton.UpdatePosition(guiScale, this.m_ship, camera, this.m_gameCamera, ref this.m_lowestScreenPos);
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x00015D04 File Offset: 0x00013F04
	private void PlaceBkg(float guiScale)
	{
		Camera camera = this.m_guiCamera.camera;
		float num = Mathf.Clamp(guiScale, 1f, FlowerMenu.m_maxGuiScale);
		float num2 = (this.m_ship.GetLength() / 2f + 6f) * num;
		float num3 = (this.m_ship.GetWidth() / 2f + 6f) * num;
		this.m_bkg.transform.position = this.m_ship.transform.position + new Vector3(0f, this.m_ship.m_deckHeight, 0f);
		this.m_bkg.transform.rotation = this.m_ship.GetRealRot();
		SimpleSprite component = this.m_bkg.GetComponent<SimpleSprite>();
		float num4 = 1.5f;
		component.SetSize(num3 * 2f * num4, num2 * 2f * num4);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00015DEC File Offset: 0x00013FEC
	private void PlaceModuleButtons(float guiScale)
	{
		Camera camera = this.m_guiCamera.camera;
		float num = Mathf.Clamp(guiScale, 1f, FlowerMenu.m_maxGuiScale);
		float num2 = this.m_ship.m_Width / 2f;
		Vector3 right = this.m_ship.transform.right;
		Vector3 forward = this.m_ship.transform.forward;
		Vector3 position = this.m_ship.transform.position;
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			moduleButton.m_point1 = Vector3.zero;
		}
		List<ModuleButton> list = new List<ModuleButton>();
		List<ModuleButton> list2 = new List<ModuleButton>();
		foreach (ModuleButton moduleButton2 in this.m_buttons)
		{
			Vector3 position2 = moduleButton2.m_module.transform.position;
			Vector3 rhs = position2 - position;
			Vector3 normalized = rhs.normalized;
			float f = Vector3.Dot(right, rhs);
			if (Mathf.Abs(f) < 0.1f)
			{
				f = ((list.Count >= list2.Count) ? 1f : -1f);
			}
			float num3 = Mathf.Sign(f);
			if ((double)num3 < 0.1)
			{
				list.Add(moduleButton2);
			}
			else
			{
				list2.Add(moduleButton2);
			}
		}
		float length = (this.m_ship.GetLength() / 2f + 6f) * num;
		float width = (this.m_ship.GetWidth() / 2f + 6f) * num;
		this.LayoutSide(list, true, length, width, camera);
		this.LayoutSide(list2, false, length, width, camera);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00016008 File Offset: 0x00014208
	private void LayoutSide(List<ModuleButton> buttons, bool left, float length, float width, Camera guiCamera)
	{
		float num = 160f / (float)(buttons.Count + 1);
		for (int i = 0; i < buttons.Count; i++)
		{
			float num2 = 10f + (float)(i + 1) * num;
			float f = 0.017453292f * num2;
			Vector3 vector = new Vector3(Mathf.Sin(f) * width, 0f, Mathf.Cos(f) * length);
			if (left)
			{
				vector.x *= -1f;
			}
			vector = this.m_ship.transform.TransformDirection(vector);
			ModuleButton moduleButton = buttons[i];
			moduleButton.m_point0 = moduleButton.m_module.transform.position;
			moduleButton.m_point1 = this.m_ship.transform.position + vector;
			Vector3 position = GuiUtils.WorldToGuiPos(this.m_gameCamera, guiCamera, moduleButton.m_point1);
			moduleButton.m_button.transform.position = position;
			if (moduleButton.m_button.transform.position.y - 19f < this.m_lowestScreenPos)
			{
				this.m_lowestScreenPos = moduleButton.m_button.transform.position.y - 19f;
			}
		}
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00016158 File Offset: 0x00014358
	public void Update(float dt)
	{
		if (this.m_dragging != null)
		{
			this.m_dragging.m_button.GetComponent<UIButton>().CancelDrag();
			this.m_onModuleDragged(this.m_dragging.m_module);
			this.m_dragging = null;
			return;
		}
		if (this.m_moveForward && this.m_onMoveForward != null)
		{
			this.m_forwardButton.m_button.GetComponent<UIButton>().CancelDrag();
			this.m_onMoveForward(this.m_ship);
			return;
		}
		if (this.m_moveReverse && this.m_onMoveReverse != null)
		{
			this.m_reverseButton.m_button.GetComponent<UIButton>().CancelDrag();
			this.m_onMoveReverse(this.m_ship);
			return;
		}
		if (this.m_moveRotate && this.m_onMoveRotate != null)
		{
			this.m_rotateButton.m_button.GetComponent<UIButton>().CancelDrag();
			this.m_onMoveRotate(this.m_ship);
			return;
		}
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			this.m_lineDrawer.DrawLine(moduleButton.m_point0, moduleButton.m_point1, this.m_lineType, 0.2f);
			Gun gun = moduleButton.m_module as Gun;
			if (gun && gun.GetMaxAmmo() > 0 && this.m_localOwner)
			{
				float num = gun.GetLoadedSalvo() + (float)gun.GetAmmo();
				int num2 = (int)(num / (float)gun.GetSalvoSize());
				moduleButton.m_ammoText.Text = num2.ToString();
				if (num <= 0f && !gun.IsDisabled())
				{
					moduleButton.m_noammoIcon.SetActiveRecursively(true);
				}
				else
				{
					moduleButton.m_noammoIcon.SetActiveRecursively(false);
				}
			}
			if (moduleButton.m_module.IsDisabled())
			{
				moduleButton.m_disabledButton.SetActiveRecursively(true);
				moduleButton.m_button.GetComponent<UIButton>().SetControlState(UIButton.CONTROL_STATE.DISABLED);
			}
			else
			{
				moduleButton.m_disabledButton.SetActiveRecursively(false);
				UIButton component = moduleButton.m_button.GetComponent<UIButton>();
				if (component.controlState == UIButton.CONTROL_STATE.DISABLED)
				{
					component.SetControlState(UIButton.CONTROL_STATE.NORMAL);
				}
			}
			float i;
			float time;
			moduleButton.m_module.GetChargeLevel(out i, out time);
			moduleButton.SetCharge(i, time);
		}
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x000163D8 File Offset: 0x000145D8
	private void OnButtonPressed(IUIObject obj)
	{
		GameObject gameObject = obj.gameObject;
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			if (moduleButton.m_button == gameObject || moduleButton.m_disabledButton == gameObject)
			{
				this.m_onModuleSelected(moduleButton.m_module);
				break;
			}
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00016478 File Offset: 0x00014678
	private void OnButtonDragged(EZDragDropParams obj)
	{
		if (this.m_dragging != null)
		{
			return;
		}
		GameObject gameObject = obj.dragObj.gameObject;
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			if (moduleButton.m_button == gameObject)
			{
				ToolTipDisplay toolTip = ToolTipDisplay.GetToolTip(moduleButton.m_button);
				if (toolTip)
				{
					toolTip.StopToolTip();
				}
				this.m_dragging = moduleButton;
				break;
			}
		}
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x0001652C File Offset: 0x0001472C
	private void OnButtonForward(EZDragDropParams obj)
	{
		this.m_moveForward = true;
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00016538 File Offset: 0x00014738
	private void OnButtonReverse(EZDragDropParams obj)
	{
		this.m_moveReverse = true;
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x00016544 File Offset: 0x00014744
	private void OnButtonRotate(EZDragDropParams obj)
	{
		this.m_moveRotate = true;
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00016550 File Offset: 0x00014750
	public float GetLowestScreenPos()
	{
		return this.m_lowestScreenPos;
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00016558 File Offset: 0x00014758
	public bool IsMouseOver()
	{
		UIManager instance = UIManager.instance;
		foreach (ModuleButton moduleButton in this.m_buttons)
		{
			if (moduleButton.MouseOver())
			{
				return true;
			}
		}
		return (this.m_forwardButton != null && this.m_forwardButton.MouseOver()) || (this.m_reverseButton != null && this.m_reverseButton.MouseOver()) || (this.m_rotateButton != null && this.m_rotateButton.MouseOver()) || (this.m_anchorButton != null && this.m_anchorButton.MouseOver()) || (this.m_supplyButton != null && GuiUtils.HasPointerRecursive(UIManager.instance, this.m_supplyButton));
	}

	// Token: 0x0400020F RID: 527
	public static float m_maxGuiScale = 2.5f;

	// Token: 0x04000210 RID: 528
	public Action<HPModule> m_onModuleSelected;

	// Token: 0x04000211 RID: 529
	public Action<HPModule> m_onModuleDragged;

	// Token: 0x04000212 RID: 530
	public Action<Ship> m_onMoveForward;

	// Token: 0x04000213 RID: 531
	public Action<Ship> m_onMoveReverse;

	// Token: 0x04000214 RID: 532
	public Action<Ship> m_onMoveRotate;

	// Token: 0x04000215 RID: 533
	public Action<Ship> m_onToggleSupply;

	// Token: 0x04000216 RID: 534
	private List<ModuleButton> m_buttons = new List<ModuleButton>();

	// Token: 0x04000217 RID: 535
	private MoveButton m_forwardButton;

	// Token: 0x04000218 RID: 536
	private MoveButton m_reverseButton;

	// Token: 0x04000219 RID: 537
	private MoveButton m_rotateButton;

	// Token: 0x0400021A RID: 538
	private AnchorButton m_anchorButton;

	// Token: 0x0400021B RID: 539
	private GameObject m_supplyButton;

	// Token: 0x0400021C RID: 540
	private GameObject m_bkg;

	// Token: 0x0400021D RID: 541
	private GameObject m_guiCamera;

	// Token: 0x0400021E RID: 542
	private Camera m_gameCamera;

	// Token: 0x0400021F RID: 543
	private LineDrawer m_lineDrawer;

	// Token: 0x04000220 RID: 544
	private Ship m_ship;

	// Token: 0x04000221 RID: 545
	private int m_lineType;

	// Token: 0x04000222 RID: 546
	private bool m_canOrder;

	// Token: 0x04000223 RID: 547
	private bool m_localOwner;

	// Token: 0x04000224 RID: 548
	private ModuleButton m_dragging;

	// Token: 0x04000225 RID: 549
	private bool m_moveForward;

	// Token: 0x04000226 RID: 550
	private bool m_moveReverse;

	// Token: 0x04000227 RID: 551
	private bool m_moveRotate;

	// Token: 0x04000228 RID: 552
	private float m_lowestScreenPos;
}
