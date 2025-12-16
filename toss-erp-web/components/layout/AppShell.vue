<template>
  <div class="min-h-screen bg-muted/40 text-foreground flex">
    <aside class="hidden lg:block w-72 shrink-0 border-r border-border bg-card">
      <slot name="sidebar" />
    </aside>

    <div class="flex-1 flex min-h-screen flex-col">
      <header class="border-b border-border bg-card/70 backdrop-blur supports-[backdrop-filter]:bg-card/60 sticky top-0 z-20">
        <slot name="topbar" />
      </header>
      <main class="flex-1 p-4 md:p-6 lg:p-8">
        <slot />
      </main>
    </div>
  </div>
</template>

