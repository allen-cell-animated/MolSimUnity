using System.Collections;
using UnityEngine;
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

        ComponentDef componentP = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["X"][0].moleculeDef.componentDefs["p"][0];
        Reaction reaction = reactor.collisionFreeReactions[0];

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfComponentsInState( componentP, "0" ) == reaction.events );
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

        ComponentDef componentP = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["X"][0].moleculeDef.componentDefs["p"][0];
        Reaction phosphorylationReaction = null, dephosphorylationReaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.definition == phosphorylationReactionDef)
            {
                phosphorylationReaction = reaction;
            }
            else if (reaction.definition == dephosphorylationReactionDef)
            {
                dephosphorylationReaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfComponentsInState( componentP, "1" ) == phosphorylationReaction.events - dephosphorylationReaction.events );
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

        ComponentDef componentP = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["S"][0].moleculeDef.componentDefs["p"][0];
        Reaction phosphorylationReaction = null, dephosphorylationReaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.definition == phosphorylationReactionDef)
            {
                phosphorylationReaction = reaction;
            }
            else if (reaction.definition == dephosphorylationReactionDef)
            {
                dephosphorylationReaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfComponentsInState( componentP, "P" ) == phosphorylationReaction.events - dephosphorylationReaction.events );
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

        ComponentDef componentP1 = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["Z"][0].moleculeDef.componentDefs["p1"][0];
        ComponentDef componentP2 = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["Z"][0].moleculeDef.componentDefs["p2"][0];
        ComponentDef componentP3 = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["Z"][0].moleculeDef.componentDefs["p3"][0];
        ComponentDef componentP4 = reactor.modelDef.complexes[0].complexPattern.moleculePatterns["Z"][0].moleculeDef.componentDefs["p4"][0];

        Reaction e1Reaction = null, f1Reaction = null, e2Reaction = null, f2Reaction = null,
                 e3Reaction = null, f3Reaction = null, e4Reaction = null, f4Reaction = null;
        foreach (CollisionFreeReaction reaction in reactor.collisionFreeReactions)
        {
            if (reaction.definition == e1)
            {
                e1Reaction = reaction;
            }
            else if (reaction.definition == f1)
            {
                f1Reaction = reaction;
            }
            else if (reaction.definition == e2)
            {
                e2Reaction = reaction;
            }
            else if (reaction.definition == f2)
            {
                f2Reaction = reaction;
            }
            else if (reaction.definition == e3)
            {
                e3Reaction = reaction;
            }
            else if (reaction.definition == f3)
            {
                f3Reaction = reaction;
            }
            else if (reaction.definition == e4)
            {
                e4Reaction = reaction;
            }
            else if (reaction.definition == f4)
            {
                f4Reaction = reaction;
            }
        }

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
            AssertIsTrue( GetNumberOfComponentsInState( componentP1, "P" ) == e1Reaction.events - f1Reaction.events );
            AssertIsTrue( GetNumberOfComponentsInState( componentP2, "P" ) == e2Reaction.events - f2Reaction.events );
            AssertIsTrue( GetNumberOfComponentsInState( componentP3, "P" ) == e3Reaction.events - f3Reaction.events );
            AssertIsTrue( GetNumberOfComponentsInState( componentP4, "P" ) == e4Reaction.events - f4Reaction.events );
        }

        DestroyReactor( reactor );
    }

    int GetNumberOfComponentsInState (ComponentDef componentDef, string state)
    {
        int n = 0;
        MoleculeComponent[] components = GameObject.FindObjectsOfType<MoleculeComponent>();
        foreach (MoleculeComponent component in components)
        {
            if (component.definition == componentDef && component.state == state)
            {
                n++;
            }
        }
        return n;
    }
}
