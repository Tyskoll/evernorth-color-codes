using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using UnityEngine.TextCore.Text;

namespace Evernorth.ColourCodes
{
    public class Controller : MonoBehaviour
    {
        //Classes
        public Encrypt encrypt;
        public Decrypt decrypt;
        public IOServer sendReceive;
        public Texture texture;

        Vector3Int[] eV3Data;
        string eStringData;

        //Rendering
        public Image imagePixelCode; 
        public Image imageColorStream;
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
        public TMP_Text textShiftSeed;
        public TMP_InputField textInput;
        public TMP_InputField textOutput;
        public TMP_Text textValidate;
        public TMP_Text textFileLength;

        public Button btn_encrypt;
        public Button btn_send;
        public Button btn_decrypt;
        public Button btn_strCompare;

        public Toggle tgl_isString;
        public Toggle tgl_isVector3;
        public Toggle tgl_isRendering;
        public Toggle tgl_isTexturing;


        public float value;
        public Slider slider_percentage;
        public TMP_Text percentage_txt;
        public Image sliderFillImage;

        private IEnumerator progressBar;
        private IEnumerator colorToString;

        public bool isString = true;
        public bool isVector3 = false;
        public bool isTexturing = true;
        public bool isRendering = false;


        // Start is called before the first frame update
        void Start()
        {
            SetToggles();
            

            Thread thread = Thread.CurrentThread;
            // Instantiate new Encrypt and generate KeyValuePairs
            encrypt = new Encrypt();

            // Swap pairs
            Dictionary<Vector3Int, char> colorToCharKey = SwapKeyPair1();
            //Dictionary<char, Vector3Int> charToColorKey = SwapKeyPair2();

            // Instantiate new Decrypt with IOServer reference, swapped KeyValuePairs and shuffled key
            decrypt = new Decrypt(sendReceive, colorToCharKey, encrypt.charactersS);//, charToColorKey);

            // Instantiate new SendReceive with image reference
            sendReceive = new IOServer(imageColorStream);


            textShiftSeed.text = encrypt.iSeedTxt;

            Debug.Log(
                $"e: {encrypt.charactersS}\n" +
                $"d: {decrypt.charactersS}");
        }

        Dictionary<Vector3Int, char> SwapKeyPair1()
        {
            Dictionary<Vector3Int, char> colorToCharKey = new Dictionary<Vector3Int, char>();

            foreach(KeyValuePair<char, Vector3Int> pair in encrypt.CharToColorKey)
            {
                colorToCharKey.Add(pair.Value, pair.Key);
            }

            return colorToCharKey;
        }

        Dictionary<char, Vector3Int> SwapKeyPair2()
        {
            Dictionary<char, Vector3Int> charToColorKey = new Dictionary<char, Vector3Int>();

            foreach (KeyValuePair<Vector3Int, char> pair in encrypt.ColorToCharKey)
            {
                charToColorKey.Add(pair.Value, pair.Key);
            }

            return charToColorKey;
        }



        #region UI
        public void SetToggles()
        {
            isString = false;
            isVector3 = true;
            isTexturing = true;
            isRendering = false;

            tgl_isString.interactable = true;
            tgl_isVector3.interactable = true;
            tgl_isRendering.interactable = true;
            tgl_isTexturing.interactable = true;
        }

        public void RenderToggle()
        {
            isRendering = true;
            tgl_isTexturing.isOn = false;
            isTexturing = false;
        }

        public void TextureToggle()
        {
            isTexturing = true;
            tgl_isRendering.isOn = false;
            isRendering = false;
        }

        public void StringToggle()
        {
            tgl_isRendering.interactable = false;
            tgl_isRendering.isOn = false;
            isRendering = false;
            tgl_isTexturing.interactable = false;
            tgl_isTexturing.isOn = false;
            isTexturing = false;

            isString = true;
            isVector3 = false;
        }

        public void Vector3Toggle()
        {
            tgl_isRendering.interactable = true;
            tgl_isTexturing.interactable = true;

            isVector3 = true;
            isString = false;
        }

        public void EncryptString()
        {
            if(!hasData && !endOfStream)
            {
                string s = textInput.text;
                textFileLength.text = $"File Length: {s.Length.ToString("#,#")}";
                eV3Data = encrypt.StringToColor(s);

                if (isString)
                {
                    encrypt.BuildString(eV3Data);
                    eStringData = encrypt.Compress(encrypt.vString);
                    /*
                    eStringData = encrypt.ColorToString(eV3Data);
                    Debug.Log(
                        $"StepTwo: \n" +
                        $"{eStringData}");
                    */
                    textFileLength.text =
                    $"NoE File Length: {s.Length.ToString("#,#")}\n" +
                    $"E String Length: {eStringData.Length.ToString("#,#")}" +
                    $"Unique Values:    {encrypt.uniqueValueCount.ToString("#,#")}";

                }
                else
                {
                    textFileLength.text =
                        $"File Length:      {s.Length.ToString("#,#")}\n" +
                        $"Unique Values:    {encrypt.uniqueValueCount.ToString("#,#")}";
                }
            }
        }

        public void SendData()
        {
            sendReceive.iSeed = encrypt.iSeed;

//TEST FOR STRIPPED & PADDED FILE
            textOutput.text = encrypt.vString;

            if (!isString && !hasData && !endOfStream)
            {
                if (isTexturing)
                {
                    Debug.Log("isTexturing");
                    sendReceive.ReceiveData(eV3Data);
                    texture = new Texture(imagePixelCode, sendReceive.dataStream);
                    sentData = true;

                    Texture2D newTexture = texture.CreateTexture();

                    imagePixelCode.material.SetTexture("_BaseMap", newTexture);
                    imagePixelCode.color = Color.white;
                }
                else
                {
                    sendReceive.ReceiveData(eV3Data);
                    sentData = true;
                }
                
                
            }

            if (isString && !hasData && !endOfStream)
            {
                sendReceive.ReceiveSData(eStringData);
                
                sentData = true;
                textOutput.text = eStringData;
            }
        }

        public void DecryptData()
        {
            decrypt.iSeed = sendReceive.iSeed;

            if (!isString)
            {
                decrypt.isString = false;
                decrypt.ReceiveData(sendReceive.dataStream);
            }

            if (isString)
            {
                decrypt.isString = true;
                decrypt.ReceiveSData(sendReceive.sDataStream);
            }



            // Start a new thread to run decryption on
            Thread t2 = new Thread(decrypt.DecryptStart);
            t2.Start();

            progressBar = ProgressBar();
            StartCoroutine(progressBar);
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
#endregion

#region Update
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

                UpdateSlider(100f); //<--- for when it decrypts too fast to update the progress bar.
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
                //Debug.Log("-- End of Stream --");
            }


        }
#endregion

#region ProgressBar

        IEnumerator ProgressBar()
        {
            if (isDecrypted)
                UpdateSlider(100f);
            else
            {
                float slideValue = 0.0f;
                float endValue = 0;

                //if (decrypt.colArray != null)
                    endValue = decrypt.dataLengthTotal;

                //Debug.Log($"EndValue: {endValue}");

                while (value <= endValue)
                {
                    value = decrypt.dataPos1 + decrypt.dataPos2;

                    if (value != 0)
                    {
                        slideValue = value / endValue * 100;
                    }

                    /*
                    Debug.Log(
                    $"dataPos: {value}\n" +
                    $"slideValue: {slideValue}\n" +
                    $"endValue: {endValue}");
                    */

                    yield return new WaitForEndOfFrame();

                    UpdateSlider(slideValue);
                }
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
#endregion
    }

}
