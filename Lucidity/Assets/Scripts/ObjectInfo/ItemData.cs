using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public LocalizedString itemName;
    public LocalizedString description;
    public Sprite image;
}
