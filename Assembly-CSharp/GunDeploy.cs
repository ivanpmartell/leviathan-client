using System;
using System.IO;
using UnityEngine;

// Token: 0x02000090 RID: 144
internal class GunDeploy : AIState<Gun>
{
	// Token: 0x0600059C RID: 1436 RVA: 0x0002CE88 File Offset: 0x0002B088
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		this.m_timeLeft = owner.GetPreFireTime();
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0002CE98 File Offset: 0x0002B098
	public override string GetStatusText()
	{
		return "Prepare deployment " + this.m_timeLeft.ToString("F1");
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0002CEB4 File Offset: 0x0002B0B4
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		Vector3 vector = owner.transform.position + owner.transform.forward * (owner.m_aim.m_minRange + owner.m_aim.m_maxRange) * 0.5f;
		if (owner.AimAt(vector))
		{
			this.m_timeLeft -= dt;
			if (this.m_timeLeft <= 0f)
			{
				GunTarget target = new GunTarget(vector);
				owner.SetTarget(target);
				sm.ChangeState("fire");
				owner.SetDeploy(false);
			}
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x0002CF50 File Offset: 0x0002B150
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.m_timeLeft);
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x0002CF60 File Offset: 0x0002B160
	public override void Load(BinaryReader reader)
	{
		this.m_timeLeft = reader.ReadSingle();
	}

	// Token: 0x040004A7 RID: 1191
	private float m_timeLeft;
}
