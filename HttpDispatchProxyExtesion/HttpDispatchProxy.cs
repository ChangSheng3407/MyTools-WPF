using HttpDispatchProxyExtesion.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HttpDispatchProxyExtension
{
    public class HttpDispatchProxy : DispatchProxy, IHttpDispatchProxy
    {
        private HttpDispatchProxyOption Option;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            lock (Option)
            {
                if (targetMethod == null) throw new ArgumentNullException(nameof(targetMethod));
                if (targetMethod.ReturnType == typeof(Task) || targetMethod.ReturnType == typeof(Task<>)) throw new NotImplementedException("Task返回类型暂未实现");
                object result = null;
                Task.Run(async () =>
                {
                    result = await SendHttpRequestAsync(targetMethod, args);
                }).Wait();
                return result;
            }
        }

        private async Task<object> SendHttpRequestAsync(MethodInfo? targetMethod, object?[]? args)
        {
            try
            {
                var encoding = targetMethod.GetCustomAttribute<EncodingAttribute>();
                var requestBase = targetMethod.GetCustomAttribute<BaseHttpMethod>();
                CheckParameter(targetMethod.GetParameters());
                using (HttpClient client = new HttpClient())
                {
                    var requestMessage = BuildRequestMessage(requestBase, targetMethod.GetParameters(), args);
                    var response = await client.SendAsync(requestMessage);
                    response.EnsureSuccessStatusCode();
                    string responseString = string.Empty;
                    var buffer = await response.Content.ReadAsByteArrayAsync();
                    if (encoding != null)
                    {
                        responseString = encoding.Encoding.GetString(buffer);
                    }
                    else
                    {
                        responseString = Encoding.Default.GetString(buffer);
                    }
                    if (targetMethod.ReturnType == typeof(string))
                    {
                        return responseString;
                    }
                    return JsonConvert.DeserializeObject(responseString, targetMethod.ReturnType);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private HttpRequestMessage BuildRequestMessage(BaseHttpMethod? requestBase, ParameterInfo[] parameterInfos, object?[]? args)
        {
            var (routePathParameters, queryParameters, formParameters, bodyParameters) = BuildRequestParameters(parameterInfos, args);
            var url = Option.BaseUrl;
            if (!string.IsNullOrEmpty(routePathParameters))
            {
                url = Path.Join(url, routePathParameters);
            }
            if (!string.IsNullOrEmpty(queryParameters))
            {
                url += $"?{queryParameters}";
            }
            var requestMessage = new HttpRequestMessage(requestBase.Method, url);
            return requestMessage;
        }

        /// <summary>
        /// 构建请求参数
        /// </summary>
        /// <param name="parameterInfos"></param>
        /// <param name="args"></param>
        /// <returns>(RoutePath参数,Query参数,Form参数,Body参数)</returns>
        private (string routePathParam, string queryParam, Dictionary<string, object> formParam, string bodyParam) BuildRequestParameters(ParameterInfo[] parameterInfos, object?[]? args)
        {
            List<string> routePathParam = new List<string>();
            List<string> queryParam = new List<string>();
            Dictionary<string, object> formDataParam = new Dictionary<string, object>();
            string bodyParam = "";

            ParameterInfo parameterInfo;
            object? arg;
            CustomAttributeData attribute;

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                parameterInfo = parameterInfos[i];
                arg = args[i];
                if (parameterInfo.CustomAttributes.Count() > 1) throw new Exception($"参数{parameterInfo.Name}包含太多特性");
                attribute = parameterInfo.CustomAttributes.FirstOrDefault();
                if (attribute == null) continue;
                switch (attribute.AttributeType.Name)
                {
                    case nameof(RoutePathAttribute):
                        routePathParam.Add(arg.ToString());
                        break;
                    case nameof(QueryAttribute):
                        BuildQueryData(queryParam, parameterInfo, arg);
                        break;
                    case nameof(FormDataAttribute):
                        BuildFormData(formDataParam, parameterInfo, arg);
                        break;
                    case nameof(BodyAttribute):
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return (string.Join("/", routePathParam), string.Join("&", queryParam), formDataParam, bodyParam);
        }

        private static void CheckParameter(ParameterInfo[] parameterInfos)
        {
            var groupByAttribute = parameterInfos.SelectMany(o => o.CustomAttributes).GroupBy(o => o.AttributeType.Name, (o, p) => new { TypeName = o, Count = p.Count() });
            var validAttribute = groupByAttribute.Where(o => o.Count > 1 && (o.TypeName == nameof(FormDataAttribute) || o.TypeName == nameof(BodyAttribute))).Any();
            if (validAttribute) throw new ArgumentException("不允许出现重复的Body或FormData参数");
        }

        /// <summary>
        /// 构建FormData
        /// </summary>
        /// <param name="formParam"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="arg"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void BuildFormData(Dictionary<string, object> formDataParam, ParameterInfo parameterInfo, object? arg)
        {
            if (arg is not IEnumerable<KeyValuePair<string, object>>) throw new ArgumentException("参数类型需要是IEnumerable<KeyValuePair<string, object>>或者Dictionary<string, object>");
            foreach (var item in (IEnumerable<KeyValuePair<string, object>>)arg)
            {
                formDataParam.Add(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 构建QueryData
        /// </summary>
        /// <param name="queryDataParam"></param>
        /// <param name="parameterInfo"></param>
        /// <param name="arg"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuildQueryData(List<string> queryDataParam, ParameterInfo parameterInfo, object? arg)
        {
            if (arg is not IEnumerable<KeyValuePair<string, object>>) throw new ArgumentException("参数类型需要是IEnumerable<KeyValuePair<string, object>>或者Dictionary<string, object>");
            foreach (var item in (IEnumerable<KeyValuePair<string, object>>)arg)
            {
                queryDataParam.Add($"{item.Key}={item.Value}");
            }
        }

        public static T CreateProxy<T>(HttpDispatchProxyOption option) where T : IHttpDispatchProxy
        {
            var proxy = Create<T, HttpDispatchProxy>();
            (proxy as HttpDispatchProxy).Option = option;
            return proxy;
        }
        public static T CreateProxy<T>(string baseUrl, Dictionary<string, object> headers = null) where T : IHttpDispatchProxy
        {
            var option = new HttpDispatchProxyOption
            {
                BaseUrl = baseUrl,
                Headers = headers,
            };
            return CreateProxy<T>(option);
        }
    }
}