using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeLookWeb : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook cmFreeLookCam;
    [SerializeField] float zoomSpeed = 50f;
    [SerializeField] float minZoom = 10f;
    [SerializeField] float maxZoom = 60f;
    void Update()
    {
        float scrollWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");

        float newZoom = cmFreeLookCam.m_Lens.FieldOfView - scrollWheelInput * zoomSpeed;
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        cmFreeLookCam.m_Lens.FieldOfView = newZoom;
        if (Input.GetMouseButton(2))
        {
            cmFreeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
        }
        if (Input.GetMouseButtonUp(2))
        {
            cmFreeLookCam.m_XAxis.m_InputAxisName = null;
            cmFreeLookCam.m_XAxis.m_InputAxisValue = 0;
        }
    }
}
