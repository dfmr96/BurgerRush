namespace Save
{
    [System.Serializable]
    public class PlayerStatsSaveWrapper
    {
        public PlayerStatsSaveData data;
        public string checksum;
        public long lastSavedAt;
    }
}