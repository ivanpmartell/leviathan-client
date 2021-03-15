using System;
using System.IO;

// Token: 0x02000096 RID: 150
internal class GunReload : AIState<Gun>
{
	// Token: 0x060005C2 RID: 1474 RVA: 0x0002D5D4 File Offset: 0x0002B7D4
	public override void Enter(Gun owner, AIStateMachine<Gun> sm)
	{
		this.m_reloadTime = owner.GetReloadTime();
		this.m_timeLeft = this.m_reloadTime;
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0002D5F0 File Offset: 0x0002B7F0
	public override string GetStatusText()
	{
		return "Reloading " + this.m_timeLeft.ToString("F1");
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x0002D60C File Offset: 0x0002B80C
	public override void Update(Gun owner, AIStateMachine<Gun> sm, float dt)
	{
		this.m_timeLeft -= dt;
		if (owner.GetAmmo() == 0)
		{
			sm.PopState();
			return;
		}
		if (this.m_timeLeft <= 0f)
		{
			owner.LoadGun();
			sm.PopState();
		}
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x0002D64C File Offset: 0x0002B84C
	public override void GetCharageLevel(out float i, out float time)
	{
		i = 1f - this.m_timeLeft / this.m_reloadTime;
		time = this.m_timeLeft;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0002D66C File Offset: 0x0002B86C
	public override void Save(BinaryWriter writer)
	{
		writer.Write(this.m_timeLeft);
		writer.Write(this.m_reloadTime);
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0002D688 File Offset: 0x0002B888
	public override void Load(BinaryReader reader)
	{
		this.m_timeLeft = reader.ReadSingle();
		this.m_reloadTime = reader.ReadSingle();
	}

	// Token: 0x040004B0 RID: 1200
	private float m_timeLeft;

	// Token: 0x040004B1 RID: 1201
	private float m_reloadTime;
}
