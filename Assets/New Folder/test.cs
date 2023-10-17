using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	public Rigidbody2D go1;
	public static Rigidbody2D go2;
	
	private void Update() {
		if(go1 && go2)
		{
			Debug.Log(go1.velocity + " : " + go2.velocity);
		}
	}
}
