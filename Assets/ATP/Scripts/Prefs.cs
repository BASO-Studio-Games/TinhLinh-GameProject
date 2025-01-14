using UnityEngine;

public static class Prefs
{
    public static int coins
    {
        set => PlayerPrefs.SetInt("coins", value);
        get => PlayerPrefs.GetInt("coins", 0);
    }

    public static string playerData
    {
        set => PlayerPrefs.SetString("player_data", value);
        get => PlayerPrefs.GetString("player_data");
    }

    public static string playerWeaponData
    {
        set => PlayerPrefs.SetString("player_weapon_data", value);
        get => PlayerPrefs.GetString("player_weapon_data");
    }

    public static string enemyData
    {
        set => PlayerPrefs.SetString("enemy_data", value);
        get => PlayerPrefs.GetString("enemy_data");
    }

    public static bool IsEnoughCoins(int coinToCheck)
    {
        return coins >= coinToCheck;
    }
}
