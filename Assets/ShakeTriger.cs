using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTriger : MonoBehaviour
{
    private CameraShake camh;

    public void Shake()
    {
        StartCoroutine((string)camh.Shake(.15f, .4f));
    }
}
