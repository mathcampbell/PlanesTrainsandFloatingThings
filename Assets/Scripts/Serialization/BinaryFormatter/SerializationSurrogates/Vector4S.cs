using System.Runtime.Serialization;

using UnityEngine;

namespace Serialization.BinaryFormatter.SerializationSurrogates
{
	[DefaultSerializationSurrogate(typeof(Vector4))]
	public class Vector4S : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector4)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
			info.AddValue("z", v.z);
			info.AddValue("w", v.w);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new Vector4(
				info.GetSingle("x"),
				info.GetSingle("y"),
				info.GetSingle("z"),
				info.GetSingle("w"));
		}
	}
}
