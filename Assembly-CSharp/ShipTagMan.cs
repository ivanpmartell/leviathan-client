using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000066 RID: 102
internal class ShipTagMan
{
	// Token: 0x0600047D RID: 1149 RVA: 0x00026C94 File Offset: 0x00024E94
	public ShipTagMan(GameObject guiCamera)
	{
		this.m_guiCamera = guiCamera;
		Unit.m_onCreated = (Action<Unit>)Delegate.Remove(Unit.m_onCreated, new Action<Unit>(this.OnUnitCreated));
		Unit.m_onRemoved = (Action<Unit>)Delegate.Remove(Unit.m_onRemoved, new Action<Unit>(this.OnUnitRemoved));
		Unit.m_onCreated = (Action<Unit>)Delegate.Combine(Unit.m_onCreated, new Action<Unit>(this.OnUnitCreated));
		Unit.m_onRemoved = (Action<Unit>)Delegate.Combine(Unit.m_onRemoved, new Action<Unit>(this.OnUnitRemoved));
		this.SetupPrefabList();
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00026D54 File Offset: 0x00024F54
	public void SetGameType(GameType type)
	{
		this.m_gameType = type;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00026D60 File Offset: 0x00024F60
	public void Close()
	{
		Unit.m_onCreated = (Action<Unit>)Delegate.Remove(Unit.m_onCreated, new Action<Unit>(this.OnUnitCreated));
		Unit.m_onRemoved = (Action<Unit>)Delegate.Remove(Unit.m_onRemoved, new Action<Unit>(this.OnUnitRemoved));
		foreach (ShipTag shipTag in this.m_tags)
		{
			shipTag.Close();
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00026E08 File Offset: 0x00025008
	public void Update(Camera camera, float dt)
	{
		int localPlayer = NetObj.GetLocalPlayer();
		int playerTeam = TurnMan.instance.GetPlayerTeam(localPlayer);
		foreach (ShipTag shipTag in this.m_tags)
		{
			shipTag.Update(camera, localPlayer, playerTeam);
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00026E84 File Offset: 0x00025084
	private void OnUnitCreated(Unit unit)
	{
		if (unit.m_shipTag == Unit.ShipTagType.None)
		{
			return;
		}
		ShipTag shipTag = new ShipTag(unit, this.m_guiCamera, this.m_gameType, this.m_statusIconPrefabs);
		shipTag.SetVisible(this.m_visible);
		this.m_tags.Add(shipTag);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00026ED0 File Offset: 0x000250D0
	private void OnUnitRemoved(Unit unit)
	{
		for (int i = 0; i < this.m_tags.Count; i++)
		{
			if (this.m_tags[i].GetUnit() == unit)
			{
				this.m_tags[i].Close();
				this.m_tags.RemoveAt(i);
				break;
			}
		}
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00026F38 File Offset: 0x00025138
	public void SetVisible(bool visible)
	{
		this.m_visible = visible;
		foreach (ShipTag shipTag in this.m_tags)
		{
			shipTag.SetVisible(visible);
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00026FA8 File Offset: 0x000251A8
	private void SetupPrefabList()
	{
		for (int i = 0; i < 7; i++)
		{
			this.m_statusIconPrefabs.Add(null);
		}
		this.m_statusIconPrefabs[0] = (Resources.Load("gui/ShipTagStatusListItems/StatusHPDisabledListItem") as GameObject);
		this.m_statusIconPrefabs[1] = (Resources.Load("gui/ShipTagStatusListItems/StatusMoveDisabledListItem") as GameObject);
		this.m_statusIconPrefabs[2] = (Resources.Load("gui/ShipTagStatusListItems/StatusOOCListItem") as GameObject);
		this.m_statusIconPrefabs[3] = (Resources.Load("gui/ShipTagStatusListItems/StatusRepairListItem") as GameObject);
		this.m_statusIconPrefabs[4] = (Resources.Load("gui/ShipTagStatusListItems/StatusSinkListItem") as GameObject);
		this.m_statusIconPrefabs[5] = (Resources.Load("gui/ShipTagStatusListItems/StatusViewDisabledListItem") as GameObject);
		this.m_statusIconPrefabs[6] = (Resources.Load("gui/ShipTagStatusListItems/StatusGroundedListItem") as GameObject);
	}

	// Token: 0x040003D6 RID: 982
	private GameObject m_guiCamera;

	// Token: 0x040003D7 RID: 983
	private GameType m_gameType;

	// Token: 0x040003D8 RID: 984
	private List<ShipTag> m_tags = new List<ShipTag>();

	// Token: 0x040003D9 RID: 985
	private bool m_visible = true;

	// Token: 0x040003DA RID: 986
	private List<GameObject> m_statusIconPrefabs = new List<GameObject>();
}
