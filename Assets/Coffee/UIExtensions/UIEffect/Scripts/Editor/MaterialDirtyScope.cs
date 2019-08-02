using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// Changes in this scope cause the graphic's material to be dirty.
	/// </summary>
	internal class MaterialDirtyScope : EditorGUI.ChangeCheckScope
	{
		readonly Object [] targets;

		public MaterialDirtyScope (Object [] targets)
		{
			this.targets = targets;
		}

		protected override void CloseScope ()
		{
			if (changed)
			{
				var graphics = targets.OfType<UIEffectBase> ()
					.Select (x => x.targetGraphic)
					.Where (x => x);

				foreach (var g in graphics)
				{
					g.SetMaterialDirty ();
				}
			}

			base.CloseScope ();
		}
	}
}