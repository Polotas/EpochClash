using UnityEngine;

public class UnitView : MonoBehaviour
{
    public Animator animator;
    public GameObject visual;
    public AudioClip audioHit;
    
    public void SpeedAnimation(float value) => animator.SetFloat("Speed",value);
    
    public void StartAnimation() => animator.Play("CavMan");

    public void CallAttack() => animator.Play("Attack");
    
    public void Dead() => animator.Play("Dead");
}
