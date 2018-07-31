using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIExtensions
{
	public interface IParameterTexture
	{
		int parameterIndex { get; set; }
		ParameterTexture ptex { get; }
	}


	public class ParameterTexture
	{
		Texture2D _texture;
		bool _needUpload;
		int _propertyId;
		readonly string _propertyName;
		readonly int _channels;
		readonly int _instanceLimit;
		readonly byte[] _data;
		readonly Stack<int> _stack;

		public ParameterTexture(int channels, int instanceLimit, string propertyName)
		{
			_propertyName = propertyName;
			_channels = ((channels - 1) / 4 + 1) * 4;
			_instanceLimit = ((instanceLimit - 1) / 2 + 1) * 2;
			_data = new byte[_channels * _instanceLimit];

			_stack = new Stack<int>(_instanceLimit);
			for (int i = 1; i < _instanceLimit + 1; i++)
			{
				_stack.Push(i);
			}
		}

		void Initialize()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}
#endif

			if (!_texture)
			{
				_texture = new Texture2D(_channels / 4, _instanceLimit, TextureFormat.RGBA32, false, false);
				_texture.filterMode = FilterMode.Point;
				_texture.wrapMode = TextureWrapMode.Clamp;

				// Update dispatcher
				Canvas.willRenderCanvases += () =>
				{
					if (_needUpload && _texture)
					{
						_needUpload = false;
						_texture.LoadRawTextureData(_data);
						_texture.Apply(false, false);
					}
				};
			}
		}


		public void Register(IParameterTexture target)
		{
			Initialize();
			if (target.parameterIndex <= 0 && 0 < _stack.Count)
			{
				target.parameterIndex = _stack.Pop();
				Debug.LogFormat("<color=green>@@@ Register {0} : {1}</color>", target, target.parameterIndex);
			}
		}

		public void Unregister(IParameterTexture target)
		{
			if (0 < target.parameterIndex)
			{
				Debug.LogFormat("<color=red>@@@ Unregister {0} : {1}</color>", target, target.parameterIndex);
				_stack.Push(target.parameterIndex);
				target.parameterIndex = 0;
			}
		}

		public void SetData(IParameterTexture target, int channelId, byte value)
		{
			if (0 < target.parameterIndex)
			{
				_data[(target.parameterIndex -1) * _channels + channelId] = value;
				_needUpload = true;
			}
		}

		public void SetData(IParameterTexture target, int channelId, float value)
		{
			SetData(target, channelId, (byte)(Mathf.Clamp01(value) * 255));
		}

		public void RegisterMaterial(Material mat)
		{
			if (_propertyId == 0)
			{
				_propertyId = Shader.PropertyToID(_propertyName);
			}
			if (mat)
			{
				mat.SetTexture(_propertyId, _texture);
			}
		}

		public float GetNormalizedIndex(IParameterTexture target)
		{
			return ((float)target.parameterIndex) / _instanceLimit;
		}
	}
}