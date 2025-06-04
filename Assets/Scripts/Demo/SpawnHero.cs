using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnHero : MonoBehaviour
{
    [SerializeField] GameObject hero;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var a = Instantiate(hero, t , Quaternion.identity);
        }
    }
}
