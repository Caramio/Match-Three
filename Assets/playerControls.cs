using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControls : MonoBehaviour
{
    //Camera obj
    private Camera mainCamera;

    //Grid generator obj
    public GameObject mainGridGeneratorObj;
    //component to be used
    private mainGridGenerator mainGridGeneratorComponent;
    void Start()
    {
        //Obj to access gridRows/gridColumns
        mainGridGeneratorComponent = mainGridGeneratorObj.GetComponent<mainGridGenerator>();

        //Assign the camera
        mainCamera = Camera.main;


      
    }

    // Update is called once per frame
    void Update()
    {
        //Allow the player to select a piece
        if (tileSwapHandler.startedMoveTileRoutine == false)
        {
            dragPiece();
        }
    }


    //Helper method to check if a value is between a range
    private bool checkIfBetween(float a, float b, float variance)
    {
        if (Mathf.Abs(a - b) > variance)
        {
            return false;
        }

        return true;
    }



    //Indicates wether or not a tile is currently being moved
    private bool startedMovePieceRoutine;

    //Bool to check if we should raycast or not
    private bool foundPiece;

    //RaycastHit2D that will be assigned to the tile
    private RaycastHit2D hitPiece;

    //The piece that we are going to be moving, raycast will assign this when it hits a tile
    private GameObject selectedObject;
    //Vector2 to store the initial place of the selected piece, we will use this to determine which side the player has clicked to move
    private Vector2 selectedPieceInitialTransform;

    //THE OBJECT THAT HANDLES THE MATH OF TILE SWAPPING
    public GameObject tileSwapperObj;
    
    private void dragPiece()
    {

        //Click to select an object
        if (Input.GetMouseButton(0) && startedMovePieceRoutine == false)
        {

            if (foundPiece == false)
            {
                hitPiece = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            }


            //If we found a moveablePiece type object with our raycast
            if (hitPiece && hitPiece.collider.tag == "moveablePiece")
            {

                selectedObject = hitPiece.collider.gameObject;

                GameObject selectedPiece = hitPiece.collider.gameObject;

                selectedPieceInitialTransform = selectedPiece.transform.position;




                //Move to the right if we dragged right
                if (mainCamera.ScreenToWorldPoint(Input.mousePosition).x > selectedPieceInitialTransform.x &&
                    checkIfBetween(mainCamera.ScreenToWorldPoint(Input.mousePosition).y, selectedPieceInitialTransform.y, 0.2f))
                {


                    //Testing here
                    // tileSwapperObj.GetComponent<tileSwapper>().checkValidity("Right", selectedPiece.GetComponent<tileInfoComponent>().numOfTile);


                    

                    if (startedMovePieceRoutine == false && 
                        selectedPiece.GetComponent<tileInfoComponent>().tileLocation.x + 1 < mainGridGeneratorComponent.gridColumns)
                    {
                        

                        if (tileSwapHandler.startedMoveTileRoutine == false)
                        {
                          
                            StartCoroutine(tileSwapperObj.GetComponent<tileSwapHandler>().moveTileRoutine(selectedPiece, "Right"));
                        }
                    }

                }

                //Move to the left if we dragged left
                if (mainCamera.ScreenToWorldPoint(Input.mousePosition).x < selectedPieceInitialTransform.x &&
                    checkIfBetween(mainCamera.ScreenToWorldPoint(Input.mousePosition).y, selectedPieceInitialTransform.y, 0.2f))
                {
                    //Testing here
                    //tileSwapperObj.GetComponent<tileSwapper>().checkValidity("Left", selectedPiece.GetComponent<tileInfoComponent>().numOfTile);

                  


                    if (startedMovePieceRoutine == false && selectedPiece.GetComponent<tileInfoComponent>().tileLocation.x - 1 >= 0)
                    {
                        
                        if (tileSwapHandler.startedMoveTileRoutine == false)
                        {
                           
                            StartCoroutine(tileSwapperObj.GetComponent<tileSwapHandler>().moveTileRoutine(selectedPiece, "Left"));
                        }
                    }

                }

                //Move down if we dragged down
                if (mainCamera.ScreenToWorldPoint(Input.mousePosition).y < selectedPieceInitialTransform.y &&
                    checkIfBetween(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, selectedPieceInitialTransform.x, 0.2f))
                {

                    //Testing here
                    // tileSwapperObj.GetComponent<tileSwapper>().checkValidity("Down", selectedPiece.GetComponent<tileInfoComponent>().numOfTile);



                    if (startedMovePieceRoutine == false && 
                        selectedPiece.GetComponent<tileInfoComponent>().tileLocation.y + 1 < mainGridGeneratorComponent.gridRows)
                    {
                        
                        if (tileSwapHandler.startedMoveTileRoutine == false)
                        {
                            
                            StartCoroutine(tileSwapperObj.GetComponent<tileSwapHandler>().moveTileRoutine(selectedPiece, "Down"));
                        }
                    }

                }

                //Move down if we dragged Up
                if (mainCamera.ScreenToWorldPoint(Input.mousePosition).y > selectedPieceInitialTransform.y &&
                    checkIfBetween(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, selectedPieceInitialTransform.x, 0.2f))
                {

                    //Testing here
                    //tileSwapperObj.GetComponent<tileSwapper>().checkValidity("Up", selectedPiece.GetComponent<tileInfoComponent>().numOfTile);

                 


                    if (startedMovePieceRoutine == false && selectedPiece.GetComponent<tileInfoComponent>().tileLocation.y - 1 >= 0)
                    {
                        
                        if (tileSwapHandler.startedMoveTileRoutine == false)
                        {
                            
                            StartCoroutine(tileSwapperObj.GetComponent<tileSwapHandler>().moveTileRoutine(selectedPiece, "Up"));
                        }
                    }

                }


            }




        }
        //mouse up to unselect
        if (Input.GetMouseButtonUp(0) && startedMovePieceRoutine == false)
        {
            foundPiece = false;
        }

    }
    
}
