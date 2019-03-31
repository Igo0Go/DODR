using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Part
{
    public RectTransform rTransform;
    public Vector2 pos;
}

public class Crosshair : MonoBehaviour {

    public bool Visible
    {
        set
        {
            _visible = value;

            foreach (var c in parts)
            {
                c.rTransform.gameObject.SetActive(_visible);
            }
        }
    }
    public float currentSpread;
    public float speedSpread;

    public Part[] parts;
    public CharacterMovement characterMovement;
	
	private CharacterStatus characterStatus;
    private Part part;
    private float t;
    private float curSpread;
    private bool _visible;


	void Update () {
        
		if(characterStatus.isAiming)
		{
			currentSpread = 20;
		}
		else if(characterMovement.moveAmounth > 0)
        {
            currentSpread = 20 * (5 + characterMovement.moveAmounth);
        }
        else
        {
            currentSpread = 20;
        }

        CrosslineUpdate();
	}
	
	public void Initiolize(CharacterStatus status)
	{
		characterStatus = status;
	}
	
    private void CrosslineUpdate()
    {
        t = Time.deltaTime * speedSpread;
        curSpread = Mathf.Lerp(curSpread, currentSpread, t);

        for (int i = 0; i < parts.Length; i++)
        {
            part = parts[i];
            part.rTransform.anchoredPosition = part.pos * currentSpread;
        }
    }
}
