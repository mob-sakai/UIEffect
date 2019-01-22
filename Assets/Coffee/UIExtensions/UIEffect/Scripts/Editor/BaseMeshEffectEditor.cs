using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions.Editors
{
	public class BaseMeshEffectEditor : Editor
	{
		List<MaterialEditor> _materialEditors = new List<MaterialEditor> ();

		protected virtual void OnEnable ()
		{
			ClearMaterialEditors ();
		}

		protected virtual void OnDisable ()
		{
			ClearMaterialEditors ();
		}

		void ClearMaterialEditors ()
		{
			foreach (var e in _materialEditors)
			{
				if (e)
				{
					DestroyImmediate (e);
				}
			}
			_materialEditors.Clear ();
		}

		protected void ShowMaterialEditors (Material [] materials, int startIndex, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (_materialEditors.Count == i)
				{
					_materialEditors.Add (null);
				}

				var mat = materials [startIndex + i];
				var editor = _materialEditors [i];
				if (editor && editor.target != mat)
				{
					DestroyImmediate (editor);
					editor = null;
				}

				if (!editor)
				{
					editor = _materialEditors [i] = Editor.CreateEditor (mat) as MaterialEditor;
				}

				editor.DrawHeader ();
				editor.OnInspectorGUI ();
			}
		}


		protected void ShowCanvasChannelsWarning ()
		{
			BaseMeshEffect effect = target as BaseMeshEffect;
			if (!effect || !effect.graphic)
			{
				return;
			}

#if UNITY_5_6_OR_NEWER
			AdditionalCanvasShaderChannels channels = effect.requiredChannels;
			var canvas = effect.graphic.canvas;
			if (canvas && (canvas.additionalShaderChannels & channels) != channels)
			{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.HelpBox (string.Format ("Enable {1} of Canvas.additionalShaderChannels to use {0}.", effect.GetType ().Name, channels), MessageType.Warning);
				if (GUILayout.Button ("Fix"))
				{
					canvas.additionalShaderChannels |= channels;
				}
				EditorGUILayout.EndHorizontal ();
			}
#endif
		}
	}
}