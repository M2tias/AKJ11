using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField]
    private float shootDelay;
    private float shootTime = 0f;

    [SerializeField]
    private Transform entityContainer;
    [SerializeField]
    private GameObject fireballPrefab;

    private Aiming aiming;

    // Start is called before the first frame update
    void Start()
    {
        aiming = GetComponent<Aiming>();
    }

    // Update is called once per frame
    void Update()
    {
        bool canShootAgain = Time.time - shootTime > shootDelay;
        if (Input.GetKey(KeyCode.Mouse0) && canShootAgain)
        {
            Debug.Log("Piu!");
            shootTime = Time.time;

            GameObject fireballInstance = Instantiate(fireballPrefab);
            fireballInstance.transform.parent = entityContainer;
            Vector3 fp = fireballPrefab.transform.position;
            fireballPrefab.transform.position = new Vector3(fp.x, fp.y, 0);

            Vector3 targetDir = aiming.GetDirection();
            Vector3 spawnPos = transform.position + targetDir.normalized * 0.6f;
            fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir);
        }
    }
}
