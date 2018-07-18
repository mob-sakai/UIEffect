using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Area for effect.
	/// </summary>
	public enum EffectArea
	{
		RectTransform,
		Fit,
		Character,
	}

	public static class EffectAreaExtensions
	{
		static readonly Rect rectForCharacter = new Rect(0, 0, 1, 1);

		/// <summary>
		/// Gets effect for area.
		/// </summary>
		public static Rect GetEffectArea(this EffectArea area, VertexHelper vh, Graphic graphic)
		{
			switch (area)
			{
				case EffectArea.RectTransform:
					return graphic.rectTransform.rect;
				case EffectArea.Character:
					return rectForCharacter;
				case EffectArea.Fit:
					{
						// Fit to contents.
						Rect rect = default(Rect);
						UIVertex vertex = default(UIVertex);
						rect.xMin = rect.yMin = float.MaxValue;
						rect.xMax = rect.yMax = float.MinValue;
						for (int i = 0; i < vh.currentVertCount; i++)
						{
							vh.PopulateUIVertex(ref vertex, i);
							rect.xMin = Mathf.Min(rect.xMin, vertex.position.x);
							rect.yMin = Mathf.Min(rect.yMin, vertex.position.y);
							rect.xMax = Mathf.Max(rect.xMax, vertex.position.x);
							rect.yMax = Mathf.Max(rect.yMax, vertex.position.y);
						}
						return rect;
					}
				default:
					return graphic.rectTransform.rect;
			}
		}
	}
}
