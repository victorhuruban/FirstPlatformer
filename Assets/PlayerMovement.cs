using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform trans;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private AudioManager audioManager;
    private AnimatorSpawner animatorSpawner;
    private GameObject ledgeCollider;

    ////////////////
    // HEALTH BAR //
    ////////////////
    [Header("Health Bar Variables")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private int maxHealthPoints = 100;
    private int currentHealth; // USED TO KEEP TRACK OF CURRENT HEALTH
    
    [Space]
    /////////////////////
    // PLAYER MOVEMENT //
    /////////////////////
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] float moveHoldTimeLimit;
    [SerializeField] int crouchSpeed;
    private bool isCrouched; // VARIABLE TO CHECK IF THE PLAYER IS CROUCHED
    private bool canMove; // VARIABLE TO CHECK IS THE PLAYER CAN OR CANNOT MOVE
    private bool isBasicPlayer; // FREDERICK MOVEMENT
    private bool isSecondTypePlayer; // SECOND CHARACTER MOVEMENT
    private bool isThirdTypePlayer; // THIRD CHARACTER MOVEMENT
    private bool flip; // TRUE = RIGHT; FALSE = LEFT
    private bool canMoveLeft; // CARIABLE TO CHECK IF THE PLAYER CAN MOVE LEFT
    private bool canMoveRight; // CARIABLE TO CHECK IF THE PLAYER CAN MOVE RIGHT
    private float notMoved; // CHECKS THE TIME YOU WERE HOLDING THE A OR D BUTTON BEFORE SWITCHING THE DIRECTIOn
    private bool AOrDUp; // CHECK IT UP

    [Space]
    /////////////////
    // PLAYER JUMP //
    /////////////////
    [Header("Jump Variables")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float stompSpeed;
    [SerializeField] private float knockBackForce;
    [SerializeField] private float fallingThreshold; // MIGHT MAKE IT PRIVATE
    [Tooltip("How much you jump if you hold down 'Space' button")]
    [SerializeField] private float jumpTime; // HOW MUCH IT JUMPS IF YOU HOLD SPACE
    private bool inAir; // CHECKS IF THE PLAYER IS IN AIR
    private bool stompAction; // CHECKS IF YOU PRESSED 'S' WHILE IN AIR TO STOMP
    private bool isGrounded; // CHECKS IF THE PLAYER IS GROUNDED FOR VARIOUS REASONS (ATTACK,JUMPING)
    private bool falling; // CHECKS IF THE PLAYER IS CURRENTLY FALLING IN THE 'isFalling()' METHOD
    private bool isJumping; // CHECKS IF YOU ARE HOLDING THE JUMP BUTTON TO KEEP PUSHING THE PLAYER UPWARDS
    private float jumpTimeCounter; // USED TO KEEP TRACK OF JUMP BUTTON HOLDING
    
    [Space]
    ///////////////////
    // PLAYER Attack //
    ///////////////////
    [Header("Player Attack Variables")]
    [Tooltip("GameObject Rectangle for Light Attack (J)")]
    [SerializeField] private Transform attackPositionJ;
    [Tooltip("GameObject Rectangle for Heavy/Upwards Attack (K)")]
    [SerializeField] private Transform attackPositionK;
    [SerializeField] private float attackRangeXJ;
    [SerializeField] private float attackRangeYJ;
    [SerializeField] private float attackRangeXK;
    [SerializeField] private float attackRangeYK;
    private bool airAttack; // CHECKS IF YOU PRESS ATTACK BUTTON IN AIR
    
    ///////////////////////////
    // LEDGE CLIMB VARIABLES //
    ///////////////////////////
    private bool waitAfterLedgeClimb;// CHECKS IF YOU WAITED ENOUGH TIME BEFORE ANOTHER LEDGE CLIMB
    private bool ledgeDetection; // CHECKS IF THE LEDGE COLLIDER COLLIDES WITH A LEDGE
    private bool leftLedge; // CHECKS IF THE LEDGE IS ON THE LEFT SIDE
    private bool rightLedge; // CHECKS IF THE LEDGE IS ON THE RIGHT SIDE
    [Space]
    [Header("Miscellaneous")]
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    [Tooltip("The particle prefab for when you collect a coin")]
    [SerializeField] private GameObject coin;

    void Start()
    {
        animatorSpawner = AnimatorSpawner.instance;
        audioManager = AudioManager.instance;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        ledgeCollider = trans.GetChild(0).gameObject;
        canMove = true;
        isGrounded = true;
        flip = true;
        
        isBasicPlayer = true;
        isSecondTypePlayer = false;
        isThirdTypePlayer = false;

        canMoveLeft = true;
        canMoveRight = true;

        ledgeDetection = true;

        healthBar.SetMaxHealth(maxHealthPoints);
        currentHealth = maxHealthPoints;
    }

    void Update() {
        if (canMove) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                // Transform to first character
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                // Transform to second character
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                // Transform to third character
            }
            ManageMovement();
            ManageAttack(); 
            ManageParry();
        } 
        surroundingCheck();
        isFalling();
    }

    // MOVEMENT SETTINGS
    //
    private void ManageMovement() {
        if (isBasicPlayer) {
            // Frederick Movement
            //
            //
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) {
                // STOP CHARACTER
                rb.velocity = new Vector2(0, rb.velocity.y);
                anim.SetBool("Run", false);
                anim.SetBool("StartRun", false); 
                anim.SetBool("StopRun", true);

            } else if ((Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))) {
                anim.SetBool("StopRun", false);
                if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
                    if (isCrouched) {
                        Flip(0);
                        anim.SetBool("CrouchIdleToWalk", true);
                        MovePlayer(moveSpeed - crouchSpeed, 0.5f, 1);
                    } 
                } else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
                    if (isCrouched) {
                        Flip(1);
                        anim.SetBool("CrouchIdleToWalk", true);
                        MovePlayer(moveSpeed - crouchSpeed, 0.65f, 0);
                    } 
                }
            } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
                anim.SetBool("StopRun", false);
                if (Input.GetKey(KeyCode.D)) {
                    if (canMoveRight) {
                        if (!isCrouched) {
                            AOrDUp = false;
                            Flip(0);
                            MovePlayer(moveSpeed, 1f, 1);
                            if (!falling) {
                                anim.SetBool("StartRun", true);
                            } else if (falling) {
                                anim.SetTrigger("Falling");
                            }
                            if (notMoved < 0.09) {
                                notMoved += Time.deltaTime;
                            } 
                        }
                    } else {
                        anim.SetBool("StartRun", false);
                        anim.SetBool("StopRun", true);
                    }
                } else if (Input.GetKey(KeyCode.A)) {
                    if (canMoveLeft) {
                        if (!isCrouched) {
                            AOrDUp = false;
                            Flip(1);
                            MovePlayer(moveSpeed, 1f, 0);
                            if (!falling) {
                                anim.SetBool("StartRun", true);
                            } else if (falling) {
                                anim.SetTrigger("Falling");
                            }
                            if (notMoved < 0.09) {
                                notMoved += Time.deltaTime;
                            }  
                        }
                    } else {
                        anim.SetBool("StartRun", false);
                        anim.SetBool("StopRun", true);
                    }
                }
                
            } else {
                if (notMoved > 0) {
                    notMoved -= Time.deltaTime;
                } else if (notMoved < 0) {
                    anim.SetBool("Run", false);
                    anim.SetBool("StartRun", false); 
                }
                
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
                AOrDUp = true;
                if (notMoved > 0) {
                    if (Input.GetKeyUp(KeyCode.A)) {
                        Flip(1);
                    }
                    if (Input.GetKeyUp(KeyCode.D)) {
                        Flip(0); 
                    }
                } else {
                    anim.SetBool("Run", false);
                    anim.SetBool("StartRun", false);                    
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    if (isGrounded && (canMoveLeft && canMoveRight)) {
                        animatorSpawner.SpawnAnimation("stopDust");
                    }
                }  
                if (isCrouched) {
                    anim.SetBool("CrouchIdleToWalk", false);
                }
            } 
            // JUMP CODE
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
                anim.SetTrigger("Jump");
                audioManager.PlaySound("jump");
                animatorSpawner.SpawnAnimation("jumpDust");
                rb.drag = 0;
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rb.velocity = Vector2.up * jumpPower;
            }
            if (Input.GetKey(KeyCode.Space) && isJumping) {
                if (jumpTimeCounter > 0) {
                    rb.velocity = Vector2.up * jumpPower;
                    jumpTimeCounter -= Time.deltaTime;
                    if (Input.GetKey(KeyCode.A)) {
                        MovePlayer(moveSpeed, 1f, 0);
                    } else if (Input.GetKey(KeyCode.D)) {
                        MovePlayer(moveSpeed, 1f, 1);
                    }
                } else {
                    isJumping = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                isJumping = false;
            }
            // CROUCH
            if (Input.GetKeyDown(KeyCode.S) && inAir) {
                // AIR SLAM / AIR STOMP
                rb.velocity = Vector2.down * stompSpeed;
                stompAction = true;
                anim.SetTrigger("StompFall");
                setCanMove();
            } else if (Input.GetKeyDown(KeyCode.S) && !inAir && isGrounded) {
                // COURCH
                anim.SetTrigger("Crouch");
                isCrouched = true;
            }
            // UNCROUCH
            if (Input.GetKeyUp(KeyCode.S)) {
                anim.SetTrigger("CrouchToIdle");
                isCrouched = false;
                // FROM CROUCH TO IDLe
            }
        } else if (isSecondTypePlayer) {
            // SECOND CHARACTER MOVEMENT
        } else if (isThirdTypePlayer) {
            // THIRD CHARACTER MOVEMENT
        }
        if (notMoved <= 0 && AOrDUp) {
            AOrDUp = true;
            anim.SetBool("StopRun", true);
        }
    }

    private void ManageAttack() {
        if (isBasicPlayer && canMove) {
            if (Input.GetKeyDown(KeyCode.J) && isGrounded) {
                setCanMove(); // MAKE UNABLE TO MOVE
                anim.SetTrigger("Ground_Attack_" + (int)Random.Range(1,3));
                Attack(20, "J");
                audioManager.PlaySound("swordswoosh");
                Invoke("setCanMove", 0.25f);
            }
            if ((Input.GetKeyDown(KeyCode.J) || ((Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.A)) || (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.D)))) && !isGrounded && airAttack) {
                airAttack = false;
                anim.SetTrigger("Jump_Attack");
                Attack(20, "J");
                audioManager.PlaySound("swordswoosh");
            }
            if (Input.GetKeyDown(KeyCode.K) && isGrounded) {
                setCanMove(); // MAKE UNABLE TO MOVE
                anim.SetTrigger("Ground_Heavy_Attack");
                Attack(35, "K");
                audioManager.PlaySound("swordswoosh");
                Invoke("setCanMove", 0.35f);
            }
            if ((Input.GetKeyDown(KeyCode.K) || ((Input.GetKeyDown(KeyCode.K) && Input.GetKey(KeyCode.A)) || (Input.GetKeyDown(KeyCode.K) && Input.GetKey(KeyCode.D)))) && !isGrounded && airAttack) {
                airAttack = false;
                anim.SetTrigger("Jump_Heavy_Attack");
                Attack(35, "K");
                audioManager.PlaySound("swordswoosh");
            }
        }
    }

    private void ManageParry() {
        if (Input.GetKeyDown(KeyCode.L)) {
            anim.SetBool("TryParry", true);
        }
        if (Input.GetKeyUp(KeyCode.L)) {
            anim.SetBool("TryParry", false);
        }
    }

    private void Attack(int damage, string type) {
        Collider2D[] enemiesToDamage = null;
        if (type == "J") {
            enemiesToDamage = Physics2D.OverlapBoxAll(attackPositionJ.position, new Vector2(attackRangeXJ, attackRangeYJ), 0, enemyLayerMask);
        } else if (type == "K") {
            enemiesToDamage = Physics2D.OverlapBoxAll(attackPositionK.position, new Vector2(attackRangeXK, attackRangeYK), 0, enemyLayerMask);
        }
        
        for (int i = 0; i < enemiesToDamage.Length; i++) {
            enemiesToDamage[i].GetComponent<EnemyAIFlying>().TakeDamage(damage);
        }
    }

    /*void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(attackPositionJ.position, new Vector3(attackRangeXJ, attackRangeYJ, 1));
        //Gizmos.DrawWireCube(attackPositionK.position, new Vector3(attackRangeXK, attackRangeYK, 1));
    }*/

    private void surroundingCheck() {
        // Ground Check
        float extraHeightTest = .1f;
        float extraSideLength = .05f;
        RaycastHit2D groundCheckRC = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,  0f, Vector2.down, extraHeightTest, platformLayerMask);
        bool groundCheckRCbool = groundCheckRC.collider != null;
        //Color tempColor = Color.green;
        //Color tempColorL = Color.green;
        //Color tempColorR = Color.green;
        //Color tempColorLedgeL = Color.green;
        //Color tempColorLedgeR = Color.green;
        if (groundCheckRCbool) {
            if (stompAction) {
                animatorSpawner.SpawnAnimation("stompDust");
                audioManager.PlaySound("airslamlanding");
                anim.SetTrigger("StompToIdle");
                anim.SetBool("StartRun", false);
                stompAction = false;
                Invoke("setCanMove", 0.58f);
            } else if (!isGrounded && !waitAfterLedgeClimb) {
                audioManager.PlaySound("landing");
                animatorSpawner.SpawnAnimation("fallDust");
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
                    anim.SetTrigger("FallToRun");
                } else anim.SetTrigger("FallToIdle");
            }
            //tempColor = Color.green;
            inAir = false;
            isGrounded = true;
            airAttack = true;
        } else if (!groundCheckRCbool) {
            //tempColor = Color.red;
            inAir = true;
            isGrounded = false;
        }
        // GROUND CHECK DRAWRAY DEBUG
        //Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightTest), tempColor, 0, true);
        //Debug.DrawRay(boxCollider.bounds.center + new Vector3(-boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightTest), tempColor, 0, true);
        //Debug.DrawRay(boxCollider.bounds.center + new Vector3(-boxCollider.bounds.extents.x, -boxCollider.bounds.extents.y - extraHeightTest), Vector2.right * (boxCollider.bounds.extents.y - extraHeightTest), tempColor, 0, true);

        /////////////

        // Wall Check
        //
        // Left
        RaycastHit2D wallCheckRCLeft = Physics2D.Raycast(boxCollider.bounds.center, Vector2.right, 
            -(boxCollider.bounds.extents.x + extraSideLength), wallLayerMask);
        if (wallCheckRCLeft.collider != null) {
            canMoveLeft = false;
            //tempColorL = Color.red;
        } else {
            canMoveLeft = true;
            //tempColorL = Color.green;
        }
        //Debug.DrawRay(boxCollider.bounds.center, new Vector2(-(boxCollider.bounds.extents.x + extraSideLength), 0), tempColorL, 0);

        // Right
        RaycastHit2D wallCheckRCRight = Physics2D.Raycast(boxCollider.bounds.center, Vector2.right, 
            boxCollider.bounds.extents.x + extraSideLength, wallLayerMask);
        if (wallCheckRCRight.collider != null) {
            canMoveRight = false;
            //tempColorR = Color.red;
        } else {
            canMoveRight = true;
            //tempColorR = Color.green;
        }
        //Debug.DrawRay(boxCollider.bounds.center, new Vector2(boxCollider.bounds.extents.x + extraSideLength, 0), tempColorR, 0);
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Enemy") {
            setCanMove();
            if (Input.GetKey(KeyCode.S)) {
                anim.SetBool("Crouch", false);
            }
            audioManager.PlaySound("damage");
            Vector2 playerPos = transform.position;
            Vector2 dir = col.GetContact(0).point - playerPos;

            rb.velocity = new Vector2(0,0);
            rb.inertia = 0;

            rb.AddForce(-dir.normalized * knockBackForce, ForceMode2D.Impulse);
            anim.SetBool("StartRun", false);
            anim.SetTrigger("Damage");
            Invoke("setCanMove", 0.3f);
            currentHealth -= 20;
            healthBar.SetHealth(currentHealth);
            if (currentHealth <= 0) {
                setCanMove();
                anim.SetTrigger("Death");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Coin") { // COIN COLLECTION
            audioManager.PlaySound("coin_collect");
            Vector2 loc = col.gameObject.transform.position;
            Instantiate(coin, loc, Quaternion.identity);
            Destroy(col.gameObject);
        }
        if (col.gameObject.tag == "Heart") {
            if (currentHealth == maxHealthPoints) {
                Debug.Log("Max health!");
            } else if (currentHealth < maxHealthPoints) {
                if (currentHealth + 20 > maxHealthPoints) {
                    currentHealth = maxHealthPoints;
                } else {
                    currentHealth += 20;
                }
                Destroy(col.gameObject);
                healthBar.SetHealth(currentHealth);
            }
        }
        if (col.gameObject.tag == "Interactable_foliage") {
            Animator temp = col.gameObject.GetComponent<Animator>();
            temp.SetTrigger("Trigger");
        }
        if (col.gameObject.tag == "Ledge") {
            if (ledgeDetection) {
                canMove = false;
                rb.isKinematic = true;
                rb.velocity = new Vector2(0,0);
                anim.SetTrigger("Ledge_Grab");
                anim.SetTrigger("Climb_Ledge");
                if (flip) {
                    rightLedge = true;
                } else leftLedge = true; 
                ledgeDetection = false;
            }
        }
    }

    private void RandomFootstepAudioGenerator() {
        audioManager.PlaySound("footstep_" + (int)Random.Range(1,4));
    }

    private void SheatheSwordAudio() {
        audioManager.PlaySound("sheathsword");
    }

    private void MovePlayer(float moveSpeed, float sprintSpeed, int direction) {
        if (isGrounded) {
            rb.drag = 0;
        }
        if (direction == 0) {
            rb.velocity = new Vector2(-1 * moveSpeed * sprintSpeed, rb.velocity.y);
        } else rb.velocity = new Vector2(1 * moveSpeed * sprintSpeed, rb.velocity.y);
    }

    private void setCanMove() {
        canMove = !canMove;
    }

    private void Flip(int type) {
        if (type == 0) {
            if (!flip) {
                flip = true;
                transform.localRotation = Quaternion.Euler(0,0,0);
            }
        } else if (type == 1) {
            if (flip) {
                flip = false;
                transform.localRotation = Quaternion.Euler(0,180,0);
            }
        }
    }

    private void isFalling() {
        if (rb.velocity.y < fallingThreshold) {
            falling = true;
            if (!stompAction && !isGrounded) {
                anim.SetTrigger("Falling");
            }
        } else {
            falling = false;
        }
    }

    private void setNewLedgePosition() {
        if (leftLedge) {
            transform.position = new Vector2(trans.position.x - 0.7f, trans.position.y + 1.42f);
        } else if (rightLedge) {
            transform.position = new Vector2(trans.position.x + 0.7f, trans.position.y + 1.42f);
        }
        waitAfterLedgeClimb = true;
        climbLedgeToNormal();
    }

    private void climbLedgeToNormal() {
        canMove = true;
        rb.isKinematic = false;
        ledgeDetection = true;
        Invoke("setWaitAfterLedgeClimb", 0.5f);
    }

    private void setWaitAfterLedgeClimb() {
        waitAfterLedgeClimb = false;
        leftLedge = false;
        rightLedge = false;
    }

    private void climbLedgeToIdleOrRun() {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
            anim.SetTrigger("ClimbLedgeToRun");
        } else {
            anim.SetTrigger("ClimbLedgeToIdle");
        }
    }
}
