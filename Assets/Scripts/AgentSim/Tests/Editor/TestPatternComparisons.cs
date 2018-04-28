using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;
using AICS.AgentSim;

public class TestPatternComparisons
{
    public bool debug = false;

    [Test]
    public void Molecule1TypeOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[3];
        components1[0] = new ComponentState( "A", "0" );
        components1[1] = new ComponentState( "A", "0" );
        components1[2] = new ComponentState( "A", "1" );

        ComponentState[] components2 = new ComponentState[3];
        components2[0] = new ComponentState( "A", "1" );
        components2[1] = new ComponentState( "A", "0" );
        components2[2] = new ComponentState( "A", "0" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void Molecule2TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[8];
        components1[0] = new ComponentState( "A", "0" );
        components1[1] = new ComponentState( "s", "0" );
        components1[2] = new ComponentState( "s", "1" );
        components1[3] = new ComponentState( "A", "0" );
        components1[4] = new ComponentState( "A", "1" );
        components1[5] = new ComponentState( "A", "0" );
        components1[6] = new ComponentState( "s", "1" );
        components1[7] = new ComponentState( "s", "0" );

        ComponentState[] components2 = new ComponentState[8];
        components2[0] = new ComponentState( "s", "1" );
        components2[1] = new ComponentState( "s", "0" );
        components2[2] = new ComponentState( "A", "0" );
        components2[3] = new ComponentState( "s", "0" );
        components2[4] = new ComponentState( "A", "0" );
        components2[5] = new ComponentState( "A", "0" );
        components2[6] = new ComponentState( "A", "1" );
        components2[7] = new ComponentState( "s", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[6];
        components1[0] = new ComponentState( "A", "0" );
        components1[1] = new ComponentState( "A", "0" );
        components1[2] = new ComponentState( "A", "1" );
        components1[3] = new ComponentState( "A", "0" );
        components1[4] = new ComponentState( "A", "2" );
        components1[5] = new ComponentState( "A", "2" );

        ComponentState[] components2 = new ComponentState[6];
        components2[0] = new ComponentState( "A", "2" );
        components2[1] = new ComponentState( "A", "0" );
        components2[2] = new ComponentState( "A", "0" );
        components2[3] = new ComponentState( "A", "2" );
        components2[4] = new ComponentState( "A", "0" );
        components2[5] = new ComponentState( "A", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeUniqueSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[6];
        components1[0] = new ComponentState( "A", "0" );
        components1[1] = new ComponentState( "B", "0" );
        components1[2] = new ComponentState( "C", "1" );
        components1[3] = new ComponentState( "D", "0" );
        components1[4] = new ComponentState( "E", "2" );
        components1[5] = new ComponentState( "F", "2" );

        ComponentState[] components2 = new ComponentState[6];
        components2[0] = new ComponentState( "B", "0" );
        components2[1] = new ComponentState( "D", "0" );
        components2[2] = new ComponentState( "E", "2" );
        components2[3] = new ComponentState( "A", "0" );
        components2[4] = new ComponentState( "F", "2" );
        components2[5] = new ComponentState( "C", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void Molecule3TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[6];
        components1[0] = new ComponentState( "p", "0" );
        components1[1] = new ComponentState( "p", "0" );
        components1[2] = new ComponentState( "p", "1" );
        components1[3] = new ComponentState( "p", "0" );
        components1[4] = new ComponentState( "active", "0" );
        components1[5] = new ComponentState( "inhibit", "0" );

        ComponentState[] components2 = new ComponentState[6];
        components2[0] = new ComponentState( "active", "0" );
        components2[1] = new ComponentState( "inhibit", "0" );
        components2[2] = new ComponentState( "p", "1" );
        components2[3] = new ComponentState( "p", "0" );
        components2[4] = new ComponentState( "p", "0" );
        components2[5] = new ComponentState( "p", "0" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSitesInWrongStates ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentState[] components1 = new ComponentState[6];
        components1[0] = new ComponentState( "A", "0" );
        components1[1] = new ComponentState( "A", "1" );
        components1[2] = new ComponentState( "A", "1" );
        components1[3] = new ComponentState( "A", "0" );
        components1[4] = new ComponentState( "A", "2" );
        components1[5] = new ComponentState( "A", "2" );

        ComponentState[] components2 = new ComponentState[6];
        components2[0] = new ComponentState( "A", "2" );
        components2[1] = new ComponentState( "A", "0" );
        components2[2] = new ComponentState( "A", "0" );
        components2[3] = new ComponentState( "A", "2" );
        components2[4] = new ComponentState( "A", "0" );
        components2[5] = new ComponentState( "A", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsFalse( molecule1.Matches( molecule2 ) );
        Assert.IsFalse( molecule2.Matches( molecule1 ) );
    }
}