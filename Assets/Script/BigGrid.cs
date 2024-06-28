using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BigGrid : MonoBehaviour
{
    public static BigGrid instance;

    public TextMeshProUGUI TextPro;

    public int gridSizeX, gridSizeY, gridSizeZ;
    public GameObject[] blockList;

    public GameObject bottomPlane;
    public GameObject N,S,W,E;

    
    public Transform[,,] gameGrid;

    int score = 0;

    void Awake()//runs before start
    {
        instance = this;//make sure that any code referencing BigGrid.instance
                        //will access the correct and only instance of BigGrid 
    }

    void Start()
    {
        gameGrid = new Transform[gridSizeX, gridSizeY, gridSizeZ];
        SpawnNewBlock();
    }

    public void UpdateGrid(TetrisBlock block)
    {
        for (int x = 0 ; x < gridSizeX; x++)//nested 3D array loop (go through the grid)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    if(gameGrid[x,y,z] != null)//check every singular point of gameGrid to see if there are any block on it
                    {
                        if (gameGrid[x, y, z].parent == block.transform)//check if current block detected is part of active Tetris block and not dead blocks
                        {
                            gameGrid[x, y, z] = null;//remove reference point to prevent inconsistency
                        }
                    }
                }
            }
        }
        //Fill in child object
        foreach(Transform child in block.transform)
        {
            Vector3 pos = Round(child.position);
            if (pos.y < gridSizeY)
            {
                gameGrid[(int)pos.x,(int)pos.y,(int)pos.z] = child;
            }
        }
    }

    public Vector3 Round(Vector3 number)
    {
        return new Vector3(Mathf.RoundToInt(number.x),
                            Mathf.RoundToInt(number.y),
                            Mathf.RoundToInt(number.z));
    }

    public bool CheckInsideGrid(Vector3 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridSizeX &&
                (int)pos.z >= 0 && (int)pos.z < gridSizeZ &&
                (int)pos.y >= 0);
    }

    public Transform TetricBlockPosition(Vector3 pos)
    {
        //update the grid with the new positions of the parts of a TetrisBlock after the block has moved
        if (pos.y > gridSizeY - 1)
        {
            return null;
        }
        else
        {
            return gameGrid[(int)pos.x,(int)pos.y,(int)pos.z];
        }
    }

    public void SpawnNewBlock()
    {
        Vector3 spawnPoint = new Vector3((int)transform.position.x + (int)gridSizeX / 2,
                                          (int)transform.position.y + gridSizeY,
                                          (int)transform.position.z + (int)gridSizeZ / 2);
        int randomIndex = Random.Range(0, blockList.Length);

        //Spawn the Block
        GameObject newBlock = Instantiate(blockList[randomIndex], spawnPoint, Quaternion.identity) as GameObject;

    }

    public void DeleteLayer()
    {
        for (int y = gridSizeY-1; y >= 0; y--)
        {
            if (CheckFullLayer(y))
            {
                DeleteLayerAt(y);
                MoveAllLayerDown(y);
                score = score + 1;
                TextPro.text = "Score : " + (score);

            }
        }
    }

    bool CheckFullLayer(int y)
    {
        for(int x = 0; x < gridSizeX; x++)//nested 2D in [x,z]
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if(gameGrid[x,y,z] == null)//if one point is empty then layer is not full
                {
                    return false;
                }
            }
        }
        return true;
    }

    void DeleteLayerAt(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Destroy(gameGrid[x, y, z].gameObject);
                gameGrid[x, y, z] = null; //reset the cell
            }
        }
    }

    void MoveAllLayerDown(int y)
    {
        for (int i = y; i < gridSizeY; i++)
        {
            MoveOneLayerDown(i);
        }
    }

    void MoveOneLayerDown(int y)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if(gameGrid[x,y,z] != null)
                {
                    gameGrid[x, y - 1, z] = gameGrid[x, y, z];//set current plane to be plane above
                    gameGrid[x, y, z] = null; //clear plane above
                    gameGrid[x, y - 1, z].position += Vector3.down;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if(bottomPlane != null)
        {
            //A plane's default size is 10x10, so we divide it by 10 to create a 1x1 tile
            Vector3 scalar = new Vector3((float) gridSizeX/10,1, (float)gridSizeZ / 10);
            bottomPlane.transform.localScale = scalar;

            //transform.position.x and z are at -0.5 so a 5x5 grid would be -0.5 + 5/2 = 2 at true position
            bottomPlane.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                                         transform.position.y,
                                                         transform.position.z + (float)gridSizeZ / 2);

            //mainTextureScale lets the material be scale to repeat itself rather than stretching to fit the size of the plane
            bottomPlane.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeZ);
        }
        if (N != null)
        {
            Vector3 scalar = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeY / 10);
            N.transform.localScale = scalar;

            N.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                               transform.position.y + (float)gridSizeY/2,
                                               transform.position.z + gridSizeZ);

            N.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);
        }
        if (S != null)
        {
            Vector3 scalar = new Vector3((float)gridSizeX / 10, 1, (float)gridSizeY / 10);
            S.transform.localScale = scalar;

            S.transform.position = new Vector3(transform.position.x + (float)gridSizeX / 2,
                                               transform.position.y + (float)gridSizeY / 2,
                                               transform.position.z);

            S.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeX, gridSizeY);
        }
        if (E != null)
        {
            Vector3 scalar = new Vector3((float)gridSizeZ / 10, 1, (float)gridSizeY / 10);
            E.transform.localScale = scalar;

            E.transform.position = new Vector3(transform.position.x + gridSizeX,
                                               transform.position.y + (float)gridSizeY / 2,
                                               transform.position.z + (float)gridSizeZ/2);

            E.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);
        }
        if (W != null)
        {
            Vector3 scalar = new Vector3((float)gridSizeZ / 10, 1, (float)gridSizeY / 10);
            W.transform.localScale = scalar;

            W.transform.position = new Vector3(transform.position.x,
                                               transform.position.y + (float)gridSizeY / 2,
                                               transform.position.z + (float)gridSizeZ / 2);

            W.GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(gridSizeZ, gridSizeY);
        }
    }
}
