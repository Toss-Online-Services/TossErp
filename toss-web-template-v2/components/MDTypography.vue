<template>
  <component 
    :is="tag || 'span'" 
    :class="typographyClasses" 
    :style="typographyStyles"
  >
    <slot />
  </component>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  tag?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'p' | 'span' | 'div'
  variant?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'body1' | 'body2' | 'button' | 'caption' | 'overline'
  color?: 'inherit' | 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark' | 'text' | 'white'
  fontWeight?: 'light' | 'regular' | 'medium' | 'bold' | false
  textTransform?: 'none' | 'capitalize' | 'uppercase' | 'lowercase'
  verticalAlign?: 'unset' | 'baseline' | 'sub' | 'super' | 'text-top' | 'text-bottom' | 'middle' | 'top' | 'bottom'
  textGradient?: boolean
  opacity?: number
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'body1',
  color: 'dark',
  fontWeight: false,
  textTransform: 'none',
  verticalAlign: 'unset',
  textGradient: false,
  opacity: 1
})

const typographyClasses = computed(() => {
  const classes = ['md-typography']
  
  if (props.variant) {
    classes.push(`md-typography-${props.variant}`)
  }
  
  if (props.textGradient) {
    classes.push('md-typography-gradient', `md-typography-gradient-${props.color}`)
  } else if (props.color && props.color !== 'inherit') {
    classes.push(`md-typography-color-${props.color}`)
  }
  
  if (props.fontWeight) {
    classes.push(`md-typography-weight-${props.fontWeight}`)
  }
  
  if (props.textTransform !== 'none') {
    classes.push(`md-typography-transform-${props.textTransform}`)
  }
  
  if (props.verticalAlign !== 'unset') {
    classes.push(`md-typography-valign-${props.verticalAlign}`)
  }
  
  return classes.join(' ')
})

const typographyStyles = computed(() => {
  const styles: Record<string, any> = {}
  
  if (props.opacity !== 1) {
    styles.opacity = props.opacity
  }
  
  return styles
})
</script>

<style scoped>
.md-typography {
  font-family: 'Roboto', sans-serif;
  margin: 0;
}

/* Variants */
.md-typography-h1 {
  font-size: 3.052rem;
  line-height: 1.25;
  font-weight: 300;
  letter-spacing: -0.01562rem;
}

.md-typography-h2 {
  font-size: 2.441rem;
  line-height: 1.3;
  font-weight: 300;
  letter-spacing: -0.00833rem;
}

.md-typography-h3 {
  font-size: 1.953rem;
  line-height: 1.375;
  font-weight: 300;
  letter-spacing: 0;
}

.md-typography-h4 {
  font-size: 1.563rem;
  line-height: 1.375;
  font-weight: 400;
  letter-spacing: 0.00735rem;
}

.md-typography-h5 {
  font-size: 1.25rem;
  line-height: 1.375;
  font-weight: 400;
  letter-spacing: 0;
}

.md-typography-h6 {
  font-size: 1rem;
  line-height: 1.625;
  font-weight: 600;
  letter-spacing: 0.0075rem;
}

.md-typography-body1 {
  font-size: 1rem;
  line-height: 1.625;
  font-weight: 400;
  letter-spacing: 0.03125rem;
}

.md-typography-body2 {
  font-size: 0.875rem;
  line-height: 1.57143;
  font-weight: 400;
  letter-spacing: 0.00714rem;
}

.md-typography-button {
  font-size: 0.875rem;
  line-height: 1.75;
  font-weight: 700;
  letter-spacing: 0.02857rem;
  text-transform: uppercase;
}

.md-typography-caption {
  font-size: 0.75rem;
  line-height: 1.66667;
  font-weight: 400;
  letter-spacing: 0.03333rem;
}

.md-typography-overline {
  font-size: 0.75rem;
  line-height: 2.66667;
  font-weight: 400;
  letter-spacing: 0.08333rem;
  text-transform: uppercase;
}

/* Colors */
.md-typography-color-primary {
  color: var(--md-primary) !important;
}

.md-typography-color-secondary {
  color: var(--md-secondary) !important;
}

.md-typography-color-info {
  color: var(--md-info) !important;
}

.md-typography-color-success {
  color: var(--md-success) !important;
}

.md-typography-color-warning {
  color: var(--md-warning) !important;
}

.md-typography-color-error {
  color: var(--md-error) !important;
}

.md-typography-color-light {
  color: rgb(248, 249, 250) !important;
}

.md-typography-color-dark {
  color: rgb(52, 71, 103) !important;
}

.md-typography-color-text {
  color: rgb(123, 128, 154) !important;
}

.md-typography-color-white {
  color: white !important;
}

/* Font weights */
.md-typography-weight-light {
  font-weight: 300 !important;
}

.md-typography-weight-regular {
  font-weight: 400 !important;
}

.md-typography-weight-medium {
  font-weight: 600 !important;
}

.md-typography-weight-bold {
  font-weight: 700 !important;
}

/* Text transform */
.md-typography-transform-capitalize {
  text-transform: capitalize !important;
}

.md-typography-transform-uppercase {
  text-transform: uppercase !important;
}

.md-typography-transform-lowercase {
  text-transform: lowercase !important;
}

/* Vertical align */
.md-typography-valign-baseline {
  vertical-align: baseline !important;
}

.md-typography-valign-sub {
  vertical-align: sub !important;
}

.md-typography-valign-super {
  vertical-align: super !important;
}

.md-typography-valign-text-top {
  vertical-align: text-top !important;
}

.md-typography-valign-text-bottom {
  vertical-align: text-bottom !important;
}

.md-typography-valign-middle {
  vertical-align: middle !important;
}

.md-typography-valign-top {
  vertical-align: top !important;
}

.md-typography-valign-bottom {
  vertical-align: bottom !important;
}

/* Gradient text */
.md-typography-gradient {
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  position: relative;
  z-index: 1;
}

.md-typography-gradient-primary {
  background-image: var(--md-gradient-primary);
}

.md-typography-gradient-secondary {
  background-image: var(--md-gradient-secondary);
}

.md-typography-gradient-info {
  background-image: var(--md-gradient-info);
}

.md-typography-gradient-success {
  background-image: var(--md-gradient-success);
}

.md-typography-gradient-warning {
  background-image: var(--md-gradient-warning);
}

.md-typography-gradient-error {
  background-image: var(--md-gradient-error);
}

.md-typography-gradient-dark {
  background-image: var(--md-gradient-dark);
}
</style>
