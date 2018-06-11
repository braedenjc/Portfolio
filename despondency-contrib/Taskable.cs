using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskable : MonoBehaviour {

    /// <summary>
    /// Code by Braeden Christensen
    /// This class implements the slave taskable object to each object.
    /// This taskable object accesses values from the resource manager in order to
    /// decide the difficult of a task.
    /// This is now an all in one solution, taking over from TaskManager
    /// </summary>

    
    public string taskName;
    //public string description;
    public int difficulty;
    public bool active;
    public GameObject highlight;

    private Narrate.ProximityNarrationTrigger caption;
   

    private ResourceManager resource; //used to access values such as depression.
    private ActiveTask task; 
    
    public void Start()
    {
        resource = GameObject.Find("Canvas-Resources").GetComponent<ResourceManager>(); //Load in the resource manager from our Scene in Unity. 
        task = GameObject.Find("Canvas-ActiveTask").GetComponent<ActiveTask>();

        caption = transform.GetComponent<Narrate.ProximityNarrationTrigger>();
    }

    /// <summary>
    /// Ensures narrative captions are enabled only when the task is active
    /// Added by Sergio
    /// </summary>
    void Update()
    {
        if (active && !task.anyActive)
        {
            if (caption != null)
            {
                caption.enabled = true;
            }
            highlight.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            if (caption != null)
            {
                caption.enabled = false;
            }
            highlight.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    /// <summary>
    /// This method handles the main character getting to a task.
    /// </summary>
    /// <param name="col"></param>
    public void OnCollisionStay2D(Collision2D col)
    {
        if (active && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)))
        {
            int mazeDifficulty = difficulty + (resource.getDep() / 30); //TODO: clean up, make it work with Sergio 
                                                                        ////-- converted "completed" to "active" and used it to check that only active taskables trigger tasks. -Sergio  

            task.TriggerTask
                (
                    this
                );

        }
    }

}
