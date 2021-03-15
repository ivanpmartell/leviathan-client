using System;

// Token: 0x02000162 RID: 354
internal struct VersionInfo
{
	// Token: 0x06000D3F RID: 3391 RVA: 0x0005F3B4 File Offset: 0x0005D5B4
	public static string GetFullVersionString()
	{
		return VersionInfo.m_minorVersion + "(" + VersionInfo.m_majorVersion.ToString() + ")";
	}

	// Token: 0x06000D40 RID: 3392 RVA: 0x0005F3D4 File Offset: 0x0005D5D4
	public static string GetMajorVersionString()
	{
		return VersionInfo.m_majorVersion.ToString();
	}

	// Token: 0x06000D41 RID: 3393 RVA: 0x0005F3E0 File Offset: 0x0005D5E0
	public static bool VerifyVersion(string versionString)
	{
		return versionString == VersionInfo.GetMajorVersionString() || versionString == VersionInfo.GetFullVersionString() || versionString == VersionInfo.m_alternativeMajorVersion.ToString();
	}

	// Token: 0x04000AF0 RID: 2800
	public static string m_minorVersion = "1.2.11741";

	// Token: 0x04000AF1 RID: 2801
	public static int m_majorVersion = 3;

	// Token: 0x04000AF2 RID: 2802
	public static int m_alternativeMajorVersion = 3;
}
