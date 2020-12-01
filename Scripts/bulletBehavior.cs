using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    public bulletDetector script;
    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.tag == "Spike") {
            Destroy(gameObject);
            if (trigger.gameObject.name == "Spike2(Clone)") {
                script.hit = true;
                Destroy(trigger.gameObject);
                Destroy(gameObject);
            } 
        }
    }
}
