using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Unit))]
public class UseComponent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float useDist = 6.0f;

    private Unit owner;
    private ItemData currentItem;

    void Awake()
    {
        owner = GetComponent<Unit>();
    }

    public void SetItemToUse(ItemData item)
    {
        currentItem = item;
    }

    void Update()
    {
        if(currentItem != null)
        {
            spriteRenderer.sprite = currentItem.sprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }

    public void Use(Vector3 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(Vector2.Distance(pos, transform.position) > useDist)
        {
            return;
        }

        if(currentItem == null)
        {
            ProcessNoItemUse(pos);
        }
        else
        {
            ProcessItemUse(currentItem, pos);
        }
    }

    private void ProcessItemUse(ItemData item, Vector3 pos)
    {
        if(item.usableType == UsableType.None) return;

        var logic = UsableLogicFactory.Instance.GetLogic(item.usableType, item);
        if(logic == null) return;

        Usable usable = GetUsableUnderMouse(pos);
        logic.SetUsable(usable);
        logic.SetPos(pos);
        logic.SetOwnerUnit(owner);


        logic.Use();
    }

    private void ProcessNoItemUse(Vector3 pos)
    {
        
    }

    private Usable GetUsableUnderMouse(Vector3 mousePosition)
    {
        Vector2 worldPoint = mousePosition;
        
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        
        if (hit.collider != null)
        {
            Usable usable = hit.collider.GetComponent<Usable>();
            
            if (usable == null)
            {
                usable = hit.collider.GetComponentInParent<Usable>();
            }
            
            return usable;
        }
        
        return null;
    }
}
