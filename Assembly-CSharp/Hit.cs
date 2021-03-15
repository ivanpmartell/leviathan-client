using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class Hit
{
	// Token: 0x060006B9 RID: 1721 RVA: 0x000336D4 File Offset: 0x000318D4
	public Hit(NetObj dealer, int damage, int armorPiercing, Vector3 point, Vector3 dir)
	{
		this.m_dealer = dealer;
		this.m_damage = damage;
		this.m_armorPiercing = armorPiercing;
		this.m_point = point;
		this.m_dir = dir;
		this.m_havePoint = true;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x0003372C File Offset: 0x0003192C
	public Hit(NetObj dealer, int damage, int ap)
	{
		this.m_dealer = dealer;
		this.m_damage = damage;
		this.m_armorPiercing = ap;
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00033760 File Offset: 0x00031960
	public Hit(int damage, int ap, Vector3 point, Vector3 dir)
	{
		this.m_damage = damage;
		this.m_armorPiercing = ap;
		this.m_point = point;
		this.m_dir = dir;
		this.m_havePoint = true;
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000337B0 File Offset: 0x000319B0
	public Hit(int damage, int ap)
	{
		this.m_damage = damage;
		this.m_armorPiercing = ap;
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x000337E8 File Offset: 0x000319E8
	public Gun GetGun()
	{
		return this.m_dealer as Gun;
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x000337F8 File Offset: 0x000319F8
	public Unit GetUnit()
	{
		Gun gun = this.m_dealer as Gun;
		if (gun != null)
		{
			return gun.GetUnit();
		}
		return this.m_dealer as Unit;
	}

	// Token: 0x040005B0 RID: 1456
	public NetObj m_dealer;

	// Token: 0x040005B1 RID: 1457
	public int m_damage;

	// Token: 0x040005B2 RID: 1458
	public int m_armorPiercing;

	// Token: 0x040005B3 RID: 1459
	public Vector3 m_point = Vector3.zero;

	// Token: 0x040005B4 RID: 1460
	public Vector3 m_dir = Vector3.zero;

	// Token: 0x040005B5 RID: 1461
	public bool m_havePoint;

	// Token: 0x040005B6 RID: 1462
	public bool m_collision;
}
