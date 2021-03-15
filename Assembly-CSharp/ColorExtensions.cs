using System;
using UnityEngine;

// Token: 0x0200018E RID: 398
public static class ColorExtensions
{
	// Token: 0x06000EE7 RID: 3815 RVA: 0x0006844C File Offset: 0x0006664C
	public static int ToARGB(this Color color)
	{
		int num = (int)(color.a * 255f);
		int num2 = (int)(color.r * 255f);
		int num3 = (int)(color.g * 255f);
		int num4 = (int)(color.b * 255f);
		if (num > 255)
		{
			num = 255;
		}
		else if (num < 0)
		{
			num = 0;
		}
		if (num2 > 255)
		{
			num2 = 255;
		}
		else if (num2 < 0)
		{
			num2 = 0;
		}
		if (num3 > 255)
		{
			num3 = 255;
		}
		else if (num3 < 0)
		{
			num3 = 0;
		}
		if (num4 > 255)
		{
			num4 = 255;
		}
		else if (num4 < 0)
		{
			num4 = 0;
		}
		return num << 24 | num2 << 16 | num3 << 8 | num4;
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x00068520 File Offset: 0x00066720
	public static string ToHex(this Color color)
	{
		int num = color.ToARGB();
		return "#" + (num & 16777215).ToString("X6");
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x00068554 File Offset: 0x00066754
	public static string ToSimpleSpriteColorTag(this Color color)
	{
		return "[" + color.ToHex() + "]";
	}
}
