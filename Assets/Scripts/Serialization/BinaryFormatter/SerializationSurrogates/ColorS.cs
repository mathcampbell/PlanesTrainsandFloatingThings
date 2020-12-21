using System.Runtime.Serialization;

using UnityEngine;

namespace Serialization.BinaryFormatter.SerializationSurrogates
{
	[DefaultSerializationSurrogate(typeof(Color))]
	public class ColorS : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var v = (Color)obj;
			info.AddValue("r", v.r);
			info.AddValue("g", v.g);
			info.AddValue("b", v.b);
			info.AddValue("a", v.a);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new Color(
				info.GetSingle("r"),
				info.GetSingle("g"),
				info.GetSingle("b"),
				info.GetSingle("a"));
		}
	}
}
