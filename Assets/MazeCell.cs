using UnityEngine;

// Most of this class comes from the tutorial video that Miguel shared (https://www.youtube.com/watch?v=_aeYq5BmDMg),
// names may be adjusted to the way I was doing things before following the video.
public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject westWall;
    [SerializeField]
    private GameObject eastWall;
    [SerializeField]
    private GameObject northWall;
    [SerializeField]
    private GameObject southWall;
    [SerializeField]
    private GameObject unvisitedBlock;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        unvisitedBlock.SetActive(false);
    }

    // These ClearWall() helper functions are used just for the maze generation to delete walls in order to make
    // a path through the maze
    public void ClearWestWall()
    {
        westWall.SetActive(false);
    }

    public void ClearEastWall()
    {
        eastWall.SetActive(false);
    }

    public void ClearNorthWall()
    {
        northWall.SetActive(false);
    }

    public void ClearSouthWall()
    {
        southWall.SetActive(false);
    }

    // EnableCollision() and DisableCollsion() are used for the cheat that the player can activate on pressing
    // E (keyboard)/Y (controller) to allow them to move through walls.
    public void DisableCollision()
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
    }

    public void EnableCollision()
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }
}
