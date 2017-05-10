using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemScript : MonoBehaviour, IPointerClickHandler
{
    private GameObject invenPanel;
    public static GameObject selectedItem;
    public static IntVector2 selectedItemSize;
    public static bool isDragging = false;
    
    private float slotSize;


    public ItemClass item;

    private void Awake()
    {
        invenPanel = GameObject.FindGameObjectWithTag("InvenPanel");
        slotSize = invenPanel.GetComponent<InvenGridScript>().slotSize;
    }


    public void SetItemEquipSize(IntVector2 size)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x * slotSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y * slotSize);
    }

    //becomes obsolete later
    public void OnPointerClick(PointerEventData eventData)
    {
        SetSelectedItem(this.gameObject);
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        canvas.blocksRaycasts = false;
        canvas.alpha = 0.5f;
        transform.SetParent(GameObject.Find("DragParent").transform);
    }

    private void Update()
    {
        if (isDragging)
        {
            selectedItem.transform.position = Input.mousePosition;
        }
    }

    public static void SetSelectedItem(GameObject obj)
    {
        selectedItem = obj;
        selectedItemSize = obj.GetComponent<ItemScript>().item.size;
        isDragging = true;
    }

    public static void ResetSelectedItem()
    {
        selectedItem = null;
        selectedItemSize = IntVector2.Zero;
        isDragging = false;
    }
}
