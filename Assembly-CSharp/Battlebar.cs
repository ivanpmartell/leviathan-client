using System;
using System.Collections.Generic;
using PTech;
using UnityEngine;

// Token: 0x02000029 RID: 41
internal class Battlebar
{
	// Token: 0x06000179 RID: 377 RVA: 0x00009280 File Offset: 0x00007480
	public Battlebar(GameType gameType, GameObject guiRoot, GameObject guiCamera, TurnMan turnMan)
	{
		this.m_gameType = gameType;
		this.m_turnMan = turnMan;
		this.m_guiRoot = guiRoot;
		this.m_guiCamera = guiCamera;
		this.m_targetScore = GuiUtils.FindChildOfComponent<SpriteText>(guiRoot, "TargetScore");
		this.m_team1Score = GuiUtils.FindChildOfComponent<SpriteText>(guiRoot, "ScoreLabel_Team1");
		this.m_team2Score = GuiUtils.FindChildOfComponent<SpriteText>(guiRoot, "ScoreLabel_Team2");
		this.m_team1List = GuiUtils.FindChildOfComponent<UIScrollList>(guiRoot, "Team1_List");
		this.m_team2List = GuiUtils.FindChildOfComponent<UIScrollList>(guiRoot, "Team2_List");
		this.m_team1Flagship[0] = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(guiRoot, "Team1_Flagship1");
		this.m_team1Flagship[1] = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(guiRoot, "Team1_Flagship2");
		this.m_team2Flagship[0] = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(guiRoot, "Team2_Flagship1");
		this.m_team2Flagship[1] = GuiUtils.FindChildOfComponent<UIStateToggleBtn>(guiRoot, "Team2_Flagship2");
		GameObject gameObject = GuiUtils.FindChildOf(this.m_guiRoot, "Button_GlobalChat");
		if (gameObject != null)
		{
			this.m_globalChatGlow = GuiUtils.FindChildOfComponent<PackedSprite>(gameObject, "Button_Glow");
		}
		GameObject gameObject2 = GuiUtils.FindChildOf(this.m_guiRoot, "Button_TeamChat");
		if (gameObject2 != null)
		{
			this.m_teamChatGlow = GuiUtils.FindChildOfComponent<PackedSprite>(gameObject2, "Button_Glow");
		}
		if (this.m_team1Flagship[0])
		{
			this.m_team1Flagship[0].Hide(true);
		}
		if (this.m_team1Flagship[1])
		{
			this.m_team1Flagship[1].Hide(true);
		}
		if (this.m_team2Flagship[0])
		{
			this.m_team2Flagship[0].Hide(true);
		}
		if (this.m_team2Flagship[1])
		{
			this.m_team2Flagship[1].Hide(true);
		}
		this.m_players.Add(null);
		this.m_players.Add(null);
		this.m_players.Add(null);
		this.m_players.Add(null);
		this.SetGlobalChatGlow(false);
		this.SetTeamChatGlow(false);
		this.SetVisible(true);
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00009498 File Offset: 0x00007698
	public void Close()
	{
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000949C File Offset: 0x0000769C
	public void SetVisible(bool visible)
	{
		this.m_guiRoot.SetActiveRecursively(visible);
		this.m_visible = visible;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x000094B4 File Offset: 0x000076B4
	public void Update(List<ClientPlayer> players, int localPlayerID)
	{
		if (this.m_gameType == GameType.Points)
		{
			this.m_team1Score.Text = this.m_turnMan.GetTeamScore(0).ToString();
			this.m_team2Score.Text = this.m_turnMan.GetTeamScore(1).ToString();
		}
		foreach (ClientPlayer clientPlayer in players)
		{
			Battlebar.PlayerItem createPlayerItem = this.GetCreatePlayerItem(clientPlayer);
			createPlayerItem.SetStatus(clientPlayer.m_status, localPlayerID);
			createPlayerItem.SetPhase(clientPlayer.m_turnStatus);
			if (this.m_gameType == GameType.Assassination)
			{
				if (createPlayerItem.m_flagshipStatus[0] != null)
				{
					createPlayerItem.m_flagshipStatus[0].SetToggleState((this.m_turnMan.GetFlagshipKiller(clientPlayer.m_id, 0) < 0) ? 0 : 1);
				}
				if (createPlayerItem.m_flagshipStatus[1] != null)
				{
					createPlayerItem.m_flagshipStatus[1].SetToggleState((this.m_turnMan.GetFlagshipKiller(clientPlayer.m_id, 1) < 0) ? 0 : 1);
				}
			}
		}
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00009604 File Offset: 0x00007804
	public void SetTargetScore(int targetScore)
	{
		if (this.m_targetScore != null)
		{
			this.m_targetScore.Text = targetScore.ToString();
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x0000962C File Offset: 0x0000782C
	public void SetGlobalChatGlow(bool enabled)
	{
		if (this.m_globalChatGlow != null)
		{
			if (enabled)
			{
				this.m_globalChatGlow.PlayAnim(0);
			}
			else
			{
				this.m_globalChatGlow.StopAnim();
			}
		}
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00009664 File Offset: 0x00007864
	public void SetTeamChatGlow(bool enabled)
	{
		if (this.m_teamChatGlow != null)
		{
			if (enabled)
			{
				this.m_teamChatGlow.PlayAnim(0);
			}
			else
			{
				this.m_teamChatGlow.StopAnim();
			}
		}
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000969C File Offset: 0x0000789C
	private Battlebar.PlayerItem GetCreatePlayerItem(ClientPlayer player)
	{
		if (this.m_players[player.m_id] != null)
		{
			return this.m_players[player.m_id];
		}
		Battlebar.PlayerItem playerItem = new Battlebar.PlayerItem();
		playerItem.m_clientPlayer = player;
		if (this.m_gameType == GameType.Assassination || this.m_gameType == GameType.Points)
		{
			int playerTeam = this.m_turnMan.GetPlayerTeam(player.m_id);
			bool flag = this.m_gameType == GameType.Assassination && this.m_turnMan.GetNrOfPlayers() == 3 && this.m_turnMan.GetTeamSize(playerTeam) == 1;
			if (playerTeam == 0)
			{
				playerItem.m_gui = GuiUtils.CreateGui("IngameGui/Battlebar/Battlebar_Team1_Listitem", this.m_guiCamera);
				DebugUtils.Assert(playerItem.m_gui != null);
				if (this.m_gameType == GameType.Assassination)
				{
					playerItem.m_flagshipStatus[0] = this.m_team1Flagship[this.m_team1List.Count];
					playerItem.m_flagshipStatus[0].Hide(false);
					if (flag)
					{
						playerItem.m_flagshipStatus[1] = this.m_team1Flagship[this.m_team1List.Count + 1];
						playerItem.m_flagshipStatus[1].Hide(false);
					}
				}
				this.m_team1List.AddItem(playerItem.m_gui.GetComponent<UIListItemContainer>());
			}
			else
			{
				playerItem.m_gui = GuiUtils.CreateGui("IngameGui/Battlebar/Battlebar_Team2_Listitem", this.m_guiCamera);
				DebugUtils.Assert(playerItem.m_gui != null);
				if (this.m_gameType == GameType.Assassination)
				{
					playerItem.m_flagshipStatus[0] = this.m_team2Flagship[this.m_team2List.Count];
					playerItem.m_flagshipStatus[0].Hide(false);
					if (flag)
					{
						playerItem.m_flagshipStatus[1] = this.m_team2Flagship[this.m_team2List.Count + 1];
						playerItem.m_flagshipStatus[1].Hide(false);
					}
				}
				this.m_team2List.AddItem(playerItem.m_gui.GetComponent<UIListItemContainer>());
			}
		}
		else
		{
			playerItem.m_gui = GuiUtils.CreateGui("IngameGui/Battlebar/Battlebar_Player" + (player.m_id + 1).ToString() + "_Listitem", this.m_guiCamera);
			DebugUtils.Assert(playerItem.m_gui != null);
			this.m_team1List.AddItem(playerItem.m_gui.GetComponent<UIListItemContainer>());
		}
		playerItem.m_flag = GuiUtils.FindChildOfComponent<SimpleSprite>(playerItem.m_gui, "PlayerFlag");
		playerItem.m_name = GuiUtils.FindChildOfComponent<SpriteText>(playerItem.m_gui, "PlayerNameLabel");
		playerItem.m_phase = GuiUtils.FindChildOfComponent<SpriteText>(playerItem.m_gui, "PlayerPhaseLabel");
		playerItem.m_statusOnline = GuiUtils.FindChildOfComponent<SimpleSprite>(playerItem.m_gui, "PlayerStatus_Online");
		playerItem.m_statusOffline = GuiUtils.FindChildOfComponent<SimpleSprite>(playerItem.m_gui, "PlayerStatus_Offline");
		playerItem.m_statusPresent = GuiUtils.FindChildOfComponent<SimpleSprite>(playerItem.m_gui, "PlayerStatus_Present");
		this.m_players[player.m_id] = playerItem;
		playerItem.SetName(this.m_turnMan.GetPlayerName(player.m_id));
		playerItem.SetFlag(this.m_turnMan.GetPlayerFlag(player.m_id));
		playerItem.m_gui.SetActiveRecursively(this.m_visible);
		return playerItem;
	}

	// Token: 0x040000E7 RID: 231
	private TurnMan m_turnMan;

	// Token: 0x040000E8 RID: 232
	private GameType m_gameType;

	// Token: 0x040000E9 RID: 233
	private GameObject m_guiRoot;

	// Token: 0x040000EA RID: 234
	private GameObject m_guiCamera;

	// Token: 0x040000EB RID: 235
	private bool m_visible;

	// Token: 0x040000EC RID: 236
	private UIScrollList m_team1List;

	// Token: 0x040000ED RID: 237
	private UIScrollList m_team2List;

	// Token: 0x040000EE RID: 238
	private SpriteText m_targetScore;

	// Token: 0x040000EF RID: 239
	private SpriteText m_team1Score;

	// Token: 0x040000F0 RID: 240
	private SpriteText m_team2Score;

	// Token: 0x040000F1 RID: 241
	private UIStateToggleBtn[] m_team1Flagship = new UIStateToggleBtn[2];

	// Token: 0x040000F2 RID: 242
	private UIStateToggleBtn[] m_team2Flagship = new UIStateToggleBtn[2];

	// Token: 0x040000F3 RID: 243
	private List<Battlebar.PlayerItem> m_players = new List<Battlebar.PlayerItem>();

	// Token: 0x040000F4 RID: 244
	private PackedSprite m_globalChatGlow;

	// Token: 0x040000F5 RID: 245
	private PackedSprite m_teamChatGlow;

	// Token: 0x0200002A RID: 42
	private class PlayerItem
	{
		// Token: 0x06000182 RID: 386 RVA: 0x000099CC File Offset: 0x00007BCC
		public void SetName(string name)
		{
			this.m_name.Text = name;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000099DC File Offset: 0x00007BDC
		public void SetFlag(int flag)
		{
			Texture2D flagTexture = GuiUtils.GetFlagTexture(flag);
			GuiUtils.SetImage(this.m_flag, flagTexture);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x000099FC File Offset: 0x00007BFC
		public void SetStatus(PlayerPresenceStatus status, int localPlayerID)
		{
			this.m_statusOnline.Hide(status != PlayerPresenceStatus.Online || this.m_clientPlayer.m_id == localPlayerID);
			this.m_statusPresent.Hide(status != PlayerPresenceStatus.InGame || this.m_clientPlayer.m_id == localPlayerID);
			this.m_statusOffline.Hide(status != PlayerPresenceStatus.Offline || this.m_clientPlayer.m_id == localPlayerID);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00009A74 File Offset: 0x00007C74
		public void SetPhase(PlayerTurnStatus status)
		{
			if (ClientGame.instance.IsReplayMode())
			{
				this.m_phase.Text = string.Empty;
				return;
			}
			switch (status)
			{
			case PlayerTurnStatus.Planning:
				this.m_phase.Text = Localize.instance.TranslateKey("hud_battlebar_planning");
				break;
			case PlayerTurnStatus.Done:
				this.m_phase.Text = Localize.instance.TranslateKey("hud_battlebar_commited");
				break;
			case PlayerTurnStatus.Dead:
				this.m_phase.Text = Localize.instance.TranslateKey("hud_battlebar_dead");
				break;
			}
		}

		// Token: 0x040000F6 RID: 246
		public GameObject m_gui;

		// Token: 0x040000F7 RID: 247
		public ClientPlayer m_clientPlayer;

		// Token: 0x040000F8 RID: 248
		public SimpleSprite m_flag;

		// Token: 0x040000F9 RID: 249
		public SpriteText m_name;

		// Token: 0x040000FA RID: 250
		public SpriteText m_phase;

		// Token: 0x040000FB RID: 251
		public SimpleSprite m_statusOnline;

		// Token: 0x040000FC RID: 252
		public SimpleSprite m_statusOffline;

		// Token: 0x040000FD RID: 253
		public SimpleSprite m_statusPresent;

		// Token: 0x040000FE RID: 254
		public UIStateToggleBtn[] m_flagshipStatus = new UIStateToggleBtn[2];
	}
}
