using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEditor.ShaderGraph.Serialization;

public class MazeCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject wallSegment;
    private List<GameObject> walls = new();
    private List<int[]> previousLocations = new();

    private const int MAX_WALLS = 20;
    private const int CHECKS_FOR_EXISTING = 100000;

    void Start()
    {
        int[] entranceLocation = new int[2];
        entranceLocation[0] = 3; entranceLocation[1] = 3;
        previousLocations.Add(entranceLocation);
    }

    void FixedUpdate()
    {
        if (walls.Count < MAX_WALLS)
        {
            Vector2 horizontal = new(Random.Range(0, 5), Random.Range(0, 5));
            for (int i = 0; i < previousLocations.Count; i++)
            {
                for (int j = 0; j < 15*CHECKS_FOR_EXISTING; j++)
                {
                    if (horizontal.x == previousLocations[i][0] && horizontal.y == previousLocations[i][1]){
                    if (horizontal.x == previousLocations[i][0])
                    {
                        Debug.Log("X Coordinate already used, reselecting...");
                        horizontal = new(Random.Range(-4, 5), horizontal.y);
                    }
                        if (horizontal.y == previousLocations[i][1])
                        {
                            Debug.Log("Y Coordinate already used, reselecting...");
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
