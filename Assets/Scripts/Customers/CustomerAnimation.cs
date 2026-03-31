using UnityEngine;
using Pathfinding;

public class CustomerAnimation : MonoBehaviour
{
    private Animator mAnimator;
    private AILerp aiLerp;
    private Vector2 lastPosition;

    private void Awake()
    {
        mAnimator = GetComponent<Animator>();
        aiLerp = GetComponent<AILerp>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        SetAnimation();
        CheckMovement();
        lastPosition = transform.position;
    }

    private void SetAnimation()
    {
        Vector2 movement = (Vector2)transform.position - lastPosition;

        if (movement != Vector2.zero)
        {
            mAnimator.SetFloat("X", movement.x);
            mAnimator.SetFloat("Y", movement.y);
        }
    }

    private void CheckMovement()
    {
        Vector2 movement = (Vector2)transform.position - lastPosition;
        bool isMoving = movement.sqrMagnitude > 0.001f;
        mAnimator.SetBool("isWalk", isMoving);
    }
}
