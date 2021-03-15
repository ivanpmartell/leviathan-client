using System;
using System.Diagnostics;

// Token: 0x02000134 RID: 308
public class DebugUtils
{
	// Token: 0x06000BE6 RID: 3046 RVA: 0x00055628 File Offset: 0x00053828
	[Conditional("DEBUG")]
	public static void Assert(bool condition)
	{
		DebugUtils.Assert(condition, string.Empty);
	}

	// Token: 0x06000BE7 RID: 3047 RVA: 0x00055638 File Offset: 0x00053838
	[Conditional("DEBUG")]
	public static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new Exception(message);
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00055648 File Offset: 0x00053848
	public static void PrintCallstack()
	{
		StackTrace stackTrace = new StackTrace();
		StackFrame[] frames = stackTrace.GetFrames();
		foreach (StackFrame stackFrame in frames)
		{
			PLog.Log(stackFrame.GetMethod().Name);
		}
	}
}
