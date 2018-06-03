using UnityEngine;
using Finch;
using System.Text;

/// <summary>
/// Add this script to the scene if you want to see debug information
/// </summary>
public class FinchStatusDebug : MonoBehaviour
{
    /// <summary>
    /// Leave this field empty if you don't want to use text mesh as an output
    /// </summary>
    public TextMesh Output;

    public bool ShowFPS = true;
    public bool WriteToDebugLog = true;

    private float fps = 60;
    private const string MsfFormat = "#.#";
    private const string DisplayTextFormat = "{0} msf\n({1} FPS)";
    private const float MsPerSec = 1000f;

    private StringBuilder sb = new StringBuilder();
    private string fpsText = "";

    private const FinchNodesStateType state = FinchNodesStateType.Connected;
    private FinchNodesState fns;

    private readonly bool[] connectedState = new bool[(int)FinchNodeType.Last];

    void LateUpdate()
    {
        sb.Remove(0, sb.Length);

        float deltaTime = Time.unscaledDeltaTime;
        float interp = deltaTime / (0.5f + deltaTime);
        float currentFPS = 1.0f / deltaTime;
        fps = Mathf.Lerp(fps, currentFPS, interp);
        float msf = MsPerSec / fps;
        fpsText = string.Format(DisplayTextFormat, msf.ToString(MsfFormat), Mathf.RoundToInt(fps));

        fns = FinchVR.GetNodesState();

        sb.Append("Finch debug info. ");
        if (ShowFPS)
            sb.Append("FPS: ").Append(fpsText).Append(". ");

        sb.Append("rh rua lh lua: ")
          .Append(fns.GetState(FinchNodeType.RightHand, state))
          .Append(" ")
          .Append(fns.GetState(FinchNodeType.RightUpperArm, state))
          .Append(" ")
          .Append(fns.GetState(FinchNodeType.LeftHand, state))
          .Append(" ")
          .Append(fns.GetState(FinchNodeType.LeftUpperArm, state))
          .Append(". ");

        for (int i = 0; i < (int)FinchNodeType.Last; ++i)
        {
            bool current = fns.GetState((FinchNodeType)i, state);
            if (connectedState[i] ^ current)
                sb.Append("Was ").Append(current ? "connected" : "disconnected").Append(": ").Append((FinchNodeType)i).Append(". ");

            connectedState[i] = current;
        }

        if (Output != null)
            Output.text = sb.ToString();

        if (WriteToDebugLog)
            Debug.Log(sb.ToString());
    }
}