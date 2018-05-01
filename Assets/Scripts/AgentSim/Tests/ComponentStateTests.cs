using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ComponentStateTests : MolSimTests
{
    [UnityTest]
    public IEnumerator SetMoleculeState ()
    {
        Reactor reactor = CreateReactor( "SetMoleculeState" );

        yield return new WaitForEndOfFrame();

        Molecule molecule = reactor.GetComponentInChildren<Molecule>();

        MoleculeDef moleculeDef = Resources.Load( "Tests/Molecules/Tester" ) as MoleculeDef;
        ComponentPattern[] components = new ComponentPattern[6];
        components[0] = new ComponentPattern( "p", "1" );
        components[1] = new ComponentPattern( "p", "1" );
        components[2] = new ComponentPattern( "p", "0" );
        components[3] = new ComponentPattern( "p", "0" );
        components[4] = new ComponentPattern( "active", "1" );
        components[5] = new ComponentPattern( "inhibit", "0" );
        MoleculePattern pattern = new MoleculePattern( moleculeDef, components );

        yield return new WaitForSeconds( 0.1f );

        pattern.SetStateOfMoleculeComponents( molecule );
        AssertIsTrue( pattern.Matches( molecule ) );

        DestroyReactor( reactor );
    }
}
