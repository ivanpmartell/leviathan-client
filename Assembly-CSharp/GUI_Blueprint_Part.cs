using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000003 RID: 3
[AddComponentMenu("Scripts/Gui/GUI_Blueprint_Part")]
public sealed class GUI_Blueprint_Part : MonoBehaviour
{
	// Token: 0x0600000C RID: 12 RVA: 0x00002368 File Offset: 0x00000568
	private void Start()
	{
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000236C File Offset: 0x0000056C
	public void Initialize(string name, ShipMenu menu)
	{
		this.m_name = name;
		this.m_menu = menu;
	}

	// Token: 0x0600000E RID: 14 RVA: 0x0000237C File Offset: 0x0000057C
	public void Initialize()
	{
		if (this.m_hasInitialized)
		{
			return;
		}
		base.gameObject.GetComponent<UIListItem>().AddInputDelegate(new EZInputDelegate(this.PartsDelegate));
		GuiUtils.ValidateGuLabel(base.gameObject, "lblName", out this.m_lblName);
		GuiUtils.ValidateGuLabel(base.gameObject, "lblInfo", out this.m_lblInfo);
		GuiUtils.ValidateGuLabel(base.gameObject, "lblSize", out this.m_lblSize);
		GuiUtils.ValidateGuLabel(base.gameObject, "lblCost", out this.lblCost);
		GuiUtils.ValidateSimpelSprite(base.gameObject, "Icon", out this.m_sprIcon);
		this.m_hasInitialized = true;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000242C File Offset: 0x0000062C
	private void PartsDelegate(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG && Input.mousePosition.x > 300f)
		{
			ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
			return;
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE)
		{
			return;
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.NO_CHANGE)
		{
			return;
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002480 File Offset: 0x00000680
	private void FillData()
	{
		GameObject prefab = ObjectFactory.instance.GetPrefab(this.m_name);
		if (prefab == null)
		{
			this.m_lblName.GetComponent<SpriteText>().Text = "DEV: " + this.m_name;
			this.m_lblInfo.GetComponent<SpriteText>().Text = "DO NOT USE";
			this.m_lblSize.GetComponent<SpriteText>().Text = "DO NOT USE";
			return;
		}
		Section component = prefab.GetComponent<Section>();
		if (component != null)
		{
			SectionSettings section = ComponentDB.instance.GetSection(this.m_name);
			DebugUtils.Assert(section != null);
			this.m_lblName.GetComponent<SpriteText>().Text = Localize.instance.Translate(component.GetName());
			this.lblCost.GetComponent<SpriteText>().Text = section.m_value.ToString();
			if (component.m_GUITexture != null)
			{
				SimpleSprite component2 = this.m_sprIcon.GetComponent<SimpleSprite>();
				float width = component2.width;
				float height = component2.height;
				component2.SetTexture(component.m_GUITexture);
				float x = 64f;
				float y = 64f;
				if (component.m_GUITexture != null)
				{
					x = (float)component.m_GUITexture.width;
					y = (float)component.m_GUITexture.height;
				}
				SimpleSprite component3 = this.m_sprIcon.GetComponent<SimpleSprite>();
				component3.Setup(width, height, new Vector2(0f, y), new Vector2(x, y));
				component3.SetTexture(component.m_GUITexture);
				component3.UpdateUVs();
			}
		}
		HPModule component4 = prefab.GetComponent<HPModule>();
		if (component4 != null)
		{
			HPModuleSettings module = ComponentDB.instance.GetModule(this.m_name);
			DebugUtils.Assert(module != null);
			List<string> hardpointInfo = component4.GetHardpointInfo();
			string text = component4.m_width.ToString() + "x" + component4.m_length.ToString();
			this.m_lblName.GetComponent<SpriteText>().Text = component4.GetName();
			this.lblCost.GetComponent<SpriteText>().Text = module.m_value.ToString();
			this.m_lblSize.GetComponent<SpriteText>().Text = text;
			if (hardpointInfo.Count >= 1)
			{
				this.m_lblInfo.GetComponent<SpriteText>().Text = hardpointInfo[0];
			}
			if (component4.m_GUITexture != null)
			{
				SimpleSprite component5 = this.m_sprIcon.GetComponent<SimpleSprite>();
				component5.SetTexture(component4.m_GUITexture);
				float width2 = component5.width;
				float height2 = component5.height;
				float x2 = 64f;
				float y2 = 64f;
				if (component4.m_GUITexture != null)
				{
					x2 = (float)component4.m_GUITexture.width;
					y2 = (float)component4.m_GUITexture.height;
				}
				SimpleSprite component6 = this.m_sprIcon.GetComponent<SimpleSprite>();
				component6.Setup(width2, height2, new Vector2(0f, y2), new Vector2(x2, y2));
				component6.SetTexture(component4.m_GUITexture);
				component6.UpdateUVs();
			}
		}
	}

	// Token: 0x06000011 RID: 17 RVA: 0x000027A0 File Offset: 0x000009A0
	private void OnDragDropped(EZDragDropParams dropParams)
	{
		if (dropParams.dragObj == null)
		{
			return;
		}
		bool flag = string.Compare(dropParams.evt.ToString().Trim().ToLower(), "dropped") == 0;
		if (flag)
		{
			if (dropParams.dragObj.DropTarget == null)
			{
				dropParams.dragObj.DropHandled = false;
				dropParams.dragObj.CancelDrag();
				return;
			}
			dropParams.dragObj.DropHandled = this.HandleDrop(dropParams.dragObj.DropTarget);
			dropParams.dragObj.CancelDrag();
		}
		dropParams.dragObj.DropHandled = true;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002854 File Offset: 0x00000A54
	private bool HandleDrop(GameObject target)
	{
		return false;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002858 File Offset: 0x00000A58
	[Obsolete("This does not scale all the textures of the children properly. Don't use it untill solved.")]
	private void SetSizes(float width, float height)
	{
		if (this.m_UIListItem_Component != null)
		{
			this.m_UIListItem_Component.SetSize(width, height);
		}
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002878 File Offset: 0x00000A78
	public void OnTap()
	{
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000287C File Offset: 0x00000A7C
	public void OnPress()
	{
		this.m_menu.SetPart(this.m_name, true);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002890 File Offset: 0x00000A90
	public void OnPressInfo()
	{
		this.m_menu.ShowInfo(this.m_name);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x000028A4 File Offset: 0x00000AA4
	public void OnRelease()
	{
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000028A8 File Offset: 0x00000AA8
	public void OnMove()
	{
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000019 RID: 25 RVA: 0x000028AC File Offset: 0x00000AAC
	// (set) Token: 0x0600001A RID: 26 RVA: 0x000028CC File Offset: 0x00000ACC
	public bool AllowDragDrop
	{
		get
		{
			return this.m_UIListItem_Component != null && this.m_UIListItem_Component.IsDraggable;
		}
		set
		{
			if (this.m_UIListItem_Component != null)
			{
				this.m_UIListItem_Component.IsDraggable = value;
			}
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600001B RID: 27 RVA: 0x000028EC File Offset: 0x00000AEC
	private float ListItemWidth
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.width : 0f;
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600001C RID: 28 RVA: 0x00002920 File Offset: 0x00000B20
	private float ListItemHeight
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.height : 0f;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600001D RID: 29 RVA: 0x00002954 File Offset: 0x00000B54
	private Vector3 ListItemPosition
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.transform.position : Vector3.zero;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600001E RID: 30 RVA: 0x00002984 File Offset: 0x00000B84
	public UIListItem UIListItemComponent
	{
		get
		{
			return this.m_UIListItem_Component;
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x0000298C File Offset: 0x00000B8C
	internal void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x06000020 RID: 32 RVA: 0x0000299C File Offset: 0x00000B9C
	internal void Show()
	{
		base.gameObject.SetActiveRecursively(true);
	}

	// Token: 0x04000009 RID: 9
	public GUI_Blueprint_Part.OnTrashPressedDelegate m_trashPressedDelegate;

	// Token: 0x0400000A RID: 10
	private ShipMenu m_menu;

	// Token: 0x0400000B RID: 11
	private string m_name = "Undefined";

	// Token: 0x0400000C RID: 12
	private UIListItem m_UIListItem_Component;

	// Token: 0x0400000D RID: 13
	private GameObject m_lblName;

	// Token: 0x0400000E RID: 14
	private GameObject m_lblInfo;

	// Token: 0x0400000F RID: 15
	private GameObject m_lblSize;

	// Token: 0x04000010 RID: 16
	private GameObject lblCost;

	// Token: 0x04000011 RID: 17
	private GameObject m_sprIcon;

	// Token: 0x04000012 RID: 18
	private bool m_hasInitialized;

	// Token: 0x02000191 RID: 401
	// (Invoke) Token: 0x06000EF4 RID: 3828
	public delegate void OnTrashPressedDelegate(GameObject target);
}
