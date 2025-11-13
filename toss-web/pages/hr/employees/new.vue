<template>
  <div class="new-employee">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
      <div>
        <nav class="flex items-center space-x-2 text-sm text-gray-500 dark:text-gray-400 mb-2">
          <NuxtLink to="/hr" class="hover:text-gray-700 dark:hover:text-gray-300">HR</NuxtLink>
          <Icon name="mdi:chevron-right" class="w-4 h-4" />
          <NuxtLink to="/hr/employees" class="hover:text-gray-700 dark:hover:text-gray-300">Employees</NuxtLink>
          <Icon name="mdi:chevron-right" class="w-4 h-4" />
          <span class="text-gray-900 dark:text-white">New Employee</span>
        </nav>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Add New Employee
        </h1>
        <p class="text-gray-600 dark:text-gray-400 mt-1">
          Create a new employee record with South African compliance
        </p>
      </div>
      <div class="mt-4 sm:mt-0">
        <button
          @click="navigateTo('/hr/employees')"
          class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 text-sm font-medium rounded-md hover:bg-gray-50 dark:hover:bg-gray-600"
        >
          <Icon name="mdi:arrow-left" class="w-4 h-4 mr-2" />
          Back to Employees
        </button>
      </div>
    </div>

    <!-- Form Container -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
      <form @submit.prevent="submitForm">
        <!-- Form Progress Indicator -->
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-4">
              <div
                v-for="(step, index) in formSteps"
                :key="step.id"
                :class="[
                  'flex items-center',
                  index < formSteps.length - 1 ? 'mr-8' : ''
                ]"
              >
                <div
                  :class="[
                    'flex items-center justify-center w-8 h-8 rounded-full text-sm font-medium',
                    currentStep >= index + 1
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-200 dark:bg-gray-700 text-gray-600 dark:text-gray-400'
                  ]"
                >
                  {{ index + 1 }}
                </div>
                <span
                  :class="[
                    'ml-2 text-sm font-medium',
                    currentStep >= index + 1
                      ? 'text-blue-600 dark:text-blue-400'
                      : 'text-gray-500 dark:text-gray-400'
                  ]"
                >
                  {{ step.title }}
                </span>
                <div
                  v-if="index < formSteps.length - 1"
                  :class="[
                    'ml-8 w-24 h-0.5',
                    currentStep > index + 1
                      ? 'bg-blue-600'
                      : 'bg-gray-200 dark:bg-gray-700'
                  ]"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Form Steps -->
        <div class="px-6 py-8">
          <!-- Step 1: Personal Information -->
          <div v-if="currentStep === 1" class="space-y-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-6">
              Personal Information
            </h3>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- First Name -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  First Name *
                </label>
                <input
                  v-model="employee.personalInfo.firstName"
                  type="text"
                  required
                  :class="[
                    'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white',
                    errors.firstName 
                      ? 'border-red-300 dark:border-red-600' 
                      : 'border-gray-300 dark:border-gray-600'
                  ]"
                  placeholder="Enter first name"
                />
                <p v-if="errors.firstName" class="text-red-600 dark:text-red-400 text-sm mt-1">
                  {{ errors.firstName }}
                </p>
              </div>

              <!-- Last Name -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Last Name *
                </label>
                <input
                  v-model="employee.personalInfo.lastName"
                  type="text"
                  required
                  :class="[
                    'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white',
                    errors.lastName 
                      ? 'border-red-300 dark:border-red-600' 
                      : 'border-gray-300 dark:border-gray-600'
                  ]"
                  placeholder="Enter last name"
                />
                <p v-if="errors.lastName" class="text-red-600 dark:text-red-400 text-sm mt-1">
                  {{ errors.lastName }}
                </p>
              </div>

              <!-- Middle Name -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Middle Name(s)
                </label>
                <input
                  v-model="employee.personalInfo.middleName"
                  type="text"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="Enter middle name(s)"
                />
              </div>

              <!-- Preferred Name -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Preferred Name
                </label>
                <input
                  v-model="employee.personalInfo.preferredName"
                  type="text"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="Enter preferred name"
                />
              </div>

              <!-- SA ID Number -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  SA ID Number *
                </label>
                <input
                  v-model="employee.personalInfo.idNumber"
                  type="text"
                  required
                  maxlength="13"
                  pattern="[0-9]{13}"
                  @input="validateSAID"
                  :class="[
                    'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white',
                    errors.idNumber 
                      ? 'border-red-300 dark:border-red-600' 
                      : 'border-gray-300 dark:border-gray-600'
                  ]"
                  placeholder="Enter 13-digit SA ID number"
                />
                <p v-if="errors.idNumber" class="text-red-600 dark:text-red-400 text-sm mt-1">
                  {{ errors.idNumber }}
                </p>
                <p v-if="employee.personalInfo.idNumber && !errors.idNumber && idInfo.isValid" class="text-green-600 dark:text-green-400 text-sm mt-1">
                  Valid SA ID - Birth Date: {{ idInfo.birthDate }}, Gender: {{ idInfo.gender }}, Citizenship: {{ idInfo.citizenship }}
                </p>
              </div>

              <!-- Date of Birth (auto-filled from ID) -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Date of Birth *
                </label>
                <input
                  v-model="employee.personalInfo.dateOfBirth"
                  type="date"
                  required
                  :disabled="idInfo.isValid"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
                />
              </div>

              <!-- Gender (auto-filled from ID) -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Gender *
                </label>
                <select
                  v-model="employee.personalInfo.gender"
                  required
                  :disabled="idInfo.isValid"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
                >
                  <option value="">Select gender</option>
                  <option value="male">Male</option>
                  <option value="female">Female</option>
                  <option value="other">Other</option>
                  <option value="prefer-not-to-say">Prefer not to say</option>
                </select>
              </div>

              <!-- Nationality -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Nationality *
                </label>
                <select
                  v-model="employee.personalInfo.nationality"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select nationality</option>
                  <option value="south-african">South African</option>
                  <option value="zimbabwean">Zimbabwean</option>
                  <option value="mozambican">Mozambican</option>
                  <option value="lesotho">Lesotho</option>
                  <option value="botswanan">Botswanan</option>
                  <option value="swaziland">Eswatini</option>
                  <option value="other">Other</option>
                </select>
              </div>

              <!-- Language Preference -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Preferred Language *
                </label>
                <select
                  v-model="employee.personalInfo.language"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select language</option>
                  <option value="en">English</option>
                  <option value="af">Afrikaans</option>
                  <option value="zu">Zulu</option>
                  <option value="xh">Xhosa</option>
                  <option value="st">Sesotho</option>
                  <option value="tn">Setswana</option>
                  <option value="ss">Swati</option>
                  <option value="ve">Venda</option>
                  <option value="ts">Tsonga</option>
                  <option value="nr">Ndebele</option>
                  <option value="nso">Sepedi</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Step 2: Contact Information -->
          <div v-if="currentStep === 2" class="space-y-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-6">
              Contact Information
            </h3>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Phone Number -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Phone Number *
                </label>
                <input
                  v-model="employee.contactInfo.phoneNumber"
                  type="tel"
                  required
                  pattern="[0-9+\s\-\(\)]+"
                  :class="[
                    'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white',
                    errors.phoneNumber 
                      ? 'border-red-300 dark:border-red-600' 
                      : 'border-gray-300 dark:border-gray-600'
                  ]"
                  placeholder="+27 12 345 6789"
                />
                <p v-if="errors.phoneNumber" class="text-red-600 dark:text-red-400 text-sm mt-1">
                  {{ errors.phoneNumber }}
                </p>
              </div>

              <!-- Alternative Phone -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Alternative Phone
                </label>
                <input
                  v-model="employee.contactInfo.alternativePhone"
                  type="tel"
                  pattern="[0-9+\s\-\(\)]+"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="+27 12 345 6789"
                />
              </div>

              <!-- Email -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Email Address
                </label>
                <input
                  v-model="employee.contactInfo.email"
                  type="email"
                  :class="[
                    'block w-full px-3 py-2 border rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white',
                    errors.email 
                      ? 'border-red-300 dark:border-red-600' 
                      : 'border-gray-300 dark:border-gray-600'
                  ]"
                  placeholder="employee@example.com"
                />
                <p v-if="errors.email" class="text-red-600 dark:text-red-400 text-sm mt-1">
                  {{ errors.email }}
                </p>
              </div>

              <!-- WhatsApp Number -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  WhatsApp Number
                </label>
                <input
                  v-model="employee.contactInfo.whatsappNumber"
                  type="tel"
                  pattern="[0-9+\s\-\(\)]+"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="+27 82 123 4567"
                />
                <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                  Used for work communication and shift notifications
                </p>
              </div>

              <!-- Home Address -->
              <div class="md:col-span-2">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Home Address *
                </label>
                <textarea
                  v-model="employee.contactInfo.address"
                  required
                  rows="3"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="Enter full address including township/location name"
                ></textarea>
                <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                  Include landmarks or nearest taxi rank for easy location
                </p>
              </div>

              <!-- Emergency Contact Name -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Emergency Contact Name *
                </label>
                <input
                  v-model="employee.contactInfo.emergencyContactName"
                  type="text"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="Full name of emergency contact"
                />
              </div>

              <!-- Emergency Contact Phone -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Emergency Contact Phone *
                </label>
                <input
                  v-model="employee.contactInfo.emergencyContactPhone"
                  type="tel"
                  required
                  pattern="[0-9+\s\-\(\)]+"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="+27 12 345 6789"
                />
              </div>

              <!-- Emergency Contact Relationship -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Relationship to Emergency Contact *
                </label>
                <select
                  v-model="employee.contactInfo.emergencyContactRelationship"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select relationship</option>
                  <option value="spouse">Spouse</option>
                  <option value="parent">Parent</option>
                  <option value="child">Child</option>
                  <option value="sibling">Sibling</option>
                  <option value="friend">Friend</option>
                  <option value="other">Other</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Step 3: Employment Details -->
          <div v-if="currentStep === 3" class="space-y-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-6">
              Employment Details
            </h3>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Employee Number -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Employee Number
                </label>
                <input
                  v-model="employee.employeeNumber"
                  type="text"
                  :disabled="isGeneratingEmployeeNumber"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white disabled:bg-gray-100 dark:disabled:bg-gray-600"
                  placeholder="Auto-generated"
                />
                <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                  Leave blank to auto-generate
                </p>
              </div>

              <!-- Position -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Position *
                </label>
                <input
                  v-model="employee.employment.position"
                  type="text"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="e.g., Shop Assistant, Cashier, Manager"
                />
              </div>

              <!-- Department -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Department
                </label>
                <input
                  v-model="employee.employment.department"
                  type="text"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="e.g., Sales, Operations, Kitchen"
                />
              </div>

              <!-- Employment Type -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Employment Type *
                </label>
                <select
                  v-model="employee.employment.employmentType"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select type</option>
                  <option value="permanent">Permanent</option>
                  <option value="contract">Fixed-term Contract</option>
                  <option value="casual">Casual</option>
                  <option value="part-time">Part-time</option>
                </select>
              </div>

              <!-- Start Date -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Start Date *
                </label>
                <input
                  v-model="employee.employment.startDate"
                  type="date"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                />
              </div>

              <!-- Contract End Date (if contract) -->
              <div v-if="employee.employment.employmentType === 'contract'">
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Contract End Date *
                </label>
                <input
                  v-model="employee.employment.contractEndDate"
                  type="date"
                  required
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                />
              </div>

              <!-- Basic Salary -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Basic Salary (Monthly) *
                </label>
                <div class="relative">
                  <span class="absolute left-3 top-2 text-gray-500 dark:text-gray-400">R</span>
                  <input
                    v-model="employee.compensation.basicSalary"
                    type="number"
                    step="0.01"
                    min="0"
                    required
                    @input="calculateTaxes"
                    class="block w-full pl-8 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                    placeholder="0.00"
                  />
                </div>
                <p v-if="employee.compensation.basicSalary" class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                  Annual: {{ formatCurrency(employee.compensation.basicSalary * 12) }}
                </p>
              </div>

              <!-- Working Hours -->
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Working Hours per Week
                </label>
                <input
                  v-model="employee.employment.workingHours"
                  type="number"
                  step="0.5"
                  min="0"
                  max="45"
                  class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  placeholder="40"
                />
                <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                  Maximum 45 hours per week as per SA Labour Law
                </p>
              </div>
            </div>

            <!-- Tax Calculations Preview -->
            <div v-if="employee.compensation.basicSalary" class="mt-8 p-6 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
                SA Tax & Deduction Estimates
              </h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <p class="text-sm text-gray-600 dark:text-gray-400">UIF (1%)</p>
                  <p class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ formatCurrency(calculateUIF(employee.compensation.basicSalary)) }}/month
                  </p>
                </div>
                <div>
                  <p class="text-sm text-gray-600 dark:text-gray-400">PAYE (Est.)</p>
                  <p class="text-lg font-medium text-gray-900 dark:text-white">
                    {{ formatCurrency(calculatePAYE(employee.compensation.basicSalary * 12) / 12) }}/month
                  </p>
                </div>
                <div>
                  <p class="text-sm text-gray-600 dark:text-gray-400">Net Take Home</p>
                  <p class="text-lg font-medium text-green-600 dark:text-green-400">
                    {{ formatCurrency(
                      employee.compensation.basicSalary - 
                      calculateUIF(employee.compensation.basicSalary) - 
                      (calculatePAYE(employee.compensation.basicSalary * 12) / 12)
                    ) }}/month
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Step 4: Review & Submit -->
          <div v-if="currentStep === 4" class="space-y-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-6">
              Review Employee Information
            </h3>

            <!-- Review Summary -->
            <div class="space-y-6">
              <!-- Personal Info Review -->
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <h4 class="font-medium text-gray-900 dark:text-white mb-4">Personal Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div><span class="text-gray-600 dark:text-gray-400">Name:</span> {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">SA ID:</span> {{ employee.personalInfo.idNumber }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Date of Birth:</span> {{ formatDate(employee.personalInfo.dateOfBirth) }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Gender:</span> {{ employee.personalInfo.gender }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Language:</span> {{ getLanguageName(employee.personalInfo.language) }}</div>
                </div>
              </div>

              <!-- Contact Info Review -->
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <h4 class="font-medium text-gray-900 dark:text-white mb-4">Contact Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div><span class="text-gray-600 dark:text-gray-400">Phone:</span> {{ employee.contactInfo.phoneNumber }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Email:</span> {{ employee.contactInfo.email || 'Not provided' }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Emergency Contact:</span> {{ employee.contactInfo.emergencyContactName }} ({{ employee.contactInfo.emergencyContactRelationship }})</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Emergency Phone:</span> {{ employee.contactInfo.emergencyContactPhone }}</div>
                </div>
              </div>

              <!-- Employment Info Review -->
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <h4 class="font-medium text-gray-900 dark:text-white mb-4">Employment Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                  <div><span class="text-gray-600 dark:text-gray-400">Position:</span> {{ employee.employment.position }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Type:</span> {{ employee.employment.employmentType }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Start Date:</span> {{ formatDate(employee.employment.startDate) }}</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Salary:</span> {{ formatCurrency(employee.compensation.basicSalary) }}/month</div>
                  <div><span class="text-gray-600 dark:text-gray-400">Working Hours:</span> {{ employee.employment.workingHours || 40 }} hours/week</div>
                </div>
              </div>
            </div>

            <!-- Final Confirmations -->
            <div class="space-y-4">
              <label class="flex items-start space-x-3">
                <input
                  v-model="confirmations.accuracy"
                  type="checkbox"
                  required
                  class="mt-1 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <span class="text-sm text-gray-700 dark:text-gray-300">
                  I confirm that all information provided is accurate and complete
                </span>
              </label>
              
              <label class="flex items-start space-x-3">
                <input
                  v-model="confirmations.compliance"
                  type="checkbox"
                  required
                  class="mt-1 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <span class="text-sm text-gray-700 dark:text-gray-300">
                  I confirm compliance with South African Labour Laws and POPIA requirements
                </span>
              </label>

              <label class="flex items-start space-x-3">
                <input
                  v-model="confirmations.consent"
                  type="checkbox"
                  required
                  class="mt-1 h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <span class="text-sm text-gray-700 dark:text-gray-300">
                  Employee has consented to processing of personal information as per POPIA
                </span>
              </label>
            </div>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="px-6 py-4 bg-gray-50 dark:bg-gray-700 border-t border-gray-200 dark:border-gray-600 flex justify-between">
          <button
            v-if="currentStep > 1"
            type="button"
            @click="previousStep"
            class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <Icon name="mdi:chevron-left" class="w-4 h-4 mr-2" />
            Previous
          </button>
          <div v-else></div>

          <div class="flex space-x-3">
            <button
              type="button"
              @click="saveDraft"
              :disabled="isSubmitting"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Icon name="mdi:content-save" class="w-4 h-4 mr-2" />
              Save Draft
            </button>

            <button
              v-if="currentStep < 4"
              type="button"
              @click="nextStep"
              :disabled="!canProceedToNextStep"
              class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
              <Icon name="mdi:chevron-right" class="w-4 h-4 ml-2" />
            </button>

            <button
              v-if="currentStep === 4"
              type="submit"
              :disabled="isSubmitting || !canSubmit"
              class="inline-flex items-center px-6 py-2 bg-green-600 text-white text-sm font-medium rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Icon name="mdi:check" class="w-4 h-4 mr-2" />
              {{ isSubmitting ? 'Creating Employee...' : 'Create Employee' }}
            </button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, watch } from 'vue'
import type { Employee, CreateEmployeeRequest } from '~/types/hr'

// Meta
definePageMeta({
  title: 'New Employee',
  layout: 'default'
})

// Composables
const {
  isLoading,
  isSubmitting,
  error,
  createEmployee,
  validateSAIDNumber,
  formatCurrency,
  formatDate,
  calculateUIF,
  calculatePAYE
} = useHR()

// Form steps
const formSteps = [
  { id: 1, title: 'Personal' },
  { id: 2, title: 'Contact' },
  { id: 3, title: 'Employment' },
  { id: 4, title: 'Review' }
]

const currentStep = ref(1)

// Employee data
const employee = reactive<CreateEmployeeRequest>({
  personalInfo: {
    firstName: '',
    lastName: '',
    middleName: '',
    preferredName: '',
    idNumber: '',
    dateOfBirth: '',
    gender: '',
    nationality: '',
    language: ''
  },
  contactInfo: {
    phoneNumber: '',
    alternativePhone: '',
    email: '',
    whatsappNumber: '',
    address: '',
    emergencyContactName: '',
    emergencyContactPhone: '',
    emergencyContactRelationship: ''
  },
  employment: {
    position: '',
    department: '',
    employmentType: '',
    startDate: '',
    contractEndDate: '',
    workingHours: 40
  },
  compensation: {
    basicSalary: 0,
    currency: 'ZAR',
    payFrequency: 'monthly'
  },
  employeeNumber: '',
  status: 'active'
})

// Form validation
const errors = reactive({
  firstName: '',
  lastName: '',
  idNumber: '',
  phoneNumber: '',
  email: ''
})

const confirmations = reactive({
  accuracy: false,
  compliance: false,
  consent: false
})

// ID Number validation info
const idInfo = reactive({
  isValid: false,
  birthDate: '',
  gender: '',
  citizenship: ''
})

const isGeneratingEmployeeNumber = ref(false)

// Methods
const validateSAID = async () => {
  if (!employee.personalInfo.idNumber || employee.personalInfo.idNumber.length !== 13) {
    errors.idNumber = 'SA ID must be 13 digits'
    idInfo.isValid = false
    return
  }

  try {
    const validation = await validateSAIDNumber(employee.personalInfo.idNumber)
    if (validation.isValid) {
      errors.idNumber = ''
      idInfo.isValid = true
      idInfo.birthDate = validation.birthDate
      idInfo.gender = validation.gender
      idInfo.citizenship = validation.citizenship
      
      // Auto-fill derived fields
      employee.personalInfo.dateOfBirth = validation.birthDate
      employee.personalInfo.gender = validation.gender
    } else {
      errors.idNumber = validation.error || 'Invalid SA ID number'
      idInfo.isValid = false
    }
  } catch (err) {
    errors.idNumber = 'Failed to validate SA ID'
    idInfo.isValid = false
  }
}

const calculateTaxes = () => {
  // Trigger tax calculations for preview
  // This is already handled by the computed values in the template
}

const getLanguageName = (code: string) => {
  const languages: Record<string, string> = {
    'en': 'English',
    'af': 'Afrikaans',
    'zu': 'Zulu',
    'xh': 'Xhosa',
    'st': 'Sesotho',
    'tn': 'Setswana',
    'ss': 'Swati',
    've': 'Venda',
    'ts': 'Tsonga',
    'nr': 'Ndebele',
    'nso': 'Sepedi'
  }
  return languages[code] || code
}

const validateStep = (step: number): boolean => {
  // Clear previous errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  switch (step) {
    case 1:
      if (!employee.personalInfo.firstName.trim()) {
        errors.firstName = 'First name is required'
        return false
      }
      if (!employee.personalInfo.lastName.trim()) {
        errors.lastName = 'Last name is required'
        return false
      }
      if (!employee.personalInfo.idNumber || !idInfo.isValid) {
        errors.idNumber = 'Valid SA ID number is required'
        return false
      }
      return true

    case 2:
      if (!employee.contactInfo.phoneNumber.trim()) {
        errors.phoneNumber = 'Phone number is required'
        return false
      }
      if (employee.contactInfo.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(employee.contactInfo.email)) {
        errors.email = 'Valid email address required'
        return false
      }
      return true

    case 3:
      return true // Basic validation handled by required fields

    case 4:
      return confirmations.accuracy && confirmations.compliance && confirmations.consent

    default:
      return true
  }
}

const canProceedToNextStep = computed(() => {
  return validateStep(currentStep.value)
})

const canSubmit = computed(() => {
  return validateStep(4)
})

const nextStep = () => {
  if (validateStep(currentStep.value) && currentStep.value < 4) {
    currentStep.value++
  }
}

const previousStep = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const generateEmployeeNumber = async () => {
  isGeneratingEmployeeNumber.value = true
  try {
    // Generate employee number based on current year + sequential number
    const year = new Date().getFullYear()
    const randomNum = Math.floor(Math.random() * 1000).toString().padStart(3, '0')
    employee.employeeNumber = `${year}${randomNum}`
  } catch (err) {
    console.error('Failed to generate employee number:', err)
  } finally {
    isGeneratingEmployeeNumber.value = false
  }
}

const saveDraft = async () => {
  // TODO: Implement draft saving functionality
  console.log('Save draft functionality to be implemented')
}

const submitForm = async () => {
  if (!validateStep(currentStep.value)) return

  try {
    // Generate employee number if not provided
    if (!employee.employeeNumber) {
      await generateEmployeeNumber()
    }

    // Create the employee
    const newEmployee = await createEmployee(employee)
    
    // Success - redirect to employee profile
    await navigateTo(`/hr/employees/${newEmployee.id}`)
  } catch (err) {
    console.error('Failed to create employee:', err)
    // Error handling is managed by the composable
  }
}

// Auto-generate employee number on mount
onMounted(() => {
  if (!employee.employeeNumber) {
    generateEmployeeNumber()
  }
})
</script>

<style scoped>
.new-employee {
  @apply p-6 max-w-4xl mx-auto;
}
</style>