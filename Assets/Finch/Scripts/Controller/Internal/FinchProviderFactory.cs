using System;

namespace Finch
{
    public static class FinchProviderFactory
    {
        public static IFinchProvider CreateControllerProvider()
        {
            switch (FinchSettings.DataSource)
            {
                case FinchDataSource.Controller:
                    {
                        FinchControllerType deviceType = FinchSettings.DeviceType;
                        switch (deviceType)
                        {
                            case FinchControllerType.Dash:
                                if (FinchSettings.PoseTrackingAlgorithm == FinchPoseTrackingAlgorithm.GoogleVR)
                                    return new FinchGoogleAlgorithmProvider(deviceType);
                                else
                                    return new FinchOwnAlgorithmProvider(deviceType);
                            case FinchControllerType.Shift:
                            case FinchControllerType.Hand:
                                return new FinchOwnAlgorithmProvider(deviceType);
                            default:
                                return new DummyProvider();
                        }
                    }
                case FinchDataSource.Disabled:
                    return new DummyProvider();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}