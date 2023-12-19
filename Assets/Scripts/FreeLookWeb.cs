using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeLookWeb : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook cmFreeLookCam;
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            cmFreeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            cmFreeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        if (Input.GetMouseButtonUp(2))
        {
            cmFreeLookCam.m_XAxis.m_InputAxisName = null;
            cmFreeLookCam.m_YAxis.m_InputAxisName = null;
            cmFreeLookCam.m_XAxis.m_InputAxisValue = 0;
            cmFreeLookCam.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
