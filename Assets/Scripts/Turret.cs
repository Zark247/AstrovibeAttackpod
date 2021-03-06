// <copyright file="Turret.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

// comment
// Basic turret logic, if a tower is placed it at least extends this
public class Turret : MonoBehaviour
{
    //public Waypoints[] waypoints;
    private Transform target;
    [HideInInspector]
    public int killCount = 0;

    [Header("Tower Attributes")]
    public float range = 15f;
    public float firerate = 1f;
    [HideInInspector]
    public float originalFireRate;
    [HideInInspector]
    public float originalRange;
    private float fireCountdown = 0f;

    [Header("Bullet Attributes")]
    public int bulletPierce = 1;
    public int DoTDamage = 1;
    public int bulletDamage = 50;
    public float bulletSpeed = 70;
    public float bulletExplosionRadius = 0f;
    public float DoTTime = 5f;
    public float speedDebuff = 0.75f;
    public int bulletExplosionPierce = 10;
    public bool makeShrapnel = false;
    public bool tracking = false;
    public bool permaSlow = false;
    public float sprayAmount = 0f;

    //Below is to be used for buffs from the support tower
    [HideInInspector]
    public bool buffedFireRate = false;
    [HideInInspector]
    public bool buffedRange = false;
    [HideInInspector]
    public bool buffedAim = false;
    [HideInInspector]
    public bool buffedPierce = false;
    [HideInInspector]
    public bool buffedDamage = false;
    //[HideInInspector]
    public bool electricTower;
    public bool electricDoT;
    public bool slow = false;


    //Audio file name to be played when turret is firing a bullet
    public string gunShotAudio;

    [Header("Unity Setup Fields")]

    public string enemyTag = "Enemy";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject fireEffect;
    public GameObject[] fireEffectList;
    public float fireEffectLifespan = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        if(true) 
        {
            this.InvokeRepeating("UpdateTarget", 0f, 0.5f);
        }

        this.originalFireRate = this.firerate;
        this.originalRange = this.range;
    }

    // Keeps track of who the turret is currently trying to hit
    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
        float shortestDistanceToEnd = Mathf.Infinity;
        //float shortestDistanceToTurret = Mathf.Infinity;
        GameObject nearestEnemy = null;
        //int finalWaypointIndex = waypoints[0].points.Length - 1;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position); // turret to enemy distance
            float enemyDistanceToEnd = enemy.GetComponent<Enemy>().distanceLeft;
            //Vector3.Distance(enemy.transform.position, waypoints[0].points[finalWaypointIndex].position); // enemy to end distance
            // Debug.Log("Distance "+enemyDistanceToEnd);

            /* Old targeting, targest closest enemy to tower
            if (distanceToEnemy < shortestDistanceToTurret)
            {
                shortestDistanceToTurret = distanceToEnemy;
                nearestEnemy = enemy;
            }
            */

            // Targets enemy closest to last waypoint
            if (enemyDistanceToEnd < shortestDistanceToEnd && distanceToEnemy <= this.range)
            {
                shortestDistanceToEnd = enemyDistanceToEnd;
                nearestEnemy = enemy;
            }
        }

        // Sets the target to the chosen enemy
        if (nearestEnemy != null)
        {
            this.target = nearestEnemy.transform;
        }
        else
        {
            this.target = null;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        this.fireCountdown -= Time.deltaTime;
        if (this.target == null)
        {
            return;
        }
        // Target lockon
        if (!electricTower)
        {
            Vector3 dir = this.target.position - this.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(this.partToRotate.rotation, lookRotation, Time.deltaTime * this.turnSpeed).eulerAngles;
            this.partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }

        if (this.target != null) {
            this.DrawParticleEffect();
        }

        //Not electric tower
        if (this.fireCountdown <= 0f && !electricTower)
        {
            this.Shoot();
            this.fireCountdown = 1f / this.firerate;
        }

        //Electric tower
        if (this.fireCountdown <= 0f && electricTower == true)
        {
            this.ShootVolley();
            this.DrawParticleEffect();
            this.fireCountdown = 1f / this.firerate;
        }
    }

    // What happens when a turret "shoots" (the meaning of this can change depending on the turret)
    private void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(this.bulletPrefab, this.firePoint.position, this.firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bullet.SetBulletStats(bulletSpeed, bulletDamage, bulletExplosionRadius, bulletPierce, this, bulletExplosionPierce, makeShrapnel, tracking, sprayAmount);
        
        //not used now?
        if (bullet != null)
        {
            bullet.Seek(this.target);
        }
        bullet.SetBulletDirection();

        //Audio for when a "bullet" is fired
        FindObjectOfType<AudioManager>().PlayAudio(gunShotAudio);
    }

    //Only for the Electric tower
    private void ShootVolley()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(this.transform.position, enemy.transform.position);
            Enemy enemyToHurt = enemy.GetComponent<Enemy>();
            if (distanceToEnemy <= this.range)
            {
                enemyToHurt.electricTowerAffectingMe = this;
                enemyToHurt.ElectricDamage(bulletDamage);
            }
            if (distanceToEnemy <= this.range && electricDoT)
            {
                enemyToHurt.DoTDamage = this.DoTDamage;
                enemyToHurt.DoTTime = this.DoTTime;
                enemyToHurt.electricDoT = true;
            }            
            if (distanceToEnemy <= this.range && slow)
            {
                enemyToHurt.speedDebuff = this.speedDebuff;
                enemyToHurt.DoTTime = this.DoTTime;
                enemyToHurt.slow = true;
                if(permaSlow)
                {
                    enemyToHurt.permaSlow = true;
                }
            }
        }
        FindObjectOfType<AudioManager>().PlayAudio(gunShotAudio);
    }

    // Renders the effect for tower shooting (mainly flamethrower)
    private void DrawParticleEffect()
    {
        // reorganize into switch statment later
        if(fireEffectList.GetLength(0)<=0)
        {
            return;
        }
        if(fireEffectList.GetLength(0)==1)
        {
            GameObject rightFireEffect = (GameObject)Instantiate(this.fireEffect, this.fireEffectList[0].transform.position, this.fireEffectList[0].transform.rotation);
            if(electricTower)
            {
                rightFireEffect.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
            }
            Destroy(rightFireEffect,fireEffectLifespan);
        }
        if(fireEffectList.GetLength(0)==2)
        {
            GameObject rightFireEffect = (GameObject)Instantiate(this.fireEffect, this.fireEffectList[0].transform.position, this.fireEffectList[0].transform.rotation);
            Destroy(rightFireEffect,fireEffectLifespan);
            GameObject leftFireEffect = (GameObject)Instantiate(this.fireEffect, this.fireEffectList[1].transform.position, this.fireEffectList[1].transform.rotation);
            Destroy(leftFireEffect,fireEffectLifespan);
        }
        if(fireEffectList.GetLength(0)>2)
        {
            GameObject topRightFireEffect = (GameObject)Instantiate(this.fireEffect, this.fireEffectList[2].transform.position, this.fireEffectList[2].transform.rotation);
            Destroy(topRightFireEffect,fireEffectLifespan);
            GameObject topLeftFireEffect = (GameObject)Instantiate(this.fireEffect, this.fireEffectList[3].transform.position, this.fireEffectList[3].transform.rotation);
            Destroy(topLeftFireEffect,fireEffectLifespan);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.range);
    }
}
