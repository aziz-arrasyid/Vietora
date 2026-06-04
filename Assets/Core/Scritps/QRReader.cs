// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using ZXing;

// public class QRReader : MonoBehaviour
// {
//     #region Data AR
//     [Header("Scripts")]
//     [SerializeField] private WorldManager manager;
//     public static event Action<string> OnQRDetected;
//     [SerializeField] private ARCameraManager cameraManager;
//     private IBarcodeReader reader;
//     private bool isScanning = false;

//     #region UI
//     private TextMeshProUGUI textQR;
//     #endregion

//     private void Awake()
//     {
//         reader = new BarcodeReader
//         {
//             AutoRotate = true,
//             Options = new ZXing.Common.DecodingOptions
//             {
//                 TryHarder = true,
//                 PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE }
//             }
//         };
//     }

//     private void Start()
//     {
//         textQR = manager.QRStatus;
//     }

//     private void OnEnable()
//     {
//         if (cameraManager != null) cameraManager.frameReceived += OnFrameReceived;
//     }

//     private void OnDisable()
//     {
//         if (cameraManager != null) cameraManager.frameReceived -= OnFrameReceived;
//     }

//     private void OnFrameReceived(ARCameraFrameEventArgs args)
//     {
//         if (isScanning || Time.frameCount % 15 != 0) return;

//         if (cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
//         {
//             isScanning = true;
//             using (image)
//             {
//                 var conversionParams = new XRCpuImage.ConversionParams
//                 {
//                     inputRect = new RectInt(0, 0, image.width, image.height),
//                     outputDimensions = new Vector2Int(image.width, image.height),
//                     outputFormat = TextureFormat.RGBA32,
//                     transformation = XRCpuImage.Transformation.None
//                 };

//                 int size = image.GetConvertedDataSize(conversionParams);
//                 byte[] buffer = new byte[size];

//                 unsafe
//                 {
//                     fixed (byte* ptr = buffer)
//                     {
//                         image.Convert(conversionParams, (IntPtr)ptr, buffer.Length);
//                     }
//                 }

//                 var result = reader.Decode(
//                     buffer,
//                     conversionParams.outputDimensions.x,
//                     conversionParams.outputDimensions.y,
//                     RGBLuminanceSource.BitmapFormat.RGBA32
//                 );

//                 if (result != null && !string.IsNullOrEmpty(result.Text))
//                 {
//                     textQR.text = $"<color=green>Berhasil Scan QR: {result.Text}</color>";
//                     OnQRDetected?.Invoke(result.Text);
//                 }
//                 else
//                 {
//                     textQR.text = "<color=red>Gagal Scan QR</color>";
//                 }
//             }
//             isScanning = false;
//         }
//     }
//     #endregion

//     #region WebcamTexture
//     // [Header("UI Components")]
//     // [SerializeField] private RawImage cameraPreview;
//     // [SerializeField] private TextMeshProUGUI debugText; // Ganti jadi 'Text' jika menggunakan UI lama

//     // private WebCamTexture webCamTexture;
//     // private IBarcodeReader barcodeReader;
//     // private bool isScanning = false;

//     // void Start()
//     // {
//     //     // 1. Inisialisasi ZXing Reader dengan optimasi jarak jauh
//     //     barcodeReader = new BarcodeReader
//     //     {
//     //         AutoRotate = true,
//     //         Options = new ZXing.Common.DecodingOptions
//     //         {
//     //             TryHarder = true, // Sangat penting untuk mendeteksi QR kecil/jauh
//     //             PossibleFormats = new[] { BarcodeFormat.QR_CODE } // Fokus hanya pada QR Code
//     //         }
//     //     };

//     //     // 2. Setup dan Jalankan Kamera
//     //     StartCoroutine(InitializeCamera());
//     // }

//     // IEnumerator InitializeCamera()
//     // {
//     //     // Pastikan perangkat memiliki kamera
//     //     if (WebCamTexture.devices.Length == 0)
//     //     {
//     //         debugText.text = "<color=red>Kamera tidak ditemukan pada perangkat ini.</color>";
//     //         yield break;
//     //     }

//     //     // Ambil kamera belakang secara default (jika di ponsel)
//     //     string deviceName = WebCamTexture.devices[0].name;
//     //     for (int i = 0; i < WebCamTexture.devices.Length; i++)
//     //     {
//     //         if (!WebCamTexture.devices[i].isFrontFacing)
//     //         {
//     //             deviceName = WebCamTexture.devices[i].name;
//     //             break;
//     //         }
//     //     }

