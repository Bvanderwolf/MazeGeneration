using System.Collections.Generic;
using UnityEngine;

namespace BWolf.MazeGeneration.Utilities
{
    public static class WilsonGeneratorExtensions
    {
        /// <summary>
        /// Tries updates a record with given key, with given value. Retuns whether it succeeded
        /// </summary>
        /// <param name="records"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static bool TryUpdate(this List<KeyValuePair<MazeCell, Vector2Int>> records, MazeCell key, Vector2Int value)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].Key == key)
                {
                    records[i] = new KeyValuePair<MazeCell, Vector2Int>(key, value);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a record using given key
        /// </summary>
        /// <param name="records"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static KeyValuePair<MazeCell, Vector2Int> GetRecord(this List<KeyValuePair<MazeCell, Vector2Int>> records, MazeCell key)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].Key == key)
                {
                    return records[i];
                }
            }

            return default;
        }
    }
}