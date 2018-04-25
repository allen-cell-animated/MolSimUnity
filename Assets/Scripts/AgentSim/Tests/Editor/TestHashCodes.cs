using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS.AgentSim;

public class TestHashCodes
{
    public bool debug = false;

    [Test]
    public void MoleculeSnapshotSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        SiteState[] sites = new SiteState[3];
        sites[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSameSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeSnapshotSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "P" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "P" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "P" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeSnapshotSameSpeciesDifferentNumberOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        SiteState[] sites2 = new SiteState[2];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesDifferentNumberOfSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeSnapshotDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesDifferentSpeciesSameSites: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void MoleculeSnapshotSameSpeciesSitesInDifferentOrder ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );

        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );

        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef, sites2 );

        if (debug) { Debug.Log( "MoleculeStatesSameSpeciesSitesInDifferentOrder: " + snapshot1.GetHashCode() + " != " + snapshot2.GetHashCode() 
                                + " ? " + snapshot1.Equals( snapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( snapshot1, snapshot2 ) );
    }

    [Test]
    public void ComplexSnapshotSameSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesSameSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexSnapshotDifferentSpeciesSameSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        MoleculeDef moleculeDef3 = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;
        SiteState[] sites3 = new SiteState[3];
        sites3[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites3[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites3[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot3 = new MoleculeSnapshot( moleculeDef3, sites3 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot2, snapshot3} );

        if (debug) { Debug.Log( "ComplexStateDifferentSpeciesSameSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexSnapshotSameSpeciesDifferentSites ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        SiteState[] sites3 = new SiteState[3];
        sites3[0] = new SiteState( new BindingSiteRef( "a", 0 ), "P" );
        sites3[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites3[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot3 = new MoleculeSnapshot( moleculeDef2, sites3 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot3} );

        if (debug) { Debug.Log( "ComplexStateSameSpeciesDifferentSites: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void ComplexSnapshotSpeciesInDifferentOrder ()
    {
        MoleculeDef moleculeDef1 = Resources.Load( "Tests/Molecules/A test" ) as MoleculeDef;
        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot1 = new MoleculeSnapshot( moleculeDef1, sites1 );

        MoleculeDef moleculeDef2 = Resources.Load( "Tests/Molecules/B test" ) as MoleculeDef;
        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "a", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "b", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "c", 0 ), "0" );
        MoleculeSnapshot snapshot2 = new MoleculeSnapshot( moleculeDef2, sites2 );

        ComplexSnapshot complexSnapshot1 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot1, snapshot2} );
        ComplexSnapshot complexSnapshot2 = new ComplexSnapshot( new MoleculeSnapshot[] {snapshot2, snapshot1} );

        if (debug) { Debug.Log( "ComplexStateSpeciesInDifferentOrder: " + complexSnapshot1.GetHashCode() + " != " + complexSnapshot2.GetHashCode() 
                                + " ? " + complexSnapshot1.Equals( complexSnapshot2 ) ); }

        Assert.IsTrue( HashCodesMatchEquals( complexSnapshot1, complexSnapshot2 ) );
    }

    [Test]
    public void BindingSiteReferences ()
    {
        BindingSiteRef bs1 = new BindingSiteRef( "active", 4 );
        BindingSiteRef bs2 = new BindingSiteRef( "active", 5 );
        BindingSiteRef bs3 = new BindingSiteRef( "p", 0 );
        BindingSiteRef bs4 = new BindingSiteRef( "p", 2 );
        BindingSiteRef bs5 = new BindingSiteRef( "e", 0 );
        BindingSiteRef bs6 = new BindingSiteRef( "e", 0 );

        Assert.IsTrue( HashCodesMatchEquals( bs1, bs2 ) );
        Assert.IsTrue( HashCodesMatchEquals( bs3, bs4 ) );
        Assert.IsTrue( HashCodesMatchEquals( bs5, bs6 ) );
        Assert.IsTrue( HashCodesMatchEquals( bs2, bs3 ) );
        Assert.IsTrue( HashCodesMatchEquals( bs3, bs5 ) );
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