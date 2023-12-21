using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

public class FreeLookMobile : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    Image camControlArea;
    [SerializeField] CinemachineFreeLook cmFreeLookCam;
    void Start()
    {
        camControlArea = GetComponent<Image>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(camControlArea.rectTransform, eventData.position, eventData.enterEventCamera, out Vector2 posOut))
        {
            cmFreeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        cmFreeLookCam.m_XAxis.m_InputAxisName = null;
        cmFreeLookCam.m_XAxis.m_InputAxisValue = 0;
    }
}
