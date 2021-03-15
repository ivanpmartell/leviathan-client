using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
[AddComponentMenu("Scripts/Weapons/MineBoss")]
public class MineBoss : MineExplode
{
	// Token: 0x060008B1 RID: 2225 RVA: 0x0003FEE4 File Offset: 0x0003E0E4
	public override void Setup(int ownerID, int unitID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		base.Setup(ownerID, unitID, visible, seenByMask, damage, ap, splashRadius, splashDmgFactor);
		this.m_radiusMesh.renderer.enabled = false;
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0003FF18 File Offset: 0x0003E118
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0003FF20 File Offset: 0x0003E120
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0003FF28 File Offset: 0x0003E128
	public override void Update()
	{
		base.Update();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0003FF30 File Offset: 0x0003E130
	public override bool Damage(Hit hit)
	{
		return base.GetOwner() == hit.m_dealer.GetOwner() || base.Damage(hit);
	}
}
