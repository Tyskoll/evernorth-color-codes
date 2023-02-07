using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Decrypt
    {
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Vector3Int[] colArray;
        public string outputText;

        Vector3Int colV = new Vector3Int(0, 0, 0);
        char c = '\u0000';

        public int dataPos;
        public bool isDecrypting;
        public bool isDecrypted;

        public string outString;

        public Vector3Int[] cArray;

        public Decrypt(Dictionary<Vector3Int, char> colorToCharKey)
        {
            this.ColorToCharKey = colorToCharKey;      
        }

        // Receive Vector3Int array and assign values to dataStream[]
        // This is just one way of acquiring the data and is sufficient 
        // for proof of concept.
        public void ReceiveData(Vector3Int[] dStream)
        {
            colArray = dStream;
        }

        public void DecryptStart()
        {
            ColorToString(colArray);
        }
        
        public void ColorToString(Vector3Int[] cArray)
        {
            Debug.Log("called ColorToString");
            string s = "";

            isDecrypting = true;

            for (int i = 0; i < cArray.Length; i++)
            {
                char c = CharLookup(cArray[i]);
                s += c;
                dataPos += 1;

                //Debug.Log(
                //    $"dataPos: {dataPos}\n" +
                //    $"char:    {c}");
                
                if (dataPos >= cArray.Length)
                {
                    isDecrypting = false;
                    isDecrypted = true;
                    Debug.Log($"End ColorToString");
                }
            }

            outputText = s;
        }

        // Retrieve char from KeyPair dictionary based on colour Key provided
        char CharLookup(Vector3Int col)
        {
            colV.x = col.x;
            colV.y = col.y;
            colV.z = col.z;

            try
            {
                ColorToCharKey.TryGetValue(colV, out c);
                return c;
            }
            catch (System.Exception e)
            {
                Debug.Log($"character not found for {colV.ToString()}");
            }

            return c;
        }

        // To-do:
        // Read pixel colour instead of values from array.
        public void GetPixel()
        {

        }
    }

}
