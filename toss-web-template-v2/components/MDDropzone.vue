<template>
  <div 
    class="md-dropzone"
    :class="{ 
      'md-dropzone-dragging': isDragging,
      'md-dropzone-disabled': disabled 
    }"
    @drop="handleDrop"
    @dragover="handleDragOver"
    @dragenter="handleDragEnter"
    @dragleave="handleDragLeave"
  >
    <input
      ref="fileInput"
      type="file"
      :multiple="multiple"
      :accept="accept"
      :disabled="disabled"
      style="display: none;"
      @change="handleFileSelect"
    />
    
    <div v-if="files.length === 0" class="md-dropzone-content" @click="openFileDialog">
      <Icon name="mdi:cloud-upload-outline" size="48" class="md-dropzone-icon" />
      <MDTypography variant="h6" font-weight="medium" class="mb-1">
        {{ title }}
      </MDTypography>
      <MDTypography variant="caption" color="text">
        {{ subtitle }}
      </MDTypography>
      <MDButton color="primary" size="sm" class="mt-3">
        <Icon name="mdi:upload" size="18" class="me-1" />
        Browse Files
      </MDButton>
    </div>
    
    <div v-else class="md-dropzone-files">
      <div 
        v-for="(file, index) in files"
        :key="index"
        class="md-dropzone-file"
      >
        <div class="md-dropzone-file-preview">
          <img 
            v-if="isImage(file) && file.preview"
            :src="file.preview"
            :alt="file.name"
            class="md-dropzone-image"
          />
          <div v-else class="md-dropzone-file-icon">
            <Icon :name="getFileIcon(file)" size="32" />
          </div>
        </div>
        
        <div class="md-dropzone-file-info">
          <MDTypography variant="button" font-weight="medium" class="mb-0">
            {{ file.name }}
          </MDTypography>
          <MDTypography variant="caption" color="text">
            {{ formatFileSize(file.size) }}
          </MDTypography>
          
          <div v-if="file.uploading" class="md-dropzone-progress mt-2">
            <MDProgress :value="file.progress" color="success" />
            <MDTypography variant="caption" color="text" class="mt-1">
              {{ file.progress }}% uploaded
            </MDTypography>
          </div>
          
          <div v-if="file.error" class="mt-2">
            <MDTypography variant="caption" color="error">
              {{ file.error }}
            </MDTypography>
          </div>
        </div>
        
        <div class="md-dropzone-file-actions">
          <button
            v-if="!file.uploading"
            type="button"
            class="md-dropzone-remove"
            @click="removeFile(index)"
          >
            <Icon name="mdi:close" size="20" />
          </button>
        </div>
      </div>
      
      <MDButton 
        v-if="multiple"
        color="secondary" 
        size="sm" 
        variant="outlined"
        class="mt-3"
        @click="openFileDialog"
      >
        <Icon name="mdi:plus" size="18" class="me-1" />
        Add More Files
      </MDButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface FileWithPreview extends File {
  preview?: string
  uploading?: boolean
  progress?: number
  error?: string
}

interface Props {
  modelValue?: FileWithPreview[]
  multiple?: boolean
  accept?: string
  maxSize?: number // in bytes
  maxFiles?: number
  disabled?: boolean
  title?: string
  subtitle?: string
  autoUpload?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  multiple: false,
  accept: '*',
  maxSize: 10485760, // 10MB
  maxFiles: 10,
  disabled: false,
  title: 'Drop files here to upload',
  subtitle: 'or click to browse',
  autoUpload: false
})

const emit = defineEmits<{
  'update:modelValue': [files: FileWithPreview[]]
  'upload': [file: FileWithPreview]
  'error': [error: string]
}>()

const fileInput = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)
const files = ref<FileWithPreview[]>(props.modelValue)

const openFileDialog = () => {
  if (props.disabled) return
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) {
    addFiles(Array.from(target.files))
  }
}

const handleDragEnter = (event: DragEvent) => {
  event.preventDefault()
  if (props.disabled) return
  isDragging.value = true
}

const handleDragOver = (event: DragEvent) => {
  event.preventDefault()
}

const handleDragLeave = (event: DragEvent) => {
  event.preventDefault()
  isDragging.value = false
}

