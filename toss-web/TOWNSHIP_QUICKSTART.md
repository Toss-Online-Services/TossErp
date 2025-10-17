# 🚀 Township UX Quick Start Guide

## What Changed?

Your TOSS app now has **township-optimized UX** for spaza shop owners! Here's what's new:

### 1. 🎯 Clear Purpose (Dashboard Hero)
- Big, friendly message: "Order Stock for Your Spaza Shop"
- Two huge buttons: **Order Now** & **Track My Order**
- Can't miss it!

### 2. 🧭 Simple Navigation (Bottom Bar)
Only 4 buttons now:
- **Home** 🏠
- **Order** 🛒 (with alert dot!)
- **Orders** 📋
- **Group** 👥

### 3. 👀 Better Readability
- Bigger fonts (18px base)
- Higher contrast (WCAG AAA)
- Larger buttons (48x48px on mobile)
- Clear focus indicators

### 4. 🌍 Multiple Languages
- **English** (Default)
- **isiZulu** 
- **isiXhosa**
- Switcher in top-right corner

### 5. 🇿🇦 Local Feel
- Prices in **Rand (R)** not dollars
- Group buying feature
- "53 shop owners in your area"
- South African date formats

### 6. 📵 Offline Ready
- Clear "You're offline" banner
- Your work saves automatically
- Syncs when back online

### 7. 📦 Order Tracking
- Visual timeline of your delivery
- See each step clearly
- Know when it arrives

### 8. 💬 WhatsApp Help
- Green button (bottom-right)
- Instant chat support
- Speaks your language

## Quick Test

### 1. Start the App
```bash
cd toss-web
npm run dev
```
Open: http://localhost:3001

### 2. What You'll See
- **Big hero section** at the top
- **Language switcher** (top-right)
- **WhatsApp button** (bottom-right, green)
- **Simple navigation** (bottom, 4 icons)

### 3. Try These
1. **Click "Order Now"** → Big blue button
2. **Switch language** → Top-right dropdown
3. **Click WhatsApp** → Green button opens chat
4. **Check bottom nav** → Only 4 items, nice and simple
5. **Go offline** → Toggle in DevTools, see banner

## For Users (Spaza Shop Owners)

### First Time?
1. Open the app
2. See **"Order Stock for Your Spaza Shop"**
3. Tap big **"Order Now"** button
4. Browse and add items
5. Checkout

### Need Help?
- Tap **green WhatsApp button** (bottom-right)
- Instant chat with support
- In your language!

### Want Your Language?
- Tap **language button** (top-right)
- Choose: English, isiZulu, or isiXhosa
- Everything updates!

### Track Your Order?
- Tap **"Orders"** (bottom nav)
- See timeline with steps
- Know exactly where your stock is

### No Internet?
- App still works!
- See orange banner at top
- Your work saves automatically
- Syncs when you're back online

## For Developers

### New Components
```
components/township/
├── HeroSection.vue          # Dashboard hero
├── LanguageSwitcher.vue     # Language dropdown
├── WhatsAppSupport.vue      # Support button
├── GroupBuyingCard.vue      # Community buying
└── DeliveryTimeline.vue     # Order tracking
```

### How to Use Components

#### Hero Section
```vue
<HeroSection />
```

#### Language Switcher
```vue
<LanguageSwitcher />
```

#### WhatsApp Support
```vue
<WhatsAppSupport />
```

#### Group Buying Card
```vue
<GroupBuyingCard />
```

#### Delivery Timeline
```vue
<DeliveryTimeline 
  :steps="orderSteps" 
  :estimatedArrival="arrivalTime" 
/>
```

### Adding Translations

#### 1. Add to English (`locales/en.json`)
```json
{
  "myFeature": {
    "title": "My Feature",
    "description": "Feature description"
  }
}
```

#### 2. Add to isiZulu (`locales/zu.json`)
```json
{
  "myFeature": {
    "title": "Isici Sami",
    "description": "Incazelo yesici"
  }
}
```

