namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification {

    public enum EServerNotificationType {
        EMPTY = 0,
        PROCESS_RESULT = 1
    }

    public abstract record ServerNotificationBase {
        public abstract EServerNotificationType Type { get; }
        public string ClientId { get; init; }
    }

    public record ProcessResult : ServerNotificationBase
    {
        public override EServerNotificationType Type => EServerNotificationType.PROCESS_RESULT;

        public string Id { init; get; }
        public string OwnerId { init; get; }
        public string ProcessName { init; get; }
        public object[] InOuts { init; get; }
        public object[] Outputs { init; get; }
        public long TimeStamp { init; get; }

    }

}