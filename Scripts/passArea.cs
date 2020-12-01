using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passArea : MonoBehaviour
{
    public bool triggered;
    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.tag == "Spike") {
            triggered = true;
        }
    }
}
