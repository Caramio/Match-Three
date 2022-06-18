using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;

using UnityEngine.UI;
public class tileSwapHandler : MonoBehaviour
{
    //MAIN GRID THAT WILL BE USED TO STORE INFORMATION FOR THE GAME
    private GameObject[,] gridObjectsArray;

    //Grid generator obj
    public GameObject mainGridGeneratorObj;
    //component to be used
    private mainGridGenerator mainGridGeneratorComponent;

    //Score text holder
    public TMPro.TextMeshProUGUI scoreText;

    //gridRows and gridColumns that will be taken from mainGridGenerator
    private int gridRows, gridColumns;

    //initial pieces, references from mainGridGenerator
    private List<GameObject> gridObjectsList;

    // Clear console
    static class UsefulShortcuts
    {
        // Alt + C
        [Shortcut("Clear Console", KeyCode.E, ShortcutModifiers.Control)]
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
    //Clear console

    void Start()
    {
        mainGridGeneratorComponent = mainGridGeneratorObj.GetComponent<mainGridGenerator>();
        
       

        //Assign gridRows/Columns
        gridRows = mainGridGeneratorComponent.gridRows;
        gridColumns = mainGridGeneratorComponent.gridColumns;

        //Initialize with the size of the grid
        gridObjectsArray = new GameObject[gridColumns, gridRows];

        //Assign the gridObjectsList, this list contains the initial pieces set by us
        gridObjectsList = mainGridGeneratorComponent.pooledTiles;

        //POPULATE THE MAIN 2D ARRAY INITIALLY USING THE LIST
      
        populate2DArray();


        Debug.Log("Rows " + gridRows);
        Debug.Log("Columns " + gridColumns);

        //Debug.Log("0 len" + gridObjectsArray.GetLength(0));
        //Debug.Log("1 len" + gridObjectsArray.GetLength(1));
    }



    // Update is called once per frame
    void Update()
    {

      

        if (Input.GetKeyDown(KeyCode.A))
        {
            
            for(int i = 0; i < gridObjectsArray.GetLength(0);i++)
            {
                for (int j = 0; j < gridObjectsArray.GetLength(1);j++)
                {
                    Debug.Log(" Array " + i + " " + j + " is " + gridObjectsArray[i, j]);
                }
            }

            
            

            /*
            foreach (GameObject matchedobj in selectedRightMatchesList)
            {
                if (matchedobj)
                {
                    Debug.Log("Matched is " + matchedobj);
                }
            }
            */
        }
        
        
        

       


    }

    //Turn the preassigned list into a 2D Array for easier calculations
    private int indexCounter = -1;
    private void populate2DArray()
    {

        for (int i = 0; i < gridColumns; i++)
        {

            for (int j = 0; j < gridRows; j++)
            {

                indexCounter += 1;
                             
                gridObjectsArray[i, j] = gridObjectsList[indexCounter];
            }


        }

    }

 



    //Bool to not double call
    public static bool startedMoveTileRoutine;

    //Timer to swap pieces
    private float moveTileTimer = 0.5f;
    private float moveTileCounter;

    //Assign the selected and targeted objects
    private GameObject selectedTile, targetedTile; 
    //Initial Vectors for the locations of 
    private Vector2 selectedTileStartPos, targetedTileStartPos;

