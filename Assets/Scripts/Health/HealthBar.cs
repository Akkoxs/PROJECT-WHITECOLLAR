using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider; 
    [SerializeField] private Health health; 
    [SerializeField] private Image barFill;
    [SerializeField] private float flashDuration = 0.2f;

    private Color flashColor = Color.white; 
    private Color regColor;

    public void Awake()
    {
        regColor = barFill.color;
    }

    private void OnEnable()
    {
        UpdateHealth(health.CurrentHealth, health.MaxHealth);
        barFill.color = regColor;
        health.healthChanged.AddListener(UpdateHealth);
    }

    private void OnDisable()
    {
        health.healthChanged.RemoveListener(UpdateHealth);   
    }

    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        
        //PLAN
        //Build a prefab called HealthPoint, with the border image, filled spirte, empty sprite everything, also a SetFilled boolean method if its filled or not 
        //From health bar create a bunch of these healthPoints prefabs into a horizontalLayout component which will automatically handle spacing 
        
        




        //StartCoroutine(uiHelper.BarFlash(flashDuration, flashColor, regColor, barFill));
    }

    //show your current health and then have it fade away after a certain period
    private void ShowHealth()
    {

    }
}