using Steamworks;
using System; // To have access to the manager in order to create it without requiring it in the scene.
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This section creates the static references needed to access these objects within the level without duplicates.
    public static SteamManager SteamInstance;
    public static GameManager GameInstance;
    
    // Using Awake for things that need to be loaded before the engine is ready for the object.
    private void Awake() // Keeps it out of the public access allowing it to function without issues.
    {
        // Ensuring there is only 1 Game Manager within the Project
        if (GameInstance == null)
        {
            GameInstance = this;
            // This here allows the Manager to not be destroyed on Load of a Scene.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }    

        // Steamworks.NET API creation/initialisation within the scene.
        if (SteamInstance != null) // This checks if the newInstance has something within it.
        {
            Debug.Log("Steam Manager is Already Active");
        }
        else if (FindAnyObjectByType<SteamManager>() == true) // This checks if there is a Steam Manager already in the scene
        {
            Debug.Log("Steam Manager found, bypassing creation and setting Manager to Variable.");
            SteamInstance = FindAnyObjectByType<SteamManager>();
        }
        else // If nothing else, it will create a new one.
        {
            Debug.Log("No Steam Manager Present");
            SteamInstance = new GameObject("SteamManager").AddComponent<SteamManager>();
        }

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This grabs the API Name (But also with an extra update since all achievement stuff I'm doing so far needs the name)
    // Using the "Out" within the brackets, it allows for multiple returns.
    private void AchievementGetter(int AchievementID, out string APIName, out string AchiName)
    {
        // This takes the int and converts it to uint for the GetAchievementName
        uint UAchievementID = Convert.ToUInt32(AchievementID);
        // Grabs the API Name for other functions to work. (Also outputs the APIName needed)
        APIName = SteamUserStats.GetAchievementName(UAchievementID);
        // Grabs the Achievement Display Name and outputs it to AchiName
        AchiName = SteamUserStats.GetAchievementDisplayAttribute(APIName, "name");
        return;
        // Returns the API String needed for the rest of the functions to work. (Old)
        //return achievementAPIName;
    }

    public void AchievementChecker(int AchievementID)
    {
        string APIName;
        bool unlockedAchievement;
        string achievementDisplayName;
        // This uses the new version of the Achievement Getter
        AchievementGetter(AchievementID, out APIName, out achievementDisplayName);

        // This grabs the Name for the Display/User Side (But can also do Description with "desc" and Hidden for hidden achievements)
        // Was originally in AchievementGetter, but removed to have it be more modular and reusable.
        //string achievementDisplayName = SteamUserStats.GetAchievementDisplayAttribute(APIName, "name"); (Removed due to using AchievementGetter New)
        // This checks if the achievement has been unlocked or not.
        SteamUserStats.GetAchievement(APIName, out unlockedAchievement);
        // Outputs the result. (Old Version)
        // Debug.Log(achievementDisplayName + ": " + trueOrFalseTest);
        // Outputs the Result - New Version (using the Singleton Instance Passthrough)
        GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + ": " + unlockedAchievement);
    }
    public void AchievementDesc(int AchievementID)
    {
        string APIName;
        string achievementDisplayName;
        AchievementGetter(AchievementID, out APIName, out achievementDisplayName);
        // This does the same in the Checker, but looks for the descrption instead of the name. (Not needing to grab name now due to new Achievement Getter)
        string achievementDescription = SteamUserStats.GetAchievementDisplayAttribute(APIName, "desc");
        // Using the \n to ensure the description is on a seperate line.
        GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\n" + achievementDescription);

    }

    public void AchievementUnlocker(int AchievementID)
    {
        string APIName;
        bool AchievementBool;
        string achievementDisplayName;
        AchievementGetter(AchievementID, out APIName, out achievementDisplayName);
        // This section focuses on unlocking the achievements via steam
        SteamUserStats.GetAchievement(APIName, out AchievementBool);
        // This is here to reduce the amount of calls to Steam's Server. If it is already unlocked it doesn't need to be called to be unlocked.
        if (AchievementBool == true)
        {
            GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\n--- Already Unlocked ---");
        }
        else
        {
            SteamUserStats.SetAchievement(APIName);
            GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\n--- UNLOCKED ---");
            // This sends a call to Steam Servers to unlock the achievement. (Or Update the Stats)
            SteamUserStats.StoreStats();
        }
    }

    public void AchievementResetter (int AchievementID) // DEBUG/DEVELOPMENT Only (Good for testing if Achievements are meeting requirements during development)
    {
        string APIName;
        string achievementDisplayName;
        AchievementGetter(AchievementID, out APIName, out achievementDisplayName);
        SteamUserStats.ClearAchievement(APIName);
        GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\n--- RESET ---");
        SteamUserStats.StoreStats();
    }
    
    // Not Working At the moment. Will delve deeper into Monday/Tuesday next week.
    public void AchievementStatChange (int AchievementID, bool IsIncreasing)
    {
        string APIName;
        string StatName;
        string achievementDisplayName;
        AchievementGetter(AchievementID, out APIName, out achievementDisplayName);
        float amountAdjustmentF = 1.0f;
        int amountAdjustmentI = 1;
        bool IsFloat = false;
        uint maximumRange;

        // Steam Documentation has a Step-by-Step for Stats.
        //SteamUserStats.GetAchievementProgressLimits(APIName, out minimumRange, out maximumRange);
        // Despite the 100 games, it doesn't have an attached range, so the range will be done manually.
        //Debug.Log(maximumRange);


        // This section here is to assign the stat API Name in order to change stat levels.
        // Now made into a function that handles all the sections needed.
        AchievementStatReference(AchievementID, out StatName, out IsFloat, out maximumRange);
        


        // Defuncted Version of the Stat System
        /*
        for (int i = 0; i < floatAchievementIDs.GetLength(0); i++)
        {
            if (floatAchievementIDs[i] == AchievementID)
            {
                IsFloat = true;
                FoundType = true;
                i = floatAchievementIDs.GetLength(0) + 1;
            }
        }
        if (FoundType != true)
        {
            for (int i = 0; i < floatAchievementIDs.GetLength(0); i++)
            {
                if (intAchievementIDs[i] == AchievementID)
                {
                    IsFloat = false;
                    FoundType = true;
                    i = intAchievementIDs.GetLength(0) + 1;
                }
            }
        }*/
        

        if (IsFloat)
        {
            float currentStat;
            SteamUserStats.GetStat(StatName, out currentStat);
            if (IsIncreasing)
                currentStat += amountAdjustmentF;
            else
                currentStat -= amountAdjustmentF;


            if (currentStat < maximumRange)
            {
                Debug.Log("Within Range of Achievement Stat");
                
            }
            else if (currentStat == maximumRange)
            {
                AchievementUnlocker(AchievementID);
                Debug.Log("Achievement Unlocked Via Stats");
            }
            SteamUserStats.SetStat(StatName, currentStat);
            GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\nNew Stat: " + currentStat);
        }
        else
        {
            int currentStat;
            bool test = SteamUserStats.GetStat(StatName, out currentStat);
            Debug.Log(test);
            if (IsIncreasing)
                currentStat += amountAdjustmentI;
            else
                currentStat -= amountAdjustmentI;

            // This is useless
            if (currentStat < maximumRange)
            {
                Debug.Log("Within Range of Achievement Stat");

            }
            test = SteamUserStats.SetStat(StatName, currentStat);
            if ((currentStat % (maximumRange / 10)) == 0)
            {
                SteamUserStats.IndicateAchievementProgress(APIName, Convert.ToUInt32(currentStat), maximumRange);
            }
            Debug.Log(test);
            GameUIVisuals.uiVisualsInstance.UpdateLog(achievementDisplayName + "\nNew Stat: " + currentStat);
        }
        
        SteamUserStats.StoreStats();
    }

    public void AchievementStatIncrease(int AchievementID)
    {
        AchievementStatChange(AchievementID, true);
    }

    public void AchievementStatDecrease(int AchievementID)
    {
        AchievementStatChange(AchievementID, false);
    }

    public void AchievementStatReset()
    {
        SteamUserStats.ResetAllStats(true);
        GameUIVisuals.uiVisualsInstance.UpdateLog("All Stats Reset");
    }

    // Potential reusable function allowing for easier Stat Referencing.
    public void AchievementStatReference(int AchievementID, out string StatName, out bool IsFloat, out uint maximumRange)
    {
        switch (AchievementID)
        {
            case (0):
                StatName = "FeetTraveled";
                IsFloat = true;
                maximumRange = 5280;
                break;
            case (1):
                StatName = "MaxFeetTraveled";
                IsFloat = true;
                maximumRange = 500;
                break;
            case (2):
                StatName = "NumWins";
                IsFloat = false;
                maximumRange = 100;
                break;
            case (3):
                StatName = "NumWins";
                IsFloat = false;
                maximumRange = 1;
                break;
            default:
                StatName = null;
                IsFloat = false;
                maximumRange = 0;
                break;
        }
    }
}
