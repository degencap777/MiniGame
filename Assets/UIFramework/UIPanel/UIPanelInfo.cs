using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class UIPanelInfo:ISerializationCallbackReceiver
{
    [NonSerialized]
    public UIPanelType PanelType;

    public string PanelTypeString;
    public string Path;

    public void OnAfterDeserialize()
    {
        PanelType = (UIPanelType) Enum.Parse(typeof(UIPanelType), PanelTypeString);
    }

    public void OnBeforeSerialize()
    {
        throw new NotImplementedException();
    }
}
