﻿using UnityEngine;

public class TensileLabConfigurator : MonoBehaviour
{
    private void Start()
    {
        var lever = FindObjectOfType<LeverPressed>();
        var lever_trigger_events = lever?.GetComponent<TriggerEvents>();
        if (lever_trigger_events)
            lever_trigger_events.AddTriggerEnterHandler((other) =>
            {
                if (other.gameObject.tag == "leftHand" || other.gameObject.tag == "rightHand")
                {
                    GameObject graph = Instantiate(Resources.Load("LineGraph", typeof(GameObject))) as GameObject;
                    Debug.Log("Instantiated LineGraph");
                    lever.OnPress();
                }
            });
        else
            Debug.LogError("Could not find lever trigger events component");

        var tester = FindObjectOfType<InstronTester>();
        var tester_collision_events = tester?.GetComponent<CollisionEvents>();
        if (tester_collision_events)
            tester_collision_events.AddCollisionEnterHandler((collision) =>
            {
                if (collision.gameObject.tag == "material")
                {
                    // Assign the tester's subject to the other collider's GameObject
                    tester.Subject = collision.gameObject;
                }
            });
        else
            Debug.LogError("Could not find tensile tester collision events component");
    }
}