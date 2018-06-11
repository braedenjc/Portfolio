
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskList : MonoBehaviour {


    /// <summary>
    /// This class implements a TaskList. This class is backed by an GameObject[] for speed due to the
    /// nature of the methods inside the class.
    /// 
    /// This class should provide a mechanism of taking a look at five random objects, and then enable the taskable script.
    /// As a start, we disable ALL Taskables at first, and the perform the quick randomization.
    /// </summary>
    /// 
    public Text taskListText;
    public List<Taskable> allTasks = new List<Taskable>();
    public List<Taskable> daysTasks = new List<Taskable>();

    // Added by Sergio to work with newDay trigger form task completion; feel free to change after Alpha
    public int numTasks;
    //public int numActive;

	public void Start()
    {
        GameObject taskableContainer = GameObject.Find("Taskable"); //Loads the Taskable gameobject from the scene.
        foreach(Transform t in taskableContainer.GetComponentInChildren<Transform>())
        {
            allTasks.Add(t.GetComponent<Taskable>());
        }

        // Add the special taskable (calling the counselor) - change chair for phone once we get a phone
        allTasks.Add(GameObject.Find("chair").GetComponent<Taskable>());

        newTasks(0);
        FillList();
    }

    public void Update() //TODO: THIS NEEDS TO BE REMOVED FOR PERFORMANCE // REMOVE AFTER ALPHA
    {
        FillList();
    }

    /// <summary>
    /// This method selects the five new tasks for the day.
    /// </summary>
    public void newTasks(int confidence)
    {
        //Our value for determining the the number of tasks Ollie can handle.
        int upperRange = confidence >= 95 ? allTasks.Count : allTasks.Count - 1; 

        daysTasks.Clear(); //We empty out on a new day.
        foreach (Taskable task in allTasks)
        { 
            task.active = false;  // Rather than disabling the entire component, simply flag it as not active. -Sergio
        }

        //numActive = 0;
        for (int i = 0; i < numTasks; i++)
        {
            //This code selects the days tasks. We pick a number between 0 and day's tasks.
            int random = new System.Random().Next(0, upperRange);

            //And the we use that number to pick a task.
            Taskable task = allTasks[random].GetComponent<Taskable>();

            //We check to see if we already have the task. If so, we pick a random new one.
            while (daysTasks.Contains(task)) 
            {
                random = new System.Random().Next(0, upperRange);
                task = allTasks[random].GetComponent<Taskable>();
            }

            task.active = true; //// Rather than re-enabling the entire component (never disable the component), simply flag it as active. -Sergio
           //numActive++; // Increase count of active tasks
            daysTasks.Add(task); //We add it to our day tasks lists - This allows for tracking and adding the names to an onscreen displayed list.
        }
    }

    /// <summary>
    /// This method populates the Canvas-TaskList
    /// </summary>
    public void FillList()
    {
        taskListText.text = ""; //clear on repopulation
        foreach(Taskable task in daysTasks)
        {
            if(task.active)
            {
                taskListText.text += (task.taskName + "\n");
            }
        }
    }
    
    /// <summary>
    /// This method removes a task from the activeList after a maze is completed.
    /// Triggers a new day if all tasks were completed.
    /// </summary>
    /// <param name="task">The Task we need to disable</param>
    public void RemoveTask(Taskable task)
    {
        task.active = false;

        //// Added by Sergio for Alpha; feel free to update with cleaner fix after Alpha
        // Note: Added this way to work with current implementation. I tried to minimize changes to the rest of the script.

        // Decrease count of active tasks
        daysTasks.Remove(task);

        // If there are no more active tasks, trigger a new day
        if(daysTasks.Count <= 0)
        {
            GameObject.Find("Ollie_Sprite").GetComponent<DayManager>().newDay(true);
        }
    }


}
