using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
    public class GraphicConnector : BaseConnector
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            AddConnector(new GraphicConnector());
        }

        protected override int priority
        {
            get { return 0; }
        }

        public override Material GetMaterial(Graphic graphic)
        {
            return graphic ? graphic.material : null;
        }

        public override void SetMaterial(Graphic graphic, Material material)
        {
            if (graphic)
                graphic.material = material;
        }

        public override AdditionalCanvasShaderChannels extraChannel
        {
            get { return AdditionalCanvasShaderChannels.TexCoord1; }
        }

        public override Shader FindShader(string shaderName)
        {
            return Shader.Find("Hidden/" + shaderName);
        }

        protected override bool IsValid(Graphic graphic)
        {
            return graphic;
        }

        public override void SetVerticesDirty(Graphic graphic)
        {
            if (graphic)
                graphic.SetVerticesDirty();
        }

        public override void SetMaterialDirty(Graphic graphic)
        {
            if (graphic)
                graphic.SetMaterialDirty();
        }

        public override bool IsText(Graphic graphic)
        {
            return graphic && graphic is Text;
        }

        public override void SetExtraChannel(ref UIVertex vertex, Vector2 value)
        {
            vertex.uv1 = value;
        }
    }
}
