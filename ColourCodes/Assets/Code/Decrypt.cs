using System.Collections.Generic;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Vector3Int[] colArray;
        public string outputText;

        public Decrypt(Dictionary<Vector3Int, char> colorToCharKey)
        {
            this.ColorToCharKey = colorToCharKey;      
        }

        // Retrieve char from KeyPair dictionary based on colour Key provided
        char CharLookup(Vector3Int col)
        {
            char c;
            Vector3Int colV = new Vector3Int(col.x, col.y, col.z);
            ColorToCharKey.TryGetValue(colV, out c);

            return c;
        }

        // Build our string ready for output
        public string ColorToString(Vector3Int[] cArray)
        {
            string s = "";

            for (int i = 0; i < cArray.Length; i++)
            {
                char c = CharLookup(cArray[i]);
                s += c;
            }

            return s;
        }

        // Receive Vector3Int array and assign values to dataStream[]
        // This is just one way of acquiring the data and is sufficient 
        // for proof of concept.
        public string ReceiveData(Vector3Int[] dStream)
        {
            Debug.Log("Decrypt.ReceiveData->");
            colArray = new Vector3Int[dStream.Length];
            System.Array.Copy(dStream, colArray, dStream.Length);

            return ColorToString(colArray);
        }

        // To-do:
        // Read pixel colour instead of values from array.
        public void GetPixel()
        {

        }
    }

}
