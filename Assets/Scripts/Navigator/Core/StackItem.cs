namespace Navigator.Core
{
    public class StackItem
    {
        public readonly string id;
        public readonly Screen screen;

        public StackItem(string id, Screen screen)
        {
            this.id = id;
            this.screen = screen;
        }
    }
}