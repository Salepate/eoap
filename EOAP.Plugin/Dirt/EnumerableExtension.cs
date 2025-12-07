using Dirt.Game.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dirt.Game.Extensions
{
    public static class EnumerableExtension
    {
        public static void Shuffle<T>(this IList<T> list, RNGLite rng)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Range(0, n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, RNGLite rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }


        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, RNGLite rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Range(i, buffer.Count - 1);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            var buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}