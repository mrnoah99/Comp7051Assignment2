using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.ShaderGraph.Serialization;

public class MazeCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject wallSegment;
    [SerializeField]
    private GameObject playerRef;
    private List<GameObject> walls = new();
    private List<int[]> previousLocations = new();

    private const int MAX_WALLS = 20;
    private const int CHECKS_FOR_EXISTING = 100000;

    void Start()
    { // Randomly selects Start and End locations, positions Player at the Start. 
        int[] entranceLocation = new int[2];
        entranceLocation[0] = Random.Range(-4, 5); entranceLocation[1] = -5;
        previousLocations.Add(entranceLocation);
        
        int[] exitLocation = new int[2];
        exitLocation[0] = Random.Range(-4, 5); exitLocation[1] = 5;
        previousLocations.Add(exitLocation);
        playerRef.transform.position = new Vector3(entranceLocation[0], 1, (float)-4.5);
        
        int[] playerSpawn = new int[2];
        playerSpawn[0] = entranceLocation[0]; playerSpawn[1] = -5;
        previousLocations.Add(playerSpawn);
        
        for (int i = -5; i < 6; i++) // Spawns an outer wall to create the edges of the maze. Ignores the Start/End positions.
        {
            for (int j = -5; j < 6; j++)
            {
                int[] outerLocation = new int[2];
                outerLocation[0] = i;outerLocation[1] = j;
                bool exists = false;
                
                for (int a = 0; a < previousLocations.Count; a++)
                {
                    if (exists == false) exists = outerLocation[0] == previousLocations[a][0] && outerLocation[1] == previousLocations[a][1];
                }
                
                if (i == -5 && !exists || i == 5 && !exists)
                {
                    previousLocations.Add(outerLocation);
                    GameObject wall = Instantiate(wallSegment);
                    wall.transform.position = new Vector3(outerLocation[0], 0, outerLocation[1]);
                } else
                if (j == -5 && !exists || j == 5 && !exists)
                {
                    previousLocations.Add(outerLocation);
                    GameObject wall = Instantiate(wallSegment);
                    wall.transform.position = new Vector3(outerLocation[0], 0, outerLocation[1]);
                }
            }
        }
    }

    void FixedUpdate()
    { // Handles spawning the maze, currently just spawns randomly with no real algorithm to make a functional maze.
        if (walls.Count < MAX_WALLS)
        {
            Vector2 horizontal = new(Random.Range(0, 5), Random.Range(0, 5));
            for (int i = 0; i < previousLocations.Count; i++)
            { // This loop prevents the walls from spawning inside of one another.
                for (int j = 0; j < 15 * CHECKS_FOR_EXISTING; j++)
                {
                    if (horizontal.x == previousLocations[i][0] && horizontal.y == previousLocations[i][1])
                    {
                        if (horizontal.x == previousLocations[i][0])
                        {
                            horizontal = new(Random.Range(-4, 5), horizontal.y);
                        }
                        if (horizontal.y == previousLocations[i][1])
                        {
                            horizontal = new(horizontal.x, Random.Range(-4, 5));
                        }
                    }
                }
            }
            
            GameObject newWall = Instantiate(wallSegment);
            newWall.transform.position = new Vector3(horizontal.x, 0, horizontal.y);
            walls.Add(newWall);
            int[] newLocation = new int[2];
            newLocation[0] = (int) horizontal.x; newLocation[1] = (int)horizontal.y;
            previousLocations.Add(newLocation);
        }
    }
}
