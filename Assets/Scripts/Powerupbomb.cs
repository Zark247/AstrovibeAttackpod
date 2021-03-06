// <copyright file="Powerupbomb.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine;

public class Powerupbomb : Powerupbase
{
    public override void Activate ()
    {
        base.Activate();
        Invoke("explode", lifespan * .25f);
    }
    private void explode()
    {
        GameObject obj = (GameObject)Instantiate(this.explosionEffect, this.transform.position, this.transform.rotation);
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 10);
        foreach (Collider collider in colliders)
        {
            // if it hits an enemy it damages it and reduces the amount of things it can still hit
            if (collider.tag == "Enemy")
            {
                collider.GetComponent<Enemy>().TakeDamage(75);
            }
        }
        Destroy(obj, 3f);
        Deactivate();
    }
}
