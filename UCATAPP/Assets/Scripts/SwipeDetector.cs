using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    public float minSwipeDistance = 50f; // Minimum distance for a valid swipe
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    public delegate void SwipeAction();
    public event SwipeAction OnSwipeLeft;
    public event SwipeAction OnSwipeRight;

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
                    OnSwipeRight?.Invoke();
                }
                else
                {
                    OnSwipeLeft?.Invoke();
                }
            }
        }
    }
}
