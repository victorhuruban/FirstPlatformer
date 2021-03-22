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
    [SerializeField] float knockBackForce;
    [SerializeField] int crouchSpeed;
    [SerializeField] private LayerMask platformLayerMask;
    bool isGrounded;
    bool rotated;
    bool sprinting;
    bool jumpOrFall;
    bool justFalled;
    bool canMove;
    bool isBasicPlayer;
    bool isSecondTypePlayer;
    bool isThirdTypePlayer;
    bool flip; // TRUE = RIGHT; FALSE = LEFT
    float chargeJump;

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
                // Transform in ThirdType
                //isRectangle = false;
                //isCircle = false;
                //isSquare = true;
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                // Transform in SecondType
                //isRectangle = false;
                //isCircle = true;
                //isSquare = false;
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                // Transform in BasicType
                //isRectangle = true;
                //isCircle = false;
                //isSquare = false;
            }

            ManageMovement();
            // MOVEMENT SETTINGS
            //
            // RECTANGLE MOVEMENT
            //
            // RIGHT SPRINT
            //if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift)) {
            //    MoveSprintRectangle(moveSpeed, sprintSpeed, 1);
            //    RotatePlayerRectangle(false, true);
            // RIGHT MOVEMENT
            /*}*/ 
            //if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
            //    MoveSprintRectangle(moveSpeed - crouchSpeed, 1f, 1);
            //    ResetAfterMovementRectangle();
            //} else if (Input.GetKey(KeyCode.D)) {
            //    MoveSprintRectangle(moveSpeed, 1f, 1);
            //    RotatePlayerRectangle(false);
            //} 

            // LEFT SPRINT
            //if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift)) {
            //    MoveSprintRectangle(moveSpeed, sprintSpeed, 0);
            //    RotatePlayerRectangle(true, true);
            // LEFT MOVEMENT
            /*}*/ 
            //if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
            //    MoveSprintRectangle(moveSpeed - crouchSpeed, 1f, 0);
            //    ResetAfterMovementRectangle();
            //} else if (Input.GetKey(KeyCode.A)) {
            //    MoveSprintRectangle(moveSpeed, 1f, 0);
            //    RotatePlayerRectangle(true);
            //} 

            // JUMP
            //if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isGroundedCheck()) {
            //    chargeJump += Time.deltaTime;
            //}

            // RESET THE RECTANGLE ANGLE / JUMP FUNC
            /*if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) {
                ResetAfterMovementRectangle();
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

            if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space)) && isGroundedCheck()) {
                anim.SetTrigger("Jump");
                rb.drag = 0;
                if (chargeJump < 0.2f) {
                    rb.velocity = Vector2.up * jumpPower;
                } else if (chargeJump > 0.2f) {
                    rb.velocity = Vector2.up * (jumpPower + 5);
                }
                chargeJump = 0;
            }
            
            if (Input.GetKey(KeyCode.S)) {
                anim.SetBool("Crouch", true);
            } 

            if (Input.GetKeyUp(KeyCode.S)) {
                anim.SetBool("Crouch", false);
            }
            */
        }
    }

    // MOVEMENT SETTINGS
    //
    private void ManageMovement() {
        if (isBasicPlayer) {
            // RECTANGLE MOVEMENT
            //
            // RIGHT MOVEMENT CROUCHED OR NOT
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
                if (!flip) {
                    flip = true;
                    transform.localRotation = Quaternion.Euler(0,0,0);
                }
                MovePlayer(moveSpeed - crouchSpeed, 1f, 1);
                anim.SetBool("Run", true);
            } else if (Input.GetKey(KeyCode.D)) {
                if (!flip) {
                    flip = true;
                    transform.localRotation = Quaternion.Euler(0,0,0);
                }
                MovePlayer(moveSpeed, 1f, 1);
                anim.SetBool("Run", true);
            } 
            // LEFT MOVEMENT CROUCHED OR NOT
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
                if (flip) {
                    flip = false;
                    transform.localRotation = Quaternion.Euler(0,180,0);
                }
                MovePlayer(moveSpeed - crouchSpeed, 1f, 0);
                anim.SetBool("Run", true);
            } else if (Input.GetKey(KeyCode.A)) {
                if (flip) {
                    flip = false;
                    transform.localRotation = Quaternion.Euler(0,180,0);
                }
                MovePlayer(moveSpeed, 1f, 0);
                anim.SetBool("Run", true);
            } 

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
                anim.SetBool("Run", false);
                anim.SetTrigger("StopRun");
                if (isGroundedCheck()) {
                    animatorSpawner.SpawnAnimation("stopDust");
                }
            }
            // CHARGE JUMP / IS LESS THAN 0.2 SECONDS HOLD, NORMAL JUMP
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isGroundedCheck()) {
                chargeJump += Time.deltaTime;
            }
            // JUMP CODE
            if ((Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space)) && isGroundedCheck()) {
                anim.SetTrigger("Jump");
                audioManager.PlaySound("jump");
                animatorSpawner.SpawnAnimation("jumpDust");
                rb.drag = 0;
                if (chargeJump < 0.2f) {
                    rb.velocity = Vector2.up * jumpPower;
                } else if (chargeJump > 0.2f) {
                    rb.velocity = Vector2.up * (jumpPower + 5);
                }
                chargeJump = 0;
            }
            // RESET THE RECTANGLE ANGLE
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            // CROUCH
            if (Input.GetKey(KeyCode.S)) {
                // CROUCH
            } 
            // UNCROUCH
            if (Input.GetKeyUp(KeyCode.S)) {
                // FROM CROUCH TO IDLe
            }
        } else if (isSecondTypePlayer) {
            // SQUARE MOVEMENT
            //
            // RIGHT MOVEMENT
            /*if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
                MovePlayer(moveSpeed - crouchSpeed, 1f, 1);
            } else if (Input.GetKey(KeyCode.D)) {
                MovePlayer(moveSpeed, 1f, 1);
            }
            // LEFT MOVEMENT
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
                MovePlayer(moveSpeed - crouchSpeed, 1f, 0);
            } else if (Input.GetKey(KeyCode.A)) {
                MovePlayer(moveSpeed, 1f, 0);
            } */
        } else if (isThirdTypePlayer) {
            // CIRCLE MOVEMENT
        }
    }

    void FixedUpdate() {
        
    }

    private bool isGroundedCheck() {
        float extraHeightTest = .05f;
        RaycastHit2D rchit = Physics2D.BoxCast(cc.bounds.center, cc.bounds.size,  0f, Vector2.down, extraHeightTest, platformLayerMask);
        Debug.Log(rchit.collider != null);
        return rchit.collider != null;
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Coin") { // COIN COLLECTION
            audioManager.PlaySound("coin_collect");
            Vector2 loc = col.gameObject.transform.position;
            Instantiate(coin, loc, Quaternion.identity);
            Destroy(col.gameObject);
        }
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
            anim.SetTrigger("Damage");
            Invoke("setCanMove", 0.2f);
        }
        if (col.gameObject.tag == "Ground") {
            audioManager.PlaySound("landing");
            animatorSpawner.SpawnAnimation("fallDust");
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

    private void Flip() {
       
    }
}
