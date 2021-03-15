using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000006 RID: 6
[AddComponentMenu("Scripts/Gui/FleetEditor_PreviewArea")]
public class FleetEditor_PreviewArea : MonoBehaviour
{
	// Token: 0x06000053 RID: 83 RVA: 0x00003738 File Offset: 0x00001938
	private void Start()
	{
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0000373C File Offset: 0x0000193C
	private void Update()
	{
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00003740 File Offset: 0x00001940
	public void Clear()
	{
		this.GetAllClones().ForEach(delegate(GameObject child)
		{
			UnityEngine.Object.Destroy(child);
		});
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003778 File Offset: 0x00001978
	public void AddClone(GUI_Blueprint_Ship blueprint)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.m_GUIShip_prefab) as GameObject;
		GUI_Blueprint_Ship component = gameObject.GetComponent<GUI_Blueprint_Ship>();
		string text = blueprint.Name;
		int num = 0;
		foreach (GameObject gameObject2 in this.GetAllClones())
		{
			GUI_Blueprint_Ship component2 = gameObject2.GetComponent<GUI_Blueprint_Ship>();
			if (!(component2 == null))
			{
				string name = component2.Name;
				string name2 = blueprint.Name;
				bool flag = name.StartsWith(name2);
				bool flag2 = string.Compare(component2.Type, blueprint.Type) == 0;
				bool flag3 = component2.Cost == blueprint.Cost;
				if (flag && flag2 && flag3)
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			text = string.Format("{0}({1})", text, num + 1);
		}
		component.Initialize(blueprint.ShipDefinition, null);
		component.Name = text;
		component.AllowDragDrop = false;
		component.DisableEdit();
		component.EnableTrashButton(new GUI_Blueprint_Ship.OnTrashPressedDelegate(this.OnItemDelete));
		this.AddCloneAndUpdate(gameObject);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000038C8 File Offset: 0x00001AC8
	private void AddCloneAndUpdate(GameObject clone)
	{
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		int num = component.transform.childCount - 2;
		if (num < 0)
		{
			num = 0;
		}
		int num2 = (int)Math.Floor((double)num / (double)this.m_shipsPerRow);
		int num3 = num - num2 * this.m_shipsPerRow;
		if (num3 < 0)
		{
			num3 = 0;
		}
		clone.transform.position = Vector3.zero;
		clone.transform.localPosition = Vector3.zero;
		component.MakeChild(clone);
		clone.transform.position = new Vector3(this.CalcXPosOfPanelItemNr(num3), this.CalcYPosOfPanelNr(num2), -0.1f);
		if (this.m_onItemAdded != null)
		{
			this.m_onItemAdded(clone.GetComponent<GUI_Blueprint_Ship>().ShipDefinition, this.NumItems);
		}
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003990 File Offset: 0x00001B90
	private void OnItemDelete(GameObject target)
	{
		List<GameObject> allClones = this.GetAllClones();
		int num = allClones.IndexOf(target);
		if (num == -1)
		{
			return;
		}
		string name = target.GetComponent<GUI_Blueprint_Ship>().Name;
		StringUtils.TryRemoveCopyText(ref name);
		ShipDef shipDefinition = target.GetComponent<GUI_Blueprint_Ship>().ShipDefinition;
		UnityEngine.Object.Destroy(target);
		allClones.Remove(allClones[num]);
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		if (num < component.transform.childCount - 1)
		{
			int num2 = 0;
			foreach (GameObject gameObject in allClones)
			{
				if (num2 >= num)
				{
					int num3 = (int)Math.Floor((double)num2 / (double)this.m_shipsPerRow);
					int nr = num2 - num3 * this.m_shipsPerRow;
					GUI_Blueprint_Ship component2 = gameObject.GetComponent<GUI_Blueprint_Ship>();
					string text = component2.Name;
					bool flag = StringUtils.ContainsParanthesesAndNumber(text) && text.Contains(name);
					if (flag)
					{
						string text2 = StringUtils.ExtractCopyNumber(text, false);
						if (text2 == "2")
						{
							StringUtils.TryRemoveCopyText(ref text);
						}
						else
						{
							string str = (int.Parse(text2) - 1).ToString();
							text = text.Replace("(" + text2 + ")", "(" + str + ")");
						}
						component2.Name = text;
					}
					gameObject.transform.position = new Vector3(this.CalcXPosOfPanelItemNr(nr), this.CalcYPosOfPanelNr(num3), -0.1f);
				}
				num2++;
			}
		}
		if (this.m_onItemRemoved != null)
		{
			this.m_onItemRemoved(shipDefinition, component.transform.childCount - 3);
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003B78 File Offset: 0x00001D78
	private List<GameObject> GetAllClones()
	{
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in component.transform)
		{
			Transform transform = (Transform)obj;
			GameObject gameObject = transform.gameObject;
			GUI_Blueprint_Ship component2 = gameObject.GetComponent<GUI_Blueprint_Ship>();
			if (!(component2 == null))
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003C24 File Offset: 0x00001E24
	public List<ShipDef> GetAllShips()
	{
		List<ShipDef> list = new List<ShipDef>();
		foreach (GameObject gameObject in this.GetAllClones())
		{
			GUI_Blueprint_Ship component = gameObject.GetComponent<GUI_Blueprint_Ship>();
			if (component.ShipDefinition != null)
			{
				list.Add(component.ShipDefinition);
			}
		}
		return list;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00003CB0 File Offset: 0x00001EB0
	public List<ShipInstanceDef> GetAllShipsAsInstanceDefs()
	{
		List<ShipInstanceDef> list = new List<ShipInstanceDef>();
		foreach (GameObject gameObject in this.GetAllClones())
		{
			list.Add(new ShipInstanceDef(gameObject.GetComponent<GUI_Blueprint_Ship>().Name));
		}
		return list;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00003D2C File Offset: 0x00001F2C
	internal void Hide()
	{
		foreach (GameObject gameObject in this.GetAllClones())
		{
			gameObject.GetComponent<GUI_Blueprint_Ship>().Hide();
		}
		base.gameObject.SetActiveRecursively(false);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00003DA4 File Offset: 0x00001FA4
	internal void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		foreach (GameObject gameObject in this.GetAllClones())
		{
			gameObject.GetComponent<GUI_Blueprint_Ship>().Show();
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003E1C File Offset: 0x0000201C
	private float CalcRowLength()
	{
		float num = 0f;
		int num2 = this.m_shipsPerRow - 1;
		if (num2 > 0)
		{
			num = (float)num2 * 3f;
		}
		return (float)this.m_shipsPerRow * this.CloneWidth + num;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003E58 File Offset: 0x00002058
	private float CalcXPosOfPanelItemNr(int nr)
	{
		float num = (nr <= 0) ? 0f : (3f * (float)nr);
		return this.FirstItemCenterPos.position.x + num + (float)nr * this.CloneWidth;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003EA0 File Offset: 0x000020A0
	private float CalcYPosOfPanelNr(int nr)
	{
		float num = (nr <= 0) ? 0f : (6f * (float)nr);
		return this.FirstItemCenterPos.position.y - (num + (float)nr * this.CloneHeight);
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000061 RID: 97 RVA: 0x00003EE8 File Offset: 0x000020E8
	private Transform FirstItemCenterPos
	{
		get
		{
			Transform transform = base.gameObject.transform.Find("FirstItemCenterPos");
			if (transform != null)
			{
				return transform;
			}
			return base.transform;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000062 RID: 98 RVA: 0x00003F20 File Offset: 0x00002120
	public int NumItems
	{
		get
		{
			UIPanel component = base.gameObject.GetComponent<UIPanel>();
			return (!(component == null)) ? (component.transform.childCount - 1) : 0;
		}
	}

	// Token: 0x04000029 RID: 41
	private const float CloneSpacing = 3f;

	// Token: 0x0400002A RID: 42
	private const float PanelYSpacing = 6f;

	// Token: 0x0400002B RID: 43
	private float CloneWidth = 290f;

	// Token: 0x0400002C RID: 44
	private float CloneHeight = 140f;

	// Token: 0x0400002D RID: 45
	public int m_shipsPerRow = 2;

	// Token: 0x0400002E RID: 46
	public int m_maxTotalShips = 9;

	// Token: 0x0400002F RID: 47
	public GameObject m_GUIShip_prefab;

	// Token: 0x04000030 RID: 48
	public FleetEditor_PreviewArea.CollectionChanged m_onItemAdded;

	// Token: 0x04000031 RID: 49
	public FleetEditor_PreviewArea.CollectionChanged m_onItemRemoved;

	// Token: 0x02000193 RID: 403
	// (Invoke) Token: 0x06000EFC RID: 3836
	public delegate void CollectionChanged(ShipDef def, int numItemsInCollection);
}