//     //     // Minta resolusi menengah-tinggi (1280x720) agar detail QR jauh tetap tajam
//     //     webCamTexture = new WebCamTexture(deviceName, 1280, 720, 30);
//     //     cameraPreview.texture = webCamTexture;
//     //     webCamTexture.Play();

//     //     // Tunggu sampai kamera benar-benar aktif mendapatkan frame gambar
//     //     while (webCamTexture.width < 100)
//     //     {
//     //         yield return null;
//     //     }

//     //     // Koreksi orientasi gambar jika di ponsel (rotasi WebCamTexture)
//     //     cameraPreview.rectTransform.localRotation = Quaternion.Euler(0, 0, -webCamTexture.videoRotationAngle);

//     //     // 3. Mulai siklus pemindaian berkala (Frame Skipping)
//     //     StartCoroutine(ScanQRRoutine());
//     // }

//     // IEnumerator ScanQRRoutine()
//     // {
//     //     while (true)
//     //     {
//     //         // Melakukan scan setiap 0.15 detik sekali (sekitar 6-7 kali sedetik)
//     //         // Taktik ini menjaga game tidak patah-patah dibanding melakukan scan tiap frame
//     //         yield return new WaitForSeconds(0.15f);

//     //         if (webCamTexture != null && webCamTexture.isPlaying && !isScanning)
//     //         {
//     //             isScanning = true;
//     //             ScanFrame();
//     //             isScanning = false;
//     //         }
//     //     }
//     // }

//     // private void ScanFrame()
//     // {
//     //     try
//     //     {
//     //         // Ambil dimensi asli kamera
//     //         int camWidth = webCamTexture.width;
//     //         int camHeight = webCamTexture.height;

//     //         // TRIK DIGITAL CROP: Ambil area tengah (50% dari total ukuran)
//     //         // Ini membuat QR code yang jauh terlihat lebih besar bagi algoritma ZXing
//     //         int cropWidth = camWidth / 2;
//     //         int cropHeight = camHeight / 2;
//     //         int startX = (camWidth - cropWidth) / 2;
//     //         int startY = (camHeight - cropHeight) / 2;

//     //         // Ambil data warna piksel pada area yang di-crop
//     //         Color32[] cPixels = webCamTexture.GetPixels32();
//     //         Color32[] croppedPixels = new Color32[cropWidth * cropHeight];

//     //         // Proses pemotongan array piksel ke memori baru
//     //         for (int y = 0; y < cropHeight; y++)
//     //         {
//     //             Array.Copy(cPixels, (startY + y) * camWidth + startX, croppedPixels, y * cropWidth, cropWidth);
//     //         }

//     //         // Decode array piksel menggunakan format Color32 bawaan Unity
//     //         Result result = barcodeReader.Decode(croppedPixels, cropWidth, cropHeight);

//     //         if (result != null && !string.IsNullOrEmpty(result.Text))
//     //         {
//     //             // DEBUGGING TEXT: Menampilkan hasil scan jika sukses
//     //             debugText.text = $"<color=green>[Berhasil]\nIsi QR: {result.Text}\nDi-scan pada: {DateTime.Now:HH:mm:ss}</color>";

//     //             // Di sini Anda bisa memicu event game, misal membuka panel, mencocokan ID, dll.
//     //             OnQRScannedSuccessfully(result.Text);
//     //         }
//     //         else
//     //         {
//     //             // DEBUGGING TEXT: Menunjukkan sistem sedang aktif mencari kode
//     //             debugText.text = "<color=yellow>[Mencari QR...] Posisikan kode di tengah layar</color>";
//     //         }
//     //     }
//     //     catch (Exception ex)
//     //     {
//     //         debugText.text = $"<color=red>Error saat scanning: {ex.Message}</color>";
//     //     }
//     // }

//     // private void OnQRScannedSuccessfully(string data)
//     // {
//     //     // Logika custom game Anda ditempatkan di sini
//     //     Debug.Log("Aplikasi mendeteksi data: " + data);
//     // }

//     // void OnDestroy()
//     // {
//     //     if (webCamTexture != null)
//     //     {
//     //         webCamTexture.Stop();
//     //     }
//     // }
//     #endregion
// }
