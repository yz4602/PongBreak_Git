using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class BricksGenerator : MonoBehaviour
{
	public int rows = 9;
	public int columns = 5;
	public float spacing = 0.1f;
	[SerializeField] float brickLength = 0.5f;
	[SerializeField] float brickWeight= 1f;
	[Header("Attribute Ratio")]
	public float normalRate = 0.25f;
	public float flameRate = 0.25f;
	public float waterRate = 0.25f;
	public float leafRate = 0.25f;
	[Header("Penetrable Rate")]
	[Range(0,1)]public float penetrableRate = 0.1f;
	
	public bool canMove;
	
	public static Dictionary<string,Brick> brickDict;
	
	private void Awake() 
	{
		brickDict = new Dictionary<string,Brick>();
		canMove = false;
	}
	
	// Start is called before the first frame update
	void Start()
	{
		Spawnbricks();
	}
	
	void Update() 
	{
		//Debug.Log(brickDict.Count);
		if(brickDict.Count == 0)
		{
			Spawnbricks();
			RandomDeleteBrick();
			PutGenerator();
		}
		if(canMove) MoveGenerator();
	}
	
	private void Spawnbricks()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				Brick brick = new Brick("Brick" + i + "-" + j, brickLength, brickWeight, ElementType.normal);
				brickDict.Add(brick.name, brick);
				
				// Create a new GameObject for each brick
				GameObject brickGo = new GameObject(brick.name);
				brickGo.tag = "Brick";
				brickGo.transform.parent = this.transform;
				
				// Set the position of the brick
				Vector3 position = new Vector3((j - 2) * (brick.length + spacing), (i - 4) * (brick.weight + spacing), 0);
				brickGo.transform.position = position;
				brickGo.transform.localScale = new Vector3(brick.length, brickWeight, 1f);

				// Add Rigid body to the brick
				Rigidbody2D rb = brickGo.AddComponent<Rigidbody2D>();
				rb.gravityScale = 0;
				rb.sharedMaterial = Resources.Load<PhysicsMaterial2D>("Materials/BrickPM");
				
				rb.bodyType = RigidbodyType2D.Kinematic;
				rb.AddComponent<BoxCollider2D>();
				
				// Add a Sprite Renderer component to the brick
				SpriteRenderer spriteRenderer = brickGo.AddComponent<SpriteRenderer>();

				// Set the sprite of the Sprite Renderer (assuming you have a sprite named "SquareSprite" in your Assets)
				spriteRenderer.sprite = Resources.Load<Sprite>("Square");

				// Assign an attribute to the brick
				brick.elementType = takeAttribute();
				AssignAttribute(brick.elementType, spriteRenderer);
				
				//Assign an penetrability
				brick.penetrable = takePenetrability();
				if(brick.penetrable)
				{
					Color currentColor = spriteRenderer.color;
					currentColor.a = 0.4f;
					spriteRenderer.color = currentColor;
					
					Destroy(brickGo.GetComponent<Rigidbody2D>());
					brickGo.GetComponent<BoxCollider2D>().isTrigger = true;
					
					if(brick.elementType == ElementType.normal)
					{
						brick.multiplier = 2;
						spriteRenderer.sprite = Resources.Load<Sprite>("x2");
					}
				}
			}
		}
	}
	
	private void RandomDeleteBrick()
	{
		int deleteTime = Random.Range((int)(rows * columns * 0.7), (int)(rows * columns * 0.9));
		string dBrick;
		for(int i = 0; i < deleteTime; i++)
		{
			do
			{
				dBrick = "Brick" + Random.Range(0, rows) + "-" + Random.Range(0, columns);
			} 
			while(!brickDict.ContainsKey(dBrick));
			
			//Debug.Log(dBrick);
			Destroy(GameObject.Find(dBrick));
			brickDict.Remove(dBrick);
		}
	}
	
	private void PutGenerator()
	{
		int positionNum = Random.Range(0,2);
		transform.position = new Vector3(0, positionNum == 0 ? -10 : 10, 0);
		Invoke("SetCanMoveTrue", 2f);
	}
	
	private void MoveGenerator()
	{
		if(transform.position.y > 0)
		{
			transform.position -= Vector3.up * Time.deltaTime;
			if(transform.position.y <= 0)
			{
				canMove = false;
			}
		}
		else if(transform.position.y < 0)
		{
			transform.position += Vector3.up * Time.deltaTime;
			if(transform.position.y >= 0)
			{
				canMove = false;
			}
		}
	}
	
	private ElementType takeAttribute()
	{
		float randomNum = Random.Range(0, 100)/100f;
		if (randomNum < normalRate) return ElementType.normal;
		else if (randomNum < normalRate + flameRate) return ElementType.flame;
		else if (randomNum < normalRate + flameRate + waterRate) return ElementType.water;
		else return ElementType.leaf;
	}
	
	public static void AssignAttribute(ElementType elementType, SpriteRenderer spriteRenderer)
	{
		switch(elementType)
				{
					case ElementType.normal: spriteRenderer.material = Resources.Load<Material>("Materials/Normal"); break;
					case ElementType.flame: spriteRenderer.material = Resources.Load<Material>("Materials/Flame"); break;
					case ElementType.water: spriteRenderer.material = Resources.Load<Material>("Materials/Water"); break;
					case ElementType.leaf: spriteRenderer.material = Resources.Load<Material>("Materials/Leaf"); break;
					default: Debug.LogWarning("Problem in assign attribute!"); break;
				}
	}
	
	private bool takePenetrability()
	{
		return (Random.Range(0, 100)/100f < penetrableRate);
	}
	
	public void SetCanMoveTrue()
	{
		canMove = true;
	}
}

[CustomEditor(typeof(BricksGenerator))]
public class DictionaryHolderEditor : Editor
{	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if(EditorApplication.isPlaying)
		{
			foreach (var kvp in BricksGenerator.brickDict)
			{
				EditorGUILayout.LabelField("Key: " + kvp.Key, "Value: " + kvp.Value.elementType);
			}
		}
	}
}
