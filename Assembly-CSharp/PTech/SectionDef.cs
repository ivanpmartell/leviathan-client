using System;
using System.Collections.Generic;

namespace PTech
{
	// Token: 0x02000155 RID: 341
	public class SectionDef
	{
		// Token: 0x06000CCE RID: 3278 RVA: 0x0005BE0C File Offset: 0x0005A00C
		public void RemoveModule(int battery, Vector2i pos)
		{
			foreach (ModuleDef moduleDef in this.m_modules)
			{
				if (moduleDef.m_battery == battery && moduleDef.m_pos.x == pos.x && moduleDef.m_pos.y == pos.y)
				{
					this.m_modules.Remove(moduleDef);
					break;
				}
			}
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x0005BEB8 File Offset: 0x0005A0B8
		public List<string> GetHardpointNames()
		{
			List<string> list = new List<string>();
			foreach (ModuleDef moduleDef in this.m_modules)
			{
				list.Add(moduleDef.m_prefab);
			}
			return list;
		}

		// Token: 0x04000A98 RID: 2712
		public string m_prefab;

		// Token: 0x04000A99 RID: 2713
		public List<ModuleDef> m_modules = new List<ModuleDef>();
	}
}
