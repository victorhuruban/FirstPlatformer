using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Transform trans;
    BoxCollider2D boxCollider;
    Animator anim;
    AudioManager audioManager;
    AnimatorSpawner animatorSpawner;
    GameObject ledgeCollider;
    [SerializeField] GameObject coin;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float sprintSpeed;
    [SerializeField] float stompSpeed;
    [SerializeField] float knockBackForce;
    [SerializeField] float fallingThreshold;
    [SerializeField] float moveHoldTimeLimit;
    [SerializeField] int crouchSpeed;
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private LayerMask ledgeLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    bool inAir;
    bool stompAction;
    bool isGrounded;
    bool isCrouched;
    bool falling;
    bool rotated;
    bool sprinting;
    bool jumpOrFall;
    bool justFalled;
    bool isJumping;
    float jumpTimeCounter;
    public float jumpTime;
    bool canMove;
    bool isBasicPlayer;
    bool isSecondTypePlayer;
    bool isThirdTypePlayer;
    bool flip; // TRUE = RIGHT; FALSE = LEFT
    bool AOrDUp;
    float moveHoldTime;
    bool moveHoldTimeDownTrigger;
    bool canMoveLeft;
    bool canMoveRight;
    bool airAttack;
    bool ledgeMovement;
    bool ledgeDetection;
    bool leftLedge;
    bool rightLedge;
    bool waitAfterLedgeClimb;
    float notMoved;
    [SerializeField] Transform attackPositionJ;
    [SerializeField] Transform attackPositionK;
    public float attackRangeXJ;
    public float attackRangeYJ;
    public float attackRangeXK;
    public float attackRangeYK;

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
        moveHoldTimeDownTrigger = false;
        
        isBasicPlayer = true;
        isSecondTypePlayer = false;
        isThirdTypePlayer = false;

        canMoveLeft = true;
        canMoveRight = true;

        ledgeDetection = true;
    }

    void Update() {
        Debug.Log(ledgeCollider);
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
        } 
        if (ledgeMovement) {
            ManageLedgeMovement();
        }
        moveHoldTimeDown();
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
                moveHoldTimeDownTrigger = false;

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
                moveHoldTimeDownTrigger = true;
                anim.SetBool("StopRun", false);
                if (Input.GetKey(KeyCode.D)) {
                    if (canMoveRight) {
                        if (!isCrouched) {
                            AOrDUp = false;
                            if (moveHoldTime < 0.5f) {
                                moveHoldTime += Time.deltaTime;
                            }
                            if (moveHoldTime > moveHoldTimeLimit) {
                                Flip(0);
                                MovePlayer(moveSpeed, 1f, 1);
                                if (!falling) {
                                    anim.SetBool("StartRun", true);
                                } else if (falling) {
                                    anim.SetTrigger("Falling");
                                }
                            } 
                            if (notMoved < 0.04) {
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
                            if (moveHoldTime < 0.5f) {
                                moveHoldTime += Time.deltaTime;
                            }
                            if (moveHoldTime > moveHoldTimeLimit) {
                                Flip(1);
                                MovePlayer(moveSpeed, 1f, 0);
                                if (!falling) {
                                    anim.SetBool("StartRun", true);
                                } else if (falling) {
                                    anim.SetTrigger("Falling");
                                }
                            } 
                            if (notMoved < 0.04) {
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
                }
                anim.SetBool("Run", false);
                anim.SetBool("StartRun", false); 
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
                AOrDUp = true;
                if (moveHoldTime < moveHoldTimeLimit) {
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
                moveHoldTimeDownTrigger = false;
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

    private void ManageLedgeMovement() {
        anim.SetTrigger("Climb_Ledge");
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

    private void Attack(int damage, string type) {
        Collider2D[] enemiesToDamage = null;
        if (type == "J") {
            enemiesToDamage = Physics2D.OverlapBoxAll(attackPositionJ.position, new Vector2(attackRangeXJ, attackRangeYJ), 0, enemyLayerMask);
        } else if (type == "K") {
            enemiesToDamage = Physics2D.OverlapBoxAll(attackPositionK.position, new Vector2(attackRangeXK, attackRangeYK), 0, enemyLayerMask);
        }
        
        for (int i = 0; i < enemiesToDamage.Length; i++) {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(attackPositionJ.position, new Vector3(attackRangeXJ, attackRangeYJ, 1));
        Gizmos.DrawWireCube(attackPositionK.position, new Vector3(attackRangeXK, attackRangeYK, 1));
    }

    private void surroundingCheck() {
        // Ground Check
        float extraHeightTest = .1f;
        float extraSideLength = .05f;
        RaycastHit2D groundCheckRC = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size,  0f, Vector2.down, extraHeightTest, platformLayerMask);
        bool groundCheckRCbool = groundCheckRC.collider != null;
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
        //Debug.DrawRay(capsuleCollider.bounds.center + new Vector3(capsuleCollider.bounds.extents.x, 0), Vector2.down * (capsuleCollider.bounds.extents.y + extraHeightTest), tempColor, 0, true);
        //Debug.DrawRay(capsuleCollider.bounds.center + new Vector3(-capsuleCollider.bounds.extents.x, 0), Vector2.down * (capsuleCollider.bounds.extents.y + extraHeightTest), tempColor, 0, true);
        //Debug.DrawRay(capsuleCollider.bounds.center + new Vector3(-capsuleCollider.bounds.extents.x, -capsuleCollider.bounds.extents.y - extraHeightTest), Vector2.right * (capsuleCollider.bounds.extents.y - extraHeightTest), tempColor, 0, true);

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
        //Debug.DrawRay(capsuleCollider.bounds.center, new Vector2(-(capsuleCollider.bounds.extents.x + extraSideLength), 0), tempColorL, 0);

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
        //Debug.DrawRay(capsuleCollider.bounds.center, new Vector2(capsuleCollider.bounds.extents.x + extraSideLength, 0), tempColorR, 0);
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
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Coin") { // COIN COLLECTION
            audioManager.PlaySound("coin_collect");
            Vector2 loc = col.gameObject.transform.position;
            Instantiate(coin, loc, Quaternion.identity);
            Destroy(col.gameObject);
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
                ledgeMovement = true;
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

    private void moveHoldTimeDown() {
        if (!moveHoldTimeDownTrigger && moveHoldTime >= 0) {
            moveHoldTime -= Time.deltaTime;
        }
    }

    private void setLedgeDetection() {
        ledgeDetection = true;
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
        ledgeMovement = false;
        canMove = true;
        rb.isKinematic = false;
        setLedgeDetection();
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
