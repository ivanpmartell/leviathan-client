using System;
using System.Text.RegularExpressions;

// Token: 0x0200018D RID: 397
public class StringUtils
{
	// Token: 0x06000EE2 RID: 3810 RVA: 0x00068330 File Offset: 0x00066530
	public static void TryRemoveCopyText(ref string fromString)
	{
		string text = StringUtils.ExtractCopyNumber(fromString, true);
		if (!string.IsNullOrEmpty(text))
		{
			fromString = fromString.Replace(text, string.Empty);
		}
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x00068360 File Offset: 0x00066560
	public static bool ContainsParanthesesAndNumber(string fromString)
	{
		return !string.IsNullOrEmpty(StringUtils.ExtractCopyNumber(fromString, false));
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00068374 File Offset: 0x00066574
	public static string ExtractCopyNumber(string fromString, bool includeParantheses)
	{
		string text = fromString.Trim();
		int num = text.IndexOf('(');
		if (num < 0)
		{
			return string.Empty;
		}
		int num2 = text.LastIndexOf(')');
		if (num2 < 0)
		{
			return string.Empty;
		}
		int num3 = num2 - num - 1;
		if (num3 < 0)
		{
			return string.Empty;
		}
		string text2 = text.Substring(num + 1, num3);
		int num4 = 0;
		if (int.TryParse(text2, out num4))
		{
			return ((!includeParantheses) ? string.Empty : "(") + text2 + ((!includeParantheses) ? string.Empty : ")");
		}
		return string.Empty;
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0006841C File Offset: 0x0006661C
	public static void TryRemoveNonNumbers(ref string text)
	{
		text = Regex.Replace(text, "\\D", string.Empty);
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x00068434 File Offset: 0x00066634
	public static void TryRemoveNumbers(ref string text)
	{
		text = Regex.Replace(text, "\\d", string.Empty);
	}
}
