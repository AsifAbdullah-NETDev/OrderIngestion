using Microsoft.OpenApi.Models;
using OrderIngestion.Application.Models;
using OrderIngestionService;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace OrderIngestion.Api.Swagger
{
    public class DTOsSchemaFilter : ISchemaFilter
    {
        private static readonly Type[] TypesToInclude =
        {
            typeof(OrderDTO),
            typeof(CustomerDTO),
            typeof(OrderItemDTO),
            typeof(OrderRequest),
            typeof(ValidationResult),
            typeof(WeatherForecast)
        };

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            foreach (var type in TypesToInclude)
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            }
        }
    }
}
