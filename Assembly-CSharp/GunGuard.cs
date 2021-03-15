using System;
using System.IO;

// Token: 0x02000094 RID: 148
internal class GunGuard : AIState<Gun>
{
	// Token: 0x060005B8 RID: 1464 RVA: 0x0002D408 File Offset: 0x0002B608
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		this.m_updateTargetTimer = PRand.Range(0f, 1f);
		this.m_idleTime = 0f;
		this.m_haveResetTower = false;
		owner.SetTarget(null);
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0002D444 File Offset: 0x0002B644
	public override string GetStatusText()
	{
		return "Looking for target";
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x0002D44C File Offset: 0x0002B64C
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		if (owner.GetFirstOrder() != null || owner.GetDeploy())
		{
			sm.PopState();
			return;
		}
		this.m_idleTime += dt;
		if (this.m_idleTime > this.m_resetTowerDelay && !this.m_haveResetTower && owner.ResetTower())
		{
			this.m_haveResetTower = true;
		}
		if (owner.GetOwner() <= 3 && owner.m_aim.m_noAutotarget)
		{
			return;
		}
		if (owner.GetUnit().IsCloaked() || owner.GetUnit().IsDoingMaintenance())
		{
			return;
		}
		this.m_updateTargetTimer -= dt;
		if (this.m_updateTargetTimer <= 0f)
		{
			this.m_updateTargetTimer = 1f;
			if (owner.GetStance() == Gun.Stance.FireAtWill)
			{
				GunTarget gunTarget = owner.FindTarget();
				if (gunTarget != null)
				{
					owner.SetTarget(gunTarget);
					sm.ChangeState("aim");
				}
			}
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0002D544 File Offset: 0x0002B744
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.m_updateTargetTimer);
		writer.Write(this.m_idleTime);
		writer.Write(this.m_haveResetTower);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0002D578 File Offset: 0x0002B778
	public override void Load(BinaryReader reader)
	{
		this.m_updateTargetTimer = reader.ReadSingle();
		this.m_idleTime = reader.ReadSingle();
		this.m_haveResetTower = reader.ReadBoolean();
	}

	// Token: 0x040004AC RID: 1196
	private readonly float m_resetTowerDelay = 5f;

	// Token: 0x040004AD RID: 1197
	private float m_updateTargetTimer;

	// Token: 0x040004AE RID: 1198
	private float m_idleTime;

	// Token: 0x040004AF RID: 1199
	private bool m_haveResetTower;
}
