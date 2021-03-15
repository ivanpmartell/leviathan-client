using System;
using UnityEngine;

// Token: 0x020000B6 RID: 182
internal class Constants
{
	// Token: 0x0600068E RID: 1678 RVA: 0x00032194 File Offset: 0x00030394
	// Note: this type is marked as 'beforefieldinit'.
	static Constants()
	{
		bool[] array = new bool[20];
		array[11] = true;
		array[16] = true;
		Constants.m_achivementHidden = array;
		Constants.m_IosDLCProductIDs = new string[]
		{
			"maps_challenge_map1",
			"maps_versus_pack1",
			"unit_elites_pack1",
			"unit_marauders_pack1",
			"unit_commonwealth_pack1"
		};
		Constants.m_IosFreeDlcPacks = new string[]
		{
			"maps_versus_pack2"
		};
		Constants.m_SteamPrePurchaseDlcID = 236310U;
		Constants.m_SteamPrePurchaseDlcPacks = new string[]
		{
			"unit_marauders_pack1"
		};
		Constants.m_Steam_CommonWealth_pack2_DlcID = 245030U;
		Constants.m_Steam_CommonWealth_pack2_DlcPacks = new string[]
		{
			"unit_commonwealth_pack1"
		};
		Constants.m_turnTimeLimits = new double[]
		{
			30.0,
			60.0,
			300.0,
			3600.0,
			86400.0,
			604800.0
		};
	}

	// Token: 0x04000522 RID: 1314
	public const bool m_debugLocalize = false;

	// Token: 0x04000523 RID: 1315
	public const float m_groundedDamagePercentage = 0.2f;

	// Token: 0x04000524 RID: 1316
	public const int m_repairCost = 12;

	// Token: 0x04000525 RID: 1317
	public const int m_repairPerSupply = 10;

	// Token: 0x04000526 RID: 1318
	public const float m_gravity = 5f;

	// Token: 0x04000527 RID: 1319
	public const float m_damageForceMultiplier = 4f;

	// Token: 0x04000528 RID: 1320
	public const float m_maintenanceModeEnterDelay = 1f;

	// Token: 0x04000529 RID: 1321
	public const float m_maintenanceModeExitDelay = 6f;

	// Token: 0x0400052A RID: 1322
	public const float m_sinkHealthThreshold = 0.35f;

	// Token: 0x0400052B RID: 1323
	public const float m_sinkChance = 0.08f;

	// Token: 0x0400052C RID: 1324
	public const float m_engineDamageHealthThreshold = 0.9f;

	// Token: 0x0400052D RID: 1325
	public const float m_engineDamageChance = 0.06f;

	// Token: 0x0400052E RID: 1326
	public const float m_bridgeDamageHealthThreshold = 0.75f;

	// Token: 0x0400052F RID: 1327
	public const float m_bridgeDamageChance = 0.07f;

	// Token: 0x04000530 RID: 1328
	public const float m_outOfControlHealthThreshold = 0.75f;

	// Token: 0x04000531 RID: 1329
	public const float m_outOfControlChance = 0.07f;

	// Token: 0x04000532 RID: 1330
	public const float m_monsterMine_DebuffSpeed = 0.5f;

	// Token: 0x04000533 RID: 1331
	public const float m_monsterMine_Dot = 35f;

	// Token: 0x04000534 RID: 1332
	public const int m_monsterMine_Ap = 25;

	// Token: 0x04000535 RID: 1333
	public const float m_shipyardZoomTime = 0.5f;

	// Token: 0x04000536 RID: 1334
	public const float m_shipyardZoomMin = 20f;

	// Token: 0x04000537 RID: 1335
	public const float m_shipyardZoomMax = 60f;

	// Token: 0x04000538 RID: 1336
	public const float m_shipyardMoveMaxX = 10f;

	// Token: 0x04000539 RID: 1337
	public const float m_shipyardMoveMaxZ = 10f;

	// Token: 0x0400053A RID: 1338
	public const float m_shipyardFleetZoom = 80f;

	// Token: 0x0400053B RID: 1339
	public const int m_maxShipsInFleet = 8;

	// Token: 0x0400053C RID: 1340
	public const int m_maxHardpointsOnShip = 8;

	// Token: 0x0400053D RID: 1341
	public const float m_messageFadeTime = 0.5f;

	// Token: 0x0400053E RID: 1342
	public const float m_messageDisplayTime = 2f;

	// Token: 0x0400053F RID: 1343
	public const float m_messageTimeNewsflash = 4f;

	// Token: 0x04000540 RID: 1344
	public const float m_messageTimeObjective = 4f;

