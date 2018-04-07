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

        BindingSite bindingSiteP = reactor.model.complexes[0].complexState.moleculeStates[0].molecule.bindingSites["P"];
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
        Reaction phosphorylationReaction = Resources.Load( "Tests/Reactions/simpleCatalysis" ) as Reaction;
        Reaction dephosphorylationReaction = Resources.Load( "Tests/Reactions/simpleDephosphorylation" ) as Reaction;

        yield return new WaitForEndOfFrame();

        BindingSite bindingSiteP = reactor.model.complexes[0].complexState.moleculeStates[0].molecule.bindingSites["P"];
        ReactionSimulator phosphorylationReactionSimulator = null, dephosphorylationReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reaction == phosphorylationReaction)
            {
                phosphorylationReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == dephosphorylationReaction)
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
        Reaction phosphorylationReaction = Resources.Load( "Tests/Reactions/pushPullCatalysisC" ) as Reaction;
        Reaction dephosphorylationReaction = Resources.Load( "Tests/Reactions/pushPullCatalysisD" ) as Reaction;

        yield return new WaitForEndOfFrame();

        BindingSite bindingSiteP = reactor.model.complexes[2].complexState.moleculeStates[0].molecule.bindingSites["p"];
        ReactionSimulator phosphorylationReactionSimulator = null, dephosphorylationReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reaction == phosphorylationReaction)
            {
                phosphorylationReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == dephosphorylationReaction)
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
        Reaction e1 = Resources.Load( "Tests/Reactions/multisitePCatalysisE1" ) as Reaction;
        Reaction f1 = Resources.Load( "Tests/Reactions/multisitePCatalysisF1" ) as Reaction;
        Reaction e2 = Resources.Load( "Tests/Reactions/multisitePCatalysisE2" ) as Reaction;
        Reaction f2 = Resources.Load( "Tests/Reactions/multisitePCatalysisF2" ) as Reaction;
        Reaction e3 = Resources.Load( "Tests/Reactions/multisitePCatalysisE3" ) as Reaction;
        Reaction f3 = Resources.Load( "Tests/Reactions/multisitePCatalysisF3" ) as Reaction;
        Reaction e4 = Resources.Load( "Tests/Reactions/multisitePCatalysisE4" ) as Reaction;
        Reaction f4 = Resources.Load( "Tests/Reactions/multisitePCatalysisF4" ) as Reaction;

        yield return new WaitForEndOfFrame();
        Debug.Log( reactor.model.complexes[2].complexState.moleculeStates[0].molecule.name );
        BindingSite bindingSiteP1 = reactor.model.complexes[2].complexState.moleculeStates[0].molecule.bindingSites["p1"];
        BindingSite bindingSiteP2 = reactor.model.complexes[2].complexState.moleculeStates[0].molecule.bindingSites["p2"];
        BindingSite bindingSiteP3 = reactor.model.complexes[2].complexState.moleculeStates[0].molecule.bindingSites["p3"];
        BindingSite bindingSiteP4 = reactor.model.complexes[2].complexState.moleculeStates[0].molecule.bindingSites["p4"];
        ReactionSimulator e1ReactionSimulator = null, f1ReactionSimulator = null, e2ReactionSimulator = null, f2ReactionSimulator = null,
                          e3ReactionSimulator = null, f3ReactionSimulator = null, e4ReactionSimulator = null, f4ReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reaction == e1)
            {
                e1ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == f1)
            {
                f1ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == e2)
            {
                e2ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == f2)
            {
                f2ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == e3)
            {
                e3ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == f3)
            {
                f3ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == e4)
            {
                e4ReactionSimulator = reactionSimulator;
            }
            else if (reactionSimulator.reaction == f4)
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

    int GetNumberOfBindingSiteSimulatorsInState (BindingSite bindingSite, string state)
    {
        int n = 0;
        BindingSiteSimulator[] bindingSiteSimulators = GameObject.FindObjectsOfType<BindingSiteSimulator>();
        foreach (BindingSiteSimulator bindingSiteSimulator in bindingSiteSimulators)
        {
            if (bindingSiteSimulator.bindingSite == bindingSite && bindingSiteSimulator.state == state)
            {
                n++;
            }
        }
        Debug.Log( n );
        return n;
    }
}
