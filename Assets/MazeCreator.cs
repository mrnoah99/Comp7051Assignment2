using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;

public class MazeCreator : MonoBehaviour
{
    [SerializeField]
    private MazeCell wallSegment;
    [SerializeField]
    private GameObject playerRef;
    [SerializeField]
    private int mazeWidth = 11;
    [SerializeField]
    private int mazeDepth = 11;
    [SerializeField]
    private GameObject entranceLoc;
    [SerializeField]
    private GameObject exitLoc;

    private MazeCell[,] mazeGrid;
    private InputActions inputActions;
    private InputAction interact;
    private InputAction reset;
    private bool disableCollision = false;
    private float runtime = 0;

    IEnumerator Start()
    {
        // Used for handling the walk-through-walls cheat and the reset buttons.
        inputActions = new();
        interact = inputActions.Player.Interact;
        interact.Enable();
        reset = inputActions.Player.Reset;
        reset.Enable();

        // Fills out the entire maze area with the wall segments.
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                mazeGrid[i, j] = Instantiate(wallSegment, new Vector3(i, 0, j), Quaternion.identity);
            }
        }

        // Randomly selects Start and End locations, positions Player at the Start.
        int start_x = Random.Range(0, mazeWidth);
        mazeGrid[start_x, 0].ClearSouthWall();
        entranceLoc.transform.position = new Vector3(start_x, 0, 0);

        int end_x = Random.Range(0, mazeWidth);
        mazeGrid[end_x, mazeDepth - 1].ClearNorthWall();
        exitLoc.transform.position = new Vector3(end_x, 0, mazeDepth);

        playerRef.transform.position = new Vector3(start_x, 1, 0);

        // Starts generating the maze
        yield return GenerateMaze(null, mazeGrid[start_x, 0]);
    }

    private IEnumerator GenerateMaze(MazeCell previous, MazeCell current)
    {
        current.Visit();
        ClearWalls(previous, current);

        // Used only to make the maze be able to generate over time instead of instantly, can be removed/commented
        // out later if we don't want it.
        yield return new WaitForSeconds(0.025f);

        MazeCell nextCell;

        do // As long as there are any available spaces, keep randomly selecting neighbours to break walls between.
        {
            nextCell = GetNextUnvisited(current);

            if (nextCell != null)
            {
                yield return GenerateMaze(current, nextCell);
            } else if (current.transform.position.z == mazeDepth)
            {
                current.ClearEastWall();
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisited(MazeCell current)
    {
        var unvisitedCells = GetAllUnvisitedNeighbour(current);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }
    
    // Returns all cells neighbouring the one currently being worked on that have not yet been visited by the generator.
    private IEnumerable<MazeCell> GetAllUnvisitedNeighbour(MazeCell current)
    {
        int x = (int)current.transform.position.x;
        int z = (int)current.transform.position.z;

        if (x + 1 < mazeWidth)
        {
            var cellToEast = mazeGrid[x + 1, z];

            if (!cellToEast.IsVisited)
            {
                yield return cellToEast;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToWest = mazeGrid[x - 1, z];

            if (!cellToWest.IsVisited)
            {
                yield return cellToWest;
            }
        }

        if (z + 1 < mazeDepth)
        {
            var cellToNorth = mazeGrid[x, z + 1];

            if (!cellToNorth.IsVisited)
            {
                yield return cellToNorth;
            }
        }
        
        if (z - 1 >= 0)
        {
            var cellToSouth = mazeGrid[x, z - 1];

            if (!cellToSouth.IsVisited)
            {
                yield return cellToSouth;
            }
        }
    }

    // Handles clearing walls between the previous and current cell.
    private void ClearWalls(MazeCell previous, MazeCell current)
    {
        if (previous == null)
        {
            return;
        }
        Vector3 prev = previous.transform.position;
        Vector3 curr = current.transform.position;

        if (prev.x < curr.x)
        {
            previous.ClearEastWall();
            current.ClearWestWall();
            return;
        }

        if (prev.x > curr.x)
        {
            previous.ClearWestWall();
            current.ClearEastWall();
            return;
        }

        if (prev.z < curr.z)
        {
            previous.ClearNorthWall();
            current.ClearSouthWall();
            return;
        }

        if (prev.z > curr.z)
        {
            previous.ClearSouthWall();
            current.ClearNorthWall();
            return;
        }
    }

    // Toggles collision of the walls so that the player can walk through them. The enemy shouldn't be allowed
    // to path through the walls when in this state in case it gets stuck.
    private void ToggleCollision()
    {
        disableCollision = !disableCollision;
        if (disableCollision)
        {
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    mazeGrid[i, j].DisableCollision();
                }
            }
        }
        else
        {
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    mazeGrid[i, j].EnableCollision();
                }
            }
        }
    }

    // Helper function for resetting the player and enemy positions back to where they started.
    private void ResetPositions()
    {
        playerRef.transform.SetPositionAndRotation(entranceLoc.transform.position, entranceLoc.transform.rotation);
    }
    
    // Adding the functions to *action*.performed wasn't working for some reason, so I am just using this. Effectively
    // does the same thing.
    void FixedUpdate()
    {
        runtime += Time.fixedDeltaTime;
        if (interact.IsPressed() && runtime >= 0.4)
        {
            runtime = 0;
            ToggleCollision();
        }

        if (reset.IsPressed() && runtime >= 0.4)
        {
            ResetPositions();
        }
    }
}
