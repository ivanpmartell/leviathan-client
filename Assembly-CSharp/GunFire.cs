using System;
using System.IO;

// Token: 0x02000091 RID: 145
internal class GunFire : AIState<Gun>
{
	// Token: 0x060005A2 RID: 1442 RVA: 0x0002CF78 File Offset: 0x0002B178
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		this.m_timeLeft = owner.GetPreFireTime();
		this.m_prefireTime = this.m_timeLeft;
		owner.PreFireGun();
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x0002CF98 File Offset: 0x0002B198
	public override void Exit(Gun owner)
	{
		owner.StopPreFire();
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0002CFA0 File Offset: 0x0002B1A0
	public void SetTimeLeft(float timeLeft)
	{
		this.m_timeLeft = timeLeft;
		this.m_prefireTime = timeLeft;
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x0002CFB0 File Offset: 0x0002B1B0
	public override void GetCharageLevel(out float i, out float time)
	{
		i = 1f - this.m_timeLeft / this.m_prefireTime;
		time = this.m_timeLeft;
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0002CFD0 File Offset: 0x0002B1D0
	public override string GetStatusText()
	{
		if (this.m_timeLeft > 0f)
		{
			return "Firing in " + this.m_timeLeft.ToString("F1");
		}
		return "Firing";
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0002D010 File Offset: 0x0002B210
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		if (owner.GetLoadedSalvo() == 0f)
		{
			if (owner.GetAmmo() != 0)
			{
				sm.PushState("reload");
			}
			else
			{
				sm.PopState();
			}
			return;
		}
		this.m_timeLeft -= dt;
		if (this.m_timeLeft < 0f)
		{
			bool flag = owner.FireGun();
			if (flag && owner.GetLoadedSalvo() > 0f)
			{
				GunFire gunFire = sm.ChangeState("fire") as GunFire;
				gunFire.SetTimeLeft(owner.GetSalvoDelay());
			}
			else
			{
				sm.ChangeState("reload");
			}
		}
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0002D0B8 File Offset: 0x0002B2B8
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.m_timeLeft);
		writer.Write(this.m_prefireTime);
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0002D0D4 File Offset: 0x0002B2D4
	public override void Load(BinaryReader reader)
	{
		this.m_timeLeft = reader.ReadSingle();
		this.m_prefireTime = reader.ReadSingle();
	}

	// Token: 0x040004A8 RID: 1192
	public float m_timeLeft;

	// Token: 0x040004A9 RID: 1193
	public float m_prefireTime;
}
