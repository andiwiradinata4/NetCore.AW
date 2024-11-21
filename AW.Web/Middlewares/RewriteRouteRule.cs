using Microsoft.AspNetCore.Rewrite;

namespace AW.Web.Middlewares
{
    public class RewriteRouteRule
    {
        public static void ReWriteRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (request.Path.Value.Contains("//"))
            {
                string[] splitlist = request.Path.Value.Split("/");
                var newarray = splitlist.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                var newpath = "";

                foreach (var item in newarray)
                {
                    newpath += "/" + item;
                }
                request.Path = newpath;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
