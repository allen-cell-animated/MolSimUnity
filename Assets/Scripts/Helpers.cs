using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
    public static class Helpers 
    {
        public static void Shuffle<T> (this T[] array)
        {
                //UnityEngine.Profiling.Profiler.BeginSample("Shuffle");
            int k, n = array.Length;
            T value;
            while (n > 1) 
            { 
                k = Random.Range( 0, n );
                n--;

                value = array[k];  
                array[k] = array[n];  
                array[n] = value; 
            } 
                //UnityEngine.Profiling.Profiler.EndSample();
        }

        public static void Shuffle<T> (this List<T> list)
        {
                //UnityEngine.Profiling.Profiler.BeginSample("Shuffle");
            int k, n = list.Count;
            T value;
            while (n > 1) 
            { 
                k = Random.Range( 0, n );
                n--;

                value = list[k];  
                list[k] = list[n];  
                list[n] = value; 
            } 
                //UnityEngine.Profiling.Profiler.EndSample();
        }

        public static int GetRandomIndex<T> (this T[] array)
        {
            return Mathf.RoundToInt( Random.value * (array.Length - 1) );
        }

        public static int GetRandomIndex<T> (this List<T> list)
        {
            return Mathf.RoundToInt( Random.value * (list.Count - 1) );
        }

        public static float SampleExponentialDistribution (float mean)
        {
            return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
        }

        public static float SampleNormalDistribution (float mean, float standardDeviation)
        {
            float n;
            float min = mean - 3.5f * standardDeviation;
            float max = mean + 3.5f * standardDeviation;

            do
            {
                n = mean + GetGaussian() * standardDeviation;
            } 
            while (n < min || n > max);

            return n;
        }

        public static float GetGaussian ()
        {
            float v1 = 0, v2 = 0, s = 0;
            while (s >= 1f || s == 0) 
            {
                v1 = 2f * Random.value - 1f;
                v2 = 2f * Random.value - 1f;
                s = v1 * v1 + v2 * v2;
            }
            s = Mathf.Sqrt( (-2f * Mathf.Log( s )) / s );
            return v1 * s;
        }
    }
}