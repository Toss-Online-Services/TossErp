import * as Sentry from "@sentry/nuxt";

export default defineNitroPlugin(() => {
  // Prefer private server-only DSN if provided; fall back to public DSN for simplicity
  const dsn = process.env.SENTRY_DSN || process.env.NUXT_PUBLIC_SENTRY_DSN;
  if (!dsn) return;

  Sentry.init({
    dsn,
    tracesSampleRate: 0.1,
    enableLogs: false
  });
});
