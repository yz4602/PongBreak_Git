using System.Collections;
using TMPro;
using UnityEngine;

public class PaddleControl : MonoBehaviour
{
	public float speed = 10f;
	public bool isRight;
	public string paddleName;
	private float length;
	[SerializeField] private ElementType elementType;
	[SerializeField] private bool canMoveUp = true;
	[SerializeField] private bool canMoveDown = true;
	[SerializeField] private TextMeshProUGUI stackText;
	[SerializeField] private int marbleStack = 0;
	[SerializeField] private GameObject marbleGo;
	[SerializeField] private Transform marbleParent;
	
	
	private Rigidbody2D rb;

	private void Awake() 
	{
		rb = GetComponent<Rigidbody2D>();
		length = this.gameObject.transform.localScale.y;
		paddleName = isRight ? "PaddleR" : "PaddleL";
	}
	
	void Update()
	{
		MovePaddle();
		ServeMarble();
	}
	
	void MovePaddle()
	{
		if (!isRight)
		{
			float vL = Input.GetAxisRaw("VerticalL");
			if(!canMoveUp)
			{
				vL = (vL > 0) ? 0 : vL;
			}
			if(!canMoveDown)
			{
				vL = (vL < 0) ? 0 : vL;
			}
			rb.velocity = new Vector2(0, vL) * speed;
		}
		else
		{
			float vR = Input.GetAxisRaw("VerticalR");
			if(!canMoveUp)
			{
				vR = (vR > 0) ? 0 : vR;
			}
			if(!canMoveDown)
			{
				vR = (vR < 0) ? 0 : vR;
			}
			rb.velocity = new Vector2(0, vR) * speed;
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if(other.tag == "border")
		{
			if(other.transform.position.y > this.transform.position.y)
			{
				canMoveUp = false;
			}
			else
			{
				canMoveDown = false;
			}
		}
	}
	
	private void OnTriggerExit2D(Collider2D other) 
	{
		if(other.tag == "border")
		{
			if(other.transform.position.y > this.transform.position.y)
			{
				canMoveUp = true;
			}
			else
			{
				canMoveDown = true;
			}
		}
	}
	
	public void ChangeAttribute(ElementType e)
	{
		elementType = e;
	}
	
	public ElementType GetAttribute()
	{
		return elementType;
	}
	
	public void SetLength(float length)
	{
		this.length = length;
		gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, length, gameObject.transform.localScale.z);
	}
	
	public float GetLength()
	{
		return length;
	}
	
	public void marbleStackAdd()
	{
		marbleStack++;
		stackText.text = (int.Parse(stackText.text) + 1).ToString();
	}
	
	//TODO: Serve Marble
	public void ServeMarble()
	{
		if(marbleStack > 0)
		{
			marbleStack--;
			Invoke("_ServeMarble", 2f);
		}
	}
	
	private void _ServeMarble()
	{	
		float positionX = isRight ? -0.5f : 0.5f;
		GameObject newMarble = Instantiate(marbleGo, transform.position + new Vector3(positionX, 0, 0), Quaternion.identity);
		MarbleControl mc = newMarble.GetComponent<MarbleControl>();
		mc.SetFromPaddle(this);
		newMarble.transform.parent = marbleParent;
		stackText.text = (int.Parse(stackText.text) - 1).ToString();
	}
}
