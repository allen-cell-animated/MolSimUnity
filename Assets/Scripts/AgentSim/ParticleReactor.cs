using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleReactor : MonoBehaviour 
    {
        public Reaction[] reactions;

        public ParticleReaction[] reactionData;

        void Start ()
        {
            SetupReactionData();
        }

        protected virtual void SetupReactionData ()
        {
            reactionData = new ParticleReaction[reactions.Length];
            for (int i = 0; i < reactions.Length; i++)
            {
                reactionData[i] = new ParticleReaction( reactions[i] );
            }
        }
    }

    [System.Serializable]
    public class ParticleReaction
    {
        public Reaction reaction;

        public int attempts;
        public int events;
        public float observedRate;

        public ParticleReaction (Reaction _reaction)
        {
            reaction = _reaction;
        }

        public bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * reaction.rate;
            }
        }

        public bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * reaction.rate;
            }
        }

        public bool ShouldHappen ()
        {
            attempts++;

            bool react;
            if (observedRateTooLow)
            {
                react = true;
            }
            else if (observedRateTooHigh)
            {
                react = false;
            }
            else 
            {
                react = Random.value <= reaction.rate * World.Instance.dT * (World.Instance.steps / attempts);
            }
            events = react ? events + 1 : events;
            observedRate = Mathf.Round( events / World.Instance.time );
            return react;
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }
    }
}