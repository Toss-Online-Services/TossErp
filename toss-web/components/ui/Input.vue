<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    modelValue?: string | number
    type?: string
    placeholder?: string
    disabled?: boolean
    class?: string
  }>(),
  {
    type: 'text',
    placeholder: '',
    disabled: false,
    class: ''
  }
)

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>

<template>
  <input
    :type="type"
    :value="modelValue"
    :placeholder="placeholder"
    :disabled="disabled"
    :class="[
      'flex h-10 w-full rounded-[var(--ct-radius)] border border-[hsl(var(--ct-border))] bg-[hsl(var(--ct-surface-variant))] px-3 py-2 text-sm text-[hsl(var(--ct-on-surface))] placeholder:text-[hsl(var(--ct-secondary))] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[hsl(var(--ct-primary))] focus-visible:border-transparent disabled:cursor-not-allowed disabled:opacity-70 transition-colors',
      props.class
    ]"
    @input="onInput"
  />
</template>


