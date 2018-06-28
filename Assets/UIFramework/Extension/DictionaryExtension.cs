using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对dictionary的扩展
/// </summary>
public static class DictionaryExtension {
    /// <summary>
    /// 尝试根据key得到value，如果没有得到直接返回null,并清理空值
    /// </summary>
    public static Tvalue TryGet<Tkey,Tvalue>(this Dictionary<Tkey,Tvalue> dict,Tkey key)
    {
        Tvalue value;
        if (dict.ContainsKey(key) && dict[key] == null)
            dict.Remove(key);
        dict.TryGetValue(key, out value);
        return value;
    }
}
