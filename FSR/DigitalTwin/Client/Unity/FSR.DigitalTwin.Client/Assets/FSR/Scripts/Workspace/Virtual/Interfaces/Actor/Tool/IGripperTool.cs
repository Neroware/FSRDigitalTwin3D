namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Actor.Tool {

    public interface IGripperTool {

        bool Opened { get; }

        void OpenGripper();
        void CloseGripper();

    }

}