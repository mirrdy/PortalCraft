using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RayToPlayer : MonoBehaviour
{
    [SerializeField] private Transform rayPoint;
    [SerializeField] private LayerMask layerMask_Ray;
    [SerializeField] private GameObject Third_Cam;
    [SerializeField] private GameObject First_Cam;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private Cinemachine3rdPersonFollow _3rdPersonFollow;

    public enum CameraState { Third = 0, First };
    public CameraState cameraState;
    

    private void Start()
    {
        _3rdPersonFollow = cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        cameraState = CameraState.Third;
    }

    private void Update()
    {
        ChangeCameraState();
        Debug.Log(cameraState);

        if (Input.GetKeyDown(KeyCode.C))
        {
            _3rdPersonFollow.ShoulderOffset = new Vector3(1, 0.46f, 0);
            _3rdPersonFollow.CameraDistance = -0.52f;
        }

        Vector3 rayDir_Third = rayPoint.position - transform.position;
        Debug.DrawRay(transform.position, rayDir_Third * 20f, Color.red);

        if (Physics.Raycast(transform.position, rayDir_Third, out RaycastHit hitInfo, 20f, layerMask_Ray))
        {
            if (hitInfo.transform.CompareTag("Player"))
            {
                Debug.Log("플레이어 감지");
            }
            else
            {
                gameObject.SetActive(false);
                First_Cam.SetActive(true);
                PlayerControl.instance.currentView = PlayerControl.CameraView.FirstPerson;
            }
        }
    }

    private void ChangeCameraState()
    {
        if (Input.GetButtonDown("ViewChange"))
        {
            if (cameraState == CameraState.Third)
            {
                cameraState = CameraState.First;
            }
            else 
            {
                cameraState = CameraState.Third;
            }
        }
    }


}



