using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileInfoComponent : MonoBehaviour
{

    //The tile color that is used to determine matches...
    public string tileColor;


    //Location information for the 2D Grid, [0,1],[3,4] etc...
    public Vector2Int tileLocation;

    //Spriterenderer of this object
    private SpriteRenderer tileSpriteRen;
    //Color with no alpha
    private Color noAlphaColor;


    //tileSwapperObj
    public GameObject tileSwapperWith2DArrayObj;
    tileSwapHandler tileSwapperWith2DArrayComponent;



    void Start()
    {

        tileSpriteRen = this.GetComponent<SpriteRenderer>();
        noAlphaColor = new Color(tileSpriteRen.color.r, tileSpriteRen.color.g, tileSpriteRen.color.b, 0f);
         
        //Assign initially
        tileSwapperWith2DArrayComponent = tileSwapperWith2DArrayObj.GetComponent<tileSwapHandler>();


    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool startedDeactivationRoutine;

    private float fadeTimer = 1f;
    private float fadeCounter;
    public IEnumerator fadeRoutine()
    {

        startedDeactivationRoutine = true;
        


        //Beginning color for the piece
        Color initialColor = tileSpriteRen.color;

        while (fadeCounter <= fadeTimer)
        {


            //Fade the sprites color
            tileSpriteRen.color = Color.Lerp(initialColor, noAlphaColor, fadeCounter / fadeTimer);

            fadeCounter += Time.deltaTime;

            yield return null;
        }

        //Add the tiles to the pooled objects list in tileSwapperWith2DArray
        if (tileColor == "Blue")
        {
            //Debug.Log("added" + this.gameObject);
            tileSwapperWith2DArrayComponent.pooledBlueReplacements.Add(this.gameObject);
        }
        if (tileColor == "Green")
        {
            //Debug.Log("added" + this.gameObject);
            tileSwapperWith2DArrayComponent.pooledGreenReplacements.Add(this.gameObject);
        }
        if (tileColor == "Yellow")
        {
            // Debug.Log("added" + this.gameObject);
            tileSwapperWith2DArrayComponent.pooledYellowReplacements.Add(this.gameObject);
        }
        if (tileColor == "Red")
        {
            //Debug.Log("added" + this.gameObject);
            tileSwapperWith2DArrayComponent.pooledRedReplacements.Add(this.gameObject);
        }


        fadeCounter = 0f;

        startedDeactivationRoutine = false;

        tileSpriteRen.color = initialColor;

        this.gameObject.SetActive(false);

        //Reset the color to a color that has alpha
        

    }

    //Timer to move
    private float moveTileTimer = 1f;
    public float moveTileCounter;

    //check if started
    public bool startedDropTileRoutine;

    //Starting and ending position vector
    //private Vector2 startPos, endPos;
    public IEnumerator dropTileRoutine(int amountToMove)
    {

        Vector2 startPos = this.transform.position;
        Vector2 endPos = new Vector2(startPos.x, startPos.y - amountToMove);

        //Debug.Log("Gots here with " + this.gameObject);

        //Debug.Log((Mathf.RoundToInt(startPos.x) - mainGridGenerator.firstGridX) + " " + ((Mathf.RoundToInt(startPos.y - amountToMove)) - mainGridGenerator.firstGridY) + " for game object " + this.name);
        //Reassign the gridObjectsArray


        startedDropTileRoutine = true;

        while (moveTileCounter <= moveTileTimer)
        {
            transform.position = Vector2.Lerp(startPos, endPos, moveTileCounter / moveTileTimer);

            moveTileCounter += Time.deltaTime;

            yield return null;

        }
       
        moveTileCounter = 0f;

        startedDropTileRoutine = false;

    }


    //Non coroutine version
    public void dropTile(int amountToMove)
    {
        Vector2 startPos = this.transform.position;
        Vector2 endPos = new Vector2(startPos.x, startPos.y - amountToMove);

        float moveTileCounterTest = 0f;
        float moveTileTimerTest = 0.5f;

        while (moveTileCounterTest <= moveTileTimerTest)
        {
            transform.position = Vector2.Lerp(startPos, endPos, moveTileCounterTest / moveTileTimerTest);

            moveTileCounterTest += Time.deltaTime;


        }

    }


}
