using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using TMPro;

public class gameArea : Area
{
    public playerAgent playerAgent;
    public TextMeshProUGUI rewardText;
    public GameObject spikePrefab;
    public GameObject spike2Prefab;
    public GameObject floor;
    public GameObject despawn;
    public GameObject spawn;
    public GameObject bulletDespawn;
    public float groundSpeed = 0;
    public float difficulty;
    public float spawnTimer = 0;
    private float rand;
    private float rand2;
    private int spikeNumber = 1;
    private int spike2Number = 1;
    private float spikeTimer = 0;
    private float spike2Timer = 0;
    private const float SPAWN_RATE = 1.34f;

    public override void ResetArea()
    {
        RemoveAllSpikes();
        spawnTimer = 0;
        spikeTimer = 0;
        spike2Timer = 0;
        for (int i = 0; i < 3; i++)
        {
            Instantiate(spikePrefab, new Vector3((spawn.transform.position.x + (1.34f * i)), spawn.transform.position.y, 0), Quaternion.identity);
            rand = Random.Range(0, 2);
            if (rand == 0)
            {
                break;
            }
        }
    }

    private void RemoveAllSpikes()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(floor.transform.position, 30);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D collider = colliders[i];
            GameObject colliderObject = collider.transform.gameObject;

            if (colliderObject.name.Equals("Spike(Clone)"))
            {
                Destroy(colliderObject);
            }

            if (colliderObject.name.Equals("Spike2(Clone)"))
            {
                Destroy(colliderObject);
            }
        }
    }

    void FixedUpdate()
    {
        spawnTimer += groundSpeed;
        rand = Random.Range(0, 10);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(floor.transform.position, 30);

        if (difficulty == 1) {
            spikeNumber = 1;
            spike2Number = 1;
        }
        else if (difficulty == 2) {
            spikeNumber = 2;
            spike2Number = 2;
        }
        else if (difficulty == 3) {
            spikeNumber = 3;
            spike2Number = 3;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D collider = colliders[i];
            GameObject colliderObject = collider.transform.gameObject;

            if (colliderObject.tag == "Spike")
            {
                GameObject aSpike = colliderObject;

                aSpike.transform.position = aSpike.transform.position - new Vector3(groundSpeed, 0, 0);
                if (spawnTimer >= SPAWN_RATE)
                {
                    spikeTimer += 1;
                    if (spikeTimer >= (6 + rand))
                    {
                        rand2 = Random.Range(0, 3);
                        if (spike2Timer >= (3 + rand2)) {
                            spike2Timer = 0;
                            rand = Random.Range(spike2Number, 3);
                            for (int k = 0; k < rand; k++) {
                                Instantiate(spike2Prefab, new Vector3((spawn.transform.position.x + (3.333f * k)), (spawn.transform.position.y + 0.9815f), 0), Quaternion.identity);
                            }
                        }
                        else {
                            rand = Random.Range(spikeNumber, 3);
                            for (int j = 0; j < rand; j++)
                            {
                                Instantiate(spikePrefab, new Vector3((spawn.transform.position.x + (1.34f * j)), spawn.transform.position.y, 0), Quaternion.identity);
                            }
                        }
                        spike2Timer += 1;
                        spikeTimer = 0;
                    }
                    spawnTimer = 0;
                }
                if (aSpike.transform.position.x <= despawn.transform.position.x)
                {
                    Destroy(aSpike);
                }
            }
            else if (colliderObject.tag == "Bullet") {
                GameObject aBullet = colliderObject;

                aBullet.transform.position = aBullet.transform.position + new Vector3(groundSpeed, 0, 0);

                if (aBullet.transform.position.x >= bulletDespawn.transform.position.x) {
                    Destroy (aBullet);
                }
            }
        }


        rewardText.text = playerAgent.GetCumulativeReward().ToString("0.00");
    }
}
