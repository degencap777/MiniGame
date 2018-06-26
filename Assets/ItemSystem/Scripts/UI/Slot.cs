using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    private GameObject itemPrefab;
    private Image filledImage;
    private GameObject pickedImage;
    private bool inColding = false;
    private float timer = 0;
    private float coldTime = 0;
    private bool hasItem = false;

    

    private Image itemImage;
    private Text amountText;

    private Knapsack knapsack;
    
    
    private Vector3 targetScale = new Vector3(1.0f, 1.0f, 1.0f);
    private readonly Vector3 animationScale = new Vector3(1.2f, 1.2f, 1.2f);
    private float smoothing = 4f;
    

    public Button Button { get; set; }

    public int Amount { get; set; }

    public Item Item { get; set; }

    void Start()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
        filledImage = transform.Find("FilledImage").GetComponent<Image>();
        pickedImage = transform.Find("PickedImage").gameObject;

        

        knapsack = transform.parent.parent.GetComponent<Knapsack>();

        itemPrefab = transform.Find("Item").gameObject;
        itemImage = itemPrefab.GetComponent<Image>();
        amountText = itemPrefab.GetComponentInChildren<Text>();

        Button.interactable = false;
        Hide();
    }

    void Update()
    {
        if (inColding )
        {
            timer += Time.deltaTime;
            filledImage.fillAmount = (coldTime - timer) / coldTime;
            if (timer >= coldTime)
            {
                filledImage.fillAmount = 0;
                timer = 0;
                inColding = false;
                SetButtonActive();
            }
        }
        if (itemPrefab.transform.localScale != targetScale)
        {
            float scale = Mathf.Lerp(itemPrefab.transform.localScale.x, targetScale.x, Time.deltaTime * smoothing);
            itemPrefab.transform.localScale = new Vector3(scale, scale, scale);
            if (Mathf.Abs(itemPrefab.transform.localScale.x - targetScale.x) < 0.02f)
            {
                itemPrefab.transform.localScale = targetScale;
            }
        }
        
    }

    public void OnTipClick()
    {
        pickedImage.SetActive(false);
    }
    public void SetButtonActive()
    {
        if (Amount != 0)
            Button.interactable = true;
    }
    public void AddItem(Item item, int amount = 1)
    {
        Show();
        Button.interactable = true;
        if (this.Amount == 0)
        {
            itemPrefab.transform.localScale = animationScale;
            this.Item = item;
            this.Amount = amount;
            //update UI
            itemImage.sprite = item.Sprites[0];
            amountText.text = amount.ToString();
        }
        else
        {
            AddAmount(amount);
        }
    }
    private void AddAmount(int amount = 1)
    {
        float capacity;
        Item.parameters.TryGetValue("Capacity", out capacity);
        capacity = capacity == 0 ? 1 : capacity;
        if (capacity >= this.Amount + amount)
        {
            itemPrefab.transform.localScale = animationScale;
            this.Amount += amount;
            amountText.text = this.Amount.ToString();
        }
    }

    private void OnClick()
    {
        pickedImage.SetActive(true);
        knapsack.OnSlotClick(this);
    }

    public void Clear()
    {
        Amount = 0;
        Item = null;
        Button.interactable = false;
        coldTime = 0;
        pickedImage.SetActive(false);
        amountText.text = "";
        Hide();
    }

    public void UseItem(int amount = 1)
    {
        pickedImage.SetActive(false);
        itemPrefab.transform.localScale = animationScale;

        coldTime = Item.parameters.TryGet("CD");
        if (coldTime != 0)
            Button.interactable = false;

        this.Amount -= amount;
        if (this.Amount > 1)
        {
            inColding = true;
            amountText.text = this.Amount.ToString();
        }
        else
        {
            amountText.text = "";
            if (this.Amount <= 0)
            {
                Hide();
                coldTime = 0;
                this.Item = null;
                this.Amount = 0;
                Button.interactable = false;
            }
            else
            {
                inColding = true;
            }
        }
    }

    public void StopUseItem()
    {
        pickedImage.SetActive(false);
        Button.interactable = true;
    }
    public void Show()
    {
        itemPrefab.SetActive(true);
    }

    public void Hide()
    {
        itemPrefab.SetActive(false);
    }

    
}
