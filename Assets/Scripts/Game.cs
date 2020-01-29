using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static int gridWidth = 4, gridHeight = 4;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    
    void Start()
    {
        GenerateNewTile(2);
    }

    
    void Update()
    {
        GetUserInput();
    }


    private void GetUserInput()
    {
        bool down = Input.GetKeyDown(KeyCode.DownArrow), right = Input.GetKeyDown(KeyCode.RightArrow), up = Input.GetKeyDown(KeyCode.UpArrow), left = Input.GetKeyDown(KeyCode.LeftArrow);

        PrepareTilesForMerging();

        if (down || up || left || right)
        {
            if (up) 
            {
                MoveAllTiles(Vector2.up);
            }

            if (down) 
            {
                MoveAllTiles(Vector2.down);
            }

            if (left) 
            {
                MoveAllTiles(Vector2.left);
            }

            if (right) 
            {
                MoveAllTiles(Vector2.right);
            }
        }
    }


    private void MoveAllTiles(Vector2 direction)
    {
        int tilesMovedCount = 0;

        if (direction == Vector2.left)
        {
            for (int x = 0; x < gridWidth; x ++)
            {
                for (int y  = 0; y < gridHeight; y++)
                {
                    if (grid[x,y] != null)
                    {
                        if (CanMoveTile(grid[x,y], direction))
                        {
                            tilesMovedCount++;
                        }
                    }
                }
            }
        }

        if (direction == Vector2.right)
        {
            for (int x = gridWidth-1; x >= 0; x--)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (CanMoveTile(grid[x, y], direction))
                        {
                            tilesMovedCount++;
                        }
                    }
                }
            }
        }

        if (direction == Vector2.down)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        if (CanMoveTile(grid[x, y], direction))
                        {
                            tilesMovedCount++;
                        }
                    }
                }
            }
        }

        if (direction == Vector2.up)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = gridHeight-1; y >= 0; y --)
                {
                    if (grid[x, y] != null)
                    {
                        if (CanMoveTile(grid[x, y], direction))
                        {
                            tilesMovedCount++;
                        }
                    }
                }
            }
        }

        if (tilesMovedCount != 0)
        {
            GenerateNewTile(1);
        }

    }

    private bool CanMoveTile(Transform tile, Vector2 direction)
    {
        // get the local position of the tile
        Vector2 startPosition = tile.localPosition;

        while (true)
        {
            // moving the tile into next transform
            tile.transform.localPosition += (Vector3)direction;

            // getting the transform location after moving the tile
            Vector2 pos = tile.transform.localPosition;

            if (IsInGrid(pos))
            {
                if (CheckOverlap(pos))
                {
                    UpdateGrid();
                }
                else
                {
                    if (!CheckAndMergeTiles(tile))
                    {
                        // resetting the tile transform
                        tile.transform.localPosition += -(Vector3)direction;

                        // checking that the tile has been moved or not
                        if (tile.transform.localPosition == (Vector3)startPosition)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                // resetting the tile transform
                tile.transform.localPosition += -(Vector3)direction;

                // checking that the tile has been moved or not
                if (tile.transform.localPosition == (Vector3)startPosition)
                {
                    return false;
                }else
                {
                    return true;
                }
            }
        }
    }


    bool CheckAndMergeTiles(Transform movingTile)
    {
        Vector2 pos = movingTile.transform.localPosition;

        Transform collidingTile = grid[(int)pos.x, (int)pos.y];

        int movingTileValue = movingTile.GetComponent<Tile>().tileValue;
        int collidingTileValue = collidingTile.GetComponent<Tile>().tileValue;

        if (movingTileValue == collidingTileValue && !movingTile.GetComponent<Tile>().PreviousMerge && !collidingTile.GetComponent<Tile>().PreviousMerge)
        {
            Destroy(movingTile.gameObject);
            Destroy(collidingTile.gameObject);

            grid[(int)pos.x, (int)pos.y] = null;

            string newTileName = "tile_" + movingTileValue * 2;

            GameObject newTile = (GameObject)Instantiate(Resources.Load(newTileName, typeof(GameObject)), pos, Quaternion.identity);
            newTile.transform.parent = transform;

            newTile.GetComponent<Tile>().PreviousMerge = true;

            UpdateGrid();

            return true;

        }

        return false;
    }

    private void GenerateNewTile(int tileCount)
    {
        
        for (int i = 0; i < tileCount; i++)
        {
            Vector2 locationForNewTile = GetRandomLocationForNewTile();

            if (locationForNewTile != null)
            {
                string tile = "tile_2";

                float chanceoFTwo = Random.Range(0f, 1f);

                if (chanceoFTwo > 0.9f)
                {
                    tile = "tile_4";
                }

                GameObject newTile = (GameObject)Instantiate(Resources.Load(tile, typeof(GameObject)), locationForNewTile, Quaternion.identity);
                newTile.transform.parent = transform;
            }
            // else

        }

        UpdateGrid();
    }


    private void UpdateGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x,y] != null)
                {
                    if (grid[x,y].parent == transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach(Transform tile in transform)
        {
            Vector2 v = new Vector2(Mathf.Round(tile.position.x), Mathf.Round(tile.position.y));
            grid[(int)v.x, (int)v.y] = tile;
        }
    }

    private Vector2 GetRandomLocationForNewTile() {
        
        List<int> x = new List<int>();
        List<int> y = new List<int>();

        for (int j = 0; j < gridWidth; j++)
        {
            for (int i = 0; i < gridHeight; i++)
            {
                if (grid[j, i] == null)
                {
                    x.Add(j);
                    y.Add(i);
                    // once hit
                }
            }
        }

        int randIndex = Random.Range(0, x.Count);

        int randX = x.ElementAt(randIndex);
        int randY = y.ElementAt(randIndex);

        return new Vector2(randX, randY);
    }

    private bool IsInGrid(Vector2 pos)
    {
        if (pos.x >= 0 && pos.x <= gridWidth-1 && pos.y >= 0 && pos.y <= gridHeight - 1)
        {
            return true;
        }
        return false;
    }

    private bool CheckOverlap(Vector2 pos)
    {
        if (grid[(int)pos.x, (int)pos.y] == null)
        {
            return true;
        }

        return false;
    }

    private void PrepareTilesForMerging()
    {
        foreach(Transform t in transform)
        {
            t.GetComponent<Tile>().PreviousMerge = false; 
        }
    }

    /// <summary>
    /// Restart the game, with a new grid of Transform
    /// </summary>
    public void PlayAgain()
    {
        grid = new Transform[gridWidth, gridHeight];
    }
}
