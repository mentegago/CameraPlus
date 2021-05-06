using Newtonsoft.Json;

namespace CameraPlus.Camera2Utils
{
    internal enum CameraType
    {
        FirstPerson,
        Attached, //Unused for now, but mostly implemented - For parenting to arbitrary things like possibly to a saber, etc.
        Positionable
    }
    internal enum WallVisiblity
    {
        Visible,
        Transparent,
        Hidden
    }
    internal enum WorldCamVisibility
    {
        Visible = 0,
        HiddenWhilePlaying,
        Hidden = 2,
    }
    internal enum NoteVisibility
    {
        Hidden,
        Visible,
        ForceCustomNotes
    }
    [JsonObject("visibleObjects")]
    public class visibleObjectsElement
    {
        [JsonProperty("Walls")]
        public string Walls { get; set; }
        [JsonProperty("Debris")]
        public bool Debris { get; set; }
        [JsonProperty("UI")]
        public bool UI { get; set; }
        [JsonProperty("Avatar")]
        public bool Avatar { get; set; }
        [JsonProperty("Floor")]
        public bool Floor { get; set; }
        [JsonProperty("CutParticles")]
        public bool CutParticles { get; set; }
        [JsonProperty("Notes")]
        public string Notes { get; set; }
    }

    [JsonObject("viewRect")]
    public class viewRectElement
    {
        [JsonProperty("x")]
        public float x { get; set; }
        [JsonProperty("y")]
        public float y { get; set; }
        [JsonProperty("width")]
        public float width { get; set; }
        [JsonProperty("height")]
        public float height { get; set; }
    }

    [JsonObject("FPSLimiter")]
    public class FPSLimiterElement
    {
        [JsonProperty("fpsLimit")]
        public int fpsLimit { get; set; }
    }
    [JsonObject("Smoothfollow")]
    public class SmoothfollowElement
    {
        [JsonProperty("position")]
        public float position { get; set; }
        [JsonProperty("rotation")]
        public float rotation { get; set; }
        [JsonProperty("forceUpright")]
        public bool forceUpright { get; set; }
        [JsonProperty("followReplayPosition")]
        public bool followReplayPosition { get; set; }
        [JsonProperty("pivotingOffset")]
        public bool pivotingOffset { get; set; }
    }

    [JsonObject("ModmapExtensions")]
    public class ModmapExtensionsElement
    {
        [JsonProperty("moveWithMap")]
        public bool moveWithMap { get; set; }
        [JsonProperty("autoOpaqueWalls")]
        public bool autoOpaqueWalls { get; set; }
        [JsonProperty("autoHideHUD")]
        public bool autoHideHUD { get; set; }
    }
    [JsonObject("Follow360")]
    public class Follow360Element
    {
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("smoothing")]
        public float smoothing { get; set; }
    }
    [JsonObject("targetPos")]
    public class targetPosElement
    {
        [JsonProperty("x")]
        public float x { get; set; }
        [JsonProperty("y")]
        public float y { get; set; }
        [JsonProperty("z")]
        public float z { get; set; }
    }
    [JsonObject("targetRot")]
    public class targetRotElement
    {
        [JsonProperty("x")]
        public float x { get; set; }
        [JsonProperty("y")]
        public float y { get; set; }
        [JsonProperty("z")]
        public float z { get; set; }
    }
    [JsonObject("MovementScript")]
    public class MovementScriptElement
    {
        [JsonProperty("scriptList")]
        public string[] scriptList { get; set; }
        [JsonProperty("fromOrigin")]
        public bool fromOrigin { get; set; }
        [JsonProperty("enableInMenu")]
        public bool enableInMenu { get; set; }
    }

    [JsonObject("Camera2Config")]
    public class Camera2Config
    {
        public string type { get; set; }
        public string worldCamVisibility { get; set; }
        public float FOV { get; set; }
        public float previewScreenSize { get; set; }
        public int layer { get; set; }
        public int antiAliasing { get; set; }
        public float renderScale { get; set; }
        public visibleObjectsElement visibleObject { get; set; }
        public viewRectElement viewRect { get; set; }

        public FPSLimiterElement fpsLimitter { get; set; }
        public SmoothfollowElement Smoothfollow { get; set; }
        public Follow360Element follow360 { get; set; }
        public ModmapExtensionsElement modmapExtensions { get; set; }

        public targetPosElement targetPos { get; set; }
        public targetRotElement targetRot { get; set; }
        public MovementScriptElement movementScript { get; set; }
    }
}