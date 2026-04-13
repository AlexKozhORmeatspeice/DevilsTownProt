using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriceItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text marketPriceText;
    [SerializeField] private TMP_Text demandText;

    [SerializeField] private TMP_Text playerPriceTxt;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button setPriceBtn;


    private ItemData itemData;

    void OnDestroy()
    {
        setPriceBtn.onClick.RemoveAllListeners();
    }

    public void SetData(ItemData item)
    {
        setPriceBtn.onClick.RemoveListener(OnSetPriceClicked);

        itemData = item;
        
        UpdateData();

        setPriceBtn.onClick.AddListener(OnSetPriceClicked);
    }

    void Update()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        if(itemData == null) return;

        icon.sprite = itemData.sprite;
        marketPriceText.text = $"Рыночная цена: {PriceSystem.instance.GetMarketPrice(itemData):0.##} $";
        demandText.text = $"Спрос: {PriceSystem.instance.GetMarketDemand(itemData):0.##}";

        playerPriceTxt.text = PriceSystem.instance.GetPlayerPrice(itemData).ToString() + " $";
        //inputField.text = PriceSystem.instance.GetPlayerPrice(itemData).ToString("0.##");
    }

    private void OnSetPriceClicked()
    {
        if(float.TryParse(inputField.text, out float newPrice))
        {
            if(newPrice < 0)
            {
                Debug.LogWarning("Цена не может быть отрицательной");
                return;
            }

            PriceSystem.instance.SetPlayerPrice(itemData, newPrice);
            playerPriceTxt.text = PriceSystem.instance.GetPlayerPrice(itemData).ToString();
            inputField.text = "";
        }
        else
        {
            Debug.LogWarning("Некорректный ввод цены");
        }
    }
}
