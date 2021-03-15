using System;

// Token: 0x0200013B RID: 315
public class FleetSize
{
	// Token: 0x06000BFA RID: 3066 RVA: 0x00055DA0 File Offset: 0x00053FA0
	public FleetSize(int min, int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00055DB8 File Offset: 0x00053FB8
	public bool IsEqual(FleetSize size)
	{
		return this.min == size.min && this.max == size.max;
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00055DE4 File Offset: 0x00053FE4
	public bool ValidSize(int size, bool dubble)
	{
		if (dubble)
		{
			return size >= this.min * 2 && size <= this.max * 2;
		}
		return size >= this.min && size <= this.max;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00055E34 File Offset: 0x00054034
	public bool ValidSize(int size)
	{
		return size >= this.min && size <= this.max;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00055E54 File Offset: 0x00054054
	public override string ToString()
	{
		return this.max.ToString();
	}

	// Token: 0x040009CB RID: 2507
	public int min;

	// Token: 0x040009CC RID: 2508
	public int max;
}
