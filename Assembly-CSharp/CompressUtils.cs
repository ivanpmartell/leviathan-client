using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

// Token: 0x0200017C RID: 380
internal class CompressUtils
{
	// Token: 0x06000E47 RID: 3655 RVA: 0x00064B70 File Offset: 0x00062D70
	public static byte[] Compress(byte[] data)
	{
		if (data.Length == 0)
		{
			return new byte[0];
		}
		MemoryStream memoryStream = new MemoryStream();
		GZipOutputStream gzipOutputStream = new GZipOutputStream(memoryStream);
		BinaryWriter binaryWriter = new BinaryWriter(gzipOutputStream);
		binaryWriter.Write(data.Length);
		binaryWriter.Write(data);
		binaryWriter.Close();
		gzipOutputStream.Close();
		return memoryStream.ToArray();
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00064BC4 File Offset: 0x00062DC4
	public static byte[] Decompress(byte[] data)
	{
		if (data.Length == 0)
		{
			return new byte[0];
		}
		MemoryStream baseInputStream = new MemoryStream(data);
		GZipInputStream input = new GZipInputStream(baseInputStream);
		BinaryReader binaryReader = new BinaryReader(input);
		int count = binaryReader.ReadInt32();
		return binaryReader.ReadBytes(count);
	}
}
