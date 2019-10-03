using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipControllerWithoutReactors : MonoBehaviour {

    [SerializeField] float maxPower;
    [SerializeField] [Range(0.01f, 3)] float acceleration;
    [SerializeField] SimpleHealthBar powerBar;
    [SerializeField] Text altitude;
    [SerializeField] Text attractorName;
    [SerializeField] Text nearestPlanet;

    float power;
    float powerToHold;
    // what if several attractors at the same time?
    // can it be done without the attractor class, and just with "onTriggerStay"?
    Attractor nearestAttractor;
    float distanceFromNearestAttractor;
    bool isAttracted;
    float reactorNWForce;
    float reactorNEForce;
    float reactorSWForce;
    float reactorSEForce;
    bool rotateLeft;
    bool rotateRight;
    bool propulseForward;

    void Update() {
        float powerToAdd = Input.GetAxis("PowerAxis") * acceleration;
        float xMovement = Input.GetAxis("HMovementAxis");
        float yMovement = Input.GetAxis("VMovementAxis");
        bool holdPower = Input.GetButtonDown("HoldPowerBtn");
        bool releasePower = Input.GetButtonUp("HoldPowerBtn");
        rotateLeft = Input.GetButton("RotateLeftBtn");
        rotateRight = Input.GetButton("RotateRightBtn");
        propulseForward = Input.GetButton("PropulseForwardBtn");

        if(holdPower) {
            powerToHold = power;
            power = 0;
        }
        else if(releasePower) {
            power = powerToHold;
        }
        else {
            power += powerToAdd;
            power = Mathf.Clamp(power, 0, maxPower);
        }
        float force = power / 10;

        reactorNWForce = force;
        float reactorNWBrake = -xMovement + yMovement;
        if(reactorNWBrake > 0) {
            reactorNWForce -= reactorNWBrake;
            if(reactorNWForce < 0) {
                reactorNWForce = 0;
            }
        }

        reactorNEForce = force;
        float reactorNEBrake = xMovement + yMovement;
        if(reactorNEBrake > 0) {
            reactorNEForce -= reactorNEBrake;
            if(reactorNEForce < 0) {
                reactorNEForce = 0;
            }
        }

        reactorSWForce = force;
        float reactorSWBrake = -xMovement + (-yMovement);
        if(reactorSWBrake > 0) {
            reactorSWForce -= reactorSWBrake;
            if(reactorSWForce < 0) {
                reactorSWForce = 0;
            }
        }

        reactorSEForce = force;
        float reactorSEBrake = xMovement + (-yMovement);
        if(reactorSEBrake > 0) {
            reactorSEForce -= reactorSEBrake;
            if(reactorSEForce < 0) {
                reactorSEForce = 0;
            }
        }
    }

    void FixedUpdate() {
        Rigidbody rb = this.GetComponent<Rigidbody>();
        rb.AddForceAtPosition(transform.up * (reactorNWForce), new Vector3(-16, 1020, 30));
        rb.AddForceAtPosition(transform.up * (reactorNEForce), new Vector3(16, 1020, 30));
        rb.AddForceAtPosition(transform.up * (reactorSWForce), new Vector3(-25, 1020, -25));
        rb.AddForceAtPosition(transform.up * (reactorSEForce), new Vector3(25, 1020, -25));

        if(rotateLeft) {
            transform.Rotate(0, -1, 0);
        }
        if(rotateRight) {
            transform.Rotate(0, 1, 0);
        }
        if(propulseForward) {
            transform.Translate(0, 0, 2);
        }
        if(!isAttracted) {
            // execute at larger intervals?
            distanceFromNearestAttractor = GetDistanceFromNearestAttractor();
        }
        else {
            distanceFromNearestAttractor = GetDistanceFromAttractor(nearestAttractor);
        }
    }

    void LateUpdate() {
        powerBar.UpdateBar(power, maxPower);
        altitude.text = "Altitude : " + distanceFromNearestAttractor.ToString("0.0");
        if(!isAttracted) {
            attractorName.text = "In space";
            nearestPlanet.text = nearestAttractor.name;
            nearestPlanet.gameObject.SetActive(true);
        }
        else {
            attractorName.text = "Currently on " + nearestAttractor.name;
            nearestPlanet.gameObject.SetActive(false);
        }
    }

    float GetDistanceFromNearestAttractor() {
        float minDistance = float.MaxValue;
        foreach(Attractor attractor in Attractor.getAttractors()) {
            float distance = GetDistanceFromAttractor(attractor);
            if(distance < minDistance) {
                minDistance = distance;
                nearestAttractor = attractor;
            }
        }
        return minDistance;
    }

    float GetDistanceFromAttractor(Attractor attractor) {
        float distance = (
                transform.position -
                attractor.GetComponent<Collider>().ClosestPoint(transform.position)
        ).magnitude;
        return distance;
    }

    public void setAttracted(bool isAttracted) {
        this.isAttracted = isAttracted;
    }
}
