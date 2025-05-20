using System.Collections.Generic;

[System.Serializable]
public class StringKeyValuePair
{
    public string key;
    public string value;

    public StringKeyValuePair(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class StringDictionary
{
    public List<StringKeyValuePair> pairs = new List<StringKeyValuePair>();

    public string GetValue(string key)
    {
        foreach (var pair in pairs)
        {
            if (pair.key == key)
                return pair.value;
        }
        return null;
    }

    public void SetValue(string key, string value)
    {
        // Buscar si ya existe el key
        for (int i = 0; i < pairs.Count; i++)
        {
            if (pairs[i].key == key)
            {
                pairs[i].value = value;
                return;
            }
        }

        // Si no existe, añadirlo
        pairs.Add(new StringKeyValuePair(key, value));
    }

    public bool ContainsKey(string key)
    {
        foreach (var pair in pairs)
        {
            if (pair.key == key)
                return true;
        }
        return false;
    }
}