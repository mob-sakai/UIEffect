using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    public class BaseConnector
    {
        private static readonly List<BaseConnector> s_Connectors = new List<BaseConnector>();
        private static readonly Dictionary<Type, BaseConnector> s_ConnectorMap = new Dictionary<Type, BaseConnector>();
        private static readonly BaseConnector s_EmptyConnector = new BaseConnector();

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            AddConnector(s_EmptyConnector);
        }

        protected static void AddConnector(BaseConnector connector)
        {
            s_Connectors.Add(connector);
            s_Connectors.Sort((x, y) => y.priority - x.priority);
        }

        public static BaseConnector FindConnector(Graphic graphic)
        {
            if (!graphic) return s_EmptyConnector;

            var type = graphic.GetType();
            BaseConnector connector = null;
            if (s_ConnectorMap.TryGetValue(type, out connector)) return connector;

            foreach (var c in s_Connectors)
            {
                if (!c.IsValid(graphic)) continue;

                s_ConnectorMap.Add(type, c);
                return c;
            }

            return s_EmptyConnector;
        }

        /// <summary>
        /// The connector is valid for the component.
        /// </summary>
        protected virtual bool IsValid(Graphic graphic)
        {
            return true;
        }

        /// <summary>
        /// Find effect shader.
        /// </summary>
        public virtual Shader FindShader(string shaderName)
        {
            return null;
        }

        /// <summary>
        /// Priority.
        /// </summary>
        protected virtual int priority
        {
            get { return -1; }
        }

        /// <summary>
        /// Extra channel.
        /// </summary>
        public virtual AdditionalCanvasShaderChannels extraChannel
        {
            get { return AdditionalCanvasShaderChannels.None; }
        }

        /// <summary>
        /// Set material.
        /// </summary>
        public virtual void SetMaterial(Graphic graphic, Material material)
        {
        }

        /// <summary>
        /// Get material.
        /// </summary>
        public virtual Material GetMaterial(Graphic graphic)
        {
            return null;
        }

        /// <summary>
        /// Mark the vertices as dirty.
        /// </summary>
        public virtual void SetVerticesDirty(Graphic graphic)
        {
        }

        /// <summary>
        /// Mark the material as dirty.
        /// </summary>
        public virtual void SetMaterialDirty(Graphic graphic)
        {
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        public virtual void OnEnable(Graphic graphic)
        {
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled () or inactive.
        /// </summary>
        public virtual void OnDisable(Graphic graphic)
        {
        }

        /// <summary>
        /// Event that is called just before Canvas rendering happens.
        /// This allows you to delay processing / updating of canvas based elements until just before they are rendered.
        /// </summary>
        protected virtual void OnWillRenderCanvases()
        {
        }

        /// <summary>
        /// Gets position factor for area.
        /// </summary>
        public void GetPositionFactor(EffectArea area, int index, Rect rect, Vector2 position, out float x, out float y)
        {
            if (area == EffectArea.Fit)
            {
                x = Mathf.Clamp01((position.x - rect.xMin) / rect.width);
                y = Mathf.Clamp01((position.y - rect.yMin) / rect.height);
            }
            else
            {
                x = Mathf.Clamp01(position.x / rect.width + 0.5f);
                y = Mathf.Clamp01(position.y / rect.height + 0.5f);
            }
        }

        /// <summary>
        /// Normalize vertex position by local matrix.
        /// </summary>
        public void GetNormalizedFactor(EffectArea area, int index, Matrix2x3 matrix, Vector2 position,
            out Vector2 normalizedPos)
        {
            normalizedPos = matrix * position;
        }

        public virtual bool IsText(Graphic graphic)
        {
            return false;
        }

        public virtual void SetExtraChannel(ref UIVertex vertex, Vector2 value)
        {
        }
    }
}
