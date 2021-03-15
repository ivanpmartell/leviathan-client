using System;
using System.Collections.Generic;
using System.IO;

namespace PTech
{
	// Token: 0x02000159 RID: 345
	internal class TurnData
	{
		// Token: 0x06000CE8 RID: 3304 RVA: 0x0005CA6C File Offset: 0x0005AC6C
		public TurnData(int turn, int players, int playbackFrames, int frames, TurnType type)
		{
			this.m_turn = turn;
			this.m_type = type;
			this.m_playbackFrames = playbackFrames;
			this.m_frames = frames;
			for (int i = 0; i < players; i++)
			{
				this.m_newOrders.Add(null);
			}
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0005CAE0 File Offset: 0x0005ACE0
		public TurnData()
		{
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0005CB18 File Offset: 0x0005AD18
		public bool AllCommited()
		{
			using (List<byte[]>.Enumerator enumerator = this.m_newOrders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x0005CB88 File Offset: 0x0005AD88
		public bool Commited(int playerID)
		{
			return this.m_newOrders[playerID] != null;
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x0005CB9C File Offset: 0x0005AD9C
		public int GetTurn()
		{
			return this.m_turn;
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0005CBA4 File Offset: 0x0005ADA4
		public int GetPlaybackFrames()
		{
			return this.m_playbackFrames;
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0005CBAC File Offset: 0x0005ADAC
		public int GetFrames()
		{
			return this.m_frames;
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0005CBB4 File Offset: 0x0005ADB4
		public TurnType GetTurnType()
		{
			return this.m_type;
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0005CBBC File Offset: 0x0005ADBC
		public void SetSurrender(int player)
		{
			if (!this.m_newSurrenders.Contains(player))
			{
				this.m_newSurrenders.Add(player);
			}
		}

		// Token: 0x06000CF1 RID: 3313 RVA: 0x0005CBDC File Offset: 0x0005ADDC
		public void Save(BinaryWriter stream)
		{
			this.SaveByteArray(this.m_startState, stream);
			this.SaveByteArray(this.m_startOrders, stream);
			this.SaveByteArray(this.m_endState, stream);
			this.SaveByteArray(this.m_endOrders, stream);
			stream.Write(this.m_newOrders.Count);
			foreach (byte[] data in this.m_newOrders)
			{
				this.SaveByteArray(data, stream);
			}
			stream.Write(this.m_startSurrenders.Length);
			foreach (int num in this.m_startSurrenders)
			{
				stream.Write((byte)num);
			}
			stream.Write(this.m_newSurrenders.Count);
			foreach (int num2 in this.m_newSurrenders)
			{
				stream.Write((byte)num2);
			}
			stream.Write(this.m_turn);
			stream.Write((int)this.m_type);
			stream.Write(this.m_playbackFrames);
			stream.Write(this.m_frames);
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0005CD60 File Offset: 0x0005AF60
		public void Load(BinaryReader stream)
		{
			this.m_startState = this.LoadByteArray(stream);
			this.m_startOrders = this.LoadByteArray(stream);
			this.m_endState = this.LoadByteArray(stream);
			this.m_endOrders = this.LoadByteArray(stream);
			int num = stream.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				this.m_newOrders.Add(this.LoadByteArray(stream));
			}
			int num2 = stream.ReadInt32();
			this.m_startSurrenders = new int[num2];
			for (int j = 0; j < num2; j++)
			{
				this.m_startSurrenders[j] = (int)stream.ReadByte();
			}
			int num3 = stream.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				this.m_newSurrenders.Add((int)stream.ReadByte());
			}
			this.m_turn = stream.ReadInt32();
			this.m_type = (TurnType)stream.ReadInt32();
			this.m_playbackFrames = stream.ReadInt32();
			this.m_frames = stream.ReadInt32();
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x0005CE60 File Offset: 0x0005B060
		private void SaveByteArray(byte[] data, BinaryWriter stream)
		{
			if (data == null)
			{
				stream.Write(-1);
			}
			else
			{
				stream.Write(data.Length);
				stream.Write(data);
			}
		}

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0005CE90 File Offset: 0x0005B090
		private byte[] LoadByteArray(BinaryReader stream)
		{
			int num = stream.ReadInt32();
			if (num == -1)
			{
				return null;
			}
			return stream.ReadBytes(num);
		}

		// Token: 0x04000AA4 RID: 2724
		public byte[] m_startState;

		// Token: 0x04000AA5 RID: 2725
		public byte[] m_startOrders;

		// Token: 0x04000AA6 RID: 2726
		public int[] m_startSurrenders = new int[0];

		// Token: 0x04000AA7 RID: 2727
		public byte[] m_endState;

		// Token: 0x04000AA8 RID: 2728
		public byte[] m_endOrders;

		// Token: 0x04000AA9 RID: 2729
		public List<byte[]> m_newOrders = new List<byte[]>();

		// Token: 0x04000AAA RID: 2730
		public List<int> m_newSurrenders = new List<int>();

		// Token: 0x04000AAB RID: 2731
		private int m_turn;

		// Token: 0x04000AAC RID: 2732
		private TurnType m_type;

		// Token: 0x04000AAD RID: 2733
		private int m_playbackFrames;

		// Token: 0x04000AAE RID: 2734
		private int m_frames;
	}
}
