﻿using UnityEngine;
using System.Collections;
using Pathfinding;
using UnityEngine.UI;

/* this script will contain all methods and variables 
 * used when moving pieces and attacking during a turn, 
 * it will call A* to start a piece moving and also test 
 * the distance to the target tile between the current 
 * tile to check if it is a possible legal move. 
 */

public class OnTurnActions : MonoBehaviour
{

    public Vector3 targetPosition;
    public CharacterController pieceCharacterController;
    public int MaxPathNodes;
    public float MaxMove;
    public GameObject SoundController;
    public GameObject MoveToTile;
    public GameObject OnHoverTile;
    public GameObject OnHoverPiece;
    public GameObject CurrentTile;
    public GameObject SelectedPiece;
    public GameObject PieceStatPanel;
    public GameObject NameText; 
    public GameObject PowerSlider;
    public GameObject PowerText;
    public GameObject SpeedSlider;
    public GameObject SpeedText;
    public GameObject FactionText;
    public GameObject SpecialText;
    public GameObject BoostSlider;
    public GameObject BoostText;
    public GameObject LvlSlider;
    public GameObject LvlText; 
    public GameObject PieceSpawnController;
    public GameObject TutorialController; 
    public bool hasSelectedPiece = false;
    public bool CanClick = true;
    public static bool EndingTurn = false; 
    //Pathfinder variables
    public Seeker seeker;
    public Path path;
    public float nextWaypointDistance = 2f;
    public int currentWaypoint = 0;
    public bool isGenPath = false;
    public bool drawnPath = false;
    public static GameObject[] allTiles;
    public GameObject DummyPiece; 

    public bool isFighting = false;



