using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x0200012D RID: 301
	internal class ChatServer
	{
		// Token: 0x06000BC3 RID: 3011 RVA: 0x00054744 File Offset: 0x00052944
		public ChatServer()
		{
			this.m_channels[0] = new ChatChannel(ChannelID.General);
			this.m_channels[1] = new ChatChannel(ChannelID.Team0);
			this.m_channels[2] = new ChatChannel(ChannelID.Team1);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x00054790 File Offset: 0x00052990
		public void Register(GamePlayer player, bool sendOldMessages)
		{
			User user = player.GetUser();
			this.m_channels[0].AddUser(user, sendOldMessages);
			if (player.m_team == 0)
			{
				this.m_channels[1].AddUser(user, sendOldMessages);
			}
			else if (player.m_team == 1)
			{
				this.m_channels[2].AddUser(user, sendOldMessages);
			}
			user.m_rpc.Register("ChatMessage", new RPC.Handler(this.RPC_ChatMessage));
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0005480C File Offset: 0x00052A0C
		public void Unregister(GamePlayer player)
		{
			User user = player.GetUser();
			foreach (ChatChannel chatChannel in this.m_channels)
			{
				chatChannel.RemoveUser(user);
			}
			if (user.m_rpc != null)
			{
				user.m_rpc.Unregister("ChatMessage");
			}
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x00054864 File Offset: 0x00052A64
		private void RPC_ChatMessage(RPC rpc, List<object> args)
		{
			int num = (int)args[0];
			string message = (string)args[1];
			if (num < 0 || num >= this.m_channels.Length)
			{
				return;
			}
			this.m_channels[num].AddMessage(rpc, message);
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x000548B0 File Offset: 0x00052AB0
		public void Save(BinaryWriter stream)
		{
			foreach (ChatChannel chatChannel in this.m_channels)
			{
				chatChannel.Save(stream);
			}
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x000548E4 File Offset: 0x00052AE4
		public void Load(BinaryReader stream)
		{
			foreach (ChatChannel chatChannel in this.m_channels)
			{
				chatChannel.Load(stream);
			}
		}

		// Token: 0x04000991 RID: 2449
		private ChatChannel[] m_channels = new ChatChannel[3];
	}
}
