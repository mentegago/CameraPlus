using Newtonsoft.Json;

namespace CameraPlus.Configuration
{
    public class StartPos
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public string FOV { get; set; }
    }

    public class StartRot
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }
    public class StartHeadOffset
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }
    public class EndPos
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
        public string FOV { get; set; }
    }

    public class EndRot
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }
    public class EndHeadOffset
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    [JsonObject("Movements")]
    public class JSONMovement
    {
        [JsonProperty("StartPos")]
        public StartPos startPos { get; set; }
        [JsonProperty("StartRot")]
        public StartRot startRot { get; set; }
        [JsonProperty("StartHeadOffset")]
        public StartHeadOffset startHeadOffset { get; set; }
        [JsonProperty("EndPos")]
        public EndPos endPos { get; set; }
        [JsonProperty("EndRot")]
        public EndRot endRot { get; set; }
        [JsonProperty("EndHeadOffset")]
        public EndHeadOffset endHeadOffset { get; set; }

        public string TurnToHead { get; set; }
        public string Duration { get; set; }
        public string Delay { get; set; }
        public string EaseTransition { get; set; }
    }

    public class MovementScriptJson
    {
        public string ActiveInPauseMenu { get; set; }
        public string TurnToHeadUseCameraSetting { get; set; }
        [JsonProperty("Movements")]
        public JSONMovement[] Jsonmovement { get; set; }
    }
}
