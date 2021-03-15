using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class NetObj : MonoBehaviour
{
	// Token: 0x060008F6 RID: 2294 RVA: 0x00041944 File Offset: 0x0003FB44
	public virtual void Awake()
	{
		if (NetObj.m_nextNetID >= 1)
		{
			this.m_netID = NetObj.m_nextNetID++;
			NetObj.m_wsObjects.Add(this.m_netID, this);
			NetObj.m_wsObjectList.Add(this);
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x0004198C File Offset: 0x0003FB8C
	public virtual void OnDestroy()
	{
		if (this.m_netID >= 1)
		{
			NetObj.m_wsObjects.Remove(this.m_netID);
			int num = NetObj.m_wsObjectList.IndexOf(this);
			if (num >= 0)
			{
				int index = NetObj.m_wsObjectList.Count - 1;
				NetObj.m_wsObjectList[num] = NetObj.m_wsObjectList[index];
				NetObj.m_wsObjectList.RemoveAt(index);
			}
		}
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000419F8 File Offset: 0x0003FBF8
	public int GetNetID()
	{
		return this.m_netID;
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00041A00 File Offset: 0x0003FC00
	public void SetNetID(int id)
	{
		if (this.m_netID >= 1)
		{
			NetObj.m_wsObjects.Remove(this.m_netID);
			NetObj.m_wsObjectList.Remove(this);
		}
		this.m_netID = id;
		NetObj.m_wsObjects.Add(this.m_netID, this);
		NetObj.m_wsObjectList.Add(this);
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00041A5C File Offset: 0x0003FC5C
	public static NetObj GetByID(int id)
	{
		NetObj result;
		if (NetObj.m_wsObjects.TryGetValue(id, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00041A80 File Offset: 0x0003FC80
	public static List<NetObj> GetAll()
	{
		return NetObj.m_wsObjectList;
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00041A88 File Offset: 0x0003FC88
	public static NetObj[] GetAllToSave()
	{
		List<NetObj> list = new List<NetObj>();
		foreach (NetObj netObj in NetObj.m_wsObjectList)
		{
			if (netObj.m_save)
			{
				if (netObj.gameObject == null)
				{
					PLog.LogError("Object has been destroyed " + netObj.gameObject.name + ", Please inform mr DVOID about this incident...or burn in hell for all eternity!!!");
				}
				else
				{
					list.Add(netObj);
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00041B3C File Offset: 0x0003FD3C
	public static void ResetObjectDB()
	{
		NetObj.m_nextNetID = 0;
		NetObj.m_wsObjects.Clear();
		NetObj.m_wsObjectList.Clear();
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00041B58 File Offset: 0x0003FD58
	public static void SetNextNetID(int id)
	{
		NetObj.m_nextNetID = id;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00041B60 File Offset: 0x0003FD60
	public static int GetNextNetID()
	{
		return NetObj.m_nextNetID;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00041B68 File Offset: 0x0003FD68
	public virtual void SaveState(BinaryWriter writer)
	{
		writer.Write(this.GetNetID());
		writer.Write((byte)this.m_owner);
		writer.Write((short)this.m_seenBy);
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00041B9C File Offset: 0x0003FD9C
	public virtual void LoadState(BinaryReader reader)
	{
		int netID = reader.ReadInt32();
		this.SetNetID(netID);
		this.SetOwner((int)reader.ReadByte());
		this.m_seenBy = (int)reader.ReadInt16();
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00041BD0 File Offset: 0x0003FDD0
	public virtual bool IsVisible()
	{
		return this.m_visible;
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00041BD8 File Offset: 0x0003FDD8
	public virtual void SetVisible(bool visible)
	{
		this.m_visible = visible;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00041BE4 File Offset: 0x0003FDE4
	public virtual bool IsSeenByPlayer(int playerID)
	{
		if (playerID < 0)
		{
			return true;
		}
		DebugUtils.Assert(playerID >= 0);
		int playerTeam = TurnMan.instance.GetPlayerTeam(playerID);
		return this.IsSeenByTeam(playerTeam);
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00041C1C File Offset: 0x0003FE1C
	private void SetSeenByPlayer(int playerID)
	{
		if (TurnMan.instance == null)
		{
			return;
		}
		int playerTeam = TurnMan.instance.GetPlayerTeam(playerID);
		this.m_seenBy |= 1 << playerTeam;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00041C58 File Offset: 0x0003FE58
	public virtual bool IsSeenByTeam(int teamID)
	{
		int num = 1 << teamID;
		return (this.m_seenBy & num) != 0;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00041C7C File Offset: 0x0003FE7C
	public int GetSeenByMask()
	{
		return this.m_seenBy;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00041C84 File Offset: 0x0003FE84
	public void SetSeenByMask(int mask)
	{
		this.m_seenBy = mask;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00041C90 File Offset: 0x0003FE90
	public void UpdateSeenByMask(int mask)
	{
		if (NetObj.m_phase == TurnPhase.Testing)
		{
			mask = (this.m_seenBy & mask);
		}
		if (this.m_stayVisible)
		{
			this.m_seenBy |= mask;
		}
		else
		{
			this.m_seenBy = mask;
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00041CCC File Offset: 0x0003FECC
	protected virtual void OnSetSimulating(bool enabled)
	{
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00041CD0 File Offset: 0x0003FED0
	protected virtual void OnSetDrawOrders(bool enabled)
	{
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00041CD4 File Offset: 0x0003FED4
	public virtual void SetOwner(int owner)
	{
		this.m_owner = owner;
		this.SetSeenByPlayer(owner);
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00041CE4 File Offset: 0x0003FEE4
	public int GetOwner()
	{
		return this.m_owner;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00041CEC File Offset: 0x0003FEEC
	public int GetOwnerTeam()
	{
		return TurnMan.instance.GetPlayerTeam(this.m_owner);
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00041D00 File Offset: 0x0003FF00
	public bool GetUpdateSeenBy()
	{
		return this.m_updateSeenBy;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00041D08 File Offset: 0x0003FF08
	public static void SetDrawOrders(bool enabled)
	{
		NetObj.m_drawOrders = enabled;
		foreach (NetObj netObj in NetObj.m_wsObjectList)
		{
			netObj.OnSetDrawOrders(enabled && netObj.GetOwner() == NetObj.m_localPlayerID);
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00041D88 File Offset: 0x0003FF88
	public static bool GetDrawOrders()
	{
		return NetObj.m_drawOrders;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00041D90 File Offset: 0x0003FF90
	public static void SetPhase(TurnPhase phase)
	{
		NetObj.m_phase = phase;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00041D98 File Offset: 0x0003FF98
	public static void SetSimulating(bool enabled)
	{
		NetObj.m_simulating = enabled;
		foreach (NetObj netObj in NetObj.m_wsObjectList)
		{
			netObj.OnSetSimulating(enabled);
		}
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00041E04 File Offset: 0x00040004
	public static bool IsSimulating()
	{
		return NetObj.m_simulating;
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00041E0C File Offset: 0x0004000C
	public static void SetLocalPlayer(int localPlayerID)
	{
		NetObj.m_localPlayerID = localPlayerID;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00041E14 File Offset: 0x00040014
	public static int GetLocalPlayer()
	{
		return NetObj.m_localPlayerID;
	}

	// Token: 0x0400073E RID: 1854
	private static Dictionary<int, NetObj> m_wsObjects = new Dictionary<int, NetObj>();

	// Token: 0x0400073F RID: 1855
	private static List<NetObj> m_wsObjectList = new List<NetObj>();

	// Token: 0x04000740 RID: 1856
	private static int m_nextNetID = 0;

	// Token: 0x04000741 RID: 1857
	public bool m_stayVisible;

	// Token: 0x04000742 RID: 1858
	protected static int m_localPlayerID = -1;

	// Token: 0x04000743 RID: 1859
	protected static bool m_simulating = false;

	// Token: 0x04000744 RID: 1860
	protected static TurnPhase m_phase = TurnPhase.Planning;

	// Token: 0x04000745 RID: 1861
	protected static bool m_drawOrders = false;

	// Token: 0x04000746 RID: 1862
	protected bool m_save = true;

	// Token: 0x04000747 RID: 1863
	protected bool m_updateSeenBy = true;

	// Token: 0x04000748 RID: 1864
	private int m_netID;

	// Token: 0x04000749 RID: 1865
	private bool m_visible = true;

	// Token: 0x0400074A RID: 1866
	private int m_seenBy;

	// Token: 0x0400074B RID: 1867
	private int m_owner = 7;
}
