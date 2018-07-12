using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
    public static class Helpers 
    {
        public static void Shuffle<T> (this T[] array)
        {
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
        }

        public static void Shuffle<T> (this List<T> list)
        {
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

        public static string FormatRoundedValue (float value, int significantFigures)
        {
            // if value = 0
            if (value > -float.Epsilon && value < float.Epsilon)
            {
                if (significantFigures < 2)
                {
                    return "0";
                }

                string zero = "0.";
                while (zero.Length < significantFigures + 1)
                {
                    zero += "0";
                }
                return zero;
            }

            // round to significant figures
            int exp = Mathf.FloorToInt( Mathf.Log10( Mathf.Abs( value ) ) ) - (significantFigures - 1);
            int roundedValue = Mathf.RoundToInt( Mathf.Abs( value ) / Mathf.Pow( 10f, exp ) );

            // if rounded up and added a digit
            if (roundedValue.ToString().Length > significantFigures) 
            {
                roundedValue = Mathf.RoundToInt( roundedValue / 10f );
                exp += 1;
            }

            // build string
            string result = "";
            if (exp >= 0)
            {
                result = (roundedValue * Mathf.Pow( 10f, exp )).ToString();
            }
            else
            {
                string valueString = roundedValue.ToString();
                int digits = valueString.Length;
                int decimals = Mathf.Abs( exp );
                if (digits > decimals)
                {
                    result = valueString.Substring( 0, digits - decimals) + "." + valueString.Substring( digits - decimals, decimals );
                }
                else 
                {
                    string zeroes = "";
                    while (zeroes.Length < decimals - digits)
                    {
                        zeroes += "0";
                    }
                    result = "0." + zeroes + valueString;
                }
            }

            // add the sign back in
            if (Mathf.RoundToInt( value / Mathf.Abs( value ) ) < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        public static string FormatSIValue (float value, int significantFigures, string baseUnit)
        {
            //get order of magnitude
            int orderOfMagnitude = value > -float.Epsilon && value < float.Epsilon ? 0 : 3 * Mathf.FloorToInt( Mathf.Log10( Mathf.Abs( value ) ) / 3f );

            //round to significant figures
            int exp = value > -float.Epsilon && value < float.Epsilon ? 0 : Mathf.FloorToInt( Mathf.Log10( Mathf.Abs( value ) ) ) - (significantFigures - 1);
            int roundedValue = Mathf.RoundToInt( Mathf.Abs( value ) / Mathf.Pow( 10f, exp ) );

            // if rounded up and added a digit
            if (roundedValue.ToString().Length > significantFigures) 
            {
                roundedValue = Mathf.RoundToInt( roundedValue / 10f );
                exp += 1;
                if (exp > orderOfMagnitude + 1)
                {
                    orderOfMagnitude += 3;
                }
            }

            //multiply the sign back in
            roundedValue *= Mathf.RoundToInt( value / Mathf.Abs( value ) );

            //add extra zeroes
            string result = (roundedValue * Mathf.Pow( 10f, exp - orderOfMagnitude )).ToString();
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

            return result + " " + GetSIPrefixSymbol( orderOfMagnitude ) + baseUnit;
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
                    Debug.LogWarning( "unrecognized SI exponent " + exponent );
                    return "";
            }
        }
    }
}