using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace AspNetCore.GZip
{

    public class GZipMiddleware
    {
        private readonly RequestDelegate _next;

        public GZipMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString()?.ToLower();
            
            if ((acceptEncoding?.Contains("gzip")??false) || (acceptEncoding?.Contains("deflate")??false))
            {
                var memoryStream = new MemoryStream();
                var originalStream = context.Response.Body;
                context.Response.Body = memoryStream;
                
                await _next(context);

                if (acceptEncoding.Contains("gzip"))
                    using(var gZipStream = new GZipStream(originalStream, CompressionLevel.Optimal))
                    {
                        context.Response.Headers.Add("Content-Encoding", new[] {"gzip"});
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(gZipStream);
                    }
                else
                    using(var deflatestream = new DeflateStream(originalStream, CompressionLevel.Optimal))
                    {
                        context.Response.Headers.Add("Content-Encoding", new[] { "deflate" });
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(deflatestream);
                    }
            }
            else
                await _next(context);
                
        }

    }

    public static class GZipMiddlewareExtension
    {
        public static IApplicationBuilder UseGZipMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GZipMiddleware>();
        }

    }

}