using System;
using System.Collections.Generic;
using PTech;

// Token: 0x020000AA RID: 170
public class ChatClient
{
	// Token: 0x06000630 RID: 1584 RVA: 0x0002ED00 File Offset: 0x0002CF00
	public ChatClient(RPC rpc)
	{
		this.m_rpc = rpc;
		this.m_rpc.Register("ChatMsg", new RPC.Handler(this.RPC_ChatMessage));
		for (int i = 0; i < 3; i++)
		{
			this.m_channels[i] = new Queue<ChatClient.ChatMessage>();
		}
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0002ED64 File Offset: 0x0002CF64
	public void Close()
	{
		this.m_rpc.Unregister("ChatMsg");
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0002ED78 File Offset: 0x0002CF78
	private void RPC_ChatMessage(RPC rpc, List<object> args)
	{
		ChannelID channelID = (ChannelID)((int)args[0]);
		DateTime date = DateTime.FromBinary((long)args[1]);
		string name = (string)args[2];
		string msg = (string)args[3];
		ChatClient.ChatMessage chatMessage = new ChatClient.ChatMessage(date, name, msg);
		this.CacheMessage(channelID, chatMessage);
		if (this.m_onNewMessage != null)
		{
			this.m_onNewMessage(channelID, chatMessage);
		}
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x0002EDEC File Offset: 0x0002CFEC
	public void SendMessage(ChannelID channel, string message)
	{
		this.m_rpc.Invoke("ChatMessage", new object[]
		{
			(int)channel,
			message
		});
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0002EE14 File Offset: 0x0002D014
	private void CacheMessage(ChannelID channel, ChatClient.ChatMessage msg)
	{
		Queue<ChatClient.ChatMessage> queue = this.m_channels[(int)channel];
		queue.Enqueue(msg);
		while (queue.Count > 40)
		{
			queue.Dequeue();
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0002EE4C File Offset: 0x0002D04C
	public List<ChatClient.ChatMessage> GetAllMessages(ChannelID channel)
	{
		List<ChatClient.ChatMessage> list = new List<ChatClient.ChatMessage>();
		Queue<ChatClient.ChatMessage> queue = this.m_channels[(int)channel];
		foreach (ChatClient.ChatMessage item in queue)
		{
			list.Add(item);
		}
		return list;
	}

	// Token: 0x040004BF RID: 1215
	private const int m_maxQueueSize = 40;

	// Token: 0x040004C0 RID: 1216
	public Action<ChannelID, ChatClient.ChatMessage> m_onNewMessage;

	// Token: 0x040004C1 RID: 1217
	private RPC m_rpc;

	// Token: 0x040004C2 RID: 1218
	private Queue<ChatClient.ChatMessage>[] m_channels = new Queue<ChatClient.ChatMessage>[3];

	// Token: 0x020000AB RID: 171
	public struct ChatMessage
	{
		// Token: 0x06000636 RID: 1590 RVA: 0x0002EEC0 File Offset: 0x0002D0C0
		public ChatMessage(DateTime date, string name, string msg)
		{
			this.m_date = date;
			this.m_name = name;
			this.m_message = msg;
		}

		// Token: 0x040004C3 RID: 1219
		public DateTime m_date;

		// Token: 0x040004C4 RID: 1220
		public string m_name;

		// Token: 0x040004C5 RID: 1221
		public string m_message;
	}
}
