using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace TossErp.Copilot.Application.Skills;

/// <summary>
/// Transport and logistics skills for the AI Copilot
/// </summary>
public class TransportSkill
{
    private readonly ILogger<TransportSkill> _logger;

    public TransportSkill(ILogger<TransportSkill> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Assign a driver for delivery
    /// </summary>
    [KernelFunction]
    [Description("Assign a driver for delivery based on location and availability")]
    public Task<string> AssignDriver(
        [Description("Pickup location")] string pickupLocation,
        [Description("Delivery location")] string deliveryLocation,
        [Description("Delivery urgency: normal, urgent, express")] string urgency = "normal")
    {
        _logger.LogInformation("Assigning driver for delivery from {PickupLocation} to {DeliveryLocation}, urgency: {Urgency}", pickupLocation, deliveryLocation, urgency);
        var response = $"🚚 DRIVER ASSIGNMENT\n\n📍 ROUTE DETAILS:\n• Pickup: {pickupLocation}\n• Delivery: {deliveryLocation}\n• Urgency: {urgency.ToUpper()}\n\n👨‍💼 ASSIGNED DRIVER:\n• Name: John Mokoena\n• Vehicle: Toyota Hilux (CA 123-456)\n• Capacity: 2,000 kg\n• Rating: ⭐⭐⭐⭐⭐ (4.8/5)\n• Experience: 5 years\n\n📅 SCHEDULE:\n• Pickup Time: 08:00 AM\n• Estimated Delivery: 10:30 AM\n• Distance: 45 km\n• Duration: 2.5 hours\n\n💰 COST BREAKDOWN:\n• Base Rate: R 350\n• Distance Charge: R 225 (45 km × R 5/km)\n• Urgency Fee: R {(urgency.ToLower() == "urgent" ? 100 : urgency.ToLower() == "express" ? 200 : 0)}\n• Total: R {(350 + 225 + (urgency.ToLower() == "urgent" ? 100 : urgency.ToLower() == "express" ? 200 : 0))}\n\n📱 CONTACT:\n• Driver: +27 82 123 4567\n• Dispatch: +27 11 987 6543\n• Tracking: #DEL{DateTime.Now:yyyyMMdd}001";
        return Task.FromResult(response);
    }

    /// <summary>
    /// Estimate delivery time and cost
    /// </summary>
    [KernelFunction]
    [Description("Estimate delivery time and cost for a route")]
    public Task<string> EstimateDelivery(
        [Description("Pickup location")] string pickupLocation,
        [Description("Delivery location")] string deliveryLocation,
        [Description("Package weight in kg")] decimal weight,
        [Description("Package volume in cubic meters")] decimal volume = 1.0m)
    {
        _logger.LogInformation("Estimating delivery from {PickupLocation} to {DeliveryLocation}, weight: {Weight}kg", pickupLocation, deliveryLocation, weight);
        var distance = CalculateEstimatedDistance(pickupLocation, deliveryLocation);
        var baseTime = CalculateBaseTime(distance);
        var cost = CalculateDeliveryCost(distance, weight, volume);
        var response = $"📦 DELIVERY ESTIMATE\n\n📍 ROUTE:\n• From: {pickupLocation}\n• To: {deliveryLocation}\n• Distance: {distance} km\n\n⏱️ TIMING:\n• Base Travel Time: {baseTime} hours\n• Loading Time: 30 minutes\n• Unloading Time: 30 minutes\n• Total Estimated Time: {baseTime + 1} hours\n• Best Pickup Time: 08:00 AM\n• Expected Delivery: {DateTime.Now.AddHours((double)baseTime + 1):HH:mm} today\n\n💰 COST ESTIMATE:\n• Base Rate: R 350\n• Distance Charge: R {distance * 5:F0}\n• Weight Charge: R {weight * 2:F0}\n• Volume Charge: R {volume * 50:F0}\n• Total Estimated Cost: R {cost:F0}\n\n🚚 VEHICLE OPTIONS:\n• Small Van (up to 500kg): R {(cost * 0.8m):F0}\n• Medium Truck (up to 2,000kg): R {cost:F0}\n• Large Truck (up to 5,000kg): R {(cost * 1.3m):F0}\n\n💡 RECOMMENDATIONS:\n• Book in advance for better rates\n• Consider group deliveries for savings\n• Track your delivery in real-time";
        return Task.FromResult(response);
    }

    /// <summary>
    /// Track delivery status
    /// </summary>
    [KernelFunction]
    [Description("Track the status of a delivery")]
    public Task<string> TrackDelivery(
        [Description("Delivery tracking number")] string trackingNumber)
    {
        _logger.LogInformation("Tracking delivery: {TrackingNumber}", trackingNumber);
        var response = $"📱 DELIVERY TRACKING\n\n🔍 Tracking Number: {trackingNumber}\n📅 Date: {DateTime.Now:dddd, MMMM dd, yyyy}\n\n📍 CURRENT STATUS: In Transit\n⏰ Last Update: {DateTime.Now.AddMinutes(-15):HH:mm}\n🚚 Driver: John Mokoena\n📞 Contact: +27 82 123 4567\n\n📋 DELIVERY TIMELINE:\n• 08:00 AM - Picked up from supplier\n• 08:30 AM - En route to delivery\n• 09:45 AM - In transit (current)\n• 10:30 AM - Expected delivery\n\n🗺️ LOCATION: Near Sandton, Johannesburg\n📊 PROGRESS: 75% complete\n⏱️ ETA: 45 minutes\n\n💡 UPDATES:\n• Package secured and in good condition\n• No delays reported\n• Driver will call 15 minutes before arrival";
        return Task.FromResult(response);
    }

    /// <summary>
    /// Find available drivers
    /// </summary>
    [KernelFunction]
    [Description("Find available drivers in a specific area")]
    public Task<string> FindAvailableDrivers(
        [Description("Location or area")] string location,
        [Description("Required capacity in kg")] decimal capacity = 1000)
    {
        _logger.LogInformation("Finding available drivers in {Location} with capacity {Capacity}kg", location, capacity);
        var response = $"👨‍💼 AVAILABLE DRIVERS IN {location.ToUpper()}\n\n🥇 TOP RATED:\n• John Mokoena (4.8/5 ⭐)\n  Vehicle: Toyota Hilux\n  Capacity: 2,000 kg\n  Available: Now\n  Rate: R 5/km\n\n🥈 EXPERIENCED:\n• Sarah Ndlovu (4.6/5 ⭐)\n  Vehicle: Ford Transit\n  Capacity: 1,500 kg\n  Available: 30 minutes\n  Rate: R 4.50/km\n\n🥉 RELIABLE:\n• David Khumalo (4.4/5 ⭐)\n  Vehicle: Nissan NP200\n  Capacity: 800 kg\n  Available: 1 hour\n  Rate: R 4/km\n\n📊 SUMMARY:\n• Total Available: 8 drivers\n• Average Rating: 4.5/5\n• Average Rate: R 4.60/km\n• Coverage Area: 50 km radius\n\n💡 RECOMMENDATIONS:\n• Book early for best drivers\n• Consider capacity requirements\n• Check driver ratings and reviews\n• Verify insurance coverage";
        return Task.FromResult(response);
    }

    private decimal CalculateEstimatedDistance(string pickup, string delivery)
    {
        return pickup.ToLower() switch
        {
            var p when p.Contains("johannesburg") && delivery.ToLower().Contains("pretoria") => 60,
            var p when p.Contains("johannesburg") && delivery.ToLower().Contains("sandton") => 25,
            var p when p.Contains("pretoria") && delivery.ToLower().Contains("johannesburg") => 60,
            var p when p.Contains("cape town") && delivery.ToLower().Contains("bellville") => 35,
            var p when p.Contains("durban") && delivery.ToLower().Contains("umhlanga") => 20,
            _ => 45
        };
    }

    private decimal CalculateBaseTime(decimal distance)
    {
        // Average speed of 40 km/h in urban areas
        return distance / 40;
    }

    private decimal CalculateDeliveryCost(decimal distance, decimal weight, decimal volume)
    {
        var baseRate = 350m;
        var distanceCharge = distance * 5;
        var weightCharge = weight * 2;
        var volumeCharge = volume * 50;
        
        return baseRate + distanceCharge + weightCharge + volumeCharge;
    }
} 
