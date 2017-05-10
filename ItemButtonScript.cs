using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemButtonScript : MonoBehaviour {

    public Button buttonComponent;
    public Text nameText;
    public Image iconImage;
    public Text LvlText;
    public Text QualityText;

    public ItemClass item;
    private Sprite iconSprite;
    private ItemListManager listManager;
    public ObjectPoolScript itemEquipPool;

    public static InvenGridManager invenManager;

    private void Start()
    {
        buttonComponent.onClick.AddListener(SpawnStoredItem);
    }

    private void SpawnStoredItem()
    {
        if (ItemScript.selectedItem != null) //selecting another button when already a selected button
        {
            invenManager.selectedButton.GetComponent<CanvasGroup>().alpha = 1f;
            listManager.itemEquipPool.ReturnObject(ItemScript.selectedItem);
        }
        GameObject newItem = itemEquipPool.GetObject();
        newItem.GetComponent<ItemScript>().item = item;
        newItem.GetComponent<ItemScript>().SetItemEquipSize(item.size);
        newItem.GetComponent<Image>().sprite = iconSprite;
        newItem.transform.SetParent(GameObject.FindGameObjectWithTag("DragParent").transform);
        newItem.GetComponent<RectTransform>().localScale = Vector3.one;

        ItemScript.SetSelectedItem(newItem);
        invenManager.selectedButton = this.gameObject;

        GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    public void SetUpButton(ItemClass passedItem, ItemListManager passedListManager)
    {
        listManager = passedListManager;
        item = passedItem;
        nameText.text = item.itemType;
        LvlText.text = "Lvl: " + item.level.ToString();
        QualityText.text = item.QualityIntToString();
        GetComponent<LayoutElement>().preferredHeight = transform.parent.GetComponent<RectTransform>().rect.width / 4;
        iconSprite = listManager.SetIconSprite(item.itemType);
        iconImage.sprite = iconSprite;
        itemEquipPool = passedListManager.itemEquipPool;
    }
    
    
}
