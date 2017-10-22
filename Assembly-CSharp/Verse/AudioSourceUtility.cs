using UnityEngine;

namespace Verse
{
	public static class AudioSourceUtility
	{
		public static float GetSanitizedVolume(float volume, object debugInfo)
		{
			float result;
			if (float.IsNegativeInfinity(volume))
			{
				Log.ErrorOnce("Volume was negative infinity (" + debugInfo + ")", 863653423);
				result = 0f;
			}
			else if (float.IsPositiveInfinity(volume))
			{
				Log.ErrorOnce("Volume was positive infinity (" + debugInfo + ")", 954354323);
				result = 1f;
			}
			else if (float.IsNaN(volume))
			{
				Log.ErrorOnce("Volume was NaN (" + debugInfo + ")", 231846572);
				result = 1f;
			}
			else
			{
				result = Mathf.Clamp(volume, 0f, 1000f);
			}
			return result;
		}

		public static float GetSanitizedPitch(float pitch, object debugInfo)
		{
			float result;
			if (float.IsNegativeInfinity(pitch))
			{
				Log.ErrorOnce("Pitch was negative infinity (" + debugInfo + ")", 546475990);
				result = 0.0001f;
			}
			else if (float.IsPositiveInfinity(pitch))
			{
				Log.ErrorOnce("Pitch was positive infinity (" + debugInfo + ")", 309856435);
				result = 1f;
			}
			else if (float.IsNaN(pitch))
			{
				Log.ErrorOnce("Pitch was NaN (" + debugInfo + ")", 800635427);
				result = 1f;
			}
			else if (pitch < 0.0)
			{
				Log.ErrorOnce("Pitch was negative " + pitch + " (" + debugInfo + ")", 384765707);
				result = 0.0001f;
			}
			else
			{
				result = Mathf.Clamp(pitch, 0.0001f, 1000f);
			}
			return result;
		}
	}
}
