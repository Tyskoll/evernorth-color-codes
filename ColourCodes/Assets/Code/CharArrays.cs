using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Evernorth.ColourCodes
{
    public class CharArrays
    {
        public char[,,] CharArray3D;
        public string characters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890!@#$%^&*()-_=+[{]}|\\;:'\",./<>? ";

        public Dictionary<Vector3Int, char> charToColorTable;

        public CharArrays()
        {
            CharArray3D = new char[255, 255, 255];
            charToColorTable = new Dictionary<Vector3Int, char>();
        }



        public void SetChar(char cKey, Vector3Int cPos)
        {
            CharArray3D[cPos.x, cPos.y, cPos.z] = cKey;
        }
        public char GetChar(Vector3Int cPos)
        {
            char c = CharArray3D[cPos.x, cPos.y, cPos.z];
            return c;
        }

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

        public void SetCharColorPair(Vector3Int col, char c)
        {
            charToColorTable.Add(col, c);
        }
    }
}
