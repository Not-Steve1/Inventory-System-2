using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenGridManager : MonoBehaviour {

    public GameObject[,] slotGrid;
    public GameObject highlightedSlot;
    public Transform dropParent;
    public Transform dragParent;
    [HideInInspector]
    public IntVector2 gridSize;

    public ItemListManager listManager;
    public GameObject selectedButton;

    private IntVector2 totalOffset, checkSize, checkStartPos;
    private IntVector2 otherItemPos, otherItemSize; //*3

    private int checkState;
    private bool isOverEdge = false;

    /* to do list
     * make the ColorChangeLoop on swap items take arrguements fron the other item, not hte private variables *1
     * transfer the CheckArea() into inside RefreshColor() *2
     * have *3 be local variables of CheckArea(). SwapItem() uses the variable, may need to rewrite.
     */
    private void Start()
    {
        ItemButtonScript.invenManager = this;
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (highlightedSlot != null && ItemScript.selectedItem != null && !isOverEdge)
            {
                switch (checkState)
                {
                    case 0: //store on empty slots
                        StoreItem(ItemScript.selectedItem);
                        ColorChangeLoop(ColorHighlights.Blue, ItemScript.selectedItemSize, totalOffset);
                        ItemScript.ResetSelectedItem();
                        ResetSelectedButton();
                        break;
                    case 1: //swap items
                        ItemScript.SetSelectedItem(SwapItem(ItemScript.selectedItem));
                        SlotSectorScript.sectorScript.PosOffset();
                        ColorChangeLoop(Color.white, otherItemSize, otherItemPos); //*1
                        RefrechColor(true);
                        ResetSelectedButton();
                        break;
                }
            }// retrieve items
            else if (highlightedSlot != null && ItemScript.selectedItem == null && highlightedSlot.GetComponent<SlotScript>().isOccupied == true)
            {
                ColorChangeLoop(Color.white, highlightedSlot.GetComponent<SlotScript>().storedItemSize, highlightedSlot.GetComponent<SlotScript>().storedItemStartPos);
                ItemScript.SetSelectedItem(GetItem(highlightedSlot));
                SlotSectorScript.sectorScript.PosOffset();
                RefrechColor(true);
            }
        }
    }

    private void CheckArea(IntVector2 itemSize) //*2
    {
        IntVector2 halfOffset;
        IntVector2 overCheck;
        halfOffset.x = (itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2;
        halfOffset.y = (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2;
        totalOffset = highlightedSlot.GetComponent<SlotScript>().gridPos - (halfOffset + SlotSectorScript.posOffset);
        checkStartPos = totalOffset;
        checkSize = itemSize;
        overCheck = totalOffset + itemSize;
        isOverEdge = false;
        //checks if item to stores is outside grid
        if (overCheck.x > gridSize.x)
        {
            checkSize.x = gridSize.x - totalOffset.x;
            isOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            checkSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            isOverEdge = true;
        }
        if (overCheck.y > gridSize.y)
        {
            checkSize.y = gridSize.y - totalOffset.y;
            isOverEdge = true;
        }
        if (totalOffset.y < 0)
        {
            checkSize.y = itemSize.y + totalOffset.y;
            checkStartPos.y = 0;
            isOverEdge = true;
        }
    }
    private int SlotCheck(IntVector2 itemSize)
    {
        GameObject obj = null;
        SlotScript instanceScript;
        if (!isOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    instanceScript = slotGrid[checkStartPos.x + x, checkStartPos.y + y].GetComponent<SlotScript>();
                    if (instanceScript.isOccupied)
                    {
                        if (obj == null)
                        {
                            obj = instanceScript.storedItem;
                            otherItemPos = instanceScript.storedItemStartPos;
                            otherItemSize = obj.GetComponent<ItemScript>().item.size;
                        }
                        else if (obj != instanceScript.storedItem)
                            return 2; // if cheack Area has 1+ item occupied
                    }
                }
            }
            if (obj == null)
                return 0; // if checkArea is not accupied
            else
                return 1; // if checkArea only has 1 item occupied
        }
        return 2; // check areaArea is over the grid
    }
    public void RefrechColor(bool enter)
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize);
            checkState = SlotCheck(ItemScript.selectedItemSize);
            switch (checkState)
            {
                case 0: ColorChangeLoop(ColorHighlights.Green, checkSize, checkStartPos); break; //no item in area
                case 1:
                    ColorChangeLoop(ColorHighlights.Yellow, otherItemSize, otherItemPos); //1 item on area and can swap
                    ColorChangeLoop(ColorHighlights.Green, checkSize, checkStartPos);
                    break;
                case 2: ColorChangeLoop(ColorHighlights.Red, checkSize, checkStartPos); break; //invalid position (more then 2 items in area or area is outside grid)
            }
        }
        else //on pointer exit. resets colors to white
        {
            isOverEdge = false;
            //checkArea(); //commented out for performance. may cause bugs if not included
            ColorChangeLoop(Color.white, checkSize, checkStartPos);
            if (checkState == 1)
            {
                ColorChangeLoop(Color.white, otherItemSize, otherItemPos);
            }
        }
        
    }
    public void ColorChangeLoop(Color32 color, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = color;
            }
        }
    }
    private void StoreItem(GameObject item)
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = item.GetComponent<ItemScript>().item.size;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                //set each slot parameters
                instanceScript = slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<SlotScript>();
                instanceScript.storedItem = item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = totalOffset;
                instanceScript.isOccupied = true;
                slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<Image>().color = Color.white;
            }
        }//set dropped parameters
        item.transform.SetParent(dropParent);
        item.GetComponent<RectTransform>().pivot = Vector2.zero;
        item.transform.position = slotGrid[totalOffset.x, totalOffset.y].transform.position;
        item.GetComponent<CanvasGroup>().alpha = 0.75f;
    }
    private GameObject GetItem(GameObject slotObject)
    {
        SlotScript slotObjectScript = slotObject.GetComponent<SlotScript>();
        GameObject retItem = slotObjectScript.storedItem;
        IntVector2 tempItemPos = slotObjectScript.storedItemStartPos;
        IntVector2 itemSizeL = retItem.GetComponent<ItemScript>().item.size;

        SlotScript instanceScript;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                //reset each slot parameters
                instanceScript = slotGrid[tempItemPos.x + x, tempItemPos.y + y].GetComponent<SlotScript>();
                instanceScript.storedItem = null;
                instanceScript.storedItemSize = IntVector2.Zero;
                instanceScript.storedItemStartPos = IntVector2.Zero;
                instanceScript.isOccupied = false;
            }
        }//returned item set item parameters
        retItem.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        retItem.GetComponent<CanvasGroup>().alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        retItem.transform.SetParent(dragParent);
        return retItem;
    }
    private GameObject SwapItem(GameObject item)
    {
        GameObject tempItem;
        tempItem = GetItem(slotGrid[otherItemPos.x, otherItemPos.y]);
        StoreItem(item);
        return tempItem;
    }

    private void ResetSelectedButton()
    {
        if (selectedButton != null)
        {
            listManager.RemoveButton(selectedButton);
            selectedButton = null;
        }
    }
    
}



public struct ColorHighlights
{
    public static Color32 Green
    { get { return new Color32(128, 255, 128, 255); } }
    public static Color32 Yellow
    { get { return new Color32(255, 255, 64, 255); } }
    public static Color32 Red
    { get { return new Color32(255, 128, 128, 255); } }
    public static Color32 Blue
    { get { return new Color32(192, 192, 255, 255); } }
}
