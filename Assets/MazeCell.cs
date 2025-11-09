using UnityEngine;

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

    public bool isVisited { get; private set; }

    public void Visit()
    {
        isVisited = true;
        unvisitedBlock.SetActive(false);
    }

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
}
