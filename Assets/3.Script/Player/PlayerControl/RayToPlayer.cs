using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayToPlayer : MonoBehaviour
{
    private Input_Info input;
    public GameObject cinemachineCameraTarget_Third; //3ÀÎÄªÅ¸°Ù
    private const float threshold = 0.01f;
    private float cinemachineTargetYaw_Third;
    private float cinemachineTargetPitch_Third;
    private float topClamp_Third = 70.0f;
    private float bottomClamp_Third = -30.0f;
    private void Start()
    {
        input = GetComponent<Input_Info>();
        cinemachineTargetYaw_Third = cinemachineCameraTarget_Third.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (input.cameraLook.sqrMagnitude > threshold)
        {
            cinemachineTargetYaw_Third += input.look.x;
            cinemachineTargetPitch_Third += input.look.y;
        }

        cinemachineTargetYaw_Third = ClampAngle(cinemachineTargetYaw_Third, float.MinValue, float.MaxValue);
        cinemachineTargetPitch_Third = ClampAngle(cinemachineTargetPitch_Third, bottomClamp_Third, topClamp_Third);

        cinemachineCameraTarget_Third.transform.rotation = Quaternion.Euler(cinemachineTargetPitch_Third, cinemachineTargetYaw_Third, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
        {
            lfAngle += 360f;
        }

        if (lfAngle > 360f)
        {
            lfAngle -= 360f;
        }

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
