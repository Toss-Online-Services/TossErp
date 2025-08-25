import { c as defineEventHandler, r as readBody, e as createError, f as setCookie } from '../../../_/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

const login_post = defineEventHandler(async (event) => {
  const body = await readBody(event);
  const { email, password } = body;
  if (!email || !password) {
    throw createError({
      statusCode: 400,
      statusMessage: "Email and password are required"
    });
  }
  const demoUsers = [
    {
      id: "1",
      email: "owner@demo.toss.co.za",
      password: "password123",
      firstName: "Thabo",
      lastName: "Molefe",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "owner",
      status: "active"
    },
    {
      id: "2",
      email: "manager@demo.toss.co.za",
      password: "password123",
      firstName: "Nomsa",
      lastName: "Dlamini",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "manager",
      status: "active"
    },
    {
      id: "3",
      email: "employee@demo.toss.co.za",
      password: "password123",
      firstName: "Sipho",
      lastName: "Mthembu",
      businessName: "Thabo's Spaza Shop",
      businessId: "business_1",
      role: "employee",
      status: "active"
    }
  ];
  const user = demoUsers.find((u) => u.email === email && u.password === password);
  if (!user) {
    throw createError({
      statusCode: 401,
      statusMessage: "Invalid email or password"
    });
  }
  const token = Buffer.from(`${user.id}:${Date.now()}`).toString("base64");
  setCookie(event, "auth-token", token, {
    maxAge: 60 * 60 * 24 * 7,
    // 7 days
    secure: true,
    sameSite: "strict",
    httpOnly: true
  });
  const { password: _, ...userWithoutPassword } = user;
  return {
    success: true,
    user: {
      ...userWithoutPassword,
      createdAt: "2024-01-01T00:00:00Z",
      updatedAt: (/* @__PURE__ */ new Date()).toISOString()
    },
    token,
    permissions: user.role === "owner" ? ["admin"] : [user.role]
  };
});

export { login_post as default };
//# sourceMappingURL=login.post.mjs.map
