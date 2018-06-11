using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour {
	GameObject head;
	Rigidbody2D rb;

    GameObject zombieBody;
    GameObject animator;
	public float maxSpeed; //how fast we can move
	public float bounceFloat; //how high we bounce
	public float jumpFloat; //how high we jump
	public float moveFloat; //how far we move.
	public float inAirAdjustFloat; //how far we move in mid-air.
	public float maximumJumpTime; //experimental
	public float maxInAirAdjustFloat; //Get rid of?

	private bool b = false;
	private bool jumpAllowed = false;
	private bool bounceAllowed = false;
	private bool firstJump = true;
	private float nextJump = 0.0f;


	//variables to hold our start-condition values for head copying.
	private Vector2 savedHeadPos;
	private GameObject savedHead;
	private SpringJoint2D savedSpring;

	//variables to hold our start-condition values for head pickup
	//We need to first, pop the head back to its local position, and re-copy by hand each variable of the original spring.
	//Then, we need to tell the game that the head is reconnected.
	private SpringJoint2D newSpring;
	private Rigidbody2D newConnectedBody;
	private Vector2 newConnectedAnchor;
	private float newDistance;
	private Vector2 newPostion = new Vector2(.72f, 1.7f);
	private float newFrequency;
	private int springSpawn = 0;

    //Line renderer implementation
    LineRenderer lineDrawn;


    // Use this for initialization
    void Start () {

		head = gameObject;
        zombieBody = GameObject.Find("Zombie");
        if(zombieBody != null)
        {
            Debug.Log("Loaded Body");
        }

        rb = GetComponent<Rigidbody2D> ();
		SpringJoint2D spring = GetComponent<SpringJoint2D>();

		//This is insurance against a potential bug where when the scene reloads, it doesn't reset the head.
		jumpAllowed = false;
		bounceAllowed = false;
		firstJump = true;

		//save stuff needed for the head-tracers
		savedHead = head.gameObject;
		savedHeadPos = rb.position;
		savedSpring = GetComponent<SpringJoint2D>();

		//save stuff for our reconnecting spring below upon pickup
		newSpring = GetComponent<SpringJoint2D>();
		newConnectedBody = spring.connectedBody;
		newConnectedAnchor = spring.connectedAnchor;
		newDistance = spring.distance;
		newFrequency = spring.frequency;

        animator = GameObject.Find("ZombieAnim");
    }

	void FixedUpdate () {


        if (b == true) {
			rb.isKinematic = false;

			if (((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))))
            {
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    head.GetComponent<SpriteRenderer>().flipX = true;
                    if (!jumpAllowed) //we assume that if a jump is not allowed, than the player is in midair.
					{
						if (rb.velocity.x >= -1 * maxInAirAdjustFloat) //Capping logic line. 
						{
							rb.AddForce(new Vector2(inAirAdjustFloat * -1, 0), ForceMode2D.Force);
							Debug.Log("Inair push");
						}
					}


					else
					{

						if (bounceAllowed)
						{
							if (rb.velocity.x > -maxSpeed)
							{
								rb.AddForce(new Vector2(-moveFloat, bounceFloat), ForceMode2D.Impulse); //In air movements need fine tuning.
								Debug.Log("Firing MoveLeft: Vector: " + bounceFloat + "against applied force: " + rb.velocity.y);
								bounceAllowed = false;
							}

							else if (rb.velocity.x <= -maxSpeed)
							{
								rb.AddForce(new Vector2(0, bounceFloat), ForceMode2D.Impulse); //In air movements need fine tuning.
								Debug.Log("Firing MoveLeft: Vector: " + bounceFloat + "against applied force: " + rb.velocity.y);
								bounceAllowed = false;
							}
						}
					}
				}

				if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
				{
                    head.GetComponent<SpriteRenderer>().flipX = false;

                    if (!jumpAllowed) //we assume that if a jump is not allowed, than the player is in midair.
					{
						if (rb.velocity.x < maxInAirAdjustFloat)
						{
							rb.AddForce(new Vector2(inAirAdjustFloat, 0), ForceMode2D.Force);
                  
						}
					}

					else
					{

                        if (bounceAllowed)
                        {
                            if (rb.velocity.x < maxSpeed)
                            {
                                rb.AddForce(new Vector2(moveFloat, bounceFloat), ForceMode2D.Impulse); //In air movements need fine tuning.
                                Debug.Log("Firing MoveRight: Vector: " + bounceFloat + "against applied force: " + rb.velocity.y);
                                bounceAllowed = false;
                                //jumpAllowed = false;
                            }

                            else if (rb.velocity.x >= maxSpeed)
                            {
                                rb.AddForce(new Vector2(0, bounceFloat), ForceMode2D.Impulse); //In air movements need fine tuning.
                                Debug.Log("Firing MoveRight: Vector: " + bounceFloat + "against applied force: " + rb.velocity.y);
                                bounceAllowed = false;
                            }
                        }
					}
				}
			}

			if (Input.GetKey(KeyCode.Space) && jumpAllowed)
			{
				if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
				{
					Debug.Log("Firing dual key Combo!");
					rb.AddForce(new Vector2(0, jumpFloat - bounceFloat), ForceMode2D.Impulse);
					bounceAllowed = false;
					jumpAllowed = false;
					maximumJumpTime += .5f; //Not really needed

				}

				else
				{
					rb.AddForce(new Vector2(0, jumpFloat), ForceMode2D.Impulse);
					jumpAllowed = false;
					bounceAllowed = false;

				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if ((col.gameObject.tag == "Level" || col.gameObject.tag == "Beam" || col.gameObject.tag == "Lever"
            || col.gameObject.tag == "Mplat"||col.gameObject.tag == "Mplat2" || col.gameObject.tag == "Elevator") 
            && GameObject.Find("Head").tag == "Player")
		{
			b = true;
			jumpAllowed = true;
			bounceAllowed = true;
			rb.velocity = new Vector2(0, 0); //No more head sliding! :D

            //Create the conditions needed for the body to be touched by the head.
            //zombieBody.GetComponent<EdgeCollider2D>().enabled = true;
            //zombieBody.GetComponent<BoxCollider2D>().enabled = true;
            //zombieBody.GetComponent<CircleCollider2D>().enabled = true;
            zombieBody.GetComponent<CapsuleCollider2D>().enabled = true;
        }

        //Fake head logic below

		//Picking up the head code

		if(col.gameObject.tag == "Player" && gameObject.tag == "Player")//We are assuming that if two player objects collide, it is
			//body picking up the head time.
			//Also, we are create a condition where the head isn't picked up as soon as we launch it, damnit!
            //Additionally, this causes the head to be set as kinematic so it doesn't auto launch.
		{
            //reset head conditions to start once picked up.
            head.GetComponent<Rigidbody2D>().isKinematic = true;
            b = false;
            jumpAllowed = false;
            bounceAllowed = false;
            head.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY;

            //Emergency Case to add head to the transform of the zombie.
            head.transform.parent = zombieBody.transform;


            if (zombieBody.GetComponent<SpriteRenderer>().flipX)
            {
                rb.transform.localPosition = new Vector3(-.72f, 1.7f, 0);
                head.GetComponent<SpriteRenderer>().flipX = true;
            }

            else
            {
                rb.transform.localPosition = new Vector3(.72f, 1.7f, 0);
                head.GetComponent<SpriteRenderer>().flipX = false;
            }

			SpringJoint2D tempSpring = head.AddComponent<SpringJoint2D>();
			tempSpring.enabled = false; //Don't load this guy up.
			tempSpring.connectedAnchor = newConnectedAnchor;
			tempSpring.connectedBody = newConnectedBody;
			tempSpring.autoConfigureConnectedAnchor = false;
			tempSpring.autoConfigureDistance = false;
			tempSpring.distance = newDistance;
			tempSpring.frequency = newFrequency;

			rb.isKinematic = true; //Stop our head from moving.
			zombieBody.GetComponent<BodyMovement>().SetConnectedHead(true); //The head has returned to its rightful place.
			b = false;
            animator.SetActive(true);
            //rb.velocity.Set(0, 0); //stop sliding
            //zombieBody.GetComponent<Rigidbody2D>().velocity.Set(0, 0);
        }
    }
}
