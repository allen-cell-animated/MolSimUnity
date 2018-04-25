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
        sites1[0] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "A", 2 ), "1" );

        SiteState[] sites2 = new SiteState[3];
        sites2[0] = new SiteState( new BindingSiteRef( "A", 0 ), "1" );
        sites2[1] = new SiteState( new BindingSiteRef( "A", 2 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );

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
        sites1[0] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "s", 3 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "s", 2 ), "1" );
        sites1[3] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites1[4] = new SiteState( new BindingSiteRef( "A", 2 ), "1" );
        sites1[5] = new SiteState( new BindingSiteRef( "A", 3 ), "0" );
        sites1[6] = new SiteState( new BindingSiteRef( "s", 1 ), "1" );
        sites1[7] = new SiteState( new BindingSiteRef( "s", 0 ), "0" );

        SiteState[] sites2 = new SiteState[8];
        sites2[0] = new SiteState( new BindingSiteRef( "s", 1 ), "1" );
        sites2[1] = new SiteState( new BindingSiteRef( "s", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites2[3] = new SiteState( new BindingSiteRef( "s", 3 ), "0" );
        sites2[4] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites2[5] = new SiteState( new BindingSiteRef( "A", 2 ), "0" );
        sites2[6] = new SiteState( new BindingSiteRef( "A", 3 ), "1" );
        sites2[7] = new SiteState( new BindingSiteRef( "s", 2 ), "1" );

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
        sites1[0] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "A", 2 ), "1" );
        sites1[3] = new SiteState( new BindingSiteRef( "A", 3 ), "0" );
        sites1[4] = new SiteState( new BindingSiteRef( "A", 4 ), "2" );
        sites1[5] = new SiteState( new BindingSiteRef( "A", 5 ), "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( new BindingSiteRef( "A", 3 ), "2" );
        sites2[1] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "A", 2 ), "0" );
        sites2[3] = new SiteState( new BindingSiteRef( "A", 0 ), "2" );
        sites2[4] = new SiteState( new BindingSiteRef( "A", 4 ), "0" );
        sites2[5] = new SiteState( new BindingSiteRef( "A", 5 ), "1" );

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
        sites1[0] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "B", 0 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "C", 0 ), "1" );
        sites1[3] = new SiteState( new BindingSiteRef( "D", 0 ), "0" );
        sites1[4] = new SiteState( new BindingSiteRef( "E", 0 ), "2" );
        sites1[5] = new SiteState( new BindingSiteRef( "F", 0 ), "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( new BindingSiteRef( "B", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "D", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "E", 0 ), "2" );
        sites2[3] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites2[4] = new SiteState( new BindingSiteRef( "F", 0 ), "2" );
        sites2[5] = new SiteState( new BindingSiteRef( "C", 0 ), "1" );

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
        sites1[0] = new SiteState( new BindingSiteRef( "p", 2 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "p", 3 ), "0" );
        sites1[2] = new SiteState( new BindingSiteRef( "p", 0 ), "1" );
        sites1[3] = new SiteState( new BindingSiteRef( "p", 1 ), "0" );
        sites1[4] = new SiteState( new BindingSiteRef( "active", 0 ), "0" );
        sites1[5] = new SiteState( new BindingSiteRef( "inhibit", 0 ), "0" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( new BindingSiteRef( "active", 0 ), "0" );
        sites2[1] = new SiteState( new BindingSiteRef( "inhibit", 0 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "p", 2 ), "1" );
        sites2[3] = new SiteState( new BindingSiteRef( "p", 1 ), "0" );
        sites2[4] = new SiteState( new BindingSiteRef( "p", 0 ), "0" );
        sites2[5] = new SiteState( new BindingSiteRef( "p", 3 ), "0" );

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
        sites1[0] = new SiteState( new BindingSiteRef( "A", 0 ), "0" );
        sites1[1] = new SiteState( new BindingSiteRef( "A", 1 ), "1" );
        sites1[2] = new SiteState( new BindingSiteRef( "A", 2 ), "1" );
        sites1[3] = new SiteState( new BindingSiteRef( "A", 3 ), "0" );
        sites1[4] = new SiteState( new BindingSiteRef( "A", 4 ), "2" );
        sites1[5] = new SiteState( new BindingSiteRef( "A", 5 ), "2" );

        SiteState[] sites2 = new SiteState[6];
        sites2[0] = new SiteState( new BindingSiteRef( "A", 0 ), "2" );
        sites2[1] = new SiteState( new BindingSiteRef( "A", 1 ), "0" );
        sites2[2] = new SiteState( new BindingSiteRef( "A", 2 ), "0" );
        sites2[3] = new SiteState( new BindingSiteRef( "A", 3 ), "2" );
        sites2[4] = new SiteState( new BindingSiteRef( "A", 4 ), "0" );
        sites2[5] = new SiteState( new BindingSiteRef( "A", 5 ), "1" );

        MoleculeSnapshot molecule1 = new MoleculeSnapshot( moleculeDef, sites1 );
        MoleculeSnapshot molecule2 = new MoleculeSnapshot( moleculeDef, sites2 );

        Assert.IsFalse( molecule1.IsSatisfiedBy( molecule2 ) );
        Assert.IsFalse( molecule2.IsSatisfiedBy( molecule1 ) );
    }
}