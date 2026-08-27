using UnityEngine;

public class ARLudoVFXManager : MonoBehaviour
{
    public static ARLudoVFXManager Instance;
    
    public GameObject captureParticlePrefab;
    public GameObject goalParticlePrefab;
    public GameObject fireworksPrefab;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void PlayCaptureVFX(Vector3 position)
    {
        if (captureParticlePrefab != null)
        {
            GameObject fx = Instantiate(captureParticlePrefab, position, Quaternion.identity);
            Destroy(fx, 3f);
        }
    }
    
    public void PlayGoalVFX(Vector3 position)
    {
        if (goalParticlePrefab != null)
        {
            GameObject fx = Instantiate(goalParticlePrefab, position, Quaternion.identity);
            Destroy(fx, 3f);
        }
    }
    
    public void PlayWinVFX(Vector3 position)
    {
        if (fireworksPrefab != null)
        {
            GameObject fx = Instantiate(fireworksPrefab, position, Quaternion.identity);
            Destroy(fx, 5f);
        }
    }
}