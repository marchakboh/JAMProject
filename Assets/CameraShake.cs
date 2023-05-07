using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerable Shake(float dur, float magnet)
    {
        Vector3 origPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * magnet;
            float y = Random.Range(-1f, 1f) * magnet;

            transform.localPosition = new Vector3(x, y, origPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = origPos;
    }
}
