using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimal", menuName = "AnimalEscape/AnimalDefinition")]
public class AnimalDefinition : ScriptableObject
{
    public string id;
    public string animalName;
    public string emoji;
    public Color color = Color.white;

    [Header("Stats")]
    public int speed;
    public int armor;
    public int stealth;

    [Header("Unlock")]
    public int price;

    [Header("Ability")]
    public string abilityName;

    [TextArea]
    public string description;

    [Header("Visuals")]
    [Tooltip("Assign a prefab for this animal. If null, a procedural placeholder is generated.")]
    public GameObject modelPrefab;
}
