using System;
using System.Collections.Generic;

// Token: 0x02000037 RID: 55
public class EndGameData
{
	// Token: 0x040001A0 RID: 416
	public GameOutcome m_outcome;

	// Token: 0x040001A1 RID: 417
	public string m_outcomeText = string.Empty;

	// Token: 0x040001A2 RID: 418
	public int m_winnerTeam = -1;

	// Token: 0x040001A3 RID: 419
	public int m_localPlayerID;

	// Token: 0x040001A4 RID: 420
	public int m_autoJoinGameID;

	// Token: 0x040001A5 RID: 421
	public int m_turns;

	// Token: 0x040001A6 RID: 422
	public EndGame_PlayerStatistics m_localPlayer;

	// Token: 0x040001A7 RID: 423
	public List<EndGame_PlayerStatistics> m_players = new List<EndGame_PlayerStatistics>();

	// Token: 0x040001A8 RID: 424
	public string m_AccoladeDestroy = string.Empty;

	// Token: 0x040001A9 RID: 425
	public string m_AccoladeHarmless = string.Empty;

	// Token: 0x040001AA RID: 426
	public string m_AccoladeShields = string.Empty;

	// Token: 0x040001AB RID: 427
	public string m_AccoladeDistance = string.Empty;

	// Token: 0x040001AC RID: 428
	public string m_AccoladePlanning = string.Empty;
}
