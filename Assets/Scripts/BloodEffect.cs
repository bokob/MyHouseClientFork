using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    
    private Material originalMaterial;

    public Material hitMaterial;

    private Renderer objectRenderer;

    void Start()
    {
        
        objectRenderer = GetComponent<Renderer>();
        
        originalMaterial = objectRenderer.material;
    }

    
    public void GetHit()
    {
        
        objectRenderer.material = hitMaterial;
        
        StartCoroutine(ResetMaterialAfterDelay(1.7f));
    }

    
    private IEnumerator ResetMaterialAfterDelay(float delay)
    {
        
        yield return new WaitForSeconds(delay);
       
        objectRenderer.material = originalMaterial;
    }
}
