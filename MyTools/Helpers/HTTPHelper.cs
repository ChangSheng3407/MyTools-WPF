using Masuit.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Helpers
{
    public class HTTPHelper
    {
        #region 基础方法

        private readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// 发送GET请求到指定的URL，并附带给定的请求头和查询参数。
        /// </summary>
        /// <param name="url">要发送GET请求的URL。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns>响应内容作为指定类型的对象返回。</returns>
        public async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null, string EncodingFormat = null)
        {
            try
            {
                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // 确保请求成功
                byte[] contentBytes = await response.Content.ReadAsByteArrayAsync();
                string responseContent = "";
                if (!EncodingFormat.IsNullOrEmpty())
                {
                    responseContent = Encoding.GetEncoding(EncodingFormat).GetString(contentBytes); // 使用指定编码读取字节数组
                }
                else
                {
                    responseContent = Encoding.UTF8.GetString(contentBytes); // 使用UTF-8编码读取字节数组
                }
                if (typeof(T) == typeof(string)) return (T)(object)responseContent; // 如果指定类型为string，则直接返回响应内容
                return JsonConvert.DeserializeObject<T>(responseContent); // 反序列化响应内容为指定类型
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("发送GET请求时发生错误。", ex);
            }
        }

        /// <summary>
        /// 发送POST Json请求到指定的URL，并附带给定的内容、请求头和查询参数。
        /// </summary>
        /// <param name="url">要发送POST请求的URL。</param>
        /// <param name="content">要在POST请求中发送的内容。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns>响应内容作为指定类型的对象返回。</returns>
        public async Task<T> PostJsonAsync<T>(string url, string content, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json"); // 创建HTTP内容

                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.PostAsync(url, httpContent); // 发送POST请求
                response.EnsureSuccessStatusCode(); // 确保请求成功
                string responseContent = await response.Content.ReadAsStringAsync(); // 读取响应内容
                return JsonConvert.DeserializeObject<T>(responseContent); // 反序列化响应内容为指定类型
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("发送POST请求时发生错误。", ex);
            }
        }

        /// <summary>
        /// 发送POST Form请求到指定的URL，并附带给定的内容、请求头和查询参数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">要发送POST请求的URL。</param>
        /// <param name="content">要在POST请求中发送的内容。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<T> PostFormAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> content, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                HttpContent httpContent = new FormUrlEncodedContent(content); // 创建HTTP内容

                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.PostAsync(url, httpContent); // 发送POST请求
                response.EnsureSuccessStatusCode(); // 确保请求成功
                string responseContent = await response.Content.ReadAsStringAsync(); // 读取响应内容
                return JsonConvert.DeserializeObject<T>(responseContent); // 反序列化响应内容为指定类型
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("发送POST请求时发生错误。", ex);
            }
        }

        /// <summary>
        /// 发送PUT请求到指定的URL，并附带给定的内容、请求头和查询参数。
        /// </summary>
        /// <param name="url">要发送PUT请求的URL。</param>
        /// <param name="content">要在PUT请求中发送的内容。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns>响应内容作为指定类型的对象返回。</returns>
        public async Task<T> PutAsync<T>(string url, string content, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json"); // 创建HTTP内容

                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.PutAsync(url, httpContent); // 发送PUT请求
                response.EnsureSuccessStatusCode(); // 确保请求成功
                string responseContent = await response.Content.ReadAsStringAsync(); // 读取响应内容
                return JsonConvert.DeserializeObject<T>(responseContent); // 反序列化响应内容为指定类型
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("发送PUT请求时发生错误。", ex);
            }
        }

        /// <summary>
        /// 发送DELETE请求到指定的URL，并附带给定的请求头和查询参数。
        /// </summary>
        /// <param name="url">要发送DELETE请求的URL。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns>响应内容作为指定类型的对象返回。</returns>
        public async Task<T> DeleteAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.DeleteAsync(url); // 发送DELETE请求
                response.EnsureSuccessStatusCode(); // 确保请求成功
                string responseContent = await response.Content.ReadAsStringAsync(); // 读取响应内容
                return JsonConvert.DeserializeObject<T>(responseContent); // 反序列化响应内容为指定类型
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("发送DELETE请求时发生错误。", ex);
            }
        }

        /// <summary>
        /// 上传文件到指定的URL，并附带给定的请求头和查询参数。
        /// </summary>
        /// <param name="url">要上传文件的URL。</param>
        /// <param name="filePath">要上传的文件路径。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UploadFileAsync(string url, string filePath, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new FileStream(filePath, FileMode.Open)), "file", Path.GetFileName(filePath));

                    AddHeaders(headers);
                    AddQueryParams(ref url, queryParams);

                    HttpResponseMessage response = await _client.PostAsync(url, content); // 发送POST请求上传文件
                    response.EnsureSuccessStatusCode(); // 确保请求成功
                    return response; // 返回响应对象，可以根据需要进一步处理或检查响应状态码等。
                }
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("上传文件时发生错误。", ex);
            }
        }

        /// <summary>
        /// 下载文件从指定的URL，并附带给定的请求头和查询参数。
        /// </summary>
        /// <param name="url">要下载文件的URL。</param>
        /// <param name="headers">要在请求中包含的请求头。</param>
        /// <param name="queryParams">要在请求中包含的查询参数。</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFileAsync(string url, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null)
        {
            try
            {
                AddHeaders(headers);
                AddQueryParams(ref url, queryParams);

                HttpResponseMessage response = await _client.GetAsync(url); // 发送GET请求下载文件
                response.EnsureSuccessStatusCode(); // 确保请求成功
                return await response.Content.ReadAsByteArrayAsync(); // 读取响应内容为字节数组并返回
            }
            catch (Exception ex)
            {
                // 记录异常或根据需要处理它
                throw new Exception("下载文件时发生错误。", ex);
            }
        }

        /// <summary>
        /// 将查询参数添加到指定的URL中。如果存在查询参数，则将其以适当的格式附加到URL后。
        /// </summary>
        /// <param name="url">要添加查询参数的URL引用。</param>
        /// <param name="queryParams">要添加到URL中的查询参数字典。</param>
        private void AddQueryParams(ref string url, Dictionary<string, string> queryParams)
        {
            if (queryParams != null && queryParams.Count > 0)
            {
                url += "?" + BuildQueryString(queryParams);
            }
        }

        /// <summary>
        /// 将给定的请求头添加到HttpClient的默认请求头中。
        /// </summary>
        /// <param name="headers">要添加到HttpClient默认请求头的字典，包含键值对形式的请求头信息。</param>
        private void AddHeaders(Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// 构建查询字符串。
        /// </summary>
        /// <param name="queryParams">查询参数字典。</param>
        /// <returns></returns>
        private string BuildQueryString(Dictionary<string, string> queryParams)
        {
            return string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        }

        #endregion
        #region 常用的HTTP方法
        /// <summary>
        /// 获取公网IP地址
        /// </summary>
        /// <returns></returns>
        public string GetPublicIPAddressAsync()
        {
            var response = GetAsync<Dictionary<string, string>>("https://httpbin.org/ip").Result;
            return response["origin"];
        }
        #endregion
    }
}
