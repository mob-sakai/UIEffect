using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffectInternal
{
    /// <summary>
    /// Provides additional callbacks related to canvas and UI system.
    /// </summary>
    internal static class UIExtraCallbacks
    {
        private static bool s_IsInitializedAfterCanvasRebuild;
        private static readonly FastAction s_AfterCanvasRebuildAction = new FastAction();
        private static readonly FastAction s_LateAfterCanvasRebuildAction = new FastAction();
        private static readonly FastAction s_BeforeCanvasRebuildAction = new FastAction();
        private static readonly FastAction s_OnScreenSizeChangedAction = new FastAction();
        private static Vector2Int s_LastScreenSize;

        static UIExtraCallbacks()
        {
            Canvas.willRenderCanvases += OnBeforeCanvasRebuild;
            Logger.LogMulticast(typeof(Canvas), "willRenderCanvases", message: "ctor");
        }

        /// <summary>
        /// Event that occurs after canvas rebuilds.
        /// </summary>
        public static event Action onLateAfterCanvasRebuild
        {
            add => s_LateAfterCanvasRebuildAction.Add(value);
            remove => s_LateAfterCanvasRebuildAction.Remove(value);
        }

        /// <summary>
        /// Event that occurs before canvas rebuilds.
        /// </summary>
        public static event Action onBeforeCanvasRebuild
        {
            add => s_BeforeCanvasRebuildAction.Add(value);
            remove => s_BeforeCanvasRebuildAction.Remove(value);
        }

        /// <summary>
        /// Event that occurs after canvas rebuilds.
        /// </summary>
        public static event Action onAfterCanvasRebuild
        {
            add => s_AfterCanvasRebuildAction.Add(value);
            remove => s_AfterCanvasRebuildAction.Remove(value);
        }

        /// <summary>
        /// Event that occurs when the screen size changes.
        /// </summary>
        public static event Action onScreenSizeChanged
        {
            add => s_OnScreenSizeChangedAction.Add(value);
            remove => s_OnScreenSizeChangedAction.Remove(value);
        }

        /// <summary>
        /// Initializes the UIExtraCallbacks to ensure proper event handling.
        /// </summary>
        private static void InitializeAfterCanvasRebuild()
        {
            if (s_IsInitializedAfterCanvasRebuild) return;
            s_IsInitializedAfterCanvasRebuild = true;

            // Explicitly set `Canvas.willRenderCanvases += CanvasUpdateRegistry.PerformUpdate`.
            CanvasUpdateRegistry.IsRebuildingLayout();
#if TMP_ENABLE
            // Explicitly set `Canvas.willRenderCanvases += TMP_UpdateManager.DoRebuilds`.
            typeof(TMPro.TMP_UpdateManager)
                .GetProperty("instance", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
#endif
            Canvas.willRenderCanvases -= OnAfterCanvasRebuild;
            Canvas.willRenderCanvases += OnAfterCanvasRebuild;
            Logger.LogMulticast(typeof(Canvas), "willRenderCanvases",
                message: "InitializeAfterCanvasRebuild");
        }

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#elif UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoad()
        {
            Canvas.willRenderCanvases -= OnAfterCanvasRebuild;
            s_IsInitializedAfterCanvasRebuild = false;
            s_LastScreenSize = default;
        }

        /// <summary>
        /// Callback method called before canvas rebuilds.
        /// </summary>
        private static void OnBeforeCanvasRebuild()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (s_LastScreenSize != screenSize)
            {
                if (s_LastScreenSize != default)
                {
                    s_OnScreenSizeChangedAction.Invoke();
                }

                s_LastScreenSize = screenSize;
            }

            s_BeforeCanvasRebuildAction.Invoke();
            InitializeAfterCanvasRebuild();
        }

        /// <summary>
        /// Callback method called after canvas rebuilds.
        /// </summary>
        private static void OnAfterCanvasRebuild()
        {
            s_AfterCanvasRebuildAction.Invoke();
            s_LateAfterCanvasRebuildAction.Invoke();
        }
    }
}
