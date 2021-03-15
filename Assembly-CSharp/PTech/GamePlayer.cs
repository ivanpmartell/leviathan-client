using System;
using System.IO;

namespace PTech
{
	// Token: 0x02000142 RID: 322
	internal class GamePlayer
	{
		// Token: 0x06000C78 RID: 3192 RVA: 0x000597F0 File Offset: 0x000579F0
		public GamePlayer(int id)
		{
			this.m_id = id;
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00059840 File Offset: 0x00057A40
		public void SetUser(User u)
		{
			this.m_user = u;
			this.m_userName = u.m_name;
			this.m_user.m_inGames++;
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x00059874 File Offset: 0x00057A74
		public User GetUser()
		{
			return this.m_user;
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0005987C File Offset: 0x00057A7C
		public PlayerPresenceStatus GetPlayerPresenceStatus()
		{
			if (this.m_user == null)
			{
				return PlayerPresenceStatus.Offline;
			}
			if (this.m_inGame)
			{
				return PlayerPresenceStatus.InGame;
			}
			if (this.m_user.m_rpc != null)
			{
				return PlayerPresenceStatus.Online;
			}
			return PlayerPresenceStatus.Offline;
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x000598AC File Offset: 0x00057AAC
		public void Save(BinaryWriter stream)
		{
			stream.Write(this.m_userName);
			stream.Write(this.m_team);
			stream.Write(this.m_leftGame);
			stream.Write(this.m_seenEndGame);
			stream.Write(this.m_dead);
			stream.Write(this.m_surrender);
			stream.Write(this.m_admin);
			stream.Write(this.m_readyToStart);
			stream.Write(this.m_selectedFleetName);
			stream.Write(this.m_score);
			stream.Write(this.m_teamScore);
			stream.Write(this.m_place);
			stream.Write(this.m_flagshipKiller[0]);
			stream.Write(this.m_flagshipKiller[1]);
			if (this.m_fleet != null)
			{
				byte[] array = this.m_fleet.ToArray();
				stream.Write(array.Length);
				stream.Write(array);
			}
			else
			{
				stream.Write(-1);
			}
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x00059998 File Offset: 0x00057B98
		public void Load(BinaryReader stream)
		{
			this.m_userName = stream.ReadString();
			this.m_team = stream.ReadInt32();
			this.m_leftGame = stream.ReadBoolean();
			this.m_seenEndGame = stream.ReadBoolean();
			this.m_dead = stream.ReadBoolean();
			this.m_surrender = stream.ReadBoolean();
			this.m_admin = stream.ReadBoolean();
			this.m_readyToStart = stream.ReadBoolean();
			this.m_selectedFleetName = stream.ReadString();
			this.m_score = stream.ReadInt32();
			this.m_teamScore = stream.ReadInt32();
			this.m_place = stream.ReadInt32();
			this.m_flagshipKiller[0] = stream.ReadInt32();
			this.m_flagshipKiller[1] = stream.ReadInt32();
			int num = stream.ReadInt32();
			if (num != -1)
			{
				byte[] data = stream.ReadBytes(num);
				this.m_fleet = new FleetDef(data);
			}
			else
			{
				this.m_fleet = null;
			}
		}

		// Token: 0x04000A0D RID: 2573
		public string m_userName;

		// Token: 0x04000A0E RID: 2574
		public bool m_inGame;

		// Token: 0x04000A0F RID: 2575
		public int m_id;

		// Token: 0x04000A10 RID: 2576
		public int m_team;

		// Token: 0x04000A11 RID: 2577
		public bool m_leftGame;

		// Token: 0x04000A12 RID: 2578
		public bool m_seenEndGame;

		// Token: 0x04000A13 RID: 2579
		public bool m_dead;

		// Token: 0x04000A14 RID: 2580
		public bool m_surrender;

		// Token: 0x04000A15 RID: 2581
		public bool m_admin;

		// Token: 0x04000A16 RID: 2582
		public bool m_readyToStart;

		// Token: 0x04000A17 RID: 2583
		public string m_selectedFleetName = string.Empty;

		// Token: 0x04000A18 RID: 2584
		public FleetDef m_fleet;

		// Token: 0x04000A19 RID: 2585
		public int m_score = -1;

		// Token: 0x04000A1A RID: 2586
		public int m_teamScore = -1;

		// Token: 0x04000A1B RID: 2587
		public int m_place = -1;

		// Token: 0x04000A1C RID: 2588
		public int[] m_flagshipKiller = new int[]
		{
			-1,
			-1
		};

		// Token: 0x04000A1D RID: 2589
		private User m_user;
	}
}
