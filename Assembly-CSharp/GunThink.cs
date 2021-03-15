using System;

// Token: 0x02000097 RID: 151
internal class GunThink : AIState<Gun>
{
	// Token: 0x060005C9 RID: 1481 RVA: 0x0002D6AC File Offset: 0x0002B8AC
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		owner.SetTarget(null);
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0002D6B8 File Offset: 0x0002B8B8
	public override string GetStatusText()
	{
		if (this.m_noAmmo)
		{
			return "Out of ammo";
		}
		return "Playing cards";
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0002D6D0 File Offset: 0x0002B8D0
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		Unit unit = owner.GetUnit();
		if (unit.GetAi().m_inactive)
		{
			sm.ChangeState("off");
			return;
		}
		if (owner.GetLoadedSalvo() == 0f && owner.GetAmmo() == 0)
		{
			this.m_noAmmo = true;
			return;
		}
		this.m_noAmmo = false;
		if (owner.m_canDeploy)
		{
			if (owner.GetDeploy())
			{
				sm.PushState("deploy");
				return;
			}
		}
		else if (owner.GetFirstOrder() != null)
		{
			sm.PushState("followorder");
		}
		else
		{
			sm.PushState("guard");
		}
	}

	// Token: 0x040004B2 RID: 1202
	private bool m_noAmmo;
}
