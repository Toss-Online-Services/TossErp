import { FastifyInstance } from 'fastify';
import Joi from 'joi';

export function createValidationPlugin() {
  return async function validationPlugin(fastify: FastifyInstance) {
    fastify.decorate('validate', function (schema: Joi.ObjectSchema, data: any) {
      const { error, value } = schema.validate(data, {
        abortEarly: false,
        stripUnknown: true,
      });

      if (error) {
        const errorDetails = error.details.map(detail => ({
          field: detail.path.join('.'),
          message: detail.message,
          value: detail.context?.value,
        }));

        throw new Error(`Validation failed: ${JSON.stringify(errorDetails)}`);
      }

      return value;
    });

    // Common validation schemas
    fastify.decorate('schemas', {
      pagination: Joi.object({
        page: Joi.number().integer().min(1).default(1),
        limit: Joi.number().integer().min(1).max(100).default(10),
        sortBy: Joi.string().default('createdAt'),
        sortOrder: Joi.string().valid('asc', 'desc').default('desc'),
        search: Joi.string().allow('').optional(),
      }),

      uuid: Joi.string().uuid(),

      email: Joi.string().email(),

      phone: Joi.string().pattern(/^(\+27|0)[0-9]{9}$/),

      money: Joi.object({
        amount: Joi.number().min(0).required(),
        currency: Joi.string().length(3).default('ZAR'),
      }),

      address: Joi.object({
        street: Joi.string().required(),
        city: Joi.string().required(),
        province: Joi.string().required(),
        postalCode: Joi.string().required(),
        country: Joi.string().default('South Africa'),
      }),

      contactInfo: Joi.object({
        email: Joi.string().email().optional(),
        phone: Joi.string().pattern(/^(\+27|0)[0-9]{9}$/).optional(),
        mobile: Joi.string().pattern(/^(\+27|0)[0-9]{9}$/).optional(),
        fax: Joi.string().optional(),
        website: Joi.string().uri().optional(),
      }),

      businessHours: Joi.object({
        monday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        tuesday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        wednesday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        thursday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        friday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        saturday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
        sunday: Joi.object({
          open: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          close: Joi.string().pattern(/^([01]?[0-9]|2[0-3]):[0-5][0-9]$/),
          closed: Joi.boolean().default(false),
        }).optional(),
      }),
    });
  };
}

export default { createValidationPlugin };
