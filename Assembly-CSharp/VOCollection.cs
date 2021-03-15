using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class VOCollection : MonoBehaviour
{
	// Token: 0x0400082D RID: 2093
	public List<VOCollection.VOEvent> m_voEvents = new List<VOCollection.VOEvent>();

	// Token: 0x0200010A RID: 266
	[Serializable]
	public class VOEvent
	{
		// Token: 0x0400082E RID: 2094
		public string m_name = string.Empty;

		// Token: 0x0400082F RID: 2095
		public GameObject m_effects;

		// Token: 0x04000830 RID: 2096
		public bool m_oncePerTurn;
	}
}
