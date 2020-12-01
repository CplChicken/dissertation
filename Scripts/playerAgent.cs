using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MLAgents;

public class playerAgent : Agent
{
    private float jumpHeight = 3f;
    private float jumpSpeed = 0.15f;
    private float rotationSpeed = 6.5f;
    public passArea passScript;
    public bulletDetector bulletScript;
    public GameObject spawn;
    public GameObject bullet;
    private float ammo = 4;
    private gameArea gameArea;
    private RayPerception2D rayPerception;
    private bool grounded = true;
    private bool jumping = false;
    private bool shoot = false;
    private float shootCooldown = 0f;
    private float ammoCooldown = 20f;
    private Rigidbody2D rigidBody;
    private GameObject[] spike;
    private float minDistance;
    private float distance = 1000;
    
    public override void AgentAction(float[] vectorAction, string textAction) {
        
        if (vectorAction[0] == 1f && grounded) {
            grounded = false;
            jumping = true;
        }
        else if (vectorAction[0] == 0f && !grounded) {
            jumping = false;
        }

        if (vectorAction[1] == 1f) {
            shoot = true;
        }
        else if (vectorAction[1] == 0f) {
            shoot = false;
        }

        AddReward(-1f / agentParameters.maxStep);
    }

    public override void AgentReset() {
        jumpHeight = 3f;
        jumpSpeed = 0.15f;
        rotationSpeed = 6.5f;
        ammo = 4f;
        ammoCooldown = 20f;
        shootCooldown = 0f;
        shoot = false;
        grounded = true;
        jumping = false;
        transform.position = spawn.transform.position;
        transform.rotation = Quaternion.identity;
        gameArea.ResetArea();
    }

    public override void CollectObservations() {
        AddVectorObs(grounded);

        AddVectorObs(jumping);

        float rayDistance = 10f;
        float[] rayAngles = { 270f, 300f, 330, 0f, 30f, 60f, 90f};
        string[] detectableObjects = {"Spike"};
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        AddVectorObs(minDistance);

        AddVectorObs(gameArea.GetComponent<gameArea>().groundSpeed);

        AddVectorObs(transform.position.y);

        AddVectorObs(ammo);

        AddVectorObs(shootCooldown);
    }

    private void Start() {
        gameArea = GetComponentInParent<gameArea>();
        rayPerception = GetComponent<RayPerception2D>();
    }
    
    void FixedUpdate()
    {
        spike = GameObject.FindGameObjectsWithTag("Spike");
        distance = 1000;
        foreach (GameObject aSpike in spike) {
            if (Vector2.Distance(transform.position, aSpike.transform.position) < distance) {
                distance = Vector2.Distance(transform.position, aSpike.transform.position);
            }
        }
        minDistance = distance;
        
        jumpSpeed = gameArea.GetComponent<gameArea>().groundSpeed * 0.75f;
        rotationSpeed = jumpSpeed * 43;

        if (!grounded && jumping) {
            transform.position += new Vector3(0, jumpSpeed, 0);
            if (transform.rotation.eulerAngles.z >= -90) {
                transform.Rotate(0, 0, -rotationSpeed);
            }
        }

        if (transform.position.y >= jumpHeight) {
            jumping = false;
        }

        if (!grounded && !jumping && transform.position.y >= (0 + jumpSpeed) ) {
            transform.position -= new Vector3(0, jumpSpeed, 0);
            if (transform.rotation.eulerAngles.z >= -180) {
                transform.Rotate(0, 0, -rotationSpeed);
            }
        }
        else if (transform.position.y < (0 + jumpSpeed)) {
            grounded = true;
            transform.rotation = Quaternion.identity;
        }

        shootCooldown -= gameArea.GetComponent<gameArea>().groundSpeed;
        ammoCooldown -= gameArea.GetComponent<gameArea>().groundSpeed;

        if (shoot && shootCooldown <= 0f && ammo > 0f) {
            GameObject newBullet = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
            bulletBehavior bulletScript = newBullet.GetComponent<bulletBehavior>();
            bulletScript.script = gameArea.transform.GetChild(9).gameObject.GetComponent<bulletDetector>();
            ammo -= 1f;
            shootCooldown = 5f;
        }

        if (ammoCooldown <= 0f && ammo < 4) {
            ammo += 1;
            ammoCooldown = 20f;
        }

        if (passScript.triggered) {
            AddReward(1f);
            passScript.triggered = false;
        }

        if (bulletScript.hit) {
            AddReward(1f);
            bulletScript.hit = false;
        }
    }

    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.tag == "Spike") {
            AddReward(-1f);
            AgentReset();
        }
    }

}