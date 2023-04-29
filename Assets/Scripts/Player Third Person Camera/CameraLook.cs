using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraLook : MonoBehaviour
{
    [SerializeField] private float LookSpeed_X = 200.0f;
    [SerializeField] private float LookSpeed_Y = 1.0f;

    private CinemachineFreeLook cinemachine;
    private PlayerActions playerActions;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineFreeLook>();
        playerActions = new PlayerActions();
    }

    void OnEnable()
    {
        playerActions.Enable();
    }

    void OnDisable()
    {
        playerActions.Disable();
    }

    void Update()
    {
        Vector2 delta = playerActions.Controls.Look.ReadValue<Vector2>().normalized;
        if (delta != Vector2.zero)
        {
            cinemachine.m_XAxis.Value += delta.x * LookSpeed_X * Time.deltaTime;
            cinemachine.m_YAxis.Value += delta.y * LookSpeed_Y * Time.deltaTime;
        }
    }
}
