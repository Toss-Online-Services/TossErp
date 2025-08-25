// Base configuration interface for all services
export interface ServiceConfig {
  name: string;
  port: number;
  host: string;
  environment: 'development' | 'staging' | 'production';
  database: {
    url: string;
    ssl?: boolean;
    maxConnections?: number;
  };
  redis: {
    host: string;
    port: number;
    password?: string;
    db?: number;
  };
  jwt: {
    secret: string;
    expiresIn: string;
    refreshExpiresIn: string;
  };
  cors: {
    origins: string[];
    credentials: boolean;
  };
  rateLimit: {
    max: number;
    timeWindow: string;
  };
  logging: {
    level: 'trace' | 'debug' | 'info' | 'warn' | 'error' | 'fatal';
    prettyPrint: boolean;
  };
  swagger: {
    enabled: boolean;
    title: string;
    description: string;
    version: string;
  };
}

// Default configuration factory
export function createServiceConfig(serviceName: string, port: number): ServiceConfig {
  return {
    name: serviceName,
    port,
    host: process.env.HOST || '0.0.0.0',
    environment: (process.env.NODE_ENV as any) || 'development',
    database: {
      url: process.env.DATABASE_URL || `postgresql://postgres:password@localhost:5432/${serviceName}`,
      ssl: process.env.NODE_ENV === 'production',
      maxConnections: parseInt(process.env.DB_MAX_CONNECTIONS || '10'),
    },
    redis: {
      host: process.env.REDIS_HOST || 'localhost',
      port: parseInt(process.env.REDIS_PORT || '6379'),
      password: process.env.REDIS_PASSWORD,
      db: 0,
    },
    jwt: {
      secret: process.env.JWT_SECRET || 'toss-erp-secret-key-change-in-production',
      expiresIn: process.env.JWT_EXPIRES_IN || '24h',
      refreshExpiresIn: process.env.JWT_REFRESH_EXPIRES_IN || '7d',
    },
    cors: {
      origins: process.env.CORS_ORIGINS?.split(',') || ['http://localhost:3000', 'http://localhost:3006'],
      credentials: true,
    },
    rateLimit: {
      max: parseInt(process.env.RATE_LIMIT_MAX || '100'),
      timeWindow: process.env.RATE_LIMIT_WINDOW || '1 minute',
    },
    logging: {
      level: (process.env.LOG_LEVEL as any) || 'info',
      prettyPrint: process.env.NODE_ENV === 'development',
    },
    swagger: {
      enabled: process.env.SWAGGER_ENABLED !== 'false',
      title: `TOSS ERP III - ${serviceName} Service`,
      description: `${serviceName} microservice for rural township enterprises`,
      version: '1.0.0',
    },
  };
}

// Global types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  tenantId: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export interface Tenant {
  id: string;
  name: string;
  type: TenantType;
  settings: Record<string, any>;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
}

export enum UserRole {
  SUPER_ADMIN = 'SUPER_ADMIN',
  TENANT_ADMIN = 'TENANT_ADMIN',
  MANAGER = 'MANAGER',
  EMPLOYEE = 'EMPLOYEE',
  CUSTOMER = 'CUSTOMER',
  VENDOR = 'VENDOR',
}

export enum TenantType {
  RETAIL_TRADE = 'RETAIL_TRADE',
  FOOD_HOSPITALITY = 'FOOD_HOSPITALITY',
  PERSONAL_SERVICES = 'PERSONAL_SERVICES',
  TRADES_TECHNICAL = 'TRADES_TECHNICAL',
  AGRICULTURE = 'AGRICULTURE',
  MANUFACTURING = 'MANUFACTURING',
  TRANSPORTATION = 'TRANSPORTATION',
  CONSTRUCTION = 'CONSTRUCTION',
  TECHNOLOGY = 'TECHNOLOGY',
  HEALTHCARE = 'HEALTHCARE',
  EDUCATION = 'EDUCATION',
  CREATIVE_ARTS = 'CREATIVE_ARTS',
  PROFESSIONAL_SERVICES = 'PROFESSIONAL_SERVICES',
  FINANCIAL_SERVICES = 'FINANCIAL_SERVICES',
  ENERGY_UTILITIES = 'ENERGY_UTILITIES',
  WASTE_RECYCLING = 'WASTE_RECYCLING',
  ENTERTAINMENT_EVENTS = 'ENTERTAINMENT_EVENTS',
  SECURITY_SERVICES = 'SECURITY_SERVICES',
  CLEANING_MAINTENANCE = 'CLEANING_MAINTENANCE',
  COMMUNITY_SERVICES = 'COMMUNITY_SERVICES',
  OTHER = 'OTHER',
}

