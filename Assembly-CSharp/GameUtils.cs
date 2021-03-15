using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017F RID: 383
internal class GameUtils
{
	// Token: 0x06000E4D RID: 3661 RVA: 0x00064EA4 File Offset: 0x000630A4
	public static bool FindAveragePlayerPos(int playerID, out Vector3 pos)
	{
		List<NetObj> all = NetObj.GetAll();
		pos = new Vector3(0f, 0f, 0f);
		int num = 0;
		foreach (NetObj netObj in all)
		{
			if (netObj.GetOwner() == playerID)
			{
				num++;
				pos += netObj.transform.position;
			}
		}
		pos /= (float)num;
		return num > 0;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x00064F60 File Offset: 0x00063160
	public static bool FindCameraStartPos(int playerID, out Vector3 pos)
	{
		List<NetObj> all = NetObj.GetAll();
		pos = new Vector3(0f, 0f, 0f);
		int num = -1;
		foreach (NetObj netObj in all)
		{
			if (netObj.GetOwner() == playerID)
			{
				Ship ship = netObj as Ship;
				if (ship != null)
				{
					int totalValue = ship.GetTotalValue();
					if (!ship.IsDead() && totalValue > num)
					{
						num = totalValue;
						pos = netObj.transform.position;
					}
				}
			}
		}
		return num >= 0;
	}
}
