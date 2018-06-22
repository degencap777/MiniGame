using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour {

    #region Data
    public Item Item { get; private set; }
    private int amount = 0;
    #endregion

    public void SetLocalPosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}
