using System.Runtime.Serialization;

using UnityEngine;

namespace DataTypes.SerializationSurrogates
{
	[DefaultSerializationSurrogate(typeof(Quaternion))]
	public class QuaternionS : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Quaternion)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
			info.AddValue("z", v.z);
			info.AddValue("w", v.w);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new Quaternion(
				info.GetSingle("x"),
				info.GetSingle("y"),
				info.GetSingle("z"),
				info.GetSingle("w"));
		}
	}
}
