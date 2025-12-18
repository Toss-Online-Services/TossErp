<template>
  <button
    type="button"
    class="md-social-button"
    :class="[
      `md-social-button-${provider}`,
      `md-social-button-${size}`,
      { 'md-social-button-icon-only': iconOnly }
    ]"
    :disabled="disabled"
    @click="handleClick"
  >
    <Icon :name="iconName" :size="iconSize" class="md-social-button-icon" />
    <span v-if="!iconOnly" class="md-social-button-text">
      {{ text || `Sign in with ${providerLabel}` }}
    </span>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

type SocialProvider = 'google' | 'facebook' | 'twitter' | 'github' | 'linkedin' | 'apple' | 'microsoft' | 'instagram'

interface Props {
  provider: SocialProvider
  text?: string
  size?: 'sm' | 'md' | 'lg'
  iconOnly?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  iconOnly: false,
  disabled: false
})

const emit = defineEmits<{
  click: []
}>()

const providerConfig: Record<SocialProvider, { icon: string; label: string }> = {
  google: { icon: 'mdi:google', label: 'Google' },
  facebook: { icon: 'mdi:facebook', label: 'Facebook' },
  twitter: { icon: 'mdi:twitter', label: 'Twitter' },
  github: { icon: 'mdi:github', label: 'GitHub' },
  linkedin: { icon: 'mdi:linkedin', label: 'LinkedIn' },
  apple: { icon: 'mdi:apple', label: 'Apple' },
  microsoft: { icon: 'mdi:microsoft', label: 'Microsoft' },
  instagram: { icon: 'mdi:instagram', label: 'Instagram' }
}

const iconName = computed(() => providerConfig[props.provider].icon)
const providerLabel = computed(() => providerConfig[props.provider].label)

const iconSize = computed(() => {
  switch (props.size) {
    case 'sm': return '16'
    case 'lg': return '24'
    default: return '20'
  }
})

const handleClick = () => {
  if (!props.disabled) {
    emit('click')
  }
}
</script>

<style scoped>
.md-social-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-weight: 500;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
  white-space: nowrap;
}

.md-social-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Sizes */
.md-social-button-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
}

.md-social-button-sm.md-social-button-icon-only {
  padding: 0.375rem;
  width: 2rem;
  height: 2rem;
}

.md-social-button-md {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
}

.md-social-button-md.md-social-button-icon-only {
  padding: 0.5rem;
  width: 2.5rem;
  height: 2.5rem;
}

.md-social-button-lg {
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
}

.md-social-button-lg.md-social-button-icon-only {
  padding: 0.75rem;
  width: 3rem;
  height: 3rem;
}

/* Google */
.md-social-button-google {
  background-color: #fff;
  color: #757575;
  border: 1px solid #dadce0;
}

.md-social-button-google:hover:not(:disabled) {
  background-color: #f8f9fa;
  box-shadow: 0 1px 2px 0 rgba(60,64,67,.3), 0 1px 3px 1px rgba(60,64,67,.15);
}

.md-social-button-google .md-social-button-icon {
  color: #4285f4;
}

/* Facebook */
.md-social-button-facebook {
  background-color: #1877f2;
  color: white;
}

.md-social-button-facebook:hover:not(:disabled) {
  background-color: #166fe5;
  box-shadow: 0 4px 8px rgba(24, 119, 242, 0.3);
}

/* Twitter */
.md-social-button-twitter {
  background-color: #1da1f2;
  color: white;
}

.md-social-button-twitter:hover:not(:disabled) {
  background-color: #1a94da;
  box-shadow: 0 4px 8px rgba(29, 161, 242, 0.3);
}

/* GitHub */
.md-social-button-github {
  background-color: #24292e;
  color: white;
}

.md-social-button-github:hover:not(:disabled) {
  background-color: #1b1f23;
  box-shadow: 0 4px 8px rgba(36, 41, 46, 0.3);
}

/* LinkedIn */
.md-social-button-linkedin {
  background-color: #0077b5;
  color: white;
}

.md-social-button-linkedin:hover:not(:disabled) {
  background-color: #006399;
  box-shadow: 0 4px 8px rgba(0, 119, 181, 0.3);
}

/* Apple */
.md-social-button-apple {
  background-color: #000;
  color: white;
}

.md-social-button-apple:hover:not(:disabled) {
  background-color: #1a1a1a;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}

/* Microsoft */
.md-social-button-microsoft {
  background-color: #fff;
  color: #5e5e5e;
  border: 1px solid #8c8c8c;
}

.md-social-button-microsoft:hover:not(:disabled) {
  background-color: #f3f3f3;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.md-social-button-microsoft .md-social-button-icon {
  color: #f25022;
}

/* Instagram */
.md-social-button-instagram {
  background: linear-gradient(45deg, #f09433 0%, #e6683c 25%, #dc2743 50%, #cc2366 75%, #bc1888 100%);
  color: white;
}

.md-social-button-instagram:hover:not(:disabled) {
  box-shadow: 0 4px 12px rgba(188, 24, 136, 0.4);
  transform: translateY(-1px);
}

/* Icon */
.md-social-button-icon {
  flex-shrink: 0;
}

.md-social-button-text {
  font-weight: 500;
}
</style>
