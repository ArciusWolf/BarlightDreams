using UnityEngine;
using System.Collections;
using System.Collections.*;
using Pathfinding;
using DamageNumbersPro;

public class CustomerCore : MonoBehaviour
{
    [Header("Customer Type")]
    [SerializeField] private CustomerType customerType;

    [Header("Pathfinding Components")]
    private AILerp aiPath;
    private AIDestinationSetter destinationSetter;
    private Chair assignedChair;
    private bool isSeated = false;

    [Header("References")]
    private CustomerOrder customerOrder;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Popup")]
    public DamageNumber popupResponse;

    [Header("Customer Attributes")]
    [SerializeField] private float interactTimeLimit = 60f;
    [SerializeField] private float orderTimeLimit = 60f;

    [Header("Timer")]
    [SerializeField] private CustomerTimer timer;

    private bool hasOrdered = false;

    private void Awake()
    {
        aiPath = GetComponent<AILerp>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        customerOrder = GetComponent<CustomerOrder>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        customerOrder.SetCore(this);

        if (timer == null)
        {
            timer = gameObject.AddComponent<CustomerTimer>();
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        int currentDay = 1;
        if (TimeSystem.Instance != null)
        {
            currentDay = TimeSystem.Instance.CurrentDay;
        }

        float vipChance = Mathf.Min(currentDay * 0.02f, 0.20f);
        float specialChance = Mathf.Min(currentDay * 0.01f, 0.10f);
        float regularChance = 1f - vipChance - specialChance;

        float roll = Random.value;
        if (roll < regularChance)
        {
            customerType = CustomerType.Regular;
        }
        else if (roll < regularChance + vipChance)
        {
            customerType = CustomerType.VIP;
        }
        else
        {
            customerType = CustomerType.Special;
        }

        isSeated = false;
        hasOrdered = false;
        FindChairAndMove();
    }

    private void OnDisable()
    {
        if (assignedChair != null)
        {
            assignedChair.FreeChair();
            assignedChair = null;
        }

        customerOrder.HideAllEmotions();
        aiPath.canMove = true;
        aiPath.enabled = true;
        destinationSetter.enabled = true;
        isSeated = false;
        customerOrder.active = false;
        spriteRenderer.sortingOrder = 0;

        if (customerOrder.thoughtBubble != null)
        {
            customerOrder.thoughtBubble.SetActive(false);
        }

        transform.position = CustomerManager.Instance.spawnPoint.position;
        timer.StopTimer(this);
    }

    private void Update()
    {
        if (!isSeated && assignedChair != null && aiPath.reachedDestination)
        {
            SitDown();
        }
    }

    private void FindChairAndMove()
    {
        if (ChairManager.Instance == null)
        {
            Debug.LogError("ChairManager is null!");
            return;
        }

        if (ChairManager.Instance.chairs == null)
        {
            Debug.LogError("Chair list is null!");
            return;
        }

        var chairs = new List<Chair>(ChairManager.Instance.chairs);
        int n = chairs.Count;

        for (int i = 0; i < n - 1; i++)
        {
            int j = Random.Range(i, n);
            Chair temp = chairs[i];
            chairs[i] = chairs[j];
            chairs[j] = temp;
        }

        for (int i = 0; i < chairs.Count; i++)
        {
            Chair chair = chairs[i];
            if (chair.ReserveChair())
            {
                assignedChair = chair;
                destinationSetter.target = chair.sitPoint;
                return;
            }
        }

        Debug.LogWarning("No available chair!");
    }

    private void SitDown()
    {
        isSeated = true;
        transform.position = assignedChair.sitPoint.position;

        aiPath.canMove = false;
        aiPath.enabled = false;
        destinationSetter.enabled = false;

        spriteRenderer.sortingOrder = 1;
        SetSitDirection();

        StartCoroutine(ShowBubbleAfterDelay(1f));
    }

    private void SetSitDirection()
    {
        int x = 0;
        int y = 0;

        if (assignedChair.direction == ChairDirection.Left)
        {
            x = 1;
            y = 0;
            animator.SetBool("isSit", true);
        }
        else if (assignedChair.direction == ChairDirection.Right)
        {
            x = -1;
            y = 0;
            animator.SetBool("isSit", true);
        }
        else if (assignedChair.direction == ChairDirection.Up)
        {
            x = 0;
            y = 1;
            spriteRenderer.sortingOrder = 0;
        }

        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }

    private IEnumerator ShowBubbleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (customerOrder != null)
        {
            customerOrder.ShowQuestionMark();
        }

        timer.StartInteractTimer(this, interactTimeLimit, OnInteractTimeout);
    }

    private void OnInteractTimeout()
    {
        if (customerOrder != null)
        {
            customerOrder.HandleFailure("Player didn't interact.");
        }

        LeaveChair();
    }

    public void BeginOrderCountdown()
    {
        hasOrdered = true;
        timer.StopTimer(this);
        timer.StartOrderTimer(this, orderTimeLimit, OnOrderTimeout);
    }

    private void OnOrderTimeout()
    {
        if (customerOrder != null && customerOrder.active)
        {
            customerOrder.HandleFailure("Customer waited too long for drink!");
        }

        LeaveChair();
    }

    private void LeaveChair()
    {
        if (assignedChair != null)
        {
            assignedChair.FreeChair();
            assignedChair = null;
        }

        customerOrder.active = false;
        animator.SetBool("isSit", false);
        animator.SetBool("isWalk", true);
        aiPath.enabled = true;
        aiPath.canMove = true;
        destinationSetter.enabled = true;
        isSeated = false;

        if (CustomerManager.Instance != null && CustomerManager.Instance.spawnPoint != null)
        {
            destinationSetter.target = CustomerManager.Instance.spawnPoint;
            StartCoroutine(ReturnToSpawnAndReset());
        }
        else
        {
            Debug.LogWarning("Spawn point not assigned in CustomerManager.");
            ResetCustomer();
        }
    }

    private IEnumerator ReturnToSpawnAndReset()
    {
        while (!aiPath.reachedDestination)
        {
            yield return null;
        }

        ResetCustomer();
    }

    private void ResetCustomer()
    {
        customerOrder.active = false;
        destinationSetter.target = null;
        CustomerManager.Instance.ReturnCustomerToPool(customerOrder);
    }

    public void LeaveAfterDelay(float delay)
    {
        StartCoroutine(LeaveAfterDelayRoutine(delay));
    }

    private IEnumerator LeaveAfterDelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        LeaveChair();
    }

    public CustomerType GetCustomerType()
    {
        return customerType;
    }

    public void HideAllOrderUI()
    {
        if (timer != null)
        {
            timer.StopTimer(this);
        }

        if (customerOrder != null)
        {
            if (customerOrder.thoughtBubble != null)
            {
                customerOrder.thoughtBubble.SetActive(false);
            }
        }
    }
}
