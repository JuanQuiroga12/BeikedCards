using System.Collections.Generic;
using UnityEngine;

public static class QRCodeManager
{
    private static HashSet<string> _usedQRCodes = null;

    // Inicializar la lista de c�digos
    public static void Initialize()
    {
        _usedQRCodes = new HashSet<string>();
        LoadQRCodes();
    }

    // Verificar si un c�digo ya fue usado
    public static bool IsQRCodeUsed(string qrCode)
    {
        if (_usedQRCodes == null)
            Initialize();

        return _usedQRCodes.Contains(qrCode);
    }

    // A�adir un c�digo usado
    public static void AddUsedQRCode(string qrCode)
    {
        if (_usedQRCodes == null)
            Initialize();

        if (!string.IsNullOrEmpty(qrCode))
        {
            _usedQRCodes.Add(qrCode);
            SaveQRCodes();
        }
    }

    // Cargar c�digos desde PlayerPrefs
    private static void LoadQRCodes()
    {
        string savedCodes = PlayerPrefs.GetString("UsedQRCodes", "");
        if (!string.IsNullOrEmpty(savedCodes))
        {
            string[] codes = savedCodes.Split('|');
            foreach (string code in codes)
            {
                if (!string.IsNullOrEmpty(code))
                {
                    _usedQRCodes.Add(code);
                }
            }
        }
        Debug.Log($"QRCodeManager: Cargados {_usedQRCodes.Count} c�digos QR usados");
    }

    // Guardar c�digos a PlayerPrefs
    private static void SaveQRCodes()
    {
        if (_usedQRCodes == null || _usedQRCodes.Count == 0)
            return;

        string savedCodes = string.Join("|", _usedQRCodes);
        PlayerPrefs.SetString("UsedQRCodes", savedCodes);
        PlayerPrefs.Save();
    }

    // Limpiar c�digos (para pruebas)
    public static void ClearQRCodes()
    {
        _usedQRCodes?.Clear();
        PlayerPrefs.DeleteKey("UsedQRCodes");
        PlayerPrefs.Save();
    }
}