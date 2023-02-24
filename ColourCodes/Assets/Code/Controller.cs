using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using UnityEngine.TextCore.Text;
using System.IO;

namespace Evernorth.ColourCodes
{
    public class Controller : MonoBehaviour
    {
        //Classes
        public Encrypt encrypt;
        public Decrypt decrypt;
        public IOServer ioServer;
        public Texture texture;

        public Tokenize tokenize;

        Vector3Int[] eV3Data;
        string eStringData;

        //Rendering
        public Image imagePixelCode; 
        public Image imageColorStream;
        public Color32 currentColor;

        //Helpers
        public bool isEncrypting;
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

        bool runOnce = false;

        public float decryptValue;
        public Slider slider_DecryptPercentage;
        public TMP_Text decryptPercentage_txt;
        public Image sliderDecryptFillImage;
        private IEnumerator decryptProgressBar;

        public float encryptValue;
        public Slider slider_EncryptPercentage;
        public TMP_Text encryptPercentage_txt;
        public Image sliderEncryptFillImage;
        private IEnumerator encryptProgressBar;

        public bool isString = true;
        public bool isVector3 = false;
        public bool isTexturing = true;
        public bool isRendering = false;


        // Start is called before the first frame update
        void Start()
        {
            SetToggles();

            LoadResource();

            Thread t0 = Thread.CurrentThread;
            // Instantiate new Encrypt and generate KeyValuePairs
            encrypt = new Encrypt();

            // Swap pairs
            Dictionary<Vector3Int, char> colorToCharKey = SwapKeyPair1();

            // Instantiate new Decrypt with IOServer reference, swapped KeyValuePairs and shuffled key
            decrypt = new Decrypt(ioServer, colorToCharKey, encrypt.charactersS);

            // Instantiate new ioServer with image reference
            ioServer = new IOServer(imageColorStream);

            textShiftSeed.text = encrypt.iSeedTxt;
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

        public void LoadResource()
        {
            string path = @"Assets/Resources/cantrbry_plus/alice29.txt";

            //Read the text from directly from the txt file

            StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8);

            Debug.Log($"FileLength: {reader.BaseStream.Length}");

            textInput.text = reader.ReadToEnd();

            reader.Close();
        }

        #region Tokenizer

        public void BuildTokens()
        {
            tokenize = new Tokenize(textInput.text);
            tokenize.BuildTokenList();
        }

        #endregion

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

        public void UpdateFileDataUI()
        {
            if (isString)
            {
                textFileLength.text =
                $"NoE File Length: {encrypt.stringData.Length.ToString("#,#")}\n" +
                $"E String Length: {encrypt.eStringData.Length.ToString("#,#")}\n" +
                $"Unique Values:    {encrypt.uniqueValueCount.ToString("#,#")}";
            }
            else
            {
                textFileLength.text =
                    $"File Length:      {encrypt.stringData.Length.ToString("#,#")}\n" +
                    $"Unique Values:    {encrypt.uniqueValueCount.ToString("#,#")}";
            }
        }

        public void EncryptData()
        {
            if (isString)
                encrypt.isString = true;

            if (!isString)
                encrypt.isString = false;

            if (!hasData && !endOfStream)
            {
                encrypt.stringData = textInput.text;

                // Start a new thread to run decryption on
                Thread t1 = new Thread(encrypt.EncryptStart);
                t1.Start();

                encryptProgressBar = EncryptProgressBar();
                StartCoroutine(encryptProgressBar);
            }
        }

        public void SendData()
        {
            ioServer.iSeed = encrypt.iSeed;

//TEST FOR STRIPPED & PADDED FILE
            //textOutput.text = encrypt.eStringData;

            if (!isString && !hasData && !endOfStream)
            {
                if (isTexturing)
                {
                    ioServer.ReceiveData(encrypt.eV3Data);
                    texture = new Texture(imagePixelCode, ioServer.dataStream);
                    sentData = true;

                    Texture2D newTexture = texture.CreateTexture();

                    imagePixelCode.material.SetTexture("_BaseMap", newTexture);
                    imagePixelCode.color = Color.white;
                }
                else
                {
                    ioServer.ReceiveData(encrypt.eV3Data);
                    sentData = true;
                }
            }

            if (isString && !hasData && !endOfStream)
            {
                ioServer.ReceiveSData(encrypt.eStringData);
                
                sentData = true;
                //textOutput.text = encrypt.eStringData;
            }
        }

