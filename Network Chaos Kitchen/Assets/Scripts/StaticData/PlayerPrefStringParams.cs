
public static class PlayerPrefStringParams{
        
    public const string MainMusicVolume = "MainMusicVolume";
    public const string SfxMusicVolume = "SfxMusicVolume";
    public const string PlayerCustomInput = "PlayerCustomInput";
    public const string CurrentUnlockedLevel = "CurrentUnlockedLevel";

    public static string LevelScore(Level level) {
        return LevelScore((int)level);
    }

    public static string LevelScore(int level) {
        return "Level_" + level.ToString();
    }

}


