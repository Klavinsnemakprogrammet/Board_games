using System.Collections;
using UnityEngine;

public class DiceAnimationScript : MonoBehaviour
{
    private static readonly int IsRolling = Animator.StringToHash("isRolling");
    private Animator animator;
    private Coroutine rollRoutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShakeDice()
    {
        if (rollRoutine != null)
        {
            return;
        }

        animator.SetBool(IsRolling, true);
        rollRoutine = StartCoroutine(StopAfterRoll());
    }

    private IEnumerator StopAfterRoll()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DiceRoll"))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        animator.SetBool(IsRolling, false);
        rollRoutine = null;
    }
}
