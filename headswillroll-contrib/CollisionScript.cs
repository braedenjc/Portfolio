using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollisionScript : MonoBehaviour {

	// Use this for initialization

	public Rigidbody2D head;
	public Rigidbody2D head1;

	private bool isGameOver; 
	private bool isGameOver2;
	bool isBool = false;
	private float timerTime = 0.0f;
	private bool isTrue = false;
	private float timer = 0.0f;
	float DeathTimer = 0.0f;
	public bool isDead = false;
	public int deathCounter = 0;
	int cnt = 0;
	private bool elevatorEnabled = false;
	GameObject[] Candies;
	GameObject[] Levers;
    GameObject animator;
    GameObject headAnimator;
    GameObject body;
    GameObject headMain;
	public Rigidbody2D player;
	public LevelManage LM;
	private GUIStyle guiStyle = new GUIStyle(); 

	void Start () {
		DeathTimer = 0.0f;	
		isDead = false;
		GameObject.FindGameObjectWithTag ("Dead").GetComponent<SpriteRenderer>().enabled = false;
		LM = FindObjectOfType<LevelManage> ();
        animator = GameObject.Find("ZombieAnim");
        if(animator != null)
        {
            Debug.Log("Loaded Animator Body");
        }

        headAnimator = GameObject.Find("HeadDead");
        headMain = GameObject.Find("Head");
        body = GameObject.Find("Zombie");

    }
	void Awake()
	{

	}
	
	// Update is called once per frame
	void Update () {	
		if (isTrue == true) {
			Application.LoadLevel (Application.loadedLevel);
		}			
		Candies = GameObject.FindGameObjectsWithTag ("Candy");
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Candy" && !(gameObject.tag == "FakeHead")) {
			Destroy (col.gameObject);

		} else if (col.gameObject.tag == "Finish" && Candies.Length == 0 
            && !(gameObject.tag == "FakeHead" && GameObject.Find("Zombie").GetComponent<BodyMovement>().isHeadConnected)) 
            {
            GameObject zombie = GameObject.Find("Zombie");
            isGameOver = true;
            zombie.GetComponent<BodyMovement>().enabled = false;
            animator.SetActive(false);
            body.GetComponent<SpriteRenderer>().enabled = true;
            head.GetComponent<SpriteRenderer>().enabled = true;

        } else if ((col.gameObject.tag == "Hazard") && !(gameObject.tag == "FakeHead")) {
            
            //Zombie logic
            GameObject zombie = GameObject.Find("Zombie");
            zombie.GetComponent<SpriteRenderer>().enabled = false;
            zombie.GetComponent<Rigidbody2D>().isKinematic = true; //Stops the body from moving down, and being moveable.
            //zombie.GetComponent<BoxCollider2D>().enabled = false;
            //zombie.GetComponent<CircleCollider2D>().enabled = false;
            zombie.GetComponent<CapsuleCollider2D>().enabled = false;
            zombie.GetComponent<BodyMovement>().enabled = false;
            animator.SetActive(false);
            headAnimator.GetComponent<SpriteRenderer>().enabled = true;
            headAnimator.GetComponent<Animator>().SetTrigger("Explode");
            

            //Head logic
            GameObject head = GameObject.Find("Head");
            head.GetComponent<SpriteRenderer>().enabled = false;
            head.GetComponent<CircleCollider2D>().enabled = false;
            head.GetComponent<CapsuleCollider2D>().enabled = false; 
            head.GetComponent<Rigidbody2D>().isKinematic = true;
            head.GetComponent<BallMovement>().enabled = false;

            //DeadHead copy
            GameObject headDead = GameObject.Find("HeadDead");
            headDead.GetComponent<SpriteRenderer>().enabled = true;
            headDead.GetComponent<CircleCollider2D>().enabled = true;
            headDead.GetComponent<Rigidbody2D>().isKinematic = false;
            headDead.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;

            //A final bit of control, we stop animation from running.
            animator.SetActive(false);


			isDead = true;
			StartCoroutine (Wait ());
		}

        else if (gameObject.name == "HeadDead" && col.gameObject.tag == "Hazard")
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            
        }

		//TO DO: End game script with body
		else if ((col.gameObject.tag == "Body"))
        {
			Physics2D.IgnoreCollision (head1.GetComponent<CapsuleCollider2D> (), GameObject.FindGameObjectWithTag ("Body").GetComponent<BoxCollider2D> ());
		}

	}

    void OnCollisionStay2D(Collision2D col)
    {
        if (gameObject.name == "Head" && (col.gameObject.tag == "Mplat" || col.gameObject.tag == "Mplat2")) //When the head hits a moving platform, it attaches to the platform.
        {
            transform.parent = col.gameObject.transform;
        }

        else if (gameObject.name == "Head")
        {
            transform.parent = GameObject.Find("Zombie").transform; //reconnect the parent pointer back to the zombie body.
        }

        else if (gameObject.name == "Zombie" && ((col.gameObject.tag == "Mplat") || (col.gameObject.tag == "Mplat2")))
        { 
            transform.parent = col.gameObject.transform;
        }

        else if (gameObject.name == "HeadDead" && col.gameObject.tag == "Hazard")
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }

        else
        {
            transform.parent = null; //release the body from the transform of the moving platform.
        }
    }
   

	void OnGUI()
	{
		if (isGameOver == true&& timer>0.0f && timer<=2.0f) {
			Debug.Log ("Level Complete");	
		}
		if (isBool == true) {
			//Application.LoadLevel (Application.loadedLevel + 1);
		}


	}
	void OnTriggerEnter2D(Collider2D col) { 
	
		if (col.gameObject.tag == "EndGame")
        {
			isGameOver2 = true;
            
		}
        else if ((col.gameObject.tag == "Lever") || col.gameObject.tag == "Lever2") {
			//If there is a movable elevator, we enable it here! Be wary of multiple ones though
			Destroy (GameObject.FindGameObjectWithTag ("Boulder"));
            col.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            col.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
		}
			else if(col.gameObject.tag == "DeadZone")
			{
				LM.RespawnPlayer();
			}

	}
	IEnumerator Wait()
	{
		yield return new WaitForSeconds (2); //We need to reduce the amount of time it takes to iterate from failure
		isTrue = true;
	}
}