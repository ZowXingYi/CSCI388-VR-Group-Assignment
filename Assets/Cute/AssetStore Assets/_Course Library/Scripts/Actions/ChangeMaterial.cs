using UnityEngine;

/// <summary>
/// This script makes it easier to toggle between a new material, and the objects original material.
/// </summary>
public class ChangeMaterial : MonoBehaviour
{
    [Tooltip("The material that's switched to.")]
    public Material otherMaterial = null;
    [SerializeField] private bool isFindChild = false;

    private bool usingOther = false;
    private MeshRenderer meshRenderer = null;
    private Material originalMaterial = null;

    private void Awake()
    {
        InitializeMaterial();
    }

    private void InitializeMaterial()
    {
        if (meshRenderer == null)
        {
            if (isFindChild)
            {
                meshRenderer = GetComponentInChildren<MeshRenderer>();
            }
            else
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }
            //Debug.Log("Mesh Renderer: " + meshRenderer);

            if (meshRenderer)
            {
                originalMaterial = meshRenderer.material;
            }
        }
    }

    public void SetOtherMaterial()
    {
        InitializeMaterial();
        usingOther = true;
        meshRenderer.material = otherMaterial;
    }

    public void SetOriginalMaterial()
    {
        InitializeMaterial();
        usingOther = false;
        meshRenderer.material = originalMaterial;
    }

    public void ToggleMaterial()
    {
        InitializeMaterial();
        usingOther = !usingOther;

        if(usingOther)
        {
            meshRenderer.material = otherMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
