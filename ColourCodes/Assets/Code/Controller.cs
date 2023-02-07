using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Evernorth.ColourCodes
{
    public class Controller : MonoBehaviour
    {
        //Classes
        public Encrypt encrypt;
        public Decrypt decrypt;
        public SendReceive sendReceive;

        Vector3Int[] eV3Data;

        //Rendering
        public Image colorImage;
        public Light spotLight;
        public Color32 currentColor;

        //Helpers
        public bool isEncrypted;
        public bool isDecrypted;
        public bool hasData;
        public bool bCasting;
        public bool endOfStream;
        public bool sentData;
        public bool isDecrypting;

        public bool compareMatch;

        public float ticker;

        //UI
        public TMP_InputField textInput;
        public TMP_InputField textOutput;
        public TMP_Text textValidate;

        public Button btn_encrypt;
        public Button btn_send;
        public Button btn_decrypt;
        public Button btn_strCompare;

        public float value;
        public Slider slider_percentage;
        public TMP_Text percentage_txt;
        public Image sliderFillImage;

        private IEnumerator progressBar;
        private IEnumerator colorToString;

        public bool isRendering;

        // Start is called before the first frame update
        void Start()
        {
            // Instantiate new Encrypt and generate KeyValuePairs
            encrypt = new Encrypt();

            // Swap pairs
            Dictionary<Vector3Int, char> colorToCharKey = SwapPairs();
            // Instantiate new Decrypt with swapped KeyValuePairs
            decrypt = new Decrypt(colorToCharKey);

            sendReceive = new SendReceive(colorImage, spotLight);
        }

        Dictionary<Vector3Int, char> SwapPairs()
        {
            Dictionary<Vector3Int, char> colorToCharKey = new Dictionary<Vector3Int, char>();

            foreach(KeyValuePair<char, Vector3Int> pair in encrypt.CharToColorKey)
            {
                colorToCharKey.Add(pair.Value, pair.Key);
            }

            return colorToCharKey;
        }

        public void RenderToggle()
        {
            if (!isRendering)
                isRendering = true;
            else if (isRendering)
                isRendering = false;
        }

        public void EncryptString()
        {
            if(!hasData && !endOfStream)
            {
                string s = textInput.text;
                eV3Data = encrypt.StringToColor(s);
            }
        }

        public void SendData()
        {
            if (!hasData && !endOfStream)
            {
                sentData = true;
                sendReceive.ReceiveData(eV3Data);
            }
        }

        public void DecryptData()
        {


            //string s = decrypt.ReceiveData(sendReceive.dataStream);
            decrypt.ReceiveData(sendReceive.dataStream);

            progressBar = ProgressBar();
            StartCoroutine(progressBar);

            colorToString = decrypt.ColorToString(decrypt.colArray);
            StartCoroutine(colorToString);

            //textOutput.text = s;


            //isDecrypted = true;
        }

        public void Validate()
        {
            //textInput.text = "NO U"; //to confirm mismatch works

            string i = textInput.text;
            string o = textOutput.text;

            compareMatch = string.Equals(i, o);

            if(compareMatch)
            {
                textValidate.text = "Match";
                textValidate.color = Color.green;
            }
            else if(!compareMatch)
            {
                textValidate.text = "Mismatch";
                textValidate.color = Color.red;
            }
        }

        // Update is called once per frame
        void Update()
        {
            ticker = sendReceive.ticker;
            isEncrypted = encrypt.isEncrytped;
            bCasting = sendReceive.bCasting;
            hasData = sendReceive.hasData;
            endOfStream = sendReceive.endOfStream;
            currentColor = sendReceive.colorOut;
            isDecrypting = decrypt.isDecrypting;
            isDecrypted = decrypt.isDecrypted;

            if(isEncrypted && !hasData)
            {
                btn_encrypt.interactable = false;
                btn_send.interactable = true;
            }

            if (bCasting && !endOfStream)
            {
                btn_encrypt.interactable = false;
                btn_send.interactable = false;
                btn_decrypt.interactable = false;
                btn_strCompare.interactable = false;
            }
            
            if(endOfStream && !isDecrypted)
            {
                btn_send.interactable = false;
                btn_decrypt.interactable = true;
            }

            if(isDecrypted)
            {
                btn_decrypt.interactable = false;
                btn_strCompare.interactable = true;
                StopCoroutine(colorToString);
                StopCoroutine(progressBar);
                textOutput.text = decrypt.outputText;
            }

            if (isRendering && sentData && !endOfStream)
            {
                sendReceive.Update();
            }
            else if (!isRendering && sentData && !endOfStream && hasData)
            {
                btn_send.interactable = false;
                btn_decrypt.interactable = true;

                sendReceive.endOfStream = true;
                Debug.Log("-- End of Stream --");
                
            }


        }

        IEnumerator ProgressBar()
        {
            Debug.Log("Called: ProgressBar");

            float slideValue = 0.0f;
            //value = decrypt.dataPos;
            float endValue = 0;

            if(decrypt.colArray != null)
                endValue = decrypt.colArray.Length;

            while (value <= endValue) //for (int i = 0; i < endValue; i++) //
            {
                value = decrypt.dataPos;

                if (value != 0)
                {
                    slideValue = value / endValue * 100;
                }

                Debug.Log(
                $"dataPos: {value}\n" +
                $"slideValue: {slideValue}\n" +
                $"endValue: {endValue}");

                yield return new WaitForEndOfFrame();

                UpdateSlider(slideValue);
            }
        }

        void UpdateSlider(float slideValue)
        {
            slider_percentage.value = slideValue;
            percentage_txt.text = $"{Mathf.Round(slider_percentage.value)}%";
            if(Mathf.Round(slider_percentage.value) == 100)
            {
                sliderFillImage.color = Color.green;
            }
        }

    }

}
