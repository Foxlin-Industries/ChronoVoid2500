using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

namespace ChronoVoid.Client
{
    [System.Serializable]
    public class NexusRealmDto
    {
        public int id;
        public string name;
        public int nodeCount;
        public int quantumStationSeedRate;
        public bool noDeadNodes;
        public string createdAt;
        public bool isActive;
    }

    [System.Serializable]
    public class CreateRealmRequest
    {
        public string name;
        public int nodeCount;
        public int quantumStationSeedRate;
        public bool noDeadNodes;
    }

    [System.Serializable]
    public class NodeDetailDto
    {
        public int id;
        public int nodeNumber;
        public string realmName;
        public int coordinateX;
        public int coordinateY;
        public bool hasQuantumStation;
        public ConnectedNodeDto[] connectedNodes;
    }

    [System.Serializable]
    public class ConnectedNodeDto
    {
        public int nodeId;
        public int nodeNumber;
        public bool hasQuantumStation;
        public int coordinateX;
        public int coordinateY;
    }

    [System.Serializable]
    public class MoveRequestDto
    {
        public int userId;
        public int fromNodeId;
        public int toNodeId;
    }

    [System.Serializable]
    public class NavigationResultDto
    {
        public bool success;
        public string message;
        public NodeDetailDto currentNode;
        public string timestamp;
    }

    [System.Serializable]
    public class UserLocationDto
    {
        public int userId;
        public string username;
        public int currentNodeId;
        public int currentNodeNumber;
        public int realmId;
        public string realmName;
        public string lastLogin;
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class RegisterRequest
    {
        public string username;
        public string email;
        public string password;
    }

    [System.Serializable]
    public class ForgotPasswordRequest
    {
        public string emailOrUsername;
    }

    [System.Serializable]
    public class AuthResponse
    {
        public int userId;
        public string username;
        public string email;
        public int? currentNodeId;
        public int? realmId;
        public int cargoHolds;
        public string lastLogin;
        public string token;
    }

    [System.Serializable]
    public class JoinRealmRequest
    {
        public int userId;
        public int realmId;
    }

    [System.Serializable]
    public class JoinRealmResponse
    {
        public bool success;
        public string message;
        public int? startingNodeId;
        public NodeDetailDto startingNode;
    }

    public class ApiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        public string apiBaseUrl = "http://localhost:7000"; // Local development server
        
        public static ApiClient Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Realm Management
        public void GetRealms(System.Action<NexusRealmDto[]> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(GetRealmsCoroutine(onSuccess, onError));
        }

        private IEnumerator GetRealmsCoroutine(System.Action<NexusRealmDto[]> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/api/realm"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = 30; // Unity 6 improved timeout handling
                
                var operation = request.SendWebRequest();
                yield return operation;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    try
                    {
                        // Unity 6 improved JSON parsing
                        if (json.StartsWith("["))
                        {
                            // Wrap array in object for Unity JSON parsing
                            string wrappedJson = $"{{\"realms\":{json}}}";
                            var wrapper = JsonUtility.FromJson<RealmArrayWrapper>(wrappedJson);
                            onSuccess?.Invoke(wrapper.realms);
                        }
                        else
                        {
                            onError?.Invoke("Invalid JSON format received");
                        }
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"API Error: {request.error} - Response Code: {request.responseCode}");
                }
            }
        }

        public void CreateRealm(CreateRealmRequest realmData, System.Action<NexusRealmDto> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(CreateRealmCoroutine(realmData, onSuccess, onError));
        }

        private IEnumerator CreateRealmCoroutine(CreateRealmRequest realmData, System.Action<NexusRealmDto> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(realmData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/realm", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var realm = JsonUtility.FromJson<NexusRealmDto>(request.downloadHandler.text);
                        onSuccess?.Invoke(realm);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"API Error: {request.error} - {request.downloadHandler.text}");
                }
            }
        }

        // Navigation
        public void GetNodeDetails(int nodeId, System.Action<NodeDetailDto> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(GetNodeDetailsCoroutine(nodeId, onSuccess, onError));
        }

        private IEnumerator GetNodeDetailsCoroutine(int nodeId, System.Action<NodeDetailDto> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/api/navigation/node/{nodeId}"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var nodeDetail = JsonUtility.FromJson<NodeDetailDto>(request.downloadHandler.text);
                        onSuccess?.Invoke(nodeDetail);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"API Error: {request.error}");
                }
            }
        }

        public void MoveToNode(MoveRequestDto moveData, System.Action<NavigationResultDto> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(MoveToNodeCoroutine(moveData, onSuccess, onError));
        }

        private IEnumerator MoveToNodeCoroutine(MoveRequestDto moveData, System.Action<NavigationResultDto> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(moveData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/navigation/move", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var result = JsonUtility.FromJson<NavigationResultDto>(request.downloadHandler.text);
                        onSuccess?.Invoke(result);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"API Error: {request.error} - {request.downloadHandler.text}");
                }
            }
        }

        public void GetUserLocation(int userId, System.Action<UserLocationDto> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(GetUserLocationCoroutine(userId, onSuccess, onError));
        }

        private IEnumerator GetUserLocationCoroutine(int userId, System.Action<UserLocationDto> onSuccess, System.Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/api/navigation/user/{userId}/location"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var location = JsonUtility.FromJson<UserLocationDto>(request.downloadHandler.text);
                        onSuccess?.Invoke(location);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"API Error: {request.error}");
                }
            }
        }

        // Authentication Methods
        public void Login(LoginRequest loginData, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(LoginCoroutine(loginData, onSuccess, onError));
        }

        private IEnumerator LoginCoroutine(LoginRequest loginData, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(loginData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/auth/login", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                        onSuccess?.Invoke(authResponse);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Login failed: {request.downloadHandler.text}");
                }
            }
        }

        public void Register(RegisterRequest registerData, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(RegisterCoroutine(registerData, onSuccess, onError));
        }

        private IEnumerator RegisterCoroutine(RegisterRequest registerData, System.Action<AuthResponse> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(registerData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/auth/register", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var authResponse = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                        onSuccess?.Invoke(authResponse);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Registration failed: {request.downloadHandler.text}");
                }
            }
        }

        public void ForgotPassword(ForgotPasswordRequest forgotData, System.Action<string> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(ForgotPasswordCoroutine(forgotData, onSuccess, onError));
        }

        private IEnumerator ForgotPasswordCoroutine(ForgotPasswordRequest forgotData, System.Action<string> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(forgotData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/auth/forgot-password", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    onSuccess?.Invoke("Password reset successful. Check your email for the temporary password.");
                }
                else
                {
                    onError?.Invoke($"Password reset failed: {request.downloadHandler.text}");
                }
            }
        }

        public void JoinRealm(JoinRealmRequest joinData, System.Action<JoinRealmResponse> onSuccess, System.Action<string> onError)
        {
            StartCoroutine(JoinRealmCoroutine(joinData, onSuccess, onError));
        }

        private IEnumerator JoinRealmCoroutine(JoinRealmRequest joinData, System.Action<JoinRealmResponse> onSuccess, System.Action<string> onError)
        {
            string json = JsonUtility.ToJson(joinData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/api/auth/join-realm", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var joinResponse = JsonUtility.FromJson<JoinRealmResponse>(request.downloadHandler.text);
                        onSuccess?.Invoke(joinResponse);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"JSON Parse Error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Join realm failed: {request.downloadHandler.text}");
                }
            }
        }

        [System.Serializable]
        private class RealmArrayWrapper
        {
            public NexusRealmDto[] realms;
        }
    }
}