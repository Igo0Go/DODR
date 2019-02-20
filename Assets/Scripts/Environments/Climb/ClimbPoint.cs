
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClimbType
{
    Final,
    Simple
}




[System.Serializable]
[ExecuteInEditMode]
public class ClimbPoint : MonoBehaviour {

    public Vector3 playerPositionOffset;
    public ClimbType climbType;


    [Header("Соседи")]
    public ClimbPoint right;
    public ClimbPoint left;
    public ClimbPoint up;
    public ClimbPoint down;

    [Space(10)]
    public bool checkNeighbours;
    private void Update()
    {
        if(checkNeighbours)
        {
            CheckNeighboursNow();
            checkNeighbours = false;
        }
    }
    private void CheckNeighboursNow()
    {
        if(right != null && right.left != this)
        {
            right.left = this;
        }
        if (up != null && up.down != this)
        {
            up.down = this;
        }
        if (left != null && left.right != this)
        {
            left.right = this;
        }
        if (down != null && down.up != this)
        {
            down.up = this;
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch(climbType)
        {
            case ClimbType.Final:
                Gizmos.color = Color.green;
                break;
            case ClimbType.Simple:
                Gizmos.color = Color.blue;
                break;
        }

        Gizmos.DrawCube(transform.position, new Vector3(0.2f,0.2f,0.2f));

        Gizmos.color = Color.blue;
        if(right != null)
        {
            Gizmos.DrawLine(transform.position, right.transform.position);
        }
        if (left != null)
        {
            Gizmos.DrawLine(transform.position, left.transform.position);
        }
        if (up != null)
        {
            Gizmos.DrawLine(transform.position, up.transform.position);
        }
        if (down != null)
        {
            Gizmos.DrawLine(transform.position, down.transform.position);
        }
    }
}
