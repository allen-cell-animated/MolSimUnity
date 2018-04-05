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
        Molecule molecule = Resources.Load( "Tests/Molecules/A" ) as Molecule;

        Dictionary<string,string> sites = new Dictionary<string, string>();
        sites.Add( "a", "0" );
        sites.Add( "b", "0" );
        sites.Add( "c", "0" );

        MoleculeState state1 = new MoleculeState( molecule, sites );
        MoleculeState state2 = new MoleculeState( molecule, sites );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSameSites: " + state1.GetHashCode() + " != " + state2.GetHashCode() + " ? " + state1.Equals( state2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( state1, state2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesDifferentSites ()
    {
        Molecule molecule = Resources.Load( "Tests/Molecules/A" ) as Molecule;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "P" );
        sites2.Add( "b", "P" );
        sites2.Add( "c", "P" );

        MoleculeState state1 = new MoleculeState( molecule, sites1 );
        MoleculeState state2 = new MoleculeState( molecule, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentSites: " + state1.GetHashCode() + " != " + state2.GetHashCode() + " ? " + state1.Equals( state2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( state1, state2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesDifferentNumberOfSites ()
    {
        Molecule molecule = Resources.Load( "Tests/Molecules/A" ) as Molecule;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "c", "0" );

        MoleculeState state1 = new MoleculeState( molecule, sites1 );
        MoleculeState state2 = new MoleculeState( molecule, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentNumberOfSites: " + state1.GetHashCode() + " != " + state2.GetHashCode() + " ? " + state1.Equals( state2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( state1, state2 ) );
    }

    [Test]
    public void MoleculeStatesDifferentSpeciesSameSites ()
    {
        Molecule molecule1 = Resources.Load( "Tests/Molecules/A" ) as Molecule;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Molecule molecule2 = Resources.Load( "Tests/Molecules/Basic" ) as Molecule;

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );

        MoleculeState state1 = new MoleculeState( molecule1, sites1 );
        MoleculeState state2 = new MoleculeState( molecule2, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesDifferentSpeciesSameSites: " + state1.GetHashCode() + " != " + state2.GetHashCode() + " ? " + state1.Equals( state2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( state1, state2 ) );
    }

    [Test]
    public void MoleculeStatesSameSpeciesSitesInDifferentOrder ()
    {
        Molecule molecule = Resources.Load( "Tests/Molecules/A" ) as Molecule;

        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );

        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "c", "0" );
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );

        MoleculeState state1 = new MoleculeState( molecule, sites1 );
        MoleculeState state2 = new MoleculeState( molecule, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSitesInDifferentOrder: " + state1.GetHashCode() + " != " + state2.GetHashCode() + " ? " + state1.Equals( state2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( state1, state2 ) );
    }

    [Test]
    public void ComplexStateSameSpeciesSameSites ()
    {
        Molecule molecule1 = Resources.Load( "Tests/Molecules/A" ) as Molecule;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeState state1 = new MoleculeState( molecule1, sites1 );

        Molecule molecule2 = Resources.Load( "Tests/Molecules/B" ) as Molecule;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeState state2 = new MoleculeState( molecule2, sites2 );

        ComplexState complexState1 = new ComplexState( new MoleculeState[] {state1, state2} );
        ComplexState complexState2 = new ComplexState( new MoleculeState[] {state1, state2} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesSameSites: " + complexState1.GetHashCode() + " != " + complexState2.GetHashCode() + " ? " + complexState1.Equals( complexState2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexState1, complexState2 ) );
    }

    [Test]
    public void ComplexStateDifferentSpeciesSameSites ()
    {
        Molecule molecule1 = Resources.Load( "Tests/Molecules/A" ) as Molecule;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeState state1 = new MoleculeState( molecule1, sites1 );

        Molecule molecule2 = Resources.Load( "Tests/Molecules/B" ) as Molecule;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeState state2 = new MoleculeState( molecule2, sites2 );

        Molecule molecule3 = Resources.Load( "Tests/Molecules/Basic" ) as Molecule;
        Dictionary<string,string> sites3 = new Dictionary<string, string>();
        sites3.Add( "a", "0" );
        sites3.Add( "b", "0" );
        sites3.Add( "c", "0" );
        MoleculeState state3 = new MoleculeState( molecule3, sites3 );

        ComplexState complexState1 = new ComplexState( new MoleculeState[] {state1, state2} );
        ComplexState complexState2 = new ComplexState( new MoleculeState[] {state2, state3} );

        if (debug) { Debug.Log( "ComplexStateDifferentSpeciesSameSites: " + complexState1.GetHashCode() + " != " + complexState2.GetHashCode() + " ? " + complexState1.Equals( complexState2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexState1, complexState2 ) );
    }

    [Test]
    public void ComplexStateSameSpeciesDifferentSites ()
    {
        Molecule molecule1 = Resources.Load( "Tests/Molecules/A" ) as Molecule;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeState state1 = new MoleculeState( molecule1, sites1 );

        Molecule molecule2 = Resources.Load( "Tests/Molecules/B" ) as Molecule;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeState state2 = new MoleculeState( molecule2, sites2 );

        Dictionary<string,string> sites3 = new Dictionary<string, string>();
        sites3.Add( "a", "P" );
        sites3.Add( "b", "0" );
        sites3.Add( "c", "0" );
        MoleculeState state3 = new MoleculeState( molecule2, sites3 );

        ComplexState complexState1 = new ComplexState( new MoleculeState[] {state1, state2} );
        ComplexState complexState2 = new ComplexState( new MoleculeState[] {state1, state3} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesDifferentSites: " + complexState1.GetHashCode() + " != " + complexState2.GetHashCode() + " ? " + complexState1.Equals( complexState2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexState1, complexState2 ) );
    }

    [Test]
    public void ComplexStateSpeciesInDifferentOrder ()
    {
        Molecule molecule1 = Resources.Load( "Tests/Molecules/A" ) as Molecule;
        Dictionary<string,string> sites1 = new Dictionary<string, string>();
        sites1.Add( "a", "0" );
        sites1.Add( "b", "0" );
        sites1.Add( "c", "0" );
        MoleculeState state1 = new MoleculeState( molecule1, sites1 );

        Molecule molecule2 = Resources.Load( "Tests/Molecules/B" ) as Molecule;
        Dictionary<string,string> sites2 = new Dictionary<string, string>();
        sites2.Add( "a", "0" );
        sites2.Add( "b", "0" );
        sites2.Add( "c", "0" );
        MoleculeState state2 = new MoleculeState( molecule2, sites2 );

        ComplexState complexState1 = new ComplexState( new MoleculeState[] {state1, state2} );
        ComplexState complexState2 = new ComplexState( new MoleculeState[] {state2, state1} );

        if (debug) { Debug.Log( "ComplexStateSpeciesInDifferentOrder: " + complexState1.GetHashCode() + " != " + complexState2.GetHashCode() + " ? " + complexState1.Equals( complexState2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexState1, complexState2 ) );
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