using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EditorTools.JSON;
using UnityEngine;

namespace StudyTools.StudyEvaluator
{
    /// <summary>
    /// Base class for StudyState Evaluators
    /// </summary>
    public class StudyStateEvaluator : MonoBehaviour
    {
        [Header("Identification")] 
        [Tooltip("Name of the study")]
        public string studyName;

        [Tooltip("Format string for Identification Index")]
        public string identifierPattern = "{0}";
        
        public int version = 1;

        private int identifierIndex = 1;
        public string identifier { get; private set; } = "";

        [Header("Output")] 
        [Tooltip("Path to the Evaluator Config file")]
        public string config = "study/config/evaluator.json";
        
        [Tooltip("PBase path for evaluation results")]
        public string basePath = "study/results/";
        [Tooltip("Path relative to base path for additional data")]
        public string dataPath = "data/";
        
        
        [Header("Serialisation Options")] 
        public int indent;

        private int currentIndent = 0;
        
        private StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Called on Initialisation
        /// </summary>
        public void Awake()
        {
            if (File.Exists(config))
            {
                JsonParser parser = new JsonParser();
                parser.loadFile(config);
                Dictionary<string, object> dict = parser.readObject();

                identifierIndex = (int) dict["identifierIdx"];
            }

            identifier = String.Format(identifierPattern, identifierIndex);
            Debug.Log("Created Identifier: " + identifier);
        }

        #region serialisation

        #region tools
        /// <summary>
        /// Increment indent for serialisation
        /// </summary>
        public void incrementIndent()
        {
            currentIndent += indent;
        }

        /// <summary>
        /// Decrement indent for serialisation
        /// </summary>
        public void decrementIndent()
        {
            currentIndent -= indent;
        }

        /// <summary>
        /// Print indent for serialisation
        /// </summary>
        public void addIndent()
        {
            for (int i = 0; i < currentIndent; i++)
            {
                sb.Append(" ");
            }
        }

        /// <summary>
        /// Append a new line for serialization
        /// </summary>
        public void newline()
        {
            sb.Append("\n");
        }

        /// <summary>
        /// Bsgins a JSON Dictionary
        /// </summary>
        /// <param name="key">key of the dictionary</param>
        public void beginNamedObject(string key)
        {
            prepareLine(key);
            sb.Append("{");
            incrementIndent();
            newline();
        }
        
        /// <summary>
        /// Ends a JSON dictionary
        /// </summary>
        public void endObject()
        {
            decrementIndent();
            addIndent();
            sb.Append("},");
            newline();
        }
        
        /// <summary>
        /// Prepares a JSON line until the value has to be printed
        /// </summary>
        /// <param name="key">key to use for this line</param>
        private void prepareLine(string key)
        {
            addIndent();
            sb.Append("\"");
            sb.Append(key);
            sb.Append("\": ");
        }
        
        /// <summary>
        /// Write a line with a string value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="value">Value for the line</param>
        public void writeLine(string key, string value)
        {
            prepareLine(key);
            sb.Append("\"");
            sb.Append(value);
            sb.Append("\"");
            sb.Append(",");
            newline();
        }

        /// <summary>
        /// Write a line with an int value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="value">Value for the line</param>
        public void writeLine(string key, int value)
        {
            prepareLine(key);
            sb.Append(value);
            sb.Append(",");
            newline();
        }
        
        /// <summary>
        /// Write a line with a float value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="value">Value for the line</param>
        public void writeLine(string key, float value)
        {
            prepareLine(key);
            sb.Append(value);
            sb.Append(",");
            newline();
        }
        
        /// <summary>
        /// Write a line with a boolean value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="value">Value for the line</param>
        public void writeLine(string key, bool value)
        {
            prepareLine(key);
            sb.Append(value);
            sb.Append(",");
            newline();
        }

        /// <summary>
        /// Write a line with a Vector value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="vector">Value for the line</param>
        public void writeLine(string key, Vector3 vector)
        {
            prepareLine(key);
            sb.Append("[");
            sb.Append(vector.x);
            sb.Append(", ");
            sb.Append(vector.y);
            sb.Append(", ");
            sb.Append(vector.z);
            sb.Append("],");
            newline();
        }
        
