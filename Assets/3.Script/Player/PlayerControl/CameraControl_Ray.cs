using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl_Ray : MonoBehaviour
{
    //Ä«¸Þ¶ó ½ÃÁ¡ ¿­°ÅÇü
    public enum CameraView { ThirdPerson = 0, FirstPerson = 1 };
    public CameraView cameraView = CameraView.ThirdPerson;

    public Input_Camera input_Camera;

    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask layerMask_Ray;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private Cinemachine3rdPersonFollow _3rdPersonFollow;
    [SerializeField] private Transform followTarget;

    private float topClamp_Third = 80.0f;
    private float bottomClamp_Third = -60.0f;
    private const float threshold = 0.01f;

    private float cinemachineTargetYaw_Third;
    private float cinemachineTargetPitch_Third;

    public GameObject cinemachineCameraTarget_Third; //3ÀÎÄªÅ¸°Ù


    private void Start()
    {
        cinemachineCamera = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineVirtualCamera>();

        followTarget = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1);
        GetComponent<CinemachineVirtualCamera>().Follow = followTarget;

        input_Camera = GetComponent<Input_Camera>();

        _3rdPersonFollow = cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        cinemachineCameraTarget_Third = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).gameObject;
        rayPoint = GameObject.FindGameObjectWithTag("RayPoint").transform;

        cameraView = CameraView.ThirdPerson;
    }

    void Update()
    {
        VeiwChange();

        Vector3 rayDir = rayPoint.position - transform.position;
        Debug.DrawRay(transform.position, rayDir, Color.red);
        switch (cameraView)
        {
            case CameraView.ThirdPerson:
                {
                    if (Physics.Raycast(transform.position, rayDir, out RaycastHit hitInfo, layerMask_Ray) && !PlayerControl.instance.isDodging)
                    {
                        if (hitInfo.transform.CompareTag("Block")) //3ÀÎÄª¿¡¼­ -> 1ÀÎÄª
                        {
                            _3rdPersonFollow.ShoulderOffset = new Vector3(-0.1f, 0.8f, 0);
                            _3rdPersonFollow.CameraDistance = -0.94f;
                        }
                        if (hitInfo.transform.CompareTag("Player")) //1ÀÎÄª¿¡¼­ -> 3ÀÎÄª
                        {
                            _3rdPersonFollow.ShoulderOffset = new Vector3(1, 1.6f, 0);
                            _3rdPersonFollow.CameraDistance = 8f;
                        }
                    }
                    break;
                }
            case CameraView.FirstPerson:
                {
                    break;
                }
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void VeiwChange()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (cameraView == CameraView.ThirdPerson)
            {
                cameraView = CameraView.FirstPerson;
            }
            else if (cameraView == CameraView.FirstPerson)
            {
                cameraView = CameraView.ThirdPerson;
            }
        }
    }

    private void CameraRotation()
    {
        if (input_Camera.look.sqrMagnitude > threshold)
        {
            cinemachineTargetYaw_Third += input_Camera.look.x;
            cinemachineTargetPitch_Third += input_Camera.look.y;
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
