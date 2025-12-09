using OrderIngestion.Application.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace OrderIngestion.Application.Validators;

public static class OrderValidator
{
    public static OrderIngestion.Application.Models.ValidationResult Validate(OrderRequest request)
    {
        var result = new OrderIngestion.Application.Models.ValidationResult();

        if (!Guid.TryParse(request.RequestId, out _))
            result.Errors.Add("Invalid RequestId");

        if (string.IsNullOrWhiteSpace(request.OrderNumber))
            result.Errors.Add("OrderNumber is required");

        if (request.Customer == null)
            result.Errors.Add("Customer is required");
        else
        {
            if (string.IsNullOrWhiteSpace(request.Customer.Name))
                result.Errors.Add("Customer name is required");

            if (!Regex.IsMatch(request.Customer.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                result.Errors.Add("Invalid email format");
        }

        if (request.Items == null || request.Items.Count == 0)
            result.Errors.Add("At least one item is required");

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0) result.Errors.Add("Quantity must be > 0");
            if (item.Price <= 0) result.Errors.Add("Price must be > 0");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}