        /// <summary>
        /// Write a line with a transform value
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="transform">Value for the line</param>
        public void writeLine(string key, Transform transform)
        {
            beginNamedObject(key);
            
            writeLine("position", transform.position);
            writeLine("rotation", transform.eulerAngles);
            writeLine("localScale", transform.localScale);

            endObject();
        }

        /// <summary>
        /// Write a line with a reference to a texture file in data
        /// </summary>
        /// <param name="key">Key for the line</param>
        /// <param name="name">Name of the texture to store</param>
        /// <param name="texture">Texture to store</param>
        public void writeLine(string key, string name, Texture2D texture)
        {

            String texturePath = basePath + identifier + "/" + dataPath + name + ".png";
            texture.alphaIsTransparency = true;
            byte[] tex = texture.EncodeToPNG();
            
            Directory.CreateDirectory(Path.GetDirectoryName(texturePath));
            File.WriteAllBytes(texturePath, tex);
            
            writeLine(key, texturePath);
        }

        #endregion
        
        /// <summary>
        /// Writes the header lines 
        /// </summary>
        public void writeHeader()
        {
            sb.Append("[");
            incrementIndent();
            newline();
            addIndent();
            sb.Append("{");
            incrementIndent();
            newline();

            writeLine("timestamp", DateTime.Now.ToString("s"));
            writeLine("study", studyName);
            writeLine("id", this.identifier);
            writeLine("version", version);
            
            writeHeaderLines();
            
            addIndent();
            sb.Append("\"states\": [");
            incrementIndent();
        }

        /// <summary>
        /// Writes the footer lines
        /// </summary>
        public void writeFooter()
        {
            decrementIndent();
            addIndent();
            sb.Append("],");
            decrementIndent();
            newline();
            addIndent();
            sb.Append("}");
            decrementIndent();
            newline();
            addIndent();
            sb.Append("]");
        }

        /// <summary>
        /// Starts writing a state
        /// </summary>
        /// <param name="state">Index of the state to write</param>
        public void startWriteState(int state)
        {
            addIndent();
            sb.Append("{");
            incrementIndent();
            newline();
            writeLine("state", state);
        }

        /// <summary>
        /// End writing state
        /// </summary>
        /// <param name="state">Index of the state to end writing</param>
        public void endWriteState(int state)
        {
            decrementIndent();
            addIndent();
            sb.Append("},");
            newline();
        }
        
        /// <summary>
        /// Open the state to write
        /// </summary>
        /// <param name="state">Index of the State</param>
        public void startState(int state)
        {
            startWriteState(state);
            startStateData(state);
        }
        
        /// <summary>
        /// End state data writing
        /// </summary>
        /// <param name="state">index of the state</param>
        public void endState(int state)
        {
            endStateData(state);
            endWriteState(state);
            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Finalize writing the file, and writes the actual data to disk
        /// </summary>
        public void finalize()
        {
            writeFooter();
            Directory.CreateDirectory(Path.GetDirectoryName(basePath + identifier + ".json"));
            File.WriteAllText(basePath + identifier + ".json", sb.ToString(), Encoding.UTF8);
            Debug.Log("Protocol written to " + basePath + identifier + ".json");

            String configText = "{\n  \"identifierIdx\": " + identifierIndex +  " , \n }";
            Directory.CreateDirectory(Path.GetDirectoryName(config));
            File.WriteAllText(config, configText, Encoding.UTF8);
        }

        #endregion
        
        /// <summary>
        /// Override this function to add custom JSON text to the headder
        /// </summary>
        public virtual void writeHeaderLines()
        {
            // Not Implemented
        }

        /// <summary>
        /// Override this to add custom data to the start of a state (when the state is loaded)
        /// </summary>
        /// <param name="state">index of the state to get the data for</param>
        public virtual void startStateData(int state)
        {
            // Not Implemented
        }
        
        /// <summary>
        /// Override this to add custom data to the end of a state (when the state is unloaded)
        /// </summary>
        /// <param name="state">index of the state to get the data for</param>
        public virtual void endStateData(int state)
        {
            // Not Implemented
        }
    }
}