using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {

    // earth : 6 371 km radius / 6 x 10^24 kg
    // my planet : 1010 m radius (sort of imposed by the mesh) -> 9.51 x 10^20 kg to keep earth ratio
    // 6307.9 x smaller so G must be divided by that number to get same gravity -> 1.057 x 10^-14
    // then G and planet mass are balanced to fit Unity limits (doesn't change the math)
    const float G = 1057f;
    static List<Attractor> Attractors;
    [SerializeField] List<GameObject> players;
    Rigidbody rb;
    List<Rigidbody> targets;

    void Start() {
        rb = GetComponent<Rigidbody>();
        targets = new List<Rigidbody>();
    }

    void OnEnable() {
        if(Attractors == null) {
            Attractors = new List<Attractor>();
        }
        Attractors.Add(this);
    }

    void OnDisable() {
        Attractors.Remove(this);
    }

    void FixedUpdate() {
        foreach(Rigidbody target in targets) {
            Vector3 direction = rb.position - target.position;
            float distance = direction.magnitude;

            float forceMagnitude = G * rb.mass * target.mass / Mathf.Pow(distance, 2);
            Vector3 force = direction.normalized * forceMagnitude;

            target.AddForce(force);
        }
    }

    void OnTriggerEnter(Collider target) {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        targets.Add(targetRigidbody);
        if(players.Contains(target.gameObject)) {
            target.GetComponent<ShipControllerWithoutReactors>().setAttracted(true);
        }
    }

    void OnTriggerExit(Collider target) {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        targets.Remove(targetRigidbody);
        if(players.Contains(target.gameObject)) {
            target.GetComponent<ShipControllerWithoutReactors>().setAttracted(false);
        }
    }

    public static List<Attractor> getAttractors() {
        return Attractors;
    }
}
