using System;
using PTech;
using UnityEngine;

// Token: 0x020000AC RID: 172
internal class CheatMan
{
	// Token: 0x06000637 RID: 1591 RVA: 0x0002EED8 File Offset: 0x0002D0D8
	public CheatMan()
	{
		CheatMan.m_instance = this;
		this.ResetCheats();
		if (Debug.isDebugBuild)
		{
			this.m_showFps = true;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000638 RID: 1592 RVA: 0x0002EF00 File Offset: 0x0002D100
	public static CheatMan instance
	{
		get
		{
			return CheatMan.m_instance;
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x0002EF08 File Offset: 0x0002D108
	public void Close()
	{
		CheatMan.m_instance = null;
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0002EF10 File Offset: 0x0002D110
	private bool IsCheatsAvaible()
	{
		return Debug.isDebugBuild;
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0002EF18 File Offset: 0x0002D118
	private void PrintHelp(Hud hud, string text)
	{
		if (hud == null)
		{
			return;
		}
		ChatClient.ChatMessage msg = new ChatClient.ChatMessage(DateTime.Now, "Cheat", text);
		hud.AddChatMessage(ChannelID.General, msg);
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0002EF48 File Offset: 0x0002D148
	public void ActivateCheat(string name, Hud hud)
	{
		if (!this.IsCheatsAvaible())
		{
			return;
		}
		if (name == "cheats")
		{
			string text = "win - wins the game next commit\n";
			text += "lose - lose the game next commit.\n";
			text += "god - Toggle player 0 no longer takes damage.\n";
			text += "instagib - Toggle huge damage on first player.\n";
			text += "fpslock - Lock render fps to min 20.\n";
			text += "showfps - Toggle fps display.\n";
			text += "nodmg - Toggle no damage (all ship resist damage)\n";
			text += "warfog - Toggle draw of fog of war\n";
			text += "brains - Debug AI";
			this.PrintHelp(hud, text);
		}
		if (name == "blame")
		{
			this.PrintHelp(hud, "Dumping memory usage");
			Utils.DumpMemoryUsage();
		}
		if (name == "win")
		{
			this.m_forceEndGame = GameOutcome.Victory;
			this.PrintHelp(hud, "Game will be won next turn");
		}
		if (name == "lose")
		{
			this.m_forceEndGame = GameOutcome.Defeat;
			this.PrintHelp(hud, "Game will be lost next turn");
		}
		if (name == "god")
		{
			this.m_playerIsImmortal = !this.m_playerIsImmortal;
			this.PrintHelp(hud, "Player is immortal: " + this.m_playerIsImmortal.ToString());
		}
		if (name == "instagib")
		{
			this.m_playerHasInstagibGun = !this.m_playerHasInstagibGun;
			this.PrintHelp(hud, "Player has Instagib Gun: " + this.m_playerHasInstagibGun.ToString());
		}
		if (name == "nodmg")
		{
			this.m_noDamage = !this.m_noDamage;
			this.PrintHelp(hud, "All ships damage immune: " + this.m_noDamage.ToString());
		}
		if (name == "warfog")
		{
			this.m_noFogOfWar = !this.m_noFogOfWar;
			this.PrintHelp(hud, "Fog of War Drawer: " + this.m_noFogOfWar.ToString());
		}
		if (name == "brains")
		{
			this.m_debugAi = !this.m_debugAi;
			this.PrintHelp(hud, "Debug AI: " + this.m_debugAi.ToString());
		}
		if (name == "fpslock")
		{
			float num = 0.05f;
			float maximumDeltaTime = 0.2f;
			if (Time.maximumDeltaTime <= num + 0.01f)
			{
				Time.maximumDeltaTime = maximumDeltaTime;
				this.PrintHelp(hud, "unlocking fps");
			}
			else
			{
				Time.maximumDeltaTime = num;
				this.PrintHelp(hud, "locking fps");
			}
		}
		if (name == "showfps")
		{
			this.m_showFps = !this.m_showFps;
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0002F1E4 File Offset: 0x0002D3E4
	public void ResetCheats()
	{
		this.m_forceEndGame = GameOutcome.None;
		this.m_playerIsImmortal = false;
		this.m_playerHasInstagibGun = false;
		this.m_noDamage = false;
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x0002F204 File Offset: 0x0002D404
	public GameOutcome GetEndGameStatus()
	{
		return this.m_forceEndGame;
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x0002F20C File Offset: 0x0002D40C
	public bool GetPlayerImmortal()
	{
		return this.m_playerIsImmortal;
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0002F214 File Offset: 0x0002D414
	public bool GetNoDamage()
	{
		return this.m_noDamage;
	}

	// Token: 0x06000641 RID: 1601 RVA: 0x0002F21C File Offset: 0x0002D41C
	public bool GetNoFogOfWar()
	{
		return this.m_noFogOfWar;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x0002F224 File Offset: 0x0002D424
	public bool GetInstaGib()
	{
		return this.m_playerHasInstagibGun;
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x0002F22C File Offset: 0x0002D42C
	public bool DebugAi()
	{
		return this.m_debugAi;
	}

	// Token: 0x040004C6 RID: 1222
	public GameOutcome m_forceEndGame;

	// Token: 0x040004C7 RID: 1223
	public bool m_playerIsImmortal;

	// Token: 0x040004C8 RID: 1224
	public bool m_playerHasInstagibGun;

	// Token: 0x040004C9 RID: 1225
	public bool m_showFps;

	// Token: 0x040004CA RID: 1226
	public bool m_noDamage;

	// Token: 0x040004CB RID: 1227
	public bool m_noFogOfWar;

	// Token: 0x040004CC RID: 1228
	public bool m_debugAi;

	// Token: 0x040004CD RID: 1229
	private static CheatMan m_instance;
}
