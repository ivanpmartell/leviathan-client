using System;
using System.IO;
using UnityEngine;

// Token: 0x0200007B RID: 123
[AddComponentMenu("Scripts/Mission/MNPlayers")]
public class MNPlayers : MNode
{
	// Token: 0x06000532 RID: 1330 RVA: 0x0002AA18 File Offset: 0x00028C18
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0002AA20 File Offset: 0x00028C20
	private void FixedUpdate()
	{
		if (NetObj.m_simulating && this.m_atStartup)
		{
			this.DoAction();
			this.m_atStartup = false;
		}
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0002AA50 File Offset: 0x00028C50
	public override void DoAction()
	{
		foreach (MNPlayers.PlayerUpdate playerUpdate in this.m_commands)
		{
			if (playerUpdate.m_type == MNPlayers.PlayerUpdateType.Flag)
			{
				TurnMan.instance.SetPlayerFlag(playerUpdate.m_playerId, playerUpdate.m_parameter);
			}
			if (playerUpdate.m_type == MNPlayers.PlayerUpdateType.Team)
			{
				TurnMan.instance.SetPlayerTeam(playerUpdate.m_playerId, playerUpdate.m_parameter);
			}
			if (playerUpdate.m_type == MNPlayers.PlayerUpdateType.Color)
			{
				TurnMan.instance.SetPlayerColors(playerUpdate.m_playerId, playerUpdate.m_color);
			}
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0002AAE4 File Offset: 0x00028CE4
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_atStartup);
		writer.Write(this.m_commands.Length);
		foreach (MNPlayers.PlayerUpdate playerUpdate in this.m_commands)
		{
			playerUpdate.SaveState(writer);
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0002AB38 File Offset: 0x00028D38
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_atStartup = reader.ReadBoolean();
		int num = reader.ReadInt32();
		this.m_commands = new MNPlayers.PlayerUpdate[num];
		for (int i = 0; i < num; i++)
		{
			MNPlayers.PlayerUpdate playerUpdate = new MNPlayers.PlayerUpdate();
			this.m_commands[i] = playerUpdate;
			playerUpdate.LoadState(reader);
		}
	}

	// Token: 0x0400043B RID: 1083
	public MNPlayers.PlayerUpdate[] m_commands = new MNPlayers.PlayerUpdate[0];

	// Token: 0x0400043C RID: 1084
	public bool m_atStartup;

	// Token: 0x0200007C RID: 124
	public enum PlayerUpdateType
	{
		// Token: 0x0400043E RID: 1086
		Team,
		// Token: 0x0400043F RID: 1087
		Flag,
		// Token: 0x04000440 RID: 1088
		Color
	}

	// Token: 0x0200007D RID: 125
	[Serializable]
	public class PlayerUpdate
	{
		// Token: 0x06000538 RID: 1336 RVA: 0x0002ABC4 File Offset: 0x00028DC4
		public void SaveState(BinaryWriter writer)
		{
			writer.Write(this.m_playerId);
			writer.Write((int)this.m_type);
			writer.Write(this.m_parameter);
			writer.Write(this.m_color.r);
			writer.Write(this.m_color.g);
			writer.Write(this.m_color.b);
			writer.Write(this.m_color.a);
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0002AC3C File Offset: 0x00028E3C
		public void LoadState(BinaryReader reader)
		{
			this.m_playerId = reader.ReadInt32();
			this.m_type = (MNPlayers.PlayerUpdateType)reader.ReadInt32();
			this.m_parameter = reader.ReadInt32();
			this.m_color = new Color
			{
				r = reader.ReadSingle(),
				g = reader.ReadSingle(),
				b = reader.ReadSingle(),
				a = reader.ReadSingle()
			};
		}

		// Token: 0x04000441 RID: 1089
		public int m_playerId;

		// Token: 0x04000442 RID: 1090
		public MNPlayers.PlayerUpdateType m_type;

		// Token: 0x04000443 RID: 1091
		public int m_parameter;

		// Token: 0x04000444 RID: 1092
		public Color m_color = new Color(1f, 1f, 0f);
	}
}
