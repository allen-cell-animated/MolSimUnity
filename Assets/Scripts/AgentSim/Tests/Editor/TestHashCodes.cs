using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS.AgentSim;

public class TestHashCodes
{
    public bool debug = false;

    [Test]
    public void MoleculeStatesSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        Dictionary<string,string> sites = new Dictionary<string, string>();
        sites.Add( "a", "0" );
        sites.Add( "b", "0" );
        sites.Add( "c", "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSameSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "P" );
        sites2.Add( "b", "P" );
        sites2.Add( "c", "P" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesDifferentNumberOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "c", "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentNumberOfSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeStatesDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesDifferentSpeciesSameSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesSitesInDifferentOrder ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "c", "0" );
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSitesInDifferentOrder: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void ComplexStateSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesSameSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexStateDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        MoleculeDef moleculeDef3 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;
        Dictionary<string,string> sites3 = new Dictionary<string, string>();
        sites3.Add( "a", "0" );
        sites3.Add( "b", "0" );
        sites3.Add( "c", "0" );
        MoleculeSnapshot snapshot3 = new MoleculeSnapshot( moleculeDef3, sites3 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot2, snapshot3} );

        if (debug) { Debug.Log( "ComplexStateDifferentSpeciesSameSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexStateSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        Dictionary<string,string> sites3 = new Dictionary<string, string>();
        sites3.Add( "a", "P" );
        sites3.Add( "b", "0" );
        sites3.Add( "c", "0" );
        MoleculeSnapshot snapshot3 = new MoleculeSnapshot( moleculeDef2, sites3 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot3} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesDifferentSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexStateSpeciesInDifferentOrder ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot2, snapshot1} );

        if (debug) { Debug.Log( "ComplexStateSpeciesInDifferentOrder: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
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