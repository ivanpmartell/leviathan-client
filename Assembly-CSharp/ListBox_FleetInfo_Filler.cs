using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
[AddComponentMenu("Scripts/Gui/ListBox_FleetInfo_Filler")]
public class ListBox_FleetInfo_Filler : MonoBehaviour
{
	// Token: 0x0600008F RID: 143 RVA: 0x000047B8 File Offset: 0x000029B8
	private void Start()
	{
	}

	// Token: 0x06000090 RID: 144 RVA: 0x000047BC File Offset: 0x000029BC
	private void Update()
	{
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000047C0 File Offset: 0x000029C0
	public void Initialize()
	{
		DebugUtils.Assert(this.Validate_ScrollList(), "ListBox_FleetInfo_Filler must be put on a GameObject with a UIScrollList-script !");
		this.ValidatePrefab();
		this.ScrollList.AddValueChangedDelegate(new EZValueChangedDelegate(this.ScrollListValueChanged));
	}

	// Token: 0x06000092 RID: 146 RVA: 0x000047FC File Offset: 0x000029FC
	private void ScrollListValueChanged(IUIObject obj)
	{
		string selectedItemsName = this.GetSelectedItemsName();
		if (this.m_onFleetChangedDelegate != null)
		{
			this.m_onFleetChangedDelegate(selectedItemsName);
		}
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004828 File Offset: 0x00002A28
	public void Hide()
	{
		this.ScrollList.SetSelectedItem(-1);
		this.ScrollList.ClearList(true);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x00004850 File Offset: 0x00002A50
	public void Clear()
	{
		this.ScrollList.ClearList(true);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00004860 File Offset: 0x00002A60
	public GameObject AddItem(string name, string size, string date)
	{
		name = name.Trim();
		size = size.Trim();
		date = date.Trim();
		if (this.prefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.prefab) as GameObject;
			this.SetDate(this.SetSize(this.SetName(gameObject, name), size), date);
			this.ScrollList.AddItem(gameObject);
			this.ScrollList.PositionItems();
			this.ScrollList.LateUpdate();
			return gameObject;
		}
		return null;
	}

	// Token: 0x06000096 RID: 150 RVA: 0x000048E4 File Offset: 0x00002AE4
	public string GetSelectedItemsName()
	{
		IUIObject lastClickedControl = this.ScrollList.LastClickedControl;
		if (lastClickedControl == null || lastClickedControl.gameObject == null)
		{
			return string.Empty;
		}
		if (!lastClickedControl.controlIsEnabled)
		{
			return string.Empty;
		}
		return lastClickedControl.transform.FindChild("lblName").gameObject.GetComponent<SpriteText>().Text;
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000494C File Offset: 0x00002B4C
	public void UnSelect()
	{
		this.ScrollList.SetSelectedItem(-1);
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0000495C File Offset: 0x00002B5C
	internal void RemoveFleet(string fleetNameToRemove)
	{
		int num = -1;
		for (int i = 0; i < this.ScrollList.Count; i++)
		{
			GameObject gameObject = this.ScrollList.GetItem(i).gameObject;
			if (!(gameObject.GetComponent<UIListItemContainer>() == null))
			{
				Transform transform = gameObject.transform.FindChild("bkg/lblName");
				if (!(transform == null))
				{
					GameObject gameObject2 = transform.gameObject;
					if (!(gameObject2 == null))
					{
						SpriteText component = gameObject2.GetComponent<SpriteText>();
						if (!(component == null))
						{
							if (string.Compare(component.Text, fleetNameToRemove) == 0)
							{
								num = i;
								break;
							}
						}
					}
				}
			}
		}
		if (num < 0)
		{
			Debug.LogWarning(string.Format("ListBox_FleetInfo_Filler::RemoveFleet( {0} ) Failed to find object to remove", fleetNameToRemove));
			return;
		}
		this.ScrollList.RemoveItem(num, true);
		this.ScrollList.PositionItems();
		this.ScrollList.LateUpdate();
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00004A5C File Offset: 0x00002C5C
	private GameObject SetName(GameObject O, string name)
	{
		O.transform.FindChild("bkg/lblName").gameObject.GetComponent<SpriteText>().Text = name;
		return O;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00004A8C File Offset: 0x00002C8C
	private GameObject SetSize(GameObject O, string size)
	{
		O.transform.FindChild("bkg/lblSize").gameObject.GetComponent<SpriteText>().Text = size;
		return O;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00004ABC File Offset: 0x00002CBC
	private GameObject SetDate(GameObject O, string date)
	{
		O.transform.FindChild("bkg/lblDate").gameObject.GetComponent<SpriteText>().Text = date;
		return O;
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00004AEC File Offset: 0x00002CEC
	private void ValidatePrefab()
	{
		this.prefab = (Resources.Load("gui/FleetInfoListItem", typeof(GameObject)) as GameObject);
		DebugUtils.Assert(this.prefab != null, "ListBox_FleetInfo_Filler failed to locate the prefab \"gui/FleetInfoListItem\" in Resources !");
		DebugUtils.Assert(this.Validate_lblName(), "ListBox_FleetInfo_Filler failed to validate label named lblName !");
		DebugUtils.Assert(this.Validate_lblSize(), "ListBox_FleetInfo_Filler failed to validate label named lblSize !");
		DebugUtils.Assert(this.Validate_lblDate(), "ListBox_FleetInfo_Filler failed to validate label named lblDate !");
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00004B60 File Offset: 0x00002D60
	private bool Validate_ScrollList()
	{
		this.ScrollList = base.gameObject.GetComponent<UIScrollList>();
		return this.ScrollList != null;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00004B8C File Offset: 0x00002D8C
	private bool Validate_lblName()
	{
		GameObject gameObject = this.prefab.transform.FindChild("bkg/lblName").gameObject;
		return !(gameObject == null) && gameObject.GetComponent<SpriteText>() != null;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00004BD0 File Offset: 0x00002DD0
	private bool Validate_lblSize()
	{
		GameObject gameObject = this.prefab.transform.FindChild("bkg/lblSize").gameObject;
		return !(gameObject == null) && gameObject.GetComponent<SpriteText>() != null;
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00004C14 File Offset: 0x00002E14
	private bool Validate_lblDate()
	{
		GameObject gameObject = this.prefab.transform.FindChild("bkg/lblDate").gameObject;
		return !(gameObject == null) && gameObject.GetComponent<SpriteText>() != null;
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004C58 File Offset: 0x00002E58
	// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004C60 File Offset: 0x00002E60
	public UIScrollList ScrollList
	{
		get
		{
			return this.lstThis;
		}
		private set
		{
			this.lstThis = value;
		}
	}

	// Token: 0x0400004F RID: 79
	private UIScrollList lstThis;

	// Token: 0x04000050 RID: 80
	private GameObject prefab;

	// Token: 0x04000051 RID: 81
	public ListBox_FleetInfo_Filler.OnFleetChanged m_onFleetChangedDelegate;

	// Token: 0x02000199 RID: 409
	// (Invoke) Token: 0x06000F14 RID: 3860
	public delegate void OnFleetChanged(string fleetName);
}
