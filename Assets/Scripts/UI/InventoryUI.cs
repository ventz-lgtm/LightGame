using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public static InventoryUI instance;

    public bool inventoryOpen = false;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip dropItemSound;
    public AudioClip craftSound;

    private GameObject slotsObject;
    private GameObject recipeSlotsObject;
    private Character player;
    private GameObject backgroundObject;
    private List<InventoryUISlot> slots;
    private List<InventoryRecipeSlot> recipeSlots;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        player = GameManager.instance.playerCharacter;
        player.onInventoryChanged += UpdateInventorySlots;
        player.onInventoryChanged += UpdateRecipeSlots;

        backgroundObject = transform.Find("Background").gameObject;

        slotsObject = backgroundObject.transform.Find("Slots").gameObject;
        slots = new List<InventoryUISlot>();
        for(int i = 0; i < slotsObject.transform.childCount; i++)
        {
            GameObject child = slotsObject.transform.GetChild(i).gameObject;
            InventoryUISlot slot = child.GetComponent<InventoryUISlot>();
            slot.slotIndex = slotsObject.transform.childCount - i - 1;
            slots.Insert(0, slot);
        }

        recipeSlotsObject = backgroundObject.transform.Find("Crafting").Find("RecipeSlots").gameObject;
        recipeSlots = new List<InventoryRecipeSlot>();
        for(int i = 0; i < recipeSlotsObject.transform.childCount; i++)
        {
            GameObject child = recipeSlotsObject.transform.GetChild(i).gameObject;
            InventoryRecipeSlot recipeSlot = child.GetComponent<InventoryRecipeSlot>();
            recipeSlots.Add(recipeSlot);
        }

        UpdateRecipeSlots();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryOpen)
            {
                AudioPlayer.instance.PlaySound(openSound, 0.03f, Random.Range(0.95f, 1.05f));
            }
            else
            {
                AudioPlayer.instance.PlaySound(closeSound, 0.03f, Random.Range(0.95f, 1.05f));
            }
        }

        if (inventoryOpen)
        {
            if (!backgroundObject.activeSelf)
            {
                backgroundObject.SetActive(true);
            }
        }
        else
        {
            if (backgroundObject.activeSelf)
            {
                backgroundObject.SetActive(false);
            }
        }
	}

    void UpdateInventorySlots()
    {
        int index = 0;
        foreach(InventoryUISlot slot in slots)
        {
            InventoryItemType itemType = player.InventoryItemAt(index);
            if(itemType == null)
            {
                slot.hasItem = false;
            }
            else
            {
                slot.itemType = itemType;
                slot.hasItem = true;
            }

            slot.UpdateSlot();
            index++;
        }
    }

    void UpdateRecipeSlots()
    {
        int index = 0;
        foreach(InventoryRecipeSlot recipeSlot in recipeSlots)
        {
            bool recipeExists = GameManager.instance.recipes.Length > index;

            InventoryRecipe recipe = null;

            if (recipeExists)
            {
                recipe = GameManager.instance.recipes[index];
            }

            if (recipeExists)
            {
                recipeSlot.hasRecipe = true;
                recipeSlot.recipe = recipe;
            }
            else
            {
                recipeSlot.hasRecipe = false;
            }

            recipeSlot.UpdateSlot();

            index++;
        }
    }

    public void OnItemDropped()
    {
        AudioPlayer.instance.PlaySound(dropItemSound, 0.03f, Random.Range(0.95f, 1.05f));
    }

    public void OnItemCrafted()
    {
        AudioPlayer.instance.PlaySound(craftSound, 0.03f, Random.Range(0.95f, 1.05f));
    }
}
