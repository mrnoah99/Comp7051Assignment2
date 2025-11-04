using UnityEngine;
using System.Collections.Generic;

public class MazeCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject wallSegment;
    [SerializeField]
    private GameObject playerRef;
    private List<GameObject> walls = new();
    private List<Vector2Int> previousLocations = new();
    private List<Vector2Int> pathLocations = new();
    private bool success = false;

    private const int CHECKS_FOR_EXISTING = 10000;

    void Start()
    { // Randomly selects Start and End locations, positions Player at the Start. 
        Vector2Int[] enterExit = StartMaze();

        BuildOuterWall();

        // Generates the path through the maze, random branching paths, and builds the maze.
        GenerateMaze(enterExit[0]);

        if (!IsConnected(enterExit[0], enterExit[1]))
        {
            success = false;
            RetryMaze();
        } else
        {
            success = true;
        }

        while (!success) {}
        AddBranchPaths();

        DrawMaze();
    }

    private void RetryMaze()
    {
        Debug.LogWarning("Maze not fully connected, regenerating...");
        previousLocations.Clear();
        pathLocations.Clear();
        Vector2Int[] retry = StartMaze();
        GenerateMaze(retry[0]);
        if (!IsConnected(retry[0], retry[1]))
        {
            success = false;
            RetryMaze();
            return;
        } else
        {
            success = true;
            return;
        }
    }

    private bool IsConnected(Vector2Int start, Vector2Int end)
    { // Provided by GPT, modified to work since it didn't include logic to add directions to check, and also included some random function that was never written.
        HashSet<Vector2Int> visited = new();
        Queue<Vector2Int> queue = new();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == end)
                {Debug.Log("End detected, path works.");return true;}

            List<Vector2Int> directions = new()
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0)
            };

            foreach (var dir in directions) // (0,1), (1,0), (0,-1), (-1,0)
            {
                var next = current + dir;
                Debug.Log("Checking position: " + next.x + "," + next.y + " from position: " + current.x + "," + current.y);
                if (pathLocations.Contains(next) && !visited.Contains(next))
                {
                    Debug.Log("Next unvisited cell is in path, adding to visited and queuing");
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }
        return false;
    }


    private Vector2Int[] StartMaze()
    {
        Vector2Int entranceLocation = new(Random.Range(-4, 5), -5);
        previousLocations.Add(entranceLocation);
        pathLocations.Add(entranceLocation);

        Vector2Int exitLocation = new(Random.Range(-4, 5), 5);
        previousLocations.Add(exitLocation);
        pathLocations.Add(exitLocation);

        Vector2Int exit2Location = new(exitLocation.x, 4);
        previousLocations.Add(exit2Location);
        pathLocations.Add(exit2Location);

        playerRef.transform.position = new Vector3(entranceLocation.x, 1, (float)-4.5);

        Vector2Int playerSpawn = new(entranceLocation[0], -5);
        previousLocations.Add(playerSpawn);
        pathLocations.Add(playerSpawn);

        Vector2Int[] locPair = new Vector2Int[2];
        locPair[0] = entranceLocation; locPair[1] = exitLocation;
        return locPair;
    }

    private void BuildOuterWall()
    {
        for (int i = -5; i < 6; i++) // Spawns an outer wall to create the edges of the maze. Ignores the Start/End positions.
        {
            for (int j = -5; j < 6; j++)
            {
                Vector2Int outerLocation = new(i, j);
                bool exists = false;

                for (int a = 0; a < previousLocations.Count; a++)
                {
                    if (exists == false) exists = outerLocation.x == previousLocations[a].x && outerLocation.y == previousLocations[a].y;
                }

                if (i == -5 && !exists || i == 5 && !exists)
                {
                    previousLocations.Add(outerLocation);
                    GameObject wall = Instantiate(wallSegment);
                    wall.transform.position = new Vector3(outerLocation.x, 0, outerLocation.y);
                }
                else
                if (j == -5 && !exists || j == 5 && !exists)
                {
                    previousLocations.Add(outerLocation);
                    GameObject wall = Instantiate(wallSegment);
                    wall.transform.position = new Vector3(outerLocation.x, 0, outerLocation.y);
                }
            }
        }
    }

    private void AddBranchPaths()
    {
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            Vector2Int rand_pos = pathLocations[Random.Range(2, 21)];

            List<Vector2Int> directions = new()
            {
                new Vector2Int(0, 2),
                new Vector2Int(2, 0),
                new Vector2Int(0, -2),
                new Vector2Int(-2, 0)
            };
            Shuffle(directions);

            foreach (var dir in directions)
            {
                Vector2Int next = rand_pos + dir;
                if (IsInBounds(next) && !pathLocations.Contains(rand_pos))
                {
                    Vector2Int wallBetween = rand_pos + (dir / 2);
                    if (!previousLocations.Contains(wallBetween)) previousLocations.Add(wallBetween);
                    if (!pathLocations.Contains(wallBetween)) pathLocations.Add(wallBetween);
                    previousLocations.Add(next);
                    pathLocations.Add(next);
                }
            }
        }
    }

    private void GenerateMaze(Vector2Int current)
    { // GPT was used to assist in writing this, changed by myself to make it work with the cube walls we use
        if (!previousLocations.Contains(current))
        {
            previousLocations.Add(current);
            pathLocations.Add(current);
        }

        // Shuffle directions for randomness
        List<Vector2Int> directions = new()
        {
            new Vector2Int(0, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, -2),
            new Vector2Int(-2, 0)
        };
        Shuffle(directions);

        foreach (var dir in directions)
        { // Goes through each direction and picks the first one that fits within the bounds of the maze.
            Vector2Int next = current + dir;
            if (IsInBounds(next) && !pathLocations.Contains(next))
            {
                // Carve wall between
                Vector2Int wallBetween = current + (dir / 2);
                if (!previousLocations.Contains(wallBetween)) previousLocations.Add(wallBetween);
                if (!pathLocations.Contains(wallBetween)) pathLocations.Add(wallBetween);
                previousLocations.Add(next);
                pathLocations.Add(next);
                GenerateMaze(next);
            }
        }
    }

    void Shuffle(List<Vector2Int> list)
    { // Provided by GPT to randomly pick the direction to go in for generating the maze path.
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    bool IsInBounds(Vector2Int pos)
    { // Checks if the provided position fits within the maze area.
        return pos.x > -5 && pos.x < 5 && pos.y > -5 && pos.y < 5;
    }
    
    void DrawMaze()
    {
        bool mazeComplete = false;
        int numTimesFailed = 0;

        for (int i = 0; i < previousLocations.Count; i++)
        { // This loop prevents the walls from spawning inside of one another.
            if (!mazeComplete)
            {
                Vector2Int horizontal = new(Random.Range(-4, 5), Random.Range(-4, 5));
                for (int j = 0; j < CHECKS_FOR_EXISTING; j++)
                {
                    if (previousLocations.Contains(horizontal))
                    {
                        numTimesFailed++;
                        foreach (var loc in previousLocations)
                        {
                            if (horizontal.x == loc.x)
                            {
                                horizontal = new(Random.Range(-4, 5), horizontal.y);
                            }
                            if (horizontal.y == loc.y)
                            {
                                horizontal = new(horizontal.x, Random.Range(-4, 5));
                            }
                        }
                    } else
                    {
                        numTimesFailed = 0;
                    }
                    if (numTimesFailed == CHECKS_FOR_EXISTING)
                    {
                        mazeComplete = true;
                    }
                }

                GameObject newWall = Instantiate(wallSegment);
                newWall.transform.position = new Vector3(horizontal.x, 0, horizontal.y);
                walls.Add(newWall);
                previousLocations.Add(horizontal);
            }
        }
    }
}
