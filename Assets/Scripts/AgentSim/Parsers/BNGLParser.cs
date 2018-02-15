using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public static class BNGLParser
    {
        [MenuItem("AICS/Parse BNGL")]
        public static void ParseBNGLFile ()
        {
            string filePath = EditorUtility.OpenFilePanel( "Choose BNGL file", "Assets/Models", "bngl" );
            string data = Parser.LoadTextData( filePath );
            if (data.Length > 0)
            {
                //Model model = ScriptableObject.CreateInstance<Model>();
                string[] lines = data.Split ('\n');
                bool inSection = false;
                List<string> sectionLines = new List<string>();
                //Dictionary<string,float> parameters;
                foreach (string line in lines)
                {
                    if (line.Length >= 5 && line.Substring( 0, 5 ) == "begin")
                    {
                        inSection = true;
                        sectionLines.Clear();
                    }
                    else if (line.Length >= 3 && line.Substring( 0, 3 ) == "end")
                    {
                        inSection = false;
                        if (line.Contains( "parameters" ))
                        {
                            //parameters = ParseParameters( sectionLines );
                        }
                        else if (line.Contains( "molecule types" ))
                        {
                            ParseMoleculeTypes( sectionLines );
                        }
                        else if (line.Contains( "species" ))
                        {
                            ParseSpecies( sectionLines );
                        }
                        else if (line.Contains( "reaction rules" ))
                        {
                            ParseReactionRules( sectionLines );
                        }
                    }
                    else if (line.Length > 0 && inSection)
                    {
                        sectionLines.Add( line );
                    }
                }
            }
        }

        static Dictionary<string,float> ParseParameters (List<string> lines)
        {
            Dictionary<string,float> parameters = new Dictionary<string,float>();
            foreach (string line in lines)
            {
                string[] words = line.Split( null );
                if (words.Length >= 2)
                {
                    float value;
                    if (float.TryParse( words[1], out value ))
                    {
                        parameters.Add( words[0], value );
                    }
                }
            }
            return parameters;
        }

        static void ParseMoleculeTypes (List<string> lines)
        {
            //List<Molecule> molecules = new List<Molecule>();
            foreach (string line in lines)
            {
                line.Trim();
                //string[] s = line.Split( '(' );
                //TODO
            }
        }

        static void ParseSpecies (List<string> lines)
        {
            Debug.Log( "SPECIES----------------" );
            foreach (string line in lines)
            {
                Debug.Log( line );
            }
        }

        static void ParseReactionRules (List<string> lines)
        {
            Debug.Log( "REACTION RULES----------------" );
            foreach (string line in lines)
            {
                Debug.Log( line );
            }
        }
    }
}