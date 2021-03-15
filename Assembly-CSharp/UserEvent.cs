using System;
using System.IO;

// Token: 0x0200015D RID: 349
public class UserEvent
{
	// Token: 0x06000D24 RID: 3364 RVA: 0x0005E268 File Offset: 0x0005C468
	public UserEvent(byte[] data)
	{
		this.FromArray(data);
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x0005E278 File Offset: 0x0005C478
	public UserEvent()
	{
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x0005E280 File Offset: 0x0005C480
	public UserEvent(UserEvent.EventType type, string gameName, int gameID, string friendName, int achievementID, int turn)
	{
		this.m_type = type;
		this.m_gameName = gameName;
		this.m_gameID = gameID;
		this.m_friendName = friendName;
		this.m_achievementID = achievementID;
		this.m_turn = turn;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x0005E2B8 File Offset: 0x0005C4B8
	public UserEvent.EventType GetEventType()
	{
		return this.m_type;
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x0005E2C0 File Offset: 0x0005C4C0
	public string GetGameName()
	{
		return this.m_gameName;
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x0005E2C8 File Offset: 0x0005C4C8
	public int GetGameID()
	{
		return this.m_gameID;
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0005E2D0 File Offset: 0x0005C4D0
	public string GetFriendName()
	{
		return this.m_friendName;
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x0005E2D8 File Offset: 0x0005C4D8
	public int GetAchievementID()
	{
		return this.m_achievementID;
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x0005E2E0 File Offset: 0x0005C4E0
	public int GetTurn()
	{
		return this.m_turn;
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x0005E2E8 File Offset: 0x0005C4E8
	public void Save(BinaryWriter writer)
	{
		writer.Write((int)this.m_type);
		writer.Write(this.m_gameName);
		writer.Write(this.m_gameID);
		writer.Write(this.m_friendName);
		writer.Write(this.m_achievementID);
		writer.Write(this.m_turn);
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x0005E340 File Offset: 0x0005C540
	public void Load(BinaryReader reader)
	{
		this.m_type = (UserEvent.EventType)reader.ReadInt32();
		this.m_gameName = reader.ReadString();
		this.m_gameID = reader.ReadInt32();
		this.m_friendName = reader.ReadString();
		this.m_achievementID = reader.ReadInt32();
		this.m_turn = reader.ReadInt32();
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x0005E398 File Offset: 0x0005C598
	public byte[] ToArray()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter writer = new BinaryWriter(memoryStream);
		this.Save(writer);
		return memoryStream.ToArray();
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x0005E3C0 File Offset: 0x0005C5C0
	public void FromArray(byte[] data)
	{
		MemoryStream input = new MemoryStream(data);
		BinaryReader reader = new BinaryReader(input);
		this.Load(reader);
	}

	// Token: 0x04000AC8 RID: 2760
	private UserEvent.EventType m_type;

	// Token: 0x04000AC9 RID: 2761
	private string m_gameName;

	// Token: 0x04000ACA RID: 2762
	private int m_gameID;

	// Token: 0x04000ACB RID: 2763
	private string m_friendName;

	// Token: 0x04000ACC RID: 2764
	private int m_achievementID;

	// Token: 0x04000ACD RID: 2765
	private int m_turn;

	// Token: 0x0200015E RID: 350
	public enum EventType
	{
		// Token: 0x04000ACF RID: 2767
		NewTurn,
		// Token: 0x04000AD0 RID: 2768
		Achievement,
		// Token: 0x04000AD1 RID: 2769
		FriendRequest,
		// Token: 0x04000AD2 RID: 2770
		GameInvite,
		// Token: 0x04000AD3 RID: 2771
		FriendRequestAccepted,
		// Token: 0x04000AD4 RID: 2772
		NewAccount,
		// Token: 0x04000AD5 RID: 2773
		ResetPassword,
		// Token: 0x04000AD6 RID: 2774
		ServerMessage
	}
}
