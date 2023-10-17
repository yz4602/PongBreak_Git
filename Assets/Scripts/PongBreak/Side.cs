using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Side : MonoBehaviour
{	
	[SerializeField] private float health = 100;
	[SerializeField] private TextMeshProUGUI healthText;
	public PaddleControl paddle;
	
	public void ReduceHealth(float num)
	{
		health -= num;
		healthText.text = string.Format("{0:F1}", health);
		paddle.marbleStackAdd();
	}
}
