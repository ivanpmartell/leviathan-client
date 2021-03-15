using System;
using System.Collections.Generic;
using System.IO;
using PTech;
using UnityEngine;

// Token: 0x02000114 RID: 276
[AddComponentMenu("Scripts/Units/Ship")]
public class Ship : Unit
{
	// Token: 0x06000A55 RID: 2645 RVA: 0x0004BF8C File Offset: 0x0004A18C
	public Ship()
	{
		this.m_Ai = new ShipAi
		{
			m_ship = this
		};
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0004C130 File Offset: 0x0004A330
	public static void RegisterAIStates()
	{
		Ship.m_aiStateFactory.Register<ShipInactive>("inactive");
		Ship.m_aiStateFactory.Register<ShipPatrol>("patrol");
		Ship.m_aiStateFactory.Register<ShipGuard>("guard");
		Ship.m_aiStateFactory.Register<ShipHuman>("human");
		Ship.m_aiStateFactory.Register<ShipFollow>("follow");
		Ship.m_aiStateFactory.Register<ShipThink>("think");
		Ship.m_aiStateFactory.Register<ShipCombat>("combat");
		Ship.m_aiStateFactory.Register<ShipCombat_TurnAndFire>("c_turnandfire");
		Ship.m_aiStateFactory.Register<ShipCombat_DriveBy>("c_driveby");
		Ship.m_aiStateFactory.Register<ShipCombat_ChickenRace>("c_chicken");
		Ship.m_aiStateFactory.Register<ShipCombat_Surround>("c_surround");
		Ship.m_aiStateFactory.Register<ShipAttack>("attack");
		Ship.m_aiStateFactory.Register<ShipBossC1m3>("BossC1M3");
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0004C200 File Offset: 0x0004A400
	public override void Awake()
	{
		base.Awake();
		this.m_realPos = base.transform.position;
		this.m_realRot = base.transform.rotation;
		this.m_sightRayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("blocksight"));
		this.m_shallowRayMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("shallow") | 1 << LayerMask.NameToLayer("beach"));
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = false;
		rigidbody.isKinematic = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		for (int i = 0; i < 4; i++)
		{
			this.m_sections.Add(null);
		}
		this.m_waterSurface = GameObject.Find("WaterSurface").GetComponent<WaterSurface>();
		this.m_damageMan = new DamageMan(base.gameObject);
		this.SaveParticleData(this.m_forwardEmitters, out this.m_forwardEmittersData);
		this.SaveParticleData(this.m_engineParticles, out this.m_engineParticlesData);
		this.SpawnShipTrigger();
		this.ResetAiState();
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x0004C328 File Offset: 0x0004A528
	private void SpawnShipTrigger()
	{
		new GameObject("egg")
		{
			transform = 
			{
				parent = base.gameObject.transform,
				localPosition = new Vector3(0f, 0f, 0f)
			},
			layer = 19
		}.AddComponent("SphereCollider");
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x0004C38C File Offset: 0x0004A58C
	public void GetAllHPModules(ref List<HPModule> modules)
	{
		foreach (Section section in this.m_sections)
		{
			section.GetAllHPModules(ref modules);
		}
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x0004C3F4 File Offset: 0x0004A5F4
	public void ResetAiState()
	{
		this.m_stateMachine = new AIStateMachine<Ship>(this, Ship.m_aiStateFactory);
		this.m_stateMachine.PushState("think");
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0004C418 File Offset: 0x0004A618
	private void SaveParticleData(GameObject[] objects, out Ship.EmitterData[] emitterData)
	{
		emitterData = new Ship.EmitterData[objects.Length];
		for (int i = 0; i < objects.Length; i++)
		{
			Ship.EmitterData emitterData2 = new Ship.EmitterData();
			emitterData[i] = emitterData2;
			emitterData2.m_ps = objects[i].GetComponent<ParticleSystem>();
			emitterData2.m_maxEmission = emitterData2.m_ps.emissionRate;
			emitterData2.m_maxSpeed = emitterData2.m_ps.startSpeed;
			emitterData2.m_ps.emissionRate = 0f;
			emitterData2.m_ps.startSpeed = 0f;
		}
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0004C4A0 File Offset: 0x0004A6A0
	public override void Update()
	{
		base.Update();
		if (NetObj.m_drawOrders)
		{
			this.DrawOrders();
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x0004C4B8 File Offset: 0x0004A6B8
	protected virtual bool SetupLineDrawer()
	{
		if (this.m_lineDrawer == null)
		{
			this.m_lineDrawer = Camera.main.GetComponent<LineDrawer>();
			if (this.m_lineDrawer == null)
			{
				return false;
			}
			this.m_moveOrderLineMaterialID = this.m_lineDrawer.GetTypeID("moveOrder");
			this.m_forwardLineMaterialID = this.m_lineDrawer.GetTypeID("moveForward");
			this.m_forwardCloseLineMaterialID = this.m_lineDrawer.GetTypeID("moveForwardClose");
			this.m_reverseLineMaterialID = this.m_lineDrawer.GetTypeID("moveReverse");
			this.m_reverseCloseLineMaterialID = this.m_lineDrawer.GetTypeID("moveReverseClose");
			this.m_blockedLineMaterialID = this.m_lineDrawer.GetTypeID("moveBlocked");
			DebugUtils.Assert(this.m_forwardLineMaterialID >= 0 && this.m_blockedLineMaterialID >= 0);
		}
		return true;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x0004C5A0 File Offset: 0x0004A7A0
	private void OnGUI()
	{
		if (!NetObj.m_drawOrders)
		{
			return;
		}
		if (base.GetOwner() != NetObj.m_localPlayerID)
		{
			return;
		}
		this.m_route.OnGUI();
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x0004C5CC File Offset: 0x0004A7CC
	private void DrawOrders()
	{
		if (base.GetOwner() != NetObj.m_localPlayerID)
		{
			return;
		}
		if (!this.SetupLineDrawer())
		{
			return;
		}
		if (this.m_orders.Count > 0)
		{
			int predictMaterialID = this.m_forwardLineMaterialID;
			int predictCloseMaterialID = this.m_forwardCloseLineMaterialID;
			if (this.m_route.IsReverse())
			{
				predictMaterialID = this.m_reverseLineMaterialID;
				predictCloseMaterialID = this.m_reverseCloseLineMaterialID;
			}
			float maxTime = 120f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.m_route.Draw(base.transform.position, this.m_lineDrawer, this.m_moveOrderLineMaterialID, predictMaterialID, predictCloseMaterialID, this.m_realPos, this.m_realRot, this.m_velocity, this.m_rotVelocity, maxTime, fixedDeltaTime, this.m_Width, this.m_maxTurnSpeed, this.m_maxSpeed, this.m_maxReverseSpeed, this.m_acceleration, this.m_reverseAcceleration, this.m_breakAcceleration, this.m_forwardFriction, this.m_sideWayFriction, this.m_rotationFriction);
		}
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x0004C6BC File Offset: 0x0004A8BC
	protected override void FixedUpdate()
	{
		this.DebugDrawRoute();
		base.FixedUpdate();
		if (NetObj.m_simulating)
		{
			if (!this.m_dead)
			{
				this.UpdateSink(Time.fixedDeltaTime);
				if (this.m_selfDestruct)
				{
					this.Explode();
				}
				this.UpdateAutoRepair(Time.fixedDeltaTime);
				this.UpdateMaintenance(Time.fixedDeltaTime);
			}
			this.UpdateMotion();
			this.UpdateSpeedParticles();
			this.UpdateShallowTest(Time.fixedDeltaTime);
			this.UpdateMonsterMines(Time.fixedDeltaTime);
			this.m_collisionDamageTimer -= Time.fixedDeltaTime;
			this.m_shallowHitTimer -= Time.fixedDeltaTime;
			this.m_groundedTimer -= Time.fixedDeltaTime;
			this.m_suppliedTimer -= Time.fixedDeltaTime;
			this.m_engineDamagedTimer -= Time.fixedDeltaTime;
			this.m_bridgeDamagedTimer -= Time.fixedDeltaTime;
			this.m_outOfControlTimer -= Time.fixedDeltaTime;
			this.m_damageEffectTimer -= Time.fixedDeltaTime;
			this.m_inMonsterMineTimer -= Time.fixedDeltaTime;
			this.m_stateMachine.Update(Time.fixedDeltaTime);
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x0004C7F0 File Offset: 0x0004A9F0
	protected override void OnSetSimulating(bool enabled)
	{
		base.OnSetSimulating(enabled);
		if (enabled)
		{
			foreach (Ship.EmitterData emitterData in this.m_forwardEmittersData)
			{
				emitterData.m_ps.Play();
			}
		}
		else
		{
			foreach (Ship.EmitterData emitterData2 in this.m_forwardEmittersData)
			{
				emitterData2.m_ps.Pause();
			}
		}
		this.m_damageMan.SetSimulating(enabled);
		foreach (Section section in this.m_sections)
		{
			section.OnSetSimulating(enabled);
		}
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x0004C8D4 File Offset: 0x0004AAD4
	private void UpdateMonsterMines(float dt)
	{
		if (base.IsDead())
		{
			return;
		}
		if (this.m_inMonsterMineTimer <= 0f)
		{
			return;
		}
		this.m_monsterMineDamageTimer -= dt;
		if (this.m_monsterMineDamageTimer >= 0f)
		{
			return;
		}
		this.m_monsterMineDamageTimer = 2f;
		Section section = this.m_sections[PRand.Range(0, 3)];
		section.Damage(new Hit(17, 25));
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0004C94C File Offset: 0x0004AB4C
	private void UpdateShallowTest(float dt)
	{
		if (!this.m_deepKeel)
		{
			return;
		}
		this.m_shallowTestTimer += dt;
		if (this.m_shallowTestTimer > 1f)
		{
			this.m_shallowTestTimer = 0f;
			Vector3 position = base.transform.position;
			if (Physics.Raycast(position, -base.transform.up, 10f, this.m_shallowRayMask))
			{
				this.OnHitShallow();
			}
		}
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0004C9C8 File Offset: 0x0004ABC8
	public bool IsGrounded()
	{
		return this.m_groundedTimer > 0f;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x0004C9D8 File Offset: 0x0004ABD8
	private void OnHitShallow()
	{
		if (NetObj.m_simulating && this.m_shallowHitTimer <= 0f && this.m_groundedTimer <= 0f)
		{
			this.m_shallowHitTimer = 4f;
			if (this.m_velocity.magnitude > 0.1f && (double)PRand.Value() < 0.25)
			{
				if (this.IsVisible())
				{
					HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipGroundedText);
				}
				if (base.GetOwner() == NetObj.m_localPlayerID)
				{
					VOSystem.instance.DoEvent("Grounded");
				}
				float num = PRand.Value();
				this.m_groundedTimer = 1f + num * 9f;
				this.m_velocity *= 0.15f;
				this.m_rotVelocity *= 0.15f;
				Section section = this.m_sections[PRand.Range(0, 3)];
				section.Damage(new Hit((int)((float)this.m_maxHealth * num * 0.2f), section.m_armorClass));
			}
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x0004CB08 File Offset: 0x0004AD08
	private void UpdateSink(float dt)
	{
		if (this.m_dead)
		{
			return;
		}
		if (this.m_sinkTimer >= 0f)
		{
			this.m_sinkTimer -= dt;
			if (this.m_sinkTimer < 0f)
			{
				if (this.IsVisible())
				{
					HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipSinkingText);
				}
				this.Sink();
			}
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0004CB88 File Offset: 0x0004AD88
	private void StartToSink()
	{
		if (this.m_sinkTimer < 0f && this.m_sinking == Ship.SinkStyle.None)
		{
			this.m_sinkTimer = this.m_sinkDelay;
			if (this.IsVisible())
			{
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, this.m_sinkTimer.ToString() + "s", Constants.m_shipSinkingWarningText);
			}
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x0004CBFC File Offset: 0x0004ADFC
	private void Explode()
	{
		foreach (Section section in this.m_sections)
		{
			section.Explode();
		}
		this.Sink();
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x0004CC68 File Offset: 0x0004AE68
	private void Sink()
	{
		this.m_sinking = (Ship.SinkStyle)PRand.Range(1, 4);
		this.OnKilled();
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x0004CC80 File Offset: 0x0004AE80
	protected override void OnKilled()
	{
		if (this.m_dead)
		{
			return;
		}
		base.OnKilled();
		foreach (Section section in this.m_sections)
		{
			section.OnKilled();
		}
		this.ClearOrders();
		this.SetSelected(false, false);
		if (this.m_lastDamageDealer >= 0)
		{
			TurnMan.instance.AddShipsSunk(this.m_lastDamageDealer);
		}
		TurnMan.instance.AddShipsLost(base.GetOwner());
		foreach (Ship.EmitterData emitterData in this.m_engineParticlesData)
		{
			emitterData.m_ps.emissionRate = 0f;
		}
		this.DisableAOPlane();
		if (base.IsKing())
		{
			VOSystem.instance.DoEvent("Flagship sunk");
		}
		else if (base.GetOwner() == NetObj.m_localPlayerID)
		{
			VOSystem.instance.DoEvent("Player ship sunk");
		}
		else if (NetObj.m_localPlayerID != -1)
		{
			if (TurnMan.instance.IsHostile(NetObj.m_localPlayerID, base.GetOwner()))
			{
				VOSystem.instance.DoEvent("Enemy ship sunk");
			}
			else
			{
				VOSystem.instance.DoEvent("Allie ship sunk");
			}
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x0004CDF8 File Offset: 0x0004AFF8
	private void DisableAOPlane()
	{
		Transform transform = base.transform.FindChild("ShipAO");
		if (transform != null)
		{
			transform.gameObject.SetActiveRecursively(false);
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x0004CE30 File Offset: 0x0004B030
	public Section GetSectionFront()
	{
		return this.m_sections[0];
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0004CE40 File Offset: 0x0004B040
	public Section GetSectionMid()
	{
		return this.m_sections[1];
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x0004CE50 File Offset: 0x0004B050
	public Section GetSectionRear()
	{
		return this.m_sections[2];
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x0004CE60 File Offset: 0x0004B060
	public Section GetSectionTop()
	{
		return this.m_sections[3];
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0004CE70 File Offset: 0x0004B070
	public Section GetSection(Section.SectionType stype)
	{
		return this.m_sections[(int)stype];
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0004CE80 File Offset: 0x0004B080
	public override string GetTooltip()
	{
		string text = string.Empty;
		if (base.IsKing())
		{
			text += "KING \n";
		}
		text += this.GetName();
		if (CheatMan.instance.DebugAi())
		{
			text = text + " (" + base.GetNetID().ToString() + ")";
		}
		text += "\n";
		string text2 = text;
		text = string.Concat(new object[]
		{
			text2,
			"Health: ",
			this.m_health,
			"\n"
		});
		if (this.IsTakingWater())
		{
			text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Sinking: ",
				this.m_sinkTimer,
				"s"
			});
		}
		if (CheatMan.instance.DebugAi())
		{
			text += base.GetAi().ToString();
			text += this.m_stateMachine.ToString();
		}
		return text;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0004CF90 File Offset: 0x0004B190
	private void UpdateMotion()
	{
		if (this.m_sinking != Ship.SinkStyle.None)
		{
			Vector3 position = base.rigidbody.position;
			Quaternion rotation = base.rigidbody.rotation;
			position.y -= Time.fixedDeltaTime * 0.5f;
			Vector3 eulerAngles = rotation.eulerAngles;
			switch (this.m_sinking)
			{
			case Ship.SinkStyle.RollLeft:
				eulerAngles.z -= Time.fixedDeltaTime * 4f;
				break;
			case Ship.SinkStyle.RollRight:
				eulerAngles.z += Time.fixedDeltaTime * 4f;
				break;
			case Ship.SinkStyle.RollForward:
				eulerAngles.x += Time.fixedDeltaTime * 3f;
				break;
			case Ship.SinkStyle.RollBackward:
				eulerAngles.x -= Time.fixedDeltaTime * 3f;
				break;
			}
			rotation.eulerAngles = eulerAngles;
			base.rigidbody.MovePosition(position);
			base.rigidbody.MoveRotation(rotation);
			this.m_damageMan.OnSinkingUpdate();
			float num = -20f;
			if (position.y <= num)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if (!this.m_dead)
			{
				this.UpdateEngine();
			}
			Vector3 position2;
			Quaternion rot;
			this.UpdateWaterMotionPos(out position2, out rot);
			this.UpdateRocking(ref rot);
			base.rigidbody.MovePosition(position2);
			base.rigidbody.MoveRotation(rot);
		}
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0004D10C File Offset: 0x0004B30C
	private bool GetRotVelTowards(ref float rotVel, Vector3 dir, bool reverse)
	{
		float y = Quaternion.LookRotation(dir).eulerAngles.y;
		float num = this.m_realRot.eulerAngles.y;
		if (reverse)
		{
			num += 180f;
		}
		float num2 = Mathf.DeltaAngle(num, y);
		float num3 = Mathf.Clamp(num2, -85f, 85f) / 85f;
		rotVel += num3 * 30f * Time.fixedDeltaTime;
		if (Mathf.Abs(rotVel) > this.m_maxTurnSpeed)
		{
			rotVel = ((rotVel <= 0f) ? (-this.m_maxTurnSpeed) : this.m_maxTurnSpeed);
		}
		return Mathf.Abs(num2) < 2f && Mathf.Abs(rotVel) < 1f;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x0004D1E4 File Offset: 0x0004B3E4
	private bool ReachedWP(Vector3 realPos, Vector3 forward, Vector3 right, Vector3 wpPos, float width)
	{
		Vector3 rhs = wpPos - realPos;
		float f = Vector3.Dot(forward, rhs);
		if (Mathf.Abs(f) > 4f)
		{
			return false;
		}
		float f2 = Vector3.Dot(right, rhs);
		return Mathf.Abs(f2) < width / 2f;
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0004D22C File Offset: 0x0004B42C
	private void UpdateEngine()
	{
		float num = Mathf.Max(5f, this.m_length / 2f);
		float num2 = 0f;
		float num3 = this.m_maxSpeed;
		Vector3 vector = this.m_realRot * Vector3.forward;
		Vector3 vector2 = this.m_realRot * Vector3.right;
		float num4 = Vector3.Dot(vector, this.m_velocity);
		if (this.m_shallowHitTimer > 0f)
		{
			num3 *= 0.5f;
		}
		if (this.m_inMonsterMineTimer > 0f)
		{
			num3 *= 0.2f;
		}
		if (this.m_route.NrOfWaypoints() > 0)
		{
			DebugUtils.Assert(this.m_orders.Count > 0);
			Order value = this.m_orders.First.Value;
			Route.Waypoint nextWaypoint = this.m_route.GetNextWaypoint();
			this.m_reverse = nextWaypoint.m_reverse;
			if (!value.HasReachedPosition())
			{
				if (this.ReachedWP(this.m_realPos, vector, vector2, nextWaypoint.m_pos, this.m_Width))
				{
					value.SetReachedPosition(true);
				}
				num2 = this.m_route.GetSpeedFactor(0, this.m_realPos, vector, vector2, this.m_breakAcceleration, this.m_maxTurnSpeed, this.m_maxSpeed, num4);
			}
			bool flag = false;
			if (this.m_outOfControlTimer <= 0f && this.m_engineDamagedTimer <= 0f)
			{
				Vector3 dir = (!value.HasReachedPosition() || !nextWaypoint.m_haveDirection) ? (nextWaypoint.m_pos - this.m_realPos).normalized : nextWaypoint.m_direction;
				flag = this.GetRotVelTowards(ref this.m_rotVelocity, dir, nextWaypoint.m_reverse);
			}
			if (value.HasReachedPosition() && (!nextWaypoint.m_haveDirection || flag))
			{
				this.RemoveFirstOrder();
				if (base.IsOrdersEmpty())
				{
					base.GetAi().m_goalPosition = null;
					base.GetAi().m_goalFacing = null;
				}
			}
		}
		float num5 = (!this.m_reverse) ? (num2 * num3) : (num2 * -this.m_maxReverseSpeed);
		if (this.m_outOfControlTimer > 0f)
		{
			num5 = num4;
		}
		if (this.m_engineDamagedTimer > 0f)
		{
			num5 = 0f;
		}
		if (!this.IsGrounded())
		{
			if (!this.m_reverse)
			{
				if (num5 > num4)
				{
					this.m_velocity += vector * this.m_acceleration * Time.fixedDeltaTime;
				}
				else if (num5 < num4)
				{
					this.m_velocity -= vector * this.m_breakAcceleration * Time.fixedDeltaTime;
				}
			}
			else if (num5 < num4)
			{
				this.m_velocity += vector * -this.m_reverseAcceleration * Time.fixedDeltaTime;
			}
			else if (num5 > num4)
			{
				this.m_velocity -= vector * -this.m_breakAcceleration * Time.fixedDeltaTime;
			}
		}
		Vector3 a = Utils.Project(this.m_velocity, vector);
		Vector3 a2 = Utils.Project(this.m_velocity, vector2);
		this.m_velocity -= a * this.m_forwardFriction;
		this.m_velocity -= a2 * this.m_sideWayFriction;
		this.m_rotVelocity -= this.m_rotVelocity * this.m_rotationFriction;
		this.m_realPos += this.m_velocity * Time.fixedDeltaTime;
		this.m_realRot *= Quaternion.Euler(new Vector3(0f, this.m_rotVelocity * Time.fixedDeltaTime, 0f));
		Utils.NormalizeQuaternion(ref this.m_realRot);
		this.m_velocity.y = 0f;
		this.m_realPos.y = 0f;
		if (this.m_reverse)
		{
			this.UpdateEngineParticles(num4 / -this.m_maxReverseSpeed, true);
		}
		else
		{
			this.UpdateEngineParticles(num4 / this.m_maxSpeed, false);
		}
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0004D694 File Offset: 0x0004B894
	private void UpdateEngineParticles(float enginePower, bool reverse)
	{
		float num = enginePower;
		if (reverse)
		{
			num = -num;
		}
		foreach (Ship.EmitterData emitterData in this.m_engineParticlesData)
		{
			emitterData.m_ps.emissionRate = emitterData.m_maxEmission * enginePower;
			emitterData.m_ps.startSpeed = emitterData.m_maxSpeed * num;
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0004D6F4 File Offset: 0x0004B8F4
	private void UpdateSpeedParticles()
	{
		float num = Vector3.Dot(this.m_velocity, base.transform.forward);
		if (this.m_sinking != Ship.SinkStyle.None)
		{
			num = 0f;
		}
		float num2 = Mathf.Clamp((num - 1f) / 9f, 0f, 1f);
		foreach (Ship.EmitterData emitterData in this.m_forwardEmittersData)
		{
			emitterData.m_ps.emissionRate = num2 * emitterData.m_maxEmission;
			emitterData.m_ps.startSpeed = num2 * emitterData.m_maxSpeed;
		}
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0004D798 File Offset: 0x0004B998
	private void UpdateRoute()
	{
		List<Route.Waypoint> list = new List<Route.Waypoint>();
		foreach (Order order in this.m_orders)
		{
			if (order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward || order.m_type == Order.Type.MoveRotate)
			{
				bool reverse = order.m_type == Order.Type.MoveBackward;
				bool havePosition = order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward;
				list.Add(new Route.Waypoint(order.GetPos(), order.GetFacing(), reverse, havePosition, order.HaveFacing()));
			}
		}
		this.m_route.SetWaypoints(list);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0004D874 File Offset: 0x0004BA74
	public override void OnOrdersChanged()
	{
		this.UpdateRoute();
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0004D87C File Offset: 0x0004BA7C
	private void UpdateRocking(ref Quaternion visRot)
	{
		this.m_rockVelocityZ -= this.m_rockAngleZ * Time.fixedDeltaTime * 10f;
		this.m_rockVelocityZ -= this.m_rockVelocityZ * Time.fixedDeltaTime * 2f;
		this.m_rockAngleZ += this.m_rockVelocityZ * Time.fixedDeltaTime;
		this.m_rockVelocityX -= this.m_rockAngleX * Time.fixedDeltaTime * 10f;
		this.m_rockVelocityX -= this.m_rockVelocityX * Time.fixedDeltaTime * 2f;
		this.m_rockAngleX += this.m_rockVelocityX * Time.fixedDeltaTime;
		this.m_rockAngleX = Mathf.Clamp(this.m_rockAngleX, -this.m_maxRockAngleX, this.m_maxRockAngleX);
		this.m_rockAngleZ = Mathf.Clamp(this.m_rockAngleZ, -this.m_maxRockAngleZ, this.m_maxRockAngleZ);
		visRot *= Quaternion.Euler(new Vector3(this.m_rockAngleX, 0f, this.m_rockAngleZ));
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0004D9A0 File Offset: 0x0004BBA0
	private void UpdateWaterMotionPos(out Vector3 visPos, out Quaternion visRot)
	{
		float num = this.m_Width / 2f;
		float num2 = this.m_length / 2f;
		Vector3[] array = new Vector3[]
		{
			new Vector3(0f, 0f, num2),
			new Vector3(0f, 0f, -num2),
			new Vector3(-num, 0f, 0f),
			new Vector3(num, 0f, 0f)
		};
		Vector3[] array2 = new Vector3[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = this.m_realPos + this.m_realRot * array[i];
		}
		for (int j = 0; j < 4; j++)
		{
			float worldWaveHeight = this.m_waterSurface.GetWorldWaveHeight(array2[j]);
			float num3 = worldWaveHeight - this.m_waterHeight[j];
			if (num3 > 0f)
			{
				this.m_waterVel[j] += Mathf.Abs(num3) * num3 * 5000f * Time.fixedDeltaTime / this.m_mass;
			}
			else
			{
				this.m_waterVel[j] += num3 * 1000f * Time.fixedDeltaTime / this.m_mass;
			}
			this.m_waterVel[j] -= this.m_waterVel[j] * 0.01f;
			this.m_waterHeight[j] += this.m_waterVel[j] * Time.fixedDeltaTime;
			array[j].y = this.m_waterHeight[j];
		}
		Vector3 normalized = (array[0] - array[1]).normalized;
		Vector3 normalized2 = (array[3] - array[2]).normalized;
		Vector3 upwards = Vector3.Cross(normalized, normalized2);
		float y = (array[0].y + array[1].y + array[2].y + array[3].y) / 4f;
		Vector3 vector = new Vector3(this.m_realPos.x, y, this.m_realPos.z);
		Quaternion quaternion = this.m_realRot * Quaternion.LookRotation(normalized, upwards);
		visPos = vector;
		visRot = quaternion;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0004DC64 File Offset: 0x0004BE64
	public override void SaveState(BinaryWriter writer)
	{
		base.SaveState(writer);
		writer.Write(1);
		this.m_stateMachine.Save(writer);
		writer.Write(this.m_realPos.x);
		writer.Write(this.m_realPos.y);
		writer.Write(this.m_realPos.z);
		writer.Write(this.m_realRot.x);
		writer.Write(this.m_realRot.y);
		writer.Write(this.m_realRot.z);
		writer.Write(this.m_realRot.w);
		writer.Write(this.m_reverse);
		writer.Write((int)this.m_sinking);
		writer.Write(this.m_velocity.x);
		writer.Write(this.m_velocity.y);
		writer.Write(this.m_velocity.z);
		writer.Write(this.m_rockAngleX);
		writer.Write(this.m_rockAngleZ);
		writer.Write(this.m_rockVelocityX);
		writer.Write(this.m_rockVelocityZ);
		writer.Write(this.m_rotVelocity);
		writer.Write(this.m_collisionDamageTimer);
		writer.Write(this.m_shallowHitTimer);
		writer.Write(this.m_groundedTimer);
		writer.Write(this.m_sinkTimer);
		writer.Write(this.m_engineDamagedTimer);
		writer.Write(this.m_bridgeDamagedTimer);
		writer.Write(this.m_outOfControlTimer);
		writer.Write(this.m_damageEffectTimer);
		writer.Write(this.m_inMonsterMineTimer);
		writer.Write(this.m_monsterMineDamageTimer);
		writer.Write(this.m_maintenanceMode);
		writer.Write(this.m_maintenanceTimer);
		writer.Write(this.m_maintenanceHealTimer);
		writer.Write((short)this.m_health);
		foreach (float value in this.m_waterHeight)
		{
			writer.Write(value);
		}
		foreach (float value2 in this.m_waterVel)
		{
			writer.Write(value2);
		}
		Trace[] componentsInChildren = base.gameObject.GetComponentsInChildren<Trace>(true);
		foreach (Trace trace in componentsInChildren)
		{
			trace.Save(writer);
		}
		Wake[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Wake>(true);
		foreach (Wake wake in componentsInChildren2)
		{
			wake.Save(writer);
		}
		this.m_damageMan.SaveDamageEffects(writer);
		foreach (Section section in this.m_sections)
		{
			writer.Write(section.gameObject.name);
			section.SaveState(writer);
		}
		this.m_aiSettings.SaveState(writer);
		if (base.GetOwner() >= 4)
		{
			if (this.GetPath() == null)
			{
				writer.Write(0);
			}
			else
			{
				writer.Write(this.GetPath().GetComponent<NetObj>().GetNetID());
			}
			this.m_Ai.SaveState(writer);
			this.SaveOrders(writer);
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x0004DFD8 File Offset: 0x0004C1D8
	public override void LoadState(BinaryReader reader)
	{
		base.LoadState(reader);
		short num = reader.ReadInt16();
		this.m_stateMachine.Load(reader);
		this.m_realPos.x = reader.ReadSingle();
		this.m_realPos.y = reader.ReadSingle();
		this.m_realPos.z = reader.ReadSingle();
		this.m_realRot.x = reader.ReadSingle();
		this.m_realRot.y = reader.ReadSingle();
		this.m_realRot.z = reader.ReadSingle();
		this.m_realRot.w = reader.ReadSingle();
		Utils.NormalizeQuaternion(ref this.m_realRot);
		this.m_reverse = reader.ReadBoolean();
		this.m_sinking = (Ship.SinkStyle)reader.ReadInt32();
		this.m_velocity.x = reader.ReadSingle();
		this.m_velocity.y = reader.ReadSingle();
		this.m_velocity.z = reader.ReadSingle();
		this.m_rockAngleX = reader.ReadSingle();
		this.m_rockAngleZ = reader.ReadSingle();
		this.m_rockVelocityX = reader.ReadSingle();
		this.m_rockVelocityZ = reader.ReadSingle();
		this.m_rotVelocity = reader.ReadSingle();
		this.m_collisionDamageTimer = reader.ReadSingle();
		this.m_shallowHitTimer = reader.ReadSingle();
		this.m_groundedTimer = reader.ReadSingle();
		this.m_sinkTimer = reader.ReadSingle();
		this.m_engineDamagedTimer = reader.ReadSingle();
		this.m_bridgeDamagedTimer = reader.ReadSingle();
		this.m_outOfControlTimer = reader.ReadSingle();
		this.m_damageEffectTimer = reader.ReadSingle();
		this.m_inMonsterMineTimer = reader.ReadSingle();
		this.m_monsterMineDamageTimer = reader.ReadSingle();
		this.m_maintenanceMode = reader.ReadBoolean();
		this.m_maintenanceTimer = reader.ReadSingle();
		this.m_maintenanceHealTimer = reader.ReadSingle();
		this.m_health = (int)reader.ReadInt16();
		for (int i = 0; i < this.m_waterHeight.Length; i++)
		{
			this.m_waterHeight[i] = reader.ReadSingle();
		}
		for (int j = 0; j < this.m_waterVel.Length; j++)
		{
			this.m_waterVel[j] = reader.ReadSingle();
		}
		Trace[] componentsInChildren = base.gameObject.GetComponentsInChildren<Trace>(true);
		foreach (Trace trace in componentsInChildren)
		{
			trace.Load(reader);
		}
		Wake[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Wake>(true);
		foreach (Wake wake in componentsInChildren2)
		{
			wake.Load(reader);
		}
		this.m_damageMan.LoadDamageEffects(reader);
		for (int m = 0; m < this.m_sections.Count; m++)
		{
			string name = reader.ReadString();
			Section section = this.SetSection((Section.SectionType)m, name);
			section.LoadState(reader);
		}
		this.m_aiSettings.LoadState(reader);
		if (base.GetOwner() >= 4)
		{
			this.m_pathNetID = reader.ReadInt32();
			this.m_Ai.LoadState(reader);
			this.LoadOrders(reader);
		}
		if (this.m_dead)
		{
			this.DisableAOPlane();
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0004E300 File Offset: 0x0004C500
	public override void ClearOrders()
	{
		base.ClearOrders();
		foreach (Section section in this.m_sections)
		{
			section.ClearOrders();
		}
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0004E36C File Offset: 0x0004C56C
	public override void SaveOrders(BinaryWriter writer)
	{
		base.SaveOrders(writer);
		writer.Write(this.m_selfDestruct);
		writer.Write(this.m_requestedMaintenanceMode);
		DebugUtils.Assert(this.m_sections.Count == 4);
		foreach (Section section in this.m_sections)
		{
			section.SaveOrders(writer);
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0004E404 File Offset: 0x0004C604
	public override void LoadOrders(BinaryReader reader)
	{
		base.LoadOrders(reader);
		this.m_selfDestruct = reader.ReadBoolean();
		this.m_requestedMaintenanceMode = reader.ReadBoolean();
		DebugUtils.Assert(this.m_sections.Count == 4);
		foreach (Section section in this.m_sections)
		{
			section.LoadOrders(reader);
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0004E49C File Offset: 0x0004C69C
	public bool Damage(Hit hit, Section hitSection)
	{
		if (this.m_health <= 0 || this.m_dead)
		{
			return true;
		}
		if (CheatMan.instance.GetPlayerImmortal() && TurnMan.instance.IsHuman(base.GetOwner()))
		{
			return true;
		}
		if (CheatMan.instance.GetNoDamage())
		{
			return true;
		}
		int num;
		GameRules.HitOutcome hitOutcome = GameRules.CalculateDamage(this.m_health, hitSection.m_armorClass, hit.m_damage, hit.m_armorPiercing, out num);
		if (num > 0 && hit.m_havePoint)
		{
			this.m_damageMan.EnableCloseDamageEffect(hit.m_point, num);
		}
		this.m_health -= num;
		this.m_damageMan.OnShipHealthChanged((float)this.m_health / (float)this.m_maxHealth);
		if (hit.m_dealer != null && num > 0)
		{
			Unit unit = hit.GetUnit();
			Gun gun = hit.GetGun();
			if (unit != null)
			{
				ShipAi shipAi = base.GetAi() as ShipAi;
				shipAi.SetTargetId(unit);
				this.m_lastDamageDealer = unit.GetOwner();
			}
			string gunName = (!(gun != null)) ? string.Empty : gun.name;
			TurnMan.instance.AddPlayerDamage(hit.m_dealer.GetOwner(), num, hit.m_dealer.GetOwnerTeam() == base.GetOwnerTeam(), gunName);
			if (!hit.m_collision && NetObj.m_localPlayerID == hit.m_dealer.GetOwner() && !TurnMan.instance.IsHostile(base.GetOwner(), hit.m_dealer.GetOwner()))
			{
				VOSystem.instance.DoEvent("Friendly fire");
			}
		}
		if (this.IsVisible())
		{
			switch (hitOutcome)
			{
			case GameRules.HitOutcome.CritHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipCriticalHit);
				break;
			case GameRules.HitOutcome.PiercedArmor:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipPiercingHit);
				break;
			case GameRules.HitOutcome.GlancingHit:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, num, Constants.m_shipGlancingHit);
				break;
			case GameRules.HitOutcome.Deflected:
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipDeflectHit);
				break;
			}
			if (this.m_health <= 0)
			{
				HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, this.GetName(), Constants.m_shipDestroyedHit);
			}
		}
		if (this.m_health > 0)
		{
			this.ActivateDamageEffect();
		}
		if (this.m_health <= 0)
		{
			this.Explode();
		}
		if (hit.m_havePoint && hit.m_dir != Vector3.zero)
		{
			float d = (float)num * 4f;
			Vector3 worldDir = hit.m_dir * d;
			this.ApplyImpulse(hit.m_point, worldDir, false);
		}
		if (this.m_onTakenDamage != null)
		{
			this.m_onTakenDamage();
		}
		return num > 0;
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0004E7D4 File Offset: 0x0004C9D4
	private void ActivateDamageEffect()
	{
		if (!this.m_systemFailuresEnabled)
		{
			return;
		}
		if (this.m_damageEffectTimer > 0f)
		{
			return;
		}
		if (this.m_engineDamagedTimer > 0f || this.m_bridgeDamagedTimer > 0f || this.m_outOfControlTimer > 0f || this.m_sinkTimer > 0f)
		{
			return;
		}
		this.m_damageEffectTimer = 1f;
		switch (PRand.Range(0, 4))
		{
		case 0:
			if (this.m_engineDamagedTimer <= 0f && (float)this.m_health < 0.9f * (float)this.m_maxHealth && PRand.Value() < 0.06f)
			{
				this.m_engineDamagedTimer = 15f;
				if (this.IsVisible())
				{
					HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipEngineDamagedText);
				}
				if (base.GetOwner() == NetObj.m_localPlayerID)
				{
					VOSystem.instance.DoEvent("Engine damage");
				}
			}
			break;
		case 1:
			if (this.m_bridgeDamagedTimer <= 0f && (float)this.m_health < 0.75f * (float)this.m_maxHealth && PRand.Value() < 0.07f)
			{
				this.m_bridgeDamagedTimer = 15f;
				if (this.IsVisible())
				{
					HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipBridgeDamagedText);
				}
				if (base.GetOwner() == NetObj.m_localPlayerID)
				{
					VOSystem.instance.DoEvent("Bridge damaged");
				}
			}
			break;
		case 2:
			if (this.m_outOfControlTimer <= 0f && (float)this.m_health < 0.75f * (float)this.m_maxHealth && PRand.Value() < 0.07f)
			{
				this.m_outOfControlTimer = 8f;
				if (this.IsVisible())
				{
					HitText.instance.AddDmgText(base.GetNetID(), base.transform.position, string.Empty, Constants.m_shipOutOfControlText);
				}
				if (base.GetOwner() == NetObj.m_localPlayerID)
				{
					VOSystem.instance.DoEvent("Out of control");
				}
			}
			break;
		case 3:
			if (this.m_sinkTimer <= 0f && (float)this.m_health < 0.35f * (float)this.m_maxHealth && PRand.Value() < 0.08f)
			{
				this.StartToSink();
				if (base.GetOwner() == NetObj.m_localPlayerID)
				{
					VOSystem.instance.DoEvent("Ship sinking");
				}
			}
			break;
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0004EA98 File Offset: 0x0004CC98
	public void SelfDestruct()
	{
		this.m_selfDestruct = true;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0004EAA4 File Offset: 0x0004CCA4
	protected override void ClearGunOrdersAndTargets()
	{
		foreach (Section section in this.m_sections)
		{
			section.ClearGunOrdersAndTargets();
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0004EB0C File Offset: 0x0004CD0C
	public Section SetSection(Section.SectionType type, string name)
	{
		if (this.m_sections[(int)type] != null)
		{
			UnityEngine.Object.Destroy(this.m_sections[(int)type].gameObject);
			this.m_sections[(int)type] = null;
		}
		GameObject gameObject = ObjectFactory.instance.Create(name, base.transform.position, base.transform.rotation);
		if (gameObject == null)
		{
			PLog.LogError("failed to create sect ion " + name + " for ship " + this.GetName());
			return null;
		}
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		Section component = gameObject.GetComponent<Section>();
		if (component.m_series != this.m_series)
		{
			PLog.LogError(string.Concat(new string[]
			{
				"Added section ",
				name,
				" of series ",
				component.m_series,
				" to ship ",
				this.GetName(),
				" of series ",
				this.m_series
			}));
		}
		if (component.GetSectionType() != type)
		{
			PLog.LogError(string.Concat(new object[]
			{
				"Added section ",
				name,
				" of type ",
				component.GetSectionType(),
				" as ",
				type
			}));
		}
		this.m_sections[(int)type] = component;
		component.Setup(this);
		component.SetOwner(base.GetOwner());
		this.UpdateStats();
		if (this.m_shipMaterial == null)
		{
			this.m_shipMaterial = component.GetMaterial();
			this.SetupShipColor();
		}
		component.SetMaterial(this.m_shipMaterial);
		return component;
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0004ECE0 File Offset: 0x0004CEE0
	public override bool TestLOS(NetObj obj)
	{
		if (this.IsInSmoke())
		{
			return false;
		}
		Platform platform = obj as Platform;
		if (platform != null)
		{
			return this.TestLOS(platform);
		}
		Ship ship = obj as Ship;
		if (ship != null)
		{
			return this.TestLOS(ship);
		}
		Projectile projectile = obj as Projectile;
		if (projectile != null)
		{
			return this.TestLOS(projectile);
		}
		MineExplode mineExplode = obj as MineExplode;
		if (mineExplode != null)
		{
			return this.TestLOS(mineExplode);
		}
		Mine mine = obj as Mine;
		return mine != null && this.TestLOS(mine);
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0004ED88 File Offset: 0x0004CF88
	private bool TestLOS(Ship othership)
	{
		if (othership.IsCloaked())
		{
			return false;
		}
		float sightRange = this.GetSightRange();
		float num = Vector3.Distance(base.transform.position, othership.transform.position) - othership.GetLength() / 2f;
		if (num >= sightRange)
		{
			return false;
		}
		Vector3[] viewPoints = this.GetViewPoints();
		Vector3[] viewPoints2 = othership.GetViewPoints();
		foreach (Vector3 vector in viewPoints2)
		{
			if (Vector3.Distance(base.transform.position, vector) <= sightRange)
			{
				foreach (Vector3 start in viewPoints)
				{
					if (!Physics.Linecast(start, vector, this.m_sightRayMask))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0004EE7C File Offset: 0x0004D07C
	public override float GetSightRange()
	{
		if (this.m_bridgeDamagedTimer > 0f)
		{
			return this.m_length / 2f;
		}
		return base.GetSightRange();
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0004EEA4 File Offset: 0x0004D0A4
	private bool TestLOS(Platform platform)
	{
		if (platform.m_alwaysVisible)
		{
			return true;
		}
		if (platform.IsCloaked())
		{
			return false;
		}
		float num = Vector3.Distance(base.transform.position, platform.transform.position);
		return num < this.GetSightRange() && !Physics.Linecast(base.transform.position, platform.transform.position, this.m_sightRayMask);
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0004EF20 File Offset: 0x0004D120
	private bool TestLOS(Projectile projectile)
	{
		float num = Vector3.Distance(base.transform.position, projectile.transform.position);
		return num < this.GetSightRange() && !Physics.Linecast(base.transform.position, projectile.transform.position, this.m_sightRayMask);
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0004EF80 File Offset: 0x0004D180
	private bool TestLOS(MineExplode mine)
	{
		float num = Vector3.Distance(base.transform.position, mine.transform.position) - this.m_length / 2f;
		return num < mine.GetVisibleDistance();
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0004EFC4 File Offset: 0x0004D1C4
	private bool TestLOS(Mine mine)
	{
		Vector3 position = mine.transform.position;
		return this.TestLOS(position);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0004EFE4 File Offset: 0x0004D1E4
	public override bool TestLOS(Vector3 point)
	{
		float num = Vector3.Distance(base.transform.position, point);
		if (num > this.GetSightRange())
		{
			return false;
		}
		Vector3[] viewPoints = this.GetViewPoints();
		foreach (Vector3 start in viewPoints)
		{
			if (!Physics.Linecast(start, point, this.m_sightRayMask))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0004F058 File Offset: 0x0004D258
	public override int GetTotalValue()
	{
		int num = this.m_settings.m_value;
		foreach (Section section in this.m_sections)
		{
			num += section.GetTotalValue();
		}
		return num;
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x0004F0D0 File Offset: 0x0004D2D0
	private void AddSectionTargetPoint(Section.SectionType sectionType, ref List<Vector3> points)
	{
		Section section = this.m_sections[(int)sectionType];
		if (!section.IsInSmoke())
		{
			Vector3 center = section.GetCenter();
			float num = this.m_deckHeight * 0.8f;
			if (center.y < num)
			{
				center.y = num;
			}
			points.Add(center);
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0004F128 File Offset: 0x0004D328
	public override Vector3[] GetTargetPoints()
	{
		List<Vector3> list = new List<Vector3>();
		switch (PRand.Range(0, 4))
		{
		case 0:
			this.AddSectionTargetPoint(Section.SectionType.Front, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Mid, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Rear, ref list);
			break;
		case 1:
			this.AddSectionTargetPoint(Section.SectionType.Rear, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Mid, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Front, ref list);
			break;
		case 2:
			this.AddSectionTargetPoint(Section.SectionType.Mid, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Rear, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Front, ref list);
			break;
		case 3:
			this.AddSectionTargetPoint(Section.SectionType.Mid, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Front, ref list);
			this.AddSectionTargetPoint(Section.SectionType.Rear, ref list);
			break;
		}
		return list.ToArray();
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x0004F1E8 File Offset: 0x0004D3E8
	public override Vector3[] GetViewPoints()
	{
		List<Vector3> list = new List<Vector3>();
		if (!this.m_sections[1].IsInSmoke())
		{
			list.Add(this.m_sections[1].GetCenter());
		}
		if (!this.m_sections[0].IsInSmoke())
		{
			list.Add(this.m_sections[0].GetCenter());
		}
		if (!this.m_sections[2].IsInSmoke())
		{
			list.Add(this.m_sections[2].GetCenter());
		}
		Vector3[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].y < this.m_deckHeight)
			{
				array[i].y = this.m_deckHeight;
			}
		}
		return array;
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x0004F2C8 File Offset: 0x0004D4C8
	public override bool IsInSmoke()
	{
		foreach (Section section in this.m_sections)
		{
			if (!section.IsInSmoke())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x0004F33C File Offset: 0x0004D53C
	public override void SetOwner(int owner)
	{
		base.SetOwner(owner);
		foreach (Section section in this.m_sections)
		{
			if (section != null)
			{
				section.SetOwner(owner);
			}
		}
		this.SetupShipColor();
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x0004F3BC File Offset: 0x0004D5BC
	private void SetupShipColor()
	{
		if (this.m_shipMaterial)
		{
			Color white = Color.white;
			if (TurnMan.instance != null)
			{
				TurnMan.instance.GetPlayerColors(base.GetOwner(), out white);
			}
			this.m_shipMaterial.SetColor("_TeamColor0", white);
		}
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x0004F40C File Offset: 0x0004D60C
	public void SetHighlight(bool enabled)
	{
		if (this.m_shipMaterial)
		{
			this.m_shipMaterial.SetFloat("_Highlight", (!enabled) ? 0f : 0.3f);
		}
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0004F444 File Offset: 0x0004D644
	protected override bool CanMove(Vector3 from, Vector3 to)
	{
		Vector3 normalized = (to - from).normalized;
		int layerMask;
		if (this.m_deepKeel)
		{
			layerMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("shallow") | 1 << LayerMask.NameToLayer("bottom_raytrace_only"));
		}
		else
		{
			layerMask = 1 << LayerMask.NameToLayer("Default");
		}
		float distance = Vector3.Distance(from, to);
		RaycastHit raycastHit;
		return !Physics.SphereCast(from, this.m_Width / 2f, normalized, out raycastHit, distance, layerMask);
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0004F4DC File Offset: 0x0004D6DC
	public void UpdateStats()
	{
		float num = 0f;
		this.m_maxSpeed = this.m_baseSettings.m_speed;
		this.m_maxReverseSpeed = this.m_baseSettings.m_reverseSpeed;
		this.m_maxTurnSpeed = this.m_baseSettings.m_turnSpeed;
		this.m_acceleration = this.m_baseSettings.m_acceleration;
		this.m_reverseAcceleration = this.m_baseSettings.m_reverseAcceleration;
		this.m_breakAcceleration = this.m_baseSettings.m_breakAcceleration;
		num = this.m_baseSettings.m_sightRange;
		this.m_maxHealth = this.m_baseSettings.m_maxHealth;
		foreach (Section section in this.m_sections)
		{
			if (section != null)
			{
				this.m_acceleration += section.m_modifiers.m_acceleration;
				this.m_reverseAcceleration += section.m_modifiers.m_reverseAcceleration;
				this.m_breakAcceleration += section.m_modifiers.m_breakAcceleration;
				this.m_maxTurnSpeed += section.m_modifiers.m_turnSpeed;
				this.m_maxSpeed += section.m_modifiers.m_speed;
				this.m_maxReverseSpeed += section.m_modifiers.m_reverseSpeed;
				num += section.m_modifiers.m_sightRange;
				this.m_maxHealth += section.m_maxHealth;
			}
		}
		base.SetSightRange(num);
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0004F688 File Offset: 0x0004D888
	public void ResetStats()
	{
		this.m_health = this.m_maxHealth;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0004F698 File Offset: 0x0004D898
	public override void SetSelected(bool selected, bool explicitSelected)
	{
		base.SetSelected(selected, explicitSelected);
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0004F6A4 File Offset: 0x0004D8A4
	private void OnCollisionStay(Collision collision)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			vector += contactPoint.normal;
			vector2 += contactPoint.point;
		}
		vector.y = 0f;
		vector.Normalize();
		vector2 /= (float)collision.contacts.Length;
		vector2.y = this.m_realPos.y;
		float num = 99999f;
		float num2 = 0f;
		Vector3 rhs = Vector3.zero;
		Section component = collision.contacts[0].thisCollider.GetComponent<Section>();
		if (!component && collision.contacts[0].thisCollider.transform.parent != null)
		{
			component = collision.contacts[0].thisCollider.transform.parent.GetComponent<Section>();
		}
		Ship component2 = collision.gameObject.GetComponent<Ship>();
		if (component2 != null)
		{
			if (component != null)
			{
				Vector3 lhs = base.transform.position - collision.gameObject.transform.position;
				if (Vector3.Dot(lhs, vector) < 0f)
				{
					vector = -vector;
				}
				num2 = component2.GetMoment(vector2, vector);
				num = component2.m_mass;
				rhs = component2.GetVelAtPoint(vector2) - this.GetVelAtPoint(vector2);
				if (this.m_collisionDamageTimer <= 0f)
				{
					this.m_collisionDamageTimer = 0.2f;
					component.Damage(new Hit(component2, 10, component.m_armorClass, vector2, Vector3.zero)
					{
						m_collision = true
					});
					if (this.IsVisible() && this.m_hitEffect != null)
					{
						UnityEngine.Object.Instantiate(this.m_hitEffect, vector2, Quaternion.identity);
					}
				}
			}
		}
		else if (component != null)
		{
			rhs = -this.GetVelAtPoint(vector2);
			if (this.m_collisionDamageTimer <= 0f)
			{
				this.m_collisionDamageTimer = 0.2f;
				component.Damage(new Hit(10, component.m_armorClass, vector2, Vector3.zero)
				{
					m_collision = true
				});
				if (this.IsVisible() && this.m_hitEffect != null)
				{
					UnityEngine.Object.Instantiate(this.m_hitEffect, vector2, Quaternion.identity);
				}
			}
		}
		float moment = this.GetMoment(vector2, vector);
		float num3 = Vector3.Dot(vector, rhs) / (1f / this.m_mass + 1f / num + (moment + num2));
		if (num3 < 0f)
		{
			num3 *= -1f;
		}
		num3 = Mathf.Clamp(num3, 50f, 200f);
		this.ApplyImpulse(vector2, vector * num3, true);
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0004F9B8 File Offset: 0x0004DBB8
	private Vector3 GetVelAtPoint(Vector3 worldPos)
	{
		Vector3 rhs = worldPos - this.m_realPos;
		Vector3 lhs = new Vector3(0f, this.m_rotVelocity * 0.017453292f, 0f);
		Vector3 b = Vector3.Cross(lhs, rhs);
		return this.m_velocity + b;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0004FA04 File Offset: 0x0004DC04
	private float GetMoment(Vector3 worldPos, Vector3 dir)
	{
		Vector3 lhs = worldPos - this.m_realPos;
		return Vector3.Cross(lhs, dir).magnitude / this.m_mass;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0004FA34 File Offset: 0x0004DC34
	public void ApplyImpulse(Vector3 worldPos, Vector3 worldDir, bool separate)
	{
		Quaternion rotation = Quaternion.Inverse(this.m_realRot);
		Vector3 lhs = rotation * (worldPos - this.m_realPos);
		Vector3 rhs = rotation * worldDir;
		this.m_velocity += worldDir * (1f / this.m_mass);
		if (separate)
		{
			float magnitude = this.m_velocity.magnitude;
			if (magnitude < 0.2f)
			{
				this.m_velocity = this.m_velocity.normalized * 0.2f;
			}
			if (magnitude > 2f)
			{
				this.m_velocity = this.m_velocity.normalized * 2f;
			}
		}
		Vector3 vector = Vector3.Cross(lhs, rhs);
		float num = this.m_mass * this.m_length * this.m_length / 12f;
		float num2 = this.m_mass * this.m_length * this.m_length / 500f;
		float mass = this.m_mass;
		this.m_rotVelocity += vector.y / num * 57.29578f;
		this.m_rockVelocityX += vector.x / num2 * 57.29578f;
		this.m_rockVelocityZ += vector.z / mass * 57.29578f;
		this.m_rockVelocityX = Mathf.Clamp(this.m_rockVelocityX, -this.m_maxRockVel, this.m_maxRockVel);
		this.m_rockVelocityZ = Mathf.Clamp(this.m_rockVelocityZ, -this.m_maxRockVel, this.m_maxRockVel);
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x0004FBC8 File Offset: 0x0004DDC8
	public int GetBatterySlots()
	{
		int num = 0;
		num += this.GetSectionFront().GetBatterySlots();
		num += this.GetSectionMid().GetBatterySlots();
		num += this.GetSectionRear().GetBatterySlots();
		return num + this.GetSectionTop().GetBatterySlots();
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x0004FC10 File Offset: 0x0004DE10
	public override void SetVisible(bool visible)
	{
		if (this.IsVisible() == visible)
		{
			return;
		}
		base.SetVisible(visible);
		Trace[] componentsInChildren = base.gameObject.GetComponentsInChildren<Trace>(true);
		foreach (Trace trace in componentsInChildren)
		{
			trace.SetVisible(visible);
		}
		Wake[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Wake>(true);
		foreach (Wake wake in componentsInChildren2)
		{
			wake.SetVisible(visible);
		}
		for (int k = 0; k < base.transform.childCount; k++)
		{
			Transform child = base.transform.GetChild(k);
			if (child.renderer)
			{
				child.renderer.renderer.enabled = visible;
			}
		}
		foreach (Section section in this.m_sections)
		{
			section.SetVisible(visible);
		}
		if (base.IsKing() && base.GetObjectiveIcon() != null)
		{
			base.GetObjectiveIcon().renderer.enabled = true;
		}
		this.m_damageMan.SetVisible(visible);
		if (this.m_dead)
		{
			this.DisableAOPlane();
		}
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x0004FD94 File Offset: 0x0004DF94
	public override float GetLength()
	{
		return this.m_length;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0004FD9C File Offset: 0x0004DF9C
	public override float GetWidth()
	{
		return this.m_Width;
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x0004FDA4 File Offset: 0x0004DFA4
	public override Vector3 GetVelocity()
	{
		return this.m_velocity;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x0004FDAC File Offset: 0x0004DFAC
	public override bool IsTakingWater()
	{
		return this.m_sinkTimer >= 0f;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x0004FDC0 File Offset: 0x0004DFC0
	public bool IsEngineDamaged()
	{
		return this.m_engineDamagedTimer > 0f;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x0004FDD0 File Offset: 0x0004DFD0
	public bool IsBridgeDamaged()
	{
		return this.m_bridgeDamagedTimer > 0f;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x0004FDE0 File Offset: 0x0004DFE0
	public bool IsOutOfControl()
	{
		return this.m_outOfControlTimer > 0f;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x0004FDF0 File Offset: 0x0004DFF0
	public float GetEngineRepairTime()
	{
		return this.m_engineDamagedTimer;
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x0004FDF8 File Offset: 0x0004DFF8
	public float GetBridgeRepairTime()
	{
		return this.m_bridgeDamagedTimer;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x0004FE00 File Offset: 0x0004E000
	public float GetControlRepairTime()
	{
		return this.m_outOfControlTimer;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x0004FE08 File Offset: 0x0004E008
	public float GetGroundedTime()
	{
		return this.m_groundedTimer;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x0004FE10 File Offset: 0x0004E010
	public float GetTimeToSink()
	{
		return this.m_sinkTimer;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x0004FE18 File Offset: 0x0004E018
	public override bool IsSinking()
	{
		return this.m_sinking != Ship.SinkStyle.None;
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x0004FE28 File Offset: 0x0004E028
	public bool IsSupplied()
	{
		return this.m_suppliedTimer > 0f;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0004FE38 File Offset: 0x0004E038
	public bool IsAutoRepairing()
	{
		return !base.IsDead() && !this.IsTakingWater() && (float)this.m_health < (float)this.m_maxHealth * this.m_autoRepairTreshold;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0004FE74 File Offset: 0x0004E074
	private void UpdateAutoRepair(float dt)
	{
		if (this.IsAutoRepairing())
		{
			this.m_autoRepairTimer += dt;
			if (this.m_autoRepairTimer >= 1f)
			{
				this.m_autoRepairTimer = 0f;
				this.Heal(this.m_autoRepairAmount);
			}
		}
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0004FEC4 File Offset: 0x0004E0C4
	public void SetRequestedMaintenanceMode(bool enabled)
	{
		GameType gameType = TurnMan.instance.GetGameType();
		if (gameType == GameType.Campaign || gameType == GameType.Challenge)
		{
			return;
		}
		if (this.m_king)
		{
			return;
		}
		if (this.m_maintenanceTimer < 0f)
		{
			this.m_requestedMaintenanceMode = enabled;
		}
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x0004FF10 File Offset: 0x0004E110
	public bool GetRequestedMaintenanceMode()
	{
		return this.m_requestedMaintenanceMode;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0004FF18 File Offset: 0x0004E118
	public override bool IsDoingMaintenance()
	{
		return this.m_maintenanceMode || this.m_requestedMaintenanceMode;
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x0004FF30 File Offset: 0x0004E130
	public bool GetCurrentMaintenanceMode()
	{
		return this.m_maintenanceMode;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x0004FF38 File Offset: 0x0004E138
	public float GetMaintenanceTimer()
	{
		if (this.m_maintenanceTimer >= 0f)
		{
			float num = (!this.m_requestedMaintenanceMode) ? 6f : 1f;
			return this.m_maintenanceTimer / num;
		}
		if (this.m_maintenanceMode != this.m_requestedMaintenanceMode)
		{
			return 0f;
		}
		return -1f;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0004FF98 File Offset: 0x0004E198
	private void UpdateMaintenance(float dt)
	{
		if (this.m_maintenanceMode)
		{
			this.m_maintenanceHealTimer -= dt;
			if (this.m_maintenanceHealTimer <= 0f)
			{
				this.m_maintenanceHealTimer = 1f;
				int num = 10000;
				this.Supply(ref num);
				if (num == 10000)
				{
					this.SetRequestedMaintenanceMode(false);
				}
			}
		}
		if (this.m_maintenanceMode != this.m_requestedMaintenanceMode)
		{
			if (this.m_maintenanceTimer < 0f)
			{
				this.m_maintenanceTimer = 0f;
				if (this.m_requestedMaintenanceMode)
				{
					if (this.m_onMaintenanceActivation != null)
					{
						this.m_onMaintenanceActivation();
					}
					this.ClearOrders();
				}
			}
			else
			{
				this.m_maintenanceTimer += dt;
				float num2 = (!this.m_requestedMaintenanceMode) ? 6f : 1f;
				if (this.m_maintenanceTimer >= num2)
				{
					this.m_maintenanceTimer = -1f;
					this.InternalSetMaintenanceMode(this.m_requestedMaintenanceMode);
				}
			}
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x000500A0 File Offset: 0x0004E2A0
	private void InternalSetMaintenanceMode(bool enabled)
	{
		this.m_maintenanceMode = enabled;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x000500AC File Offset: 0x0004E2AC
	public void Heal(int health)
	{
		if (this.m_dead)
		{
			return;
		}
		this.m_health += health;
		if (this.m_health > this.m_maxHealth)
		{
			this.m_health = this.m_maxHealth;
		}
		if (this.m_sinkTimer >= 0f && this.m_health > this.m_sinkTreshold)
		{
			this.m_sinkTimer = -1f;
		}
		this.m_damageMan.HealDamageEffects(health * 2);
		this.m_damageMan.OnShipHealthChanged((float)this.m_health / (float)this.m_maxHealth);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00050144 File Offset: 0x0004E344
	public override void Supply(ref int resources)
	{
		if (this.m_dead)
		{
			return;
		}
		if (resources <= 0)
		{
			return;
		}
		int num = resources;
		if (resources > 12 && this.m_health < this.m_maxHealth)
		{
			resources -= 12;
			this.Heal(10);
		}
		foreach (Section section in this.m_sections)
		{
			section.Supply(ref resources);
		}
		if (resources < num)
		{
			this.m_suppliedTimer = 2.1f;
		}
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00050200 File Offset: 0x0004E400
	public Route GetRoute()
	{
		return this.m_route;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00050208 File Offset: 0x0004E408
	public void SetPath(GameObject path)
	{
		this.m_path = path;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00050214 File Offset: 0x0004E414
	public GameObject GetPath()
	{
		if (this.m_path != null)
		{
			return this.m_path;
		}
		if (this.m_pathNetID == 0)
		{
			return null;
		}
		this.m_path = NetObj.GetByID(this.m_pathNetID).gameObject;
		return this.m_path;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00050264 File Offset: 0x0004E464
	public override int GetHealth()
	{
		return this.m_health;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0005026C File Offset: 0x0004E46C
	public override int GetMaxHealth()
	{
		return this.m_maxHealth;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00050274 File Offset: 0x0004E474
	public string GetClassName()
	{
		return this.m_displayClassName;
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0005027C File Offset: 0x0004E47C
	public Vector3 GetRealPos()
	{
		return this.m_realPos;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x00050284 File Offset: 0x0004E484
	public Quaternion GetRealRot()
	{
		return this.m_realRot;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x0005028C File Offset: 0x0004E48C
	public bool IsWater(Vector3 position)
	{
		NavMeshHit navMeshHit;
		return NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000502B4 File Offset: 0x0004E4B4
	protected bool CanMove2(Vector3 from, Vector3 to)
	{
		Vector3 normalized = (to - from).normalized;
		from += normalized * this.GetLength() * 0.75f;
		from.y += 1f;
		to.y += 1f;
		int layerMask = 256;
		RaycastHit raycastHit;
		bool flag = Physics.Linecast(from, to, out raycastHit, layerMask);
		Debug.DrawLine(from, to, Color.green, 0f, false);
		if (flag)
		{
			PLog.Log("CanMove2 blocked by: " + raycastHit.collider.gameObject.ToString());
		}
		return !flag;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00050364 File Offset: 0x0004E564
	public bool IsPathBlocked()
	{
		Vector3 from = base.transform.position;
		foreach (Order order in this.m_orders)
		{
			if (order.m_type == Order.Type.MoveForward || order.m_type == Order.Type.MoveBackward)
			{
				Vector3 pos = order.GetPos();
				if (!this.CanMove2(from, pos))
				{
					return true;
				}
				from = pos;
			}
		}
		return false;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0005040C File Offset: 0x0004E60C
	public bool IsBlocked()
	{
		Vector3 vector = base.transform.position + base.transform.forward * this.GetLength() * 0.6f;
		Vector3 end = vector + base.transform.forward * this.GetLength();
		int layerMask = 256;
		RaycastHit raycastHit;
		bool result = Physics.Linecast(vector, end, out raycastHit, layerMask);
		Debug.DrawLine(vector, end, Color.yellow, 0f, false);
		return result;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00050490 File Offset: 0x0004E690
	public void SetOrdersTo(Vector3 position)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		NavMesh.CalculatePath(base.transform.position, position, 1, navMeshPath);
		this.ClearMoveOrders();
		for (int i = 0; i < navMeshPath.corners.Length; i++)
		{
			Order order = new Order(this, Order.Type.MoveForward, navMeshPath.corners[i]);
			this.AddOrder(order);
		}
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x000504F8 File Offset: 0x0004E6F8
	public void DebugDrawRoute()
	{
		Route route = this.GetRoute();
		if (route.NrOfWaypoints() == 0)
		{
			return;
		}
		List<Vector3> positions = route.GetPositions();
		Debug.DrawLine(base.transform.position, positions[0], Color.yellow, 0f, false);
		Vector3 b = new Vector3(0f, 2f, 0f);
		for (int i = 0; i < positions.Count - 1; i++)
		{
			Debug.DrawLine(positions[i] + b, positions[i + 1] + b, Color.red, 0f, false);
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0005059C File Offset: 0x0004E79C
	public virtual Dictionary<string, string> GetShipEditorInfo()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Sight Range"] = this.GetSightRange().ToString();
		dictionary["Speed"] = this.GetMaxSpeed().ToString();
		dictionary["Turn Speedua"] = this.GetMaxReverseSpeed().ToString();
		return dictionary;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x000505FC File Offset: 0x0004E7FC
	public ShipAi GetShipAi()
	{
		return this.m_Ai as ShipAi;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0005060C File Offset: 0x0004E80C
	public ShipAISettings GetAiSettings()
	{
		return this.m_aiSettings;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00050614 File Offset: 0x0004E814
	public void SetAiSettings(ShipAISettings aiSetting)
	{
		this.m_aiSettings.Transfer(aiSetting);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00050624 File Offset: 0x0004E824
	public bool IsPositionForward(Vector3 position)
	{
		Plane plane = new Plane(base.transform.forward, base.transform.position);
		float distanceToPoint = plane.GetDistanceToPoint(position);
		return distanceToPoint >= 0f;
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00050668 File Offset: 0x0004E868
	public bool IsPositionRight(Vector3 position)
	{
		Plane plane = new Plane(base.transform.right, base.transform.position);
		float distanceToPoint = plane.GetDistanceToPoint(position);
		return distanceToPoint >= 0f;
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x000506AC File Offset: 0x0004E8AC
	public void SetInMonsterMineField(GameObject monsterMine)
	{
		this.m_monsterMine = monsterMine;
		this.m_inMonsterMineTimer = 1f;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x000506C0 File Offset: 0x0004E8C0
	public float GetMaxSpeed()
	{
		return this.m_maxSpeed;
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x000506C8 File Offset: 0x0004E8C8
	public float GetMaxReverseSpeed()
	{
		return this.m_maxReverseSpeed;
	}

	// Token: 0x040008A2 RID: 2210
	public Texture2D m_icon;

	// Token: 0x040008A3 RID: 2211
	public bool m_editByPlayer = true;

	// Token: 0x040008A4 RID: 2212
	public Ship.BaseSettings m_baseSettings;

	// Token: 0x040008A5 RID: 2213
	public string m_series = string.Empty;

	// Token: 0x040008A6 RID: 2214
	public string m_displayClassName = "Unkown";

	// Token: 0x040008A7 RID: 2215
	public int m_sinkTreshold = 100;

	// Token: 0x040008A8 RID: 2216
	public float m_sinkDelay = 30f;

	// Token: 0x040008A9 RID: 2217
	public bool m_deepKeel;

	// Token: 0x040008AA RID: 2218
	public float m_Width = 7f;

	// Token: 0x040008AB RID: 2219
	public float m_length = 23f;

	// Token: 0x040008AC RID: 2220
	public float m_deckHeight = 2f;

	// Token: 0x040008AD RID: 2221
	public float m_mass = 100f;

	// Token: 0x040008AE RID: 2222
	public int m_autoRepairAmount = 2;

	// Token: 0x040008AF RID: 2223
	public float m_autoRepairTreshold = 0.4f;

	// Token: 0x040008B0 RID: 2224
	public float m_sideWayFriction = 0.015f;

	// Token: 0x040008B1 RID: 2225
	public float m_forwardFriction = 0.001f;

	// Token: 0x040008B2 RID: 2226
	public float m_rotationFriction = 0.01f;

	// Token: 0x040008B3 RID: 2227
	public bool m_systemFailuresEnabled = true;

	// Token: 0x040008B4 RID: 2228
	public GameObject m_hitEffect;

	// Token: 0x040008B5 RID: 2229
	public GameObject[] m_forwardEmitters;

	// Token: 0x040008B6 RID: 2230
	private Ship.EmitterData[] m_forwardEmittersData;

	// Token: 0x040008B7 RID: 2231
	public GameObject[] m_engineParticles;

	// Token: 0x040008B8 RID: 2232
	private Ship.EmitterData[] m_engineParticlesData;

	// Token: 0x040008B9 RID: 2233
	private float m_maxSpeed;

	// Token: 0x040008BA RID: 2234
	private float m_maxReverseSpeed;

	// Token: 0x040008BB RID: 2235
	private float m_acceleration;

	// Token: 0x040008BC RID: 2236
	private float m_reverseAcceleration;

	// Token: 0x040008BD RID: 2237
	private float m_maxTurnSpeed;

	// Token: 0x040008BE RID: 2238
	private float m_breakAcceleration;

	// Token: 0x040008BF RID: 2239
	private int m_maxHealth;

	// Token: 0x040008C0 RID: 2240
	private int m_health;

	// Token: 0x040008C1 RID: 2241
	private bool m_requestedMaintenanceMode;

	// Token: 0x040008C2 RID: 2242
	private bool m_maintenanceMode;

	// Token: 0x040008C3 RID: 2243
	private float m_maintenanceTimer = -1f;

	// Token: 0x040008C4 RID: 2244
	private float m_maintenanceHealTimer;

	// Token: 0x040008C5 RID: 2245
	private Vector3 m_realPos;

	// Token: 0x040008C6 RID: 2246
	private Quaternion m_realRot;

	// Token: 0x040008C7 RID: 2247
	private Vector3 m_velocity;

	// Token: 0x040008C8 RID: 2248
	private float m_rotVelocity;

	// Token: 0x040008C9 RID: 2249
	private float m_rockAngleX;

	// Token: 0x040008CA RID: 2250
	private float m_rockAngleZ;

	// Token: 0x040008CB RID: 2251
	private float m_rockVelocityX;

	// Token: 0x040008CC RID: 2252
	private float m_rockVelocityZ;

	// Token: 0x040008CD RID: 2253
	private float m_maxRockAngleZ = 30f;

	// Token: 0x040008CE RID: 2254
	private float m_maxRockAngleX = 15f;

	// Token: 0x040008CF RID: 2255
	private float m_maxRockVel = 15f;

	// Token: 0x040008D0 RID: 2256
	private bool m_reverse;

	// Token: 0x040008D1 RID: 2257
	private Ship.SinkStyle m_sinking;

	// Token: 0x040008D2 RID: 2258
	private bool m_selfDestruct;

	// Token: 0x040008D3 RID: 2259
	private float m_collisionDamageTimer;

	// Token: 0x040008D4 RID: 2260
	private int m_sightRayMask;

	// Token: 0x040008D5 RID: 2261
	private int m_shallowRayMask;

	// Token: 0x040008D6 RID: 2262
	private Material m_shipMaterial;

	// Token: 0x040008D7 RID: 2263
	protected LineDrawer m_lineDrawer;

	// Token: 0x040008D8 RID: 2264
	protected int m_moveOrderLineMaterialID = -1;

	// Token: 0x040008D9 RID: 2265
	protected int m_forwardLineMaterialID = -1;

	// Token: 0x040008DA RID: 2266
	protected int m_forwardCloseLineMaterialID = -1;

	// Token: 0x040008DB RID: 2267
	protected int m_reverseLineMaterialID = -1;

	// Token: 0x040008DC RID: 2268
	protected int m_reverseCloseLineMaterialID = -1;

	// Token: 0x040008DD RID: 2269
	protected int m_blockedLineMaterialID = -1;

	// Token: 0x040008DE RID: 2270
	private Route m_route = new Route();

	// Token: 0x040008DF RID: 2271
	private List<Section> m_sections = new List<Section>();

	// Token: 0x040008E0 RID: 2272
	private DamageMan m_damageMan;

	// Token: 0x040008E1 RID: 2273
	private WaterSurface m_waterSurface;

	// Token: 0x040008E2 RID: 2274
	private float[] m_waterHeight = new float[4];

	// Token: 0x040008E3 RID: 2275
	private float[] m_waterVel = new float[4];

	// Token: 0x040008E4 RID: 2276
	private float m_shallowTestTimer;

	// Token: 0x040008E5 RID: 2277
	private float m_shallowHitTimer;

	// Token: 0x040008E6 RID: 2278
	private float m_groundedTimer;

	// Token: 0x040008E7 RID: 2279
	private float m_suppliedTimer;

	// Token: 0x040008E8 RID: 2280
	private float m_autoRepairTimer;

	// Token: 0x040008E9 RID: 2281
	private float m_damageEffectTimer;

	// Token: 0x040008EA RID: 2282
	private float m_sinkTimer = -1f;

	// Token: 0x040008EB RID: 2283
	private float m_engineDamagedTimer = -1f;

	// Token: 0x040008EC RID: 2284
	private float m_bridgeDamagedTimer = -1f;

	// Token: 0x040008ED RID: 2285
	private float m_outOfControlTimer = -1f;

	// Token: 0x040008EE RID: 2286
	private float m_inMonsterMineTimer = -1f;

	// Token: 0x040008EF RID: 2287
	private float m_monsterMineDamageTimer = 2f;

	// Token: 0x040008F0 RID: 2288
	public static GenericFactory<AIState<Ship>> m_aiStateFactory = new GenericFactory<AIState<Ship>>();

	// Token: 0x040008F1 RID: 2289
	private AIStateMachine<Ship> m_stateMachine;

	// Token: 0x040008F2 RID: 2290
	public GameObject m_path;

	// Token: 0x040008F3 RID: 2291
	public int m_pathNetID;

	// Token: 0x040008F4 RID: 2292
	private ShipAISettings m_aiSettings = new ShipAISettings();

	// Token: 0x040008F5 RID: 2293
	private GameObject m_monsterMine;

	// Token: 0x040008F6 RID: 2294
	public int m_maxHardpoints = 8;

	// Token: 0x02000115 RID: 277
	private enum SinkStyle
	{
		// Token: 0x040008F8 RID: 2296
		None,
		// Token: 0x040008F9 RID: 2297
		RollLeft,
		// Token: 0x040008FA RID: 2298
		RollRight,
		// Token: 0x040008FB RID: 2299
		RollForward,
		// Token: 0x040008FC RID: 2300
		RollBackward
	}

	// Token: 0x02000116 RID: 278
	[Serializable]
	public class BaseSettings
	{
		// Token: 0x040008FD RID: 2301
		public float m_speed = 10f;

		// Token: 0x040008FE RID: 2302
		public float m_reverseSpeed = 5f;

		// Token: 0x040008FF RID: 2303
		public float m_acceleration = 1f;

		// Token: 0x04000900 RID: 2304
		public float m_reverseAcceleration = 0.5f;

		// Token: 0x04000901 RID: 2305
		public float m_turnSpeed = 10f;

		// Token: 0x04000902 RID: 2306
		public float m_sightRange = 50f;

		// Token: 0x04000903 RID: 2307
		public float m_breakAcceleration = 7f;

		// Token: 0x04000904 RID: 2308
		public int m_maxHealth = 1;
	}

	// Token: 0x02000117 RID: 279
	[Serializable]
	public class EmitterData
	{
		// Token: 0x04000905 RID: 2309
		public ParticleSystem m_ps;

		// Token: 0x04000906 RID: 2310
		public float m_maxEmission;

		// Token: 0x04000907 RID: 2311
		public float m_maxSpeed;
	}
}
