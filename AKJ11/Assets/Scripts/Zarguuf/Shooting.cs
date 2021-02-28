using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    [SerializeField]
    private Transform entityContainer;
    [SerializeField]
    private GameObject fireballPrefab; // default attack spell prefab
    [SerializeField]
    private GameObject wallPrefab;
    [SerializeField]
    private SpellBaseConfig attackSpell1Config;
    [SerializeField]
    private SpellBaseConfig attackSpell2Config;
    [SerializeField]
    private SpellBaseConfig spellWallConfig;

    //private Experience playerExperience;

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
    private float attack1CD;
    private float attack1Used = -100f;
    private float attack2CD;
    private float attack2Used = -100f;
    private float spellWallCD;
    private float spellWallUsed = -100f;

    // Start is called before the first frame update
    void Start()
    {
        //playerExperience = GetComponent<Experience>();
        aiming = GetComponent<Aiming>();
        SetupSpells();
    }

    private void SetupSpells()
    {
        attackSpell1 = Instantiate(fireballPrefab);
        Fireball spell1 = attackSpell1.GetComponent<Fireball>();
        spell1.SetConfig(attackSpell1Config);
        attack1CD = attackSpell1Config.Cooldown[0];

        attackSpell1.SetActive(false);

        attackSpell2 = Instantiate(fireballPrefab);
        Fireball spell2 = attackSpell2.GetComponent<Fireball>();
        spell2.SetConfig(attackSpell2Config);
        attack2CD = attackSpell2Config.Cooldown[0];
        attackSpell2.SetActive(false);

        spellWallCD = spellWallConfig.Cooldown[0];
    }

    // Update is called once per frame
    void Update()
    {
        attack1CD = attackSpell1Config.Cooldown[Experience.main.GetSpell1Runtime().CooldownLevel];
        attack2CD = attackSpell2Config.Cooldown[Experience.main.GetSpell2Runtime().CooldownLevel];
        spellWallCD = spellWallConfig.Cooldown[Experience.main.GetSpellWallRuntime().CooldownLevel];
//        Debug.Log(Experience.main.GetSpellWallRuntime().CooldownLevel);
        bool canShoot1Again = Time.time - attack1Used > attack1CD;
        bool canShoot2Again = Time.time - attack2Used > attack2CD;
        bool canSpellWallAgain = Time.time - spellWallUsed > spellWallCD;
        if (Input.GetKey(KeyCode.Mouse0) && canShoot1Again && currentSpell == Spell.fireball)
        {
            ShootSpell1();
        }
        else if (Input.GetKey(KeyCode.Mouse1) && canShoot2Again && currentSpell == Spell.fireball)
        {
            ShootSpell2();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && currentSpell == Spell.wall)
        {
            ShowGhostWall();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && currentSpell == Spell.wall)
        {
            PlaceSpellWall();
        }

        if (currentSpell == Spell.wall)
        {
            RotateGhostWall();
        }

        if (Input.GetKeyDown(KeyCode.Q) && canSpellWallAgain)
        {
            SetCurrentSpell(Spell.wall);
        }
    }

    // Show visualisation for wall placement
    private void ShowGhostWall()
    {
        ghostWallRotation.SetActive(true);
        ghostWallPos.SetActive(true);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        wallSpawnPos = new Vector3(worldPosition.x, worldPosition.y, 0);
        ghostWallPos.transform.position = wallSpawnPos;
    }

    private void PlaceSpellWall()
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
        spellWallUsed = Time.time;
        if (SpellBar.main != null)
        {
            SpellBar.main.SpellWasCast(spellWallConfig);
        }
    }

    private void RotateGhostWall()
    {
        Vector3 targetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ghostWallPos.transform.position;
        float angleDiff = Vector2.SignedAngle(ghostWallRotation.transform.right, targetDir);
        ghostWallRotation.transform.Rotate(Vector3.forward, angleDiff);
    }

    private void ShootSpell1()
    {
        attack1Used = Time.time;

        GameObject fireballInstance = Instantiate(attackSpell1);
        fireballInstance.transform.parent = entityContainer;
        Vector3 fp = attackSpell1.transform.position;
        attackSpell1.transform.position = new Vector3(fp.x, fp.y, 0);
        fireballInstance.SetActive(true);

        Vector3 targetDir = aiming.GetDirection();
        Vector3 spawnPos = transform.position + targetDir.normalized * 0.6f;

        fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir, Experience.main.GetSpell1Runtime());
        if (SoundManager.main != null) {
            SoundManager.main.PlaySound(GameSoundType.Zing);
        }
        if (SpellBar.main != null) {
            SpellBar.main.SpellWasCast(attackSpell1Config);
        }
    }

    private void ShootSpell2()
    {
        attack2Used = Time.time;

        GameObject fireballInstance = Instantiate(attackSpell2);
        fireballInstance.transform.parent = entityContainer;
        Vector3 fp = attackSpell2.transform.position;
        attackSpell2.transform.position = new Vector3(fp.x, fp.y, 0);
        fireballInstance.SetActive(true);

        Vector3 targetDir = aiming.GetDirection();
        Vector3 spawnPos = transform.position + targetDir.normalized * 0.6f;

        fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir, Experience.main.GetSpell2Runtime());
        fireballInstance.GetComponent<Fireball>().Initialize(spawnPos, targetDir, Experience.main.GetSpell2Runtime());

        if (SoundManager.main != null) {
            SoundManager.main.PlaySound(GameSoundType.FireBall);
        }
        if (SpellBar.main != null) {
            SpellBar.main.SpellWasCast(attackSpell2Config);
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
    public int PlayerLevel
    {
        get => Experience.main.GetLevel();
    }
}
