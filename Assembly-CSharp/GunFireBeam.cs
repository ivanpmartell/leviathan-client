using System;
using System.IO;
using UnityEngine;

// Token: 0x02000092 RID: 146
internal class GunFireBeam : AIState<Gun>
{
	// Token: 0x060005AB RID: 1451 RVA: 0x0002D0F8 File Offset: 0x0002B2F8
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		PLog.Log("enter fire beam");
		this.m_timeLeft = owner.GetPreFireTime();
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0002D110 File Offset: 0x0002B310
	public override void Exit(Gun owner)
	{
		PLog.Log("exit firebeam");
		if (owner.IsFiring())
		{
			owner.StopFiring();
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0002D130 File Offset: 0x0002B330
	public void SetTimeLeft(float timeLeft)
	{
		this.m_timeLeft = timeLeft;
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0002D13C File Offset: 0x0002B33C
	public override string GetStatusText()
	{
		if (this.m_timeLeft > 0f)
		{
			return "Firing in " + this.m_timeLeft.ToString("F1");
		}
		return "Firing";
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0002D17C File Offset: 0x0002B37C
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		this.m_timeLeft -= dt;
		if (this.m_timeLeft > 0f)
		{
			return;
		}
		if (this.m_timeLeft < 0f)
		{
			this.m_timeLeft = 0f;
		}
		GunTarget target = owner.GetTarget();
		Vector3 vector;
		if (target != null && target.GetTargetWorldPos(out vector, owner.GetOwnerTeam()))
		{
			if (!owner.InFiringCone(vector))
			{
				PLog.Log("target is out of firing cone");
				sm.PopState();
				return;
			}
			if (owner.AimAt(vector))
			{
				this.m_updateOrderTimer -= dt;
				if (this.m_updateOrderTimer < 0f)
				{
					this.m_updateOrderTimer = 0.25f;
					this.UpdateTarget(owner);
				}
			}
			if (!owner.IsFiring())
			{
				owner.StartFiring();
			}
			else if (owner.GetLoadedSalvo() <= 0f)
			{
				sm.ChangeState("reload");
			}
		}
		else
		{
			sm.ChangeState("reload");
		}
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0002D280 File Offset: 0x0002B480
	private bool UpdateTarget(Gun owner)
	{
		Order firstOrder = owner.GetFirstOrder();
		if (firstOrder != null && firstOrder.m_type == Order.Type.Fire)
		{
			GunTarget target;
			if (firstOrder.IsStaticTarget())
			{
				target = new GunTarget(firstOrder.GetLocalTargetPos());
			}
			else
			{
				target = new GunTarget(firstOrder.GetTargetID(), firstOrder.GetLocalTargetPos());
			}
			if (owner.InFiringCone(target))
			{
				owner.SetTarget(target);
				owner.RemoveFirstOrder();
				return true;
			}
		}
		return false;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0002D2F4 File Offset: 0x0002B4F4
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.m_timeLeft);
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0002D304 File Offset: 0x0002B504
	public override void Load(BinaryReader reader)
	{
		this.m_timeLeft = reader.ReadSingle();
	}

	// Token: 0x040004AA RID: 1194
	public float m_timeLeft;

	// Token: 0x040004AB RID: 1195
	private float m_updateOrderTimer;
}
