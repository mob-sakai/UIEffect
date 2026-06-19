#if UNITEXT_ENABLE
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [ExecuteAlways]
    public class UniTextProxyRenderer : MaskableGraphic
    {
        public override Texture mainTexture => _texture;
        private Mesh _mesh;
        private Texture _texture;

        private UniTextProxyRenderer()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            _mesh.CopyTo(vh);

            var count = vh.currentVertCount;
            UIVertex vt = default;
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vt, i);
                vt.position = new Vector3(vt.position.x, vt.position.y);
                vh.SetUIVertex(vt, i);
            }
        }

        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            return baseMaterial;
        }

        public void Rebuild(Mesh mesh, Material mat, Texture texture)
        {
            _mesh = mesh;
            _texture = texture;
            m_Material = mat;

            SetVerticesDirty();
            SetMaterialDirty();
            Rebuild(CanvasUpdate.PreRender);
        }
    }
}
#endif
