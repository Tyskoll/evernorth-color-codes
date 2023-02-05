using System;
using System.Collections.Generic;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Encrypt
    {
        public char[,,] CharArray3D;
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$%^&*()-_=+[{]}|\\;:'\",./<>? \n\r";

        public Dictionary<char, Vector3Int> CharToColorKey;

        public Encrypt()
        {
            CharArray3D = new char[255, 255, 255];
            CharToColorKey = new Dictionary<char, Vector3Int>();
            AssignCharColors();
        }

        // Cheeck if the index position is empty
        public bool IndexZero(Vector3Int cPos)
        {
            string c = CharArray3D[cPos.x, cPos.y, cPos.z].ToString();

            bool isZero = CharArray3D[cPos.x, cPos.y, cPos.z] == '\u0000';

            switch (isZero)
            {
                case true: return true;
                case false: return false;
            }
        }

        // Add a KeyValuePair to the dictionary
        public void SetCharColorPair(Vector3Int col, char c)
        {
            CharToColorKey.Add(c, col);
        }

        // Look up a character and get a Vector3Int from the dictionary
        Vector3Int ColorLookup(char c)
        {
            Vector3Int colV;
            CharToColorKey.TryGetValue(c, out colV);

            return colV;
        }

        // Encode our provided string as Vector3Int using ColorLookup()
        public Vector3Int[] StringToColor(string s)
        {
            Vector3Int[] cArray = new Vector3Int[s.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                Vector3Int colV = ColorLookup(s[i]);
                cArray[i] = colV;
                Debug.Log(
                    $"Char: {s[i]}\n" +
                    $"Colr: {colV}"
                    );
            }

            return cArray;
        }

        // Generate random Vector3Int for each char in character string
        public void AssignCharColors()
        {
            int x;
            int y;
            int z;
            Vector3Int cPos;

            foreach (char c in characters)
            {
                x = UnityEngine.Random.Range(0, 255);
                y = UnityEngine.Random.Range(0, 255);
                z = UnityEngine.Random.Range(0, 255);

                cPos = new Vector3Int(x, y, z);

                try
                {
                    if (IndexZero(cPos))
                    {
                        CharArray3D[cPos.x, cPos.y, cPos.z] = c;
                        SetCharColorPair(cPos, c);

                        //InstCube(cPos);
                        //Debug.Log($"Inserted {c} at {cPos.ToString()}");
                    }
                    else
                    {
                        Debug.Log($"Index not empty at: {cPos.ToString()}, occupied by: {CharArray3D[cPos.x, cPos.y, cPos.z].ToString()}");
                    }

                }
                catch (Exception e)
                {
                    Debug.Log($"Error: {e}");
                }

            }

            Debug.Log(
                $"-- Dictionary Key -- \n" +
                $"Length: {CharToColorKey.Count}");
        }
    }

}
