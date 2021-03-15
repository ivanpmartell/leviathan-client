using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace PTech
{
	// Token: 0x02000150 RID: 336
	public class RPC
	{
		// Token: 0x06000CB1 RID: 3249 RVA: 0x0005ADD4 File Offset: 0x00058FD4
		public RPC(PacketSocket socket)
		{
			this.m_socket = socket;
			this.m_stopWatch.Start();
			this.m_pingThread = new Thread(new ThreadStart(this.PingThread));
			this.m_pingThread.Start();
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0005AE60 File Offset: 0x00059060
		public void SetMainThreadTimeout(float timeInSec)
		{
			this.m_mainThreadTimeout = timeInSec;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0005AE6C File Offset: 0x0005906C
		public bool Update(bool recvAll)
		{
			if (this.m_socket.GotError())
			{
				return false;
			}
			if (!this.m_socket.IsOpen())
			{
				return false;
			}
			float deltaTime = this.GetDeltaTime();
			if (!this.UpdatePingTimeout(deltaTime))
			{
				return false;
			}
			this.UpdateStats(deltaTime);
			PacketSocket.RecvStatus recvStatus;
			do
			{
				byte[] array;
				recvStatus = this.m_socket.Recv(out array);
				if (recvStatus == PacketSocket.RecvStatus.PackageReady)
				{
					this.m_timeSinceLastRecvPackage = 0f;
					this.m_recvData += array.Length;
					this.m_totalRecvData += array.Length;
					MemoryStream input = new MemoryStream(array);
					BinaryReader binaryReader = new BinaryReader(input);
					try
					{
						RPC.PackageType packageType = (RPC.PackageType)binaryReader.ReadByte();
						RPC.PackageType packageType2 = packageType;
						if (packageType2 != RPC.PackageType.Invocation)
						{
							if (packageType2 != RPC.PackageType.Ping)
							{
							}
						}
						else
						{
							this.RecvInvocation(binaryReader);
						}
					}
					catch (Exception ex)
					{
						PLog.LogError("ERROR: exception while handling rpc , disconnecting client: \n " + ex.ToString());
						this.m_socket.Close();
					}
				}
				else if (recvStatus == PacketSocket.RecvStatus.ReceivedData)
				{
					this.m_timeSinceLastRecvPackage = 0f;
				}
			}
			while (recvStatus != PacketSocket.RecvStatus.NoData && recvAll);
			return true;
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0005AFAC File Offset: 0x000591AC
		private void UpdateStats(float dt)
		{
			this.m_updateStatsTimer += dt;
			if (this.m_updateStatsTimer > 1f)
			{
				this.m_sentDataPerSec = (int)((float)this.m_sentData / this.m_updateStatsTimer);
				this.m_recvDataPerSec = (int)((float)this.m_recvData / this.m_updateStatsTimer);
				this.m_sentData = 0;
				this.m_recvData = 0;
				this.m_updateStatsTimer = 0f;
			}
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x0005B01C File Offset: 0x0005921C
		private void RecvInvocation(BinaryReader reader)
		{
			byte[] b = reader.ReadBytes(16);
			Guid key = new Guid(b);
			RPC.Handler handler;
			if (this.m_methods.TryGetValue(key, out handler))
			{
				char c = reader.ReadChar();
				List<object> list = new List<object>();
				for (int i = 0; i < (int)c; i++)
				{
					this.Deserialize(reader, list);
				}
				handler(this, list);
			}
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x0005B088 File Offset: 0x00059288
		private string GetHandlerName(Guid id)
		{
			foreach (KeyValuePair<string, Guid> keyValuePair in this.m_nameToID)
			{
				if (keyValuePair.Value == id)
				{
					return keyValuePair.Key;
				}
			}
			return string.Empty;
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x0005B110 File Offset: 0x00059310
		private float GetDeltaTime()
		{
			float num = (float)this.m_stopWatch.ElapsedMilliseconds / 1000f;
			this.m_stopWatch.Reset();
			this.m_stopWatch.Start();
			if (num > 0.1f)
			{
				num = 0.1f;
			}
			return num;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x0005B158 File Offset: 0x00059358
		private void PingThread()
		{
			float num = 0f;
			float num2 = 0.2f;
			while (!this.m_abortPingThread && this.m_socket.IsOpen())
			{
				num += num2;
				if (num > this.m_pingInterval)
				{
					num = 0f;
					MemoryStream memoryStream = new MemoryStream();
					memoryStream.WriteByte(2);
					this.m_socket.Send(memoryStream.ToArray());
				}
				Thread.Sleep((int)(num2 * 1000f));
				this.m_timeSinceMainThreadUpdate += num2;
				if (this.m_mainThreadTimeout > 0f && this.m_timeSinceMainThreadUpdate > this.m_mainThreadTimeout)
				{
					this.m_socket.Close();
				}
			}
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x0005B210 File Offset: 0x00059410
		private bool UpdatePingTimeout(float dt)
		{
			this.m_timeSinceMainThreadUpdate = 0f;
			this.m_timeSinceLastRecvPackage += dt;
			return this.m_timeSinceLastRecvPackage <= this.m_pingTimeout;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x0005B240 File Offset: 0x00059440
		public void Register(string name, RPC.Handler del)
		{
			Guid guid = this.GenerateGuid(name);
			this.m_nameToID[name] = guid;
			this.m_methods[guid] = del;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0005B270 File Offset: 0x00059470
		public void Unregister(string name)
		{
			Guid key;
			if (this.m_nameToID.TryGetValue(name, out key))
			{
				this.m_nameToID.Remove(name);
				this.m_methods.Remove(key);
			}
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x0005B2B0 File Offset: 0x000594B0
		public void Invoke(string name, List<object> args)
		{
			object[] array = new object[args.Count];
			for (int i = 0; i < args.Count; i++)
			{
				array[i] = args[i];
			}
			this.Invoke(name, array);
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x0005B2F4 File Offset: 0x000594F4
		public void Invoke(string name, params object[] args)
		{
			Guid value;
			if (!this.m_nameToID.TryGetValue(name, out value))
			{
				value = this.GenerateGuid(name);
				this.m_nameToID.Add(name, value);
			}
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			memoryStream.WriteByte(1);
			byte[] array = value.ToByteArray();
			memoryStream.Write(array, 0, array.Length);
			binaryWriter.Write((char)args.Length);
			int num = 0;
			foreach (object data in args)
			{
				try
				{
					this.Serialize(binaryWriter, data);
				}
				catch (Exception ex)
				{
					PLog.LogError("Serialize error: " + ex.Message);
					PLog.LogError(string.Concat(new object[]
					{
						"Error invoking ",
						name,
						" in argument ",
						num
					}));
					throw;
				}
				num++;
			}
			this.m_sentData += (int)memoryStream.Length;
			this.m_totalSentData += (int)memoryStream.Length;
			this.m_socket.Send(memoryStream.ToArray());
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x0005B438 File Offset: 0x00059638
		private void Serialize(BinaryWriter writer, object data)
		{
			if (data is int)
			{
				writer.Write('\0');
				writer.Write((int)data);
			}
			else if (data is bool)
			{
				writer.Write('\u0001');
				writer.Write((bool)data);
			}
			else if (data is float)
			{
				writer.Write('\u0002');
				writer.Write((float)data);
			}
			else if (data is double)
			{
				writer.Write('\u0003');
				writer.Write((double)data);
			}
			else if (data is long)
			{
				writer.Write('\t');
				writer.Write((long)data);
			}
			else if (data is string)
			{
				string value = data as string;
				writer.Write('\u0004');
				writer.Write(value);
			}
			else if (data is byte[])
			{
				byte[] array = data as byte[];
				writer.Write('\u0005');
				writer.Write(array.Length);
				writer.Write(array);
			}
			else if (data is float[])
			{
				float[] array2 = data as float[];
				writer.Write('\u0006');
				writer.Write(array2.Length);
				foreach (float value2 in array2)
				{
					writer.Write(value2);
				}
			}
			else if (data is int[])
			{
				int[] array4 = data as int[];
				writer.Write('\v');
				writer.Write(array4.Length);
				foreach (int value3 in array4)
				{
					writer.Write(value3);
				}
			}
			else if (data is double[])
			{
				double[] array6 = data as double[];
				writer.Write('\a');
				writer.Write(array6.Length);
				foreach (double value4 in array6)
				{
					writer.Write(value4);
				}
			}
			else if (data is string[])
			{
				string[] array8 = data as string[];
				writer.Write('\n');
				writer.Write(array8.Length);
				foreach (string value5 in array8)
				{
					writer.Write(value5);
				}
			}
			else if (data is Dictionary<string, int>)
			{
				writer.Write('\b');
				Dictionary<string, int> dictionary = data as Dictionary<string, int>;
				writer.Write(dictionary.Count);
				foreach (KeyValuePair<string, int> keyValuePair in dictionary)
				{
					writer.Write(keyValuePair.Key);
					writer.Write(keyValuePair.Value);
				}
			}
			else
			{
				if (data == null)
				{
					throw new Exception("Unhandled type, is null ");
				}
				throw new Exception("Unhandled type " + data.GetType().ToString());
			}
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x0005B760 File Offset: 0x00059960
		private void Deserialize(BinaryReader reader, List<object> args)
		{
			RPC.Type type = (RPC.Type)reader.ReadChar();
			switch (type)
			{
			case RPC.Type.Int:
				args.Add(reader.ReadInt32());
				break;
			case RPC.Type.Bool:
				args.Add(reader.ReadBoolean());
				break;
			case RPC.Type.Float:
				args.Add(reader.ReadSingle());
				break;
			case RPC.Type.Double:
				args.Add(reader.ReadDouble());
				break;
			case RPC.Type.String:
				args.Add(reader.ReadString());
				break;
			case RPC.Type.ByteArray:
			{
				int count = reader.ReadInt32();
				byte[] item = reader.ReadBytes(count);
				args.Add(item);
				break;
			}
			case RPC.Type.FloatArray:
			{
				int num = reader.ReadInt32();
				float[] array = new float[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = reader.ReadSingle();
				}
				args.Add(array);
				break;
			}
			case RPC.Type.DoubleArray:
			{
				int num2 = reader.ReadInt32();
				double[] array2 = new double[num2];
				for (int j = 0; j < num2; j++)
				{
					array2[j] = reader.ReadDouble();
				}
				args.Add(array2);
				break;
			}
			case RPC.Type.StringIntDic:
			{
				int num3 = reader.ReadInt32();
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				for (int k = 0; k < num3; k++)
				{
					string key = reader.ReadString();
					int value = reader.ReadInt32();
					dictionary.Add(key, value);
				}
				args.Add(dictionary);
				break;
			}
			case RPC.Type.Long:
				args.Add(reader.ReadInt64());
				break;
			case RPC.Type.StringArray:
			{
				int num4 = reader.ReadInt32();
				string[] array3 = new string[num4];
				for (int l = 0; l < num4; l++)
				{
					array3[l] = reader.ReadString();
				}
				args.Add(array3);
				break;
			}
			case RPC.Type.IntArray:
			{
				int num5 = reader.ReadInt32();
				int[] array4 = new int[num5];
				for (int m = 0; m < num5; m++)
				{
					array4[m] = reader.ReadInt32();
				}
				args.Add(array4);
				break;
			}
			default:
				throw new Exception("Unhandled type " + type.ToString());
			}
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x0005B9B4 File Offset: 0x00059BB4
		private Guid GenerateGuid(string name)
		{
			Guid result;
			using (MD5 md = MD5.Create())
			{
				byte[] b = md.ComputeHash(Encoding.Default.GetBytes(name));
				Guid guid = new Guid(b);
				result = guid;
			}
			return result;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0005BA18 File Offset: 0x00059C18
		public void Close()
		{
			this.m_abortPingThread = true;
			this.m_pingThread.Join();
			this.m_socket.Close();
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0005BA38 File Offset: 0x00059C38
		public int GetTotalSentData()
		{
			return this.m_totalSentData;
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x0005BA40 File Offset: 0x00059C40
		public int GetTotalRecvData()
		{
			return this.m_totalRecvData;
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x0005BA48 File Offset: 0x00059C48
		public int GetSentDataPerSec()
		{
			return this.m_sentDataPerSec;
		}

		// Token: 0x06000CC5 RID: 3269 RVA: 0x0005BA50 File Offset: 0x00059C50
		public int GetRecvDataPerSec()
		{
			return this.m_recvDataPerSec;
		}

		// Token: 0x04000A71 RID: 2673
		private float m_pingTimeout = 16f;

		// Token: 0x04000A72 RID: 2674
		private float m_pingInterval = 1f;

		// Token: 0x04000A73 RID: 2675
		private float m_timeSinceLastRecvPackage;

		// Token: 0x04000A74 RID: 2676
		private float m_mainThreadTimeout = -1f;

		// Token: 0x04000A75 RID: 2677
		private float m_timeSinceMainThreadUpdate;

		// Token: 0x04000A76 RID: 2678
		private Thread m_pingThread;

		// Token: 0x04000A77 RID: 2679
		private bool m_abortPingThread;

		// Token: 0x04000A78 RID: 2680
		private Stopwatch m_stopWatch = new Stopwatch();

		// Token: 0x04000A79 RID: 2681
		private PacketSocket m_socket;

		// Token: 0x04000A7A RID: 2682
		private Dictionary<string, Guid> m_nameToID = new Dictionary<string, Guid>();

		// Token: 0x04000A7B RID: 2683
		private Dictionary<Guid, RPC.Handler> m_methods = new Dictionary<Guid, RPC.Handler>();

		// Token: 0x04000A7C RID: 2684
		private int m_totalSentData;

		// Token: 0x04000A7D RID: 2685
		private int m_totalRecvData;

		// Token: 0x04000A7E RID: 2686
		private int m_sentData;

		// Token: 0x04000A7F RID: 2687
		private int m_recvData;

		// Token: 0x04000A80 RID: 2688
		private int m_sentDataPerSec;

		// Token: 0x04000A81 RID: 2689
		private int m_recvDataPerSec;

		// Token: 0x04000A82 RID: 2690
		private float m_updateStatsTimer;

		// Token: 0x02000151 RID: 337
		private enum Type
		{
			// Token: 0x04000A84 RID: 2692
			Int,
			// Token: 0x04000A85 RID: 2693
			Bool,
			// Token: 0x04000A86 RID: 2694
			Float,
			// Token: 0x04000A87 RID: 2695
			Double,
			// Token: 0x04000A88 RID: 2696
			String,
			// Token: 0x04000A89 RID: 2697
			ByteArray,
			// Token: 0x04000A8A RID: 2698
			FloatArray,
			// Token: 0x04000A8B RID: 2699
			DoubleArray,
			// Token: 0x04000A8C RID: 2700
			StringIntDic,
			// Token: 0x04000A8D RID: 2701
			Long,
			// Token: 0x04000A8E RID: 2702
			StringArray,
			// Token: 0x04000A8F RID: 2703
			IntArray
		}

		// Token: 0x02000152 RID: 338
		private enum PackageType
		{
			// Token: 0x04000A91 RID: 2705
			Invocation = 1,
			// Token: 0x04000A92 RID: 2706
			Ping
		}

		// Token: 0x020001B3 RID: 435
		// (Invoke) Token: 0x06000F7C RID: 3964
		public delegate void Handler(RPC rpc, List<object> arg);
	}
}
