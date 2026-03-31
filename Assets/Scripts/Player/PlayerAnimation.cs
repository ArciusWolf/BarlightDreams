using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerMovement mMovement;
    private Animator mAnimator;

    private void Awake()
    {
        mMovement = GetComponent<PlayerMovement>();
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        SetAnimation();
        checkMovement();
    }

    private void SetAnimation()
    {
        if (mMovement.movementInput != Vector2.zero)
        {
            mAnimator.SetFloat("XInput", mMovement.movementInput.x);
            mAnimator.SetFloat("YInput", mMovement.movementInput.y);
        }
    }

    private void checkMovement()
    {
        if (mMovement.movementInput != Vector2.zero)
        {
            mAnimator.SetBool("isWalk", true);
        } else
        {
            mAnimator.SetBool("isWalk", false);
        }
    }
}