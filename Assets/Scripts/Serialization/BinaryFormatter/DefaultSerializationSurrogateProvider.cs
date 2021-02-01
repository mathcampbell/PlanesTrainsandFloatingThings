using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Serialization.BinaryFormatter
{
	/// <summary>
	/// This class provides an <see cref="ISurrogateSelector"/> that contains all the <see cref="ISerializationSurrogate"/>s that are decorated with the <see cref="DefaultSerializationSurrogateAttribute"/>
	/// Usage: apply <see cref="AddDefaultSurrogatesSelector"/> on an instance of <see cref="BinaryFormatter"/>.
	/// </summary>
	public static class DefaultSerializationSurrogateProvider
	{
		private static readonly (Type, DefaultSerializationSurrogateAttribute)[] array;


		static DefaultSerializationSurrogateProvider()
		{
			// Expensive: do it only once.
			array = GetTypes(Assembly.GetExecutingAssembly()).ToArray();
		}

		/// <summary>
		/// Set or <see cref="SurrogateSelector.ChainSelector"/> a <see cref="ISurrogateSelector"/> that applies all the <see cref="ISerializationSurrogate"/> marked with the <see cref="DefaultSerializationSurrogateAttribute"/>
		/// </summary>
		/// <param name="instance"></param>
		public static void AddDefaultSurrogatesSelector(this System.Runtime.Serialization.Formatters.Binary.BinaryFormatter instance)
		{
			// We create a new one each time, because otherwise the on instance we keep could get modified after it's handed out.
			// For example by chaining a custom selector, that may be needed for custom behaviour.

			var selector = new SurrogateSelector();
			foreach (var tuple in array)
			{
				(Type type, DefaultSerializationSurrogateAttribute attribute) = tuple;

				var surrogateInstance = Activator.CreateInstance(type) as ISerializationSurrogate;

				selector.AddSurrogate(
					attribute.Target,
					new StreamingContext(StreamingContextStates.All),
					surrogateInstance
				);
			}

			if (null != instance.SurrogateSelector)
			{
				instance.SurrogateSelector.ChainSelector(selector);
			}
			else
			{
				instance.SurrogateSelector = selector;
			}
		}

		/// <summary>
		/// Set or <see cref="SurrogateSelector.ChainSelector"/> a <see cref="ISurrogateSelector"/> that applies all the <see cref="ISerializationSurrogate"/> marked with the <see cref="DefaultSerializationSurrogateAttribute"/>
		/// </summary>
		/// <param name="instance"></param>
		public static void AddDefaultSurrogatesSelector(this DataContractSerializer instance)
		{
			throw new NotImplementedException("Only works with the BinaryFormatter right now.");
			// We create a new one each time, because otherwise the instance we keep could get modified after it's handed out.
			// For example by chaining a custom selector, that may be needed for custom behaviour.
			/*
			var selector = new SurrogateSelector();
			foreach (var tuple in array)
			{
				(Type type, DefaultSerializationSurrogateAttribute attribute) = tuple;

				var surrogateInstance = Activator.CreateInstance(type) as ISerializationSurrogate;

				selector.AddSurrogate(
					attribute.Target,
					new StreamingContext(StreamingContextStates.All),
					surrogateInstance
				);
			}

			if (null != instance.DataContractSurrogate)
			{
				// todo: fix
				//instance.DataContractSurrogate.  .ChainSelector(selector);
			}
			else
			{
				// todo: fix
				//instance.DataContractSurrogate = selector;
			}
			*/
		}

		private static IEnumerable<(Type type, DefaultSerializationSurrogateAttribute attribute)> GetTypes(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				var attribute = type.GetCustomAttribute<DefaultSerializationSurrogateAttribute>();
				if (null != attribute)
				{
					yield return (type, attribute);
				}
			}
		}
	}
}
