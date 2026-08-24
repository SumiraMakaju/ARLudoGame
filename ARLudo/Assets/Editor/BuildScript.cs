using UnityEditor;
using System.Linq;

public static class BuildScript
{
    public static void BuildWindows()
    {
        string[] scenes = new string[] { "Assets/Scenes/LudoTraining.unity" };
        BuildPipeline.BuildPlayer(scenes, "Builds/LudoTraining.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
}
