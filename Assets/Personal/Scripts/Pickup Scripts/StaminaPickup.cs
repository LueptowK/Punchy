using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPickup : PickupController
{
    [SerializeField] int StaminaAmount = 100;


    public override void takeDamage()
    {

        pickupSpawner.gameObject.GetComponent<PickupSpawner>().PickedUp(gameObject);
        pickupSpawner.gameObject.GetComponent<PlayerStamina>().RegainStaminaWithoutRegen(StaminaAmount);
        Destroy(gameObject);
    }

}