	// Token: 0x04000541 RID: 1345
	public const float m_messageTimeObjectiveDone = 3f;

	// Token: 0x04000542 RID: 1346
	public const float m_messageTimeTurn = 0.6f;

	// Token: 0x04000543 RID: 1347
	public const float m_messageTimeVictory = 4f;

	// Token: 0x04000544 RID: 1348
	public const float m_tooltipShowDelay = 1f;

	// Token: 0x04000545 RID: 1349
	public const float m_tooltipHideDelay = 4f;

	// Token: 0x04000546 RID: 1350
	public const float m_tooltipZ = -80f;

	// Token: 0x04000547 RID: 1351
	public const float m_standardMusicVolume = 0.5f;

	// Token: 0x04000548 RID: 1352
	public const float m_standardSfxVolume = 1f;

	// Token: 0x04000549 RID: 1353
	public const int m_connectTimeout = 10000;

	// Token: 0x0400054A RID: 1354
	public const float m_androidTimeout = 300f;

	// Token: 0x0400054B RID: 1355
	public static string[] m_languages = new string[]
	{
		"english",
		"german"
	};

	// Token: 0x0400054C RID: 1356
	public static readonly Color[] m_coopColors = new Color[]
	{
		new Color(0.98f, 0.31f, 0.04f),
		new Color(0.04f, 0.46f, 0.98f),
		new Color(0.83f, 0.76f, 0.05f),
		new Color(0.18f, 0.67f, 0.16f)
	};

	// Token: 0x0400054D RID: 1357
	public static readonly Color[] m_teamColors1 = new Color[]
	{
		new Color(0.98f, 0.31f, 0.04f),
		new Color(0.96f, 0.12f, 0.14f),
		new Color(0.27f, 0.06f, 0.01f)
	};

	// Token: 0x0400054E RID: 1358
	public static readonly Color[] m_teamColors2 = new Color[]
	{
		new Color(0.03f, 0.6f, 0.99f),
		new Color(0.05f, 0.66f, 0.95f),
		new Color(0f, 0.15f, 0.36f)
	};

	// Token: 0x0400054F RID: 1359
	public static string m_buffColor = "[#00FF00]";

	// Token: 0x04000550 RID: 1360
	public static string m_nerfColor = "[#FF0000]";

	// Token: 0x04000551 RID: 1361
	public static HitTextDef m_shipCriticalHit = new HitTextDef(HitTextDef.FontSize.Medium, new Color(1f, 0f, 0f));

	// Token: 0x04000552 RID: 1362
	public static HitTextDef m_shipGlancingHit = new HitTextDef(HitTextDef.FontSize.Medium, new Color(1f, 1f, 0f));

	// Token: 0x04000553 RID: 1363
	public static HitTextDef m_shipPiercingHit = new HitTextDef(HitTextDef.FontSize.Medium, new Color(1f, 1f, 0f));

	// Token: 0x04000554 RID: 1364
	public static HitTextDef m_shipDeflectHit = new HitTextDef(HitTextDef.FontSize.Medium, new Color(0.8f, 0.8f, 0.8f), "$hittext_shipdeflecthit", string.Empty);

