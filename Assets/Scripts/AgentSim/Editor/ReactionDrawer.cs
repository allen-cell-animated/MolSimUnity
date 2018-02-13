//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Reflection;

//namespace AICS.AgentSim
//{
//    [CustomPropertyDrawer(typeof(Reaction))]
//    public class ReactionDrawer : PropertyDrawer
//    {
//        List<float> heights = new List<float>();

//        float lineHeight
//        {
//            get
//            {
//                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
//            }
//        }

//        public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
//        {
//            int index = GetPropertyIndex( fieldInfo, property );
//            LoadHeight( index );
//            return EditorGUIUtility.singleLineHeight + (property.isExpanded && index >= 0 ? heights[index] : 0);
//        }

//        static int GetPropertyIndex (FieldInfo fieldInfo, SerializedProperty property)
//        {
//            var obj = fieldInfo.GetValue( property.serializedObject.targetObject );
//            if (obj != null && obj.GetType().IsArray)
//            {
//                return Convert.ToInt32( property.propertyPath.Split( '[' )[1].Split( ']' )[0] );
//            }
//            return -1;
//        }

//        void LoadHeight (int index)
//        {
//            while (heights.Count < index + 1)
//            {
//                heights.Add( 0 );
//            }
//        }

//        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
//        {
//            int index = GetPropertyIndex( fieldInfo, property );
//            LoadHeight( index );
//            heights[index] = 0;

//            if (property.objectReferenceValue != null)
//            {
//                SerializedObject reaction = new SerializedObject( property.objectReferenceValue as Reaction );
//                SerializedProperty description = reaction.FindProperty( "description" );
//                SerializedProperty rate = reaction.FindProperty( "rate" );
//                SerializedProperty singleReactants = reaction.FindProperty( "singleReactants" );

//                EditorGUI.BeginProperty( position, GUIContent.none, property );

//                EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), property, 
//                                         new GUIContent( (GetPropertyIndex( fieldInfo, property ) + 1).ToString() ) );

//                property.isExpanded = EditorGUI.Foldout( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), property.isExpanded, "" );

//                if (property.isExpanded)
//                {
//                    EditorGUI.indentLevel++;

//                    position.y += lineHeight;
//                    EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), description );
//                    position.y += lineHeight;
//                    EditorGUI.PropertyField( new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight ), rate );

//                    DrawSingleMolecules( singleReactants );




//                    heights[index] += 2 * lineHeight;
//                }

//                EditorGUI.EndProperty();

//                reaction.ApplyModifiedProperties();
//            }
//            else
//            {
//                Rect fieldRect = new Rect( position.x + EditorGUIUtility.labelWidth, position.y, EditorGUIUtility.currentViewWidth / 3f, position.height );
//                Rect btnRect = new Rect( position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.currentViewWidth / 3f + 10f, position.y, 80f, position.height );

//                EditorGUI.LabelField( position, "Select Reaction..." );

//                var indent = EditorGUI.indentLevel;
//                EditorGUI.indentLevel = 0;

//                EditorGUI.PropertyField( fieldRect, property, new GUIContent( "" ) );

//                EditorGUI.indentLevel = indent;
//                if (GUI.Button( btnRect, "create new" ))
//                {
//                    string path = EditorUtility.SaveFilePanel("Create Reaction", "Assets/Models/Reactions/", "newReaction", "asset");
//                    if (!string.IsNullOrEmpty( path ))
//                    {
//                        string[] p = path.Split( new string[]{ "Assets" }, System.StringSplitOptions.None );
//                        if (p.Length > 1)
//                        {
//                            Reaction asset = ScriptableObject.CreateInstance<Reaction>();
//                            AssetDatabase.CreateAsset(asset, "Assets" + p[1]);
//                            AssetDatabase.SaveAssets();
//                            property.objectReferenceValue = asset;
//                        }
//                        else
//                        {
//                            EditorUtility.DisplayDialog( "Failed to Save Reaction!", "Please choose a path inside the Assets directory.", "ok" );
//                        }
//                    }
//                }

//                property.isExpanded = EditorGUI.Foldout( position, property.isExpanded, "" );
//            }
//        }

//        void DrawSingleMolecules (SerializedProperty singleMolecules)
//        {

//        }
//    }
//}