using System;
using System.IO;
using UnityEngine;

// Token: 0x0200007F RID: 127
[Serializable]
public class ShipAISettings
{
	// Token: 0x06000544 RID: 1348 RVA: 0x0002AEEC File Offset: 0x000290EC
	public virtual void SaveState(BinaryWriter writer)
	{
		writer.Write((int)this.m_targetOwner);
		writer.Write((int)this.m_mission);
		writer.Write((int)this.m_combatStyle);
		if (this.GetTarget() == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(this.GetTarget().GetComponent<NetObj>().GetNetID());
		}
		if (this.GetOnCombat() == null)
		{
			writer.Write(0);
		}
		else
		{
			writer.Write(this.GetOnCombat().GetComponent<NetObj>().GetNetID());
		}
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x0002AF84 File Offset: 0x00029184
	public virtual void LoadState(BinaryReader reader)
	{
		this.m_targetOwner = (ShipAISettings.PlayerId)reader.ReadInt32();
		this.m_mission = (ShipAISettings.AiMission)reader.ReadInt32();
		this.m_combatStyle = (ShipAISettings.AiCombat)reader.ReadInt32();
		this.m_targetNetID = reader.ReadInt32();
		this.m_onCombatNetID = reader.ReadInt32();
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x0002AFD0 File Offset: 0x000291D0
	public void Transfer(ShipAISettings aiSetting)
	{
		if (aiSetting.m_combatStyle != ShipAISettings.AiCombat.None)
		{
			this.m_combatStyle = aiSetting.m_combatStyle;
		}
		if (aiSetting.m_mission != ShipAISettings.AiMission.None)
		{
			this.m_mission = aiSetting.m_mission;
		}
		this.m_target = aiSetting.m_target;
		this.m_targetNetID = aiSetting.m_targetNetID;
		this.m_onCombat = aiSetting.m_onCombat;
		this.m_onCombatNetID = aiSetting.m_onCombatNetID;
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x0002B03C File Offset: 0x0002923C
	public GameObject GetTarget()
	{
		if (this.m_target != null)
		{
			return this.m_target;
		}
		if (this.m_targetNetID == 0)
		{
			return null;
		}
		this.m_target = NetObj.GetByID(this.m_targetNetID).gameObject;
		return this.m_target;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0002B08C File Offset: 0x0002928C
	public GameObject GetOnCombat()
	{
		if (this.m_onCombat != null)
		{
			return this.m_onCombat;
		}
		if (this.m_onCombatNetID == 0)
		{
			return null;
		}
		this.m_onCombat = NetObj.GetByID(this.m_onCombatNetID).gameObject;
		return this.m_onCombat;
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0002B0DC File Offset: 0x000292DC
	public void RunOnCombatEvent()
	{
		if (this.m_onCombatNetID == 0)
		{
			return;
		}
		NetObj byID = NetObj.GetByID(this.m_onCombatNetID);
		if (byID)
		{
			byID.gameObject.GetComponent<MNode>().DoAction();
			this.m_onCombatNetID = 0;
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0002B124 File Offset: 0x00029324
	public void OnDrawGizmosSelected(GameObject parent)
	{
		GameObject target = this.GetTarget();
		if (target == null)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(parent.transform.position, target.transform.position);
	}

	// Token: 0x04000447 RID: 1095
	public ShipAISettings.PlayerId m_targetOwner = ShipAISettings.PlayerId.Enemy;

	// Token: 0x04000448 RID: 1096
	public ShipAISettings.AiMission m_mission = ShipAISettings.AiMission.Defend;

	// Token: 0x04000449 RID: 1097
	public ShipAISettings.AiCombat m_combatStyle;

	// Token: 0x0400044A RID: 1098
	public GameObject m_target;

	// Token: 0x0400044B RID: 1099
	public GameObject m_onCombat;

	// Token: 0x0400044C RID: 1100
	private int m_targetNetID;

	// Token: 0x0400044D RID: 1101
	private int m_onCombatNetID;

	// Token: 0x02000080 RID: 128
	public enum AiMission
	{
		// Token: 0x0400044F RID: 1103
		None,
		// Token: 0x04000450 RID: 1104
		Defend,
		// Token: 0x04000451 RID: 1105
		Attack,
		// Token: 0x04000452 RID: 1106
		Patrol,
		// Token: 0x04000453 RID: 1107
		Goto,
		// Token: 0x04000454 RID: 1108
		BossC1M3,
		// Token: 0x04000455 RID: 1109
		Inactive
	}

	// Token: 0x02000081 RID: 129
	public enum AiCombat
	{
		// Token: 0x04000457 RID: 1111
		None,
		// Token: 0x04000458 RID: 1112
		Offensive,
		// Token: 0x04000459 RID: 1113
		Defensive,
		// Token: 0x0400045A RID: 1114
		Suicide
	}

	// Token: 0x02000082 RID: 130
	public enum PlayerId
	{
		// Token: 0x0400045C RID: 1116
		NoChange = -1,
		// Token: 0x0400045D RID: 1117
		Player1,
		// Token: 0x0400045E RID: 1118
		Player2,
		// Token: 0x0400045F RID: 1119
		Player3,
		// Token: 0x04000460 RID: 1120
		Player4,
		// Token: 0x04000461 RID: 1121
		Neutral,
		// Token: 0x04000462 RID: 1122
		Enemy,
		// Token: 0x04000463 RID: 1123
		Enemy2,
		// Token: 0x04000464 RID: 1124
		Enemy3,
		// Token: 0x04000465 RID: 1125
		Enemy4,
		// Token: 0x04000466 RID: 1126
		Enemy5
	}
}
