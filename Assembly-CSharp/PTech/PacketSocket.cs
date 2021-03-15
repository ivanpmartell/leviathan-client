using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace PTech
{
	// Token: 0x02000148 RID: 328
	public class PacketSocket
	{
		// Token: 0x06000C96 RID: 3222 RVA: 0x0005A63C File Offset: 0x0005883C
		public PacketSocket(Socket socket)
		{
			this.m_socket = socket;
			this.m_socket.Blocking = false;
			this.m_thread = new Thread(new ThreadStart(this.RecvThread));
			this.m_thread.Start();
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0005A6C8 File Offset: 0x000588C8
		public bool Send(byte[] packet)
		{
			if (this.m_error)
			{
				return false;
			}
			this.m_sendQueueMutex.WaitOne();
			this.m_sendQueue.Enqueue(packet);
			this.m_sendQueueMutex.ReleaseMutex();
			return true;
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0005A6FC File Offset: 0x000588FC
		public PacketSocket.RecvStatus Recv(out byte[] packet)
		{
			packet = this.GetRecvQueuePackage();
			if (packet != null)
			{
				this.m_received = false;
				return PacketSocket.RecvStatus.PackageReady;
			}
			if (this.m_received)
			{
				this.m_received = false;
				return PacketSocket.RecvStatus.ReceivedData;
			}
			this.m_received = false;
			return PacketSocket.RecvStatus.NoData;
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0005A740 File Offset: 0x00058940
		private void RecvThread()
		{
			while (!this.m_abortThread && !this.m_error)
			{
				try
				{
					this.ReceiveData();
					this.SendData();
				}
				catch (Exception ex)
				{
					this.m_error = true;
					PLog.Log("Socket error:" + ex.ToString());
					break;
				}
				Thread.Sleep(10);
			}
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0005A7C4 File Offset: 0x000589C4
		private void ReceiveData()
		{
			if (this.m_gotSizeBytes < 4)
			{
				SocketError socketError;
				int num = this.m_socket.Receive(this.m_idBuffer, this.m_gotSizeBytes, 4 - this.m_gotSizeBytes, SocketFlags.None, out socketError);
				this.m_gotSizeBytes += num;
				if (num > 0)
				{
					this.m_received = true;
				}
				if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
				{
					this.m_error = true;
				}
				if (this.m_gotSizeBytes == 4)
				{
					this.m_packetSize = BitConverter.ToInt32(this.m_idBuffer, 0);
					this.m_gotPacketBytes = 0;
					this.m_packetBuffer = new byte[this.m_packetSize];
				}
			}
			else
			{
				SocketError socketError2;
				int num2 = this.m_socket.Receive(this.m_packetBuffer, this.m_gotPacketBytes, this.m_packetSize - this.m_gotPacketBytes, SocketFlags.None, out socketError2);
				this.m_gotPacketBytes += num2;
				if (num2 > 0)
				{
					this.m_received = true;
				}
				if (socketError2 != SocketError.Success && socketError2 != SocketError.WouldBlock)
				{
					this.m_error = true;
				}
				if (this.m_gotPacketBytes == this.m_packetSize)
				{
					this.m_recvQueueMutex.WaitOne();
					this.m_recvQueue.Enqueue(this.m_packetBuffer);
					this.m_recvQueueMutex.ReleaseMutex();
					this.m_packetBuffer = null;
					this.m_packetSize = 0;
					this.m_gotSizeBytes = 0;
				}
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0005A91C File Offset: 0x00058B1C
		private void SendData()
		{
			if (this.m_error)
			{
				return;
			}
			for (byte[] sendQueuePackage = this.GetSendQueuePackage(); sendQueuePackage != null; sendQueuePackage = this.GetSendQueuePackage())
			{
				if (!this.SendPackage(sendQueuePackage))
				{
					return;
				}
			}
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0005A960 File Offset: 0x00058B60
		private byte[] GetSendQueuePackage()
		{
			byte[] result = null;
			this.m_sendQueueMutex.WaitOne();
			if (this.m_sendQueue.Count > 0)
			{
				result = this.m_sendQueue.Dequeue();
			}
			this.m_sendQueueMutex.ReleaseMutex();
			return result;
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x0005A9A4 File Offset: 0x00058BA4
		private byte[] GetRecvQueuePackage()
		{
			byte[] result = null;
			this.m_recvQueueMutex.WaitOne();
			if (this.m_recvQueue.Count > 0)
			{
				result = this.m_recvQueue.Dequeue();
			}
			this.m_recvQueueMutex.ReleaseMutex();
			return result;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0005A9E8 File Offset: 0x00058BE8
		public bool SendPackage(byte[] packet)
		{
			int value = packet.Length;
			byte[] bytes = BitConverter.GetBytes(value);
			bool result;
			try
			{
				this.m_socket.Blocking = true;
				this.m_socket.Send(bytes);
				this.m_socket.Send(packet);
				this.m_socket.Blocking = false;
				result = true;
			}
			catch (SocketException ex)
			{
				this.m_error = true;
				result = false;
			}
			return result;
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0005AA70 File Offset: 0x00058C70
		public bool GotError()
		{
			return this.m_error;
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0005AA78 File Offset: 0x00058C78
		public void Close()
		{
			this.m_closeMutex.WaitOne();
			if (this.m_socket != null)
			{
				this.m_socket.Close();
				this.m_thread.Abort();
				this.m_socket = null;
			}
			this.m_closeMutex.ReleaseMutex();
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0005AAC4 File Offset: 0x00058CC4
		public bool IsOpen()
		{
			return this.m_socket != null;
		}

		// Token: 0x04000A45 RID: 2629
		private Socket m_socket;

		// Token: 0x04000A46 RID: 2630
		private bool m_error;

		// Token: 0x04000A47 RID: 2631
		private int m_packetSize;

		// Token: 0x04000A48 RID: 2632
		private int m_gotPacketBytes;

		// Token: 0x04000A49 RID: 2633
		private byte[] m_packetBuffer;

		// Token: 0x04000A4A RID: 2634
		private int m_gotSizeBytes;

		// Token: 0x04000A4B RID: 2635
		private byte[] m_idBuffer = new byte[4];

		// Token: 0x04000A4C RID: 2636
		private Thread m_thread;

		// Token: 0x04000A4D RID: 2637
		private Mutex m_recvQueueMutex = new Mutex();

		// Token: 0x04000A4E RID: 2638
		private Mutex m_sendQueueMutex = new Mutex();

		// Token: 0x04000A4F RID: 2639
		private Mutex m_closeMutex = new Mutex();

		// Token: 0x04000A50 RID: 2640
		private Queue<byte[]> m_recvQueue = new Queue<byte[]>();

		// Token: 0x04000A51 RID: 2641
		private Queue<byte[]> m_sendQueue = new Queue<byte[]>();

		// Token: 0x04000A52 RID: 2642
		private bool m_abortThread;

		// Token: 0x04000A53 RID: 2643
		private bool m_received;

		// Token: 0x02000149 RID: 329
		public enum RecvStatus
		{
			// Token: 0x04000A55 RID: 2645
			PackageReady,
			// Token: 0x04000A56 RID: 2646
			ReceivedData,
			// Token: 0x04000A57 RID: 2647
			NoData
		}
	}
}
