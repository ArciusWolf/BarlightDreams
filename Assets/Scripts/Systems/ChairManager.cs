using UnityEngine;
using System.Collections.Generic;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance { get; private set; }

    public List<Chair> chairs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Multiple instances of ChairManager detected. Destroying duplicate instance.");
            Destroy(gameObject);
        }
    }
}
