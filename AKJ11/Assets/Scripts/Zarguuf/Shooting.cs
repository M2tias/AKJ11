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
    private GameObject fireballPrefab; // default attack spell prefab
    [SerializeField]
    private GameObject wallPrefab;

    // Wall spell placement visuals
    [SerializeField]
    private GameObject ghostWallPos;
    [SerializeField]
    private GameObject ghostWallRotation;
    private Vector3 wallSpawnPos;

    private Aiming aiming;

    private Spell currentSpell = Spell.fireball;

    // dynamic spell prefabs
    private GameObject attackSpell1;
    private GameObject attackSpell2;


    // Start is called before the first frame update
    void Start()
    {
        aiming = GetComponent<Aiming>();

        attackSpell1 = Instantiate(fireballPrefab);
        Fireball spell1 = attackSpell1.GetComponent<Fireball>();
        spell1.SetConfig(10, 2, 0, 0, 0);
        attackSpell1.SetActive(false);

        attackSpell2 = Instantiate(fireballPrefab);
        Fireball spell2 = attackSpell2.GetComponent<Fireball>();
        spell2.SetConfig(5, 0, 0, 5, 5);
        attackSpell2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool canShootAgain = Time.time - shootTime > shootDelay;
        if (Input.GetKey(KeyCode.Mouse0) && canShootAgain && currentSpell == Spell.fireball)
        {
            Debug.Log("Piu!");
            shootTime = Time.time;

            GameObject fireballInstance = Instantiate(attackSpell1);
            fireballInstance.transform.parent = entityContainer;
            Vector3 fp = attackSpell1.transform.position;
            attackSpell1.transform.position = new Vector3(fp.x, fp.y, 0);
            fireballInstance.SetActive(true);

            Vector3 targetDir = aiming.GetDirection();
            Vector3 spawnPos = transform.position + targetDir.normalized * 0.6f;
            fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir);
        }
        else if (Input.GetKey(KeyCode.Mouse1) && canShootAgain && currentSpell == Spell.fireball)
        {
            Debug.Log("Pau!");
            shootTime = Time.time;

            GameObject fireballInstance = Instantiate(attackSpell2);
            fireballInstance.transform.parent = entityContainer;
            Vector3 fp = attackSpell2.transform.position;
            attackSpell2.transform.position = new Vector3(fp.x, fp.y, 0);
            fireballInstance.SetActive(true);

            Vector3 targetDir = aiming.GetDirection();
            Vector3 spawnPos = transform.position + targetDir.normalized * 0.6f;
            fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && currentSpell == Spell.wall)
        {
            Debug.Log("Wall visible");
            ghostWallRotation.SetActive(true);
            ghostWallPos.SetActive(true);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            wallSpawnPos = new Vector3(worldPosition.x, worldPosition.y, 0);
            ghostWallPos.transform.position = wallSpawnPos;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && currentSpell == Spell.wall)
        {
            GameObject wallInstance = Instantiate(wallPrefab);
            wallInstance.transform.parent = entityContainer;
            Vector3 fp = fireballPrefab.transform.position;
            fireballPrefab.transform.position = new Vector3(fp.x, fp.y, 0);

            Vector3 targetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ghostWallPos.transform.position;
            Vector3 spawnPos = wallSpawnPos;
            wallInstance.GetComponent<Wall>().Initialize(spawnPos, targetDir);

            // after casting
            SetCurrentSpell(Spell.fireball);
            ghostWallPos.SetActive(false);
        }

        if (currentSpell == Spell.wall)
        {
            Vector3 targetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ghostWallPos.transform.position;
            float angleDiff = Vector2.SignedAngle(ghostWallRotation.transform.right, targetDir);
            ghostWallRotation.transform.Rotate(Vector3.forward, angleDiff);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetCurrentSpell(Spell.wall);
        }
    }

    public Spell GetCurrentSpell()
    {
        return currentSpell;
    }

    private void SetCurrentSpell(Spell spell)
    {
        if (currentSpell == spell)
        {
            currentSpell = Spell.fireball;
        }
        else
        {
            currentSpell = spell;
        }

        aiming.SetReticule(currentSpell);
    }
}
