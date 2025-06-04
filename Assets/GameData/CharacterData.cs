using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Stas")]
    public GameObject battlePrefab; // Prefab của nhân vật 
    public Sprite characterPortrait; //Ảnh đại hiện cho Nv
    public float health;
    public float magic;
    public float melee;
    public float defense;
    public float magicRange;
    public float speed;
    public float experience;
}
