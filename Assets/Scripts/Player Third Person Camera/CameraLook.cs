using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraLook : MonoBehaviour
{
    [SerializeField] private float LookSpeed_X = 200.0f;
    [SerializeField] private float LookSpeed_Y = 1.0f;

    private CinemachineFreeLook cinemachine;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineFreeLook>();
    }

    public void UpdateDelta(Vector2 delta)
    {
        if (delta != Vector2.zero)
        {
            cinemachine.m_XAxis.Value += delta.x * LookSpeed_X * Time.deltaTime;
            cinemachine.m_YAxis.Value += delta.y * LookSpeed_Y * Time.deltaTime;
        }
    }
}
