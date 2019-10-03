using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] Transform anchor;
    [SerializeField] Transform gravityReference;
    [SerializeField] [Range(0.1f, 10.0f)] float rotationSpeed = 3.0f;
    [SerializeField] [Range(0.1f, 10.0f)] float smoothSpeed = 1.0f;

    Vector3 initialAnchorLocalPosition;
    float xRotation;
    float yRotation;
    Vector3 velocity = Vector3.zero;

    void Start () {
        initialAnchorLocalPosition = anchor.localPosition;
        transform.position = anchor.position;
    }

    void LateUpdate() {
        RotateAnchor();

        Vector3 expectedPosition = anchor.position;
        transform.position = expectedPosition;
        // transform.position = Vector3.SmoothDamp(transform.position, expectedPosition, ref velocity, smoothSpeed);
        transform.LookAt(target, target.up);
        // transform.LookAt(target, transform.position - gravityReference.position);
    }

    void RotateAnchor() {
        float xInput = Input.GetAxis("HCameraAxis") * rotationSpeed;
        float yInput = Input.GetAxis("VCameraAxis") * rotationSpeed;

        if(xInput == 0 && yInput == 0) {
            anchor.localPosition = initialAnchorLocalPosition;
            xRotation = 0;
            yRotation = 0;
        }
        else {
            xRotation = Mathf.Clamp(xRotation + xInput, -90, 90);
            yRotation = Mathf.Clamp(yRotation - yInput, -20, 60);
            anchor.localPosition = Quaternion.Euler(yRotation, xRotation, 0) * initialAnchorLocalPosition;
        }
    }
}
