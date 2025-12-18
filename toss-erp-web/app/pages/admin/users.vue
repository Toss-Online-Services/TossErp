<template>
  <div class="min-vh-100 bg-gray-100 g-sidenav-show">
    <!-- Page Header -->
    <div class="py-4">
      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-auto">
            <div class="mb-0">
              <p class="text-sm text-muted-foreground">Admin</p>
              <h1 class="text-3xl font-bold">Users & Roles</h1>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Users List -->
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12 col-lg-8">
          <MaterialCard variant="default">
            <div class="card-body">
              <h5 class="card-title mb-4">Team Members</h5>
              <div class="space-y-3">
                <div class="flex items-center justify-between p-4 border border-slate-200 dark:border-slate-700 rounded-lg hover:bg-gray-50 dark:hover:bg-slate-700/30 transition-colors">
                  <div class="flex items-center gap-4">
                    <div class="p-2 bg-primary/10 rounded-lg">
                      <span class="material-symbols-rounded text-primary">person</span>
                    </div>
                    <div>
                      <p class="font-semibold text-slate-900 dark:text-white">Naledi</p>
                      <p class="text-sm text-slate-600 dark:text-slate-400">Owner</p>
                    </div>
                  </div>
                  <span class="inline-flex px-3 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">Active</span>
                </div>
                <div class="flex items-center justify-between p-4 border border-slate-200 dark:border-slate-700 rounded-lg hover:bg-gray-50 dark:hover:bg-slate-700/30 transition-colors">
                  <div class="flex items-center gap-4">
                    <div class="p-2 bg-primary/10 rounded-lg">
                      <span class="material-symbols-rounded text-primary">person</span>
                    </div>
                    <div>
                      <p class="font-semibold text-slate-900 dark:text-white">Thabo</p>
                      <p class="text-sm text-slate-600 dark:text-slate-400">Cashier</p>
                    </div>
                  </div>
                  <span class="inline-flex px-3 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">Active</span>
                </div>
              </div>
            </div>
          </MaterialCard>
        </div>
      </div>
    </div>
  </div>
</template>

