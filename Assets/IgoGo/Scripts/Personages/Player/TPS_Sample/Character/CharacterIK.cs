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


    //#region IK служебное
    //private Vector3 rightFootPosition, LeftFootPosition, RightFootIKPosition, LeftFootIKPosition;
    //private Quaternion leftFootIKRotation, rightFootIKRotation;
    //private float lastPelvisPositionY, LastRightFootPositionY, LastLeftFoorPositionY;

    //[Header("Feet Grounder")]
    //private bool enableFeetIK;
    //[Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    //[Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    //[SerializeField] private LayerMask environmentMask;
    //[SerializeField] private float pelvisOffset = 0f;
    //[Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.28f;
    //[Range(0, 1)] [SerializeField] private float feetToIKPositionSpeed = 0.5f;

    //public string leftFootAnimVirableName = "LeftFootCurve";
    //public string rightFootAnimVirableName = "RightFootCurve";

    //public bool useProIKFeature = false;
    //public bool showSolverDebug = true;
    //#endregion

    public void Initiolize(SampleController sampleController)
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
                rHandWeight += Time.deltaTime * 2;
            }
            else
            {
                rHandWeight -= Time.deltaTime * 2;
            }
            rHandWeight = Mathf.Clamp01(rHandWeight);
        }
        else
        {
            if (characterStatus.isAiming)
            {
                rHandWeight += Time.deltaTime * 2;
                lHandWeight += Time.deltaTime * 2;
            }
            else
            {
                rHandWeight -= Time.deltaTime * 2;
                lHandWeight -= Time.deltaTime * 2;
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

    //private void IKforFeet()
    //{
    //    anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
    //    if (useProIKFeature)
    //    {
    //        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, anim.GetFloat(rightFootAnimVirableName));
    //    }
    //    MovefeetToIKPoint(AvatarIKGoal.RightFoot, RightFootIKPosition, rightFootIKRotation, ref LastRightFootPositionY);

    //    anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
    //    if (useProIKFeature)
    //    {
    //        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, anim.GetFloat(leftFootAnimVirableName));
    //    }
    //    MovefeetToIKPoint(AvatarIKGoal.LeftFoot, LeftFootIKPosition, leftFootIKRotation, ref LastLeftFoorPositionY);
    //}

    //private void MovefeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    //{
    //    Vector3 targetIKPos = anim.GetIKPosition(foot);

    //    if(positionIKHolder != Vector3.zero)
    //    {
    //        targetIKPos = transform.InverseTransformPoint(targetIKPos);
    //        positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

    //        float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
    //        targetIKPos.y += yVariable;

    //        lastFootPositionY = yVariable;

    //        targetIKPos = transform.TransformPoint(targetIKPos);

    //        anim.SetIKRotation(foot, rotationIKHolder);
    //    }

    //    anim.SetIKPosition(foot, targetIKPos);
    //}

    //private void MovePelvisHeight()
    //{
    //    if (RightFootIKPosition == Vector3.zero || LeftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
    //    {
    //        lastPelvisPositionY = anim.bodyPosition.y;
    //        return;
    //    }

    //    float lOffsetPosition = LeftFootIKPosition.y - transform.position.y;
    //    float rOffsetPosition = RightFootIKPosition.y - transform.position.y;

    //    float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

    //    Vector3 newPelvisPos = anim.bodyPosition + Vector3.up * totalOffset;

    //    newPelvisPos.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPos.y, pelvisUpAndDownSpeed);

    //    lastPelvisPositionY = anim.bodyPosition.y;
    //}

    //private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    //{
    //    RaycastHit outHit;

    //    if(showSolverDebug)
    //    {
    //        Debug.DrawLine(fromSkyPosition, fromSkyPosition +
    //            Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
    //    }

    //    if(Physics.Raycast(fromSkyPosition, Vector3.down, out outHit, raycastDownDistance + heightFromGroundRaycast, environmentMask))
    //    {
    //        feetIKPositions = fromSkyPosition;
    //        feetIKPositions.y = outHit.point.y + pelvisOffset;
    //        feetIKRotations = Quaternion.FromToRotation(Vector3.up, outHit.normal) * transform.rotation;

    //        return;
    //    }

    //    feetIKPositions = Vector3.zero;

    //}

    //private void AdjustFeetTurget(ref Vector3 feetPosition, HumanBodyBones foot)
    //{
    //    feetPosition = anim.GetBoneTransform(foot).position;
    //    feetPosition.y = transform.position.y + heightFromGroundRaycast;
    //}
}
