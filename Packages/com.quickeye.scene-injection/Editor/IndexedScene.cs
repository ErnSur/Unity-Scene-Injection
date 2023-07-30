namespace QuickEye.SceneInjection
{
    public readonly struct IndexedScene
    {
        public readonly string ScenePath;
        public readonly string OrderKey;
        public readonly bool Enabled;
        public decimal FractionalIndex => decimal.Parse($"0.{OrderKey}");

        public IndexedScene(string orderKey, string scenePath, bool enabled = true)
        {
            ScenePath = scenePath;
            OrderKey = orderKey;
            Enabled = enabled;
        }
        // public IndexedScene(decimal fractionalIndex, string scenePath)
        // {
        //     ScenePath = scenePath;
        //     FractionalIndex = fractionalIndex;
        // }
    }
}