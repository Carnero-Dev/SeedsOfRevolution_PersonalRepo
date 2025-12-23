using System;
using System.Collections.Generic;

public static class SeedRandom {
    private static bool debugSeedEnabled;
    private static int fixedDebugSeed = 696969; // Semilla para testeos

    private static int seed;
    private static Dictionary<string, int> keyIndexes = new();
    private static Dictionary<string, Random> rngs = new();

/// <summary>
/// Inicializa la Seed de la partida con la que hayas facilitado
/// </summary>
/// <param name="newSeed">agrega 0 o ninguna para que genere una aleatoriamente</param>
    public static void Init(int newSeed = 0) {
        if(debugSeedEnabled) newSeed = fixedDebugSeed; // Para testeos en una seed debug

        seed = newSeed == 0 ? Guid.NewGuid().GetHashCode() : newSeed;
        keyIndexes.Clear();
        rngs.Clear();
    }

    public static int RangeInt(SeedCategory category, int min, int max) {
        string key = category.ToString();
        if (!rngs.ContainsKey(key)) {
            rngs[key] = new Random(CombineHash(seed, key));
            keyIndexes[key] = 0;
        }

        keyIndexes[key]++;
        return rngs[key].Next(min, max);
    }
/// <summary>
/// Obtiene la posición de los randoms de la Seed para guardar partida
/// </summary>
/// <returns></returns>
    public static SeedState GetState() {
        return new SeedState {
            seed = seed,
            keyIndexes = new Dictionary<string, int>(keyIndexes)
        };
    }
/// <summary>
/// Restaura la posición de los randoms de la Seed para cargar partida
/// </summary>
/// <param name="state"></param>
    public static void RestoreState(SeedState state) {
        seed = state.seed;
        keyIndexes = new Dictionary<string, int>(state.keyIndexes);
        rngs.Clear();

        foreach (var kvp in keyIndexes) {
            var rng = new Random(CombineHash(seed, kvp.Key));
            for (int i = 0; i < kvp.Value; i++) rng.Next();
            rngs[kvp.Key] = rng;
        }
    }
    public static int GetSeed() => seed;
    public static bool SetDebugMode(bool state = true) => debugSeedEnabled = state;
    public static bool GetDebugState() => debugSeedEnabled;

    private static int CombineHash(int seed, string key) {
        // Crea un hash determinista entre la seed y la key
        unchecked {
        int hash = 17;
        hash = hash * 31 + seed;
        foreach (char c in key)
            hash = hash * 31 + c;
        return hash;
    }
    }

}