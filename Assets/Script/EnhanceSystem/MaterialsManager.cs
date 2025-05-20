using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Dictionary<string, int> materials = new Dictionary<string, int>();

    void Start()
    {
        // Khởi tạo vật liệu ban đầu
        AddMaterial("IronOre", 5);
        AddMaterial("Wood", 5);
        AddMaterial("Stone", 3);
    }

    // Kiểm tra đủ vật liệu chưa
    public bool HasMaterials(string materialName, int amount)
    {
        return materials.ContainsKey(materialName) && materials[materialName] >= amount;
    }

    // Trừ vật liệu khi dùng
    public bool SpendMaterial(string materialName, int amount)
    {
        if (HasMaterials(materialName, amount))
        {
            materials[materialName] -= amount;
            Debug.Log($"Đã trừ {amount} {materialName}. Còn lại: {materials[materialName]}");
            return true;
        }
        Debug.Log($"Không đủ {materialName}!");
        return false;
    }

    // Thêm vật liệu
    public void AddMaterial(string materialName, int amount)
    {
        if (!materials.ContainsKey(materialName))
            materials[materialName] = 0;
        materials[materialName] += amount;
        Debug.Log($"Đã cộng {amount} {materialName}. Hiện có: {materials[materialName]}");
    }
}
