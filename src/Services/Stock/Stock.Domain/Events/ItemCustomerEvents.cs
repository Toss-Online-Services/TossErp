using TossErp.Stock.Domain.Entities;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

public record ItemCustomerCreatedEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerUpdatedEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerDefaultSetEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerPreferredSetEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerPricingUpdatedEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerUOMUpdatedEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerDisabledEvent(ItemCustomer ItemCustomer) : IDomainEvent;
public record ItemCustomerEnabledEvent(ItemCustomer ItemCustomer) : IDomainEvent; 
