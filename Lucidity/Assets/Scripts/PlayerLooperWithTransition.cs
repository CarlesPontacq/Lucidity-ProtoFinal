    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerLooperWithTransition : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private Transform teleportDestination;
        [SerializeField] private Rigidbody playerRb;
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private CameraRotation playerCameraRotationRef;

        [Header("Transition Settings")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float blackHoldDuration = 0.2f;
        [SerializeField] private float fadeInDuration = 0.3f;

        private bool isTeleporting = false;

        private void Awake()
        {
            if (fadeImage != null)
            {
                fadeImage.gameObject.SetActive(true);
                fadeImage.raycastTarget = false;

                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || isTeleporting)
                return;

            isTeleporting = true;
            StartCoroutine(TeleportRoutine());
        }

        private IEnumerator TeleportRoutine()
        {
            if (fadeImage != null)
                yield return FadeAlpha(0f, 1f, fadeOutDuration);

            if (GameManager.Instance.finishedLoops)
            {
                SceneController.Instance.LoadNextScene();
                yield break;    
            }

            if (blackHoldDuration > 0f)
                yield return new WaitForSecondsRealtime(blackHoldDuration);

            playerRb.isKinematic = true;

            float halfHeight = playerCollider.height * 0.5f;
            float centerOffset = playerCollider.center.y;
            Vector3 safePosition = teleportDestination.position + Vector3.up * (halfHeight - centerOffset + 0.02f);

            playerRb.position = safePosition;
            playerRb.rotation = teleportDestination.rotation;

            if (playerCameraRotationRef != null)
                playerCameraRotationRef.SetRotation(teleportDestination.rotation);

            playerRb.isKinematic = false;

            GameManager.Instance.OnExitDoorCrossed();

            if (fadeImage != null)
                yield return FadeAlpha(1f, 0f, fadeInDuration);

            isTeleporting = false;
        }

        private IEnumerator FadeAlpha(float from, float to, float duration)
        {
            float t = 0f;
            Color color = fadeImage.color;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
                color.a = Mathf.Lerp(from, to, a);
                fadeImage.color = color;
                yield return null;
            }

            color.a = to;
            fadeImage.color = color;
        }
    }