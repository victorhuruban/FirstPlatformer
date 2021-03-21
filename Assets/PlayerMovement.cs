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
    bool jumped;
    bool jump;
    bool justFalled;
    bool canMove;
    bool isRectangle;
    bool isSquare;
    bool isCircle;
    float chargeJump;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();
        jumped = false;
        jump = false;
        canMove = true;
        isGrounded = true;

        isRectangle = true;
        isSquare = false;
        isCircle = false;
    }

    void Update() {
        if (canMove) {
            if (Input.GetKeyDown(KeyCode.Q)) {
                // Transform in Square
                //isRectangle = false;
                //isCircle = false;
                //isSquare = true;
                anim.SetTrigger("Transform_To_Square_From_Rectangle");
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                // Transform in Circle
                //isRectangle = false;
                //isCircle = true;
                //isSquare = false;
                anim.SetTrigger("Transform_To_Circle_From_Rectangle");
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                // Transform in Rectangle
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
        if (isRectangle) {
            // RECTANGLE MOVEMENT
            //
            // RIGHT MOVEMENT CROUCHED OR NOT
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) {
                MoveSprintRectangle(moveSpeed - crouchSpeed, 1f, 1);
                ResetAfterMovementRectangle();
            } else if (Input.GetKey(KeyCode.D)) {
                MoveSprintRectangle(moveSpeed, 1f, 1);
                RotatePlayerRectangle(false);
            } 
            // LEFT MOVEMENT CROUCHED OR NOT
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) {
                MoveSprintRectangle(moveSpeed - crouchSpeed, 1f, 0);
                ResetAfterMovementRectangle();
            } else if (Input.GetKey(KeyCode.A)) {
                MoveSprintRectangle(moveSpeed, 1f, 0);
                RotatePlayerRectangle(true);
            } 
            // CHARGE JUMP / IS LESS THAN 0.2 SECONDS HOLD, NORMAL JUMP
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isGroundedCheck()) {
                chargeJump += Time.deltaTime;
            }
            // JUMP CODE
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
            // RESET THE RECTANGLE ANGLE
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) {
                ResetAfterMovementRectangle();
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            // CROUCH
            if (Input.GetKey(KeyCode.S)) {
                anim.SetBool("Crouch", true);
            } 
            // UNCROUCH
            if (Input.GetKeyUp(KeyCode.S)) {
                anim.SetBool("Crouch", false);
            }
        } else if (isSquare) {
            // SQUARE MOVEMENT
        } else if (isCircle) {
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
            ResetAfterMovementRectangle();
            if (Input.GetKey(KeyCode.S)) {
                anim.SetBool("Crouch", false);
            }
            audioManager.PlaySound("damage_" + (int)Random.Range(1, 5));
            Vector2 playerPos = transform.position;
            Vector2 dir = col.GetContact(0).point - playerPos;

            rb.velocity = new Vector2(0,0);
            rb.inertia = 0;

            rb.AddForce(-dir.normalized * knockBackForce, ForceMode2D.Impulse);
            anim.SetTrigger("Damage");
            Invoke("setCanMove", 0.2f);
        }
    }

    private void MoveSprintRectangle(float moveSpeed, float sprintSpeed, int direction) {
        if (isGrounded) {
            rb.drag = 0;
        }
        if (direction == 0) {
            rb.velocity = new Vector2(-1 * moveSpeed * sprintSpeed, rb.velocity.y);
        } else rb.velocity = new Vector2(1 * moveSpeed * sprintSpeed, rb.velocity.y);
    }

    private void ResetAfterMovementRectangle() {
        if (isGroundedCheck()) {
            rb.drag = 10;
        }
        trans.rotation = Quaternion.identity;
        rotated = false;
        sprinting = false;
        
    }

    private void setCanMove() {
        canMove = !canMove;
    }

    private void RotatePlayerRectangle(bool left) {
        if (left) {
            if (!rotated) {
                rotated = true;
                trans.Rotate(new Vector3(0,0,8f));
            }
        } else {
            if (!rotated) {
                rotated = true;
                trans.Rotate(new Vector3(0,0,-8f));
            }
        }
    }
    private void RotatePlayerSquare(bool left, bool sprint) {
        if (!sprint) {
            if (left) {
                if (!rotated) {
                    rotated = true;
                    trans.Rotate(new Vector3(0,0,8f));
                }
                if (sprinting) {
                    sprinting = false;
                    trans.Rotate(new Vector3(0,0,-12f));
                }
            } else {
                if (!rotated) {
                    rotated = true;
                    trans.Rotate(new Vector3(0,0,-8f));
                }
                if (sprinting) {
                    sprinting = false;
                    trans.Rotate(new Vector3(0,0,12f));
                }
            }
        } else {
            if (!left) {
                if (!sprinting) {
                    sprinting = true;
                    trans.Rotate(new Vector3(0,0,-12f));
                }
            } else {
               if (!sprinting) {
                    sprinting = true;
                    trans.Rotate(new Vector3(0,0,12f));
                } 
            }
        }
    }
}
