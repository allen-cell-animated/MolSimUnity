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

        public static string FormatTime (float timeSeconds, int significantFigures)
        {
            //round to significant figures
            float logValue = Mathf.Log10( timeSeconds );
            float multiplier = Mathf.Pow( 10f, Mathf.Floor( logValue ) - significantFigures - 1f );
            float number = Mathf.Round( timeSeconds / multiplier ) * multiplier;

            //convert to correct units
            int exp = number > 0 ? GetSIExponent( number ) : 1;
            number *= Mathf.Pow( 10f, -exp );

            if (Mathf.Log10( number ) >= 3f)
            {
                number /= 1000f;
                exp += 3;
            }

            //add extra zeros
            string result = number.ToString();
            string[] splitResult = result.Split( '.' );
            int digits = splitResult[0].Length + (splitResult.Length > 1 ? splitResult[1].Length : 0);
            if (digits < significantFigures)
            {
                if (splitResult.Length < 2)
                {
                    result += ".";
                }
                while (digits < significantFigures)
                {
                    result += "0";
                    digits++;
                }
            }

            //Debug.Log( timeSeconds + " -> " + result + " " + GetSIPrefixSymbol( exp ) + "s" );
            return result + " " + GetSIPrefixSymbol( exp ) + "s";
        }

        public static int GetSIExponent (float _valueInBaseUnits)
        {
            float orderOfMagnitude = Mathf.Log10( _valueInBaseUnits );
            return 3 * Mathf.FloorToInt( orderOfMagnitude / 3f );
        }

        public static string GetSIPrefixSymbol (int exponent)
        {
            switch (exponent)
            {
                case 24 :
                    return "Y"; //yotta
                case 21 :
                    return "Z"; //zetta
                case 18 :
                    return "E"; //exa
                case 15 :
                    return "P"; //peta
                case 12 :
                    return "T"; //tera
                case 9 :
                    return "G"; //giga
                case 6 :
                    return "M"; //mega
                case 3 :
                    return "k"; //kilo
                case 2 :
                    return "h"; //hecto
                case 1 :
                    return "da"; //deca
                case 0 :
                    return ""; //base
                case -1 :
                    return "d"; //deci
                case -2 :
                    return "c"; //centi
                case -3 :
                    return "m"; //milli
                case -6 :
                    return "μ"; //micro
                case -9 :
                    return "n"; //nano
                case -12 :
                    return "p"; //pico
                case -15 :
                    return "f"; //femto
                case -18 :
                    return "a"; //atto
                case -21 :
                    return "z"; //zepto
                case -24 :
                    return "y"; //yocto
                default:
                    Debug.Log( "unrecognized SI exponent " + exponent );
                    return "";
            }
        }
    }
}