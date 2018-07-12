using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS;

public class TestUnitConversion
{
    [Test]
    public void TestSIFormatting ()
    {
        Assert.IsTrue( Helpers.FormatSIValue( 999000000f, 2, "s" ) == "1.0 Gs" );
        Assert.IsTrue( Helpers.FormatSIValue( 99900000f, 2, "s" ) == "100 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 9990000f, 2, "s" ) == "10 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 999000f, 2, "s" ) == "1.0 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 99900f, 2, "s" ) == "100 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 9990f, 2, "s" ) == "10 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 999f, 2, "s" ) == "1.0 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 99.9f, 2, "s" ) == "100 s" );
        Assert.IsTrue( Helpers.FormatSIValue( 9.99f, 2, "s" ) == "10 s" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.999f, 2, "s" ) == "1.0 s" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.0999f, 2, "s" ) == "100 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.00999f, 2, "s" ) == "10 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000999f, 2, "s" ) == "1.0 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.0000999f, 2, "s" ) == "100 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.00000999f, 2, "s" ) == "10 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000000999f, 2, "s" ) == "1.0 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.0000000999f, 2, "s" ) == "100 ns" );

        Assert.IsTrue( Helpers.FormatSIValue( -111900f, 3, "s" ) == "-112 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 992000f, 2, "s" ) == "990 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 999999f, 2, "s" ) == "1.0 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.0009f, 2, "s" ) == "900 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000999999f, 2, "s" ) == "1.0 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( -0.000999991f, 5, "s" ) == "-999.99 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000009f, 2, "s" ) == "9.0 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000009f, 3, "s" ) == "9.00 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( -0.1234f, 2, "s" ) == "-120 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.1234567f, 3, "s" ) == "123 ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000001236f, 3, "s" ) == "1.24 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( -1141.0009f, 3, "s" ) == "-1.14 ks" );
        Assert.IsTrue( Helpers.FormatSIValue( 320000000f, 2, "s" ) == "320 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 326810000f, 3, "s" ) == "327 Ms" );
        Assert.IsTrue( Helpers.FormatSIValue( 0.000009f, 1, "s" ) == "9 μs" );
        Assert.IsTrue( Helpers.FormatSIValue( -1.26f, 2, "s" ) == "-1.3 s" );
        Assert.IsTrue( Helpers.FormatSIValue( 0, 2, "s" ) == "0.0 s" );
    }

    [Test]
    public void TestRounding ()
    {
        Assert.IsTrue( Helpers.FormatRoundedValue( 99900f, 2 ) == "100000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 9990f, 2 ) == "10000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 999f, 2 ) == "1000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 99.9f, 2 ) == "100" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 9.99f, 2 ) == "10" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.999f, 2 ) == "1.0" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.0999f, 2 ) == "0.10" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.00999f, 2 ) == "0.010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000999f, 2 ) == "0.0010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.0000999f, 2 ) == "0.00010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.00000999f, 2 ) == "0.000010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000000999f, 2 ) == "0.0000010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.0000000999f, 2 ) == "0.00000010" );

        Assert.IsTrue( Helpers.FormatRoundedValue( -111900f, 3 ) == "-112000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 992000f, 2 ) == "990000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 999999f, 2 ) == "1000000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.0009f, 2 ) == "0.00090" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000999999f, 2 ) == "0.0010" );
        Assert.IsTrue( Helpers.FormatRoundedValue( -0.000999991f, 5 ) == "-0.00099999" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000009f, 2 ) == "0.0000090" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000009f, 3 ) == "0.00000900" );
        Assert.IsTrue( Helpers.FormatRoundedValue( -0.1234f, 2 ) == "-0.12" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.1234567f, 3 ) == "0.123" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000001236f, 3 ) == "0.00000124" );
        Assert.IsTrue( Helpers.FormatRoundedValue( -1141.0009f, 3 ) == "-1140" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 320000f, 2 ) == "320000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 326810f, 3 ) == "327000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0.000009f, 1 ) == "0.000009" );
        Assert.IsTrue( Helpers.FormatRoundedValue( -1.26f, 2 ) == "-1.3" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 11f, 5 ) == "11.000" );
        Assert.IsTrue( Helpers.FormatRoundedValue( 0, 5 ) == "0.0000" );
    }
}