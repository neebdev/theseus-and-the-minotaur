using UnityEngine;

public class InputManager : MonoBehaviour
{
	public const float MAX_SWIPE_TIME = 0.5f;
	public const float MIN_SWIPE_DISTANCE = 0.17f;

	public static bool swipedRight = false;
	public static bool swipedLeft = false;
	public static bool swipedUp = false;
	public static bool swipedDown = false;

	public bool debugWithArrowKeys = true;

	public delegate void SwipeEvent(Vector2 dir);
	public static event SwipeEvent OnSwipe;

	Vector2 startPos;
	float startTime;

    public void Update()
	{
		
		if (Input.touches.Length > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
				startTime = Time.time;
			}
			if (t.phase == TouchPhase.Ended)
			{
				if (Time.time - startTime > MAX_SWIPE_TIME) // press too long
					return;

				Vector2 endPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

				Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

				if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
					return;

				if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
				{ // Horizontal swipe
					if (swipe.x > 0)
					{
						OnSwipe?.Invoke(Vector2.right);
					}
					else
					{
						OnSwipe?.Invoke(Vector2.left);
					}
				}
				else
				{ // Vertical swipe
					if (swipe.y > 0)
					{
						OnSwipe?.Invoke(Vector2.up);
					}
					else
					{
						OnSwipe?.Invoke(Vector2.down);
					}
				}
			}
		}

		if (debugWithArrowKeys)
		{
			if (Input.GetKeyDown(KeyCode.DownArrow)) OnSwipe?.Invoke(Vector2.down);
			if (Input.GetKeyDown(KeyCode.UpArrow)) OnSwipe?.Invoke(Vector2.up);
			if (Input.GetKeyDown(KeyCode.RightArrow)) OnSwipe?.Invoke(Vector2.right);
			if (Input.GetKeyDown(KeyCode.LeftArrow)) OnSwipe?.Invoke(Vector2.left);
		}
	}
}
