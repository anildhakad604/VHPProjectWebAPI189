using System.Diagnostics;
using System.Text;
using System.Text.Json;
using VHPProjectCommonUtility.Logger;

namespace STEIWebAPI.Helper
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _loggerManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        //IEmailRepository? _emailRepository;
        private string requestInfo = string.Empty;
        public GlobalExceptionMiddleware(RequestDelegate next,ILoggerManager loggerManager, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _loggerManager = loggerManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task InvokeAsync(HttpContext context //,IEmailRepository emailRepository
            )
        {
            //_emailRepository = emailRepository;
            try
            {
                requestInfo = await GetRequestDetailsAsync(context);
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string sourceInfo = GetExceptionSourceDetails(ex);

            string systemErrorMessage =
                $"{sourceInfo}: Exception Message: {ex.Message}\n" +
                $"Stack Trace: {ex.StackTrace}\n" +
                $"Inner Exception: {ex.InnerException?.Message}\n" +
                $"{requestInfo}";

            string businessErrorMessage =
                "An unexpected error occurred while processing the request. Kindly retry or contact the System Administrator.";

            //_loggerManager.LogError(systemErrorMessage);
            var routeData = context.GetRouteData();

            string? controllerName = routeData?.Values["controller"]?.ToString();
            string? actionName = routeData?.Values["action"]?.ToString();
            //_emailRepository?.SendEmail(systemErrorMessage, $"{controllerName}/{actionName}");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            object errorResult;
            if (_webHostEnvironment.IsDevelopment())
            {
                errorResult = new
                {
                    IsBusinessError = true,
                    BusinessErrorMessage = businessErrorMessage,
                    SystemErrorMessage = systemErrorMessage
                };
            }
            else 
            {
                errorResult = new
                {
                    IsBusinessError = true,
                    BusinessErrorMessage = businessErrorMessage,
                };
            }
            context.Response.StatusCode = 400; 
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResult));
        }

        private async Task<string> GetRequestDetailsAsync(HttpContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"HTTP Method: {context.Request.Method}");
            sb.AppendLine($"Endpoint: {context.Request.Path}");

            if (context.Request.Method == HttpMethods.Get)
            {
                var parameters = new Dictionary<string, object>();

                // Capture route parameters (excluding controller/action)
                foreach (var (key, value) in context.Request.RouteValues)
                {
                    if (key is "controller" or "action") continue;
                    parameters[key] = value;
                }

                // Capture query parameters
                foreach (var (key, value) in context.Request.Query)
                {
                    parameters[key] = value.Count == 1
                        ? (object)value.ToString()
                        : value.ToArray();
                }

                sb.AppendLine("Request Parameters:");
                sb.Append(FormatParameters(parameters));
            }
            else
            {
                string body = string.Empty;
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0;
                using (var memoryStream = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Read the content as a string
                    body = Encoding.UTF8.GetString(memoryStream.ToArray());
                }

                // Reset the request body stream position for further middleware
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                sb.AppendLine("Request Body:");
                sb.AppendLine(string.IsNullOrWhiteSpace(body) ? "[empty]" : body);
            }
            return sb.ToString();
        }

        private static string FormatParameters(Dictionary<string, object> parameters)
        {
            if (parameters.Count == 0) return "{ }";

            var paramStrings = new List<string>();
            foreach (var (key, value) in parameters)
            {
                var formattedValue = value switch
                {
                    string[] array => string.Join(",", array),
                    _ => value.ToString()
                };
                paramStrings.Add($"{key}: {formattedValue}");
            }

            return $"{{ {string.Join(", ", paramStrings)} }}";
        }

        private string GetExceptionSourceDetails(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var frames = st.GetFrames() ?? Array.Empty<StackFrame>();

            // Add your application's root namespace(s) here
            var userNamespaces = new[] { "MasterProjBAL", "MasterProjWebAPI", "MasterProjDAL" };

            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                var declaringType = method?.DeclaringType;

                if (declaringType == null) continue;

                // Check if the type belongs to your application's namespace
                if (userNamespaces.Any(ns => declaringType.FullName?.StartsWith(ns) == true))
                {
                    var methodName = method.Name;
                    var className = declaringType.Name;

                    // Handle async state machine methods
                    if (methodName == "MoveNext" && declaringType.Name.StartsWith("<"))
                    {
                        var originalMethod = declaringType.Name.TrimStart('<').Split('>')[0];
                        var parentClass = declaringType.DeclaringType?.Name ?? className;
                        return $"{parentClass} => {originalMethod}";
                    }

                    return $"{className} => {methodName}";
                }
            }

            // Fallback to first frame that's not in system namespaces
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                var declaringType = method?.DeclaringType;
                if (declaringType == null) continue;

                if (!declaringType.FullName.StartsWith("System.") &&
                    !declaringType.FullName.StartsWith("Microsoft."))
                {
                    return $"{declaringType.Name} => {method.Name}";
                }
            }

            return "UnknownClass => UnknownMethod";
        }
    }
}

