using System;

namespace PTech
{
	// Token: 0x02000154 RID: 340
	public class ModuleDef
	{
		// Token: 0x06000CCC RID: 3276 RVA: 0x0005BDD0 File Offset: 0x00059FD0
		public ModuleDef(string prefab, int battery, Vector2i pos, Direction direction)
		{
			this.m_prefab = prefab;
			this.m_battery = battery;
			this.m_pos = pos;
			this.m_direction = direction;
		}

		// Token: 0x04000A94 RID: 2708
		public string m_prefab;

		// Token: 0x04000A95 RID: 2709
		public int m_battery;

		// Token: 0x04000A96 RID: 2710
		public Vector2i m_pos;

		// Token: 0x04000A97 RID: 2711
		public Direction m_direction;
	}
}
