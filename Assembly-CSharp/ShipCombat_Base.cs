using System;

// Token: 0x0200009B RID: 155
internal class ShipCombat_Base : AIState<Ship>
{
	// Token: 0x060005E0 RID: 1504 RVA: 0x0002DD34 File Offset: 0x0002BF34
	public override string DebugString(Ship owner)
	{
		Unit unit = NetObj.GetByID(owner.GetAi().m_targetId) as Unit;
		string str = string.Empty;
		str = str + owner.GetAi().m_targetId.ToString() + "\n";
		str = str + "-   Dist: " + this.RangeToTarget(owner).ToString() + "\n";
		str = str + "-   Target is Forward = " + owner.IsPositionForward(unit.transform.position).ToString();
		return str + " / Right = " + owner.IsPositionRight(unit.transform.position).ToString();
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0002DDE8 File Offset: 0x0002BFE8
	public bool SwitchState(Ship owner, AIStateMachine<Ship> sm)
	{
		Unit x = this.VerifyTarget(owner);
		if (x == null)
		{
			sm.ChangeState("combat");
			return false;
		}
		return false;
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0002DE18 File Offset: 0x0002C018
	public Unit VerifyTarget(Ship owner)
	{
		Unit unit = NetObj.GetByID(owner.GetAi().m_targetId) as Unit;
		if (unit == null)
		{
			owner.GetAi().m_targetId = 0;
			return null;
		}
		return unit;
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0002DE58 File Offset: 0x0002C058
	public float RangeToTarget(Ship owner)
	{
		Unit unit = NetObj.GetByID(owner.GetAi().m_targetId) as Unit;
		return (unit.transform.position - owner.transform.position).magnitude;
	}
}
