using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlling : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float yLimit = 100f;
    [SerializeField] private float sensitivity = 1f;
    private float currentX = 0f;
    private float currentY = 0f;
    public int cameraFov = 60;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam.fieldOfView = cameraFov;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        currentX += mouseX;
        currentY -= mouseY;
        currentY = Mathf.Clamp(currentY, -yLimit, yLimit);

        cam.transform.localRotation = Quaternion.Euler(currentY, currentX, 0f);
    }
}
