using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private EntityPlayer m_player;

    public Vector3 currentCameraRot;

    public Camera playerCamera { get { return transform.GetComponentInChildren<Camera>(); } }

    public void SetupCameraLook(EntityPlayer player)
    {
        // setup player id, setup string input
        m_player = player;
    }

    private void Update()
    {
        if (!m_player || m_player.deadTurnLeft > 0)
            return; 

        float moveH = Input.GetAxisRaw("Horizontal" + " #" + m_player.playerId);
        float moveV = Input.GetAxisRaw("Vertical" + " #" + m_player.playerId);
        float camH = Input.GetAxis("Camera X" + " #" + m_player.playerId);
        float camV = Input.GetAxis("Camera Y" + " #" + m_player.playerId);
        bool moveMod = Input.GetButton("Move Modifier" + " #" + m_player.playerId);
        bool camMod = Input.GetButton("Camera Modifier 1" + " #" + m_player.playerId) || Input.GetButton("Camera Modifier 2" + " #" + m_player.playerId);

        Vector2 camRot = (camMod) ? new Vector2(moveH, moveV) : new Vector2(camH, camV);
        _HandleCameraInput(camRot.x, camRot.y, moveMod);
    }

    private void _HandleCameraInput(float camH, float camV, bool moveMod)
    {
        currentCameraRot.y = Mathf.Repeat(currentCameraRot.y + camH, 360);
        currentCameraRot.x = Mathf.Clamp(currentCameraRot.x - camV, -60.0f, 90.0f);

        transform.rotation = Quaternion.Euler(currentCameraRot.x, currentCameraRot.y, 0.0f);
    }
}
