using Dapper;

namespace PetProject
{
    public class Utility
    {
        public static DynamicParameters CreateParamettersFromEntity<TEntity>(TEntity entity)
        {
            var parameters = new DynamicParameters();
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyName = $"@{property.Name}";
                var propertyValue = property.GetValue(entity);
                parameters.Add(propertyName, propertyValue);
            }
            return parameters;
        }
    }
}
