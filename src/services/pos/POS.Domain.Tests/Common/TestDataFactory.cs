using Bogus;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Common.ValueObjects;

namespace POS.Domain.Tests.Common;

public static class TestDataFactory
{
    private static readonly Faker Faker = new();

    public static class Sale
    {
        public static SaleAggregate CreateValidSale()
        {
            var saleNumber = Faker.Random.AlphaNumeric(10).ToUpper();
            var customerId = Faker.Random.Guid();
            var staffId = Faker.Random.Guid();
            var storeId = Faker.Random.Guid();
            var terminalId = Faker.Random.Guid();
            var saleDate = Faker.Date.Recent();

            return new SaleAggregate(
                saleNumber,
                customerId,
                staffId,
                storeId,
                terminalId,
                saleDate
            );
        }

        public static SaleItem CreateValidSaleItem()
        {
            var productId = Faker.Random.Guid();
            var quantity = Faker.Random.Int(1, 10);
            var unitPrice = Faker.Random.Decimal(1, 1000);
            var discount = Faker.Random.Decimal(0, unitPrice * 0.2m);
            var tax = Faker.Random.Decimal(0, unitPrice * 0.1m);

            return new SaleItem(
                productId,
                quantity,
                unitPrice,
                discount,
                tax
            );
        }

        public static SalePayment CreateValidSalePayment()
        {
            var method = Faker.PickRandom<PaymentMethod>();
            var amount = Faker.Random.Decimal(1, 1000);
            var reference = Faker.Random.AlphaNumeric(10);
            var notes = Faker.Lorem.Sentence();

            return new SalePayment(
                method,
                amount,
                reference,
                notes
            );
        }

        public static SaleDiscount CreateValidSaleDiscount()
        {
            var type = Faker.PickRandom<DiscountType>();
            var amount = Faker.Random.Decimal(1, 1000);
            var reason = Faker.Lorem.Sentence();

            return new SaleDiscount(
                type,
                amount,
                reason
            );
        }
    }

    public static class ValueObjects
    {
        public static Money CreateValidMoney()
        {
            var amount = Faker.Random.Decimal(1, 1000);
            var currency = Faker.PickRandom("USD", "EUR", "GBP");

            return new Money(amount, currency);
        }

        public static Address CreateValidAddress()
        {
            var street = Faker.Address.StreetAddress();
            var city = Faker.Address.City();
            var state = Faker.Address.State();
            var zipCode = Faker.Address.ZipCode();
            var country = Faker.Address.Country();

            return new Address(street, city, state, zipCode, country);
        }
    }
} 
