using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ConvertController : MonoBehaviour {
    public OperatePanel m_Input;
    public OperatePanel m_Output;
    public Button btn_Convert;

    private byte[] srcByteArr;
    private InputBaseMode inputBaseMode;    //  current input base mode
    private OutputBaseMode outputBaseMode;  //  current output base mode

    private void Awake() {
        m_Input.btn_Clear.onClick.AddListener(OnClearInputArea);    //  add listener to btn: input clear
        m_Input.baseMode.onValueChanged.AddListener(OnInputBaseModeConfirm);    //  add listener to input dropdown
        
        m_Output.btn_Clear.onClick.AddListener(OnClearOutputArea);  //  add listener to btn: output clear
        m_Output.baseMode.onValueChanged.AddListener(OnOutputBaseModeConfirm);  //  add listener to output dropdown

        btn_Convert.onClick.AddListener(OnConvert); //  add listener to btn: convert
    }

    private void Start() {
        //  set input base mode to HEX on start
        m_Input.baseMode.value = 2;
        inputBaseMode = InputBaseMode.HEX;

        //  set output base mode to DEC_BYTE on start
        m_Output.baseMode.value = 1;
        outputBaseMode = OutputBaseMode.DEC;
    }

    private void OnDisable() {
        m_Input.btn_Clear.onClick.RemoveAllListeners();
        m_Input.baseMode.onValueChanged.RemoveAllListeners();

        m_Output.btn_Clear.onClick.RemoveAllListeners();
        m_Output.baseMode.onValueChanged.RemoveAllListeners();

        btn_Convert.onClick.RemoveAllListeners();
    }

    private void ConvertSingleNum(string str) {
        int len = CheckStorageSpace(str);
        if (len < 1) {
            AddToOutput("<color #ff0000>ERROR: input str is illegal!</color>");
            return;
        }

        byte[] byteArr = new byte[len];

        if (inputBaseMode == InputBaseMode.BIN) {
            if (ConvertHelper.IsByteRange(str)) {
                byteArr[0] = BinStr2Byte(str);
            } else if (ConvertHelper.IsInt16Range(str)) {
                byteArr = BitConverter.GetBytes(BinStr2Int16(str));
                if (BitConverter.IsLittleEndian) Array.Reverse(byteArr);
            } else if (ConvertHelper.IsInt32Range(str)) {
                byteArr = BitConverter.GetBytes(BinStr2Int32(str));
                if (BitConverter.IsLittleEndian) Array.Reverse(byteArr);
            }
        } else if (inputBaseMode == InputBaseMode.DEC) {
            byte res_Byte = 0;
            short res_Short = 0;
            int res_Int = 0;

            if (ConvertHelper.IsByteRange(str, out res_Byte)) {
                byteArr[0] = res_Byte;
            } else if (ConvertHelper.IsInt16Range(str, out res_Short)) {
                var byArr_Int16 = BitConverter.GetBytes(res_Short);
                if (BitConverter.IsLittleEndian) Array.Reverse(byArr_Int16);

                Buffer.BlockCopy(byArr_Int16, 0, byteArr, 0, byArr_Int16.Length);
            } else if (ConvertHelper.IsInt32Range(str, out res_Int)) {
                var byArr_Int32 = BitConverter.GetBytes(res_Int);
                if (BitConverter.IsLittleEndian) Array.Reverse(byArr_Int32);

                Buffer.BlockCopy(byArr_Int32, 0, byteArr, 0, byArr_Int32.Length);
            }
        } else if (inputBaseMode == InputBaseMode.HEX) {
            if (ConvertHelper.IsByteRange(str)) {
                byteArr[0] = HexStr2Byte(str);
            } else if (ConvertHelper.IsInt16Range(str)) {
                byteArr = BitConverter.GetBytes(HexStr2Int16(str));
                if (BitConverter.IsLittleEndian) Array.Reverse(byteArr);
            } else if (ConvertHelper.IsInt32Range(str)) {
                byteArr = BitConverter.GetBytes(HexStr2Int32(str));
                if (BitConverter.IsLittleEndian) Array.Reverse(byteArr);
            }
        }

        if (outputBaseMode == OutputBaseMode.BIN) {
            AddToOutput(ConvertHelper.ByteArrToBin(byteArr));
        } else if (outputBaseMode == OutputBaseMode.DEC) {
            if (ConvertHelper.IsByteRange(str))
                AddToOutput(ConvertHelper.ByteArrToByte(byteArr));
            else if(ConvertHelper.IsInt16Range(str))
                AddToOutput(ConvertHelper.ByteArrToInt16(byteArr));
            else if (ConvertHelper.IsInt32Range(str))
                AddToOutput(ConvertHelper.ByteArrToInt32(byteArr));
        } else if (outputBaseMode == OutputBaseMode.HEX) {
            AddToOutput(ConvertHelper.ByteArrToHex(byteArr));
        }
    }

    #region callbacks
    private void OnConvert() {
        var inputStrArr = m_Input.txtArea.text.TrimEnd().Split(' ');
        if (inputStrArr.Length < 1) {
            return;
        }

        for (int i = 0; i < inputStrArr.Length; i++) {
            ConvertSingleNum(inputStrArr[i]);
        }
    }
    
    private void OnInputBaseModeConfirm(int inputBaseValue) {
        switch (inputBaseValue) {
            case 0: inputBaseMode = InputBaseMode.BIN; break;
            case 1: inputBaseMode = InputBaseMode.DEC; break;
            case 2: inputBaseMode = InputBaseMode.HEX; break;
            default: inputBaseMode = InputBaseMode.DEC; break;
        }
        Debug.Log("inputBaseMode:" + inputBaseMode);
    }

    private void OnOutputBaseModeConfirm(int outputBaseValue) {
        switch (outputBaseValue) {
            case 0: outputBaseMode = OutputBaseMode.BIN; break;
            case 1: outputBaseMode = OutputBaseMode.DEC; break;
            case 2: outputBaseMode = OutputBaseMode.HEX; break;
            default: outputBaseMode = OutputBaseMode.DEC; break;
        }
        Debug.Log("outputBaseMode: " + outputBaseMode);
    }

    private void OnClearInputArea() {
        m_Input.txtArea.text = "";
    }

    private void OnClearOutputArea() {
        m_Output.txtArea.text = "";
    }
    #endregion

    //  the needed storage space after converting a single input number
    private int CheckStorageSpace(string inputStrElem) {
        int len = 0;

        if (ConvertHelper.IsByteRange(inputStrElem)) len += 1;
        else if (ConvertHelper.IsInt16Range(inputStrElem)) len += 2;
        else if (ConvertHelper.IsInt32Range(inputStrElem)) len += 4;
        else len += 0;

        return len;
    }

    private void AddToOutput(string str) {
        m_Output.txtArea.text += str + "\n";
    }
    
    #region bin str converter
    private byte BinStr2Byte(string binStr) {
        if (BinStrExceptionCheck(binStr, 8))
            return Convert.ToByte(binStr, 2);
        else
            return 0;
    }

    private short BinStr2Int16(string binStr) {
        if (BinStrExceptionCheck(binStr, 16))
            return Convert.ToInt16(binStr, 2);
        else
            return 0;
    }

    private int BinStr2Int32(string binStr) {
        if (BinStrExceptionCheck(binStr, 32))
            return Convert.ToInt32(binStr, 2);
        else
            return 0;
    }

    private bool BinStrExceptionCheck(string binStr, int checkLen) {
        if (!ConvertHelper.IsBin(binStr)) {
            Debug.LogError("ERROR: input str is not in binary!");
            return false;
        }
        if (binStr.Length > checkLen) {
            AddToOutput("<color #ff0000>ERROR: input len is too large</color>");
            return false;
        }
        return true;
    }
    #endregion

    #region hex str converter
    private byte HexStr2Byte(string hexStr) {
        if (HexStrExceptionCheck(hexStr, 8))
            return Convert.ToByte(hexStr, 16);
        else
            return 0;
    }

    private short HexStr2Int16(string hexStr) {
        if (HexStrExceptionCheck(hexStr, 16))
            return Convert.ToInt16(hexStr, 16);
        else
            return 0;
    }

    private int HexStr2Int32(string hexStr) {
        if (HexStrExceptionCheck(hexStr, 32))
            return Convert.ToInt32(hexStr, 16);
        else
            return 0;
    }

    private bool HexStrExceptionCheck(string hexStr, int checkLen) {
        if (!ConvertHelper.IsHex(hexStr)) {
            Debug.LogError("ERROR: input str is not in binary!");
            return false;
        }
        if (hexStr.Length > checkLen) {
            AddToOutput("<color #ff0000>ERROR: input len is too large</color>");
            return false;
        }
        return true;
    }
    #endregion
}

[Serializable]
public class OperatePanel {
    public TMP_InputField txtArea;
    public TMP_Dropdown baseMode;
    public Button btn_Clear;
}

public enum InputBaseMode { BIN, DEC, HEX }
public enum OutputBaseMode { BIN, DEC, HEX }
