import { c as defineEventHandler, f as setCookie } from '../../../_/nitro.mjs';
import 'node:http';
import 'node:https';
import 'node:events';
import 'node:buffer';
import 'node:fs';
import 'node:path';
import 'node:crypto';
import 'node:url';

const logout_post = defineEventHandler(async (event) => {
  setCookie(event, "auth-token", "", {
    maxAge: 0,
    secure: true,
    sameSite: "strict",
    httpOnly: true
  });
  return {
    success: true,
    message: "Logged out successfully"
  };
});

export { logout_post as default };
//# sourceMappingURL=logout.post.mjs.map
