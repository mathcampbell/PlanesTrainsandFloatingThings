using System;
using System.Runtime.Serialization;

namespace DataTypes
{
	/// <summary>
	/// Marks the decorated class as the default <see cref="ISerializationSurrogate"/> for the provided target class.
	/// This means that the <see cref="DefaultSerializationSurrogateProvider"/> will use this automatically.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DefaultSerializationSurrogateAttribute : Attribute
	{
		public readonly Type Target;
		public DefaultSerializationSurrogateAttribute(Type target)
		{
			Target = target;
		}
	}
}
