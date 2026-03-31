using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Pooling")]
    public List<GameObject> customerPrefabs;
    public Transform spawnPoint;
    public int poolSize;

    [Header("Difficulty Settings")]
    [SerializeField] private float baseSpawnRate = 8f;
    [SerializeField] private float spawnRateMultiplierPerDay = 0.92f;

    [SerializeField] private int baseMaxCustomers = 5; // Max customers at once on Day 1
    [SerializeField] private int additionalCustomersPerDay = 1; // Extra customers allowed per day

    [Header("Spawn Effect")]
    [SerializeField] private GameObject sharedSmokeEffect;

    private Coroutine spawnRoutine;
    private List<CustomerOrder> customerPool = new List<CustomerOrder>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TimeSystem.Instance.OnBarOpen += StartSpawning;
        TimeSystem.Instance.OnBarClosed += StopSpawning;
    }

    private void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnBarOpen -= StartSpawning;
            TimeSystem.Instance.OnBarClosed -= StopSpawning;
        }
    }

    private void InitializePool()
    {
        int prefabCount = customerPrefabs.Count;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = customerPrefabs[i % prefabCount];
            GameObject customerGO = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            customerGO.SetActive(false);

            CustomerOrder customerOrder = customerGO.GetComponent<CustomerOrder>();
            if (customerOrder != null)
            {
                customerPool.Add(customerOrder);
                customerGO.name = "Customer " + prefab.name;
            }
            else
            {
                Debug.LogWarning("Prefab does not have a CustomerOrder component attached.");
            }
        }
    }

    private IEnumerator EnableRandomCustomerRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            int currentDay = TimeSystem.Instance.CurrentDay;
            float effectiveRate = baseSpawnRate * Mathf.Pow(spawnRateMultiplierPerDay, currentDay - 1);

            float spawnDelay = Random.Range(effectiveRate, effectiveRate * 2f);
            yield return new WaitForSeconds(spawnDelay);

            int maxCustomers = baseMaxCustomers + additionalCustomersPerDay * (currentDay - 1);
            int currentCustomers = 0;

            foreach (var c in customerPool)
            {
                if (c.gameObject.activeInHierarchy)
                    currentCustomers++;
            }

            if (currentCustomers < maxCustomers)
            {
                CustomerOrder newCustomer = GetCustomerFromPool();
                if (newCustomer != null)
                {
                    newCustomer.gameObject.SetActive(true);
                    newCustomer.transform.position = spawnPoint.position;

                    if (sharedSmokeEffect != null)
                    {
                        sharedSmokeEffect.transform.position = spawnPoint.position;
                        sharedSmokeEffect.SetActive(true);
                    }
                }
            }
        }
    }


    private void StartSpawning()
    {
        spawnRoutine = StartCoroutine(EnableRandomCustomerRoutine());
    }

    private void StopSpawning()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    public CustomerOrder GetCustomerFromPool()
    {
        List<CustomerOrder> available = new List<CustomerOrder>();

        foreach (CustomerOrder customer in customerPool)
        {
            if (!customer.gameObject.activeInHierarchy)
                available.Add(customer);
        }

        if (available.Count == 0)
        {
            Debug.LogWarning("No available customers in pool.");
            return null;
        }

        return available[Random.Range(0, available.Count)];
    }


    public void ReturnCustomerToPool(CustomerOrder customer)
    {
        if (customer != null)
        {
            customer.gameObject.SetActive(false);
        }
    }

    public List<CustomerOrder> GetAllCustomers()
    {
        return customerPool;
    }
}
