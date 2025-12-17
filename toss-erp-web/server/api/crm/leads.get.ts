import { defineEventHandler, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    // Return mock data based on our PostgreSQL sample data for fast response
    return await getMockLeads();
  } catch (error) {
    console.error('Error fetching leads:', error);
    throw createError({
      statusCode: 500,
      statusMessage: 'An error occurred while retrieving leads'
    });
  }
});

async function getMockLeads() {
  // Mock data based on our PostgreSQL sample data
  return [
    {
      id: "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      firstName: "Peter",
      lastName: "Parker",
      company: "Daily Bugle",
      email: "peter@dailybugle.com",
      jobTitle: "Photographer",
      source: "Website",
      industry: "Media",
      status: "New",
      score: 75,
      contactAttempts: 0,
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
      firstName: "Bruce",
      lastName: "Wayne",
      company: "Wayne Industries", 
      email: "bruce@wayneind.com",
      jobTitle: "CEO",
      source: "Referral",
      industry: "Technology",
      status: "New",
      score: 90,
      contactAttempts: 0,
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "cccccccc-cccc-cccc-cccc-cccccccccccc",
      firstName: "Clark",
      lastName: "Kent",
      company: "Daily Planet",
      email: "clark@dailyplanet.com",
      jobTitle: "Journalist", 
      source: "SocialMedia",
      industry: "Media",
      status: "New",
      score: 85,
      contactAttempts: 0,
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "dddddddd-dddd-dddd-dddd-dddddddddddd",
      firstName: "Diana",
      lastName: "Prince",
      company: "Themyscira Embassy",
      email: "diana@themyscira.gov",
      jobTitle: "Ambassador",
      source: "Email",
      industry: "Government",
      status: "New",
      score: 95,
      contactAttempts: 0,
      createdAt: new Date().toISOString(),
      isDeleted: false
    },
    {
      id: "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
      firstName: "Barry",
      lastName: "Allen",
      company: "Central City Police",
      email: "barry@ccpd.gov",
      jobTitle: "Forensic Scientist",
      source: "TradeShow",
      industry: "Law Enforcement",
      status: "New",
      score: 80,
      contactAttempts: 0,
      createdAt: new Date().toISOString(),
      isDeleted: false
    }
  ];
}
