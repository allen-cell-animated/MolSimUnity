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
        List<float> heights = new List<float>();

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
            LoadHeight( index );
            return EditorGUIUtility.singleLineHeight + (property.FindPropertyRelative( "molecule" ).isExpanded && index >= 0 ? heights[index] : 0);
        }

        void LoadHeight (int index)
        {
            while (heights.Count < index + 1)
            {
                heights.Add( 0 );
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
            heights[index] = 0;

            SerializedProperty moleculeProperty = property.FindPropertyRelative( "molecule" );

            EditorGUI.BeginProperty( position, label, property );

            string title = (molecule == null) ? "Choose a molecule..." : molecule.species;
            EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), moleculeProperty, new GUIContent( title ) );

            if (molecule != null)
            {
                moleculeProperty.isExpanded = EditorGUI.Foldout( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), moleculeProperty.isExpanded, "" );
                if (moleculeProperty.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    position.y += lineHeight;
                    EditorGUI.LabelField( position, "Select relevant components..." );

                    foreach (MoleculeComponent component in molecule.components)
                    {
                        Debug.Log( component.id + " " + component.states[0] );
                    }

                    heights[index] += lineHeight;
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}