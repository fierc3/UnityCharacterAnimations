using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class PoetAnimationController : MonoBehaviour
{
    private Animator _animator;
    private const string IDLE_BOOL = "Idle";
    private const string RUNFORWARD_BOOL = "RunForward";

    [SerializeField]
    private LookTargetFinder lookTargetFinder;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void AnimateIdle()
    {
        ActivateSingleAnimation(IDLE_BOOL);
    }

    public void AnimateRunForward()
    {
        ActivateSingleAnimation(RUNFORWARD_BOOL);
    }

    public void ToggleLookAt()
    {
        if (lookTargetFinder)
        {
            lookTargetFinder.ToggleLooking();
        }
    }

    private void ActivateSingleAnimation(string animParameter)
    {
        // Activate only Parameter which name matches animParameter
        _animator.parameters.ToList().ForEach(param => { _animator.SetBool(param.name, param.name.Equals(animParameter)); });
    }

}
