using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200008D RID: 141
[AddComponentMenu("Scripts/Mission/MNTutorial")]
public class MNTutorial : MNode
{
	// Token: 0x0600057F RID: 1407 RVA: 0x0002C134 File Offset: 0x0002A334
	public override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0002C13C File Offset: 0x0002A33C
	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0002C144 File Offset: 0x0002A344
	private void PlayDialog(string name)
	{
		MNAction.MNActionElement[] array = new MNAction.MNActionElement[]
		{
			new MNAction.MNActionElement()
		};
		array[0].m_type = MNAction.ActionType.ShowTutorial;
		array[0].m_parameter = "leveldata/campaign/tutorial/" + name;
		TurnMan.instance.m_dialog = array;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0002C188 File Offset: 0x0002A388
	private bool DoEventNow(int eventId)
	{
		if (eventId == this.m_nextEvent)
		{
			this.m_time = 0f;
			this.m_runEvent = true;
			return true;
		}
		return false;
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0002C1AC File Offset: 0x0002A3AC
	private void PlayDialogPlatform(string name)
	{
		string name2;
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			name2 = name + "_pad";
		}
		else
		{
			name2 = name + "_pc";
		}
		this.PlayDialog(name2);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0002C1F8 File Offset: 0x0002A3F8
	private void UpdateTutorial()
	{
		string str = this.m_levelName + "/" + this.m_levelName + "_";
		if (this.IsTutorialOver())
		{
			return;
		}
		if (this.DoEventNow(1))
		{
			this.PlayDialogPlatform(str + this.m_currentTurn.ToString() + "_1");
		}
		if (this.m_runEvent)
		{
			this.m_nextEvent++;
		}
		this.m_runEvent = false;
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0002C278 File Offset: 0x0002A478
	private void NextTurn()
	{
		if (this.m_currentTurn == 0)
		{
			MNTutorial.m_endTutorial = false;
		}
		this.m_currentTurn++;
		this.m_timeInTurn = 0f;
		if (MNTutorial.m_endTutorial)
		{
			this.m_endTutorialTurn = this.m_currentTurn;
		}
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0002C2C8 File Offset: 0x0002A4C8
	private void UpdateGui()
	{
		if (this.m_mainObj == null)
		{
			this.m_mainObj = GameObject.Find("mainobject");
		}
		if (this.m_mainObj)
		{
			if (this.m_hideCommit)
			{
				GameObject gameObject = GuiUtils.FindChildOf(this.m_mainObj.transform, "ControlPanel_Planning");
				if (gameObject)
				{
					gameObject.SetActiveRecursively(!this.m_hideCommit);
				}
			}
			GameObject gameObject2 = GuiUtils.FindChildOf(this.m_mainObj.transform, "Replay_Button");
			if (gameObject2)
			{
				gameObject2.SetActiveRecursively(false);
			}
		}
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0002C36C File Offset: 0x0002A56C
	protected Ship GetPlayerShip(int player)
	{
		List<NetObj> all = NetObj.GetAll();
		foreach (NetObj netObj in all)
		{
			Ship component = netObj.GetComponent<Ship>();
			if (component != null && !component.IsDead() && component.GetOwner() == player)
			{
				return component;
			}
		}
		return null;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0002C404 File Offset: 0x0002A604
	public void OnCommand(string command, string parameter, string parameter2)
	{
		if (command == "clearorder")
		{
			int player = int.Parse(parameter);
			Ship playerShip = this.GetPlayerShip(player);
			playerShip.ClearOrders();
		}
		if (command == "addorder")
		{
			GameObject gameObject = GameObject.Find(parameter);
			Ship playerShip2 = this.GetPlayerShip(0);
			Order order = new Order(playerShip2, Order.Type.MoveForward, gameObject.transform.position);
			playerShip2.AddOrder(order);
		}
		if (command == "addorderto")
		{
			PLog.Log("addorderto 1");
			int player2 = int.Parse(parameter);
			Ship playerShip3 = this.GetPlayerShip(player2);
			GameObject gameObject2 = GameObject.Find(parameter2);
			Order order2 = new Order(playerShip3, Order.Type.MoveForward, gameObject2.transform.position);
			playerShip3.AddOrder(order2);
			PLog.Log("addorderto 2");
		}
		if (command == "mine")
		{
			int player3 = int.Parse(parameter);
			Ship playerShip4 = this.GetPlayerShip(player3);
			GameObject gameObject3 = GameObject.Find(parameter2);
			Gun componentInChildren = playerShip4.gameObject.GetComponentInChildren<Gun>();
			if (componentInChildren == null)
			{
				return;
			}
			PLog.Log("mine: ");
			Order order3 = new Order(componentInChildren, Order.Type.Fire, gameObject3.transform.position);
			componentInChildren.ClearOrders();
			componentInChildren.AddOrder(order3);
		}
		if (command == "attack")
		{
			int player4 = int.Parse(parameter);
			Ship playerShip5 = this.GetPlayerShip(player4);
			int player5 = int.Parse(parameter2);
			Ship playerShip6 = this.GetPlayerShip(player5);
			Gun componentInChildren2 = playerShip5.gameObject.GetComponentInChildren<Gun>();
			if (componentInChildren2 == null)
			{
				return;
			}
			PLog.Log("attack: ");
			Order order4 = new Order(componentInChildren2, Order.Type.Fire, playerShip6.transform.position);
			componentInChildren2.ClearOrders();
			componentInChildren2.AddOrder(order4);
		}
		if (command == "stopattack")
		{
			int player6 = int.Parse(parameter);
			Ship playerShip7 = this.GetPlayerShip(player6);
		}
		if (command == "deploy")
		{
			int player7 = int.Parse(parameter);
			Ship playerShip8 = this.GetPlayerShip(player7);
			HPModule hpmodule = null;
			if (parameter2 == "radar")
			{
				hpmodule = playerShip8.gameObject.GetComponentInChildren<Radar>();
			}
			if (parameter2 == "cloak")
			{
				hpmodule = playerShip8.gameObject.GetComponentInChildren<Cloak>();
			}
			if (parameter2 == "shield")
			{
				hpmodule = playerShip8.gameObject.GetComponentInChildren<Shield>();
			}
			if (hpmodule == null)
			{
				return;
			}
			PLog.Log("Deploy: " + parameter2);
			if (parameter2 == "shield")
			{
				hpmodule.GetComponent<Shield>().SetDeployShield(Shield.DeployType.Forward);
			}
			else
			{
				hpmodule.SetDeploy(true);
			}
		}
		if (command == "commitoff")
		{
			this.m_hideCommit = true;
		}
		if (command == "selectionon")
		{
			this.m_allowSelection = true;
		}
		if (command == "selectionoff")
		{
			this.m_allowSelection = false;
		}
		if (command == "flowermenuon")
		{
			this.m_allowFlowerMenu = true;
		}
		if (command == "flowermenuoff")
		{
			this.m_allowFlowerMenu = false;
		}
		if (command == "commiton")
		{
			this.m_hideCommit = false;
			this.UpdateGui();
		}
		if (command == "cameraon")
		{
			GameCamera component = GameObject.Find("GameCamera").GetComponent<GameCamera>();
			component.SetMode(GameCamera.Mode.Active);
		}
		if (command == "cameraoff")
		{
			GameCamera component2 = GameObject.Find("GameCamera").GetComponent<GameCamera>();
			component2.SetMode(GameCamera.Mode.Disabled);
		}
		if (command == "end")
		{
			MNTutorial.m_endTutorial = true;
		}
		if (command == "selectplayer")
		{
			int player8 = int.Parse(parameter);
			int num = int.Parse(parameter2);
			Ship playerShip9 = this.GetPlayerShip(player8);
			GameCamera component3 = GameObject.Find("GameCamera").GetComponent<GameCamera>();
			component3.SetAllowSelection(true);
			component3.SetSelected(playerShip9.gameObject);
			component3.SetAllowSelection(this.m_allowSelection);
			component3.SetFocus(playerShip9.gameObject.transform.position, (float)num);
		}
		if (command == "focus")
		{
			GameCamera component4 = GameObject.Find("GameCamera").GetComponent<GameCamera>();
			GameObject gameObject4 = GameObject.Find(parameter);
			int num2 = int.Parse(parameter2);
			if (gameObject4)
			{
				component4.SetFocus(gameObject4.transform.position, (float)num2);
			}
		}
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0002C874 File Offset: 0x0002AA74
	private bool IsTutorialOver()
	{
		return this.m_endTutorialTurn != -1 && this.m_currentTurn >= this.m_endTutorialTurn;
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0002C8A4 File Offset: 0x0002AAA4
	public void Update()
	{
		if (this.m_endTutorialTurn != -1 && this.m_currentTurn >= this.m_endTutorialTurn)
		{
			this.m_allowSelection = true;
			this.m_allowFlowerMenu = true;
			this.m_hideCommit = false;
		}
		GameCamera component = GameObject.Find("GameCamera").GetComponent<GameCamera>();
		if (component == null)
		{
			return;
		}
		component.SetAllowSelection(this.m_allowSelection);
		component.SetAllowFlowerMenu(this.m_allowFlowerMenu);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0002C918 File Offset: 0x0002AB18
	private void FixedUpdate()
	{
		this.UpdateGui();
		if (NetObj.m_simulating)
		{
			this.m_timeInTurn += Time.fixedDeltaTime;
			if (this.b_startOfTurn)
			{
				this.NextTurn();
				PLog.Log("Starting turn: " + this.m_currentTurn.ToString() + " End Turn is: " + this.m_endTutorialTurn.ToString());
				this.b_startOfTurn = false;
			}
		}
		Camera component = GameObject.Find("GameCamera").GetComponent<Camera>();
		if (component == null)
		{
			return;
		}
		if (!component.enabled)
		{
			return;
		}
		if (NetObj.IsSimulating())
		{
			return;
		}
		if (Dialog.IsDialogActive())
		{
			return;
		}
		this.m_time += Time.fixedDeltaTime;
		if (this.m_time < 2f)
		{
			return;
		}
		this.UpdateTutorial();
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0002C9F4 File Offset: 0x0002ABF4
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(this.m_currentTurn);
		writer.Write(this.m_hideCommit);
		writer.Write(this.m_levelName);
		writer.Write(this.m_endTutorialTurn);
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0002CA38 File Offset: 0x0002AC38
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		this.m_currentTurn = reader.ReadInt32();
		this.m_hideCommit = reader.ReadBoolean();
		this.m_levelName = reader.ReadString();
		this.m_endTutorialTurn = reader.ReadInt32();
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0002CA7C File Offset: 0x0002AC7C
	public static bool IsTutorialActive()
	{
		GameObject exists = GameObject.Find("tutorial");
		return exists;
	}

	// Token: 0x0400049A RID: 1178
	public string m_levelName = "t1m2";

	// Token: 0x0400049B RID: 1179
	private float m_time;

	// Token: 0x0400049C RID: 1180
	private int m_nextEvent = 1;

	// Token: 0x0400049D RID: 1181
	private bool m_runEvent;

	// Token: 0x0400049E RID: 1182
	private bool b_startOfTurn = true;

	// Token: 0x0400049F RID: 1183
	private int m_currentTurn;

	// Token: 0x040004A0 RID: 1184
	private float m_timeInTurn;

	// Token: 0x040004A1 RID: 1185
	private bool m_hideCommit = true;

	// Token: 0x040004A2 RID: 1186
	private bool m_allowSelection;

	// Token: 0x040004A3 RID: 1187
	private bool m_allowFlowerMenu;

	// Token: 0x040004A4 RID: 1188
	private int m_endTutorialTurn = -1;

	// Token: 0x040004A5 RID: 1189
	private static bool m_endTutorial;

	// Token: 0x040004A6 RID: 1190
	private GameObject m_mainObj;
}
