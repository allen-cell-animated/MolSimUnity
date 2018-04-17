using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ReactionTests : AgentSimTests
{
    int numberOfTimesToCheck = 5;

    float waitTime
    {
        get
        {
            return 2f + 3f * Random.value;
        }
    }

    [UnityTest]
    public IEnumerator SingleStateChangeReaction ()
    {
        Reactor reactor = CreateReactor( "XDephosphorylation" );

        yield return new WaitForEndOfFrame();

        BindingSiteDef bindingSiteP = reactor.modelDef.complexes[0].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["P"];
        ReactionSimulator reactionSimulator = reactor.collisionFreeReactionSimulators[0];

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "0" ) == reactionSimulator.events );
        }

        DestroyReactor( reactor );
    }

    [UnityTest]
    public IEnumerator StateChangeInSingleEnzymeSystemOfReactions ()
    {
        Reactor reactor = CreateReactor( "SimpleSystem" );
        ReactionDef phosphorylationReactionDef = Resources.Load( "Tests/Reactions/simpleCatalysis" ) as ReactionDef;
        ReactionDef dephosphorylationReactionDef = Resources.Load( "Tests/Reactions/simpleDephosphorylation" ) as ReactionDef;

        yield return new WaitForEndOfFrame();

        BindingSiteDef bindingSiteP = reactor.modelDef.complexes[0].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["P"];
        ReactionSimulator phosphorylationReactionSimulator = null, dephosphorylationReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reactionDef == phosphorylationReactionDef)
            {
                phosphorylationReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == dephosphorylationReactionDef)
            {
                dephosphorylationReactionSimulator = reactionSimulator;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "1" ) == phosphorylationReactionSimulator.events - dephosphorylationReactionSimulator.events );
        }

        DestroyReactor( reactor );
    }

    [UnityTest]
    public IEnumerator StateChangeInMultiEnzymeSystemOfReactions ()
    {
        Reactor reactor = CreateReactor( "PushPull" );
        ReactionDef phosphorylationReactionDef = Resources.Load( "Tests/Reactions/pushPullCatalysisC" ) as ReactionDef;
        ReactionDef dephosphorylationReactionDef = Resources.Load( "Tests/Reactions/pushPullCatalysisD" ) as ReactionDef;

        yield return new WaitForEndOfFrame();

        BindingSiteDef bindingSiteP = reactor.modelDef.complexes[2].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p"];
        ReactionSimulator phosphorylationReactionSimulator = null, dephosphorylationReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reactionDef == phosphorylationReactionDef)
            {
                phosphorylationReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == dephosphorylationReactionDef)
            {
                dephosphorylationReactionSimulator = reactionSimulator;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "P" ) == phosphorylationReactionSimulator.events - dephosphorylationReactionSimulator.events );
        }

        DestroyReactor( reactor );
    }

    [UnityTest]
    public IEnumerator MultiStateChangeInMultiEnzymeSystemOfReactions ()
    {
        Reactor reactor = CreateReactor( "MultisitePhosphorylationE4" );
        ReactionDef e1 = Resources.Load( "Tests/Reactions/multisitePCatalysisE1" ) as ReactionDef;
        ReactionDef f1 = Resources.Load( "Tests/Reactions/multisitePCatalysisF1" ) as ReactionDef;
        ReactionDef e2 = Resources.Load( "Tests/Reactions/multisitePCatalysisE2" ) as ReactionDef;
        ReactionDef f2 = Resources.Load( "Tests/Reactions/multisitePCatalysisF2" ) as ReactionDef;
        ReactionDef e3 = Resources.Load( "Tests/Reactions/multisitePCatalysisE3" ) as ReactionDef;
        ReactionDef f3 = Resources.Load( "Tests/Reactions/multisitePCatalysisF3" ) as ReactionDef;
        ReactionDef e4 = Resources.Load( "Tests/Reactions/multisitePCatalysisE4" ) as ReactionDef;
        ReactionDef f4 = Resources.Load( "Tests/Reactions/multisitePCatalysisF4" ) as ReactionDef;

        yield return new WaitForEndOfFrame();

        BindingSiteDef bindingSiteP1 = reactor.modelDef.complexes[2].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p1"];
        BindingSiteDef bindingSiteP2 = reactor.modelDef.complexes[2].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p2"];
        BindingSiteDef bindingSiteP3 = reactor.modelDef.complexes[2].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p3"];
        BindingSiteDef bindingSiteP4 = reactor.modelDef.complexes[2].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p4"];
        ReactionSimulator e1ReactionSimulator = null, f1ReactionSimulator = null, e2ReactionSimulator = null, f2ReactionSimulator = null,
                          e3ReactionSimulator = null, f3ReactionSimulator = null, e4ReactionSimulator = null, f4ReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reactionDef == e1)
            {
                e1ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == f1)
            {
                f1ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == e2)
            {
                e2ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == f2)
            {
                f2ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == e3)
            {
                e3ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == f3)
            {
                f3ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == e4)
            {
                e4ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reactionDef == f4)
            {
                f4ReactionSimulator = reactionSimulator;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP1, "P" ) == e1ReactionSimulator.events - f1ReactionSimulator.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP2, "P" ) == e2ReactionSimulator.events - f2ReactionSimulator.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP3, "P" ) == e3ReactionSimulator.events - f3ReactionSimulator.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP4, "P" ) == e4ReactionSimulator.events - f4ReactionSimulator.events );
        }

        DestroyReactor( reactor );
    }

    int GetNumberOfBindingSiteSimulatorsInState (BindingSiteDef bindingSiteDef, string state)
    {
        int n = 0;
        BindingSite[] bindingSites = GameObject.FindObjectsOfType<BindingSite>();
        foreach (BindingSite bindingSite in bindingSites)
        {
            if (bindingSite.definition == bindingSiteDef && bindingSite.state == state)
            {
                n++;
            }
        }
        return n;
    }
}
