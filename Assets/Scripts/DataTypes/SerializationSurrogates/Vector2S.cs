using System.Runtime.Serialization;

using UnityEngine;

namespace DataTypes.SerializationSurrogates
{
	[DefaultSerializationSurrogate(typeof(Vector2))]
	public class Vector2S : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Vector2)obj;
			info.AddValue("x", v.x);
			info.AddValue("y", v.y);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new Vector2(
				info.GetSingle("x"),
				info.GetSingle("y"));
		}
	}
}
