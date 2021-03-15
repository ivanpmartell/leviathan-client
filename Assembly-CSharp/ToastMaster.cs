using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x0200006A RID: 106
internal class ToastMaster
{
	// Token: 0x060004AC RID: 1196 RVA: 0x000286D0 File Offset: 0x000268D0
	public ToastMaster(PTech.RPC rpc, GameObject guiCamera, UserManClient userManClient)
	{
		this.m_rpc = rpc;
		this.m_guiCamera = guiCamera;
		this.m_userMan = userManClient;
		this.m_rpc.Register("Toast", new PTech.RPC.Handler(this.RPC_Toast));
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x00028720 File Offset: 0x00026920
	public void Close()
	{
		this.m_rpc.Unregister("Toast");
		if (this.m_toastGui != null)
		{
			UnityEngine.Object.Destroy(this.m_toastGui);
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0002875C File Offset: 0x0002695C
	private void RPC_Toast(PTech.RPC rpc, List<object> args)
	{
		PLog.Log("got toast ");
		byte[] data = (byte[])args[0];
		UserEvent item = new UserEvent(data);
		this.m_toastQueue.Enqueue(item);
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x00028794 File Offset: 0x00026994
	private void CreateToast(UserEvent ev)
	{
		this.m_time = 0f;
		string text = string.Empty;
		switch (ev.GetEventType())
		{
		case UserEvent.EventType.NewTurn:
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastNewTurn", this.m_guiCamera);
			text = "$toast_new_turn " + ev.GetGameName();
			break;
		case UserEvent.EventType.Achievement:
		{
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastAchievement", this.m_guiCamera);
			text = "$toast_achievement_unlocked $achievement_name" + ev.GetAchievementID().ToString();
			SimpleSprite sprite = GuiUtils.FindChildOfComponent<SimpleSprite>(this.m_toastGui, "ToastIcon");
			Texture2D achievementIconTexture = GuiUtils.GetAchievementIconTexture(ev.GetAchievementID(), true);
			if (achievementIconTexture != null)
			{
				GuiUtils.SetImage(sprite, achievementIconTexture);
			}
			break;
		}
		case UserEvent.EventType.FriendRequest:
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastFriendRequest", this.m_guiCamera);
			text = "$toast_friend_request " + ev.GetFriendName();
			break;
		case UserEvent.EventType.GameInvite:
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastInvite", this.m_guiCamera);
			text = "$toast_invite_pre " + ev.GetFriendName() + " $toast_invite_post " + ev.GetGameName();
			break;
		case UserEvent.EventType.FriendRequestAccepted:
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastFriendAccepted", this.m_guiCamera);
			text = ev.GetFriendName() + " $toast_accepted_friend";
			this.m_userMan.UnlockAchievement(3);
			break;
		case UserEvent.EventType.ServerMessage:
			this.m_toastGui = GuiUtils.CreateGui("Toast/ToastMessage", this.m_guiCamera);
			text = ev.GetGameName();
			break;
		}
		this.m_toastGui.GetComponent<UIPanel>().BringIn();
		SpriteText component = GuiUtils.FindChildOf(this.m_toastGui, "ToastMessage").GetComponent<SpriteText>();
		component.Text = Localize.instance.Translate(text);
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0002896C File Offset: 0x00026B6C
	public void Update(float dt)
	{
		if (this.m_toastGui == null)
		{
			if (this.m_toastQueue.Count > 0)
			{
				UserEvent ev = this.m_toastQueue.Dequeue();
				this.CreateToast(ev);
			}
		}
		else
		{
			UIPanel component = this.m_toastGui.GetComponent<UIPanel>();
			this.m_time += dt;
			if (!this.m_dismissed)
			{
				if (this.m_time > 8f)
				{
					component.Dismiss();
					this.m_dismissed = true;
				}
			}
			else if (this.m_time > 10f)
			{
				UnityEngine.Object.Destroy(this.m_toastGui);
				this.m_toastGui = null;
			}
		}
	}

	// Token: 0x04000405 RID: 1029
	private const float m_stayTime = 8f;

	// Token: 0x04000406 RID: 1030
	private const float m_fadeTime = 2f;

	// Token: 0x04000407 RID: 1031
	private GameObject m_toastGui;

	// Token: 0x04000408 RID: 1032
	private float m_time;

	// Token: 0x04000409 RID: 1033
	private bool m_dismissed;

	// Token: 0x0400040A RID: 1034
	private Queue<UserEvent> m_toastQueue = new Queue<UserEvent>();

	// Token: 0x0400040B RID: 1035
	private GameObject m_guiCamera;

	// Token: 0x0400040C RID: 1036
	private PTech.RPC m_rpc;

	// Token: 0x0400040D RID: 1037
	private UserManClient m_userMan;
}
