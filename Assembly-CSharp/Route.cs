using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class Route
{
	// Token: 0x06000D44 RID: 3396 RVA: 0x0005F450 File Offset: 0x0005D650
	public void SetWaypoints(List<Route.Waypoint> newWPs)
	{
		if (this.m_waypoints.Count != newWPs.Count)
		{
			this.m_waypoints = newWPs;
		}
		else
		{
			for (int i = 0; i < this.m_waypoints.Count; i++)
			{
				this.m_waypoints[i].Update(newWPs[i]);
			}
		}
		this.UpdateLength();
		this.MakeDirtyPath();
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x0005F4C0 File Offset: 0x0005D6C0
	public void PopWaypoint()
	{
		if (this.m_waypoints.Count > 0)
		{
			this.m_waypoints.RemoveAt(0);
		}
		this.MakeDirtyPath();
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x0005F4E8 File Offset: 0x0005D6E8
	private void MakeDirtyPath()
	{
		if (this.m_pathUpdateTimer < 0f)
		{
			this.m_pathUpdateTimer = 0f;
		}
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x0005F508 File Offset: 0x0005D708
	public float GetTotalLength()
	{
		return this.m_totalLength;
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x0005F510 File Offset: 0x0005D710
	public int NrOfWaypoints()
	{
		return this.m_waypoints.Count;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x0005F520 File Offset: 0x0005D720
	public Route.Waypoint GetNextWaypoint()
	{
		if (this.m_waypoints.Count > 0)
		{
			return this.m_waypoints[0];
		}
		return null;
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0005F544 File Offset: 0x0005D744
	private void UpdateLength()
	{
		this.m_totalLength = this.CalculateTotalLength();
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x0005F554 File Offset: 0x0005D754
	public void OnGUI()
	{
		if (!Route.m_drawGui)
		{
			return;
		}
		foreach (Route.Waypoint waypoint in this.m_waypoints)
		{
			if (waypoint.m_havePosition)
			{
				Vector2 vector = Utils.ScreenToGUIPos(Camera.main.WorldToScreenPoint(waypoint.m_pos));
				if (waypoint.m_time < 100f)
				{
					string text;
					if (waypoint.m_time < 10f)
					{
						text = " " + ((int)waypoint.m_time).ToString();
					}
					else
					{
						text = ((int)waypoint.m_time).ToString();
					}
					GUI.Label(new Rect(vector.x - 8f, vector.y - 11f, 100f, 30f), text);
				}
			}
		}
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x0005F66C File Offset: 0x0005D86C
	public void Draw(Vector3 firstPoint, LineDrawer lineDrawer, int materialID, int predictMaterialID, int predictCloseMaterialID, Vector3 initialPos, Quaternion initialRot, Vector3 initialVel, float initialRotVel, float maxTime, float stepSize, float width, float maxTurnSpeed, float maxSpeed, float maxReverseSpeed, float acceleration, float reverseAcceleration, float breakAcceleration, float forwardFriction, float sidewayFriction, float rotationFriction)
	{
		if (this.m_waypoints.Count == 0)
		{
			return;
		}
		if (this.m_pathUpdateTimer >= 0f)
		{
			this.m_pathUpdateTimer -= Time.deltaTime;
			if (this.m_pathUpdateTimer < 0f)
			{
				this.GeneratePredictedPath(initialPos, initialRot, initialVel, initialRotVel, maxTime, stepSize, width, maxTurnSpeed, maxSpeed, maxReverseSpeed, acceleration, reverseAcceleration, breakAcceleration, forwardFriction, sidewayFriction, rotationFriction, lineDrawer);
				this.GenerateLines();
			}
		}
		Vector3 start = firstPoint;
		foreach (Route.Waypoint waypoint in this.m_waypoints)
		{
			Vector3 pos = waypoint.m_pos;
			pos.y += 1f;
			lineDrawer.DrawLine(start, pos, materialID, 0.2f);
			start = pos;
		}
		if (this.m_closePoints != null)
		{
			lineDrawer.DrawLine(this.m_closePoints, predictCloseMaterialID, 3f);
		}
		if (this.m_farPoints != null)
		{
			lineDrawer.DrawLine(this.m_farPoints, predictMaterialID, 3f);
		}
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x0005F7A4 File Offset: 0x0005D9A4
	private void GenerateLines()
	{
		if (this.m_closePoints == null)
		{
			this.m_closePoints = new List<Vector3>();
		}
		else
		{
			this.m_closePoints.Clear();
		}
		if (this.m_farPoints == null)
		{
			this.m_farPoints = new List<Vector3>();
		}
		else
		{
			this.m_farPoints.Clear();
		}
		for (int i = 0; i < this.m_predictedRoute.Count; i += 10)
		{
			Route.PathNode pathNode = this.m_predictedRoute[i];
			if (pathNode.time < 10f)
			{
				this.m_closePoints.Add(pathNode.point);
			}
			else
			{
				if (this.m_farPoints.Count == 0)
				{
					this.m_closePoints.Add(pathNode.point);
				}
				this.m_farPoints.Add(pathNode.point);
			}
		}
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x0005F884 File Offset: 0x0005DA84
	private bool GetRotVelTowards(ref float rotVel, Vector3 dir, bool reverse, Quaternion realRot, float maxTurnSpeed)
	{
		float y = Quaternion.LookRotation(dir).eulerAngles.y;
		float num = realRot.eulerAngles.y;
		if (reverse)
		{
			num += 180f;
		}
		float num2 = Mathf.DeltaAngle(num, y);
		float num3 = Mathf.Clamp(num2, -85f, 85f) / 85f;
		rotVel += num3 * 40f * Time.fixedDeltaTime;
		if (Mathf.Abs(rotVel) > maxTurnSpeed)
		{
			rotVel = ((rotVel <= 0f) ? (-maxTurnSpeed) : maxTurnSpeed);
		}
		return Mathf.Abs(num2) < 2f && Mathf.Abs(rotVel) < 1f;
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x0005F94C File Offset: 0x0005DB4C
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

	// Token: 0x06000D50 RID: 3408 RVA: 0x0005F994 File Offset: 0x0005DB94
	private void GeneratePredictedPath(Vector3 initialPos, Quaternion initialRot, Vector3 initialVel, float initialRotVel, float maxTime, float stepSize, float width, float maxTurnSpeed, float maxSpeed, float maxReverseSpeed, float acceleration, float reverseAcceleration, float breakAcceleration, float forwardFriction, float sideWayFriction, float rotationFriction, LineDrawer lineDrawer)
	{
		if (this.m_predictedRoute != null)
		{
			this.m_predictedRoute.Clear();
		}
		else
		{
			this.m_predictedRoute = new List<Route.PathNode>();
		}
		int count = this.m_waypoints.Count;
		int num = 0;
		float num2 = 0f;
		Vector3 vector = initialPos;
		Quaternion quaternion = initialRot;
		Vector3 vector2 = initialVel;
		float num3 = initialRotVel;
		bool flag = false;
		while (num < count && num2 < maxTime)
		{
			Vector3 vector3 = quaternion * Vector3.forward;
			Vector3 vector4 = quaternion * Vector3.right;
			float num4 = Vector3.Dot(vector3, vector2);
			Route.Waypoint waypoint = this.m_waypoints[num];
			bool reverse = waypoint.m_reverse;
			float num5 = 0f;
			if (waypoint.m_havePosition)
			{
				if (!flag)
				{
					if (this.ReachedWP(vector, vector3, vector4, waypoint.m_pos, width))
					{
						flag = true;
					}
					float speedFactor = this.GetSpeedFactor(num, vector, vector3, vector4, breakAcceleration, maxTurnSpeed, maxSpeed, num4);
					num5 = ((!reverse) ? (speedFactor * maxSpeed) : (speedFactor * -maxReverseSpeed));
				}
			}
			else
			{
				flag = true;
			}
			Vector3 dir = (!flag || !waypoint.m_haveDirection) ? (waypoint.m_pos - vector).normalized : waypoint.m_direction;
			bool rotVelTowards = this.GetRotVelTowards(ref num3, dir, waypoint.m_reverse, quaternion, maxTurnSpeed);
			if (!reverse)
			{
				if (num5 > num4)
				{
					vector2 += vector3 * acceleration * stepSize;
				}
				else if (num5 < num4)
				{
					vector2 -= vector3 * breakAcceleration * stepSize;
				}
			}
			else if (num5 < num4)
			{
				vector2 += vector3 * -reverseAcceleration * stepSize;
			}
			else if (num5 > num4)
			{
				vector2 -= vector3 * -breakAcceleration * stepSize;
			}
			Vector3 a = Utils.Project(vector2, vector3);
			Vector3 a2 = Utils.Project(vector2, vector4);
			vector2 -= a * forwardFriction;
			vector2 -= a2 * sideWayFriction;
			num3 -= num3 * rotationFriction;
			vector += vector2 * stepSize;
			quaternion *= Quaternion.Euler(new Vector3(0f, num3 * stepSize, 0f));
			Utils.NormalizeQuaternion(ref quaternion);
			vector2.y = 0f;
			vector.y = 0f;
			if (flag && (!waypoint.m_haveDirection || rotVelTowards))
			{
				waypoint.m_time = num2;
				waypoint.m_speed = vector2.magnitude;
				num++;
				flag = false;
			}
			Route.PathNode item = default(Route.PathNode);
			item.point = vector;
			item.point.y = item.point.y + 0.2f;
			item.time = num2;
			this.m_predictedRoute.Add(item);
			num2 += stepSize;
		}
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x0005FCAC File Offset: 0x0005DEAC
	private float CalculateTotalLength()
	{
		float num = 0f;
		for (int i = 0; i < this.m_waypoints.Count - 1; i++)
		{
			float num2 = Vector3.Distance(this.m_waypoints[i].m_pos, this.m_waypoints[i + 1].m_pos);
			num += num2;
		}
		return num;
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x0005FD0C File Offset: 0x0005DF0C
	private float GetTurnTime(Vector3 currentDir, Vector3 targetDir, float maxTurnSpeed, bool reverse)
	{
		if (reverse)
		{
			currentDir = -currentDir;
		}
		float num = Vector3.Angle(currentDir, targetDir);
		return num / maxTurnSpeed;
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x0005FD38 File Offset: 0x0005DF38
	private float GetEta(float distance, float forwardSpeed, bool reverse)
	{
		if (reverse)
		{
			return (forwardSpeed >= 0f) ? 0f : (distance / -forwardSpeed);
		}
		return (forwardSpeed <= 0f) ? 0f : (distance / forwardSpeed);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x0005FD80 File Offset: 0x0005DF80
	public List<Vector3> GetPositions()
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.m_waypoints.Count; i++)
		{
			list.Add(this.m_waypoints[i].m_pos);
		}
		return list;
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x0005FDC8 File Offset: 0x0005DFC8
	public float GetSpeedFactor(int startWP, Vector3 currentPos, Vector3 currentDir, Vector3 right, float breakAcceleration, float maxTurnSpeed, float maxForwardSpeed, float currentForwardSpeed)
	{
		if (Mathf.Abs(currentForwardSpeed) < 0.5f)
		{
			return 1f;
		}
		bool reverse = this.m_waypoints[0].m_reverse;
		float num = (!reverse) ? currentForwardSpeed : (-currentForwardSpeed);
		Vector3 normalized = (this.m_waypoints[startWP].m_pos - currentPos).normalized;
		float distance = Vector3.Distance(currentPos, this.m_waypoints[startWP].m_pos);
		float turnTime = this.GetTurnTime(currentDir, normalized, maxTurnSpeed, reverse);
		float eta = this.GetEta(distance, currentForwardSpeed, reverse);
		if (this.IsPointInsideTurnRadius(num + 1f, maxTurnSpeed * 0.5f, currentPos, right, this.m_waypoints[startWP].m_pos, null))
		{
			return 0f;
		}
		Vector3 a = currentPos;
		float num2 = 0f;
		for (int i = startWP; i < this.m_waypoints.Count; i++)
		{
			Route.Waypoint waypoint = this.m_waypoints[i];
			num2 += Vector3.Distance(a, waypoint.m_pos);
			if (i == this.m_waypoints.Count - 1)
			{
				float eta2 = this.GetEta(num2, currentForwardSpeed, reverse);
				float num3 = Mathf.Sqrt(num2 / breakAcceleration);
				if (num3 > eta2)
				{
					return 0f;
				}
			}
			else
			{
				Route.Waypoint waypoint2 = this.m_waypoints[i + 1];
				Vector3 normalized2 = (waypoint2.m_pos - waypoint.m_pos).normalized;
				Vector3 right2 = new Vector3(normalized2.z, 0f, -normalized2.x);
				if (this.IsPointInsideTurnRadius(num + 1f, maxTurnSpeed * 0.5f, waypoint.m_pos, right2, waypoint2.m_pos, null))
				{
					return 0f;
				}
			}
			a = waypoint.m_pos;
		}
		return 1f;
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x0005FFAC File Offset: 0x0005E1AC
	private bool IsPointInsideTurnRadius(float vel, float turnVel, Vector3 center, Vector3 right, Vector3 point, LineDrawer lineDrawer)
	{
		float num = this.TurnDiameter(vel, turnVel);
		float num2 = num;
		Vector3 vector = center + right * num2;
		Vector3 vector2 = center - right * num2;
		if (lineDrawer != null)
		{
			lineDrawer.DrawXZCircle(vector, num2, 30, 1, 0.1f);
			lineDrawer.DrawXZCircle(vector2, num2, 30, 2, 0.1f);
		}
		return Vector3.Distance(vector, point) < num2 || Vector3.Distance(vector2, point) < num2;
	}

	// Token: 0x06000D57 RID: 3415 RVA: 0x00060034 File Offset: 0x0005E234
	public bool IsReverse()
	{
		return this.m_waypoints.Count != 0 && this.m_waypoints[0].m_reverse;
	}

	// Token: 0x06000D58 RID: 3416 RVA: 0x0006005C File Offset: 0x0005E25C
	private float TurnDiameter(float vel, float maxTurnSpeed)
	{
		float f = 0.017453292f * maxTurnSpeed;
		return vel / Mathf.Tan(f);
	}

	// Token: 0x04000AF3 RID: 2803
	private List<Route.Waypoint> m_waypoints = new List<Route.Waypoint>();

	// Token: 0x04000AF4 RID: 2804
	private float m_totalLength;

	// Token: 0x04000AF5 RID: 2805
	public static bool m_drawGui = true;

	// Token: 0x04000AF6 RID: 2806
	private float m_pathUpdateTimer = -1f;

	// Token: 0x04000AF7 RID: 2807
	private List<Route.PathNode> m_predictedRoute;

	// Token: 0x04000AF8 RID: 2808
	private List<Vector3> m_closePoints;

	// Token: 0x04000AF9 RID: 2809
	private List<Vector3> m_farPoints;

	// Token: 0x02000164 RID: 356
	public class Waypoint
	{
		// Token: 0x06000D59 RID: 3417 RVA: 0x0006007C File Offset: 0x0005E27C
		public Waypoint(Vector3 point, Vector3 direction, bool reverse, bool havePosition, bool haveDirection)
		{
			this.m_pos = point;
			this.m_direction = direction;
			this.m_reverse = reverse;
			this.m_haveDirection = haveDirection;
			this.m_havePosition = havePosition;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x000600AC File Offset: 0x0005E2AC
		public void Update(Route.Waypoint wp)
		{
			this.m_pos = wp.m_pos;
			this.m_direction = wp.m_direction;
			this.m_haveDirection = wp.m_haveDirection;
			this.m_havePosition = wp.m_havePosition;
			this.m_reverse = wp.m_reverse;
		}

		// Token: 0x04000AFA RID: 2810
		public Vector3 m_pos;

		// Token: 0x04000AFB RID: 2811
		public Vector3 m_direction;

		// Token: 0x04000AFC RID: 2812
		public bool m_havePosition;

		// Token: 0x04000AFD RID: 2813
		public bool m_haveDirection;

		// Token: 0x04000AFE RID: 2814
		public bool m_reverse;

		// Token: 0x04000AFF RID: 2815
		public float m_time;

		// Token: 0x04000B00 RID: 2816
		public float m_speed;
	}

	// Token: 0x02000165 RID: 357
	private struct PathNode
	{
		// Token: 0x04000B01 RID: 2817
		public Vector3 point;

		// Token: 0x04000B02 RID: 2818
		public float time;
	}
}
