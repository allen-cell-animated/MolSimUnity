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
            symbols["parameters"] = new Func<List<string>, BNGLFileData, int>(ParseParameters);
            symbols["molecule types"] = new Func<List<string>, BNGLFileData, int>(ParseMoleculeTypes);
            symbols["seed species"] = new Func<List<string>, BNGLFileData, int>(ParseSeedSpecies);
            symbols["observables"] = new Func<List<string>, BNGLFileData, int>(ParseObservables);
            symbols["reaction rules"] = new Func<List<string>, BNGLFileData, int>(ParseReactionRules);
            symbols["actions"] = new Func<List<string>, BNGLFileData, int>(ParseActions);

            //@TODO: What are species? They are not in the textbook spec
        }

        // The Directory where we expect to find BNGL files
        private const string bnglFileDirectory = "Assets/Models/BNGL";

        // The location in the Unity Editor where this parser can be used from
        private const string unityEditorMenuLocation = "AICS/Parse BNGL";

        private const string commentSymbol = "#";

        private const string fileErrorMsg = "Improperly Formatted BNGL File:";

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
        /// <summary>
        /// Parse a file's contents for keywords (words that signify the type of parsing needed)
        /// e.g. "molecule type" -> Molecule specific parsing
        /// </summary>
        /// <param name="file_contents"> A string containing the contents of a local file read into memory</param>
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
                Debug.LogError(String.Format("{0} the number of begin statments does not equal the number of end statements",fileErrorMsg));
                return;
            }

            if (begin_indicies.Count != keywords.Count)
            {
                Debug.LogError(String.Format("{0} the number of begin statments does not equal the number of keywords", fileErrorMsg));
                return;
            }

            BNGLFileData fdata = new BNGLFileData();

            // Divide the document into info-blocks and parse accordingly
            while (begin_indicies.Count > 0)
            {
                int begin = begin_indicies.Dequeue();
                int end = end_indicies.Dequeue();
                List<string> info = new List<string>();
                string key = keywords.Dequeue();

                for(int j = begin + 1; j < end; ++j) // ignore the lines with 'begin' or 'end'
                {
                    // Check for comment lines
                    string currentLine = file_lines[j];
                    if (currentLine.Contains(commentSymbol))
                    {
                        currentLine = currentLine.Substring(0, currentLine.IndexOf(commentSymbol)); // remove comments from file line
                    }

                    // Check for empty or null lines
                    currentLine = currentLine.Trim();
                    if (string.IsNullOrEmpty(currentLine)) continue;

                    // Check for the line continuation character
                    while(currentLine.EndsWith(@"\") && j < (end - 1))
                    {
                        j++;

                        // remove the continuation character and append the next line
                        currentLine = currentLine.Substring(0, currentLine.Length - 1) + file_lines[j];
                    }

                    // If the line isn't a comment line, empty, or null, add the line to the parseable info
                    info.Add(currentLine);
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

        /// <summary>
        /// Parses a list of file-lines, line by line, for parameter data
        /// Generally, expected to be called first in file parsing
        /// Parameter data is used in subsequent sections of the BNGL file
        /// </summary>
        /// <param name="info"> The strings (file lines) to parse for data</param>
        /// <param name="fdata"> The BNGL File data object that holds parsed data</param>
        /// <returns>0(int) for successfull completion</returns>
        private int ParseParameters(List<string> info, BNGLFileData fdata)
        {
            foreach (string p in info)
            {
                string[] p_arr = p.Split(
                                        new[] { " " },
                                        StringSplitOptions.RemoveEmptyEntries);
                if (p_arr.Length != 2)
                {
                    Debug.LogError(String.Format("{0} Parameter has too many or too few arguments.\n{1}", fileErrorMsg, p));
                    return -1;
                }

                float num = 0;
                bool isNum = float.TryParse(p_arr[1], out num);

                if (isNum) // a number was assigned as a parameter value
                {
                    fdata.parameters.Add(p_arr[0], num);
                }
                else // an equation (with parameters) was defined as a parameter value
                {
                    num = MathParser.Evaluate(p_arr[1], fdata.parameters);
                    fdata.parameters.Add(p_arr[0], num);
                }
            }

            return 0;
        }

        /// <summary>
        /// Parses a list of file-lines, line by line, for molecule type data
        /// Generally, expected to be called second in file parsing
        /// Molecule types describe the allowed states of molecule binding-sites in the simulation
        /// </summary>
        /// <param name="info"> The strings (file lines) to parse for data</param>
        /// <param name="fdata"> The BNGL File data object that holds parsed data</param>
        /// <returns>0(int) for successfull completion</returns>
        private int ParseMoleculeTypes(List<string> info, BNGLFileData fdata)
        {
            // parse each molecule line-by-line
            foreach (string s in info)
            {
                for (int i = 0; i < info.Count; ++i)
                {
                    string line = info[i];

                    MoleculeFileData md = ParseMolecule(line);
                    fdata.molecules.Add(md);
                }
            }

            return 0;
        }


        /// <summary>
        /// Parses a list of file-lines, line by line, for seed species data
        /// Generally, expected to be called third in file parsing
        /// Seed species are species present at the beginning of the simulation
        /// </summary>
        /// <param name="info"> The strings (file lines) to parse for data</param>
        /// <param name="fdata"> The BNGL File data object that holds parsed data</param>
        /// <returns>0(int) for successfull completion</returns>
        private int ParseSeedSpecies(List<string> info, BNGLFileData fdata)
        {
            foreach(string s in info)
            {
                int rp = s.IndexOf(")"); // assuming the following layout:
                                         // [molecule]"(" [binding-sites] ")" [numerical value]

                MoleculeFileData md = ParseMolecule(s.Substring(0, rp + 1));
                string iv_string = s.Substring(rp + 1).Trim();

                // Check that a value was specified for the seed species
                if(String.IsNullOrEmpty(iv_string))
                {
                    Debug.LogError(String.Format("{0} Seed Species is missing a value.\n{1}", fileErrorMsg, s));
                    return -1;
                }

                SeedSpeciesFileData ssd = new SeedSpeciesFileData();
                ssd.molecule = md;

                float n = 0;
                bool isNum = float.TryParse(iv_string, out n);

                if(isNum) // A number was specified as the initial value for this seed species
                {
                    ssd.initialValue = n;
                }
                else // A parameter was specified as the initial value for this seed species
                {
                    // Check that the specified parameter was specified and parsed
                    if(!fdata.parameters.ContainsKey(iv_string))
                    {
                        Debug.LogError(String.Format("{0} A Seed Species referenced an undefined parameter.\n{1}",fileErrorMsg,s));
                        return -1;
                    }

                    ssd.initialValue = fdata.parameters[iv_string];
                }
            }

            return 0;
        }

        /// <summary>
        /// Parses a list of file-lines, line by line, for observables data
        /// Generally, expected to be called fourth in file parsing
        /// Observables are quantities of interest that are measured (observed) at regular intervals
        /// </summary>
        /// <param name="info"> The strings (file lines) to parse for data</param>
        /// <param name="fdata"> The BNGL File data object that holds parsed data</param>
        /// <returns>0(int) for successfull completion</returns>
        private int ParseObservables(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        /// <summary>
        /// Parses a list of file-lines, line by line, for reaction rules data
        /// Generally, expected to be called fifth in file parsing
        /// Reaction Rules are a text equation that describe how molecules can interact
        /// </summary>
        /// <param name="info"> The strings (file lines) to parse for data</param>
        /// <param name="fdata"> The BNGL File data object that holds parsed data</param>
        /// <returns>0(int) for successfull completion</returns>
        private int ParseReactionRules(List<string> info, BNGLFileData fdata)
        {
            return 0;
        }

        // Unsure if needed at the moment (specifically for the unity simulation prototype)
        private int ParseActions(List<string> info, BNGLFileData fdata)
        {
            Debug.LogWarning("Parsing actions is currently unimplemented");
            return -1;
        }
        #endregion

        #region Private Data Type Parsing
        // All of the Private Data Type Parsing functions assume previous validation for null, empty, or comment lines

        /// <summary>
        /// Parse a file line into a molecule type data object, with a list of binding sites
        /// </summary>
        /// <param name="line"> The file line to parse for data</param>
        /// <returns> The data constructed by parsing the file line</returns>
        private MoleculeFileData ParseMolecule(string line)
        {
            if(!line.Contains("(") || !line.Contains(")"))
            {
                Debug.LogError(String.Format("{0} a molecule is missing parentheses or binding site information.\n{1}", fileErrorMsg,line));
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

        /// <summary>
        /// Parse a string into a a binding site data object, with allowable states
        /// </summary>
        /// <param name="line"> The file line to parse for data</param>
        /// <returns> The data constructed by parsing the file line</returns>
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

        /// <summary>
        /// A file IO internal data object containing information about the parsed BNGL file
        /// </summary>
        private class BNGLFileData
        {
            public List<MoleculeFileData> molecules = new List<MoleculeFileData>();
            public Dictionary<string, float> parameters = new Dictionary<string, float>();
        }

        /// <summary>
        /// A file IO internal data object containing information about a parsed Moleculte Type
        /// </summary>
        private class BindingSiteFileData
        {
            public string name = "";
            public List<string> allowableStates = new List<string>();
        }

        /// <summary>
        /// A file IO internal data object containing information about a parsed Binding Site
        /// </summary>
        private class MoleculeFileData
        {
            public string name = "";
            public List<BindingSiteFileData> bindingSites = new List<BindingSiteFileData>();
        }

        private class SeedSpeciesFileData
        {
            public MoleculeFileData molecule = new MoleculeFileData();
            public float initialValue = 0.0f;
        }
        #endregion
    }
}