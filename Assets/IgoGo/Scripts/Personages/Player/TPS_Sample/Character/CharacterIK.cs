using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour, IPlayerPart {

    public Transform lHandTarget;

    private Animator anim;
    private CharacterStatus characterStatus;
    private Transform targetLook;
    public Transform lHand;
    public Transform rHand;
    private Quaternion lHandRot;
    private Transform sholder;
    private Transform aimPivot;
    private float rHandWeight;
    private float lHandWeight;


    #region FeetIK

    private bool IsActiveIK
    {
        get
        {
            if(enableFeetIK && !characterStatus.isMove && characterStatus.isGround && !characterStatus.isAiming)
            {
                return true;
            }
            else
            {
                CheckFootPosition(leftFoot.position, ref leftFootTargetPos, ref leftFootTargetRot);
                CheckFootPosition(rightFoot.position, ref rightFootTargetPos, ref rightFootTargetRot);
                return false;
            }
        }
    }


    private Quaternion leftFootTargetRot;
    private Quaternion rightFootTargetRot;
    private Vector3 leftFootTargetPos;
    private Vector3 rightFootTargetPos;
    private Transform leftFoot;
    private Transform rightFoot;


    [SerializeField] private float offset;
    [SerializeField] private bool enableFeetIK;

    #endregion

    public void Initialize(SampleController sampleController)
    {
        anim = sampleController.anim;
        characterStatus = sampleController.characterStatus;
        targetLook = sampleController.cameraHandler.targetLook;

        sholder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);

        aimPivot = new GameObject().transform;
        aimPivot.name = "aimPivot";
        aimPivot.transform.parent = transform;

        rHand = new GameObject().transform;
        rHand.name = "right hand";
        rHand.transform.parent = aimPivot;

        lHand = new GameObject().transform;
        lHand.name = "left hand";
        lHand.transform.parent = aimPivot;

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
    }


    private void Update()
    {
        if(lHandTarget != null)
        {
            lHandRot = lHandTarget.rotation;
            lHand.position = lHandTarget.position;
        }

        if(anim.GetInteger("WeaponType") >= 2)
        {
            lHandWeight = 1;
            if (characterStatus.isAiming)
            {
                rHandWeight += Time.deltaTime * 10;
            }
            else
            {
                rHandWeight -= Time.deltaTime * 10;
            }
            rHandWeight = Mathf.Clamp01(rHandWeight);
        }
        else
        {
            if (characterStatus.isAiming)
            {
                rHandWeight += Time.deltaTime * 10;
                lHandWeight += Time.deltaTime * 10;
            }
            else
            {
                rHandWeight -= Time.deltaTime * 10;
                lHandWeight -= Time.deltaTime * 10;
            }
            rHandWeight = Mathf.Clamp01(rHandWeight);
            lHandWeight = Mathf.Clamp01(lHandWeight);
        }

        FootUpdate();
    }

    private void OnAnimatorIK()
    {
        #region IK позиционирование рук
        aimPivot.position = sholder.position;

        if(characterStatus.isAiming)
        {
            aimPivot.LookAt(targetLook);

            anim.SetLookAtWeight(1, .4f , 1);
            anim.SetLookAtPosition(targetLook.position);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHandWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHandWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, lHand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, lHandRot);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rHandWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rHandWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rHand.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rHand.rotation);
        }
        else
        {
            anim.SetLookAtWeight(.3f, 0, .3f);
            anim.SetLookAtPosition(targetLook.position);

            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHandWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHandWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, lHand.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, lHandRot);

            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rHandWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rHandWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rHand.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rHand.rotation);
        }

        #endregion

        #region IK позиционирование ног

        FootIK();
        #endregion
    }

    private void FootUpdate()
    {
        if(!IsActiveIK || !characterStatus.isGround)
        {
            return;
        }
        CheckFootPosition(leftFoot.position, ref leftFootTargetPos, ref leftFootTargetRot);
        CheckFootPosition(rightFoot.position, ref rightFootTargetPos, ref rightFootTargetRot);
    }

    private void CheckFootPosition(Vector3 currentFootPos, ref Vector3 nextFootPos, ref Quaternion nextFootRot)
    {
        RaycastHit hit;
        Debug.DrawRay(currentFootPos + Vector3.up * 0.5f, Vector3.down, Color.red);
        if (Physics.Raycast(currentFootPos + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            nextFootPos = Vector3.Lerp(currentFootPos, hit.point + Vector3.up * offset, Time.deltaTime * 10);
            nextFootRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }

    private void FootIK()
    {
        if (!IsActiveIK || !characterStatus.isGround)
        {
            return;
        }

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTargetPos);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTargetRot);

        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTargetPos);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTargetRot);

    }
}
