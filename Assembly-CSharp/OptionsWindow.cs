using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
internal class OptionsWindow
{
	// Token: 0x0600040B RID: 1035 RVA: 0x000212D0 File Offset: 0x0001F4D0
	public OptionsWindow(GameObject guiCamera, bool inGame)
	{
		this.m_guiCamera = guiCamera;
		this.m_inGame = inGame;
		this.m_optionsGui = GuiUtils.CreateGui("OptionsWindow", this.m_guiCamera);
		this.m_oldMusicVolume = MusicManager.instance.GetVolume();
		this.m_oldSfxVolume = AudioManager.instance.GetVolume();
		this.m_bloom = (this.m_oldBloom = PostEffector.IsBloomEnabled());
		this.m_aa = (this.m_oldAA = PostEffector.IsFXAAEnabled());
		this.m_jazzyGraphic = (this.m_oldJazzyGraphic = (PlayerPrefs.GetInt("JazzyMode") != 0));
		this.m_customVSMusic = (this.m_oldCustomVSMusic = PlayerPrefs.GetString("CustomVSMusic"));
		this.m_vO = (this.m_oldVO = PlayerPrefs.GetString("VO"));
		GuiUtils.FindChildOf(this.m_optionsGui, "MusicVolumeSlider").GetComponent<UISlider>().defaultValue = this.m_oldMusicVolume;
		GuiUtils.FindChildOf(this.m_optionsGui, "SfxVolumeSlider").GetComponent<UISlider>().defaultValue = this.m_oldSfxVolume;
		this.RefreshProgressBars();
		GuiUtils.FindChildOf(this.m_optionsGui, "BloomCheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_oldBloom) ? 1 : 0);
		GuiUtils.FindChildOf(this.m_optionsGui, "AACheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_oldAA) ? 1 : 0);
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_oldJazzyGraphic) ? 1 : 0);
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().SetState((this.m_oldCustomVSMusic.Length == 0) ? 1 : 0);
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().SetState((this.m_oldVO.Length == 0) ? 1 : 0);
		if (inGame)
		{
			GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().controlIsEnabled = false;
			if (this.m_jazzyGraphic)
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().SetState(2);
			}
			else
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().SetState(3);
			}
			GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().controlIsEnabled = false;
			if (this.m_customVSMusic.Length != 0)
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().SetState(2);
			}
			else
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().SetState(3);
			}
			GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().controlIsEnabled = false;
			if (this.m_vO.Length != 0)
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().SetState(2);
			}
			else
			{
				GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().SetState(3);
			}
			GuiUtils.FindChildOf(this.m_optionsGui, "JazzButton").GetComponent<UIButton>().controlIsEnabled = false;
		}
		GuiUtils.FindChildOf(this.m_optionsGui, "CancelButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnCancelPressed));
		GuiUtils.FindChildOf(this.m_optionsGui, "ApplyButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnApplyPressed));
		GuiUtils.FindChildOf(this.m_optionsGui, "MusicVolumeSlider").GetComponent<UISlider>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnMusicVolume));
		GuiUtils.FindChildOf(this.m_optionsGui, "SfxVolumeSlider").GetComponent<UISlider>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnSFXVolume));
		GuiUtils.FindChildOf(this.m_optionsGui, "BloomCheckbox").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGraphicBloom));
		GuiUtils.FindChildOf(this.m_optionsGui, "AACheckbox").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnGraphicAA));
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnJazzGraphic));
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnJazzTrack));
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnJazzVo));
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzButton").GetComponent<UIButton>().SetValueChangedDelegate(new EZValueChangedDelegate(this.OnJazzButton));
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00021798 File Offset: 0x0001F998
	public void Close()
	{
		if (this.m_optionsGui != null)
		{
			UnityEngine.Object.Destroy(this.m_optionsGui);
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x000217B8 File Offset: 0x0001F9B8
	private void OnCancelPressed(IUIObject obj)
	{
		MusicManager.instance.SetVolume(this.m_oldMusicVolume);
		AudioManager.instance.SetVolume(this.m_oldSfxVolume);
		PostEffector.SetBloomEnabled(this.m_oldBloom);
		PostEffector.SetFXAAEnabled(this.m_oldAA);
		this.Close();
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00021804 File Offset: 0x0001FA04
	private void OnApplyPressed(IUIObject obj)
	{
		PlayerPrefs.SetFloat("MusicVolume", MusicManager.instance.GetVolume());
		PlayerPrefs.SetFloat("SfxVolume", AudioManager.instance.GetVolume());
		PlayerPrefs.SetInt("JazzyMode", (!this.m_jazzyGraphic) ? 0 : 1);
		PlayerPrefs.SetString("CustomVSMusic", this.m_customVSMusic);
		PlayerPrefs.SetString("VO", this.m_vO);
		PlayerPrefs.Save();
		PLog.Log("m_jazzyGraphic saved as: " + this.m_jazzyGraphic.ToString());
		this.Close();
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x0002189C File Offset: 0x0001FA9C
	private void RefreshProgressBars()
	{
		float value = GuiUtils.FindChildOf(this.m_optionsGui, "MusicVolumeSlider").GetComponent<UISlider>().Value;
		int num = (int)(value * 100f);
		GuiUtils.FindChildOf(this.m_optionsGui, "MusicValueLabel").GetComponent<SpriteText>().Text = num.ToString() + "%";
		GuiUtils.FindChildOf(this.m_optionsGui, "MusicVolumeProgressbar").GetComponent<UIProgressBar>().Value = value;
		float value2 = GuiUtils.FindChildOf(this.m_optionsGui, "SfxVolumeSlider").GetComponent<UISlider>().Value;
		int num2 = (int)(value2 * 100f);
		GuiUtils.FindChildOf(this.m_optionsGui, "SfxValueLabel").GetComponent<SpriteText>().Text = num2.ToString() + "%";
		GuiUtils.FindChildOf(this.m_optionsGui, "SfxVolumeProgressbar").GetComponent<UIProgressBar>().Value = value2;
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00021980 File Offset: 0x0001FB80
	private void OnMusicVolume(IUIObject obj)
	{
		float value = GuiUtils.FindChildOf(this.m_optionsGui, "MusicVolumeSlider").GetComponent<UISlider>().Value;
		MusicManager.instance.SetVolume(value);
		this.RefreshProgressBars();
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x000219BC File Offset: 0x0001FBBC
	private void OnSFXVolume(IUIObject obj)
	{
		float value = GuiUtils.FindChildOf(this.m_optionsGui, "SfxVolumeSlider").GetComponent<UISlider>().Value;
		AudioManager.instance.SetVolume(value);
		this.RefreshProgressBars();
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x000219F8 File Offset: 0x0001FBF8
	private void OnGraphicBloom(IUIObject obj)
	{
		this.m_bloom = !this.m_bloom;
		GuiUtils.FindChildOf(this.m_optionsGui, "BloomCheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_bloom) ? 1 : 0);
		PostEffector.SetBloomEnabled(this.m_bloom);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00021A4C File Offset: 0x0001FC4C
	private void OnGraphicAA(IUIObject obj)
	{
		this.m_aa = !this.m_aa;
		GuiUtils.FindChildOf(this.m_optionsGui, "AACheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_aa) ? 1 : 0);
		PostEffector.SetFXAAEnabled(this.m_aa);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00021AA0 File Offset: 0x0001FCA0
	private void OnJazzGraphic(IUIObject obj)
	{
		this.m_jazzyGraphic = !this.m_jazzyGraphic;
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzGraphicsCheckbox").GetComponent<UIStateToggleBtn>().SetState((!this.m_jazzyGraphic) ? 1 : 0);
		PLog.Log("m_jazzyGraphic changed to: " + this.m_jazzyGraphic.ToString());
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00021B04 File Offset: 0x0001FD04
	private void OnJazzTrack(IUIObject obj)
	{
		if (this.m_customVSMusic.Length == 0)
		{
			this.m_customVSMusic = "jazzy";
		}
		else
		{
			this.m_customVSMusic = string.Empty;
		}
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzTrackCheckbox").GetComponent<UIStateToggleBtn>().SetState((this.m_customVSMusic.Length == 0) ? 1 : 0);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00021B70 File Offset: 0x0001FD70
	private void OnJazzVo(IUIObject obj)
	{
		if (this.m_vO.Length == 0)
		{
			this.m_vO = "JazzyBoatman";
		}
		else
		{
			this.m_vO = string.Empty;
		}
		GuiUtils.FindChildOf(this.m_optionsGui, "JazzCommentaryCheckbox").GetComponent<UIStateToggleBtn>().SetState((this.m_vO.Length == 0) ? 1 : 0);
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00021BDC File Offset: 0x0001FDDC
	private void OnJazzButton(IUIObject obj)
	{
		this.m_jazzyGraphic = false;
		this.m_customVSMusic = string.Empty;
		this.m_vO = string.Empty;
		this.OnJazzGraphic(null);
		this.OnJazzTrack(null);
		this.OnJazzVo(null);
	}

	// Token: 0x04000367 RID: 871
	private bool m_inGame;

	// Token: 0x04000368 RID: 872
	private bool m_bloom;

	// Token: 0x04000369 RID: 873
	private bool m_aa;

	// Token: 0x0400036A RID: 874
	private bool m_jazzyGraphic;

	// Token: 0x0400036B RID: 875
	private string m_customVSMusic;

	// Token: 0x0400036C RID: 876
	private string m_vO;

	// Token: 0x0400036D RID: 877
	private GameObject m_guiCamera;

	// Token: 0x0400036E RID: 878
	private GameObject m_optionsGui;

	// Token: 0x0400036F RID: 879
	private float m_oldMusicVolume;

	// Token: 0x04000370 RID: 880
	private float m_oldSfxVolume;

	// Token: 0x04000371 RID: 881
	private bool m_oldBloom;

	// Token: 0x04000372 RID: 882
	private bool m_oldAA;

	// Token: 0x04000373 RID: 883
	private bool m_oldJazzyGraphic;

	// Token: 0x04000374 RID: 884
	private string m_oldCustomVSMusic;

	// Token: 0x04000375 RID: 885
	private string m_oldVO;
}
