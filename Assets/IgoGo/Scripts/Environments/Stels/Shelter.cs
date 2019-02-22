using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShelterType
{
    Standart,
    Angel
}


[System.Serializable]
public class ShelterAngel
{
    public Transform rightHelper;
    public Transform leftHelper;
    public Shelter rightShelter;
    public Shelter leftShelter;

    public Shelter rightNeigbor;
    public Shelter leftNeigbor;
}

[ExecuteInEditMode]
public class Shelter : MonoBehaviour {

    public ShelterType shelterType;
    public Vector3 direction;

    [Space(10)]
    public Transform playerPoint;
    public ShelterAngel shelterAngel;

    [Space(20)]
    [Header("Простроить связи")]
    public bool checkHelpers;
    public bool drawMarkers;
    public bool fullShelter;

    private void Update()
    {
        if(checkHelpers)
        {
            if(shelterType != ShelterType.Standart)
            {
                Shelter bufer;
                if(shelterAngel.rightShelter != null)
                {
                    bufer = shelterAngel.rightShelter;
                    bufer.shelterAngel.leftHelper = shelterAngel.rightHelper;
                    bufer.shelterAngel.leftShelter = this;
                }
                if (shelterAngel.leftShelter != null)
                {
                    bufer = shelterAngel.leftShelter;
                    bufer.shelterAngel.rightHelper = shelterAngel.leftHelper;
                    bufer.shelterAngel.rightShelter = this;
                }
            }
            checkHelpers = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if(drawMarkers)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.DrawLine(transform.position, transform.position + direction);
            if (shelterType == ShelterType.Angel)
            {
                Gizmos.color = Color.yellow;

                if (shelterAngel.rightShelter != null)
                {
                    Gizmos.DrawCube(shelterAngel.rightHelper.position, new Vector3(0.3f, 0.3f, 0.3f));
                    Gizmos.DrawLine(shelterAngel.rightHelper.position, transform.position);
                    Gizmos.DrawCube(shelterAngel.rightShelter.transform.position, new Vector3(0.3f, 0.3f, 0.3f));
                    Gizmos.DrawLine(shelterAngel.rightHelper.position, shelterAngel.rightShelter.transform.position);
                }
                if (shelterAngel.leftShelter != null)
                {
                    Gizmos.DrawCube(shelterAngel.leftHelper.position, new Vector3(0.3f, 0.3f, 0.3f));
                    Gizmos.DrawLine(shelterAngel.leftHelper.position, transform.position);
                    Gizmos.DrawCube(shelterAngel.leftShelter.transform.position, new Vector3(0.3f, 0.3f, 0.3f));
                    Gizmos.DrawLine(shelterAngel.leftHelper.position, shelterAngel.leftShelter.transform.position);
                }
                if(shelterAngel.rightNeigbor != null)
                {
                    Gizmos.DrawLine(shelterAngel.rightNeigbor.transform.position, transform.position);
                }
                if (shelterAngel.leftNeigbor != null)
                {
                    Gizmos.DrawLine(shelterAngel.leftNeigbor.transform.position, transform.position);
                }
            }
        }
       
    }
}
