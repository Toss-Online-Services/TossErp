import { Redis } from 'ioredis';
import { DomainEvent } from './types';
import { v4 as uuidv4 } from 'uuid';

export interface EventHandler {
  eventType: string;
  handler: (event: DomainEvent) => Promise<void>;
}

export class EventBus {
  private handlers: Map<string, EventHandler[]> = new Map();
  private redis: Redis;
  private streamName: string;
  private consumerGroup: string;
  private consumerId: string;

  constructor(redis: Redis, serviceName: string) {
    this.redis = redis;
    this.streamName = 'toss-events';
    this.consumerGroup = `${serviceName}-group`;
    this.consumerId = `${serviceName}-${uuidv4()}`;
  }

  async initialize(): Promise<void> {
    try {
      // Create consumer group if it doesn't exist
      await this.redis.xgroup('CREATE', this.streamName, this.consumerGroup, '0', 'MKSTREAM');
    } catch (error) {
      // Group might already exist, which is fine
      if (!error.message.includes('BUSYGROUP')) {
        throw error;
      }
    }

    // Start consuming events
    this.startConsuming();
  }

  register(eventType: string, handler: (event: DomainEvent) => Promise<void>): void {
    if (!this.handlers.has(eventType)) {
      this.handlers.set(eventType, []);
    }
    this.handlers.get(eventType)!.push({ eventType, handler });
  }

  async publish(event: DomainEvent): Promise<void> {
    const eventData = {
      id: event.id,
      type: event.type,
      aggregateId: event.aggregateId,
      aggregateType: event.aggregateType,
      version: event.version.toString(),
      data: JSON.stringify(event.data),
      metadata: JSON.stringify(event.metadata),
    };

    await this.redis.xadd(this.streamName, '*', ...Object.entries(eventData).flat());
  }

  private async startConsuming(): Promise<void> {
    while (true) {
      try {
        const messages = await this.redis.xreadgroup(
          'GROUP',
          this.consumerGroup,
          this.consumerId,
          'COUNT',
          10,
          'BLOCK',
          1000,
          'STREAMS',
          this.streamName,
          '>'
        );

        if (messages && messages.length > 0) {
          for (const [stream, streamMessages] of messages) {
            for (const [messageId, fields] of streamMessages) {
              await this.processMessage(messageId, fields);
            }
          }
        }
      } catch (error) {
        console.error('Error consuming events:', error);
        await new Promise(resolve => setTimeout(resolve, 5000));
      }
    }
  }

  private async processMessage(messageId: string, fields: string[]): Promise<void> {
    try {
      const eventData: any = {};
      for (let i = 0; i < fields.length; i += 2) {
        eventData[fields[i]] = fields[i + 1];
      }

      const event: DomainEvent = {
        id: eventData.id,
        type: eventData.type,
        aggregateId: eventData.aggregateId,
        aggregateType: eventData.aggregateType,
        version: parseInt(eventData.version),
        data: JSON.parse(eventData.data),
        metadata: JSON.parse(eventData.metadata),
      };

      const handlers = this.handlers.get(event.type) || [];
      
      for (const { handler } of handlers) {
        try {
          await handler(event);
        } catch (error) {
          console.error(`Error handling event ${event.type}:`, error);
          // Could implement retry logic here
        }
      }

      // Acknowledge message
      await this.redis.xack(this.streamName, this.consumerGroup, messageId);
    } catch (error) {
      console.error('Error processing message:', error);
    }
  }
}

export function createEvent(
  type: string,
  aggregateId: string,
  aggregateType: string,
  data: Record<string, any>,
  metadata: {
    userId?: string;
    tenantId: string;
    correlationId?: string;
    causationId?: string;
  }
): DomainEvent {
  return {
    id: uuidv4(),
    type,
    aggregateId,
    aggregateType,
    version: 1,
    data,
    metadata: {
      ...metadata,
      timestamp: new Date(),
    },
  };
}

// Event types
export const Events = {
  // User events
  USER_CREATED: 'user.created',
  USER_UPDATED: 'user.updated',
  USER_DELETED: 'user.deleted',
  USER_LOGGED_IN: 'user.logged_in',
  USER_LOGGED_OUT: 'user.logged_out',

  // Customer events
  CUSTOMER_CREATED: 'customer.created',
  CUSTOMER_UPDATED: 'customer.updated',
  CUSTOMER_DELETED: 'customer.deleted',

  // Product events
  PRODUCT_CREATED: 'product.created',
  PRODUCT_UPDATED: 'product.updated',
  PRODUCT_DELETED: 'product.deleted',
  PRODUCT_STOCK_UPDATED: 'product.stock_updated',

  // Order events
  ORDER_CREATED: 'order.created',
  ORDER_UPDATED: 'order.updated',
  ORDER_CONFIRMED: 'order.confirmed',
  ORDER_SHIPPED: 'order.shipped',
  ORDER_DELIVERED: 'order.delivered',
  ORDER_CANCELLED: 'order.cancelled',

  // Invoice events
  INVOICE_CREATED: 'invoice.created',
  INVOICE_SENT: 'invoice.sent',
  INVOICE_PAID: 'invoice.paid',
  INVOICE_OVERDUE: 'invoice.overdue',
  INVOICE_CANCELLED: 'invoice.cancelled',

  // Payment events
  PAYMENT_INITIATED: 'payment.initiated',
  PAYMENT_COMPLETED: 'payment.completed',
  PAYMENT_FAILED: 'payment.failed',
  PAYMENT_REFUNDED: 'payment.refunded',

  // Workflow events
  WORKFLOW_STARTED: 'workflow.started',
  WORKFLOW_STEP_COMPLETED: 'workflow.step_completed',
  WORKFLOW_COMPLETED: 'workflow.completed',
  WORKFLOW_FAILED: 'workflow.failed',

  // Group buying events
  GROUP_BUY_CREATED: 'group_buy.created',
  GROUP_BUY_JOINED: 'group_buy.joined',
  GROUP_BUY_TARGET_REACHED: 'group_buy.target_reached',
  GROUP_BUY_CLOSED: 'group_buy.closed',

  // Asset sharing events
  ASSET_SHARED: 'asset.shared',
  ASSET_BOOKED: 'asset.booked',
  ASSET_USED: 'asset.used',
  ASSET_RETURNED: 'asset.returned',

  // Credit events
  CREDIT_APPLICATION_SUBMITTED: 'credit.application_submitted',
  CREDIT_APPLICATION_APPROVED: 'credit.application_approved',
  CREDIT_APPLICATION_REJECTED: 'credit.application_rejected',
  CREDIT_DISBURSED: 'credit.disbursed',
  CREDIT_PAYMENT_MADE: 'credit.payment_made',
  CREDIT_PAYMENT_OVERDUE: 'credit.payment_overdue',

  // AI events
  AI_INSIGHT_GENERATED: 'ai.insight_generated',
  AI_RECOMMENDATION_MADE: 'ai.recommendation_made',
  AI_AUTOMATION_TRIGGERED: 'ai.automation_triggered',
} as const;

export type EventType = typeof Events[keyof typeof Events];

export default {
  EventBus,
  createEvent,
  Events,
};
