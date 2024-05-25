namespace Shuile.Gameplay
{
    public static class PlayerCommands
    {
        public static void Move(float xInput, IMoveController moveController)
        {
            moveController.XMove(xInput);
        }
    }
}
