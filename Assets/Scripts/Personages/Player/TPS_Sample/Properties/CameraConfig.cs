using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Config/Camera")]
public class CameraConfig : ScriptableObject {

    public float turnSmooth;
    public float pivotSpeed;
    public float xRotSpeed;
    public float yRotSpeed;
    public float minYAngel;
    public float maxYAngel;
    public float minXAngel;
    public float maxXAngel;
    public float normalZ;
    public float normalX;
    public float normalY;
    public float aimZ;
    public float aimX;
}
