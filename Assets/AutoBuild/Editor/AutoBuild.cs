using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class AutoBuild
{
    private const string ANDROID_SDK = "ANDROID_SDK";
    private const string AndroidSdkRoot = "AndroidSdkRoot";
    private const string ANDROID_NDK = "ANDROID_NDK";
    private const string AndroidNdkRoot = "AndroidNdkRoot";

    private const string OUTPUT_PATH_KEY = "-output";
    private static string OUTPUT_PATH_VALUE = ".\\Build";
    private const string BUILD_TARGET_KEY = "-buildType";
    private static string BUILD_TARGET_VALUE = null;

    public static void SetBuildSetting()
    {
        Debug.Log("SetBuildSetting");
        //Unity Run and Jenkins Run , regedit path is different
        //need to set path in scipt
        string android_sdk = System.Environment.GetEnvironmentVariable(ANDROID_SDK);
        if (!string.IsNullOrEmpty(android_sdk))
            EditorPrefs.SetString(AndroidSdkRoot, android_sdk);

        string android_ndk = System.Environment.GetEnvironmentVariable(ANDROID_NDK);
        if (!string.IsNullOrEmpty(android_ndk))
            EditorPrefs.SetString(AndroidNdkRoot, android_ndk);
    }

    private static void GetCommandLineArgs()
    {
        if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
        {
            Debug.Log("GetCommandLineArgs");
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log("ARG " + i + ": " + args[i]);
                if (args[i].Equals(OUTPUT_PATH_KEY))
                {
                    OUTPUT_PATH_VALUE = args[i + 1];
                }
                else if (args[i].Equals(BUILD_TARGET_KEY))
                {
                    BUILD_TARGET_VALUE = args[i + 1];
                }
            }
        }
    }

    private static BuildTarget GetBuildTarget()
    {
        Debug.Log("GetBuildTarget");
        switch (BUILD_TARGET_VALUE)
        {
            case "StandaloneWindows":
                return BuildTarget.StandaloneWindows;
            case "Android":
                return BuildTarget.Android;
            default:
                return BuildTarget.NoTarget;
        }
    }

    private static BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            //case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                return BuildTargetGroup.Standalone;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;
            case BuildTarget.Tizen:
                return BuildTargetGroup.Tizen;
            case BuildTarget.PSP2:
                return BuildTargetGroup.PSP2;
            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;
            case BuildTarget.PSM:
                return BuildTargetGroup.PSM;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.N3DS:
                return BuildTargetGroup.N3DS;
            case BuildTarget.WiiU:
                return BuildTargetGroup.WiiU;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            case BuildTarget.NoTarget:
            default:
                return BuildTargetGroup.Standalone;
        }
    }

    private static string GetExtension(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            //case BuildTarget.StandaloneOSX:
                //break;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.iOS:
                break;
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.StandaloneLinux:
                break;
            case BuildTarget.WebGL:
                break;
            case BuildTarget.WSAPlayer:
                break;
            case BuildTarget.StandaloneLinux64:
                break;
            case BuildTarget.StandaloneLinuxUniversal:
                break;
            case BuildTarget.Tizen:
                break;
            case BuildTarget.PSP2:
                break;
            case BuildTarget.PS4:
                break;
            case BuildTarget.PSM:
                break;
            case BuildTarget.XboxOne:
                break;
            case BuildTarget.N3DS:
                break;
            case BuildTarget.WiiU:
                break;
            case BuildTarget.tvOS:
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.NoTarget:
                break;
            default:
                break;
        }

        return ".unknown";
    }

    private static BuildPlayerOptions GetDefaultPlayerOptions()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        List<string> sceneList = new List<string>();
        EditorBuildSettingsScene[] temp = EditorBuildSettings.scenes;
        for (int i = 0, iMax = temp.Length; i < iMax; ++i)
            sceneList.Add(temp[i].path);

        buildPlayerOptions.scenes = sceneList.ToArray();
        buildPlayerOptions.options = BuildOptions.None;

        // To define
        // buildPlayerOptions.locationPathName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\LightGunBuild\\Android\\LightGunMouseArcadeRoom.apk";
        // buildPlayerOptions.target = BuildTarget.Android;

        return buildPlayerOptions;
    }

    static void DefaultBuild(BuildTarget buildTarget)
    {
        Debug.Log("DefaultBuild, buildTarget = " + buildTarget);
        if (buildTarget == BuildTarget.NoTarget)
        {
            Debug.Log("buildTarget is BuildTarget.NoTarget, skip build");
            return;
        }

        BuildTargetGroup targetGroup = GetBuildTargetGroup(buildTarget);

        string path = Path.Combine(Path.Combine(OUTPUT_PATH_VALUE, targetGroup.ToString()), PlayerSettings.productName + "_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH") + "h" + System.DateTime.Now.ToString("mm") + "_" + buildTarget);
        string name = PlayerSettings.productName + GetExtension(buildTarget);


        //string defineSymbole = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole + ";BUILD");

        //PlayerSettings.Android.keystorePass = androidKeystorePass;
        //PlayerSettings.Android.keyaliasName = androidKeyaliasName;
        //PlayerSettings.Android.keyaliasPass = androidKeyaliasPass;

        BuildPlayerOptions buildPlayerOptions = GetDefaultPlayerOptions();

        buildPlayerOptions.locationPathName = Path.Combine(path, name);
        buildPlayerOptions.target = buildTarget;

        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

        string result = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log(result);
        if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
        {
            if (!string.IsNullOrEmpty(result))
            {
                Debug.Log("BuildPlayer fail!!! result = " + result);
                EditorApplication.Exit(1);
            }
        }
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole);

        //if (buildTarget == BuildTarget.Android)
        //AndroidLastBuildVersionCode = PlayerSettings.Android.bundleVersionCode;

        //UnityEditor.EditorUtility.RevealInFinder(path);
    }

    [MenuItem("AutoBuild/BuildAndroid")]
    static void BuildAndroid()
    {
        DefaultBuild(BuildTarget.Android);
    }

    [MenuItem("AutoBuild/BatchmodeBuild")]
    public static void BatchmodeBuild()
    {
        SetBuildSetting();
        GetCommandLineArgs();

        DefaultBuild(GetBuildTarget());

        /*
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        List<string> sceneList = new List<string>();
        EditorBuildSettingsScene[] temp = EditorBuildSettings.scenes;
        for (int i = 0, iMax = temp.Length; i < iMax; ++i)
            sceneList.Add(temp[i].path);

        BuildPipeline.BuildPlayer(sceneList.ToArray(), "./android.apk", BuildTarget.Android, BuildOptions.None);
        */
    }
}
