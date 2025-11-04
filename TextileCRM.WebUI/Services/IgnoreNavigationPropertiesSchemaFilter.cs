using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using System.Reflection;

namespace TextileCRM.WebUI.Services
{
    public class IgnoreNavigationPropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context?.Type == null)
                return;

            try
            {
                var excludedProperties = context.Type.GetProperties()
                    .Where(p => 
                        p.GetCustomAttribute<JsonIgnoreAttribute>() != null ||
                        IsNavigationProperty(p));

                foreach (var excludedProperty in excludedProperties)
                {
                    var propertyName = char.ToLowerInvariant(excludedProperty.Name[0]) + excludedProperty.Name.Substring(1);
                    
                    if (schema.Properties.ContainsKey(propertyName))
                    {
                        schema.Properties.Remove(propertyName);
                    }
                    
                    // Pascal case'i de kontrol et
                    if (schema.Properties.ContainsKey(excludedProperty.Name))
                    {
                        schema.Properties.Remove(excludedProperty.Name);
                    }
                }
            }
            catch
            {
                // Hata durumunda sessizce devam et
            }
        }

        private bool IsNavigationProperty(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            
            // ICollection, IEnumerable gibi koleksiyon tipleri
            if (propertyType.IsGenericType)
            {
                var genericType = propertyType.GetGenericTypeDefinition();
                if (genericType == typeof(ICollection<>) || 
                    genericType == typeof(IEnumerable<>) ||
                    genericType == typeof(List<>))
                {
                    return true;
                }
            }
            
            // TextileCRM.Domain.Entities namespace'inden gelen entity tipleri
            if (propertyType.Namespace != null && 
                propertyType.Namespace.StartsWith("TextileCRM.Domain.Entities"))
            {
                return true;
            }
            
            return false;
        }
    }
}

