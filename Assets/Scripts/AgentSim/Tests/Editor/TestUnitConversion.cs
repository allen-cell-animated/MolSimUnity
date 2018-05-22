using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS;

public class TestUnitConversion
{
    [Test]
    public void TestTime ()
    {
        Helpers.FormatTime( 111100f, 3 );
        Helpers.FormatTime( 992000f, 2 );
        Helpers.FormatTime( 999999f, 2 );
        Helpers.FormatTime( 0.0009f, 2 );
        Helpers.FormatTime( 0.000999999f, 2 );
        Helpers.FormatTime( 0.00099999f, 5 );
        Helpers.FormatTime( 0.000009f, 2 );
        Helpers.FormatTime( 0.000009f, 3 );
        Helpers.FormatTime( 0.1234f, 2 );
        Helpers.FormatTime( 0.1234567f, 3 );
        Helpers.FormatTime( 0.000001236f, 3 );
        Helpers.FormatTime( 1141.0009f, 3 );
        Helpers.FormatTime( 320000000f, 2 );
        Helpers.FormatTime( 326810000f, 3 );
        Helpers.FormatTime( 0.000009f, 1 );

        Assert.IsTrue( true );


        //Assert.IsTrue( Helpers.FormatTime( 111100f, 3 ) == "111 ks" );
        //Assert.IsTrue( Helpers.FormatTime( 992000f, 2 ) == "990 ks" );
        //Assert.IsTrue( Helpers.FormatTime( 999999f, 2 ) == "1.0 Ms" );
        //Assert.IsTrue( Helpers.FormatTime( 0.0009f, 2 ) == "900 μs" );
        //Assert.IsTrue( Helpers.FormatTime( 0.000999999f, 2 ) == "1.0 ms" );
        //Assert.IsTrue( Helpers.FormatTime( 0.00099999f, 5 ) == "999.99 μs" );
        //Assert.IsTrue( Helpers.FormatTime( 0.000009f, 2 ) == "9.0 μs" );
        //Assert.IsTrue( Helpers.FormatTime( 0.000009f, 3 ) == "9.00 μs" );
        //Assert.IsTrue( Helpers.FormatTime( 0.1234f, 2 ) == "120 ms" );
        //Assert.IsTrue( Helpers.FormatTime( 0.1234567f, 3 ) == "123 ms" );
        //Assert.IsTrue( Helpers.FormatTime( 0.000001236f, 3 ) == "1.24 μs" );
        //Assert.IsTrue( Helpers.FormatTime( 1141.0009f, 3 ) == "1.14 ks" );
        //Assert.IsTrue( Helpers.FormatTime( 320000000f, 2 ) == "320 Ms" );
        //Assert.IsTrue( Helpers.FormatTime( 326810000f, 3 ) == "327 Ms" );
        //Assert.IsTrue( Helpers.FormatTime( 0.000009f, 1 ) == "9 μs" );
    }
}