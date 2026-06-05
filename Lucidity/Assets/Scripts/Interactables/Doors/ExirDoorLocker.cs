using UnityEngine;

public class ExitDoorBlocker : MonoBehaviour
{
    [SerializeField] private Collider blockerCollider;

    private void Awake()
    {
        if (blockerCollider == null)
            blockerCollider = GetComponent<Collider>();

        LockPassage();
    }

    public void LockPassage()
    {
        if (blockerCollider != null)
            blockerCollider.enabled = true;
    }

    public void UnlockPassage()
    {
        if (blockerCollider != null)
            blockerCollider.enabled = false;
    }
}
