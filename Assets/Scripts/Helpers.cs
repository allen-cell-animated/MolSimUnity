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
    }
}