using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AICS.AgentSim
{
    public class BNGLFileParser : FileParser
    {
        // Symbols are parsed words that indicate the need for a specific parsing function
        // e.g. "Molecule" -> ParseMolecule(...)
        // Each symbol is mapped to the requisite parsing function
        private Dictionary<string, Delegate> symbols = new Dictionary<string, Delegate>();

        private BNGLFileParser()
        {
            symbols["molecule types"] = new Func<List<string>, BNGLFileData, int>(ParseMoleculeTypes);
            symbols["seed species"] = new Func<List<string>, BNGLFileData, int>(ParseSeedSpecies);
            symbols["parameters"] = new Func<List<string>, BNGLFileData, int>(ParseParameters);
            symbols["observables"] = new Func<List<string>, BNGLFileData, int>(ParseObservables);
            symbols["reaction rules"] = new Func<List<string>, BNGLFileData, int>(ParseReactionRules);
            symbols["actions"] = new Func<List<string>, BNGLFileData, int>(ParseActions);
        }

        // The Directory where we expect to find BNGL files
        private const string bnglFileDirectory = "Assets/Models/BNGL";

        // The location in the Unity Editor where this parser can be used from
        private const string unityEditorMenuLocation = "AICS/Parse BNGL";

        private const string commentSymbol = "#";

        // A BNGL Parser singleton, for static access in the Unity Editor
        private static BNGLFileParser bnglParser_instance;
        private static BNGLFileParser Instance
        {
            get {
                if (bnglParser_instance == null)
                {
                    bnglParser_instance = new BNGLFileParser();
                }
                return bnglParser_instance;
            }
        }

        // The function to be called by the Unity Editor
        [MenuItem(unityEditorMenuLocation)]
        public static void MenuParse()
        {
            string filePath = EditorUtility.OpenFilePanel("Choose BNGL file", bnglFileDirectory, "bngl");
            Instance.Parse(filePath);
        }

        #region Inherited from FileParser
        public override void Parse(string file_path)
        {
            string file_contents = "";
            this.ReadIntoMemory(file_path, out file_contents);
            this.ParseForKeywords(file_contents);
        }
        #endregion


        #region BNGL Specific Parsing
        private void ParseForKeywords(string file_contents)
        {
            // Divide the string by new lines
            string[] file_lines = file_contents.Split(
                                new[] { "\r\n", "\r", "\n" },
                                StringSplitOptions.RemoveEmptyEntries);

            Queue<int> begin_indicies = new Queue<int>();
            Queue<int> end_indicies = new Queue<int>();
            Queue<string> keywords = new Queue<string>();

            for (int i = 0; i < file_lines.Length; ++i)
            {
                // Check if this line is a comment
                if (file_lines[i].StartsWith(commentSymbol))
                {
                    continue;
                }

                // Record the indicies of the begin and end statements
                // Record relevant keywords attached to begin statements
                if (file_lines[i].StartsWith("begin"))
                {
                    begin_indicies.Enqueue(i);
                    keywords.Enqueue(file_lines[i].Substring(5));
                    continue;
                }

                if (file_lines[i].StartsWith("end"))
                {
                    end_indicies.Enqueue(i);
                    continue;
                }
            }

            // Validity checks
            if(begin_indicies.Count != end_indicies.Count)
            {
                Debug.LogError("Improperly formatted BNGL File: the number of begin statments does not equal the number of end statements");
                return;
            }

            if (begin_indicies.Count != keywords.Count)
            {
                Debug.LogError("Improperly formatted BNGL File: the number of begin statments does not equal the number of keywords");
                return;
            }

            BNGLFileData fdata = new BNGLFileData();

            // Divide the document into info-blocks and parse accordingly
            while (begin_indicies.Count > 0)
            {
                //@TODO: Parse lines ending with continuation character '\' 
                int begin = begin_indicies.Dequeue();
                int end = end_indicies.Dequeue();
                List<string> info = new List<string>();
                string key = keywords.Dequeue();

                for(int j = begin + 1; j < end; ++j) // ignore the lines with 'begin' or 'end'
                {
                    info.Add(file_lines[j]);
                }

                // If the keyword matches one of the defined symbols, call the needed parse-helper function
                foreach(string k in symbols.Keys)
                {
                    if (k.Equals(key.Trim().ToLower()))
                    {
                        symbols[k].DynamicInvoke(info, fdata);
                    }
                }
            }
        }

        private int ParseMoleculeTypes(List<string> info, BNGLFileData fdata)
        {
            // parse each molecule line-by-line
            foreach(string s in info)
            {
                for(int i = 0; i < info.Count; ++i)
                {
                    string line = info[i];
                    if (line.Contains(commentSymbol))
                    {
                        line = line.Substring(0, line.IndexOf(commentSymbol)); // remove comments from file line
                    }

                    line = line.Trim();
                    if (string.IsNullOrEmpty(line)) continue;


                    MoleculeFileData md = ParseMolecule(line);
                    fdata.molecules.Add(md);
                }
            }

            return 0;
        }

        private int ParseReactionRules(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        private int ParseParameters(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        private int ParseSeedSpecies(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        private int ParseObservables(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        private int ParseActions(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }
        #endregion

        #region Private Data Type Parsing
        private MoleculeFileData ParseMolecule(string line)
        {
            if(!line.Contains("(") || !line.Contains(")"))
            {
                Debug.LogError("Improperly formatted BNGL File: a molecule is missing parentheses or binding site information");
                return new MoleculeFileData();
            }

            MoleculeFileData m = new MoleculeFileData();

            int lp = line.IndexOf("(");
            int rp = line.IndexOf(")");
            m.name = line.Substring(0, lp).Trim();

            // remove the name & the "(" and ")" characters
            line = line.Substring(lp + 1, rp - lp - 1);

            // split the binding site info by the "," character
            string[] binding_site_strings = line.Split(
                                new[] { "," },
                                StringSplitOptions.RemoveEmptyEntries);

            // parse the info for each binding site
            for(int i = 0; i < binding_site_strings.Length; ++i)
            {
                m.bindingSites.Add(ParseBindingSite(binding_site_strings[i]));
            }

            return m;
        }

        private BindingSiteFileData ParseBindingSite(string line)
        {
            BindingSiteFileData b = new BindingSiteFileData();
            
            // check if the binding site has states specified
            if(line.Contains("~"))
            {
                int t = line.IndexOf("~");
                b.name = line.Substring(0, t);

                string[] states = line.Substring(t).Split(
                                        new[] { "~" }, 
                                        StringSplitOptions.RemoveEmptyEntries);

                foreach(string s in states)
                {
                    b.allowableStates.Add(s);
                }
            }
            else
            {
                // no states specified
            }

            return b;
        }
        #endregion

        #region Private Data Types
        // these data types are only intended for use in file IO, not for simulation
        private class BNGLFileData
        {
            public List<MoleculeFileData> molecules = new List<MoleculeFileData>();
        }

        private class BindingSiteFileData
        {
            public string name = "";
            public List<string> allowableStates = new List<string>();
        }

        private class MoleculeFileData
        {
            public string name = "";
            public List<BindingSiteFileData> bindingSites = new List<BindingSiteFileData>();
        }
        #endregion
    }
}