const handleDrop = (event: DragEvent) => {
  event.preventDefault()
  isDragging.value = false
  
  if (props.disabled) return
  
  const droppedFiles = event.dataTransfer?.files
  if (droppedFiles) {
    addFiles(Array.from(droppedFiles))
  }
}

const addFiles = (newFiles: File[]) => {
  const validFiles: FileWithPreview[] = []
  
  for (const file of newFiles) {
    // Check file size
    if (file.size > props.maxSize) {
      emit('error', `File ${file.name} exceeds maximum size of ${formatFileSize(props.maxSize)}`)
      continue
    }
    
    // Check max files
    if (files.value.length + validFiles.length >= props.maxFiles) {
      emit('error', `Maximum ${props.maxFiles} files allowed`)
      break
    }
    
    // Create preview for images
    const fileWithPreview: FileWithPreview = file as FileWithPreview
    if (isImage(file)) {
      const reader = new FileReader()
      reader.onload = (e) => {
        fileWithPreview.preview = e.target?.result as string
      }
      reader.readAsDataURL(file)
    }
    
    validFiles.push(fileWithPreview)
    
    // Auto upload if enabled
    if (props.autoUpload) {
      uploadFile(fileWithPreview)
    }
  }
  
  if (!props.multiple) {
    files.value = validFiles.slice(0, 1)
  } else {
    files.value = [...files.value, ...validFiles]
  }
  
  emit('update:modelValue', files.value)
}

const removeFile = (index: number) => {
  files.value.splice(index, 1)
  emit('update:modelValue', files.value)
}

const uploadFile = (file: FileWithPreview) => {
  file.uploading = true
  file.progress = 0
  
  // Simulate upload progress
  const interval = setInterval(() => {
    if (file.progress! < 100) {
      file.progress! += 10
    } else {
      clearInterval(interval)
      file.uploading = false
      emit('upload', file)
    }
  }, 200)
}

const isImage = (file: File): boolean => {
  return file.type.startsWith('image/')
}

const getFileIcon = (file: File): string => {
  const type = file.type
  
  if (type.includes('pdf')) return 'mdi:file-pdf-box'
  if (type.includes('word') || type.includes('document')) return 'mdi:file-word'
  if (type.includes('excel') || type.includes('spreadsheet')) return 'mdi:file-excel'
  if (type.includes('powerpoint') || type.includes('presentation')) return 'mdi:file-powerpoint'
  if (type.includes('zip') || type.includes('rar')) return 'mdi:folder-zip'
  if (type.includes('video')) return 'mdi:file-video'
  if (type.includes('audio')) return 'mdi:file-music'
  
  return 'mdi:file-document'
}

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}
</script>

<style scoped>
.md-dropzone {
  border: 2px dashed #d2d6da;
  border-radius: 0.75rem;
  padding: 2rem;
  text-align: center;
  transition: all 0.3s;
  background-color: #f8f9fa;
  cursor: pointer;
}

.md-dropzone:hover:not(.md-dropzone-disabled) {
  border-color: #5e72e4;
  background-color: rgba(94, 114, 228, 0.05);
}

.md-dropzone-dragging {
  border-color: #5e72e4;
  background-color: rgba(94, 114, 228, 0.1);
  transform: scale(1.02);
}

.md-dropzone-disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.md-dropzone-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.md-dropzone-icon {
  color: #67748e;
  margin-bottom: 0.5rem;
}

.md-dropzone-files {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  text-align: left;
}

.md-dropzone-file {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 0.5rem;
  border: 1px solid #e9ecef;
}

.md-dropzone-file-preview {
  flex-shrink: 0;
  width: 60px;
  height: 60px;
  border-radius: 0.375rem;
  overflow: hidden;
  background-color: #f8f9fa;
  display: flex;
  align-items: center;
  justify-content: center;
}

.md-dropzone-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.md-dropzone-file-icon {
  color: #67748e;
}

.md-dropzone-file-info {
  flex: 1;
  min-width: 0;
}

.md-dropzone-file-info .mb-0 {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.md-dropzone-progress {
  width: 100%;
}

.md-dropzone-file-actions {
  flex-shrink: 0;
}

.md-dropzone-remove {
  background: transparent;
  border: none;
  color: #67748e;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.md-dropzone-remove:hover {
  background-color: #f8f9fa;
  color: #ef5350;
}
</style>
