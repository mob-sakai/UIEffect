using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffectInternal
{
    internal static class MeshExtensions
    {
        internal static readonly InternalObjectPool<Mesh> s_MeshPool = new InternalObjectPool<Mesh>(
            () =>
            {
                var mesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave | HideFlags.NotEditable
                };
                mesh.MarkDynamic();
                return mesh;
            },
            mesh => mesh,
            mesh =>
            {
                if (mesh)
                {
                    mesh.Clear();
                }
            });

        public static Mesh Rent()
        {
            return s_MeshPool.Rent();
        }

        public static void Return(ref Mesh mesh)
        {
            s_MeshPool.Return(ref mesh);
        }

        public static void CopyTo(this Mesh self, Mesh dst)
        {
            if (!self || !dst) return;

            var vector3List = InternalListPool<Vector3>.Rent();
            var vector4List = InternalListPool<Vector4>.Rent();
            var color32List = InternalListPool<Color32>.Rent();
            var intList = InternalListPool<int>.Rent();

            dst.Clear(false);

            self.GetVertices(vector3List);
            dst.SetVertices(vector3List);

            self.GetTriangles(intList, 0);
            dst.SetTriangles(intList, 0);

            self.GetNormals(vector3List);
            dst.SetNormals(vector3List);

            self.GetTangents(vector4List);
            dst.SetTangents(vector4List);

            self.GetColors(color32List);
            dst.SetColors(color32List);

            self.GetUVs(0, vector4List);
            dst.SetUVs(0, vector4List);

            self.GetUVs(1, vector4List);
            dst.SetUVs(1, vector4List);

            self.GetUVs(2, vector4List);
            dst.SetUVs(2, vector4List);

            self.GetUVs(3, vector4List);
            dst.SetUVs(3, vector4List);

            dst.RecalculateBounds();
            InternalListPool<Vector3>.Return(ref vector3List);
            InternalListPool<Vector4>.Return(ref vector4List);
            InternalListPool<Color32>.Return(ref color32List);
            InternalListPool<int>.Return(ref intList);
        }

        public static void CopyTo(this Mesh self, VertexHelper dst)
        {
            if (!self || dst == null) return;

            var vertexCount = self.vertexCount;
            var indexCount = self.triangles.Length;
            self.CopyTo(dst, vertexCount, indexCount);
        }

        public static void CopyTo(this Mesh self, VertexHelper dst, int vertexCount, int indexCount)
        {
            if (!self || dst == null) return;

            var positions = InternalListPool<Vector3>.Rent();
            var normals = InternalListPool<Vector3>.Rent();
            var uv0 = InternalListPool<Vector4>.Rent();
            var uv1 = InternalListPool<Vector4>.Rent();
            var tangents = InternalListPool<Vector4>.Rent();
            var colors = InternalListPool<Color32>.Rent();
            var indices = InternalListPool<int>.Rent();
            self.GetVertices(positions);
            self.GetColors(colors);
            self.GetUVs(0, uv0);
            self.GetUVs(1, uv1);
            self.GetNormals(normals);
            self.GetTangents(tangents);
            self.GetIndices(indices, 0);

            dst.Clear();
            for (var i = 0; i < vertexCount; i++)
            {
                dst.AddVert(positions.GetOrDefault(i), colors.GetOrDefault(i), uv0.GetOrDefault(i), uv1.GetOrDefault(i),
                    normals.GetOrDefault(i), tangents.GetOrDefault(i));
            }

            var count = Mathf.Clamp(indexCount, 0, indices.Count);
            for (var i = 0; i < count - 2; i += 3)
            {
                dst.AddTriangle(indices[i], indices[i + 1], indices[i + 2]);
            }

            InternalListPool<Vector3>.Return(ref positions);
            InternalListPool<Vector3>.Return(ref normals);
            InternalListPool<Vector4>.Return(ref uv0);
            InternalListPool<Vector4>.Return(ref uv1);
            InternalListPool<Vector4>.Return(ref tangents);
            InternalListPool<Color32>.Return(ref colors);
            InternalListPool<int>.Return(ref indices);
        }

        private static T GetOrDefault<T>(this List<T> self, int index)
        {
            return 0 <= index && index < self.Count
                ? self[index]
                : default;
        }
    }
}
