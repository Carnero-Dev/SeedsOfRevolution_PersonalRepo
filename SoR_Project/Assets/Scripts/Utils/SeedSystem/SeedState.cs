using System;
using System.Collections.Generic;

public class SeedState {
    public int seed;
    // public List<string> keys = new();
    // public List<int> values = new();

    public Dictionary<string, int>keyIndexes = new();

    //  public void Serialize() {
    //     keys.Clear();
    //     values.Clear();
    //     foreach (var kvp in keyIndexes) {
    //         keys.Add(kvp.Key);
    //         values.Add(kvp.Value);
    //     }
    // }

    // public void Deserialize() {
    //     keyIndexes.Clear();
    //     for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++) {
    //         keyIndexes[keys[i]] = values[i];
    //     }
    // }
}
