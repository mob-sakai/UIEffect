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
		public static Rect GetEffectArea(this EffectArea area, VertexHelper vh, Graphic graphic, float aspectRatio = -1)
		{
			Rect rect = default(Rect);
			switch (area)
			{
				case EffectArea.RectTransform:
					rect = graphic.rectTransform.rect;
					break;
				case EffectArea.Character:
					rect = rectForCharacter;
					break;
				case EffectArea.Fit:
					// Fit to contents.
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
					break;
				default:
					rect = graphic.rectTransform.rect;
					break;
			}


			if(0 < aspectRatio)
			{
				if (rect.width < rect.height)
				{
					rect.width =  rect.height * aspectRatio;
				}
				else
				{
					rect.height = rect.width / aspectRatio;
				}
			}
			return rect;
		}
	}
}
