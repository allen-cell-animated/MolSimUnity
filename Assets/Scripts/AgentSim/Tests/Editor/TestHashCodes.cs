using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS.AgentSim;

public class TestHashCodes
{
    public bool debug = false;

    [Test]
    public void MoleculePatternSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        ComponentPattern[] components = new ComponentPattern[3];
        components[0] = new ComponentPattern( "a", "0", false );
        components[1] = new ComponentPattern( "b", "0", false );
        components[2] = new ComponentPattern( "c", "0", false );

        MoleculePattern pattern1 = new MoleculePattern( moleculeDef, components );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef, components );

        if (debug) { Debug.Log( "MoleculePatternSameSpeciesSameSites: " + pattern1.GetHashCode() + " != " + pattern2.GetHashCode() + " ? " + pattern1.Equals( pattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( pattern1, pattern2 ) );
    }

    [Test]
    public void MoleculePatternSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );

        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "P", false );
        components2[1] = new ComponentPattern( "b", "P", false );
        components2[2] = new ComponentPattern( "c", "P", false );

        MoleculePattern pattern1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef, components2 );

        if (debug) { Debug.Log( "MoleculePatternSameSpeciesDifferentSites: " + pattern1.GetHashCode() + " != " + pattern2.GetHashCode() 
                                + " ? " + pattern1.Equals( pattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( pattern1, pattern2 ) );
    }

    [Test]
    public void MoleculePatternSameSpeciesDifferentNumberOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );

        ComponentPattern[] components2 = new ComponentPattern[2];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "c", "0", false );

        MoleculePattern pattern1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef, components2 );

        if (debug) { Debug.Log( "MoleculePatternSameSpeciesDifferentNumberOfSites: " + pattern1.GetHashCode() + " != " + pattern2.GetHashCode() 
                                + " ? " + pattern1.Equals( pattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( pattern1, pattern2 ) );
    }

    [Test]
    public void MoleculePatternDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "b", "0", false );
        components2[2] = new ComponentPattern( "c", "0", false );

        MoleculePattern pattern1 = new MoleculePattern( moleculeDef1, components1 );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef2, components2 );

        if (debug) { Debug.Log( "MoleculePatternDifferentSpeciesSameSites: " + pattern1.GetHashCode() + " != " + pattern2.GetHashCode() 
                                + " ? " + pattern1.Equals( pattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( pattern1, pattern2 ) );
    }

    [Test]
    public void MoleculePatternSameSpeciesSitesInDifferentOrder ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );

        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "c", "0", false );
        components2[1] = new ComponentPattern( "a", "0", false );
        components2[2] = new ComponentPattern( "b", "0", false );

        MoleculePattern pattern1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef, components2 );

        if (debug) { Debug.Log( "MoleculePatternSameSpeciesSitesInDifferentOrder: " + pattern1.GetHashCode() + " != " + pattern2.GetHashCode() 
                                + " ? " + pattern1.Equals( pattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( pattern1, pattern2 ) );
    }

    [Test]
    public void ComplexPatternSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern1 = new MoleculePattern( moleculeDef1, components1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "b", "0", false );
        components2[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef2, components2 );

        ComplexPattern complexPattern1 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern2} );
        ComplexPattern complexPattern2 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern2} );

        if (debug) { Debug.Log( "ComplexPatternSameSpeciesSameSites: " + complexPattern1.GetHashCode() + " != " + complexPattern2.GetHashCode() 
                                + " ? " + complexPattern1.Equals( complexPattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexPattern1, complexPattern2 ) );
    }

    [Test]
    public void ComplexPatternDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern1 = new MoleculePattern( moleculeDef1, components1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "b", "0", false );
        components2[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef2, components2 );

        MoleculeDef moleculeDef3 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;
        ComponentPattern[] components3 = new ComponentPattern[3];
        components3[0] = new ComponentPattern( "a", "0", false );
        components3[1] = new ComponentPattern( "b", "0", false );
        components3[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern3 = new MoleculePattern( moleculeDef3, components3 );

        ComplexPattern complexPattern1 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern2} );
        ComplexPattern complexPattern2 = new ComplexPattern( new MoleculePattern[] {pattern2, pattern3} );

        if (debug) { Debug.Log( "ComplexPatternDifferentSpeciesSameSites: " + complexPattern1.GetHashCode() + " != " + complexPattern2.GetHashCode() 
                                + " ? " + complexPattern1.Equals( complexPattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexPattern1, complexPattern2 ) );
    }

    [Test]
    public void ComplexPatternSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern1 = new MoleculePattern( moleculeDef1, components1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "b", "0", false );
        components2[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef2, components2 );

        ComponentPattern[] components3 = new ComponentPattern[3];
        components3[0] = new ComponentPattern( "a", "P", false );
        components3[1] = new ComponentPattern( "b", "0", false );
        components3[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern3 = new MoleculePattern( moleculeDef2, components3 );

        ComplexPattern complexPattern1 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern2} );
        ComplexPattern complexPattern2 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern3} );

        if (debug) { Debug.Log( "ComplexPatternSameSpeciesDifferentSites: " + complexPattern1.GetHashCode() + " != " + complexPattern2.GetHashCode() 
                                + " ? " + complexPattern1.Equals( complexPattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexPattern1, complexPattern2 ) );
    }

    [Test]
    public void ComplexPatternSpeciesInDifferentOrder ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "a", "0", false );
        components1[1] = new ComponentPattern( "b", "0", false );
        components1[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern1 = new MoleculePattern( moleculeDef1, components1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "a", "0", false );
        components2[1] = new ComponentPattern( "b", "0", false );
        components2[2] = new ComponentPattern( "c", "0", false );
        MoleculePattern pattern2 = new MoleculePattern( moleculeDef2, components2 );

        ComplexPattern complexPattern1 = new ComplexPattern( new MoleculePattern[] {pattern1, pattern2} );
        ComplexPattern complexPattern2 = new ComplexPattern( new MoleculePattern[] {pattern2, pattern1} );

        if (debug) { Debug.Log( "ComplexPatternSpeciesInDifferentOrder: " + complexPattern1.GetHashCode() + " != " + complexPattern2.GetHashCode() 
                                + " ? " + complexPattern1.Equals( complexPattern2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexPattern1, complexPattern2 ) );
    }

    bool HashCodesMatchEquals (System.Object obj1, System.Object obj2)
    {
        if (obj1.Equals( obj2 ))
        {
            return obj1.GetHashCode() == obj2.GetHashCode();
        }
        return obj1.GetHashCode() != obj2.GetHashCode();
    }
}