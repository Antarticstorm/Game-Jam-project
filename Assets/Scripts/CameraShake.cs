using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 shakeOffset = Vector3.zero;

    public Vector3 GetShakeOffset()
    {
        return shakeOffset;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}