using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.XR;

public class RagdollToggler : MonoBehaviour
{


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
        AnimationSampleTransformsToLocalBoneTransforms("Getting Up", standUpBoneTransforms);
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
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up") == false)
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
        elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

        for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
        {
            bones[boneIndex].localPosition = Vector3.Lerp(
                ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            bones[boneIndex].localRotation = Quaternion.Lerp(
                ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >= 0.3)
        {
            currentState = RagdollState.StandingUp;
            //animator.parameters.ToList().ForEach(param => { animator.SetBool(param.name, param.name.Equals("Getting Up")); });
            TurnOffRagdollMode();
            animator.Play("Getting Up");
        }
    }

    public void StartDeathRagdoll()
    {
        TurnOnRagdollMode();
    }

    public void StartAlive()
    {
        Debug.Log("Time for Aliving");
        TurnOffRagdollWithAlignment();
    }


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

    void TurnOffRagdollMode()
    {
        currentState = RagdollState.Deactivated;
        foreach (var col in ragdollColliders)
        {
            col.enabled = false;
        }

        foreach (var bod in ragdollBodies)
        {
            bod.isKinematic = true;
        }
        ingameCollider.enabled = true;
        ingameBody.isKinematic = false;
        animator.enabled = true;
    }

    void TurnOnRagdollMode()
    {
        currentState = RagdollState.Ragdoll;

        foreach (var col in ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (var bod in ragdollBodies)
        {
            bod.isKinematic = false;
        }
        ingameCollider.enabled = false;
        ingameBody.isKinematic = true;
        animator.enabled = false;

        //add force so he falls backwards
        ragdollBodies[0].AddForceAtPosition(new Vector3() { x =-101, y = 50f, z = 0 }, new Vector3() { x = 0, y = 0, z = 0 }, ForceMode.Impulse);
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = hipsBone.position;
        Quaternion originalHipsRotation = hipsBone.rotation;

        Vector3 desiredDirection = hipsBone.up;
        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        hipsBone.position = originalHipsPosition;
        hipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = hipsBone.position;
        transform.position = hipsBone.position;
        Vector3 positionOffset = standUpBoneTransforms[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }

        hipsBone.position = originalHipsPosition;
    }

    private void TransformsToLocalBoneTransforms(LocalBoneTransform[] target)
    {
        int i = 0;
        bones.ToList().ForEach((bone) => {
            target[i].Position = bone.localPosition; 
            target[i].Rotation = bone.localRotation;
            i++;
        });
        Debug.Log("Time2" + target[0]);
    }

    private void AnimationSampleTransformsToLocalBoneTransforms(string clipName, LocalBoneTransform[] boneTransforms)
    {
        Debug.Log("Time xxxxxxxxxx");
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("Time xxxxxxxxxx" + clip.name);
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 1f);
                TransformsToLocalBoneTransforms(boneTransforms);
                break;
            }
        }
        
        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }
}
