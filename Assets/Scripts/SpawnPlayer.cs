using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab; //create a prefab of the player
    public string spawnPointTag = "SpawnPoint";

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        //Find all GameObjects with the specified tag
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(spawnPointTag);

        //Check if any spawn points were found
        if (spawnPoints.Length > 0)
        {
            //For simplicity, we just pick the first spawn point we find
            Transform spawnTransform = spawnPoints[0].transform;

            //Instantiate the player prefab at the spawn point's position and rotation
            if (playerPrefab != null)
            {
                Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            }
            else
            {
                Debug.LogError("Player Prefab is not assigned in the PlayerSpawner script!");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the tag: " + spawnPointTag);
        }
    }
}