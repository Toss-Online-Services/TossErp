/**
 * HR & Payroll TypeScript Definitions
 * Tailored for South African SMME requirements
 */

export interface Employee {
  id: string
  employeeNumber: string
  personalInfo: {
    firstName: string
    lastName: string
    idNumber: string // South African ID number
    dateOfBirth: string
    gender: 'male' | 'female' | 'other'
    nationality: string
    homeLanguage: 'en' | 'zu' | 'xh' | 'af' | 'st' | 'other'
    maritalStatus: 'single' | 'married' | 'divorced' | 'widowed'
    profilePhoto?: string
  }
  contactInfo: {
    email?: string
    phoneNumber: string
    alternativeNumber?: string
    address: {
      street: string
      suburb: string
      city: string
      province: string
      postalCode: string
    }
    emergencyContact: {
      name: string
      relationship: string
      phoneNumber: string
    }
    nextOfKin: {
      name: string
      relationship: string
      phoneNumber: string
      idNumber?: string
    }
  }
  employment: {
    startDate: string
    endDate?: string
    position: string
    department?: string
    employmentType: 'permanent' | 'contract' | 'casual' | 'part-time'
    workSchedule: 'full-time' | 'part-time' | 'shift-work'
    probationEndDate?: string
    contractEndDate?: string
    supervisor?: string
  }
  compensation: {
    salaryStructureId: string
    basicSalary: number
    allowances?: {
      transport?: number
      housing?: number
      meal?: number
      other?: number
    }
    paymentMethod: 'cash' | 'bank-transfer' | 'mobile-money'
    bankDetails?: {
      bankName: string
      branchCode: string
      accountNumber: string
      accountType: 'savings' | 'cheque'
    }
    mobileMoneyDetails?: {
      provider: 'vodacom' | 'mtn' | 'cellc' | 'telkom'
      number: string
    }
  }
  taxInfo: {
    taxNumber?: string
    uifNumber?: string
    medicalAidNumber?: string
    medicalAidScheme?: string
    pensionFundNumber?: string
  }
  status: 'active' | 'inactive' | 'terminated' | 'suspended'
  createdAt: string
  updatedAt: string
  createdBy: string
}

export interface EmployeeCreate {
  employeeNumber?: string
  personalInfo: Omit<Employee['personalInfo'], 'profilePhoto'> & { profilePhoto?: File }
  contactInfo: Employee['contactInfo']
  employment: Omit<Employee['employment'], 'probationEndDate' | 'contractEndDate'> & {
    probationEndDate?: string
    contractEndDate?: string
  }
  compensation: Employee['compensation']
  taxInfo?: Partial<Employee['taxInfo']>
}

export interface EmployeeUpdate {
  personalInfo?: Partial<Employee['personalInfo']>
  contactInfo?: Partial<Employee['contactInfo']>
  employment?: Partial<Employee['employment']>
  compensation?: Partial<Employee['compensation']>
  taxInfo?: Partial<Employee['taxInfo']>
  status?: Employee['status']
}

export interface SalaryStructure {
  id: string
  name: string
  description?: string
  basicSalaryRange: {
    min: number
    max: number
  }
  allowances: {
    transport: number
    housing: number
    meal: number
    other: number
  }
  deductions: {
    uif: boolean
    paye: boolean
    medicalAid?: number
    pensionFund?: number
  }
  payFrequency: 'weekly' | 'bi-weekly' | 'monthly'
  createdAt: string
  updatedAt: string
}

export interface AttendanceRecord {
  id: string
  employeeId: string
  date: string
  clockIn?: string
  clockOut?: string
  totalHours?: number
  status: 'present' | 'absent' | 'late' | 'half-day' | 'overtime'
  location?: {
    lat: number
    lng: number
    address?: string
  }
  notes?: string
  approvedBy?: string
  createdAt: string
  updatedAt: string
}

export interface LeaveRequest {
  id: string
  employeeId: string
  leaveType: 'annual' | 'sick' | 'maternity' | 'paternity' | 'study' | 'compassionate' | 'unpaid'
  startDate: string
  endDate: string
  totalDays: number
  reason: string
  status: 'pending' | 'approved' | 'rejected' | 'cancelled'
  appliedDate: string
  approvedBy?: string
  approvedDate?: string
  rejectionReason?: string
  attachments?: string[]
  createdAt: string
  updatedAt: string
}

export interface LeaveBalance {
  employeeId: string
  leaveType: LeaveRequest['leaveType']
  totalEntitlement: number
  used: number
  pending: number
  remaining: number
  carryOver?: number
  expiryDate?: string
  updatedAt: string
}

export interface TaxCalculation {
  grossSalary: number
  paye: number
  uif: number
  sdl: number
  totalDeductions: number
  netSalary: number
  payPeriod: 'weekly' | 'monthly'
}

