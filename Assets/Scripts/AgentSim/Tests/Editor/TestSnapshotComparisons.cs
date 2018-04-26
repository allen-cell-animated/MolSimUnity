using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS.AgentSim;

public class TestSnapshotComparisons
{
    public bool debug = false;

    [Test]
    public void Molecule1TypeOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[3];
        sites1[0] = new SiteState( "A", "0" );
        sites1[1] = new SiteState( "A", "0" );
        sites1[2] = new SiteState( "A", "1" );

        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( "A", "1" );
        sites2[1] = new SiteState( "A", "0" );
        sites2[2] = new SiteState( "A", "0" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsTrue( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsTrue( molecule2.IsSatisfiedBy( molecule1 ) );
    }

    [Test]
    public void Molecule2TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[8];
        sites1[0] = new SiteState( "A", "0" );
        sites1[1] = new SiteState( "s", "0" );
        sites1[2] = new SiteState( "s", "1" );
        sites1[3] = new SiteState( "A", "0" );
        sites1[4] = new SiteState( "A", "1" );
        sites1[5] = new SiteState( "A", "0" );
        sites1[6] = new SiteState( "s", "1" );
        sites1[7] = new SiteState( "s", "0" );

        SiteState[] sites2 = new SiteState[8];
        sites2[0] = new SiteState( "s", "1" );
        sites2[1] = new SiteState( "s", "0" );
        sites2[2] = new SiteState( "A", "0" );
        sites2[3] = new SiteState( "s", "0" );
        sites2[4] = new SiteState( "A", "0" );
        sites2[5] = new SiteState( "A", "0" );
        sites2[6] = new SiteState( "A", "1" );
        sites2[7] = new SiteState( "s", "1" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsTrue( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsTrue( molecule2.IsSatisfiedBy( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[6];
        sites1[0] = new SiteState( "A", "0" );
        sites1[1] = new SiteState( "A", "0" );
        sites1[2] = new SiteState( "A", "1" );
        sites1[3] = new SiteState( "A", "0" );
        sites1[4] = new SiteState( "A", "2" );
        sites1[5] = new SiteState( "A", "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( "A", "2" );
        sites2[1] = new SiteState( "A", "0" );
        sites2[2] = new SiteState( "A", "0" );
        sites2[3] = new SiteState( "A", "2" );
        sites2[4] = new SiteState( "A", "0" );
        sites2[5] = new SiteState( "A", "1" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsTrue( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsTrue( molecule2.IsSatisfiedBy( molecule1 ) );
    }

    [Test]
    public void MoleculeUniqueSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[6];
        sites1[0] = new SiteState( "A", "0" );
        sites1[1] = new SiteState( "B", "0" );
        sites1[2] = new SiteState( "C", "1" );
        sites1[3] = new SiteState( "D", "0" );
        sites1[4] = new SiteState( "E", "2" );
        sites1[5] = new SiteState( "F", "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( "B", "0" );
        sites2[1] = new SiteState( "D", "0" );
        sites2[2] = new SiteState( "E", "2" );
        sites2[3] = new SiteState( "A", "0" );
        sites2[4] = new SiteState( "F", "2" );
        sites2[5] = new SiteState( "C", "1" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsTrue( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsTrue( molecule2.IsSatisfiedBy( molecule1 ) );
    }

    [Test]
    public void Molecule3TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[6];
        sites1[0] = new SiteState( "p", "0" );
        sites1[1] = new SiteState( "p", "0" );
        sites1[2] = new SiteState( "p", "1" );
        sites1[3] = new SiteState( "p", "0" );
        sites1[4] = new SiteState( "active", "0" );
        sites1[5] = new SiteState( "inhibit", "0" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( "active", "0" );
        sites2[1] = new SiteState( "inhibit", "0" );
        sites2[2] = new SiteState( "p", "1" );
        sites2[3] = new SiteState( "p", "0" );
        sites2[4] = new SiteState( "p", "0" );
        sites2[5] = new SiteState( "p", "0" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsTrue( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsTrue( molecule2.IsSatisfiedBy( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSitesInWrongStates ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        SiteState[] sites1 = new SiteState[6];
        sites1[0] = new SiteState( "A", "0" );
        sites1[1] = new SiteState( "A", "1" );
        sites1[2] = new SiteState( "A", "1" );
        sites1[3] = new SiteState( "A", "0" );
        sites1[4] = new SiteState( "A", "2" );
        sites1[5] = new SiteState( "A", "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( "A", "2" );
        sites2[1] = new SiteState( "A", "0" );
        sites2[2] = new SiteState( "A", "0" );
        sites2[3] = new SiteState( "A", "2" );
        sites2[4] = new SiteState( "A", "0" );
        sites2[5] = new SiteState( "A", "1" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsFalse( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsFalse( molecule2.IsSatisfiedBy( molecule1 ) );
    }
}