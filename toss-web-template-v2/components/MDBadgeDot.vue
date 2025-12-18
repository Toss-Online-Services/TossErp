<template>
  <span 
    class="md-badge-dot"
    :class="[
      `md-badge-dot-${color}`,
      `md-badge-dot-${size}`,
      { 'md-badge-dot-border': border }
    ]"
  >
    <span class="md-badge-dot-inner" />
  </span>
</template>

<script setup lang="ts">
interface Props {
  color?: 'primary' | 'secondary' | 'success' | 'error' | 'warning' | 'info' | 'light' | 'dark'
  size?: 'sm' | 'md' | 'lg'
  border?: boolean
}

withDefaults(defineProps<Props>(), {
  color: 'primary',
  size: 'md',
  border: false
})
</script>

<style scoped>
.md-badge-dot {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.md-badge-dot-inner {
  display: block;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.md-badge-dot-border .md-badge-dot-inner {
  box-shadow: 0 0 0 2px rgba(255, 255, 255, 1);
}

/* Sizes */
.md-badge-dot-sm .md-badge-dot-inner {
  width: 6px;
  height: 6px;
}

.md-badge-dot-md .md-badge-dot-inner {
  width: 8px;
  height: 8px;
}

.md-badge-dot-lg .md-badge-dot-inner {
  width: 10px;
  height: 10px;
}

/* Colors */
.md-badge-dot-primary .md-badge-dot-inner {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.md-badge-dot-secondary .md-badge-dot-inner {
  background: linear-gradient(195deg, #627594 0%, #a8b8d8 100%);
}

.md-badge-dot-success .md-badge-dot-inner {
  background: linear-gradient(195deg, #66BB6A 0%, #43A047 100%);
}

.md-badge-dot-error .md-badge-dot-inner {
  background: linear-gradient(195deg, #EF5350 0%, #E53935 100%);
}

.md-badge-dot-warning .md-badge-dot-inner {
  background: linear-gradient(195deg, #FFA726 0%, #FB8C00 100%);
}

.md-badge-dot-info .md-badge-dot-inner {
  background: linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%);
}

.md-badge-dot-light .md-badge-dot-inner {
  background: #f0f2f5;
}

.md-badge-dot-dark .md-badge-dot-inner {
  background: linear-gradient(195deg, #42424a 0%, #191919 100%);
}

/* Pulse animation */
@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 currentColor;
  }
  
  70% {
    box-shadow: 0 0 0 4px transparent;
  }
  
  100% {
    box-shadow: 0 0 0 0 transparent;
  }
}

.md-badge-dot-primary {
  color: rgba(94, 114, 228, 0.4);
}

.md-badge-dot-secondary {
  color: rgba(98, 117, 148, 0.4);
}

.md-badge-dot-success {
  color: rgba(102, 187, 106, 0.4);
}

.md-badge-dot-error {
  color: rgba(239, 83, 80, 0.4);
}

.md-badge-dot-warning {
  color: rgba(255, 167, 38, 0.4);
}

.md-badge-dot-info {
  color: rgba(26, 115, 232, 0.4);
}

.md-badge-dot-light {
  color: rgba(240, 242, 245, 0.4);
}

.md-badge-dot-dark {
  color: rgba(66, 66, 74, 0.4);
}
</style>