        public void DecryptData()
        {
            decrypt.iSeed = ioServer.iSeed;

            if (!isString)
            {
                decrypt.isString = false;
                decrypt.ReceiveData(ioServer.dataStream);
            }

            if (isString)
            {
                decrypt.isString = true;
                decrypt.ReceiveSData(ioServer.sDataStream);
            }

            // Start a new thread to run decryption on
            Thread t2 = new Thread(decrypt.DecryptStart);
            t2.Start();

            decryptProgressBar = DecryptProgressBar();
            StartCoroutine(decryptProgressBar);
        }

        public void Validate()
        {
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

        void UpdateBools()
        {
            ticker = ioServer.ticker;
            isEncrypting = encrypt.isEncrypting;
            isEncrypted = encrypt.isEncrypted;
            bCasting = ioServer.bCasting;
            hasData = ioServer.hasData;
            endOfStream = ioServer.endOfStream;
            currentColor = ioServer.colorOut;
            isDecrypting = decrypt.isDecrypting;
            isDecrypted = decrypt.isDecrypted;
        }

#region Update
        // Update is called once per frame
        void Update()
        {
            UpdateBools();

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

                UpdateDecryptSlider(100f); //<--- for when it decrypts too fast to update the progress bar.
                StopCoroutine(decryptProgressBar);
                textOutput.text = decrypt.outputText;
            }

            if (isEncrypted)
            {
                btn_encrypt.interactable = false;

                UpdateEncryptSlider(100f); //<--- for when it encrypts too fast to update the progress bar.
                StopCoroutine(encryptProgressBar);
            }

            if (isRendering && sentData && !endOfStream)
            {
                ioServer.Update();
            }
            
            if (!isRendering && sentData && !endOfStream && hasData)
            {
                btn_send.interactable = false;
                btn_decrypt.interactable = true;

                ioServer.endOfStream = true;
            }

            if (isEncrypted && !hasData)
            {
                btn_encrypt.interactable = false;
                btn_send.interactable = true;

                if (!runOnce)
                {
                    runOnce = true;
                    UpdateFileDataUI();
                }
            }

        }
        #endregion

        #region ProgressBar


        IEnumerator EncryptProgressBar()
        {
            if (isEncrypted)
                UpdateEncryptSlider(100f);
            else
            {
                float slideValue = 0.0f;
                float endValue = encrypt.dataLengthTotal;

                while (encryptValue <= endValue)
                {
                    encryptValue = encrypt.dataPos1 + encrypt.dataPos2;

                    if (encryptValue != 0)
                    {
                        slideValue = encryptValue / endValue * 100;
                    }

                    yield return new WaitForEndOfFrame();

                    UpdateEncryptSlider(slideValue);
                }
            }
        }

        void UpdateEncryptSlider(float slideValue)
        {
            slider_EncryptPercentage.value = slideValue;
            encryptPercentage_txt.text = $"{Mathf.Round(slider_EncryptPercentage.value)}%";
            if (Mathf.Round(slider_EncryptPercentage.value) == 100)
            {
                sliderEncryptFillImage.color = Color.green;
            }
        }

        IEnumerator DecryptProgressBar()
        {
            if (isDecrypted)
                UpdateDecryptSlider(100f);
            else
            {
                float slideValue = 0.0f;
                float endValue = decrypt.dataLengthTotal;

                while (decryptValue <= endValue)
                {
                    decryptValue = decrypt.dataPos1 + decrypt.dataPos2;

                    if (decryptValue != 0)
                    {
                        slideValue = decryptValue / endValue * 100;
                    }

                    yield return new WaitForEndOfFrame();

                    UpdateDecryptSlider(slideValue);
                }
            }
        }

        void UpdateDecryptSlider(float slideValue)
        {
            slider_DecryptPercentage.value = slideValue;
            decryptPercentage_txt.text = $"{Mathf.Round(slider_DecryptPercentage.value)}%";
            if(Mathf.Round(slider_DecryptPercentage.value) == 100)
            {
                sliderDecryptFillImage.color = Color.green;
            }
        }
#endregion
    }

}
