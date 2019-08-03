using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions.Editors
{
	public class BaseMeshEffectEditor : Editor
	{
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