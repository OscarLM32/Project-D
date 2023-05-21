
using UnityEngine;

/// <summary>
/// Logger class for the other systems to use
/// </summary>
public static class AppLogger
{

    public static void Log(DebugFlag flag, string msg)
    {
        if (IsDebugActivated(flag))
        {
            Debug.Log("["+flag+"]: " + msg);
        }
    }
    
    public static void LogWarning(DebugFlag flag, string msg)
    {
        if (IsDebugActivated(flag))
        {
            Debug.LogWarning("["+flag+"]: " + msg);
        }
    } 
    
    public static void LogError(DebugFlag flag, string msg)
    {
        if (IsDebugActivated(flag))
        {
            Debug.LogError("["+flag+"]: " + msg);
        }
    } 

    private static bool IsDebugActivated(DebugFlag flag)
    {
        switch (flag)
        {
            case DebugFlag.MENU_SYSTEM : return MenuSystem;
            case DebugFlag.SAVE_LOAD_SYSTEM : return SaveLoadSystem;
            case DebugFlag.PLAYER : return Player;
            case DebugFlag.SCENE_SYSTEM : return SceneSystem;
            case DebugFlag.INPUT_SYSTEM : return InputSystem;
            case DebugFlag.AUDIO_SYSTEM : return AudioSystem;
            case DebugFlag.CLASSIFIED_DOCUMENTS_HANDLER : return ClassifiedDocumentsHandler;
            default: return false;
        }
    }
    
    private static readonly bool MenuSystem = true;
    private static readonly bool SaveLoadSystem = true;
    private static readonly bool Player = true;
    private static readonly bool SceneSystem = true;
    private static readonly bool InputSystem = true;
    private static readonly bool AudioSystem = true; 
    private static readonly bool ClassifiedDocumentsHandler = true; 
}

/// <summary>
/// List of the systems that can make use of this class
/// </summary>
public enum DebugFlag
{
    MENU_SYSTEM,
    SAVE_LOAD_SYSTEM,
    PLAYER,
    SCENE_SYSTEM,
    INPUT_SYSTEM,
    AUDIO_SYSTEM,
    CLASSIFIED_DOCUMENTS_HANDLER
}