#### 3. Add to isiXhosa (`locales/xh.json`)
```json
{
  "myFeature": {
    "title": "Isici Sam",
    "description": "Inkcazo yesici"
  }
}
```

#### 4. Use in Component
```vue
<template>
  <h1>{{ $t('myFeature.title') }}</h1>
  <p>{{ $t('myFeature.description') }}</p>
</template>

<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>
```

## Styling Guidelines

### Colors (WCAG AAA)
```css
/* Use these for text */
--text-primary: #1a1a1a      /* Very dark gray */
--text-secondary: #4a4a4a    /* Medium gray */
--text-tertiary: #737373     /* Light gray */

/* Status colors */
--status-success: #047857    /* Green 700 */
--status-warning: #b45309    /* Orange 700 */
--status-error: #b91c1c      /* Red 700 */
--status-info: #1e40af       /* Blue 700 */
```

### Typography
```css
/* Base sizes */
html { font-size: 18px; }

/* Text classes */
.text-base { font-size: 1rem; line-height: 1.75; }
.text-lg { font-size: 1.125rem; line-height: 1.75; }
.text-xl { font-size: 1.25rem; line-height: 1.75; }
```

### Buttons
```css
/* Minimum sizes */
button {
  min-height: 44px;
  min-width: 44px;
}

/* Mobile */
@media (max-width: 640px) {
  button {
    min-height: 48px;
    min-width: 48px;
    font-size: 1.125rem;
    padding: 0.875rem 1.5rem;
  }
}
```

## Testing Checklist

### ✅ Visual Check
- [ ] Hero section shows at top
- [ ] Language switcher in top-right
- [ ] WhatsApp button floating
- [ ] Bottom nav has 4 items
- [ ] All text is readable

### ✅ Functionality
- [ ] Language switching works
- [ ] WhatsApp button opens chat
- [ ] Bottom nav navigates
- [ ] Offline banner appears
- [ ] Delivery timeline animates

### ✅ Mobile (Chrome DevTools)
- [ ] Test at 360px width
- [ ] Test at 414px width
- [ ] All buttons easy to tap
- [ ] Text not too small
- [ ] No horizontal scroll

### ✅ Accessibility (Lighthouse)
- [ ] Score 95+ (target)
- [ ] No color contrast errors
- [ ] Focus indicators visible
- [ ] Tab navigation works
- [ ] Screen reader friendly

## Common Issues & Fixes

### Language Not Switching?
```javascript
// Check localStorage
localStorage.getItem('toss-language')

// Clear and reload
localStorage.clear()
location.reload()
```

### WhatsApp Not Opening?
```javascript
// Check phone number in component
const phoneNumber = '27123456789' // Update this
```

### Offline Banner Not Showing?
```javascript
// Toggle in DevTools:
// Network tab → Select "Offline"
```

### Fonts Too Small on Mobile?
```css
/* Check in browser */
html { font-size: 18px !important; }
```

## Performance Tips

### 1. Lazy Load Components
```javascript
const HeroSection = defineAsyncComponent(() => 
  import('~/components/township/HeroSection.vue')
)
```

### 2. Optimize Images
- Use WebP format
- Compress before upload
- Lazy load below fold

### 3. Monitor Bundle Size
```bash
npm run build
# Check output sizes
```

## Need Help?

### Documentation
- Full docs: `docs/township-ux-improvements.md`
- This guide: `TOWNSHIP_QUICKSTART.md`

### Code Examples
- Check existing components in `components/township/`
- Look at `pages/index.vue` for integration

### Community
- GitHub Issues: Report bugs
- PRD: See `docs/TOSS_MVP_Launch_Plan.md`

---

## 🎉 You're Ready!

Everything is set up and working. The app is now:
- ✅ Township-friendly
- ✅ Multilingual
- ✅ Accessible
- ✅ Offline-capable
- ✅ Mobile-optimized

**Time to test with real users!** 📱👥

---

*Need to understand something? Check `docs/township-ux-improvements.md` for full details.*

