﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mama : Enemy
{
    public Enemy bugToSpawn;
    int numberToSpawn = 1;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
    public override void Die()
    {
        SpawnGrub();
        base.Die();
        

    }
    public void SpawnGrub()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Enemy newEnemy = Instantiate(bugToSpawn).GetComponent<Enemy>();
            newEnemy.transform.forward = transform.forward;
            newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (i * .25f));
            newEnemy.SetWaypoint(wavepointIndex);

        }
    }


}