export interface PayrollEntry {
  id: string
  employeeId: string
  payPeriod: string // YYYY-MM format
  payDate: string
  earnings: {
    basicSalary: number
    overtime?: number
    allowances: {
      transport: number
      housing: number
      meal: number
      other: number
    }
    bonus?: number
    commission?: number
    totalEarnings: number
  }
  deductions: {
    paye: number
    uif: number
    medicalAid?: number
    pensionFund?: number
    loanDeduction?: number
    other?: number
    totalDeductions: number
  }
  netSalary: number
  paymentMethod: 'cash' | 'bank-transfer' | 'mobile-money'
  paymentStatus: 'pending' | 'paid' | 'failed'
  paymentDate?: string
  paymentReference?: string
  createdAt: string
  processedBy: string
}

export interface PayslipData {
  employee: Employee
  payrollEntry: PayrollEntry
  company: {
    name: string
    address: string
    taxNumber: string
    uifNumber: string
    logo?: string
  }
  taxYear: string
  ytdEarnings: number
  ytdDeductions: number
  ytdNetPay: number
  generatedAt: string
  generatedBy: string
}

export interface WorkShift {
  id: string
  name: string
  startTime: string // HH:mm format
  endTime: string // HH:mm format
  breakDuration: number // minutes
  workDays: ('monday' | 'tuesday' | 'wednesday' | 'thursday' | 'friday' | 'saturday' | 'sunday')[]
  isDefault: boolean
  createdAt: string
  updatedAt: string
}

export interface ShiftAssignment {
  id: string
  employeeId: string
  shiftId: string
  startDate: string
  endDate?: string
  isActive: boolean
  createdAt: string
  assignedBy: string
}

export interface EmployeeDocument {
  id: string
  employeeId: string
  documentType: 'contract' | 'id-copy' | 'cv' | 'qualification' | 'reference' | 'photo' | 'other'
  fileName: string
  fileUrl: string
  fileSize: number
  mimeType: string
  uploadedAt: string
  uploadedBy: string
  expiryDate?: string
  isVerified: boolean
}

export interface PerformanceReview {
  id: string
  employeeId: string
  reviewPeriod: string
  reviewType: 'annual' | 'probation' | 'promotion' | 'disciplinary'
  overallRating: number // 1-5 scale
  goals: {
    description: string
    target: string
    achieved: boolean
    rating: number
  }[]
  strengths: string[]
  improvementAreas: string[]
  developmentPlan?: string
  reviewerComments: string
  employeeComments?: string
  reviewDate: string
  reviewerId: string
  status: 'draft' | 'completed' | 'acknowledged'
  createdAt: string
  updatedAt: string
}

export interface TrainingRecord {
  id: string
  employeeId: string
  trainingName: string
  trainingType: 'internal' | 'external' | 'online' | 'certification'
  provider?: string
  startDate: string
  endDate?: string
  duration: number // hours
  status: 'enrolled' | 'in-progress' | 'completed' | 'failed' | 'cancelled'
  cost?: number
  certificateUrl?: string
  expiryDate?: string
  notes?: string
  createdAt: string
  updatedAt: string
}

export interface DisciplinaryAction {
  id: string
  employeeId: string
  incidentDate: string
  incidentDescription: string
  actionType: 'verbal-warning' | 'written-warning' | 'final-warning' | 'suspension' | 'dismissal'
  reason: string
  investigationNotes?: string
  actionTaken: string
  suspensionPeriod?: {
    startDate: string
    endDate: string
    isPaid: boolean
  }
  followUpRequired: boolean
  followUpDate?: string
  status: 'active' | 'resolved' | 'appealed'
  issuedBy: string
  issuedDate: string
  acknowledgedBy?: string
  acknowledgedDate?: string
  createdAt: string
  updatedAt: string
}

// Form validation schemas
export interface ValidationRule {
  required?: boolean
  minLength?: number
  maxLength?: number
  min?: number
  max?: number
  pattern?: RegExp
}

export interface EmployeeFormValidation {
  personalInfo: {
    firstName: ValidationRule
    lastName: ValidationRule
    idNumber: ValidationRule
    dateOfBirth: ValidationRule
    phoneNumber: ValidationRule
  }
  employment: {
    startDate: ValidationRule
    position: ValidationRule
    employmentType: ValidationRule
  }
  compensation: {
    basicSalary: ValidationRule
    paymentMethod: ValidationRule
  }
}

// API Response types
export interface HRApiResponse<T> {
  success: boolean
  data: T
  message?: string
  errors?: Record<string, string[]>
}

export interface PaginatedResponse<T> {
  data: T[]
  pagination: {
    page: number
    limit: number
    total: number
    pages: number
  }
}

// Filter and search types
export interface EmployeeFilters {
  status?: Employee['status']
  department?: string
  employmentType?: Employee['employment']['employmentType']
  startDateFrom?: string
  startDateTo?: string
  search?: string
}

export interface AttendanceFilters {
  employeeId?: string
  dateFrom: string
  dateTo: string
  status?: AttendanceRecord['status']
}

export interface PayrollFilters {
  payPeriodFrom: string
  payPeriodTo: string
  employeeId?: string
  paymentStatus?: PayrollEntry['paymentStatus']
}