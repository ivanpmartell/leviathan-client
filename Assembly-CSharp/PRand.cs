using System;
using UnityEngine;

// Token: 0x02000129 RID: 297
internal class PRand
{
	// Token: 0x06000BB5 RID: 2997 RVA: 0x0005431C File Offset: 0x0005251C
	public static int GetSeed()
	{
		return PRand.m_seed;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x00054324 File Offset: 0x00052524
	public static void SetSeed(int seed)
	{
		PRand.m_seed = seed;
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0005432C File Offset: 0x0005252C
	public static int Range(int min, int max)
	{
		UnityEngine.Random.seed = PRand.m_seed;
		int result = UnityEngine.Random.Range(min, max);
		PRand.m_seed = UnityEngine.Random.seed;
		return result;
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00054358 File Offset: 0x00052558
	public static float Range(float min, float max)
	{
		UnityEngine.Random.seed = PRand.m_seed;
		float result = UnityEngine.Random.Range(min, max);
		PRand.m_seed = UnityEngine.Random.seed;
		return result;
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00054384 File Offset: 0x00052584
	public static float Value()
	{
		UnityEngine.Random.seed = PRand.m_seed;
		float value = UnityEngine.Random.value;
		PRand.m_seed = UnityEngine.Random.seed;
		return value;
	}

	// Token: 0x04000985 RID: 2437
	private static int m_seed;
}
