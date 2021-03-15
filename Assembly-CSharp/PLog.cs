using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class PLog
{
	// Token: 0x06000E96 RID: 3734 RVA: 0x000665D0 File Offset: 0x000647D0
	public static void Log(object o)
	{
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000665D4 File Offset: 0x000647D4
	public static void LogError(object o)
	{
		Debug.LogError(string.Concat(new object[]
		{
			DateTime.Now.ToString(),
			": ",
			o,
			"\n"
		}));
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x00066618 File Offset: 0x00064818
	public static void LogWarning(object o)
	{
		Debug.LogWarning(string.Concat(new object[]
		{
			DateTime.Now.ToString(),
			": ",
			o,
			"\n"
		}));
	}
}
