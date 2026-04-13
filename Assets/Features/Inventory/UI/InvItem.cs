using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemData data;
    private InvSlot currentSlot;
    private Image image;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform originalParent;

    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(ItemData itemData, InvSlot slot)
    {
        data = itemData;
        currentSlot = slot;

        if (data != null)
        {
            if(image == null)
                image = GetComponent<Image>();
            
            image.sprite = data.sprite;
        }
    }

    public ItemData GetData()
    {
        return data;
    }

    public InvSlot GetCurrentSlot()
    {
        return currentSlot;
    }

    public void SetSlot(InvSlot slot)
    {
        currentSlot = slot;
        transform.SetParent(slot.transform);
        transform.localPosition = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;
        originalParent = transform.parent;
        
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InvSlot>() == null)
        {
            ReturnToOriginalPosition();
        }
        else
        {
            transform.SetParent(currentSlot.transform);
            transform.localPosition = Vector2.zero;
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(currentSlot.transform);
        transform.localPosition = Vector2.zero;
    }
}