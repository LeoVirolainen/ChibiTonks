using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; // Singleton instance

    public Transform cam;
    private Vector3 originalPosition;
    private float originalRotation;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        originalPosition = cam.transform.localPosition;
        originalRotation = cam.transform.localRotation.z;
    }

    void LateUpdate()
    {
        originalPosition = cam.transform.localPosition;
        originalRotation = cam.transform.localRotation.z;
        if (shakeDuration > 0)
        {
            cam.transform.localPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude * shakeDuration;
            cam.transform.localRotation = Quaternion.Euler(0, 0, originalRotation + Random.Range(-2.5f, 2.5f) * shakeMagnitude * shakeDuration);

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
