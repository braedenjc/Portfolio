using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileDragging : MonoBehaviour
{
    public float maxStretch = 3.0f;
    public LineRenderer catapultLineFront;
    public LineRenderer catapultLineBack;
    public float copyRate; //How fast we are spawning heads.
    public float terminationRate; //The amount of time it will take to delete a copy of a head after it collides
    public Rigidbody2D head;
	public float velocityMultiplier;

    private SpringJoint2D spring;
    private Transform catapult;
    private Ray rayToMouse;
    private Ray leftCatapultToProjectile;
    private float maxStretchSqr;
    private float circleRadius;
    private bool clickedOn;
    private Vector2 prevVelocity;
    private int cnt = 0;

    private GameObject zombieBody;
    private GameObject headCopy;
    private float fakeHeadLaunch;
    private bool copyHeadCreated = false;
    private float terminationCounter;

    CircleCollider2D circle;

    //Line rendering stuff
    LineRenderer lineDrawn;
    Vector2 lineVelocity;

    void Awake()
    {
        spring = GetComponent<SpringJoint2D>();
        catapult = spring.connectedBody.transform;
        fakeHeadLaunch = 0.0f;
        terminationCounter = 0.0f;
    }

    void Start()
    {
        LineRendererSetup();
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        leftCatapultToProjectile = new Ray(catapultLineFront.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
        circle = GetComponent<CircleCollider2D>();
        circleRadius = circle.radius;
        zombieBody = GameObject.Find("Zombie");

        if(zombieBody != null)
        {
            Debug.Log("Loaded zombie into memory");
        }

        lineDrawn = GetComponent<LineRenderer>();
        lineDrawn.numPositions = 20;
        lineDrawn.enabled = false; //We have nothing to draw yet, so don't show anything.
        
    }

    void Update()
    {
        rayToMouse = new Ray(catapult.position, Vector3.zero);
        leftCatapultToProjectile = new Ray(catapultLineFront.transform.position, Vector3.zero);
        maxStretchSqr = maxStretch * maxStretch;
        circle = GetComponent<CircleCollider2D>();
        circleRadius = circle.radius;
        spring = GetComponent<SpringJoint2D>();
        
        if (clickedOn && head.isKinematic == true)
        {
            Dragging();
        }

        if (spring != null)
        {
            if (!GetComponent<Rigidbody2D>().isKinematic && prevVelocity.sqrMagnitude > GetComponent<Rigidbody2D>().velocity.sqrMagnitude)
            {
                Destroy(spring);
				GetComponent<Rigidbody2D>().velocity = prevVelocity * velocityMultiplier;
            }

            if (!clickedOn)
                prevVelocity = GetComponent<Rigidbody2D>().velocity;

            LineRendererUpdate();

        }
        else
        {
            catapultLineFront.enabled = false;
            catapultLineBack.enabled = false;
        }
    }

    void LineRendererSetup()
    {
        catapultLineFront.SetPosition(0, catapultLineFront.transform.position);
        catapultLineBack.SetPosition(0, catapultLineBack.transform.position);

        catapultLineFront.sortingLayerName = "Foreground";
        catapultLineBack.sortingLayerName = "Foreground";

        catapultLineFront.sortingOrder = 3;
        catapultLineBack.sortingOrder = 1;
    }

    void OnMouseDown()
    {
        spring.enabled = true;
        clickedOn = true;
        zombieBody.GetComponent<BodyMovement>().SetConnectedHead(false); //Cause the body to stop moving.
        //zombieBody.GetComponent<EdgeCollider2D>().enabled = false; //Make it so that the heads launching from body do not touch.
        //zombieBody.GetComponent<BoxCollider2D>().enabled = false;
        //zombieBody.GetComponent<CircleCollider2D>().enabled = false;
        zombieBody.GetComponent<CapsuleCollider2D>().enabled = false;
        GameObject animator = GameObject.Find("ZombieAnim");
        animator.SetActive(false);
        zombieBody.GetComponent<SpriteRenderer>().enabled = true;
        head.GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnMouseUp()
    {
        //spring.enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        clickedOn = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        
        foreach (GameObject headTrash in GameObject.FindGameObjectsWithTag("FakeHead"))
        {
            Destroy(headTrash);
        }
        copyHeadCreated = false;
        
    }

    void Dragging()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 catapultToMouse = mouseWorldPoint - catapult.position;
     
        if (catapultToMouse.sqrMagnitude > maxStretchSqr)
        {
            rayToMouse.direction = catapultToMouse;
            mouseWorldPoint = rayToMouse.GetPoint(maxStretch);
        }

        mouseWorldPoint.z = 0f;
        transform.position = mouseWorldPoint;    
    }

    void LineRendererUpdate()
    {
        Vector2 catapultToProjectile = transform.position - catapultLineFront.transform.position;
        leftCatapultToProjectile.direction = catapultToProjectile;
        Vector3 holdPoint = leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude + circleRadius);
        catapultLineFront.SetPosition(1, holdPoint);
        catapultLineBack.SetPosition(1, holdPoint);
    }

    void SetCopyHeadCreatedBool(bool isMade)
    {
        copyHeadCreated = isMade;
    }
}
