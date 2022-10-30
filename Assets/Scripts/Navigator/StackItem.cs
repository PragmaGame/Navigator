namespace Navigator
{
    public class StackItem
    {
        public readonly string id;
        public readonly BaseScreen screen;

        public StackItem(string id, BaseScreen screen)
        {
            this.id = id;
            this.screen = screen;
        }
    }
}