using UnityEngine;
public class SwipeDetection : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float minSwipeDistance = 50f; // Minimum distance for a valid swipe

    void Update()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    HandleSwipe();
                    break;
            }
        }
    }

    private void HandleSwipe()
    {
        float swipeDistance = Vector2.Distance(startTouchPosition, endTouchPosition);

        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDirection = endTouchPosition - startTouchPosition;
            float x = swipeDirection.x;

            if (Mathf.Abs(x) > Mathf.Abs(swipeDirection.y)) // Check horizontal swipe
            {
                if (x > 0)
                {
                    SwipeRight();
                }
                else
                {
                    SwipeLeft();
                }
            }
        }
    }

    private void SwipeRight()
    {
        Debug.Log("Swiped Right!");
        // Call your function here
    }

    private void SwipeLeft()
    {
        Debug.Log("Swiped Left!");
        // Call your function here
    }
}
