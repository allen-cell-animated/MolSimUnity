using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticlePopulation : MonoBehaviour 
    {
        public Reactor reactor;
        public ComplexState complexState;
        public Molecule[] molecules;
        public string species;
        public float diffusionCoefficient;
        public float collisionRadius;
        public float interactionRadius;
        public Dictionary<MoleculeBindingSite,BindingSitePopulation> bindingSitePopulations;

        int amount;
        float concentration
        {
            get
            {
                return amount / ( reactor.container.volume * 6.022141e23f );
            }
        }

        string complexName
        {
            get
            {
                return species + transform.childCount;
            }
        }

        public virtual void Init (ComplexConcentration complexConcentration, Reactor _reactor)
        {
            reactor = _reactor;
            complexState = complexConcentration.complexState;
            molecules = new Molecule[complexConcentration.moleculeCount];
            for (int i = 0; i < complexConcentration.moleculeCount; i++)
            {
                molecules[i] = complexState.moleculeStates[i].molecule;
            }
            species = complexConcentration.species;
            diffusionCoefficient = complexState.diffusionCoefficient;
            collisionRadius = interactionRadius = complexState.radius;
            amount = Mathf.RoundToInt( complexConcentration.concentration * reactor.container.volume * 6.022141e23f );

            CreateBindingSitePopulations( complexState );

            if (amount > 0 && complexConcentration.moleculeCount > 0)
            {
                SpawnComplexes( complexState.moleculeStates );
            }
        }

        protected virtual void CreateBindingSitePopulations (ComplexState complexState)
        {
            bindingSitePopulations = new Dictionary<MoleculeBindingSite,BindingSitePopulation>();
            string initialState = "";
            BindingSitePopulation sitePopulation;
            foreach (Molecule molecule in molecules)
            {
                foreach (BindingSite site in molecule.sites)
                {
                    foreach (MoleculeState moleculeState in complexState.moleculeStates)
                    {
                        if (moleculeState.molecule == molecule && moleculeState.bindingSiteStates.ContainsKey( site.id ))
                        {
                            initialState = moleculeState.bindingSiteStates[site.id];
                        }
                    }

                    sitePopulation = gameObject.AddComponent<BindingSitePopulation>();
                    sitePopulation.Init( molecule, site, initialState, this );
                    RegisterBindingSitePopulation( sitePopulation );
                    bindingSitePopulations.Add( new MoleculeBindingSite( molecule, site.id ), sitePopulation );
                }
            }
        }

        void RegisterBindingSitePopulation (BindingSitePopulation sitePopulation)
        {
            foreach (CollisionFreeReactionWatcher reactionWatcher in reactor.collisionFreeReactionWatchers)
            {
                reactionWatcher.RegisterBindingSitePopulation( sitePopulation, complexState );
            }
        }

        protected virtual void SpawnComplexes (MoleculeState[] moleculeStates)
        {
            GameObject complex;
            ParticleSimulator particle;
            MoleculeSimulator[] _molecules;
            if (moleculeStates.Length == 1)
            {
                // if the complex only has one molecule, create one object per instance with both a ParticleSimulator and a MoleculeSimulator
                _molecules = new MoleculeSimulator[1];
                for (int i = 0; i < amount; i++)
                {
                    complex = SpawnMolecule( moleculeStates[0] );
                    NameAndPlaceComplex( complex, reactor.container.GetRandomPointInBounds( 0.1f ), Random.rotation );
                    particle = complex.AddComponent<ParticleSimulator>();
                    _molecules[0] = CreateMoleculeSimulator( complex, moleculeStates[0], particle );
                    particle.molecules = new List<MoleculeSimulator>( _molecules );
                    particle.Init( this );
                }
            }
            else 
            {
                // if the complex has multiple molecules, create a complex object with a ParticleSimulator and child molecule objects with MoleculeSimulators
                RelativeTransform[] moleculeTransforms = GetMoleculeTransforms( moleculeStates );
                for (int i = 0; i < amount; i++)
                {
                    complex = new GameObject();
                    NameAndPlaceComplex( complex, reactor.container.GetRandomPointInBounds( 0.1f ), Random.rotation );
                    particle = complex.AddComponent<ParticleSimulator>();
                    _molecules = SpawnMoleculesInComplex( particle, moleculeStates, moleculeTransforms );
                    particle.molecules = new List<MoleculeSimulator>( _molecules );
                    particle.Init( this );
                }
            }
        }

        protected virtual RelativeTransform[] GetMoleculeTransforms (MoleculeState[] moleculeStates)
        {
            RelativeTransform[] transforms = new RelativeTransform[moleculeStates.Length];
            transforms[0] = new RelativeTransform( Vector3.zero, Vector3.zero );
            Vector3 averagePosition = Vector3.zero;

            Transform molecule1 = new GameObject( "molecule1" ).transform;
            Transform molecule2 = new GameObject( "molecule2" ).transform;
            Transform site1 = new GameObject( "site1" ).transform;
            site1.SetParent( molecule1 );
            Transform site2 = new GameObject( "site2" ).transform;
            site2.SetParent( molecule2 );
            BindingSite bindingSite1, bindingSite2;

            for (int i = 0; i < moleculeStates.Length - 1; i++)
            {
                foreach (KeyValuePair<string,string> siteState1 in moleculeStates[i].bindingSiteStates)
                {
                    if (siteState1.Value.Contains( "!" ))
                    {
                        for (int j = i + 1; j < moleculeStates.Length; j++)
                        {
                            foreach (KeyValuePair<string,string> siteState2 in moleculeStates[j].bindingSiteStates)
                            {
                                if (siteState1.Value == siteState2.Value)
                                {
                                    molecule1.position = transforms[i].position;
                                    molecule1.rotation = Quaternion.Euler( transforms[i].rotation );
                                    bindingSite1 = moleculeStates[i].molecule.GetSiteByID( siteState1.Key );
                                    bindingSite1.transformOnMolecule.Apply( molecule1, site1 );

                                    molecule2.position = Vector3.zero;
                                    molecule2.rotation = Quaternion.identity;
                                    bindingSite2 = moleculeStates[j].molecule.GetSiteByID( siteState2.Key );
                                    bindingSite2.transformOnMolecule.Apply( molecule2, site2 );

                                    molecule2.position = site1.TransformPoint( site2.InverseTransformPoint( molecule2.position ) );
                                    molecule2.rotation = molecule2.rotation * Quaternion.Inverse( site2.rotation ) * site1.rotation;
                                    
                                    transforms[j] = new RelativeTransform( molecule2.position, molecule2.rotation.eulerAngles );
                                    averagePosition += transforms[j].position;
                                }
                            }
                        }
                    }
                }
            }

            averagePosition /= transforms.Length;
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].position -= averagePosition;
            }

            Destroy( molecule1.gameObject );
            Destroy( molecule2.gameObject );

            return transforms;
        }

        protected virtual void NameAndPlaceComplex (GameObject complex, Vector3 position, Quaternion rotation)
        {
            complex.name = complexName;
            complex.transform.SetParent( transform );
            complex.transform.position = position;
            complex.transform.rotation = rotation;
        }

        protected virtual MoleculeSimulator[] SpawnMoleculesInComplex (ParticleSimulator particle, MoleculeState[] moleculeStates, RelativeTransform[] moleculeTransforms)
        {
            GameObject molecule;
            MoleculeSimulator[] _molecules = new MoleculeSimulator[moleculeStates.Length];
            for (int i = 0; i < moleculeStates.Length; i++)
            {
                molecule = SpawnMolecule( moleculeStates[i] );
                if (molecule != null)
                {
                    NameAndPlaceMoleculeInComplex( molecule, moleculeStates[i].molecule.species, particle.transform, moleculeTransforms[i] );
                    _molecules[i] = CreateMoleculeSimulator( molecule, moleculeStates[i], particle );
                }
            }
            ConnectBoundSites( _molecules );
            return _molecules;
        }

        protected virtual GameObject SpawnMolecule (MoleculeState moleculeState)
        {
            if (moleculeState.molecule.visualizationPrefab == null)
            {
                Debug.LogWarning( name + "'s molecule prefab is null!" );
                return null;
            }

            GameObject molecule = Instantiate( moleculeState.molecule.visualizationPrefab );
            return molecule;
        }

        protected virtual void NameAndPlaceMoleculeInComplex (GameObject molecule, string _species, Transform complex, RelativeTransform relativeTransform)
        {
            molecule.name = complex.name + "_" + _species;
            molecule.transform.SetParent( complex );
            molecule.transform.position = complex.TransformPoint( relativeTransform.position );
            molecule.transform.rotation = complex.rotation * Quaternion.Euler( relativeTransform.rotation );
        }

        protected virtual MoleculeSimulator CreateMoleculeSimulator (GameObject obj, MoleculeState moleculeState, ParticleSimulator particle)
        {
            MoleculeSimulator molecule = obj.AddComponent<MoleculeSimulator>();
            molecule.Init( moleculeState, particle );
            return molecule;
        }

        protected virtual void ConnectBoundSites (MoleculeSimulator[] _molecules)
        {
            Dictionary<string,BindingSiteSimulator> boundSites = new Dictionary<string, BindingSiteSimulator>();
            string boundState;
            foreach (MoleculeSimulator molecule in _molecules)
            {
                foreach (KeyValuePair<string,BindingSiteSimulator> bindingSite in molecule.bindingSites)
                {
                    boundState = bindingSite.Value.state;
                    if (boundState.Contains( "!" ))
                    {
                        if (!boundSites.ContainsKey( boundState ))
                        {
                            boundSites.Add( boundState, bindingSite.Value );
                        }
                        else
                        {
                            boundSites[boundState].boundSite = bindingSite.Value;
                            bindingSite.Value.boundSite = boundSites[boundState];
                        }
                    }
                }
            }
        }

        public virtual void CreateComplexWithMolecules (Transform centerTransform, List<MoleculeSimulator> _molecules)
        {
            ParticleSimulator particle = null;
            if (_molecules.Count == 1)
            {
                particle = _molecules[0].gameObject.AddComponent<ParticleSimulator>();
                particle.name = complexName;
            }
            else if (_molecules.Count > 1)
            {
                GameObject complex = new GameObject();
                NameAndPlaceComplex( complex, centerTransform.position, centerTransform.rotation );
                particle = complex.AddComponent<ParticleSimulator>();
            }

            particle.molecules = _molecules;
            particle.Init( this );
            foreach (MoleculeSimulator molecule in _molecules)
            {
                molecule.MoveToComplex( particle );
            }
        }

        public virtual BindingSitePopulation GetBindingSitePopulation (string _species, string bindingSiteID)
        {
            foreach (MoleculeBindingSite site in bindingSitePopulations.Keys)
            {
                if (site.molecule.species == _species && site.bindingSiteID == bindingSiteID)
                {
                    return bindingSitePopulations[site];
                }
            }
            return null;
        }
    }
}