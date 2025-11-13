<template>
  <div class="employee-detail">
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

    <!-- Employee Detail View -->
    <div v-else-if="employee" class="space-y-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center">
        <div>
          <nav class="flex items-center space-x-2 text-sm text-gray-500 dark:text-gray-400 mb-2">
            <NuxtLink to="/hr" class="hover:text-gray-700 dark:hover:text-gray-300">HR</NuxtLink>
            <Icon name="mdi:chevron-right" class="w-4 h-4" />
            <NuxtLink to="/hr/employees" class="hover:text-gray-700 dark:hover:text-gray-300">Employees</NuxtLink>
            <Icon name="mdi:chevron-right" class="w-4 h-4" />
            <span class="text-gray-900 dark:text-white">{{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}</span>
          </nav>
          <div class="flex items-center space-x-4">
            <div class="w-16 h-16 bg-blue-500 rounded-full flex items-center justify-center">
              <span class="text-white font-medium text-lg">
                {{ employee.personalInfo.firstName.charAt(0) }}{{ employee.personalInfo.lastName.charAt(0) }}
              </span>
            </div>
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
              </h1>
              <p class="text-gray-600 dark:text-gray-400">
                {{ employee.employment.position }} • {{ employee.employeeNumber }}
              </p>
              <div class="flex items-center mt-1">
                <StatusBadge :status="employee.status" />
                <span class="ml-2 text-sm text-gray-500 dark:text-gray-400">
                  Started {{ formatDate(employee.employment.startDate) }}
                </span>
              </div>
            </div>
          </div>
        </div>
        <div class="mt-4 sm:mt-0 flex space-x-3">
          <button
            @click="navigateTo(`/hr/employees/${employee.id}/edit`)"
            class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 text-sm font-medium rounded-md hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <Icon name="mdi:pencil" class="w-4 h-4 mr-2" />
            Edit
          </button>
          <button
            @click="toggleEmployeeStatus"
            :disabled="isUpdating"
            :class="[
              'inline-flex items-center px-4 py-2 text-sm font-medium rounded-md disabled:opacity-50 disabled:cursor-not-allowed',
              employee.status === 'active' 
                ? 'bg-yellow-600 text-white hover:bg-yellow-700'
                : 'bg-green-600 text-white hover:bg-green-700'
            ]"
          >
            <Icon :name="employee.status === 'active' ? 'mdi:pause' : 'mdi:play'" class="w-4 h-4 mr-2" />
            {{ employee.status === 'active' ? 'Deactivate' : 'Activate' }}
          </button>
        </div>
      </div>

      <!-- Quick Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <!-- Salary Card -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Monthly Salary</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ formatCurrency(employee.compensation.basicSalary) }}
              </p>
            </div>
            <Icon name="mdi:cash" class="w-8 h-8 text-green-500" />
          </div>
        </div>

        <!-- Years of Service -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Years of Service</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ calculateYearsOfService(employee.employment.startDate) }}
              </p>
            </div>
            <Icon name="mdi:calendar-clock" class="w-8 h-8 text-blue-500" />
          </div>
        </div>

        <!-- Leave Balance -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Leave Balance</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ employee.leave?.annual?.balance || 0 }} days
              </p>
            </div>
            <Icon name="mdi:beach" class="w-8 h-8 text-orange-500" />
          </div>
        </div>

        <!-- Performance -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Performance</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">
                {{ employee.performance?.currentRating || 'N/A' }}
              </p>
            </div>
            <Icon name="mdi:star" class="w-8 h-8 text-purple-500" />
          </div>
        </div>
      </div>

      <!-- Main Content Tabs -->
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
        <!-- Tab Navigation -->
        <div class="border-b border-gray-200 dark:border-gray-700">
          <nav class="flex space-x-8 px-6" aria-label="Tabs">
            <button
              v-for="tab in tabs"
              :key="tab.id"
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
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <!-- Basic Information -->
              <div class="space-y-4">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Basic Information</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Full Name:</span>
                    <span class="text-gray-900 dark:text-white font-medium">
                      {{ employee.personalInfo.firstName }} {{ employee.personalInfo.middleName }} {{ employee.personalInfo.lastName }}
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Preferred Name:</span>
                    <span class="text-gray-900 dark:text-white font-medium">
                      {{ employee.personalInfo.preferredName || employee.personalInfo.firstName }}
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">SA ID Number:</span>
                    <span class="text-gray-900 dark:text-white font-mono">{{ employee.personalInfo.idNumber }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Date of Birth:</span>
                    <span class="text-gray-900 dark:text-white">{{ formatDate(employee.personalInfo.dateOfBirth) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Age:</span>
                    <span class="text-gray-900 dark:text-white">{{ calculateAge(employee.personalInfo.dateOfBirth) }} years</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Gender:</span>
                    <span class="text-gray-900 dark:text-white capitalize">{{ employee.personalInfo.gender }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Nationality:</span>
                    <span class="text-gray-900 dark:text-white capitalize">{{ employee.personalInfo.nationality }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Language:</span>
                    <span class="text-gray-900 dark:text-white">{{ getLanguageName(employee.personalInfo.language) }}</span>
                  </div>
                </div>
              </div>

              <!-- Contact Information -->
              <div class="space-y-4">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Contact Information</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Phone:</span>
                    <a :href="`tel:${employee.contactInfo.phoneNumber}`" class="text-blue-600 dark:text-blue-400 hover:underline">
                      {{ employee.contactInfo.phoneNumber }}
                    </a>
                  </div>
                  <div v-if="employee.contactInfo.alternativePhone" class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Alt Phone:</span>
                    <a :href="`tel:${employee.contactInfo.alternativePhone}`" class="text-blue-600 dark:text-blue-400 hover:underline">
                      {{ employee.contactInfo.alternativePhone }}
                    </a>
                  </div>
                  <div v-if="employee.contactInfo.email" class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Email:</span>
                    <a :href="`mailto:${employee.contactInfo.email}`" class="text-blue-600 dark:text-blue-400 hover:underline">
                      {{ employee.contactInfo.email }}
                    </a>
                  </div>
                  <div v-if="employee.contactInfo.whatsappNumber" class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">WhatsApp:</span>
                    <a :href="`https://wa.me/${employee.contactInfo.whatsappNumber.replace(/[^0-9]/g, '')}`" target="_blank" class="text-green-600 dark:text-green-400 hover:underline">
                      {{ employee.contactInfo.whatsappNumber }}
                    </a>
                  </div>
                  <div class="flex justify-between items-start">
                    <span class="text-gray-600 dark:text-gray-400">Address:</span>
                    <span class="text-gray-900 dark:text-white text-right max-w-xs">{{ employee.contactInfo.address }}</span>
                  </div>
                </div>

                <!-- Emergency Contact -->
                <div class="mt-6 pt-6 border-t border-gray-200 dark:border-gray-700">
                  <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">Emergency Contact</h4>
                  <div class="space-y-3">
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Name:</span>
                      <span class="text-gray-900 dark:text-white">{{ employee.contactInfo.emergencyContactName }}</span>
                    </div>
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Phone:</span>
                      <a :href="`tel:${employee.contactInfo.emergencyContactPhone}`" class="text-blue-600 dark:text-blue-400 hover:underline">
                        {{ employee.contactInfo.emergencyContactPhone }}
                      </a>
                    </div>
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Relationship:</span>
                      <span class="text-gray-900 dark:text-white capitalize">{{ employee.contactInfo.emergencyContactRelationship }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Employment Tab -->
          <div v-if="activeTab === 'employment'" class="space-y-6">
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <!-- Employment Details -->
              <div class="space-y-4">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Employment Details</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Employee Number:</span>
                    <span class="text-gray-900 dark:text-white font-mono">{{ employee.employeeNumber }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Position:</span>
                    <span class="text-gray-900 dark:text-white">{{ employee.employment.position }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Department:</span>
                    <span class="text-gray-900 dark:text-white">{{ employee.employment.department || 'Not assigned' }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Employment Type:</span>
                    <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200">
                      {{ employee.employment.employmentType }}
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Start Date:</span>
                    <span class="text-gray-900 dark:text-white">{{ formatDate(employee.employment.startDate) }}</span>
                  </div>
                  <div v-if="employee.employment.contractEndDate" class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Contract End Date:</span>
                    <span class="text-gray-900 dark:text-white">{{ formatDate(employee.employment.contractEndDate) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Working Hours:</span>
                    <span class="text-gray-900 dark:text-white">{{ employee.employment.workingHours || 40 }} hours/week</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Status:</span>
                    <StatusBadge :status="employee.status" />
                  </div>
                </div>
              </div>

              <!-- Compensation -->
              <div class="space-y-4">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">Compensation</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Basic Salary:</span>
                    <span class="text-gray-900 dark:text-white font-bold">{{ formatCurrency(employee.compensation.basicSalary) }}/month</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Annual Salary:</span>
                    <span class="text-gray-900 dark:text-white">{{ formatCurrency(employee.compensation.basicSalary * 12) }}</span>
                  </div>
                  
                  <!-- SA Tax Calculations -->
                  <div class="mt-6 pt-4 border-t border-gray-200 dark:border-gray-700">
                    <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">SA Tax Deductions</h4>
                    <div class="space-y-2">
                      <div class="flex justify-between">
                        <span class="text-gray-600 dark:text-gray-400">UIF (1%):</span>
                        <span class="text-gray-900 dark:text-white">{{ formatCurrency(calculateUIF(employee.compensation.basicSalary)) }}/month</span>
                      </div>
                      <div class="flex justify-between">
                        <span class="text-gray-600 dark:text-gray-400">PAYE (Est.):</span>
                        <span class="text-gray-900 dark:text-white">{{ formatCurrency(calculatePAYE(employee.compensation.basicSalary * 12) / 12) }}/month</span>
                      </div>
                      <div class="flex justify-between font-medium pt-2 border-t border-gray-200 dark:border-gray-700">
                        <span class="text-gray-900 dark:text-white">Net Take Home:</span>
                        <span class="text-green-600 dark:text-green-400 font-bold">
                          {{ formatCurrency(
                            employee.compensation.basicSalary - 
                            calculateUIF(employee.compensation.basicSalary) - 
                            (calculatePAYE(employee.compensation.basicSalary * 12) / 12)
                          ) }}/month
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Leave Tab -->
          <div v-if="activeTab === 'leave'" class="space-y-6">
            <div class="flex justify-between items-center">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Leave Management</h3>
              <button 
                @click="requestLeave"
                class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700"
              >
                <Icon name="mdi:plus" class="w-4 h-4 mr-2" />
                Request Leave
              </button>
            </div>

            <!-- Leave Balances -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <div class="text-center">
                  <Icon name="mdi:calendar" class="w-8 h-8 text-blue-500 mx-auto mb-2" />
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ employee.leave?.annual?.balance || 0 }}</p>
                  <p class="text-sm text-gray-600 dark:text-gray-400">Annual Leave Days</p>
                </div>
              </div>
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <div class="text-center">
                  <Icon name="mdi:medical-bag" class="w-8 h-8 text-red-500 mx-auto mb-2" />
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ employee.leave?.sick?.balance || 0 }}</p>
                  <p class="text-sm text-gray-600 dark:text-gray-400">Sick Leave Days</p>
                </div>
              </div>
              <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
                <div class="text-center">
                  <Icon name="mdi:baby-face" class="w-8 h-8 text-pink-500 mx-auto mb-2" />
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ employee.leave?.maternity?.balance || 0 }}</p>
                  <p class="text-sm text-gray-600 dark:text-gray-400">Maternity Leave Days</p>
                </div>
              </div>
            </div>

            <!-- Leave History -->
            <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
              <h4 class="font-medium text-gray-900 dark:text-white mb-4">Recent Leave Requests</h4>
              <div v-if="!employee.leaveHistory?.length" class="text-center py-8">
                <Icon name="mdi:calendar-blank" class="w-12 h-12 text-gray-400 mx-auto mb-2" />
                <p class="text-gray-500 dark:text-gray-400">No leave requests yet</p>
              </div>
              <div v-else class="space-y-3">
                <div 
                  v-for="leave in employee.leaveHistory.slice(0, 5)" 
                  :key="leave.id"
                  class="flex items-center justify-between p-3 bg-white dark:bg-gray-600 rounded"
                >
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">{{ leave.type }} Leave</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">
                      {{ formatDate(leave.startDate) }} - {{ formatDate(leave.endDate) }}
                    </p>
                  </div>
                  <span :class="[
                    'px-2 py-1 text-xs font-semibold rounded-full',
                    leave.status === 'approved' ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' :
                    leave.status === 'pending' ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200' :
                    'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                  ]">
                    {{ leave.status }}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <!-- Performance Tab -->
          <div v-if="activeTab === 'performance'" class="space-y-6">
            <div class="flex justify-between items-center">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Performance Management</h3>
              <button 
                @click="addPerformanceReview"
                class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700"
              >
                <Icon name="mdi:plus" class="w-4 h-4 mr-2" />
                Add Review
              </button>
            </div>

            <!-- Performance Overview -->
            <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-lg">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <h4 class="font-medium text-gray-900 dark:text-white mb-4">Current Performance</h4>
                  <div class="space-y-3">
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Overall Rating:</span>
                      <span class="text-gray-900 dark:text-white font-bold">
                        {{ employee.performance?.currentRating || 'Not rated' }}
                      </span>
                    </div>
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Last Review:</span>
                      <span class="text-gray-900 dark:text-white">
                        {{ employee.performance?.lastReviewDate ? formatDate(employee.performance.lastReviewDate) : 'Never' }}
                      </span>
                    </div>
                    <div class="flex justify-between">
                      <span class="text-gray-600 dark:text-gray-400">Next Review:</span>
                      <span class="text-gray-900 dark:text-white">
                        {{ employee.performance?.nextReviewDate ? formatDate(employee.performance.nextReviewDate) : 'TBD' }}
                      </span>
                    </div>
                  </div>
                </div>
                <div>
                  <h4 class="font-medium text-gray-900 dark:text-white mb-4">Goals Progress</h4>
                  <div v-if="!employee.performance?.goals?.length" class="text-gray-500 dark:text-gray-400">
                    No goals set
                  </div>
                  <div v-else class="space-y-2">
                    <div 
                      v-for="goal in employee.performance.goals.slice(0, 3)" 
                      :key="goal.id"
                      class="flex items-center justify-between"
                    >
                      <span class="text-sm text-gray-900 dark:text-white">{{ goal.title }}</span>
                      <span class="text-sm text-gray-600 dark:text-gray-400">{{ goal.progress || 0 }}%</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Performance History -->
            <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg">
              <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
                <h4 class="font-medium text-gray-900 dark:text-white">Performance History</h4>
              </div>
              <div class="p-6">
                <div v-if="!employee.performanceHistory?.length" class="text-center py-8">
                  <Icon name="mdi:chart-line" class="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <p class="text-gray-500 dark:text-gray-400">No performance reviews yet</p>
                </div>
                <div v-else class="space-y-4">
                  <div 
                    v-for="review in employee.performanceHistory.slice(0, 3)" 
                    :key="review.id"
                    class="border border-gray-200 dark:border-gray-600 rounded-lg p-4"
                  >
                    <div class="flex justify-between items-start">
                      <div>
                        <p class="font-medium text-gray-900 dark:text-white">{{ review.period }}</p>
                        <p class="text-sm text-gray-600 dark:text-gray-400">Reviewed by {{ review.reviewerName }}</p>
                      </div>
                      <div class="text-right">
                        <p class="font-bold text-lg text-gray-900 dark:text-white">{{ review.rating }}</p>
                        <p class="text-sm text-gray-600 dark:text-gray-400">{{ formatDate(review.date) }}</p>
                      </div>
                    </div>
                    <p v-if="review.comments" class="mt-2 text-sm text-gray-700 dark:text-gray-300">
                      {{ review.comments }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Documents Tab -->
          <div v-if="activeTab === 'documents'" class="space-y-6">
            <div class="flex justify-between items-center">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">Employee Documents</h3>
              <button 
                @click="uploadDocument"
                class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700"
              >
                <Icon name="mdi:upload" class="w-4 h-4 mr-2" />
                Upload Document
              </button>
            </div>

            <!-- Document Categories -->
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <Icon name="mdi:card-account-details" class="w-6 h-6 text-blue-500 mr-2" />
                  <h4 class="font-medium text-gray-900 dark:text-white">Identity Documents</h4>
                </div>
                <div class="space-y-2 text-sm">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">SA ID Copy:</span>
                    <span class="text-green-600 dark:text-green-400">✓ Uploaded</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Proof of Address:</span>
                    <span class="text-yellow-600 dark:text-yellow-400">⚠ Pending</span>
                  </div>
                </div>
              </div>

              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <Icon name="mdi:briefcase" class="w-6 h-6 text-green-500 mr-2" />
                  <h4 class="font-medium text-gray-900 dark:text-white">Employment</h4>
                </div>
                <div class="space-y-2 text-sm">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Contract:</span>
                    <span class="text-green-600 dark:text-green-400">✓ Uploaded</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Job Description:</span>
                    <span class="text-green-600 dark:text-green-400">✓ Uploaded</span>
                  </div>
                </div>
              </div>

              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-6">
                <div class="flex items-center mb-4">
                  <Icon name="mdi:school" class="w-6 h-6 text-purple-500 mr-2" />
                  <h4 class="font-medium text-gray-900 dark:text-white">Qualifications</h4>
                </div>
                <div class="space-y-2 text-sm">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Matric Certificate:</span>
                    <span class="text-red-600 dark:text-red-400">✗ Missing</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">References:</span>
                    <span class="text-green-600 dark:text-green-400">✓ Uploaded</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Recent Documents -->
            <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg">
              <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
                <h4 class="font-medium text-gray-900 dark:text-white">Recent Documents</h4>
              </div>
              <div class="p-6">
                <div v-if="!employee.documents?.length" class="text-center py-8">
                  <Icon name="mdi:file-outline" class="w-12 h-12 text-gray-400 mx-auto mb-2" />
                  <p class="text-gray-500 dark:text-gray-400">No documents uploaded yet</p>
                </div>
                <div v-else class="space-y-3">
                  <div 
                    v-for="doc in employee.documents.slice(0, 5)" 
                    :key="doc.id"
                    class="flex items-center justify-between p-3 border border-gray-200 dark:border-gray-600 rounded"
                  >
                    <div class="flex items-center">
                      <Icon name="mdi:file" class="w-6 h-6 text-gray-400 mr-3" />
                      <div>
                        <p class="font-medium text-gray-900 dark:text-white">{{ doc.name }}</p>
                        <p class="text-sm text-gray-600 dark:text-gray-400">
                          Uploaded {{ formatDate(doc.uploadDate) }}
                        </p>
                      </div>
                    </div>
                    <div class="flex space-x-2">
                      <button class="text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300">
                        <Icon name="mdi:download" class="w-5 h-5" />
                      </button>
                      <button class="text-red-600 dark:text-red-400 hover:text-red-800 dark:hover:text-red-300">
                        <Icon name="mdi:delete" class="w-5 h-5" />
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { Employee } from '~/types/hr'

// Meta
definePageMeta({
  title: 'Employee Detail',
  layout: 'default'
})

// Route params
const route = useRoute()
const employeeId = route.params.id as string

// Composables
const {
  isLoading,
  isUpdating,
  error,
  employee,
  getEmployee,
  updateEmployee,
  formatCurrency,
  formatDate,
  calculateUIF,
  calculatePAYE
} = useHR()

// Local state
const activeTab = ref('personal')

const tabs = [
  { id: 'personal', name: 'Personal', icon: 'mdi:account' },
  { id: 'employment', name: 'Employment', icon: 'mdi:briefcase' },
  { id: 'leave', name: 'Leave', icon: 'mdi:calendar' },
  { id: 'performance', name: 'Performance', icon: 'mdi:chart-line' },
  { id: 'documents', name: 'Documents', icon: 'mdi:file-multiple' }
]

// Methods
const calculateAge = (dateOfBirth: string): number => {
  const birth = new Date(dateOfBirth)
  const today = new Date()
  let age = today.getFullYear() - birth.getFullYear()
  const monthDiff = today.getMonth() - birth.getMonth()
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
    age--
  }
  
  return age
}

const calculateYearsOfService = (startDate: string): string => {
  const start = new Date(startDate)
  const today = new Date()
  const years = today.getFullYear() - start.getFullYear()
  const months = today.getMonth() - start.getMonth()
  
  if (years === 0) {
    return `${months} months`
  } else if (months >= 0) {
    return `${years} years`
  } else {
    return `${years - 1} years, ${12 + months} months`
  }
}

const getLanguageName = (code: string): string => {
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

const toggleEmployeeStatus = async () => {
  if (!employee.value) return
  
  const newStatus = employee.value.status === 'active' ? 'inactive' : 'active'
  const confirmed = confirm(
    `Are you sure you want to ${newStatus === 'active' ? 'activate' : 'deactivate'} ${employee.value.personalInfo.firstName} ${employee.value.personalInfo.lastName}?`
  )
  
  if (confirmed) {
    try {
      await updateEmployee(employeeId, { status: newStatus })
      // Refresh employee data
      await getEmployee(employeeId)
    } catch (err) {
      console.error('Failed to update employee status:', err)
    }
  }
}

// Placeholder methods for future implementation
const requestLeave = () => {
  console.log('Request leave functionality to be implemented')
}

const addPerformanceReview = () => {
  console.log('Add performance review functionality to be implemented')
}

const uploadDocument = () => {
  console.log('Upload document functionality to be implemented')
}

// Lifecycle
onMounted(async () => {
  try {
    await getEmployee(employeeId)
  } catch (err) {
    console.error('Failed to load employee:', err)
  }
})
</script>

<script lang="ts">
// Status Badge Component (reused from employee list)
const StatusBadge = defineComponent({
  props: {
    status: {
      type: String as PropType<Employee['status']>,
      required: true
    }
  },
  setup(props) {
    const statusConfig = computed(() => {
      switch (props.status) {
        case 'active':
          return { class: 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200', label: 'Active' }
        case 'inactive':
          return { class: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200', label: 'Inactive' }
        case 'terminated':
          return { class: 'bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200', label: 'Terminated' }
        case 'suspended':
          return { class: 'bg-yellow-100 dark:bg-yellow-900 text-yellow-800 dark:text-yellow-200', label: 'Suspended' }
        default:
          return { class: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200', label: 'Unknown' }
      }
    })

    return () => h('span', {
      class: `inline-flex px-2 py-1 text-xs font-semibold rounded-full ${statusConfig.value.class}`
    }, statusConfig.value.label)
  }
})
</script>

<style scoped>
.employee-detail {
  @apply p-6 max-w-7xl mx-auto;
}
</style>