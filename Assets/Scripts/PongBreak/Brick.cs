using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick
{
	public string name;
	public float length = 0.5f;
	public float weight = 1f;
	public ElementType elementType;
	public bool penetrable;
	public int multiplier = 1;
	
	public Brick(string name)
	{
		this.name = name;
	}
		
	public Brick (string name, float length, float weight, ElementType elementType): this(name)
	{
		this.length = length;
		this.weight = weight;
		this.elementType = elementType;
	}
	
	public Brick (string name, float length, float weight, ElementType elementType, bool penetrable): this(name, length, weight, elementType)
	{
		this.penetrable = penetrable;
	}
	
	public Brick (string name, float length, float weight, ElementType elementType, bool penetrable, int multiplier): this(name, length, weight, elementType, penetrable)
	{
		this.multiplier = multiplier;
	}
}
