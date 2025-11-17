<template>
  <div class="edit-employee">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600 dark:text-gray-400">Loading employee information...</p>
      </div>
    </div>

    <!-- Employee Not Found -->
    <div v-else-if="error" class="text-center py-12">
      <Icon name="mdi:account-alert" class="w-16 h-16 text-gray-400 mx-auto mb-4" />
      <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-2">Employee Not Found</h3>
      <p class="text-gray-500 dark:text-gray-400 mb-4">{{ error }}</p>
      <button
        @click="navigateTo('/hr/employees')"
        class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700"
      >
        <Icon name="mdi:arrow-left" class="w-4 h-4 mr-2" />
        Back to Employees
      </button>
    </div>

    <!-- Edit Form -->
    <div v-else-if="employee" class="space-y-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center">
        <div>
          <nav class="flex items-center space-x-2 text-sm text-gray-500 dark:text-gray-400 mb-2">
            <NuxtLink to="/hr" class="hover:text-gray-700 dark:hover:text-gray-300">HR</NuxtLink>
            <Icon name="mdi:chevron-right" class="w-4 h-4" />
            <NuxtLink to="/hr/employees" class="hover:text-gray-700 dark:hover:text-gray-300">Employees</NuxtLink>
            <Icon name="mdi:chevron-right" class="w-4 h-4" />
            <NuxtLink :to="`/hr/employees/${employee.id}`" class="hover:text-gray-700 dark:hover:text-gray-300">
              {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
            </NuxtLink>
            <Icon name="mdi:chevron-right" class="w-4 h-4" />
            <span class="text-gray-900 dark:text-white">Edit</span>
          </nav>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            Edit Employee - {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
          </h1>
          <p class="text-gray-600 dark:text-gray-400 mt-1">
            Update employee information and employment details
          </p>
        </div>
        <div class="mt-4 sm:mt-0 flex space-x-3">
          <button
            @click="navigateTo(`/hr/employees/${employee.id}`)"
            class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 text-sm font-medium rounded-md hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <Icon name="mdi:arrow-left" class="w-4 h-4 mr-2" />
            Cancel
          </button>
        </div>
      </div>

      <!-- Form Container -->
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
        <form @submit.prevent="submitForm">
          <!-- Form Tabs -->
          <div class="border-b border-gray-200 dark:border-gray-700">
            <nav class="flex space-x-8 px-6" aria-label="Tabs">
              <button
                v-for="tab in tabs"
                :key="tab.id"
                type="button"
                @click="activeTab = tab.id"
                :class="[
                  'py-4 px-1 border-b-2 font-medium text-sm whitespace-nowrap',
                  activeTab === tab.id
                    ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                    : 'border-transparent text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:border-gray-300 dark:hover:border-gray-600'
                ]"
              >
                <Icon :name="tab.icon" class="w-5 h-5 inline mr-2" />
                {{ tab.name }}
              </button>
            </nav>
          </div>

          <!-- Tab Content -->
          <div class="p-6">
            <!-- Personal Information Tab -->
            <div v-if="activeTab === 'personal'" class="space-y-6">
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
                    v-model="formData.personalInfo.firstName"
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
                    v-model="formData.personalInfo.lastName"
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
                    v-model="formData.personalInfo.middleName"
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
                    v-model="formData.personalInfo.preferredName"
                    type="text"
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                    placeholder="Enter preferred name"
                  />
                </div>

                <!-- SA ID Number (read-only) -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    SA ID Number
                  </label>
                  <input
                    :value="formData.personalInfo.idNumber"
                    type="text"
                    disabled
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-gray-100 dark:bg-gray-600 text-gray-500 dark:text-gray-400 font-mono"
                  />
                  <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                    ID number cannot be changed
                  </p>
                </div>

                <!-- Date of Birth -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Date of Birth *
                  </label>
                  <input
                    v-model="formData.personalInfo.dateOfBirth"
                    type="date"
                    required
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  />
                </div>

                <!-- Gender -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Gender *
                  </label>
                  <select
                    v-model="formData.personalInfo.gender"
                    required
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
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
                    v-model="formData.personalInfo.nationality"
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
                    v-model="formData.personalInfo.language"
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

            <!-- Contact Information Tab -->
            <div v-if="activeTab === 'contact'" class="space-y-6">
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
                    v-model="formData.contactInfo.phoneNumber"
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
                    v-model="formData.contactInfo.alternativePhone"
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
                    v-model="formData.contactInfo.email"
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
                    v-model="formData.contactInfo.whatsappNumber"
                    type="tel"
                    pattern="[0-9+\s\-\(\)]+"
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                    placeholder="+27 82 123 4567"
                  />
                </div>

                <!-- Home Address -->
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Home Address *
                  </label>
                  <textarea
                    v-model="formData.contactInfo.address"
                    required
                    rows="3"
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                    placeholder="Enter full address including township/location name"
                  ></textarea>
                </div>

                <!-- Emergency Contact Name -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Emergency Contact Name *
                  </label>
                  <input
                    v-model="formData.contactInfo.emergencyContactName"
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
                    v-model="formData.contactInfo.emergencyContactPhone"
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
                    v-model="formData.contactInfo.emergencyContactRelationship"
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

            <!-- Employment Details Tab -->
            <div v-if="activeTab === 'employment'" class="space-y-6">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-6">
                Employment Details
              </h3>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Employee Number (read-only) -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Employee Number
                  </label>
                  <input
                    :value="formData.employeeNumber"
                    type="text"
                    disabled
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-gray-100 dark:bg-gray-600 text-gray-500 dark:text-gray-400 font-mono"
                  />
                  <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                    Employee number cannot be changed
                  </p>
                </div>

                <!-- Position -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Position *
                  </label>
                  <input
                    v-model="formData.employment.position"
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
                    v-model="formData.employment.department"
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
                    v-model="formData.employment.employmentType"
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

                <!-- Start Date (read-only) -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Start Date
                  </label>
                  <input
                    :value="formData.employment.startDate"
                    type="date"
                    disabled
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-gray-100 dark:bg-gray-600 text-gray-500 dark:text-gray-400"
                  />
                  <p class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                    Start date cannot be changed
                  </p>
                </div>

                <!-- Contract End Date (if contract) -->
                <div v-if="formData.employment.employmentType === 'contract'">
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Contract End Date *
                  </label>
                  <input
                    v-model="formData.employment.contractEndDate"
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
                      v-model="formData.compensation.basicSalary"
                      type="number"
                      step="0.01"
                      min="0"
                      required
                      @input="calculateTaxes"
                      class="block w-full pl-8 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                      placeholder="0.00"
                    />
                  </div>
                  <p v-if="formData.compensation.basicSalary" class="text-gray-500 dark:text-gray-400 text-sm mt-1">
                    Annual: {{ formatCurrency(formData.compensation.basicSalary * 12) }}
                  </p>
                </div>

                <!-- Working Hours -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Working Hours per Week
                  </label>
                  <input
                    v-model="formData.employment.workingHours"
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

                <!-- Employee Status -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    Employee Status *
                  </label>
                  <select
                    v-model="formData.status"
                    required
                    class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
                  >
                    <option value="active">Active</option>
                    <option value="inactive">Inactive</option>
                    <option value="suspended">Suspended</option>
                    <option value="terminated">Terminated</option>
                  </select>
                </div>
              </div>

              <!-- Tax Calculations Preview -->
              <div v-if="formData.compensation.basicSalary" class="mt-8 p-6 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
                  SA Tax & Deduction Estimates (Updated)
                </h4>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <p class="text-sm text-gray-600 dark:text-gray-400">UIF (1%)</p>
                    <p class="text-lg font-medium text-gray-900 dark:text-white">
                      {{ formatCurrency(calculateUIF(formData.compensation.basicSalary)) }}/month
                    </p>
                  </div>
                  <div>
                    <p class="text-sm text-gray-600 dark:text-gray-400">PAYE (Est.)</p>
                    <p class="text-lg font-medium text-gray-900 dark:text-white">
                      {{ formatCurrency(calculatePAYE(formData.compensation.basicSalary * 12) / 12) }}/month
                    </p>
                  </div>
                  <div>
                    <p class="text-sm text-gray-600 dark:text-gray-400">Net Take Home</p>
                    <p class="text-lg font-medium text-green-600 dark:text-green-400">
                      {{ formatCurrency(
                        formData.compensation.basicSalary - 
                        calculateUIF(formData.compensation.basicSalary) - 
                        (calculatePAYE(formData.compensation.basicSalary * 12) / 12)
                      ) }}/month
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="px-6 py-4 bg-gray-50 dark:bg-gray-700 border-t border-gray-200 dark:border-gray-600 flex justify-between">
            <div class="flex space-x-3">
              <button
                type="button"
                @click="activeTab = getPreviousTab()"
                :disabled="isFirstTab"
                class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Icon name="mdi:chevron-left" class="w-4 h-4 mr-2" />
                Previous
              </button>
            </div>

            <div class="flex space-x-3">
              <button
                type="button"
                @click="resetForm"
                :disabled="isSubmitting"
                class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Icon name="mdi:refresh" class="w-4 h-4 mr-2" />
                Reset Changes
              </button>

              <button
                v-if="!isLastTab"
                type="button"
                @click="activeTab = getNextTab()"
                class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
              >
                Next
                <Icon name="mdi:chevron-right" class="w-4 h-4 ml-2" />
              </button>

              <button
                v-if="isLastTab"
                type="submit"
                :disabled="isSubmitting || !isFormValid"
                class="inline-flex items-center px-6 py-2 bg-green-600 text-white text-sm font-medium rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Icon name="mdi:check" class="w-4 h-4 mr-2" />
                {{ isSubmitting ? 'Updating Employee...' : 'Update Employee' }}
              </button>
            </div>
          </div>
        </form>
      </div>

      <!-- Danger Zone -->
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg border border-red-200 dark:border-red-700 overflow-hidden">
        <div class="px-6 py-4 bg-red-50 dark:bg-red-900/20 border-b border-red-200 dark:border-red-700">
          <h3 class="text-lg font-medium text-red-900 dark:text-red-200">Danger Zone</h3>
          <p class="text-red-600 dark:text-red-300 text-sm mt-1">
            Irreversible and destructive actions
          </p>
        </div>
        <div class="px-6 py-4 space-y-4">
          <div class="flex justify-between items-center">
            <div>
              <h4 class="text-sm font-medium text-gray-900 dark:text-white">Delete Employee</h4>
              <p class="text-sm text-gray-600 dark:text-gray-400">
                Permanently delete this employee record. This action cannot be undone.
              </p>
            </div>
            <button
              @click="deleteEmployee"
              :disabled="isDeleting"
              class="inline-flex items-center px-4 py-2 bg-red-600 text-white text-sm font-medium rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Icon name="mdi:delete" class="w-4 h-4 mr-2" />
              {{ isDeleting ? 'Deleting...' : 'Delete Employee' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, watch } from 'vue'
import type { Employee, UpdateEmployeeRequest } from '~/types/hr'

// Meta
definePageMeta({
  title: 'Edit Employee',
  layout: 'default'
})

// Route params
const route = useRoute()
const employeeId = route.params.id as string

// Composables
const {
  isLoading,
  isSubmitting,
  error,
  employee,
  getEmployee,
  updateEmployee,
  deleteEmployee: deleteEmployeeAction,
  formatCurrency,
  formatDate,
  calculateUIF,
  calculatePAYE
} = useHR()

// Local state
const activeTab = ref('personal')
const isDeleting = ref(false)

const tabs = [
  { id: 'personal', name: 'Personal', icon: 'mdi:account' },
  { id: 'contact', name: 'Contact', icon: 'mdi:phone' },
  { id: 'employment', name: 'Employment', icon: 'mdi:briefcase' }
]

// Form data
const formData = reactive<UpdateEmployeeRequest>({
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
  phoneNumber: '',
  email: ''
})

// Computed properties
const isFirstTab = computed(() => activeTab.value === tabs[0].id)
const isLastTab = computed(() => activeTab.value === tabs[tabs.length - 1].id)

const isFormValid = computed(() => {
  return formData.personalInfo.firstName.trim() &&
         formData.personalInfo.lastName.trim() &&
         formData.contactInfo.phoneNumber.trim() &&
         formData.employment.position.trim() &&
         formData.compensation.basicSalary > 0
})

// Methods
const populateForm = () => {
  if (!employee.value) return

  Object.assign(formData.personalInfo, employee.value.personalInfo)
  Object.assign(formData.contactInfo, employee.value.contactInfo)
  Object.assign(formData.employment, employee.value.employment)
  Object.assign(formData.compensation, employee.value.compensation)
  formData.employeeNumber = employee.value.employeeNumber
  formData.status = employee.value.status
}

const resetForm = () => {
  populateForm()
  // Clear errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })
}

const validateForm = (): boolean => {
  // Clear previous errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })

  let isValid = true

  if (!formData.personalInfo.firstName.trim()) {
    errors.firstName = 'First name is required'
    isValid = false
  }

  if (!formData.personalInfo.lastName.trim()) {
    errors.lastName = 'Last name is required'
    isValid = false
  }

  if (!formData.contactInfo.phoneNumber.trim()) {
    errors.phoneNumber = 'Phone number is required'
    isValid = false
  }

  if (formData.contactInfo.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.contactInfo.email)) {
    errors.email = 'Valid email address required'
    isValid = false
  }

  return isValid
}

const getPreviousTab = (): string => {
  const currentIndex = tabs.findIndex(tab => tab.id === activeTab.value)
  return currentIndex > 0 ? tabs[currentIndex - 1].id : tabs[0].id
}

const getNextTab = (): string => {
  const currentIndex = tabs.findIndex(tab => tab.id === activeTab.value)
  return currentIndex < tabs.length - 1 ? tabs[currentIndex + 1].id : tabs[tabs.length - 1].id
}

const calculateTaxes = () => {
  // Trigger tax calculations for preview
  // This is already handled by the computed values in the template
}

const submitForm = async () => {
  if (!validateForm()) {
    // Switch to the first tab with errors
    if (errors.firstName || errors.lastName) {
      activeTab.value = 'personal'
    } else if (errors.phoneNumber || errors.email) {
      activeTab.value = 'contact'
    }
    return
  }

  try {
    await updateEmployee(employeeId, formData)
    // Success - redirect to employee profile
    await navigateTo(`/hr/employees/${employeeId}`)
  } catch (err) {
    console.error('Failed to update employee:', err)
  }
}

const deleteEmployee = async () => {
  if (!employee.value) return

  const confirmed = confirm(
    `Are you sure you want to permanently delete ${employee.value.personalInfo.firstName} ${employee.value.personalInfo.lastName}? This action cannot be undone and will remove all associated records.`
  )

  if (confirmed) {
    const doubleConfirmed = confirm(
      'This is your final warning. Deleting an employee will permanently remove their record, payroll history, leave records, and all associated data. Type "DELETE" to confirm.'
    )

    if (doubleConfirmed) {
      isDeleting.value = true
      try {
        await deleteEmployeeAction(employeeId)
        // Success - redirect to employee list
        await navigateTo('/hr/employees')
      } catch (err) {
        console.error('Failed to delete employee:', err)
      } finally {
        isDeleting.value = false
      }
    }
  }
}

// Watchers
watch(() => employee.value, (newEmployee) => {
  if (newEmployee) {
    populateForm()
  }
}, { immediate: true })

// Lifecycle
onMounted(async () => {
  try {
    await getEmployee(employeeId)
  } catch (err) {
    console.error('Failed to load employee:', err)
  }
})
</script>

<style scoped>
.edit-employee {
  @apply p-6 max-w-4xl mx-auto;
}
</style>