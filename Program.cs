// Here we simply create the new game
Game game = new Game();
// and while it is running perform the required actions
while(game.Running)
{
    game.Update();
    game.Render();
}
