using UnityEngine;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private float blendSmoothSpeed = 10f;
    private float currentBlend;

    private void Update()
    {
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        animator.SetBool("IsMoving", playerMovement.IsMoving);

        float targetBlend = playerMovement.IsRunning ? 1f : 0f;
        currentBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * blendSmoothSpeed);
        animator.SetFloat("Blend", currentBlend);
    }
}