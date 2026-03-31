using UnityEngine;

public class EffectAssigner : MonoBehaviour
{
    public GameObject moneyEffect;
    public GameObject loseLifeEffect;

    void Start()
    {
        var pm = PlayerManager.Instance;
        if (pm != null)
        {
            pm.moneyEffect = moneyEffect;
            pm.loseLifeEffect = loseLifeEffect;
            Debug.Log("Assigned effects to PlayerManager.");
        }
        else
        {
            Debug.LogWarning("PlayerManager.Instance not found.");
        }
    }
}
