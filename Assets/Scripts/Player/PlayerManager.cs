using DamageNumbersPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Effect Prefabs")]
    public GameObject moneyEffect;
    public GameObject loseLifeEffect;

    public GameObject moneyEffectPrefab;
    public GameObject loseLifeEffectPrefab;

    // Singleton instance
    public static PlayerManager Instance { get; private set; }

    public DamageNumber playerPopup;
    public List<IngredientSO> selectedIngredients = new List<IngredientSO>();
    public DrinkRecipeSO currentMix;
    public MysteryDrinkSO mysteryDrinkReference;
    public int currentLife;
    [SerializeField] private int _moneyHeld = 0;
    public int moneyHeld
    {
        get => _moneyHeld;
        set
        {
            _moneyHeld = value;
            PlayerUI.Instance?.UpdateMoneyText();
        }
    }

    public int MaxLife
    {
        get
        {
            if (PlayerUpgradeManager.Instance == null) return 3;
            return 3 + Mathf.RoundToInt(PlayerUpgradeManager.Instance.GetValue(UpgradeType.MaxLife));
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentLife = MaxLife;
        PlayerUI.Instance?.InitLivesDisplay(currentLife);
        moneyHeld = _moneyHeld;
    }

    public bool TrySpendMoney(int amount)
    {
        if (moneyHeld >= amount)
        {
            moneyHeld -= amount;
            return true;
        }
        NoticeUI.Instance.ShowNotice("Not enough money to upgrade!");
        return false;
    }

    public void AddMoney(int amount)
    {
        moneyHeld += amount;
        PlayEffect(moneyEffect);
        SoundManager.Instance.PlayMoneySound();
    }

    public void LoseLife()
    {
        if (currentLife <= 0) return;

        currentLife = Mathf.Max(0, currentLife - 1);
        PlayEffect(loseLifeEffect);
        PlayerUI.Instance?.LoseLifeDisplay();
        SoundManager.Instance.PlayLoseLifeSound();

        if (currentLife == 0)
        {
            GameOverUI.Instance.Show();
        }
    }


    public void RecalculateMaxLife()
    {
        currentLife = 3; // base life
        if (PlayerUpgradeManager.Instance != null)
        {
            int bonus = Mathf.RoundToInt(PlayerUpgradeManager.Instance.GetValue(UpgradeType.MaxLife));
            currentLife += bonus;
        }

        PlayerUI.Instance?.InitLivesDisplay(currentLife);
    }
    public void PlayEffect(GameObject effectObject)
    {
        if (effectObject == null) return;

        GameObject fx = effectObject;

        if (fx == null || !fx)
        {
            if (effectObject == moneyEffect && moneyEffectPrefab != null)
            {
                fx = Instantiate(moneyEffectPrefab, transform);
                moneyEffect = fx;
            }
            else if (effectObject == loseLifeEffect && loseLifeEffectPrefab != null)
            {
                fx = Instantiate(loseLifeEffectPrefab, transform);
                loseLifeEffect = fx;
            }
            else
            {
                return;
            }
        }

        fx.SetActive(true);

        Transform tf = fx.transform;
        Vector3 originalPos = tf.localPosition;

        Sequence seq = DOTween.Sequence();
        seq.Append(tf.DOLocalMoveY(originalPos.y + 1f, 0.5f).SetEase(Ease.OutBack))
           .AppendInterval(0.2f)
           .OnComplete(() =>
           {
               fx.SetActive(false);
               tf.localPosition = originalPos;
           });
    }

}
