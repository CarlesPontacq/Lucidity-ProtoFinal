using UnityEngine;
using UnityEngine.Android;

public class PlayerArmsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CameraManager cameraManager;

    [SerializeField] private GameObject cameraModelObject;
    [SerializeField] private GameObject armsMeshObject;

    [SerializeField] private float blendSmoothSpeed = 10f;
    private float currentBlend;

    private bool hidden = true;

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

    public void PlayRaiseCamera()
    {
        animator.SetTrigger("RaiseCamera");
    }

    public void PlayLowerCamera()
    {
        animator.SetTrigger("LowerCamera");
    }

    public void CameraRaised()
    {
        cameraManager.OnCameraRaised();
    }

    public void CameraLowered()
    {
        cameraManager.OnCameraLowered();
    }

    public void HideArms()
    {
        hidden = true;
        cameraModelObject.SetActive(false);
        armsMeshObject.SetActive(false);
    }

    public void ShowArms()
    {
        cameraModelObject.SetActive(true);
        armsMeshObject.SetActive(true);
    }
}