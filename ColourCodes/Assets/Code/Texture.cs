using System;
using UnityEngine;
using UnityEngine.UI;

namespace Evernorth.ColourCodes
{
    public class Texture
    {
        public Image image;
        public Material material;
        public Texture2D texture;

        public Vector3Int[] dataStream;

        public Texture(Image image, Vector3Int[] dataStream) 
        {
            this.image = image;
            this.dataStream = dataStream;
            
        }
        public Texture2D CreateTexture()
        {
            int sqrScale = (int)Math.Round(Math.Sqrt(dataStream.Length));
            //Debug.Log($"SqrScale: {sqrScale}");
            image.rectTransform.sizeDelta = new Vector2(sqrScale, sqrScale);
            texture = new Texture2D(sqrScale, sqrScale, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.anisoLevel= 0;

            int i = 0;

            for (int bY = sqrScale; bY > 0; bY--)
            {
                for (int bX = sqrScale; bX > 0; bX--)
                {
                    if (i < dataStream.Length)
                    {
                        int colX = dataStream[i].x;
                        int colY = dataStream[i].y;
                        int colZ = dataStream[i].z;
                        int colA = 255;

                        Color32 newCol = new Color32((byte)colX, (byte)colY, (byte)colZ, (byte)colA);

                        //Debug.Log($"x: {bX} y: {bY} col: {newCol}");
                        texture.SetPixel(bX, bY, newCol);

                        i++;
                    }
                    else
                    {
                        Color32 newCol = new Color32((byte)0, (byte)0, (byte)0, (byte)255);

                        //Debug.Log($"x: {bX} y: {bY} col: {newCol}");
                        texture.SetPixel(bX, bY, newCol);
                    }
                }
            }

            texture.Apply();

            Save_Texture2D(texture, @"F:\Unity Projects\EncryptToColour\Image Output\out.png");

            return texture;
        }

        void Save_Texture2D(in Texture2D tex, string path)
        {
            byte[] bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
        }
    }
}

