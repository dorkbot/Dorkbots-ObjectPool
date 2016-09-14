using System;

namespace Dorkbots.ObjectPooling
{
	public interface IObjectForPool
	{
		/// <summary>
		/// Use this to inject the ObjectPool that contains this object.
		/// </summary>
		IObjectPool objectPool{set;}
	}
}