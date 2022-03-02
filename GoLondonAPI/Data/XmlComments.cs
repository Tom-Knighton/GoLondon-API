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
                var xmlFile = $"GoLondonAPI.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                return xmlPath;
            }
        }
    }
}

