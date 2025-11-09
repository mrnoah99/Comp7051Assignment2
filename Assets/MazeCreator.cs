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
    private bool disableCollision = false;

    IEnumerator Start()
    { // Randomly selects Start and End locations, positions Player at the Start.

        inputActions = new();
        interact = inputActions.Player.Interact;
        interact.performed += ToggleCollision;
        interact.Enable();

        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeDepth; j++)
            {
                mazeGrid[i, j] = Instantiate(wallSegment, new Vector3(i, 0, j), Quaternion.identity);
            }
        }

        int start_x = Random.Range(0, mazeWidth);
        mazeGrid[start_x, 0].ClearSouthWall();
        entranceLoc.transform.position = new Vector3(start_x, 0, 0);

        int end_x = Random.Range(0, mazeWidth);
        mazeGrid[end_x, mazeDepth - 1].ClearNorthWall();
        exitLoc.transform.position = new Vector3(end_x, 0, mazeDepth);

        playerRef.transform.position = new Vector3(start_x, 1, 0);

        yield return GenerateMaze(null, mazeGrid[start_x, 0]);
    }

    private IEnumerator GenerateMaze(MazeCell previous, MazeCell current)
    {
        current.Visit();
        ClearWalls(previous, current);

        yield return new WaitForSeconds(0.025f);

        MazeCell nextCell;

        do
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
    
    private IEnumerable<MazeCell> GetAllUnvisitedNeighbour(MazeCell current)
    {
        int x = (int)current.transform.position.x;
        int z = (int)current.transform.position.z;

        if (x + 1 < mazeWidth)
        {
            var cellToEast = mazeGrid[x + 1, z];

            if (!cellToEast.isVisited)
            {
                yield return cellToEast;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToWest = mazeGrid[x - 1, z];

            if (!cellToWest.isVisited)
            {
                yield return cellToWest;
            }
        }

        if (z + 1 < mazeDepth)
        {
            var cellToNorth = mazeGrid[x, z + 1];

            if (!cellToNorth.isVisited)
            {
                yield return cellToNorth;
            }
        }
        
        if (z - 1 >= 0)
        {
            var cellToSouth = mazeGrid[x, z - 1];

            if (!cellToSouth.isVisited)
            {
                yield return cellToSouth;
            }
        }
    }

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

    private void ToggleCollision(InputAction.CallbackContext callbackContext)
    {
        disableCollision = !disableCollision;
        if (disableCollision)
        {
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    mazeGrid[i, j].GetComponent<Collider>().enabled = false;
                }
            }
        } else
        {
            for (int i = 0; i < mazeWidth; i++)
            {
                for (int j = 0; j < mazeDepth; j++)
                {
                    mazeGrid[i, j].GetComponent<Collider>().enabled = true;
                }
            }
        }
    }
}
