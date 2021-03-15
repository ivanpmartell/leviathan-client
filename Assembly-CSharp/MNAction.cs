using System;
using System.IO;
using UnityEngine;

// Token: 0x02000072 RID: 114
[AddComponentMenu("Scripts/Mission/MNAction")]
public class MNAction : MNode
{
	// Token: 0x060004FA RID: 1274 RVA: 0x00029CD0 File Offset: 0x00027ED0
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00029CD8 File Offset: 0x00027ED8
	private void FixedUpdate()
	{
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00029CDC File Offset: 0x00027EDC
	public virtual void OnDrawGizmosSelected()
	{
		foreach (MNAction.MNActionElement mnactionElement in this.m_commands)
		{
			GameObject target = mnactionElement.GetTarget();
			if (target != null)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(base.GetComponent<Transform>().position, target.GetComponent<Transform>().position);
			}
		}
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00029D40 File Offset: 0x00027F40
	private void AddCommands(MNAction.MNActionElement[] commands)
	{
		if (TurnMan.instance.m_dialog == null)
		{
			TurnMan.instance.m_dialog = this.m_commands;
		}
		else
		{
			MNAction.MNActionElement[] array = new MNAction.MNActionElement[TurnMan.instance.m_dialog.Length + commands.Length];
			TurnMan.instance.m_dialog.CopyTo(array, 0);
			this.m_commands.CopyTo(array, TurnMan.instance.m_dialog.Length);
			TurnMan.instance.m_dialog = TurnMan.instance.m_dialog;
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00029DC4 File Offset: 0x00027FC4
	public override void DoAction()
	{
		Camera component = GameObject.Find("GameCamera").GetComponent<Camera>();
		if (component != null && component.enabled && NetObj.IsSimulating())
		{
			this.AddCommands(this.m_commands);
		}
		else
		{
			Dialog dialog = new Dialog(null, null, null, null);
			dialog.SetCommands(this.m_commands);
			dialog.PlayAll();
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00029E34 File Offset: 0x00028034
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_commands.Length);
		foreach (MNAction.MNActionElement mnactionElement in this.m_commands)
		{
			mnactionElement.SaveState(writer);
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00029E7C File Offset: 0x0002807C
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		int num = reader.ReadInt32();
		this.m_commands = new MNAction.MNActionElement[num];
		for (int i = 0; i < num; i++)
		{
			MNAction.MNActionElement mnactionElement = new MNAction.MNActionElement();
			mnactionElement.LoadState(reader);
			this.m_commands[i] = mnactionElement;
		}
	}

	// Token: 0x04000416 RID: 1046
	public MNAction.MNActionElement[] m_commands = new MNAction.MNActionElement[0];

	// Token: 0x02000073 RID: 115
	public enum ActionType
	{
		// Token: 0x04000418 RID: 1048
		PlayScene,
		// Token: 0x04000419 RID: 1049
		UpdateObjective,
		// Token: 0x0400041A RID: 1050
		MissionVictory,
		// Token: 0x0400041B RID: 1051
		MissionDefeat,
		// Token: 0x0400041C RID: 1052
		MissionGameOver = 3,
		// Token: 0x0400041D RID: 1053
		Marker,
		// Token: 0x0400041E RID: 1054
		Message,
		// Token: 0x0400041F RID: 1055
		PlayerChange,
		// Token: 0x04000420 RID: 1056
		Event,
		// Token: 0x04000421 RID: 1057
		ShowBriefing,
		// Token: 0x04000422 RID: 1058
		ShowTutorial,
		// Token: 0x04000423 RID: 1059
		ShowDebriefing,
		// Token: 0x04000424 RID: 1060
		MissionAchievement
	}

	// Token: 0x02000074 RID: 116
	public enum ObjectiveStatus
	{
		// Token: 0x04000426 RID: 1062
		Hide,
		// Token: 0x04000427 RID: 1063
		Visible,
		// Token: 0x04000428 RID: 1064
		Active,
		// Token: 0x04000429 RID: 1065
		Done
	}

	// Token: 0x02000075 RID: 117
	[Serializable]
	public class MNActionElement
	{
		// Token: 0x06000502 RID: 1282 RVA: 0x00029EE8 File Offset: 0x000280E8
		public void SaveState(BinaryWriter writer)
		{
			writer.Write((int)this.m_type);
			writer.Write(this.m_parameter);
			writer.Write((int)this.m_objectiveStatus);
			if (this.GetTarget() == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(this.GetTarget().GetComponent<NetObj>().GetNetID());
			}
			writer.Write((int)this.m_objectiveType);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00029F58 File Offset: 0x00028158
		public void LoadState(BinaryReader reader)
		{
			this.m_type = (MNAction.ActionType)reader.ReadInt32();
			this.m_parameter = reader.ReadString();
			this.m_objectiveStatus = (MNAction.ObjectiveStatus)reader.ReadInt32();
			this.m_targetNetID = reader.ReadInt32();
			this.m_objectiveType = (Unit.ObjectiveTypes)reader.ReadInt32();
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x00029FA4 File Offset: 0x000281A4
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

		// Token: 0x0400042A RID: 1066
		public MNAction.ActionType m_type;

		// Token: 0x0400042B RID: 1067
		public string m_parameter = string.Empty;

		// Token: 0x0400042C RID: 1068
		public MNAction.ObjectiveStatus m_objectiveStatus;

		// Token: 0x0400042D RID: 1069
		public GameObject m_target;

		// Token: 0x0400042E RID: 1070
		public Unit.ObjectiveTypes m_objectiveType = Unit.ObjectiveTypes.Move;

		// Token: 0x0400042F RID: 1071
		private int m_targetNetID;
	}
}
