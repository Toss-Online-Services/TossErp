import { ServiceConfig } from './types';
import pino from 'pino';

export function createLogger(config: ServiceConfig) {
  const logger = pino({
    name: config.name,
    level: config.logging.level,
    formatters: {
      bindings: (bindings) => ({
        service: config.name,
        pid: bindings.pid,
        hostname: bindings.hostname,
      }),
    },
    timestamp: () => `,"timestamp":"${new Date().toISOString()}"`,
    transport: config.logging.prettyPrint ? {
      target: 'pino-pretty',
      options: {
        colorize: true,
        translateTime: 'SYS:standard',
        ignore: 'pid,hostname',
      },
    } : undefined,
  });

  return logger;
}

export default { createLogger };
