namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces {

    public interface IGripperTool {

        bool Opened { get; }

        void OpenGripper();
        void CloseGripper();

    }

}