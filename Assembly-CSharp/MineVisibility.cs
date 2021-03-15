using System;
using System.IO;
using UnityEngine;

// Token: 0x020000E5 RID: 229
[AddComponentMenu("Scripts/Weapons/MineVisibility")]
public class MineVisibility : Mine
{
	// Token: 0x060008CE RID: 2254 RVA: 0x000404F8 File Offset: 0x0003E6F8
	public override void Setup(int ownerID, int unitID, bool visible, int seenByMask, int damage, int ap, float splashRadius, float splashDmgFactor)
	{
		base.Setup(ownerID, unitID, visible, seenByMask, damage, ap, splashRadius, splashDmgFactor);
		this.SetOwner(ownerID);
		this.SetVisible(visible);
		base.SetSeenByMask(seenByMask);
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00040530 File Offset: 0x0003E730
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00040538 File Offset: 0x0003E738
	protected override void FixedUpdate()
	{
		if (base.IsDeployed())
		{
			base.SetSightRange(this.m_baseSightRange);
		}
		else
		{
			base.SetSightRange(0f);
		}
		base.FixedUpdate();
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00040568 File Offset: 0x0003E768
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_baseSightRange);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x00040580 File Offset: 0x0003E780
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_baseSightRange = reader.ReadSingle();
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x00040598 File Offset: 0x0003E798
	public override bool TestLOS(NetObj obj)
	{
		if (!base.IsDeployed())
		{
			return false;
		}
		float num = Vector3.Distance(base.transform.position, obj.transform.position);
		return num <= this.m_baseSightRange;
	}

	// Token: 0x04000718 RID: 1816
	public float m_baseSightRange = 50f;
}