// API Response types
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  error?: {
    code: string;
    message: string;
    details?: any;
  };
  meta?: {
    total?: number;
    page?: number;
    limit?: number;
    hasNext?: boolean;
    hasPrev?: boolean;
  };
}

export interface PaginationParams {
  page?: number;
  limit?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  search?: string;
}

export interface FilterParams {
  [key: string]: any;
}

// Event types for inter-service communication
export interface DomainEvent {
  id: string;
  type: string;
  aggregateId: string;
  aggregateType: string;
  version: number;
  data: Record<string, any>;
  metadata: {
    userId?: string;
    tenantId: string;
    timestamp: Date;
    correlationId?: string;
    causationId?: string;
  };
}

// Common audit fields
export interface AuditFields {
  createdAt: Date;
  updatedAt: Date;
  createdBy?: string;
  updatedBy?: string;
  version: number;
}

// Money type for financial amounts
export interface Money {
  amount: number;
  currency: string; // ISO 4217 currency code (ZAR for South African Rand)
}

// Address type
export interface Address {
  street: string;
  city: string;
  province: string;
  postalCode: string;
  country: string;
}

// Contact information
export interface ContactInfo {
  email?: string;
  phone?: string;
  mobile?: string;
  fax?: string;
  website?: string;
}

// File upload types
export interface FileUpload {
  id: string;
  filename: string;
  originalName: string;
  mimeType: string;
  size: number;
  url: string;
  uploadedBy: string;
  uploadedAt: Date;
}

// Business hours
export interface BusinessHours {
  monday?: { open: string; close: string; closed?: boolean };
  tuesday?: { open: string; close: string; closed?: boolean };
  wednesday?: { open: string; close: string; closed?: boolean };
  thursday?: { open: string; close: string; closed?: boolean };
  friday?: { open: string; close: string; closed?: boolean };
  saturday?: { open: string; close: string; closed?: boolean };
  sunday?: { open: string; close: string; closed?: boolean };
}

// Notification types
export enum NotificationType {
  EMAIL = 'EMAIL',
  SMS = 'SMS',
  PUSH = 'PUSH',
  IN_APP = 'IN_APP',
}

export interface Notification {
  id: string;
  type: NotificationType;
  recipient: string;
  subject: string;
  content: string;
  data?: Record<string, any>;
  sentAt?: Date;
  readAt?: Date;
  status: 'PENDING' | 'SENT' | 'DELIVERED' | 'FAILED' | 'READ';
}

