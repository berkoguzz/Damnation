using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana Settings")]
    public float maxMana = 1000f;
    public float currentMana;

    [Header("UI")]
    public Image manaFill;

    [Header("Regen Settings")]
    public float regenRate = 200f;
    public float fillAnimationSpeed = 50f;

    private float displayMana;

    void Start()
    {
        currentMana = maxMana;
        displayMana = maxMana;

        if (manaFill == null)
            Debug.LogError("❌ ManaFill Image assign edilmedi!");
    }

    void Update()
    {
        RegenMana();
        UpdateUI();
    }

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }

        return false;
    }

    void RegenMana()
    {
        currentMana = Mathf.Clamp(currentMana + regenRate * Time.deltaTime, 0, maxMana);
    }

    void UpdateUI()
    {
        if (manaFill != null)
        {
            displayMana = Mathf.Lerp(displayMana, currentMana, fillAnimationSpeed * Time.deltaTime);
            manaFill.fillAmount = displayMana / maxMana;
        }
    }
}
