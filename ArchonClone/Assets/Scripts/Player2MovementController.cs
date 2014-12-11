﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player2MovementController : MonoBehaviour
{
    public GameObject SynthTank;
    public GameObject SynthScout;
    public GameObject SynthRunner;
    public GameObject SynthGrunt;

    public GameObject OrganicTank;
    public GameObject OrganicScout;
    public GameObject OrganicRunner;
    public GameObject OrganicGrunt;

    public static int xSensitivity = 3;
    public static int ySensitivity = 3;

    public float speed = 6.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lastLooking;
    public bool topDownView = false;

    public Rigidbody robotBullet;
    public Rigidbody alienBullet;
    public Rigidbody robotMissile;
    public Rigidbody alienMissile;
    public GameObject Sword;
    public bool isMelee = false;
    public int bulletSpeed = 25;

    bool reloading = false;
    public bool swinging = false;
    float attackRate = 1.5f;
    float attackTimer = 0;
    int bulletSize;

    public GameObject healthPieceGreen;
    public Sprite healthPieceRed;
    public Sprite HealthPieceGreenSprite;
    public GameObject special;

    public Camera camMine;
    public Camera camEnemy;

    public float specialTimer = 5.0f;
    bool specialAvailable = false;
    public bool usingShield = false;
    float shieldPower = 100f;
    bool shieldOverheat = false;
    bool boost = false;

    Player1MovementController enemy;

    public float health;
    public float MaxHealth;

    public bool win = false;
    CharacterController controller;

    public GameObject MoveController;

    public bool isAlien;

    public GameObject myCanvas;
    Quaternion canvasRotation;

    float endTimer;

    string enemyName;
    int enemyStartHealth;
    bool printStats = false;

    ParticleSystem ps;
    public ParticleSystem powerUpParticles;

    int damagePowerUp = 0;
    int speedPowerUp = 0;
    float initialSpeed;
    float initialDamage;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        powerUpParticles.enableEmission = true;
        canvasRotation = new Quaternion(-1, 0, 0, 1);
        myCanvas.transform.rotation = canvasRotation;
        printStats = false;
        enemy = GameObject.Find("Player1(Clone)").GetComponent<Player1MovementController>();
        MoveController = GameObject.Find("MovementController");
        controller = GetComponent<CharacterController>();

        //health = 100;
        speed = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement;
        initialSpeed = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement;
        initialDamage = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Damage;
        health = (float)MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Health;
        MaxHealth = (float)MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().MaxHealth;
        attackRate = 2;//MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().AttackRate;
        attackTimer = attackRate;
        bulletSize = 1;
        lastLooking = transform.forward;

        if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteTank(Clone)")
        {
            isMelee = true;
            special.SetActive(true);
            SynthTank.SetActive(true);
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteScout(Clone)")
        {
            isMelee = false;
            special.SetActive(true);
            SynthScout.SetActive(true);
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteRunner(Clone)")
        {
            isMelee = false;
            special.SetActive(true);
            SynthRunner.SetActive(true);
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteGrunt(Clone)")
        {
            ps.startColor = new Color(3f / 255f, 212f / 255f, 177f / 255f, 45f / 255f);
            isMelee = true;
            special.SetActive(true);
            SynthGrunt.SetActive(true);
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackTank(Clone)")
        {
            isMelee = true;
            special.SetActive(true);
            OrganicTank.SetActive(true);
            isAlien = true;
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackScout(Clone)")
        {
            isMelee = false;
            special.SetActive(true);
            OrganicScout.SetActive(true);
            isAlien = true;
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackRunner(Clone)")
        {
            isMelee = false;
            special.SetActive(true);
            OrganicRunner.SetActive(true);
            isAlien = true;
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackGrunt(Clone)")
        {
            ps.startColor = new Color(20f / 255f, 158f / 255f, 15f / 255f, 45f / 255f);
            isMelee = true;
            special.SetActive(true);
            OrganicGrunt.SetActive(true);
            isAlien = true;
        }
        healthPieceGreen.GetComponent<Image>().fillAmount = (float)((health * 2) / (MaxHealth * 3));
        enemyName = MoveController.GetComponent<PawnMove>().Player01.name;
        enemyStartHealth = MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Health;
    }
    void Update()
    {
        if (MoveController.GetComponent<PawnMove>().Player02 != null && MoveController.GetComponent<PawnMove>().Player01 != null)
        {
            if (GameObject.Find("Fight") == null)
            {
                if (topDownView)
                {
                    if (controller.isGrounded)
                    {
                        if (Input.GetJoystickNames().Length == 2)
                        {
                            if (Input.GetAxis("360_HorizontalRightStick2") == 0 && Input.GetAxis("360_VerticalRightStick2") == 0)
                            {
                                transform.forward = lastLooking;
                            }
                            else
                            {
                                transform.forward = new Vector3(Input.GetAxis("360_HorizontalRightStick2"), 0, Input.GetAxis("360_VerticalRightStick2"));
                            }
                            moveDirection = new Vector3(Input.GetAxis("360_HorizontalLeftStick2"), 0, Input.GetAxis("360_VerticalLeftStick2"));
                            moveDirection *= speed;
                        }
                        else
                        {
                            if (Input.GetAxis("Horizontal2") == 0 && Input.GetAxis("Vertical2") == 0)
                            {
                                transform.forward = lastLooking;
                            }
                            else
                            {
                                transform.forward = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));
                            }
                            moveDirection = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2")) + transform.right * Input.GetAxis("Strafe2");
                            moveDirection *= speed;
                        }

                    }
                    lastLooking = transform.forward;
                    moveDirection.y -= gravity * Time.deltaTime;
                    controller.Move(moveDirection * Time.deltaTime);
                }
                else
                {
                    if (controller.isGrounded)
                    {
                        if (Input.GetJoystickNames().Length == 2)
                        {
                            transform.Rotate(Vector3.up, xSensitivity * Input.GetAxis("360_HorizontalRightStick2"));

                            moveDirection = new Vector3(Input.GetAxis("360_HorizontalLeftStick2"), 0, Input.GetAxis("360_VerticalLeftStick2"));
                            moveDirection = transform.TransformDirection(moveDirection);
                            moveDirection *= speed;
                        }
                        else
                        {
                            transform.Rotate(transform.up, xSensitivity * Input.GetAxis("Horizontal2"));
                            if (Input.GetAxis("Vertical2") == 0)
                            {
                                moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical2"));
                            }
                            else
                            {
                                moveDirection = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2"));
                            }
                            moveDirection = transform.TransformDirection(moveDirection) + (transform.right * Input.GetAxis("Strafe2"));
                            moveDirection *= speed;

                            /*transform.Rotate(Vector3.up, xSensitivity * Input.GetAxis("Horizontal2"));

                            moveDirection = new Vector3(Input.GetAxis("Horizontal2"), 0, Input.GetAxis("Vertical2")) + transform.right * Input.GetAxis("Strafe2");
                            moveDirection = transform.TransformDirection(moveDirection);
                            moveDirection *= speed;*/
                        }
                    }
                    moveDirection.y -= gravity * Time.deltaTime;
                    controller.Move(moveDirection * Time.deltaTime);
                }
                if (isMelee == false)
                {
                    if (reloading)
                    {
                        attackTimer -= Time.deltaTime;
                        if (attackTimer < 0)
                        {
                            attackTimer = attackRate;
                            reloading = false;
                        }
                    }

                    if (win == false)
                    {
                        if (Input.GetJoystickNames().Length == 2)
                        {
                            if ((Input.GetAxis("360_RightTrigger2") == 1) && reloading == false)
                            {
                                if (isAlien)
                                {
                                    Rigidbody bulletClone = Instantiate(alienBullet, transform.position + 1.2f * bulletSize * this.transform.forward, transform.rotation) as Rigidbody;
                                    bulletClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                                    bulletClone.rigidbody.useGravity = false;
                                    bulletClone.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
                                    Destroy(bulletClone.gameObject, 1.5f);
                                }
                                else
                                {
                                    Rigidbody bulletClone = Instantiate(robotBullet, transform.position + 1.2f * bulletSize * this.transform.forward, transform.rotation) as Rigidbody;
                                    bulletClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                                    bulletClone.rigidbody.useGravity = false;
                                    bulletClone.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
                                    Destroy(bulletClone.gameObject, 1.5f);
                                }
                                audio.Play();
                                reloading = true;
                            }
                        }
                        else
                        {
                            if ((Input.GetAxis("Fire2") == 1) && reloading == false)
                            {
                                if (isAlien)
                                {
                                    Rigidbody bulletClone = Instantiate(alienBullet, transform.position + 1.2f * bulletSize * this.transform.forward, transform.rotation) as Rigidbody;
                                    bulletClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                                    bulletClone.rigidbody.useGravity = false;
                                    bulletClone.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
                                    Destroy(bulletClone.gameObject, 1.5f);
                                }
                                else
                                {
                                    Rigidbody bulletClone = Instantiate(robotBullet, transform.position + 1.2f * bulletSize * this.transform.forward, transform.rotation) as Rigidbody;
                                    bulletClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                                    bulletClone.rigidbody.useGravity = false;
                                    bulletClone.velocity = transform.TransformDirection(Vector3.forward * bulletSpeed);
                                    Destroy(bulletClone.gameObject, 1.5f);
                                }
                                audio.Play();
                                reloading = true;
                            }
                        }
                    }
                }
                else
                {
                    if (swinging)
                    {
                        attackTimer -= Time.deltaTime;
                        if (attackTimer < 0)
                        {
                            attackTimer = attackRate;
                            swinging = false;
                        }
                    }

                    if (win == false)
                    {
                        if (Input.GetJoystickNames().Length == 2)
                        {
                            if ((Input.GetAxis("360_RightTrigger1") == 1) && swinging == false)
                            {
                                GameObject sword = Instantiate(Sword, transform.position + 3*this.transform.forward, transform.rotation) as GameObject;
                                sword.tag = tag;
                                swinging = true;
                                Destroy(sword.gameObject, 0.2f);
                            }
                        }
                        else
                        {
                            if ((Input.GetAxis("Fire2") == 1) && swinging == false)
                            {
                                GameObject sword = Instantiate(Sword, transform.position + 3*this.transform.forward, transform.rotation) as GameObject;
                                sword.tag = tag;
                                swinging = true;
                                Destroy(sword.gameObject, 0.2f);
                            }
                        }
                    }
                }
                myCanvas.transform.rotation = canvasRotation;
                if (enemy.GetComponent<Player1MovementController>().win == false)
                {
                    if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteTank(Clone)" && win == false && enemy.win == false)
                    {
                        RobotTankSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteScout(Clone)" && win == false && enemy.win == false)
                    {
                        //RobotScoutSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteRunner(Clone)" && win == false && enemy.win == false)
                    {
                        RobotRunnerSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteGrunt(Clone)" && win == false && enemy.win == false)
                    {
                        RobotGruntSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackTank(Clone)" && win == false && enemy.win == false)
                    {
                        AlienTankSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackScout(Clone)" && win == false && enemy.win == false)
                    {
                        //AlienScoutSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackRunner(Clone)" && win == false && enemy.win == false)
                    {
                        AlienRunnerSpecial();
                    }
                    else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackGrunt(Clone)" && win == false && enemy.win == false)
                    {
                        AlienGruntSpecial();
                    }
                }

                healthPieceGreen.GetComponent<Image>().fillAmount = (float)((health * 2) / (MaxHealth * 3));

                if ((float)((health * 2) / (MaxHealth * 3)) <= 0.16f)
                {
                    healthPieceGreen.GetComponent<Image>().sprite = healthPieceRed;
                }
                if (health <= 0 && win == false)
                {
                    MoveController.GetComponent<PawnMove>().Player02.GetComponent<pieceMove>().datSprite.SetActive(false);
                    if (MoveController.GetComponent<PawnMove>().Player02.tag == "White")
                    {
                        SpawnBasicUnits.WhitePieceCount--;
                    }
                    else
                    {
                        SpawnBasicUnits.BlackPieceCount--;
                    }
                    Destroy(MoveController.GetComponent<PawnMove>().Player02);
                    enemy.win = true;
                    //MoveController.GetComponent<PawnMove>().MoveToTile.GetComponent<TileProperties>().UnitOnTile = MoveController.GetComponent<PawnMove>().Player01;
                    //Destroy(this.gameObject);
                }
            }
        }
        if (win == true)
        {
            if (!printStats)
            {
                string path = Application.streamingAssetsPath;

                path += "/WhoWins.txt";
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("Battle between .... " + MoveController.GetComponent<PawnMove>().Player01.name + " with " + MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Health + " health  vs   " + enemyName + " with " + enemyStartHealth + " health");
                    sw.WriteLine("Winner is " + MoveController.GetComponent<PawnMove>().Player01.name);
                    sw.WriteLine("-------------------");
                    UpdateStats();
                    printStats = true;
                    BattleStats.GameOver = true;
                    BattleStats.UpdateStats();
                }
            }
            //win = false; 
            //TurnStateMachine.fightDone = true; 

            if (endTimer <= 3)
            {
                endTimer += Time.deltaTime;
            }
            else
            {
                if (health <= 0)
                {
                    if (MoveController.GetComponent<PawnMove>().Player02.tag == "White")
                    {
                        SpawnBasicUnits.WhitePieceCount--;
                    }
                    else
                    {
                        SpawnBasicUnits.BlackPieceCount--;
                    }
                    Destroy(MoveController.GetComponent<PawnMove>().Player02);
                    MoveController.GetComponent<PawnMove>().Player02.GetComponent<pieceMove>().datSprite.SetActive(false);
                    MoveController.GetComponent<PawnMove>().MoveToTile.GetComponent<TileProperties>().UnitOnTile = null;
                    MoveController.GetComponent<PawnMove>().MoveToTile.GetComponent<TileProperties>().datNode.SetActive(true);
                    GameObject.Find("A*").GetComponent<AstarPath>().Scan();
                }
                else
                {
                    MoveController.GetComponent<PawnMove>().MoveToTile.GetComponent<TileProperties>().UnitOnTile = MoveController.GetComponent<PawnMove>().Player02;
                    if (health <= 1)
                    {
                        health = 1;
                    }
                    MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Health = (int)health;
                }
                TurnStateMachine.fightDone = true;
                BattleStats.winner = tag;
                Destroy(GameObject.Find("BattleSceneAdditive"));
                Destroy(MoveController.GetComponent<PawnMove>().Player01);
                //Application.LoadLevel("TestingHexTiles");
                //Destroy(this.gameObject);
            }
        }
    }

    IEnumerator DamageBoost()
    {
        yield return new WaitForSeconds(5f);
        if (MoveController.GetComponent<PawnMove>().Player02 != null)
        {
            if (damagePowerUp == 1)
            {
                MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Damage = initialDamage;
                powerUpParticles.Stop();
                powerUpParticles.Clear();
                damagePowerUp--;
            }
            else
            {
                StartCoroutine("DamageBoost");
                damagePowerUp--;
            }
        }
    }

    IEnumerator SpeedBoost()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(2.5f, 5f));
        if (MoveController.GetComponent<PawnMove>().Player02 != null)
        {
            if (speedPowerUp == 1)
            {
                MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement = initialSpeed;
                speed = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement;
                powerUpParticles.Stop();
                powerUpParticles.Clear();
                speedPowerUp--;
            }
            else
            {
                StartCoroutine("SpeedBoost");
                speedPowerUp--;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (MoveController.GetComponent<PawnMove>().Player02 != null && MoveController.GetComponent<PawnMove>().Player01 != null)
        {
            if (other.name == "PowerUp")
            {
                float statBoost = UnityEngine.Random.Range(0, 100);
                if (other.GetComponent<PowerUpController>().power == PowerUpController.PowerupType.Damage)
                {
                    if (damagePowerUp == 0)
                    {
                        StartCoroutine("DamageBoost");
                        damagePowerUp++;
                        MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Damage = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Damage * 1.5f;
                        powerUpParticles.startColor = Color.red;
                        powerUpParticles.Play();
                    }
                    else
                    {
                        damagePowerUp++;
                    }
                }
                else if (other.GetComponent<PowerUpController>().power == PowerUpController.PowerupType.Speed)
                {
                    if (speedPowerUp == 0)
                    {
                        StartCoroutine("SpeedBoost");
                        speedPowerUp++;
                        MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement * 1.5f;
                        speed = MoveController.GetComponent<PawnMove>().Player02.GetComponent<PiecePropScript>().Movement;
                        powerUpParticles.startColor = Color.blue;
                        powerUpParticles.Play();
                    }
                    else
                    {
                        speedPowerUp++;
                    }
                }
                else if (other.GetComponent<PowerUpController>().power == PowerUpController.PowerupType.Health)
                {
                    health += UnityEngine.Random.Range(5, 15);
                    if (health > MaxHealth)
                    {
                        health = MaxHealth;
                    }
                    if ((float)((health * 2) / (MaxHealth * 3)) >= 0.16f)
                    {
                        healthPieceGreen.GetComponent<Image>().sprite = HealthPieceGreenSprite;
                    }
                }
                ItemSpawner.numPowerUps--;
                other.transform.parent.gameObject.GetComponent<ItemSpawner>().empty = true;
                other.gameObject.SetActive(false);
            }
            //If the player gets shot
            if (!isAlien && other.tag == "alienBullet")
            {
                Destroy(other.gameObject);
                if (!usingShield)
                {
                    health -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                }
                else
                {
                    if ((shieldPower - MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage) <= 0.1)
                    {
                        usingShield = false;
                        shieldPower = 0.1f;
                    }
                    else
                    {
                        shieldPower -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                    }
                }
            }
            else if (isAlien && other.tag == "robotBullet")
            {
                Destroy(other.gameObject);
                if (!usingShield)
                {
                    health -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                }
                else
                {
                    if ((shieldPower - MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage) <= 0.1)
                    {
                        usingShield = false;
                        shieldPower = 0.1f;
                    }
                    else
                    {
                        shieldPower -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                    }
                }
            }
            if (other.tag == "Laser" && other.GetComponent<LaserController>().shooting && usingShield == false)
            {
                health -= 5f * Time.deltaTime;
            }
            if (other.tag == "Geyser" && other.GetComponent<Geyser>().erupting && usingShield == false)
            {
                health -= 5f * Time.deltaTime;
            }
            //If the player gets hit with melee
            if (other.name == "Sword(Clone)" && other.tag != tag)
            {
                Destroy(other.gameObject);
                Vector3 this2That = new Vector3(this.transform.position.x - enemy.transform.position.x, 0, this.transform.position.z - enemy.transform.position.z);
                controller.SimpleMove(MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage * (Vector3.Normalize(this2That)));
                if (!usingShield)
                {
                    health -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                }
                else
                {
                    if ((shieldPower - MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage) <= 0.1)
                    {
                        usingShield = false;
                        shieldPower = 0.1f;
                    }
                    else
                    {
                        shieldPower -= MoveController.GetComponent<PawnMove>().Player01.GetComponent<PiecePropScript>().Damage;
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Laser" && other.GetComponent<LaserController>().shooting && usingShield == false)
        {
            health -= 5*Time.deltaTime;
        }
        if (other.tag == "Geyser" && other.GetComponent<Geyser>().erupting && usingShield == false)
        {
            health -= 5f * Time.deltaTime;
        }
    }

    void RobotGruntSpecial()
    {
        if (shieldPower >= 0.1)
        {
            special.GetComponent<Image>().fillAmount = shieldPower / 100;
        }
        else
        {
            shieldPower = 0.1f;
        }
        if (usingShield == false && shieldPower <= 100 && !shieldOverheat)
        {
            shieldPower += Time.deltaTime * 4;
            if (shieldPower > 100)
            {
                shieldPower = 100;
            }
            ps.Stop(false);
            ps.Clear(false);
        }
        else if (usingShield)
        {
            shieldPower -= Time.deltaTime * 10;
            if (shieldPower <= 0.1)
            {
                shieldPower = 0.1f;
                shieldOverheat = true;
                usingShield = false;
            }
            ps.Play(false);
        }
        if (shieldOverheat)
        {
            ps.Stop(false);
            ps.Clear(false);
            shieldPower += 2 * Time.deltaTime;
            if (shieldPower >= 30)
            {
                shieldOverheat = false;
            }
        }

        if (shieldPower >= 0.1 && !shieldOverheat)
        {
            if (Input.GetJoystickNames().Length != 0) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger2") == 1)
                {
                    usingShield = true;
                }
                else
                {
                    usingShield = false;
                }
            }
            else
            {
                if (Input.GetAxis("Special2") == 1)
                {
                    usingShield = true;
                }
                else
                {
                    usingShield = false;
                }
            }
        }
    }

    /*void RobotScoutSpecial()
    {
        if (specialAvailable)
        {
            if (Input.GetJoystickNames().Length != 0) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger1") == 1)
                {

                }
            }
            else
            {
                if (Input.GetAxis("Special1") == 1)
                {

                }
            }
        }
    }*/

    void RobotTankSpecial()
    {
        if (specialAvailable == false)
        {
            specialTimer -= Time.deltaTime;
            if (specialTimer < 0)
            {
                specialTimer = 10.0f;
                special.GetComponent<Image>().fillAmount = 1.0f;
                specialAvailable = true;
            }
        }
        if (Input.GetJoystickNames().Length == 2) // If there is a controller connected
        {
            if (Input.GetAxis("360_LeftTrigger2") == 1 && specialAvailable)
            {
                Rigidbody missileClone = Instantiate(robotMissile, transform.position + (1.2f * bulletSize * this.transform.forward) + (1.2f * this.transform.up), transform.rotation) as Rigidbody;
                missileClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                missileClone.GetComponent<TargetEnemy>().target = enemy.gameObject;
                missileClone.rigidbody.useGravity = false;
                missileClone.velocity = transform.TransformDirection(Vector3.forward * 0.75f * bulletSpeed);
                Destroy(missileClone.gameObject, 20);
                audio.Play();
                audio.Play();
                special.GetComponent<Image>().fillAmount = 0.001f;
                specialAvailable = false;
            }
        }
        else
        {
            if (Input.GetAxis("Special2") == 1 && specialAvailable)
            {
                Rigidbody missileClone = Instantiate(robotMissile, transform.position + (1.2f * bulletSize * this.transform.forward) + (1.2f * this.transform.up), transform.rotation) as Rigidbody;
                missileClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                missileClone.GetComponent<TargetEnemy>().target = enemy.gameObject;
                missileClone.rigidbody.useGravity = false;
                missileClone.velocity = transform.TransformDirection(Vector3.forward * 0.75f * bulletSpeed);
                Destroy(missileClone.gameObject, 20);
                audio.Play();
                audio.Play();
                special.GetComponent<Image>().fillAmount = 0.001f;
                specialAvailable = false;
            }
        }
    }

    void RobotRunnerSpecial()
    {
        if (specialAvailable)
        {
            if (Input.GetJoystickNames().Length == 2) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger2") == 1)
                {
                    if (controller.velocity.magnitude != 0)
                    {
                        controller.Move(moveDirection * 3);
                        boost = true;
                        specialAvailable = false;
                        special.GetComponent<Image>().fillAmount = 0.001f;
                    }
                }
            }
            else
            {
                if (Input.GetAxis("Special2") == 1)
                {
                    if (controller.velocity.magnitude != 0)
                    {
                        controller.Move(moveDirection * 3);
                        boost = true;
                        specialAvailable = false;
                        special.GetComponent<Image>().fillAmount = 0.001f;
                    }
                }
            }
        }
        else
        {
            specialTimer -= Time.deltaTime;
            if (specialTimer < 0)
            {
                specialTimer = 8.0f;
                special.GetComponent<Image>().fillAmount = 1.0f;
                specialAvailable = true;
            }
        }
    }

    void AlienGruntSpecial()
    {
        if (shieldPower >= 0.1)
        {
            special.GetComponent<Image>().fillAmount = shieldPower / 100;
        }
        else
        {
            shieldPower = 0.1f;
        }
        if (usingShield == false && shieldPower <= 100 && !shieldOverheat)
        {
            shieldPower += Time.deltaTime * 4;
            if (shieldPower > 100)
            {
                shieldPower = 100;
            }
            ps.Stop(false);
            ps.Clear(false);
        }
        else if (usingShield)
        {
            shieldPower -= Time.deltaTime * 10;
            if (shieldPower <= 0.1)
            {
                shieldPower = 0.1f;
                shieldOverheat = true;
                usingShield = false;
            }
            ps.Play(false);
        }
        if (shieldOverheat)
        {
            ps.Stop(false);
            ps.Clear(false);
            shieldPower += 2 * Time.deltaTime;
            if (shieldPower >= 30)
            {
                shieldOverheat = false;
            }
        }

        if (shieldPower >= 1 && !shieldOverheat)
        {
            if (Input.GetJoystickNames().Length == 2) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger2") == 1)
                {
                    usingShield = true;
                }
                else
                {
                    usingShield = false;
                }
            }
            else
            {
                if (Input.GetAxis("Special2") == 1)
                {
                    usingShield = true;
                }
                else
                {
                    usingShield = false;
                }
            }
        }
    }

    /*void AlienScoutSpecial()
    {
        if (specialAvailable)
        {
            if (Input.GetJoystickNames().Length != 0) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger1") == 1)
                {

                }
            }
            else
            {
                if (Input.GetAxis("Special1") == 1)
                {

                }
            }
        }
    }*/

    void AlienTankSpecial()
    {
        if (specialAvailable == false)
        {
            specialTimer -= Time.deltaTime;
            if (specialTimer < 0)
            {
                specialTimer = 10.0f;
                special.GetComponent<Image>().fillAmount = 1.0f;
                specialAvailable = true;
            }
        }
        if (Input.GetJoystickNames().Length != 0) // If there is a controller connected
        {
            if (Input.GetAxis("360_LeftTrigger2") == 1 && specialAvailable)
            {
                Rigidbody missileClone = Instantiate(alienMissile, transform.position + (1.2f * bulletSize * this.transform.forward) + (1.2f * this.transform.up), transform.rotation) as Rigidbody;
                missileClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                missileClone.GetComponent<TargetEnemy>().target = enemy.gameObject;
                missileClone.rigidbody.useGravity = false;
                missileClone.velocity = transform.TransformDirection(Vector3.forward * 0.75f * bulletSpeed);
                Destroy(missileClone.gameObject, 20);
                audio.Play();
                audio.Play();
                special.GetComponent<Image>().fillAmount = 0.001f;
                specialAvailable = false;
            }
        }
        else
        {
            if (Input.GetAxis("Special2") == 1 && specialAvailable)
            {
                Rigidbody missileClone = Instantiate(alienMissile, transform.position + (1.2f * bulletSize * this.transform.forward) + (1.2f * this.transform.up), transform.rotation) as Rigidbody;
                missileClone.gameObject.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                missileClone.GetComponent<TargetEnemy>().target = enemy.gameObject;
                missileClone.rigidbody.useGravity = false;
                missileClone.velocity = transform.TransformDirection(Vector3.forward * 0.75f * bulletSpeed);
                Destroy(missileClone.gameObject, 20);
                audio.Play();
                audio.Play();
                special.GetComponent<Image>().fillAmount = 0.001f;
                specialAvailable = false;
            }
        }
    }

    void AlienRunnerSpecial()
    {
        if (specialAvailable)
        {
            if (Input.GetJoystickNames().Length == 2) // If there is a controller connected
            {
                if (Input.GetAxis("360_LeftTrigger2") == 1)
                {
                    if (controller.velocity.magnitude != 0)
                    {
                        controller.Move(moveDirection * 3);
                        boost = true;
                        specialAvailable = false;
                        special.GetComponent<Image>().fillAmount = 0.001f;
                    }
                }
            }
            else
            {
                if (Input.GetAxis("Special2") == 1)
                {
                    if (controller.velocity.magnitude != 0)
                    {
                        controller.Move(moveDirection * 3);
                        boost = true;
                        specialAvailable = false;
                        special.GetComponent<Image>().fillAmount = 0.001f;
                    }
                }
            }
        }
        else
        {
            specialTimer -= Time.deltaTime;
            if (specialTimer < 0)
            {
                specialTimer = 8.0f;
                special.GetComponent<Image>().fillAmount = 1.0f;
                specialAvailable = true;
            }
        }
    }

    void UpdateStats()
    {
        if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteGrunt(Clone)")
        {
            if (enemyName == "BlackGrunt(Clone)")
            {
                BattleStats.RobotGruntWinsVSAlienGrunt++;
            }
            else if (enemyName == "BlackTank(Clone)")
            {
                BattleStats.RobotGruntWinsVSAlienTank++;
            }
            else if (enemyName == "BlackScout(Clone)")
            {
                BattleStats.RobotGruntWinsVSAlienScout++;
            }
            else if (enemyName == "BlackRunner(Clone)")
            {
                BattleStats.RobotGruntWinsVSAlienRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteTank(Clone)")
        {
            if (enemyName == "BlackGrunt(Clone)")
            {
                BattleStats.RobotTankWinsVSAlienGrunt++;
            }
            else if (enemyName == "BlackTank(Clone)")
            {
                BattleStats.RobotTankWinsVSAlienTank++;
            }
            else if (enemyName == "BlackScout(Clone)")
            {
                BattleStats.RobotTankWinsVSAlienScout++;
            }
            else if (enemyName == "BlackRunner(Clone)")
            {
                BattleStats.RobotTankWinsVSAlienRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteScout(Clone)")
        {
            if (enemyName == "BlackGrunt(Clone)")
            {
                BattleStats.RobotScoutWinsVSAlienGrunt++;
            }
            else if (enemyName == "BlackTank(Clone)")
            {
                BattleStats.RobotScoutWinsVSAlienTank++;
            }
            else if (enemyName == "BlackScout(Clone)")
            {
                BattleStats.RobotScoutWinsVSAlienScout++;
            }
            else if (enemyName == "BlackRunner(Clone)")
            {
                BattleStats.RobotScoutWinsVSAlienRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "WhiteRunner(Clone)")
        {
            if (enemyName == "BlackGrunt(Clone)")
            {
                BattleStats.RobotRunnerWinsVSAlienGrunt++;
            }
            else if (enemyName == "BlackTank(Clone)")
            {
                BattleStats.RobotRunnerWinsVSAlienTank++;
            }
            else if (enemyName == "BlackScout(Clone)")
            {
                BattleStats.RobotRunnerWinsVSAlienScout++;
            }
            else if (enemyName == "BlackRunner(Clone)")
            {
                BattleStats.RobotRunnerWinsVSAlienRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackGrunt(Clone)")
        {
            if (enemyName == "WhiteGrunt(Clone)")
            {
                BattleStats.AlienGruntWinsVSRobotGrunt++;
            }
            else if (enemyName == "WhiteTank(Clone)")
            {
                BattleStats.AlienGruntWinsVSRobotTank++;
            }
            else if (enemyName == "WhiteScout(Clone)")
            {
                BattleStats.AlienGruntWinsVSRobotScout++;
            }
            else if (enemyName == "WhiteRunner(Clone)")
            {
                BattleStats.AlienGruntWinsVSRobotRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackTank(Clone)")
        {
            if (enemyName == "WhiteGrunt(Clone)")
            {
                BattleStats.AlienTankWinsVSRobotGrunt++;
            }
            else if (enemyName == "WhiteTank(Clone)")
            {
                BattleStats.AlienTankWinsVSRobotTank++;
            }
            else if (enemyName == "WhiteScout(Clone)")
            {
                BattleStats.AlienTankWinsVSRobotScout++;
            }
            else if (enemyName == "WhiteRunner(Clone)")
            {
                BattleStats.AlienTankWinsVSRobotRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackScout(Clone)")
        {
            if (enemyName == "WhiteGrunt(Clone)")
            {
                BattleStats.AlienScoutWinsVSRobotGrunt++;
            }
            else if (enemyName == "WhiteTank(Clone)")
            {
                BattleStats.AlienScoutWinsVSRobotTank++;
            }
            else if (enemyName == "WhiteScout(Clone)")
            {
                BattleStats.AlienScoutWinsVSRobotScout++;
            }
            else if (enemyName == "WhiteRunner(Clone)")
            {
                BattleStats.AlienScoutWinsVSRobotRunner++;
            }
        }
        else if (MoveController.GetComponent<PawnMove>().Player02.name == "BlackRunner(Clone)")
        {
            if (enemyName == "WhiteGrunt(Clone)")
            {
                BattleStats.AlienRunnerWinsVSRobotGrunt++;
            }
            else if (enemyName == "WhiteTank(Clone)")
            {
                BattleStats.AlienRunnerWinsVSRobotTank++;
            }
            else if (enemyName == "WhiteScout(Clone)")
            {
                BattleStats.AlienRunnerWinsVSRobotScout++;
            }
            else if (enemyName == "WhiteRunner(Clone)")
            {
                BattleStats.AlienRunnerWinsVSRobotRunner++;
            }
        }
    }
}
