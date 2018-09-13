using System;

public class ConvertHelper {
    #region base check
    public static bool IsBin(string srcStr) {
        foreach (char c in srcStr) {
            if (c < '0' || c > '1') {
                return false;
            }
        }
        return true;
    }

    public static bool IsDec(string srcStr) {
        foreach (char c in srcStr) {
            if (c < '0' || c > '9') {
                return false;
            }
        }
        return true;
    }

    public static bool IsHex(string srcStr) {
        // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
        return System.Text.RegularExpressions.Regex.IsMatch(srcStr, @"\A\b[0-9a-fA-F]+\b\Z") || 
                System.Text.RegularExpressions.Regex.IsMatch(srcStr, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
    }
    #endregion

    #region Check value range for dec string
    public static bool IsByteRange(string str, out byte res) {
        return byte.TryParse(str, out res);
    }

    public static bool IsByteRange(string str) {
        if (IsBin(str)) {
            var toInt32 = Convert.ToInt32(str, 2);
            return toInt32 >= byte.MinValue && toInt32 <= byte.MaxValue;
        } else if (IsHex(str)) {
            var toInt32 = Convert.ToInt32(str, 16);
            return toInt32 >= byte.MinValue && toInt32 <= byte.MaxValue;
        }

        byte res = 0;
        return byte.TryParse(str, out res);
    }

    public static bool IsInt16Range(string str, out short res) {
        return short.TryParse(str, out res);
    }

    public static bool IsInt16Range(string str) {
        if (IsBin(str)) {
            var toInt32 = Convert.ToInt32(str, 2);
            return toInt32 >= short.MinValue && toInt32 <= short.MaxValue;
        } else if (IsHex(str)) {
            var toInt32 = Convert.ToInt32(str, 16);
            return toInt32 >= short.MinValue && toInt32 <= short.MaxValue;
        }

        short res = 0;
        return short.TryParse(str, out res);
    }

    public static bool IsInt32Range(string str, out int res) {
        return int.TryParse(str, out res);
    }

    public static bool IsInt32Range(string str) {
        if (IsBin(str)) {
            var toInt32 = Convert.ToInt32(str, 2);
            return toInt32 >= int.MinValue && toInt32 <= int.MaxValue;
        } else if (IsHex(str)) {
            var toInt32 = Convert.ToInt32(str, 16);
            return toInt32 >= int.MinValue && toInt32 <= int.MaxValue;
        }

        int res = 0;
        return int.TryParse(str, out res);
    }
    #endregion

    #region byte array converter
    public static string ByteArrToBin(byte[] byteArr) {
        var result = "";
        for (int i = 0; i < byteArr.Length; i++) {
            var sByteStr = Convert.ToString(byteArr[i], 2).PadLeft(8, '0'); //  左边不满 8 位的填充 '0'
            result = i == 0 ? sByteStr : string.Concat(result, " ", sByteStr);
        }
        return result;
    }

    public static string ByteArrToByte(byte[] byteArr) {
        var result = "";
        if (byteArr.Length < 1) return "<color #ff0000>ERROR: input length is less than expected</color>";

        for (int i = 0; i < byteArr.Length; i++) {
            result = (i == 0) ? byteArr[i].ToString() : string.Concat(result, " ", byteArr[i]);
        }
        return result;
    }

    public static string ByteArrToInt16(byte[] byteArr) {
        var result = "";
        if (byteArr.Length < 2) return "<color #ff0000>ERROR: input length is less than expected</color>";

        for (int i = 0; i < byteArr.Length; i += 2) {
            var byArrForInt16 = new byte[2] { byteArr[i], byteArr[i + 1] };
            var toInt16Str = BitConverter.ToInt16(byArrForInt16, 0);
            result = (i == 0) ? toInt16Str.ToString() : string.Concat(result, " ", toInt16Str);
        }
        return result;
    }

    public static string ByteArrToInt32(byte[] byteArr) {
        var result = "";
        if (byteArr.Length < 4) return "<color #ff0000>ERROR: input length is less than expected</color>";

        for (int i = 0; i < byteArr.Length; i += 4) {
            var byArrForInt32 = new byte[4] { byteArr[i], byteArr[i + 1], byteArr[i+2], byteArr[i+3] };
            var toInt32Str = BitConverter.ToInt32(byArrForInt32, 0);
            result = (i == 0) ? toInt32Str.ToString() : string.Concat(result, " ", toInt32Str);
        }
        return result;
    }

    public static string ByteArrToHex(byte[] byteArr) {
        if (byteArr.Length < 1) {
            return null;
        }
        return BitConverter.ToString(byteArr);
    }
    #endregion
}
