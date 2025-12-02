export default defineEventHandler(() => {
  // Intentionally throw to verify server-side Sentry capture
  const err = new Error('Sentry server test error');
  // Throwing will propagate through Nitro; Sentry should capture automatically when initialized
  throw err;
});
