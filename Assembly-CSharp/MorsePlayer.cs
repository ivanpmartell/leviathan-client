using System;
using UnityEngine;

// Token: 0x02000185 RID: 389
internal class MorsePlayer
{
	// Token: 0x06000E88 RID: 3720 RVA: 0x00065E3C File Offset: 0x0006403C
	public void SetText(string text)
	{
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x00065E4C File Offset: 0x0006404C
	private string GetMorseCode(char c)
	{
		if (c == 'A')
		{
			return "· –";
		}
		if (c == 'B')
		{
			return "– · · ·";
		}
		if (c == 'C')
		{
			return "– · – ·";
		}
		if (c == 'D')
		{
			return "– · ·";
		}
		if (c == 'E')
		{
			return "·";
		}
		if (c == 'F')
		{
			return "· · – ·";
		}
		if (c == 'G')
		{
			return "– – ·";
		}
		if (c == 'H')
		{
			return "· · · ·";
		}
		if (c == 'I')
		{
			return "· ·";
		}
		if (c == 'J')
		{
			return "· – – –";
		}
		if (c == 'K')
		{
			return "– · –";
		}
		if (c == 'L')
		{
			return "· – · ·";
		}
		if (c == 'M')
		{
			return "– –";
		}
		if (c == 'N')
		{
			return "– ·";
		}
		if (c == 'O')
		{
			return "– – –";
		}
		if (c == 'P')
		{
			return "· – – ·";
		}
		if (c == 'Q')
		{
			return "– – · –";
		}
		if (c == 'R')
		{
			return "· – ·";
		}
		if (c == 'S')
		{
			return "· · ·";
		}
		if (c == 'T')
		{
			return "–";
		}
		if (c == 'U')
		{
			return "· · –";
		}
		if (c == 'V')
		{
			return "· · · –";
		}
		if (c == 'W')
		{
			return "· – –";
		}
		if (c == 'X')
		{
			return "– · · –";
		}
		if (c == 'Y')
		{
			return "– · – –";
		}
		if (c == 'Z')
		{
			return "– – · ·";
		}
		if (c == '.')
		{
			return "· – · – · –";
		}
		return "··";
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x00065FD8 File Offset: 0x000641D8
	public void Update()
	{
		this.m_totalTime += Time.deltaTime;
		if (this.m_text.Length == 0)
		{
			return;
		}
		this.m_time -= Time.deltaTime;
		if (this.m_time >= 0f)
		{
			return;
		}
		this.m_time = 0.05f;
		if (this.m_characterMorse.Length == 0)
		{
			if (this.m_currentCharacter == this.m_text.Length)
			{
				this.m_text = string.Empty;
				return;
			}
			char c = this.m_text[this.m_currentCharacter];
			this.m_currentCharacter++;
			if (c == ' ')
			{
				this.m_time = this.m_mediumGap;
				return;
			}
			this.m_characterMorse = this.GetMorseCode(c);
			this.m_characterIndex = 0;
			PLog.Log("MorsePlayer.Update: " + this.m_characterMorse);
			this.m_time = this.m_shortGap;
			return;
		}
		else
		{
			char c2 = this.m_characterMorse[this.m_characterIndex];
			if (c2 == ' ')
			{
				this.m_time = this.m_interGap;
				this.m_characterIndex++;
				return;
			}
			if (c2 == '·')
			{
				PLog.Log("· " + this.m_totalTime.ToString());
				this.m_dit.GetComponent<AudioSource>().Play();
				this.m_time = this.m_shortMark;
			}
			if (c2 == '–')
			{
				PLog.Log("– " + this.m_totalTime.ToString());
				this.m_dah.GetComponent<AudioSource>().Play();
				this.m_time = this.m_longMark;
			}
			this.m_characterIndex++;
			if (this.m_characterIndex == this.m_characterMorse.Length)
			{
				this.m_characterMorse = string.Empty;
				return;
			}
			PLog.Log("Time: " + this.m_time.ToString());
			return;
		}
	}

	// Token: 0x04000B86 RID: 2950
	private const float m_unitLength = 0.05f;

	// Token: 0x04000B87 RID: 2951
	private string m_text = string.Empty;

	// Token: 0x04000B88 RID: 2952
	private int m_currentCharacter;

	// Token: 0x04000B89 RID: 2953
	private string m_characterMorse = string.Empty;

	// Token: 0x04000B8A RID: 2954
	private int m_characterIndex;

	// Token: 0x04000B8B RID: 2955
	private float m_time;

	// Token: 0x04000B8C RID: 2956
	private float m_totalTime;

	// Token: 0x04000B8D RID: 2957
	private float m_shortMark = 0.05f;

	// Token: 0x04000B8E RID: 2958
	private float m_longMark = 0.15f;

	// Token: 0x04000B8F RID: 2959
	private float m_interGap = 0.05f;

	// Token: 0x04000B90 RID: 2960
	private float m_shortGap = 0.15f;

	// Token: 0x04000B91 RID: 2961
	private float m_mediumGap = 0.35f;

	// Token: 0x04000B92 RID: 2962
	private GameObject m_dit;

	// Token: 0x04000B93 RID: 2963
	private GameObject m_dah;
}
