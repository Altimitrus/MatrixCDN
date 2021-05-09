﻿using Microsoft.AspNetCore.Builder;

namespace MatrixCDN.Engine.Middlewares
{
    public static class Extensions
    {
        public static IApplicationBuilder UseModHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ModHeaders>();
        }
    }
}
