using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Transform trans;
    CapsuleCollider2D cc;
    Animator anim;
    AudioManager audioManager;
    AnimatorSpawner animatorSpawner;
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
    bool inAir;
    [SerializeField] bool stompAction;
    bool isGrounded;
    bool falling;
    bool rotated;
    bool sprinting;
    bool jumpOrFall;
    bool justFalled;
    bool canMove;
    bool isBasicPlayer;
    bool isSecondTypePlayer;
    bool isThirdTypePlayer;
    bool flip; // TRUE = RIGHT; FALSE = LEFT
    bool AOrDUp;
    float moveHoldTime;
    float notMoved;

    // Start is called before the first frame update
    void Start()
    {
        animatorSpawner = AnimatorSpawner.instance;
        audioManager = AudioManager.instance;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();
        canMove = true;
        isGrounded = true;
        flip = true;

        isBasicPlayer = true;
        isSecondTypePlayer = false;
        isThirdTypePlayer = false;
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
            
        }
        isGroundedCheck();
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
                    AOrDUp = false;
                    moveHoldTime += Time.deltaTime;
                    if (moveHoldTime > moveHoldTimeLimit) {
                        Flip(0);
                        MovePlayer(moveSpeed - crouchSpeed, 1f, 1);
                        if (!falling) {
                            anim.SetBool("StartRun", true);
                        } else if (falling) {
                            anim.SetTrigger("Falling");
                        }
                    }
                    if (notMoved < 0.04) {
                        notMoved += Time.deltaTime;
                    }
                } else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
                    AOrDUp = false;
                    moveHoldTime += Time.deltaTime;
                    if (moveHoldTime > moveHoldTimeLimit) {
                        Flip(1);
                        MovePlayer(moveSpeed - crouchSpeed, 1f, 0);
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
            } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
                anim.SetBool("StopRun", false);
                if (Input.GetKey(KeyCode.D)) {
                    AOrDUp = false;
                    moveHoldTime += Time.deltaTime;
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
                } else if (Input.GetKey(KeyCode.A)) {
                    AOrDUp = false;
                    moveHoldTime += Time.deltaTime;
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
                if (notMoved > 0) {
                    notMoved -= Time.deltaTime;
                }
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
                AOrDUp = true;
                if (moveHoldTime < moveHoldTimeLimit) {
                    if (Input.GetKeyUp(KeyCode.A)) {
                        Flip(1);
                    } else if (Input.GetKeyUp(KeyCode.D)) {
                        Flip(0); 
                    }
                } else {
                    anim.SetBool("Run", false);
                    anim.SetBool("StartRun", false);                    
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    if (isGrounded) {
                        animatorSpawner.SpawnAnimation("stopDust");
                    }
                }  
                moveHoldTime = 0;
            } 
            // JUMP CODE
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
                anim.SetTrigger("Jump");
                audioManager.PlaySound("jump");
                animatorSpawner.SpawnAnimation("jumpDust");
                rb.drag = 0;
                rb.velocity = Vector2.up * jumpPower;
            }
            // CROUCH
            if (Input.GetKeyDown(KeyCode.S) && inAir) {
                // AIR SLAM / AIR STOMP
                rb.velocity = Vector2.down * stompSpeed;
                stompAction = true;
                anim.SetTrigger("StompFall");
                setCanMove();
            } 
            // UNCROUCH
            if (Input.GetKeyUp(KeyCode.S)) {
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

    void FixedUpdate() {
        
    }

    private void isGroundedCheck() {
        float extraHeightTest = .05f;
        RaycastHit2D rchit = Physics2D.BoxCast(cc.bounds.center, cc.bounds.size,  0f, Vector2.down, extraHeightTest, platformLayerMask);
        bool temp = rchit.collider != null;
        if (temp) {
            inAir = false;
            isGrounded = true;
            Debug.Log("2st");
        } else if (!temp && stompAction) {
            inAir = true;
            isGrounded = false;
            Debug.Log("3st");
        } else if (!temp) {
            inAir = true;
            isGrounded = false;
            Debug.Log("4st");
        }
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
            Invoke("setCanMove", 0.2f);
        }
        if (col.gameObject.tag == "Ground") {
            if (stompAction) {
                animatorSpawner.SpawnAnimation("stompDust");
                audioManager.PlaySound("airslamlanding");
                anim.SetTrigger("StompToIdle");
                anim.SetBool("StartRun", false);
                inAir = false;
                isGrounded = true;
                stompAction = false;
                Invoke("setCanMove", 0.58f);
            } else {
                audioManager.PlaySound("landing");
                animatorSpawner.SpawnAnimation("fallDust");
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
                    anim.SetTrigger("FallToRun");
                } else anim.SetTrigger("FallToIdle");
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
    }

    private void RandomFootstepAudioGenerator() {
        audioManager.PlaySound("footstep_" + (int)Random.Range(1,4));
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
            if (!stompAction && isGrounded) {
                anim.SetTrigger("Falling");
            }
        } else {
            falling = false;
        }
    }
}
