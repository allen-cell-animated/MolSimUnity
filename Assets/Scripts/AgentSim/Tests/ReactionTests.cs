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
    public IEnumerator StateChangeInSystemOfReactions ()
    {
        Reactor reactor = CreateReactor( "SimpleSystem" );
        Reaction catalysisReaction = Resources.Load( "Tests/Reactions/simpleCatalysis" ) as Reaction;
        Reaction dephosphorylationReaction = Resources.Load( "Tests/Reactions/simpleDephosphorylation" ) as Reaction;

        yield return new WaitForEndOfFrame();

        BindingSite bindingSiteP = reactor.model.complexes[0].complexState.moleculeStates[0].molecule.bindingSites["P"];
        ReactionSimulator catalysisReactionSimulator = null, dephosphorylationReactionSimulator = null;
        foreach (CollisionFreeReactionSimulator reactionSimulator in reactor.collisionFreeReactionSimulators)
        {
            if (reactionSimulator.reaction == catalysisReaction)
            {
                catalysisReactionSimulator = reactionSimulator;
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

            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "1" ) == catalysisReactionSimulator.events - dephosphorylationReactionSimulator.events );
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
        //Debug.Log( n );
        return n;
    }
}
