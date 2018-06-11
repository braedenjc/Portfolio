using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Narrate;

/// <summary>
/// Code By Braeden
/// 
/// As of this moment, this script DISABLES movement until the bars are filled.
/// Integer will determine the wakeup amount of the bar. Once the bar hits one hundred, we are good to go.
/// The fill rate is determined by a depression equation.
/// </summary>
public class WakeupMinigame : MonoBehaviour {

    public float awakeAmount; //This value is referenced by the slider. It needs to be reset at each new day.
    public float fillMultipler; //To be determined, this value controls how fast our bar fills. For now, it is a one.
    public float fillAmount; //This is a base level fill amount to be determined by play testing.
    public float decaySpeed; //This is how we determine how much this decrease.
    public CanvasGroup display; //This is how we control the fade out.
    public float countDown; //This is our FIGBTB countdown timer.
    public bool woken; //This is the control boolean for our animation.
    public Transform size;
    Animator animator; //This is the placeholder for our animation access. --THIS NEEDS TO BE BETTER.
	SubtitleManager subManager;

    // Use this for initialization
    void Start () {
        display = GameObject.Find("Canvas-Wakeup").GetComponent<CanvasGroup>();
        //StartWakeup();

		subManager = GameObject.Find("SubtitleManager").GetComponent<SubtitleManager>();
    }
	
	// Update is called once per frame
    /// <summary>
    /// This Update method checks to make sure we aren't awake yet. Once we are, we disable this script, and enable Ollie's ability to move.
    /// </summary>
	void Update () {
        gameObject.GetComponent<Slider>().value = awakeAmount;
		if(awakeAmount >= 100 && display.alpha > 0)
        {
            
            GameObject.Find("Ollie_Sprite").GetComponent<Ollie_Move>().enabled = true;
            display.alpha -= Time.deltaTime;
            animator.SetTrigger("Wakeup");
            animator.Play("Wakeup");
            animator.StartPlayback();
            animator.speed = 1;

			subManager.DisplaySubtitle ("Ollie: I guess I can try today..", 3);
            
        }

        if (awakeAmount > 0 && awakeAmount < 100)
        {
            awakeAmount -= (Time.deltaTime) * decaySpeed;
            gameObject.GetComponent<Slider>().value = awakeAmount; //Update the parent slider.
        }

        if (awakeAmount < 100)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                awakeAmount += (fillAmount * fillMultipler);
                gameObject.GetComponent<Slider>().value = awakeAmount; //Update the parent slider.

            }
        }
	}

    /// <summary>
    /// This method starts the wakeup minigame.
    /// </summary>
    public void StartWakeup(float decaySpeed)
    {
        // Set the decay speed to the values specified
        this.decaySpeed = decaySpeed;

        awakeAmount = 0;
        display.alpha = 100;
        GameObject.Find("Ollie_Sprite").GetComponent<Ollie_Move>().enabled = false; //we can't move until we wakeup.
        display = GameObject.Find("Canvas-Wakeup").GetComponent<CanvasGroup>();
        animator = GameObject.Find("Ollie_Sprite").GetComponent<Animator>();
        //since we start each day with Ollie in bed, we set out animation to speed zero, and set the animation false;
        animator.Play("Wakeup"); //Load the wakeup animation state.
        animator.speed = 0; //We pause the playback engine --THIS IS A HACK THERE IS A BETTER WAY TO DO THIS.
    }
}
