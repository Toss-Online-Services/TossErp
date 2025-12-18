<template>
  <img 
    :src="src" 
    :alt="alt" 
    :class="avatarClasses"
    :style="avatarStyles"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  src: string
  alt?: string
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl'
  shadow?: 'none' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl'
  bgColor?: string
}

const props = withDefaults(defineProps<Props>(), {
  alt: '',
  size: 'md',
  shadow: 'none',
  bgColor: 'transparent'
})

const avatarClasses = computed(() => {
  const classes = ['md-avatar', `md-avatar-${props.size}`]
  
  if (props.shadow && props.shadow !== 'none') {
    classes.push(`md-avatar-shadow-${props.shadow}`)
  }
  
  return classes.join(' ')
})

const avatarStyles = computed(() => {
  const styles: Record<string, any> = {}
  
  if (props.bgColor !== 'transparent') {
    styles.backgroundColor = `var(--md-${props.bgColor}, ${props.bgColor})`
  }
  
  return styles
})
</script>

<style scoped>
.md-avatar {
  display: inline-block;
  border-radius: 0.75rem;
  object-fit: cover;
}

/* Sizes */
.md-avatar-xs {
  width: 1.5rem;
  height: 1.5rem;
}

.md-avatar-sm {
  width: 2.25rem;
  height: 2.25rem;
}

.md-avatar-md {
  width: 3rem;
  height: 3rem;
}

.md-avatar-lg {
  width: 4.125rem;
  height: 4.125rem;
}

.md-avatar-xl {
  width: 5.25rem;
  height: 5.25rem;
}

.md-avatar-xxl {
  width: 7.125rem;
  height: 7.125rem;
}

/* Shadows */
.md-avatar-shadow-sm {
  box-shadow: var(--md-box-shadow-sm);
}

.md-avatar-shadow-md {
  box-shadow: var(--md-box-shadow-md);
}

.md-avatar-shadow-lg {
  box-shadow: var(--md-box-shadow-lg);
}

.md-avatar-shadow-xl {
  box-shadow: var(--md-box-shadow-xl);
}

.md-avatar-shadow-xxl {
  box-shadow: var(--md-box-shadow-xxl);
}
</style>
