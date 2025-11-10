import * as Sentry from "@sentry/nuxt";

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig();
  const dsn = config.public?.sentry?.dsn;
  if (!dsn) return;

  Sentry.init({
    dsn,
    tracesSampleRate: 0.1,
    replaysSessionSampleRate: 0.05,
    replaysOnErrorSampleRate: 1.0,
    enableLogs: false
  });
});
