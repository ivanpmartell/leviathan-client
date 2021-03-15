using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x0200012B RID: 299
	internal class ChatChannel
	{
		// Token: 0x06000BBA RID: 3002 RVA: 0x000543AC File Offset: 0x000525AC
		public ChatChannel(ChannelID id)
		{
			this.m_channelID = id;
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x000543D4 File Offset: 0x000525D4
		public void AddUser(User user, bool sendOldMessages)
		{
			if (this.m_users.Contains(user))
			{
				return;
			}
			this.m_users.Add(user);
			if (sendOldMessages)
			{
				foreach (ChatChannel.ChatMessage chatMessage in this.m_messages)
				{
					user.m_rpc.Invoke("ChatMsg", new object[]
					{
						(int)this.m_channelID,
						chatMessage.m_date,
						chatMessage.m_name,
						chatMessage.m_message
					});
				}
			}
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x000544A0 File Offset: 0x000526A0
		public void RemoveUser(User user)
		{
			this.m_users.Remove(user);
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x000544B0 File Offset: 0x000526B0
		public void AddMessage(RPC rpc, string message)
		{
			User userByRPC = this.GetUserByRPC(rpc);
			if (userByRPC != null)
			{
				this.AddMessage(userByRPC.m_name, message);
			}
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x000544D8 File Offset: 0x000526D8
		public void AddMessage(string name, string message)
		{
			long num = DateTime.Now.ToBinary();
			foreach (User user in this.m_users)
			{
				user.m_rpc.Invoke("ChatMsg", new object[]
				{
					(int)this.m_channelID,
					num,
					name,
					message
				});
			}
			this.m_messages.Enqueue(new ChatChannel.ChatMessage(num, name, message));
			while (this.m_messages.Count > 40)
			{
				this.m_messages.Dequeue();
			}
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x000545B0 File Offset: 0x000527B0
		public void Save(BinaryWriter stream)
		{
			stream.Write(this.m_messages.Count);
			foreach (ChatChannel.ChatMessage chatMessage in this.m_messages)
			{
				stream.Write(chatMessage.m_date);
				stream.Write(chatMessage.m_name);
				stream.Write(chatMessage.m_message);
			}
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00054648 File Offset: 0x00052848
		public void Load(BinaryReader stream)
		{
			this.m_messages.Clear();
			int num = stream.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ChatChannel.ChatMessage item = default(ChatChannel.ChatMessage);
				item.m_date = stream.ReadInt64();
				item.m_name = stream.ReadString();
				item.m_message = stream.ReadString();
				this.m_messages.Enqueue(item);
			}
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x000546B4 File Offset: 0x000528B4
		private User GetUserByRPC(RPC rpc)
		{
			foreach (User user in this.m_users)
			{
				if (user.m_rpc == rpc)
				{
					return user;
				}
			}
			return null;
		}

		// Token: 0x0400098A RID: 2442
		private const int m_maxMessages = 40;

		// Token: 0x0400098B RID: 2443
		private ChannelID m_channelID;

		// Token: 0x0400098C RID: 2444
		private Queue<ChatChannel.ChatMessage> m_messages = new Queue<ChatChannel.ChatMessage>();

		// Token: 0x0400098D RID: 2445
		private List<User> m_users = new List<User>();

		// Token: 0x0200012C RID: 300
		private struct ChatMessage
		{
			// Token: 0x06000BC2 RID: 3010 RVA: 0x0005472C File Offset: 0x0005292C
			public ChatMessage(long date, string name, string msg)
			{
				this.m_date = date;
				this.m_name = name;
				this.m_message = msg;
			}

			// Token: 0x0400098E RID: 2446
			public long m_date;

			// Token: 0x0400098F RID: 2447
			public string m_name;

			// Token: 0x04000990 RID: 2448
			public string m_message;
		}
	}
}
