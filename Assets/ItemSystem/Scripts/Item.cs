using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Item
{
    public ItemManager sm;

    public delegate int ItemDelegate();
    public Vector3 Position = Vector3.positiveInfinity;
    public string resourcesName_;
    private GameObject resources_ = null;
    public GameObject resources
    {
        get
        {
            if (resources_ == null)
            {
                resources_ = Resources.Load<GameObject>("ItemPrefabs/" + resourcesName_);
            }
            return Object.Instantiate(resources_);
        }
    }

    private GameObject _prefab;
    public GameObject Prefab
    {
        get
        {
            if (_prefab == null)
            {
                _prefab = Resources.Load<GameObject>("ItemPrefabs/" + resourcesName_);
            }
            return _prefab;
        }
    }

    private List<Sprite> _sprites = null;
    public List<Sprite> Sprites
    {
        get
        {
            if (_sprites == null)
            {
                List<Sprite> sprites = new List<Sprite>();
                GameObject res = Object.Instantiate(Prefab);
                SpriteRenderer[] sr = res.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer spriteRenderer in sr)
                {
                    sprites.Add(spriteRenderer.sprite);
                }
                _sprites = sprites;
                Object.Destroy(res);
            }
            return _sprites;
        }
    }
    public string name
    {
        get { return name_; }
    }


    protected string name_ = "null";
    public GameObject owner = null;


    public Dictionary<string, float> parameters = new Dictionary<string, float>();

    public float timeSinceItemStart = 0;


    public IEnumerator Execute(ItemDelegate didStart = null, ItemDelegate didAction = null, ItemDelegate didEnd = null)
    {
        timeSinceItemStart = 0;
        if (!ItemStart()) yield break;
        if (didStart != null) didStart();
        sm.StartAItem(this);
        while (!ItemAction())
        {
            if (didAction != null) didAction();
            timeSinceItemStart += Time.deltaTime;
            yield return 0;
        }
        ItemEnd();
        if (didEnd != null) didEnd();
    }

    protected abstract bool ItemStart();
    protected abstract bool ItemAction();
    protected abstract void ItemEnd();

    public abstract string GetDescription();

    public void SetName(string Name)
    {
        name_ = Name;
    }
}


public class ItemInfo : Item
{
    override protected bool ItemStart() { return true; }
    override protected bool ItemAction() { return false; }
    override protected void ItemEnd() { }
    override public string GetDescription() { return ""; }
}
