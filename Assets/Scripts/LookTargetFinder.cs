using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LookTargetFinder : MonoBehaviour
{
    [SerializeField]
    private string tagToFind;
    [SerializeField]
    private float radius;
    [SerializeField]
    private bool looking = true;
    [SerializeField]
    private GameObject defaultTarget;
    [SerializeField]
    private float interpolationSpeed = 0.2f;

    private void Update()
    {
        // If we're not actively looking for targets, set our position to the default target and return
        if (!looking)
        {
            UpdateLookTargetPosition(defaultTarget.transform.position);
            return;
        }

        // Find all game objects within the specified radius around the default target position
        Collider[] colliders = Physics.OverlapSphere(defaultTarget.transform.position, radius);

        // Loop through all the colliders we found
        foreach (Collider collider in colliders)
        {
            // If we find a collider with the desired tag, set our position to that collider's position
            if (collider.gameObject.tag == tagToFind)
            {
                Debug.Log("Collider Found to look at with tag " + tagToFind);

                // Update our position to smoothly move towards the new target
                UpdateLookTargetPosition(collider.transform.position);

                // Return so we only update our position to the first valid target we find
                return;
            }
        }

        // If we didn't find any valid targets, set our position to the default target
        UpdateLookTargetPosition(defaultTarget.transform.position);
    }

    // Smoothly updates our position to move towards the given target position
    private void UpdateLookTargetPosition(Vector3 target)
    {
        transform.position = Vector3.Lerp(transform.position, target, interpolationSpeed);
    }

    // Toggles whether or not this script should be actively looking for targets
    public void ToggleLooking()
    {
        looking = !looking;
    }

    // For presentation purposes
    private void OnDrawGizmos()
    {
        // Gizmos.DrawWireSphere(defaultTarget.transform.position, radius);
    }
}
