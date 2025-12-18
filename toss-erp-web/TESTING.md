# TOSS ERP Web - Material Dashboard Pro Style Testing Guide

## ✅ Configuration Verified

### CSS Files
- ✅ `app/assets/css/main.css` - Main stylesheet with Material Dashboard Pro variables
- ✅ `app/assets/css/material-bridge.css` - Compatibility layer for Material Dashboard Pro

### Fonts
- ✅ Material Symbols Rounded - Loaded from Google Fonts
- ✅ Roboto - Loaded from Google Fonts (Material Design standard)

### Material Dashboard Pro Colors
- ✅ Primary: `#e91e63` (Pink)
- ✅ Secondary: `#737373` (Gray)
- ✅ Success: `#4CAF50` (Green)
- ✅ Info: `#1A73E8` (Blue)
- ✅ Warning: `#fb8c00` (Orange)
- ✅ Danger: `#F44335` (Red)

### Utility Classes Available
- ✅ Gradients: `bg-gradient-primary`, `bg-gradient-success`, etc.
- ✅ Shadows: `shadow-material`, `shadow-material-primary`, etc.
- ✅ Buttons: `btn-material-primary`
- ✅ Border Radius: `border-radius-lg`, `border-radius-xl`, etc.

## 🧪 Testing Checklist

### 1. Server Access
- [ ] Open http://localhost:3000 in browser
- [ ] Verify page loads without errors
- [ ] Check browser console for any errors

### 2. Visual Style Verification
- [ ] Sidebar has white background with Material shadow
- [ ] Top navigation bar has Material Design styling
- [ ] Cards use Material Design elevation (shadows)
- [ ] Buttons have Material Design ripple effect on hover
- [ ] Colors match Creative Tim reference:
  - Primary buttons should be pink (#e91e63)
  - Success elements should be green (#4CAF50)
  - Info elements should be blue (#1A73E8)

### 3. Font Verification
- [ ] Roboto font is applied to text
- [ ] Material Symbols Rounded icons display correctly
- [ ] No font fallback warnings in console

### 4. Component Testing
- [ ] Dashboard page (`/dashboard`) - Stats cards with gradients
- [ ] Stock page (`/stock/items`) - Material cards and buttons
- [ ] POS page (`/sales/pos`) - Material input and button styles
- [ ] Sidebar navigation - Material Design styling
- [ ] Top navigation - Search bar and notifications styled

### 5. Responsive Design
- [ ] Mobile view (< 640px) - Sidebar collapses
- [ ] Tablet view (640px - 1024px) - Layout adapts
- [ ] Desktop view (> 1024px) - Full layout with sidebar

### 6. Dark Mode (if applicable)
- [ ] Toggle dark mode
- [ ] Colors adapt correctly
- [ ] Contrast is maintained

## 🔍 Browser DevTools Checks

1. **Network Tab**
   - Verify CSS files load (main.css, material-bridge.css)
   - Check Google Fonts load (Material Symbols, Roboto)
   - No 404 errors for stylesheets

2. **Console Tab**
   - No JavaScript errors
   - No CSS loading warnings
   - No font loading errors

3. **Elements Tab**
   - Inspect cards - should have `shadow-material` or `shadow-material-lg`
   - Inspect buttons - should have Material Design classes
   - Check computed styles match Material Dashboard Pro values

## 📝 Expected Visual Elements

Based on Creative Tim Material Dashboard Pro reference:

1. **Cards**: White background, rounded corners (0.5rem), Material shadow
2. **Sidebar**: White background, Material shadow, rounded on desktop
3. **Top Nav**: White background, Material shadow, rounded corners
4. **Buttons**: Primary color (#e91e63), Material elevation on hover
5. **Stats Cards**: Gradient backgrounds, Material shadows
6. **Typography**: Roboto font family, proper spacing

## 🐛 Common Issues to Check

- If styles don't load: Check browser cache, hard refresh (Ctrl+F5)
- If fonts don't load: Check internet connection (Google Fonts)
- If colors are wrong: Verify CSS variables in DevTools
- If shadows missing: Check Tailwind config and CSS classes

## 🚀 Quick Test Commands

```bash
# Check if server is running
netstat -ano | findstr :3000

# Restart dev server
cd toss-erp-web
npm run dev
```

## 📊 Reference

- Creative Tim Material Dashboard Pro: https://demos.creative-tim.com/material-dashboard-pro-react/#/dashboards/sales
- Material Design Guidelines: https://material.io/design




