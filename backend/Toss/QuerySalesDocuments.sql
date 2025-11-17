-- Query to check SalesDocuments for ShopId = 1
-- This mimics the backend query logic: d.ShopId == 1 OR d.Sale.ShopId == 1

-- First, let's see the breakdown by ShopId in SalesDocuments
SELECT 
    "ShopId",
    COUNT(*) as DocumentCount,
    COUNT(*) FILTER (WHERE "DocumentType" = 1) as InvoiceCount,
    COUNT(*) FILTER (WHERE "DocumentType" = 2) as ReceiptCount
FROM "SalesDocuments"
GROUP BY "ShopId"
ORDER BY "ShopId";

-- Now, let's see documents that match the filter (ShopId = 1 OR Sale.ShopId = 1)
SELECT 
    sd."Id",
    sd."DocumentNumber",
    sd."DocumentType",
    sd."ShopId" as DocumentShopId,
    s."ShopId" as SaleShopId,
    sd."CustomerId",
    sd."SaleId",
    sd."DocumentDate",
    sd."TotalAmount"
FROM "SalesDocuments" sd
LEFT JOIN "Sales" s ON sd."SaleId" = s."Id"
WHERE sd."ShopId" = 1 OR s."ShopId" = 1
ORDER BY sd."DocumentDate" DESC
LIMIT 20;

-- Count of documents matching the filter
SELECT 
    COUNT(*) as TotalDocuments,
    COUNT(*) FILTER (WHERE sd."ShopId" = 1) as DocumentsWithShopId1,
    COUNT(*) FILTER (WHERE sd."ShopId" != 1 AND s."ShopId" = 1) as DocumentsWithSaleShopId1,
    COUNT(*) FILTER (WHERE sd."DocumentType" = 1) as InvoiceCount,
    COUNT(*) FILTER (WHERE sd."DocumentType" = 2) as ReceiptCount
FROM "SalesDocuments" sd
LEFT JOIN "Sales" s ON sd."SaleId" = s."Id"
WHERE sd."ShopId" = 1 OR s."ShopId" = 1;