    //Assigning transforms to not reuse later
    private Transform selectedTilesTransform, targetedTilesTransform;
    public IEnumerator moveTileRoutine(GameObject selectedTile , string movedSide)
    {

        startedMoveTileRoutine = true;

        //Holder tileInfoComp for selected
        tileInfoComponent selectedTileInfoComponent = selectedTile.GetComponent<tileInfoComponent>();
        
        //Assign the targeted tile depending on the moved side and selectedTiles location
        if (movedSide == "Right")
        {
            targetedTile = gridObjectsArray[selectedTileInfoComponent.tileLocation.x + 1, selectedTileInfoComponent.tileLocation.y];
        }
        if (movedSide == "Left")
        {
            targetedTile = gridObjectsArray[selectedTileInfoComponent.tileLocation.x - 1, selectedTileInfoComponent.tileLocation.y];
        }
        if (movedSide == "Up")
        {
            targetedTile = gridObjectsArray[selectedTileInfoComponent.tileLocation.x , selectedTileInfoComponent.tileLocation.y - 1];
        }
        if (movedSide == "Down")
        {
            targetedTile = gridObjectsArray[selectedTileInfoComponent.tileLocation.x , selectedTileInfoComponent.tileLocation.y + 1];
        }

        //Now that we know the targeted tile, we can set a holder for its tileinfocomponent
        tileInfoComponent targetedTileInfoComponent = targetedTile.GetComponent<tileInfoComponent>();

        //Assign the Transform component initially
        selectedTilesTransform = selectedTile.transform;
        targetedTilesTransform = targetedTile.transform;

        //Assign initial transforms vectors
        selectedTileStartPos = selectedTile.transform.position;
        targetedTileStartPos = targetedTile.transform.position; 


        

        while(moveTileCounter <= moveTileTimer)
        {
            //Swap the places of the two tiles 
            selectedTilesTransform.position = Vector3.Lerp(selectedTileStartPos, targetedTileStartPos, moveTileCounter / moveTileTimer);
            targetedTilesTransform.position = Vector3.Lerp(targetedTileStartPos, selectedTileStartPos, moveTileCounter / moveTileTimer);


            moveTileCounter += Time.deltaTime;

            yield return null;
        }
        //Once the movement has ended, rearrange our main array
        //Assign the targeted tile and the selected tile depending on the moved side and selectedTiles location
        if (movedSide == "Right")
        {                   
            //Array position
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x + 1, selectedTileInfoComponent.tileLocation.y] = selectedTile;
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x, selectedTileInfoComponent.tileLocation.y] = targetedTile;
        }
        if (movedSide == "Left")
        {
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x - 1, selectedTileInfoComponent.tileLocation.y] = selectedTile;
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x, selectedTileInfoComponent.tileLocation.y] = targetedTile;
        }
        if (movedSide == "Up")
        {
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x , selectedTileInfoComponent.tileLocation.y - 1] = selectedTile;
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x, selectedTileInfoComponent.tileLocation.y] = targetedTile;
        }
        if (movedSide == "Down")
        {
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x , selectedTileInfoComponent.tileLocation.y + 1] = selectedTile;
            gridObjectsArray[selectedTileInfoComponent.tileLocation.x, selectedTileInfoComponent.tileLocation.y] = targetedTile;
        }

        //Tile Location information updated aswell for both selected and targeted tile
        //Holder to swap
        Vector2Int swapVectorHolder = selectedTileInfoComponent.tileLocation;
        selectedTileInfoComponent.tileLocation = new Vector2Int(targetedTileInfoComponent.tileLocation.x, targetedTileInfoComponent.tileLocation.y);
        targetedTileInfoComponent.tileLocation = swapVectorHolder;
        //---REARRANGE MOVED PIECES COMPLETE---
        //******************************************************
        //******************************************************



        //******************************************************
        //******************************************************
        //After we moved our pieces, we need to check for any potential matches that they have made
        //STARTING TO SEARCH FOR MATCHES 
        if (movedSide == "Right")
        {
            
            //Start checking matches for the SELECTED TILE
            checkRightMatches(selectedTileInfoComponent.tileLocation,selectedTile);
            checkLowerMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkUpMatches(selectedTileInfoComponent.tileLocation, selectedTile);

            //Start checking matches for the TARGETED TILE
            checkTargetLeftMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetLowerMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetUpMatches(targetedTileInfoComponent.tileLocation, targetedTile);
        }
        
      
        if (movedSide == "Left")
        {
            //Start checking matches for the SELECTED TILE
            checkLeftMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkLowerMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkUpMatches(selectedTileInfoComponent.tileLocation, selectedTile);

            //Start checking matches for the TARGETED TILE
            checkTargetRightMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetLowerMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetUpMatches(targetedTileInfoComponent.tileLocation, targetedTile);
        }
        if (movedSide == "Up")
        {
            //Start checking matches for the SELECTED TILE
            checkRightMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkLeftMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkUpMatches(selectedTileInfoComponent.tileLocation, selectedTile);

            //Start checking matches for the TARGETED TILE
            checkTargetLeftMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetLowerMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetRightMatches(targetedTileInfoComponent.tileLocation, targetedTile);
        }
        if (movedSide == "Down")
        {
            //Start checking matches for the SELECTED TILE
            checkRightMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkLowerMatches(selectedTileInfoComponent.tileLocation, selectedTile);
            checkLeftMatches(selectedTileInfoComponent.tileLocation, selectedTile);

            //Start checking matches for the TARGETED TILE
            checkTargetLeftMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetRightMatches(targetedTileInfoComponent.tileLocation, targetedTile);
            checkTargetUpMatches(targetedTileInfoComponent.tileLocation, targetedTile);
        }
        //COMPLETED CHECKING FOR MATCHES FOR PLAYERS MOVEMENT
        //******************************************************
        //******************************************************




        //******************************************************
        //******************************************************
        //CALCULATE SHAPE OF THE GIVEN MATCHES
        //If any of the matches are a shape that we want to be matched...

        //Holder for the shape list that the calculateMatchShape will bring, one for selected, one for targeted...
        List<GameObject> selectedMatchShape = new List<GameObject>();
        List<GameObject> targetedMatchShape = new List<GameObject>();
        if (movedSide == "Right")
        {
           
            //Start checking MATCHED SHAPES for the SELECTED TILE
            selectedMatchShape = calculateMatchShape("Right", selectedTile, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter,
                upMatchesCounter, selectedLeftMatchesList, selectedRightMatchesList,
                selectedUpMatchesList,selectedLowerMatchesList );

            //Start checking MATCHED SHAPES for the TARGETED TILE
            targetedMatchShape = calculateMatchShape("Left", targetedTile, leftMatchesTargetCounter, rightMatchesTargetedCounter, lowerTargetMatchesCounter,
                upMatchesTargetCounter, targetedLeftMatchesList, targetedRightMatchesList,
                targetedUpMatchesList, targetedLowerMatchesList);
        }
        if (movedSide == "Left")
        {
            //Start checking MATCHED SHAPES for the SELECTED TILE
            selectedMatchShape = calculateMatchShape("Left", selectedTile, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter,
                upMatchesCounter, selectedLeftMatchesList, selectedRightMatchesList,
                selectedUpMatchesList, selectedLowerMatchesList);

            //Start checking MATCHED SHAPES for the TARGETED TILE
            targetedMatchShape = calculateMatchShape("Right", targetedTile, leftMatchesTargetCounter, rightMatchesTargetedCounter, lowerTargetMatchesCounter,
                upMatchesTargetCounter, targetedLeftMatchesList, targetedRightMatchesList,
                targetedUpMatchesList, targetedLowerMatchesList);
        }
        if (movedSide == "Up")
        {
            //Start checking MATCHED SHAPES for the SELECTED TILE
            selectedMatchShape = calculateMatchShape("Up", selectedTile, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter,
                upMatchesCounter, selectedLeftMatchesList, selectedRightMatchesList,
                selectedUpMatchesList, selectedLowerMatchesList);

            //Start checking MATCHED SHAPES for the TARGETED TILE
            targetedMatchShape = calculateMatchShape("Down", targetedTile, leftMatchesTargetCounter, rightMatchesTargetedCounter, lowerTargetMatchesCounter,
                upMatchesTargetCounter, targetedLeftMatchesList, targetedRightMatchesList,
                targetedUpMatchesList, targetedLowerMatchesList);
        }
        if (movedSide == "Down")
        {
            //Start checking MATCHED SHAPES for the SELECTED TILE
            selectedMatchShape = calculateMatchShape("Down", selectedTile, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter,
                upMatchesCounter, selectedLeftMatchesList, selectedRightMatchesList,
                selectedUpMatchesList, selectedLowerMatchesList);

            //Start checking MATCHED SHAPES for the TARGETED TILE
            targetedMatchShape = calculateMatchShape("Up", targetedTile, leftMatchesTargetCounter, rightMatchesTargetedCounter, lowerTargetMatchesCounter,
                upMatchesTargetCounter, targetedLeftMatchesList, targetedRightMatchesList,
                targetedUpMatchesList, targetedLowerMatchesList);
        }
        //COMPLETED SHAPES CALCULATIONS FOR PLAYER MOVED TILES
        //******************************************************
        //******************************************************

        //Clear lists to use later
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();
        //Targeted
        targetedLeftMatchesList.Clear();
        targetedRightMatchesList.Clear();
        targetedUpMatchesList.Clear();
        targetedLowerMatchesList.Clear();
        //
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //Targeted
        upMatchesTargetCounter = 0;
        lowerTargetMatchesCounter = 0;
        leftMatchesTargetCounter = 0;
        rightMatchesTargetedCounter = 0;

        List<GameObject> combinedMatchShapes = new List<GameObject>();
        //If both are not null
        if (selectedMatchShape != null && targetedMatchShape != null)
        {
             combinedMatchShapes = targetedMatchShape.Union(selectedMatchShape).ToList();
        }
        //if either one is null
        if(selectedMatchShape == null && targetedMatchShape != null)
        {
            combinedMatchShapes = targetedMatchShape;
        }
        if (targetedMatchShape == null && selectedMatchShape != null)
        {
            combinedMatchShapes = selectedMatchShape;
        }


        //******************************************************
        //******************************************************
        //RESHAPE THE MAIN ARRAY AND UPDATE TILE LOCATIONS
        //Holders lists that store the changed pieces
        List<GameObject> selectedReshapedTilestList = new List<GameObject>();
        List<GameObject> targetedReshapedTilesList = new List<GameObject>();
        /*
        if (selectedMatchShape != null)
        {
            selectedReshapedTilestList = reshapeArray(selectedMatchShape);
        }

        if (targetedMatchShape != null)
        {
            targetedReshapedTilesList = reshapeArray(targetedMatchShape);
        }
        */
        if(combinedMatchShapes != null && combinedMatchShapes.Count > 0) 
        {
            
            //Debug.Log("reshaped as");
            reshapeArray(combinedMatchShapes);
        }

        List<GameObject> combinedReshapedTilesList = selectedReshapedTilestList.Union(targetedReshapedTilesList).ToList();

        //Debug.Log("Couty se !" + combinedReshapedTilesList.Count);

        //checkNaturalMatches(combinedReshapedTilesList , gridObjectsArray);

        //reformGridObjectsArray(combinedReshapedTilesList);

        /*
        checkNaturalMatches(selectedReshapedTilestList, gridObjectsArray);
        reformGridObjectsArray(selectedReshapedTilestList);

        
        checkNaturalMatches(targetedReshapedTilesList, gridObjectsArray);
        reformGridObjectsArray(targetedReshapedTilesList);
        */


      

        //reformGridObjectsArray(selectedReshapedTilestList);
        //reformGridObjectsArray(targetedReshapedTilesList);

        //RESET CERTAIN VARIABLES FOR THE NEXT ITERATIONS
        //Reset counters
        //Selected
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //Targeted
        upMatchesTargetCounter = 0;
        lowerTargetMatchesCounter = 0;
        leftMatchesTargetCounter = 0;
        rightMatchesTargetedCounter = 0;

        //******************************

        //Clear lists to use later
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();
        //Targeted      
        targetedLeftMatchesList.Clear();
        targetedRightMatchesList.Clear();
        targetedUpMatchesList.Clear();
        targetedLowerMatchesList.Clear();

        //General lists
        targetedMatchedShapesList.Clear();
        selectedMatchedShapesList.Clear();


        startedMoveTileRoutine = false;
        moveTileCounter = 0;

        
    }

    //test VERSION *********IGNORE THIS METHOD*************
    private List<GameObject> reshapeArrayTest(List<GameObject> matchedShapes)
    {

        //Increment score by matched shape amount and change score
        userScoreInt += matchedShapes.Count;
        scoreText.text = userScoreInt.ToString();

        //Clear the holder lists and numbers for the next iteration
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //******************************
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();



        //Use this to keep track of every changed tile so we can update it later
        List<tileInfoComponent> changedPiecesList = new List<tileInfoComponent>();

        //Holder to customize the main array, will later equalize gridObjectsArray to this
        GameObject[,] holderGridObjectsArray = new GameObject[gridColumns, gridRows];

        holderGridObjectsArray = gridObjectsArray;






        // Fade the matched tiles RECHECK LATER
        foreach (GameObject matchedSelectedTile in matchedShapes)
        {


            if (matchedSelectedTile.GetComponent<tileInfoComponent>().startedDeactivationRoutine == false)
            {
                tileInfoComponent matchedSelectedTileComponent = matchedSelectedTile.GetComponent<tileInfoComponent>();
                //Add to available pooled items

                if (matchedSelectedTileComponent.tileColor == "Blue")
                {
                    pooledBlueReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Red")
                {
                    pooledRedReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Green")
                {
                    pooledGreenReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Yellow")
                {
                    pooledYellowReplacements.Add(matchedSelectedTileComponent.gameObject);
                }


                //StartCoroutine(matchedSelectedTileComponent.fadeRoutine());
            }

            matchedSelectedTile.SetActive(false);
        }


        foreach (GameObject matched in matchedShapes)
        {
            //Debug.Log("Matched was" + matched);
        }



        //Dropped amount for the given column
        int[] droppedColumnAmountHolder = new int[gridColumns];
        int[] droppedColumnHighestMatchPointHolder = new int[gridColumns];



        //Initialize the array assuming the max number of drops
        for (int i = 0; i < droppedColumnHighestMatchPointHolder.Length; i++)
        {
            droppedColumnHighestMatchPointHolder[i] = gridRows + 1;
        }

        //Iterate through each of the matched tiles to reshape the array
        foreach (GameObject matchedTile in matchedShapes)
        {
            //Assign location component to reuse later
            tileInfoComponent matchedTileInfoComponent = matchedTile.GetComponent<tileInfoComponent>();

            //Highest match point      
            if (droppedColumnHighestMatchPointHolder[matchedTileInfoComponent.tileLocation.x] > matchedTileInfoComponent.tileLocation.y)
            {
                droppedColumnHighestMatchPointHolder[matchedTileInfoComponent.tileLocation.x] = matchedTileInfoComponent.tileLocation.y;
            }

            //Amount dropped
            droppedColumnAmountHolder[matchedTileInfoComponent.tileLocation.x] += 1;
        }


        /*
        Debug.Log("[" + droppedColumnAmountHolder[0] + "] " + "[" + droppedColumnAmountHolder[1] + "] " + "[" + droppedColumnAmountHolder[2] + "] " + "[" + droppedColumnAmountHolder[3] + "] " +
            droppedColumnAmountHolder[4] + "] ");

        Debug.Log("[" + droppedColumnHighestMatchPointHolder[0] + "] " + "[" + droppedColumnHighestMatchPointHolder[1] + "] " + "[" + droppedColumnHighestMatchPointHolder[2] + "] " + "[" + droppedColumnHighestMatchPointHolder[3] + "] " +
            droppedColumnHighestMatchPointHolder[4] + "] ");
        
        */

        /*

        //** Now we know the dropped amount and highest match point for the specific columns
        // we have 5 columns and 6 rows
          
        //Adjust from the highest match point and up 
        for(int i = 0; i < holderGridObjectsArray.GetLength(0); i++)
        {
            if (droppedColumnAmountHolder[i] > 0)
            {

                for (int j = 0; j < droppedColumnHighestMatchPointHolder[i]; j++)
                {
              
                    //Assign the tileInfo once 
                    tileInfoComponent checkedTileInfoComponent = holderGridObjectsArray[i, j].GetComponent<tileInfoComponent>();
                    //Update tileLoc info                
                    checkedTileInfoComponent.tileLocation = new Vector2Int(checkedTileInfoComponent.tileLocation.x, checkedTileInfoComponent.tileLocation.y + droppedColumnAmountHolder[i]);
                    //Add the changed piece to the list to iterate through later and update gridobjectsarray
                    changedPiecesList.Add(checkedTileInfoComponent);



                    //Debug.Log("Dropping " + checkedTileInfoComponent.gameObject);
                    checkedTileInfoComponent.moveTileCounter = 0f;
                    //StartCoroutine(checkedTileInfoComponent.dropTileRoutine(droppedColumnAmountHolder[i]));
                    checkedTileInfoComponent.dropTile(droppedColumnAmountHolder[i]);


                }
            }

        }
        */

        //**** HERE We visually dropped the pieces that were already on the board and updated their tile info *****

        //**** Now we need to drop new pieces to fill in the empty spaces ****
        List<GameObject> newTilesList = replacementTileGenerator(matchedShapes.Count);

        //Debug.Log("12aa" + newTilesList.Count);
        //Debug.Log(matchedShapes.Count);

        //see which index we should take the tiles from
        int newTilesListIncrementer = 0;
        //Update the newly falling pieces tileLocation information 
        for (int i = 0; i < droppedColumnAmountHolder.Length; i++)
        {

            //If a drop has occured in the given column
            if (droppedColumnAmountHolder[i] > 0)
            {

                //Iterate through the list of new tiles to place them
                for (int j = 0; j < droppedColumnAmountHolder[i]; j++)
                {

                    //Set active the newly coming pieces
                    newTilesList[newTilesListIncrementer].SetActive(true);
                    //Assign once
                    tileInfoComponent newTilesInfoComponent = newTilesList[newTilesListIncrementer].GetComponent<tileInfoComponent>();
                    //Update tileLocation information with the matchedShapes information
                    newTilesInfoComponent.tileLocation = new Vector2Int(i, j);

                    //Set transform to above the screen 
                    newTilesInfoComponent.transform.position = new Vector2(mainGridGenerator.firstGridX + newTilesInfoComponent.tileLocation.x, mainGridGenerator.firstGridY + 1);

                    //Add the dropped pieces to the changed pieces list
                    changedPiecesList.Add(newTilesInfoComponent);

                    //Drop the tiles UNCOMMENT LATER TO WORK PROPERLY                    
                    if (newTilesInfoComponent.startedDropTileRoutine == false)
                    {
                        newTilesInfoComponent.dropTile(j + 1);
                        //StartCoroutine(newTilesInfoComponent.dropTileRoutine(j + 1));
                    }






                    newTilesListIncrementer += 1;
                }

            }
        }


        //** Now we know the dropped amount and highest match point for the specific columns
        // we have 5 columns and 6 rows

        //Adjust from the highest match point and up 
        for (int i = 0; i < holderGridObjectsArray.GetLength(0); i++)
        {
            if (droppedColumnAmountHolder[i] > 0)
            {

                for (int j = 0; j < droppedColumnHighestMatchPointHolder[i]; j++)
                {

                    //Assign the tileInfo once 
                    tileInfoComponent checkedTileInfoComponent = holderGridObjectsArray[i, j].GetComponent<tileInfoComponent>();
                    //Update tileLoc info                
                    checkedTileInfoComponent.tileLocation = new Vector2Int(checkedTileInfoComponent.tileLocation.x, checkedTileInfoComponent.tileLocation.y + droppedColumnAmountHolder[i]);
                    //Add the changed piece to the list to iterate through later and update gridobjectsarray
                    changedPiecesList.Add(checkedTileInfoComponent);



                    //Debug.Log("Dropping " + checkedTileInfoComponent.gameObject);
                    checkedTileInfoComponent.moveTileCounter = 0f;
                    //StartCoroutine(checkedTileInfoComponent.dropTileRoutine(droppedColumnAmountHolder[i]));
                    checkedTileInfoComponent.dropTile(droppedColumnAmountHolder[i]);




                }
            }

        }

        //Reform the gridobjectsarray to the correct information
        //reformGridObjectsArray(droppedColumnAmountHolder, changedPiecesList);





        //tester

        List<GameObject> testObjList = new List<GameObject>();

        //access the gameobject of changedpieces list to call



        // Debug.Log("Count a" +testObjList.Count);

        /*
        // test pring
        for (int i = 0; i < gridObjectsArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridObjectsArray.GetLength(1); j++)
            {
                Debug.Log(" Array " + i + " " + j + " is " + gridObjectsArray[i, j]);
            }
        }
        */

        foreach (tileInfoComponent testAdd in changedPiecesList)
        {
            testObjList.Add(testAdd.gameObject);
            // Debug.Log("Added asd" + testAdd.gameObject + " Loc was : " + testAdd.GetComponent<tileInfoComponent>().tileLocation);
        }


        reformGridObjectsArray(testObjList);

        List<GameObject> SortedList = testObjList.OrderBy(o => o.GetComponent<tileInfoComponent>().tileLocation.y).ToList();



        foreach (GameObject testAdd in SortedList)
        {
            //SortedList.Add(testAdd.gameObject);
            Debug.Log("Added " + testAdd.gameObject + " Loc was : " + testAdd.GetComponent<tileInfoComponent>().tileLocation);
        }

        //Reverse the list to iterate from the bottom up 
        SortedList.Reverse();


        if (SortedList.Count > 0)
        {

            //checkNaturalMatches(testObjList , gridObjectsArray);
            checkNaturalMatchesTest(SortedList, gridObjectsArray);

        }


        //Debug.Log("Returned a list the size of " + testObjList.Count);
        return testObjList;


    }






    //************************** CURRENTLY USED METHOD TO RESHAPE THE ARRAY ************************************
    //score holder int
    static int userScoreInt;
    private List<GameObject> reshapeArray(List<GameObject> matchedShapes)
    {

        //Increment score by matched shape amount and change score
        userScoreInt += matchedShapes.Count;
        scoreText.text = userScoreInt.ToString();

        //Clear the holder lists and numbers for the next iteration
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //******************************
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();



        //Use this to keep track of every changed tile so we can update it later
        List<tileInfoComponent> changedPiecesList = new List<tileInfoComponent>();

        //Holder to customize the main array, will later equalize gridObjectsArray to this
        GameObject[,] holderGridObjectsArray = new GameObject[gridColumns, gridRows];
        
        holderGridObjectsArray = gridObjectsArray;



        


        // Fade the matched tiles RECHECK LATER
        foreach (GameObject matchedSelectedTile in matchedShapes)
        {
            

            if (matchedSelectedTile.GetComponent<tileInfoComponent>().startedDeactivationRoutine == false)
            {
                tileInfoComponent matchedSelectedTileComponent = matchedSelectedTile.GetComponent<tileInfoComponent>();
                //Add to available pooled items
                
                if(matchedSelectedTileComponent.tileColor == "Blue")
                {
                    pooledBlueReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Red")
                {
                    pooledRedReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Green")
                {
                    pooledGreenReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                if (matchedSelectedTileComponent.tileColor == "Yellow")
                {
                    pooledYellowReplacements.Add(matchedSelectedTileComponent.gameObject);
                }
                
                
                //StartCoroutine(matchedSelectedTileComponent.fadeRoutine());
            }

            matchedSelectedTile.SetActive(false);
        }


        foreach (GameObject matched in matchedShapes)
        {
            //Debug.Log("Matched was" + matched);
        }



        //Dropped amount for the given column
        int[] droppedColumnAmountHolder = new int[gridColumns];
        int[] droppedColumnHighestMatchPointHolder = new int[gridColumns];

    

        //Initialize the array assuming the max number of drops
        for (int i = 0; i < droppedColumnHighestMatchPointHolder.Length; i++)
        {
            droppedColumnHighestMatchPointHolder[i] = gridRows + 1;
        }

        //Iterate through each of the matched tiles to reshape the array
        foreach(GameObject matchedTile in matchedShapes)
        {
            //Assign location component to reuse later
            tileInfoComponent matchedTileInfoComponent = matchedTile.GetComponent<tileInfoComponent>();    

            //Highest match point      
            if (droppedColumnHighestMatchPointHolder[matchedTileInfoComponent.tileLocation.x] > matchedTileInfoComponent.tileLocation.y)
            {
                droppedColumnHighestMatchPointHolder[matchedTileInfoComponent.tileLocation.x] = matchedTileInfoComponent.tileLocation.y;
            }

            //Amount dropped
            droppedColumnAmountHolder[matchedTileInfoComponent.tileLocation.x] += 1;         
        }

    
        /*
        Debug.Log("[" + droppedColumnAmountHolder[0] + "] " + "[" + droppedColumnAmountHolder[1] + "] " + "[" + droppedColumnAmountHolder[2] + "] " + "[" + droppedColumnAmountHolder[3] + "] " +
            droppedColumnAmountHolder[4] + "] ");

        Debug.Log("[" + droppedColumnHighestMatchPointHolder[0] + "] " + "[" + droppedColumnHighestMatchPointHolder[1] + "] " + "[" + droppedColumnHighestMatchPointHolder[2] + "] " + "[" + droppedColumnHighestMatchPointHolder[3] + "] " +
            droppedColumnHighestMatchPointHolder[4] + "] ");
        
        */

        /*

        //** Now we know the dropped amount and highest match point for the specific columns
        // we have 5 columns and 6 rows
          
        //Adjust from the highest match point and up 
        for(int i = 0; i < holderGridObjectsArray.GetLength(0); i++)
        {
            if (droppedColumnAmountHolder[i] > 0)
            {

                for (int j = 0; j < droppedColumnHighestMatchPointHolder[i]; j++)
                {
              
                    //Assign the tileInfo once 
                    tileInfoComponent checkedTileInfoComponent = holderGridObjectsArray[i, j].GetComponent<tileInfoComponent>();
                    //Update tileLoc info                
                    checkedTileInfoComponent.tileLocation = new Vector2Int(checkedTileInfoComponent.tileLocation.x, checkedTileInfoComponent.tileLocation.y + droppedColumnAmountHolder[i]);
                    //Add the changed piece to the list to iterate through later and update gridobjectsarray
                    changedPiecesList.Add(checkedTileInfoComponent);



                    //Debug.Log("Dropping " + checkedTileInfoComponent.gameObject);
                    checkedTileInfoComponent.moveTileCounter = 0f;
                    //StartCoroutine(checkedTileInfoComponent.dropTileRoutine(droppedColumnAmountHolder[i]));
                    checkedTileInfoComponent.dropTile(droppedColumnAmountHolder[i]);


                }
            }

        }
        */
        
        //**** HERE We visually dropped the pieces that were already on the board and updated their tile info *****

        //**** Now we need to drop new pieces to fill in the empty spaces ****
        List<GameObject> newTilesList = replacementTileGenerator(matchedShapes.Count);

        //Debug.Log("12aa" + newTilesList.Count);
        //Debug.Log(matchedShapes.Count);

        //see which index we should take the tiles from
        int newTilesListIncrementer = 0;
        //Update the newly falling pieces tileLocation information 
        for(int i = 0; i < droppedColumnAmountHolder.Length; i++)
        {
     
            //If a drop has occured in the given column
            if(droppedColumnAmountHolder[i] > 0)
            {
               
                //Iterate through the list of new tiles to place them
                for (int j = 0; j < droppedColumnAmountHolder[i]; j++)
                {
                    
                    //Set active the newly coming pieces
                    newTilesList[newTilesListIncrementer].SetActive(true);
                    //Assign once
                    tileInfoComponent newTilesInfoComponent = newTilesList[newTilesListIncrementer].GetComponent<tileInfoComponent>();
                    //Update tileLocation information with the matchedShapes information
                    newTilesInfoComponent.tileLocation = new Vector2Int(i , j);

                    //Set transform to above the screen 
                    newTilesInfoComponent.transform.position = new Vector2(mainGridGenerator.firstGridX + newTilesInfoComponent.tileLocation.x, mainGridGenerator.firstGridY + 1);

                    //Add the dropped pieces to the changed pieces list
                    changedPiecesList.Add(newTilesInfoComponent);

                    //Drop the tiles UNCOMMENT LATER TO WORK PROPERLY                    
                    if (newTilesInfoComponent.startedDropTileRoutine == false)
                    {
                        newTilesInfoComponent.dropTile(j + 1);
                        //StartCoroutine(newTilesInfoComponent.dropTileRoutine(j + 1));
                    }
                    

                   



                    newTilesListIncrementer += 1;
                }
                            
            }
        }


        //** Now we know the dropped amount and highest match point for the specific columns
        // we have 5 columns and 6 rows

        //Adjust from the highest match point and up 
        for (int i = 0; i < holderGridObjectsArray.GetLength(0); i++)
        {
            if (droppedColumnAmountHolder[i] > 0)
            {

                for (int j = 0; j < droppedColumnHighestMatchPointHolder[i]; j++)
                {

                    //Assign the tileInfo once 
                    tileInfoComponent checkedTileInfoComponent = holderGridObjectsArray[i, j].GetComponent<tileInfoComponent>();
                    //Update tileLoc info                
                    checkedTileInfoComponent.tileLocation = new Vector2Int(checkedTileInfoComponent.tileLocation.x, checkedTileInfoComponent.tileLocation.y + droppedColumnAmountHolder[i]);
                    //Add the changed piece to the list to iterate through later and update gridobjectsarray
                    changedPiecesList.Add(checkedTileInfoComponent);


                    
                    //Debug.Log("Dropping " + checkedTileInfoComponent.gameObject);
                    checkedTileInfoComponent.moveTileCounter = 0f;
                    //StartCoroutine(checkedTileInfoComponent.dropTileRoutine(droppedColumnAmountHolder[i]));
                    checkedTileInfoComponent.dropTile(droppedColumnAmountHolder[i]);
                    



                }
            }

        }

        //Reform the gridobjectsarray to the correct information
        //reformGridObjectsArray(droppedColumnAmountHolder, changedPiecesList);





        //tester

        List<GameObject> testObjList = new List<GameObject>();

        //access the gameobject of changedpieces list to call



        // Debug.Log("Count a" +testObjList.Count);

        /*
        // test pring
        for (int i = 0; i < gridObjectsArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridObjectsArray.GetLength(1); j++)
            {
                Debug.Log(" Array " + i + " " + j + " is " + gridObjectsArray[i, j]);
            }
        }
        */

        foreach (tileInfoComponent testAdd in changedPiecesList)
        {
            testObjList.Add(testAdd.gameObject);
           // Debug.Log("Added asd" + testAdd.gameObject + " Loc was : " + testAdd.GetComponent<tileInfoComponent>().tileLocation);
        }


        reformGridObjectsArray(testObjList);

        List<GameObject> SortedList = testObjList.OrderBy(o => o.GetComponent<tileInfoComponent>().tileLocation.y).ToList();

        

        foreach (GameObject testAdd in SortedList)
        {
            //SortedList.Add(testAdd.gameObject);
            Debug.Log("Added " + testAdd.gameObject + " Loc was : " + testAdd.GetComponent<tileInfoComponent>().tileLocation);
        }

        //Reverse the list to iterate from the bottom up 
        SortedList.Reverse();


        if (SortedList.Count > 0 )
        {

            //checkNaturalMatches(testObjList , gridObjectsArray);
            checkNaturalMatchesTest(SortedList, gridObjectsArray);

        }


        //Debug.Log("Returned a list the size of " + testObjList.Count);
        return testObjList;

         
    }

    //459328690438t69043ty9043ý90y*g43ý90hg3490hý309h43ý0h4ý309*hý34*0hý943
    //3029jfg0932gj0923gj0923gj0932gj3209

    //**************************** CURRENTLY USED SCRIPT TO CHECK NATURAL MATCHES ************************************
    private void checkNaturalMatchesTest(List<GameObject> changedPiecesList, GameObject[,] gridObjectsArray)
    {
        //testint += 1;
        //Clear between objects
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //******************************
        //Clear lists to use later
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();


        List<GameObject> matchesList = new List<GameObject>();

        for (int i = 0; i < gridObjectsArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridObjectsArray.GetLength(1); j++)
            {

                matchesList.Add(gridObjectsArray[i, j]);
            }
        }


        foreach (GameObject asd in changedPiecesList)
        {
            Debug.Log("Changed pieces contained " + " in iteration " + naturalMatchTester + "  " + asd.name + " Loc was : " + asd.GetComponent<tileInfoComponent>().tileLocation);
        }

       

        foreach (GameObject changedPiece in matchesList)
        {

            
            //Tile location info
            tileInfoComponent changedTileInfo = changedPiece.GetComponent<tileInfoComponent>();
            Vector2Int changedTileLocVec = new Vector2Int(changedTileInfo.tileLocation.x, changedTileInfo.tileLocation.y);

            Debug.Log("Iteration " + naturalMatchTester + " Started checking game object " + changedTileInfo.gameObject + " From location " + changedTileLocVec);

            //Test checking for up too
            checkUpMatches(changedTileLocVec, changedPiece);
            //---test---

            checkLeftMatches(changedTileLocVec, changedPiece);
            checkRightMatches(changedTileLocVec, changedPiece);
            checkLowerMatches(changedTileLocVec, changedPiece);

            //assign new matches to matchesList
            /*
            List<GameObject> matchesList = calculateMatchShape("Down", changedPiece, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter, upMatchesCounter,
                selectedLeftMatchesList, selectedRightMatchesList, selectedUpMatchesList, selectedLowerMatchesList);
            */

            matchesList = calculateMatchShape("Down", changedPiece, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter, upMatchesCounter,
                selectedLeftMatchesList, selectedRightMatchesList, selectedUpMatchesList, selectedLowerMatchesList);



            if (matchesList != null)
            {
                Debug.Log("Was not null for " + changedTileInfo.gameObject);
            
                naturalMatchTester += 1;
                Debug.Log("Some natural match occured NUMBER :" + naturalMatchTester);
                //Debug.Log("---MATCH START---");
                foreach (GameObject testobj in matchesList)
                {
                    Debug.Log("These were matched in some way " + testobj.name);
                }
                //Debug.Log("---MATCH END---");
                //Debug.Log("matches list count " + matchesList.Count);

                //Reshape the array
                reshapeArray(matchesList);

                //Clear the matchesList to get ready for the next iteration
                matchesList.Clear();

                //Clear the holder lists and numbers for the next iteration
                upMatchesCounter = 0;
                lowerMatchesCounter = 0;
                leftMatchesCounter = 0;
                rightMatchesCounter = 0;
                //******************************
                //Selected
                selectedLeftMatchesList.Clear();
                selectedRightMatchesList.Clear();
                selectedUpMatchesList.Clear();
                selectedLowerMatchesList.Clear();

                return;


            }



            //Clear the holder lists and numbers for the next iteration
            upMatchesCounter = 0;
            lowerMatchesCounter = 0;
            leftMatchesCounter = 0;
            rightMatchesCounter = 0;
            //******************************
            //Selected
            selectedLeftMatchesList.Clear();
            selectedRightMatchesList.Clear();
            selectedUpMatchesList.Clear();
            selectedLowerMatchesList.Clear();

        }
    }


    //Check the whole board again for possible matches after the new board is formed THIS IS THE OLD VERSION
    //test VERSION *********IGNORE THIS METHOD *************
    static int naturalMatchTester;
    private void checkNaturalMatches(List<GameObject> changedPiecesList , GameObject[,] gridObjectsArray)
    {
        //testint += 1;
        //Clear between objects
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //******************************
        //Clear lists to use later
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();

       

        foreach (GameObject asd in changedPiecesList)
        {
            Debug.Log("Changed pieces contained " + " in iteration " + naturalMatchTester+"  " + asd.name + " Loc was : " + asd.GetComponent<tileInfoComponent>().tileLocation);
        }

        //Keeping track of pieces that were already matched with this list
        List<GameObject> alreadyMatchedList = new List<GameObject>();

        List<GameObject> matchesList = new List<GameObject>();
            

        foreach (GameObject changedPiece in changedPiecesList)
        {
            
                
            

            //Clear between objects
            upMatchesCounter = 0;
            lowerMatchesCounter = 0;
            leftMatchesCounter = 0;
            rightMatchesCounter = 0;
            //******************************
            //Clear lists to use later
            //Selected
            selectedLeftMatchesList.Clear();
            selectedRightMatchesList.Clear();
            selectedUpMatchesList.Clear();
            selectedLowerMatchesList.Clear();

            foreach (GameObject asd in alreadyMatchedList)
            {
                Debug.Log("ALREADY MATCHED PIECES  " + " in iteration " + naturalMatchTester + "  " + asd.name + " Loc was : " + asd.GetComponent<tileInfoComponent>().tileLocation);
            } 

            if (alreadyMatchedList.Contains(changedPiece) == false)
            {
            
                //Tile location info
                tileInfoComponent changedTileInfo = changedPiece.GetComponent<tileInfoComponent>();
                Vector2Int changedTileLocVec = new Vector2Int(changedTileInfo.tileLocation.x, changedTileInfo.tileLocation.y);

                Debug.Log("Iteration " + naturalMatchTester + " Started checking game object " + changedTileInfo.gameObject + " From location " + changedTileLocVec);

                //Test checking for up too
                checkUpMatches(changedTileLocVec, changedPiece);
                //---test---

                checkLeftMatches(changedTileLocVec, changedPiece);
                checkRightMatches(changedTileLocVec, changedPiece);
                checkLowerMatches(changedTileLocVec, changedPiece);

                

                //assign new matches to matchesList
                /*
                List<GameObject> matchesList = calculateMatchShape("Down", changedPiece, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter, upMatchesCounter,
                    selectedLeftMatchesList, selectedRightMatchesList, selectedUpMatchesList, selectedLowerMatchesList);
                */
                

                matchesList = calculateMatchShape("Down", changedPiece, leftMatchesCounter, rightMatchesCounter, lowerMatchesCounter, upMatchesCounter,
                    selectedLeftMatchesList, selectedRightMatchesList, selectedUpMatchesList, selectedLowerMatchesList);



                if (matchesList != null)
                {
                    Debug.Log("Was not null for " + changedTileInfo.gameObject);
                    //Populate the already matched pieces list to not iterate over them
                    foreach (GameObject matchedPieces in matchesList)
                    {                        
                        alreadyMatchedList.Add(matchedPieces);
                    }
                    naturalMatchTester += 1;
                    Debug.Log("Some natural match occured NUMBER :" + naturalMatchTester);
                    //Debug.Log("---MATCH START---");
                    foreach (GameObject testobj in matchesList)
                    {
                        Debug.Log("These were matched in some way " + testobj.name);
                    }
                    //Debug.Log("---MATCH END---");
                    //Debug.Log("matches list count " + matchesList.Count);

                    //Reshape the array
                    reshapeArray(matchesList);

                    //Clear the matchesList to get ready for the next iteration
                    matchesList.Clear();

                    //Clear the holder lists and numbers for the next iteration
                    upMatchesCounter = 0;
                    lowerMatchesCounter = 0;
                    leftMatchesCounter = 0;
                    rightMatchesCounter = 0;
                    //******************************
                    //Selected
                    selectedLeftMatchesList.Clear();
                    selectedRightMatchesList.Clear();
                    selectedUpMatchesList.Clear();
                    selectedLowerMatchesList.Clear();

                    return;
                    
                }
                else
                {
                    Debug.Log("WAS NULL  for " + changedTileInfo.gameObject);
                    //If no match was available, add the piece to the already checked list aswell for each direction
                    foreach (GameObject nonmatchedTiles in selectedLeftMatchesList)
                    {
                        alreadyMatchedList.Add(nonmatchedTiles);
                    }
                    foreach (GameObject nonmatchedTiles in selectedRightMatchesList)
                    {

                        alreadyMatchedList.Add(nonmatchedTiles);
                    }
                    foreach (GameObject nonmatchedTiles in selectedUpMatchesList)
                    {

                        alreadyMatchedList.Add(nonmatchedTiles);
                    }
                    foreach (GameObject nonmatchedTiles in selectedLowerMatchesList)
                    {

                        alreadyMatchedList.Add(nonmatchedTiles);
                    }

                    //Clear the holder lists and numbers for the next iteration
                    upMatchesCounter = 0;
                    lowerMatchesCounter = 0;
                    leftMatchesCounter = 0;
                    rightMatchesCounter = 0;
                    //******************************
                    //Selected
                    selectedLeftMatchesList.Clear();
                    selectedRightMatchesList.Clear();
                    selectedUpMatchesList.Clear();
                    selectedLowerMatchesList.Clear();

                }
               

            }
        

        }

     

        //Clear the holder lists and numbers for the next iteration
        upMatchesCounter = 0;
        lowerMatchesCounter = 0;
        leftMatchesCounter = 0;
        rightMatchesCounter = 0;
        //******************************
        //Selected
        selectedLeftMatchesList.Clear();
        selectedRightMatchesList.Clear();
        selectedUpMatchesList.Clear();
        selectedLowerMatchesList.Clear();

    }


    //************* CURRENTLY USED METHOD TO REFORM THE GRIDOBJECTS ARRAY ******************
    //Method to use tileLocations to update gridObjectsArray
    //Use the droppedColumnAmount array to limit the reform to needed parts
    private void reformGridObjectsArray(List<GameObject> changedPiecesList)
    {

        GameObject[,] holderArray = gridObjectsArray;
            

        foreach (GameObject changedPieceTile in changedPiecesList)
        {
            tileInfoComponent changedPieceTileComponent = changedPieceTile.GetComponent<tileInfoComponent>();
            //Update gridObjectsArray with given locations
            //Debug.Log("Changed with : + " + gridObjectsArray[changedPieceTileComponent.tileLocation.x, changedPieceTileComponent.tileLocation.y] + "  <-> " + changedPieceTileComponent.gameObject);

            holderArray[changedPieceTileComponent.tileLocation.x, changedPieceTileComponent.tileLocation.y] = changedPieceTile;

            //Debug.Log(firstForm[4, 2] + " 4-2 " + gridObjectsArray[4, 2]);
            //Debug.Log(firstForm[4, 3] + " 4-3 " + gridObjectsArray[4, 3]);
        }
        

        //Assign gridobjectsarray to holderArray
        gridObjectsArray = holderArray;

      
    }



    //When making new pooled method, add the list above//***noted***
    public List<GameObject> pooledBlueReplacements, pooledGreenReplacements, pooledYellowReplacements, pooledRedReplacements;
    //Generate a list of replacement tiles to randomly drop
    private List<GameObject> replacementTileGenerator(int numOfTilesToGenerate)
    {
        List<GameObject> replacementTilesList = new List<GameObject>();
        //Keep track of how many tiles we added
        int generatedAmountTracker = 0;

        
        
        //Keep track of which list element to add
        int addedBlueAmount = 0;
        int addedGreenAmount = 0;
        int addedRedAmount = 0;
        int addedYellowAmount = 0;

        //Last assigned tile
        string lastGeneratedTile = "";

        while (generatedAmountTracker < numOfTilesToGenerate)
        {
            //Generate random int that corresponds to a random color to drop
            int randomColor = Random.Range(0, 4);

            //Skip if the color is the same as the last generated tile
            if(lastGeneratedTile == "Blue" && randomColor == 0)
            {
                randomColor = Random.Range(1, 4);
             
            }
            if (lastGeneratedTile == "Red" && randomColor == 1)
            {
                randomColor = Random.Range(0, 4);
                if(randomColor == 1)
                {
                    randomColor += 1;
                }

            }
            if (lastGeneratedTile == "Green" && randomColor == 2)
            {
                randomColor = Random.Range(0, 4);
                if (randomColor == 2)
                {
                    randomColor += 1;
                }

            }
            if (lastGeneratedTile == "Yellow" && randomColor == 3)
            {
                randomColor = Random.Range(0, 4);
                if (randomColor == 3)
                {
                    randomColor -= 1;
                }
            }
            //***********************

            //Generate blue
            if (randomColor == 0)
            {

      
                replacementTilesList.Add(pooledBlueReplacements[addedBlueAmount]);
                //Remove from the original list
                pooledBlueReplacements.Remove(pooledBlueReplacements[addedBlueAmount]);
                addedBlueAmount += 1;

                lastGeneratedTile = "Blue";


            }
            //Generate red
            if (randomColor == 1)
            {
                replacementTilesList.Add(pooledRedReplacements[addedRedAmount]);
                //Remove from the original list
                pooledRedReplacements.Remove(pooledRedReplacements[addedRedAmount]);

                addedRedAmount += 1;

                lastGeneratedTile = "Red";
            }
            //Generate green
            if (randomColor == 2)
            {
                replacementTilesList.Add(pooledGreenReplacements[addedGreenAmount]);
                //Remove from the original list
                pooledGreenReplacements.Remove(pooledGreenReplacements[addedGreenAmount]);

                addedGreenAmount += 1;

                lastGeneratedTile = "Green";
            }
            //Generate yellow
            if (randomColor == 3)
            {
                replacementTilesList.Add(pooledYellowReplacements[addedYellowAmount]);
                //Remove from the original list
                pooledYellowReplacements.Remove(pooledYellowReplacements[addedYellowAmount]);

                addedYellowAmount += 1;

                lastGeneratedTile = "Yellow";
            }




            generatedAmountTracker += 1;
        }



        return replacementTilesList;
    }

  

   
    //***noted***

    //Storage for targeted and selected matches list, this will be used in calculateMatchShape to store objects to access later(see which pieces were matched in what shape)
    public List<GameObject> targetedMatchedShapesList = new List<GameObject>();
    public List<GameObject> selectedMatchedShapesList = new List<GameObject>();

    private List<GameObject> calculateMatchShape(string movedSide, GameObject movedTile, int leftMatchesCounter, int rightMatchesCounter, int lowerMatchesCounter, int upMatchesCounter,
        List<GameObject> leftMatchesList, List<GameObject> rightMatchesList, List<GameObject> upMatchesList, List<GameObject> lowerMatchesList)
    {

        //List to store the matched shapes that are picked from the matched shapes list that get the values from checkRight/Left etc... Methods

        List<GameObject> matchedShapesList = new List<GameObject>();


        //Debug.Log("got here with " + movedSide + " " + movedTile);
        //Assign tile info component of the selected object
        tileInfoComponent movedTileInfo = movedTile.GetComponent<tileInfoComponent>();

        //IF THE TILE WAS MOVED TO THE LEFT
        if (movedSide == "Left")
        {

            Debug.Log(leftMatchesCounter + " " + upMatchesCounter);

            //---SQUARE MATCH CONDITONS---
            //Lower square match
            if (leftMatchesCounter == 1 && lowerMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y + 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(leftMatchesList[0]);
                    matchedShapesList.Add(lowerMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y + 1]);
                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it 
                    //clearLists(isSelectedTile);
                    //RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                  

                    Debug.Log("We matched square LEFT LOWER --- 'LEFT' ---");
                    return matchedShapesList;
                }
            }
            //Upper square match
            if (leftMatchesCounter == 1 && upMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y - 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(leftMatchesList[0]);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y - 1]);
                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    ////RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                  


                    Debug.Log("We matched square LEFT UPPER --- 'LEFT' ---");
                    return matchedShapesList;
                }
            }
            //------------------------
            //--- L SHAPED MATCHES ---
            //Opposite L match
            if (leftMatchesCounter == 2 && upMatchesCounter == 2)
            {
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                Debug.Log("We matched L shape --- 'LEFT' ---");
                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //clearLists(isSelectedTile);
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
             
                return matchedShapesList;
            }
            //Regular L shapes match
            if (leftMatchesCounter == 2 && lowerMatchesCounter == 2)
            {
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);
                 
                //Clear the list after we used it
                //clearLists(isSelectedTile);
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               

                Debug.Log("We matched L shape LOWER L --- 'LEFT' ---");
                return matchedShapesList;
            }

            //------------------------
            //PLACED INTO THE MIDDLE MATCHES
            if (lowerMatchesCounter >= 1 && upMatchesCounter >= 1)
            {

                matchedShapesList.Add(movedTile);

                for (int i = 0; i < lowerMatchesCounter; i++)
                {
                    matchedShapesList.Add(lowerMatchesList[i]);
                }
                for (int j = 0; j < upMatchesCounter; j++)
                {
                    matchedShapesList.Add(upMatchesList[j]);
                }

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //clearLists(isSelectedTile);
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
                


                Debug.Log("We matched IN THE RIGHT MIDDLE --- 'LEFT' ---");
                return matchedShapesList;
            }

            //---REGULAR HORIZONTAL MATCHES---
            if (leftMatchesCounter == 2)
            {
                Debug.Log("we matched LEFT HORIZ REGULAR --- 'LEFT' ---");

                matchedShapesList.Add(movedTile);

                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //clearLists(isSelectedTile);
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               


                return matchedShapesList;
            }
            //---REGULAR VERTICAL MATCHES---
            if (upMatchesCounter == 2)
            {
                Debug.Log("we matched LEFT UPPER VERT REGULAR --- 'LEFT' ---");




                matchedShapesList.Add(movedTile);

                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //clearLists(isSelectedTile);
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;
            }
            if (lowerMatchesCounter == 2)
            {
                Debug.Log("we matched LEFT LOWER VERT REGULAR --- 'LEFT' ---");

                matchedShapesList.Add(movedTile);


                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;
            }



        }

        //---------------------------------
        //---------------------------------
        //---------------------------------

        //IF THE TILE WAS MOVED TO THE RIGHT
        if (movedSide == "Right")
        {
            //---SQUARE MATCH CONDITONS---
            //Lower square match
            if (rightMatchesCounter == 1 && lowerMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y + 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched square RIGHT LOWER --- 'RIGHT' ---");

                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(rightMatchesList[0]);
                    matchedShapesList.Add(lowerMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y + 1]);
                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    //clearLists(isSelectedTile);
                    //RESHAPE THE ARRAY
                    // reshapeArrayAndTileInfo(matchedShapesList);
                    


                    return matchedShapesList;
                }
            }
            //Upper square match
            if (rightMatchesCounter == 1 && upMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y - 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched square RIGHT UPPER --- 'RIGHT' ---");


                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(rightMatchesList[0]);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y - 1]);
                    //Fade the pieces
                    // fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    //clearLists(isSelectedTile);
                    //RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                   

                    return matchedShapesList;
                }
            }
            //------------------------
            //L SHAPED MATCHES
            //L Shaped match regular
            if (rightMatchesCounter == 2 && upMatchesCounter == 2)
            {
                Debug.Log("we matched L SHAPE REGULAR --- 'RIGHT' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                // reshapeArrayAndTileInfo(matchedShapesList);
             

                return matchedShapesList;
            }
            //L Shaped opposite match
            if (rightMatchesCounter == 2 && lowerMatchesCounter == 2)
            {
                Debug.Log("we matched L SHAPE OPPOSITE --- 'RIGHT' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);

                //Fade the pieces
                // fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                // reshapeArrayAndTileInfo(matchedShapesList);
                

                return matchedShapesList;
            }
            //------------------------
            //PLACED INTO THE MIDDLE MATCHES
            //TESTING TILE DROPS HERE TOO
            if (lowerMatchesCounter >= 1 && upMatchesCounter >= 1)
            {
                Debug.Log("We matched IN THE LEFT MIDDLE --- 'RIGHT' --- starting from " + movedTile);

                matchedShapesList.Add(movedTile);
            

                for (int i = 0; i < lowerMatchesCounter; i++)
                {
                    matchedShapesList.Add(lowerMatchesList[i]);
                   
                }
                for (int j = 0; j < upMatchesCounter; j++)
                {
                 
                    matchedShapesList.Add(upMatchesList[j]);
                }

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it, RESHAPE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
            

                return matchedShapesList;
            }


            //---REGULAR HORIZONTAL MATCHES---
            if (rightMatchesCounter == 2)
            {
                Debug.Log("we matched RIGHT HORIZ REGULAR --- 'RIGHT' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
           

                return matchedShapesList;
            }
            //---REGULAR VERTICAL MATCHES---
            //TESTING NEW MEFFOD HERE
            if (upMatchesCounter == 2)
            {
                Debug.Log("we matched RIGHT UPPER VERT REGULAR --- 'RIGHT' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY                

                //reshapeArrayAndTileInfo(matchedShapesList);


                //-------
            

                return matchedShapesList;
            }
            if (lowerMatchesCounter == 2)
            {
                Debug.Log("we matched RIGHT LOWER VERT REGULAR --- 'RIGHT' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               

                return matchedShapesList;
            }



        }


        //---------------------------------
        //---------------------------------
        //---------------------------------

        //IF THE TILE WAS MOVED UP
        if (movedSide == "Up")
        {
            //---SQUARE MATCH CONDITONS---
            //Left side square
            if (leftMatchesCounter == 1 && upMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y - 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched UPPER LEFT SQUARE --- 'UP' ---");

                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(leftMatchesList[0]);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y - 1]);

                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    //RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                    


                    return matchedShapesList;
                }
            }
            //Right side square
            if (rightMatchesCounter == 1 && upMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y - 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched UPPER RIGHT SQUARE --- 'UP' ---");

                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(rightMatchesList[0]);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y - 1]);

                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    //RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                    



                    return matchedShapesList;
                }
            }
            //L SHAPED MATCHES
            //Opposite L shaped match
            if (leftMatchesCounter == 2 && upMatchesCounter == 2)
            {
                Debug.Log("We matched L Shape opposite --- 'UP' ---");


                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               


                return matchedShapesList;
            }
            //Regular L shaped match
            if (rightMatchesCounter == 2 && upMatchesCounter == 2)
            {
                Debug.Log("We matched L Shape regular --- 'UP' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                //Fade the pieces
                // fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                // reshapeArrayAndTileInfo(matchedShapesList);
                


                return matchedShapesList;
            }
            //MIDDLE MATCHES
            if (leftMatchesCounter >= 1 && rightMatchesCounter >= 1)
            {
                Debug.Log("We matched IN THE MIDDLE --- 'UP' ---");

                matchedShapesList.Add(movedTile);

                //Iterate through all the possible left right matches and add
                for (int i = 0; i < leftMatchesCounter; i++)
                {
                    matchedShapesList.Add(leftMatchesList[i]);
                }
                for (int j = 0; j < rightMatchesCounter; j++)
                {
                    matchedShapesList.Add(rightMatchesList[j]);
                }


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
             



                return matchedShapesList;
            }

            //---REGULAR HORIZONTAL MATCHES---
            if (leftMatchesCounter == 2)
            {
                Debug.Log("We matched HORIZ LEFT --- 'UP' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
             

                return matchedShapesList;
            }
            if (rightMatchesCounter == 2)
            {
                Debug.Log("We matched HORIZ RIGHT --- 'UP' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;
            }

            //---REGULAR VERTICAL MATCHES---
            if (upMatchesCounter == 2)
            {
                Debug.Log("We matched VERT UPPER --- 'UP' ---");

                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(upMatchesList[0]);
                matchedShapesList.Add(upMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                //RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               

                return matchedShapesList;
            }

        }

         
        //IF THE TILE WAS MOVED DOWN
        if (movedSide == "Down")
        {

            //SPECIAL SQUARE CONDITONS FOR WHEN NATURAL MATCHES OCCUR
            if (upMatchesCounter == 1 && rightMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y - 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched SPECIAL SQUARE UP AND RIGHT ---SPECIAL---");

                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(rightMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y -1]);
                    ////Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    ////RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);


                    return matchedShapesList;
                }
            }

            //****

            if (upMatchesCounter == 1 && leftMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y -1 ].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched SPECIAL SQUARE UP AND RIGHT ---SPECIAL---");

                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(upMatchesList[0]);
                    matchedShapesList.Add(leftMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y - 1]);
                    ////Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    ////RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);


                    return matchedShapesList;
                }
            }

            //*** END OF SPECIAL SQUARE FORMATIONS
            //-------------------------------------------------------

            //Debug.Log("Trying this with MEFFOD TEST " + movedTile);

            //---SQUARE MATCH CONDITONS---
            //Left side square
            if (leftMatchesCounter == 1 && lowerMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y + 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched UPPER LEFT SQUARE --- 'DOWN' ---");

                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(leftMatchesList[0]);
                    matchedShapesList.Add(lowerMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x - 1, movedTileInfo.tileLocation.y + 1]);
                    ////Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    ////RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                   

                    return matchedShapesList;
                }
            }
            //Right side square
            if (rightMatchesCounter == 1 && lowerMatchesCounter == 1)
            {
                if (gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y + 1].GetComponent<tileInfoComponent>().tileColor == movedTileInfo.tileColor)
                {
                    Debug.Log("We matched UPPER RIGHT SQUARE --- 'DOWN' ---");

                    //Add the pieces that formed the square to a list to fade them afterwards
                    matchedShapesList.Add(movedTile);
                    matchedShapesList.Add(rightMatchesList[0]);
                    matchedShapesList.Add(lowerMatchesList[0]);
                    matchedShapesList.Add(gridObjectsArray[movedTileInfo.tileLocation.x + 1, movedTileInfo.tileLocation.y + 1]);
                    //Fade the pieces
                    //fadeMatchedPieces(matchedShapesList);

                    //Clear the list after we used it
                    ////RESHAPE THE ARRAY
                    //reshapeArrayAndTileInfo(matchedShapesList);
                   

                    return matchedShapesList;
                }
            }
            //T SHAPED MATCH CHECK LATER
            /*
            if (rightMatchesCounter == 1 && lowerMatchesCounter == 2 && leftMatchesCounter == 1)
            {

                Debug.Log("T SHAPED MATCH --- ONLY POSSIBLE FROM DOWN ---");

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[0]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;

            }
            */
            //L SHAPED MATCHES
            //Left Opposite L shaped match
            if (leftMatchesCounter == 2 && lowerMatchesCounter == 2)
            {

                Debug.Log("LEFT L SHAPED MATCH REGULAR --- DOWN ---");

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
               

                return matchedShapesList;

            }
            //Right Opposite L shaped match
            if (rightMatchesCounter == 2 && lowerMatchesCounter == 2)
            {

                Debug.Log("LEFT L SHAPED MATCH OPPOSITE --- DOWN ---");

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
            

                return matchedShapesList;

            }
            //--- MIDDLE MATCHES ---
            if (leftMatchesCounter >= 1 && rightMatchesCounter >= 1)
            {
                Debug.Log("We matched MIDDLE --- 'DOWN' ---");

                Debug.Log("We matched with " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                for (int i = 0; i < leftMatchesCounter; i++)
                {
                    matchedShapesList.Add(leftMatchesList[i]);
                }
                for (int j = 0; j < rightMatchesCounter; j++)
                {
                    matchedShapesList.Add(rightMatchesList[j]);
                }


                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              


                return matchedShapesList;
            }
            //Possible matches when new pieces are dropping, uncontrolled by the player
            if (leftMatchesCounter > 2)
            {
                Debug.Log("We matched HORIZ RIGHT --- 'DOWN' --- starting from " + movedTile);


                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                for (int i = 0; i < leftMatchesCounter; i++)
                {
                    matchedShapesList.Add(leftMatchesList[i]);
                }

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);

                return matchedShapesList;
            }

            //---REGULAR HORIZONTAL MATCHES---
            if (leftMatchesCounter == 2)
            {
                Debug.Log("We matched HORIZ LEFT --- 'DOWN' --- starting from " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(leftMatchesList[0]);
                matchedShapesList.Add(leftMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;
            }
            //Possible matches when new pieces are dropping, uncontroller by the player
            if (rightMatchesCounter > 2)
            {
                Debug.Log("We matched HORIZ RIGHT --- 'DOWN' --- with " + movedTile);
                Debug.Log("with " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                for (int i = 0; i < rightMatchesCounter; i++)
                {
                    matchedShapesList.Add(rightMatchesList[i]);
                }

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);

                return matchedShapesList;
            }


            if (rightMatchesCounter == 2)
            {
                Debug.Log("We matched HORIZ RIGHT --- 'DOWN' --- with " + movedTile);
                Debug.Log("with " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(rightMatchesList[0]);
                matchedShapesList.Add(rightMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
              

                return matchedShapesList;
            }

            //---MATCH THAT MIGHT OCCUR WHEN NEW NATURAL PIECES ARE FALLING *SPECIAL*---
            if (lowerMatchesCounter >= 1 && upMatchesCounter >= 1 )
            {
                Debug.Log("We matched VERT **SPECIAL CASEEEE** --- 'DOWN' ---" + " main mathces name was " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                for (int i = 0; i < upMatchesList.Count; i++)
                {
                    matchedShapesList.Add(upMatchesList[i]);
                }

                for (int j = 0; j < lowerMatchesList.Count; j++)
                {
                    matchedShapesList.Add(lowerMatchesList[j]);
                }
       

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);
                 
                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);


                return matchedShapesList;
            }
            //---MATCH THAT MIGHT OCCUR WHEN NEW NATURAL PIECES ARE FALLING *SPECIAL*---
            if (upMatchesCounter >= 2)
            {
                Debug.Log("WE MATCHED UP WHEN MOVING DOWN , SPECIAL CASE FOR NATURAL ***NATURAL***" + movedTile);


                matchedShapesList.Add(movedTile);
                for (int i = 0; i < upMatchesList.Count; i++)
                {
                    matchedShapesList.Add(upMatchesList[i]);
                }

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);


                return matchedShapesList;
            }

            //Possible vertical matches from up top that are more than 2
            if (lowerMatchesCounter > 2)
            {
                Debug.Log("We matched VERT UPPER --- 'DOWN' --- SPECIALISO CKASO with" + movedTile );

               
                matchedShapesList.Add(movedTile);
                for (int i = 0; i < lowerMatchesCounter; i++)
                {
                    matchedShapesList.Add(lowerMatchesList[i]);
                }
             
                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);


                return matchedShapesList;
            }
            //---REGULAR VERTICAL MATCHES---
            if (lowerMatchesCounter == 2)
            {
                Debug.Log("We matched VERT UPPER --- 'DOWN' --- " + " with piece " + movedTile);

                //Add the pieces that formed the square to a list to fade them afterwards
                matchedShapesList.Add(movedTile);
                matchedShapesList.Add(lowerMatchesList[0]);
                matchedShapesList.Add(lowerMatchesList[1]);

                //Fade the pieces
                //fadeMatchedPieces(matchedShapesList);

                //Clear the list after we used it
                ////RESHAPE THE ARRAY
                //reshapeArrayAndTileInfo(matchedShapesList);
          

                return matchedShapesList;
            }

        }

        return null;

    }
































    //METHODS TO CHECK MATCH SIDES FOR SELECTED AND TARGETED TILES
    //----------------------------------------------------------------
    //-----------------CHECK MATCHES FOR SELECTED TILE----------------
    //----------------------------------------------------------------

    //Lists to contain possible matches
    private List<GameObject> selectedRightMatchesList = new List<GameObject>();
    private List<GameObject> selectedLeftMatchesList = new List<GameObject>();
    private List<GameObject> selectedUpMatchesList = new List<GameObject>();
    private List<GameObject> selectedLowerMatchesList = new List<GameObject>();
    //CHECK MATCHES FOR THE RIGHT SIDE
    private int rightMatchesCounter;
    private int rightCheckedX;


    //private List<GameObject> selectedRightMatchesList = new List<GameObject>();

    private void checkRightMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        
        rightCheckedX = checkedTileLoc.x;


        if (rightCheckedX + 1 < gridObjectsArray.GetLength(0))
        {

            //Debug.Log("CHECK THIS " + gridObjectsArray.GetLength(0));
            //Debug.Log("CHECK THIS TOO" + gridObjectsArray[rightCheckedX + 1, checkedTileLoc.y]);
            // If we have the same color
            if (gridObjectsArray[rightCheckedX + 1, checkedTileLoc.y].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {


                //Increment the counter and checked
                rightMatchesCounter += 1;
                rightCheckedX += 1;

                //---TESTER HERE---
                selectedRightMatchesList.Add(gridObjectsArray[rightCheckedX, checkedTileLoc.y]);
                //---TESTER HERE---
         

                checkRightMatches(new Vector2Int(rightCheckedX, checkedTileLoc.y), checkTileObj);

            }
        }

    }

    //CHECK MATCHES FOR THE LEFT SIDE
    private int leftMatchesCounter;
    private int leftCheckedX;




    //private List<GameObject> selectedLeftMatchesList = new List<GameObject>();

    private void checkLeftMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        leftCheckedX = checkedTileLoc.x;


        if (leftCheckedX - 1 >= 0)
        {
            // If we have the same color
            if (gridObjectsArray[leftCheckedX - 1, checkedTileLoc.y].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {

                
                //Increment the counter and checked
                leftMatchesCounter += 1;
                leftCheckedX -= 1;


                //Debug.Log("Gots to me" + gridObjectsArray[leftCheckedX, checkedTileLoc.y]);
                //---TESTER HERE---
                selectedLeftMatchesList.Add(gridObjectsArray[leftCheckedX, checkedTileLoc.y]);
                //---TESTER HERE---


                checkLeftMatches(new Vector2Int(leftCheckedX, checkedTileLoc.y), checkTileObj);

            }

        }







    }

    //CHECK MATCHES FOR THE UPPER SIDE
    private int upMatchesCounter;
    private int upCheckedY;



    // private List<GameObject> selectedUpMatchesList = new List<GameObject>();
    private void checkUpMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        upCheckedY = checkedTileLoc.y;


        if (upCheckedY - 1 >= 0)
        {
            // If we have the same color
            if (gridObjectsArray[checkedTileLoc.x, upCheckedY - 1].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {

                

                //Increment the counter and checked
                upMatchesCounter += 1;
                upCheckedY -= 1;


                //---TESTER HERE---
                selectedUpMatchesList.Add(gridObjectsArray[checkedTileLoc.x, upCheckedY]);
                //---TESTER HERE---

                checkUpMatches(new Vector2Int(checkedTileLoc.x, upCheckedY), checkTileObj);

            }
            else
            {
                Debug.Log("We didnt see the same tile color, we saw " + gridObjectsArray[checkedTileLoc.x, upCheckedY - 1].gameObject.name);
            }



        }


    }

    //CHECK MATCHES FOR THE LOWER SIDE
    private int lowerMatchesCounter;
    private int lowerCheckedY;




    //private List<GameObject> selectedLowerMatchesList = new List<GameObject>();
    private void checkLowerMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        lowerCheckedY = checkedTileLoc.y;


        if (lowerCheckedY + 1 < gridObjectsArray.GetLength(1))
        {
            
            // If we have the same color
            if (gridObjectsArray[checkedTileLoc.x, lowerCheckedY + 1].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {
                
                //Increment the counter and checked
                lowerMatchesCounter += 1;
                lowerCheckedY += 1;

                //---TESTER HERE---
                selectedLowerMatchesList.Add(gridObjectsArray[checkedTileLoc.x, lowerCheckedY]);
                //---TESTER HERE---

                checkLowerMatches(new Vector2Int(checkedTileLoc.x, lowerCheckedY), checkTileObj);

            }



        }


    }

    //----------------------------------------------------------------
    //-----------------CHECK MATCHES FOR TARGETED TILE----------------
    //----------------------------------------------------------------
    private List<GameObject> targetedRightMatchesList = new List<GameObject>();
    private List<GameObject> targetedLeftMatchesList = new List<GameObject>();
    private List<GameObject> targetedUpMatchesList = new List<GameObject>();
    private List<GameObject> targetedLowerMatchesList = new List<GameObject>();


    //CHECK MATCHES FOR THE RIGHT SIDE
    private int rightMatchesTargetedCounter;
    private int rightTargetedCheckedX;

    private void checkTargetRightMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        rightTargetedCheckedX = checkedTileLoc.x;


        if (rightTargetedCheckedX + 1 < gridObjectsArray.GetLength(0))
        {
            // If we have the same color
            if (gridObjectsArray[rightTargetedCheckedX + 1, checkedTileLoc.y].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {
                //Increment the counter and checked
                rightMatchesTargetedCounter += 1;
                rightTargetedCheckedX += 1;

               // Debug.Log("added" + gridObjectsArray[rightTargetedCheckedX, checkedTileLoc.y]);

                //---TESTER HERE---
                targetedRightMatchesList.Add(gridObjectsArray[rightTargetedCheckedX, checkedTileLoc.y]);
                //---TESTER HERE---


                checkTargetRightMatches(new Vector2Int(rightTargetedCheckedX, checkedTileLoc.y), checkTileObj);

            }



        }


    }

    //CHECK MATCHES FOR THE LEFT SIDE
    private int leftMatchesTargetCounter;
    private int leftTargetCheckedX;

    //private List<GameObject> targetedLeftMatchesList = new List<GameObject>();


    private void checkTargetLeftMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        leftTargetCheckedX = checkedTileLoc.x;


        if (leftTargetCheckedX - 1 >= 0)
        {
            // If we have the same color
            if (gridObjectsArray[leftTargetCheckedX - 1, checkedTileLoc.y].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {


                //Increment the counter and checked
                leftMatchesTargetCounter += 1;
                leftTargetCheckedX -= 1;


                //---TESTER HERE---
                targetedLeftMatchesList.Add(gridObjectsArray[leftTargetCheckedX, checkedTileLoc.y]);
                //---TESTER HERE---

                checkTargetLeftMatches(new Vector2Int(leftTargetCheckedX, checkedTileLoc.y), checkTileObj);

            }


        }

    }

    //CHECK MATCHES FOR THE UPPER SIDE
    private int upMatchesTargetCounter;
    private int upTargetCheckedY;


    private List<GameObject> targetedVerticalMatchesList = new List<GameObject>();

    //public List<GameObject> targetedUpMatchesList = new List<GameObject>();

    private void checkTargetUpMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        upTargetCheckedY = checkedTileLoc.y;


        if (upTargetCheckedY - 1 >= 0)
        {
            // If we have the same color
            if (gridObjectsArray[checkedTileLoc.x, upTargetCheckedY - 1].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {


                //Increment the counter and checked
                upMatchesTargetCounter += 1;
                upTargetCheckedY -= 1;


                //---TESTER HERE---

                targetedUpMatchesList.Add(gridObjectsArray[checkedTileLoc.x, upTargetCheckedY]);
                //---TESTER HERE---


                checkTargetUpMatches(new Vector2Int(checkedTileLoc.x, upTargetCheckedY), checkTileObj);

            }


        }


    }

    //CHECK MATCHES FOR THE LOWER SIDE
    private int lowerTargetMatchesCounter;
    private int lowerTargetCheckedY;

    //private List<GameObject> targetedLowerMatchesList = new List<GameObject>();
    private void checkTargetLowerMatches(Vector2Int checkedTileLoc, GameObject checkTileObj)
    {
        lowerTargetCheckedY = checkedTileLoc.y;


        if (lowerTargetCheckedY + 1 < gridObjectsArray.GetLength(1))
        {
            // If we have the same color
            if (gridObjectsArray[checkedTileLoc.x, lowerTargetCheckedY + 1].GetComponent<tileInfoComponent>().tileColor == checkTileObj.GetComponent<tileInfoComponent>().tileColor)
            {


                //Increment the counter and checked
                lowerTargetMatchesCounter += 1;
                lowerTargetCheckedY += 1;


                //---TESTER HERE---
                targetedLowerMatchesList.Add(gridObjectsArray[checkedTileLoc.x, lowerTargetCheckedY]);
                //---TESTER HERE---

                checkTargetLowerMatches(new Vector2Int(checkedTileLoc.x, lowerTargetCheckedY), checkTileObj);

            }


        }


    }



}
