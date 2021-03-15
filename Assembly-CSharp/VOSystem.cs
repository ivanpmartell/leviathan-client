using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018F RID: 399
public class VOSystem
{
	// Token: 0x06000EEA RID: 3818 RVA: 0x0006856C File Offset: 0x0006676C
	public VOSystem()
	{
		if (VOSystem.m_instance != null)
		{
			VOSystem.m_instance.Close();
		}
		VOSystem.m_instance = this;
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000EEC RID: 3820 RVA: 0x000685CC File Offset: 0x000667CC
	public static VOSystem instance
	{
		get
		{
			return VOSystem.m_instance;
		}
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x000685D4 File Offset: 0x000667D4
	public void Close()
	{
		VOSystem.m_instance = null;
		this.m_events.Clear();
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000685E8 File Offset: 0x000667E8
	public void SetAnnouncer(string name)
	{
		if (this.m_announcer == name)
		{
			return;
		}
		this.m_events.Clear();
		if (name == string.Empty)
		{
			return;
		}
		this.m_announcer = name;
		GameObject gameObject = Resources.Load("vo/" + name) as GameObject;
		VOCollection component = gameObject.GetComponent<VOCollection>();
		foreach (VOCollection.VOEvent voevent in component.m_voEvents)
		{
			VOSystem.VOEventData voeventData = new VOSystem.VOEventData();
			voeventData.m_event = voevent;
			this.m_events.Add(voevent.m_name, voeventData);
		}
		PLog.Log(string.Concat(new object[]
		{
			"Announcer set to ",
			name,
			"  events:",
			this.m_events.Count
		}));
	}

	// Token: 0x06000EEF RID: 3823 RVA: 0x000686F4 File Offset: 0x000668F4
	public void ResetTurnflags()
	{
		foreach (KeyValuePair<string, VOSystem.VOEventData> keyValuePair in this.m_events)
		{
			keyValuePair.Value.m_played = false;
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00068760 File Offset: 0x00066960
	public void DoEvent(string name)
	{
		if (this.m_timeSinceAnnouncement < this.m_minAnnouncementDelay)
		{
			return;
		}
		if (this.m_events.Count == 0)
		{
			return;
		}
		if (this.m_lastSound != null)
		{
			AudioSource component = this.m_lastSound.GetComponent<AudioSource>();
			if (component != null && component.isPlaying)
			{
				return;
			}
		}
		VOSystem.VOEventData voeventData;
		if (!this.m_events.TryGetValue(name, out voeventData))
		{
			PLog.LogWarning("Missing event " + name);
			return;
		}
		if (voeventData.m_event.m_oncePerTurn && voeventData.m_played)
		{
			return;
		}
		if (voeventData.m_event.m_effects == null)
		{
			return;
		}
		voeventData.m_played = true;
		PLog.Log("Doing event " + name);
		this.m_lastSound = (UnityEngine.Object.Instantiate(voeventData.m_event.m_effects, Camera.main.transform.position, Quaternion.identity) as GameObject);
		this.m_timeSinceAnnouncement = 0f;
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00068870 File Offset: 0x00066A70
	public void Update(float dt)
	{
		this.m_timeSinceAnnouncement += dt;
	}

	// Token: 0x04000BAB RID: 2987
	private static VOSystem m_instance;

	// Token: 0x04000BAC RID: 2988
	private string m_announcer = string.Empty;

	// Token: 0x04000BAD RID: 2989
	private float m_timeSinceAnnouncement = 1000f;

	// Token: 0x04000BAE RID: 2990
	private float m_minAnnouncementDelay = 2f;

	// Token: 0x04000BAF RID: 2991
	private GameObject m_lastSound;

	// Token: 0x04000BB0 RID: 2992
	private Dictionary<string, VOSystem.VOEventData> m_events = new Dictionary<string, VOSystem.VOEventData>();

	// Token: 0x02000190 RID: 400
	private class VOEventData
	{
		// Token: 0x04000BB1 RID: 2993
		public VOCollection.VOEvent m_event;

		// Token: 0x04000BB2 RID: 2994
		public bool m_played;
	}
}
