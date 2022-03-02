using System;
using System.Reflection;

namespace GoLondonAPI.Data
{
    public static class XmlComments
    {
        public static string CommentFilePath
        {
            get
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                return xmlPath;
            }
        }
    }
}

