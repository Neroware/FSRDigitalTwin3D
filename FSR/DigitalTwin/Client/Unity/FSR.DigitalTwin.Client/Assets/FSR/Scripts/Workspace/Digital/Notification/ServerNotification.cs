namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification {

    public enum EServerNotificationType {
        EMPTY = 0,
        PROCESS_INVOKED = 1
    }

    public abstract record ServerNotificationBase {
        public abstract EServerNotificationType Type { get; }
    }

    public record ProcessInvocation : ServerNotificationBase {
        public override EServerNotificationType Type => EServerNotificationType.PROCESS_INVOKED;
        public string Id { init; get; }
        public string OwnerId { init; get; }
        public string ProcessName { init; get; }
        public object[] Inputs { init; get; }
        public object[] InOuts { init; get; }
        public long TimeStamp { init; get; }
    }

}