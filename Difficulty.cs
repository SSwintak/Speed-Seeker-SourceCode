
public enum DifficultyLevel
{
    Slow,
    Fast,
    VeryFast,
    LightSpeed,
    SpeedSeeker
}

public class Difficulty
{
    public static float MIN_DIFFICULTY_MULTIPLIER => 1f;
    public static float MAX_DIFFICULTY_MULTIPLIER => 4f;    

    public DifficultyLevel difficultyLevel;

    public static float GetMultiplierFromLevel(DifficultyLevel difficultyLevel)
    {        
        return difficultyLevel switch
        {
            DifficultyLevel.Slow => 1f,
            DifficultyLevel.Fast => 1.25f,
            DifficultyLevel.VeryFast => 1.5f,
            DifficultyLevel.LightSpeed => 2.25f,
            DifficultyLevel.SpeedSeeker => 3.25f,
            _ => 1f,
        };
    }
}
