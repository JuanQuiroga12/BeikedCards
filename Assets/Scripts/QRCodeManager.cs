using System.Collections.Generic;
using UnityEngine;

public static class QRCodeManager
{
    private static HashSet<string> _usedQRCodes = null;

    // Inicializar la lista de códigos
    public static void Initialize()
    {
        _usedQRCodes = new HashSet<string>();
        LoadQRCodes();
    }

    // Verificar si un código ya fue usado
    public static bool IsQRCodeUsed(string qrCode)
    {
        if (_usedQRCodes == null)
            Initialize();

        return _usedQRCodes.Contains(qrCode);
    }

    // Añadir un código usado
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

    // Cargar códigos desde PlayerPrefs
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
        Debug.Log($"QRCodeManager: Cargados {_usedQRCodes.Count} códigos QR usados");
    }

    // Guardar códigos a PlayerPrefs
    private static void SaveQRCodes()
    {
        if (_usedQRCodes == null || _usedQRCodes.Count == 0)
            return;

        string savedCodes = string.Join("|", _usedQRCodes);
        PlayerPrefs.SetString("UsedQRCodes", savedCodes);
        PlayerPrefs.Save();
    }

    // Limpiar códigos (para pruebas)
    public static void ClearQRCodes()
    {
        _usedQRCodes?.Clear();
        PlayerPrefs.DeleteKey("UsedQRCodes");
        PlayerPrefs.Save();
    }
}