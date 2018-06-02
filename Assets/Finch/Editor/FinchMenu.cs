using UnityEditor;
using UnityEngine;

public class FinchMenu
{
    [MenuItem("Finch/Documentation", false, 100)]
    private static void OpenDocumentation()
    {
        Application.OpenURL("https://finch-vr.com/sdk");
    }

    [MenuItem("Finch/About Finch", false, 100)]
    private static void OpenAboutGeneral()
    {
        Application.OpenURL("https://finch-vr.com");
    }
}
