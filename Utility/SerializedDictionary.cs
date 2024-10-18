using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public abstract class AbstractSerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver, ISerializable
{
    [SerializeField, HideInInspector]
    private List<TKey> keyData = new List<TKey>();

    [SerializeField, HideInInspector]
    private List<TValue> valueData = new List<TValue>();

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < this.keyData.Count && i < this.valueData.Count; i++)
        {
            this[this.keyData[i]] = this.valueData[i];
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        this.keyData.Clear();
        this.valueData.Clear();

        foreach (var item in this)
        {
            this.keyData.Add(item.Key);
            this.valueData.Add(item.Value);
        }
    }
}

[Serializable]
public class SerializedDictionary<TKey, TValue> : AbstractSerializedDictionary<TKey, TValue> { }


