# TOSS Web - Quick Start Guide

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ installed
- npm or pnpm package manager

### Installation & Running

```bash
# Navigate to the project directory
cd toss-web

# Install dependencies (if not already done)
npm install

# Start the development server
npm run dev
```

The application will be available at: **http://localhost:3000**

---

## 📱 What You'll See

### Dashboard (Home Page)
When you open the application, you'll see:

1. **Left Sidebar** with navigation:
   - Home (Dashboard)
   - Sales
   - Stock
   - Money
   - People
   - Jobs
   - Settings

2. **Top Bar** with:
   - Menu toggle button
   - TOSS branding
   - User menu (avatar with "U")

3. **Main Dashboard** showing:
   - Today's date and "Today's Overview" heading
   - Three KPI cards:
     - **Today's Sales:** R 15,420 (Blue card)
     - **Money In:** R 12,300 (Green card)
     - **Money Out:** R 4,500 (Orange card)
   - Alert cards for Low Stock, Pending Orders, Overdue Invoices
   - Weekly sales trend chart
   - Quick action buttons

---

## 🎨 Current Features

✅ **Responsive Design** - Works on mobile, tablet, and desktop  
✅ **Dark Mode Ready** - Theme switching capability built-in  
✅ **Mock Data** - Dashboard shows sample data for demonstration  
✅ **Interactive UI** - Hover effects and smooth transitions  
✅ **Clean Layout** - Material Dashboard-inspired design  

---

## 🛠️ Development Commands

```bash
# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linting
npm run lint
```

---

## 📂 Key Files to Know

- **`pages/index.vue`** - Dashboard/Home page
- **`layouts/default.vue`** - Main layout with sidebar
- **`components/ui/`** - Reusable UI components
- **`nuxt.config.ts`** - Application configuration
- **`tailwind.config.js`** - Styling configuration

---

## 🔧 Troubleshooting

### Server won't start
```bash
# Kill any existing Node processes
Get-Process | Where-Object {$_.ProcessName -like "*node*"} | Stop-Process -Force

# Clear cache and restart
Remove-Item -Recurse -Force .nuxt
npm run dev
```

### Port already in use
The server will automatically use the next available port (3001, 3002, etc.)

### CSS not loading
Clear the Nuxt cache:
```bash
Remove-Item -Recurse -Force .nuxt
Remove-Item -Recurse -Force node_modules/.cache
npm run dev
```

---

## 📖 Next Steps

1. **Explore the Dashboard** - Click around and see the current features
2. **Check PROGRESS_SUMMARY.md** - Detailed progress and technical info
3. **Review TODO List** - See what's coming next
4. **Read the PRD** - Understand the full product vision

---

## 💡 Tips

- The sidebar can be toggled with the menu button
- All data is currently mock data (hardcoded)
- Navigation links are set up but pages aren't created yet
- The design follows Material Dashboard Pro aesthetic
- Mobile-first approach means it looks great on phones

---

## 🎯 What's Working

✅ Application loads without errors  
✅ Dashboard displays correctly  
✅ Navigation structure is in place  
✅ Responsive design works  
✅ Components are reusable  
✅ Tailwind CSS is configured  
✅ TypeScript is set up  

---

## 📞 Need Help?

- Check the terminal for any error messages
- Review `PROGRESS_SUMMARY.md` for detailed information
- Look at the browser console (F12) for client-side errors
- Refer to Nuxt 4 documentation: https://nuxt.com

---

**Happy Coding! 🚀**

