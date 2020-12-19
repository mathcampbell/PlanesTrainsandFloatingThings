using System.Runtime.Serialization;

using UnityEngine;

namespace Serialization.BinaryFormatter.SerializationSurrogates
{
	[DefaultSerializationSurrogate(typeof(Vector3))]
	public class Vector3S : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector3) obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
			info.AddValue("z", v.z);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new Vector3(
				info.GetSingle("x"),
				info.GetSingle("y"),
				info.GetSingle("z"));
		}
	}
}
