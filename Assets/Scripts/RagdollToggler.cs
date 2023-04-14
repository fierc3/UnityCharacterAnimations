using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR;

public class RagdollToggler : MonoBehaviour
{
    [SerializeField]
    private const string StandupClip = "Getting Up";
    [SerializeField]
    private BoxCollider ingameCollider;
    [SerializeField]
    private Rigidbody ingameBody;
    [SerializeField]
    private GameObject rig;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float timeToResetBones = 3;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollBodies;

    private class LocalBoneTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
       
    }

    private Transform hipsBone;
    private LocalBoneTransform[] standUpBoneTransforms;
    private LocalBoneTransform[] ragdollBoneTransforms;
    private Transform[] bones;

    private enum RagdollState
    {
        Ragdoll,
        StandingUp,
        Resetting,
        Deactivated,
    }

    private RagdollState currentState = RagdollState.Deactivated;
    private float elapsedResetBonesTime;




    // Start is called before the first frame update
    void Awake()
    {
        PrepareRagdoll();
        AnimationSampleTransformsToLocalBoneTransforms(StandupClip, standUpBoneTransforms);
        TurnOffRagdollMode();

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Time c " + currentState);
        switch (currentState)
        {
            case RagdollState.Ragdoll:
                // nothing to do when ragdoll, because we manually tell when ragdolling is over via UI
                break;
            case RagdollState.StandingUp:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(StandupClip))
                {
                    currentState = RagdollState.Deactivated; // once GettingUp has finished playing, ragdoll is no longer relevant until activated again
                }
                break;
            case RagdollState.Resetting:
                ResettingBonesBehaviour();
                break;
        }
    }

    private void ResettingBonesBehaviour()
    {
        // increment elapsed time since resetting started
        elapsedResetBonesTime += Time.deltaTime;
        // calculate percentage of elapsed time
        float elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

        // loop through all bones
        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            // lerp between current bone position/rotation and original stand-up position/rotation
            bones[boneIndex].localPosition = Vector3.Lerp(
                ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            bones[boneIndex].localRotation = Quaternion.Lerp(
                ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        // if we have stood up to 30% of the way
        if (elapsedPercentage >= 0.3)
        {
            // set current state to StandingUp
            currentState = RagdollState.StandingUp;
            // turn off ragdoll mode
            TurnOffRagdollMode();
            // set all animator parameters to false
            animator.parameters.ToList().ForEach(p => animator.SetBool(p.name, false));
            // play the Standup animation clip
            animator.Play(StandupClip);
        }
    }


    // Entrypoint for UI
    public void StartDeathRagdoll()
    {
        TurnOnRagdollMode();
    }

    // Entrypoint for UI
    public void StartAlive()
    {
        Debug.Log("Time for Aliving");
        TurnOffRagdollWithAlignment();
    }


    // Prepares Data needed for processing
    void PrepareRagdoll()
    {
        ragdollColliders = rig.GetComponentsInChildren<Collider>();
        ragdollBodies = rig.GetComponentsInChildren<Rigidbody>();
        hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
        bones = hipsBone.GetComponentsInChildren<Transform>();
        standUpBoneTransforms = new LocalBoneTransform[bones.Length];
        ragdollBoneTransforms = new LocalBoneTransform[bones.Length];
        int i = 0;
        bones.ToList().ForEach(x =>
        {
            standUpBoneTransforms[i] = new LocalBoneTransform();
            ragdollBoneTransforms[i] = new LocalBoneTransform();
            i++;
        });
    }

    void TurnOffRagdollWithAlignment()
    {
        // some magic alignment stuff
        AlignRotationToHips();
        AlignPositionToHips();

        TransformsToLocalBoneTransforms(ragdollBoneTransforms);

        currentState = RagdollState.Resetting;
        elapsedResetBonesTime = 0;
    }

    // Turns off ragdoll mode, and enables the character's in-game collider, rigidbody, and animator
    void TurnOffRagdollMode()
    {
        // Changes the current state to deactivated
        currentState = RagdollState.Deactivated;

        // Disables the colliders for the ragdoll components
        foreach (var col in ragdollColliders)
        {
            col.enabled = false;
        }

        // Sets the rigidbodies for the ragdoll components to be kinematic
        foreach (var bod in ragdollBodies)
        {
            bod.isKinematic = true;
        }

        // Enables the collider and sets the rigidbody for the in-game character
        ingameCollider.enabled = true;
        ingameBody.isKinematic = false;

        // Enables the animator for the in-game character
        animator.enabled = true;
    }

    // Turns on ragdoll mode, disabling the character's in-game collider, rigidbody, and animator
    void TurnOnRagdollMode()
    {
        // Changes the current state to ragdoll
        currentState = RagdollState.Ragdoll;

        // Enables the colliders for the ragdoll components
        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
        }

        // Sets the rigidbodies for the ragdoll components to be non-kinematic
        foreach (var bod in ragdollBodies)
        {
            bod.isKinematic = false;
        }

        // Disables the collider and sets the rigidbody for the in-game character
        ingameCollider.enabled = false;
        ingameBody.isKinematic = true;

        // Disables the animator for the in-game character

        animator.enabled = false;

        // Applies an initial force to the ragdoll body to make it fall backwards
        ragdollBodies[0].AddForceAtPosition(new Vector3() { x = -101, y = 50f, z = 0 }, new Vector3() { x = 0, y = 0, z = 0 }, ForceMode.Impulse);
    }

    // Aligns the rotation of the character to the hips bone.
    private void AlignRotationToHips()
    {
        // Store original hips position and rotation.
        Vector3 originalHipsPosition = hipsBone.position;
        Quaternion originalHipsRotation = hipsBone.rotation;

        // Calculate the desired direction of the character based on the hips up direction.
        Vector3 desiredDirection = hipsBone.up;
        desiredDirection.y = 0;
        desiredDirection.Normalize();

        // Rotate the character from its current forward direction to the desired direction.
        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        // Restore original hips position and rotation.
        hipsBone.position = originalHipsPosition;
        hipsBone.rotation = originalHipsRotation;
    }

    // Aligns the position of the character to the hips bone.
    private void AlignPositionToHips()
    {
        // Store original hips position.
        Vector3 originalHipsPosition = hipsBone.position;

        // Set character position to hips position.
        transform.position = hipsBone.position;

        // Calculate the offset between the character's position and the stand up bone's position.
        Vector3 positionOffset = standUpBoneTransforms[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;

        // Move the character to the calculated offset position.
        transform.position -= positionOffset;

        // Cast a ray downwards from the character's position to find the ground.
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            // Set the character's y position to the hit point.
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        // Restore original hips position.
        hipsBone.position = originalHipsPosition;
    }

    // This function maps the positions and rotations of bones to local bone transforms.
    private void TransformsToLocalBoneTransforms(LocalBoneTransform[] target)
    {
        int i = 0;
        // For each bone, set the position and rotation of the corresponding local bone transform.
        bones.ToList().ForEach((bone) => {
            target[i].Position = bone.localPosition;
            target[i].Rotation = bone.localRotation;
            i++;
        });
    }

    // This function samples an animation clip and maps the bone transforms to local bone transforms.
    private void AnimationSampleTransformsToLocalBoneTransforms(string clipName, LocalBoneTransform[] boneTransforms)
    {
        // Save the position and rotation of the game object before sampling the animation.
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        // Find the animation clip with the specified name and sample it.
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 1f);
                // Map the bone transforms to local bone transforms.
                TransformsToLocalBoneTransforms(boneTransforms);
                break;
            }
        }

        // Restore the position and rotation of the game object.
        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

}
