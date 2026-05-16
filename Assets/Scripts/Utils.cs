using UnityEngine;

namespace KnightAdventure.Utils
{
    public static class Utils
    {
        public static Vector2 GetRandomDir()
        {
            return new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;
        }
    }
}