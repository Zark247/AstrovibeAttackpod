// <copyright file="WaveSpawner.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Controls what waves consist of
public class WaveSpawner : MonoBehaviour
{
    public static bool gameWon = false;
    //Enemy Prefabs
    public Transform grubPrefab;
    public Transform scorpPrefab;
    public Transform dronePrefab;
    public Transform beetlePrefab;
    public Transform mamaPrefab;
    public Transform carrierPrefab;
    public Transform[] spawnPoint;
    public Waypoints[] waypoints;
    public GamerManager gamerManager;

    private int index = -1;
    public string enemyTag = "Enemy";
    private bool spawning = false;
    private int waveSpawnerToUse = 0;

    //Array[15,4] for Spawning enemies(0:grub, 1:scorp, 2:beetle, 3:drone, 4:mama, 5:carrier)
    private int[,] spawnerIndex = {//G,S,B,D,M,C
                                    {1,0,0,1,0,0},   
                                    {5,0,0,1,0,0},
                                    {5,1,0,2,0,0},
                                    {7,1,0,2,0,0},
                                    {9,2,1,1,0,0},
                                    {10,3,2,2,0,0},
                                    {10,5,3,3,1,0},
                                    {15,7,3,5,1,0},
                                    {20,10,4,5,3,0},
                                    {15,10,4,5,5,1},
                                    {10,5,3,1,1,2},
                                    {15,10,4,3,2,3},
                                    {20,15,6,5,3,2},
                                    {15,10,10,7,3,3},
                                    {15,10,10,10,10,5} 
                                    };
    
    

    private int maxRounds = 0;
    public float timeBetweenRounds = 5f;
    public float countdown = 11f;

    public Text roundText;
    private int round = 0;

    //testing
    // private int grub = 0;
    // private int scorp = 0;
    // private int drone = 0;
    // private int mamas = 0;
    // private int carrier = 0;
    // private bool allSpawned, grubSpawned, scorpSpawned, droneSpawned, mamaSpawned, carrierSpawned = false;
    // private int totalSpawned = 0;

    // Runs before first frame, starts the rounds from the start
    private void Start()
    {
        // Locates START(green block) in the scene
        //Transform spawnTransform = GameObject.Find("START").transform;
        // Declares that the static spawnPoint takes on the transform of START(green block)
        //spawnPoint = spawnTransform;
        this.maxRounds = this.spawnerIndex.GetLength(0);
        this.roundText.text = ("Round: " + this.round + "/" + this.maxRounds);
        
        PlayerStats.Rounds = 0;
        index = -1;
        gameWon = false;

    }

    // Constantly checks if the game is done
    private void Update()
    {
        if(this.checkForEnemies() && !spawning)
        {
            Debug.Log("reset play");
            gamerManager.resetPlayButton();
        }
        if(index >= maxRounds - 1 && this.checkForEnemies() && !spawning)
        {
            gameWon = true;
        }
    }

    //Pressing the play button calls the SpawnWave function
    public void StartWave()
    {
        
        //Debug.Log("Play button pressed, starting SpawnWave");
        if(this.checkForEnemies())
        {
            ++this.index;
            ++this.round;
            this.roundText.text = ("Round: " + this.round + "/" + this.maxRounds);
            ++PlayerStats.Rounds;
            this.StartCoroutine(this.SpawnWave(index));
        }
        else
        {
            
        }
    }

    // Logic for sending each wave out of the spawn room
    private IEnumerator SpawnWave(int index)
    {
        //Extracts the amount of the coresponding enemy to spawn
        //15 waves(i), 6 enemy types(j)
        //for(int i=0; i < spawnerIndex.GetLength(0); ++i)
        //{
            spawning = true;
            gamerManager.resetFastForwardButton();
            //Debug.Log("Wave "+(index+1));
            for(int j=0; j < spawnerIndex.GetLength(1); ++j)
            {
                int amountSpanwed = spawnerIndex[index,j];
                //totalSpawned += amountSpanwed;
                switch(j)
                {
                    case 0: //grub
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //++grub;
                            //Debug.Log(amountSpanwed+ " Grubs Spawning");
                            SpawnEnemy(grubPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //grubSpawned = true;
                        break;
                    case 1: //scorp
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //++scorp;
                            //Debug.Log(amountSpanwed+ " Scorps Spawning");
                            SpawnEnemy(scorpPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //scorpSpawned = true;
                        break;
                    case 2: //beetle
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //++drone;
                            //Debug.Log(amountSpanwed+ " Beetle Spawning");
                            SpawnEnemy(beetlePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                            //beelteSpawned = true;
                        break;
                    case 3: //drone
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //Debug.Log(amountSpanwed+ " Drones Spawning");
                            SpawnEnemy(dronePrefab);
                            yield return new WaitForSeconds(1f);
                        }
                            //droneSpawned = true;
                        break;   
                    case 4: //mama
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //++mamas;
                            //Debug.Log(amountSpanwed+ " Mamas Spawning");
                            SpawnEnemy(mamaPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                        //mamaSpawned = true;
                        break;
                    case 5: //carrier
                        for (int k = 0; k < amountSpanwed; ++k)
                        {
                            waveSpawnerToUse = (waveSpawnerToUse + 1) % waypoints.Length;
                            //Debug.Log(waveSpawnerToUse);
                            //++carrier;
                            //Debug.Log(amountSpanwed+ " Carrier Spawning");
                            SpawnEnemy(carrierPrefab);
                            yield return new WaitForSeconds(1f);
                        }
                            //carrierSpawned = true;
                        break;
                }
            }
            spawning = false;
            
            //Testing
            // Debug.Log("Total spawned: "+totalSpawned
            //     +"\n Grubs: "+grub
            //     +"\n Scorps: "+scorp
            //     +"\n Drones: "+drone
            //     +"\n Mamas: "+mamas
            //     +"\n Carriers: "+carrier);

            // Time between rounds
            //yield return new WaitForSeconds(2f);
            // Increments the round counter
            //++this.round;
            //++PlayerStats.Rounds;
        }

    // Logic for spawning each individual enemy within a wave
    public void SpawnEnemy(Transform enemy)
    {
        enemy.GetComponent<Enemy>().waypoints = waypoints[waveSpawnerToUse];
        Instantiate(enemy, spawnPoint[waveSpawnerToUse].position, spawnPoint[waveSpawnerToUse].rotation);
    }
    
    // returns true if no enemies in scene
    public bool checkForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(this.enemyTag);
        if(enemies.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}