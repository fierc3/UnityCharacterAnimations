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
        if (!looking) {
            UpdateLookTargetPosition(defaultTarget.transform.position);
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(defaultTarget.transform.position, radius);

        foreach (Collider collider in colliders)
        {
            
            if (collider.gameObject.tag == tagToFind)
            {
                Debug.Log("Collider Found to look at with tag " + tagToFind);
                //transform.position = collider.transform.position;
                //transform.position = Vector3.Lerp(transform.position, collider.transform.position, interpolationSpeed);
                UpdateLookTargetPosition(collider.transform.position);
                return;
            }
        }


        //this.transform.position = defaultTarget.transform.position;
        UpdateLookTargetPosition(defaultTarget.transform.position);
    }

    private void UpdateLookTargetPosition(Vector3 target)
    {
        transform.position = Vector3.Lerp(transform.position, target, interpolationSpeed);
    }


    public void ToggleLooking()
    {
        looking = !looking;
    }

    private void OnDrawGizmos()
    {
       // Gizmos.DrawWireSphere(defaultTarget.transform.position, radius);
    }
}
