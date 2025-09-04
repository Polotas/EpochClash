using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name);
        }
    }

    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void PlayMoveAnimation(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

    public void PlayDieAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    // Add more animation control methods as needed
}