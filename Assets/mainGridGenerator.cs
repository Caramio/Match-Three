using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainGridGenerator : MonoBehaviour
{
    //Learn the size of the grid
    public int gridColumns, gridRows;


    //Vector2 that corresponds to the first grid location
    public static float firstGridX, firstGridY;

    //Main grid that will act as the square behind
    public GameObject mainGrid;

    //List of pooled objects
    public List<GameObject> pooledTiles;

    //int to keep track of which tile number we are placing
    private int placedTileNum;

    void Start()
    {

        //Define 2D Array
        //GameObject[,] pooledTilesArray = new GameObject[gridRows, gridColumns];



        //First position for the first tile, round down (X Should be negative since it starts at the left)
        if (gridColumns % 2 == 0)
        {
            firstGridX = -((gridColumns / 2) - 0.5f);

        }
        else
        {
            firstGridX = -gridColumns / 2;
        }

        if (gridRows % 2 == 0)
        {
            firstGridY = (gridRows / 2) - 0.5f;
        }
        else
        {
            firstGridY = (gridRows / 2);
        }

        mainGrid.transform.localScale = new Vector2(gridColumns, gridRows);

        generateGrid();




    }

    // Update is called once per frame
    void Update()
    {

    }


    //Generates the basic outline of the grid
    private void generateGrid()
    {



        for (int i = 0; i < gridColumns; i++)
        {


            for (int j = 0; j < gridRows; j++)
            {


                //Assign object to not repeat usage
                GameObject currentTile = pooledTiles[placedTileNum];

                currentTile.SetActive(true);
                currentTile.transform.position = new Vector2(firstGridX + i, firstGridY - j);

                currentTile.GetComponent<tileInfoComponent>().tileLocation = new Vector2Int(i, j);

                //Move to the next tile in the list
                placedTileNum += 1;



            }


        }


    }
}
