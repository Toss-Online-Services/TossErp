<template>
  <nav aria-label="Pagination">
    <ul class="pagination" :class="paginationClasses">
      <li class="page-item" :class="{ disabled: currentPage === 1 }">
        <a 
          class="page-link" 
          href="#"
          @click.prevent="changePage(currentPage - 1)"
          aria-label="Previous"
        >
          <Icon name="mdi:chevron-left" size="18" />
        </a>
      </li>
      
      <li 
        v-for="page in displayPages" 
        :key="page"
        class="page-item"
        :class="{ active: page === currentPage }"
      >
        <a 
          v-if="page !== '...'"
          class="page-link" 
          href="#"
          @click.prevent="changePage(page as number)"
        >
          {{ page }}
        </a>
        <span v-else class="page-link">...</span>
      </li>
      
      <li class="page-item" :class="{ disabled: currentPage === totalPages }">
        <a 
          class="page-link" 
          href="#"
          @click.prevent="changePage(currentPage + 1)"
          aria-label="Next"
        >
          <Icon name="mdi:chevron-right" size="18" />
        </a>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  currentPage?: number
  totalPages?: number
  color?: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark'
  size?: 'small' | 'medium' | 'large'
  variant?: 'contained' | 'gradient'
}

const props = withDefaults(defineProps<Props>(), {
  currentPage: 1,
  totalPages: 1,
  color: 'info',
  size: 'medium',
  variant: 'gradient'
})

const emit = defineEmits<{
  (e: 'update:currentPage', page: number): void
}>()

const paginationClasses = computed(() => {
  const classes: string[] = []
  
  if (props.size === 'small') {
    classes.push('pagination-sm')
  } else if (props.size === 'large') {
    classes.push('pagination-lg')
  }
  
  classes.push(`pagination-${props.color}`)
  
  if (props.variant === 'gradient') {
    classes.push('pagination-gradient')
  }
  
  return classes
})

const displayPages = computed(() => {
  const pages: (number | string)[] = []
  const maxVisible = 5
  
  if (props.totalPages <= maxVisible) {
    for (let i = 1; i <= props.totalPages; i++) {
      pages.push(i)
    }
  } else {
    pages.push(1)
    
    if (props.currentPage > 3) {
      pages.push('...')
    }
    
    const start = Math.max(2, props.currentPage - 1)
    const end = Math.min(props.totalPages - 1, props.currentPage + 1)
    
    for (let i = start; i <= end; i++) {
      if (!pages.includes(i)) {
        pages.push(i)
      }
    }
    
    if (props.currentPage < props.totalPages - 2) {
      pages.push('...')
    }
    
    if (!pages.includes(props.totalPages)) {
      pages.push(props.totalPages)
    }
  }
  
  return pages
})

const changePage = (page: number) => {
  if (page >= 1 && page <= props.totalPages && page !== props.currentPage) {
    emit('update:currentPage', page)
  }
}
</script>

<style scoped>
.pagination {
  display: flex;
  padding-left: 0;
  list-style: none;
  border-radius: 0.5rem;
  margin: 0;
}

.page-item {
  margin: 0 0.125rem;
}

.page-link {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #344767;
  text-decoration: none;
  background-color: transparent;
  border: 0;
  border-radius: 0.375rem;
  transition: all 0.15s ease-in;
  cursor: pointer;
  min-width: 2.25rem;
  height: 2.25rem;
}

.page-link:hover {
  background-color: #f0f2f5;
  color: #344767;
}

.page-item.active .page-link {
  z-index: 3;
  color: white;
  box-shadow: 0 3px 5px -1px rgba(0, 0, 0, 0.09), 0 2px 3px -1px rgba(0, 0, 0, 0.07);
}

.page-item.disabled .page-link {
  color: #9ca3af;
  pointer-events: none;
  cursor: default;
  opacity: 0.6;
}

/* Size variants */
.pagination-sm .page-link {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
  min-width: 1.75rem;
  height: 1.75rem;
}

.pagination-lg .page-link {
  padding: 0.75rem 1rem;
  font-size: 1rem;
  min-width: 2.75rem;
  height: 2.75rem;
}

/* Color variants - Gradient */
.pagination-gradient.pagination-info .page-item.active .page-link {
  background: linear-gradient(310deg, var(--md-gradient-info-main, #2152ff), var(--md-gradient-info-state, #21d4fd));
}

.pagination-gradient.pagination-primary .page-item.active .page-link {
  background: linear-gradient(310deg, var(--md-gradient-primary-main, #7928ca), var(--md-gradient-primary-state, #ff0080));
}

.pagination-gradient.pagination-success .page-item.active .page-link {
  background: linear-gradient(310deg, var(--md-gradient-success-main, #17ad37), var(--md-gradient-success-state, #98ec2d));
}

.pagination-gradient.pagination-warning .page-item.active .page-link {
  background: linear-gradient(310deg, var(--md-gradient-warning-main, #f53939), var(--md-gradient-warning-state, #fbcf33));
}

.pagination-gradient.pagination-error .page-item.active .page-link {
  background: linear-gradient(310deg, var(--md-gradient-error-main, #ea0606), var(--md-gradient-error-state, #ff667c));
}

/* Color variants - Contained */
.pagination-info .page-item.active .page-link {
  background-color: var(--md-info, #1a73e8);
}

.pagination-primary .page-item.active .page-link {
  background-color: var(--md-primary, #e91e63);
}

.pagination-success .page-item.active .page-link {
  background-color: var(--md-success, #4caf50);
}

.pagination-warning .page-item.active .page-link {
  background-color: var(--md-warning, #fb8c00);
}

.pagination-error .page-item.active .page-link {
  background-color: var(--md-error, #f44335);
}
</style>
