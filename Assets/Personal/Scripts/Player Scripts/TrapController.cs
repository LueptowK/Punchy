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
	float trapWarmup = 15;


    void Start()
    {
        
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
			Debug.Log(trapTimer);
		}
    }

	private void TriggerTrap() {
		if(!trapOn) {
			gameObject.GetComponent<Renderer> ().material.color = Color.red;
			trapOn = true;
			Debug.Log(trapTimer);

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
		Debug.Log("Boop");
		if(col.gameObject.tag == "Player" ) {
			if(trapOn) {
				col.gameObject.GetComponent<PlayerHealth>().TakeDamage( 5, new Vector3(0 , -1, 0), 0);
			}	
		}
	}
}
