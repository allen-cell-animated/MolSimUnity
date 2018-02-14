using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace AICS.AgentSim
{
    [CustomPropertyDrawer(typeof(MoleculeState))]
    public class MoleculeStateDrawer : PropertyDrawer
    {
        List<MoleculeStateDrawerProperties> properties = new List<MoleculeStateDrawerProperties>();

        float lineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
        {
            int index = GetPropertyIndex( fieldInfo, property );
            Load( index );
            return EditorGUIUtility.singleLineHeight + (property.FindPropertyRelative( "molecule" ).isExpanded && index >= 0 ? properties[index].height : 0);
        }

        void Load (int index)
        {
            while (properties.Count < index + 1)
            {
                properties.Add( new MoleculeStateDrawerProperties() );
            }
        }

        static int GetPropertyIndex (FieldInfo fieldInfo, SerializedProperty property)
        {
            var obj = fieldInfo.GetValue( property.serializedObject.targetObject );
            if (obj != null && obj.GetType().IsArray)
            {
                return Convert.ToInt32( property.propertyPath.Split( '[' )[1].Split( ']' )[0] );
            }
            return -1;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            MoleculeState[] moleculeStates = fieldInfo.GetValue( property.serializedObject.targetObject ) as MoleculeState[];
            int index = GetPropertyIndex( fieldInfo, property );
            Molecule molecule = null;
            if (index < moleculeStates.Length)
            {
                molecule = moleculeStates[index].molecule;
            }
            properties[index].height = 0;

            SerializedProperty moleculeProperty = property.FindPropertyRelative( "molecule" );

            EditorGUI.BeginProperty( position, label, property );

            string title = (molecule == null) ? "Choose a molecule..." : molecule.species;
            EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), moleculeProperty, new GUIContent( title ) );

            if (molecule != null)
            {
                moleculeProperty.isExpanded = EditorGUI.Foldout( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), moleculeProperty.isExpanded, "" );
                if (moleculeProperty.isExpanded)
                {
                    position.y += lineHeight;
                    EditorGUI.indentLevel++;

                    //Draw the current required components
                    List<string> componentsToRemove = new List<string>();
                    Dictionary<string,string> editedComponents = new Dictionary<string,string>();
                    foreach (KeyValuePair<string,string> component in moleculeStates[index].componentStates)
                    {
                        EditorGUI.LabelField( new Rect( position.x, position.y, position.width, position.height ), component.Key );
                        EditorGUI.LabelField( new Rect( position.x + EditorGUIUtility.labelWidth - 30f, position.y, position.width, position.height ), "require state:" );

                        string[] states = molecule.GetComponentByID( component.Key ).states;
                        EditorGUI.BeginChangeCheck();
                        int stateIndex = EditorGUI.Popup( new Rect( position.x + EditorGUIUtility.labelWidth + 50f, position.y, EditorGUIUtility.currentViewWidth / 3f, EditorGUIUtility.singleLineHeight ), 
                                                          GetIndexOfStringInArray( component.Value, states ), states );
                        if (EditorGUI.EndChangeCheck())
                        {
                            editedComponents.Add( component.Key, states[stateIndex] );
                        }

                        //Remove this component
                        if (GUI.Button( new Rect( position.x + EditorGUIUtility.labelWidth + 55f + EditorGUIUtility.currentViewWidth / 3f, position.y, 50f, EditorGUIUtility.singleLineHeight ), 
                                       "remove" ))
                        {
                            componentsToRemove.Add( component.Key );
                        }

                        position.y += lineHeight;
                    }

                    //Remove components that are no longer required
                    foreach (string component in componentsToRemove)
                    {
                        moleculeStates[index].componentStates.Remove( component );
                    }

                    //Edit components whose states have been changed
                    foreach (KeyValuePair<string,string> component in editedComponents)
                    {
                        moleculeStates[index].componentStates[component.Key] = component.Value;
                    }

                    //Add another required component
                    position.y += lineHeight;
                    if (properties[index].showAdd)
                    {
                        //Get all the components that could be added
                        List<string> components = new List<string>();
                        List<int> componentIndices = new List<int>();
                        for (int i = 0; i < molecule.components.Length; i++)
                        {
                            if (!moleculeStates[index].componentStates.ContainsKey( molecule.components[i].id ))
                            {
                                components.Add( molecule.components[i].id );
                                componentIndices.Add( i );
                            }
                        }

                        EditorGUI.LabelField( position, "Choose required component:" );
                        properties[index].selectedComponent = EditorGUI.Popup( new Rect( position.x + EditorGUIUtility.labelWidth - 30f, position.y, EditorGUIUtility.currentViewWidth / 3f, EditorGUIUtility.singleLineHeight ), 
                                                                               properties[index].selectedComponent, components.ToArray() );
                        
                        if (GUI.Button( new Rect( position.x + EditorGUIUtility.labelWidth - 25f + EditorGUIUtility.currentViewWidth / 3f, position.y, 50f, EditorGUIUtility.singleLineHeight ), 
                                        "add" ))
                        {
                            //Actually add the component to the MoleculeState
                            moleculeStates[index].componentStates.Add( components[properties[index].selectedComponent], molecule.components[componentIndices[properties[index].selectedComponent]].states[0] );
                            properties[index].showAdd = false;
                        }
                        if (GUI.Button( new Rect( position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.currentViewWidth / 3f + 30f, position.y, 50f, EditorGUIUtility.singleLineHeight ), 
                                        "cancel" ))
                        {
                            properties[index].showAdd = false;
                        }
                    }

                    if (molecule.components.Length > moleculeStates[index].componentStates.Count)
                    {
                        if (!properties[index].showAdd)
                        {
                            properties[index].showAdd = GUI.Button( new Rect( position.x + 30f, position.y, 200f, EditorGUIUtility.singleLineHeight ), 
                                                      "add required component state" );
                        }
                    }
                    else
                    {
                        properties[index].showAdd = false;
                    }

                    properties[index].height += 5 * lineHeight;
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        int GetIndexOfStringInArray (string s, string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (s == array[i])
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public class MoleculeStateDrawerProperties
    {
        public float height = 0;
        public bool showAdd = false;
        public int selectedComponent = 0;

        public MoleculeStateDrawerProperties () { }
    }
}