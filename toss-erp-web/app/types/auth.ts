// Shared authentication types

export interface AuthUser {
  id: number
  name: string
  email: string
  roles: string[]
  permissions: string[]
  avatar?: string
  lastLogin?: Date
  role?: string
}

export interface StoreUser {
  id: string
  email: string
  firstName: string
  lastName: string
  avatar?: string
  businessId?: string
  businessName?: string
  role: string
  status: 'active' | 'inactive' | 'pending'
  createdAt: string
  updatedAt: string
}

export interface LoginCredentials {
  email: string
  password: string
  rememberMe?: boolean
}

export interface AuthResponse {
  token: string
  refreshToken: string
  user: AuthUser
  expiresIn: number
}

export interface TokenPayload {
  sub: string
  email: string
  roles: string[]
  permissions: string[]
  exp: number
  iat: number
}

export interface RefreshTokenResponse {
  token: string
  expiresIn: number
}

export interface ChangePasswordData {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}

