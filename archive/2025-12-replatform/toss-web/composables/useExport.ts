import * as XLSX from 'xlsx'
import jsPDF from 'jspdf'
import 'jspdf-autotable'
import html2canvas from 'html2canvas'
import { saveAs } from 'file-saver'

export interface ExportColumn {
  key: string
  label: string
  width?: number
  format?: (value: any) => string
}

export interface ExportOptions {
  filename?: string
  title?: string
  subtitle?: string
  columns?: ExportColumn[]
  includeTimestamp?: boolean
  orientation?: 'portrait' | 'landscape'
}

export const useExport = () => {
  // Generate filename with timestamp
  const generateFilename = (baseName: string, extension: string, includeTimestamp: boolean = true): string => {
    const timestamp = includeTimestamp ? `_${new Date().toISOString().slice(0, 19).replace(/[:.]/g, '-')}` : ''
    return `${baseName}${timestamp}.${extension}`
  }

  // Format data for export
  const formatDataForExport = (data: any[], columns?: ExportColumn[]): any[] => {
    if (!columns) return data

    return data.map(item => {
      const formatted: any = {}
      columns.forEach(col => {
        const value = item[col.key]
        formatted[col.label] = col.format ? col.format(value) : value
      })
      return formatted
    })
  }

  // Export to CSV
  const exportToCSV = (data: any[], options: ExportOptions = {}) => {
    try {
      const {
        filename = 'export',
        columns,
        includeTimestamp = true
      } = options

      const formattedData = formatDataForExport(data, columns)
      const ws = XLSX.utils.json_to_sheet(formattedData)
      const wb = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(wb, ws, 'Data')
      
      const csvOutput = XLSX.write(wb, { bookType: 'csv', type: 'string' })
      const blob = new Blob([csvOutput], { type: 'text/csv;charset=utf-8;' })
      
      saveAs(blob, generateFilename(filename, 'csv', includeTimestamp))
    } catch (error) {
      console.error('Error exporting to CSV:', error)
      throw new Error('Failed to export CSV file')
    }
  }

  // Export to Excel
  const exportToExcel = (data: any[], options: ExportOptions = {}) => {
    try {
      const {
        filename = 'export',
        title,
        columns,
        includeTimestamp = true
      } = options

      const formattedData = formatDataForExport(data, columns)
      const ws = XLSX.utils.json_to_sheet(formattedData)

      // Set column widths
      if (columns) {
        const colWidths = columns.map(col => ({ wch: col.width || 15 }))
        ws['!cols'] = colWidths
      }

      // Add title if provided
      if (title) {
        XLSX.utils.sheet_add_aoa(ws, [[title]], { origin: 'A1' })
        XLSX.utils.sheet_add_aoa(ws, [['']], { origin: 'A2' }) // Empty row
        const range = XLSX.utils.decode_range(ws['!ref'] || 'A1')
        range.s.r = 2 // Start data from row 3
        ws['!ref'] = XLSX.utils.encode_range(range)
      }

      const wb = XLSX.utils.book_new()
      XLSX.utils.book_append_sheet(wb, ws, 'Data')
      
      XLSX.writeFile(wb, generateFilename(filename, 'xlsx', includeTimestamp))
    } catch (error) {
      console.error('Error exporting to Excel:', error)
      throw new Error('Failed to export Excel file')
    }
  }

  // Export to PDF
  const exportToPDF = async (data: any[], options: ExportOptions = {}) => {
    try {
      const {
        filename = 'export',
        title,
        subtitle,
        columns,
        includeTimestamp = true,
        orientation = 'portrait'
      } = options

      const doc = new jsPDF({
        orientation,
        unit: 'mm',
        format: 'a4'
      })

      let yPosition = 20

      // Add title
      if (title) {
        doc.setFontSize(18)
        doc.setFont('helvetica', 'bold')
        doc.text(title, 20, yPosition)
        yPosition += 10
      }

      // Add subtitle
      if (subtitle) {
        doc.setFontSize(12)
        doc.setFont('helvetica', 'normal')
        doc.text(subtitle, 20, yPosition)
        yPosition += 10
      }

      // Add timestamp
      if (includeTimestamp) {
        doc.setFontSize(10)
        doc.setFont('helvetica', 'normal')
        doc.text(`Generated: ${new Date().toLocaleString()}`, 20, yPosition)
        yPosition += 15
      }

      // Prepare table data
      const formattedData = formatDataForExport(data, columns)
      
      if (formattedData.length > 0) {
        const headers = columns ? columns.map(col => col.label) : Object.keys(formattedData[0])
        const rows = formattedData.map(item => 
          columns ? columns.map(col => item[col.label] || '') : Object.values(item)
        )

        // Add table using jspdf-autotable
        ;(doc as any).autoTable({
          head: [headers],
          body: rows,
          startY: yPosition,
          styles: {
            fontSize: 8,
            cellPadding: 2,
          },
          headStyles: {
            fillColor: [59, 130, 246], // Blue header
            textColor: 255,
            fontStyle: 'bold'
          },
          alternateRowStyles: {
            fillColor: [248, 250, 252] // Light gray for alternate rows
          },
          margin: { top: 20, right: 20, bottom: 20, left: 20 },
        })
      }

      doc.save(generateFilename(filename, 'pdf', includeTimestamp))
    } catch (error) {
      console.error('Error exporting to PDF:', error)
      throw new Error('Failed to export PDF file')
    }
  }

  // Export chart as image
  const exportChartAsImage = async (chartElementId: string, filename: string = 'chart') => {
    try {
      const chartElement = document.getElementById(chartElementId)
      if (!chartElement) {
        throw new Error('Chart element not found')
      }

      const canvas = await html2canvas(chartElement, {
        backgroundColor: '#ffffff',
        scale: 2, // Higher quality
        logging: false
      })

      canvas.toBlob((blob) => {
        if (blob) {
          saveAs(blob, generateFilename(filename, 'png'))
        }
      })
    } catch (error) {
      console.error('Error exporting chart:', error)
      throw new Error('Failed to export chart image')
    }
  }

  // Export dashboard as PDF
  const exportDashboardToPDF = async (elementId: string, options: ExportOptions = {}) => {
    try {
      const {
        filename = 'dashboard',
        title = 'Dashboard Report',
        includeTimestamp = true,
        orientation = 'landscape'
      } = options

      const element = document.getElementById(elementId)
      if (!element) {
        throw new Error('Dashboard element not found')
      }

      const canvas = await html2canvas(element, {
        backgroundColor: '#ffffff',
        scale: 1,
        logging: false,
        useCORS: true
      })

      const imgData = canvas.toDataURL('image/png')
      const doc = new jsPDF({
        orientation,
        unit: 'mm',
        format: 'a4'
      })

      const pageWidth = doc.internal.pageSize.getWidth()
      const pageHeight = doc.internal.pageSize.getHeight()
      const imgWidth = pageWidth - 20 // 10mm margin on each side
      const imgHeight = (canvas.height * imgWidth) / canvas.width

      let yPosition = 20

      // Add title
      if (title) {
        doc.setFontSize(16)
        doc.setFont('helvetica', 'bold')
        doc.text(title, 20, yPosition)
        yPosition += 10
      }

      // Add timestamp
      if (includeTimestamp) {
        doc.setFontSize(10)
        doc.setFont('helvetica', 'normal')
        doc.text(`Generated: ${new Date().toLocaleString()}`, 20, yPosition)
        yPosition += 10
      }

      // Add image
      if (imgHeight > pageHeight - yPosition - 20) {
        // Image too tall, scale it down
        const scaledHeight = pageHeight - yPosition - 20
        const scaledWidth = (canvas.width * scaledHeight) / canvas.height
        doc.addImage(imgData, 'PNG', 20, yPosition, scaledWidth, scaledHeight)
      } else {
        doc.addImage(imgData, 'PNG', 20, yPosition, imgWidth, imgHeight)
      }

      doc.save(generateFilename(filename, 'pdf', includeTimestamp))
    } catch (error) {
      console.error('Error exporting dashboard:', error)
      throw new Error('Failed to export dashboard PDF')
    }
  }

  // Predefined export configurations for common data types
  const getExportConfig = (type: string): { columns: ExportColumn[] } => {
    const configs: Record<string, { columns: ExportColumn[] }> = {
      sales: {
        columns: [
          { key: 'orderNumber', label: 'Order #', width: 15 },
          { key: 'customerName', label: 'Customer', width: 20 },
          { key: 'date', label: 'Date', width: 12, format: (date) => new Date(date).toLocaleDateString() },
          { key: 'amount', label: 'Amount', width: 12, format: (amount) => `R${(amount / 100).toFixed(2)}` },
          { key: 'status', label: 'Status', width: 12 }
        ]
      },
      inventory: {
        columns: [
          { key: 'sku', label: 'SKU', width: 15 },
          { key: 'productName', label: 'Product', width: 25 },
          { key: 'category', label: 'Category', width: 15 },
          { key: 'currentStock', label: 'Stock', width: 10 },
          { key: 'unitPrice', label: 'Unit Price', width: 12, format: (price) => `R${(price / 100).toFixed(2)}` },
          { key: 'totalValue', label: 'Total Value', width: 15, format: (value) => `R${(value / 100).toFixed(2)}` }
        ]
      },
      customers: {
        columns: [
          { key: 'customerNumber', label: 'Customer #', width: 15 },
          { key: 'name', label: 'Name', width: 20 },
          { key: 'email', label: 'Email', width: 25 },
          { key: 'phone', label: 'Phone', width: 15 },
          { key: 'totalOrders', label: 'Orders', width: 10 },
          { key: 'totalSpent', label: 'Total Spent', width: 15, format: (amount) => `R${(amount / 100).toFixed(2)}` }
        ]
      },
      workOrders: {
        columns: [
          { key: 'workOrderNumber', label: 'WO #', width: 12 },
          { key: 'productName', label: 'Product', width: 20 },
          { key: 'quantityOrdered', label: 'Qty Ordered', width: 12 },
          { key: 'quantityProduced', label: 'Qty Produced', width: 12 },
          { key: 'status', label: 'Status', width: 12 },
          { key: 'plannedStartDate', label: 'Start Date', width: 12, format: (date) => new Date(date).toLocaleDateString() }
        ]
      },
      boms: {
        columns: [
          { key: 'bomNumber', label: 'BOM #', width: 12 },
          { key: 'productName', label: 'Product', width: 20 },
          { key: 'version', label: 'Version', width: 8 },
          { key: 'componentCount', label: 'Components', width: 12 },
          { key: 'totalCost', label: 'Total Cost', width: 12, format: (cost) => `R${(cost / 100).toFixed(2)}` },
          { key: 'status', label: 'Status', width: 10 }
        ]
      },
      financial: {
        columns: [
          { key: 'account', label: 'Account', width: 20 },
          { key: 'description', label: 'Description', width: 25 },
          { key: 'debit', label: 'Debit', width: 15, format: (amount) => amount ? `R${(amount / 100).toFixed(2)}` : '' },
          { key: 'credit', label: 'Credit', width: 15, format: (amount) => amount ? `R${(amount / 100).toFixed(2)}` : '' },
          { key: 'balance', label: 'Balance', width: 15, format: (amount) => `R${(amount / 100).toFixed(2)}` }
        ]
      }
    }

    return configs[type] || { columns: [] }
  }

  return {
    exportToCSV,
    exportToExcel,
    exportToPDF,
    exportChartAsImage,
    exportDashboardToPDF,
    getExportConfig,
    generateFilename,
    formatDataForExport
  }
}
