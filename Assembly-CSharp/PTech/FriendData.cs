using System;
using System.IO;

namespace PTech
{
	// Token: 0x0200013D RID: 317
	internal class FriendData
	{
		// Token: 0x06000C07 RID: 3079 RVA: 0x000560B4 File Offset: 0x000542B4
		public byte[] ToArray()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(this.m_friendID);
			binaryWriter.Write(this.m_name);
			binaryWriter.Write((int)this.m_status);
			binaryWriter.Write(this.m_flagID);
			binaryWriter.Write(this.m_online);
			return memoryStream.ToArray();
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x00056110 File Offset: 0x00054310
		public void FromArray(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			this.m_friendID = binaryReader.ReadInt32();
			this.m_name = binaryReader.ReadString();
			this.m_status = (FriendData.FriendStatus)binaryReader.ReadInt32();
			this.m_flagID = binaryReader.ReadInt32();
			this.m_online = binaryReader.ReadBoolean();
		}

		// Token: 0x040009CE RID: 2510
		public int m_friendID;

		// Token: 0x040009CF RID: 2511
		public string m_name;

		// Token: 0x040009D0 RID: 2512
		public FriendData.FriendStatus m_status;

		// Token: 0x040009D1 RID: 2513
		public int m_flagID;

		// Token: 0x040009D2 RID: 2514
		public bool m_online;

		// Token: 0x0200013E RID: 318
		public enum FriendStatus
		{
			// Token: 0x040009D4 RID: 2516
			IsFriend,
			// Token: 0x040009D5 RID: 2517
			Requested,
			// Token: 0x040009D6 RID: 2518
			NeedAccept,
			// Token: 0x040009D7 RID: 2519
			NotFriend
		}
	}
}
