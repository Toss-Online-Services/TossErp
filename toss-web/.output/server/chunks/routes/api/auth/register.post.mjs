import { c as defineEventHandler, r as readBody, e as createError } from '../../../_/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

const register_post = defineEventHandler(async (event) => {
  const body = await readBody(event);
  const { businessName, firstName, lastName, email, phone, password, businessType } = body;
  if (!businessName || !firstName || !lastName || !email || !password || !businessType) {
    throw createError({
      statusCode: 400,
      statusMessage: "All required fields must be provided"
    });
  }
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    throw createError({
      statusCode: 400,
      statusMessage: "Invalid email format"
    });
  }
  if (password.length < 8) {
    throw createError({
      statusCode: 400,
      statusMessage: "Password must be at least 8 characters long"
    });
  }
  const existingEmails = [
    "owner@demo.toss.co.za",
    "manager@demo.toss.co.za",
    "employee@demo.toss.co.za"
  ];
  if (existingEmails.includes(email)) {
    throw createError({
      statusCode: 409,
      statusMessage: "An account with this email already exists"
    });
  }
  const newUser = {
    id: `user_${Date.now()}`,
    email,
    firstName,
    lastName,
    businessName,
    businessId: `business_${Date.now()}`,
    phone,
    businessType,
    role: "owner",
    // First user of a business becomes owner
    status: "pending",
    // Requires email verification
    createdAt: (/* @__PURE__ */ new Date()).toISOString(),
    updatedAt: (/* @__PURE__ */ new Date()).toISOString()
  };
  return {
    success: true,
    message: "Account created successfully. Please check your email to verify your account.",
    user: newUser
  };
});

export { register_post as default };
//# sourceMappingURL=register.post.mjs.map
