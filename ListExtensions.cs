using System.Collections.Generic;

public static class ListExtensions
{
    // Fonction pour mélanger une liste
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // Utilisation de Unity's Random.Range pour générer un indice aléatoire
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}