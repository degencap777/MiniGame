using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对dictionary的扩展
/// </summary>
public static class DictionaryExtension {
    /// <summary>
    /// 尝试根据key得到value，如果没有得到直接返回null
    /// </summary>
    public static Tvalue TryGet<Tkey,Tvalue>(this Dictionary<Tkey,Tvalue> dict,Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }
}
