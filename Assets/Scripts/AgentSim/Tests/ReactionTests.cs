using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ReactionTests : MolSimTests
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

        BindingSiteDef bindingSiteP = reactor.modelDef.complexes[0].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p"];
        Reaction reaction = reactor.collisionFreeReactions[0];

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "0" ) == reaction.events );
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

        BindingSiteDef bindingSiteP = reactor.modelDef.complexes[0].complexSnapshot.moleculeSnapshots[0].moleculeDef.bindingSiteDefs["p"];
        Reaction phosphorylationReaction = null, dephosphorylationReaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.reactionDef == phosphorylationReactionDef)
            {
                phosphorylationReaction = reaction;
            }
            else if (reaction.reactionDef == dephosphorylationReactionDef)
            {
                dephosphorylationReaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "1" ) == phosphorylationReaction.events - dephosphorylationReaction.events );
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
        Reaction phosphorylationReaction = null, dephosphorylationReaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.reactionDef == phosphorylationReactionDef)
            {
                phosphorylationReaction = reaction;
            }
            else if (reaction.reactionDef == dephosphorylationReactionDef)
            {
                dephosphorylationReaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP, "P" ) == phosphorylationReaction.events - dephosphorylationReaction.events );
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
        Reaction e1Reaction = null, f1Reaction = null, e2Reaction = null, f2Reaction = null,
                 e3Reaction = null, f3Reaction = null, e4Reaction = null, f4Reaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.reactionDef == e1)
            {
                e1Reaction = reaction;
            }
            else if (reaction.reactionDef == f1)
            {
                f1Reaction = reaction;
            }
            else if (reaction.reactionDef == e2)
            {
                e2Reaction = reaction;
            }
            else if (reaction.reactionDef == f2)
            {
                f2Reaction = reaction;
            }
            else if (reaction.reactionDef == e3)
            {
                e3Reaction = reaction;
            }
            else if (reaction.reactionDef == f3)
            {
                f3Reaction = reaction;
            }
            else if (reaction.reactionDef == e4)
            {
                e4Reaction = reaction;
            }
            else if (reaction.reactionDef == f4)
            {
                f4Reaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP1, "P" ) == e1Reaction.events - f1Reaction.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP2, "P" ) == e2Reaction.events - f2Reaction.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP3, "P" ) == e3Reaction.events - f3Reaction.events );
            AssertIsTrue( GetNumberOfBindingSiteSimulatorsInState( bindingSiteP4, "P" ) == e4Reaction.events - f4Reaction.events );
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
