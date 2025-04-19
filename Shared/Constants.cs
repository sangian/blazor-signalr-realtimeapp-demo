namespace Shared
{
    public class Constants
    {
        public const string CLIENT_START_REQUEST = "StartRequest";
        public const string CLIENT_STOP_REQUEST = "StopRequest";

        public const string SERVER_START_RESPONSE = "StartResponse";
        public const string SERVER_STOP_RESPONSE = "StopResponse";
        public const string SERVER_AIRPLANE_ARRIVED = "AirplaneArrived";
        public const string SERVER_AIRPLANE_CRASHED = "AirplaneCrashed";
        public const string SERVER_STREAM_TELEMETRY = "StreamTelemetry";
        public const string SERVER_IS_TYPING = "IsTyping";
        public const string SERVER_REGISTER_STREAM_CHANNEL = "RegisterStreamChannel";
        public const string SERVER_UNREGISTER_STREAM_CHANNEL = "UnregisterStreamChannel";
        public const string SERVER_STREAM_STARTED = "StreamStarted";
        public const string SERVER_STREAM_STOPPED = "StreamStopped";
        public const string SERVER_STREAM_REMOTE_CHANNELS = "StreamRemoteChannels";

        public const string PING = "Ping";
        public const string PONG = "Pong";

        public const string CLIENT_NOTIFY_AIRPLANE_START_RESPONSE = "NotifyAirplaneStartResponse";
        public const string CLIENT_NOTIFY_AIRPLANE_STOP_RESPONSE = "NotifyAirplaneStopResponse";

        public const string CLIENT_CHAT_ROOM_CREATED = "ChatRoomCreated";
        public const string CLIENT_CHAT_ROOM_MEMBER_ADDED = "ChatRoomMemberAdded";
        public const string CLIENT_CHAT_ROOM_MEMBER_REMOVED = "ChatRoomMemberRemoved";
        public const string CLIENT_CHAT_ROOM_DELETED = "ChatRoomDeleted";

        public const string CLIENT_CHAT_MESSAGE_TYPING = "IsTyping";
        public const string CLIENT_CHAT_MESSAGE_SENT = "ChatMessageSent";
        public const string CLIENT_CHAT_MESSAGE_DELIVERED = "ChatMessageDelivered";
        public const string CLIENT_CHAT_MESSAGE_READ = "ChatMessageRead";

        public const string CLIENT_STREAM_STARTED = "StreamStarted";
        public const string CLIENT_STREAM_STOPPED = "StreamStopped";
    }
}
