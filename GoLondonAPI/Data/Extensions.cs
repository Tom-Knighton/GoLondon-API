
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI
{
    public static class Extensions
    {
        public static string GetValue<T>(this T value) where T : Enum
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value ?? "";
        }

        /// <summary>
        /// Specifies a related entity to include in the query, if the specified condition is met
        /// </summary>
        /// <typeparam name="TEntity">The entity type to include</typeparam>
        /// <param name="source">The query</param>
        /// <param name="condition">The condition to include the entity on</param>
        /// <param name="navigationPropertyPaths">The related entities as paths i.e. s => s.User</param>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeIf<TEntity>([NotNull] this IQueryable<TEntity> source, bool condition, params Expression<Func<TEntity, object>>[] navigationPropertyPaths)
               where TEntity : class
        {
            if (condition)
            {
                if (navigationPropertyPaths != null)
                {
                    foreach (var navigationPropertyPath in navigationPropertyPaths)
                    {
                        source = source.Include(navigationPropertyPath);
                    }
                }
            }
            return source;
        }
    }

    public class JsonModelBinder<T> : IModelBinder where T : Enum
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string rawData = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            rawData = JsonConvert.SerializeObject(rawData); //turns value to valid json
            try
            {
                T result = JsonConvert.DeserializeObject<T>(rawData); //manually deserializing value
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (JsonSerializationException ex)
            {
                //do nothing since "failed" result is set by default
            }


            return Task.CompletedTask;
        }
    }
}

