using UnityEngine;
using System.Collections;

public class BodyMovement : MonoBehaviour {
    public GameObject body; //This gameobject. This is a bit of a misnomer, but this is a good idea just from a understanding method
    Rigidbody2D rb; //THIS object's rigidbody.
    GameObject head;
    GameObject zombieAnim;

    private GameObject girlFriend;

    public float movementSpeed;
    public float jumpSpeed;
    public float maxMoveSpeed;
    public float slowDownRate;

    public bool isHeadConnected;

    // Use this for initialization
    void Start () {
        body = gameObject;
        rb = body.GetComponent<Rigidbody2D>();
        isHeadConnected = true; //the head always starts connected!
        head = GameObject.Find("Head");
        if(head != null)
        {
            Debug.Log("Loaded Head");
        }
        zombieAnim = GameObject.Find("ZombieAnim");

        if (zombieAnim != null)
        {
            Debug.Log("Animation Object Loaded");
            zombieAnim.GetComponent<Rigidbody2D>().transform.localPosition = new Vector3(0, 0);
        }

        girlFriend = GameObject.Find("Girl_Zombie");
        
        if(girlFriend != null)
        {
            Debug.Log("Girlfriend loaded fine. Go get her, tiger.");
        }        
    }
	
	// Update is called once per frame
	void Update () { //We will enforce a state here for the body.

        zombieAnim.GetComponent<Rigidbody2D>().transform.position = 
            new Vector3(body.transform.position.x, body.transform.position.y + .54f);//Keep animation object with our body

       
        //Rotating the body should NEVER be set. However, since the state is an ENUM, we have to specifically set the state.
        if (isHeadConnected) //if the head is connected, the body should NOT be kinematic.
        {
            body.GetComponent<Rigidbody2D>().isKinematic = false;
            girlFriend.GetComponent<BoxCollider2D>().enabled = true;
            head.GetComponent<Rigidbody2D>().Sleep();  //This makes it so the rigid bodies don't mess with each other.
        }

        if (!isHeadConnected) //This should stop the body from moving.
        {
            body.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            body.GetComponent<Rigidbody2D>().isKinematic = true;
            girlFriend.GetComponent<BoxCollider2D>().enabled = false;
            head.GetComponent<Rigidbody2D>().WakeUp(); //Make it so that the rigidbody kicks in so it can fall.
        }        

    }

    void FixedUpdate()
    {
        float x = body.transform.position.x;
        float y = body.transform.position.y;

        //Slow down our shit.
        if (rb.velocity.x < 0)
        {
            rb.velocity += new Vector2(slowDownRate, 0);
        }

        if (rb.velocity.x > 0)
        {
            rb.velocity -= new Vector2(slowDownRate, 0);
        }

        if (isHeadConnected)
        {
            if (((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))))
            {
                zombieAnim.GetComponent<Animator>().SetTrigger("WalkPress");
                zombieAnim.GetComponent<SpriteRenderer>().enabled = true;
                body.GetComponent<SpriteRenderer>().enabled = false;
                head.GetComponent<SpriteRenderer>().enabled = false;
                if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
                {
                    if (!body.GetComponent<SpriteRenderer>().flipX)
                    {
                        body.GetComponent<SpriteRenderer>().flipX = true;
                        head.GetComponent<SpriteRenderer>().flipX = true;
                        head.GetComponent<Rigidbody2D>().transform.localPosition = new Vector3(-.72f, 1.7f, 0);
                        zombieAnim.GetComponent<SpriteRenderer>().flipX = true;
                    }

                    if (rb.velocity.x >= (-1 * maxMoveSpeed))
                    {
                        rb.AddForce(new Vector2((-1 * movementSpeed),0), ForceMode2D.Impulse);
                    }                    
                }
                
                if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
                {
                    if (body.GetComponent<SpriteRenderer>().flipX)
                    {
                        body.GetComponent<SpriteRenderer>().flipX = false;
                        head.GetComponent<SpriteRenderer>().flipX = false;
                        head.GetComponent<Rigidbody2D>().transform.localPosition = new Vector3(.72f, 1.7f, 0);
                        zombieAnim.GetComponent<SpriteRenderer>().flipX = false;
                    }
                
                    if (rb.velocity.x <= maxMoveSpeed)
                    {

                        rb.AddForce(new Vector2(movementSpeed, 0), ForceMode2D.Impulse);
                    }
                }
            }

            if(((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) || ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)))) || rb.velocity.x == 0)
            {
                rb.velocity = new Vector2(0, 0);
                rb.velocity.Set(0, 0); //Stop the body from sliding.
                Debug.Log("Stopping Animation");
                zombieAnim.GetComponent<Animator>().SetTrigger("WalkStop");
                zombieAnim.GetComponent<Animator>().ResetTrigger("WalkPress");
                zombieAnim.GetComponent<SpriteRenderer>().enabled = false;
                body.GetComponent<SpriteRenderer>().enabled = true;
                head.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    //We will need this method in order to tell the body that it is okay to move, since the body is now attached.
    public void SetConnectedHead(bool connectedNow)
    {
        isHeadConnected = connectedNow;
    }
}
