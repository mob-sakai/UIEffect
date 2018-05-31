using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Packer
{
	/// <summary>
	/// Pack 4 low-precision [0-1] floats values to a float.
	/// Each value [0-1] has 64 steps(6 bits).
	/// </summary>
	public static float ToFloat(float x, float y, float z, float w)
	{
		const int PRECISION = (1 << 6) - 1;
		return (Mathf.FloorToInt(w * PRECISION) << 18)
		+ (Mathf.FloorToInt(z * PRECISION) << 12)
		+ (Mathf.FloorToInt(y * PRECISION) << 6)
		+ Mathf.FloorToInt(x * PRECISION);
	}

	/// <summary>
	/// Pack 4 low-precision [0-1] floats values to a float.
	/// Each value [0-1] has 64 steps(6 bits).
	/// </summary>
	public static float ToFloat(Vector4 factor)
	{
		return ToFloat(Mathf.Clamp01(factor.x), Mathf.Clamp01(factor.y), Mathf.Clamp01(factor.z), Mathf.Clamp01(factor.w));
	}

	/// <summary>
	/// Pack 3 low-precision [0-1] floats values to a float.
	/// Each value [0-1] has 256 steps(8 bits).
	/// </summary>
	public static float ToFloat(float x, float y, float z)
	{
		const int PRECISION = (1 << 8) - 1;
		return (Mathf.FloorToInt(z * PRECISION) << 16)
		+ (Mathf.FloorToInt(y * PRECISION) << 8)
		+ Mathf.FloorToInt(x * PRECISION);
	}

	/// <summary>
	/// Pack 2 low-precision [0-1] floats values to a float.
	/// Each value [0-1] has 4096 steps(12 bits).
	/// </summary>
	public static float ToFloat(float x, float y)
	{
		const int PRECISION = (1 << 12) - 1;
		return (Mathf.FloorToInt(y * PRECISION) << 12)
		+ Mathf.FloorToInt(x * PRECISION);
	}
}
