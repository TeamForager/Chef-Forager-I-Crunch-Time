using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocateCamera : MonoBehaviour
{
    public Transform currentPlayerTransform;
    public float scale = 1.0f;
    public float minCameraDistance = 1.0f;
    public float maxCameraDistance = 10.0f;

    Vector3 initialPlayerLocation;
    Vector3 initialCameraLocation;

    private bool isShaking = false;
    private float shakeAmount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        initialCameraLocation = transform.position;
        initialPlayerLocation = currentPlayerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPlayerLocation = currentPlayerTransform.position;
        if (Input.mouseScrollDelta.y != 0) {
            Vector3 playerToCamera = initialCameraLocation - initialPlayerLocation;
            float distance = playerToCamera.magnitude;
            float zoomChange = -Input.mouseScrollDelta.y * scale;
            if ((distance + zoomChange > minCameraDistance) && (distance + zoomChange < maxCameraDistance)) {
                playerToCamera = playerToCamera * (distance + zoomChange) / distance;
                initialCameraLocation = initialPlayerLocation + playerToCamera;
            }
        }

        float xOffset = currentPlayerLocation.x - initialPlayerLocation.x;
        float yOffset = 0.0f;
        float zOffset = currentPlayerLocation.z - initialPlayerLocation.z;

        if (isShaking) {
            xOffset = xOffset + Random.Range(-shakeAmount / 2, shakeAmount / 2);
            yOffset = 0.0f;
            zOffset = zOffset + Random.Range(-shakeAmount / 2, shakeAmount / 2);
        }

        transform.position = new Vector3(
            initialCameraLocation.x + xOffset,
            initialCameraLocation.y + yOffset,
            initialCameraLocation.z + zOffset);
    }

    public void SetShake(float shakeAmt)
    {
        isShaking = true;
        shakeAmount = shakeAmt;
    }
}
