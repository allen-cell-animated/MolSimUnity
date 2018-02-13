using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    [CustomPropertyDrawer(typeof(Reaction))]
    public class ReactionDrawer : PropertyDrawer
    {
        float height = 0;

        float lineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
        {
            return height;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            height = 0;

            if (property.objectReferenceValue != null)
            {
                SerializedObject reaction = new SerializedObject( property.objectReferenceValue as Reaction );
                SerializedProperty description = reaction.FindProperty( "description" );
                SerializedProperty rate = reaction.FindProperty( "rate" );

                EditorGUI.BeginProperty( position, label, property );

                EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), property );

                property.isExpanded = EditorGUI.Foldout( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), property.isExpanded, "" );
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    height += lineHeight;
                    EditorGUI.PropertyField( new Rect( position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight ), description );
                    height += lineHeight;
                    EditorGUI.PropertyField( new Rect( position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight ), rate );
                }

                EditorGUI.EndProperty();

                reaction.ApplyModifiedProperties();
            }
            else
            {
                Rect fieldRect = new Rect( position.x + EditorGUIUtility.labelWidth, position.y, EditorGUIUtility.currentViewWidth / 3f, position.height );
                Rect btnRect = new Rect( position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.currentViewWidth / 3f + 10f, position.y, 80f, position.height );

                EditorGUI.LabelField( position, "Select Reaction..." );

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                label.text = "";
                EditorGUI.PropertyField( fieldRect, property, label );

                EditorGUI.indentLevel = indent;
                if (GUI.Button( btnRect, "create new" ))
                {
                    string path = EditorUtility.SaveFilePanel("Create Reaction", "Assets/Models/Reactions/", "newReaction", "asset");
                    if (!string.IsNullOrEmpty( path ))
                    {
                        string[] p = path.Split( new string[]{ "Assets" }, System.StringSplitOptions.None );
                        if (p.Length > 1)
                        {
                            Reaction asset = ScriptableObject.CreateInstance<Reaction>();
                            AssetDatabase.CreateAsset(asset, "Assets" + p[1]);
                            AssetDatabase.SaveAssets();
                            property.objectReferenceValue = asset;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog( "Failed to Save Reaction!", "Please choose a path inside the Assets directory.", "ok" );
                        }
                    }
                }
            }

            height += EditorGUIUtility.singleLineHeight;
        }
    }
}