using System;
using UnityEngine;

// Token: 0x02000171 RID: 369
[Serializable]
public sealed class LODInstruction : IComparable
{
	// Token: 0x06000DFD RID: 3581 RVA: 0x00063084 File Offset: 0x00061284
	public LODInstruction() : this(null, 0f, 1f)
	{
	}

	// Token: 0x06000DFE RID: 3582 RVA: 0x00063098 File Offset: 0x00061298
	public LODInstruction(GameObject target, float minDist, float maxDist) : this(target, minDist, maxDist, true, false)
	{
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x000630A8 File Offset: 0x000612A8
	public LODInstruction(GameObject target, float minDist, float maxDist, bool useAll, bool isPrefab)
	{
		this.m_target = target;
		this.SafeSet_MinDist(minDist);
		this.SafeSet_MaxDist(maxDist);
		this.m_useAllRenderers = useAll;
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x000630E0 File Offset: 0x000612E0
	public int CompareTo(object obj)
	{
		if (obj is LODInstruction)
		{
			return ((obj as LODInstruction).m_minDist <= this.m_minDist) ? (((obj as LODInstruction).m_minDist != this.m_minDist) ? 1 : 0) : -1;
		}
		return 1;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00063134 File Offset: 0x00061334
	public override bool Equals(object obj)
	{
		return obj is LODInstruction && ((obj as LODInstruction).m_minDist == this.m_minDist && (obj as LODInstruction).m_maxDist == this.m_maxDist) && (obj as LODInstruction).m_target == this.m_target;
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00063194 File Offset: 0x00061394
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0006319C File Offset: 0x0006139C
	public void SafeSet_MinDist(float f)
	{
		this.m_minDist = f;
		if (this.m_minDist < 0f)
		{
			this.m_minDist = 0f;
		}
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x000631CC File Offset: 0x000613CC
	public void SafeSet_MaxDist(float f)
	{
		this.m_maxDist = f;
		if (this.m_maxDist <= this.m_minDist)
		{
			this.m_maxDist = this.m_minDist + 1f;
		}
	}

	// Token: 0x04000B41 RID: 2881
	public GameObject m_target;

	// Token: 0x04000B42 RID: 2882
	public float m_minDist;

	// Token: 0x04000B43 RID: 2883
	public float m_maxDist = 1f;

	// Token: 0x04000B44 RID: 2884
	public bool m_useAllRenderers = true;
}
