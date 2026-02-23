using UnityEngine;

public class VoidDisplacement : MonoBehaviour
{
    public float destroyDistance = 15f; // 离开玩家 15 米就消失
    public float shrinkSpeed = 2f;      // 消失时的缩小速度
    private Transform playerTransform;

    void Start()
    {
        // 自动找到玩家的位置
        playerTransform = Camera.main.transform;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > destroyDistance)
        {
            // 慢慢缩小直到消失
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);
            
            if (transform.localScale.x < 0.01f)
            {
                Destroy(gameObject);
                Debug.Log("Anxiety manifestation has entered the void.");
            }
        }
    }
}