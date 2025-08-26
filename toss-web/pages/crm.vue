<template>
  <div class="min-h-screen bg-gray-50">
    <!-- CRM Header -->
    <div class="bg-white shadow-sm border-b border-gray-200">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center py-6">
          <div>
            <h1 class="text-3xl font-bold text-gray-900">Customer Relationship Management</h1>
            <p class="mt-1 text-gray-600">Manage your leads, customers, and sales pipeline</p>
          </div>
        </div>
        
        <!-- Navigation Tabs -->
        <div class="border-t border-gray-200">
          <nav class="-mb-px flex space-x-8">
            <NuxtLink 
              to="/crm/pipeline" 
              class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
              active-class="border-blue-500 text-blue-600"
            >
              Sales Pipeline
            </NuxtLink>
            <NuxtLink 
              to="/crm/leads" 
              class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
              active-class="border-blue-500 text-blue-600"
            >
              Leads
            </NuxtLink>
            <NuxtLink 
              to="/crm/customers" 
              class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
              active-class="border-blue-500 text-blue-600"
            >
              Customers
            </NuxtLink>
            <NuxtLink 
              to="/crm/opportunities" 
              class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
              active-class="border-blue-500 text-blue-600"
            >
              Opportunities
            </NuxtLink>
            <NuxtLink 
              to="/crm/contacts" 
              class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
              active-class="border-blue-500 text-blue-600"
            >
              Contacts
            </NuxtLink>
          </nav>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <NuxtPage />
    </div>
  </div>
</template>
