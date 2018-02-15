using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AICS.AgentSim
{
    public static class Parser 
    {
        static bool madeTXTFile = false;

        public static string LoadTextData (string filePath) 
        {
            string path = CreateTXTFile( filePath );
            TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath( path, typeof(TextAsset) );

            if (asset == null) 
            {
                Debug.LogWarning( "Data at " + path + " could not be loaded as a text asset or is empty." );
                DeleteTXTFile( path );
                return "";
            }
            string data = asset.text;
            DeleteTXTFile( path );
            return data;
        }

        static string CreateTXTFile (string path)
        {
            if (!Path.GetExtension( path ).Contains( "txt" )) 
            {
                madeTXTFile = true;
                string newPath = "Assets/Models/" + Path.GetFileNameWithoutExtension( path ) + ".txt";
                File.Copy( path, newPath, true );
                AssetDatabase.ImportAsset( newPath );
                return newPath;
            }
            else
            {
                madeTXTFile = false;
                return path;
            }
        }

        static void DeleteTXTFile (string path)
        {
            if (madeTXTFile && Path.GetExtension( path ).Contains("txt")) 
            {
                AssetDatabase.DeleteAsset( path );
            }
        }
    }
}