using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTrhowObjetct : StateMachineBehaviour
{

    public bool isThrowing;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isThrowing)
        {
            PlayerInventory pGunEquipped = animator.GetComponentInParent<PlayerInventory>();
            if (pGunEquipped.gunEquipped == null) return;

            pGunEquipped.gunEquipped.SetActive(false);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerInventory pGunEquipped = animator.GetComponentInParent<PlayerInventory>();
        if (pGunEquipped.gunEquipped == null) return;

        pGunEquipped.gunEquipped.SetActive(true);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
