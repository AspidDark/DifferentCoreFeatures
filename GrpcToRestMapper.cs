using System;

public static class GrpcToRestMapper
{
    public static List<GrpcMethodInfo> GetGrpcMethods(Type grpcServiceType)
    {
        var methodsInfo = new List<GrpcMethodInfo>();

        var methods = grpcServiceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            string methodName = method.Name;
            string returnType = "Unknown";
            Type? returnTypeObject = null;
            string genericType = "None";
            Type? genericTypeObject = null;
            bool isStream = false; // ✅ Default to false, will be set to true if it's a stream

            // Extract request parameter types (excluding ServerCallContext)
            List<string> parameterTypes = new();
            List<Type> parameterTypeObjects = new();

            foreach (var param in parameters)
            {
                if (!typeof(ServerCallContext).IsAssignableFrom(param.ParameterType))
                {
                    parameterTypes.Add(param.ParameterType.Name);
                    parameterTypeObjects.Add(param.ParameterType);
                }
            }

            var returnTypeInfo = method.ReturnType;

            // 1️⃣ **Streaming Response (First parameter is IServerStreamWriter<T>)**
            if (parameters.Length > 1 && parameters[1].ParameterType.IsGenericType &&
                parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(IServerStreamWriter<>))
            {
                var streamType = parameters[1].ParameterType.GetGenericArguments()[0];
                returnType = $"Stream<{streamType.Name}>";
                returnTypeObject = streamType;
                genericType = streamType.Name;
                genericTypeObject = streamType;
                isStream = true; // ✅ Mark as a streaming method
            }
            // 2️⃣ **Unary Response (Returns Task<T>)**
            else if (returnTypeInfo.IsGenericType && returnTypeInfo.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var responseType = returnTypeInfo.GetGenericArguments()[0];
                returnType = responseType.Name;
                returnTypeObject = responseType;
                genericType = responseType.Name;
                genericTypeObject = responseType;
                isStream = false; // ✅ Not a streaming method
            }
            // 3️⃣ **Streaming method returning Task instead of Task<T>**
            else if (returnTypeInfo == typeof(Task) && parameters.Length > 1 &&
                     parameters[1].ParameterType.IsGenericType &&
                     parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(IServerStreamWriter<>))
            {
                var streamType = parameters[1].ParameterType.GetGenericArguments()[0];
                returnType = $"Stream<{streamType.Name}>";
                returnTypeObject = streamType;
                genericType = streamType.Name;
                genericTypeObject = streamType;
                isStream = true; // ✅ Mark as a streaming method
            }
            // 4️⃣ **Unary method returning Task (void equivalent in async)**
            else if (returnTypeInfo == typeof(Task))
            {
                returnType = "Void";
                returnTypeObject = typeof(void);
                isStream = false; // ✅ Not a streaming method
            }

            methodsInfo.Add(new GrpcMethodInfo
            {
                MethodName = methodName,
                ReturnType = returnType,
                ReturnTypeObject = returnTypeObject,
                GenericType = genericType,
                GenericTypeObject = genericTypeObject,
                ParameterTypes = parameterTypes,
                ParameterTypeObjects = parameterTypeObjects,
                IsStream = isStream // ✅ Set whether it's a streaming method
            });
        }

        return methodsInfo;
    }
}

public class GrpcMethodInfo
{
    public string MethodName { get; set; } = string.Empty;
    public string ReturnType { get; set; } = "Unknown";  // Full return type (string)
    public Type? ReturnTypeObject { get; set; }         // Return Type as Type
    public string GenericType { get; set; } = "None";   // Extracted generic type T (string)
    public Type? GenericTypeObject { get; set; }        // Generic Type <T> as Type
    public List<string> ParameterTypes { get; set; } = new(); // List of input types as strings
    public List<Type> ParameterTypeObjects { get; set; } = new(); // List of input types as Type
    public bool IsStream { get; set; } // ✅ True if it's a streaming method, False otherwise
}