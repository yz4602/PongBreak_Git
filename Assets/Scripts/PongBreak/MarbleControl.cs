using UnityEngine;

public class MarbleControl : MonoBehaviour
{
	public float speed = 10f;
	public float atk = 5f;
	private float atkMultiplier = 1f;
	public ElementType elementType;
	[SerializeField] PaddleControl fromPaddle;
	private Rigidbody2D rb;

	void Start()
	{
		if(fromPaddle.isRight)
			GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
		else
			GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
			
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.CompareTag("Paddle"))
		{
			CollidePaddle(col);
		}
		if (col.collider.CompareTag("Side"))
		{
			CollideSide(col);
		}
		if (col.collider.CompareTag("Brick"))
		{
			CollideBrick(col);
		}
	}
	
	private void OnTriggerStay2D(Collider2D col) 
	{
		if (col.CompareTag("Brick"))
		{
			TriggerBrick(col);
		}
	}
	
	private void CollidePaddle(Collision2D col)
	{
		fromPaddle = col.gameObject.GetComponent<PaddleControl>();
		
		//TODO: Change attribute logic (Every Condition)
		switch(elementType)
		{
			case ElementType.normal:
				elementType = fromPaddle.GetAttribute();
				BricksGenerator.AssignAttribute(elementType, GetComponent<SpriteRenderer>());
				break;
			case ElementType.flame:
				switch(fromPaddle.GetAttribute())
				{
					case ElementType.water:
						elementType = ElementType.normal;
						atkMultiplier = 1f;
						BricksGenerator.AssignAttribute(elementType, GetComponent<SpriteRenderer>());
						break;
					case ElementType.leaf:
						fromPaddle.SetLength(fromPaddle.GetLength() / 1.1f);
						break;
					default:
						break;
				}
				break;
			case ElementType.water:
				switch(fromPaddle.GetAttribute())
				{
					case ElementType.flame:
						atkMultiplier = 0.8f;
						break;
					case ElementType.leaf:
						fromPaddle.SetLength(fromPaddle.GetLength() * 1.1f);
						break;
					default:
						break;
				}
				break;
			case ElementType.leaf:
				switch(fromPaddle.GetAttribute())
				{
					case ElementType.water:
						atkMultiplier = 1.25f;
						break;
					case ElementType.flame:
						elementType = ElementType.flame;
						BricksGenerator.AssignAttribute(elementType, GetComponent<SpriteRenderer>());
						atkMultiplier = 1.25f;
						break;
					default:
						break;
				}
				break;
			default: break;
		}	
		// elementType = fromPaddle.GetAttribute();
		// BricksGenerator.AssignAttribute(elementType, GetComponent<SpriteRenderer>());
		
		//How the marble will move when they collide a paddle
		float y = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
		if(col.gameObject.GetComponent<PaddleControl>().isRight)
		{
			Vector2 dir = new Vector2(-1, -y).normalized;
			rb.velocity = dir * speed;
		}
		else
		{
			Vector2 dir = new Vector2(1, -y).normalized;
			rb.velocity = dir * speed;
		}
	}
	
	private void CollideBrick(Collision2D col)
	{
		if(BricksGenerator.brickDict.ContainsKey(col.gameObject.name) &&
		   BricksGenerator.brickDict[col.gameObject.name].elementType != ElementType.normal)
		{
			fromPaddle.ChangeAttribute(BricksGenerator.brickDict[col.gameObject.name].elementType);
			BricksGenerator.AssignAttribute(fromPaddle.GetAttribute(),fromPaddle.gameObject.GetComponent<SpriteRenderer>());
		}
		BricksGenerator.brickDict.Remove(col.gameObject.name);
		Destroy(col.gameObject);
	}
	
	private void TriggerBrick(Collider2D col)
	{
		//TODO: Trigger Double
		if(BricksGenerator.brickDict.ContainsKey(col.gameObject.name)&&
		   BricksGenerator.brickDict[col.gameObject.name].elementType == ElementType.normal && 
		   BricksGenerator.brickDict[col.gameObject.name].multiplier == 2)
		{
			GameObject duplicatedMarble = Instantiate(gameObject, transform.position, Quaternion.identity);
			Rigidbody2D dRb = duplicatedMarble.GetComponent<Rigidbody2D>();
			dRb.velocity = rb.velocity; //FIXME:右侧球撞x2后有一球往右飞
			Debug.Log(dRb.velocity +  " : " + rb.velocity);
			dRb.AddForce(Vector2.down * 20f);
			rb.AddForce(Vector2.up * 20f);
			duplicatedMarble.transform.parent = transform.parent;
		}
		
		if(BricksGenerator.brickDict.ContainsKey(col.gameObject.name) &&
		   BricksGenerator.brickDict[col.gameObject.name].elementType != ElementType.normal)
		{
			elementType = BricksGenerator.brickDict[col.gameObject.name].elementType;
			BricksGenerator.AssignAttribute(elementType, GetComponent<SpriteRenderer>());
		}
		
		BricksGenerator.brickDict.Remove(col.gameObject.name);
		Destroy(col.gameObject);
	}
	
	private void CollideSide(Collision2D col)
	{
		Side side = col.gameObject.GetComponent<Side>();
		float effectMultiplier;
		if (DetermineRestraint(side.paddle.GetAttribute()) > 0)
		{
			effectMultiplier = 1.25f;
		}
		else if(DetermineRestraint(side.paddle.GetAttribute()) < 0)
		{
			effectMultiplier = 0.8f;
		}
		else
		{
			effectMultiplier = 1f;
		}
		
		side.ReduceHealth(5 * atkMultiplier * effectMultiplier); //Take Damage
		if(!GameManager.instance.sideCanRebound) Destroy(this.gameObject);
	}

	float hitFactor(Vector2 marblePos, Vector2 paddlePos, float paddleHeight)
	{
		return (paddlePos.y - marblePos.y) / paddleHeight;
	}
	
	int DetermineRestraint(ElementType sideElement)
	{
		if(sideElement == ElementType.flame)
		{
			if(elementType == ElementType.water)
			{
				return 1;
			}
			else if(elementType == ElementType.leaf)
			{
				return -1;
			}
		}
		else if(sideElement == ElementType.water)
		{
			if(elementType == ElementType.leaf)
			{
				return 1;
			}
			else if(elementType == ElementType.flame)
			{
				return -1;
			}
		}
		else if(sideElement == ElementType.leaf)
		{
			if(elementType == ElementType.flame)
			{
				return 1;
			}
			else if(elementType == ElementType.water)
			{
				return -1;
			}
		}
		return 0;
	}
	
	public void SetFromPaddle(PaddleControl pc)
	{
		fromPaddle = pc;
	}
}
