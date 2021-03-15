using System;

// Token: 0x0200013C RID: 316
public class FleetSizes
{
	// Token: 0x06000C01 RID: 3073 RVA: 0x00055ED0 File Offset: 0x000540D0
	public static FleetSizeClass GetSizeClass(int points)
	{
		if (FleetSizes.sizes[0].ValidSize(points))
		{
			return FleetSizeClass.Small;
		}
		if (FleetSizes.sizes[1].ValidSize(points))
		{
			return FleetSizeClass.Medium;
		}
		if (FleetSizes.sizes[2].ValidSize(points))
		{
			return FleetSizeClass.Heavy;
		}
		return FleetSizeClass.Custom;
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00055F1C File Offset: 0x0005411C
	public static string GetSizeClassName(int points)
	{
		FleetSizeClass sizeClass = FleetSizes.GetSizeClass(points);
		if (sizeClass == FleetSizeClass.Small)
		{
			return "fleetsize_small";
		}
		if (sizeClass == FleetSizeClass.Medium)
		{
			return "fleetsize_medium";
		}
		if (sizeClass == FleetSizeClass.Heavy)
		{
			return "fleetsize_large";
		}
		return "fleetsize_invalid";
	}

	// Token: 0x06000C03 RID: 3075 RVA: 0x00055F5C File Offset: 0x0005415C
	public static FleetSizeClass GetSizeClass(FleetSize size)
	{
		if (FleetSizes.sizes[0].IsEqual(size))
		{
			return FleetSizeClass.Small;
		}
		if (FleetSizes.sizes[1].IsEqual(size))
		{
			return FleetSizeClass.Medium;
		}
		if (FleetSizes.sizes[2].IsEqual(size))
		{
			return FleetSizeClass.Heavy;
		}
		return FleetSizeClass.Custom;
	}

	// Token: 0x06000C04 RID: 3076 RVA: 0x00055FA8 File Offset: 0x000541A8
	public static string GetSizeClassName(FleetSize size)
	{
		PLog.Log("GetSizeClassName " + size.min.ToString() + " / " + size.max.ToString());
		if (FleetSizes.sizes[0].IsEqual(size))
		{
			return "fleetsize_small";
		}
		if (FleetSizes.sizes[1].IsEqual(size))
		{
			return "fleetsize_medium";
		}
		if (FleetSizes.sizes[2].IsEqual(size))
		{
			return "fleetsize_large";
		}
		return string.Empty;
	}

	// Token: 0x06000C05 RID: 3077 RVA: 0x0005602C File Offset: 0x0005422C
	public static string GetSizeLimit(FleetSize fleetSize, int size)
	{
		if (size < fleetSize.min)
		{
			return "fleetsize_minimum";
		}
		if (size > fleetSize.max)
		{
			return "fleetsize_exceeded";
		}
		if (FleetSizes.sizes[0].ValidSize(size))
		{
			return "fleetsize_small";
		}
		if (FleetSizes.sizes[1].ValidSize(size))
		{
			return "fleetsize_medium";
		}
		if (FleetSizes.sizes[2].ValidSize(size))
		{
			return "fleetsize_large";
		}
		return "fleetsize_invalid";
	}

	// Token: 0x040009CD RID: 2509
	public static FleetSize[] sizes = new FleetSize[]
	{
		new FleetSize(1, 3000),
		new FleetSize(1, 6000),
		new FleetSize(1, 8000),
		new FleetSize(1, 6000),
		new FleetSize(0, 0),
		new FleetSize(0, 0)
	};
}
