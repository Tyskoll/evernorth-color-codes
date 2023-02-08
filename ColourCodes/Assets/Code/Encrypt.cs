using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Evernorth.ColourCodes
{
    public class Encrypt
    {
        public char[,,] CharArray3D;
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$%^&*()-_=+[{]}|\\;:'\",./<>?`~ \n\r";
        public string charactersE = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$%^&*()-_=+[{]}|\\;:'\",./<>?`~ÁÉÚ";

        public Dictionary<char, Vector3Int> CharToColorKey;
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public List<char> UsedChars;

        public bool isEncrytped;

        public Encrypt()
        {
            CharArray3D = new char[255, 255, 255];
            CharToColorKey = new Dictionary<char, Vector3Int>();
            AssignCharColors();

            //step 2
            ColorToCharKey = new Dictionary<Vector3Int, char>();
            AssignColorToChars(CharToColorKey);

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
                $"-- Dictionary Key1 -- \n" +
                $"Length: {CharToColorKey.Count}");
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

        // Encode our provided string as Vector3Int using ColorLookup()
        public Vector3Int[] StringToColor(string s)
        {
            Vector3Int[] cArray = new Vector3Int[s.Length];

            for (int i = 0; i < cArray.Length; i++)
            {
                Vector3Int colV = ColorLookup(s[i]);
                cArray[i] = colV;
                /*
                Debug.Log(
                    $"Char: {s[i]}\n" +
                    $"Colr: {colV}"
                    );
                */
            }

            isEncrytped = true;
            Debug.Log($"Encryption completed.");

            return cArray;
        }

        public string ColorToString(Vector3Int[] cArray)
        {
            Debug.Log($"ColorToString, Length: {cArray.Length}");
            string s = "";

            for (int i = 0; i < cArray.Length; i++)
            {
                char c = CharLookup(cArray[i]);
                
                s += c;
            }


            return s;
        }

        public string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            System.Random rng = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }

        // Select random char for each Vector3Int in first key
        public void AssignColorToChars(Dictionary<char, Vector3Int> cArray) //pass the first key to here
        {
            UsedChars = new List<char>();

            string randCharacters = Shuffle(charactersE);

            for (int i = 0; i < randCharacters.Length; i++)
            {
                Vector3Int v = cArray.ElementAt(i).Value;

                char c = randCharacters[i];

                if (!UsedChars.Contains(c))
                {
                    ColorToCharKey.TryAdd(v, c);
                    UsedChars.Add(c);
                    /*Debug.Log(
                        $"Col : {v}\n" +
                        $"Char: {c}");*/
                }
                else
                {
                    Debug.Log($"Already used: {c}");
                }
            }

            Debug.Log(
                $"-- Dictionary Key2 -- \n" +
                $"Length: {ColorToCharKey.Count}");  
        }

        // Add a KeyValuePair to the step two dictionary
        public void SetColorCharPair(char c, Vector3Int col)
        {
            ColorToCharKey.Add(col, c);
        }

        // Look up a character and get a Vector3Int from the dictionary
        Vector3Int ColorLookup(char c)
        {
            Vector3Int colV;
            CharToColorKey.TryGetValue(c, out colV);

            return colV;
        }

        char CharLookup(Vector3Int ev3)
        {
            char c;
            ColorToCharKey.TryGetValue(ev3, out c);

            return c;
        }


    }

}
