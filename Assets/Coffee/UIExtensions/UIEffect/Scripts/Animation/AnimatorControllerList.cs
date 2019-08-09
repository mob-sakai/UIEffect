using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatorControllerList", menuName = "ScriptableUIEffect/AnimationControllerList", order = 1)]
public class AnimatorControllerList : ScriptableObject
{
    public List<ControllerItem> AnimatorControllers = null;

    public ControllerItem GetAnimatorController(string Id)
    {
        ControllerItem animatorController = default;

        IEnumerable<ControllerItem> items = AnimatorControllers.Where(ci => ci.Id == Id);
        if (items.Count() > 0)
        {
            animatorController = items.First();
        }

        return animatorController;
    }
}
