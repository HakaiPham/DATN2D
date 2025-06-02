using UnityEngine;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class Item
{
    public string itemName;                        // Tên vật phẩm
    public string category;                        // Thể loại
    public List<ItemAttribute> attributes = new List<ItemAttribute>(); // Danh sách thuộc tính
    public Sprite image;                           // Ảnh vật phẩm
    public int quantity = 1;                       // Số lượng (mặc định 1)
    public int rate;                              // Tỷ lệ rơi (drop rate)

    // Tạo bản sao (clone) của Item
    public Item Clone()
    {
        Item clone = new Item
        {
            itemName = this.itemName,
            category = this.category,
            image = this.image,
            quantity = this.quantity,
            rate = this.rate,
            attributes = new List<ItemAttribute>()
        };

        foreach (var attr in this.attributes)
        {
            clone.attributes.Add(new ItemAttribute
            {
                type = attr.type,
                value = attr.value,
                isPercent = attr.isPercent
            });
        }

        return clone;
    }

    // Trả về chuỗi mô tả tất cả thuộc tính (dùng để hiển thị UI)
    public string GetAttributesDescription()
    {
        if (attributes == null || attributes.Count == 0)
            return "Không có thuộc tính";

        StringBuilder sb = new StringBuilder();

        foreach (var attr in attributes)
        {
            sb.AppendLine(attr.GetDescription());
        }

        return sb.ToString();
    }
}

[System.Serializable]
public class ItemAttribute
{
    public enum AttributeType
    {
        Attack,
        Defense,
        Speed,
        HP,
        CriticalRate
    }

    public AttributeType type;       // Loại thuộc tính
    public float value;              // Giá trị thuộc tính (ví dụ: 50 hoặc 50%)
    public bool isPercent = false;   // true = cộng theo %, false = cộng tuyệt đối

    // Trả về chuỗi mô tả thuộc tính, ví dụ "+50 Attack" hoặc "+20% CriticalRate"
    public string GetDescription()
    {
        string percentSign = isPercent ? "%" : "";
        string attrName = type.ToString();
        return $"+{value}{percentSign} {attrName}";
    }
}
