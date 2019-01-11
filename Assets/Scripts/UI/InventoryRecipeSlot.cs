using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRecipeSlot : MonoBehaviour {

    public InventoryRecipe recipe;
    public bool hasRecipe;

    private InventoryUI ui;
    private Button craftButton;
    private Button button;
    private Image image;
    private Text title;
    private Text toolTipText;
    private Character character;
    private bool hovered = false;

    private void Start()
    {
        ui = transform.parent.parent.parent.parent.GetComponent<InventoryUI>();
        image = transform.Find("SlotButton").Find("Image").GetComponent<Image>();
        craftButton = transform.Find("CraftButton").GetComponent<Button>();
        button = transform.Find("SlotButton").GetComponent<Button>();
        craftButton.onClick.AddListener(CraftItem);
        title = transform.Find("Label").GetComponent<Text>();
        toolTipText = transform.parent.parent.Find("ToolTip").GetComponent<Text>();
        character = GameManager.instance.playerCharacter;
    }

    private void Update()
    {
        if (recipe != null && hovered)
        {
            toolTipText.text = GetRecipeText();
        }
        else
        {
            if(toolTipText.text == GetRecipeText())
            {
                toolTipText.text = "";
            }
        }
    }

    public void CraftItem()
    {
        foreach(string type in recipe.ingredients)
        {
            character.InventoryDrop(type, true);
        }

        GameObject item = Instantiate(recipe.prefab);
        item.transform.position = character.gameObject.transform.position + character.transform.forward;

        ui.OnItemCrafted();
    }

    public void HoverStart()
    {
        hovered = true;
    }

    public void HoverEnd()
    {
        hovered = false;
    }

    public void UpdateSlot()
    {
        if (hasRecipe && image != null)
        {
            image.gameObject.SetActive(true);
            craftButton.gameObject.SetActive(true);
            image.sprite = recipe.icon;            
            title.text = recipe.name;

            if (CanCraft())
            {
                craftButton.interactable = true;
            }
            else
            {
                craftButton.interactable = false;
            }
        }
        else if(image != null)
        {
            image.gameObject.SetActive(false);
            craftButton.gameObject.SetActive(false);
            title.text = "";
        }
    }

    public bool CanCraft()
    {
        if(recipe == null) { return false; }

        foreach(string name in recipe.ingredients)
        {
            if(character.GetInventoryItemCount(name) <= 0) { return false; }
        }

        return true;
    }

    public string GetRecipeText()
    {
        if(recipe == null) { return ""; }

        string text = "Recipe: \n";

        foreach(string name in recipe.ingredients)
        {
            text = text + name + "\n";
        }

        return text;
    }
}
