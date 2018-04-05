using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
    public static class Helpers 
    {
        public static void Shuffle<T> (this T[] array)
        {
            int n = array.Length;
            while (n > 1) 
            { 
                int k = Random.Range( 0, n );
                n--;

                T value = array[k];  
                array[k] = array[n];  
                array[n] = value; 
            } 
        }

        public static void Shuffle<T> (this List<T> list)
        {
            int n = list.Count;
            while (n > 1) 
            { 
                int k = Random.Range( 0, n );
                n--;

                T value = list[k];  
                list[k] = list[n];  
                list[n] = value; 
            } 
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