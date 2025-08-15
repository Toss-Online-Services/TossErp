namespace TossErp.Stock.Application.Common.Interfaces;

/// <summary>
/// Service for barcode scanning and validation
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// Validates a barcode format
    /// </summary>
    /// <param name="barcode">The barcode to validate</param>
    /// <returns>True if the barcode format is valid</returns>
    Task<bool> ValidateBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans a barcode and returns product information
    /// </summary>
    /// <param name="barcode">The barcode to scan</param>
    /// <returns>Product information from the barcode</returns>
    Task<BarcodeScanResult?> ScanBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a barcode for a product
    /// </summary>
    /// <param name="productCode">The product code to encode</param>
    /// <param name="barcodeType">The type of barcode to generate</param>
    /// <returns>The generated barcode</returns>
    Task<string> GenerateBarcodeAsync(string productCode, BarcodeType barcodeType = BarcodeType.Code128, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the barcode type from a barcode string
    /// </summary>
    /// <param name="barcode">The barcode to analyze</param>
    /// <returns>The detected barcode type</returns>
    Task<BarcodeType> GetBarcodeTypeAsync(string barcode, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a barcode scan
/// </summary>
public record BarcodeScanResult
{
    /// <summary>
    /// The scanned barcode
    /// </summary>
    public string Barcode { get; init; } = string.Empty;

    /// <summary>
    /// The product code extracted from the barcode
    /// </summary>
    public string? ProductCode { get; init; }

    /// <summary>
    /// The product name (if available from external service)
    /// </summary>
    public string? ProductName { get; init; }

    /// <summary>
    /// The barcode type
    /// </summary>
    public BarcodeType BarcodeType { get; init; }

    /// <summary>
    /// Whether the barcode is valid
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Additional metadata from the scan
    /// </summary>
    public Dictionary<string, string> Metadata { get; init; } = new();
}

/// <summary>
/// Types of barcodes supported
/// </summary>
public enum BarcodeType
{
    /// <summary>
    /// Code 128 barcode
    /// </summary>
    Code128,

    /// <summary>
    /// Code 39 barcode
    /// </summary>
    Code39,

    /// <summary>
    /// EAN-13 barcode
    /// </summary>
    EAN13,

    /// <summary>
    /// EAN-8 barcode
    /// </summary>
    EAN8,

    /// <summary>
    /// UPC-A barcode
    /// </summary>
    UPCA,

    /// <summary>
    /// UPC-E barcode
    /// </summary>
    UPCE,

    /// <summary>
    /// QR Code
    /// </summary>
    QRCode,

    /// <summary>
    /// Data Matrix
    /// </summary>
    DataMatrix,

    /// <summary>
    /// PDF417
    /// </summary>
    PDF417,

    /// <summary>
    /// Unknown barcode type
    /// </summary>
    Unknown
}
