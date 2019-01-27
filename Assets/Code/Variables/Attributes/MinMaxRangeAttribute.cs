// ---------------------------------------------------------------------------- 
// Author: Richard Fine
// Source: https://bitbucket.org/richardfine/scriptableobjectdemo
// ----------------------------------------------------------------------------

using System;

public class MinMaxRangeAttribute : Attribute {
	public MinMaxRangeAttribute(float min, float max) {
		FloatMin = min;
		FloatMax = max;
	}

	public MinMaxRangeAttribute(int min, int max) {
		IntMin = min;
		IntMax = max;
	}

	public float FloatMin { get; }
	public float FloatMax { get; }	
	public int IntMin { get; }
	public int IntMax { get; }
}

[Serializable]
public struct RangedFloat {
	public float Min;
	public float Max;
}

[Serializable]
public struct RangedInt { 
	public int Min;
	public int Max;
}