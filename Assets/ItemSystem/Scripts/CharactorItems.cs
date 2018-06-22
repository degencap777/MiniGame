using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorItems : MonoBehaviour {

    public List<string> initItems;

    public Dictionary<string,Item> Items = new Dictionary<string, Item>();

    private ItemManager sm;

	void Start () {
        sm = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
        foreach (var itemClassName in initItems)
        {
            Items.Add(itemClassName, sm.GetInstanceOfItemWithString(itemClassName, gameObject));
        }
	}

    public void UseItem(string itemClassName, Item.ItemDelegate didStart = null, Item.ItemDelegate didAction = null, Item.ItemDelegate didEnd = null)
    {
        StartCoroutine(Items[itemClassName].Execute(didStart, didAction, didEnd));
    }
    public void UseItem<TItem>(Item.ItemDelegate didStart = null, Item.ItemDelegate didAction = null, Item.ItemDelegate didEnd = null)
    {
        foreach (var kvPair in Items)
        {
            if (typeof(TItem) == kvPair.Value.GetType())
            {
                StartCoroutine(kvPair.Value.Execute(didStart, didAction, didEnd));
                return;
            }
        }
    }

    public void AddItem(string itemClassName)
    {
        Items.Add(itemClassName, sm.GetInstanceOfItemWithString(itemClassName, gameObject));
    }
    public void AddItem<TItem>() where TItem : Item, new()
    {
        Items.Add(typeof(TItem).ToString(), sm.GetInstanceOfItem<TItem>(gameObject));
    }
    public void RemoveItem(string itemClassName)
    {
        Items.Remove(itemClassName);
    }
    public void RemoveItem<TItem>() where TItem : Item
    {
        foreach (var kvPair in Items)
        {
            if (typeof(TItem) == kvPair.Value.GetType())
            {
                Items.Remove(kvPair.Key);
                return;
            }
        }
    }
}
