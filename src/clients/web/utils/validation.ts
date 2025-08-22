// Validation utility for form validation
import { reactive, readonly, computed } from 'vue'

export interface ValidationRule {
  required?: boolean
  minLength?: number
  maxLength?: number
  pattern?: RegExp
  custom?: (value: any) => string | null
  message?: string
}

export interface ValidationResult {
  isValid: boolean
  errors: Record<string, string[]>
}

export interface FieldValidation {
  [fieldName: string]: ValidationRule[]
}

// Common validation rules
export const commonValidations = {
  required: (message = 'This field is required'): ValidationRule => ({
    required: true,
    message
  }),
  
  minLength: (length: number, message?: string): ValidationRule => ({
    minLength: length,
    message: message || `Minimum length is ${length} characters`
  }),
  
  maxLength: (length: number, message?: string): ValidationRule => ({
    maxLength: length,
    message: message || `Maximum length is ${length} characters`
  }),
  
  pattern: (regex: RegExp, message: string): ValidationRule => ({
    pattern: regex,
    message
  }),
  
  email: (message = 'Please enter a valid email address'): ValidationRule => ({
    pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    message
  }),
  
  url: (message = 'Please enter a valid URL'): ValidationRule => ({
    pattern: /^https?:\/\/.+/,
    message
  }),
  
  numeric: (message = 'Please enter a valid number'): ValidationRule => ({
    pattern: /^\d+(\.\d+)?$/,
    message
  }),
  
  integer: (message = 'Please enter a valid integer'): ValidationRule => ({
    pattern: /^\d+$/,
    message
  }),
  
  positive: (message = 'Please enter a positive number'): ValidationRule => ({
    custom: (value) => {
      const num = parseFloat(value)
      return isNaN(num) || num <= 0 ? message : null
    }
  }),
  
  minValue: (min: number, message?: string): ValidationRule => ({
    custom: (value) => {
      const num = parseFloat(value)
      return isNaN(num) || num < min ? (message || `Value must be at least ${min}`) : null
    }
  }),
  
  maxValue: (max: number, message?: string): ValidationRule => ({
    custom: (value) => {
      const num = parseFloat(value)
      return isNaN(num) || num > max ? (message || `Value must be at most ${max}`) : null
    }
  }),

  custom: (validator: (value: any) => string | null): ValidationRule => ({
    custom: validator
  })
}

// Stock item specific validations
export const stockItemValidations: FieldValidation = {
  name: [
    commonValidations.required('Item name is required'),
    commonValidations.minLength(2, 'Item name must be at least 2 characters'),
    commonValidations.maxLength(100, 'Item name cannot exceed 100 characters')
  ],
  
  sku: [
    commonValidations.required('SKU is required'),
    commonValidations.pattern(/^[A-Z0-9-]+$/, 'SKU must contain only uppercase letters, numbers, and hyphens'),
    commonValidations.minLength(3, 'SKU must be at least 3 characters'),
    commonValidations.maxLength(20, 'SKU cannot exceed 20 characters')
  ],
  
  category: [
    commonValidations.required('Category is required'),
    commonValidations.minLength(2, 'Category must be at least 2 characters'),
    commonValidations.maxLength(50, 'Category cannot exceed 50 characters')
  ],
  
  warehouse: [
    commonValidations.required('Warehouse is required'),
    commonValidations.minLength(2, 'Warehouse must be at least 2 characters'),
    commonValidations.maxLength(50, 'Warehouse cannot exceed 50 characters')
  ],
  
  quantity: [
    commonValidations.required('Quantity is required'),
    commonValidations.integer('Quantity must be a whole number'),
    commonValidations.minValue(0, 'Quantity cannot be negative')
  ],
  
  unitCost: [
    commonValidations.required('Unit cost is required'),
    commonValidations.numeric('Unit cost must be a valid number'),
    commonValidations.minValue(0, 'Unit cost cannot be negative')
  ],
  
  reorderLevel: [
    commonValidations.required('Reorder level is required'),
    commonValidations.integer('Reorder level must be a whole number'),
    commonValidations.minValue(0, 'Reorder level cannot be negative')
  ],
  
  description: [
    commonValidations.maxLength(500, 'Description cannot exceed 500 characters')
  ]
}

// Stock movement specific validations
export const stockMovementValidations: FieldValidation = {
  itemId: [
    commonValidations.required('Item selection is required'),
    {
      custom: (value: any) => {
        return value > 0 ? null : 'Please select a valid item'
      }
    }
  ],
  
  type: [
    commonValidations.required('Movement type is required'),
    commonValidations.custom((value) => {
      const validTypes = ['in', 'out', 'adjustment', 'transfer']
      return validTypes.includes(value) ? null : 'Please select a valid movement type'
    })
  ],
  
  quantity: [
    commonValidations.required('Quantity is required'),
    commonValidations.integer('Quantity must be a whole number'),
    commonValidations.minValue(1, 'Quantity must be at least 1')
  ],
  
  warehouse: [
    commonValidations.required('Warehouse is required'),
    commonValidations.minLength(2, 'Warehouse must be at least 2 characters'),
    commonValidations.maxLength(50, 'Warehouse cannot exceed 50 characters')
  ],
  
  reference: [
    commonValidations.required('Reference is required'),
    commonValidations.minLength(3, 'Reference must be at least 3 characters'),
    commonValidations.maxLength(50, 'Reference cannot exceed 50 characters')
  ],
  
  reason: [
    commonValidations.required('Reason is required'),
    commonValidations.minLength(10, 'Reason must be at least 10 characters'),
    commonValidations.maxLength(200, 'Reason cannot exceed 200 characters')
  ],
  
  createdBy: [
    commonValidations.required('Created by is required'),
    commonValidations.minLength(2, 'Created by must be at least 2 characters'),
    commonValidations.maxLength(100, 'Created by cannot exceed 100 characters')
  ]
}

