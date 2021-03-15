using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200000D RID: 13
[AddComponentMenu("Scripts/Gui/ListBox_Ships_Filler")]
public class ListBox_Ships_Filler : MonoBehaviour
{
	// Token: 0x060000AB RID: 171 RVA: 0x00004E98 File Offset: 0x00003098
	private void Start()
	{
	}

	// Token: 0x060000AC RID: 172 RVA: 0x00004E9C File Offset: 0x0000309C
	public void Initialize(List<ShipDef> ships, FleetMenu menu)
	{
		this.m_ships = ships;
		this.m_menu = menu;
		this.m_parent = base.gameObject.GetComponent<UIScrollList>();
		DebugUtils.Assert(this.m_parent != null, "ListBox_Ships_Filler script must be attached to an UIScrollList !");
		if (this.m_parent == null)
		{
			return;
		}
		DebugUtils.Assert(this.m_GUI_Prefab != null, "ListBox_Ships_Filler script has no GUI prefab !");
		if (this.m_GUI_Prefab == null)
		{
			return;
		}
		GUI_Blueprint_Ship component = this.m_GUI_Prefab.GetComponent<GUI_Blueprint_Ship>();
		DebugUtils.Assert(component != null, "ListBox_Ships_Filler script's prefab does not have a GUI_Blueprint_Ship-script !");
		if (component == null)
		{
			return;
		}
		if (this.m_ships != null && this.m_ships.Count > 0)
		{
			foreach (ShipDef def in this.m_ships)
			{
				this.Add(def);
			}
			this.m_parent.PositionItems();
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00004FC4 File Offset: 0x000031C4
	public void Add(ShipDef def)
	{
		GameObject itemGO;
		GUI_Blueprint_Ship gui_Blueprint_Ship = this.ShipDef_to_GUI(def, out itemGO);
		gui_Blueprint_Ship.DisableTrashButton();
		this.m_parent.AddItem(itemGO);
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00004FF0 File Offset: 0x000031F0
	public GUI_Blueprint_Ship ShipDef_to_GUI(ShipDef def, out GameObject obj)
	{
		obj = (UnityEngine.Object.Instantiate(this.m_GUI_Prefab) as GameObject);
		GUI_Blueprint_Ship component = obj.GetComponent<GUI_Blueprint_Ship>();
		component.Initialize(def, this.m_menu);
		return component;
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005028 File Offset: 0x00003228
	private void Update()
	{
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x0000502C File Offset: 0x0000322C
	public List<GUI_Blueprint_Ship> CreateListItems_From(List<ShipDef> definitions)
	{
		if (definitions == null || definitions.Count == 0)
		{
			return null;
		}
		List<GUI_Blueprint_Ship> list = new List<GUI_Blueprint_Ship>();
		foreach (ShipDef def in definitions)
		{
			GameObject obj;
			GUI_Blueprint_Ship item = this.ShipDef_to_GUI(def, out obj);
			list.Add(item);
			UnityEngine.Object.DestroyImmediate(obj);
		}
		return list;
	}

	// Token: 0x04000056 RID: 86
	private UIScrollList m_parent;

	// Token: 0x04000057 RID: 87
	private FleetMenu m_menu;

	// Token: 0x04000058 RID: 88
	public GameObject m_GUI_Prefab;

	// Token: 0x04000059 RID: 89
	public List<ShipDef> m_ships;
}
