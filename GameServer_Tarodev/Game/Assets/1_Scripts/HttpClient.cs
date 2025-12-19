using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// using으로 네임스페이스 넣고, get, post 함수 호출해서 사용하기
// 기본 타임아웃 판정은 3초
namespace _1_Scripts
{
    public class HttpClient
    {
        private const int DEFAULT_TIMEOUT_SECONDS = 3;

        public static async Task<T> Get<T>(string endpoint, int timeoutSeconds = DEFAULT_TIMEOUT_SECONDS)
        {
            using var request = UnityWebRequest.Get(endpoint);
            request.downloadHandler = new DownloadHandlerBuffer();
            return await SendRequest<T>(request, timeoutSeconds);
        }

        public static async Task<T> Post<T>(string endpoint, object payload, int timeoutSeconds = DEFAULT_TIMEOUT_SECONDS)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(payload));
            using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return await SendRequest<T>(request, timeoutSeconds);
        }
        
        private static async Task<T> SendRequest<T>(UnityWebRequest request, int timeoutSeconds)
        {
            var operation = request.SendWebRequest();
            float startTime = Time.time;

            while (!operation.isDone)
            {
                if (Time.time - startTime > timeoutSeconds)
                {
                    Debug.LogError("Request timed out");
                    return default;
                }
                
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Request failed: {request.error}");
                return default;
            }

            return JsonUtility.FromJson<T>(request.downloadHandler.text);
        }
    }
}