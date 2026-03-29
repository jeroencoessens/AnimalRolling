using System.Collections.Generic;

/// <summary>
/// Persistent data that survives between runs. Serialized to JSON.
/// </summary>
[System.Serializable]
public class PersistentData
{
    public int totalCash = 0;
    public List<string> unlockedAnimals = new List<string> { "brave_pig" };
    public int dice = 100;
    public float lastDiceUpdate;

    // Per-animal meal totals: parallel lists since Unity JsonUtility doesn't support Dictionary
    public List<string> mealAnimalIds = new List<string>();
    public List<float> mealAnimalValues = new List<float>();

    public float GetAnimalMeals(string animalId)
    {
        int idx = mealAnimalIds.IndexOf(animalId);
        return idx >= 0 ? mealAnimalValues[idx] : 0f;
    }

    public void AddAnimalMeals(string animalId, float amount)
    {
        int idx = mealAnimalIds.IndexOf(animalId);
        if (idx >= 0)
        {
            mealAnimalValues[idx] += amount;
        }
        else
        {
            mealAnimalIds.Add(animalId);
            mealAnimalValues.Add(amount);
        }
    }
}
