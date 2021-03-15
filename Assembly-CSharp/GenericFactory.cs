using System;
using System.Collections.Generic;

// Token: 0x02000183 RID: 387
public class GenericFactory<T>
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x00065164 File Offset: 0x00063364
	public void Register<TY>(string name)
	{
		this.m_states.Add(name, typeof(TY));
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x0006517C File Offset: 0x0006337C
	public T Create(string name, params object[] args)
	{
		Type type;
		if (this.m_states.TryGetValue(name, out type))
		{
			return (T)((object)Activator.CreateInstance(type, args));
		}
		return default(T);
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x000651B4 File Offset: 0x000633B4
	public string GetTypeName(T refObject)
	{
		Type type = refObject.GetType();
		foreach (KeyValuePair<string, Type> keyValuePair in this.m_states)
		{
			if (keyValuePair.Value == type)
			{
				return keyValuePair.Key;
			}
		}
		return null;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00065240 File Offset: 0x00063440
	public int GetNrOfRegisteredTypes()
	{
		return this.m_states.Count;
	}

	// Token: 0x04000B85 RID: 2949
	private Dictionary<string, Type> m_states = new Dictionary<string, Type>();
}
