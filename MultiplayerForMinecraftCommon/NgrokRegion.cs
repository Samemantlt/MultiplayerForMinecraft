namespace MultiplayerForMinecraftCommon
{
    public class NgrokRegion
    {
        public static readonly NgrokRegion UnitedStates = new NgrokRegion("США", "us");
        public static readonly NgrokRegion Europe = new NgrokRegion("Европа", "eu");


        public string FriendlyName { get; set; }
        public string TwoCharName { get; set; }


        private NgrokRegion(string friendlyName, string twoCharName)
        {
            FriendlyName = friendlyName;
            TwoCharName = twoCharName;
        }
    }
}
