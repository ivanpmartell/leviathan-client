using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

// Token: 0x0200018B RID: 395
internal class Utils
{
	// Token: 0x06000EBA RID: 3770 RVA: 0x00066D48 File Offset: 0x00064F48
	public static long GetTimeMS()
	{
		return DateTime.Now.Ticks / 10000L;
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00066D6C File Offset: 0x00064F6C
	public static bool IsEmailAddress(string email)
	{
		if (email.Contains("..") || email.Contains("...") || email.Contains("...."))
		{
			return false;
		}
		Regex regex = new Regex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$");
		return regex.IsMatch(email);
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x00066DC0 File Offset: 0x00064FC0
	public static List<string> GetDistinctList(List<string> data)
	{
		HashSet<string> collection = new HashSet<string>(data);
		return new List<string>(collection);
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00066DDC File Offset: 0x00064FDC
	public static List<int> GetDistinctList(List<int> data)
	{
		HashSet<int> collection = new HashSet<int>(data);
		return new List<int>(collection);
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x00066DF8 File Offset: 0x00064FF8
	public static Utils.ValidationStatus IsValidUsername(string name)
	{
		if (name.Length < 3 || name.Length > 12)
		{
			return Utils.ValidationStatus.ToShort;
		}
		List<char> list = new List<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_".ToCharArray());
		foreach (char item in name)
		{
			if (!list.Contains(item))
			{
				return Utils.ValidationStatus.InvalidCharacter;
			}
		}
		return Utils.ValidationStatus.Ok;
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x00066E60 File Offset: 0x00065060
	public static Utils.ValidationStatus IsValidPassword(string name)
	{
		if (name.Length < 6)
		{
			return Utils.ValidationStatus.ToShort;
		}
		List<char> list = new List<char>(" abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_#%&$()/?!@+,.=".ToCharArray());
		foreach (char item in name)
		{
			if (!list.Contains(item))
			{
				return Utils.ValidationStatus.InvalidCharacter;
			}
		}
		return Utils.ValidationStatus.Ok;
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x00066EBC File Offset: 0x000650BC
	public static string FormatTimeLeftString(double time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		if (timeSpan.Days > 0)
		{
			if (timeSpan.Days == 1)
			{
				return timeSpan.Days + " $commit_timer_day ";
			}
			return timeSpan.Days + " $commit_timer_days ";
		}
		else if (timeSpan.Hours > 0)
		{
			if (timeSpan.Hours == 1)
			{
				return timeSpan.Hours + " $commit_timer_hour ";
			}
			return timeSpan.Hours + " $commit_timer_hours ";
		}
		else if (timeSpan.Minutes > 0)
		{
			if (timeSpan.Minutes == 1)
			{
				return timeSpan.Minutes + " $commit_timer_minute ";
			}
			return timeSpan.Minutes + " $commit_timer_minutes ";
		}
		else
		{
			if (timeSpan.Seconds <= 0)
			{
				return string.Empty;
			}
			if (timeSpan.Seconds == 1)
			{
				return timeSpan.Seconds + " $commit_timer_second ";
			}
			return timeSpan.Seconds + " $commit_timer_seconds ";
		}
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x00066FF8 File Offset: 0x000651F8
	public static void WriteVector3(BinaryWriter writer, Vector3 vector)
	{
		writer.Write(vector.x);
		writer.Write(vector.y);
		writer.Write(vector.z);
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00067024 File Offset: 0x00065224
	public static Vector3 ReadVector3(BinaryReader reader)
	{
		return new Vector3
		{
			x = reader.ReadSingle(),
			y = reader.ReadSingle(),
			z = reader.ReadSingle()
		};
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00067064 File Offset: 0x00065264
	public static void WriteVector3Nullable(BinaryWriter writer, Vector3? vector)
	{
		if (vector == null)
		{
			writer.Write(false);
		}
		else
		{
			writer.Write(true);
			Utils.WriteVector3(writer, vector.Value);
		}
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x000670A0 File Offset: 0x000652A0
	public static void ReadVector3Nullable(BinaryReader reader, out Vector3? vector)
	{
		vector = null;
		bool flag = reader.ReadBoolean();
		if (flag)
		{
			vector = new Vector3?(Utils.ReadVector3(reader));
		}
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x000670DC File Offset: 0x000652DC
	public static void WriteVector3Nullable(BinaryWriter writer, Vector3 vector)
	{
		writer.Write(true);
		Utils.WriteVector3(writer, vector);
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x000670EC File Offset: 0x000652EC
	private static string GetFileName(string name)
	{
		return name;
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x000670F0 File Offset: 0x000652F0
	public static void DumpMemoryUsage()
	{
		PLog.Log("Dumping memory usage to " + Directory.GetCurrentDirectory() + "memory_*.csv");
		string text = string.Empty;
		int num = 0;
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
		foreach (UnityEngine.Object @object in array)
		{
			int num2 = Profiler.GetRuntimeMemorySize(@object) / 1024;
			if (num2 > 0)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					@object.GetType().ToString(),
					";",
					@object.name,
					";",
					num2,
					";\n"
				});
				num += num2;
			}
		}
		text = text + "TOTAL MEMORY;" + num.ToString() + "\n";
		File.WriteAllText(Utils.GetFileName("memory_all.csv"), text);
		num = 0;
		text = string.Empty;
		UnityEngine.Object[] array3 = Resources.FindObjectsOfTypeAll(typeof(Texture));
		foreach (Texture texture in array3)
		{
			string text3 = texture.width.ToString() + "x" + texture.height.ToString();
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				texture.name,
				";",
				Profiler.GetRuntimeMemorySize(texture),
				";",
				text3,
				"\n"
			});
			num += Profiler.GetRuntimeMemorySize(texture);
		}
		text = text + "TOTAL MEMORY;" + num.ToString() + "\n";
		File.WriteAllText(Utils.GetFileName("memory_texture.csv"), text);
		num = 0;
		text = string.Empty;
		UnityEngine.Object[] array5 = Resources.FindObjectsOfTypeAll(typeof(Mesh));
		foreach (Mesh mesh in array5)
		{
			int num3 = mesh.triangles.Length / 3;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				mesh.name,
				";",
				Profiler.GetRuntimeMemorySize(mesh),
				";",
				num3.ToString(),
				"\n"
			});
			num += Profiler.GetRuntimeMemorySize(mesh);
		}
		text = text + "TOTAL MEMORY;" + num.ToString() + "\n";
		File.WriteAllText(Utils.GetFileName("memory_mesh.csv"), text);
		num = 0;
		text = string.Empty;
		UnityEngine.Object[] array7 = Resources.FindObjectsOfTypeAll(typeof(AudioClip));
		foreach (AudioClip audioClip in array7)
		{
			string text4 = "L: " + audioClip.length.ToString() + " Freq: " + audioClip.frequency.ToString();
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				audioClip.name,
				";",
				Profiler.GetRuntimeMemorySize(audioClip),
				";",
				text4,
				"\n"
			});
			num += Profiler.GetRuntimeMemorySize(audioClip);
		}
		text = text + "TOTAL MEMORY;" + num.ToString() + "\n";
		File.WriteAllText(Utils.GetFileName("memory_audio.csv"), text);
	}

	// Token: 0x06000EC8 RID: 3784 RVA: 0x00067490 File Offset: 0x00065690
	public static XmlDocument[] LoadXmlInDirectory(string dir)
	{
		UnityEngine.Object[] array = Resources.LoadAll(dir, typeof(TextAsset));
		XmlDocument[] array2 = new XmlDocument[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			TextAsset xmlText = array[i] as TextAsset;
			array2[i] = Utils.LoadXml(xmlText);
		}
		return array2;
	}

	// Token: 0x06000EC9 RID: 3785 RVA: 0x000674E0 File Offset: 0x000656E0
	public static XmlDocument LoadXml(string file)
	{
		TextAsset textAsset = Resources.Load(file) as TextAsset;
		if (textAsset == null)
		{
			return null;
		}
		return Utils.LoadXml(textAsset);
	}

	// Token: 0x06000ECA RID: 3786 RVA: 0x00067510 File Offset: 0x00065710
	private static XmlDocument LoadXml(TextAsset xmlText)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.IgnoreComments = true;
		XmlReader xmlReader = XmlReader.Create(new StringReader(xmlText.text), xmlReaderSettings);
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.Load(xmlReader);
		}
		catch (XmlException ex)
		{
			PLog.LogError("Parse error " + ex.ToString());
			return null;
		}
		return xmlDocument;
	}

	// Token: 0x06000ECB RID: 3787 RVA: 0x00067594 File Offset: 0x00065794
	public static PlatformType GetPlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
			return PlatformType.Osx;
		case RuntimePlatform.OSXPlayer:
			return PlatformType.Osx;
		case RuntimePlatform.WindowsPlayer:
			return PlatformType.WindowsPC;
		case RuntimePlatform.OSXWebPlayer:
			return PlatformType.Osx;
		case RuntimePlatform.WindowsWebPlayer:
			return PlatformType.WindowsPC;
		case RuntimePlatform.WindowsEditor:
			return PlatformType.WindowsPC;
		case RuntimePlatform.IPhonePlayer:
			return PlatformType.Ios;
		case RuntimePlatform.Android:
			return PlatformType.Android;
		}
		return PlatformType.Other;
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x000675F4 File Offset: 0x000657F4
	public static void UpdateAndroidBack()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		Utils.m_androidBack = false;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Utils.m_androidBack = true;
		}
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0006761C File Offset: 0x0006581C
	public static bool IsAndroidBack()
	{
		return Application.platform == RuntimePlatform.Android && !BtnClickOnEscape.IsAnyButtonsActive() && Utils.m_androidBack;
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x00067640 File Offset: 0x00065840
	public static bool AndroidBack()
	{
		if (Utils.m_androidBack)
		{
			Utils.m_androidBack = false;
			return true;
		}
		return false;
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x00067658 File Offset: 0x00065858
	public static int GetMinPow2(int val)
	{
		if (val <= 1)
		{
			return 1;
		}
		if (val <= 2)
		{
			return 2;
		}
		if (val <= 4)
		{
			return 4;
		}
		if (val <= 8)
		{
			return 8;
		}
		if (val <= 16)
		{
			return 16;
		}
		if (val <= 32)
		{
			return 32;
		}
		if (val <= 64)
		{
			return 64;
		}
		if (val <= 128)
		{
			return 128;
		}
		if (val <= 256)
		{
			return 256;
		}
		if (val <= 512)
		{
			return 512;
		}
		if (val <= 1024)
		{
			return 1024;
		}
		if (val <= 2048)
		{
			return 2048;
		}
		if (val <= 4096)
		{
			return 4096;
		}
		return 1;
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x00067714 File Offset: 0x00065914
	public static void NormalizeQuaternion(ref Quaternion q)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			num += q[i] * q[i];
		}
		float num2 = 1f / Mathf.Sqrt(num);
		for (int j = 0; j < 4; j++)
		{
			ref Quaternion ptr = ref q;
			int index2;
			int index = index2 = j;
			float num3 = ptr[index2];
			q[index] = num3 * num2;
		}
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00067788 File Offset: 0x00065988
	public static Vector3 Project(Vector3 v, Vector3 onTo)
	{
		float d = Vector3.Dot(onTo, v);
		return onTo * d;
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x000677A4 File Offset: 0x000659A4
	public static float DistanceXZ(Vector3 v0, Vector3 v1)
	{
		Vector2 vector = new Vector2(v1.x - v0.x, v1.z - v0.z);
		return vector.magnitude;
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x000677E0 File Offset: 0x000659E0
	public static Vector3 Bezier2(Vector3 Start, Vector3 Control, Vector3 End, float delta)
	{
		return (1f - delta) * (1f - delta) * Start + 2f * delta * (1f - delta) * Control + delta * delta * End;
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x00067820 File Offset: 0x00065A20
	public static float FixDegAngle(float p_Angle)
	{
		while (p_Angle >= 360f)
		{
			p_Angle -= 360f;
		}
		while (p_Angle < 0f)
		{
			p_Angle += 360f;
		}
		return p_Angle;
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x00067858 File Offset: 0x00065A58
	public static float DegDistance(float p_a, float p_b)
	{
		if (p_a == p_b)
		{
			return 0f;
		}
		p_a = Utils.FixDegAngle(p_a);
		p_b = Utils.FixDegAngle(p_b);
		float f = p_b - p_a;
		float num = Mathf.Abs(f);
		if (num > 180f)
		{
			num = Mathf.Abs(num - 360f);
		}
		return num;
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x000678A8 File Offset: 0x00065AA8
	public static float DegDirection(float p_a, float p_b)
	{
		if (p_a == p_b)
		{
			return 0f;
		}
		p_a = Utils.FixDegAngle(p_a);
		p_b = Utils.FixDegAngle(p_b);
		float num = p_a - p_b;
		float num2 = (num <= 0f) ? -1f : 1f;
		if (Mathf.Abs(num) > 180f)
		{
			num2 *= -1f;
		}
		return num2;
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0006790C File Offset: 0x00065B0C
	public static Vector2 ScreenToGUIPos(Vector2 pos)
	{
		return new Vector2(pos.x, (float)Screen.height - pos.y);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00067928 File Offset: 0x00065B28
	public static void DisableLayer(Transform parent, int layerID)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.gameObject.layer == layerID)
			{
				transform.gameObject.active = false;
			}
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x00067974 File Offset: 0x00065B74
	public static Transform FindTransform(Transform parent, string name)
	{
		if (parent.name == name)
		{
			return parent;
		}
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name == name)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x000679CC File Offset: 0x00065BCC
	public static Bounds FindHeirarcyBounds(Transform parent)
	{
		Bounds result = default(Bounds);
		if (parent.renderer != null)
		{
			result.Encapsulate(parent.renderer.bounds.min);
			result.Encapsulate(parent.renderer.bounds.max);
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			Bounds bounds = Utils.FindHeirarcyBounds(parent.GetChild(i));
			result.Encapsulate(bounds.min);
			result.Encapsulate(bounds.max);
		}
		return result;
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x00067A68 File Offset: 0x00065C68
	public static void LoadTrail(ref TrailRenderer trail, BinaryReader reader)
	{
		bool flag = reader.ReadBoolean();
		if (flag)
		{
			trail = new TrailRenderer();
		}
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00067A8C File Offset: 0x00065C8C
	public static void SaveTrail(TrailRenderer trail, BinaryWriter writer)
	{
		if (trail != null)
		{
			writer.Write(false);
		}
		else
		{
			writer.Write(true);
		}
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x00067AB0 File Offset: 0x00065CB0
	public static void SaveParticles(ParticleSystem ps, BinaryWriter stream)
	{
		PLog.Log(string.Concat(new object[]
		{
			"partzz ",
			ps.particleCount,
			" on ",
			ps.gameObject.name
		}));
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(array);
		stream.Write(array.Length);
		foreach (ParticleSystem.Particle particle in array)
		{
			stream.Write(particle.position.x);
			stream.Write(particle.position.y);
			stream.Write(particle.position.z);
			stream.Write(particle.velocity.x);
			stream.Write(particle.velocity.y);
			stream.Write(particle.velocity.z);
			stream.Write(particle.lifetime);
			stream.Write(particle.startLifetime);
			stream.Write(particle.size);
			stream.Write(particle.rotation);
			stream.Write(particle.angularVelocity);
			stream.Write(particle.color.r);
			stream.Write(particle.color.g);
			stream.Write(particle.color.b);
			stream.Write(particle.color.a);
			stream.Write(particle.randomValue);
		}
		stream.Write(ps.startDelay);
		stream.Write(ps.time);
		stream.Write(ps.playbackSpeed);
		stream.Write(ps.emissionRate);
		stream.Write(ps.startSpeed);
		stream.Write(ps.startSize);
		stream.Write(ps.startColor.r);
		stream.Write(ps.startColor.g);
		stream.Write(ps.startColor.b);
		stream.Write(ps.startColor.a);
		stream.Write(ps.startLifetime);
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x00067D0C File Offset: 0x00065F0C
	public static void LoadParticles(ParticleSystem ps, BinaryReader stream)
	{
		int num = stream.ReadInt32();
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[num];
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.Particle particle = default(ParticleSystem.Particle);
			Vector3 position;
			position.x = stream.ReadSingle();
			position.y = stream.ReadSingle();
			position.z = stream.ReadSingle();
			particle.position = position;
			Vector3 velocity;
			velocity.x = stream.ReadSingle();
			velocity.y = stream.ReadSingle();
			velocity.z = stream.ReadSingle();
			array[i].velocity = velocity;
			particle.lifetime = stream.ReadSingle();
			particle.startLifetime = stream.ReadSingle();
			particle.lifetime = stream.ReadSingle();
			particle.rotation = stream.ReadSingle();
			particle.angularVelocity = stream.ReadSingle();
			Color32 color;
			color.r = stream.ReadByte();
			color.g = stream.ReadByte();
			color.b = stream.ReadByte();
			color.a = stream.ReadByte();
			particle.color = color;
			particle.randomValue = stream.ReadSingle();
			array[i] = particle;
		}
		ps.SetParticles(array, array.Length);
		ps.startDelay = stream.ReadSingle();
		ps.time = stream.ReadSingle();
		ps.playbackSpeed = stream.ReadSingle();
		ps.emissionRate = stream.ReadSingle();
		ps.startSpeed = stream.ReadSingle();
		ps.startSize = stream.ReadSingle();
		Color startColor;
		startColor.r = stream.ReadSingle();
		startColor.g = stream.ReadSingle();
		startColor.b = stream.ReadSingle();
		startColor.a = stream.ReadSingle();
		ps.startColor = startColor;
		ps.startLifetime = stream.ReadSingle();
		PLog.Log("loaded particles " + ps.particleCount);
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x00067EF4 File Offset: 0x000660F4
	public static void GetDimensionsOfGameObject(GameObject go, out float width, out float height, out float depth)
	{
		width = (height = (depth = 0f));
		float num3;
		float num2;
		float num = num2 = (num3 = float.MaxValue);
		float num6;
		float num5;
		float num4 = num5 = (num6 = float.MinValue);
		float x = go.transform.position.x;
		float y = go.transform.position.y;
		float z = go.transform.position.z;
		for (int i = 0; i < 2; i++)
		{
			foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())
			{
				float num7 = x + renderer.transform.localPosition.x + renderer.bounds.min.x;
				float num8 = x + renderer.transform.localPosition.x + renderer.bounds.max.x;
				float num9 = y + renderer.transform.localPosition.y + renderer.bounds.min.y;
				float num10 = y + renderer.transform.localPosition.y + renderer.bounds.max.y;
				float num11 = z + renderer.transform.localPosition.z + renderer.bounds.min.z;
				float num12 = z + renderer.transform.localPosition.z + renderer.bounds.max.z;
				if (num7 < num2)
				{
					num2 = num7;
				}
				if (num8 > num5)
				{
					num5 = num8;
				}
				if (num9 < num)
				{
					num = num9;
				}
				if (num10 > num4)
				{
					num4 = num10;
				}
				if (num11 < num3)
				{
					num3 = num11;
				}
				if (num12 > num6)
				{
					num6 = num12;
				}
			}
		}
		width = num5 - num2;
		height = num4 - num;
		depth = num6 - num3;
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0006813C File Offset: 0x0006633C
	public static bool TrySetTextureFromResources(GameObject onObject, string resourceUrl, float fallbackWidth, float fallbackHeight, bool scaleWithChildren, out string debugOutput)
	{
		debugOutput = string.Empty;
		SimpleSprite component = onObject.GetComponent<SimpleSprite>();
		if (component == null)
		{
			debugOutput = string.Format("GameObject {0} had no SimpleSprite-script on it.", new object[0]);
			return false;
		}
		Texture2D texture2D = Resources.Load(resourceUrl) as Texture2D;
		if (texture2D == null)
		{
			debugOutput = string.Format("\"{0}\" could not be found in Resources or it is no a Texture2D.", resourceUrl);
			return false;
		}
		float x = (float)texture2D.width;
		float y = (float)texture2D.height;
		component.SetTexture(texture2D);
		bool flag = true;
		float num = fallbackWidth;
		float h = fallbackHeight;
		if (onObject.renderer != null)
		{
			num = onObject.renderer.bounds.size.x;
			h = onObject.renderer.bounds.size.y;
			flag = false;
		}
		if (scaleWithChildren)
		{
			bool flag2 = false;
			foreach (Renderer renderer in onObject.GetComponentsInChildren<Renderer>())
			{
				if (renderer != null)
				{
					num = Mathf.Max(num, renderer.bounds.size.x);
					h = Mathf.Max(num, renderer.bounds.size.y);
					flag2 = true;
				}
				flag = (flag || flag2);
			}
		}
		if (flag)
		{
			if (debugOutput.Length > 0)
			{
				debugOutput += "\n";
			}
			debugOutput += "No renderer found in the GameObject";
			if (scaleWithChildren)
			{
				debugOutput += " or any of its children";
			}
			debugOutput += ".";
		}
		component.Setup(num, h, new Vector2(0f, y), new Vector2(x, y));
		component.autoResize = true;
		component.UpdateUVs();
		return true;
	}

	// Token: 0x04000BA6 RID: 2982
	private static bool m_androidBack;

	// Token: 0x0200018C RID: 396
	public enum ValidationStatus
	{
		// Token: 0x04000BA8 RID: 2984
		Ok,
		// Token: 0x04000BA9 RID: 2985
		ToShort,
		// Token: 0x04000BAA RID: 2986
		InvalidCharacter
	}
}
