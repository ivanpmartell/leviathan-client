using System;
using System.IO;

// Token: 0x020000A7 RID: 167
public class AIState<OwnerType>
{
	// Token: 0x06000615 RID: 1557 RVA: 0x0002E7F0 File Offset: 0x0002C9F0
	public virtual string GetStatusText()
	{
		return string.Empty;
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0002E7F8 File Offset: 0x0002C9F8
	public virtual void Enter(OwnerType owner, AIStateMachine<OwnerType> sm)
	{
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x0002E7FC File Offset: 0x0002C9FC
	public virtual void Exit(OwnerType owner)
	{
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0002E800 File Offset: 0x0002CA00
	public virtual void Update(OwnerType owner, AIStateMachine<OwnerType> sm, float dt)
	{
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0002E804 File Offset: 0x0002CA04
	public virtual void Save(BinaryWriter writer)
	{
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x0002E808 File Offset: 0x0002CA08
	public virtual void Load(BinaryReader reader)
	{
	}

	// Token: 0x0600061B RID: 1563 RVA: 0x0002E80C File Offset: 0x0002CA0C
	public virtual string DebugString(OwnerType owner)
	{
		return string.Empty;
	}

	// Token: 0x0600061C RID: 1564 RVA: 0x0002E814 File Offset: 0x0002CA14
	public virtual void GetCharageLevel(out float time, out float totalTime)
	{
		time = -1f;
		totalTime = -1f;
	}
}
