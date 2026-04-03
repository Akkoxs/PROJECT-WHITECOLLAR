using UnityEngine;

public class ChargeBarUI : MonoBehaviour
{
    [SerializeField] PlayerControllerUpper upperBody;
    [SerializeField] GameObject barRoot;           // the whole UI panel
    [SerializeField] RectTransform chargeIndicator; // the moving pip
    [SerializeField] float barHeight = 200f;        // total pixel height of the bar in your canvas

    void Update()
    {
        barRoot.SetActive(upperBody.AttackHeld);

        float yPos = upperBody.CurrentCharge * barHeight;
        chargeIndicator.anchoredPosition = new Vector2(chargeIndicator.anchoredPosition.x, yPos);
    }
}
