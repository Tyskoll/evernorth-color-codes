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
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$£%^&*()-_=+[{]}|\\;:'\",./<>?`~ \n\r\t";
        public string charactersE = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$£%^&*()-_=+[{]}|\\;:'\",./<>?`~ÁÉÚá³";

        public int[] iSeed;
        public string iSeedTxt;
        public int uniqueValueCount;

        public string vString;

        public Dictionary<char, Vector3Int> CharToColorKey;
        public Dictionary<Vector3Int, char> ColorToCharKey;

        public Dictionary<int, char> IntToCharKey;

        public List<char> UsedChars;

        public bool isEncrytped;

        public Encrypt()
        {
            iSeed = Seed();

            CharArray3D = new char[255, 255, 255];
            CharToColorKey = new Dictionary<char, Vector3Int>();
            AssignCharColors();

            //step 2
            //ColorToCharKey = new Dictionary<Vector3Int, char>();
            //AssignColorToChars(CharToColorKey);

            IntToCharKey = new Dictionary<int, char>();
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
                x = UnityEngine.Random.Range(10, 246);
                y = UnityEngine.Random.Range(10, 246);
                z = UnityEngine.Random.Range(10, 246);

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
   
            }

            isEncrytped = true;
            Debug.Log($"StringToColor completed.");

            Shift(cArray);

            return cArray;
        }

        public string ColorToString(Vector3Int[] nCArray)
        {
            Debug.Log($"ColorToString, Length: {nCArray.Length}");
            string s = "";

            int seedPos = 0;

            for (int i = 0; i < nCArray.Length; i++)
            {
                if (seedPos >= 8)
                    seedPos = 0;

                Debug.Log(
                    $"PreShift" +
                    $"\nx: {nCArray[i].x} y: {nCArray[i].y} z: {nCArray[i].z}");

                int x = nCArray[i].x -= iSeed[seedPos];
                seedPos++;
                int y = nCArray[i].y += iSeed[seedPos];
                seedPos++;
                int z = nCArray[i].z -= iSeed[seedPos];
                seedPos++;

                Vector3Int v3i = new Vector3Int(x, y, z);

                
                Debug.Log(
                    $"PostShift" +
                    $"\nx: {nCArray[i].x} y: {nCArray[i].y} z: {nCArray[i].z}");
                
                char c = CharLookup(v3i);
                
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

        public int[] Seed()
        {
            int[] seedValue = new int[256];

            for(int i = 0; i < 255; i++)
            {
                int d = UnityEngine.Random.Range(0, 10);
                seedValue[i] = d;
            }

            //iSeed printing
            string s = "";
            for (int i = 0; i < seedValue.Length; i++)
            {
                s += seedValue[i];
            }

            iSeed = seedValue;
            iSeedTxt = s;

            Debug.Log(
                $"Encrypt Shift started\n" +
                $"Key: {s}");
            //end

            return seedValue;
        }

        
        public Vector3Int[] Shift(Vector3Int[] cArray)
        {
            int seedPos = 0;

            for(int i = 0; i < cArray.Length; i++)
            {
                /*
                Debug.Log(
                    $"PreShift" +
                    $"\nx: {cArray[i].x} y: {cArray[i].y} z: {cArray[i].z}");
                */
                if (seedPos >= 255)
                    seedPos = 0;

                cArray[i].x += iSeed[seedPos];
                seedPos++;
                cArray[i].y -= iSeed[seedPos];
                seedPos++;
                cArray[i].z += iSeed[seedPos];
                seedPos++;
                seedPos++;  //with room to include alpha value

                /*
                Debug.Log(
                    $"PostShift" +
                    $"\nx: {cArray[i].x} y: {cArray[i].y} z: {cArray[i].z}");
                */
            }

            uniqueValueCount = cArray.Distinct().Count();

            Debug.Log($"Unique Vector3Int count: {uniqueValueCount}");

            return cArray;
        }

        public string BuildString(Vector3Int[] cArray)
        {
            //RESULT PRINTING
            for (int i = 0; i < cArray.Length; i++)
            {
                vString += cArray[i].x.ToString("000");
                vString += cArray[i].y.ToString("000");
                vString += cArray[i].z.ToString("000");
            }

            //END

            return Compress(vString);
        }

        public string Compress(string s)
        {
            string tempString = "";
            int sPos = 0;
            int sLength = (int)Math.Ceiling((double)(s.Length / 2));

            Debug.Log(
                $"s.Length: {s.Length}\n" +
                $"sLength : {sLength}");

            string randCharacters = Shuffle(charactersE);

            for (int i = 0; i < sLength ; i++)
            {
                char c1;
                if ((sPos / 2) >= sLength)
                {
                    c1 = '0'; //This probably fucks things up, may be ok to remove the trailing zero during decrypt
                }
                else
                {
                    c1 = s[sPos];
                }

                sPos++;

                char c2;
                if ((sPos / 2) >= sLength)
                {
                    c2 = '0'; //This probably fucks things up, may be ok to remove the trailing zero during decrypt
                }
                else
                {
                    c2 = s[sPos];
                }
                
                sPos++;

                string nS = ""; 
                nS = nS + $"{c1}{c2}";

                int nI = int.Parse(nS);

                tempString = tempString + randCharacters[nI];
            }

            vString = tempString;

            Debug.Log(
                $"RESULT\n" +
                $"{vString}");


            return vString;
        }
    }



}