    // Use this for initialization
    void Start()
    {
        allTiles = GameObject.FindGameObjectsWithTag("Tile");
        MixUpTiles();
        
        if(PieceStatPanel != null)
        {
            PieceStatPanel.SetActive(false);
        }
        if(GameObject.Find("UISoundController") != null)
        {
            SoundController = GameObject.Find("UISoundController");
        }
        //PieceStatPan Vars Setup----------------------------------------
        /*if(GameObject.Find("PieceStatPan2.0") != null)
        {
            PieceStatPanel = GameObject.Find("PieceStatPan2.0");
            PieceStatPanel.SetActive(false);
        }
        if(GameObject.Find("HealthSlider") != null)
        {
            HealthPanel = GameObject.Find("HealthSlider");
        }
        if(GameObject.Find("DamageSlider") != null)
        {
            DamagePanel = GameObject.Find("DamageSlider");
        }
        if(GameObject.Find("SpeedSlider") != null)
        {
            SpeedPanel = GameObject.Find("SpeedSlider");
        }
        //TileStatPan Vars Setup-------------------------------------------
        if(GameObject.Find("TileStatPan") != null)
        {
            TileStatPanel = GameObject.Find("TileStatPan");
        }
        if (GameObject.Find("FactionPan") != null)
        {
            TileFactionPan = GameObject.Find("FactionPan");
        }
        if (GameObject.Find("TileLevelSlider") != null)
        {
            TileLvlPan = GameObject.Find("TileLevelSlider");
        }
        if (GameObject.Find("TileBoostSlider") != null)
        {
            TileBoostPan = GameObject.Find("TileBoostSlider");
        } 
        if (GameObject.Find("TileBoostText") != null)
        {
            TileBoostText = GameObject.Find("TileBoostText");
        }*/
        
        if (BattleStats.currentGameType == BattleStats.GameType.Domination)
        {
            GameObject.Find("Canvas").GetComponent<DominationController>().UpdatePercentUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(CanClick)
            {
                if (OnHoverTile != null)
                {
                    if (hasSelectedPiece == false)//before player has selected a piece
                    {
                        //check if piece is current players

                        if (OnHoverTile != null && OnHoverTile.GetComponent<OnTileActions>().PieceOnTile != null)
                        {
                            if (TurnStateMachine.state == TurnStateMachine.State.playerTurn && OnHoverTile.GetComponent<OnTileActions>().PieceOnTile.tag == "White")
                            {
                                if(TutorialController != null)
                                {
                                    if (OnHoverPiece.GetComponent<PiecePropScript>().PieceName == "Synthetic Grunt" && TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout == 5)
                                    {
                                        SelectPiece(OnHoverPiece);
                                        TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout=6;
                                        CanClick = false; 
                                    }
                                    else if (OnHoverPiece.GetComponent<PiecePropScript>().PieceName == "Synthetic Tank" && TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout == 8)
                                    {
                                        SelectPiece(OnHoverPiece);
                                        TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout=9;
                                        CanClick = false; 
                                    }
                                    else
                                    {
                                        SoundController.GetComponent<UISoundsScript>().playError();
                                    }
                                }
                                else
                                {
                                    SelectPiece(OnHoverPiece);
                                }
                                
                            }
                            else if (TurnStateMachine.state == TurnStateMachine.State.otherTurn && OnHoverTile.GetComponent<OnTileActions>().PieceOnTile.tag == "Black")
                            {
                                SelectPiece(OnHoverPiece);
                            }
                            else
                            {
                                SoundController.GetComponent<UISoundsScript>().playError();
                            }

                        }
                    }
                    else
                    {
                        if (CurrentTile == OnHoverTile)
                        {

                            //SelectedPiece = null;
                            //CurrentTile = null; 
                            ResetController();
                            if(SoundController != null)
                            {
                                SoundController.GetComponent<UISoundsScript>().playDeselect();

                            }
                        }
                        else
                        {
                            if (SelectedPiece.GetComponent<pieceMovementScript>().path.vectorPath.Count <= SelectedPiece.GetComponent<pieceMovementScript>().MaxPathNodes)
                            {
                                if (OnHoverTile.GetComponent<OnTileActions>().PieceOnTile == null)
                                {
                                    if(OnHoverTile.renderer.material.color == Color.green)
                                    {
                                        if(TutorialController == null)
                                        {
                                            SetTarget(OnHoverTile);
                                        }
                                        else
                                        {
                                            SoundController.GetComponent<UISoundsScript>().playError();
                                        }
                                    }
                                    else
                                    {
                                        SoundController.GetComponent<UISoundsScript>().playError();
                                    }
                                }
                                else//brackets check when you are initiating combat
                                {
                                    if (OnHoverTile.GetComponent<OnTileActions>().PieceOnTile.tag != SelectedPiece.tag)//this check initiates combat
                                    {
                                        if(TutorialController != null)
                                        {
                                            if (OnHoverPiece.GetComponent<PiecePropScript>().PieceName == "Organic Runner" && TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout == 11)
                                            {
                                                TutorialController.GetComponent<TutorialControllerScript>().TutorialStepCout=12;
                                                SoundController.GetComponent<UISoundsScript>().playFight();
                                                isFighting = true;
                                                //OnHoverTile.GetComponent<OnTileActions>().AtkPiece = SelectedPiece;
                                                StartCombat(OnHoverTile, CurrentTile);
                                                SetTarget(OnHoverTile);
                                            }
                                            else
                                            {
                                                SoundController.GetComponent<UISoundsScript>().playError();
                                            }
                                        }
                                        else
                                        {
                                            SoundController.GetComponent<UISoundsScript>().playFight();
                                            isFighting = true;
                                            //OnHoverTile.GetComponent<OnTileActions>().AtkPiece = SelectedPiece;
                                            StartCombat(OnHoverTile, CurrentTile);
                                            SetTarget(OnHoverTile);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                SoundController.GetComponent<UISoundsScript>().playError();
                                //ResetController();
                            }
                        }
                    }
                }
            }
            else//will play an error sound when you try to click on a tile while a piece is moving
            {
                SoundController.GetComponent<UISoundsScript>().playError(); 
            }
        }
        if(OnHoverTile != null)
        {
            UpdateStatPan();
            if (PieceStatPanel != null)
            {
                PieceStatPanel.SetActive(true);
            }
        }
        else
        {
            if (PieceStatPanel != null)
            {
                PieceStatPanel.SetActive(false);
            }
        }
    }

    /* 
     * called when you assign a selected piece to move
     * it assigns the selectedPiece, MaxMove, CurrentTile, 
     * MaxPathNodes
     */
    public void SelectPiece(GameObject Piece)
    {
        //print("SelectPiece Called");
        hasSelectedPiece = true;
        //print("bool set to true");
        OnHoverTile.GetComponent<OnTileActions>().TileNode.SetActive(true);
        SelectedPiece = Piece;
        CurrentTile = OnHoverTile;
        MaxPathNodes = Piece.GetComponent<pieceMovementScript>().MaxPathNodes; 
        //MaxMove = SelectedPiece.GetComponent<PiecePropScript>()
        //MaxPathNodes = SelectedPiece.GetComponent<PiecePropScript>()
        OnHoverTile.GetComponent<OnTileActions>().isSelected = true;
        GridManager.rescan = true; 
        OnHoverTile.renderer.material.color = Color.yellow;
        if (SoundController != false)
        {
            SoundController.GetComponent<UISoundsScript>().playSelectPiece();
        }


    }

    /*
     * called when you move a piece to an opponent tile with a piece
     * occupying it, this will start combat and show the player which
     * piece won that battle.
     */
    public void StartCombat(GameObject target, GameObject current)
    {
        hasSelectedPiece = false;
        GameObject canvas = GameObject.Find("Canvas");
        canvas.GetComponent<BattleController>().SetAttackerTile(current);
        canvas.GetComponent<BattleController>().SetDefenderTile(target);
        canvas.GetComponent<BattleController>().enabled = true;
    }

    /*
     * when called the TurnController is reset to factory 
     * fresh settings this will be called after you move 
     * your piece or when you deselect your piece
     */
    public void ResetController()
    {
        //print("ResetController Called");
        if (isFighting == false)
        {
            print("ResetController Called");
            CurrentTile.renderer.material.color = Color.white;
            CurrentTile.GetComponent<OnTileActions>().isSelected = false;
            //MoveToTile.renderer.material.color = Color.white;
            //MoveToTile.GetComponent<OnTileActions>().isSelected = false;
            SelectedPiece = null;
            CurrentTile = null;
            MoveToTile = null;
            MaxMove = 0;
            MaxPathNodes = 0;
            hasSelectedPiece = false;
            resetAllTiles();
            if(TutorialController == null)
            {
                Camera.main.GetComponent<CameraZoomController>().ResetTransform();
            }
        }
        
        if (BattleStats.currentGameType == BattleStats.GameType.Domination)
        {
            GameObject.Find("Canvas").GetComponent<DominationController>().UpdatePercentUI();
        }
    }

    /*
     * EndOfTurn is obviously called at the end of a turn, it sets the 
     * variables for the tiles like if there is a piece on it and which 
     * piece is on it. It also resets the oldTile to a plain old tile.
     * This also calls NextTurn() to change who's turn it is anyway!
     */ 
    public void EndOfTurn()//this will set the selectedPiece to the target tile only when you are not initiating combat
    {
        if(TutorialController == null)
        {
            if (isFighting == false)
            {
                CurrentTile.GetComponent<OnTileActions>().PieceOnTile = null;
                MoveToTile.GetComponent<OnTileActions>().PieceOnTile = SelectedPiece;
                MoveToTile.GetComponent<OnTileActions>().isSelected = false;
                CanClick = true;
                NextTurn();
            }
        }
    }

    public void EndOfBattle()
    {
        //CurrentTile.GetComponent<OnTileActions>().PieceOnTile = null;
        //MoveToTile.GetComponent<OnTileActions>().PieceOnTile = SelectedPiece;
        MoveToTile.GetComponent<OnTileActions>().isSelected = false;
        CanClick = true;
        NextTurn(); 
    }

    /*
     * called to progress to the next players turn and that's pretty much it!
     */ 
    public void NextTurn()
    {
        if (isFighting == true)
            isFighting = false;
        if(TurnStateMachine.state == TurnStateMachine.State.playerTurn)
        {
            TurnStateMachine.state = TurnStateMachine.State.otherTurn; 
        }
        else
        {
            TurnStateMachine.state = TurnStateMachine.State.playerTurn; 
        }
        for(int i = 0; i<allTiles.Length; i++)
        {
            allTiles[i].GetComponent<OnTileActions>().updateTileLevel();
        }
    }

    public void SetTarget(GameObject targetTile)//called when you select a tile to move to
    {
        //SelectedPiece.GetComponent<pieceMovementScript>().isMoving = true;
        //SelectedPiece.GetComponent<pieceMovementScript>().startMove = true;
        CanClick = false; 
        targetTile.GetComponent<OnTileActions>().isSelected = true;
        MoveToTile = targetTile;
        if(TutorialController == null && OnHoverTile != null)
        {
            OnHoverTile.renderer.material.color = Color.green;
        }
        hasSelectedPiece = false;
        SelectedPiece.transform.position = new Vector3(SelectedPiece.transform.position.x, 0.5f, SelectedPiece.transform.position.z);//this is a quick fix for a weird bug where pice was being clocked from movng
        targetTile.GetComponent<OnTileActions>().TileNode.SetActive(false);
        GridManager.rescan = true; 
        if (SoundController != false)
        {
            SoundController.GetComponent<UISoundsScript>().playMovePiece();
        }
        if (TutorialController == null)
        {
            Camera.main.GetComponent<CameraZoomController>().SetTarget(SelectedPiece);
        }
        
        
    }

    public void GenPath()//called to generated a path from the selectedPiece in the TurnController and the OnHoverTile(the tile you are hovering over)
    {
        //print("Generated Path");
        SelectedPiece.GetComponent<pieceMovementScript>().targetPosition = OnHoverTile.transform.position;
        //SelectedPiece.GetComponent<pieceMovementScript>().GenNewPath();
        SelectedPiece.GetComponent<pieceMovementScript>().GetNewPath();
        isGenPath = true;
    }

    public void resetAllTiles()//primarily used to reset the highlighted tiles in a path, it sets all tiles on the board to a base color of white
    {
        for (int i = 0; i < allTiles.Length; i++)
        {
            allTiles[i].renderer.material.color = Color.white;
        }
    }

    public void MixUpTiles()
    {
        for (int i = 0; i < allTiles.Length; i++)
        {
            int randNum = Random.Range(0, 2);
            if (randNum < 1)
            {
                allTiles[i].GetComponent<OnTileActions>().TileState = OnTileActions.TileType.Alien;
            }
            else
            {
                allTiles[i].GetComponent<OnTileActions>().TileState = OnTileActions.TileType.Synth;
            }
        }
        if (PieceSpawnController != null)
        {
            
            PieceSpawnController.GetComponent<SpawnBasicUnits>().StartSpawn();
            GridManager.rescan = true;

        }
    }
    public void UpdateStatPan()//called to update the stats on the stat panel prior to showing panel
    {
        if(OnHoverTile != null)
        {
            if (OnHoverTile.GetComponent<OnTileActions>().PieceOnTile != null)//displaying PieceStatPanel
            {
                if (PowerSlider != null)
                {
                    PowerSlider.GetComponent<Slider>().value = OnHoverPiece.GetComponent<PiecePropScript>().PowerLevel;
                    if (PowerSlider != null)
                    {
                        PowerText.GetComponent<Text>().text = OnHoverPiece.GetComponent<PiecePropScript>().PowerLevel.ToString();
                    }
                }
                
                if(SpeedSlider != null)
                {
                    SpeedSlider.GetComponent<Slider>().value = OnHoverPiece.GetComponent<PiecePropScript>().Movement;
                    if(SpeedSlider != null)
                    {
                        SpeedText.GetComponent<Text>().text = OnHoverPiece.GetComponent<PiecePropScript>().Movement.ToString(); 
                    }
                }
                if(NameText != null)
                {
                    NameText.GetComponent<Text>().text = OnHoverPiece.GetComponent<PiecePropScript>().PieceName + " Stats";
                }
            }
            else
            {
                //set unused Piece Stats to 0/null/N/A
                if (PowerSlider != null)
                {
                    PowerSlider.GetComponent<Slider>().value = 0;
                    if (PowerSlider != null)
                    {
                        PowerText.GetComponent<Text>().text = "N/A";
                    }
                }

                if (SpeedSlider != null)
                {
                    SpeedSlider.GetComponent<Slider>().value = 0;
                    if (SpeedSlider != null)
                    {
                        SpeedText.GetComponent<Text>().text = "N/A";
                    }
                }
                if (NameText != null)
                {
                    NameText.GetComponent<Text>().text = "Tile Stats";
                }
            }
            //Just Tile Properties:
            if (FactionText != null)
            {
                FactionText.GetComponent<Text>().text = OnHoverTile.GetComponent<OnTileActions>().TileFaction;
            }
            if (BoostSlider != null)
            {
                BoostSlider.GetComponent<Slider>().value = OnHoverTile.GetComponent<OnTileActions>().TilePowerLevel * 25;
                if (BoostSlider != null)
                {
                    BoostText.GetComponent<Text>().text = (OnHoverTile.GetComponent<OnTileActions>().TilePowerLevel * 25).ToString();
                }
            }
            if (LvlSlider != null)
            {
                LvlSlider.GetComponent<Slider>().value = OnHoverTile.GetComponent<OnTileActions>().TilePowerLevel;
                if (LvlSlider != null)
                {
                    LvlText.GetComponent<Text>().text = OnHoverTile.GetComponent<OnTileActions>().TilePowerLevel.ToString();
                }
            }

        }

    }
}
