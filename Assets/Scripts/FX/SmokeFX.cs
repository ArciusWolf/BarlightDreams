using UnityEngine;

public class SmokeFX : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Reset anim to start
        animator.Play(0, -1, 0f);
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
