<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Accounts</p>
      <h1 class="text-xl font-semibold">Cashbook</h1>
    </div>

    <div class="card-surface p-4">
      <p class="text-sm text-muted-foreground">
        Cashbook entries will appear here. Hook to backend ledger endpoints when available.
      </p>
      <ul class="mt-3 space-y-2 text-sm">
        <li class="flex items-center gap-2 text-success">
          <span class="material-symbols-rounded text-base">arrow_downward</span>
          R 2,300 cash sale
        </li>
        <li class="flex items-center gap-2 text-destructive">
          <span class="material-symbols-rounded text-base">arrow_upward</span>
          R 450 expense: fuel
        </li>
      </ul>
    </div>
  </div>
</template>

