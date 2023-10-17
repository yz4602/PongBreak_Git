using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public bool isEnd;
	public bool sideCanRebound;
	public GameObject sides;
	public PhysicsMaterial2D borderPM;
	
	private void Awake() 
	{
		if(instance == null)
		{
			instance = this;
		}	
		else
		{
			Destroy(this);
		}
		
		if(sideCanRebound)
		{
			foreach(BoxCollider2D b in sides.GetComponentsInChildren<BoxCollider2D>())
			{
				b.sharedMaterial = sideCanRebound? borderPM : null;
			}
		}
	}
}
