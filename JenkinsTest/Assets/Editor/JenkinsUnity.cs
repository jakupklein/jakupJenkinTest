using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class JenkinsUnity
{
    static string[] SCENES = FindEnabledEditorScenes();
    static string APP_NAME = "VelcroSnake"; //QA just wants builds to be named as x.x.xx so this should be scrapped I guess
    static string TARGET_DIR = "Builds";
    static string COMPANY_NAME = "Team Null Dadiu";
    static string PRODUCT_NAME = "null";
    static string BUNDLE_ID = "com.null.null";
    static string VERSION = "null";
    static string BUILD_NUMBER = "-1";
    static bool APP_NAME_IS_VERSION_NUMBER = false;

    static void GetVariablesFromJenkins()
    {
        string compNameTemp = Environment.GetEnvironmentVariable("COMPANY_NAME");
        string productNameTemp = Environment.GetEnvironmentVariable("PRODUCT_NAME");
        string bundleIDTemp = Environment.GetEnvironmentVariable("BUNDLE_ID");
        string buildNumberTemp = Environment.GetEnvironmentVariable("BUILD_NUMBER");
        string versionNumberTemp = Environment.GetEnvironmentVariable("VERSION");
        string appNameTemp = Environment.GetEnvironmentVariable("APP_NAME");
        string targetDirTemp = Environment.GetEnvironmentVariable("TARGET_DIR");
        string useVersionAsAppName = Environment.GetEnvironmentVariable("VERSION_AS_NAME");

        if (!String.IsNullOrEmpty(compNameTemp)) COMPANY_NAME = compNameTemp;
        if (!String.IsNullOrEmpty(productNameTemp)) PRODUCT_NAME = productNameTemp;
        if (!String.IsNullOrEmpty(bundleIDTemp)) BUNDLE_ID = bundleIDTemp;
        if (!String.IsNullOrEmpty(buildNumberTemp)) BUILD_NUMBER = buildNumberTemp;
        if (!String.IsNullOrEmpty(versionNumberTemp)) VERSION = versionNumberTemp;
        if (!String.IsNullOrEmpty(appNameTemp)) APP_NAME = appNameTemp;
        if (!String.IsNullOrEmpty(targetDirTemp)) TARGET_DIR = targetDirTemp;

        if (!String.IsNullOrEmpty(useVersionAsAppName))
        {
            string temp = useVersionAsAppName.ToLower();
            if (temp == "true")
                APP_NAME_IS_VERSION_NUMBER = true;
            else
                APP_NAME_IS_VERSION_NUMBER = false;
        }
    }

    static void ConfigurePlayerSettings()
    {
        GetVariablesFromJenkins();

        PlayerSettings.companyName = COMPANY_NAME;
        PlayerSettings.productName = PRODUCT_NAME;
        PlayerSettings.bundleIdentifier = BUNDLE_ID;
        PlayerSettings.bundleVersion = VERSION + "." + BUILD_NUMBER;

        if (APP_NAME_IS_VERSION_NUMBER)
        {
            APP_NAME = PlayerSettings.bundleVersion;
        }
    }

    [MenuItem("Jenkins/Build Win")]
    static void PerformWinBuild()
    {
        ConfigurePlayerSettings();
        string fileName = APP_NAME + ".exe";
        GenericBuild(SCENES, TARGET_DIR + "/" + fileName, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    [MenuItem("Jenkins/Build Android")]
    static void PerformAndroidBuild()
    {
        ConfigurePlayerSettings();
        string fileName = APP_NAME + ".apk";
        string comp_dir = TARGET_DIR + "/" + fileName;
        GenericBuild(SCENES, comp_dir, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Jenkins/Build IOS")]
    static void PerformIOSBuild()
    {
        ConfigurePlayerSettings();
        string fileName = APP_NAME + ".ipa";
        GenericBuild(SCENES, TARGET_DIR + "/" + fileName, BuildTarget.iOS, BuildOptions.None);
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }
    }
}