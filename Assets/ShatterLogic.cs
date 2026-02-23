using UnityEngine;

public class ShatterLogic : MonoBehaviour
{
    [Header("视觉与预制体")]
    public GameObject fragmentsPrefab; 
    
    [Header("物理微调")]
    public float explosionForce = 50f; 
    public float breakVelocityThreshold = 2.5f;

    private bool isShattered = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isShattered) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            if (impactSpeed > breakVelocityThreshold)
            {
                ExecuteShatter();
            }
        }
    }

    void ExecuteShatter()
    {
        isShattered = true;

        GameObject pieces = Instantiate(fragmentsPrefab, transform.position, transform.rotation);

        foreach (Transform child in pieces.transform)
        {
            child.localScale = Vector3.one * Random.Range(0.4f, 0.9f);

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                Vector3 popUpForce = (Vector3.up + Random.insideUnitSphere * 0.2f) * explosionForce;
                rb.AddForce(popUpForce, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
        Debug.Log("焦虑已被击碎！");
    }
}