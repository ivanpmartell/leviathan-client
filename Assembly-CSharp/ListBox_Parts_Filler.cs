using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200000C RID: 12
[AddComponentMenu("Scripts/Gui/ListBox_Parts_Filler")]
public class ListBox_Parts_Filler : MonoBehaviour
{
	// Token: 0x060000A4 RID: 164 RVA: 0x00004C74 File Offset: 0x00002E74
	private void Start()
	{
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004C78 File Offset: 0x00002E78
	public void Initialize(List<string> ships, ShipMenu menu)
	{
		this.m_parts = ships;
		this.m_menu = menu;
		this.m_parent = base.gameObject.GetComponent<UIScrollList>();
		DebugUtils.Assert(this.m_parent != null, "ListBox_Parts_Filler script must be attached to an UIScrollList !");
		if (this.m_parent == null)
		{
			return;
		}
		DebugUtils.Assert(this.m_GUI_Prefab != null, "ListBox_Parts_Filler script has no GUI prefab !");
		if (this.m_GUI_Prefab == null)
		{
			return;
		}
		GUI_Blueprint_Part component = this.m_GUI_Prefab.GetComponent<GUI_Blueprint_Part>();
		DebugUtils.Assert(component != null, "ListBox_Parts_Filler script's prefab does not have a GUI_Blueprint_Part-script !");
		if (component == null)
		{
			return;
		}
		if (this.m_parts != null && this.m_parts.Count > 0)
		{
			foreach (string def in this.m_parts)
			{
				this.Add(def);
			}
			this.m_parent.PositionItems();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00004DA0 File Offset: 0x00002FA0
	public void Add(string def)
	{
		GameObject itemGO;
		this.Part_to_GUI(def, out itemGO);
		this.m_parent.AddItem(itemGO);
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00004DC4 File Offset: 0x00002FC4
	public GUI_Blueprint_Part Part_to_GUI(string def, out GameObject obj)
	{
		obj = (UnityEngine.Object.Instantiate(this.m_GUI_Prefab) as GameObject);
		GUI_Blueprint_Part component = obj.GetComponent<GUI_Blueprint_Part>();
		component.Initialize(def, this.m_menu);
		return component;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00004DFC File Offset: 0x00002FFC
	private void Update()
	{
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00004E00 File Offset: 0x00003000
	public List<GUI_Blueprint_Part> CreateListItems_From(List<string> definitions)
	{
		if (definitions == null || definitions.Count == 0)
		{
			return null;
		}
		List<GUI_Blueprint_Part> list = new List<GUI_Blueprint_Part>();
		foreach (string def in definitions)
		{
			GameObject obj;
			GUI_Blueprint_Part item = this.Part_to_GUI(def, out obj);
			list.Add(item);
			UnityEngine.Object.DestroyImmediate(obj);
		}
		return list;
	}

	// Token: 0x04000052 RID: 82
	private UIScrollList m_parent;

	// Token: 0x04000053 RID: 83
	private ShipMenu m_menu;

	// Token: 0x04000054 RID: 84
	public GameObject m_GUI_Prefab;

	// Token: 0x04000055 RID: 85
	private List<string> m_parts;
}
