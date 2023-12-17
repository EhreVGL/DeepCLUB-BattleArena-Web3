using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MinimapCamFollow : MonoBehaviour
{
    public Transform target; // Oyuncu karakteri
    public Vector3 avatarOffset, desiredPosition;
    public float smoothSpeed = 0.125f;
    void Update()
    {
        MinimapFollow();
    }
    void MinimapFollow()
    {
        if (target != null)
        {
            if (ServerControl.server.mainAvatar != null)
            {
                desiredPosition = target.position + avatarOffset;
            }
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * 10);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Euler(90, ServerControl.server.cMFreeLook.GetComponent<CinemachineFreeLook>().m_XAxis.Value, 0);
        }
    }
}
