using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker.Blueprints.JsonSystem;
using ScalingCantrips.Config;
using ScalingCantrips.Utilities;
//using Kingmaker.Blueprints.JsonSystem;

using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager.ModEntry;
//using static UnityModManagerNet.UnityModManager;

namespace ScalingCantrips
{
  public class Main
  {
    internal static ModLogger Logger;

    private static bool Load(UnityModManager.ModEntry modEntry)
    {
      try
      {
        Logger = modEntry.Logger;
        modEntry.OnToggle = OnToggle;
        var harmony = new Harmony(modEntry.Info.Id);

        settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
        ModSettings.ModEntry = modEntry;
        ModSettings.LoadAllSettings();

        modEntry.OnGUI = new Action<UnityModManager.ModEntry>(OnGUI);
        modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);

        harmony.PatchAll(Assembly.GetExecutingAssembly());
        PostPatchInitializer.Initialize();
      }
      catch (Exception e)
      {
        Logger?.LogException(e);
        return false;
      }

      return true;
    }

    private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
    {
      iAmEnabled = value;
      return true;
    }

    private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
    {
      settings.Save(modEntry);
    }

    private static void OnGUI(UnityModManager.ModEntry modEntry)
    {
      if (!iAmEnabled)
      {
        return;
      }

      GUILayoutOption[] options =
        new GUILayoutOption[]
        {
          GUILayout.ExpandWidth(true),
          GUILayout.MaxWidth(1000)
        };

      GUILayout.Label("FOR BEST EFFECT: restart the game after changing these settings.", options);
      GUILayout.Label("Cantrips Caster Levels Required", options);
      GUILayout.Label(settings.CasterLevelsReq.ToString(), options);
      settings.CasterLevelsReq = (int)GUILayout.HorizontalSlider(settings.CasterLevelsReq, 1, 20, options);

      GUILayout.Label("Cantrips Dice Maximum", options);
      GUILayout.Label(settings.MaxDice.ToString(), options);
      settings.MaxDice = (int)GUILayout.HorizontalSlider(settings.MaxDice, 1, 40, options);

      settings.IgnoreDivineZap =
        GUILayout.Toggle(settings.IgnoreDivineZap, "Check this to prevent Divine Zap from being scaled", options);

      GUILayout.Label("Disrupt Undead Caster Levels Required", options);
      GUILayout.Label(settings.DisruptCasterLevelsReq.ToString(), options);
      settings.DisruptCasterLevelsReq = (int)GUILayout.HorizontalSlider(settings.DisruptCasterLevelsReq, 1, 40, options);

      GUILayout.Label("Disrupt Undead Dice Maximum", options);
      GUILayout.Label(settings.DisruptMaxDice.ToString(), options);
      settings.DisruptMaxDice = (int)GUILayout.HorizontalSlider(settings.DisruptMaxDice, 1, 40, options);

      GUILayout.Label("Virtue Caster Levels Required", options);
      GUILayout.Label(settings.VirtueCasterLevelsReq.ToString(), options);
      settings.VirtueCasterLevelsReq = (int)GUILayout.HorizontalSlider(settings.VirtueCasterLevelsReq, 1, 40, options);

      GUILayout.Label("Virtue Dice Maximum", options);
      GUILayout.Label(settings.VirtueMaxDice.ToString(), options);
      settings.VirtueMaxDice = (int)GUILayout.HorizontalSlider(settings.VirtueMaxDice, 1, 40, options);

      GUILayout.Label("Jolting Grasp Caster Levels Required", options);
      GUILayout.Label(settings.JoltingGraspLevelsReq.ToString(), options);
      settings.JoltingGraspLevelsReq = (int)GUILayout.HorizontalSlider(settings.JoltingGraspLevelsReq, 1, 40, options);

      GUILayout.Label("Jolting Grasp Dice Maximum", options);
      GUILayout.Label(settings.JoltingGraspMaxDice.ToString(), options);
      settings.JoltingGraspMaxDice = (int)GUILayout.HorizontalSlider(settings.JoltingGraspMaxDice, 1, 40, options);


      settings.DontAddUnholyZap = GUILayout.Toggle(settings.DontAddUnholyZap, "Check this to prevent Unholy Zap from being added", options);

      GUILayout.Label("Unholy Zap Caster Levels Required", options);
      GUILayout.Label(settings.DisruptLifeLevelsReq.ToString(), options);
      settings.DisruptLifeLevelsReq = (int)GUILayout.HorizontalSlider(settings.DisruptLifeLevelsReq, 1, 40, options);

      GUILayout.Label("Unholy Zap Dice Maximum", options);
      GUILayout.Label(settings.DisruptLifeMaxDice.ToString(), options);
      settings.DisruptLifeMaxDice = (int)GUILayout.HorizontalSlider(settings.DisruptLifeMaxDice, 1, 40, options);

      settings.StartImmediately =
        GUILayout.Toggle(
          settings.StartImmediately,
          "Check this to have caster levels take effect immediately (e.g Wizard 2 gets you 2d3 with default settings)",
          options);
    }

    private static bool iAmEnabled;

    public static Settings settings;

    public static void Log(string msg)
    {
      ModSettings.ModEntry.Logger.Log(msg);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogDebug(string msg)
    {
      ModSettings.ModEntry.Logger.Log(msg);
    }

    public static void LogPatch(string action, [NotNull] IScriptableObjectWithAssetId bp)
    {
      Log($"{action}: {bp.AssetGuid} - {bp.name}");
    }

    public static void LogHeader(string msg)
    {
      Log($"--{msg.ToUpper()}--");
    }

    public static Exception Error(string message)
    {
      Log(message);
      return new InvalidOperationException(message);
    }

    //public static LocalizedString MakeLocalizedString(string key, string value)
    //{
    //    LocalizationManager.CurrentPack.Strings[key] = value;
    //    LocalizedString localizedString = new LocalizedString();
    //    typeof(LocalizedString).GetField("m_Key", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(localizedString, key);
    //    return localizedString;
    //}
  }
}
