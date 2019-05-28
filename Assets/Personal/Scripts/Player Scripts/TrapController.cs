using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    // Start is called before the first frame update

	float trapTime = 20;
	float trapTimer;
	bool trapOn = false;
	float trapDuration = 10;
	float damage = .1f;
	float trapWarmup = 15;
	float accumulatedDamage;
	bool playerContact = false;
	[SerializeField] GameObject player;


    void Start()
    {
        StartCoroutine(DamageOverTime());
    }

    // Update is called once per frame
    void Update()
    {
        trapTimer += Time.deltaTime;
		if(trapTimer >= trapTime & !trapOn) {
			TriggerTrap();
			trapTimer = 0;
		}
		else if(trapOn & trapTimer >= trapDuration) {
		
			TriggerTrap();
			trapTimer = 0;
		}
		else if(!trapOn & trapTimer >= trapWarmup) {
			TriggerWarmup();
		}
    }

	private void TriggerTrap() {
		if(!trapOn) {
			gameObject.GetComponent<Renderer> ().material.color = Color.red;
			trapOn = true;
		

		}
		else {
			gameObject.GetComponent<Renderer> ().material.color = Color.grey;
			trapOn = false;
		}
	
	}
	private void TriggerWarmup() {
		if(!trapOn){
			gameObject.GetComponent<Renderer>().material.color = Color.yellow;
		}
	}
	/*
	private void OnTriggerEnter(Collider col) {
		Debug.Log("Boop");
		if( col.gameObject.tag == "Player" ) {
			if(trapOn) {
			col.gameObject.GetComponent<PlayerHealth>().TakeDamage( 5,  new Vector3(0, -1, 0) , 0);
			}
		}
	} */
	private void OnTriggerStay(Collider col) {
		if(col.gameObject.tag == "Player" & trapOn) {
			
			playerContact = true;	
			accumulatedDamage += damage;
			Debug.Log("stay");
			}
				
				//col.gameObject.GetComponent<PlayerHealth>().TakeDamage( 1, new Vector3(0 , -1, 0), 0);
		
	
	}

	private void OnTriggerExit(Collider col) {
		if(col.gameObject.tag == "Player") {
			playerContact = false;
			Debug.Log("stay");
		}

	}

	  IEnumerator DamageOverTime()
    {
        while (true)
        {
            if (playerContact & trapOn) 
            {
				Debug.Log("running");
                if (accumulatedDamage < 1f) {
				accumulatedDamage = 1f;
				}
                player.GetComponent<PlayerHealth>().TakeDamage(Mathf.FloorToInt(accumulatedDamage), new Vector3(0, -1, 0), 0);
                accumulatedDamage = 0f;
            }

            yield return new WaitForSeconds(.5f);
        }
       
    }



}
