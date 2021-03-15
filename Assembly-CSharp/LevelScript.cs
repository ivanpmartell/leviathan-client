using System;
using System.IO;
using PTech;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class LevelScript : MonoBehaviour
{
	// Token: 0x0600087D RID: 2173 RVA: 0x0003EAB0 File Offset: 0x0003CCB0
	private void Awake()
	{
		WaterSurface component = this.m_waterSurface.GetComponent<WaterSurface>();
		if (component != null)
		{
			this.SetupJazzy();
			component.SetMapSize(this.m_mapSize);
		}
		else
		{
			PLog.LogError("Failed to find water");
		}
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0003EAF8 File Offset: 0x0003CCF8
	private void SetupJazzy()
	{
		GameType gameType = ClientGame.instance.GetGameType();
		if (gameType != GameType.Assassination && gameType != GameType.Points)
		{
			return;
		}
		bool flag = PlayerPrefs.GetInt("JazzyMode", 0) == 1;
		if (flag)
		{
			WaterSurface component = this.m_waterSurface.GetComponent<WaterSurface>();
			Material material = Resources.Load("WaterMaterial jazzy") as Material;
			DebugUtils.Assert(material != null);
			component.GetComponent<MeshRenderer>().sharedMaterial = material;
			Light light = base.gameObject.transform.FindChild("Directional light").light;
			light.transform.rotation = Quaternion.Euler(new Vector3(19.86757f, 126.9407f, 215.5935f));
			light.color = new Color(0.6039216f, 0.40784314f, 0.5372549f, 255f);
			light.intensity = 1f;
		}
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0003EBD8 File Offset: 0x0003CDD8
	private void Start()
	{
		this.SetupBorder();
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0003EBE0 File Offset: 0x0003CDE0
	private void OnDestroy()
	{
		if (this.m_gameModeScript != null)
		{
			this.m_gameModeScript.Close();
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0003EC00 File Offset: 0x0003CE00
	public void SetupGameMode(GameSettings gameSettings)
	{
		if (this.m_gameModeScript != null)
		{
			this.m_gameModeScript.Close();
			this.m_gameModeScript = null;
		}
		if (this.m_gameModes != null)
		{
			foreach (LevelScript.GameModeScript gameModeScript in this.m_gameModes)
			{
				DebugUtils.Assert(gameModeScript.m_gameObject != null, "GameModeScript is not attached to an GameObject");
				gameModeScript.m_gameObject.SetActiveRecursively(false);
			}
			foreach (LevelScript.GameModeScript gameModeScript2 in this.m_gameModes)
			{
				if (gameModeScript2.m_type == gameSettings.m_gameType)
				{
					gameModeScript2.m_gameObject.SetActiveRecursively(true);
					this.m_gameModeScript = gameModeScript2.m_gameObject.GetComponent<GameMode>();
					DebugUtils.Assert(this.m_gameModeScript != null, "game mode object missing GameMode-script");
					this.m_gameModeScript.Setup(gameSettings);
				}
			}
		}
		if (this.m_gameModeScript == null)
		{
			PLog.LogError("Missing game mode object in levelobject for game mode: " + gameSettings.m_gameType.ToString());
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0003ED24 File Offset: 0x0003CF24
	public void SimulationUpdate(float dt)
	{
		DebugUtils.Assert(this.m_gameModeScript != null);
		this.m_gameModeScript.SimulationUpdate(dt);
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0003ED44 File Offset: 0x0003CF44
	private void SetupBorder()
	{
		int mapSize = this.m_mapSize;
		if (mapSize != 500)
		{
			if (mapSize != 750)
			{
				if (mapSize != 1000)
				{
					if (mapSize == 2000)
					{
						UnityEngine.Object.Instantiate(this.m_border2000);
					}
				}
				else
				{
					UnityEngine.Object.Instantiate(this.m_border1000);
				}
			}
			else
			{
				UnityEngine.Object.Instantiate(this.m_border750);
			}
		}
		else
		{
			UnityEngine.Object.Instantiate(this.m_border500);
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0003EDD0 File Offset: 0x0003CFD0
	public int GetMapSize()
	{
		return this.m_mapSize;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0003EDD8 File Offset: 0x0003CFD8
	public void SaveState(BinaryWriter writer)
	{
		DebugUtils.Assert(this.m_gameModeScript != null);
		if (this.m_waterSurface != null)
		{
			this.m_waterSurface.GetComponent<WaterSurface>().SaveState(writer);
		}
		this.m_gameModeScript.SaveState(writer);
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0003EE24 File Offset: 0x0003D024
	public void LoadState(BinaryReader reader)
	{
		DebugUtils.Assert(this.m_gameModeScript != null);
		if (this.m_waterSurface != null)
		{
			this.m_waterSurface.GetComponent<WaterSurface>().LoadState(reader);
		}
		this.m_gameModeScript.LoadState(reader);
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0003EE70 File Offset: 0x0003D070
	public void SetSimulating(bool simulating)
	{
		if (this.m_waterSurface != null)
		{
			this.m_waterSurface.GetComponent<WaterSurface>().SetSimulating(simulating);
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0003EEA0 File Offset: 0x0003D0A0
	public void DEBUG_DisableWater()
	{
		if (this.m_waterSurface != null)
		{
			this.m_waterSurface.renderer.enabled = false;
		}
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0003EED0 File Offset: 0x0003D0D0
	public GameMode GetGameModeScript()
	{
		return this.m_gameModeScript;
	}

	// Token: 0x040006EE RID: 1774
	public int m_mapSize = 500;

	// Token: 0x040006EF RID: 1775
	public GameObject m_border500;

	// Token: 0x040006F0 RID: 1776
	public GameObject m_border1000;

	// Token: 0x040006F1 RID: 1777
	public GameObject m_border2000;

	// Token: 0x040006F2 RID: 1778
	public GameObject m_border750;

	// Token: 0x040006F3 RID: 1779
	public GameObject m_waterSurface;

	// Token: 0x040006F4 RID: 1780
	public LevelScript.GameModeScript[] m_gameModes;

	// Token: 0x040006F5 RID: 1781
	public string m_music = "planning";

	// Token: 0x040006F6 RID: 1782
	private GameMode m_gameModeScript;

	// Token: 0x020000DD RID: 221
	[Serializable]
	public class GameModeScript
	{
		// Token: 0x040006F7 RID: 1783
		public GameType m_type;

		// Token: 0x040006F8 RID: 1784
		public GameObject m_gameObject;
	}
}
