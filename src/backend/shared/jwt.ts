import { FastifyInstance, FastifyRequest } from 'fastify';
import { PrismaClient } from '@prisma/client';
import jwt from 'jsonwebtoken';
import { ServiceConfig, User } from './types';

declare module 'fastify' {
  interface FastifyRequest {
    user?: User;
    jwtPayload?: any;
  }
  
  interface FastifyInstance {
    authenticate: (request: FastifyRequest) => Promise<void>;
    optionalAuthenticate: (request: FastifyRequest) => Promise<void>;
    generateTokens: (user: User) => { accessToken: string; refreshToken: string };
    verifyToken: (token: string) => any;
    prisma: PrismaClient;
  }
}

export function createJWTPlugin(config: ServiceConfig) {
  return async function jwtPlugin(fastify: FastifyInstance) {
    await fastify.register(require('@fastify/jwt'), {
      secret: config.jwt.secret,
      sign: {
        expiresIn: config.jwt.expiresIn,
      },
      verify: {
        extractToken: (request) => {
          const authorization = request.headers.authorization;
          if (authorization && authorization.startsWith('Bearer ')) {
            return authorization.slice(7);
          }
          return null;
        },
      },
    });

    fastify.decorate('authenticate', async function (request: FastifyRequest) {
      try {
        const token = request.headers.authorization?.replace('Bearer ', '');
        if (!token) {
          throw new Error('No token provided');
        }

        const decoded = jwt.verify(token, config.jwt.secret) as any;
        request.user = decoded.user;
        request.jwtPayload = decoded;
      } catch (error) {
        throw new Error('Invalid token');
      }
    });

    fastify.decorate('optionalAuthenticate', async function (request: FastifyRequest) {
      try {
        const token = request.headers.authorization?.replace('Bearer ', '');
        if (token) {
          const decoded = jwt.verify(token, config.jwt.secret) as any;
          request.user = decoded.user;
          request.jwtPayload = decoded;
        }
      } catch (error) {
        // Optional authentication - ignore errors
      }
    });

    fastify.decorate('generateTokens', function (user: User) {
      const payload = { user };
      
      const accessToken = jwt.sign(payload, config.jwt.secret, {
        expiresIn: config.jwt.expiresIn,
      });

      const refreshToken = jwt.sign(payload, config.jwt.secret, {
        expiresIn: config.jwt.refreshExpiresIn,
      });

      return { accessToken, refreshToken };
    });

    fastify.decorate('verifyToken', function (token: string) {
      return jwt.verify(token, config.jwt.secret);
    });
  };
}

export default { createJWTPlugin };
