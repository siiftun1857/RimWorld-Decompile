using UnityEngine;
using Verse;

namespace RimWorld
{
	public static class PawnSkinColors
	{
		private struct SkinColorData
		{
			public float melanin;

			public float selector;

			public Color color;

			public SkinColorData(float melanin, float selector, Color color)
			{
				this.melanin = melanin;
				this.selector = selector;
				this.color = color;
			}
		}

		private static readonly SkinColorData[] SkinColors = new SkinColorData[6]
		{
			new SkinColorData(0f, 0f, new Color(0.9490196f, 0.929411769f, 0.8784314f)),
			new SkinColorData(0.25f, 0.2f, new Color(1f, 0.9372549f, 0.8352941f)),
			new SkinColorData(0.5f, 0.7f, new Color(1f, 0.9372549f, 0.7411765f)),
			new SkinColorData(0.75f, 0.8f, new Color(0.894117653f, 0.619607866f, 0.3529412f)),
			new SkinColorData(0.9f, 0.9f, new Color(0.509803951f, 0.356862754f, 0.1882353f)),
			new SkinColorData(1f, 1f, new Color(0.3882353f, 0.274509817f, 0.141176477f))
		};

		public static bool IsDarkSkin(Color color)
		{
			Color skinColor = PawnSkinColors.GetSkinColor(0.5f);
			return color.r + color.g + color.b <= skinColor.r + skinColor.g + skinColor.b + 0.0099999997764825821;
		}

		public static Color GetSkinColor(float melanin)
		{
			int skinDataIndexOfMelanin = PawnSkinColors.GetSkinDataIndexOfMelanin(melanin);
			Color result;
			if (skinDataIndexOfMelanin == PawnSkinColors.SkinColors.Length - 1)
			{
				result = PawnSkinColors.SkinColors[skinDataIndexOfMelanin].color;
			}
			else
			{
				float t = Mathf.InverseLerp(PawnSkinColors.SkinColors[skinDataIndexOfMelanin].melanin, PawnSkinColors.SkinColors[skinDataIndexOfMelanin + 1].melanin, melanin);
				result = Color.Lerp(PawnSkinColors.SkinColors[skinDataIndexOfMelanin].color, PawnSkinColors.SkinColors[skinDataIndexOfMelanin + 1].color, t);
			}
			return result;
		}

		public static float RandomMelanin(Faction fac)
		{
			float num = (fac != null) ? Rand.Range(Mathf.Clamp01(fac.centralMelanin - fac.def.geneticVariance), Mathf.Clamp01(fac.centralMelanin + fac.def.geneticVariance)) : Rand.Value;
			int num2 = 0;
			int num3 = 0;
			while (num3 < PawnSkinColors.SkinColors.Length && num >= PawnSkinColors.SkinColors[num3].selector)
			{
				num2 = num3;
				num3++;
			}
			float result;
			if (num2 == PawnSkinColors.SkinColors.Length - 1)
			{
				result = PawnSkinColors.SkinColors[num2].melanin;
			}
			else
			{
				float t = Mathf.InverseLerp(PawnSkinColors.SkinColors[num2].selector, PawnSkinColors.SkinColors[num2 + 1].selector, num);
				result = Mathf.Lerp(PawnSkinColors.SkinColors[num2].melanin, PawnSkinColors.SkinColors[num2 + 1].melanin, t);
			}
			return result;
		}

		public static float GetMelaninCommonalityFactor(float melanin)
		{
			int skinDataIndexOfMelanin = PawnSkinColors.GetSkinDataIndexOfMelanin(melanin);
			float result;
			if (skinDataIndexOfMelanin == PawnSkinColors.SkinColors.Length - 1)
			{
				result = PawnSkinColors.GetSkinDataCommonalityFactor(skinDataIndexOfMelanin);
			}
			else
			{
				float t = Mathf.InverseLerp(PawnSkinColors.SkinColors[skinDataIndexOfMelanin].melanin, PawnSkinColors.SkinColors[skinDataIndexOfMelanin + 1].melanin, melanin);
				result = Mathf.Lerp(PawnSkinColors.GetSkinDataCommonalityFactor(skinDataIndexOfMelanin), PawnSkinColors.GetSkinDataCommonalityFactor(skinDataIndexOfMelanin + 1), t);
			}
			return result;
		}

		public static float GetRandomMelaninSimilarTo(float value, float clampMin = 0f, float clampMax = 1f)
		{
			return Mathf.Clamp01(Mathf.Clamp(Rand.Gaussian(value, 0.05f), clampMin, clampMax));
		}

		private static float GetSkinDataCommonalityFactor(int skinDataIndex)
		{
			float num = 0f;
			for (int i = 0; i < PawnSkinColors.SkinColors.Length; i++)
			{
				num = Mathf.Max(num, PawnSkinColors.GetTotalAreaWhereClosestToSelector(i));
			}
			return PawnSkinColors.GetTotalAreaWhereClosestToSelector(skinDataIndex) / num;
		}

		private static float GetTotalAreaWhereClosestToSelector(int skinDataIndex)
		{
			float num = 0f;
			if (skinDataIndex == 0)
			{
				num += PawnSkinColors.SkinColors[skinDataIndex].selector;
			}
			else if (PawnSkinColors.SkinColors.Length > 1)
			{
				num = (float)(num + (PawnSkinColors.SkinColors[skinDataIndex].selector - PawnSkinColors.SkinColors[skinDataIndex - 1].selector) / 2.0);
			}
			if (skinDataIndex == PawnSkinColors.SkinColors.Length - 1)
			{
				num = (float)(num + (1.0 - PawnSkinColors.SkinColors[skinDataIndex].selector));
			}
			else if (PawnSkinColors.SkinColors.Length > 1)
			{
				num = (float)(num + (PawnSkinColors.SkinColors[skinDataIndex + 1].selector - PawnSkinColors.SkinColors[skinDataIndex].selector) / 2.0);
			}
			return num;
		}

		private static int GetSkinDataIndexOfMelanin(float melanin)
		{
			int result = 0;
			int num = 0;
			while (num < PawnSkinColors.SkinColors.Length && melanin >= PawnSkinColors.SkinColors[num].melanin)
			{
				result = num;
				num++;
			}
			return result;
		}
	}
}
