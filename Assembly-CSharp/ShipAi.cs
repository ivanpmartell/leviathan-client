using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000118 RID: 280
[Serializable]
public class ShipAi : UnitAi
{
	// Token: 0x06000AD4 RID: 2772 RVA: 0x00050748 File Offset: 0x0004E948
	public override void SetTargetId(Unit target)
	{
		TurnMan instance = TurnMan.instance;
		if (!instance.IsHostile(this.m_ship.GetOwner(), target.GetOwner()))
		{
			return;
		}
		this.m_targetId = target.GetNetID();
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00050784 File Offset: 0x0004E984
	public void FindEnemy(float dt)
	{
		this.m_nextScan -= dt;
		if (this.m_nextScan > 0f)
		{
			return;
		}
		this.m_nextScan = 2f;
		List<NetObj> all = NetObj.GetAll();
		TurnMan instance = TurnMan.instance;
		for (int i = 0; i < all.Count; i++)
		{
			Ship ship = all[i] as Ship;
			if (!(ship == null))
			{
				if (!ship.IsDead())
				{
					if (ship.IsValidTarget())
					{
						if (instance.IsHostile(this.m_ship.GetOwner(), ship.GetOwner()))
						{
							float num = Vector3.Distance(this.m_ship.transform.position, ship.transform.position);
							if (num <= this.m_ship.GetSightRange())
							{
								if (this.m_ship.TestLOS(ship))
								{
									this.m_targetId = ship.GetNetID();
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00050894 File Offset: 0x0004EA94
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x000508A0 File Offset: 0x0004EAA0
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000508AC File Offset: 0x0004EAAC
	public override UnitAi.AttackDirection GetAttackDirection(Vector3 position)
	{
		Vector3 to = position - this.m_ship.transform.position;
		to.y = 0f;
		to.Normalize();
		float num = Vector3.Angle(this.m_ship.transform.forward, to);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Front;
		}
		num = Vector3.Angle(-this.m_ship.transform.forward, to);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Back;
		}
		num = Vector3.Angle(this.m_ship.transform.right, to);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Right;
		}
		num = Vector3.Angle(-this.m_ship.transform.right, to);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Left;
		}
		return UnitAi.AttackDirection.None;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x00050980 File Offset: 0x0004EB80
	public UnitAi.AttackDirection GetModuleDirection(HPModule module)
	{
		Vector3 forward = module.transform.forward;
		float num = Vector3.Angle(this.m_ship.transform.forward, forward);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Front;
		}
		num = Vector3.Angle(-this.m_ship.transform.forward, forward);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Back;
		}
		num = Vector3.Angle(this.m_ship.transform.right, forward);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Right;
		}
		num = Vector3.Angle(-this.m_ship.transform.right, forward);
		if (num <= 45f)
		{
			return UnitAi.AttackDirection.Left;
		}
		return UnitAi.AttackDirection.Front;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00050A34 File Offset: 0x0004EC34
	public UnitAi.AttackDirection GetOptimalSide()
	{
		int[] array = new int[4];
		HPModule[] componentsInChildren = this.m_ship.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			if (hpmodule.m_type == HPModule.HPModuleType.Offensive)
			{
				UnitAi.AttackDirection moduleDirection = this.GetModuleDirection(hpmodule);
				array[(int)moduleDirection]++;
			}
		}
		UnitAi.AttackDirection result = UnitAi.AttackDirection.Front;
		int num = 0;
		for (int j = 0; j < 4; j++)
		{
			if (array[j] > num)
			{
				result = (UnitAi.AttackDirection)j;
				num = array[j];
			}
		}
		return result;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00050AC8 File Offset: 0x0004ECC8
	public List<HPModule> GetModulesOnSide(UnitAi.AttackDirection side)
	{
		List<HPModule> list = new List<HPModule>();
		HPModule[] componentsInChildren = this.m_ship.GetComponentsInChildren<HPModule>();
		foreach (HPModule hpmodule in componentsInChildren)
		{
			if (this.GetModuleDirection(hpmodule) == side && hpmodule.m_type == HPModule.HPModuleType.Offensive)
			{
				list.Add(hpmodule);
			}
		}
		return list;
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x00050B28 File Offset: 0x0004ED28
	public float GetOptimalAttackRange(UnitAi.AttackDirection side)
	{
		List<HPModule> modulesOnSide = this.GetModulesOnSide(side);
		foreach (HPModule hpmodule in modulesOnSide)
		{
		}
		return 100f;
	}

	// Token: 0x04000908 RID: 2312
	public Ship m_ship;
}
