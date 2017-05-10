using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class ItemListManager : MonoBehaviour {

    public ObjectPoolScript itemButtonPool;
    public ObjectPoolScript itemEquipPool;
    public InvenGridManager invenManger;

    public Sprite[] itemIconArr;
    public List<ItemClass> itemList;

    private Transform contentPanel;

    private void Start()
    {
        contentPanel = this.transform;
        RefreshList();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > GetComponent<Transform>().position.x) // drop item back
        {
            if (invenManger.selectedButton == null && ItemScript.selectedItem != null) //dropping back from ivenGrid
            {
                itemList.Add(ItemScript.selectedItem.GetComponent<ItemScript>().item);
                AddButton(ItemScript.selectedItem.GetComponent<ItemScript>().item);// shorten later
                itemEquipPool.ReturnObject(ItemScript.selectedItem);
                ItemScript.ResetSelectedItem();
            } 
        }

        if (Input.GetMouseButtonDown(1) && invenManger.selectedButton != null) //delesect selected item and button  by right-click
        {
            invenManger.RefrechColor(false);// not refresh color to blue when mouse is non top of occupied slot after putting back itemEquip
            invenManger.selectedButton.GetComponent<CanvasGroup>().alpha = 1f;
            invenManger.selectedButton = null;
            itemEquipPool.ReturnObject(ItemScript.selectedItem);
            ItemScript.ResetSelectedItem();

        }
    }



    private void RefreshList()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            AddButton(itemList[i]);
        }
    }

    private void AddButton(ItemClass addItem)//may add a list variable when sort list is added later
    {
        GameObject newButton = itemButtonPool.GetObject();
        newButton.transform.SetParent(contentPanel);
        newButton.GetComponent<RectTransform>().localScale = Vector3.one;
        newButton.GetComponent<ItemButtonScript>().SetUpButton(addItem, this);
    }

    public void RemoveButton(GameObject buttonObj)
    {
        buttonObj.GetComponent<CanvasGroup>().alpha = 1f;
        itemButtonPool.ReturnObject(buttonObj);
        RevomeItemFromList(buttonObj.GetComponent<ItemButtonScript>().item);
    }

    public void RevomeItemFromList(ItemClass itemToRemove)//may add a list variable when sort list is added
    {
        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            if (itemList[i] == itemToRemove)
            {
                itemList.RemoveAt(i);
                break;//temporary for now
            }
        }
    }

    public Sprite SetIconSprite(string s) // add better icons
    {
        switch (s)
        {
            case "Spear": return itemIconArr[1];
            case "Armor": return itemIconArr[2];
            case "Boots": return itemIconArr[3];
            case "Belt": return itemIconArr[4];
            case "Dagger": return itemIconArr[5];
            case "Great Sword": return itemIconArr[6];
            case "Mace": return itemIconArr[7];
            case "Axe": return itemIconArr[8];
            case "Ring": return itemIconArr[9];

            default: return itemIconArr[0];
        }
    }
}
