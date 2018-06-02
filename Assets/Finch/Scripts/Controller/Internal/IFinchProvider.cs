namespace Finch
{
    public enum Finch3DOFModel
    {
        Basic,
        Pro,
        Unknown
    }

    public enum FinchControllerModel
    {
        Shift,
        Dash,
        DashM4,
        Unknown
    }

    public interface IFinchProvider
    {
        void Exit();

        void ReadState(PlayerState outState);

        void HapticPulse(FinchNodeType type, uint millisecond);

        void HapticPulse(FinchNodeType type, params VibrationPackage[] milliseconds);

        void ChangeDevice(FinchControllerType deviceType);

        void ChangeBodyRotationMode(FinchBodyRotationMode bodyRotationMode);

        void Calibrate(FinchChirality chirality, FinchRecenterMode recenterMode);

        void Recenter(FinchChirality chirality, FinchRecenterMode recenterMode);

        float GetBatteryCharge(FinchNodeType nodeType);

        void SwapNodes(FinchNodeType firstNode, FinchNodeType secondNode);

        FinchControllerModel GetControllerModel(FinchNodeType nodeType);

        void StartChiralityRedefine();

        bool IsChiralityRedefining();

        void SetBoneLength(FinchBone bone, float length);
    }
}