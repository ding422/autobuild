using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JenkinsAdapter
{
    [MenuItem("Jenkins/JenkinsBuild")]
    public static void Build()
    {
        if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            string input = "";
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log("ARG " + i + ": " + args[i]);
                if (args[i] == "-output")
                {
                    input = args[i + 1];
                }
            }
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        List<string> sceneList = new List<string>();
        EditorBuildSettingsScene[] temp = EditorBuildSettings.scenes;
        for (int i = 0, iMax = temp.Length; i < iMax; ++i)
            sceneList.Add(temp[i].path);

        BuildPipeline.BuildPlayer(sceneList.ToArray(), "./android.apk", BuildTarget.Android, BuildOptions.None);
    }
}
