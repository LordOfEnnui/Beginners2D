using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenShaker : MonoBehaviour {

    [SerializeField]
    ScreenShakeParams defaultShake;

    [SerializeField]
    bool shaking, shake;

    [SerializeField]
    AnimationCurve shakeCurve;

    [SerializeField]
    Vector3 targetPos;

    [SerializeField]
    float speed;

    private Vector3 cameraInitialPosition, cameraInitialEulerAngles;

    private void Awake() {
        cameraInitialPosition = transform.localPosition;
        cameraInitialEulerAngles = transform.localEulerAngles;
        targetPos = transform.position;
        if (defaultShake == null) defaultShake = ScriptableObject.CreateInstance<ScreenShakeParams>();
    }

    private void Update() {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed * Time.deltaTime);

        if (shake) {
            shake = false;
            StartCoroutine(Shake(defaultShake));
        }
    }

    public void ScreenShake(ScreenShakeParams shake) {
        StartCoroutine(Shake(shake));
    }

    private IEnumerator Shake(ScreenShakeParams shake) {
        if (shaking) yield break;
        shaking = true;
        speed = shake.frequency;
        WaitForSeconds wait = new(1 / speed);
        int numShakes = (int) (shake.duration * shake.frequency);
        Vector3 targetDir = shake.direction;
        targetPos = cameraInitialPosition;

        for (int i = 0; i < numShakes; i++) {
            targetDir = (-targetDir + Random.insideUnitSphere * shake.randomness).normalized;
            targetPos = cameraInitialPosition + shake.maxAmplitude * Mathf.Pow(shakeCurve.Evaluate((float) i / numShakes), shake.exponent) * targetDir;
            yield return wait;
        }
        targetPos = cameraInitialPosition;
        shaking = false;
    }
}
