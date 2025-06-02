using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Material material;
    [SerializeField]
    private float Parallax = 0.001f;
    private float offset;
    public float GameSpeed = 0.0001f;
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        ParallaxScroll();
    }
    private void ParallaxScroll()
    {
        float speed = GameSpeed * Parallax;
        offset += Time.deltaTime * speed;
        material.SetTextureOffset("_MainTex", Vector2.right * offset);
    }
}
