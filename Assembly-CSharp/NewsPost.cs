using System;
using System.IO;

// Token: 0x02000147 RID: 327
internal class NewsPost
{
	// Token: 0x06000C92 RID: 3218 RVA: 0x0005A57C File Offset: 0x0005877C
	public NewsPost()
	{
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0005A584 File Offset: 0x00058784
	public NewsPost(byte[] data)
	{
		this.FromArray(data);
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x0005A594 File Offset: 0x00058794
	public byte[] ToArray()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(this.m_title);
		binaryWriter.Write(this.m_date.ToBinary());
		binaryWriter.Write(this.m_category);
		binaryWriter.Write(this.m_content);
		return memoryStream.ToArray();
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0005A5EC File Offset: 0x000587EC
	public void FromArray(byte[] data)
	{
		MemoryStream input = new MemoryStream(data);
		BinaryReader binaryReader = new BinaryReader(input);
		this.m_title = binaryReader.ReadString();
		this.m_date = DateTime.FromBinary(binaryReader.ReadInt64());
		this.m_category = binaryReader.ReadString();
		this.m_content = binaryReader.ReadString();
	}

	// Token: 0x04000A41 RID: 2625
	public string m_title;

	// Token: 0x04000A42 RID: 2626
	public DateTime m_date;

	// Token: 0x04000A43 RID: 2627
	public string m_category;

	// Token: 0x04000A44 RID: 2628
	public string m_content;
}
