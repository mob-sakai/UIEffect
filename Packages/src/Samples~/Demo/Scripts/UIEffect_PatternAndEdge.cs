using Coffee.UIEffects;
using UnityEngine;

public class UIEffect_PatternAndEdge : MonoBehaviour
{
    private UIEffect[] _effects;

    // Start is called before the first frame update
    private void Start()
    {
        _effects = GetComponentsInChildren<UIEffect>();
    }

    public void SetTransitionWidth(float value)
    {
        foreach (var effect in _effects)
        {
            effect.transitionWidth = value;
        }
    }

    public void SetTransitionRotation(float value)
    {
        foreach (var effect in _effects)
        {
            effect.transitionRotation = value;
        }
    }

    public void SetTransitionAutoSpeed(float value)
    {
        foreach (var effect in _effects)
        {
            effect.transitionAutoPlaySpeed = value;
        }
    }

    public void SetEdgeWidth(float value)
    {
        foreach (var effect in _effects)
        {
            effect.edgeWidth = value;
        }
    }

    public void SetShinyWidth(float value)
    {
        foreach (var effect in _effects)
        {
            effect.edgeShinyWidth = value;
        }
    }

    public void SetShinySpeed(float value)
    {
        foreach (var effect in _effects)
        {
            effect.edgeShinyAutoPlaySpeed = value;
        }
    }
}
