using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;

public class NetworkTest : MonoBehaviour
{
    // Reuse HttpClient for efficiency
    private static readonly HttpClient client = new HttpClient();

    void Start()
    {
        Debug.Log("Starting network connectivity test...");
        // Run the test asynchronously so it doesn't block the main thread
        _ = TestHttpsConnection();
    }

    async Task TestHttpsConnection()
    {
        // Using google.com as a reliable, standard HTTPS endpoint
        string testUrl = "https://www.google.com";

        Debug.Log($"Attempting HTTPS GET request to: {testUrl}");

        try
        {
            // Send a simple GET request. We just want to see if we get a successful response status.
            // Set a reasonable timeout (e.g., 10 seconds)
            using (var cts = new System.Threading.CancellationTokenSource(System.TimeSpan.FromSeconds(10)))
            {
                HttpResponseMessage response = await client.GetAsync(testUrl, cts.Token);

                // Check if the HTTP status code indicates success (2xx range)
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log($"<color=green>SUCCESS!</color> Connected to {testUrl}. Status Code: {response.StatusCode}");
                }
                else
                {
                    // Log error if status code is not success (e.g., 4xx, 5xx)
                    Debug.LogError($"<color=red>FAILED</color> to get successful status from {testUrl}. Status Code: {response.StatusCode}");
                }
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is System.TimeoutException)
        {
            Debug.LogError($"<color=red>FAILED</color> - Connection to {testUrl} timed out.");
        }
        catch (HttpRequestException httpEx)
        {
            // This often catches DNS resolution errors, connection refused, SSL/TLS issues etc.
            Debug.LogError($"<color=red>FAILED</color> - Network Error connecting to {testUrl}. Exception: {httpEx.Message}");
            if (httpEx.InnerException != null)
            {
                // Inner exceptions often contain more specific details (like SSL errors)
                Debug.LogError($"Inner Exception: {httpEx.InnerException.Message}");
            }
        }
        catch (System.Exception ex)
        {
            // Catch any other unexpected errors during the test
            Debug.LogError($"<color=red>FAILED</color> - An unexpected error occurred during network test: {ex.Message}");
        }
    }
}