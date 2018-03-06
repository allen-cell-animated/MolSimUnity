using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Container : Simulator
	{
        [Tooltip( "L" )]
        public float volume;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        [HideInInspector] public LayerMask boundaryLayer = 1 << 8;
        Vector3 size;
        Walls walls;
        List<ManagedMoleculeSimulator> molecules = new List<ManagedMoleculeSimulator>();
        List<ManagedMoleculeSimulator> activeMolecules = new List<ManagedMoleculeSimulator>();

        public virtual void Init (float _volume, bool _periodicBoundary)
        {
            volume = _volume;
            CalculateSize( volume );
            periodicBoundary = _periodicBoundary;
            if (periodicBoundary)
            {
                CreatePhysicsBounds();
            }
        }

        protected virtual void CalculateSize (float _volume)
        {
            float side = Mathf.Pow( _volume * 1E-6f, 1f / 3f ) / agent.scale;
            size = side * Vector3.one;
        }

        public virtual Vector3 GetRandomPointInBounds (float margin = 0)
        {
            return transform.position + (1f - margin) * (new Vector3( size.x * Random.value, size.y * Random.value, size.z * Random.value ) - size / 2f);
        }

        void OnDrawGizmos ()
        {
            DrawGizmo();
        }

        protected virtual void DrawGizmo ()
        {
            Gizmos.DrawWireCube( transform.position, size );
        }

        public void RegisterMolecule (ManagedMoleculeSimulator molecule)
        {
            molecules.Add( molecule );
            if (molecule.active)
            {
                activeMolecules.Add( molecule );
            }
        }

        public virtual void CreatePhysicsBounds ()
        {
            if (walls == null)
            {
                walls = gameObject.AddComponent<Walls>();
                walls.Init( size, 100f );
            }
        }

        public override void SimulateFor (float dTime)
        {
            foreach (ManagedMoleculeSimulator molecule in molecules)
            {
                molecule.Move( dTime );
            }

            for (int i = 0; i < activeMolecules.Count - 1; i++)
            {
                for (int j = i + 1; j < activeMolecules.Count; j++)
                {
                    if (MoleculesAreNearEachOther( activeMolecules[i], activeMolecules[j] ))
                    {
                        activeMolecules[i].InteractWith( activeMolecules[j] );
                    }
                }
            }
        }

        public virtual bool IsOutOfBounds (Vector3 point, out Vector3 collisionToCenter)
        {
            bool inBounds = point.x < transform.position.x + size.x / 2f && point.x > transform.position.x - size.x / 2f
                         && point.y < transform.position.y + size.y / 2f && point.y > transform.position.y - size.y / 2f
                         && point.z < transform.position.z + size.z / 2f && point.z > transform.position.z - size.z / 2f;
            collisionToCenter = inBounds ? Vector3.zero : transform.position - point;
            return !inBounds;
        }

        public virtual bool WillCollide (ManagedMoleculeSimulator molecule, Vector3 newPosition, out ManagedMoleculeSimulator[] others)
        {
            List<ManagedMoleculeSimulator> othersList = new List<ManagedMoleculeSimulator>();
            foreach (ManagedMoleculeSimulator other in molecules)
            {
                if (MoleculesAreColliding( molecule, other ))
                {
                    othersList.Add( other );
                }
            }
            others = othersList.ToArray();
            return others.Length > 0;
        }

        bool MoleculesAreColliding (ManagedMoleculeSimulator molecule1, ManagedMoleculeSimulator molecule2)
        {
            return molecule1 != molecule2 
                && Vector3.Distance( molecule1.transform.position, molecule2.transform.position ) < molecule1.collisionRadius + molecule2.collisionRadius
                && !molecule1.IsBoundToOther( molecule2 );
        }

        bool MoleculesAreNearEachOther (ManagedMoleculeSimulator molecule1, ManagedMoleculeSimulator molecule2)
        {
            return molecule1 != molecule2 
                && Vector3.Distance( molecule1.transform.position, molecule2.transform.position ) < molecule1.interactionRadius + molecule2.interactionRadius;
        }
	}
}