	// Token: 0x04000555 RID: 1365
	public static HitTextDef m_shipDestroyedHit = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 0f, 0f), string.Empty, "$hittext_shipdestroyedhit");

	// Token: 0x04000556 RID: 1366
	public static HitTextDef m_shipGroundedText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 1f), "$hittext_shipgroundedtext", string.Empty);

	// Token: 0x04000557 RID: 1367
	public static HitTextDef m_shipSinkingWarningText = new HitTextDef(HitTextDef.FontSize.Medium, new Color(1f, 1f, 1f), "$hittext_shipsinkingwarningtext ", string.Empty);

	// Token: 0x04000558 RID: 1368
	public static HitTextDef m_shipSinkingText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 0f), "$hittext_shipsinkingtext", string.Empty);

	// Token: 0x04000559 RID: 1369
	public static HitTextDef m_shipOutOfControlText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 0f), "$hittext_outofcontrol", string.Empty);

	// Token: 0x0400055A RID: 1370
	public static HitTextDef m_shipBridgeDamagedText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 0f), "$hittext_bridgedamaged", string.Empty);

	// Token: 0x0400055B RID: 1371
	public static HitTextDef m_shipEngineDamagedText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 0f), "$hittext_enginedamaged", string.Empty);

	// Token: 0x0400055C RID: 1372
	public static HitTextDef m_moduleCriticalHit = new HitTextDef(HitTextDef.FontSize.Small, new Color(1f, 0f, 0f));

	// Token: 0x0400055D RID: 1373
	public static HitTextDef m_moduleGlancingHit = new HitTextDef(HitTextDef.FontSize.Small, new Color(1f, 1f, 0f));

	// Token: 0x0400055E RID: 1374
	public static HitTextDef m_modulePiercingHit = new HitTextDef(HitTextDef.FontSize.Small, new Color(1f, 1f, 0f));

	// Token: 0x0400055F RID: 1375
	public static HitTextDef m_moduleDeflectHit = new HitTextDef(HitTextDef.FontSize.Small, new Color(0.8f, 0.8f, 0.8f), "$hittext_moduledeflecthit", string.Empty);

	// Token: 0x04000560 RID: 1376
	public static HitTextDef m_moduleDisabledHit = new HitTextDef(HitTextDef.FontSize.Medium, new Color(1f, 0f, 0f), string.Empty, "$hittext_moduledisabledhit");

	// Token: 0x04000561 RID: 1377
	public static HitTextDef m_shieldAbsorbedText = new HitTextDef(HitTextDef.FontSize.Small, new Color(0.8f, 0.8f, 0.8f), "$hittext_shieldabsorbedtext", string.Empty);

	// Token: 0x04000562 RID: 1378
	public static HitTextDef m_pointsText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 1f), string.Empty, "$hittext_points");

	// Token: 0x04000563 RID: 1379
	public static HitTextDef m_assassinatedText = new HitTextDef(HitTextDef.FontSize.Large, new Color(1f, 1f, 0f), "$hittext_assassinatedtext", string.Empty);

	// Token: 0x04000564 RID: 1380
	public static readonly Color m_shipYardSize_Valid = new Color(0f, 0.64f, 0.07f);

	// Token: 0x04000565 RID: 1381
	public static readonly Color m_shipYardSize_Invalid = new Color(0.64f, 0.09f, 0f);

	// Token: 0x04000566 RID: 1382
	public static int[] m_achivements = new int[]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		11,
		12,
		13,
		14,
		15,
		16,
		17,
		18,
		19
	};

	// Token: 0x04000567 RID: 1383
	public static bool[] m_achivementHidden;

	// Token: 0x04000568 RID: 1384
	public static string[] m_IosDLCProductIDs;

	// Token: 0x04000569 RID: 1385
	public static string[] m_IosFreeDlcPacks;

	// Token: 0x0400056A RID: 1386
	public static uint m_SteamPrePurchaseDlcID;

	// Token: 0x0400056B RID: 1387
	public static string[] m_SteamPrePurchaseDlcPacks;

	// Token: 0x0400056C RID: 1388
	public static uint m_Steam_CommonWealth_pack2_DlcID;

	// Token: 0x0400056D RID: 1389
	public static string[] m_Steam_CommonWealth_pack2_DlcPacks;

	// Token: 0x0400056E RID: 1390
	public static double[] m_turnTimeLimits;

	// Token: 0x020000B7 RID: 183
	public enum AchivementId
	{
		// Token: 0x04000570 RID: 1392
		Ach_None = -1,
		// Token: 0x04000571 RID: 1393
		Ach_ItFloatsAlright,
		// Token: 0x04000572 RID: 1394
		Ach_ShipWright,
		// Token: 0x04000573 RID: 1395
		Ach_InternationalWaters,
		// Token: 0x04000574 RID: 1396
		Ach_NotForverAlone,
		// Token: 0x04000575 RID: 1397
		Ach_SurvivedTheFury,
		// Token: 0x04000576 RID: 1398
		Ach_StrongMan,
		// Token: 0x04000577 RID: 1399
		Ach_IceCold,
		// Token: 0x04000578 RID: 1400
		Ach_Blended,
		// Token: 0x04000579 RID: 1401
		Ach_NoName,
		// Token: 0x0400057A RID: 1402
		Ach_NippedAtTheHeels,
		// Token: 0x0400057B RID: 1403
		Ach_WinWaves3,
		// Token: 0x0400057C RID: 1404
		Ach_Deconstructed,
		// Token: 0x0400057D RID: 1405
		Ach_AltasCrush,
		// Token: 0x0400057E RID: 1406
		Ach_YouAreTiny,
		// Token: 0x0400057F RID: 1407
		Ach_Trafalgar,
		// Token: 0x04000580 RID: 1408
		Ach_PowerDunk,
		// Token: 0x04000581 RID: 1409
		Ach_ExtremePowerDunk,
		// Token: 0x04000582 RID: 1410
		Ach_HaveOnOnUs,
		// Token: 0x04000583 RID: 1411
		Ach_TheGreatGiant,
		// Token: 0x04000584 RID: 1412
		Ach_YouAreAWizard
	}
}
