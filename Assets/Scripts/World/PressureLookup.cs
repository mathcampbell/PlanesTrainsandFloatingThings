using UnityEngine;

namespace World
{
	public static class PressureValues
	{
		public static AnimationCurve airPressure = new AnimationCurve(new Keyframe(0f,   100f), new Keyframe(100f,  99f),    new Keyframe(200f,  98.4f), new Keyframe(500f, 94.3f), new Keyframe(1000f, 87.8f), new Keyframe(2000f, 76.1f), new Keyframe(5000f, 49.6f), new Keyframe(10000f, 24.2f), new Keyframe(20000f, 5.8f), new Keyframe(30000f, 1.39f), new Keyframe(50000f, 0.08f));
		public static AnimationCurve waterPressure = new AnimationCurve(new Keyframe(0f, 101f), new Keyframe(1000f, 10153f), new Keyframe(5000f, 50360f));

		public static float AirPressure(float altitude)
		{
			return airPressure.Evaluate(altitude);
		}

		public static float WaterPressure(float depth)
		{
			return waterPressure.Evaluate(depth);
		}

	}
}
