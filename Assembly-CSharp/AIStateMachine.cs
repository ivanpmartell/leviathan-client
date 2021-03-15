using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020000A8 RID: 168
public class AIStateMachine<OwnerType>
{
	// Token: 0x0600061D RID: 1565 RVA: 0x0002E824 File Offset: 0x0002CA24
	public AIStateMachine(OwnerType owner, GenericFactory<AIState<OwnerType>> stateFactory)
	{
		this.m_stateFactory = stateFactory;
		this.m_owner = owner;
	}

	// Token: 0x0600061E RID: 1566 RVA: 0x0002E848 File Offset: 0x0002CA48
	public AIState<OwnerType> GetActiveState()
	{
		if (this.m_stateStack.Count > 0)
		{
			return this.m_stateStack.Peek();
		}
		return null;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x0002E868 File Offset: 0x0002CA68
	public AIState<OwnerType> ChangeState(string stateName)
	{
		AIState<OwnerType> aistate = this.m_stateFactory.Create(stateName, new object[0]);
		if (aistate == null)
		{
			PLog.LogError("Missing ai state " + stateName);
			return null;
		}
		this.ChangeState(aistate);
		return aistate;
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0002E8A8 File Offset: 0x0002CAA8
	public void ChangeState(AIState<OwnerType> newTopState)
	{
		if (this.m_stateStack.Count > 0)
		{
			AIState<OwnerType> aistate = this.m_stateStack.Pop();
			aistate.Exit(this.m_owner);
		}
		this.m_stateStack.Push(newTopState);
		newTopState.Enter(this.m_owner, this);
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0002E8F8 File Offset: 0x0002CAF8
	public void PushState(string stateName)
	{
		AIState<OwnerType> aistate = this.m_stateFactory.Create(stateName, new object[0]);
		if (aistate == null)
		{
			PLog.LogError("Missing ai state " + stateName);
			return;
		}
		this.PushState(aistate);
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0002E938 File Offset: 0x0002CB38
	public void PushState(AIState<OwnerType> newTopState)
	{
		if (this.m_stateStack.Count > 0)
		{
			AIState<OwnerType> aistate = this.m_stateStack.Peek();
			if (aistate != null)
			{
				aistate.Exit(this.m_owner);
			}
		}
		this.m_stateStack.Push(newTopState);
		newTopState.Enter(this.m_owner, this);
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0002E990 File Offset: 0x0002CB90
	public void PopChildStates(AIState<OwnerType> parent)
	{
		while (this.m_stateStack.Count > 0 && this.m_stateStack.Peek() != parent)
		{
			this.PopState();
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0002E9C0 File Offset: 0x0002CBC0
	public void PopState()
	{
		if (this.m_stateStack.Count == 0)
		{
			return;
		}
		AIState<OwnerType> aistate = this.m_stateStack.Pop();
		aistate.Exit(this.m_owner);
		if (this.m_stateStack.Count > 0)
		{
			AIState<OwnerType> aistate2 = this.m_stateStack.Peek();
			aistate2.Enter(this.m_owner, this);
		}
		if (this.m_stateStack.Count == 0)
		{
			PLog.LogWarning("Warning, statemachine is empty");
		}
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0002EA3C File Offset: 0x0002CC3C
	public void Update(float dt)
	{
		if (this.m_stateStack.Count > 0)
		{
			AIState<OwnerType> aistate = this.m_stateStack.Peek();
			if (aistate != null)
			{
				aistate.Update(this.m_owner, this, dt);
			}
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0002EA7C File Offset: 0x0002CC7C
	public void Save(BinaryWriter writer)
	{
		writer.Write((byte)this.m_stateStack.Count);
		AIState<OwnerType>[] array = this.m_stateStack.ToArray();
		for (int i = array.Length - 1; i >= 0; i--)
		{
			AIState<OwnerType> aistate = array[i];
			string typeName = this.m_stateFactory.GetTypeName(aistate);
			writer.Write(typeName);
			aistate.Save(writer);
		}
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0002EADC File Offset: 0x0002CCDC
	public void Load(BinaryReader reader)
	{
		int num = (int)reader.ReadByte();
		this.m_stateStack.Clear();
		for (int i = 0; i < num; i++)
		{
			string name = reader.ReadString();
			AIState<OwnerType> aistate = this.m_stateFactory.Create(name, new object[0]);
			DebugUtils.Assert(aistate != null);
			aistate.Load(reader);
			this.m_stateStack.Push(aistate);
		}
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0002EB48 File Offset: 0x0002CD48
	public override string ToString()
	{
		string text = "AIState:\n";
		foreach (AIState<OwnerType> aistate in this.m_stateStack.ToArray())
		{
			string typeName = this.m_stateFactory.GetTypeName(aistate);
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"   ",
				typeName,
				": ",
				aistate.DebugString(this.m_owner),
				"\n"
			});
		}
		return text;
	}

	// Token: 0x040004B7 RID: 1207
	private GenericFactory<AIState<OwnerType>> m_stateFactory;

	// Token: 0x040004B8 RID: 1208
	private Stack<AIState<OwnerType>> m_stateStack = new Stack<AIState<OwnerType>>();

	// Token: 0x040004B9 RID: 1209
	private OwnerType m_owner;
}
