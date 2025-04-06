using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class ChatGPTManager : MonoBehaviour
{
    private const string firebaseFunctionUrl = "https://us-central1-ucatapp-7f24d.cloudfunctions.net/getConfigValue";
    private string apiKey = ""; 
    private string assistantId = ""; 
    private string threadId;

    private string threadEndpoint = "https://api.openai.com/v1/threads";
    private string messageEndpointTemplate = "https://api.openai.com/v1/threads/{0}/messages";
    private string runEndpointTemplate = "https://api.openai.com/v1/threads/{0}/runs";
    private string runStatusEndpointTemplate = "https://api.openai.com/v1/threads/{0}/runs/{1}";
    private string messagesEndpointTemplate = "https://api.openai.com/v1/threads/{0}/messages";

    private async void Start()
    {
        await FetchAllKeys();
        LoadOrCreateThread();
    }


    private void LoadOrCreateThread()
    {
        StartCoroutine(CreateThread());
    }

    private IEnumerator CreateThread()
    {
        using (UnityWebRequest request = new UnityWebRequest(threadEndpoint, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<ThreadResponse>(request.downloadHandler.text);
                threadId = response.Id; // Save thread ID locally
                Debug.Log("Created and saved new thread: " + threadId);
            }
            else
            {
                Debug.LogError("Error creating thread: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }

    public void AskChatGPT(string userMessage, System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(threadId))
        {
            Debug.LogError("Thread ID is missing! Restarting session.");
            LoadOrCreateThread();
            return;
        }

        StartCoroutine(SendMessageToAssistant(userMessage, callback));
    }

    private IEnumerator SendMessageToAssistant(string userMessage, System.Action<string> callback)
    {
        string messageEndpoint = string.Format(messageEndpointTemplate, threadId);

        // Modify user message to explicitly ask for JSON response
        string modifiedUserMessage = userMessage;

        string messageJson = JsonConvert.SerializeObject(new
        {
            role = "user",
            content = modifiedUserMessage
        });

        using (UnityWebRequest request = new UnityWebRequest(messageEndpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(messageJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending message: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                callback?.Invoke(null);
                yield break;
            }
        }

        StartCoroutine(StartRun(callback));
    }


    private IEnumerator StartRun(System.Action<string> callback)
    {
        string runEndpoint = string.Format(runEndpointTemplate, threadId);

        string runJson = JsonConvert.SerializeObject(new
        {
            assistant_id = assistantId 
        });

        using (UnityWebRequest request = new UnityWebRequest(runEndpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(runJson);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<RunResponse>(request.downloadHandler.text);
                StartCoroutine(CheckRunStatus(response.Id, callback));
            }
            else
            {
                Debug.LogError("Error starting assistant run: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                callback?.Invoke(null);
            }
        }
    }

    private async Task FetchAllKeys()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(firebaseFunctionUrl))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                KeyResponse data = JsonUtility.FromJson<KeyResponse>(request.downloadHandler.text);
                apiKey = data.openai_key;
                assistantId = data.verbalReasoningAssistantId;
            }
            else
            {
                Debug.LogError("Failed to fetch keys: " + request.error);
            }
        }
    }


    private IEnumerator CheckRunStatus(string runId, System.Action<string> callback)
    {
        string runStatusEndpoint = string.Format(runStatusEndpointTemplate, threadId, runId);

        while (true)
        {
            using (UnityWebRequest request = new UnityWebRequest(runStatusEndpoint, "GET"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);
                request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<RunStatusResponse>(request.downloadHandler.text);
                    if (response.Status == "completed")
                    {
                        StartCoroutine(FetchAssistantResponse(callback));
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError("Error checking run status: " + request.error);
                    callback?.Invoke(null);
                    yield break;
                }
            }

            yield return new WaitForSeconds(0f);
        }
    }

    private IEnumerator FetchAssistantResponse(System.Action<string> callback)
    {
        string messagesEndpoint = string.Format(messagesEndpointTemplate, threadId);

        using (UnityWebRequest request = new UnityWebRequest(messagesEndpoint, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // Deserialize into MessagesResponse
                    MessagesResponse response = JsonConvert.DeserializeObject<MessagesResponse>(request.downloadHandler.text);

                    if (response != null && response.Messages != null && response.Messages.Count > 0)
                    {
                        // Find the last assistant message
                        foreach (var message in response.Messages)
                        {
                            if (message.Role == "assistant")
                            {
                                foreach (var content in message.Content)
                                {
                      
                                    if (content.Type == "text")
                                    {
                                        string assistantResponse = content.Text.Value;
                                        Debug.Log("Assistant Response: " + assistantResponse);
                                        callback?.Invoke(assistantResponse);
                                        yield break; // Exit after processing the first assistant message
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("No messages received from OpenAI.");
                        callback?.Invoke("No response received from Assistant.");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON response: " + e.Message);
                    callback?.Invoke("Error retrieving response from Assistant.");
                }
            }
            else
            {
                Debug.LogError("Error fetching assistant response: " + request.error);
                callback?.Invoke("Error retrieving response from Assistant.");
            }
        }
    }



    [Serializable]
    public class MessagesResponse
    {
        [JsonProperty("object")]
        public string ObjectType { get; set; }

        [JsonProperty("data")]
        public List<MessageData> Messages { get; set; }

        [JsonProperty("first_id")]
        public string FirstId { get; set; }

        [JsonProperty("last_id")]
        public string LastId { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }

    [Serializable]
    public class MessageData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public List<MessageContent> Content { get; set; }
    }

    [Serializable]
    public class MessageContent
    {
        [JsonProperty("type")]
        public string Type { get; set; } 

        [JsonProperty("text")]
        public MessageText Text { get; set; }
    }

    [Serializable]
    public class MessageText
    {
        [JsonProperty("value")]
        public string Value { get; set; } 

        [JsonProperty("annotations")]
        public List<object> Annotations { get; set; }
    }

    [Serializable]
    public class RunStatusResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }

        [JsonProperty("started_at")]
        public int? StartedAt { get; set; }

        [JsonProperty("completed_at")]
        public int? CompletedAt { get; set; }

        [JsonProperty("expires_at")]
        public int? ExpiresAt { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("assistant_id")]
        public string AssistantId { get; set; }
    }

    [Serializable]
    public class RunResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } 

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("assistant_id")]
        public string AssistantId { get; set; }

        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }
    }

    [Serializable]
    public class ThreadResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    [System.Serializable]
    public class KeyResponse
    {
        public string openai_key;
        public string verbalReasoningAssistantId;
        public string decisionMakingAssistantId;
        public string sitJudgeAssistantId;
        public string quantJudgeAssistantId;

    }
}
