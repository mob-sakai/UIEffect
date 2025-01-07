using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

public class ReflectionTests
{
    [Test]
    public void ShaderVariantCollectionInspector_DrawShaderEntry()
    {
        var type = Type.GetType("UnityEditor.ShaderVariantCollectionInspector, UnityEditor");
        Assert.IsNotNull(type, "'UnityEditor.ShaderVariantCollectionInspector, UnityEditor' is not found");

        var miDrawShaderEntry = type.GetMethod("DrawShaderEntry", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(miDrawShaderEntry);

        var parameters = miDrawShaderEntry.GetParameters();
        Assert.AreEqual(1, parameters.Length);
        Assert.AreEqual(typeof(int), parameters[0].ParameterType);
    }

    [Test]
    public void SerializedProperty_gradientValue()
    {
        var piGradient = typeof(SerializedProperty)
            .GetProperty("gradientValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(piGradient);

        Assert.IsTrue(piGradient.CanRead);
        Assert.IsTrue(piGradient.CanWrite);
    }
}
