
using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

