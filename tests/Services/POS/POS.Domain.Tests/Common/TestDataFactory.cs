using Bogus;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Models;
using Address = POS.Domain.Common.ValueObjects.Address;

namespace POS.Domain.Tests.Common;

public static class TestDataFactory
{
    private static readonly Faker Faker = new();

    public static class SaleFactory
    {
        public static Sale CreateValidSale()
        {
            var saleNumber = Faker.Random.AlphaNumeric(10).ToUpper();
            var storeId = Faker.Random.Guid();
            var customerId = Faker.Random.Guid();
            var staffId = Faker.Random.Guid();

            return new Sale(
                saleNumber,
                storeId,
                customerId,
                staffId
            );
        }

        public static SaleItem CreateValidSaleItem(Guid? saleId = null)
        {
            var id = Faker.Random.Guid();
            var saleIdValue = saleId ?? Faker.Random.Guid();
            var productId = Faker.Random.Guid();
            var productName = Faker.Commerce.ProductName();
            var unitPrice = Faker.Random.Decimal(1, 1000);
            var quantity = Faker.Random.Int(1, 10);
            var taxRate = Faker.Random.Decimal(0, 20);
            return new SaleItem(
                id,
                saleIdValue,
                productId,
                productName,
                unitPrice,
                quantity,
                taxRate
            );
        }

        public static Payment CreateValidPayment(Guid? saleId = null)
        {
            var id = Faker.Random.Guid();
            var saleIdValue = saleId ?? Faker.Random.Guid();
            var amount = Faker.Random.Decimal(1, 1000);
            var method = Faker.PickRandom<PaymentType>();
            var reference = Faker.Random.AlphaNumeric(10);
            var cardLast4 = Faker.Random.Bool() ? Faker.Random.ReplaceNumbers("####") : null;
            var cardType = Faker.Random.Bool() ? "Visa" : null;
            return new Payment(
                id,
                saleIdValue,
                amount,
                method,
                reference,
                cardLast4,
                cardType
            );
        }

        public static SaleDiscount CreateValidSaleDiscount()
        {
            var type = Faker.PickRandom<DiscountType>();
            var amount = Faker.Random.Decimal(1, 1000);
            var description = Faker.Lorem.Sentence();
            return new SaleDiscount(
                type,
                amount,
                description
            );
        }
    }

    public static class ValueObjects
    {
        public static Address CreateValidAddress()
        {
            var street = Faker.Address.StreetAddress();
            var city = Faker.Address.City();
            var state = Faker.Address.State();
            var zipCode = Faker.Address.ZipCode();
            var country = Faker.Address.Country();

            return new Address(street, city, state, zipCode, country);
        }

        public static Money CreateValidMoney()
        {
            var amount = Faker.Random.Decimal(1, 1000);
            var currency = Faker.PickRandom("USD", "EUR", "GBP");
            return new Money(amount, currency);
        }
    }
} 