// Workflow types
export interface WorkflowDefinition {
  id: string;
  name: string;
  description: string;
  version: number;
  isActive: boolean;
  trigger: WorkflowTrigger;
  steps: WorkflowStep[];
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface WorkflowTrigger {
  type: 'EVENT' | 'SCHEDULE' | 'MANUAL';
  eventType?: string;
  schedule?: string; // Cron expression
  conditions?: Record<string, any>;
}

export interface WorkflowStep {
  id: string;
  name: string;
  type: 'ACTION' | 'CONDITION' | 'APPROVAL' | 'DELAY';
  config: Record<string, any>;
  nextSteps: string[];
  errorHandling?: {
    retryCount: number;
    retryDelay: number;
    fallbackStep?: string;
  };
}

export interface WorkflowInstance {
  id: string;
  definitionId: string;
  status: 'RUNNING' | 'COMPLETED' | 'FAILED' | 'CANCELLED' | 'PAUSED';
  currentStep?: string;
  data: Record<string, any>;
  startedAt: Date;
  completedAt?: Date;
  error?: string;
  tenantId: string;
}

// AI types
export interface AIRequest {
  prompt: string;
  context?: Record<string, any>;
  model?: string;
  maxTokens?: number;
  temperature?: number;
}

export interface AIResponse {
  content: string;
  usage: {
    promptTokens: number;
    completionTokens: number;
    totalTokens: number;
  };
  model: string;
  finishReason: string;
}

// Rural enterprise specific types
export interface RuralEnterprise {
  id: string;
  name: string;
  type: TenantType;
  description: string;
  location: Address;
  contact: ContactInfo;
  businessHours: BusinessHours;
  ownerName: string;
  employeeCount: number;
  annualRevenue?: Money;
  establishedDate: Date;
  registrationNumber?: string;
  taxNumber?: string;
  bankingDetails?: {
    bankName: string;
    accountNumber: string;
    accountType: string;
    branchCode: string;
  };
  mobileMoney?: {
    provider: 'MTN_MONEY' | 'VODACOM_MONEY' | 'TELKOM_MONEY' | 'CELL_C_MONEY';
    number: string;
  };
  isActive: boolean;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

// Group buying types
export interface GroupBuyingInitiative {
  id: string;
  title: string;
  description: string;
  productCategory: string;
  targetQuantity: number;
  currentQuantity: number;
  unitPrice: Money;
  bulkPrice: Money;
  deadline: Date;
  status: 'OPEN' | 'CLOSED' | 'FULFILLED' | 'CANCELLED';
  organizer: string;
  participants: GroupBuyingParticipant[];
  requirements: string[];
  terms: string;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface GroupBuyingParticipant {
  userId: string;
  enterpriseId: string;
  quantity: number;
  commitment: Money;
  status: 'COMMITTED' | 'PAID' | 'DELIVERED' | 'CANCELLED';
  joinedAt: Date;
}

// Credit types
export interface CreditApplication {
  id: string;
  applicantId: string;
  enterpriseId: string;
  amount: Money;
  purpose: string;
  term: number; // months
  interestRate: number;
  status: 'PENDING' | 'APPROVED' | 'REJECTED' | 'DISBURSED' | 'REPAID';
  creditScore?: number;
  assessment?: {
    income: Money;
    expenses: Money;
    assets: Money;
    liabilities: Money;
    riskLevel: 'LOW' | 'MEDIUM' | 'HIGH';
    notes: string;
  };
  approvedAt?: Date;
  disbursedAt?: Date;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

// Asset sharing types
export interface SharedAsset {
  id: string;
  name: string;
  description: string;
  category: string;
  ownerId: string;
  location: Address;
  availability: AssetAvailability[];
  hourlyRate?: Money;
  dailyRate?: Money;
  weeklyRate?: Money;
  depositRequired?: Money;
  condition: 'EXCELLENT' | 'GOOD' | 'FAIR' | 'POOR';
  images: string[];
  specifications: Record<string, any>;
  usage: AssetUsage[];
  isActive: boolean;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AssetAvailability {
  dayOfWeek: number; // 0 = Sunday, 1 = Monday, etc.
  startTime: string; // HH:mm format
  endTime: string; // HH:mm format
}

export interface AssetUsage {
  id: string;
  assetId: string;
  userId: string;
  startDate: Date;
  endDate: Date;
  purpose: string;
  cost: Money;
  deposit?: Money;
  status: 'BOOKED' | 'IN_USE' | 'RETURNED' | 'OVERDUE' | 'DAMAGED';
  condition: {
    before: string;
    after?: string;
    notes?: string;
  };
  rating?: number; // 1-5 stars
  review?: string;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export default {
  ServiceConfig,
  createServiceConfig,
  User,
  Tenant,
  UserRole,
  TenantType,
  ApiResponse,
  PaginationParams,
  FilterParams,
  DomainEvent,
  AuditFields,
  Money,
  Address,
  ContactInfo,
  FileUpload,
  BusinessHours,
  NotificationType,
  Notification,
  WorkflowDefinition,
  WorkflowTrigger,
  WorkflowStep,
  WorkflowInstance,
  AIRequest,
  AIResponse,
  RuralEnterprise,
  GroupBuyingInitiative,
  GroupBuyingParticipant,
  CreditApplication,
  SharedAsset,
  AssetAvailability,
  AssetUsage,
};
