// Composable for rural township enterprise data
import { computed } from 'vue'

export interface EnterpriseType {
  sector: string
  enterprise: string
  type: string
  typicalScale: string
  description: string
}

// Rural Township Enterprise Data
const enterpriseData: EnterpriseType[] = [
  // Retail & Trade
  { sector: "Retail & Trade", enterprise: "Spaza / Convenience shop", type: "P/H", typicalScale: "Micro/Informal", description: "Small local shop selling groceries, airtime, household goods." },
  { sector: "Retail & Trade", enterprise: "Hawkers / Street traders", type: "P/H", typicalScale: "Micro/Informal", description: "Mobile sellers of food, fruit, small goods." },
  { sector: "Retail & Trade", enterprise: "Pavement / Market stall sellers", type: "P/H", typicalScale: "Micro/Informal", description: "Fixed-day stalls selling clothing, utensils, produce." },
  { sector: "Retail & Trade", enterprise: "Second-hand clothing (thrift) stalls", type: "P", typicalScale: "Micro", description: "Buy/resell used clothes." },
  { sector: "Retail & Trade", enterprise: "Small kiosks (tuck-shops)", type: "P", typicalScale: "Micro", description: "Single-item focused retail (snacks, cold drinks)." },
  { sector: "Retail & Trade", enterprise: "Bulk-buying group retail (stokvel shops)", type: "P/H", typicalScale: "Micro/Small", description: "Group procurement and resale." },
  
  // Food & Hospitality
  { sector: "Food & Hospitality", enterprise: "Shebeen / informal tavern", type: "S/H", typicalScale: "Micro/Informal", description: "Local drinking/social venue often serving snacks." },
  { sector: "Food & Hospitality", enterprise: "Township restaurant / Imbizo / braai stalls", type: "S/H", typicalScale: "Micro/Small", description: "Cooked-food outlets, often outdoor." },
  { sector: "Food & Hospitality", enterprise: "Home bakeries & confectioneries", type: "P/H", typicalScale: "Micro", description: "Bread, cakes made at home for sale." },
  { sector: "Food & Hospitality", enterprise: "Prepared-food vendors / street food", type: "S/H", typicalScale: "Micro/Informal", description: "Ready-to-eat meals (pap, vetkoek, samosas)." },
  { sector: "Food & Hospitality", enterprise: "Catering for events", type: "S/H", typicalScale: "Micro/Small", description: "Small-scale event catering for local functions." },
  { sector: "Food & Hospitality", enterprise: "Mobile food truck / cart", type: "S/H", typicalScale: "Micro/Small", description: "Mobile prepared-food service." },
  { sector: "Food & Hospitality", enterprise: "Informal lodging / backyard B&B", type: "S/H", typicalScale: "Micro", description: "Short-stay accommodation in homes." },
  
  // Personal & Household Services
  { sector: "Personal & Household Services", enterprise: "Hair salons / barbershops", type: "S", typicalScale: "Micro", description: "Haircuts and styling from small shops or at home." },
  { sector: "Personal & Household Services", enterprise: "Beauty & nail services", type: "S", typicalScale: "Micro", description: "Manicure, pedicure and beauty treatments." },
  { sector: "Personal & Household Services", enterprise: "Laundry / wash-and-fold", type: "S", typicalScale: "Micro/Small", description: "Manual or small-machine based laundry services." },
  { sector: "Personal & Household Services", enterprise: "Domestic cleaning / housekeepers", type: "S", typicalScale: "Micro", description: "Home cleaning and chores." },
  { sector: "Personal & Household Services", enterprise: "Childminders / day-mothers (informal)", type: "S", typicalScale: "Micro/Informal", description: "Unregistered childcare in homes." },
  
  // Early Childhood & Social Care
  { sector: "Early Childhood & Social Care", enterprise: "Daycare / ECD centres / crèches", type: "S", typicalScale: "Micro/Small", description: "Registered/unregistered childcare and early learning." },
  { sector: "Early Childhood & Social Care", enterprise: "After-school care / homework clubs", type: "S", typicalScale: "Micro", description: "Supervision & tutoring outside school hours." },
  { sector: "Early Childhood & Social Care", enterprise: "Elderly care / home caregivers", type: "S", typicalScale: "Micro", description: "Basic in-home elder support." },
  
  // Trades & Technical Services
  { sector: "Trades & Technical Services", enterprise: "Electricians", type: "S", typicalScale: "Micro/Small", description: "Residential/commercial wiring, repairs." },
  { sector: "Trades & Technical Services", enterprise: "Plumbers", type: "S", typicalScale: "Micro/Small", description: "Drainage, piping, toilet installation and repair." },
  { sector: "Trades & Technical Services", enterprise: "Carpenters & joiners", type: "P/H", typicalScale: "Micro/Small", description: "Furniture manufacture and installation." },
  { sector: "Trades & Technical Services", enterprise: "Welders & metalworkers", type: "P/H", typicalScale: "Micro/Small", description: "Gates, repairs, fabrication." },
  { sector: "Trades & Technical Services", enterprise: "Panel beaters / vehicle body repair", type: "S/H", typicalScale: "Micro/Small", description: "Repair for minibuses and cars." },
  { sector: "Trades & Technical Services", enterprise: "Mechanics (vehicle repairs)", type: "S/H", typicalScale: "Micro/Small", description: "Engine, brake, general repairs." },
  { sector: "Trades & Technical Services", enterprise: "Auto-electrical services", type: "S", typicalScale: "Micro", description: "Batteries, wiring, starter repairs." },
  { sector: "Trades & Technical Services", enterprise: "Appliance repair (phones, TVs, fridges)", type: "S/H", typicalScale: "Micro", description: "Electronics and appliance fixes." },
  { sector: "Trades & Technical Services", enterprise: "Solar installers / maintenance", type: "S/H", typicalScale: "Micro/Small", description: "Small-scale solar panel installation & upkeep." },
  { sector: "Trades & Technical Services", enterprise: "IT / PC / phone repair & mobile tech support", type: "S", typicalScale: "Micro", description: "Repair and basic IT services." },
  
  // Automotive & Mobility Services
  { sector: "Automotive & Mobility Services", enterprise: "Car washes (fixed bays)", type: "S", typicalScale: "Micro/Small", description: "Manual/pressure cleaning." },
  { sector: "Automotive & Mobility Services", enterprise: "Mobile car wash", type: "S", typicalScale: "Micro", description: "Door-to-door vehicle cleaning." },
  { sector: "Automotive & Mobility Services", enterprise: "Minibus taxi / shared transport", type: "S", typicalScale: "Small", description: "Passenger transport services." },
  { sector: "Automotive & Mobility Services", enterprise: "Courier / small goods delivery (local)", type: "S", typicalScale: "Micro", description: "Last-mile delivery." },
  { sector: "Automotive & Mobility Services", enterprise: "Vehicle detailing / upholstery", type: "S/H", typicalScale: "Micro/Small", description: "Interior cleaning and small product sales." },
  
  // Agriculture & Agribusiness
  { sector: "Agriculture & Agribusiness", enterprise: "Small-scale vegetable gardening", type: "P", typicalScale: "Micro", description: "Backyard or community-plot produce." },
  { sector: "Agriculture & Agribusiness", enterprise: "Poultry (chicken) farming", type: "P", typicalScale: "Micro/Small", description: "Eggs and broiler production." },
  { sector: "Agriculture & Agribusiness", enterprise: "Small livestock (goats, sheep)", type: "P", typicalScale: "Micro/Small", description: "Meat and breeding stock." },
  { sector: "Agriculture & Agribusiness", enterprise: "Small-scale dairy / milk collection", type: "P/H", typicalScale: "Micro/Small", description: "Local milk production & basic processing." },
  { sector: "Agriculture & Agribusiness", enterprise: "Bee-keeping / honey production", type: "P", typicalScale: "Micro", description: "Honey and beeswax products." },
  { sector: "Agriculture & Agribusiness", enterprise: "Aquaculture / fish ponds (tilapia)", type: "P", typicalScale: "Micro", description: "Local fish farming." },
  { sector: "Agriculture & Agribusiness", enterprise: "Agri-processing (sun-dried produce, preserves)", type: "P/H", typicalScale: "Micro/Small", description: "Value-added foods." },
  { sector: "Agriculture & Agribusiness", enterprise: "Packhouse / small-scale cold storage (micro)", type: "S/H", typicalScale: "Micro/Small", description: "Packing and short-term storage." },
  
  // Financial Services & Group Savings
  { sector: "Financial Services & Group Savings", enterprise: "Stokvels / rotating savings groups", type: "S/H", typicalScale: "Micro", description: "Collective savings and bulk buying." },
  { sector: "Financial Services & Group Savings", enterprise: "Burial societies", type: "S", typicalScale: "Micro/Community", description: "Social insurance via membership contributions." },
  { sector: "Financial Services & Group Savings", enterprise: "Micro-lending / hawker credit schemes", type: "S", typicalScale: "Micro/Informal", description: "Informal credit to traders." },
  { sector: "Financial Services & Group Savings", enterprise: "Agent banking / airtime & voucher shops", type: "S/H", typicalScale: "Micro", description: "Financial transactions and airtime sales." },
  
  // Digital, Mobile & Platform Services
  { sector: "Digital, Mobile & Platform Services", enterprise: "Mobile money / airtime resellers", type: "S/H", typicalScale: "Micro", description: "Digital transaction facilitation." },
  { sector: "Digital, Mobile & Platform Services", enterprise: "Small app-based service providers", type: "S", typicalScale: "Micro", description: "Platform-enabled bookings (plumbing, electrician)." }
]

