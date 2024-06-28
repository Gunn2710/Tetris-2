using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    float oldTime;
    float fallTime = 0.5f;

    void Start()
    {
        
    }
    
    void Update()
    {
        if (Time.time - oldTime > fallTime)
        {
            transform.position += Vector3.down;//move tetris block down

            if(!CheckValidMove())
            {
                transform.position += Vector3.up;//counter Vector3.down
                BigGrid.instance.DeleteLayer();//this checks whether there is a layer to be deletedd
                enabled = false;//stops the current block from furthur moving
                BigGrid.instance.SpawnNewBlock();
            }
            else
            {
                //update the grid
                BigGrid.instance.UpdateGrid(this);
            }

            oldTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetInput(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetInput(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetInput(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetInput(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            SetRotationInput(new Vector3(90, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetRotationInput(new Vector3(0, 00, 90));
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetRotationInput(new Vector3(0, 90, 0));
        }
    }

    public void SetInput(Vector3 direction)
    {
        transform.position += direction;
        if (!CheckValidMove())
        {
            transform.position -= direction;
        }
        else
        {
            BigGrid.instance.UpdateGrid(this);
        }
    }

    public void SetRotationInput(Vector3 rotation)
    {
        transform.Rotate(rotation, Space.World);
        if (!CheckValidMove())
        {
            transform.Rotate(-rotation, Space.World);
        }
        else
        {
            BigGrid.instance.UpdateGrid(this);
        }
    }

    
    bool CheckValidMove()
    {
        foreach (Transform child in transform)//loop through each child/block to check if each child collide with boundary
        {
            Vector3 BlockPosition = BigGrid.instance.Round(child.position);//round the blocks to the nearest grid
            if (!BigGrid.instance.CheckInsideGrid(BlockPosition))
            {
                return false;
            }
        }

        foreach (Transform child in transform)//check if collide with other blocks
        {
            Vector3 BlockPosition = BigGrid.instance.Round(child.position);
            Transform t = BigGrid.instance.TetricBlockPosition(BlockPosition);
            if (t != null && t.parent != transform)
            {
                return false;
            }
        }
        return true;
    }

}
