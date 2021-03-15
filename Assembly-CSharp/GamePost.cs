using System;
using System.IO;
using PTech;

// Token: 0x02000143 RID: 323
internal class GamePost : IComparable<GamePost>
{
	// Token: 0x06000C7F RID: 3199 RVA: 0x00059AAC File Offset: 0x00057CAC
	public byte[] ToArray()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(this.m_gameID);
		binaryWriter.Write(this.m_gameName);
		binaryWriter.Write(this.m_campaign);
		binaryWriter.Write(this.m_level);
		binaryWriter.Write((int)this.m_gameType);
		binaryWriter.Write((int)this.m_fleetSizeClass);
		binaryWriter.Write(this.m_maxPlayers);
		binaryWriter.Write(this.m_nrOfPlayers);
		binaryWriter.Write(this.m_connectedPlayers);
		binaryWriter.Write(this.m_turn);
		binaryWriter.Write(this.m_needAttention);
		binaryWriter.Write(this.m_createDate.ToBinary());
		return memoryStream.ToArray();
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00059B64 File Offset: 0x00057D64
	public void FromArray(byte[] data)
	{
		MemoryStream input = new MemoryStream(data);
		BinaryReader binaryReader = new BinaryReader(input);
		this.m_gameID = binaryReader.ReadInt32();
		this.m_gameName = binaryReader.ReadString();
		this.m_campaign = binaryReader.ReadString();
		this.m_level = binaryReader.ReadString();
		this.m_gameType = (GameType)binaryReader.ReadInt32();
		this.m_fleetSizeClass = (FleetSizeClass)binaryReader.ReadInt32();
		this.m_maxPlayers = binaryReader.ReadInt32();
		this.m_nrOfPlayers = binaryReader.ReadInt32();
		this.m_connectedPlayers = binaryReader.ReadInt32();
		this.m_turn = binaryReader.ReadInt32();
		this.m_needAttention = binaryReader.ReadBoolean();
		this.m_createDate = DateTime.FromBinary(binaryReader.ReadInt64());
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00059C14 File Offset: 0x00057E14
	public int CompareTo(GamePost other)
	{
		if (this.m_needAttention == other.m_needAttention)
		{
			return other.m_createDate.CompareTo(this.m_createDate);
		}
		if (this.m_needAttention)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x04000A1E RID: 2590
	public int m_gameID;

	// Token: 0x04000A1F RID: 2591
	public string m_gameName = string.Empty;

	// Token: 0x04000A20 RID: 2592
	public string m_campaign = string.Empty;

	// Token: 0x04000A21 RID: 2593
	public string m_level = string.Empty;

	// Token: 0x04000A22 RID: 2594
	public GameType m_gameType;

	// Token: 0x04000A23 RID: 2595
	public FleetSizeClass m_fleetSizeClass;

	// Token: 0x04000A24 RID: 2596
	public int m_maxPlayers;

	// Token: 0x04000A25 RID: 2597
	public int m_nrOfPlayers;

	// Token: 0x04000A26 RID: 2598
	public int m_connectedPlayers;

	// Token: 0x04000A27 RID: 2599
	public int m_turn;

	// Token: 0x04000A28 RID: 2600
	public bool m_needAttention;

	// Token: 0x04000A29 RID: 2601
	public DateTime m_createDate;
}
