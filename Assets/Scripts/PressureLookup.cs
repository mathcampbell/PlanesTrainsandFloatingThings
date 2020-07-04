using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PressureLookupScriptableObject", order = 1)]
public class PressureLookupScriptableObject : ScriptableObject
{
    public  AnimationCurve airPressure;
	public  AnimationCurve waterPressure;

	public  float AirPressure(float altitude)
	{
		return airPressure.Evaluate(altitude);
	}

    public  float WaterPressure(float depth)
    {
		return waterPressure.Evaluate(depth);
    }

}