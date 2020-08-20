using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tools
{
	/// <summary>
	/// Reflection allows reasoning about the C# code itself, such as inspecting types, attributes etc.
	/// </summary>
	public static class Reflection
	{
		/// <summary>
		/// Finds all derived <see cref="Type"/>s of <typeparamref name="TBase"/> in the given assembly.
		/// </summary>
		/// <typeparam name="TBase">The type to search derivatives of</typeparam>
		/// <param name="assembly">Assembly to search in</param>
		/// <returns>List of Types</returns>
		public static List<Type> FindAllDerivedTypes<TBase>(Assembly assembly) where TBase : class
		{
			_ = assembly ?? throw new ArgumentNullException(nameof(assembly)); //Contract.Requires(null != assembly);

			var baseType = typeof(TBase);
			return assembly
				.GetTypes()
				.Where(t
					=> t != baseType
					&& t.IsSubclassOf(baseType)
				).ToList();
		}
	}
}
