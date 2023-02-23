using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evernorth.ColourCodes
{
    public class Tokenize
    {
        public string sData;
        public List<string> tokenData;
        public int uniqueTokens;

        public Tokenize(string sData)
        {
            this.sData=sData;
        }

        public List<string> BuildTokenList()
        {
            tokenData = sData.Split(' ').Distinct().ToList();

            uniqueTokens = tokenData.Count();

            /*string temp = "";

            /*
            foreach(string token in tokenData)
            {
                temp = temp + $"\n{token}";
            }
            
            Debug.Log(
                $"Unique: {uniqueTokens}" +
                $"{temp}");
            */

            Debug.Log(
                $"Unique: {uniqueTokens}");

            return tokenData;
        }


    }

}
