using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicSceneLoader : MonoBehaviour
{
	public int sceneIndex;
	
	
	private bool replicComplete;
	private bool scenLoadComplete;
   
	public void CompleteReplic()
	{
		replicComplete = true;
	}
   
	public void CompleteSceneLoad()
	{
		scenLoadComplete = true;
	}
   
	private void CheckComplete()
	{
		if(replicComplete && scenLoadComplete)
		{
			LoadNextScene();
		}
	}
	
	private void LoadNextScene()
	{
		//Логика для загрузки уже прогруженной сцены
	}
}
