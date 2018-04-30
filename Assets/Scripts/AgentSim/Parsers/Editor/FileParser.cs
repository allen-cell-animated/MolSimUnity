using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace AICS.AgentSim
{
    public abstract class FileParser 
    {
        public abstract void Parse(string file_path);

        #region Utility Methods
        // Reads the contents of a local file into memory
        protected bool ReadIntoMemory(string file_path, out string out_file_contents)
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file_path))
                {
                    // Read the stream to a string for external use
                    out_file_contents = sr.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("The file could not be read:");
                Debug.LogError(e.Message);
            }

            out_file_contents = "";
            return false;
        }
        #endregion
    }
}