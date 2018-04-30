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

        ComponentPattern[] components1 = new ComponentPattern[3];
        components1[0] = new ComponentPattern( "A", "0" );
        components1[1] = new ComponentPattern( "A", "0" );
        components1[2] = new ComponentPattern( "A", "1" );

        ComponentPattern[] components2 = new ComponentPattern[3];
        components2[0] = new ComponentPattern( "A", "1" );
        components2[1] = new ComponentPattern( "A", "0" );
        components2[2] = new ComponentPattern( "A", "0" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void Molecule2TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[8];
        components1[0] = new ComponentPattern( "A", "0" );
        components1[1] = new ComponentPattern( "s", "0" );
        components1[2] = new ComponentPattern( "s", "1" );
        components1[3] = new ComponentPattern( "A", "0" );
        components1[4] = new ComponentPattern( "A", "1" );
        components1[5] = new ComponentPattern( "A", "0" );
        components1[6] = new ComponentPattern( "s", "1" );
        components1[7] = new ComponentPattern( "s", "0" );

        ComponentPattern[] components2 = new ComponentPattern[8];
        components2[0] = new ComponentPattern( "s", "1" );
        components2[1] = new ComponentPattern( "s", "0" );
        components2[2] = new ComponentPattern( "A", "0" );
        components2[3] = new ComponentPattern( "s", "0" );
        components2[4] = new ComponentPattern( "A", "0" );
        components2[5] = new ComponentPattern( "A", "0" );
        components2[6] = new ComponentPattern( "A", "1" );
        components2[7] = new ComponentPattern( "s", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[6];
        components1[0] = new ComponentPattern( "A", "0" );
        components1[1] = new ComponentPattern( "A", "0" );
        components1[2] = new ComponentPattern( "A", "1" );
        components1[3] = new ComponentPattern( "A", "0" );
        components1[4] = new ComponentPattern( "A", "2" );
        components1[5] = new ComponentPattern( "A", "2" );

        ComponentPattern[] components2 = new ComponentPattern[6];
        components2[0] = new ComponentPattern( "A", "2" );
        components2[1] = new ComponentPattern( "A", "0" );
        components2[2] = new ComponentPattern( "A", "0" );
        components2[3] = new ComponentPattern( "A", "2" );
        components2[4] = new ComponentPattern( "A", "0" );
        components2[5] = new ComponentPattern( "A", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeUniqueSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[6];
        components1[0] = new ComponentPattern( "A", "0" );
        components1[1] = new ComponentPattern( "B", "0" );
        components1[2] = new ComponentPattern( "C", "1" );
        components1[3] = new ComponentPattern( "D", "0" );
        components1[4] = new ComponentPattern( "E", "2" );
        components1[5] = new ComponentPattern( "F", "2" );

        ComponentPattern[] components2 = new ComponentPattern[6];
        components2[0] = new ComponentPattern( "B", "0" );
        components2[1] = new ComponentPattern( "D", "0" );
        components2[2] = new ComponentPattern( "E", "2" );
        components2[3] = new ComponentPattern( "A", "0" );
        components2[4] = new ComponentPattern( "F", "2" );
        components2[5] = new ComponentPattern( "C", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void Molecule3TypesOfSites ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[6];
        components1[0] = new ComponentPattern( "p", "0" );
        components1[1] = new ComponentPattern( "p", "0" );
        components1[2] = new ComponentPattern( "p", "1" );
        components1[3] = new ComponentPattern( "p", "0" );
        components1[4] = new ComponentPattern( "active", "0" );
        components1[5] = new ComponentPattern( "inhibit", "0" );

        ComponentPattern[] components2 = new ComponentPattern[6];
        components2[0] = new ComponentPattern( "active", "0" );
        components2[1] = new ComponentPattern( "inhibit", "0" );
        components2[2] = new ComponentPattern( "p", "1" );
        components2[3] = new ComponentPattern( "p", "0" );
        components2[4] = new ComponentPattern( "p", "0" );
        components2[5] = new ComponentPattern( "p", "0" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsTrue( molecule1.Matches( molecule2 ) );
        Assert.IsTrue( molecule2.Matches( molecule1 ) );
    }

    [Test]
    public void MoleculeSymmetricalSitesInWrongStates ()
    {
        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Basic" ) as MoleculeDef;

        ComponentPattern[] components1 = new ComponentPattern[6];
        components1[0] = new ComponentPattern( "A", "0" );
        components1[1] = new ComponentPattern( "A", "1" );
        components1[2] = new ComponentPattern( "A", "1" );
        components1[3] = new ComponentPattern( "A", "0" );
        components1[4] = new ComponentPattern( "A", "2" );
        components1[5] = new ComponentPattern( "A", "2" );

        ComponentPattern[] components2 = new ComponentPattern[6];
        components2[0] = new ComponentPattern( "A", "2" );
        components2[1] = new ComponentPattern( "A", "0" );
        components2[2] = new ComponentPattern( "A", "0" );
        components2[3] = new ComponentPattern( "A", "2" );
        components2[4] = new ComponentPattern( "A", "0" );
        components2[5] = new ComponentPattern( "A", "1" );

        MoleculePattern molecule1 = new MoleculePattern( moleculeDef, components1 );
        MoleculePattern molecule2 = new MoleculePattern( moleculeDef, components2 );

        Assert.IsFalse( molecule1.Matches( molecule2 ) );
        Assert.IsFalse( molecule2.Matches( molecule1 ) );
    }
}