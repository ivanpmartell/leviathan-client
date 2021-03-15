using System;
using System.IO;
using System.Text;

// Token: 0x02000158 RID: 344
public sealed class StringWriterWithEncoding : StringWriter
{
	// Token: 0x06000CE6 RID: 3302 RVA: 0x0005CA54 File Offset: 0x0005AC54
	public StringWriterWithEncoding(Encoding encoding)
	{
		this.encoding = encoding;
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x0005CA64 File Offset: 0x0005AC64
	public override Encoding Encoding
	{
		get
		{
			return this.encoding;
		}
	}

	// Token: 0x04000AA3 RID: 2723
	private readonly Encoding encoding;
}