export const useEnterpriseData = () => {
  // Get all unique sectors
  const sectors = computed(() => {
    const uniqueSectors = [...new Set(enterpriseData.map(item => item.sector))]
    return uniqueSectors.sort()
  })
  
  // Get enterprises by sector
  const getEnterprisesBySector = (sector: string) => {
    return enterpriseData.filter(item => item.sector === sector)
  }
  
  // Get all enterprises
  const allEnterprises = computed(() => enterpriseData)
  
  // Search enterprises
  const searchEnterprises = (query: string) => {
    if (!query) return enterpriseData
    
    const lowercaseQuery = query.toLowerCase()
    return enterpriseData.filter(item => 
      item.enterprise.toLowerCase().includes(lowercaseQuery) ||
      item.sector.toLowerCase().includes(lowercaseQuery) ||
      item.description.toLowerCase().includes(lowercaseQuery)
    )
  }
  
  // Get common enterprise types for quick selection
  const popularEnterprises = computed(() => [
    "Spaza / Convenience shop",
    "Hair salons / barbershops",
    "Mobile car wash",
    "Small-scale vegetable gardening",
    "Agent banking / airtime & voucher shops",
    "Township restaurant / Imbizo / braai stalls",
    "Electricians",
    "Plumbers",
    "Mechanics (vehicle repairs)",
    "Stokvels / rotating savings groups"
  ])
  
  return {
    sectors,
    allEnterprises,
    getEnterprisesBySector,
    searchEnterprises,
    popularEnterprises
  }
}
