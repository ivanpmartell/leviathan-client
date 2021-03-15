using System;
using PTech;
using UnityEngine;

// Token: 0x02000004 RID: 4
[AddComponentMenu("Scripts/Gui/GUI_Blueprint_Ship")]
public sealed class GUI_Blueprint_Ship : MonoBehaviour
{
	// Token: 0x06000022 RID: 34 RVA: 0x000029EC File Offset: 0x00000BEC
	private void Start()
	{
	}

	// Token: 0x06000023 RID: 35 RVA: 0x000029F0 File Offset: 0x00000BF0
	public void Initialize(ShipDef def, FleetMenu menu)
	{
		if (this.m_sectionsScale_X < 1f)
		{
			this.m_sectionsScale_X = 1f;
		}
		if (this.m_sectionsScale_Y < 1f)
		{
			this.m_sectionsScale_Y = 1f;
		}
		this.Initialize();
		this.ShipDefinition = def;
		this.m_menu = menu;
		this.DisableTrashButton();
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002A50 File Offset: 0x00000C50
	public void Initialize()
	{
		if (this.m_hasInitialized)
		{
			return;
		}
		DebugUtils.Assert(this.Validate_Parent(), "GUI_Blueprint_Ship failed to validate gameObject !");
		DebugUtils.Assert(this.Validate_NameLabel(), "GUI_Blueprint_Ship failed to validate label named NameLabel !");
		DebugUtils.Assert(this.Validate_TypeLabel(), "GUI_Blueprint_Ship failed to validate label named TypeLabel !");
		DebugUtils.Assert(this.Validate_CostLabel(), "GUI_Blueprint_Ship failed to validate label named CostLabel !");
		DebugUtils.Assert(this.Validate_ShipSections(), "GUI_Blueprint_Ship failed to validate transform named ShipParts !");
		DebugUtils.Assert(this.Validate_TrashButton(), "GUI_Blueprint_Ship failed to validate button named btnTrash !");
		this.btnEdit = GuiUtils.FindChildOf(base.transform, "btnEdit");
		this.btnEdit.GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnEditShipPressed));
		this.btnDelete = GuiUtils.FindChildOf(base.transform, "btnTrash");
		this.btnDelete.GetComponent<UIButton>().Text = Localize.instance.Translate(this.btnDelete.GetComponent<UIButton>().Text);
		this.btnEdit.GetComponent<UIButton>().Text = Localize.instance.Translate(this.btnEdit.GetComponent<UIButton>().Text);
		GuiUtils.ValidateSimpelSprite(base.gameObject, "Icon", out this.m_sprIcon);
		this.m_UIListItem_Component.AddDragDropDelegate(new EZDragDropDelegate(this.OnDragDropped));
		this.m_hasInitialized = true;
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00002B9C File Offset: 0x00000D9C
	public void DisableEdit()
	{
		this.btnEdit.SetActiveRecursively(false);
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002BAC File Offset: 0x00000DAC
	private void OnEditShipPressed(IUIObject obj)
	{
		PLog.LogWarning("EDIT SHIP " + this.m_definition.m_name);
		if (this.m_menu != null)
		{
			this.m_menu.OnShipSelectet(this.m_definition);
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002BF0 File Offset: 0x00000DF0
	public void EnableTrashButton(GUI_Blueprint_Ship.OnTrashPressedDelegate del)
	{
		this.m_btnTrash.SetActiveRecursively(true);
		this.m_btnTrash.active = true;
		foreach (Renderer renderer in this.m_btnTrash.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = true;
		}
		this.m_btnTrash.renderer.enabled = true;
		UIButton component = this.m_btnTrash.GetComponent<UIButton>();
		component.controlIsEnabled = true;
		component.Hide(false);
		component.SetSize(50f, 50f);
		if (del != null)
		{
			this.m_trashPressedDelegate = del;
		}
		component.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnTrashButtonPressed));
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002C9C File Offset: 0x00000E9C
	public void DisableTrashButton()
	{
		this.m_btnTrash.SetActiveRecursively(false);
		this.m_btnTrash.active = false;
		this.m_btnTrash.renderer.enabled = false;
		foreach (Renderer renderer in this.m_btnTrash.GetComponentsInChildren<Renderer>())
		{
			renderer.enabled = false;
		}
		UIButton component = this.m_btnTrash.GetComponent<UIButton>();
		component.controlIsEnabled = false;
		component.Hide(true);
		component.enabled = false;
		component.SetSize(0f, 0f);
		this.m_trashPressedDelegate = null;
		component.RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnTrashButtonPressed));
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002D48 File Offset: 0x00000F48
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

	// Token: 0x0600002A RID: 42 RVA: 0x00002DFC File Offset: 0x00000FFC
	private void OnTrashButtonPressed(IUIObject ignore)
	{
		if (this.m_trashPressedDelegate != null)
		{
			this.m_trashPressedDelegate(base.gameObject);
		}
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002E1C File Offset: 0x0000101C
	private bool HandleDrop(GameObject target)
	{
		if (string.Compare(target.name.ToLower().Trim(), "fleeteditor_previewarea") != 0)
		{
			return false;
		}
		FleetEditor_PreviewArea component = target.GetComponent<FleetEditor_PreviewArea>();
		if (component == null)
		{
			Debug.LogWarning("Trying to interact with object named \"FleetEditor_PreviewArea\" that does not have the correct script !");
			return false;
		}
		component.AddClone(this);
		return true;
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002E74 File Offset: 0x00001074
	[Obsolete("This does not scale all the textures of the children properly. Don't use it untill solved.")]
	private void SetSizes(float width, float height)
	{
		if (this.m_UIListItem_Component != null)
		{
			this.m_UIListItem_Component.SetSize(width, height);
		}
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002E94 File Offset: 0x00001094
	private void UpdateIconSprite()
	{
		if (this.m_sectionsContainer != null)
		{
			SimpleSprite component = this.m_sprIcon.GetComponent<SimpleSprite>();
			float width = component.width;
			float height = component.height;
			component.SetTexture(this.m_iconTexture);
			float x = 64f;
			float y = 64f;
			if (this.m_iconTexture != null)
			{
				x = (float)this.m_iconTexture.width;
				y = (float)this.m_iconTexture.height;
			}
			component.Setup(width, height, new Vector2(0f, y), new Vector2(x, y));
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002F30 File Offset: 0x00001130
	public void OnTap()
	{
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002F34 File Offset: 0x00001134
	public void OnPress()
	{
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002F38 File Offset: 0x00001138
	public void OnRelease()
	{
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002F3C File Offset: 0x0000113C
	public void OnMove()
	{
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002F40 File Offset: 0x00001140
	private bool Validate_NameLabel()
	{
		Transform transform = base.transform.Find("NameLabel");
		this.m_lblName = ((!(transform == null)) ? transform.gameObject : null);
		DebugUtils.Assert(this.m_lblName != null, "GUI_Blueprint_Ship script must be attached to a GameObject that has an GameObject named \"NameLabel\"");
		if (this.m_lblName == null)
		{
			return false;
		}
		bool flag = this.m_lblName.GetComponent<SpriteText>() != null;
		DebugUtils.Assert(flag, "GUI_Blueprint_Ship LabelName is not a SpriteText !");
		return flag;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002FC8 File Offset: 0x000011C8
	private bool Validate_TypeLabel()
	{
		Transform transform = base.transform.Find("TypeLabel");
		this.m_lblType = ((!(transform == null)) ? transform.gameObject : null);
		DebugUtils.Assert(this.m_lblType != null, "GUI_Blueprint_Ship script must be attached to a GameObject that has an GameObject named \"TypeLabel\"");
		if (this.m_lblType == null)
		{
			return false;
		}
		bool flag = this.m_lblType.GetComponent<SpriteText>() != null;
		DebugUtils.Assert(flag, "GUI_Blueprint_Ship TypeLabel is not a SpriteText !");
		return flag;
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00003050 File Offset: 0x00001250
	private bool Validate_CostLabel()
	{
		Transform transform = base.transform.Find("CostLabel");
		this.m_lblCost = ((!(transform == null)) ? transform.gameObject : null);
		DebugUtils.Assert(this.m_lblCost != null, "GUI_Blueprint_Ship script must be attached to a GameObject that has an GameObject named \"CostLabel\"");
		if (this.m_lblCost == null)
		{
			return false;
		}
		bool flag = this.m_lblCost.GetComponent<SpriteText>() != null;
		DebugUtils.Assert(flag, "GUI_Blueprint_Ship CostLabel is not a SpriteText !");
		return flag;
	}

	// Token: 0x06000035 RID: 53 RVA: 0x000030D8 File Offset: 0x000012D8
	private bool Validate_Parent()
	{
		this.m_UIListItem_Component = base.gameObject.GetComponent<UIListItem>();
		DebugUtils.Assert(this.m_UIListItem_Component != null, "GUI_Blueprint_Ship script must be attached to a GameObject that has an IUIListItem");
		return this.m_UIListItem_Component != null;
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003118 File Offset: 0x00001318
	private bool Validate_ShipSections()
	{
		Transform transform = base.transform.Find("ShipParts");
		this.m_sectionsContainer = ((!(transform == null)) ? transform.gameObject : null);
		DebugUtils.Assert(this.m_sectionsContainer != null, "GUI_Blueprint_Ship script must be attached to a GameObject that has an GameObject named \"ShipParts\"");
		if (this.m_sectionsContainer == null)
		{
			return false;
		}
		GameObject gameObject = this.m_sectionsContainer.transform.GetChild(0).gameObject;
		bool flag = gameObject.GetComponent<SimpleSprite>() != null;
		DebugUtils.Assert(flag, "GUI_Blueprint_Ship detects that ShipPart 1 isn't a SimpleSprite !");
		return flag;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000031B8 File Offset: 0x000013B8
	private bool Validate_TrashButton()
	{
		return this.ValidateTransform("btnTrash", out this.m_btnTrash) && !(this.m_btnTrash == null) && this.m_btnTrash.GetComponent<UIButton>() != null;
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00003204 File Offset: 0x00001404
	private bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = base.gameObject.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000039 RID: 57 RVA: 0x00003240 File Offset: 0x00001440
	// (set) Token: 0x0600003A RID: 58 RVA: 0x00003248 File Offset: 0x00001448
	public ShipDef ShipDefinition
	{
		get
		{
			return this.m_definition;
		}
		private set
		{
			if (this.m_definition == value)
			{
				return;
			}
			this.m_definition = value;
			if (this.m_definition != null)
			{
				this.Cost = this.m_definition.m_value;
				this.Name = this.m_definition.m_name.Trim();
				this.Type = string.Empty;
				GameObject prefab = ObjectFactory.instance.GetPrefab(this.m_definition.m_prefab);
				Ship component = prefab.GetComponent<Ship>();
				DebugUtils.Assert(component != null, "Failed to get icon from prefab in ShipDef !");
				Texture2D icon = component.m_icon;
				if (icon == null)
				{
					Debug.LogWarning("Failed to get icon from prefab in ShipDef !");
				}
				this.IconTexture = icon;
			}
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600003B RID: 59 RVA: 0x000032F8 File Offset: 0x000014F8
	// (set) Token: 0x0600003C RID: 60 RVA: 0x00003300 File Offset: 0x00001500
	public int Cost
	{
		get
		{
			return this.m_cost;
		}
		private set
		{
			if (this.m_cost == value)
			{
				return;
			}
			this.m_cost = value;
			if (this.m_lblCost != null)
			{
				this.m_lblCost.GetComponent<SpriteText>().Text = this.Cost.ToString();
			}
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600003D RID: 61 RVA: 0x00003350 File Offset: 0x00001550
	// (set) Token: 0x0600003E RID: 62 RVA: 0x00003358 File Offset: 0x00001558
	public string Name
	{
		get
		{
			return this.m_name;
		}
		set
		{
			this.m_name = value;
			if (string.IsNullOrEmpty(this.m_name))
			{
				this.m_name = "Undefined";
			}
			this.m_lblName.GetComponent<SpriteText>().Text = this.m_name;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600003F RID: 63 RVA: 0x000033A0 File Offset: 0x000015A0
	// (set) Token: 0x06000040 RID: 64 RVA: 0x000033A8 File Offset: 0x000015A8
	public string Type
	{
		get
		{
			return this.m_type;
		}
		set
		{
			this.m_type = value;
			this.m_lblType.GetComponent<SpriteText>().Text = this.m_type;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000041 RID: 65 RVA: 0x000033C8 File Offset: 0x000015C8
	// (set) Token: 0x06000042 RID: 66 RVA: 0x000033E8 File Offset: 0x000015E8
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

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000043 RID: 67 RVA: 0x00003408 File Offset: 0x00001608
	private float ListItemWidth
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.width : 0f;
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000044 RID: 68 RVA: 0x0000343C File Offset: 0x0000163C
	private float ListItemHeight
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.height : 0f;
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000045 RID: 69 RVA: 0x00003470 File Offset: 0x00001670
	private Vector3 ListItemPosition
	{
		get
		{
			return (!(this.m_UIListItem_Component == null)) ? this.m_UIListItem_Component.transform.position : Vector3.zero;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000046 RID: 70 RVA: 0x000034A0 File Offset: 0x000016A0
	public UIListItem UIListItemComponent
	{
		get
		{
			return this.m_UIListItem_Component;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000047 RID: 71 RVA: 0x000034A8 File Offset: 0x000016A8
	// (set) Token: 0x06000048 RID: 72 RVA: 0x000034B0 File Offset: 0x000016B0
	public Texture2D IconTexture
	{
		get
		{
			return this.m_iconTexture;
		}
		set
		{
			this.m_iconTexture = value;
			this.UpdateIconSprite();
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x000034C0 File Offset: 0x000016C0
	internal void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x0600004A RID: 74 RVA: 0x000034D0 File Offset: 0x000016D0
	internal void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		this.DisableEdit();
	}

	// Token: 0x04000013 RID: 19
	private const bool DEBUG = true;

	// Token: 0x04000014 RID: 20
	public GUI_Blueprint_Ship.OnTrashPressedDelegate m_trashPressedDelegate;

	// Token: 0x04000015 RID: 21
	internal Texture2D m_iconTexture;

	// Token: 0x04000016 RID: 22
	public float m_sectionsScale_X = 1.8f;

	// Token: 0x04000017 RID: 23
	public float m_sectionsScale_Y = 2.2f;

	// Token: 0x04000018 RID: 24
	private FleetMenu m_menu;

	// Token: 0x04000019 RID: 25
	private int m_cost;

	// Token: 0x0400001A RID: 26
	private ShipDef m_definition;

	// Token: 0x0400001B RID: 27
	private string m_name = "Undefined";

	// Token: 0x0400001C RID: 28
	private string m_type = "Undefined";

	// Token: 0x0400001D RID: 29
	private GameObject m_sprIcon;

	// Token: 0x0400001E RID: 30
	private GameObject btnEdit;

	// Token: 0x0400001F RID: 31
	private GameObject btnDelete;

	// Token: 0x04000020 RID: 32
	private UIListItem m_UIListItem_Component;

	// Token: 0x04000021 RID: 33
	private GameObject m_lblName;

	// Token: 0x04000022 RID: 34
	private GameObject m_lblType;

	// Token: 0x04000023 RID: 35
	private GameObject m_lblCost;

	// Token: 0x04000024 RID: 36
	private GameObject m_sectionsContainer;

	// Token: 0x04000025 RID: 37
	private GameObject m_btnTrash;

	// Token: 0x04000026 RID: 38
	private bool m_hasInitialized;

	// Token: 0x02000192 RID: 402
	// (Invoke) Token: 0x06000EF8 RID: 3832
	public delegate void OnTrashPressedDelegate(GameObject target);
}
