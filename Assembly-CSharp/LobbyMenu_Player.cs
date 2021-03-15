using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
[AddComponentMenu("Scripts/Gui/LobbyMenu_Player")]
public class LobbyMenu_Player : MonoBehaviour
{
	// Token: 0x060000B1 RID: 177 RVA: 0x000050BC File Offset: 0x000032BC
	public LobbyMenu_Player()
	{
		if (LobbyMenu_Player.m_staticOnlineMats == null)
		{
			LobbyMenu_Player.m_staticOnlineMats = new Material[]
			{
				Resources.Load("OnlineStatus/status_offline") as Material,
				Resources.Load("OnlineStatus/status_online") as Material,
				Resources.Load("OnlineStatus/status_present") as Material
			};
		}
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x0000511C File Offset: 0x0000331C
	public void Awake()
	{
		this.lblReady = base.gameObject.transform.FindChild("dialog_bg/lblReady").GetComponent<SpriteText>();
		this.lblTeam = base.gameObject.transform.FindChild("dialog_bg/lblTeam").GetComponent<SpriteText>();
		this.lblFleetValue = GuiUtils.FindChildOf(base.gameObject, "fleetValue").GetComponent<SpriteText>();
		this.lblFleetValueText = GuiUtils.FindChildOf(base.gameObject, "fleetValueText").GetComponent<SpriteText>();
		this.ValidateComponents();
		this.RegisterDelegatesToComponents();
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x000051AC File Offset: 0x000033AC
	public void Setup(LobbyPlayer player, LobbyPlayer watcher, bool visible, bool showTeam, bool noFleet, FleetSize maxFleetSize)
	{
		this.m_player = player;
		if (!visible)
		{
			base.gameObject.SetActiveRecursively(false);
			return;
		}
		bool admin = watcher.m_admin;
		bool watcherIsThis = this.m_player != null && watcher.m_name == this.m_player.m_name;
		bool thisIsAdmin = this.m_player != null && this.m_player.m_admin;
		if (player != null)
		{
			this.SetPlayer(noFleet, showTeam, maxFleetSize);
			this.SetViewMode(watcher, admin, watcherIsThis, thisIsAdmin, noFleet);
		}
		else
		{
			this.lblPlayerName.gameObject.SetActiveRecursively(false);
			this.lblFleetName.gameObject.SetActiveRecursively(false);
			this.lblFleetValueText.gameObject.SetActiveRecursively(false);
			this.lblFleetValue.gameObject.SetActiveRecursively(false);
			this.onlineStatusIcon.gameObject.SetActiveRecursively(false);
			this.lblReady.gameObject.SetActiveRecursively(false);
			this.lblTeam.gameObject.SetActiveRecursively(false);
			this.playerFlag.gameObject.SetActiveRecursively(false);
			this.lblFleetValueText.gameObject.SetActiveRecursively(false);
			this.btnInvitePlayer.gameObject.SetActiveRecursively(false);
			this.btnRemove.gameObject.SetActiveRecursively(false);
			if (admin)
			{
				this.btnInvitePlayer.gameObject.SetActiveRecursively(true);
			}
		}
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005314 File Offset: 0x00003514
	private void SetPlayer(bool noFleet, bool showTeam, FleetSize maxFleetSize)
	{
		DebugUtils.Assert(this.m_player != null);
		if (showTeam)
		{
			this.lblTeam.gameObject.SetActiveRecursively(true);
			this.lblTeam.Text = (this.m_player.m_team + 1).ToString();
		}
		else
		{
			this.lblTeam.gameObject.SetActiveRecursively(false);
		}
		this.lblPlayerName.Text = this.m_player.m_name;
		this.btnRemove.gameObject.SetActiveRecursively(true);
		if (noFleet)
		{
			this.lblFleetName.gameObject.SetActiveRecursively(false);
			this.lblFleetValue.gameObject.SetActiveRecursively(false);
			this.lblFleetValueText.gameObject.SetActiveRecursively(true);
		}
		else
		{
			this.lblFleetName.gameObject.SetActiveRecursively(true);
			if (string.IsNullOrEmpty(this.m_player.m_fleet))
			{
				this.lblFleetName.Text = Localize.instance.Translate("$gamelobby_nofleet");
				this.lblFleetValue.gameObject.SetActiveRecursively(false);
				this.lblFleetValueText.gameObject.SetActiveRecursively(true);
			}
			else
			{
				this.lblFleetName.Text = this.m_player.m_fleet;
				this.lblFleetValue.Text = this.m_player.m_fleetValue.ToString();
				this.lblFleetValue.gameObject.SetActiveRecursively(true);
				this.lblFleetValueText.gameObject.SetActiveRecursively(true);
				if (maxFleetSize != null && !maxFleetSize.ValidSize(this.m_player.m_fleetValue))
				{
					this.lblFleetValue.SetColor(Color.red);
					this.lblFleetValueText.SetColor(Color.red);
				}
				else
				{
					this.lblFleetValue.SetColor(Color.white);
					this.lblFleetValueText.SetColor(Color.white);
				}
			}
		}
		this.playerFlag.gameObject.SetActiveRecursively(true);
		Texture2D flagTexture = GuiUtils.GetFlagTexture(this.m_player.m_flag);
		this.SetPlayerFlag(flagTexture);
		this.SetOnlineStatus(this.m_player.m_status);
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x0000553C File Offset: 0x0000373C
	private void SetPlayerFlag(Texture2D value)
	{
		DebugUtils.Assert(this.playerFlag != null);
		if (this.playerFlag.renderer.material.mainTexture == value)
		{
			return;
		}
		this.playerFlag.SetTexture(value);
		float x = (float)value.width;
		float y = (float)value.height;
		this.playerFlag.Setup(this.playerFlag.width, this.playerFlag.height, new Vector2(0f, y), new Vector2(x, y));
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000055CC File Offset: 0x000037CC
	private void SetViewMode(LobbyPlayer watcher, bool watcherIsAdmin, bool watcherIsThis, bool thisIsAdmin, bool noFleet)
	{
		if (this.m_player != null)
		{
			this.lblReady.gameObject.SetActiveRecursively(this.m_player.m_readyToStart);
			this.playerFlag.gameObject.SetActiveRecursively(true);
			this.lblFleetValueText.gameObject.SetActiveRecursively(true);
			this.lblPlayerName.gameObject.SetActiveRecursively(true);
			this.btnInvitePlayer.gameObject.SetActiveRecursively(false);
			if (noFleet)
			{
				this.lblFleetValueText.gameObject.SetActiveRecursively(false);
				this.lblFleetName.gameObject.SetActiveRecursively(false);
				this.lblFleetValue.gameObject.SetActiveRecursively(false);
			}
			else
			{
				this.lblFleetName.gameObject.SetActiveRecursively(true);
				if (string.IsNullOrEmpty(this.m_player.m_fleet))
				{
					this.lblFleetName.Text = Localize.instance.Translate("$gamelobby_nofleet");
					this.lblFleetValueText.gameObject.SetActiveRecursively(false);
					this.lblFleetValue.gameObject.SetActiveRecursively(false);
				}
				else
				{
					this.lblFleetName.Text = this.m_player.m_fleet;
					this.lblFleetValueText.gameObject.SetActiveRecursively(true);
					this.lblFleetValue.gameObject.SetActiveRecursively(true);
				}
			}
			if (watcherIsThis)
			{
				this.onlineStatusIcon.gameObject.SetActiveRecursively(false);
				this.btnRemove.gameObject.SetActiveRecursively(false);
			}
			else
			{
				this.onlineStatusIcon.gameObject.SetActiveRecursively(true);
				this.btnRemove.gameObject.SetActiveRecursively(watcherIsAdmin);
			}
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005770 File Offset: 0x00003970
	private void SetOnlineStatus(PlayerPresenceStatus ps)
	{
		if (this.onlineStatusIcon == null)
		{
			return;
		}
		MeshRenderer component = this.onlineStatusIcon.gameObject.GetComponent<MeshRenderer>();
		if (component == null)
		{
			return;
		}
		if (ps < PlayerPresenceStatus.Offline || ps > (PlayerPresenceStatus)LobbyMenu_Player.m_staticOnlineMats.Length - 1)
		{
			return;
		}
		component.material = LobbyMenu_Player.m_staticOnlineMats[(int)ps];
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000057D4 File Offset: 0x000039D4
	private void RegisterDelegatesToComponents()
	{
		if (this.btnRemove != null)
		{
			this.btnRemove.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnRemoveClicked));
		}
		if (this.btnInvitePlayer != null)
		{
			this.btnInvitePlayer.SetValueChangedDelegate(new EZValueChangedDelegate(this.OnInviteClicked));
		}
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00005834 File Offset: 0x00003A34
	private void OnRemoveClicked(IUIObject obj)
	{
		PLog.Log("OnRemoveClicked()");
		if (this.m_playerRemovedDelegate != null)
		{
			this.m_playerRemovedDelegate(this.m_player);
		}
	}

	// Token: 0x060000BA RID: 186 RVA: 0x00005868 File Offset: 0x00003A68
	private void OnInviteClicked(IUIObject obj)
	{
		PLog.Log("OnInviteClicked()");
		if (this.m_onOpenInvite != null)
		{
			this.m_onOpenInvite(this.m_player);
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x0000589C File Offset: 0x00003A9C
	private void ValidateComponents()
	{
		this.Validate_InvitePlayerButton();
		this.Validate_FleetNameLabel();
		this.Validate_RemoveButton();
		this.Validate_PlayerNameLabel();
		this.Validate_StatusIcon();
		this.Validate_PlayerFlag();
	}

	// Token: 0x060000BC RID: 188 RVA: 0x000058D4 File Offset: 0x00003AD4
	private bool Validate_InvitePlayerButton()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/btnInvitePlayer", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnInvitePlayer = gameObject.GetComponent<UIButton>();
		return this.btnInvitePlayer != null;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000591C File Offset: 0x00003B1C
	private bool Validate_FleetNameLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/lblFleetName", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.lblFleetName = gameObject.GetComponent<SpriteText>();
		return this.lblFleetName != null;
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005964 File Offset: 0x00003B64
	private bool Validate_RemoveButton()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/btnRemove", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.btnRemove = gameObject.GetComponent<UIButton>();
		return this.btnRemove != null;
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000059AC File Offset: 0x00003BAC
	private bool Validate_PlayerNameLabel()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/lblPlayerName", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.lblPlayerName = gameObject.GetComponent<SpriteText>();
		return this.lblPlayerName != null;
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000059F4 File Offset: 0x00003BF4
	private bool Validate_StatusIcon()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/onlineStatusIcon", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.onlineStatusIcon = gameObject.GetComponent<SimpleSprite>();
		return this.onlineStatusIcon != null;
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00005A3C File Offset: 0x00003C3C
	private bool Validate_PlayerFlag()
	{
		GameObject gameObject;
		if (!this.ValidateTransform("dialog_bg/playerFlag", out gameObject))
		{
			return false;
		}
		if (gameObject == null)
		{
			return false;
		}
		this.playerFlag = gameObject.GetComponent<SimpleSprite>();
		return this.playerFlag != null;
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00005A84 File Offset: 0x00003C84
	private bool ValidateTransform(string name, out GameObject go)
	{
		go = null;
		Transform transform = base.gameObject.transform.FindChild(name);
		if (transform == null)
		{
			return false;
		}
		go = transform.gameObject;
		return true;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00005AC0 File Offset: 0x00003CC0
	public LobbyPlayer GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x0400005A RID: 90
	private const string ChooseFleetStr_lbl = "$gamelobby_nofleet";

	// Token: 0x0400005B RID: 91
	private const string FreeSlotStr = "$gamelobby_freeslot";

	// Token: 0x0400005C RID: 92
	public PlayerRemoved m_playerRemovedDelegate;

	// Token: 0x0400005D RID: 93
	public OpenInvite m_onOpenInvite;

	// Token: 0x0400005E RID: 94
	private UIButton btnInvitePlayer;

	// Token: 0x0400005F RID: 95
	private UIButton btnRemove;

	// Token: 0x04000060 RID: 96
	private SpriteText lblPlayerName;

	// Token: 0x04000061 RID: 97
	private SpriteText lblFleetName;

	// Token: 0x04000062 RID: 98
	private SpriteText lblFleetValue;

	// Token: 0x04000063 RID: 99
	private SpriteText lblFleetValueText;

	// Token: 0x04000064 RID: 100
	private SpriteText lblReady;

	// Token: 0x04000065 RID: 101
	private SpriteText lblTeam;

	// Token: 0x04000066 RID: 102
	private SimpleSprite onlineStatusIcon;

	// Token: 0x04000067 RID: 103
	private SimpleSprite playerFlag;

	// Token: 0x04000068 RID: 104
	private LobbyPlayer m_player;

	// Token: 0x04000069 RID: 105
	private static Material[] m_staticOnlineMats;
}
