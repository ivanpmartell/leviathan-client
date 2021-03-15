using System;
using System.Collections.Generic;
using System.Xml;
using PTech;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class Dialog
{
	// Token: 0x06000223 RID: 547 RVA: 0x0000E4E4 File Offset: 0x0000C6E4
	public Dialog(PTech.RPC rpc, GameObject guiCamera, Hud hud, TurnMan turnMan)
	{
		this.Clear();
		if (guiCamera == null)
		{
			return;
		}
		this.m_guiCamera = guiCamera;
		this.m_turnMan = turnMan;
		this.Hide();
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000E568 File Offset: 0x0000C768
	private void OnLaunch(IUIObject obj)
	{
		this.EndDialog();
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000E570 File Offset: 0x0000C770
	private void OnNextAdvise(IUIObject obj)
	{
		if (this.m_advice.Count == 0)
		{
			return;
		}
		this.m_currentAdvice++;
		if (this.m_currentAdvice >= this.m_advice.Count)
		{
			this.m_currentAdvice = 0;
		}
		SimpleSprite component = GuiUtils.FindChildOf(this.m_gui.transform, "portrait").GetComponent<SimpleSprite>();
		this.SetPortrait(component, this.m_advice[this.m_currentAdvice].m_portrait);
		GuiUtils.FindChildOf(this.m_gui.transform, "text").GetComponent<SpriteText>().Text = Localize.instance.Translate(this.m_advice[this.m_currentAdvice].m_text);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000E630 File Offset: 0x0000C830
	private void OnSkip(IUIObject obj)
	{
		if (!this.PlayNextScene())
		{
			this.EndDialog();
		}
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000E644 File Offset: 0x0000C844
	private void OnNext(IUIObject obj)
	{
		if (this.m_waitInput)
		{
			this.m_waitInput = false;
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000E658 File Offset: 0x0000C858
	public void ForceEndDialog()
	{
		this.m_onEndDialog = null;
		this.EndDialog();
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000E668 File Offset: 0x0000C868
	private void EndDialog()
	{
		Dialog.m_dialogActive = false;
		this.m_index = 0;
		this.m_commands.Clear();
		GameCamera component = GameObject.Find("GameCamera").GetComponent<GameCamera>();
		component.SetMode(this.m_lastMode);
		this.m_advice = new List<DialogAdvice>();
		this.m_currentAdvice = -1;
		this.Close();
		if (this.m_onEndDialog != null)
		{
			this.m_onEndDialog();
		}
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000E6D8 File Offset: 0x0000C8D8
	public void Close()
	{
		Dialog.m_dialogActive = false;
		UnityEngine.Object.Destroy(this.m_gui);
		this.m_gui = null;
		this.m_advice = new List<DialogAdvice>();
		this.m_currentAdvice = -1;
		this.m_wallpaper = null;
		this.m_image = null;
		this.m_box = null;
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000E724 File Offset: 0x0000C924
	public void Clear()
	{
		this.m_currentName = string.Empty;
		this.m_currentText = string.Empty;
		this.m_currentPortrait = string.Empty;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000E748 File Offset: 0x0000C948
	public void Hide()
	{
		this.m_commands.Clear();
		if (this.m_gui)
		{
			this.m_gui.SetActiveRecursively(false);
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000E774 File Offset: 0x0000C974
	public void Show()
	{
		if (this.m_gui)
		{
			this.m_gui.SetActiveRecursively(true);
		}
		if (this.m_wallpaper)
		{
			this.m_wallpaper.gameObject.SetActiveRecursively(false);
		}
		if (this.m_image)
		{
			this.m_image.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000E7E0 File Offset: 0x0000C9E0
	private void CreateGui(string gui)
	{
		this.m_currentName = string.Empty;
		this.m_currentText = string.Empty;
		this.m_currentPortrait = string.Empty;
		if (gui == "dlg")
		{
			this.m_gui = GuiUtils.CreateGui("Dialog", this.m_guiCamera);
			this.m_text = GuiUtils.FindChildOf(this.m_gui.transform, "text").GetComponent<SpriteText>();
			this.m_name = GuiUtils.FindChildOf(this.m_gui.transform, "name").GetComponent<SpriteText>();
			this.m_text.Text = string.Empty;
			this.m_name.Text = string.Empty;
			this.m_portrait = GuiUtils.FindChildOf(this.m_gui.transform, "portrait").GetComponent<SimpleSprite>();
			this.m_portraitOverlay = GuiUtils.FindChildOf(this.m_gui.transform, "portrait_overlay").GetComponent<PackedSprite>();
			this.m_wallpaper = GuiUtils.FindChildOf(this.m_gui.transform, "wallpaper").GetComponent<SimpleSprite>();
			this.m_image = GuiUtils.FindChildOf(this.m_gui.transform, "image").GetComponent<SimpleSprite>();
			this.m_box = GuiUtils.FindChildOf(this.m_gui.transform, "box").GetComponent<SimpleSprite>();
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSkip));
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNext));
		}
		if (gui == "brf")
		{
			this.m_gui = GuiUtils.CreateGui("Dialog_Briefing", this.m_guiCamera);
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnLaunch));
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNextAdvise));
		}
		if (gui == "debrf")
		{
			this.m_gui = GuiUtils.CreateGui("Dialog_Debriefing", this.m_guiCamera);
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnLaunch));
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNextAdvise));
		}
		if (gui == "tut")
		{
			this.m_gui = GuiUtils.CreateGui("Dialog_Tutorial", this.m_guiCamera);
			this.m_text = GuiUtils.FindChildOf(this.m_gui.transform, "text").GetComponent<SpriteText>();
			this.m_name = GuiUtils.FindChildOf(this.m_gui.transform, "name").GetComponent<SpriteText>();
			this.m_text.Text = string.Empty;
			this.m_name.Text = string.Empty;
			this.m_portrait = GuiUtils.FindChildOf(this.m_gui.transform, "portrait").GetComponent<SimpleSprite>();
			this.m_portraitOverlay = GuiUtils.FindChildOf(this.m_gui.transform, "portrait_overlay").GetComponent<PackedSprite>();
			this.m_wallpaper = GuiUtils.FindChildOf(this.m_gui.transform, "wallpaper").GetComponent<SimpleSprite>();
			this.m_image = GuiUtils.FindChildOf(this.m_gui.transform, "image").GetComponent<SimpleSprite>();
			this.m_box = GuiUtils.FindChildOf(this.m_gui.transform, "box").GetComponent<SimpleSprite>();
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnSkip));
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").GetComponent<UIButton>().AddValueChangedDelegate(new EZValueChangedDelegate(this.OnNext));
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000EC00 File Offset: 0x0000CE00
	public void LoadScene(string scenename)
	{
		this.m_showSkipButton = true;
		PLog.Log("LoadScene: " + scenename);
		this.m_commands.Clear();
		this.m_index = 0;
		XmlDocument xmlDocument = Utils.LoadXml(scenename);
		if (xmlDocument == null)
		{
			PLog.LogError("Missing dialog " + scenename);
			return;
		}
		XmlNode firstChild = xmlDocument.FirstChild;
		for (XmlNode xmlNode = firstChild.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			string attribute = "250";
			string attribute2 = "250";
			if (xmlNode.Attributes["width"] != null)
			{
				attribute = xmlNode.Attributes["width"].Value;
			}
			if (xmlNode.Attributes["height"] != null)
			{
				attribute2 = xmlNode.Attributes["height"].Value;
			}
			if (xmlNode.Attributes["focus"] != null)
			{
				attribute = xmlNode.Attributes["focus"].Value;
			}
			this.m_commands.Add(new Dialog.DialogCommand(xmlNode.Name, xmlNode.InnerText, attribute, attribute2));
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000ED24 File Offset: 0x0000CF24
	public void LoadDialog(string scenename)
	{
		this.m_isBriefing = false;
		this.CreateGui("dlg");
		this.LoadScene(scenename);
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000ED40 File Offset: 0x0000CF40
	public void LoadBriefing(string scenename)
	{
		this.m_isBriefing = true;
		this.CreateGui("brf");
		this.LoadScene(scenename);
		if (scenename.Contains("debriefing"))
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").GetComponent<UIButton>().Text = Localize.instance.Translate("$label_continue");
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000EDA4 File Offset: 0x0000CFA4
	public void LoadDebriefing(string scenename)
	{
		this.m_isBriefing = true;
		this.CreateGui("debrf");
		this.LoadScene(scenename);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000EDC0 File Offset: 0x0000CFC0
	public void LoadTutorial(string scenename)
	{
		this.m_isBriefing = false;
		this.CreateGui("tut");
		this.LoadScene(scenename);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0000EDDC File Offset: 0x0000CFDC
	public void SetImage(SimpleSprite sprite, string name, string attr1, string attr2)
	{
		int num = int.Parse(attr1);
		int num2 = int.Parse(attr2);
		if (name.Length == 0)
		{
			sprite.gameObject.SetActiveRecursively(false);
			return;
		}
		sprite.gameObject.SetActiveRecursively(true);
		Texture2D texture2D = Resources.Load(name) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogError("Failed to load texture");
		}
		float x = (float)texture2D.width;
		float y = (float)texture2D.height;
		sprite.Setup((float)num, (float)num2, new Vector2(0f, y), new Vector2(x, y));
		sprite.SetTexture(texture2D);
		sprite.Setup((float)num, (float)num2, new Vector2(0f, y), new Vector2(x, y));
		sprite.UpdateUVs();
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000EEA0 File Offset: 0x0000D0A0
	public void SetPortrait(SimpleSprite sprite, string name)
	{
		if (name.Length == 0)
		{
			sprite.gameObject.SetActiveRecursively(false);
			return;
		}
		sprite.gameObject.SetActiveRecursively(true);
		float width = sprite.width;
		float height = sprite.height;
		Texture2D texture2D = Resources.Load(name) as Texture2D;
		if (texture2D == null)
		{
			PLog.LogError("Failed to load texture");
		}
		float x = (float)texture2D.width;
		float y = (float)texture2D.height;
		sprite.Setup(width, height, new Vector2(0f, y), new Vector2(x, y));
		sprite.SetTexture(texture2D);
		sprite.Setup(width, height, new Vector2(0f, y), new Vector2(x, y));
		sprite.UpdateUVs();
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000EF5C File Offset: 0x0000D15C
	private bool IsPlayback()
	{
		Camera component = GameObject.Find("GameCamera").GetComponent<Camera>();
		return component != null && component.enabled;
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000EF94 File Offset: 0x0000D194
	public bool PlayNextScene()
	{
		for (;;)
		{
			this.m_currentDialog++;
			if (this.m_currentDialog >= this.m_dialog.Length)
			{
				break;
			}
			MNAction.MNActionElement mnactionElement = this.m_dialog[this.m_currentDialog];
			if (mnactionElement.m_type == MNAction.ActionType.PlayScene)
			{
				goto Block_1;
			}
			if (mnactionElement.m_type == MNAction.ActionType.ShowBriefing)
			{
				goto Block_3;
			}
			if (mnactionElement.m_type == MNAction.ActionType.ShowDebriefing)
			{
				goto Block_5;
			}
			if (mnactionElement.m_type == MNAction.ActionType.ShowTutorial)
			{
				goto Block_7;
			}
			if (mnactionElement.m_type == MNAction.ActionType.UpdateObjective)
			{
				TurnMan.instance.SetMissionObjective(mnactionElement.m_parameter, mnactionElement.m_objectiveStatus);
			}
			if (mnactionElement.m_type == MNAction.ActionType.MissionVictory)
			{
				TurnMan.instance.m_endGame = GameOutcome.Victory;
			}
			if (mnactionElement.m_type == MNAction.ActionType.MissionDefeat)
			{
				TurnMan.instance.m_endGame = GameOutcome.Defeat;
			}
			if (mnactionElement.m_type == MNAction.ActionType.MissionDefeat)
			{
				TurnMan.instance.m_endGame = GameOutcome.Defeat;
			}
			if (mnactionElement.m_type == MNAction.ActionType.Marker)
			{
				this.SetMarker(mnactionElement);
			}
			if (mnactionElement.m_type == MNAction.ActionType.Message)
			{
				MessageLog.instance.ShowMessage(MessageLog.TextPosition.Middle, "$" + mnactionElement.m_parameter, string.Empty, "NewsflashMessage", 2f);
			}
			if (mnactionElement.m_type == MNAction.ActionType.PlayerChange)
			{
				this.ActionPlayerChange(mnactionElement);
			}
			if (mnactionElement.m_type == MNAction.ActionType.Event)
			{
				this.ActionEvent(mnactionElement);
			}
			if (mnactionElement.m_type == MNAction.ActionType.MissionAchievement)
			{
				TurnMan.instance.m_missionAchievement = int.Parse(mnactionElement.m_parameter);
				Constants.AchivementId missionAchievement = (Constants.AchivementId)TurnMan.instance.m_missionAchievement;
				PLog.Log("Mission Achievement Set To: " + missionAchievement.ToString());
			}
		}
		return false;
		Block_1:
		if (!this.m_skipCutscenes)
		{
			MNAction.MNActionElement mnactionElement;
			string parameter = mnactionElement.m_parameter;
			this.LoadDialog(parameter);
			this.m_waitInput = false;
		}
		return true;
		Block_3:
		if (!this.m_skipCutscenes)
		{
			MNAction.MNActionElement mnactionElement;
			string parameter2 = mnactionElement.m_parameter;
			this.LoadBriefing(parameter2);
			this.m_waitInput = false;
		}
		return true;
		Block_5:
		if (!this.m_skipCutscenes)
		{
			MNAction.MNActionElement mnactionElement;
			string parameter3 = mnactionElement.m_parameter;
			this.LoadDebriefing(parameter3);
			this.m_waitInput = false;
		}
		return true;
		Block_7:
		if (!this.m_skipCutscenes)
		{
			MNAction.MNActionElement mnactionElement;
			string parameter4 = mnactionElement.m_parameter;
			this.LoadTutorial(parameter4);
			this.m_waitInput = false;
		}
		return true;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000F1B0 File Offset: 0x0000D3B0
	private void ActionEvent(MNAction.MNActionElement ae)
	{
		GameObject target = ae.GetTarget();
		if (target == null)
		{
			return;
		}
		MNode component = target.GetComponent<MNode>();
		if (component)
		{
			component.OnEvent(ae.m_parameter);
		}
		Platform component2 = target.GetComponent<Platform>();
		if (component2)
		{
			component2.OnEvent(ae.m_parameter);
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000F210 File Offset: 0x0000D410
	private void ActionPlayerChange(MNAction.MNActionElement ae)
	{
		GameObject target = ae.GetTarget();
		if (target == null)
		{
			return;
		}
		List<GameObject> targets = MNode.GetTargets(target);
		int owner = int.Parse(ae.m_parameter);
		for (int i = 0; i < targets.Count; i++)
		{
			GameObject gameObject = targets[i];
			Platform component = gameObject.GetComponent<Platform>();
			if (component)
			{
				component.SetOwner(owner);
			}
			else
			{
				MNSpawn component2 = gameObject.GetComponent<MNSpawn>();
				if (component2)
				{
					GameObject spawnedShip = component2.GetSpawnedShip();
					if (!(spawnedShip == null))
					{
						Ship component3 = spawnedShip.GetComponent<Ship>();
						if (!(component3 == null))
						{
							component3.SetOwner(owner);
							component3.ResetAiState();
						}
					}
				}
			}
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000F2E0 File Offset: 0x0000D4E0
	public void SetMarker(MNAction.MNActionElement ae)
	{
		GameObject target = ae.GetTarget();
		if (target == null)
		{
			return;
		}
		GameObject gameObject = target;
		Marker component = gameObject.GetComponent<Marker>();
		if (component != null)
		{
			if (ae.m_objectiveType == Unit.ObjectiveTypes.None)
			{
				component.SetVisibleState(false);
			}
			else
			{
				component.SetVisibleState(true);
			}
		}
		Unit component2 = gameObject.GetComponent<Unit>();
		if (component2 != null)
		{
			component2.SetObjective(ae.m_objectiveType);
		}
		MNSpawn component3 = gameObject.GetComponent<MNSpawn>();
		if (component3 != null)
		{
			component3.SetObjective(ae.m_objectiveType);
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000F378 File Offset: 0x0000D578
	public void SetCommands(MNAction.MNActionElement[] commands)
	{
		this.m_commands.Clear();
		this.m_index = 0;
		this.m_dialog = commands;
		this.m_currentDialog = -1;
		this.m_skipCutscenes = !this.IsPlayback();
		Dialog.m_dialogActive = true;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000F3B0 File Offset: 0x0000D5B0
	public void PlayAll()
	{
		while (this.PlayNextScene())
		{
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000F3C4 File Offset: 0x0000D5C4
	public void Update(List<ClientPlayer> players)
	{
		if (this.m_gui)
		{
			this.m_morsePlayer.Update();
		}
		this.m_players = players;
		this.SetupMacros();
		if (TurnMan.instance != null && TurnMan.instance.m_dialog != null)
		{
			this.SetCommands(TurnMan.instance.m_dialog);
			TurnMan.instance.m_dialog = null;
			if (!this.PlayNextScene())
			{
				Dialog.m_dialogActive = false;
				return;
			}
			this.m_onPlayDialog(this.m_isBriefing);
			GameCamera component = GameObject.Find("GameCamera").GetComponent<GameCamera>();
			this.m_lastMode = component.GetMode();
			component.SetMode(GameCamera.Mode.Disabled);
		}
		if (this.m_commands.Count == 0)
		{
			return;
		}
		if (this.m_waitInput)
		{
			return;
		}
		while (this.m_index < this.m_commands.Count)
		{
			this.RunCommand(this.m_commands[this.m_index]);
			this.m_index++;
			if (this.m_waitInput)
			{
				this.OnNextAdvise(null);
				return;
			}
		}
		if (this.m_index >= this.m_commands.Count && !this.PlayNextScene())
		{
			this.EndDialog();
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000F50C File Offset: 0x0000D70C
	public void FixedUpdate()
	{
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000F510 File Offset: 0x0000D710
	private void SetupMacros()
	{
		this.m_macros.Clear();
		this.m_macros = new Dictionary<string, string>();
		foreach (ClientPlayer clientPlayer in this.m_players)
		{
			string playerName = this.m_turnMan.GetPlayerName(clientPlayer.m_id);
			this.m_macros["playername" + clientPlayer.m_id.ToString()] = playerName;
		}
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000F5B8 File Offset: 0x0000D7B8
	private void RunCommand(Dialog.DialogCommand cmd)
	{
		GameCamera component = GameObject.Find("GameCamera").GetComponent<GameCamera>();
		if (cmd.m_command == "NoSkip")
		{
			this.m_showSkipButton = false;
		}
		if (cmd.m_command == "PlaySound")
		{
			AudioClip clip = (AudioClip)Resources.Load(cmd.m_param, typeof(AudioClip));
			AudioSource.PlayClipAtPoint(clip, component.transform.position);
		}
		if (cmd.m_command == "SetName")
		{
			string text = Localize.instance.Translate(cmd.m_param);
			this.m_currentName = text;
			this.m_name.Text = text;
		}
		if (cmd.m_command == "MissionAdviceName")
		{
			string text2 = Localize.instance.Translate(cmd.m_param);
			SpriteText component2 = GuiUtils.FindChildOf(this.m_gui.transform, "name").GetComponent<SpriteText>();
			component2.Text = text2;
		}
		if (cmd.m_command == "SetText")
		{
			string text3 = Localize.instance.Translate(cmd.m_param);
			this.m_currentText = text3;
			this.m_text.Text = text3;
		}
		if (cmd.m_command == "WaitInput")
		{
			this.m_waitInput = true;
		}
		if (cmd.m_command == "SetPortrait")
		{
			this.m_currentPortrait = cmd.m_param;
			this.SetPortrait(this.m_portrait, cmd.m_param);
		}
		if (cmd.m_command == "SetWallpaper")
		{
			if (cmd.m_param == string.Empty)
			{
				this.m_wallpaper.gameObject.SetActiveRecursively(false);
			}
			else
			{
				this.SetPortrait(this.m_wallpaper, cmd.m_param);
				this.m_wallpaper.gameObject.SetActiveRecursively(true);
			}
		}
		if (cmd.m_command == "SetImage")
		{
			if (cmd.m_param == string.Empty)
			{
				this.m_image.gameObject.SetActiveRecursively(false);
			}
			else
			{
				this.SetImage(this.m_image, cmd.m_param, cmd.m_attribute1, cmd.m_attribute2);
				this.m_image.gameObject.SetActiveRecursively(true);
			}
		}
		if (this.m_gui)
		{
			if (cmd.m_command == "MissionTitle")
			{
				string text4 = Localize.instance.Translate(cmd.m_param);
				GuiUtils.FindChildOf(this.m_gui.transform, "MissionTitleLabel").GetComponent<SpriteText>().Text = text4;
			}
			if (cmd.m_command == "MissionLabel")
			{
				string text5 = Localize.instance.Translate(cmd.m_param);
				GuiUtils.FindChildOf(this.m_gui.transform, "MissionNumberLabel").GetComponent<SpriteText>().Text = text5;
			}
			if (cmd.m_command == "MissionIcon")
			{
				SimpleSprite component3 = GuiUtils.FindChildOf(this.m_gui.transform, "MissionIcon").GetComponent<SimpleSprite>();
				this.SetPortrait(component3, cmd.m_param);
			}
			if (cmd.m_command == "MissionText")
			{
				GameObject original = Resources.Load("gui/Briefing/MissionBriefingListItem") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				GuiUtils.FindChildOf(gameObject.transform, "MissionBriefingText").GetComponent<SpriteText>().Text = Localize.instance.Translate(cmd.m_param).Replace("\\n", "\n");
				UIScrollList component4 = GuiUtils.FindChildOf(this.m_gui.transform, "MissionBriefingScrollist").GetComponent<UIScrollList>();
				UIListItemContainer component5 = gameObject.GetComponent<UIListItemContainer>();
				component4.AddItem(component5);
				this.m_morsePlayer.SetText("SOS SOS SOS SOS SOS SOS SOS SOS SOS SOS PARIS WE HAVE COLLISION WITH ICEBERG. SINKING. CAN HEAR NOTHING FOR NOISE OF STEAM");
			}
			if (cmd.m_command == "MissionImage1")
			{
				SimpleSprite component6 = GuiUtils.FindChildOf(this.m_gui.transform, "Image1").GetComponent<SimpleSprite>();
				GuiUtils.SetImage(component6, cmd.m_param);
			}
			if (cmd.m_command == "MissionImage2")
			{
				SimpleSprite component7 = GuiUtils.FindChildOf(this.m_gui.transform, "Image2").GetComponent<SimpleSprite>();
				GuiUtils.SetImage(component7, cmd.m_param);
			}
			if (cmd.m_command == "MissionImage3")
			{
				SimpleSprite component8 = GuiUtils.FindChildOf(this.m_gui.transform, "Image3").GetComponent<SimpleSprite>();
				this.SetPortrait(component8, cmd.m_param);
			}
			if (cmd.m_command == "MissionImage4")
			{
				SimpleSprite component9 = GuiUtils.FindChildOf(this.m_gui.transform, "Image4").GetComponent<SimpleSprite>();
				this.SetPortrait(component9, cmd.m_param);
			}
			if (cmd.m_command == "MissionPrimObj")
			{
				GameObject original2 = Resources.Load("gui/Briefing/PrimaryObjectivesListItem") as GameObject;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
				UIScrollList component10 = GuiUtils.FindChildOf(this.m_gui.transform, "PrimaryObjectivesScrollist").GetComponent<UIScrollList>();
				UIListItemContainer component11 = gameObject2.GetComponent<UIListItemContainer>();
				component10.AddItem(component11);
				GuiUtils.FindChildOf(gameObject2.transform, "PrimaryObjectivesText").GetComponent<SpriteText>().Text = Localize.instance.Translate(cmd.m_param);
			}
			if (cmd.m_command == "MissionSecObj")
			{
				GameObject original3 = Resources.Load("gui/Briefing/SecondaryObjectivesListItem") as GameObject;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(original3) as GameObject;
				UIScrollList component12 = GuiUtils.FindChildOf(this.m_gui.transform, "SecondaryObjectivesScrollist").GetComponent<UIScrollList>();
				UIListItemContainer component13 = gameObject3.GetComponent<UIListItemContainer>();
				component12.AddItem(component13);
				GuiUtils.FindChildOf(gameObject3.transform, "SecondaryObjectivesText").GetComponent<SpriteText>().Text = Localize.instance.Translate(cmd.m_param);
			}
			if (cmd.m_command == "MissionAdvice")
			{
				string text6 = Localize.instance.Translate(cmd.m_param);
				DialogAdvice dialogAdvice = new DialogAdvice();
				dialogAdvice.m_text = text6;
				dialogAdvice.m_portrait = this.m_advicePortrait;
				this.m_advice.Add(dialogAdvice);
			}
			if (cmd.m_command == "MissionAdvicePort")
			{
				this.m_advicePortrait = cmd.m_param;
			}
			if (cmd.m_command == "MissionImagePosition")
			{
				string text7 = Localize.instance.Translate(cmd.m_param);
				GuiUtils.FindChildOf(this.m_gui.transform, "PositionLabel").GetComponent<SpriteText>().Text = text7;
			}
			if (cmd.m_command == "MissionImageTime")
			{
				string text8 = Localize.instance.Translate(cmd.m_param);
				GuiUtils.FindChildOf(this.m_gui.transform, "TimeLabel").GetComponent<SpriteText>().Text = text8;
			}
		}
		if (cmd.m_command == "TUT")
		{
			GameObject gameObject4 = GameObject.Find("tutorial");
			if (gameObject4)
			{
				MNTutorial component14 = gameObject4.GetComponent<MNTutorial>();
				component14.OnCommand(cmd.m_param, cmd.m_attribute1, cmd.m_attribute2);
			}
		}
		GuiUtils.FindChildOf(this.m_gui.transform, "btnSkip").SetActiveRecursively(this.m_showSkipButton);
		if (this.m_isBriefing)
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").SetActiveRecursively(this.m_advice.Count >= 2);
		}
		else
		{
			GuiUtils.FindChildOf(this.m_gui.transform, "btnNext").SetActiveRecursively(true);
		}
		if (this.m_isBriefing && this.m_box)
		{
			this.m_box.gameObject.SetActiveRecursively(false);
		}
		if (this.m_currentName.Length != 0 || this.m_currentText.Length != 0 || this.m_currentPortrait.Length != 0)
		{
			this.m_box.gameObject.active = true;
		}
		if (this.m_currentName.Length != 0)
		{
			this.m_name.gameObject.active = true;
		}
		if (this.m_currentText.Length != 0)
		{
			this.m_text.gameObject.active = true;
		}
		if (this.m_currentPortrait.Length != 0)
		{
			this.m_portraitOverlay.gameObject.active = true;
			this.m_portrait.gameObject.active = true;
		}
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000FE5C File Offset: 0x0000E05C
	public static bool IsDialogActive()
	{
		return Dialog.m_dialogActive;
	}

	// Token: 0x04000170 RID: 368
	private List<Dialog.DialogCommand> m_commands = new List<Dialog.DialogCommand>();

	// Token: 0x04000171 RID: 369
	public int m_index;

	// Token: 0x04000172 RID: 370
	private bool m_showSkipButton = true;

	// Token: 0x04000173 RID: 371
	private GameObject m_gui;

	// Token: 0x04000174 RID: 372
	private GameObject m_guiCamera;

	// Token: 0x04000175 RID: 373
	private TurnMan m_turnMan;

	// Token: 0x04000176 RID: 374
	private SpriteText m_text;

	// Token: 0x04000177 RID: 375
	private SpriteText m_name;

	// Token: 0x04000178 RID: 376
	private SimpleSprite m_portrait;

	// Token: 0x04000179 RID: 377
	private SimpleSprite m_wallpaper;

	// Token: 0x0400017A RID: 378
	private SimpleSprite m_image;

	// Token: 0x0400017B RID: 379
	private SimpleSprite m_box;

	// Token: 0x0400017C RID: 380
	private PackedSprite m_portraitOverlay;

	// Token: 0x0400017D RID: 381
	private string m_currentName;

	// Token: 0x0400017E RID: 382
	private string m_currentText;

	// Token: 0x0400017F RID: 383
	private string m_currentPortrait;

	// Token: 0x04000180 RID: 384
	public Action<bool> m_onPlayDialog;

	// Token: 0x04000181 RID: 385
	public Action m_onEndDialog;

	// Token: 0x04000182 RID: 386
	private bool m_waitInput;

	// Token: 0x04000183 RID: 387
	private bool m_skipCutscenes;

	// Token: 0x04000184 RID: 388
	private MNAction.MNActionElement[] m_dialog;

	// Token: 0x04000185 RID: 389
	private int m_currentDialog;

	// Token: 0x04000186 RID: 390
	private GameCamera.Mode m_lastMode;

	// Token: 0x04000187 RID: 391
	private List<ClientPlayer> m_players;

	// Token: 0x04000188 RID: 392
	private Dictionary<string, string> m_macros = new Dictionary<string, string>();

	// Token: 0x04000189 RID: 393
	private List<DialogAdvice> m_advice = new List<DialogAdvice>();

	// Token: 0x0400018A RID: 394
	private int m_currentAdvice = -1;

	// Token: 0x0400018B RID: 395
	private string m_advicePortrait = string.Empty;

	// Token: 0x0400018C RID: 396
	private static bool m_dialogActive;

	// Token: 0x0400018D RID: 397
	private MorsePlayer m_morsePlayer = new MorsePlayer();

	// Token: 0x0400018E RID: 398
	private bool m_isBriefing;

	// Token: 0x02000035 RID: 53
	private class DialogCommand
	{
		// Token: 0x06000243 RID: 579 RVA: 0x0000FE64 File Offset: 0x0000E064
		public DialogCommand(string command, string param, string attribute1, string attribute2)
		{
			this.m_command = command;
			this.m_param = param;
			this.m_attribute1 = attribute1;
			this.m_attribute2 = attribute2;
		}

		// Token: 0x0400018F RID: 399
		public string m_command;

		// Token: 0x04000190 RID: 400
		public string m_param;

		// Token: 0x04000191 RID: 401
		public string m_attribute1;

		// Token: 0x04000192 RID: 402
		public string m_attribute2;
	}
}