// Validation functions
export function validateField(value: any, rules: ValidationRule[]): string[] {
  const errors: string[] = []
  
  for (const rule of rules) {
    // Required validation
    if (rule.required && (value === null || value === undefined || value === '')) {
      errors.push(rule.message || 'This field is required')
      continue
    }
    
    // Skip other validations if value is empty and not required
    if (value === null || value === undefined || value === '') {
      continue
    }
    
    // Min length validation
    if (rule.minLength && typeof value === 'string' && value.length < rule.minLength) {
      errors.push(rule.message || `Minimum length is ${rule.minLength} characters`)
    }
    
    // Max length validation
    if (rule.maxLength && typeof value === 'string' && value.length > rule.maxLength) {
      errors.push(rule.message || `Maximum length is ${rule.maxLength} characters`)
    }
    
    // Pattern validation
    if (rule.pattern && typeof value === 'string' && !rule.pattern.test(value)) {
      errors.push(rule.message || 'Invalid format')
    }
    
    // Custom validation
    if (rule.custom) {
      const customError = rule.custom(value)
      if (customError) {
        errors.push(customError)
      }
    }
  }
  
  return errors
}

export function validateForm(data: Record<string, any>, validations: FieldValidation): ValidationResult {
  const errors: Record<string, string[]> = {}
  let isValid = true
  
  for (const [fieldName, fieldRules] of Object.entries(validations)) {
    const fieldErrors = validateField(data[fieldName], fieldRules)
    if (fieldErrors.length > 0) {
      errors[fieldName] = fieldErrors
      isValid = false
    }
  }
  
  return { isValid, errors }
}

// Helper functions
export function hasError(fieldName: string, errors: Record<string, string[]>): boolean {
  return errors[fieldName] && errors[fieldName].length > 0
}

export function getFirstError(fieldName: string, errors: Record<string, string[]>): string {
  return errors[fieldName]?.[0] || ''
}

export function getErrorClass(fieldName: string, errors: Record<string, string[]>): string {
  return hasError(fieldName, errors) 
    ? 'border-red-500 focus:ring-red-500 focus:border-red-500' 
    : 'border-gray-300 dark:border-gray-600 focus:ring-primary-500 focus:border-primary-500'
}

export function getFieldErrors(fieldName: string, errors: Record<string, string[]>): string[] {
  return errors[fieldName] || []
}

export function clearFieldErrors(fieldName: string, errors: Record<string, string[]>): void {
  delete errors[fieldName]
}

export function clearAllErrors(errors: Record<string, string[]>): void {
  Object.keys(errors).forEach(key => delete errors[key])
}

// Async validation support
export async function validateFieldAsync(
  value: any, 
  rules: ValidationRule[], 
  asyncValidator?: (value: any) => Promise<string | null>
): Promise<string[]> {
  const syncErrors = validateField(value, rules)
  
  if (asyncValidator) {
    try {
      const asyncError = await asyncValidator(value)
      if (asyncError) {
        syncErrors.push(asyncError)
      }
    } catch (error) {
      syncErrors.push('Validation failed. Please try again.')
    }
  }
  
  return syncErrors
}

// Form validation state management
export function createValidationState(validations: FieldValidation) {
  const errors = reactive<Record<string, string[]>>({})
  const touched = reactive<Record<string, boolean>>({})
  const dirty = reactive<Record<string, boolean>>({})
  
  const validateFieldLocal = (fieldName: string, value: any) => {
    const fieldRules = validations[fieldName] || []
    const fieldErrors = validateField(value, fieldRules)
    
    if (fieldErrors.length > 0) {
      errors[fieldName] = fieldErrors
    } else {
      delete errors[fieldName]
    }
    
    return fieldErrors.length === 0
  }
  
  const markFieldTouched = (fieldName: string) => {
    touched[fieldName] = true
  }
  
  const markFieldDirty = (fieldName: string) => {
    dirty[fieldName] = true
  }
  
  const isFieldValid = (fieldName: string) => {
    return !hasError(fieldName, errors)
  }
  
  const isFormValid = computed(() => {
    return Object.keys(errors).length === 0
  })
  
  const resetValidation = () => {
    clearAllErrors(errors)
    Object.keys(touched).forEach(key => touched[key] = false)
    Object.keys(dirty).forEach(key => dirty[key] = false)
  }
  
  return {
    errors: readonly(errors),
    touched: readonly(touched),
    dirty: readonly(dirty),
    validateField: validateFieldLocal,
    markFieldTouched,
    markFieldDirty,
    isFieldValid,
    isFormValid,
    resetValidation
  }
}
